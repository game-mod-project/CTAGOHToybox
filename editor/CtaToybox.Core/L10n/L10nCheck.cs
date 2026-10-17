// 번역 검사(스펙 "검사"). 원문과 번역의 <…> 태그, % 자리표시(순서까지), \n 수, {…} 목록이 같아야 하고,
// 용어집의 "그대로 둘 말"이 원문에 있으면 번역에도 있어야 하며, 번역 글자가 바꾼 글꼴(main_*.fnt)에 모두 있어야 한다.
// 글꼴은 BMFont 바이너리 v3("BMF" + 3)의 블록 4(chars, 항목 20바이트, 앞 4바이트가 글자 번호)에서 글자 목록을 읽는다
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.L10n
{
    public sealed class GlossaryTerm
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string Note { get; set; }
        public bool Keep { get { return Source == Target; } }
    }

    public static class Glossary
    {
        // 형식: 원문|번역|메모. '#'로 시작하는 줄과 빈 줄은 건너뛴다. 파일이 없으면 빈 목록
        public static List<GlossaryTerm> Load(string psvPath)
        {
            var list = new List<GlossaryTerm>();
            if (!File.Exists(psvPath)) return list;
            foreach (string raw in File.ReadAllLines(psvPath, Encoding.UTF8))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;
                var p = line.Split('|');
                if (p.Length < 2 || p[0].Trim().Length == 0) continue;
                list.Add(new GlossaryTerm { Source = p[0].Trim(), Target = p[1].Trim(), Note = p.Length > 2 ? p[2].Trim() : "" });
            }
            return list;
        }
    }

    public static class BmFont
    {
        public static HashSet<int> ReadChars(byte[] fnt)
        {
            if (fnt == null || fnt.Length < 4 || fnt[0] != 'B' || fnt[1] != 'M' || fnt[2] != 'F') throw new InvalidDataException("BMFont 바이너리가 아니다");
            if (fnt[3] != 3) throw new InvalidDataException("BMFont 바이너리 버전 " + fnt[3] + "은 읽지 못한다(3만)");
            var chars = new HashSet<int>();
            int i = 4;
            while (i + 5 <= fnt.Length)
            {
                int type = fnt[i], size = BitConverter.ToInt32(fnt, i + 1);
                int data = i + 5;
                if (size < 0 || data + size > fnt.Length) throw new InvalidDataException("BMFont 블록 크기가 파일을 넘는다");
                if (type == 4) for (int k = data; k + 20 <= data + size; k += 20) chars.Add((int)BitConverter.ToUInt32(fnt, k));
                i = data + size;
            }
            return chars;
        }
    }

    public sealed class CheckIssue
    {
        public string File { get; set; }
        public string Key { get; set; }
        public string Kind { get; set; }      // tags, placeholders, newlines, braces, keep-term, glyph(오류) / untranslated, fuzzy(알림)
        public string Detail { get; set; }
        public bool IsError { get { return Kind != "untranslated" && Kind != "fuzzy"; } }
    }

    public static class L10nCheck
    {
        static readonly Regex Tag = new Regex("<[^>]*>", RegexOptions.CultureInvariant);
        static readonly Regex Placeholder = new Regex(@"%(\d+\$)?[-+ 0#]*\d*(\.\d+)?[sdifuxXc%]", RegexOptions.CultureInvariant);
        static readonly Regex Brace = new Regex(@"\{[^}]*\}", RegexOptions.CultureInvariant);

        static string Sorted(IEnumerable<string> xs) { return string.Join("\u0001", xs.OrderBy(x => x, StringComparer.Ordinal)); }
        static int Newlines(string s) { return Regex.Matches(s, @"(?<!\\)\\n").Count; }

        public static List<CheckIssue> Entry(string file, PoEntry e, List<GlossaryTerm> terms, HashSet<int> glyphs)
        {
            var issues = new List<CheckIssue>();
            void Add(string kind, string detail) { issues.Add(new CheckIssue { File = file, Key = e.Key.TrimStart('\u0004'), Kind = kind, Detail = detail }); }
            if (e.Str.Length == 0) { Add("untranslated", ""); return issues; }
            if (e.Fuzzy) Add("fuzzy", e.PreviousId ?? "");
            var st = Tag.Matches(e.Id).Select(m => m.Value).ToList(); var tt = Tag.Matches(e.Str).Select(m => m.Value).ToList();
            if (Sorted(st) != Sorted(tt)) Add("tags", "원문 " + string.Join(" ", st) + " / 번역 " + string.Join(" ", tt));
            var sp = Placeholder.Matches(e.Id).Select(m => m.Value).ToList(); var tp = Placeholder.Matches(e.Str).Select(m => m.Value).ToList();
            if (!sp.SequenceEqual(tp)) Add("placeholders", "원문 " + string.Join(" ", sp) + " / 번역 " + string.Join(" ", tp));
            if (Newlines(e.Id) != Newlines(e.Str)) Add("newlines", "원문 " + Newlines(e.Id) + " / 번역 " + Newlines(e.Str));
            var sb = Brace.Matches(e.Id).Select(m => m.Value).ToList(); var tb = Brace.Matches(e.Str).Select(m => m.Value).ToList();
            if (Sorted(sb) != Sorted(tb)) Add("braces", "원문 " + string.Join(" ", sb) + " / 번역 " + string.Join(" ", tb));
            if (terms != null)
                foreach (var t in terms.Where(t => t.Keep))
                    if (Regex.IsMatch(e.Id, @"(?<![\w])" + Regex.Escape(t.Source) + @"(?![\w])") && !e.Str.Contains(t.Source)) Add("keep-term", t.Source);
            if (glyphs != null)
            {
                string bare = Tag.Replace(e.Str, "").Replace("\\n", "").Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\t", "");
                var missing = new List<string>();
                for (int i = 0; i < bare.Length; i++)
                {
                    int cp = char.ConvertToUtf32(bare, i);
                    if (char.IsSurrogatePair(bare, i)) i++;
                    if (cp < 0x20 || glyphs.Contains(cp)) continue;
                    string label = char.ConvertFromUtf32(cp) + " U+" + cp.ToString("X4");
                    if (!missing.Contains(label)) missing.Add(label);
                }
                if (missing.Count > 0) Add("glyph", string.Join(", ", missing));
            }
            return issues;
        }

        public static List<CheckIssue> Catalog(string catalogDir, List<GlossaryTerm> terms, HashSet<int> glyphs, string fileFilter = null)
        {
            var issues = new List<CheckIssue>();
            foreach (var (path, po) in L10nCatalog.All(catalogDir))
            {
                if (fileFilter != null && path.IndexOf(fileFilter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                foreach (var e in po.Entries) issues.AddRange(Entry(path, e, terms, glyphs));
            }
            return issues;
        }

        // 대상 main_*.fnt의 글자 교집합(모든 크기에 있는 글자만 통과)
        public static HashSet<int> MainGlyphs(string gameDir, IEnumerable<string> fntNames)
        {
            HashSet<int> all = null;
            using (var z = ZipFile.OpenRead(Path.Combine(gameDir, "resource", "interface", "interface.pak")))
                foreach (string name in fntNames.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var e = z.Entries.FirstOrDefault(x => string.Equals(x.FullName, "font/" + name, StringComparison.OrdinalIgnoreCase));
                    if (e == null) throw new InvalidDataException("interface.pak에 글꼴이 없다: font/" + name);
                    byte[] bytes;
                    using (var s = e.Open()) using (var m = new MemoryStream()) { s.CopyTo(m); bytes = m.ToArray(); }
                    var chars = BmFont.ReadChars(bytes);
                    if (all == null) all = chars; else all.IntersectWith(chars);
                }
            return all ?? new HashSet<int>();
        }
    }
}
