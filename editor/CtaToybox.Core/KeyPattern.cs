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
    }
}
