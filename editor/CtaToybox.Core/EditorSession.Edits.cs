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
                // 빌드가 볼 본문: 원본에 켜진 블록 복제를 넣은 것(E18)
                checkedText = source == null ? null : TextCopies.ApplyAll(TextCodec.Decode(Stack.Read(source), out checkedKind), CopiesOf(t.File));
                checkedFile = t.File;
            }
            if (checkedText == null) return null;
            if (!TextCodec.CanEncode(newText, checkedKind)) return "새 값에 이 파일의 인코딩(" + checkedKind + ")으로 쓸 수 없는 글자가 있다: " + newText;
            return ValueRules.CheckApplies(t.File, checkedText, new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, NamedPath = t.NamedPath, NamedOccurrence = t.NamedOccurrence, Old = t.Text, New = newText, Scope = scope, Kind = KindOf(t) });
        }

        static string KindOf(TraceValue t) { return t.MacroName ? MacroSwap.Kind : null; }

        string checkedFile, checkedText;
        TextKind checkedKind;

        // 값이 든 파일에서 그 값을 감싼 블록(줄 더하기 창). 못 찾으면 null
        public BlockSpan BlockOfValue(ValueView value)
        {
            var t = value.Trace;
            if (t == null || t.File == null || t.Arg < 0) return null;
            var found = Stack.Find(t.File);
            if (found == null || LuaTable.IsLua(found.VirtualPath)) return null;
            TextKind kind;
            string text = TextCopies.ApplyAll(TextCodec.Decode(Stack.Read(found), out kind), CopiesOf(found.VirtualPath));
            string error;
            var rows = SourceRows.Of(found.VirtualPath, text, out error);
            var row = rows == null ? null : rows.FirstOrDefault(r => r.Occurrence == t.Occurrence && string.Equals(r.Path, t.KeyPath, StringComparison.OrdinalIgnoreCase));
            if (row == null || t.Arg >= row.Leaves.Count) return null;
            // 값이 든 줄 자신의 { }(예 {item "x" 5})가 아니라 그 줄을 감싼 블록(inventory)에 넣는다
            string own = row.Path.Substring(row.Path.LastIndexOf('/') + 1);
            var around = AddLine.Blocks(text).Where(b => b.Open < row.Leaves[t.Arg].Index && row.Leaves[t.Arg].Index < b.Close).OrderByDescending(b => b.Open).ToList();
            if (around.Count > 0 && around[0].Path.Substring(around[0].Path.LastIndexOf('/') + 1) == own) around.RemoveAt(0);
            return around.FirstOrDefault();
        }

        // 파일에서 그 경로의 첫 블록(고칠 값이 없는 줄, 예: 매크로로 채운 perks). 없으면 null
        public BlockSpan BlockInFile(string file, string blockPath)
        {
            var found = Stack.Find(file ?? "");
            if (found == null || LuaTable.IsLua(found.VirtualPath)) return null;
            TextKind kind;
            return AddLine.Find(TextCopies.ApplyAll(TextCodec.Decode(Stack.Read(found), out kind), CopiesOf(found.VirtualPath)), blockPath, 0);
        }

        // 블록 끝에 줄 하나를 더한다(팩션 강화). 같은 파일·블록·줄이 이미 있으면 그 수정을 돌려준다. 안 되면 ArgumentException
        public Edit AddLineEdit(string unit, string file, string blockPath, int occurrence, string line)
        {
            NeedProject();
            string bad = AddLine.CheckLine(line);
            if (bad != null) throw new ArgumentException(bad);
            var found = Stack.Find(file);
            if (found == null) throw new ArgumentException("원본에 이 파일이 없다: " + file);
            if (LuaTable.IsLua(found.VirtualPath)) throw new ArgumentException("Lua 파일에는 줄을 더하지 않는다");
            TextKind kind;
            string text = TextCopies.ApplyAll(TextCodec.Decode(Stack.Read(found), out kind), CopiesOf(found.VirtualPath));
            if (AddLine.Find(text, blockPath, occurrence) == null) throw new ArgumentException("원본에 이 블록이 없다: " + blockPath);
            if (!TextCodec.CanEncode(line, kind)) throw new ArgumentException("넣을 줄에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다");
            var same = Project.Edits.FirstOrDefault(x => x.Kind == AddLine.Kind && SourceStack.ToVirtual(x.File ?? "") == found.VirtualPath && string.Equals(x.Path, blockPath, StringComparison.OrdinalIgnoreCase) && x.Occurrence == occurrence && x.New == line);
            if (same != null) return same;
            var edit = AddLine.New(unit, found.VirtualPath, blockPath, occurrence, line);
            Project.Edits.Add(edit);
            SaveOr(() => Project.Edits.Remove(edit));
            return edit;
        }

        // 파일 사본을 더한다(팩션 강화). target이 null이면 FileCopy.NameFor(source, suffix). 안 되면 ArgumentException
        public Edit AddFileCopy(string unit, string source, string target)
        {
            NeedProject();
            string bad = FileCopy.CheckTarget(Stack, source, target);
            if (bad != null) throw new ArgumentException(bad);
            if (Project.Edits.Any(x => x.Kind == FileCopy.Kind && SourceStack.ToVirtual(x.New ?? "") == SourceStack.ToVirtual(target)))
                throw new ArgumentException("같은 사본이 이미 있다: " + target);
            var edit = FileCopy.New(unit, source, target);
            Project.Edits.Add(edit);
            SaveOr(() => Project.Edits.Remove(edit));
            RefreshOverlay();
            return edit;
        }

        // 파일 맨 위의 매크로 정의를 새 이름으로 복제한다(파일 끝에 붙는다). newName은 따옴표로 감싼 이름. 안 되면 ArgumentException
        public Edit AddDefineCopy(string unit, string file, string name, string newName)
        {
            NeedProject();
            string bad = DefineCopy.CheckName(newName);
            if (bad != null) throw new ArgumentException(bad);
            var found = Stack.Find(file);
            if (found == null) throw new ArgumentException("원본에 이 파일이 없다: " + file);
            if (LuaTable.IsLua(found.VirtualPath)) throw new ArgumentException("Lua 파일에는 매크로 정의가 없다");
            var edit = DefineCopy.New(unit ?? found.VirtualPath, found.VirtualPath, name, newName.Trim('"'));
            TextKind kind;
            string original = TextCodec.Decode(Stack.Read(found), out kind);
            var results = new List<EditResult>();
            TextCopies.Apply(original, CopiesOf(found.VirtualPath).Concat(new[] { edit }), results);
            var mine = results.Last();
            if (mine.Status != EditStatus.Applied) throw new ArgumentException(mine.Message ?? "복제할 수 없다");
            Project.Edits.Add(edit);
            SaveOr(() => Project.Edits.Remove(edit));
            RefreshOverlay();
            return edit;
        }

        // 게임 설정 파일의 따옴표 이름 블록을 새 이름으로 복제한다(E18). nameValue는 블록 이름 줄의 값(UnitTracer.BlockNameNote). 안 되면 ArgumentException
        public Edit AddBlockCopy(UnitView unit, ValueView nameValue, string newName)
        {
            NeedProject();
            var t = nameValue.Trace;
            if (t.Note != UnitTracer.BlockNameNote || t.File == null || !SettingFiles.InArea(t.File)) throw new ArgumentException("게임 설정 파일의 따옴표 이름 블록만 복제할 수 있다");
            string bad = BlockCopy.CheckName(newName);
            if (bad != null) throw new ArgumentException(bad);
            var source = Stack.Find(t.File);
            if (source == null) throw new ArgumentException("원본에 이 파일이 없다: " + t.File);
            TextKind kind;
            string original = TextCodec.Decode(Stack.Read(source), out kind);
            var parsed = SdlCore.Parse(TextCopies.ApplyAll(original, CopiesOf(t.File)));
            var row = new KeyIndex(parsed.Root).Row(t.KeyPath, t.Occurrence);
            if (row == null) throw new ArgumentException("원본에 이 블록이 없다");
            var edit = new Edit { Kind = BlockCopy.Kind, Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = 0, NamedPath = row.RawPath, NamedOccurrence = row.RawOccurrence, Old = t.Text, New = newName, Scope = "shared" };
            var results = new List<EditResult>();
            BlockCopy.Apply(original, CopiesOf(t.File).Concat(new[] { edit }), results);
            var mine = results.Last();
            if (mine.Status != EditStatus.Applied) throw new ArgumentException(mine.Message ?? "복제할 수 없다");
            Project.Edits.Add(edit);
            SaveOr(() => Project.Edits.Remove(edit));
            RefreshOverlay();
            return edit;
        }

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
                string oldNew = existing.New, oldRule = existing.Rule;
                bool oldEnabled = existing.Enabled;
                existing.New = newText;
                existing.Enabled = true;
                existing.Rule = null;   // 사람이 고친 규칙 수정은 규칙에서 떨어진다(F1)
                SaveOr(() => { existing.New = oldNew; existing.Enabled = oldEnabled; existing.Rule = oldRule; });
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
            var states = objects.Select(e => new { e, e.New, e.Enabled, e.Old, e.Rule }).ToList();
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
                    if (target != null) { target.New = token; target.Enabled = true; target.Rule = null; continue; }
                    Project.Edits.Add(new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, NamedPath = t.NamedPath, NamedOccurrence = t.NamedOccurrence, Old = t.Text, New = token, Scope = scope == "unit" ? "unit" : "shared", Kind = KindOf(t) });
                }
                Project.Save(ProjectPath);
            }
            catch
            {
                Project.Edits.Clear();
                Project.Edits.AddRange(objects);
                foreach (var s in states) { s.e.New = s.New; s.e.Enabled = s.Enabled; s.e.Old = s.Old; s.e.Rule = s.Rule; }
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
            RefreshOverlay();
        }

        public void SetEnabled(Edit edit, bool enabled)
        {
            NeedProject();
            bool before = edit.Enabled;
            string rule = edit.Rule;
            edit.Enabled = enabled;
            edit.Rule = null;   // 사람이 켜고 끈 규칙 수정은 규칙에서 떨어진다(F1)
            SaveOr(() => { edit.Enabled = before; edit.Rule = rule; });
            RefreshOverlay();
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
            var report = Project.MergeFrom(other);
            int before = Project.Edits.Count - report.Added;
            bool rules = other.Rules != null && other.Rules.Count > 0;
            // 가져오는 규칙은 목록 끝에 붙이고 다시 계산한다(F1). 겹치는 자리는 지금 프로젝트가 이긴다
            if (rules) Change(() => AppendRules(other.Rules), null);
            else if (report.Added > 0) SaveOr(() => Project.Edits.RemoveRange(before, Project.Edits.Count - before));
            RefreshOverlay();
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
            // 파일 사본(팩션 강화): 원본이 있고 사본 경로가 비어 있으면 적용된다
            if (e.Kind == FileCopy.Kind)
            {
                string bad = FileCopy.CheckTarget(Stack, e.File, e.New);
                return new EditCheck { Edit = e, Status = bad == null ? EditStatus.Applied : Stack.FindReal(file) == null ? EditStatus.TargetMissing : EditStatus.Invalid, Message = bad };
            }
            // 줄 더하기(팩션 강화): 줄이 맞고 블록이 원본에 있으면 적용된다
            if (e.Kind == AddLine.Kind)
            {
                string bad = AddLine.CheckLine(e.New);
                if (bad != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Message = bad };
                if (source.Text == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 읽히지 않는다: " + file };
                if (AddLine.Find(source.Text, e.Path, e.Occurrence) == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 블록이 없다: " + e.Path };
                return new EditCheck { Edit = e, Status = EditStatus.Applied, RowText = e.New };
            }
            // 블록 복제(E18): 빌드와 같이 원본에 넣어 본 결과가 그 수정의 점검 결과다
            if (TextCopies.IsCopy(e.Kind))
            {
                var copied = source.CopyResults.FirstOrDefault(r => r.Edit == e);
                if (copied == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 읽히지 않는다: " + file };
                // 빌드가 그 파일을 쓸 수 없으면 복제도 들어가지 않는다(손으로 넣은 출력 파일)
                string clash = copied.Status == EditStatus.Applied ? conflict(file) : null;
                if (clash != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = e.Old, RowText = e.Old, Message = clash };
                return new EditCheck { Edit = e, Status = copied.Status, Current = e.Old, RowText = e.Old, Message = copied.Message };
            }
            // 복제 블록 안의 값, 값을 복제 이름으로 바꾸는 수정: 그 복제가 적용되지 않으면 막는다
            string dependency = TextCopies.Dependency(e, Project == null ? new List<Edit>() : Project.Edits);
            if (dependency != null && !ActiveCopies(parsed, conflict).Contains(dependency))
                return new EditCheck { Edit = e, Status = EditStatus.Invalid, Message = TextCopies.MissingNote(dependency, Project.Edits) };
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
                source.Text = TextCopies.Apply(TextCodec.Decode(Stack.Read(found), out source.Kind), CopiesOf(file), source.CopyResults);
                source.Rows = SourceRows.Of(file, source.Text, out error);
            }
            parsed[file] = source;
            return source;
        }

        // 이 파일의 켜진 블록 복제(E18)
        List<Edit> CopiesOf(string file)
        {
            if (Project == null) return new List<Edit>();
            return Project.Edits.Where(x => TextCopies.IsCopy(x.Kind) && x.Enabled && string.Equals(SourceStack.ToVirtual(x.File ?? ""), file, StringComparison.Ordinal)).ToList();
        }

        // 원본에 적용되고 빌드가 그 파일을 쓸 수 있는 복제의 새 이름(따옴표 없이)
        HashSet<string> ActiveCopies(Dictionary<string, SourceCheck> parsed, Func<string, string> conflict)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (Project == null) return names;
            foreach (string file in Project.Edits.Where(x => TextCopies.IsCopy(x.Kind) && x.Enabled).Select(x => SourceStack.ToVirtual(x.File ?? "")).Distinct())
            {
                if (conflict(file) != null) continue;
                foreach (var r in SourceOf(file, parsed).CopyResults.Where(r => r.Status == EditStatus.Applied)) names.Add(r.Edit.New.Trim('"'));
            }
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
