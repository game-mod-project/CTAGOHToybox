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
            string error = ValueRules.CheckNewValue(t.File, t.Text, newText, out warning);
            if (error != null) return error;
            // 창은 글자를 칠 때마다 검사한다. 원본은 세션 동안 바뀌지 않으므로 마지막에 읽은 파일 하나를 들고 있는다
            if (!string.Equals(checkedFile, t.File, StringComparison.Ordinal))
            {
                var source = Stack.Find(t.File);
                checkedText = source == null ? null : TextCodec.Decode(Stack.Read(source), out checkedKind);
                checkedFile = t.File;
            }
            if (checkedText == null) return null;
            if (!TextCodec.CanEncode(newText, checkedKind)) return "새 값에 이 파일의 인코딩(" + checkedKind + ")으로 쓸 수 없는 글자가 있다: " + newText;
            return ValueRules.CheckApplies(t.File, checkedText, new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, Old = t.Text, New = newText, Scope = scope });
        }

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
            var edit = new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, Old = t.Text, New = newText, Scope = scope };
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
                    Project.Edits.Add(new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, Old = t.Text, New = token, Scope = scope == "unit" ? "unit" : "shared" });
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
            var check = Check(edit, new Dictionary<string, SourceCheck>(StringComparer.Ordinal));
            if (check.Current == null) return;
            string before = edit.Old;
            edit.Old = check.Current;
            SaveOr(() => edit.Old = before);
        }

        public List<EditCheck> CheckEdits()
        {
            if (Project == null) return new List<EditCheck>();
            var parsed = new Dictionary<string, SourceCheck>(StringComparer.Ordinal);
            return Project.Edits.Where(e => e.Enabled).Select(e => Check(e, parsed)).ToList();
        }

        EditCheck Check(Edit e, Dictionary<string, SourceCheck> parsed)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            SourceCheck source;
            if (!parsed.TryGetValue(file, out source))
            {
                source = new SourceCheck();
                var found = Stack.Find(file);
                if (found != null)
                {
                    string error;
                    source.Rows = SourceRows.Of(file, TextCodec.Decode(Stack.Read(found), out source.Kind), out error);
                }
                parsed[file] = source;
            }
            var rows = source.Rows;
            if (rows == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 읽히지 않는다: " + file };
            var row = rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
            var node = row != null && e.Arg >= 0 && e.Arg < row.Leaves.Count ? row.Leaves[e.Arg] : null;
            if (node == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 자리가 없다" };
            string rowText = string.Join(" ", row.Leaves.Select(l => l.Text));
            bool same = LuaTable.IsLua(file) ? LuaPatcher.SameValue(node.Text, e.Old) : node.Text == e.Old;
            if (!same) return new EditCheck { Edit = e, Status = EditStatus.SourceChanged, Current = node.Text, RowText = rowText, Message = "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text };
            if (e.Scope == "unit")
            {
                string unit = SourceStack.ToVirtual(e.Unit ?? "");
                if (Stack.Find(unit) == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Current = node.Text, RowText = rowText, Message = "원본에 이 유닛이 없다: " + unit };
                string blocker = IncludeChain.UnitScopeBlocker(new SdlResolver(Stack.ReadText).Resolve(unit).IncludeEdges, unit, file, e.Path);
                if (blocker != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = blocker };
            }
            // 빌드(ModBuilder)와 같은 규칙: 파일의 인코딩으로 쓸 수 있는가, 새 값 규칙에 맞는가
            if (e.New != null && !TextCodec.CanEncode(e.New, source.Kind))
                return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = "새 값에 이 파일의 인코딩(" + source.Kind + ")으로 쓸 수 없는 글자가 있다: " + e.New };
            string warning;
            string rule = e.New == null ? null : ValueRules.CheckNewValue(file, e.Old, e.New, out warning);
            if (rule != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, RowText = rowText, Message = rule };
            return new EditCheck { Edit = e, Status = EditStatus.Applied, Current = node.Text, RowText = rowText };
        }

        // 수정 점검이 파일마다 한 번 읽은 것: 줄과 인코딩. 파일이 없거나 읽히지 않으면 Rows가 null이다
        sealed class SourceCheck { public List<KeyRow> Rows; public TextKind Kind; }

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
