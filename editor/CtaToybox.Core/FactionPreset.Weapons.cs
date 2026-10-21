// 팩션 강화: 재장전과 보병 체력(docs/superpowers/specs/2026-10-17-faction-boost-design.md).
// 재장전: 무기는 여러 군대가 이름으로 같이 부르므로 무기 파일을 군대 이름을 붙인 사본으로 만들고(FileCopy), 사본의 rechargeTime을 고친 뒤,
// 이 군대 유닛의 이름 값만 사본 이름으로 바꾼다. 사본 자신의 줄이면 값을 고치고, 원형·매크로에서 오는 값이면 사본에 {rechargeTime N} 줄을 더한다(AddLine).
// 더한 줄이 원형 값을 이긴다는 것은 편집기의 원형 규칙(자식 쪽 키가 이긴다, E11)을 따른 것이고, 게임 안 확인은 하지 않았다(미확인).
// 체력: 펼친 perks의 "health"(마지막 것) 값의 배수를 그 보병 파일의 perks 블록 끝에 {"health" "N%"}로 더한다. 이기는지는 미확인이다
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static partial class FactionPreset
    {
        public static void AddWeaponsAndHealth(EditProject project, FactionContext c, FactionOptions o, FactionReport report)
        {
            var members = c.Armies.Members(o.Army).ToList();
            var traces = new Dictionary<string, UnitTraceResult>(StringComparer.Ordinal);
            UnitTraceResult TraceOf(string p) { UnitTraceResult t; if (!traces.TryGetValue(p, out t)) { t = c.Tracer.Trace(p); traces[p] = t; } return t; }

            // 1. 유닛이 부르는 무기 이름과 그 이름 값의 자리
            var refs = new List<KeyValuePair<string, KeyValuePair<UnitTraceResult, TraceValue>>>();   // 무기 파일 → (유닛 펼침, 이름 값)
            var names = new Dictionary<TraceValue, string>();
            foreach (string unit in members)
            {
                var traced = TraceOf(unit);
                if (traced == null) continue;
                foreach (var row in traced.Rows)
                {
                    string seg = row.Path.Substring(row.Path.LastIndexOf('/') + 1);
                    for (int i = 0; i < row.Values.Count; i++)
                    {
                        var v = row.Values[i];
                        if (v.Text == null || !v.Text.StartsWith("\"")) continue;
                        bool isName = (i == 0 && (seg == "item" || seg == "weapon")) || (i > 0 && row.Values[i - 1].Text == "filling");
                        if (!isName) continue;
                        string name = v.Text.Trim('"');
                        var link = c.Items.Find(name);
                        if (link.Files.Count == 0 || (link.Rule == ItemLinks.RuleCandidates)) continue;
                        string file = link.Files[0];
                        if (file.EndsWith(".ammo", StringComparison.Ordinal) || file.EndsWith(".grenade", StringComparison.Ordinal) || file.EndsWith(".mine", StringComparison.Ordinal)) continue;
                        refs.Add(new KeyValuePair<string, KeyValuePair<UnitTraceResult, TraceValue>>(file, new KeyValuePair<UnitTraceResult, TraceValue>(traced, v)));
                        names[v] = name;
                    }
                }
            }

            // 2. 장전 시간이 있는 무기마다 사본 하나
            var copies = new Dictionary<string, string>(StringComparer.Ordinal);   // 원본 → 사본
            int reloadEdits = 0, lines = 0;
            foreach (string file in refs.Select(r => r.Key).Distinct().OrderBy(f => f, StringComparer.Ordinal))
            {
                var weapon = c.Tracer.Trace(file);
                if (weapon == null || !weapon.Rows.Any(r => r.Path.EndsWith("/rechargetime", StringComparison.Ordinal))) continue;
                string copy = FileCopy.NameFor(file, o.Army);
                string bad = project.Edits.Any(e => e.Kind == FileCopy.Kind && SourceStack.ToVirtual(e.New) == copy) ? null : FileCopy.CheckTarget(c.Stack, file, copy);
                if (bad != null) { report.Skipped.Add(file + ": 사본을 만들지 못했다: " + bad); continue; }
                if (!project.Edits.Any(e => e.Kind == FileCopy.Kind && SourceStack.ToVirtual(e.New) == copy)) project.Edits.Add(FileCopy.New(file, file, copy));
                copies[file] = copy;
            }
            c.Stack.SetCopies(FileCopy.Pairs(project));
            var tracer = new UnitTracer(c.Stack);

            // 3. 사본의 장전 시간: 사본 자신의 줄이면 값, 아니면 줄 더하기. 클립식 포는 소수까지 남기고 발 사이 간격도 줄인다
            double gapFactor = o.ClipGap > 0 ? o.ClipGap : o.Reload;
            int gapEdits = 0;
            foreach (var kv in copies)
            {
                string copy = kv.Value;
                var traced = tracer.Trace(copy);
                string text = c.Stack.ReadText(copy);
                var blocks = AddLine.Blocks(text);
                int filling;
                string gapText;
                bool clip = IsClip(copy, traced, o.ClipMinGap, out filling, out gapText);
                var change = clip ? new ClipChange { Weapon = copy, Filling = filling, GapOld = gapText } : null;
                foreach (var row in traced.Rows.Where(r => r.Path.EndsWith("/rechargetime", StringComparison.Ordinal) && r.FromState != FromLevel.Overridden))
                {
                    var v = row.Values.Count > 0 ? row.Values[0] : null;
                    if (v == null) continue;
                    string error;
                    string next = Scale(v.Text, o.Reload, clip, out error);
                    if (next == null) { report.Skipped.Add(copy + ": 장전 시간 " + error); continue; }
                    if (change != null) { change.ReloadOld = v.Text; change.ReloadNew = next; }
                    if (v.Arg >= 0 && SourceStack.ToVirtual(v.File ?? "") == copy)
                    {
                        if (!project.Edits.Any(e => e.File == copy && e.Path == v.KeyPath && e.Occurrence == v.Occurrence && e.Arg == v.Arg))
                        { project.Edits.Add(new Edit { Unit = copy, File = copy, Path = v.KeyPath, Occurrence = v.Occurrence, Arg = v.Arg, Old = v.Text, New = next, Scope = "shared", NamedPath = v.NamedPath, NamedOccurrence = v.NamedOccurrence }); reloadEdits++; }
                        continue;
                    }
                    // 원형이나 매크로에서 오는 값: 사본의 같은 깊이 블록에 줄을 더한다. 맨 위 블록(/from)이면 사본의 첫 블록
                    string parent = row.Path.Substring(0, row.Path.LastIndexOf('/'));
                    var block = parent.Count(ch => ch == '/') <= 1 ? blocks.FirstOrDefault() : blocks.FirstOrDefault(b => string.Equals(b.Path, parent, StringComparison.OrdinalIgnoreCase));
                    if (block == null) { report.Skipped.Add(copy + ": " + row.Path + "를 넣을 블록이 사본에 없다"); continue; }
                    string line = "{rechargeTime " + next + "}";
                    if (!project.Edits.Any(e => e.Kind == AddLine.Kind && e.File == copy && e.Path == block.Path && e.Occurrence == block.Occurrence && e.New == line))
                    { project.Edits.Add(AddLine.New(copy, copy, block.Path, block.Occurrence, line)); lines++; }
                }
                if (change == null) continue;
                gapEdits += AddGap(project, copy, traced, text, blocks, gapFactor, change, report);
                if (!report.Clips.Any(x => x.Weapon == copy)) report.Clips.Add(change);
            }
            report.Edits[o.Army + " 클립식 발 사이 ×" + Num(gapFactor)] = gapEdits;

            // 4. 이 군대 유닛의 이름 값을 사본 이름으로
            var links = ItemLinks.Build(c.Stack.Paths);
            int renames = 0;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var r in refs)
            {
                string copy;
                if (!copies.TryGetValue(r.Key, out copy)) continue;
                var v = r.Value.Value;
                string name = names[v];
                string firstOld = r.Key.Substring(r.Key.LastIndexOf('/') + 1).Split('.')[0], firstNew = copy.Substring(copy.LastIndexOf('/') + 1).Split('.')[0];
                var words = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                int at = words.FindIndex(w => string.Equals(w, firstOld, StringComparison.OrdinalIgnoreCase));
                if (at < 0) { report.Skipped.Add(r.Value.Key.Unit + ": 이름 " + name + "에 파일 이름 낱말이 없어 사본 이름으로 바꾸지 못했다"); continue; }
                words[at] = firstNew;
                string newName = string.Join(" ", words);
                var found = links.Find(newName);
                if (found.Files.Count != 1 || found.Files[0] != copy) { report.Skipped.Add(r.Value.Key.Unit + ": " + newName + "이 사본 하나로 풀리지 않는다"); continue; }
                if (v.Arg < 0) { report.Skipped.Add(r.Value.Key.Unit + ": 이름 " + name + "을 고칠 수 없다: " + v.Note); continue; }
                var edit = Scoped(c, o.Army, r.Value.Key, v, "\"" + newName + "\"", report);
                if (edit == null) continue;
                string key = edit.Scope + "|" + (edit.Scope == "unit" ? edit.Unit : "") + "|" + edit.File + "|" + edit.Path + "|" + edit.Occurrence + "|" + edit.Arg;
                if (!seen.Add(key) || project.Edits.Any(e => e.Kind == null && e.Scope == edit.Scope && e.File == edit.File && e.Path == edit.Path && e.Occurrence == edit.Occurrence && e.Arg == edit.Arg && (e.Scope != "unit" || e.Unit == edit.Unit))) continue;
                project.Edits.Add(edit);
                renames++;
            }

            // 5. 보병 체력
            int health = 0;
            foreach (string unit in members.Where(m => m.StartsWith("set/breed/", StringComparison.Ordinal)))
            {
                var traced = TraceOf(unit);
                if (traced == null) continue;
                var row = traced.Rows.LastOrDefault(r => r.Path.EndsWith("/perks/\"*\"", StringComparison.Ordinal) && r.Values.Count > 1 && r.Values[0].Text == "\"health\"");
                if (row == null) { report.Skipped.Add(unit + ": perks에 health가 없다"); continue; }
                string old = row.Values[1].Text.Trim('"');
                bool percent = old.EndsWith("%");
                double n;
                if (!double.TryParse(percent ? old.TrimEnd('%') : old, NumberStyles.Float, CultureInfo.InvariantCulture, out n)) { report.Skipped.Add(unit + ": health 값이 수가 아니다: " + old); continue; }
                var block = AddLine.Find(c.Stack.ReadText(unit), "/breed/perks", 0);
                if (block == null) { report.Skipped.Add(unit + ": 파일에 /breed/perks 블록이 없다"); continue; }
                string line = "{\"health\" \"" + Num(n * o.Health) + (percent ? "%" : "") + "\"}";
                if (project.Edits.Any(e => e.Kind == AddLine.Kind && e.File == unit && e.New == line)) continue;
                project.Edits.Add(AddLine.New(unit, unit, "/breed/perks", 0, line));
                health++;
            }

            report.Edits[o.Army + " 무기 사본"] = copies.Count;
            report.Edits[o.Army + " 재장전 값 ×" + Num(o.Reload)] = reloadEdits;
            report.Edits[o.Army + " 재장전 줄 더하기"] = lines;
            report.Edits[o.Army + " 무기 이름 → 사본"] = renames;
            report.Edits[o.Army + " 보병 체력 ×" + Num(o.Health)] = health;
        }

        static readonly Regex FillingPath = new Regex("^/[^/]+/filling$", RegexOptions.CultureInvariant);
        static readonly Regex GapPath = new Regex("^/[^/]+/recoverytime$", RegexOptions.CultureInvariant);
        static readonly Regex FireRatePath = new Regex("^/[^/]+/\\(\"fire rate\"\\)$", RegexOptions.CultureInvariant);

        // 곱한 값. keepDecimals면 소수 셋째 자리까지(클립식 포의 비율을 지킨다), 아니면 일괄 규칙과 같이 정수였던 값은 정수로
        static string Scale(string oldText, double factor, bool keepDecimals, out string error)
        {
            if (!keepDecimals) return BulkRules.Compute("mul", oldText, Num(factor), out error);
            error = null;
            double v;
            if (!double.TryParse(oldText, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) { error = "수가 아니다: " + oldText; return null; }
            return Math.Round(v * factor, 3).ToString("0.###", CultureInfo.InvariantCulture);
        }

        // 클립식 포인가: set/stuff/gun 사본이고, 펼친 탄창 발수가 2 이상이며, 펼친 발 사이(마지막 줄)가 minGap초 이상
        static bool IsClip(string copy, UnitTraceResult traced, double minGap, out int filling, out string gapText)
        {
            filling = 1; gapText = null;
            if (traced == null || !copy.StartsWith("set/stuff/gun/", StringComparison.Ordinal)) return false;
            foreach (var r in traced.Rows.Where(r => FillingPath.IsMatch(r.Path) && r.FromState != FromLevel.Overridden && r.Values.Count > 1))
            {
                double n;
                if (double.TryParse(r.Values[1].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out n) && n > filling) filling = (int)n;
            }
            var last = traced.Rows.LastOrDefault(r => GapPath.IsMatch(r.Path) && r.FromState != FromLevel.Overridden && r.Values.Count > 0);
            double gap;
            if (last == null || !double.TryParse(last.Values[0].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out gap)) return false;
            gapText = last.Values[0].Text;
            return filling >= 2 && gap >= minGap;
        }

        // 클립식 포의 발 사이 간격 × factor. 사본 자신의 줄이면 줄마다 값, "fire rate" 호출이 사본에 있으면 rpm ÷ factor, 아니면 사본에 줄을 더한다
        static int AddGap(EditProject project, string copy, UnitTraceResult traced, string text, IReadOnlyList<BlockSpan> blocks, double factor, ClipChange change, FactionReport report)
        {
            int made = 0;
            string error;
            var rows = traced.Rows.Where(r => GapPath.IsMatch(r.Path) && r.FromState != FromLevel.Overridden && r.Values.Count > 0).ToList();
            foreach (var v in rows.Select(r => r.Values[0]).Where(v => !v.Synthetic && v.Arg >= 0 && SourceStack.ToVirtual(v.File ?? "") == copy))
            {
                string next = Scale(v.Text, factor, true, out error);
                if (next == null) { report.Skipped.Add(copy + ": 발 사이 " + error); continue; }
                change.GapNew = next;
                if (project.Edits.Any(e => e.File == copy && e.Path == v.KeyPath && e.Occurrence == v.Occurrence && e.Arg == v.Arg)) continue;
                project.Edits.Add(new Edit { Unit = copy, File = copy, Path = v.KeyPath, Occurrence = v.Occurrence, Arg = v.Arg, Old = v.Text, New = next, Scope = "shared", NamedPath = v.NamedPath, NamedOccurrence = v.NamedOccurrence });
                made++;
            }
            var last = rows.Last().Values[0];
            if (!last.Synthetic && last.Arg >= 0 && SourceStack.ToVirtual(last.File ?? "") == copy) return made;
            string gapNew = Scale(last.Text, factor, true, out error);
            if (gapNew == null) { report.Skipped.Add(copy + ": 발 사이 " + error); return made; }
            change.GapNew = gapNew;
            if (last.Synthetic)
            {
                string readError;
                var call = (SourceRows.Of(copy, text, out readError) ?? new List<KeyRow>()).LastOrDefault(r => FireRatePath.IsMatch(r.Path));
                int at = call == null || call.LeafNames == null ? -1 : call.LeafNames.IndexOf("rpm");
                if (at >= 0 && at < call.Leaves.Count)
                {
                    string rpm = Scale(call.Leaves[at].Text, 1 / factor, true, out error);
                    if (rpm == null) { report.Skipped.Add(copy + ": rpm " + error); return made; }
                    if (!project.Edits.Any(e => e.File == copy && string.Equals(e.Path, call.Path, StringComparison.OrdinalIgnoreCase) && e.Occurrence == call.Occurrence && e.Arg == at))
                    { project.Edits.Add(new Edit { Unit = copy, File = copy, Path = call.Path, Occurrence = call.Occurrence, Arg = at, Old = call.Leaves[at].Text, New = rpm, Scope = "shared" }); made++; }
                    return made;
                }
            }
            // 원형이나 다른 파일에서 오는 값: 사본의 첫 블록에 줄을 더한다
            var block = blocks.FirstOrDefault();
            if (block == null) { report.Skipped.Add(copy + ": 발 사이 줄을 넣을 블록이 사본에 없다"); return made; }
            string line = "{recoveryTime " + gapNew + "}";
            if (!project.Edits.Any(e => e.Kind == AddLine.Kind && e.File == copy && e.Path == block.Path && e.Occurrence == block.Occurrence && e.New == line))
            { project.Edits.Add(AddLine.New(copy, copy, block.Path, block.Occurrence, line)); made++; }
            return made;
        }

        // F1 'auto'와 같은 범위: 그 파일을 쓰는 유닛이 모두 이 군대면 '전부', 아니면 '이 유닛만'(막히면 건너뛴다)
        static Edit Scoped(FactionContext c, string army, UnitTraceResult traced, TraceValue v, string newText, FactionReport report)
        {
            var users = c.Index.UsersOf(v.File) ?? new List<string>();
            bool all = users.Count == 0 || users.All(u => c.Armies.ArmyOf(u) == army);
            var e = new Edit { Unit = traced.Unit, File = SourceStack.ToVirtual(v.File), Path = v.KeyPath, Occurrence = v.Occurrence, Arg = v.Arg, Old = v.Text, New = newText, NamedPath = v.NamedPath, NamedOccurrence = v.NamedOccurrence };
            if (all) { e.Scope = "shared"; return e; }
            int heirs = c.Index.HeirsOf(traced.Unit).Count;
            string blocker = UnitScope.Blocker(traced, v.File, v.KeyPath, v.Occurrence, v.Arg) ?? (heirs > 0 ? FromLevel.HeirsNote(heirs) : null);
            if (blocker != null) { report.Skipped.Add(traced.Unit + ": " + blocker); return null; }
            e.Scope = "unit";
            return e;
        }
    }
}
