// 한 줄의 값마다 붙이는 이름(data/annotations/values.psv). 형식과 자리 규칙은 그 파일의 머리 주석에 있다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class ValueLabels
    {
        // 경로 → (자리 → 이름)
        readonly Dictionary<string, Dictionary<string, string>> map = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

        public int Count { get; private set; }

        public static ValueLabels Load(string path)
        {
            var labels = new ValueLabels();
            if (!File.Exists(path)) return labels;
            foreach (string line in File.ReadLines(path, Encoding.UTF8))
            {
                if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                string[] p = line.Split('|');
                if (p.Length < 3) throw new FormatException("값 이름 줄 형식 오류: " + line);
                Dictionary<string, string> slots;
                if (!labels.map.TryGetValue(p[0], out slots)) { slots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); labels.map[p[0]] = slots; }
                slots[p[1]] = p[2];
                labels.Count++;
            }
            return labels;
        }

        static bool IsKeyword(string text) { return text.Length > 0 && text[0] != '"' && !SdlCore.IsNumber(text) && !text.StartsWith("%"); }

        // values.psv가 이 낱말을 열쇠말로 적었는가(=낱말 또는 낱말#n). 적지 않았고 그 자리의 #n 이름이 있으면 낱말은 값이다(예: "rifle" lvl_3의 lvl_3)
        static bool DeclaresKeyword(Dictionary<string, string> slots, string word)
        {
            if (slots.ContainsKey("=" + word)) return true;
            foreach (string k in slots.Keys) if (k.StartsWith(word + "#", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        // 문자열 속 설정 조각의 뜻(values.psv의 자리 @이름). 후보 경로를 모두 본다. 모르면 null
        public string PieceLabel(string path, string rawPath, string name)
        {
            foreach (string key in KeyPattern.Candidates(path, rawPath, true))
            {
                Dictionary<string, string> slots;
                string label;
                if (map.TryGetValue(key, out slots) && slots.TryGetValue("@" + name, out label)) return label;
            }
            return null;
        }

        // 줄의 값마다 이름. 모르는 자리는 null. texts와 names는 같은 길이다(names는 감싼 호출의 이름, 없으면 null).
        // rawPath를 주면 따옴표 이름 줄은 이름을 넣은 경로의 값 이름을 먼저 찾는다(KeyPattern.Candidates)
        public List<string> Of(string path, IList<string> texts, IList<string> names, string rawPath = null)
        {
            var result = new List<string>(texts.Count);
            Dictionary<string, string> slots = null;
            foreach (string key in KeyPattern.Candidates(path, rawPath, true))
                if (map.TryGetValue(key, out slots)) break;
            string keyword = null;
            int position = 0;
            for (int i = 0; i < texts.Count; i++)
            {
                string name = names != null && i < names.Count ? names[i] : null;
                string label = null;
                if (name != null)
                {
                    if (slots != null) slots.TryGetValue(name, out label);
                }
                else if (slots != null && !slots.ContainsKey("*") && IsKeyword(texts[i]) && (i > 0 || !slots.ContainsKey("#1"))
                         && (keyword != null || DeclaresKeyword(slots, texts[i]) || !slots.ContainsKey("#" + (position + 1))))
                {
                    // 따옴표 없는 낱말(filling, requires 등)은 뒤의 값을 묶는 열쇠말이다. 첫 값이 이름 자리면 열쇠말이 아니다
                    keyword = texts[i]; position = 0;
                    if (!slots.TryGetValue("=" + texts[i], out label)) label = "열쇠말 " + texts[i] + " (뒤의 값을 묶는다)";
                }
                else
                {
                    position++;
                    if (slots != null)
                    {
                        if (!slots.TryGetValue((keyword ?? "") + "#" + position, out label) && slots.TryGetValue("*", out label))
                            label = label.Replace("{n}", (i + 1).ToString());
                    }
                }
                result.Add(label);
            }
            return result;
        }
    }
}
