// 소환 유닛의 종류 규칙(editor/spawn/categories.psv, 손으로 고친다). 형식 "종류|규칙|값,값", '#' 뒤는 주석.
// 규칙: folder(정의 경로에 조각이 있다), name(정의 파일 이름에 단어가 있다), button(로스터 b() 값).
// 순서(스펙 "종류 나누기"): 로스터 b()가 tanks가 아니면 그 값의 button 규칙 → folder·name 줄(파일 순서) → 편집기 종류.
// b(tanks)이거나 로스터에 없으면 folder·name 규칙으로 가르고, 못 가르면 편집기 종류(tank → medium, cannon → empl, car → car)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CtaToybox.Core.Spawn
{
    public sealed class CategoryRule
    {
        public string Category { get; set; }
        public string Kind { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }

    public sealed class SpawnCategories
    {
        public static readonly string[] All = { "light", "medium", "heavy", "td", "air", "car", "empl", "other" };
        public List<CategoryRule> Rules { get; } = new List<CategoryRule>();

        public static SpawnCategories Parse(string text, List<string> warnings)
        {
            var c = new SpawnCategories();
            int n = 0;
            foreach (string raw in (text ?? "").Split('\n'))
            {
                n++;
                string line = raw;
                int hash = line.IndexOf('#');
                if (hash >= 0) line = line.Substring(0, hash);
                line = line.Trim();
                if (line.Length == 0) continue;
                var parts = line.Split('|');
                if (parts.Length != 3 || !All.Contains(parts[0].Trim()) || !(parts[1].Trim() == "folder" || parts[1].Trim() == "name" || parts[1].Trim() == "button"))
                {
                    warnings?.Add("categories.psv " + n + "행을 읽지 못했다: " + raw.Trim());
                    continue;
                }
                c.Rules.Add(new CategoryRule { Category = parts[0].Trim(), Kind = parts[1].Trim(), Values = parts[2].Split(',').Select(v => v.Trim().ToLowerInvariant()).Where(v => v.Length > 0).ToList() });
            }
            return c;
        }

        public static SpawnCategories Load(string path, List<string> warnings)
        {
            if (!File.Exists(path)) { warnings?.Add("종류 규칙 파일이 없다: " + path); return new SpawnCategories(); }
            return Parse(File.ReadAllText(path), warnings);
        }

        // def: 정의 가상 경로, button: 로스터 b() 값(없으면 null), editorKind: 편집기 종류(tank·cannon·car 등). 아무 규칙에도 안 맞을 때 쓴다
        public string Of(string def, string button, string editorKind = null)
        {
            string path = SourceStack.ToVirtual(def);
            string leaf = path.Substring(path.LastIndexOf('/') + 1);
            int dot = leaf.LastIndexOf('.');
            if (dot > 0) leaf = leaf.Substring(0, dot);
            if (button != null && !string.Equals(button, "tanks", StringComparison.OrdinalIgnoreCase))
            {
                var b = Rules.FirstOrDefault(r => r.Kind == "button" && r.Values.Contains(button.ToLowerInvariant()));
                if (b != null) return b.Category;
            }
            // folder·name 줄은 파일 순서대로 보고 처음 맞는 줄이 이긴다(이름 줄을 앞에 두면 tank_medium 폴더의 stug도 td)
            var hit = Rules.FirstOrDefault(r => (r.Kind == "folder" && r.Values.Any(v => path.Contains(v))) || (r.Kind == "name" && r.Values.Any(v => leaf.Contains(v))));
            if (hit != null) return hit.Category;
            if (string.Equals(button, "tanks", StringComparison.OrdinalIgnoreCase) || editorKind == "tank") return "medium";
            if (editorKind == "cannon") return "empl";
            if (editorKind == "car") return "car";
            return "other";
        }
    }
}
