using System.Collections.Generic;
using System.Linq;
using System.Text;
using CtaToybox.Core;
using CtaToybox.Core.Spawn;

// 스폰 모드: 스커미시 로스터(Task 3)
static class SpawnRosterTests
{
    static string Text(List<SpawnFile> files, string path)
    {
        var f = files.SingleOrDefault(x => x.Path == path);
        return f == null ? null : Encoding.UTF8.GetString(f.Bytes);
    }

    public static void Run()
    {
        string repo, stat, spawn;
        string settings = SpawnFixture.Make(out repo, out stat, out spawn,
            ("set/multiplayer/units/settings_inf.set", "(define \"inf_tankman\"\n {cp 3}\n {cost %cost}\n)\n"),
            ("set/multiplayer/games/presets/global_parameters/budget.inc", "{budgets\n\t{vehicle\n\t\t{buttons \"inf1 inf2 empl vehicles tanks\"}\n\t}\n\t{hero {buttons \"doctrine\"}}\n}\n"),
            ("set/multiplayer/units/conquest/units_ger.set", "{\"lt\" (\"vehicle_conquest\" cost(500))}\n"),
            ("interface/scene/unit_icon/button_5_ger_00.tga", "TGA5"));
        using var o = SpawnFixture.Open(settings);
        var skipped = new List<string>(); var notes = new List<string>();
        var files = RosterRewrite.Build(o.Stack, o.Catalog, new RosterOptions(), skipped, notes);
        string units = Text(files, "resource/" + SpawnFixture.UnitsGer);
        T.True(units.Contains("cd(0) ci(0)") && units.Contains("cost(0)") && units.Contains("cp(0)") && !units.Contains("cost(105)"), "values zero: " + units);
        T.True(units.Contains("b(light)") && units.Contains("b(empl)"), "light tank button, gun button kept");
        T.True(units.Contains("{\"ht\" (\"vehicle2\" period(mid) cd(0) ci(0) t() side(ger) crew1(tankman:1) crew2(tank_commander:1) g(ctb_spawn) b(heavy) cw(0) cp(0) cost(0))}"), "missing vehicle added with default crew: " + units);
        T.True(units.Contains("{\"stug3g\" (") && units.Contains("b(td)"), "a missing td gets the td button");
        T.True(!units.Contains("{\"ju87\""), "an aircraft is not added to the buy roster");
        T.Eq(0, SdlCore.Parse(units).Errors.Count, "the roster parses");
        T.True(Text(files, "resource/" + SpawnFixture.InfGer).Contains("cost(0)"), "crew unit cost zero (vehicle cost adds crew)");
        T.True(Text(files, "resource/set/multiplayer/units/settings_inf.set").Contains("{cp 0}"), "infantry cp in the breed template is zero");
        T.Eq(null, Text(files, "resource/set/multiplayer/units/conquest/units_ger.set"), "conquest roster untouched");
        T.Eq("{unitLimits \"officer=999;limit4=999\"}", Text(files, "resource/set/multiplayer/games/presets/global_parameters/unitlimits.inc").Trim(), "limits lifted");
        string budget = Text(files, "resource/set/multiplayer/games/presets/global_parameters/budget.inc");
        T.True(budget.Contains("{buttons \"inf1 inf2 empl vehicles light medium heavy td air\"}") && budget.Contains("{buttons \"doctrine\"}"), "vehicle buttons list replaced, hero list kept");
        T.Eq(new[] { "resource/interface/scene/unit_icon/button_6_ger_00.tga", "resource/interface/scene/unit_icon/button_7_ger_00.tga", "resource/interface/scene/unit_icon/button_8_ger_00.tga", "resource/interface/scene/unit_icon/button_9_ger_00.tga" },
            files.Where(f => f.Path.Contains("button_")).Select(f => f.Path).OrderBy(p => p).ToArray(), "tank icon copied to the new button numbers");
        T.True(notes.Any(n => n.Contains("미확인")), "new buttons noted as unverified");

        // 기본 단추·원래 비용
        files = RosterRewrite.Build(o.Stack, o.Catalog, new RosterOptions { BasicButtons = true, KeepCost = true }, new List<string>(), new List<string>());
        units = Text(files, "resource/" + SpawnFixture.UnitsGer);
        T.True(units.Contains("cost(105)") && units.Contains("b(tanks) cw(4)") && units.Contains("{\"ht\" (") && units.Contains("b(tanks) cw(0) cp(0) cost(100)"), "basic buttons and kept cost: " + units);
        T.True(!files.Any(f => f.Path.Contains("button_") || f.Path.Contains("budget")), "no button files with basic buttons");

        // roster_crew_missing_skipped: 전차장 병종이 없으면 새 항목을 넣지 않는다
        settings = SpawnFixture.Make(out repo, out stat, out spawn, (SpawnFixture.InfGer, "{\"mp/ger/mid/tankman\" (\"inf_tankman\" side(ger) cost(5.0))}\n"));
        using var o2 = SpawnFixture.Open(settings);
        skipped = new List<string>();
        units = Text(RosterRewrite.Build(o2.Stack, o2.Catalog, new RosterOptions(), skipped, new List<string>()), "resource/" + SpawnFixture.UnitsGer);
        T.True(!units.Contains("{\"ht\"") && skipped.Any(s => s.Contains("ht.def") && s.Contains("tank_commander")), "roster_crew_missing_skipped");
    }
}
