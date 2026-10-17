using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

// 팩션 강화 프리셋: 인벤토리·장갑·속도·연료(Task 4), 재장전·체력(Task 5), 게임 설정(Task 6)
static class FactionPresetTests
{
    public const string GerInf = "set/breed/mp/ger/mid/a.set", RusInf = "set/breed/mp/rus/mid/b.set";
    public static readonly string[] GerTanks = { "entity/-vehicle/germany/tank_light/lt/lt.def", "entity/-vehicle/germany/tank_medium/mt/mt.def", "entity/-vehicle/germany/tank_heavy/ht/ht.def", "entity/-vehicle/+germany/tank_heavy/maus/maus.def", "entity/-vehicle/germany/car/kw/kw.def" };
    public const string RusTank = "entity/-vehicle/ussr/tank_light/rt/rt.def";

    const string Breed = "{breed\n {inventory\n  {item \"k98\" filled}\n  {item \"rifle_ger clip ammo\" 60 5}\n  {item \"shovel_ger\"}\n  {item \"bandage_ger\" 22.5 2.5}\n }\n}\n";
    const string Tank = "{game_entity\n (include \"/properties/mobility.inc\")\n {chassis (\"mobility_tank\" speed(38) fuel(540) range(210))}\n {volume \"body\" {thickness 30 {front 40}}}\n {extender \"inventory\" {box {item \"bulletger_88_l56 he\" 46}}}\n}\n";
    const string Mobility = "(define \"mobility_tank\"\n {Locomotion {MaxSpeed %speed}}\n {FuelBag {volume %fuel} {rate (* 10 (/ %fuel %range))} {remain %fuel}}\n)\n";

    public static string Fixture(out string repo, out string outDir, params (string Name, string Text)[] extraSet)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        var set = new List<(string, string)>
        {
            (GerInf, Breed), (RusInf, Breed),
            ("set/multiplayer/units/conquest/inf_ger.set", "{\"mp/ger/mid/a\" (\"inf\" side(ger))}"),
            ("set/multiplayer/units/conquest/inf_rus.set", "{\"mp/rus/mid/b\" (\"inf\" side(rus))}"),
            ("set/stuff/rifle/rifle_ger.clip.ammo", "{x 1}\n"), ("set/stuff/shell/bulletger_88_l56.he.ammo", "{x 1}\n"),
            ("set/stuff/melee/shovel_ger", "{x 1}\n"), ("set/stuff/med/bandage_ger", "{x 1}\n"),
        };
        set.AddRange(extraSet);
        T.Zip(Path.Combine(game, "resource", "set.pak"), set.ToArray());
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"), GerTanks.Concat(new[] { RusTank }).Select(p => (p.Substring("entity/".Length), Tank)).ToArray());
        T.Zip(Path.Combine(game, "resource", "properties.pak"), ("properties/mobility.inc", Mobility));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static FactionContext Context(EditorSession s) { return new FactionContext { Stack = s.Stack, Index = s.Index, Tracer = new UnitTracer(s.Stack), Armies = s.Armies, Items = s.Items }; }

    public static string Out(string outDir, string virtualPath) { string p = Path.Combine(outDir, "resource", virtualPath.Replace('/', Path.DirectorySeparatorChar)); return File.Exists(p) ? File.ReadAllText(p) : null; }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using var s = EditorSession.Open(settings, repo, null);
        var c = Context(s);
        T.True(s.Items.IsAmmo("rifle_ger clip ammo") && s.Items.IsAmmo("bulletger_88_l56 he"), "ammo by file, also a shell without the word ammo");
        T.True(!s.Items.IsAmmo("shovel_ger") && !s.Items.IsAmmo("bandage_ger") && !s.Items.IsAmmo("nothing"), "tools and unknown names are not ammo");
        var supers = new List<string> { "entity/-vehicle/+germany/tank_heavy/maus/" };
        T.Eq("super", FactionPreset.ClassOf(GerTanks[3], supers), "super-heavy by the list");
        T.Eq("heavy", FactionPreset.ClassOf(GerTanks[2], supers), "heavy by folder");
        T.Eq("car", FactionPreset.ClassOf(GerTanks[4], supers), "car");
        T.Eq(null, FactionPreset.ClassOf(GerInf, supers), "infantry has no class");

        var project = new EditProject { Name = "ger" };
        var report = new FactionReport();
        FactionPreset.AddUnitStats(project, c, new FactionOptions { Army = "ger", SuperHeavy = supers }, report);
        T.True(project.Rules.Count == 11 && project.Rules.All(r => r.Army == "ger"), "eleven army rules: " + string.Join("; ", project.Rules.Select(r => r.Name)));
        var build = new ModBuilder(s.Stack, outDir).Build(project);
        T.Eq(0, build.Results.Count(r => r.Status != EditStatus.Applied), "everything applies: " + string.Join("; ", build.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Message)));
        string inf = Out(outDir, GerInf);
        T.True(inf.Contains("{item \"rifle_ger clip ammo\" 300 25}") && inf.Contains("{item \"bandage_ger\" 45 5}") && inf.Contains("{item \"shovel_ger\"}") && inf.Contains("{item \"k98\" filled}"), "infantry inventory: ammo ×5, other ×2, no count stays: " + inf);
        T.Eq(null, Out(outDir, RusInf), "another army's infantry is untouched");
        T.Eq(null, Out(outDir, RusTank), "another army's tank is untouched");
        string[] armor = { "60 {front 80}", "75 {front 100}", "90 {front 120}", "150 {front 200}", "60 {front 80}" };
        for (int i = 0; i < GerTanks.Length; i++)
        {
            string t = Out(outDir, GerTanks[i]);
            T.True(t != null && t.Contains("{thickness " + armor[i] + "}"), "armor of " + GerTanks[i] + ": " + t);
            T.True(t.Contains("speed(57)") && t.Contains("fuel(5400)") && t.Contains("range(2100)"), "speed ×1.5, fuel and range ×10: " + t);
            T.True(t.Contains("{item \"bulletger_88_l56 he\" 230}"), "shells ×5");
        }
        T.Eq(1, report.Edits.Count(kv => kv.Key.Contains("range") && kv.Value == 5), "range edits reported");
    }
}
