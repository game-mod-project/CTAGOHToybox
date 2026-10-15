// 편집기 견고성(docs/10-backlog.md W-B): 깨진 입력, 키 경로를 바꾸는 값, 수정 점검, 깨진 현지화, CLI trace·remove
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

static class RobustnessTests
{
    const string Def = "entity/-vehicle/x/a/a.def", Table = "set/t.set", Bz = "set/multiplayer/games/battle_zones.set";

    static void Utf8(string root, string relativePath, string text)
    {
        string p = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(p));
        File.WriteAllText(p, text);
    }

    static int Cli(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    static string Fixture(out string repo, out string game)
    {
        string workshop = T.TempDir(), outDir = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); game = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity {extender \"inventory\" {box {item \"mine\" 24}}} {mass 10} {name \"n\"}}"));
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/t.set", "{t {0 5} {10 6}}"),
            (Bz, "{game {parameters \"a=1;b=2\"} {settings {scoreFinal 1000}}}"));
        Utf8(repo, "data/annotations/common.psv", "*/mass|block|확인|무게|C\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(repo, "editor", "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        CliApp.GameRunning = () => false;
        string repo, game, o;
        string settings = Fixture(out repo, out game);
        string work = T.TempDir();

        // Q1: 깨진 JSON과 pak은 이유를 찍고 1로 끝난다(스택 트레이스 아님)
        string brokenProject = Path.Combine(work, "broken-project.json"), brokenSettings = Path.Combine(work, "broken-settings.json");
        File.WriteAllText(brokenProject, "{broken");
        File.WriteAllText(brokenSettings, "{broken");
        T.Throws<InvalidDataException>(() => EditProject.Load(brokenProject), "a broken project file is InvalidDataException");
        T.Throws<InvalidDataException>(() => EditorSettings.Load(brokenSettings), "a broken settings file is InvalidDataException");
        T.Eq(1, Cli(out o, "build", "--settings", settings, "--project", brokenProject), "build with a broken project ends with 1");
        T.True(o.Contains("읽지 못했다") && o.Contains("broken-project.json") && !o.Contains("   at "), "with the file name and no stack trace");
        T.Eq(1, Cli(out o, "sources", "--settings", brokenSettings), "sources with broken settings ends with 1");
        T.True(o.Contains("읽지 못했다") && o.Contains("broken-settings.json"), "with the file name");
        string game2 = T.TempDir();
        Directory.CreateDirectory(Path.Combine(game2, "resource"));
        File.WriteAllBytes(Path.Combine(game2, "resource", "gamelogic.pak"), new byte[] { 1, 2, 3, 4 });
        var s2 = EditorSettings.Load(settings);
        s2.GameDir = game2;
        string settings2 = Path.Combine(work, "s2", "settings.json");
        s2.Save(settings2);
        T.Eq(1, Cli(out o, "sources", "--settings", settings2), "a damaged pak ends with 1");
        T.True(o.Contains("gamelogic.pak") && !o.Contains("   at "), "and names the pak");

        // Q2: 키 경로를 바꾸는 새 값(블록 한정자, 숫자 행의 첫 값을 낱말로)은 넣기 전에 막는다
        string defText = "{game_entity {extender \"inventory\" {box {item \"mine\" 24}}}}";
        T.True(ValueRules.CheckApplies(Def, defText, new Edit { File = Def, Path = "/game_entity/extender[inventory]", Arg = 0, Old = "\"inventory\"", New = "\"x\"" }) != null, "changing a block qualifier is refused");
        T.True(ValueRules.CheckApplies(Def, defText, new Edit { File = Def, Path = "/game_entity/extender[inventory]/box/item", Arg = 1, Old = "24", New = "5" }) == null, "an ordinary value is fine");
        T.True(ValueRules.CheckApplies(Table, "{t {0 5} {10 6}}", new Edit { File = Table, Path = "/t/#row", Arg = 0, Old = "0", New = "1" }) == null, "a number for a row's first number is fine");
        T.True(ValueRules.CheckApplies(Table, "{t {0 5} {10 6}}", new Edit { File = Table, Path = "/t/#row", Arg = 0, Old = "0", New = "a" }) != null, "a word for a row's first number is refused");
        string project = Path.Combine(work, "p.json");
        T.Eq(1, Cli(out o, "add", "--settings", settings, "--project", project, "--file", Def, "--path", "/game_entity/extender[inventory]", "--new", "\"x\""), "CLI add refuses a qualifier change");
        T.True(o.Contains("키 경로"), "with the reason");

        using (var session = EditorSession.Open(settings, repo, null))
        {
            var unit = session.OpenUnit(Def);
            var qualifier = unit.Rows.First(r => r.Path == "/game_entity/extender[inventory]").Values[0];
            string warning;
            T.True(session.ValidateNew(unit, qualifier, "\"x\"", "shared", out warning) != null, "the session refuses a qualifier change too");

            // Q5: 수정 점검이 빌드와 같은 규칙(새 값 규칙, 인코딩)을 본다
            session.OpenProject(Path.Combine(work, "checks.json"));
            session.Project.Edits.Add(new Edit { Unit = Bz, File = Bz, Path = "/game/parameters", Arg = 0, Old = "\"a=1;b=2\"", New = "\"a=1;b=2;c=3\"" });
            session.Project.Edits.Add(new Edit { Unit = Def, File = Def, Path = "/game_entity/name", Arg = 0, Old = "\"n\"", New = "\"한\"" });
            var checks = session.CheckEdits();
            T.Eq(EditStatus.Invalid, checks[0].Status, "a piece-string rule break is Invalid in the check");
            T.Eq(EditStatus.Invalid, checks[1].Status, "a value the file's encoding cannot hold is Invalid in the check");
        }

        // Q4: 깨진 현지화 pak은 건너뛰고 경고한다(세션은 열린다)
        Directory.CreateDirectory(Path.Combine(game, "localizations"));
        File.WriteAllBytes(Path.Combine(game, "localizations", "default.pak"), new byte[] { 9, 9, 9 });
        using (var session = EditorSession.Open(settings, repo, null))
            T.True(session.Plan.Warnings.Any(w => w.Contains("현지화") && w.Contains("default.pak")), "a damaged localization pak is skipped with a warning");

        // Q9: trace가 게임 설정 값에 '이 유닛만: 불가'라고 찍는다
        T.Eq(0, Cli(out o, "trace", Bz, "--settings", settings), "trace a settings file");
        T.True(o.Contains("이 유닛만: 불가(" + SettingFiles.UnitScopeNote) && !o.Contains("이 유닛만: 가능"), "settings values are shared-only in trace");

        // E1: remove로 수정을 지운다
        T.Eq(0, Cli(out o, "add", "--settings", settings, "--project", project, "--file", Def, "--path", "/game_entity/mass", "--new", "12"), "add an edit");
        T.Eq(0, Cli(out o, "remove", "--project", project, "--file", Def, "--path", "/game_entity/mass"), "remove it");
        T.True(o.Contains("수정을 지웠다"), "remove reports what it removed");
        T.Eq(0, EditProject.Load(project).Edits.Count, "the edit is gone");
        T.Eq(1, Cli(out o, "remove", "--project", project, "--file", Def, "--path", "/game_entity/mass"), "removing a missing edit ends with 1");
        T.True(o.Contains("그 자리의 수정이 없다"), "with the reason");
    }
}
