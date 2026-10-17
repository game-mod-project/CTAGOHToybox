using System.Collections.Generic;
using System.IO;
using CtaToybox.Core;
using CtaToybox.Core.Spawn;

// 스폰 모드 테스트가 함께 쓰는 작은 게임 폴더(바닐라 + Spawn Menu of MACE 사본 + 스탯 출력 모드)
static class SpawnFixture
{
    public const string Lt = "entity/-vehicle/germany/tank_light/lt/lt.def", Ht = "entity/-vehicle/germany/tank_heavy/ht/ht.def",
        Stug = "entity/-vehicle/germany/tank_medium/stug3g/stug3g.def", Rt = "entity/-vehicle/ussr/tank_medium/rt/rt.def",
        Pak = "entity/-vehicle/germany/cannon/pak40/pak40.def", Bf = "entity/-vehicle/germany/airborne/bf109/bf109.def",
        Wreck = "entity/-vehicle/germany/x/wreck/wreck.def", Ju = "entity/-vehicle/germany/airborne/ju87/ju87.def";
    public const string UnitsGer = "set/multiplayer/units/mid/units_ger.set", InfGer = "set/multiplayer/units/mid/inf_ger.set";
    public const string Campaign = "map/single/02-ger/m1/0.mi", Conquest = "map/multi/dcg_x/campaign_capture_the_flag.mi";
    public const string MissionText = "{mission\r\n\t{a 1}\r\n}\r\n";

    public const string UnitsGerText =
        "; ger mid\n" +
        "{\"lt\"\n\t(\"vehicle2\" period(mid) cd(150) ci(60) t(light) side(ger) crew1(tankman:1) crew2(tank_commander:1) g(light1) b(tanks) cw(4) cp(8)\n\tcost(105))\n}\n" +
        "{\"bf109\" (\"vehicle2\" period(mid) cd(60) ci(60) t() side(ger) crew1(tankman:1) g(air) b(vehicles) cw(0) cp(20) cost(300))}\n" +
        "(\"squad_with2types_doc\" side(ger) name(sq_a) period(mid) scf(1) cd(0) ci(60) g(squad_1) c1(nco:1) c2(rifle:3) b(inf1) t())\n" +
        "(\"squad_with1types_doc\" side(ger) name(sq_b) period(mid) scf(1) cd(0) ci(60) g(squad_1) c1(ghost:2) b(inf1) t())\n" +
        "{\"pak40\" (\"vehicle2\" period(mid) cd(60) ci(180) t(cannon) side(ger) crew1(artilleryman:2) g(empl1) b(empl) cw(2) cp(4) cost(110))}\n";

    public static string Make(out string repo, out string statDir, out string spawnDir, params (string Name, string Text)[] extraSet)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); statDir = T.TempDir();
        spawnDir = Path.Combine(T.TempDir(), "CTAToyboxSpawn");
        T.Zip(Path.Combine(game, "resource", "properties.pak"), ("properties/tank.ext", "{x 1}\n"), ("properties/cannon.ext", "{x 1}\n"));
        string tank = "{game_entity\n (include \"/properties/tank.ext\")\n {x 1}\n}\n";
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            (Lt.Substring(7), tank), (Ht.Substring(7), tank), (Stug.Substring(7), tank), (Rt.Substring(7), tank),
            (Pak.Substring(7), "{game_entity\n (include \"/properties/cannon.ext\")\n {x 1}\n}\n"),
            (Bf.Substring(7), "{game_entity\n {x 1}\n}\n"), (Ju.Substring(7), "{game_entity\n (include \"/properties/airborne.ext\")\n {x 1}\n}\n"), (Wreck.Substring(7), "{game_entity\n {x 1}\n}\n"));
        var set = new List<(string, string)>
        {
            (UnitsGer, UnitsGerText),
            ("set/multiplayer/units/mid/units_rus.set", "{\"rt\" (\"vehicle2\" period(mid) cd(1) ci(1) t() side(rus) crew1(tankman:1) g(m) b(tanks) cw(1) cp(1) cost(100))}\n"),
            (InfGer, "{\"mp/ger/mid/tankman\" (\"inf_tankman\" side(ger) cost(5.0))}\n{\"mp/ger/mid/tank_commander\" (\"inf_tankman\" side(ger) cost(6.0))}\n"),
            ("set/breed/mp/ger/mid/nco.set", "{breed {x 1}}\n"), ("set/breed/mp/ger/mid/rifle.set", "{breed {x 1}}\n"),
            ("set/breed/mp/ger/mid/tankman.set", "{breed {x 1}}\n"), ("set/breed/mp/ger/mid/tank_commander.set", "{breed {x 1}}\n"),
            ("properties/airborne.ext", "{x 1}\n"), ("set/breed/mp/ger/mid/artilleryman.set", "{breed {x 1}}\n"),
            ("set/multiplayer/games/presets/global_parameters/unitlimits.inc", "{unitLimits \"officer=2;limit4=4\"}\n"),
        };
        set.AddRange(extraSet);
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), set.ToArray());
        T.Zip(Path.Combine(game, "resource", "map", "single.pak"), ("single/02-ger/m1/0.mi", MissionText));
        T.Zip(Path.Combine(game, "resource", "map", "multi.pak"), ("multi/dcg_x/campaign_capture_the_flag.mi", "{mission\n\t{b 2}\n}\n"), ("multi/dcg_x/dcg.inc", "{x}\n"));
        // Spawn Menu of MACE의 맵 사본(이기면 안 된다)
        T.Put(Path.Combine(workshop, "3617250510"), "resource/map/single/02-ger/m1/0.mi", "{mission\r\n\t{smm 1}\r\n\t(include \"/map/cheat/cheat.mi\")\r\n}\r\n");
        T.Put(statDir, "resource/set/z.set", "{z}\n");
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_3617250510:0\" \"" + Path.GetFileName(statDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = statDir, OutputModName = Path.GetFileName(statDir), SpawnModDir = spawnDir }.Save(settings);
        return settings;
    }

    public sealed class Opened : System.IDisposable
    {
        public SourceStack Stack; public UnitIndex Index; public ArmyIndex Armies; public SpawnCatalog Catalog; public List<string> Skipped = new List<string>(); public EditorSettings Settings;
        public void Dispose() { Stack.Dispose(); }
    }

    public static readonly string Categories = File.ReadAllText(Path.Combine(T.RepoRoot(), "editor", "spawn", "categories.psv"));

    public static Opened Open(string settingsPath)
    {
        var s = EditorSettings.Load(settingsPath);
        var plan = GameProfile.PlanLayers(s, GameProfile.ReadModEntries(File.ReadAllText(s.OptionsSetPath)));
        var o = new Opened { Settings = s, Stack = SpawnSources.Open(s, plan, new List<string>()) };
        o.Index = UnitIndex.Build(o.Stack, null);
        o.Armies = ArmyIndex.Build(o.Stack, o.Index.Units);
        o.Catalog = SpawnCatalog.Build(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(Categories, null), o.Skipped);
        return o;
    }
}
