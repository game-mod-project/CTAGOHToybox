// 팩션 강화: 보급(docs/superpowers/specs/2026-10-21-faction-boost-clip-supply-design.md).
// 보급 값(반경·총량)은 모든 군대가 같이 쓰는 properties/resupply.inc의 매크로 본문에 있다. 원래 매크로는 그대로 두고,
// 그 군대 보급 유닛이 부르는 매크로마다 이름 + _ctb_<군대> 복제본을 만들어 값을 바꾸고, 그 유닛의 호출 이름만 복제본으로 바꾼다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static partial class FactionPreset
    {
        public const string SupplyFile = "properties/resupply.inc";
        static readonly Regex SupplyDefine = new Regex("^/\\(define \"([^\"]+)\"\\)/extender\\[supply_zone\\]$", RegexOptions.CultureInvariant);

        public static void AddSupply(EditProject project, FactionContext c, FactionOptions o, FactionReport report)
        {
            string suffix = "_ctb_" + o.Army;
            var macros = SupplyMacros(c.Stack.ReadText(SupplyFile)).Where(m => !m.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)).ToList();
            if (macros.Count == 0) return;

            // 1. 그 군대 보급 유닛과 유닛 파일 안의 보급 매크로 호출
            var calls = new List<KeyValuePair<string, KeyRow>>();   // 유닛 → 호출 줄
            var supplyUsers = new HashSet<string>(c.Index.UsersOf(SupplyFile) ?? new List<string>(), StringComparer.Ordinal);
            foreach (string unit in c.Armies.Members(o.Army).Where(u => u.StartsWith("entity/-vehicle/", StringComparison.Ordinal)))
            {
                if (c.Armies.Shared.Any(x => x.StartsWith(unit + ":", StringComparison.Ordinal))) continue;
                string error;
                var rows = SourceRows.Of(unit, c.Stack.ReadText(unit), out error) ?? new List<KeyRow>();
                var mine = rows.Where(r => r.Head != null && macros.Contains(MacroSwap.NameOf(r.RawPath) ?? "", StringComparer.OrdinalIgnoreCase)).ToList();
                if (mine.Count == 0)
                {
                    if (supplyUsers.Contains(unit))
                    {
                        var traced = c.Tracer.Trace(unit);
                        if (traced != null && traced.Rows.Any(r => r.Path.Contains("/extender[supply_zone]")))
                            report.Skipped.Add(unit + ": 보급 매크로를 유닛 파일 밖(include)에서 부른다. 이름을 바꾸지 않았다");
                    }
                    continue;
                }
                var users = c.Index.UsersOf(unit) ?? new List<string>();
                if (users.Any(u => c.Armies.ArmyOf(u) != o.Army)) { report.Skipped.Add(unit + ": 다른 군대도 이 유닛 파일을 쓴다. 보급 매크로 이름을 바꾸지 않았다"); continue; }
                foreach (var row in mine) calls.Add(new KeyValuePair<string, KeyRow>(unit, row));
            }
            if (calls.Count == 0) return;

            // 2. 부르는 매크로마다 복제본
            int copies = 0;
            var used = calls.Select(kv => macros.First(m => string.Equals(m, MacroSwap.NameOf(kv.Value.RawPath), StringComparison.OrdinalIgnoreCase))).Distinct().OrderBy(m => m, StringComparer.Ordinal).ToList();
            foreach (string m in used)
            {
                string copy = m + suffix;
                if (project.Edits.Any(e => e.Kind == DefineCopy.Kind && SourceStack.ToVirtual(e.File ?? "") == SupplyFile && string.Equals(e.New, "\"" + copy + "\"", StringComparison.OrdinalIgnoreCase))) continue;
                project.Edits.Add(DefineCopy.New(SupplyFile, SupplyFile, m, copy));
                copies++;
            }

            // 3. 복제본 안의 값: 복제를 넣은 본문(덧씌우기)에서 줄을 찾는다
            c.Stack.Overlay = TextCopies.Overlay(project);
            string readError;
            var supplyRows = SourceRows.Of(SupplyFile, c.Stack.ReadText(SupplyFile), out readError) ?? new List<KeyRow>();
            int radii = 0, limits = 0;
            var changes = new Dictionary<string, SupplyChange>(StringComparer.OrdinalIgnoreCase);
            foreach (string m in used)
            {
                string copy = m + suffix, prefix = DefineCopy.PathOf(copy) + "/extender[supply_zone]/";
                var change = report.Supplies.FirstOrDefault(x => x.Macro == m) ?? new SupplyChange { Macro = m, Copy = copy };
                changes[m] = change;
                foreach (var row in supplyRows.Where(r => string.Equals(r.Path, prefix + "radius", StringComparison.OrdinalIgnoreCase) && r.Leaves.Count > 0))
                {
                    string error;
                    string old = row.Leaves[0].Text, next = Scale(old, o.SupplyRadius, false, out error);
                    if (next == null) { report.Skipped.Add(SupplyFile + ": " + copy + " 반경 " + error); continue; }
                    if (change.RadiusOld == null) { change.RadiusOld = old; change.RadiusNew = next; }
                    if (AddValue(project, SupplyFile, row, old, next)) radii++;
                }
                foreach (var row in supplyRows.Where(r => string.Equals(r.Path, prefix + "limit", StringComparison.OrdinalIgnoreCase) && r.Leaves.Count > 0))
                {
                    string old = row.Leaves[0].Text;
                    if (change.LimitOld == null) { change.LimitOld = old; change.LimitNew = o.SupplyLimit; }
                    if (AddValue(project, SupplyFile, row, old, o.SupplyLimit)) limits++;
                }
            }

            // 4. 유닛의 호출 이름을 복제본으로. 펼쳐서 복제본이 보이지 않으면(define not found로 게임이 꺼진다) 빼고 알린다
            var tracer = new UnitTracer(c.Stack);
            int swaps = 0;
            foreach (var kv in calls)
            {
                string unit = kv.Key;
                var row = kv.Value;
                string m = used.First(x => string.Equals(x, MacroSwap.NameOf(row.RawPath), StringComparison.OrdinalIgnoreCase));
                var edit = new Edit { Kind = MacroSwap.Kind, Unit = unit, File = unit, Path = row.Path.ToLowerInvariant(), Occurrence = row.Occurrence, Arg = 0, Old = row.Head.Text, New = "\"" + m + suffix + "\"", Scope = "shared" };
                var change = changes[m];
                if (!change.Units.Contains(unit)) change.Units.Add(unit);
                if (project.Edits.Any(e => e.Kind == MacroSwap.Kind && e.File == unit && string.Equals(e.Path, edit.Path, StringComparison.OrdinalIgnoreCase) && e.Occurrence == edit.Occurrence)) continue;
                string bad = MacroSwap.Check(tracer.Trace(unit), edit);
                if (bad != null) { report.Skipped.Add(unit + ": " + bad); change.Units.Remove(unit); continue; }
                project.Edits.Add(edit);
                swaps++;
            }
            foreach (var change in changes.Values) if (!report.Supplies.Contains(change)) report.Supplies.Add(change);

            report.Edits[o.Army + " 보급 매크로 복제"] = copies;
            report.Edits[o.Army + " 보급 반경 ×" + Num(o.SupplyRadius)] = radii;
            report.Edits[o.Army + " 보급 총량 → " + o.SupplyLimit] = limits;
            report.Edits[o.Army + " 보급 호출 → 복제본"] = swaps;
        }

        // 보급 범위(extender "supply_zone")를 가진 매크로 이름들(경로의 소문자 이름)
        static List<string> SupplyMacros(string text)
        {
            if (text == null) return new List<string>();
            string error;
            var rows = SourceRows.Of(SupplyFile, text, out error) ?? new List<KeyRow>();
            return rows.Select(r => SupplyDefine.Match(r.Path)).Where(x => x.Success).Select(x => x.Groups[1].Value).Distinct().ToList();
        }

        // 같은 자리 수정이 없고 새 값이 다르면 '전부' 값 수정을 더한다
        static bool AddValue(EditProject project, string file, KeyRow row, string old, string next)
        {
            if (old == next) return false;
            if (project.Edits.Any(e => e.Kind == null && e.File == file && string.Equals(e.Path, row.Path, StringComparison.OrdinalIgnoreCase) && e.Occurrence == row.Occurrence && e.Arg == 0)) return false;
            project.Edits.Add(new Edit { Unit = file, File = file, Path = row.Path, Occurrence = row.Occurrence, Arg = 0, Old = old, New = next, Scope = "shared" });
            return true;
        }
    }
}
