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

        ReviewFindings(settings, repo, work);
    }

    // W-B 최종 검토에서 나온 것(2026-10-15)
    static void ReviewFindings(string settings, string repo, string work)
    {
        string o;

        // I-1: 예전 프로젝트의 '이 유닛만' 한정자 수정이 include 파일에 있어도 빌드는 던지지 않는다. 그 수정만 적용 불가이고 같은 파일의 다른 수정은 된다
        string v = T.TempDir(), outDir = T.TempDir();
        T.Put(v, "entity/v/a.def", "{game_entity\r\n\t(include \"shared.inc\")\r\n\t{mass 10}\r\n}\r\n");
        T.Put(v, "entity/v/shared.inc", "{extender \"inventory\" {box {item \"mine\" 24}}}\r\n");
        using (var stack = new SourceStack(new[] { new SourceLayer("vanilla", v) }))
        {
            stack.Scan();
            var old = new EditProject { Name = "old" };
            old.Edits.Add(new Edit { Unit = "entity/v/a.def", File = "entity/v/shared.inc", Path = "/extender[inventory]", Arg = 0, Old = "\"inventory\"", New = "\"x\"", Scope = "unit" });
            old.Edits.Add(new Edit { Unit = "entity/v/a.def", File = "entity/v/shared.inc", Path = "/extender[inventory]/box/item", Arg = 1, Old = "24", New = "5", Scope = "unit" });
            BuildReport report = null;
            try { report = new ModBuilder(stack, outDir).Build(old); }
            catch (System.InvalidOperationException ex) { T.True(false, "an old qualifier edit must not throw out of the build: " + ex.Message); }
            if (report != null)
            {
                var bad = report.Results.SingleOrDefault(r => r.Edit.Arg == 0);
                T.True(bad != null && bad.Status == EditStatus.Invalid && bad.Message.Contains("키 경로"), "the qualifier edit is reported Invalid with the reason");
                T.Eq(EditStatus.Applied, report.Results.Single(r => r.Edit.Arg == 1).Status, "the other edit in that file still applies");
            }
        }

        // M-10: 목록 파일이 JSON이어도 파일 목록이 없으면(이 도구가 쓰는 꼴이 아니다) 깨진 것으로 보고 멈춘다
        string v2 = T.TempDir(), out2 = T.TempDir();
        T.Put(v2, "set/a.set", "{a {x 1}}");
        File.WriteAllText(Path.Combine(out2, ModBuilder.ManifestName), "{}");
        using (var stack = new SourceStack(new[] { new SourceLayer("vanilla", v2) }))
        {
            stack.Scan();
            var p = new EditProject { Name = "p" };
            p.Edits.Add(new Edit { Unit = "set/a.set", File = "set/a.set", Path = "/a/x", Old = "1", New = "2" });
            var report = new ModBuilder(stack, out2).Build(p);
            T.True(report.FileErrors.Any(f => f.Contains("목록 파일")) && report.WrittenFiles.Count == 0, "a manifest with no file list stops the build");
        }

        // M-3: 내용이 null인 JSON도 파일 이름이 든 오류다. 수정 목록이 null이면 빈 목록이다
        string nullProject = Path.Combine(work, "null-project.json"), nullSettings = Path.Combine(work, "null-settings.json"), noEdits = Path.Combine(work, "no-edits.json");
        File.WriteAllText(nullProject, "null");
        File.WriteAllText(nullSettings, "null");
        File.WriteAllText(noEdits, "{\"name\": \"x\", \"edits\": null}");
        T.Throws<InvalidDataException>(() => EditProject.Load(nullProject), "a null project file is InvalidDataException");
        T.Throws<InvalidDataException>(() => EditorSettings.Load(nullSettings), "a null settings file is InvalidDataException");
        T.Eq(0, EditProject.Load(noEdits).Edits.Count, "a project with null edits has an empty list");
        T.Eq(1, Cli(out o, "remove", "--project", nullProject, "--file", Def, "--path", "/game_entity/mass"), "remove with a null project ends with 1");
        T.True(o.Contains("null-project.json") && !o.Contains("   at "), "with the file name and no stack trace");

        // M-4: pak 안의 항목만 깨져도 어느 pak의 어느 파일인지 알린다
        string game3 = T.TempDir();
        string pak = Path.Combine(game3, "resource", "gamelogic.pak");
        T.Zip(pak, ("set/t.set", "{t {0 5}}"));
        byte[] bytes = File.ReadAllBytes(pak);
        for (int i = 0; i + 11 < bytes.Length; i++)
            if (bytes[i] == 0x50 && bytes[i + 1] == 0x4B && bytes[i + 2] == 0x01 && bytes[i + 3] == 0x02) { bytes[i + 10] = 99; bytes[i + 11] = 0; break; }   // 중앙 디렉터리의 압축 방식
        File.WriteAllBytes(pak, bytes);
        using (var stack = new SourceStack(new[] { new SourceLayer("vanilla", Path.Combine(game3, "resource")) }))
        {
            stack.Scan();
            string message = null;
            try { stack.Read(stack.Find("set/t.set")); }
            catch (InvalidDataException ex) { message = ex.Message; }
            T.True(message != null && message.Contains("gamelogic.pak") && message.Contains("set/t.set"), "a damaged pak entry names the pak and the entry");
        }

        // M-2: remove의 --unit은 '이 유닛만' 수정만 가린다. '전부' 수정은 그 유닛 화면에서 만들었어도 걸리지 않는다
        string two = Path.Combine(work, "two.json");
        var twoProject = new EditProject { Name = "two" };
        twoProject.Edits.Add(new Edit { Unit = Def, File = Def, Path = "/game_entity/mass", Old = "10", New = "11", Scope = "shared" });
        twoProject.Edits.Add(new Edit { Unit = Def, File = Def, Path = "/game_entity/mass", Old = "10", New = "12", Scope = "unit" });
        twoProject.Save(two);
        T.Eq(1, Cli(out o, "remove", "--project", two, "--file", Def, "--path", "/game_entity/mass"), "two edits at one slot need a choice");
        T.True(o.Contains("2개"), "and the output says so");
        T.Eq(0, Cli(out o, "remove", "--project", two, "--file", Def, "--path", "/game_entity/mass", "--unit", Def), "--unit picks the unit-scope edit");
        var left = EditProject.Load(two).Edits;
        T.True(left.Count == 1 && left[0].Scope == "shared", "and leaves the shared edit made from the same unit");
        T.Eq(1, Cli(out o, "remove", "--project", two, "--file", Def, "--path", "/game_entity/mass", "--unit", Def), "--unit does not match a shared edit");
        T.Eq(0, Cli(out o, "remove", "--project", two, "--file", Def, "--path", "/game_entity/mass", "--scope", "shared"), "--scope shared removes it");

        // M-8: 화면과 명령줄도 파일의 인코딩으로 쓸 수 없는 값을 넣기 전에 막는다(빌드·수정 점검과 같은 규칙)
        T.Eq(1, Cli(out o, "add", "--settings", settings, "--project", Path.Combine(work, "enc.json"), "--file", Def, "--path", "/game_entity/name", "--new", "\"한\""), "CLI add refuses a value the file cannot hold");
        T.True(o.Contains("인코딩"), "with the reason");
        using (var session = EditorSession.Open(settings, repo, null))
        {
            var unit = session.OpenUnit(Def);
            var name = unit.Rows.First(r => r.Path == "/game_entity/name").Values[0];
            string warning;
            T.True(session.ValidateNew(unit, name, "\"한\"", "shared", out warning) != null, "the session refuses it too");
            T.True(session.ValidateNew(unit, name, "\"é\"", "shared", out warning) == null, "a Latin-1 letter is fine");
        }
    }
}
