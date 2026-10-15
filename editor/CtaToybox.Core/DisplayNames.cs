// data/display-names.csv에서 엔티티의 표시 이름을 찾는다.
// 확인된 키 규칙은 desc/entity/<파일 이름> 하나다(예: desc/entity/kv85 → KV-85). 보병과 아이템의 규칙은 확인하지 않아 붙이지 않는다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class DisplayNames
    {
        readonly Dictionary<string, string> byContext = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // 현지화에 없는 이름(게임 설정 파일 등). 현지화보다 먼저 찾는다
        readonly Dictionary<string, string> byPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public void AddPathName(string virtualPath, string name) { byPath[virtualPath] = name; }

        public static DisplayNames Load(string csvPath)
        {
            var names = new DisplayNames();
            if (!File.Exists(csvPath)) return names;
            var rows = ParseCsv(File.ReadAllText(csvPath, Encoding.UTF8));
            for (int i = 1; i < rows.Count; i++)
                if (rows[i].Length >= 2 && !names.byContext.ContainsKey(rows[i][0])) names.byContext[rows[i][0]] = rows[i][1];
            return names;
        }

        // 게임 현지화 파일(.pot) 하나를 읽는다. msgctxt가 키, msgid가 글이다. 색 태그(<c(...)>, </c>)와 \n은 뗀다
        public void AddPot(string text)
        {
            string ctxt = null, id = null, mode = "";
            foreach (string raw in (text.Replace("\r", "") + "\n\n").Split('\n'))
            {
                string l = raw.TrimEnd();
                if (l.StartsWith("msgctxt ")) { ctxt = Unquote(l.Substring(8)); id = null; mode = "ctxt"; }
                else if (l.StartsWith("msgid ")) { id = Unquote(l.Substring(6)); mode = "id"; }
                else if (l.StartsWith("msgstr ")) mode = "str";
                else if (l.StartsWith("\"")) { if (mode == "id") id += Unquote(l); else if (mode == "ctxt") ctxt += Unquote(l); }
                else if (l.Trim().Length == 0)
                {
                    if (!string.IsNullOrEmpty(ctxt) && id != null) byContext[ctxt] = Clean(id);
                    ctxt = null; id = null; mode = "";
                }
            }
        }

        static string Unquote(string s)
        {
            var m = System.Text.RegularExpressions.Regex.Match(s, "\"(.*)\"\\s*$");
            return m.Success ? m.Groups[1].Value.Replace("\\\"", "\"") : "";
        }

        static string Clean(string s)
        {
            s = System.Text.RegularExpressions.Regex.Replace(s, @"<c\([^)]*\)>|</c>", "");
            return s.Replace("\\n", " ").Trim();
        }

        // 게임과 켜 둔 모드의 현지화(default)를 로드 순서대로 읽는다. 뒤의 것이 이긴다.
        // 바닐라: <게임>/localizations/default.pak. 모드: <모드>/localizations/default.pak 또는 default 폴더 아래의 .pot
        public static DisplayNames FromGame(EditorSettings settings, LayerPlan plan)
        {
            var names = new DisplayNames();
            names.AddPak(Path.Combine(settings.GameDir, "localizations", "default.pak"));
            foreach (var layer in plan.Layers)
            {
                if (layer.Name == "vanilla") continue;
                string root = Path.GetDirectoryName(Path.GetFullPath(layer.ResourceDir));
                names.AddPak(Path.Combine(root, "localizations", "default.pak"));
                string dir = Path.Combine(root, "localizations", "default");
                if (Directory.Exists(dir))
                    foreach (string f in Directory.EnumerateFiles(dir, "*.pot", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                        names.AddPot(File.ReadAllText(f, Encoding.UTF8));
            }
            return names;
        }

        void AddPak(string pak)
        {
            if (!File.Exists(pak)) return;
            using (var zip = System.IO.Compression.ZipFile.OpenRead(pak))
                foreach (var e in zip.Entries.Where(x => x.FullName.EndsWith(".pot", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase))
                    using (var r = new StreamReader(e.Open(), Encoding.UTF8)) AddPot(r.ReadToEnd());
        }

        // 유닛의 게임 속 이름. 키 규칙은 2026-10-14에 현지화 데이터로 확인했다:
        //   entity/**/<이름>.def -> desc/entity/<이름>. 없고 이름이 _x, _xx로 끝나면 그 앞 이름의 것에 파일 이름을 붙인다
        //   set/breed/<경로>.set -> desc/human/<경로>
        //   set/stuff/**/<a.b.c> -> desc/weapon|stuff|ammo/<a b c>, 또는 마지막 조각(확장자)을 뺀 <a b>
        // 찾지 못하면 null
        public string OfUnit(string unitPath)
        {
            string leaf = unitPath.Substring(unitPath.LastIndexOf('/') + 1);
            string name;
            if (byPath.TryGetValue(unitPath, out name)) return name;
            if (unitPath.StartsWith("entity/", StringComparison.Ordinal))
            {
                int dot = leaf.LastIndexOf('.');
                string stem = dot <= 0 ? leaf : leaf.Substring(0, dot);
                if (byContext.TryGetValue("desc/entity/" + stem, out name)) return name;
                string bas = System.Text.RegularExpressions.Regex.Replace(stem, "(_x+)+$", "");
                if (bas != stem && byContext.TryGetValue("desc/entity/" + bas, out name)) return name + " (" + stem + ")";
                return null;
            }
            if (unitPath.StartsWith("set/breed/", StringComparison.Ordinal) && unitPath.EndsWith(".set", StringComparison.Ordinal))
                return byContext.TryGetValue("desc/human/" + unitPath.Substring(10, unitPath.Length - 14), out name) ? name : null;
            if (unitPath.StartsWith("set/stuff/", StringComparison.Ordinal) && !unitPath.StartsWith("set/stuff/_settings/", StringComparison.Ordinal))
            {
                string[] words = leaf.Split('.');
                string ext = words.Length > 1 ? words[words.Length - 1] : "";
                // 틀·설정 파일은 아이템이 아니다: 이름을 붙이면 아이템처럼 보인다
                if (leaf.StartsWith(".") || ext == "pattern" || ext == "inc" || ext == "fsm" || ext == "hand" || ext == "presets") return null;
                // 같은 이름의 총과 탄창이 있다(desc/weapon/chauchat, desc/ammo/chauchat). 파일 종류로 이름공간의 순서를 정한다
                string[] order = ext == "ammo" ? new[] { "desc/ammo/", "desc/stuff/", "desc/weapon/" }
                               : ext == "" || ext == "weapon" ? new[] { "desc/weapon/", "desc/stuff/", "desc/ammo/" }
                               : new[] { "desc/stuff/", "desc/weapon/", "desc/ammo/" };
                var keys = new List<string> { string.Join(" ", words) };
                if (words.Length > 1) keys.Add(string.Join(" ", words, 0, words.Length - 1));
                foreach (string k in keys)
                    foreach (string ns in order)
                        if (byContext.TryGetValue(ns + k, out name)) return name;
            }
            return null;
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
