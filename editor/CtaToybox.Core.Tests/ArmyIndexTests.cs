using System.IO;
using System.Linq;
using CtaToybox.Core;

// 팩션 강화: 유닛의 군대(로스터 side와 폴더)
static class ArmyIndexTests
{
    public const string GerInf = "set/breed/mp/ger/mid/a.set", RusInf = "set/breed/mp/rus/mid/b.set", CanInf = "set/breed/mp/can/mid/c.set",
        SharedCar = "entity/-vehicle/germany/car/shared_car/shared_car.def", Solo = "entity/-vehicle/+germany/car/solo/solo.def", RusTank = "entity/-vehicle/ussr/tank_light/t1/t1.def";

    public static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        string breed = "{inventory {item \"k98\" filled}}\n{health 1}";
        string car = "{game_entity {chassis {locomotion {maxspeed 45}}}}";
        T.Zip(Path.Combine(game, "resource", "set.pak"),
            (GerInf, breed), (RusInf, breed), (CanInf, breed),
            ("set/multiplayer/units/conquest/inf_ger.set", "{\"mp/ger/mid/a\" (\"inf\" side(ger) cost(8))}"),
            ("set/multiplayer/units/conquest/inf_rus.set", "{\"mp/rus/mid/b\" (\"inf\" side(rus) cost(8))}"),
            ("set/multiplayer/units/conquest/inf_eng.set", "{\"x1\" (\"inf\" side(eng))}\n{\"mp/can/mid/c\" (\"inf\" side(can))}\n{\"x2\" (\"inf\" side(eng))}"),
            ("set/multiplayer/units/conquest/units_ger.set", "{\"shared_car\" (\"vehicle2\" side(ger))}"),
            ("set/multiplayer/units/conquest/units_fin.set", "{\"shared_car\" (\"vehicle2\" side(fin))}"));
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/germany/car/shared_car/shared_car.def", car), ("-vehicle/+germany/car/solo/solo.def", car), ("-vehicle/ussr/tank_light/t1/t1.def", car));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using var s = EditorSession.Open(settings, repo, null);
        var a = s.Armies;
        T.Eq("ger", a.ArmyOf(GerInf), "infantry by its side folder");
        T.Eq("rus", a.ArmyOf(RusInf), "another army");
        T.Eq("eng", a.ArmyOf(CanInf), "can belongs to the army whose roster files name it most");
        T.True(a.SidesOf("eng").Contains("can") && a.SidesOf("eng").Contains("eng"), "eng sides include can");
        T.Eq("ger", a.ArmyOf(Solo), "a vehicle missing from rosters goes by its +germany folder");
        T.Eq("rus", a.ArmyOf(RusTank), "ussr folder is rus");
        T.Eq(null, a.ArmyOf(SharedCar), "a vehicle in two armies' rosters belongs to neither");
        T.True(a.Shared.Any(x => x.StartsWith(SharedCar) && x.Contains("ger") && x.Contains("fin")), "and is listed as shared: " + string.Join("; ", a.Shared));
        T.True(a.Members("ger").Contains(GerInf) && !a.Members("ger").Contains(RusInf), "members");

        var rule = new BulkRule { Id = "r1", Kind = "vehicle", Army = "ger", Key = "**/locomotion/maxspeed", Op = "mul", Value = "2" };
        var preview = BulkRules.Preview(rule, new EditProject(), s.Index.Units, new UnitTracer(s.Stack), f => s.Index.UsersOf(f), u => s.Index.HeirsOf(u).Count, p => a.ArmyOf(p));
        T.Eq(new[] { Solo }, preview.Changes.Select(c => c.Unit).ToArray(), "an army rule targets only that army's vehicles");
        T.Throws<System.ArgumentException>(() => BulkRules.Preview(rule, new EditProject(), s.Index.Units, new UnitTracer(s.Stack), f => s.Index.UsersOf(f), u => 0), "an army rule needs the army index");
        T.True(BulkRules.Check(new BulkRule { Kind = "tank", Key = "**/x", Op = "mul", Value = "2", Army = "xyz" }) != null, "an unknown army is refused");
        var ex = rule.Copy(); ex.Army = null; ex.Exclude = new System.Collections.Generic.List<string> { "/solo/" };
        var excluded = BulkRules.Preview(ex, new EditProject(), s.Index.Units, new UnitTracer(s.Stack), f => s.Index.UsersOf(f), u => 0);
        T.True(!excluded.Changes.Any(c => c.Unit == Solo) && excluded.Changes.Any(c => c.Unit == RusTank), "exclude drops units whose path has the text");
        s.OpenProject(Path.Combine(repo, "editor", "projects", "army.json"));
        var saved = s.AddRule(rule);
        T.Eq("ger", s.RuleOf(saved.Id).Army, "the session keeps the army on the rule");
    }
}
