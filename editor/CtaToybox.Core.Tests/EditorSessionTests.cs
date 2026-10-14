using System.IO;
using System.Linq;
using CtaToybox.Core;

static class EditorSessionTests
{
    public const string A = "entity/-vehicle/x/a/a.def", B = "entity/-vehicle/x/a/b.def", S = "entity/-vehicle/x/a/s.inc";

    // 게임 폴더, 창작마당, options.set, 설정 파일, 저장소 루트(data/annotations, data/display-names.csv)를 가짜로 만든다
    public static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity\n (include \"s.inc\")\n {mass 10}\n {weapon \"gun_a\" filling \"gun_a_mag ammo\" 5}\n}"),
            ("-vehicle/x/a/b.def", "{game_entity (include \"s.inc\")}"),
            ("-vehicle/x/a/s.inc", "{box {item \"mine\" 24}}"));
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/stuff/smg/gun_a", "{filling \"ammo gun_a_mag\" 30}"),
            ("set/stuff/smg/gun_a_mag.ammo", "{bullet {mass 0.02}}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "common.psv"),
            "*/mass|block|확인|무게. 킬로그램|C\n**/box/item|block|추정|적재품 한 칸|S\n");
        File.WriteAllText(Path.Combine(repo, "data", "display-names.csv"), "\"Context\",\"Text\",\"File\"\n\"desc/entity/a\",\"Tank A\",\"x.pot\"\n");
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

        T.Throws<FileNotFoundException>(() => EditorSession.Open(Path.Combine(repo, "none.json"), repo, null), "a missing settings file");

        var messages = new System.Collections.Generic.List<string>();
        using var session = EditorSession.Open(settings, repo, messages.Add);
        T.True(messages.Count > 0, "progress messages are reported");
        T.True(session.Plan.OutputModActive, "the output mod is active in the fixture");
        T.Eq(new[] { A, B }, session.Search(null, "vehicle", null).Select(u => u.Path).ToArray(), "vehicles");
        T.Eq(new[] { A }, session.Search("tank a", null, null).Select(u => u.Path).ToArray(), "search by display name");
        T.True(session.OpenUnit("entity/-vehicle/x/none.def") == null, "a missing unit is null");

        var a = session.OpenUnit(A);
        T.Eq("Tank A", a.DisplayName, "display name");
        T.Eq("vehicle", a.Kind, "kind");
        T.Eq("vanilla", a.Layer, "layer");

        var mass = a.Rows.Single(r => r.Path == "/game_entity/mass");
        T.Eq("block", mass.Form, "row form");
        T.Eq("확인", mass.Note.Grade, "annotation of the row");
        T.Eq("무게", mass.SummaryLabel, "a 확인 key is in the summary");
        T.Eq(1, mass.Values[0].SharedBy, "a value in the unit's own file is used by one unit");
        T.True(mass.Values[0].UnitScopeBlocker == null, "and can be unit-scoped");

        var item = a.Rows.Single(r => r.Path == "/game_entity/box/item");
        T.Eq("추정", item.Note.Grade, "a tail annotation");
        T.True(item.SummaryLabel == null, "a 추정 key is not in the summary");
        var mine = item.Values[1];
        T.Eq("24", mine.Trace.Text, "the value");
        T.Eq(S, mine.Trace.File, "comes from the include");
        T.Eq(2, mine.SharedBy, "shared by both units");
        T.True(mine.UnitScopeBlocker == null, "unit scope is possible through the include");
        T.True(mine.Edit == null, "no edit yet");

        T.Eq(new[] { "mine", "gun_a", "gun_a_mag ammo" }, a.Items.Select(i => i.Name).ToArray(), "referenced item names, in expansion order (the include comes first)");
        T.Eq(ItemLinks.RuleNone, a.Items[0].Rule, "mine has no file in the fixture");
        T.Eq(ItemLinks.RuleName, a.Items[1].Rule, "gun_a links by name");
        T.Eq("set/stuff/smg/gun_a", a.Items[1].Files.Single(), "to the stuff file");
        T.Eq(ItemLinks.RuleWords, a.Items[2].Rule, "the ammo links by words");

        var gun = session.OpenUnit("set/stuff/smg/gun_a");
        T.Eq("stuff", gun.Kind, "a linked stuff file opens as a unit");

        string projectPath = Path.Combine(repo, "editor", "projects", "p.json");
        session.OpenProject(projectPath);
        T.Eq("p", session.Project.Name, "a missing project starts empty, named after the file");
        T.True(!File.Exists(projectPath), "and is not written until something changes");
        File.WriteAllText(Path.Combine(repo, "broken.json"), "{ not json");
        T.Throws<InvalidDataException>(() => session.OpenProject(Path.Combine(repo, "broken.json")), "a broken project is reported, not thrown as JsonException");
        T.Eq(projectPath, session.ProjectPath, "a failed open keeps the previous project");
    }
}
