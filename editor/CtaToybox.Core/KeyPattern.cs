// 키 경로로 주석이나 요약 키를 찾을 때의 조회 순서. tools/lib/Annotations.ps1의 Get-Annotation과 같은 규칙이어야 한다.
//   정확한 경로 > **/꼬리 (긴 것부터) > */마지막 조각 (allowWildcard일 때만)
// **/꼬리는 값의 자리를 가리키지 않는 조각(매크로 정의·호출, 호출 인자 이름(), 꺼 둔 블록의 #)을 뺀 경로로 만든다.
using System.Collections.Generic;

namespace CtaToybox.Core
{
    public static class KeyPattern
    {
        public static List<string> PlainKeyPath(string path)
        {
            var parts = new List<string>();
            foreach (string s in path.Split('/'))
            {
                if (s.StartsWith("(") || s.EndsWith("()")) continue;
                string t = s.TrimStart('#');
                if (t.Length > 0) parts.Add(t);
            }
            return parts;
        }

        public static IEnumerable<string> Candidates(string path, bool allowWildcard)
        {
            yield return path;
            var plain = PlainKeyPath(path);
            for (int i = 0; i < plain.Count; i++) yield return "**/" + string.Join("/", plain.GetRange(i, plain.Count - i));
            if (allowWildcard) yield return "*/" + path.Substring(path.LastIndexOf('/') + 1);
        }

        // rawPath(원문 경로)를 주면 이름을 넣은 경로(/breed/perks/"health")의 후보를 먼저 낸다(*/이름 없이).
        // 펼친 경로는 따옴표 이름을 "*"로 묶어 체력·속도·기술이 한 경로가 되므로, 주석·요약·값 이름이 이름별로 갈라질 수 있게 한다
        public static IEnumerable<string> Candidates(string path, string rawPath, bool allowWildcard)
        {
            string named = NamedPath(path, rawPath);
            if (named != null)
                foreach (string key in Candidates(named, false)) yield return key;
            foreach (string key in Candidates(path, allowWildcard)) yield return key;
        }

        // "*" 조각을 원문의 이름(소문자, 따옴표 포함)으로 바꾼 경로. 바꿀 것이 없거나 두 경로의 조각이 맞지 않으면(이름에 / 가 든 경우) null
        public static string NamedPath(string path, string rawPath)
        {
            if (rawPath == null || path.IndexOf("\"*\"", System.StringComparison.Ordinal) < 0) return null;
            string[] p = path.Split('/'), r = rawPath.Split('/');
            if (p.Length != r.Length) return null;
            for (int i = 0; i < p.Length; i++)
                if (p[i] == "\"*\"")
                {
                    if (r[i].Length < 2 || r[i][0] != '"' || r[i][r[i].Length - 1] != '"') return null;
                    p[i] = r[i].ToLowerInvariant();
                }
            return string.Join("/", p);
        }
    }
}
