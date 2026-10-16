// 텍스트 하나에 수정 여러 건을 적용한다. 파일을 다시 조립하지 않고 값 토큰의 글자만 바꾼다.
// 위치가 뒤인 것부터 바꾸므로 길이가 달라져도 앞의 위치가 밀리지 않는다.
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public enum EditStatus { Applied, SourceChanged, TargetMissing, Invalid }

    public sealed class EditResult
    {
        public Edit Edit { get; set; }
        public EditStatus Status { get; set; }
        public string Message { get; set; }
    }

    // 수정 목록에 없는 치환: include 경로를 사본으로 고쳐 쓰는 데 쓴다. Old와 New는 따옴표를 포함한 토큰 원문이다
    public sealed class RawPatch
    {
        public int Index;
        public string Old;
        public string New;
    }

    public static class TokenPatcher
    {
        // 새 값이 토큰 하나인지, 원래 값과 같은 종류(따옴표 문자열 또는 낱말)인지 본다.
        // 반환값이 null이 아니면 적용할 수 없는 이유다. warning은 막지 않는 경고다.
        public static string CheckNewValue(string oldToken, string newToken, out string warning)
        {
            warning = null;
            if (string.IsNullOrEmpty(newToken)) return "새 값이 비어 있다";
            var tokens = SdlCore.Tokenize(newToken);
            if (tokens.Count != 1 || tokens[0].Text != newToken || (tokens[0].Kind != "atom" && tokens[0].Kind != "str"))
                return "새 값은 낱말 하나이거나 따옴표로 닫힌 문자열 하나여야 한다 (공백, 괄호, 세미콜론 불가): " + newToken;
            bool oldQuoted = oldToken.StartsWith("\""), newQuoted = tokens[0].Kind == "str";
            if (oldQuoted != newQuoted) return oldQuoted ? "원래 값이 따옴표 문자열이다. 새 값도 따옴표로 감싼다" : "원래 값이 따옴표 없는 낱말이다. 새 값에 따옴표를 쓸 수 없다";
            if (SdlCore.IsNumber(oldToken) && !SdlCore.IsNumber(newToken)) warning = "원래 값은 숫자인데 새 값은 숫자가 아니다: " + newToken;
            return null;
        }

        sealed class Patch { public Edit Edit; public int Index; public int Length; public string New; }

        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)
        {
            return Apply(text, edits, results, null);
        }

        // raw의 자리가 원본과 다르거나 원본이 파싱되지 않으면 던진다: include 경로를 반만 고친 파일을 쓰면 안 된다
        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results, IReadOnlyList<RawPatch> raw)
        {
            var active = edits.Where(e => e.Enabled).ToList();
            bool hasRaw = raw != null && raw.Count > 0;
            if (active.Count == 0 && !hasRaw) return text;
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0)
            {
                foreach (var e in active) results.Add(Result(e, EditStatus.Invalid, "원본 파일이 파싱되지 않는다: " + parsed.Errors[0]));
                if (hasRaw) throw new InvalidOperationException("원본 파일이 파싱되지 않아 include 경로를 고쳐 쓸 수 없다: " + parsed.Errors[0]);
                return text;
            }

            var patches = new List<Patch>();
            var taken = new HashSet<int>();
            if (hasRaw)
            {
                foreach (var r in raw)
                {
                    if (r.Index < 0 || r.Index + r.Old.Length > text.Length || string.CompareOrdinal(text, r.Index, r.Old, 0, r.Old.Length) != 0)
                        throw new InvalidOperationException("include 경로의 자리가 원본과 다르다: " + r.Old);
                    var tokens = SdlCore.Tokenize(r.New);
                    if (tokens.Count != 1 || tokens[0].Kind != "str" || tokens[0].Text != r.New)
                        throw new InvalidOperationException("include 경로는 따옴표로 닫힌 문자열 하나여야 한다: " + r.New);
                    if (!taken.Add(r.Index)) throw new InvalidOperationException("같은 include 경로를 두 번 고쳐 쓰려 했다: " + r.Old);
                    patches.Add(new Patch { Index = r.Index, Length = r.Old.Length, New = r.New });
                }
            }
            var index = new KeyIndex(parsed.Root);
            foreach (var e in active)
            {
                // 따옴표 이름 블록의 수정은 그 자리의 이름도 같아야 쓴다(Q7). 다르면 다른 항목이다
                if (e.NamedPath != null && !SameName(index.Row(e.Path, e.Occurrence), e))
                {
                    results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 이름의 항목이 없다: " + e.NamedPath));
                    continue;
                }
                // 인자 없는 매크로 호출의 이름(E4): 호출 줄의 첫 토큰을 바꾼다. 바꾸면 키 경로도 바뀐다
                var node = e.Kind == MacroSwap.Kind ? (index.Row(e.Path, e.Occurrence) == null ? null : index.Row(e.Path, e.Occurrence).Head) : index.Find(e.Path, e.Occurrence, e.Arg);
                if (node == null) { results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 자리가 없다: " + e.Path + " [" + e.Occurrence + "] 값 " + e.Arg)); continue; }
                if (node.Text != e.Old) { results.Add(Result(e, EditStatus.SourceChanged, "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text)); continue; }
                string warning;
                string error = CheckNewValue(e.Old, e.New, out warning);
                if (error != null) { results.Add(Result(e, EditStatus.Invalid, error)); continue; }
                if (!taken.Add(node.Index)) { results.Add(Result(e, EditStatus.Invalid, "같은 값을 고치는 수정이 앞에 이미 있다")); continue; }
                patches.Add(new Patch { Edit = e, Index = node.Index, Length = node.Text.Length, New = e.New });
                results.Add(Result(e, EditStatus.Applied, warning));
            }
            if (patches.Count == 0) return text;

            var sb = new System.Text.StringBuilder(text);
            foreach (var p in patches.OrderByDescending(x => x.Index))
            {
                sb.Remove(p.Index, p.Length);
                sb.Insert(p.Index, p.New);
            }
            string patched = sb.ToString();

            // 고친 결과를 다시 읽어 구조가 그대로인지, 그 자리에 새 값이 들어갔는지 확인한다
            var check = SdlCore.Parse(patched);
            if (check.Errors.Count > 0 || !SdlCore.RoundTrip(check)) throw new InvalidOperationException("고친 파일이 다시 파싱되지 않는다");
            var checkIndex = new KeyIndex(check.Root);
            foreach (var p in patches)
            {
                if (p.Edit == null) continue;
                if (p.Edit.Kind == MacroSwap.Kind)
                {
                    // 이름이 바뀐 호출 줄이 같은 부모 아래 새 이름으로 있어야 한다
                    string parent = p.Edit.Path.Substring(0, FromLevel.LastSlash(p.Edit.Path));
                    string renamed = parent + "/(" + p.Edit.New.ToLowerInvariant() + ")";
                    if (!KeyLocator.Rows(check.Root).Any(r => string.Equals(r.Path, renamed, StringComparison.OrdinalIgnoreCase) && r.Head != null && r.Head.Text == p.Edit.New))
                        throw new InvalidOperationException("고친 뒤 새 이름의 매크로 호출이 없다: " + renamed);
                    continue;
                }
                var node = checkIndex.Find(p.Edit.Path, p.Edit.Occurrence, p.Edit.Arg);
                if (node == null || node.Text != p.Edit.New) throw new InvalidOperationException("고친 뒤 그 자리에 새 값이 없다. 이 값을 바꾸면 키 경로가 바뀐다(블록 이름 뒤의 한정자나 숫자 행의 첫 값): " + p.Edit.Path);
            }
            return patched;
        }

        // 행이 수정의 이름 경로와 같은 항목인가(원문 경로와 그 경로 가운데 몇 번째)
        public static bool SameName(KeyRow row, Edit e)
        {
            return row != null && string.Equals(row.RawPath, e.NamedPath, StringComparison.OrdinalIgnoreCase) && row.RawOccurrence == e.NamedOccurrence;
        }

        static EditResult Result(Edit e, EditStatus status, string message)
        {
            return new EditResult { Edit = e, Status = status, Message = message };
        }
    }
}
