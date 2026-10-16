using System.IO;
using System.Linq;
using CtaToybox.Core;

// Q5: 수정 점검이 빌드와 같은 규칙으로 손으로 넣은 출력 파일과의 충돌, 키 경로를 바꾸는 예전 수정을 본다
static class CheckEditsTests
{
    const string Q = "entity/-vehicle/x/q/q.def";

    public static void Run()
    {
        string repo, outDir;
        string settings = EditorSessionTests.Fixture(out repo, out outDir);
        // 블록 한정자가 있는 유닛 하나를 더한다(extender "inventory"는 키 경로의 일부다)
        T.Zip(Path.Combine(EditorSettings.Load(settings).GameDir, "resource", "entity", "q5.pak"),
            ("-vehicle/x/q/q.def", "{game_entity {extender \"inventory\" {box {item \"mine\" 24}}}}"));
        using var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(Path.Combine(repo, "editor", "projects", "q5.json"));
        var a = session.OpenUnit(EditorSessionTests.A);
        var mass = session.AddEdit(a, a.Rows.Single(r => r.Path == "/game_entity/mass").Values[0], "12", "shared");
        var mine = session.AddEdit(a, a.Rows.Single(r => r.Path == "/game_entity/box/item").Values[1], "5", "unit");
        T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "both apply with an empty output folder");

        // 손으로 넣은 파일: 빌드는 덮어쓰지 않고 파일 오류로 알린다. 점검도 같은 문장으로 막는다
        string handA = Path.Combine(outDir, "resource", "entity", "-vehicle", "x", "a", "a.def");
        Directory.CreateDirectory(Path.GetDirectoryName(handA));
        File.WriteAllText(handA, "{game_entity}");
        var checks = session.CheckEdits();
        var massCheck = checks.Single(c => c.Edit == mass);
        T.Eq(EditStatus.Invalid, massCheck.Status, "a shared edit whose output is a hand-placed file is Invalid");
        T.True(massCheck.Message.Contains("이 도구가 만들지 않은"), "with the build's reason");
        T.Eq(EditStatus.Invalid, checks.Single(c => c.Edit == mine).Status, "a unit-scoped edit writes the unit file too, so it is Invalid as well");
        var report = session.Build();
        T.True(report.FileErrors.Any(f => f.Contains("이 도구가 만들지 않은")), "the build agrees");
        File.Delete(handA);

        // '이 유닛만' 사본 자리의 손 파일
        string copy = IncludeChain.CopyName(EditorSessionTests.S, EditorSessionTests.A);
        string handCopy = Path.Combine(outDir, "resource", copy.Replace('/', Path.DirectorySeparatorChar));
        File.WriteAllText(handCopy, "{box}");
        checks = session.CheckEdits();
        T.Eq(EditStatus.Applied, checks.Single(c => c.Edit == mass).Status, "the shared edit does not write the copy");
        T.Eq(EditStatus.Invalid, checks.Single(c => c.Edit == mine).Status, "the unit-scoped edit's copy clashes");
        File.Delete(handCopy);

        // 빌드가 만든 파일은 충돌이 아니다
        report = session.Build();
        T.Eq(2, report.Count(EditStatus.Applied), "the build writes both");
        T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "files the build made are not a clash");

        // 키 경로를 바꾸는 예전 수정(Q2 전): 넣을 때는 막히지만 예전 프로젝트에는 남아 있을 수 있다
        var old = new Edit { Unit = Q, File = Q, Path = "/game_entity/extender[inventory]", Occurrence = 0, Arg = 0, Old = "\"inventory\"", New = "\"x\"" };
        session.Project.Edits.Add(old);
        var oldCheck = session.CheckEdits().Single(c => c.Edit == old);
        T.Eq(EditStatus.Invalid, oldCheck.Status, "an old edit that changes the key path is Invalid in the check");
        T.True(oldCheck.Message.Contains("키 경로"), "and says why");
    }
}
