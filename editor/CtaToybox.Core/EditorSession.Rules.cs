// 일괄 규칙(F1)을 프로젝트에 넣고 고치고 끄고 지우고 다시 적용한다.
// 규칙 목록 순서가 겹치는 자리의 우선순위라, 한 규칙이 바뀌면 그 규칙과 목록 뒤 규칙들을 차례로 다시 계산한다.
// 규칙이 만든 수정(Edit.Rule)만 지우고 다시 만든다. 사람이 만든 수정과 규칙에서 떨어진 수정은 건드리지 않는다
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed partial class EditorSession
    {
        public IReadOnlyList<BulkRule> Rules { get { return Project == null || Project.Rules == null ? new List<BulkRule>() : Project.Rules; } }

        public BulkRule RuleOf(string id) { return Rules.FirstOrDefault(r => r.Id == id); }

        // 지금 프로젝트에 넣으면 무엇이 바뀌는지. 프로젝트는 바꾸지 않는다. 이미 있는 규칙이면 그 규칙보다 앞의 수정과 사람 수정만 겹침으로 본다
        public RulePreview PreviewRule(BulkRule rule)
        {
            var probe = new EditProject { Rules = Project == null ? null : Project.Rules, Edits = Project == null ? new List<Edit>() : Project.Edits.Where(e => e.Rule == null || Before(e.Rule, rule.Id)).ToList() };
            return BulkRules.Preview(rule, probe, Index.Units, tracer, f => Index.UsersOf(f), u => Index.HeirsOf(u).Count);
        }

        // id가 rule보다 목록 앞에 있는 규칙인가. rule이 목록에 없으면(새 규칙) 모든 규칙이 앞이다
        bool Before(string id, string ruleId)
        {
            var list = Rules.Select(r => r.Id).ToList();
            int a = list.IndexOf(id), b = list.IndexOf(ruleId);
            return a >= 0 && (b < 0 || a < b);
        }

        public static string NewRuleId(IEnumerable<BulkRule> existing)
        {
            var taken = new HashSet<string>(existing.Select(r => r.Id), StringComparer.OrdinalIgnoreCase);
            string id;
            do { id = Guid.NewGuid().ToString("N").Substring(0, 8); } while (taken.Contains(id));
            return id;
        }

        public BulkRule AddRule(BulkRule rule)
        {
            NeedProject();
            string bad = BulkRules.Check(rule);
            if (bad != null) throw new ArgumentException(bad);
            var added = rule.Copy();
            added.Id = NewRuleId(Rules);
            if (string.IsNullOrWhiteSpace(added.Name)) added.Name = added.Describe();
            Change(() => { if (Project.Rules == null) Project.Rules = new List<BulkRule>(); Project.Rules.Add(added); }, added.Id);
            return added;
        }

        public void EditRule(string id, BulkRule changed)
        {
            NeedProject();
            var rule = NeedRule(id);
            string bad = BulkRules.Check(changed);
            if (bad != null) throw new ArgumentException(bad);
            Change(() =>
            {
                rule.Name = string.IsNullOrWhiteSpace(changed.Name) ? changed.Describe() : changed.Name;
                rule.Kind = changed.Kind; rule.Paths = changed.Paths == null ? null : new List<string>(changed.Paths); rule.Layer = changed.Layer;
                rule.Key = changed.Key; rule.Arg = changed.Arg; rule.Op = changed.Op; rule.Value = changed.Value; rule.Scope = changed.Scope;
            }, id);
        }

        public void SetRuleEnabled(string id, bool enabled)
        {
            NeedProject();
            var rule = NeedRule(id);
            Change(() => rule.Enabled = enabled, id);
        }

        public void RemoveRule(string id)
        {
            NeedProject();
            var rule = NeedRule(id);
            int at = Project.Rules.IndexOf(rule);
            string next = at + 1 < Project.Rules.Count ? Project.Rules[at + 1].Id : null;
            Change(() => { Project.Edits.RemoveAll(e => e.Rule == id); Project.Rules.Remove(rule); if (Project.Rules.Count == 0) Project.Rules = null; }, next);
        }

        public void ReapplyRule(string id)
        {
            NeedProject();
            NeedRule(id);
            Change(() => { }, id);
        }

        BulkRule NeedRule(string id)
        {
            var rule = RuleOf(id);
            if (rule == null) throw new ArgumentException("이 id의 규칙이 없다: " + id);
            return rule;
        }

        // change를 한 뒤 fromId 규칙부터 목록 끝까지 차례로 다시 계산하고 저장한다. 실패하면 모두 되돌린다
        void Change(Action change, string fromId)
        {
            var edits = Project.Edits.ToList();
            var rules = Project.Rules == null ? null : Project.Rules.Select(r => r.Copy()).ToList();
            var ruleObjects = Project.Rules == null ? null : Project.Rules.ToList();
            try
            {
                change();
                Recompute(fromId);
                Project.Save(ProjectPath);
            }
            catch
            {
                Project.Edits.Clear();
                Project.Edits.AddRange(edits);
                if (ruleObjects == null) Project.Rules = null;
                else
                {
                    for (int i = 0; i < ruleObjects.Count; i++) Restore(ruleObjects[i], rules[i]);
                    Project.Rules = ruleObjects;
                }
                throw;
            }
        }

        static void Restore(BulkRule target, BulkRule from)
        {
            target.Name = from.Name; target.Kind = from.Kind; target.Paths = from.Paths; target.Layer = from.Layer; target.Key = from.Key;
            target.Arg = from.Arg; target.Op = from.Op; target.Value = from.Value; target.Scope = from.Scope; target.Enabled = from.Enabled;
        }

        // fromId 규칙과 그 뒤 규칙들의 수정을 지우고 차례로 다시 만든다. fromId가 null이면 아무것도 하지 않는다
        void Recompute(string fromId)
        {
            if (fromId == null || Project.Rules == null) return;
            int from = Project.Rules.FindIndex(r => r.Id == fromId);
            if (from < 0) return;
            var later = Project.Rules.Skip(from).ToList();
            var ids = new HashSet<string>(later.Select(r => r.Id), StringComparer.Ordinal);
            Project.Edits.RemoveAll(e => e.Rule != null && ids.Contains(e.Rule));
            foreach (var rule in later.Where(r => r.Enabled))
            {
                var preview = BulkRules.Preview(rule, Project, Index.Units, tracer, f => Index.UsersOf(f), u => Index.HeirsOf(u).Count);
                Project.Edits.AddRange(preview.Edits);
            }
        }

        // 다른 프로젝트의 규칙을 목록 끝에 붙이고(id가 겹치면 새로) 다시 계산한다(MergeProject가 부른다)
        void AppendRules(IEnumerable<BulkRule> rules)
        {
            var list = rules.ToList();
            if (list.Count == 0) return;
            if (Project.Rules == null) Project.Rules = new List<BulkRule>();
            string first = null;
            foreach (var r in list)
            {
                var copy = r.Copy();
                if (Project.Rules.Any(x => string.Equals(x.Id, copy.Id, StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(copy.Id)) copy.Id = NewRuleId(Project.Rules);
                Project.Rules.Add(copy);
                if (first == null) first = copy.Id;
            }
            Recompute(first);
        }
    }
}
