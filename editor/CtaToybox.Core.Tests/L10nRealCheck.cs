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
    }
}
