using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// 팩션 강화: 컨퀘스트 자원과 명령줄 faction-preset
static class FactionPresetCliTests
{
    const string Resources = "{Standard\n\t{GlobalMaxCP 550}\n\t{StageCP 60 80 100 120 140}\n\t{Bots\n\t\t{StartMP 1.00 min 99999 attackerMultiplier 1.00}\n\t}\n\t{Resources\n\t\t{MP\n\t\t\t{StartVal 900}\n\t\t\t{WinGain \"1:200 4:250\"}\n\t\t\t{LoseGain \"1:100\"}\n\t\t}\n\t\t{SP\n\t\t\t{StartVal 3}\n\t\t\t{WinGain 1}\n\t\t\t{LoseGain 0}\n\t\t}\n\t\t{AP\n\t\t\t{StartVal 1000}\n\t\t}\n\t\t{RP\n\t\t\t{StartVal 6}\n\t\t}\n\t}\n}\n";

    static int Cli(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        string repo, outDir, o;
        string settings = FactionPresetTests.Fixture(out repo, out outDir, ("set/dynamic_campaign/resources_standard.set", Resources));
        using (var s = EditorSession.Open(settings, repo, null))
        {
            var project = new EditProject();
            var report = new FactionReport();
            FactionPreset.AddConquest(project, FactionPresetTests.Context(s), new ConquestValues(), report);
            var build = new ModBuilder(s.Stack, outDir).Build(project);
            T.Eq(0, build.Results.Count(r => r.Status != EditStatus.Applied), "conquest edits apply: " + string.Join("; ", build.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Message)));
            string text = FactionPresetTests.Out(outDir, "set/dynamic_campaign/resources_standard.set");
            T.True(text.Contains("{StartVal 99999}") && text.Contains("{StartVal 999}"), "start values: " + text);
            T.True(text.Contains("{StageCP 9999 9999 9999 9999 9999}") && text.Contains("{GlobalMaxCP 9999}"), "stage and reserve limits");
            T.True(text.Contains("{WinGain \"1:99999\"}") && text.Contains("{LoseGain \"1:99999\"}") && text.Contains("{WinGain 999}") && text.Contains("{LoseGain 999}"), "win and lose gains");
            T.True(text.Contains("{StartMP 1.00 min 99999 attackerMultiplier 1.00}"), "bots are untouched");
            T.True(report.Skipped.Count(x => x.Contains("파일이 없다")) == 3, "the three missing difficulty files are reported");
        }

        string super = Path.Combine(repo, "super.txt");
        File.WriteAllText(super, "# test\nentity/-vehicle/+germany/tank_heavy/maus/\n");
        string projectPath = Path.Combine(repo, "editor", "projects", "boost-ger.json");
        string basePath = Path.Combine(repo, "editor", "projects", "base.json");
        var baseProject = new EditProject { Name = "base" };
        baseProject.Edits.Add(new Edit { Unit = "set/dynamic_campaign/resources_standard.set", File = "set/dynamic_campaign/resources_standard.set", Path = "/standard/bots/startmp", Arg = 0, Old = "1.00", New = "1.50", Scope = "shared" });
        baseProject.Save(basePath);
        T.Eq(0, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--base", basePath, "--project", projectPath, "--super", super), "faction-preset: " + o);
        T.True(o.Contains("군대 ger") && o.Contains("무기 사본: 3"), "the report counts: " + o);
        var saved = EditProject.Load(projectPath);
        T.True(saved.Edits.Any(e => e.New == "1.50"), "the base edits come along");
        T.True(saved.Rules.Count == 11 && saved.Edits.Any(e => e.Kind == FileCopy.Kind) && saved.Edits.Any(e => e.Kind == AddLine.Kind), "rules, copies and lines are saved");
        int count = saved.Edits.Count;
        T.Eq(1, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--project", projectPath, "--super", super), "an existing project needs --force");
        T.Eq(0, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--base", basePath, "--project", projectPath, "--super", super, "--force"), "--force");
        T.Eq(count, EditProject.Load(projectPath).Edits.Count, "the same command makes the same project");
        T.Eq(2, Cli(out o, "faction-preset", "--settings", settings, "--army", "xyz", "--project", projectPath), "an unknown army is a usage error");
    }
}
