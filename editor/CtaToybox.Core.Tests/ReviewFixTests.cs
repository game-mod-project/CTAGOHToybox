using System;
using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

// 4단계 최종 리뷰의 Important 지적을 재현하는 테스트
static class ReviewFixTests
{
    public static void Run()
    {
        // I1: 유닛 자신의 파일에서 '전부'와 '이 유닛만'이 같은 자리를 고치면 '이 유닛만'이 이긴다(화면 EditFor와 같다)
        string vanilla = T.TempDir(), outDir = T.TempDir();
        T.Put(vanilla, "set/a.set", "{a {x 1}}");
        using (var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) }))
        {
            stack.Scan();
            var project = new EditProject { Name = "t" };
            project.Edits.Add(new Edit { Unit = "set/a.set", File = "set/a.set", Path = "/a/x", Old = "1", New = "2" });
            project.Edits.Add(new Edit { Unit = "set/a.set", File = "set/a.set", Path = "/a/x", Old = "1", New = "3", Scope = "unit" });
            var report = new ModBuilder(stack, outDir).Build(project);
            T.Eq("{a {x 3}}", Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", "set", "a.set"))), "I1: the unit-scoped edit wins on the unit's own file");
            T.Eq(EditStatus.Applied, report.Results.Single(r => r.Edit.Scope == "unit").Status, "I1: and is reported as applied");
        }

        string repo, outDir2;
        string settings = EditorSessionTests.Fixture(out repo, out outDir2);
        using var session = EditorSession.Open(settings, repo, null);

        // I2: 처음 연 프로젝트가 깨졌으면 프로젝트가 없다. 수정과 빌드는 이유를 담은 InvalidOperationException이다
        string broken = Path.Combine(repo, "broken.json");
        File.WriteAllText(broken, "{ not json");
        T.Throws<InvalidDataException>(() => session.OpenProject(broken), "I2: a broken first project is reported");
        T.True(session.Project == null, "I2: and there is no project");
        var a = session.OpenUnit(EditorSessionTests.A);
        var mass = a.Rows.Single(r => r.Path == "/game_entity/mass").Values[0];
        T.Throws<InvalidOperationException>(() => session.AddEdit(a, mass, "12", "shared"), "I2: AddEdit without a project");
        T.Throws<InvalidOperationException>(() => session.Build(), "I2: Build without a project");
        T.Eq(0, session.CheckEdits().Count, "I2: CheckEdits without a project is empty");

        // I4: 프로젝트를 저장하지 못하면 메모리의 수정 목록도 바뀌지 않는다
        string projectPath = Path.Combine(repo, "editor", "projects", "ro.json");
        session.OpenProject(projectPath);
        var e1 = session.AddEdit(a, mass, "12", "shared");
        File.SetAttributes(projectPath, FileAttributes.ReadOnly);
        try
        {
            T.Throws<UnauthorizedAccessException>(() => session.AddEdit(a, mass, "13", "shared"), "I4: a failed save is reported");
            T.Eq("12", e1.New, "I4: a replaced edit keeps its value when the save fails");
            var mine = a.Rows.Single(r => r.Path == "/game_entity/box/item").Values[1];
            T.Throws<UnauthorizedAccessException>(() => session.AddEdit(a, mine, "5", "unit"), "I4: adding fails too");
            T.Eq(1, session.Project.Edits.Count, "I4: the new edit is not kept in memory");
            T.Throws<UnauthorizedAccessException>(() => session.SetEnabled(e1, false), "I4: toggling fails");
            T.Eq(true, e1.Enabled, "I4: and is rolled back");
            T.Throws<UnauthorizedAccessException>(() => session.RemoveEdit(e1), "I4: removing fails");
            T.Eq(1, session.Project.Edits.Count, "I4: and is rolled back");
        }
        finally { File.SetAttributes(projectPath, FileAttributes.Normal); }
    }
}
