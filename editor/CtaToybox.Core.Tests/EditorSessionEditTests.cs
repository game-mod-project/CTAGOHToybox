using System;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class EditorSessionEditTests
{
    public static void Run()
    {
        string repo, outDir;
        string settings = EditorSessionTests.Fixture(out repo, out outDir);
        using var session = EditorSession.Open(settings, repo, null);
        string projectPath = Path.Combine(repo, "editor", "projects", "p.json");
        session.OpenProject(projectPath);

        var a = session.OpenUnit(EditorSessionTests.A);
        var mass = a.Rows.Single(r => r.Path == "/game_entity/mass").Values[0];
        var mine = a.Rows.Single(r => r.Path == "/game_entity/box/item").Values[1];
        string warning;

        T.True(session.ValidateNew(a, mass, "12", "shared", out warning) == null && warning == null, "a number for a number is fine");
        T.True(session.ValidateNew(a, mass, "fast", "shared", out warning) == null && warning != null, "a word for a number is a warning");
        T.True(session.ValidateNew(a, mass, "1 2", "shared", out warning) != null, "two tokens are refused");
        T.True(session.ValidateNew(a, mass, "12", "everyone", out warning) != null, "an unknown scope is refused");
        var gunName = a.Rows.Single(r => r.Path == "/game_entity/weapon").Values[0];
        T.True(session.ValidateNew(a, gunName, "\"gun_b\"", "shared", out warning) == null, "a quoted value for a quoted value");

        T.Throws<ArgumentException>(() => session.AddEdit(a, mass, "1 2", "shared"), "AddEdit refuses what ValidateNew refuses");
        T.True(!File.Exists(projectPath), "a refused edit does not write the project");

        var e1 = session.AddEdit(a, mass, "12", "shared");
        T.True(File.Exists(projectPath), "adding an edit saves the project");
        T.Eq("10", e1.Old, "old is the current source value");
        T.Eq(EditorSessionTests.A, e1.Unit, "the unit is recorded");
        session.RefreshEdits(a);
        T.True(mass.Edit == e1, "the value now shows its edit");

        var e2 = session.AddEdit(a, mine, "5", "unit");
        T.Eq("unit", e2.Scope, "unit scope");
        T.Eq(EditorSessionTests.S, e2.File, "the edit points at the include file");
        var e3 = session.AddEdit(a, mine, "6", "unit");
        T.True(e3 == e2, "the same slot and scope replaces, not adds");
        T.Eq("6", e2.New, "with the new value");
        T.Eq(2, session.Project.Edits.Count, "two edits");
        T.Eq(2, EditProject.Load(projectPath).Edits.Count, "two edits on disk");

        var b = session.OpenUnit(EditorSessionTests.B);
        var bMine = b.Rows.Single(r => r.Path == "/game_entity/box/item").Values[1];
        T.True(bMine.Edit == null, "another unit does not see a unit-scoped edit of a");
        var shared = session.AddEdit(b, bMine, "30", "shared");
        session.RefreshEdits(a);
        T.True(mine.Edit == e2, "for a, its unit-scoped edit wins over the shared one on the same slot");
        session.RefreshEdits(b);
        T.True(bMine.Edit == shared, "for b, the shared edit shows");

        // "이 유닛만"이 안 되는 자리: 화면이 막지 못해도 세션이 막는다
        var blocked = new ValueView { Trace = mine.Trace, UnitScopeBlocker = "이유" };
        T.Throws<ArgumentException>(() => session.AddEdit(a, blocked, "7", "unit"), "a blocked slot cannot be unit-scoped");

        var checks = session.CheckEdits();
        T.Eq(3, checks.Count, "one check per enabled edit");
        T.True(checks.All(c => c.Status == EditStatus.Applied), "all apply to the current sources");
        T.Eq("10", checks.Single(c => c.Edit == e1).Current, "current source value is reported");

        e1.Old = "99";
        var stale = session.CheckEdits().Single(c => c.Edit == e1);
        T.Eq(EditStatus.SourceChanged, stale.Status, "a stale old value is SourceChanged");
        T.Eq("10", stale.Current, "with the current value");
        session.Rebase(e1);
        T.Eq("10", e1.Old, "rebase sets old to the current value");
        T.Eq("10", EditProject.Load(projectPath).Edits.Single(e => e.Path == "/game_entity/mass").Old, "and saves");

        var report = session.Build();
        T.Eq(3, report.Count(EditStatus.Applied), "build applies all three");
        T.True(File.Exists(Path.Combine(outDir, "resource", "entity", "-vehicle", "x", "a", "a.def")), "the unit file is written");

        session.SetEnabled(e1, false);
        T.Eq(false, EditProject.Load(projectPath).Edits.Single(e => e.Path == "/game_entity/mass").Enabled, "disabling saves");
        T.Eq(2, session.CheckEdits().Count, "a disabled edit is not checked");
        session.RemoveEdit(shared);
        T.Eq(2, EditProject.Load(projectPath).Edits.Count, "removing saves");
        session.RemoveEdit(e2);
        session.RemoveEdit(e1);
        report = session.Build();
        T.Eq(0, report.Count(EditStatus.Applied), "nothing to apply");
        T.True(!Directory.EnumerateFiles(Path.Combine(outDir, "resource"), "*", SearchOption.AllDirectories).Any(), "and the outputs are removed");

        T.True(EditorSession.GameRunning() == (System.Diagnostics.Process.GetProcessesByName("call_to_arms").Length > 0 || System.Diagnostics.Process.GetProcessesByName("call_to_arms_ed").Length > 0), "GameRunning looks at the game and the editor");
    }
}
