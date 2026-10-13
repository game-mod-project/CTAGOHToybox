// data/display-names.csv에서 엔티티의 표시 이름을 찾는다.
// 확인된 키 규칙은 desc/entity/<파일 이름> 하나다(예: desc/entity/kv85 → KV-85). 보병과 아이템의 규칙은 확인하지 않아 붙이지 않는다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class DisplayNames
    {
        readonly Dictionary<string, string> byContext = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static DisplayNames Load(string csvPath)
        {
            var names = new DisplayNames();
            if (!File.Exists(csvPath)) return names;
            var rows = ParseCsv(File.ReadAllText(csvPath, Encoding.UTF8));
            for (int i = 1; i < rows.Count; i++)
                if (rows[i].Length >= 2 && !names.byContext.ContainsKey(rows[i][0])) names.byContext[rows[i][0]] = rows[i][1];
            return names;
        }

        public string OfUnit(string unitPath)
        {
            if (!unitPath.StartsWith("entity/", StringComparison.Ordinal)) return null;
            string leaf = unitPath.Substring(unitPath.LastIndexOf('/') + 1);
            int dot = leaf.LastIndexOf('.');
            string name;
            return byContext.TryGetValue("desc/entity/" + (dot <= 0 ? leaf : leaf.Substring(0, dot)), out name) ? name : null;
        }

        // 따옴표 필드 안의 쉼표, 줄바꿈, ""를 처리한다
        public static List<string[]> ParseCsv(string text)
        {
            var rows = new List<string[]>();
            var row = new List<string>();
            var field = new StringBuilder();
            bool quoted = false, any = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (quoted)
                {
                    if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                    else if (c == '"') quoted = false;
                    else field.Append(c);
                    continue;
                }
                if (c == '"') { quoted = true; any = true; }
                else if (c == ',') { row.Add(field.ToString()); field.Clear(); any = true; }
                else if (c == '\n' || c == '\r')
                {
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                    if (any || field.Length > 0) { row.Add(field.ToString()); rows.Add(row.ToArray()); }
                    row.Clear(); field.Clear(); any = false;
                }
                else field.Append(c);
            }
            if (any || field.Length > 0) { row.Add(field.ToString()); rows.Add(row.ToArray()); }
            return rows;
        }
    }
}
