using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// E18: 편집기 세션과 명령줄에서 사용자가 직접 블록을 복제한다
static class BlockCopySessionTests
{
    const string Fields = BlockCopyBuildTests.Fields, VisionSet = BlockCopyBuildTests.VisionSet;

    static ValueView NameValue(UnitView view, string name)
    {
        return view.Rows.Single(r => r.RawPath != null && r.RawPath.EndsWith("/\"" + name + "\"") && r.Path.EndsWith("/\"*\"")).Values.Single();
    }

    public static void Run()
    {
        string repo, outDir;
        string settings = BlockCopyBuildTests.Fixture(out repo, out outDir);
        string projectPath = Path.Combine(repo, "editor", "projects", "e18s.json");
        using (var session = EditorSession.Open(settings, repo, null))
        {
            session.OpenProject(projectPath);
            var view = session.OpenUnit(VisionSet);
            var copy = session.AddBlockCopy(view, NameValue(view, "tank_range_200"), "\"ctb_tank_light\"");
            T.Eq(BlockCopy.Kind, copy.Kind, "the edit is a block copy");
            T.True(File.ReadAllText(projectPath).Contains("\"kind\": \"block-copy\""), "the project keeps the kind");
            view = session.OpenUnit(VisionSet);
            var uncover = view.Rows.Single(r => r.RawPath != null && r.RawPath.Contains("\"ctb_tank_light\"") && r.Path.EndsWith("/uncover")).Values[0];
            T.True(uncover.Editable, "the copy's values can be edited");
            var edit = session.AddEdit(view, uncover, "130", "shared");
            T.True(edit.NamedPath != null && edit.NamedPath.Contains("\"ctb_tank_light\""), "the value edit names the copy");
            int occurrence = edit.Occurrence;
            session.OpenProject(projectPath);
            var reopened = session.Project.Edits.Single(e => e.Kind == null);
            T.Eq(occurrence, reopened.Occurrence, "reopening keeps the slot inside the copy");
            T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "both check");
            // 시야 필드의 spotting2 r은 로스터의 r(연구 단계)이 아니라 거리 배수다
            var labels = ValueLabels.Load(Path.Combine("data", "annotations", "values.psv"));
            T.True(labels.Of("/visions/\"*\"/(\"spotting2\")", new[] { "1.5", "0.66" }, new[] { "r", "r_human" }, "/visions/\"tank_range_200\"/(\"spotting2\")")[0].StartsWith("거리 배수"), "the vision macro r is the distance multiplier");
            T.True(labels.Of("/\"*\"/(\"inf_sniper\")", new[] { "eng", "2" }, new[] { "side", "r" }, "/\"mp/eng/early/sniper\"/(\"inf_sniper\")")[1].Contains("연구 단계"), "the roster r stays the research stage");
            T.True(session.Describe(session.Project.Edits.Single(e => e.Kind == BlockCopy.Kind)).ValueLabel == "블록 복제", "the edit list names it");

            view = session.OpenUnit(VisionSet);
            T.Throws<System.ArgumentException>(() => session.AddBlockCopy(view, NameValue(view, "tank_range_200"), "\"ctb_tank_light\""), "the same new name again");
            T.Throws<System.ArgumentException>(() => session.AddBlockCopy(view, NameValue(view, "tank_range_200"), "\"human\""), "an existing name");
            T.Throws<System.ArgumentException>(() => session.AddBlockCopy(view, NameValue(view, "tank_range_200"), "\"a b\""), "a bad name");
            T.Throws<System.ArgumentException>(() => session.AddBlockCopy(view, uncover, "\"x\""), "a value that is not a block name");

            session.SetEnabled(session.Project.Edits.Single(e => e.Kind == BlockCopy.Kind), false);
            view = session.OpenUnit(VisionSet);
            T.True(!view.Rows.Any(r => r.RawPath != null && r.RawPath.Contains("ctb_tank_light")), "a disabled copy is not shown");
        }

        // 명령줄
        string cliProject = Path.Combine(repo, "editor", "projects", "e18-cli.json");
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Fields, "--path", "/\"*\"", "--occ", "1", "--new", "\"ctb_human\"", "--kind", "block-copy" }, w), "CLI add makes a block copy");
        w = new StringWriter();
        T.Eq(1, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Fields, "--path", "/\"*\"", "--occ", "0", "--new", "\"ctb_human\"", "--kind", "block-copy" }, w), "the same new name again is refused");
        T.True(w.ToString().Contains("이미"), "with the reason");
        w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Fields, "--path", "/\"*\"/uncover", "--occ", "2", "--new", "77" }, w), "CLI add edits a value inside the copy: " + w);
        T.Eq(2, EditProject.Load(cliProject).Edits.Count, "two edits");
        w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "remove", "--project", cliProject, "--file", Fields, "--path", "/\"*\"", "--occ", "1", "--kind", "block-copy" }, w), "CLI remove takes the copy away");
        T.Eq(1, EditProject.Load(cliProject).Edits.Count, "one edit left");
    }
}
