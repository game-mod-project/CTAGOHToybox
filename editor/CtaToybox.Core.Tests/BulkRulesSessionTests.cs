using System.IO;
using System.Linq;
using CtaToybox.Core;

// F1: 규칙을 프로젝트에 넣고 고치고 끄고 지우고 다시 적용하고 합친다
static class BulkRulesSessionTests
{
    public static void Run()
    {
        string repo, outDir;
        string settings = BulkRulesTests.Fixture(out repo, out outDir);
        string projectPath = Path.Combine(repo, "editor", "projects", "bulk.json");
        using var s = EditorSession.Open(settings, repo, null);

        // 규칙이 없는 프로젝트는 rules를 쓰지 않는다
        s.OpenProject(projectPath);
        var l1 = s.OpenUnit(BulkRulesTests.L1);
        s.AddEdit(l1, l1.Rows.Single(r => r.Path == "/game_entity/name").Values[0], "\"n\"", "shared");
        T.True(!File.ReadAllText(projectPath).Contains("\"rules\"") && !File.ReadAllText(projectPath).Contains("\"rule\""), "a project without rules writes no rules field");

        var speed = s.AddRule(BulkRulesTests.Rule("**/locomotion/maxspeed", "mul", "1.1"));
        T.True(speed.Id != null && speed.Id.Length == 8, "the rule gets an 8-character id");
        string text = File.ReadAllText(projectPath);
        T.True(text.Contains("\"rules\"") && text.Contains("\"rule\": \"" + speed.Id + "\""), "the project keeps the rule and tags its edits");
        T.Eq(2, s.Project.Edits.Count(e => e.Rule == speed.Id), "two rule edits");
        T.True(s.CheckEdits().All(c => c.Status == EditStatus.Applied), "they check");
        T.Eq(3, s.Build().Count(EditStatus.Applied), "and build with the hand-made edit");

        // 사람이 규칙 수정을 고치면 떨어지고, 다시 적용해도 남는다
        l1 = s.OpenUnit(BulkRulesTests.L1);
        var l1Speed = l1.Rows.Single(r => r.Path.EndsWith("/locomotion/maxspeed")).Values[0];
        s.RefreshEdits(l1);
        var hand = s.AddEdit(l1, l1Speed, "70", "shared");
        T.True(hand.Rule == null && hand.New == "70", "a hand change detaches the edit from the rule");
        s.ReapplyRule(speed.Id);
        T.Eq("70", s.Project.Edits.Single(e => e.File == BulkRulesTests.L1 && e.Path.EndsWith("maxspeed")).New, "reapplying keeps the hand value");
        T.Eq(1, s.Project.Edits.Count(e => e.Rule == speed.Id), "the rule now owns only the other tank");

        // 두 규칙이 같은 자리: 앞 규칙이 가진다. 앞 규칙을 끄면 뒤 규칙이 가져간다
        var bigger = s.AddRule(BulkRulesTests.Rule("**/locomotion/maxspeed", "add", "5"));
        T.Eq(0, s.Project.Edits.Count(e => e.Rule == bigger.Id), "a later rule does not take the earlier rule's spot");
        T.Eq(1, s.PreviewRule(bigger).Skipped.Count(k => k.Reason.Contains("앞 규칙 '" + speed.Name + "'")), "the preview names the earlier rule by its name");
        s.SetRuleEnabled(speed.Id, false);
        T.Eq(0, s.Project.Edits.Count(e => e.Rule == speed.Id), "a disabled rule has no edits");
        T.Eq("50", s.Project.Edits.Single(e => e.Rule == bigger.Id).New, "the later rule takes the spot (45 + 5)");
        s.SetRuleEnabled(speed.Id, true);
        T.Eq("50", s.Project.Edits.Single(e => e.Rule == speed.Id).New, "turning it back on gives it back (45 × 1.1)");
        T.Eq(0, s.Project.Edits.Count(e => e.Rule == bigger.Id), "and the later rule steps back");

        // 고치기: id는 같고 값이 바뀐다
        var changed = speed.Copy();
        changed.Value = "2";
        s.EditRule(speed.Id, changed);
        T.Eq("90", s.Project.Edits.Single(e => e.Rule == speed.Id).New, "editing the factor recomputes (45 × 2)");
        T.Eq(speed.Id, s.Rules[0].Id, "the id stays");

        // 지우기
        s.RemoveRule(speed.Id);
        T.Eq(0, s.Project.Edits.Count(e => e.Rule == speed.Id), "removing a rule removes its edits");
        T.Eq("50", s.Project.Edits.Single(e => e.Rule == bigger.Id).New, "and the next rule takes the spot");
        T.Eq(new[] { bigger.Id }, s.Rules.Select(r => r.Id).ToArray(), "one rule left");
        T.Throws<System.ArgumentException>(() => s.AddRule(BulkRulesTests.Rule("**/x", "pow", "2")), "a bad rule is refused");

        // 합치기: 다른 프로젝트의 규칙이 붙고(id가 겹치면 새로) 수정은 다시 만든다
        string otherPath = Path.Combine(repo, "editor", "projects", "other.json");
        var other = new EditProject { Name = "other", Rules = new System.Collections.Generic.List<BulkRule> { new BulkRule { Id = bigger.Id, Name = "mass", Kind = "tank", Key = "**/mass", Op = "mul", Value = "3", Scope = "shared" } } };
        other.Edits.Add(new Edit { Unit = BulkRulesTests.H1, File = BulkRulesTests.TankExt, Path = "/mass", Arg = 0, Old = "10", New = "999", Scope = "shared", Rule = bigger.Id });
        other.Save(otherPath);
        s.MergeProject(otherPath);
        T.Eq(2, s.Rules.Count, "the merged rule is appended");
        var merged = s.Rules[1];
        T.True(merged.Id != bigger.Id, "a clashing id gets a new one");
        T.Eq("30", s.Project.Edits.Single(e => e.Rule == merged.Id).New, "its edits are recomputed, not copied (10 × 3)");
        T.True(!s.Project.Edits.Any(e => e.New == "999"), "the other project's rule edit is not copied");
    }
}
