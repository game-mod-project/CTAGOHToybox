using System.IO;
using System.IO.Compression;
using System.Linq;
using CtaToybox.Core.L10n;

// 한글화 Task 5: 글꼴 대응
static class L10nFontTests
{
    static void Put(ZipArchive z, string name, byte[] bytes) { using (var s = z.CreateEntry(name).Open()) s.Write(bytes, 0, bytes.Length); }

    public static void Run()
    {
        T.Eq("main_12.fnt", FontMap.TargetFor(13, false), "13 -> 12");
        T.Eq("main_15.fnt", FontMap.TargetFor(14, false), "14 -> 15");
        T.Eq("main_30.fnt", FontMap.TargetFor(42, false), "42 -> 30");
        T.Eq("main_30.fnt", FontMap.TargetFor(60, false), "60 -> 30");
        T.Eq("main_12b.fnt", FontMap.TargetFor(12, true), "bold kept when main has it");
        T.Eq("main_15.fnt", FontMap.TargetFor(15, true), "bold falls back to regular");
        T.Eq("main_10.fnt", FontMap.TargetFor(11, false), "tie between 10 and 12 goes smaller");

        string pak = Path.Combine(T.TempDir(), "interface.pak");
        using (var z = ZipFile.Open(pak, ZipArchiveMode.Create))
        {
            Put(z, "font/roboto_con_15.font", System.Text.Encoding.UTF8.GetBytes("{font_atlas\n\t{name \"roboto_con\"}\n\t{size 15}\n\t{height 21}\n\t{style 0}\n\t{binary_data \"roboto_con_15.fnt\"}\n}\n"));
            Put(z, "font/roboto_con_15.fnt", L10nCheckTests.Fnt(65));
            Put(z, "font/main_15.font", System.Text.Encoding.UTF8.GetBytes("{font_atlas\n\t{name \"main\"}\n\t{size 15}\n\t{height 21}\n\t{style 0}\n\t{binary_data \"main_15.fnt\"}\n}\n"));
            Put(z, "font/main_15.fnt", L10nCheckTests.Fnt(65, 0xAC00));
            Put(z, "font/odd_12.font", System.Text.Encoding.UTF8.GetBytes("{font_atlas\n\t{name \"odd\"}\n\t{size 12}\n\t{height 20}\n\t{style 0}\n\t{binary_data \"odd_12.fnt\"}\n}\n"));
            Put(z, "font/odd_12.fnt", L10nCheckTests.Fnt(0xAC00));
            Put(z, "font/myriadpro_12b.font", System.Text.Encoding.UTF8.GetBytes("{font_atlas\n\t{name \"myriadpro\"}\n\t{size 12}\n\t{height 20}\n\t{style 1}\n\t{binary_data \"myriadpro_12b.fnt\"}\n}\n"));
            Put(z, "font/myriadpro_12b.fnt", L10nCheckTests.Fnt(65));
        }
        var list = FontMap.Build(pak);
        T.Eq(new[] { "interface/font/myriadpro_12b.font", "interface/font/roboto_con_15.font" }, list.Select(f => f.FontPath).ToArray(), "only hangul-less non-main fonts");
        var rc = list.Single(f => f.FontPath.EndsWith("roboto_con_15.font"));
        T.True(rc.Text.Contains("{binary_data \"main_15.fnt\"}") && rc.Text.Contains("{name \"roboto_con\"}") && rc.Text.Contains("{height 21}"), "binary_data swapped, rest kept: " + rc.Text);
        T.Eq("main_12b.fnt", list.Single(f => f.FontPath.EndsWith("myriadpro_12b.font")).TargetFnt, "bold to bold");
        T.Eq(new[] { "main_12b.fnt", "main_15.fnt" }, FontMap.TargetFnts(list).ToArray(), "targets");
    }
}
