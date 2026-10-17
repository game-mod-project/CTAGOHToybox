using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core.L10n;

// 한글화 Task 4: 용어집과 검사(check)
static class L10nCheckTests
{
    // BMFont 바이너리 v3: 블록 1(info, 빈 자리)과 블록 4(chars)만
    public static byte[] Fnt(params int[] chars)
    {
        var b = new List<byte> { (byte)'B', (byte)'M', (byte)'F', 3, 1 };
        b.AddRange(BitConverter.GetBytes(2)); b.Add(0); b.Add(0);
        b.Add(4); b.AddRange(BitConverter.GetBytes(chars.Length * 20));
        foreach (int c in chars) { b.AddRange(BitConverter.GetBytes((uint)c)); b.AddRange(new byte[16]); }
        return b.ToArray();
    }

    public static void Run()
    {
        var glyphs = new HashSet<int>("전차를 점령하라 %s<>()cF0/중".Select(c => (int)c)) { ' ' };
        var terms = new List<GlossaryTerm> { new GlossaryTerm { Source = "Panzer IV", Target = "Panzer IV" }, new GlossaryTerm { Source = "Squad", Target = "분대" } };
        PoEntry E(string id, string str) => new PoEntry { Context = "k", Id = id, Str = str, Comments = new List<string>() };
        T.Eq(0, L10nCheck.Entry("f", E("<c(F00)>Capture</c> %s", "<c(F00)>점령하라</c> %s"), terms, glyphs).Count, "clean entry");
        T.Eq("tags", L10nCheck.Entry("f", E("<c(F00)>Capture</c>", "점령하라"), terms, glyphs)[0].Kind, "missing tag");
        T.Eq("placeholders", L10nCheck.Entry("f", E("%s of %d", "%d 중 %s"), terms, null)[0].Kind, "placeholder order matters");
        T.Eq("newlines", L10nCheck.Entry("f", E("a\\nb", "가나"), terms, null)[0].Kind, "newline count");
        T.Eq("braces", L10nCheck.Entry("f", E("{x} go", "가"), terms, null)[0].Kind, "braces");
        T.Eq("keep-term", L10nCheck.Entry("f", E("Panzer IV tank", "4호 전차"), terms, null)[0].Kind, "keep term must stay");
        T.Eq(0, L10nCheck.Entry("f", E("Squad", "분대"), terms, null).Count, "a translated term is not a keep term");
        var g = L10nCheck.Entry("f", E("Tank", "전차🙂"), terms, glyphs);
        T.Eq("glyph", g[0].Kind, "check_missing_glyph");
        T.True(g[0].Detail.Contains("U+1F642"), "glyph detail names the code point");
        T.Eq(0, L10nCheck.Entry("f", E("<c(F00)>a</c>\\n", "<c(F00)>전</c>\\n"), terms, glyphs).Count, "tags and escapes are not glyphs");
        T.Eq("untranslated", L10nCheck.Entry("f", E("Tank", ""), terms, glyphs)[0].Kind, "empty is untranslated notice");
        T.True(!L10nCheck.Entry("f", E("Tank", ""), terms, glyphs)[0].IsError, "untranslated is a notice");
        var fz = E("Tank", "전차"); fz.Fuzzy = true;
        T.True(L10nCheck.Entry("f", fz, terms, glyphs).Any(i => i.Kind == "fuzzy" && !i.IsError), "fuzzy notice");

        T.Eq(new HashSet<int> { 65, 0xAC00 }.OrderBy(x => x).ToArray(), BmFont.ReadChars(Fnt(65, 0xAC00)).OrderBy(x => x).ToArray(), "bmfont chars read");
        T.Throws<InvalidDataException>(() => BmFont.ReadChars(new byte[] { 1, 2, 3, 4 }), "not a bmfont");

        string dir = T.TempDir();
        string psv = Path.Combine(dir, "glossary.psv");
        File.WriteAllText(psv, "# 머리\nPanzer IV|Panzer IV|장비\n\nGrossdeutschland|그로스도이칠란트|부대\n");
        var loaded = Glossary.Load(psv);
        T.True(loaded.Count == 2 && loaded[0].Keep && !loaded[1].Keep && loaded[1].Note == "부대", "glossary loaded");
        T.Eq(0, Glossary.Load(Path.Combine(dir, "none.psv")).Count, "missing glossary is empty");
        T.Put(dir, "interface/text/a.po", "msgctxt \"k\"\nmsgid \"<c(F00)>x</c>\"\nmsgstr \"가\"\n\nmsgctxt \"k2\"\nmsgid \"y\"\nmsgstr \"\"\n");
        var cat = L10nCheck.Catalog(dir, loaded, null);
        T.True(cat.Count(i => i.IsError) == 1 && cat.Any(i => i.Kind == "untranslated" && i.Key == "k2" && i.File == "interface/text/a.po"), "catalog check");
        T.Eq(0, L10nCheck.Catalog(dir, loaded, null, "nope").Count, "file filter");
    }
}
