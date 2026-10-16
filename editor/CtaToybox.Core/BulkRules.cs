// 일괄 규칙(F1, docs/superpowers/specs/2026-10-17-f1-bulk-rules-design.md).
// 대상(편집기 종류·경로 글자·계층) × 펼친 키 × 연산(set·mul·add)으로 대상 유닛마다 키가 맞는 값의 원본 자리를 찾아 수정을 만든다.
// 범위 auto: 그 자리 파일을 쓰는 유닛이 모두 대상이면 '전부' 하나, 아니면 대상마다 '이 유닛만'(UnitScope가 막으면 건너뜀).
// 겹침: 같은 자리(파일·키 경로·몇 번째·몇 번째 값)에 사람의 '전부' 수정이나 앞 규칙의 '전부' 수정이 있으면 그 자리를 통째 건너뛰고,
// '이 유닛만' 수정이 유닛 U에 있으면 U만 건너뛴다
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class BulkRule
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }            // 편집기 종류(tank 등) 또는 원래 종류(vehicle)
        public List<string> Paths { get; set; }     // 유닛 경로에 모두 들어야 하는 글자. null이나 빈 목록이면 그 종류 전부
        public string Layer { get; set; }           // 계층 이름(vanilla, mod_…). null이면 전부
        public string Key { get; set; }             // 펼친 키(정확한 경로 또는 **/꼬리)
        public int Arg { get; set; }                // 줄의 몇 번째 값. -1이면 수인 값 모두
        public string Op { get; set; }              // set | mul | add
        public string Value { get; set; }
        public string Scope { get; set; } = "auto"; // auto | shared | unit
        public bool Enabled { get; set; } = true;

        public BulkRule Copy()
        {
            return new BulkRule { Id = Id, Name = Name, Kind = Kind, Paths = Paths == null ? null : new List<string>(Paths), Layer = Layer, Key = Key, Arg = Arg, Op = Op, Value = Value, Scope = Scope, Enabled = Enabled };
        }

        // 화면·명령줄에 보이는 한 줄 설명
        public string Describe()
        {
            string what = Op == "mul" ? "×" + Value : Op == "add" ? (Value.StartsWith("-") ? Value : "+" + Value) : "= " + Value;
            return Kind + (Paths == null || Paths.Count == 0 ? "" : " [" + string.Join(", ", Paths) + "]") + (Layer == null ? "" : " (" + Layer + ")") + " · " + Key + (Arg < 0 ? " 모든 값" : Arg > 0 ? " 값 " + Arg : "") + " " + what + (Scope == "auto" ? "" : " · " + Scope);
        }
    }

    public sealed class RuleChange { public string Unit; public string File; public string Path; public string Old; public string New; public string Scope; }
    public sealed class RuleSkip { public string Unit; public string Reason; }

    public sealed class RulePreview
    {
        public List<Edit> Edits { get; } = new List<Edit>();
        public List<RuleChange> Changes { get; } = new List<RuleChange>();   // 유닛마다 한 줄(같은 '전부' 수정을 쓰는 대상 유닛 각각)
        public List<RuleSkip> Skipped { get; } = new List<RuleSkip>();
        public int Units { get; set; }          // 값이 바뀌는 대상 유닛 수
        public int OutsideUnits { get; set; }   // '전부' 수정 때문에 함께 바뀌는 대상 밖 유닛 수
        public int Targets { get; set; }        // 대상 유닛 수
    }

    public static class BulkRules
    {
        public static readonly string[] Ops = { "set", "mul", "add" };
        public static readonly string[] Scopes = { "auto", "shared", "unit" };

        // 규칙 칸이 맞는지. 맞으면 null
        public static string Check(BulkRule rule)
        {
            if (string.IsNullOrWhiteSpace(rule.Kind)) return "대상 종류가 없다";
            if (string.IsNullOrWhiteSpace(rule.Key)) return "키가 없다";
            if (!Ops.Contains(rule.Op)) return "연산은 set, mul, add 가운데 하나다: " + rule.Op;
            if (!Scopes.Contains(rule.Scope ?? "auto")) return "범위는 auto, shared, unit 가운데 하나다: " + rule.Scope;
            if (string.IsNullOrWhiteSpace(rule.Value)) return "값이 없다";
            double v;
            if (rule.Op != "set" && !double.TryParse(rule.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return "배수·더할 값은 수다: " + rule.Value;
            if (rule.Arg < -1) return "값 자리는 0 이상이거나 -1(모든 값)이다";
            return null;
        }

        // 새 값 글자. 안 되면 null과 이유. 원래 값이 정수면 반올림해 정수로, 소수면 소수 셋째 자리까지
        public static string Compute(string op, string oldText, string value, out string error)
        {
            error = null;
            if (op == "set") return value;
            if (op != "mul" && op != "add") { error = "모르는 연산이다: " + op; return null; }
            double old, v;
            if (oldText == null || !SdlCore.IsNumber(oldText.Trim()) || !double.TryParse(oldText.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out old)) { error = "원래 값이 수가 아니다: " + oldText; return null; }
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) { error = "배수·더할 값이 수가 아니다: " + value; return null; }
            double result = op == "mul" ? old * v : old + v;
            bool integer = oldText.Trim().IndexOf('.') < 0;
            return integer ? Math.Round(result, MidpointRounding.AwayFromZero).ToString("0", CultureInfo.InvariantCulture) : result.ToString("0.###", CultureInfo.InvariantCulture);
        }

        static string SlotKey(string file, string path, int occurrence, int arg)
        {
            return SourceStack.ToVirtual(file ?? "") + "|" + (path ?? "").ToLowerInvariant() + "|" + occurrence + "|" + arg;
        }

        public static bool IsTarget(BulkRule rule, UnitEntry u)
        {
            if (!string.Equals(u.EditorKind, rule.Kind, StringComparison.OrdinalIgnoreCase) && !string.Equals(u.Kind, rule.Kind, StringComparison.OrdinalIgnoreCase)) return false;
            if (rule.Layer != null && !string.Equals(u.Layer, rule.Layer, StringComparison.OrdinalIgnoreCase)) return false;
            return rule.Paths == null || rule.Paths.All(p => string.IsNullOrEmpty(p) || u.Path.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        static bool KeyMatches(string key, TraceRow row)
        {
            foreach (string c in KeyPattern.Candidates(row.Path, row.RawPath, false))
                if (string.Equals(c, key, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        sealed class Slot { public TraceValue Value; public List<string> Units = new List<string>(); public Dictionary<string, UnitTraceResult> Traces = new Dictionary<string, UnitTraceResult>(); }

        // project의 다른 수정(사람과 앞 규칙)과의 겹침을 본다. 만든 수정의 Rule은 rule.Id다
        public static RulePreview Preview(BulkRule rule, EditProject project, IEnumerable<UnitEntry> units, UnitTracer tracer, Func<string, IReadOnlyList<string>> usersOf, Func<string, int> heirsOf)
        {
            var preview = new RulePreview();
            string bad = Check(rule);
            if (bad != null) throw new ArgumentException(bad);
            var others = (project == null ? new List<Edit>() : project.Edits).Where(e => e.Enabled && e.Rule != rule.Id).ToList();
            var sharedTaken = new Dictionary<string, Edit>(StringComparer.Ordinal);
            var unitTaken = new Dictionary<string, Edit>(StringComparer.Ordinal);
            foreach (var e in others)
            {
                string k = SlotKey(e.File, e.Path, e.Occurrence, e.Arg);
                if (e.Scope == "unit") unitTaken[k + "|" + SourceStack.ToVirtual(e.Unit ?? "")] = e; else sharedTaken[k] = e;
            }

            var targets = units.Where(u => IsTarget(rule, u)).OrderBy(u => u.Path, StringComparer.Ordinal).ToList();
            preview.Targets = targets.Count;
            var targetSet = new HashSet<string>(targets.Select(u => u.Path), StringComparer.Ordinal);
            var slots = new Dictionary<string, Slot>(StringComparer.Ordinal);
            foreach (var u in targets)
            {
                var traced = tracer.Trace(u.Path);
                if (traced == null) continue;
                bool any = false;
                foreach (var row in traced.Rows.Where(r => KeyMatches(rule.Key, r)))
                {
                    for (int i = 0; i < row.Values.Count; i++)
                    {
                        if (rule.Arg >= 0 && i != rule.Arg) continue;
                        var v = row.Values[i];
                        if (rule.Arg < 0 && !SdlCore.IsNumber((v.Text ?? "").Trim())) continue;
                        any = true;
                        if (v.Arg < 0) { preview.Skipped.Add(new RuleSkip { Unit = u.Path, Reason = v.Note ?? "고칠 수 없는 값이다" }); continue; }
                        string k = SlotKey(v.File, v.KeyPath, v.Occurrence, v.Arg);
                        Slot slot;
                        if (!slots.TryGetValue(k, out slot)) { slot = new Slot { Value = v }; slots[k] = slot; }
                        if (!slot.Units.Contains(u.Path)) { slot.Units.Add(u.Path); slot.Traces[u.Path] = traced; }
                    }
                }
                if (!any) preview.Skipped.Add(new RuleSkip { Unit = u.Path, Reason = "이 키가 없다" });
            }

            var changed = new HashSet<string>(StringComparer.Ordinal);
            var outside = new HashSet<string>(StringComparer.Ordinal);
            foreach (var kv in slots)
            {
                var v = kv.Value.Value;
                string error;
                string next = Compute(rule.Op, v.Text, rule.Value, out error);
                if (next == null) { foreach (string u in kv.Value.Units) preview.Skipped.Add(new RuleSkip { Unit = u, Reason = error }); continue; }
                if (next == v.Text) continue;
                string warning;
                string rule2 = ValueRules.CheckNewValue(v.File, v.Text, next, out warning);
                if (rule2 != null) { foreach (string u in kv.Value.Units) preview.Skipped.Add(new RuleSkip { Unit = u, Reason = rule2 }); continue; }
                Edit taken;
                if (sharedTaken.TryGetValue(kv.Key, out taken))
                {
                    foreach (string u in kv.Value.Units) preview.Skipped.Add(new RuleSkip { Unit = u, Reason = Owner(taken) + "('전부')가 이 자리에 있다" });
                    continue;
                }
                var free = new List<string>();
                foreach (string u in kv.Value.Units)
                {
                    if (unitTaken.TryGetValue(kv.Key + "|" + u, out taken)) preview.Skipped.Add(new RuleSkip { Unit = u, Reason = Owner(taken) + "('이 유닛만')가 이 유닛에 있다" });
                    else free.Add(u);
                }
                if (free.Count == 0) continue;
                var users = usersOf(v.File) ?? new List<string>();
                string scope = rule.Scope == "shared" ? "shared" : rule.Scope == "unit" ? "unit" : users.All(targetSet.Contains) ? "shared" : "unit";
                if (scope == "shared")
                {
                    preview.Edits.Add(NewEdit(rule, v, free[0], next, "shared"));
                    foreach (string u in free) { changed.Add(u); preview.Changes.Add(new RuleChange { Unit = u, File = v.File, Path = v.KeyPath, Old = v.Text, New = next, Scope = "shared" }); }
                    foreach (string u in users) if (!targetSet.Contains(u)) outside.Add(u);
                    continue;
                }
                foreach (string u in free)
                {
                    int heirs = heirsOf(u);
                    string blocker = UnitScope.Blocker(kv.Value.Traces[u], v.File, v.KeyPath, v.Occurrence, v.Arg) ?? (heirs > 0 ? FromLevel.HeirsNote(heirs) : null);
                    if (blocker != null) { preview.Skipped.Add(new RuleSkip { Unit = u, Reason = blocker }); continue; }
                    preview.Edits.Add(NewEdit(rule, v, u, next, "unit"));
                    changed.Add(u);
                    preview.Changes.Add(new RuleChange { Unit = u, File = v.File, Path = v.KeyPath, Old = v.Text, New = next, Scope = "unit" });
                }
            }
            preview.Units = changed.Count;
            preview.OutsideUnits = outside.Count;
            return preview;
        }

        static string Owner(Edit e) { return e.Rule == null ? "직접 고친 수정" : "앞 규칙 " + e.Rule + "의 수정"; }

        static Edit NewEdit(BulkRule rule, TraceValue v, string unit, string next, string scope)
        {
            return new Edit { Unit = unit, File = v.File, Path = v.KeyPath, Occurrence = v.Occurrence, Arg = v.Arg, Old = v.Text, New = next, Scope = scope,
                NamedPath = v.NamedPath, NamedOccurrence = v.NamedOccurrence, Rule = rule.Id };
        }
    }
}
