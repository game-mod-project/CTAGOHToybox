using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core.L10n;

// 한글화 Task 3: 번역 보관 갱신(extract)
static class L10nCatalogTests
{
    public static void Run()
    {
        var src = PoFile.Parse("msgid \"\"\nmsgstr \"\"\n\"Language: en\\n\"\n\nmsgctxt \"k1\"\nmsgid \"Tank\"\nmsgstr \"\"\n\nmsgctxt \"k2\"\nmsgid \"Attack now\"\nmsgstr \"\"\n\nmsgctxt \"k4\"\nmsgid \"New line\"\nmsgstr \"\"\n");
        var old = PoFile.Parse("msgid \"\"\nmsgstr \"\"\n\nmsgctxt \"k1\"\nmsgid \"Tank\"\nmsgstr \"전차\"\n\nmsgctxt \"k2\"\nmsgid \"Attack\"\nmsgstr \"공격\"\n\nmsgctxt \"k3\"\nmsgid \"Gone\"\nmsgstr \"사라짐\"\n");
        int a, c, r;
        var m = L10nCatalog.Merge(src, old, out a, out c, out r);
        T.Eq(new[] { "k1", "k2", "k4" }, m.Entries.Select(e => e.Context).ToArray(), "source order, removed key dropped");
        T.Eq("전차", m.Entries[0].Str, "unchanged keeps translation");
        T.True(!m.Entries[0].Fuzzy, "unchanged not fuzzy");
        T.True(m.Entries[1].Fuzzy, "extract_changed_marks_fuzzy");
        T.Eq("Attack", m.Entries[1].PreviousId, "previous source kept");
        T.Eq("Attack now", m.Entries[1].Id, "new source in msgid");
        T.Eq("공격", m.Entries[1].Str, "old translation kept for reuse");
        T.Eq("", m.Entries[2].Str, "new key untranslated");
        T.True(m.Header.Contains("Language: ko\\n"), "header language ko");
        T.True(a == 1 && c == 1 && r == 1, "counts added/changed/removed");
        // 같은 키가 두 번: 몇 번째로 짝짓는다
        var dup = PoFile.Parse("msgctxt \"d\"\nmsgid \"one\"\nmsgstr \"\"\n\nmsgctxt \"d\"\nmsgid \"two\"\nmsgstr \"\"\n");
        var dupOld = PoFile.Parse("msgctxt \"d\"\nmsgid \"one\"\nmsgstr \"하나\"\n\nmsgctxt \"d\"\nmsgid \"two\"\nmsgstr \"둘\"\n");
        T.Eq(new[] { "하나", "둘" }, L10nCatalog.Merge(dup, dupOld).Entries.Select(e => e.Str).ToArray(), "duplicate keys paired by position");

        string dir = T.TempDir();
        var sources = new List<L10nSource>
        {
            new L10nSource { Path = "interface/text/a.pot", Layer = "vanilla", Text = L10nSourcesTests.Pot },
            new L10nSource { Path = "interface/text/desc/b.pot", Layer = "mod_1", Text = L10nSourcesTests.Pot },
        };
        var first = L10nCatalog.Extract(sources, dir, "1.0");
        string aPo = Path.Combine(dir, "interface", "text", "a.po");
        T.True(File.Exists(aPo) && File.Exists(Path.Combine(dir, "interface", "text", "desc", "b.po")), "po files made");
        T.True(File.ReadAllText(aPo).StartsWith("# source: vanilla interface/text/a.pot\n# game-version: 1.0\nmsgid \"\""), "source and version header");
        T.True(first.Files == 2 && first.Entries == 2 && first.Untranslated == 2 && first.Added == 2, "first report");
        File.WriteAllText(aPo, File.ReadAllText(aPo).Replace("msgid \"Tank\"\nmsgstr \"\"", "msgid \"Tank\"\nmsgstr \"전차\""));
        byte[] before = File.ReadAllBytes(aPo);
        var second = L10nCatalog.Extract(sources, dir, "1.0");
        T.True(second.Added == 0 && second.Changed == 0 && second.Removed == 0 && second.Untranslated == 1, "extract_is_idempotent: report");
        T.Eq(System.Convert.ToBase64String(before), System.Convert.ToBase64String(File.ReadAllBytes(aPo)), "extract_is_idempotent: bytes");
        File.WriteAllText(Path.Combine(dir, "glossary.psv"), "Tank|Tank|\n");
        var third = L10nCatalog.Extract(sources.Take(1).ToList(), dir, "");
        T.True(!File.Exists(Path.Combine(dir, "interface", "text", "desc", "b.po")) && third.RemovedFiles.Contains("interface/text/desc/b.po"), "a po whose source is gone is removed");
        T.True(File.Exists(Path.Combine(dir, "glossary.psv")), "glossary untouched");
        T.True(!File.ReadAllText(aPo).Contains("game-version"), "no version line when empty");
    }
}
