// '이 유닛만'이 안 되는 이유를 한곳에서 정한다(앱, 명령줄 add·trace, 수정 점검, 빌드가 같이 쓴다). 되면 null.
// 원형에서 물려받은 값(E14)은 자식 파일의 맨 위 블록에 같은 키를 덧붙여 고친다. 게임 확인(G16, 2026-10-16): 자식 파일의 같은 키가 이긴다.
// 확인한 것은 맨 위 블록 바로 아래의 한 번만 적는 키(filling)뿐이라 그런 줄만 허락한다. 감싼 블록, 여러 번 적는 키, 조건 안의 줄은 막는다.
// 나머지 값은 include 사슬 규칙(IncludeChain.UnitScopeBlocker)이다. 다른 유닛이 이 유닛을 물려받는지(FromLevel.HeirsNote)는 색인이 필요해 부르는 쪽이 본다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static class UnitScope
    {
        static readonly Regex PlainKey = new Regex("^[a-z0-9_]+$", RegexOptions.CultureInvariant);

        // 원형에서 물려받은 줄 가운데 이 값(파일·키 경로·몇 번째·몇 번째 값)이 든 줄. 아니면 null
        public static TraceRow InheritedRow(UnitTraceResult traced, string file, string keyPath, int occurrence, int arg)
        {
            if (traced == null) return null;
            return traced.Rows.FirstOrDefault(r => r.From != null && r.Values.Any(v => v.Arg == arg && v.Occurrence == occurrence
                && string.Equals(v.File, file, StringComparison.Ordinal) && string.Equals(v.KeyPath, keyPath, StringComparison.OrdinalIgnoreCase)));
        }

        // 물려받은 줄을 자식 파일에 덧붙여 '이 유닛만' 고칠 수 없는 이유. 되면 null
        public static string InheritedBlocker(UnitTraceResult traced, TraceRow row)
        {
            if (row.FromState == FromLevel.Overridden) return "자식 쪽 파일에 같은 키가 있어 이 원형 값은 게임에서 쓰이지 않는다. 자식 쪽 줄을 고친다";
            if (row.FromState != FromLevel.Inherited) return "원형에서 물려받는 값이다. 이 줄은 자식 쪽에도 같은 블록·키가 있거나 조건이 걸려 덧붙였을 때 어떻게 되는지 확인하지 않았다. '전부'만 된다";
            var parts = KeyLabel.Split(row.Path);
            if (parts.Count != 2 || !PlainKey.IsMatch(parts[1]))
                return "원형에서 물려받는 값이다. '이 유닛만'은 원형의 맨 위 블록 바로 아래 키만 된다(자식 파일에 그 키를 덧붙인다). 감싼 블록 안의 키는 블록째 바뀌는지 확인하지 않았다. '전부'만 된다";
            if (row.Form != "block") return "원형의 매크로 호출 줄은 '이 유닛만' 고칠 수 없다. '전부'만 된다";
            if (row.Cond != null) return "조건(" + row.Cond + ") 안의 줄은 '이 유닛만'으로 덧붙이지 않는다. '전부'만 된다";
            if (traced.Rows.Count(r => r.From == row.From && string.Equals(r.Path, row.Path, StringComparison.OrdinalIgnoreCase)) > 1)
                return "원형에 이 키가 여러 번 있다. 덧붙이면 쌓이는지 바뀌는지 확인하지 않았다. '전부'만 된다";
            if (traced.Rows.Any(r => r.From == row.From && r.Path.StartsWith(row.Path + "/", StringComparison.OrdinalIgnoreCase)))
                return "이 키 안에 다른 블록이 있다. 덧붙이면 블록째 바뀐다. '전부'만 된다";
            if (row.Values.Count == 0 || row.Values.Any(v => v.Arg < 0 || v.File == null || v.Synthetic))
                return "계산·치환된 값이 있는 줄이라 덧붙일 줄을 만들 수 없다. '전부'만 된다";
            // 값이 원형 파일의 그 줄에 직접 적혀 있어야 한다. 매크로 인자로 만든 값({rate %r}의 r)이나 include한 파일의 값은 자리가 다르다
            if (row.Values.Any(v => v.File != row.From || !string.Equals(FromLinks.Reroot(v.KeyPath, "/from"), row.Path, StringComparison.OrdinalIgnoreCase)))
                return "원형 파일의 그 줄에 직접 적힌 값이 아니다(매크로 인자나 include한 파일의 값). '전부'만 된다";
            return null;
        }

        // 값 하나의 '이 유닛만' 이유. traced는 그 유닛을 펼친 결과
        public static string Blocker(UnitTraceResult traced, string file, string keyPath, int occurrence, int arg)
        {
            var row = InheritedRow(traced, file, keyPath, occurrence, arg);
            if (row != null) return InheritedBlocker(traced, row);
            return IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, file, keyPath);
        }

        // 자식 파일에 덧붙일 줄: 원형의 키 표기 그대로 {Key 값 값 …}에 수정한 값을 넣는다
        public static string AppendLine(TraceRow row, IEnumerable<Edit> edits)
        {
            var parts = KeyLabel.Split(row.RawPath ?? row.Path);
            var values = row.Values.Select(v => v.Text).ToList();
            foreach (var e in edits)
                if (e.Arg >= 0 && e.Arg < values.Count) values[e.Arg] = e.New;
            return "{" + parts[parts.Count - 1] + " " + string.Join(" ", values) + "}";
        }
    }
}
