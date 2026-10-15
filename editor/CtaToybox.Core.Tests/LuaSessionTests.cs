// Lua 설정을 세션과 명령줄에서: 색인, 추적, 주석, 계산값, 수정 점검, 모르는 문법
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

static class LuaSessionTests
{
    const string Lua = "script/multiplayer/modes/conquest/runtime_config.lua";

    static int Cli(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        T.True(SettingFiles.IsTop(Lua), "the conquest AI config is a top setting file");

        string repo, outDir, game;
        string settings = SettingsSessionTests.Fixture(out repo, out outDir, out game);
        T.Zip(Path.Combine(game, "resource", "script.pak"), (Lua, LuaTableTests.Sample));
        using (var session = EditorSession.Open(settings, repo, null))
        {
            var entry = session.Index.Units.Single(u => u.Path == Lua);
            T.Eq("settings", entry.EditorKind, "the Lua file is in the index");
            T.True(entry.Numbers >= 5 && entry.Unresolved == 0, "with numbers and no errors");

            var view = session.OpenUnit(Lua);
            T.Eq(14, view.Rows.Count, "one row per Lua value");
            var min = view.Rows.First(r => r.Path == "/conquestconfig.startspawntime/attackmin");
            T.Eq("Bot is attacker.", min.SourceComment, "the developer comment");
            T.Eq("Time from match start before the AI attempts its first purchase.", min.TableComment, "the table comment");
            T.Eq("1000", min.Values[0].Computed, "an expression shows its value");
            T.True(view.Rows.First(r => r.Path == "/conquestconfig.attackerwaves/delta").Values[0].Computed == null, "a plain number has no computed value");
            var nil = view.Rows.First(r => r.Path == "/conquestconfig.attackerwaves/bands/#2/max").Values[0];
            T.True(!nil.Editable, "nil is not editable");
            T.Eq(SettingFiles.UnitScopeNote, min.Values[0].UnitScopeBlocker, "shared only");

            session.OpenProject(Path.Combine(T.TempDir(), "lua.json"));
            session.ApplyRow(view, min, new[] { "5 * 60000" }, "shared");
            T.Eq(EditStatus.Applied, session.CheckEdits().Single().Status, "the check finds the Lua slot");
            var d = session.Describe(session.Project.Edits.Single());
            T.True(d != null && d.RowText == "1 * 1000", "Describe reads the Lua row");
            T.Throws<System.ArgumentException>(() => session.ApplyRow(view, min, new[] { "2 * x" }, "shared"), "a bad Lua value is refused by the session");
            var report = session.Build();
            T.Eq(1, report.Count(EditStatus.Applied), "the build writes the Lua edit");
        }

        string o;
        T.Eq(0, Cli(out o, "show", Lua, "--settings", settings), "show reads Lua");
        T.True(o.Contains("/conquestconfig.startspawntime/attackmin [0] 0=1 * 1000"), "show prints Lua rows");
        string project = Path.Combine(T.TempDir(), "p.json");
        T.Eq(0, Cli(out o, "add", "--settings", settings, "--project", project, "--file", Lua, "--path", "/conquestconfig.attackerwaves/pattern", "--new", "\"flat\""), "add a Lua value");
        T.Eq(1, Cli(out o, "add", "--settings", settings, "--project", project, "--file", Lua, "--path", "/conquestconfig.attackerwaves/pattern", "--new", "flat"), "add refuses a wrong-kind Lua value");
        // CLI build로 게임 설정과 Lua를 함께 빌드한다(Q12)
        T.Eq(0, Cli(out o, "add", "--settings", settings, "--project", project, "--file", SettingFilesTests.Res, "--path", "/standard/globalmaxcp", "--new", "2100"), "add a settings edit too");
        CliApp.GameRunning = () => false;
        T.Eq(0, Cli(out o, "build", "--settings", settings, "--project", project), "CLI build with settings and Lua edits");
        T.True(o.Contains("씀: resource/script/multiplayer/modes/conquest/runtime_config.lua") && o.Contains("씀: resource/" + SettingFilesTests.Res) && o.Contains("적용 2건"), "both files are written");

        // 모드가 읽기기가 모르는 문법으로 바꾼 Lua: 줄 없이 이유만, 이전 수정은 적용 불가
        string repo2, out2, game2;
        string settings2 = SettingsSessionTests.Fixture(out repo2, out out2, out game2);
        T.Zip(Path.Combine(game2, "resource", "script.pak"), (Lua, LuaTableTests.Sample));
        var s2 = EditorSettings.Load(settings2);
        T.Put(Path.Combine(s2.WorkshopDir, "222", "resource"), Lua, "ConquestConfig.X = function() end\n");
        using (var session = EditorSession.Open(settings2, repo2, null))
        {
            var view = session.OpenUnit(Lua);
            T.True(view.Rows.Count == 0 && view.Unresolved.Count == 1 && view.Unresolved[0].StartsWith("Lua: "), "an unreadable Lua file shows why, with no rows");
            session.OpenProject(Path.Combine(T.TempDir(), "old.json"));
            session.Project.Edits.Add(new Edit { Unit = Lua, File = Lua, Path = "/conquestconfig.startspawntime/attackmin", Occurrence = 0, Arg = 0, Old = "1 * 1000", New = "3" });
            T.Eq(EditStatus.TargetMissing, session.CheckEdits().Single().Status, "the old edit no longer finds its slot");
            var report = session.Build();
            T.Eq(1, report.Count(EditStatus.Invalid), "the build reports it instead of writing a broken file");
            T.True(!File.Exists(Path.Combine(out2, "resource", "script", "multiplayer", "modes", "conquest", "runtime_config.lua")), "nothing is written");
        }
    }
}
