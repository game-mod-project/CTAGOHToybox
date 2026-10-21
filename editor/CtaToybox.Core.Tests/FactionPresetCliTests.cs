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
        string settings = FactionPresetTests.Fixture(out repo, out outDir, ("set/dynamic_campaign/resources_standard.set", Resources),
            ("set/multiplayer/games/presets/preset_late.inc", "(define \"standard_all\"\n {common (include \"global_parameters/budget.inc\" s(%start) f(%finish) d(%doctrine))}\n)\n{presets\n {\"s:late\"\n  (\"standard_all\" start(200) finish(4500) doctrine(200) cp(0:100,900:150,1800:200))\n }\n}\n"),
            ("set/multiplayer/games/presets/laststand_late.inc", "{presets\n {\"l:late\"\n  (\"standard_all\" start_a(500) finish_a(1800) start_b(500) finish_b(4500) doctrine(200) cp_a(0:80,900:100) cp_b(0:120,900:160))\n }\n}\n"),
            ("set/multiplayer/games/presets/frontlines_late.inc", "{presets\n {\"f:late\"\n  (\"standard_all\" start_a(4500) start_b(2800))\n }\n}\n"),
            ("set/multiplayer/games/presets/global_parameters/bots_generic.inc", "{easy\n {balancingFactorBonus\n  {cp 0.80}\n  {mpStart 1.10}\n  {mpFinish 0.8}\n  {payback 0.8}\n }\n}\n"));
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

            var sk = new EditProject();
            FactionPreset.AddSkirmish(sk, FactionPresetTests.Context(s), 50, report);
            var b2 = new ModBuilder(s.Stack, outDir).Build(sk);
            T.Eq(0, b2.Results.Count(r => r.Status != EditStatus.Applied), "skirmish edits apply: " + string.Join("; ", b2.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Message)));
            string preset = FactionPresetTests.Out(outDir, "set/multiplayer/games/presets/preset_late.inc");
            T.True(preset.Contains("start(10000) finish(225000) doctrine(200) cp(0:5000,900:7500,1800:10000)"), "player MP and CP ×50, doctrine stays, macro body untouched: " + preset);
            T.True(preset.Contains("s(%start)"), "the macro body keeps %start");
            string last = FactionPresetTests.Out(outDir, "set/multiplayer/games/presets/laststand_late.inc");
            T.True(last.Contains("start_a(25000) finish_a(90000) start_b(500) finish_b(4500) doctrine(200) cp_a(0:4000,900:5000) cp_b(0:120,900:160)"), "last stand: team a only: " + last);
            T.Eq(null, FactionPresetTests.Out(outDir, "set/multiplayer/games/presets/frontlines_late.inc"), "frontlines untouched");
            string bots = FactionPresetTests.Out(outDir, "set/multiplayer/games/presets/global_parameters/bots_generic.inc");
            T.True(bots.Contains("{cp 0.016}") && bots.Contains("{mpStart 0.022}") && bots.Contains("{mpFinish 0.016}") && bots.Contains("{payback 0.8}"), "bot factors ÷50, payback stays: " + bots);
        }

        string super = Path.Combine(repo, "super.txt");
        File.WriteAllText(super, "# test\nentity/-vehicle/+germany/tank_heavy/maus/\n");
        string projectPath = Path.Combine(repo, "editor", "projects", "boost-ger.json");
        string basePath = Path.Combine(repo, "editor", "projects", "base.json");
        var baseProject = new EditProject { Name = "base" };
        baseProject.Edits.Add(new Edit { Unit = "set/dynamic_campaign/resources_standard.set", File = "set/dynamic_campaign/resources_standard.set", Path = "/standard/bots/startmp", Arg = 0, Old = "1.00", New = "1.50", Scope = "shared" });
        baseProject.Save(basePath);
        string doc = Path.Combine(repo, "docs", "boost-ger.md");
        T.Eq(0, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--base", basePath, "--project", projectPath, "--super", super, "--doc", doc), "faction-preset: " + o);
        T.True(o.Contains("군대 ger") && o.Contains("무기 사본: 3"), "the report counts: " + o);
        T.True(File.Exists(doc) && File.ReadAllText(doc).StartsWith("# 강화-독일 수정 내용"), "faction-preset writes the doc (and its folder)");
        T.True(o.Contains(doc), "and says where");
        var saved = EditProject.Load(projectPath);
        T.True(saved.Edits.Any(e => e.New == "1.50"), "the base edits come along");
        T.True(saved.Rules.Count == 11 && saved.Edits.Any(e => e.Kind == FileCopy.Kind) && saved.Edits.Any(e => e.Kind == AddLine.Kind), "rules, copies and lines are saved");
        int count = saved.Edits.Count;
        T.Eq(1, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--project", projectPath, "--super", super), "an existing project needs --force");
        T.Eq(0, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--base", basePath, "--project", projectPath, "--super", super, "--force", "--doc", doc), "--force");
        T.Eq(count, EditProject.Load(projectPath).Edits.Count, "the same command makes the same project");
        T.Eq(2, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--project", projectPath, "--super", super, "--force", "--supply-limit", "-1", "--doc", doc), "a negative supply limit is a usage error");
        T.Eq(2, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--project", projectPath, "--super", super, "--force", "--supply-limit", "many", "--doc", doc), "a supply limit must be a whole number");
        T.Eq(0, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--base", basePath, "--project", projectPath, "--super", super, "--force", "--doc", doc, "--clip-min-gap", "2", "--clip-gap", "0.4", "--supply-radius", "2", "--supply-limit", "9000"), "the new options parse: " + o);
        T.True(File.ReadAllText(doc).Contains("보급 반경 ×2") && File.ReadAllText(doc).Contains("보급 총량 9000") && File.ReadAllText(doc).Contains("발 사이 2초 이상"), "and reach the doc");
        T.Eq(2, Cli(out o, "faction-preset", "--settings", settings, "--army", "xyz", "--project", projectPath), "an unknown army is a usage error");
    }
}
