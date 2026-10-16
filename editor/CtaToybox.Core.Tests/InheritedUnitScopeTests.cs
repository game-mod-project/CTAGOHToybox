using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// E14: 원형에서 물려받은 값의 '이 유닛만'. 자식 파일의 맨 위 블록에 같은 키를 덧붙인다(G16: 자식 파일의 같은 키가 이긴다)
static class InheritedUnitScopeTests
{
    const string Bullet = "set/stuff/e14/bullet.pattern", Gun = "set/stuff/e14/gun", Gun2 = "set/stuff/e14/gun2";

    static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/stuff/e14/.presets", "(define \"fast\" {rate %r})"),
            (Bullet, "{bullet\n {mass 5}\n {Speed 800 2}\n {tag \"a\"}\n {tag \"b\"}\n {inventory {weight 10}}\n (mod \"mp\" {cal 9})\n {parameters \"ap\" {pen 90}}\n (\"fast\" r(3))\n}"),
            (Gun, "{from \"pattern bullet\"\n {range 500}\n}"),
            (Gun2, "{from \"pattern bullet\"\n {range 400}\n}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    static ValueView Value(UnitView view, string path, int index = 0)
    {
        return view.Rows.First(r => r.From == Bullet && r.Path == path).Values[index];
    }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(Path.Combine(repo, "editor", "projects", "e14.json"));
        var gun = session.OpenUnit(Gun);

        // 덧붙일 수 있는 줄: 맨 위 블록 바로 아래, 물려받는 상태, 원형에 한 번, 조건 밖, 계산된 값 없음
        T.True(Value(gun, "/from/speed", 1).UnitScopeBlocker == null, "a plain inherited key can be unit-scoped");
        T.True(Value(gun, "/from/tag").UnitScopeBlocker.Contains("여러 번"), "a key the pattern writes twice cannot");
        T.True(Value(gun, "/from/inventory/weight").UnitScopeBlocker.Contains("맨 위 블록 바로 아래"), "a key inside a wrapping block cannot");
        T.True(Value(gun, "/from/cal").UnitScopeBlocker.Contains("조건"), "a key inside (mod ...) cannot");
        T.True(Value(gun, "/from/parameters").UnitScopeBlocker != null, "a block with blocks inside cannot");
        T.True(Value(gun, "/from/rate").UnitScopeBlocker != null, "a value made by a macro cannot");
        T.True(gun.Rows.Where(r => r.IsMacroCall && r.From == Bullet).SelectMany(r => r.Values).All(v => v.UnitScopeBlocker != null), "a pattern's macro call row cannot");

        var speed = session.AddEdit(gun, Value(gun, "/from/speed", 1), "7", "unit");
        T.Eq(Bullet, speed.File, "the edit points at the pattern value");
        var range = session.AddEdit(gun, gun.Rows.Single(r => r.From == null && r.Path == "/from/range").Values[0], "600", "unit");
        session.RefreshEdits(gun);
        T.True(Value(gun, "/from/speed", 1).Edit == speed, "the inherited value shows its edit");
        T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "the check accepts both");

        var report = session.Build();
        T.Eq(2, report.Count(EditStatus.Applied), "both apply");
        string gunOut = Path.Combine(outDir, "resource", "set", "stuff", "e14", "gun");
        T.True(File.Exists(gunOut), "the unit file is written");
        T.True(!File.Exists(Path.Combine(outDir, "resource", "set", "stuff", "e14", "bullet.pattern")), "the pattern is not written");
        T.True(!File.Exists(Path.Combine(outDir, "resource", "set", "stuff", "e14", "gun2")), "the other heir is not touched");
        string text = File.ReadAllText(gunOut);
        var parsed = SdlCore.Parse(text);
        T.Eq(0, parsed.Errors.Count, "the written file parses");
        var rows = KeyLocator.Rows(parsed.Root);
        var added = rows.Single(r => r.Path == "/from/speed");
        T.Eq("Speed", added.RawPath.Substring(added.RawPath.LastIndexOf('/') + 1), "the key keeps the pattern's spelling");
        T.Eq(new[] { "800", "7" }, added.Leaves.Select(l => l.Text).ToArray(), "the whole row with the edited value");
        T.Eq("600", rows.Single(r => r.Path == "/from/range").Leaves[0].Text, "the unit's own edit is in the same file");

        // 원본이 바뀌면 덧붙이지 않는다
        speed.Old = "3";
        report = session.Build();
        T.Eq(EditStatus.SourceChanged, report.Results.Single(r => r.Edit == speed).Status, "a changed pattern value is SourceChanged");
        T.True(!KeyLocator.Rows(SdlCore.Parse(File.ReadAllText(gunOut)).Root).Any(r => r.Path == "/from/speed"), "and nothing is appended");
        T.Eq(EditStatus.SourceChanged, session.CheckEdits().Single(c => c.Edit == speed).Status, "the check agrees");

        // 막힌 줄을 예전 프로젝트처럼 직접 넣어도 빌드와 점검이 막는다
        var tag = new Edit { Unit = Gun, File = Bullet, Path = "/bullet/tag", Occurrence = 0, Arg = 0, Old = "\"a\"", New = "\"z\"", Scope = "unit" };
        session.Project.Edits.Add(tag);
        T.Eq(EditStatus.Invalid, session.CheckEdits().Single(c => c.Edit == tag).Status, "the check refuses a blocked inherited row");
        T.Eq(EditStatus.Invalid, session.Build().Results.Single(r => r.Edit == tag).Status, "and so does the build");

        // 명령줄도 같은 규칙이다
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "trace", Gun, "--settings", settings, "--filter", "speed" }, w), "trace an heir");
        T.True(w.ToString().Contains("이 유닛만: 가능"), "trace says an inherited plain key can be unit-scoped");
        string cliProject = Path.Combine(repo, "editor", "projects", "e14-cli.json");
        w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Bullet, "--path", "/bullet/mass", "--new", "6", "--scope", "unit", "--unit", Gun }, w), "CLI add accepts it");
        w = new StringWriter();
        T.Eq(1, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Bullet, "--path", "/bullet/tag", "--new", "\"z\"", "--scope", "unit", "--unit", Gun }, w), "CLI add refuses a repeated key");
        T.True(w.ToString().Contains("여러 번"), "with the reason");
    }
}
