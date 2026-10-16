// 가격·로스터 항목을 유닛에 잇고, 그 값을 고쳐 빌드한다
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class RosterTests
{
    const string Tank = "entity/-vehicle/x/t/tiger.def", Breed = "set/breed/mp/ger/mid/nco.set";
    const string Units = "set/multiplayer/units/conquest/units_ger.set", Inf = "set/multiplayer/units/conquest/inf_ger.set",
        Doctrine = "set/multiplayer/units/mid/units_ger.set", Research = "set/dynamic_campaign/unit_research_ger.set",
        LateUnits = "set/multiplayer/units/late/units_ger.set";

    const string UnitsText = "(define \"vehicle2\" {cost %cost} {cp %cp})\n(define \"tmpl\" {\"tiger\" {cost 1}})\n{\"tiger\"\n\t(\"vehicle2\" side(ger) cp(40) crew1(tankman:4))\n\t{cost 1220}\n}\n{\"panther\" (\"vehicle2\" side(ger) cp(30)) {cost 900}}\n";
    const string InfText = "{\"mp/ger/mid/nco\" (\"inf_tier2\" side(ger) lvl(lvl1 lvl2) r(1) rm(8) cost(10.0))}\n";
    const string DoctrineText = "{\"doctrine_tiger\" (\"squad_vehicle2_doctrine\" side(ger) vehicle(tiger) crew1(tankman:3))}\n";
    const string ResearchText = "{techs\n\t{\"tiger\" requires \"panther\" costs 3 position 7 62}\n}\n";
    const string SquadDefine = "(define \"squad_with3types_doc\"\n\t{\"%name(%side)\" {content \"mp/%side/%period/%c1 mp/%side/%period/%c2\"} (\"x\" c1(nco:1) side(ger) period(mid))}\n)\n";

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
        T.True(!links.EntriesOf("set/breed/mp/ger/mid/tankman.set").Any(), "crew1 without period() names no breed");

        // 분대 매크로는 병종을 content "mp/%side/%period/%c1"로 만든다(E3): cN·crew·crewN은 병종, vN은 차량
        var squads = new Dictionary<string, string>
        {
            [Doctrine] = SquadDefine + "{\"doctrine_squad_ger_signaller_mid(ger)\"\n\t(\"squad_with1types_doctrine\" side(ger) period(mid) c1(nco:1))\n\t(\"doctrine_t2\" cost(60))\n}\n" +
                "(\"squad_with3types_doc\" side(ger) name(squad_x_mid_all) d(mid_ger_allround) period(mid) c1(nco:1) c2(rifle:8))\n" +
                "(\"squad_with3types_doc\" side(ger) name(squad_y_mid_all) period(mid) c1(rifle:2))\n" +
                "(\"squad_3vehicles\" side(ger) period(mid) v1(tiger) crew(tankman:2))\n(\"doctrine_t2\" cost(1))\n",
        };
        var sl = RosterLinks.Build(squads.Keys, p => squads[p]);
        var nco = sl.EntriesOf(Breed);
        T.Eq(new[] { "doctrine_squad_ger_signaller_mid(ger)", "squad_x_mid_all(ger)" }, nco.Select(e => e.Name).ToArray(), "a breed named by c1() in a quoted entry and in a top-level squad call");
        var call = nco[1];
        T.Eq(1, call.Rows.Count, "a top-level call is one entry of its own rows");
        T.True(call.IsCall && !nco[0].IsCall, "the call entry is marked");
        T.Eq(new[] { "squad_x_mid_all(ger)", "squad_y_mid_all(ger)" }, sl.EntriesOf("set/breed/mp/ger/mid/rifle.set").Select(e => e.Name).ToArray(), "each call is its own entry");
        T.Eq(new[] { "squad_3vehicles" }, sl.EntriesOf(Tank).Select(e => e.Name).ToArray(), "v1() names a vehicle; a call without name() is named by its macro");
        T.Eq(1, sl.EntriesOf("set/breed/mp/ger/mid/tankman.set").Count, "crew() names a breed");
        T.True(!sl.EntriesOf(Breed).Any(e => e.Rows[0].Path.Contains("(define ")), "the squad template is not an entry");
        T.Eq(4, sl.EntryCount, "calls that name no unit are not entries");

        // 세션: 탭의 줄과 고치기, 빌드
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), repo = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), (Units, UnitsText), (Inf, InfText), (Doctrine, DoctrineText), (Research, ResearchText), (Breed, "{breed {inventory {item \"luger\" 1}}}"),
            (LateUnits, "(\"squad_with3types_doc\" side(ger) name(squad_x_mid_all) period(mid) c1(nco:1) c2(rifle:8))\n"));
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

        // 분대 호출 항목(E3): 병종 화면에 보이고, name()은 항목 이름이라 고치지 않는다
        var ncoView = session.OpenUnit(Breed);
        var squadRow = ncoView.Roster.Single(r => r.Values[0].Trace.File == LateUnits);
        T.Eq(LateUnits + " · squad_x_mid_all(ger)", squadRow.RosterEntry, "the squad call is grouped under its file and name");
        T.Eq(new[] { true, false, true, true, true }, squadRow.Values.Select(v => v.Editable).ToArray(), "name() is the entry name and not editable; the other values are");

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
