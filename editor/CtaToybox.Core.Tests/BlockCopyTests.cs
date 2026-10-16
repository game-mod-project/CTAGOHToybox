using System.Collections.Generic;
using System.Linq;
using CtaToybox.Core;

// E18: 게임 설정 파일의 따옴표 이름 블록 복제(텍스트 규칙)
static class BlockCopyTests
{
    const string F = "set/vision/vision_fields.inc";

    public static Edit Copy(string from, string to, string parent = "", int occurrence = 0)
    {
        return new Edit { Kind = BlockCopy.Kind, Unit = "set/vision/vision.set", File = F, Path = parent + "/\"*\"", Occurrence = occurrence, Arg = 0,
            NamedPath = parent + "/\"" + from + "\"", Old = "\"" + from + "\"", New = "\"" + to + "\"", Scope = "shared" };
    }

    static List<KeyRow> Rows(string text) { string error; return SourceRows.Of(F, text, out error); }

    public static void Run()
    {
        string text = "{\"a\"\n {uncover 20}\n (\"spotting2\" r(1.5))\n}\n{\"b\"\n {uncover 10}\n}\n";
        var results = new List<EditResult>();
        string copied = BlockCopy.Apply(text, new[] { Copy("a", "c") }, results);
        var rows = Rows(copied);
        T.Eq(new[] { EditStatus.Applied }, results.Select(r => r.Status).ToArray(), "one copy applied");
        T.Eq("20", rows.Single(r => r.RawPath == "/\"c\"/uncover").Leaves[0].Text, "the copy keeps the values");
        T.Eq("1.5", rows.Single(r => r.RawPath == "/\"c\"/(\"spotting2\")").Leaves[0].Text, "and the macro call");
        int b = rows.FindIndex(r => r.RawPath == "/\"b\""), c = rows.FindIndex(r => r.RawPath == "/\"c\"");
        T.True(b >= 0 && c > b, "the copy goes after the last sibling");
        T.Eq(new[] { 0, 1 }, rows.Where(r => r.Path == "/\"*\"/uncover" && r.RawPath != "/\"c\"/uncover").Select(r => r.Occurrence).ToArray(), "existing occurrences do not move");
        T.True(text.StartsWith(copied.Substring(0, text.Length - 1)), "the original text stays in front");

        string nested = "{visions\n {\"a\" {x 1}}\n {\"b\" {x 2}}\n}\n{other {y 3}}\n";
        results.Clear();
        var nr = Rows(BlockCopy.Apply(nested, new[] { Copy("a", "c", "/visions") }, results));
        T.Eq("1", nr.Single(r => r.RawPath == "/visions/\"c\"/x").Leaves[0].Text, "a nested block is copied inside its parent");
        T.True(nr.FindIndex(r => r.RawPath == "/visions/\"c\"") < nr.FindIndex(r => r.RawPath == "/other"), "before the parent closes");

        results.Clear();
        BlockCopy.Apply(text, new[] { Copy("a", "b") }, results);
        T.Eq(EditStatus.Invalid, results[0].Status, "an existing name is refused");
        results.Clear();
        BlockCopy.Apply(text, new[] { Copy("a", "B") }, results);
        T.Eq(EditStatus.Invalid, results[0].Status, "ignoring case");
        results.Clear();
        BlockCopy.Apply(text, new[] { Copy("a", "c"), Copy("b", "c") }, results);
        T.Eq(new[] { EditStatus.Applied, EditStatus.Invalid }, results.Select(r => r.Status).ToArray(), "a second copy to the same new name is refused");
        results.Clear();
        BlockCopy.Apply(text, new[] { Copy("zz", "c") }, results);
        T.Eq(EditStatus.TargetMissing, results[0].Status, "a missing source block");
        results.Clear();
        var changed = Copy("a", "c");
        changed.Old = "\"x\"";
        BlockCopy.Apply(text, new[] { changed }, results);
        T.Eq(EditStatus.SourceChanged, results[0].Status, "a source block whose name changed");
        results.Clear();
        var off = Copy("a", "c");
        off.Enabled = false;
        T.Eq(text, BlockCopy.Apply(text, new[] { off }, results), "a disabled copy is ignored");
        T.Eq(0, results.Count, "and reports nothing");
        T.Eq(copied, BlockCopy.ApplyAll(text, new[] { Copy("a", "c") }), "ApplyAll gives the same text");

        T.True(BlockCopy.CheckName("\"ok_1\"") == null, "a plain quoted name");
        foreach (string bad in new[] { "ok", "\"a b\"", "\"a/b\"", "\"\"", "\"a\"b\"" })
            T.True(BlockCopy.CheckName(bad) != null, "a bad name is refused: " + bad);

        var copies = new[] { Copy("a", "c") };
        T.Eq("c", BlockCopy.Dependency(new Edit { NamedPath = "/\"C\"/uncover", New = "5" }, copies), "a value inside the copy");
        T.Eq("c", BlockCopy.Dependency(new Edit { New = "\"c\"" }, copies), "a value that names the copy");
        T.True(BlockCopy.Dependency(new Edit { New = "\"c\"", Kind = MacroSwap.Kind }, copies) == null, "a macro rename is not a dependency");
        T.True(BlockCopy.Dependency(new Edit { NamedPath = "/\"a\"/uncover", New = "5" }, copies) == null, "an unrelated edit");
        T.True(BlockCopy.Dependency(copies[0], copies) == null, "the copy itself");
    }
}
