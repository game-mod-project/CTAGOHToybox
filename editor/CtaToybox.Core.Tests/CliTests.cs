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
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity (include \"s.inc\") {mass 10}}"),
            ("-vehicle/x/a/b.def", "{game_entity (include \"s.inc\")}"),
            ("-vehicle/x/a/s.inc", "{box {item \"mine\" 24}}"));
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
        T.Eq(0, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/x", "--new", "7"), "add again to the same slot succeeds");
        T.Eq(1, EditProject.Load(project).Edits.Count, "adding to the same slot changes the edit instead of adding a second one");
        T.Eq("7", EditProject.Load(project).Edits[0].New, "with the new value");
        T.Eq(0, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/x", "--new", "9"), "put it back");

        T.Eq(0, Run(out o, "build", "--settings", settings, "--project", project), "build succeeds");
        T.True(o.Contains("적용 1건"), "build prints the summary");
        T.Eq("{a {x 9} {name \"n\"}}", Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", "set", "a.set"))), "build writes the file");

        // 모르는 옵션
        T.Eq(2, Run(out o, "show", "set/a.set", "--settings", settings, "--fliter", "x"), "an unknown option is a usage error");
        T.True(o.Contains("--fliter"), "and names the option");

        // units
        const string unitA = "entity/-vehicle/x/a/a.def", unitB = "entity/-vehicle/x/a/b.def", inc = "entity/-vehicle/x/a/s.inc";
        T.Eq(0, Run(out o, "units", "--settings", settings), "units succeeds");
        T.True(o.Contains("새로 만들었다"), "the first run builds the index");
        T.True(File.Exists(Path.Combine(work, "cache", "unit-index.json")), "the cache sits next to the settings file");
        T.True(o.Contains("vehicle") && o.Contains(unitA) && o.Contains(unitB), "units lists kind and path");
        T.True(!o.Contains(inc), "an include file is not a unit");
        T.Eq(0, Run(out o, "units", "--settings", settings, "--find", "B.DEF"), "units --find succeeds");
        T.True(!o.Contains("새로 만들었다"), "the second run uses the cache");
        T.True(o.Contains(unitB) && !o.Contains(unitA), "find filters by path, ignoring case");
        T.Eq(0, Run(out o, "units", "--settings", settings, "--kind", "breed"), "units --kind succeeds");
        T.True(!o.Contains(unitA) && o.Contains("0개"), "no breeds in the fixture");
        T.Eq(2, Run(out o, "units", "--settings", settings, "--kind", "plane"), "an unknown kind is a usage error");
        T.Eq(0, Run(out o, "units", "--settings", settings, "--limit", "1"), "units --limit succeeds");
        T.True(o.Contains(unitA) && !o.Contains(unitB) && o.Contains("2개 가운데 1개"), "limit cuts the list and says so");

        // trace
        T.Eq(0, Run(out o, "trace", unitA, "--settings", settings), "trace succeeds");
        T.True(o.Contains("/game_entity/box/item [0]"), "trace prints expanded rows");
        T.True(o.Contains("1=24  ← " + inc + " /box/item [0] 값 1  공유 2  이 유닛만: 가능"), "an included value: file slot, share count, unit scope possible");
        T.True(o.Contains("0=10  ← " + unitA + " /game_entity/mass [0] 값 0  공유 1"), "the unit's own value is shared by one");
        T.Eq(0, Run(out o, "trace", unitA, "--settings", settings, "--filter", "mass"), "trace --filter succeeds");
        T.True(!o.Contains("box/item"), "the filter hides other rows");
        T.Eq(1, Run(out o, "trace", "entity/-vehicle/x/a/none.def", "--settings", settings), "trace of a missing unit fails");
        T.Eq(2, Run(out o, "trace", "--settings", settings), "trace without a unit is a usage error");

        // add --scope unit, build
        string project2 = Path.Combine(work, "p2.json");
        T.Eq(2, Run(out o, "add", "--settings", settings, "--project", project2, "--file", inc, "--path", "/box/item", "--arg", "1", "--new", "5", "--scope", "everyone"), "an unknown scope is a usage error");
        T.Eq(2, Run(out o, "add", "--settings", settings, "--project", project2, "--file", inc, "--path", "/box/item", "--arg", "1", "--new", "5", "--scope", "unit"), "scope unit needs --unit");
        T.Eq(1, Run(out o, "add", "--settings", settings, "--project", project2, "--file", "set/a.set", "--path", "/a/x", "--new", "5", "--scope", "unit", "--unit", unitA), "a file outside the unit's include chain is refused");
        T.True(o.Contains("include"), "with the reason");
        T.True(!File.Exists(project2), "refused edits are not saved");
        T.Eq(0, Run(out o, "add", "--settings", settings, "--project", project2, "--file", inc, "--path", "/box/item", "--arg", "1", "--new", "5", "--scope", "unit", "--unit", unitA), "add --scope unit succeeds");
        T.Eq("unit", EditProject.Load(project2).Edits[0].Scope, "the scope is saved");
        T.Eq(unitA, EditProject.Load(project2).Edits[0].Unit, "the unit is saved");
        T.Eq(0, Run(out o, "build", "--settings", settings, "--project", project2), "build with a unit-scoped edit succeeds");
        string copy = IncludeChain.CopyName(inc, unitA);
        T.True(File.Exists(Path.Combine(outDir, "resource", copy.Replace('/', Path.DirectorySeparatorChar))), "build writes the copy");
        T.True(File.Exists(Path.Combine(outDir, "resource", unitA.Replace('/', Path.DirectorySeparatorChar))), "and the rewritten unit file");
        T.True(!File.Exists(Path.Combine(outDir, "resource", inc.Replace('/', Path.DirectorySeparatorChar))), "the shared file is not written");
        T.True(!File.Exists(Path.Combine(outDir, "resource", "set", "a.set")), "the previous project's output was removed");
    }
}
