// 인자 없는 매크로 호출의 이름 바꾸기(E4, docs/superpowers/specs/2026-10-16-e4-macro-swap-design.md).
// 바꿀 수 있는 이름은 그 유닛을 펼칠 때 보이는 정의 가운데 지금 매크로와 같은 파일에 정의된 것이다(tier면 health.inc의 tier1~5).
// 보이지 않는 이름을 쓰면 게임이 시작할 때 "define not found"로 꺼진다(2026-10-16 G15 게임 확인). 그래서 목록 밖의 이름은 막는다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static class MacroSwap
    {
        public const string Kind = "macro-name";
        public const string NoChoiceNote = "같은 파일에 정의된 다른 매크로가 없어 바꿀 이름이 없다";

        // 매크로 호출 줄의 이름(따옴표 없이). 원문 경로의 마지막 조각 ("Tier2_Standard")에서 뽑는다. 호출 줄이 아니면 null
        public static string NameOf(string rawPath)
        {
            if (rawPath == null) return null;
            string seg = rawPath.Substring(FromLevel.LastSlash(rawPath) + 1);
            if (!seg.StartsWith("(\"", StringComparison.Ordinal) || !seg.EndsWith("\")", StringComparison.Ordinal)) return null;
            return seg.Substring(2, seg.Length - 4);
        }

        // 정의가 있는 파일(본문의 첫 토큰이 있는 파일). 본문이 비었거나 모르면 null
        static string FileOf(List<object> body)
        {
            foreach (var o in body ?? new List<object>())
            {
                var n = o as SdlNode;
                if (n == null) continue;
                if (n.File != null) return n.File;
                if (n.Children != null) { string inner = FileOf(n.Children); if (inner != null) return inner; }
            }
            return null;
        }

        static string Family(string name) { return Regex.Replace(name.ToLowerInvariant(), "[0-9]+", "#"); }

        // 바꿀 수 있는 이름들(지금 이름 포함): 같은 파일에 정의된 것. 숫자만 다른 같은 갈래가 먼저, 그다음 이름순. 정의를 모르면 빈 목록
        public static List<string> Choices(Dictionary<string, List<object>> defines, string name)
        {
            List<object> body;
            if (defines == null || name == null || !defines.TryGetValue(name, out body)) return new List<string>();
            string file = FileOf(body);
            if (file == null) return new List<string>();
            string family = Family(name);
            return defines.Where(kv => FileOf(kv.Value) == file).Select(kv => kv.Key)
                .OrderBy(k => Family(k) == family ? 0 : 1).ThenBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // 같은 갈래(숫자만 다른 이름)인가. 다른 갈래로 바꾸면 화면이 알린다
        public static bool SameFamily(string a, string b) { return a != null && b != null && Family(a) == Family(b); }

        // 수정의 새 이름이 바꿀 수 있는 이름인가. 되면 null
        public static string Check(UnitTraceResult traced, Edit e)
        {
            string oldName = e.Old == null ? null : e.Old.Trim('"'), newName = e.New == null ? null : e.New.Trim('"');
            if (newName == null || e.New.Length < 2 || e.New[0] != '"' || e.New[e.New.Length - 1] != '"') return "새 매크로 이름은 따옴표로 감싼 이름 하나다: " + e.New;
            var choices = Choices(traced == null ? null : traced.Defines, oldName);
            if (choices.Count == 0) return "이 유닛을 펼칠 때 매크로 " + oldName + "의 정의를 찾지 못했다";
            if (!choices.Any(c => string.Equals(c, newName, StringComparison.OrdinalIgnoreCase)))
                return "매크로 " + newName + "는 " + oldName + "와 같은 파일에 정의되어 있지 않다. 바꿀 수 있는 이름: " + string.Join(", ", choices.Take(20));
            return null;
        }
    }
}
