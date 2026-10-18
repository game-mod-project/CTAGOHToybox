// 한글이 없는 글꼴을 한글이 있는 main_<크기>.fnt로 돌린다(스펙 "글꼴").
// interface.pak의 font/*.font는 {font_atlas {name} {size} {height} {style} {binary_data "<이름>.fnt"}}다(2026-10-17 확인, 96개).
// main(Arial Unicode MS)만 한글 음절을 가진다(main_42는 없음). 다른 글꼴은 같은 이름의 .font를 내되 binary_data만 가장 가까운 main으로 바꾼다.
// name·size·height·style은 원래 값으로 둔다(AI 판단: 화면 배치가 원래 글꼴 크기를 기준으로 하므로)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.L10n
{
    public sealed class FontOverride
    {
        public string FontPath { get; set; }   // interface/font/roboto_con_15.font (출력 resource/ 아래)
        public string Text { get; set; }
        public string TargetFnt { get; set; }
    }

    public static class FontMap
    {
        public static readonly int[] MainSizes = { 8, 9, 10, 12, 15, 18, 21, 27, 28, 30 };
        public static readonly int[] MainBoldSizes = { 8, 9, 10, 12 };
        static readonly Regex Binary = new Regex("(\\{\\s*binary_data\\s+\")([^\"]*)(\")", RegexOptions.CultureInvariant);

        static int Nearest(int[] sizes, int size)
        {
            return sizes.OrderBy(s => Math.Abs(s - size)).ThenBy(s => s).First();
        }

        public static string TargetFor(int size, bool bold)
        {
            if (bold)
            {
                int b = Nearest(MainBoldSizes, size);
                if (b == size) return "main_" + b + "b.fnt";
            }
            return "main_" + Nearest(MainSizes, size) + ".fnt";
        }

        static string Value(SdlNode root, string key)
        {
            foreach (object o in root.Children)
            {
                var n = SdlCore.Unwrap(o);
                if (n.Kind != "brace") continue;
                foreach (object c in n.Children)
                {
                    var b = SdlCore.Unwrap(c);
                    if (b.Kind == "brace" && b.Children.Count >= 2 && string.Equals(SdlCore.Unwrap(b.Children[0]).Text, key, StringComparison.OrdinalIgnoreCase)) return SdlCore.Unwrap(b.Children[1]).Text.Trim('"');
                }
            }
            return null;
        }

        static byte[] Read(ZipArchiveEntry e) { using (var s = e.Open()) using (var m = new MemoryStream()) { s.CopyTo(m); return m.ToArray(); } }

        public static List<FontOverride> Build(string interfacePak)
        {
            var list = new List<FontOverride>();
            using (var z = ZipFile.OpenRead(interfacePak))
            {
                var entries = z.Entries.ToDictionary(e => e.FullName.ToLowerInvariant(), e => e);
                foreach (var e in z.Entries.Where(x => x.FullName.StartsWith("font/", StringComparison.OrdinalIgnoreCase) && x.FullName.EndsWith(".font", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase))
                {
                    string text = Encoding.UTF8.GetString(Read(e));
                    var root = SdlCore.Parse(text).Root;
                    if (string.Equals(Value(root, "name"), "main", StringComparison.OrdinalIgnoreCase)) continue;
                    string fnt = Value(root, "binary_data");
                    ZipArchiveEntry fe;
                    if (fnt != null && entries.TryGetValue(("font/" + fnt).ToLowerInvariant(), out fe))
                    {
                        try { if (BmFont.ReadChars(Read(fe)).Contains(0xAC00)) continue; }
                        catch (InvalidDataException) { }
                    }
                    int size;
                    if (!int.TryParse(Value(root, "size"), out size)) continue;
                    string stem = Path.GetFileNameWithoutExtension(e.FullName);
                    bool bold = Value(root, "style") is string st && st != "0" || Regex.IsMatch(stem, @"\d+b$");
                    string target = TargetFor(size, bold);
                    var m = Binary.Match(text);
                    if (!m.Success) continue;
                    string patched = text.Substring(0, m.Groups[2].Index) + target + text.Substring(m.Groups[2].Index + m.Groups[2].Length);
                    list.Add(new FontOverride { FontPath = "interface/" + e.FullName.Replace('\\', '/'), Text = patched, TargetFnt = target });
                }
            }
            return list;
        }

        public static IEnumerable<string> TargetFnts(List<FontOverride> list) { return list.Select(f => f.TargetFnt).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.Ordinal); }
    }
}
