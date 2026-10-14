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
    }

    public sealed partial class EditorSession
    {
        public string ValidateNew(UnitView unit, ValueView value, string newText, string scope, out string warning)
        {
            warning = null;
            if (!value.Editable) return value.Trace.Note ?? "편집할 수 없는 자리다";
            if (scope != "shared" && scope != "unit") return "범위는 shared 또는 unit이다: " + scope;
            if (scope == "unit" && value.UnitScopeBlocker != null) return value.UnitScopeBlocker;
            return TokenPatcher.CheckNewValue(value.Trace.Text, newText, out warning);
        }

        public Edit AddEdit(UnitView unit, ValueView value, string newText, string scope)
        {
            string warning;
            string error = ValidateNew(unit, value, newText, scope, out warning);
            if (error != null) throw new ArgumentException(error);
            var t = value.Trace;
            var existing = Project.Edits.FirstOrDefault(e => e.Scope == scope && SameSlot(e, t) && (scope != "unit" || SourceStack.ToVirtual(e.Unit ?? "") == unit.Unit));
            if (existing != null)
            {
                existing.New = newText;
                existing.Enabled = true;
                Save();
                return existing;
            }
            var edit = new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, Old = t.Text, New = newText, Scope = scope };
            Project.Edits.Add(edit);
            Save();
            return edit;
        }

        public void RemoveEdit(Edit edit) { Project.Edits.Remove(edit); Save(); }

        public void SetEnabled(Edit edit, bool enabled) { edit.Enabled = enabled; Save(); }

        // old를 원본의 지금 값으로 바꾼다. 자리가 없어졌으면 그대로 둔다
        public void Rebase(Edit edit)
        {
            var check = Check(edit, new Dictionary<string, SdlNode>(StringComparer.Ordinal));
            if (check.Current == null) return;
            edit.Old = check.Current;
            Save();
        }

        public List<EditCheck> CheckEdits()
        {
            var parsed = new Dictionary<string, SdlNode>(StringComparer.Ordinal);
            return Project.Edits.Where(e => e.Enabled).Select(e => Check(e, parsed)).ToList();
        }

        EditCheck Check(Edit e, Dictionary<string, SdlNode> parsed)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            SdlNode root;
            if (!parsed.TryGetValue(file, out root))
            {
                string text = Stack.ReadText(file);
                var p = text == null ? null : SdlCore.Parse(text);
                root = p == null || p.Errors.Count > 0 ? null : p.Root;
                parsed[file] = root;
            }
            if (root == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 파싱되지 않는다: " + file };
            var node = KeyLocator.Find(root, e.Path, e.Occurrence, e.Arg);
            if (node == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 자리가 없다" };
            if (node.Text != e.Old) return new EditCheck { Edit = e, Status = EditStatus.SourceChanged, Current = node.Text, Message = "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text };
            if (e.Scope == "unit")
            {
                string unit = SourceStack.ToVirtual(e.Unit ?? "");
                if (Stack.Find(unit) == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Current = node.Text, Message = "원본에 이 유닛이 없다: " + unit };
                string blocker = IncludeChain.UnitScopeBlocker(new SdlResolver(Stack.ReadText).Resolve(unit).IncludeEdges, unit, file, e.Path);
                if (blocker != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, Message = blocker };
            }
            return new EditCheck { Edit = e, Status = EditStatus.Applied, Current = node.Text };
        }

        public BuildReport Build()
        {
            return new ModBuilder(Stack, Settings.OutputModDir).Build(Project);
        }

        public static bool GameRunning()
        {
            return Process.GetProcessesByName("call_to_arms").Length > 0 || Process.GetProcessesByName("call_to_arms_ed").Length > 0;
        }

        void Save() { Project.Save(ProjectPath); }
    }
}
