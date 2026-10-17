using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CtaToybox.Core;
using CtaToybox.Core.L10n;

// 한글화 실제 파일 검사. `-- real`과 `-- real-l10n`이 부른다
static class L10nRealCheck
{
    public static void Run(string settingsPath)
    {
        var s = EditorSettings.Load(settingsPath);
        string pak = Path.Combine(s.GameDir, "localizations", "default.pak");
        int files = 0, entries = 0, bad = 0;
        using (var z = ZipFile.OpenRead(pak))
            foreach (var e in z.Entries.Where(e => e.FullName.EndsWith(".pot", StringComparison.OrdinalIgnoreCase)))
            {
                string text;
                using (var r = new StreamReader(e.Open())) text = r.ReadToEnd();
                var po = PoFile.Parse(text);
                var again = PoFile.Parse(po.Write());
                files++; entries += po.Entries.Count;
                if (!po.Entries.Select(x => x.Key + "\u0001" + x.Id).SequenceEqual(again.Entries.Select(x => x.Key + "\u0001" + x.Id)) || po.Header != again.Header) { bad++; if (bad <= 3) Console.WriteLine("  l10n roundtrip differs: " + e.FullName); }
            }
        Console.WriteLine("l10n-vanilla-pot files=" + files + " entries=" + entries + " roundtrip-bad=" + bad);
        T.True(files > 300 && entries > 20000, "vanilla pot files read");
        T.Eq(0, bad, "every vanilla pot round trips");

        var main12 = L10nCheck.MainGlyphs(s.GameDir, new[] { "main_12.fnt" });
        Console.WriteLine("l10n-main_12 glyphs=" + main12.Count + " hangul=" + main12.Count(c => c >= 0xAC00 && c <= 0xD7A3));
        T.True(main12.Contains(0xAC00) && main12.Contains(0xD7A3), "main_12 has hangul syllables");

        var fonts = FontMap.Build(Path.Combine(s.GameDir, "resource", "interface", "interface.pak"));
        var targets = FontMap.TargetFnts(fonts).ToList();
        var glyphs = L10nCheck.MainGlyphs(s.GameDir, targets);
        Console.WriteLine("l10n-fonts overrides=" + fonts.Count + " targets=" + string.Join(",", targets) + " common-hangul=" + glyphs.Count(c => c >= 0xAC00 && c <= 0xD7A3));
        T.True(fonts.Count > 30 && !fonts.Any(f => f.FontPath.Contains("/main_")) && fonts.Any(f => f.FontPath.EndsWith("roboto_con_15.font")), "font overrides: no main, roboto_con_15 in");
        T.Eq(11172, glyphs.Count(c => c >= 0xAC00 && c <= 0xD7A3), "every target main font has all hangul syllables");
    }
}
