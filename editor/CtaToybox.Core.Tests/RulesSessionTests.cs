// W-H: 꺼진 블록(##), (mod ...) 조건 표시, 따옴표 이름 블록을 이름으로 다시 찾기(Q7)
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class RulesSessionTests
{
    const string Tank = "entity/-vehicle/x/t/tiger.def", Units = "set/multiplayer/units/conquest/units_ger.set";
    const string TankText = "{game_entity {mass 57000} (mod \"mp\" {mass 50000}) (mod not \"mp\" {limit 3}) {##damage 5}}";
    const string UnitsV1 = "{\"panther\" (\"vehicle2\" side(ger)) {cost 900}}\n{\"tiger\" (\"vehicle2\" side(ger)) {cost 1220}}\n";
    // 앞에 같은 값(1220)의 항목이 끼어든 판: "몇 번째"만 보면 leopard를 고치게 된다
    const string UnitsV2 = "{\"panther\" (\"vehicle2\" side(ger)) {cost 900}}\n{\"leopard\" (\"vehicle2\" side(ger)) {cost 1220}}\n{\"tiger\" (\"vehicle2\" side(ger)) {cost 1220}}\n";
    const string UnitsV3 = "{\"panther\" (\"vehicle2\" side(ger)) {cost 900}}\n{\"leopard\" (\"vehicle2\" side(ger)) {cost 1220}}\n";

    static EditorSession Open(string unitsText, string project, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), repo = T.TempDir();
        outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), (Units, unitsText));
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"), ("-vehicle/x/t/tiger.def", TankText));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "common.psv"), "");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(project);
        return session;
    }

    public static void Run()
    {
        string project = Path.Combine(T.TempDir(), "p.json"), outDir;

        using (var session = Open(UnitsV1, project, out outDir))
        {
            var unit = session.OpenUnit(Tank);
            // ## 블록: 게임이 쓰지 않는 꺼진 블록이라 고치지 않는다(개발사 문서)
            var off = unit.Rows.Single(r => r.Path.Contains("##damage"));
            T.True(!off.Values[0].Editable, "a value in a ## block is not editable");
            T.True(off.Values[0].Trace.Note != null && off.Values[0].Trace.Note.Contains("꺼진 블록"), "and says why");
            // (mod ...) 조건: 줄에 조건과 읽히는 이름을 싣는다
            var masses = unit.Rows.Where(r => r.Path == "/game_entity/mass").ToList();
            T.Eq(new string[] { null, "(mod \"mp\")" }, masses.Select(r => r.Cond).ToArray(), "rows carry their condition");
            T.Eq("멀티플레이에서만", masses[1].CondLabel, "mp reads as multiplayer only");
            T.Eq("멀티플레이가 아닐 때만", unit.Rows.Single(r => r.Path == "/game_entity/limit").CondLabel, "not mp reads as outside multiplayer");
            T.True(masses[0].CondLabel == null, "no condition, no label");
            T.Eq("(mod \"coop\")일 때만 (뜻 미확인)", RowView.LabelOf("(mod \"coop\")"), "other conditions show as written");
            T.Eq("멀티플레이에서만 · (mod not \"coop\")일 때만 (뜻 미확인)", RowView.LabelOf("(mod \"mp\") (mod not \"coop\")"), "nested conditions are joined");

            // 로스터 값을 고친다: 수정은 따옴표 이름이 든 경로도 갖는다
            var cost = unit.Roster.Single(r => r.Path == "/\"*\"/cost");
            session.ApplyRow(unit, cost, new[] { "999" }, "shared");
            var edit = session.Project.Edits.Single();
            T.Eq("/\"tiger\"/cost", edit.NamedPath, "the edit keeps the named path");
            T.Eq(0, edit.NamedOccurrence, "and which one of that named path");
            T.True(File.ReadAllText(project).Contains("namedPath"), "the project file stores it");
        }

        // 원본에 같은 값의 항목이 앞에 끼어들었다: 이름으로 tiger를 다시 찾는다
        using (var session = Open(UnitsV2, project, out outDir))
        {
            var edit = session.Project.Edits.Single();
            T.Eq(2, edit.Occurrence, "the edit is re-anchored to the tiger entry by name");
            T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "and applies");
            session.Build();
            string written = File.ReadAllText(Path.Combine(outDir, "resource", "set", "multiplayer", "units", "conquest", "units_ger.set"));
            T.True(written.Contains("{\"tiger\" (\"vehicle2\" side(ger)) {cost 999}}") && written.Contains("{\"leopard\" (\"vehicle2\" side(ger)) {cost 1220}}"), "only the tiger's cost changes");
        }

        // 그 이름이 원본에서 사라졌다: 같은 자리의 다른 항목을 고치지 않는다
        using (var session = Open(UnitsV3, project, out outDir))
        {
            var check = session.CheckEdits().Single();
            T.Eq(EditStatus.TargetMissing, check.Status, "a vanished name is a missing target");
            T.True(check.Message.Contains("tiger"), "the message names it: " + check.Message);
            var report = session.Build();
            T.Eq(0, report.Count(EditStatus.Applied), "the build does not touch the leopard");
        }

        // TokenPatcher도 이름이 다르면 쓰지 않는다(빌드의 마지막 지킴)
        var results = new System.Collections.Generic.List<EditResult>();
        string text = TokenPatcher.Apply(UnitsV3, new[] { new Edit { File = Units, Path = "/\"*\"/cost", Occurrence = 1, Arg = 0, Old = "1220", New = "999", NamedPath = "/\"tiger\"/cost", NamedOccurrence = 0 } }, results);
        T.Eq(EditStatus.TargetMissing, results.Single().Status, "the patcher checks the name at the slot");
        T.Eq(UnitsV3, text, "and leaves the text alone");
    }
}
