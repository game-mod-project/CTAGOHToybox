using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// F1: 명령줄 rule
static class BulkRulesCliTests
{
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
        string settings = BulkRulesTests.Fixture(out repo, out outDir);
        string project = Path.Combine(repo, "editor", "projects", "cli-bulk.json");

        T.Eq(0, Cli(out o, "rule", "preview", "--settings", settings, "--kind", "tank", "--path", "/tank_light/", "--key", "**/locomotion/maxspeed", "--mul", "1.1"), "preview");
        T.True(o.Contains("바뀜 2건(유닛 2 / 대상 2)") && o.Contains("45 → 50"), "the preview counts and lists changes: " + o);
        T.True(!File.Exists(project), "preview writes nothing");

        T.Eq(0, Cli(out o, "rule", "add", "--settings", settings, "--project", project, "--kind", "tank", "--path", "/tank_light/", "--key", "**/locomotion/maxspeed", "--mul", "1.1", "--name", "light speed"), "add");
        var saved = EditProject.Load(project);
        string id = saved.Rules.Single().Id;
        T.Eq(2, saved.Edits.Count(e => e.Rule == id), "two rule edits saved");

        T.Eq(0, Cli(out o, "rule", "list", "--settings", settings, "--project", project), "list");
        T.True(o.Contains(id) && o.Contains("light speed") && o.Contains("수정 2건"), "list shows the rule");

        T.Eq(0, Cli(out o, "rule", "edit", "--settings", settings, "--project", project, "--id", id, "--add", "5"), "edit");
        T.Eq("50", EditProject.Load(project).Edits.First(e => e.Rule == id).New, "the edit recomputed (45 + 5)");

        T.Eq(0, Cli(out o, "rule", "disable", "--settings", settings, "--project", project, "--id", id), "disable");
        T.Eq(0, EditProject.Load(project).Edits.Count(e => e.Rule == id), "no edits while off");
        T.Eq(0, Cli(out o, "rule", "enable", "--settings", settings, "--project", project, "--id", id), "enable");
        T.Eq(0, Cli(out o, "rule", "reapply", "--settings", settings, "--project", project, "--id", id), "reapply");
        T.Eq(2, EditProject.Load(project).Edits.Count(e => e.Rule == id), "back on");

        T.Eq(1, Cli(out o, "rule", "remove", "--settings", settings, "--project", project, "--id", "nope0000"), "an unknown id is an error");
        T.Eq(0, Cli(out o, "rule", "remove", "--settings", settings, "--project", project, "--id", id), "remove");
        var after = EditProject.Load(project);
        T.True(after.Rules == null && after.Edits.Count == 0, "the rule and its edits are gone");

        T.Eq(2, Cli(out o, "rule", "preview", "--settings", settings, "--kind", "tank", "--key", "**/x", "--mul", "1", "--add", "2"), "two operations are a usage error");
        T.Eq(2, Cli(out o, "rule", "explode", "--settings", settings), "an unknown subcommand is a usage error");
        T.Eq(0, Cli(out o, "rule", "preview", "--settings", settings, "--kind", "tank", "--key", "**/locomotion/maxspeed", "--arg", "all", "--mul", "2"), "--arg all");
        T.True(o.Contains("바뀜 3건"), "every tank without a path filter");
    }
}
