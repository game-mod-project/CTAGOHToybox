using System.IO;
using System.Text;
using CtaToybox.Cli;
using CtaToybox.Core;

static class CliTests
{
    static int Run(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        string game = T.TempDir(), workshop = T.TempDir(), outDir = T.TempDir(), work = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), ("set/a.set", "{a {x 1} {name \"n\"}}"));
        T.Put(Path.Combine(workshop, "111", "resource"), "set/a.set", "{a {x 5} {name \"n\"}}");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_111:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json"), project = Path.Combine(work, "p.json");
        string o;

        T.Eq(2, Run(out o), "no command is a usage error");
        T.Eq(2, Run(out o, "frobnicate"), "an unknown command is a usage error");
        T.Eq(1, Run(out o, "sources", "--settings", settings), "a missing settings file fails");
        T.True(o.Contains("init"), "the failure tells the user to run init");

        T.Eq(0, Run(out o, "init", "--settings", settings, "--game", game, "--workshop", workshop, "--options", options, "--out", outDir), "init succeeds");
        T.Eq(Path.GetFileName(outDir), EditorSettings.Load(settings).OutputModName, "init derives the mod name from the output folder");
        T.Eq(2, Run(out o, "init", "--settings", settings, "--game", game), "init without required options is a usage error");

        T.Eq(0, Run(out o, "sources", "--settings", settings), "sources succeeds");
        T.True(o.Contains("vanilla") && o.Contains("mod_111"), "sources lists the layers");
        T.True(o.Contains("켜져 있다"), "sources reports the output mod as active");

        T.Eq(0, Run(out o, "show", "set/a.set", "--settings", settings), "show succeeds");
        T.True(o.Contains("mod_111"), "show names the winning layer");
        T.True(o.Contains("/a/x [0] 0=5"), "show prints path, occurrence and indexed values");
        T.True(o.Contains("/a/name [0] 0=\"n\""), "show prints quoted values with their quotes");
        T.Eq(0, Run(out o, "show", "set/a.set", "--filter", "name", "--settings", settings), "show with a filter succeeds");
        T.True(!o.Contains("/a/x"), "the filter hides other rows");
        T.Eq(1, Run(out o, "show", "set/zzz.set", "--settings", settings), "show of a missing file fails");

        T.Eq(0, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/x", "--new", "9"), "add succeeds");
        var p = EditProject.Load(project);
        T.Eq("5", p.Edits[0].Old, "add records the current source value as old");
        T.Eq("set/a.set", p.Edits[0].Unit, "unit defaults to the file");
        T.Eq(1, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/x", "--new", "1 2"), "add refuses an invalid value");
        T.Eq(1, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/nope", "--new", "1"), "add refuses an unknown key");
        T.Eq(1, EditProject.Load(project).Edits.Count, "refused edits are not saved");

        T.Eq(0, Run(out o, "build", "--settings", settings, "--project", project), "build succeeds");
        T.True(o.Contains("적용 1건"), "build prints the summary");
        T.Eq("{a {x 9} {name \"n\"}}", Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", "set", "a.set"))), "build writes the file");
    }
}
