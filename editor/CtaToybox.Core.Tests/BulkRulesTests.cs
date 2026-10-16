using System.IO;
using System.Linq;
using CtaToybox.Core;

// F1: 일괄 규칙의 계산과 미리 보기
static class BulkRulesTests
{
    public const string L1 = "entity/-vehicle/x/tank_light/l1/l1.def", L2 = "entity/-vehicle/x/tank_light/l2/l2.def", H1 = "entity/-vehicle/x/tank_heavy/h1/h1.def", TankExt = "properties/tank.ext";

    public static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        string unit = "{game_entity\n (include \"/properties/tank.ext\")\n {chassis {locomotion {maxspeed 45}}}\n {name \"t\"}\n (\"armor_m\")\n}";
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/tank_light/l1/l1.def", unit), ("-vehicle/x/tank_light/l2/l2.def", unit), ("-vehicle/x/tank_heavy/h1/h1.def", unit));
        T.Zip(Path.Combine(game, "resource", "properties.pak"),
            (TankExt, "{mass 10}\n{ratio 0.5}\n(define \"armor_m\" {armor 5})"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static BulkRule Rule(string key, string op, string value, string scope = "auto")
    {
        return new BulkRule { Id = "r1", Name = "t", Kind = "tank", Paths = new System.Collections.Generic.List<string> { "/tank_light/" }, Key = key, Op = op, Value = value, Scope = scope };
    }

    public static RulePreview Preview(EditorSession s, BulkRule rule, EditProject project = null)
    {
        return BulkRules.Preview(rule, project ?? new EditProject(), s.Index.Units, new UnitTracer(s.Stack), f => s.Index.UsersOf(f), u => s.Index.HeirsOf(u).Count);
    }

    public static void Run()
    {
        string error;
        T.Eq("50", BulkRules.Compute("mul", "45", "1.1", out error), "an integer stays an integer");
        T.Eq("0.55", BulkRules.Compute("mul", "0.5", "1.1", out error), "a decimal keeps three places");
        T.Eq("40", BulkRules.Compute("add", "45", "-5", out error), "add");
        T.Eq("60", BulkRules.Compute("set", "45", "60", out error), "set");
        T.True(BulkRules.Compute("mul", "\"x\"", "2", out error) == null && error.Contains("수가 아니다"), "mul on a word is refused");
        T.True(BulkRules.Compute("mul", "45", "x", out error) == null && error != null, "a bad factor is refused");
        T.True(BulkRules.Compute("pow", "45", "2", out error) == null && error != null, "an unknown operation is refused");

        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using var s = EditorSession.Open(settings, repo, null);
        T.Eq("tank", s.Index.Units.Single(u => u.Path == L1).EditorKind, "the fixture units are tanks");

        var speed = Preview(s, Rule("**/locomotion/maxspeed", "mul", "1.1"));
        T.Eq(2, speed.Edits.Count, "one edit per light tank (own file)");
        T.True(speed.Edits.All(e => e.Scope == "shared" && e.New == "50" && e.Rule == "r1"), "shared, the new value, tagged with the rule");
        T.Eq(2, speed.Units, "two units change");

        var mass = Preview(s, Rule("**/mass", "mul", "2"));
        T.Eq(new[] { "unit", "unit" }, mass.Edits.Select(e => e.Scope).ToArray(), "auto makes unit-scoped edits when a non-target uses the file");
        T.Eq(0, mass.OutsideUnits, "and changes no unit outside the targets");
        var massShared = Preview(s, Rule("**/mass", "mul", "2", "shared"));
        T.Eq(1, massShared.Edits.Count, "shared makes one edit");
        T.Eq(1, massShared.OutsideUnits, "and reports the heavy tank outside the targets");

        var armor = Preview(s, Rule("**/armor", "add", "1"));
        T.Eq(0, armor.Edits.Count, "a macro body value cannot be unit-scoped");
        T.True(armor.Skipped.Count == 2 && armor.Skipped.All(k => k.Reason.Contains("매크로")), "both units are skipped with the reason");

        var human = new EditProject();
        human.Edits.Add(new Edit { Unit = L1, File = L1, Path = speed.Edits.First(e => e.File == L1).Path, Occurrence = 0, Arg = 0, Old = "45", New = "70", Scope = "shared" });
        var withHuman = Preview(s, Rule("**/locomotion/maxspeed", "mul", "1.1"), human);
        T.Eq(new[] { L2 }, withHuman.Edits.Select(e => e.Unit).ToArray(), "a hand-made shared edit keeps its spot");
        T.True(withHuman.Skipped.Single().Unit == L1 && withHuman.Skipped.Single().Reason.Contains("직접 고친"), "the skip names it");

        T.Eq(0, Preview(s, Rule("**/locomotion/maxspeed", "mul", "1")).Edits.Count, "no edit when the value does not change");
        var name = Preview(s, Rule("**/name", "mul", "2"));
        T.True(name.Edits.Count == 0 && name.Skipped.All(k => k.Reason.Contains("수가 아니다")), "a word value is skipped");
        var all = Rule("**/locomotion/maxspeed", "mul", "1.1");
        all.Paths = null;
        T.Eq(3, Preview(s, all).Edits.Count, "without a path filter every tank is a target");
    }
}
