// 편집 세션의 수정 목록 동작: 더하기, 바꾸기, 끄기, 지우기, 지금 원본으로 검사하기, 빌드. 바뀌면 프로젝트 파일을 바로 쓴다.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class EditCheck
    {
        public Edit Edit { get; set; }
        public EditStatus Status { get; set; }   // Applied는 "지금 원본에 적용할 수 있다"는 뜻이다
        public string Current { get; set; }      // 원본의 지금 값. 자리가 없으면 null
        public string Message { get; set; }
        public string RowText { get; set; }      // 그 줄의 원본 값 전체(예: "mine m1a1" 24). 화면이 어느 줄인지 보이는 데 쓴다
    }

    public sealed partial class EditorSession
    {
        public string ValidateNew(UnitView unit, ValueView value, string newText, string scope, out string warning)
        {
            warning = null;
            if (!value.Editable) return value.Trace.Note ?? "편집할 수 없는 자리다";
            if (scope != "shared" && scope != "unit") return "범위는 shared 또는 unit이다: " + scope;
            if (scope == "unit" && value.UnitScopeBlocker != null) return value.UnitScopeBlocker;
            var t = value.Trace;
            string macroWarning = null;
            if (t.MacroName)
            {
                // 매크로 이름은 같은 파일에 정의된 이름 가운데서만 고른다. 보이지 않는 이름이면 게임이 시작할 때 꺼진다(E4)
                string name = (newText ?? "").Trim().Trim('"');
                if (value.Choices == null || !value.Choices.Any(c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase)))
                    return "바꿀 수 있는 매크로 이름이 아니다: " + name + ". 고를 수 있는 이름: " + string.Join(", ", (value.Choices ?? new List<string>()).Take(20));
                if ((newText ?? "").Trim() != "\"" + name + "\"") return "매크로 이름은 따옴표로 감싸 쓴다: \"" + name + "\"";
                if (!MacroSwap.SameFamily(t.Text.Trim('"'), name)) macroWarning = "다른 갈래의 매크로다(" + t.Text.Trim('"') + " → " + name + "). 같은 파일의 정의라 게임은 읽지만 뜻이 다를 수 있다";
            }
            string error = ValueRules.CheckNewValue(t.File, t.Text, newText, out warning);
            if (error != null) return error;
            warning = macroWarning ?? warning;
            // 창은 글자를 칠 때마다 검사한다. 원본은 세션 동안 바뀌지 않으므로 마지막에 읽은 파일 하나를 들고 있는다
            if (!string.Equals(checkedFile, t.File, StringComparison.Ordinal))
            {
                var source = Stack.Find(t.File);
                checkedText = source == null ? null : TextCodec.Decode(Stack.Read(source), out checkedKind);
                checkedFile = t.File;
            }
            if (checkedText == null) return null;
            if (!TextCodec.CanEncode(newText, checkedKind)) return "새 값에 이 파일의 인코딩(" + checkedKind + ")으로 쓸 수 없는 글자가 있다: " + newText;
            return ValueRules.CheckApplies(t.File, checkedText, new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, NamedPath = t.NamedPath, NamedOccurrence = t.NamedOccurrence, Old = t.Text, New = newText, Scope = scope, Kind = KindOf(t) });
        }

        static string KindOf(TraceValue t) { return t.MacroName ? MacroSwap.Kind : null; }

        string checkedFile, checkedText;
        TextKind checkedKind;

        public Edit AddEdit(UnitView unit, ValueView value, string newText, string scope)
        {
            NeedProject();
            string warning;
            string error = ValidateNew(unit, value, newText, scope, out warning);
            if (error != null) throw new ArgumentException(error);
            var t = value.Trace;
            var existing = Project.Edits.FirstOrDefault(e => e.Scope == scope && SameSlot(e, t) && (scope != "unit" || SourceStack.ToVirtual(e.Unit ?? "") == unit.Unit));
            if (existing != null)
            {
                string oldNew = existing.New;
                bool oldEnabled = existing.Enabled;
                existing.New = newText;
                existing.Enabled = true;
                SaveOr(() => { existing.New = oldNew; existing.Enabled = oldEnabled; });
                return existing;
            }
            var edit = new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, NamedPath = t.NamedPath, NamedOccurrence = t.NamedOccurrence, Old = t.Text, New = newText, Scope = scope, Kind = KindOf(t) };
            Project.Edits.Add(edit);
            SaveOr(() => Project.Edits.Remove(edit));
            return edit;
        }

        // 화면의 한 줄(값 여러 개)을 한 번에 고친다. newTexts는 줄의 값 순서대로 새 토큰이다(고칠 수 없는 값의 자리와 null은 건너뛴다).
        // 고른 범위의 수정만 만들고 바꾸고 지운다: '이 유닛만'을 고르면 다른 유닛 화면에서 만든 '전부' 수정은 건드리지 않는다.
        // '전부'를 고르면 이 유닛의 '이 유닛만' 수정은 지운다(남아 있으면 이 유닛에서는 '전부' 값이 보이지 않는다).
        // 값을 원래 값으로 돌리면 그 범위의 수정을 지운다. 모두 검사한 뒤 바꾸고 한 번 저장한다. 저장이 실패하면 모두 되돌린다
        public void ApplyRow(UnitView unit, RowView row, IList<string> newTexts, string scope)
        {
            NeedProject();
            if (newTexts.Count != row.Values.Count) throw new ArgumentException("값의 수가 줄과 다르다");
            var plan = new List<KeyValuePair<ValueView, string>>();
            for (int i = 0; i < row.Values.Count; i++)
            {
                var v = row.Values[i];
                if (!v.Editable || newTexts[i] == null) continue;
                if (newTexts[i] != v.Trace.Text)
                {
                    string warning;
                    string error = ValidateNew(unit, v, newTexts[i], scope, out warning);
                    if (error != null) throw new ArgumentException(error);
                }
                plan.Add(new KeyValuePair<ValueView, string>(v, newTexts[i]));
            }

            var objects = Project.Edits.ToList();
            var states = objects.Select(e => new { e, e.New, e.Enabled, e.Old }).ToList();
            try
            {
                foreach (var p in plan)
                {
                    var t = p.Key.Trace;
                    string token = p.Value;
                    var mine = Project.Edits.FirstOrDefault(e => e.Scope == "unit" && SourceStack.ToVirtual(e.Unit ?? "") == unit.Unit && SameSlot(e, t));
                    var shared = Project.Edits.FirstOrDefault(e => e.Scope != "unit" && SameSlot(e, t));
                    var target = scope == "unit" ? mine : shared;
                    if (scope != "unit" && mine != null) Project.Edits.Remove(mine);
                    if (token == t.Text) { if (target != null) Project.Edits.Remove(target); continue; }
                    if (target != null) { target.New = token; target.Enabled = true; continue; }
                    Project.Edits.Add(new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, NamedPath = t.NamedPath, NamedOccurrence = t.NamedOccurrence, Old = t.Text, New = token, Scope = scope == "unit" ? "unit" : "shared", Kind = KindOf(t) });
                }
                Project.Save(ProjectPath);
            }
            catch
            {
                Project.Edits.Clear();
                Project.Edits.AddRange(objects);
                foreach (var s in states) { s.e.New = s.New; s.e.Enabled = s.Enabled; s.e.Old = s.Old; }
                throw;
            }
        }

        public void RemoveEdit(Edit edit)
        {
            NeedProject();
            int at = Project.Edits.IndexOf(edit);
            if (at < 0) return;
            Project.Edits.RemoveAt(at);
            SaveOr(() => Project.Edits.Insert(at, edit));
        }

        public void SetEnabled(Edit edit, bool enabled)
        {
            NeedProject();
            bool before = edit.Enabled;
            edit.Enabled = enabled;
            SaveOr(() => edit.Enabled = before);
        }

        // old를 원본의 지금 값으로 바꾼다. 자리가 없어졌으면 그대로 둔다
        public void Rebase(Edit edit)
        {
            NeedProject();
            var check = Check(edit, new Dictionary<string, SourceCheck>(StringComparer.Ordinal), output => null, tracer.Trace);
            if (check.Current == null) return;
            string before = edit.Old;
            edit.Old = check.Current;
            SaveOr(() => edit.Old = before);
        }

        // 다른 프로젝트 파일의 수정을 지금 프로젝트에 더하고 저장한다(E17). 같은 자리에 다른 값이면 지금 것을 둔다(MergeReport.Conflicts).
        // 깨진 파일은 InvalidDataException, 지금 프로젝트 파일 자신이면 ArgumentException
        public MergeReport MergeProject(string path)
        {
            NeedProject();
            if (string.Equals(System.IO.Path.GetFullPath(path), System.IO.Path.GetFullPath(ProjectPath), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("지금 연 프로젝트와 같은 파일이다. 다른 프로젝트를 고른다.");
            var other = EditProject.Load(path);
            Reanchor(other.Edits);
            int before = Project.Edits.Count;
            var report = Project.MergeFrom(other);
            if (report.Added > 0) SaveOr(() => Project.Edits.RemoveRange(before, Project.Edits.Count - before));
            return report;
        }

        public List<EditCheck> CheckEdits()
        {
            if (Project == null) return new List<EditCheck>();
            var parsed = new Dictionary<string, SourceCheck>(StringComparer.Ordinal);
            var conflict = new ModBuilder(Stack, Settings.OutputModDir).OutputConflicts();
            var traces = new Dictionary<string, UnitTraceResult>(StringComparer.Ordinal);
            Func<string, UnitTraceResult> trace = u => { UnitTraceResult t; if (!traces.TryGetValue(u, out t)) { t = tracer.Trace(u); traces[u] = t; } return t; };
            return Project.Edits.Where(e => e.Enabled).Select(e => Check(e, parsed, conflict, trace)).ToList();
        }

        EditCheck Check(Edit e, Dictionary<string, SourceCheck> parsed, Func<string, string> conflict, Func<string, UnitTraceResult> trace)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            var source = SourceOf(file, parsed);
            // 블록 복제(E18): 빌드와 같이 원본에 넣어 본 결과가 그 수정의 점검 결과다
            if (e.Kind == BlockCopy.Kind)
            {
                var copied = source.CopyResults.FirstOrDefault(r => r.Edit == e);
                if (copied == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 읽히지 않는다: " + file };
                return new EditCheck { Edit = e, Status = copied.Status, Current = e.Old, RowText = e.Old, Message = copied.Message };
            }
            // 복제 블록 안의 값, 값을 복제 이름으로 바꾸는 수정: 그 복제가 적용되지 않으면 막는다
            string dependency = BlockCopy.Dependency(e, Project == null ? new List<Edit>() : Project.Edits);
            if (dependency != null && !ActiveCopies(parsed).Contains(dependency))
                return new EditCheck { Edit = e, Status = EditStatus.Invalid, Message = BlockCopy.MissingNote(dependency) };
            var rows = source.Rows;
            if (rows == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 읽히지 않는다: " + file };
            var row = rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
            if (e.NamedPath != null && !TokenPatcher.SameName(row, e))
                return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 이름의 항목이 없다: " + e.NamedPath + (e.NamedOccurrence > 0 ? " [" + e.NamedOccurrence + "]" : "") };
            var node = e.Kind == MacroSwap.Kind ? (row == null ? null : row.Head) : row != null && e.Arg >= 0 && e.Arg < row.Leaves.Count ? row.Leaves[e.Arg] : null;
            if (node == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 자리가 없다" };
            string rowText = e.Kind == MacroSwap.Kind ? node.Text : string.Join(" ", row.Leaves.Select(l => l.Text));
            bool same = LuaTable.IsLua(file) ? LuaPatcher.SameValue(node.Text, e.Old) : node.Text == e.Old;
            if (!same) return new EditCheck { Edit = e, Status = EditStatus.SourceChanged, Current = node.Text, RowText = rowText, Message = "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text };
            if (e.Scope == "unit")
            {
                string unit = SourceStack.ToVirtual(e.Unit ?? "");
                if (Stack.Find(unit) == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Current = node.Text, RowText = rowText, Message = "원본에 이 유닛이 없다: " + unit };
                // 빌드와 같은 규칙(UnitScope): include 사슬, 원형에서 물려받은 값은 덧붙일 수 있는 줄인지(E14)
                string blocker = UnitScope.Blocker(trace(unit), file, e.Path, e.Occurrence, e.Arg);
                if (blocker != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = blocker };
            }
            // 빌드(ModBuilder)와 같은 규칙: 파일의 인코딩으로 쓸 수 있는가, 새 값 규칙에 맞는가
            if (e.New != null && !TextCodec.CanEncode(e.New, source.Kind))
                return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = "새 값에 이 파일의 인코딩(" + source.Kind + ")으로 쓸 수 없는 글자가 있다: " + e.New };
            string warning;
            string rule = e.New == null ? null : ValueRules.CheckNewValue(file, e.Old, e.New, out warning);
            if (rule != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = rule };
            // 매크로 이름은 그 유닛을 펼칠 때 같은 파일에 정의된 이름이어야 한다(E4)
            if (e.Kind == MacroSwap.Kind)
            {
                string swap = MacroSwap.Check(trace(SourceStack.ToVirtual(e.Unit ?? file)), e);
                if (swap != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = swap };
            }
            // 빌드와 같이 미리 적용해 본다: 키 경로를 바꾸는 예전 수정(Q2 전에 만든 블록 한정자 등)은 여기서 걸린다(Q5)
            string applies = e.New == null ? null : ValueRules.CheckApplies(file, source.Text, e);
            if (applies != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = applies };
            // 빌드가 쓸 출력 파일이 손으로 넣은 파일과 겹치는가(Q5). '이 유닛만'이면 유닛 파일과 그 사본도 쓴다
            foreach (string output in OutputsOf(e, file, trace))
            {
                string clash = conflict(output);
                if (clash != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = clash };
            }
            return new EditCheck { Edit = e, Status = EditStatus.Applied, Current = node.Text, RowText = rowText };
        }

        // 수정 점검이 파일마다 한 번 읽은 것: 줄과 인코딩. 파일이 없거나 읽히지 않으면 Rows가 null이다
        sealed class SourceCheck { public List<KeyRow> Rows; public TextKind Kind; public string Text; public List<EditResult> CopyResults = new List<EditResult>(); }

        // 파일의 원본을 읽어 켜진 블록 복제를 넣은 본문과 줄(빌드와 같은 본문). 파일이 없거나 읽히지 않으면 Rows가 null이다
        SourceCheck SourceOf(string file, Dictionary<string, SourceCheck> parsed)
        {
            SourceCheck source;
            if (parsed.TryGetValue(file, out source)) return source;
            source = new SourceCheck();
            var found = Stack.Find(file);
            if (found != null)
            {
                string error;
                source.Text = BlockCopy.Apply(TextCodec.Decode(Stack.Read(found), out source.Kind), CopiesOf(file), source.CopyResults);
                source.Rows = SourceRows.Of(file, source.Text, out error);
            }
            parsed[file] = source;
            return source;
        }

        // 이 파일의 켜진 블록 복제(E18)
        List<Edit> CopiesOf(string file)
        {
            if (Project == null) return new List<Edit>();
            return Project.Edits.Where(x => x.Kind == BlockCopy.Kind && x.Enabled && string.Equals(SourceStack.ToVirtual(x.File ?? ""), file, StringComparison.Ordinal)).ToList();
        }

        // 원본에 적용되는 블록 복제의 새 이름(따옴표 없이)
        HashSet<string> ActiveCopies(Dictionary<string, SourceCheck> parsed)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (Project == null) return names;
            foreach (string file in Project.Edits.Where(x => x.Kind == BlockCopy.Kind && x.Enabled).Select(x => SourceStack.ToVirtual(x.File ?? "")).Distinct())
                foreach (var r in SourceOf(file, parsed).CopyResults.Where(r => r.Status == EditStatus.Applied)) names.Add(r.Edit.New.Trim('"'));
            return names;
        }

        // 이 수정 때문에 빌드가 쓸 출력 가상 경로. '전부'는 그 파일, '이 유닛만'은 유닛 파일과(다른 파일의 값이면) 그 파일의 사본
        // 원형에서 물려받은 값을 '이 유닛만' 고치면 유닛 파일에 덧붙이므로 사본은 없다(E14)
        static IEnumerable<string> OutputsOf(Edit e, string file, Func<string, UnitTraceResult> trace)
        {
            if (e.Scope != "unit") { yield return file; yield break; }
            string unit = SourceStack.ToVirtual(e.Unit ?? "");
            yield return unit;
            if (file != unit && UnitScope.InheritedRow(trace(unit), file, e.Path, e.Occurrence, e.Arg) == null) yield return IncludeChain.CopyName(file, unit);
        }

        public BuildReport Build()
        {
            NeedProject();
            return new ModBuilder(Stack, Settings.OutputModDir).Build(Project);
        }

        public static bool GameRunning()
        {
            return Process.GetProcessesByName("call_to_arms").Length > 0 || Process.GetProcessesByName("call_to_arms_ed").Length > 0;
        }

        // 처음 연 프로젝트 파일이 깨졌으면 프로젝트가 없다
        void NeedProject() { if (Project == null) throw new InvalidOperationException("열린 프로젝트가 없다. 프로젝트를 연다."); }

        // 저장하지 못하면 메모리의 변경을 되돌리고 예외를 그대로 낸다: 화면과 파일이 어긋나지 않게 한다
        void SaveOr(Action rollback)
        {
            try { Project.Save(ProjectPath); }
            catch { rollback(); throw; }
        }
    }
}
