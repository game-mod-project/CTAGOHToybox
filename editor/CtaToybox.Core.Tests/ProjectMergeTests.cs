using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// E17: 빌드는 프로젝트 하나만 쓰므로, 다른 프로젝트의 수정을 지금 프로젝트로 합친다(월드워 + 다른 수정)
static class ProjectMergeTests
{
    static Edit E(string file, string path, string old, string @new, string scope = "shared", string unit = null, int arg = 0)
    {
        return new Edit { Unit = unit ?? file, File = file, Path = path, Occurrence = 0, Arg = arg, Old = old, New = @new, Scope = scope };
    }

    public static void Run()
    {
        var mine = new EditProject { Name = "mine" };
        mine.Edits.Add(E("a.def", "/x/mass", "10", "12"));
        mine.Edits.Add(E("a.def", "/x/speed", "5", "6"));
        mine.Edits.Add(E("s.inc", "/box/item", "24", "5", "unit", "a.def", 1));
        var other = new EditProject { Name = "other" };
        other.Edits.Add(E("a.def", "/x/mass", "10", "12"));                   // 같다
        other.Edits.Add(E("a.def", "/X/Speed", "5", "9"));                    // 같은 자리, 다른 값
        other.Edits.Add(E("s.inc", "/box/item", "24", "7", "unit", "b.def", 1)); // 다른 유닛의 '이 유닛만'은 다른 자리
        other.Edits.Add(E("s.inc", "/box/item", "24", "8"));                  // '전부'는 '이 유닛만'과 다른 자리
        var off = E("v.inc", "/\"*\"/h_fov", "150", "360");
        off.Enabled = false; off.NamedPath = "/\"human\"/h_fov";
        other.Edits.Add(off);

        var report = mine.MergeFrom(other);
        T.Eq(3, report.Added, "three edits are new here");
        T.Eq(1, report.Same, "one is already here with the same value");
        T.Eq(1, report.Conflicts.Count, "one slot has a different value");
        T.True(report.Conflicts[0].Contains("/X/Speed") && report.Conflicts[0].Contains("9") && report.Conflicts[0].Contains("6"), "the conflict names the slot and both values");
        T.Eq("6", mine.Edits.Single(e => e.Path == "/x/speed").New, "on a conflict this project's edit stays");
        T.Eq(6, mine.Edits.Count, "six edits after the merge");
        var copied = mine.Edits.Single(e => e.File == "v.inc");
        T.True(copied != off && !copied.Enabled && copied.NamedPath == off.NamedPath, "a copy keeps the fields, including off and the name path");
        T.Eq(0, mine.MergeFrom(other).Added, "merging again adds nothing");

        // 명령줄: merge --project <합칠 곳> --from <가져올 프로젝트>
        string dir = T.TempDir(), a = Path.Combine(dir, "a.json"), b = Path.Combine(dir, "b.json");
        new EditProject { Name = "a", Edits = { E("a.def", "/x/mass", "10", "12") } }.Save(a);
        new EditProject { Name = "b", Edits = { E("a.def", "/x/mass", "10", "13"), E("a.def", "/x/hp", "1", "2") } }.Save(b);
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "merge", "--project", a, "--from", b }, w), "CLI merge");
        T.True(w.ToString().Contains("더함 1") && w.ToString().Contains("충돌 1"), "it reports the counts");
        T.Eq(2, EditProject.Load(a).Edits.Count, "and saves the merged project");
        T.Eq(1, CliApp.Run(new[] { "merge", "--project", a, "--from", Path.Combine(dir, "none.json") }, new StringWriter()), "a missing source project is an error");
        T.Eq(1, CliApp.Run(new[] { "merge", "--project", a, "--from", a }, new StringWriter()), "merging a project into itself is refused");

        // 앱(세션): 지금 프로젝트에 더하고 저장한다. 수정 점검·빌드가 합친 수정을 그대로 쓴다
        string repo, outDir;
        string settings = EditorSessionTests.Fixture(out repo, out outDir);
        using var session = EditorSession.Open(settings, repo, null);
        string p1 = Path.Combine(repo, "editor", "projects", "p1.json"), p2 = Path.Combine(repo, "editor", "projects", "p2.json");
        session.OpenProject(p2);
        var unitA = session.OpenUnit(EditorSessionTests.A);
        session.AddEdit(unitA, unitA.Rows.Single(r => r.Path == "/game_entity/mass").Values[0], "12", "shared");
        session.OpenProject(p1);
        T.Throws<System.ArgumentException>(() => session.MergeProject(p1), "the open project cannot merge itself");
        var merged = session.MergeProject(p2);
        T.Eq(1, merged.Added, "the session merges the other project's edit");
        T.Eq(1, EditProject.Load(p1).Edits.Count, "and saves");
        T.Eq(EditStatus.Applied, session.CheckEdits().Single().Status, "the merged edit is checked like any other");
    }
}
