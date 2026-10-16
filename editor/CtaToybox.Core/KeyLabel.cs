// 키 경로를 사람이 읽을 수 있게 바꾼다. 화면 표시용이고 경로 자체는 바꾸지 않는다.
//   /game_entity/volume/thickness/front -> volume › thickness › front (맨 앞의 최상위 블록 이름은 뺀다)
//   extender[inventory] -> inventory, ("fire rate") -> fire rate (매크로), "*" -> *
//   따옴표 안의 / 는 나누지 않는다: /"mp/ger/mid/nco" -> mp/ger/mid/nco
using System.Collections.Generic;
using System.Text;

namespace CtaToybox.Core
{
    public static class KeyLabel
    {
        static string Segment(string s)
        {
            int b = s.IndexOf('[');
            if (b > 0 && s.EndsWith("]")) return s.Substring(b + 1, s.Length - b - 2);
            if (s.StartsWith("(\"") && s.EndsWith("\")")) return s.Substring(2, s.Length - 4) + " (매크로)";
            if (s.StartsWith("(") && s.EndsWith(")")) return s.Substring(1, s.Length - 2);
            return s.Trim('"');
        }

        // 따옴표 밖의 / 로 나눈다
        public static List<string> Split(string path)
        {
            var parts = new List<string>();
            var sb = new StringBuilder();
            bool quoted = false;
            foreach (char c in path)
            {
                if (c == '"') quoted = !quoted;
                if (c == '/' && !quoted) { if (sb.Length > 0) parts.Add(sb.ToString()); sb.Clear(); continue; }
                sb.Append(c);
            }
            if (sb.Length > 0) parts.Add(sb.ToString());
            return parts;
        }

        // 펼친 경로에서 "*"로 묶인 따옴표 이름 블록의 원문 이름들(따옴표를 뗀 것), 바깥부터. 없거나 두 경로가 맞지 않으면 빈 목록.
        // 수정 목록이 "수평 시야각 (tank_range_200)"처럼 어느 블록의 값인지 보이게 한다(E16)
        public static List<string> QuotedNames(string path, string rawPath)
        {
            var names = new List<string>();
            if (path == null || rawPath == null) return names;
            var p = Split(path);
            var r = Split(rawPath);
            if (p.Count != r.Count) return names;
            for (int i = 0; i < p.Count; i++)
                if (p[i] == "\"*\"" && r[i].Length >= 2 && r[i][0] == '"') names.Add(r[i].Trim('"'));
            return names;
        }

        public static string Readable(string path)
        {
            var parts = Split(path);
            if (parts.Count == 0) return path;
            if (parts.Count > 1) parts.RemoveAt(0);
            for (int i = 0; i < parts.Count; i++) parts[i] = Segment(parts[i]);
            return string.Join(" › ", parts);
        }
    }
}
