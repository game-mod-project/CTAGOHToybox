using System.IO;
using CtaToybox.Cli;

// 스폰 모드: 명령줄 spawn-menu(Task 8)
static class SpawnCliTests
{
    public static void Run()
    {
        string repo, stat, spawn;
        string settings = SpawnFixture.Make(out repo, out stat, out spawn);
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "spawn-menu", "preview", "--settings", settings }, w), "preview: " + w);
        T.True(w.ToString().Contains("맵 2(캠페인 1, 멀티 1)"), "preview counts: " + w);
        T.True(!Directory.Exists(spawn), "preview writes nothing");

        var running = CliApp.GameRunning;
        try
        {
            CliApp.GameRunning = () => true;
            w = new StringWriter();
            T.Eq(1, CliApp.Run(new[] { "spawn-menu", "build", "--settings", settings }, w), "build refuses while the game runs");
            CliApp.GameRunning = () => false;
            w = new StringWriter();
            T.Eq(0, CliApp.Run(new[] { "spawn-menu", "build", "--settings", settings }, w), "build: " + w);
        }
        finally { CliApp.GameRunning = running; }
        T.True(File.Exists(Path.Combine(spawn, "mod.info")) && File.Exists(Path.Combine(spawn, ".ctatoybox-spawn.json")), "mod.info and own list");
        T.True(File.Exists(Path.Combine(spawn, "resource", "map", "ctatoybox_spawn", "menu.mi")), "menu written");
        T.True(File.Exists(Path.Combine(spawn, "localizations", "default", "interface", "text", "mission", "ctatoybox_spawn.pot")), "menu words written");
        w = new StringWriter();
        T.True(File.Exists(Path.Combine(spawn, "resource", SpawnFixture.UnitsGer.Replace('/', Path.DirectorySeparatorChar))), "rosters written by default");
        T.True(File.Exists(Path.Combine(spawn, "resource", "map", "ctatoybox_spawn", "templates_ger.mi")), "template file written");
        w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "spawn-menu", "build", "--settings", settings, "--no-roster" }, w), "rebuild without the roster: " + w);
        T.True(w.ToString().Contains("지웠다 ") && !File.Exists(Path.Combine(spawn, "resource", SpawnFixture.UnitsGer.Replace('/', Path.DirectorySeparatorChar))), "--no-roster removes the roster files: " + w);
        w = new StringWriter();
        T.Eq(2, CliApp.Run(new[] { "spawn-menu", "nope", "--settings", settings }, w), "unknown sub-command is a usage error");
        T.Eq(2, CliApp.Run(new[] { "spawn-menu", "preview", "--bogus", "1" }, new StringWriter()), "unknown option is a usage error");
    }
}
