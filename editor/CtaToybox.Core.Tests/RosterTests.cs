// 가격·로스터 항목을 유닛에 잇고, 그 값을 고쳐 빌드한다
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class RosterTests
{
    const string Tank = "entity/-vehicle/x/t/tiger.def", Breed = "set/breed/mp/ger/mid/nco.set";
    const string Units = "set/multiplayer/units/conquest/units_ger.set", Inf = "set/multiplayer/units/conquest/inf_ger.set",
        Doctrine = "set/multiplayer/units/mid/units_ger.set", Research = "set/dynamic_campaign/unit_research_ger.set";

    const string UnitsText = "(define \"vehicle2\" {cost %cost} {cp %cp})\n(define \"tmpl\" {\"tiger\" {cost 1}})\n{\"tiger\"\n\t(\"vehicle2\" side(ger) cp(40) crew1(tankman:4))\n\t{cost 1220}\n}\n{\"panther\" (\"vehicle2\" side(ger) cp(30)) {cost 900}}\n";
    const string InfText = "{\"mp/ger/mid/nco\" (\"inf_tier2\" side(ger) lvl(lvl1 lvl2) r(1) rm(8) cost(10.0))}\n";
    const string DoctrineText = "{\"doctrine_tiger\" (\"squad_vehicle2_doctrine\" side(ger) vehicle(tiger) crew1(tankman:3))}\n";
    const string ResearchText = "{techs\n\t{\"tiger\" requires \"panther\" costs 3 position 7 62}\n}\n";

    public static void Run()
    {
        T.Eq("tiger", RosterLinks.KeyOf(Tank), "a vehicle is named by its file");
        T.Eq("mp/ger/mid/nco", RosterLinks.KeyOf(Breed), "a breed is named by its path under set/breed");
        T.True(RosterLinks.KeyOf("set/stuff/gun/g1") == null, "items have no roster entries");
        T.True(RosterLinks.IsRosterFile(Units) && RosterLinks.IsRosterFile(Research) && !RosterLinks.IsRosterFile("set/dynamic_campaign/resources_low.set"), "roster files");
        T.Eq("mp/ger/mid/nco", KeyLabel.Readable("/\"mp/ger/mid/nco\""), "a quoted name keeps its slashes");
        T.Eq("inf_tier2 (매크로)", KeyLabel.Readable("/\"mp/ger/mid/nco\"/(\"inf_tier2\")"), "and the rest of the path is split");

        var texts = new Dictionary<string, string> { [Units] = UnitsText, [Inf] = InfText, [Doctrine] = DoctrineText, [Research] = ResearchText, ["set/stuff/gun/g1"] = "{\"tiger\" 1}" };
        var links = RosterLinks.Build(texts.Keys, p => texts[p]);
        var tiger = links.EntriesOf(Tank);
        T.Eq(new[] { Research, Units, Doctrine }, tiger.Select(e => e.File).ToArray(), "the tank's entries: research, conquest, and the squad that names it with vehicle()");
        T.Eq(3, tiger.Single(e => e.File == Units).Rows.Count, "an entry holds its own rows only");
        T.Eq(1, links.EntriesOf(Breed).Count, "the breed's entry");
        T.True(!tiger.Any(e => e.Rows[0].Path.Contains("(define ")), "a name block inside a macro definition is not an entry");
        var macro = tiger.Single(e => e.File == Units).Rows[1];
        T.Eq(new[] { "side", "cp", "crew1" }, macro.LeafNames.ToArray(), "values carry the name of the call around them");

        // 세션: 탭의 줄과 고치기, 빌드
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), repo = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), (Units, UnitsText), (Inf, InfText), (Doctrine, DoctrineText), (Research, ResearchText), (Breed, "{breed {inventory {item \"luger\" 1}}}"));
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"), ("-vehicle/x/t/tiger.def", "{game_entity {mass 57000}}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "common.psv"), "");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        using var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(Path.Combine(work, "p.json"));

        var unit = session.OpenUnit(Tank);
        T.Eq(new[] { Research, Units, Units, Units, Doctrine, Doctrine }, unit.Roster.Select(r => r.Values[0].Trace.File).ToArray(), "the unit view lists its roster rows");
        T.True(unit.Roster.All(r => r.RosterEntry != null) && unit.Rows.All(r => r.RosterEntry == null), "roster rows are kept apart from the unit's rows");
        var nameRow = unit.Roster[1];
        T.True(!nameRow.Values[0].Editable, "the entry name is not editable");
        var research = unit.Roster[0];
        T.Eq(new[] { false, false, true, false, true, false, true, true }, research.Values.Select(v => v.Editable).ToArray(), "research row: keywords are not editable, names and numbers are");
        var cost = unit.Roster.Single(r => r.Path == "/\"*\"/cost" && r.Values[0].Trace.File == Units);
        T.True(cost.Values[0].UnitScopeBlocker != null, "roster values are shared only");
        T.Eq("확인", cost.Note.Grade, "the cost row is explained");

        session.ApplyRow(unit, cost, new[] { "999" }, "shared");
        unit = session.OpenUnit(Tank);
        T.Eq("999", unit.Roster.Single(r => r.Path == "/\"*\"/cost" && r.Values[0].Trace.File == Units).Values[0].Edit.New, "the edit shows on the roster row");
        T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "the roster edit applies to the source");
        var report = session.Build();
        T.Eq(1, report.Count(EditStatus.Applied), "the build applies it");
        string written = File.ReadAllText(Path.Combine(outDir, "resource", "set", "multiplayer", "units", "conquest", "units_ger.set"));
        T.True(written.Contains("{cost 999}") && written.Contains("{cost 900}") && written.Contains("{\"tiger\" {cost 1}}"), "only the tiger's cost changes");
    }
}
