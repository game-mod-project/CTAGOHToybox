// "이 유닛만" 수정에 쓰는 include 사슬 계산.
// 공유 파일을 건드리지 않고 유닛에서 그 파일까지의 사슬만 유닛 전용 사본으로 만든다.
// 사본은 원본과 같은 폴더에 둔다: 사본 안의 상대 경로 include가 그대로 같은 파일을 가리킨다.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CtaToybox.Core
{
    public static class IncludeChain
    {
        // from에서 include를 따라 닿는 파일
        public static HashSet<string> Reachable(IEnumerable<SdlIncludeEdge> edges, string from)
        {
            var next = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var e in edges)
            {
                if (e.FromFile == null) continue;
                List<string> list;
                if (!next.TryGetValue(e.FromFile, out list)) { list = new List<string>(); next[e.FromFile] = list; }
                list.Add(e.ToFile);
            }
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var queue = new Queue<string>();
            queue.Enqueue(from);
            while (queue.Count > 0)
            {
                List<string> outs;
                if (!next.TryGetValue(queue.Dequeue(), out outs)) continue;
                foreach (string t in outs) if (seen.Add(t)) queue.Enqueue(t);
            }
            return seen;
        }

        // 사본으로 만들 파일: unit에서 닿고 target에 닿는 파일과 target. unit 자신은 뺀다
        public static HashSet<string> CopySet(IEnumerable<SdlIncludeEdge> edges, string unit, string target)
        {
            var list = edges.ToList();
            var result = new HashSet<string>(StringComparer.Ordinal);
            var fromUnit = Reachable(list, unit);
            if (!fromUnit.Contains(target)) return result;
            foreach (string f in fromUnit)
                if (f != unit && (f == target || Reachable(list, f).Contains(target))) result.Add(f);
            return result;
        }

        static string Stem(string leaf, out string ext)
        {
            int dot = leaf.LastIndexOf('.');
            ext = dot <= 0 ? "" : leaf.Substring(dot);
            return dot <= 0 ? leaf : leaf.Substring(0, dot);
        }

        // <원본 이름>__ctb_<유닛 파일 이름>_<유닛 경로의 해시 8자><확장자>
        public static string CopyName(string file, string unit)
        {
            int slash = file.LastIndexOf('/');
            string ext, ignored;
            string stem = Stem(file.Substring(slash + 1), out ext);
            string unitStem = Stem(unit.Substring(unit.LastIndexOf('/') + 1), out ignored);
            var clean = new StringBuilder();
            foreach (char c in unitStem) clean.Append((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' ? c : '_');
            string hash;
            using (var sha = SHA1.Create()) hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(unit))).Substring(0, 8).ToLowerInvariant();
            return file.Substring(0, slash + 1) + stem + "__ctb_" + clean + "_" + hash + ext;
        }

        // include 경로 토큰에서 파일 이름 부분만 사본의 이름으로 바꾼다. 폴더 부분과 구분자는 그대로 둔다
        public static string RewriteRaw(string raw, string copyFile)
        {
            string body = raw.Trim('"');
            int cut = Math.Max(body.LastIndexOf('/'), body.LastIndexOf('\\'));
            return "\"" + body.Substring(0, cut + 1) + copyFile.Substring(copyFile.LastIndexOf('/') + 1) + "\"";
        }

        // "이 유닛만"이 안 되는 이유. 되면 null
        public static string UnitScopeBlocker(IEnumerable<SdlIncludeEdge> edges, string unit, string file, string keyPath)
        {
            if (file == unit) return null;
            if (keyPath != null && keyPath.IndexOf("/(define ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다";
            if (CopySet(edges, unit, file).Count == 0)
                return "이 유닛이 include로 읽는 파일이 아니다(.presets이거나 다른 유닛의 파일). '전부'만 된다";
            return null;
        }
    }
}
