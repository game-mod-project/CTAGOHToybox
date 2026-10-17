using System.Linq;
using CtaToybox.Core.L10n;

// 한글화 Task 1: .po 모형, 읽기와 쓰기
static class L10nPoTests
{
    public static void Run()
    {
        string src = "#, fuzzy\nmsgid \"\"\nmsgstr \"\"\n\"Language: en\\n\"\n\"Content-Type: text/plain; charset=UTF-8\\n\"\n\n" +
                     "msgctxt \"a\"\nmsgid \"Hello \\\"x\\\"\\nworld\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"b\"\nmsgid \"\"\n\"Long \"\n\"text\"\nmsgstr \"\"\n\n" +
                     "#, boost-format\nmsgid \"no context\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"a\"\nmsgid \"again\"\nmsgstr \"\"\n";
        var po = PoFile.Parse(src);
        T.Eq(4, po.Entries.Count, "po_duplicate_and_no_context: all entries kept in order");
        T.Eq("Hello \\\"x\\\"\\nworld", po.Entries[0].Id, "escapes stay as written");
        T.Eq("Long text", po.Entries[1].Id, "po_multiline_roundtrip: continuation lines join");
        T.Eq(null, po.Entries[2].Context, "entry without msgctxt");
        T.Eq("\u0004no context", po.Entries[2].Key, "key falls back to msgid");
        T.Eq(new[] { "#, boost-format" }, po.Entries[2].Comments.ToArray(), "other comments kept");
        T.Eq("a", po.Entries[3].Context, "duplicate context kept as its own entry");
        T.True(po.HeaderFuzzy, "header fuzzy flag");
        var again = PoFile.Parse(po.Write());
        T.Eq(po.Entries.Select(e => e.Key + "=" + e.Id).ToArray(), again.Entries.Select(e => e.Key + "=" + e.Id).ToArray(), "po_multiline_roundtrip: write then parse is the same");
        T.Eq(po.Header, again.Header, "header round trips");
        T.True(PoFile.HeaderWithLanguage(po.Header, "ko").Contains("Language: ko\\n") && !PoFile.HeaderWithLanguage(po.Header, "ko").Contains("Language: en"), "language line replaced");
        T.True(PoFile.HeaderWithLanguage("Content-Type: x\\n", "ko").EndsWith("Language: ko\\n"), "language line added");

        string fz = "msgid \"\"\nmsgstr \"\"\n\n#, fuzzy\n#| msgid \"Old\"\nmsgctxt \"k\"\nmsgid \"New\"\nmsgstr \"번역\"\n";
        var f = PoFile.Parse(fz);
        T.True(f.Entries[0].Fuzzy, "fuzzy flag read");
        T.Eq("Old", f.Entries[0].PreviousId, "previous msgid read");
        T.Eq("번역", f.Entries[0].Str, "msgstr read");
        T.Eq(fz, f.Write(), "fuzzy entry writes back byte for byte");
        T.Eq(2, PoFile.Parse("msgctxt \"x\"\nmsgid \"a\"\nmsgstr \"\"\nmsgctxt \"y\"\nmsgid \"b\"\nmsgstr \"\"\n").Entries.Count, "entries without a blank line between");
        T.Eq(null, PoFile.Parse("msgctxt \"x\"\nmsgid \"a\"\nmsgstr \"\"\n").Header, "no header");
    }
}
