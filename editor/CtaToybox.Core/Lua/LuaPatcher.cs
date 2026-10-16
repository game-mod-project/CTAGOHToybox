// Lua 설정 값 바꾸기. 값의 원문 범위만 새 글자로 바꾸고, 고친 뒤 파일 전체를 다시 읽어 그 자리에 새 값이 있는지 본다.
// TokenPatcher와 같은 약속이다: 자리가 없으면 TargetMissing, 원래 값이 다르면 SourceChanged, 규칙에 어긋나면 Invalid,
// 고친 파일이 다시 읽히지 않으면 InvalidOperationException(빌드가 그 파일의 수정을 적용 불가로 바꾼다)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core
{
    public static class LuaPatcher
    {
        static bool Numeric(string kind) { return kind == "number" || kind == "expr"; }

        static string KindName(string kind)
        {
            return Numeric(kind) ? "수" : kind == "string" ? "문자열" : kind == "bool" ? "참·거짓" : kind == "nil" ? "nil" : "읽을 수 없음";
        }

        // 새 값이 원래 값과 같은 종류인지 본다. 맞으면 null
        public static string CheckNewValue(string oldValue, string newValue, out string warning)
        {
            warning = null;
            if (string.IsNullOrWhiteSpace(newValue)) return "새 값이 비어 있다";
            string before = LuaTable.KindOf(oldValue), after = LuaTable.KindOf(newValue);
            if (before == "nil") return "nil(값 없음)은 이번에 고칠 수 없다";
            if (after == null) return "읽을 수 없는 값이다. 수·수식(수와 * / + - ( )), 따옴표 문자열, true, false만 쓴다: " + newValue;
            if (Numeric(before) != Numeric(after) || (!Numeric(before) && before != after))
                return "원래 값과 종류가 다르다(원래 " + KindName(before) + ", 새 값 " + KindName(after) + ")";
            if (before == "string" && oldValue.Trim()[0] != newValue.Trim()[0]) return "따옴표는 원래처럼 " + oldValue.Trim()[0] + " 를 쓴다";
            return null;
        }

        // 같은 값인가: 수·수식은 공백을 빼고, 나머지는 앞뒤 공백만 빼고 비교한다
        public static bool SameValue(string a, string b)
        {
            if (a == null || b == null) return a == b;
            if (a.Trim() == b.Trim()) return true;
            if (!Numeric(LuaTable.KindOf(a)) || !Numeric(LuaTable.KindOf(b))) return false;
            return new string(a.Where(c => !char.IsWhiteSpace(c)).ToArray()) == new string(b.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)
        {
            var active = edits.Where(e => e.Enabled).ToList();
            if (active.Count == 0) return text;
            var parsed = LuaTable.Read(text);
            if (parsed.Errors.Count > 0)
            {
                foreach (var e in active) results.Add(Result(e, EditStatus.Invalid, "원본 Lua 파일을 읽지 못한다: " + parsed.Errors[0]));
                return text;
            }
            var patches = new List<KeyValuePair<Edit, LuaRow>>();
            var taken = new HashSet<int>();
            foreach (var e in active)
            {
                var row = e.Arg != 0 ? null : parsed.Rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
                if (row == null) { results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 자리가 없다: " + e.Path + " [" + e.Occurrence + "] 값 " + e.Arg)); continue; }
                if (!SameValue(row.Value, e.Old)) { results.Add(Result(e, EditStatus.SourceChanged, "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + row.Value)); continue; }
                string warning;
                string error = CheckNewValue(row.Value, e.New, out warning);
                if (error != null) { results.Add(Result(e, EditStatus.Invalid, error)); continue; }
                if (!taken.Add(row.Index)) { results.Add(Result(e, EditStatus.Invalid, "같은 값을 고치는 수정이 앞에 이미 있다")); continue; }
                patches.Add(new KeyValuePair<Edit, LuaRow>(e, row));
                results.Add(Result(e, EditStatus.Applied, warning));
            }
            if (patches.Count == 0) return text;

            var sb = new StringBuilder(text);
            foreach (var x in patches.OrderByDescending(x => x.Value.Index))
            {
                sb.Remove(x.Value.Index, x.Value.Value.Length);
                sb.Insert(x.Value.Index, x.Key.New.Trim());
            }
            string patched = sb.ToString();
            Verify(patched, patches.Select(x => x.Key));
            return patched;
        }

        // 고친 파일을 다시 읽어 수정마다 그 자리에 새 값이 있는지 본다. 아니면 InvalidOperationException.
        // 새 값 검사(CheckNewValue)를 통과한 값으로는 닿지 않는 방어 경로라 따로 떼어 시험한다(Q12)
        internal static void Verify(string patched, IEnumerable<Edit> edits)
        {
            var check = LuaTable.Read(patched);
            if (check.Errors.Count > 0) throw new InvalidOperationException("고친 Lua 파일이 다시 읽히지 않는다: " + check.Errors[0]);
            foreach (var e in edits)
            {
                var row = check.Rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
                if (row == null || row.Value != e.New.Trim()) throw new InvalidOperationException("고친 뒤 그 자리에 새 값이 없다: " + e.Path);
            }
        }

        static EditResult Result(Edit e, EditStatus status, string message)
        {
            return new EditResult { Edit = e, Status = status, Message = message };
        }
    }
}
