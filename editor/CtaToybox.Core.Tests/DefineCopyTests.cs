using System.Collections.Generic;
using System.Linq;
using CtaToybox.Core;

// 매크로 정의 복제(define-copy): 원본 파일 끝에 이름만 바꾼 정의를 붙인다(텍스트 규칙)
static class DefineCopyTests
{
    const string F = "properties/resupply.inc";
    const string Text = "(define \"a\"\n {x {radius 10} {limit %supply}}\n)\n(define \"b\" {y 1})\n";

    public static void Run()
    {
        var r = new List<EditResult>();
        string one = DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "a", "a_ctb_ger") }, r);
        T.Eq(EditStatus.Applied, r.Single().Status, "a define is copied");
        T.True(one.StartsWith(Text), "the original text stays byte for byte in front: " + one);
        T.True(one.EndsWith("(define \"a_ctb_ger\"\n {x {radius 10} {limit %supply}}\n)\n"), "the copy goes to the end with only the name changed: " + one);
        string error;
        var rows = SourceRows.Of(F, one, out error);
        T.True(rows.Any(x => x.Path == "/(define \"a_ctb_ger\")/x/radius" && x.Leaves[0].Text == "10"), "rows inside the copy have their own path: " + string.Join(", ", rows.Select(x => x.Path)));

        r.Clear(); DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "zz", "zz2") }, r);
        T.Eq(EditStatus.TargetMissing, r.Single().Status, "a missing define is TargetMissing");
        r.Clear(); DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "a", "b") }, r);
        T.Eq(EditStatus.Invalid, r.Single().Status, "a name that already exists is refused");
        r.Clear(); DefineCopy.Apply(Text, new[] { new Edit { Kind = DefineCopy.Kind, File = F, Path = DefineCopy.PathOf("a"), Old = "\"a\"", New = "\"a b\"", Enabled = true } }, r);
        T.Eq(EditStatus.Invalid, r.Single().Status, "a bad new name is refused");
        string two = DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "a", "a2"), DefineCopy.New(F, F, "b", "b2") }, new List<EditResult>());
        T.True(two.IndexOf("(define \"a2\"") > 0 && two.IndexOf("(define \"a2\"") < two.IndexOf("(define \"b2\""), "copies go in edit order: " + two);
        var off = DefineCopy.New(F, F, "a", "a3"); off.Enabled = false;
        T.Eq(Text, DefineCopy.Apply(Text, new[] { off }, new List<EditResult>()), "a disabled copy does nothing");
        T.Eq("/(define \"abc\")", DefineCopy.PathOf("ABC"), "the path names the define in lower case");
        var made = DefineCopy.New(F, F, "a", "a_ctb_ger");
        T.True(made.Kind == DefineCopy.Kind && made.Old == "\"a\"" && made.New == "\"a_ctb_ger\"" && made.Scope == "shared" && made.Path == "/(define \"a\")", "the edit shape");
        T.True(DefineCopy.CheckName("\"ok_name\"") == null && DefineCopy.CheckName("ok") != null && DefineCopy.CheckName("\"a/b\"") != null, "name rule");
        string crlf = DefineCopy.Apply("(define \"a\" {x 1})\r\n", new[] { DefineCopy.New(F, F, "a", "c") }, new List<EditResult>());
        T.Eq("(define \"a\" {x 1})\r\n(define \"c\" {x 1})\r\n", crlf, "a CRLF file gets CRLF");
        string noEnd = DefineCopy.Apply("(define \"a\" {x 1})", new[] { DefineCopy.New(F, F, "a", "c") }, new List<EditResult>());
        T.Eq("(define \"a\" {x 1})\n(define \"c\" {x 1})\n", noEnd, "a file without a final newline gets one before the copy");

        var copies = new List<Edit> { DefineCopy.New(F, F, "a", "a_ctb_ger") };
        T.Eq("a_ctb_ger", DefineCopy.Dependency(new Edit { File = F, Path = "/(define \"a_ctb_ger\")/x/radius" }, copies), "a value inside the copy depends on it");
        T.Eq("a_ctb_ger", DefineCopy.Dependency(new Edit { Kind = MacroSwap.Kind, File = "u.def", Path = "/e/(\"a\")", New = "\"a_ctb_ger\"" }, copies), "a call renamed to the copy depends on it");
        T.Eq(null, DefineCopy.Dependency(new Edit { File = F, Path = "/(define \"a\")/x/radius" }, copies), "the original does not");
        T.Eq(null, DefineCopy.Dependency(copies[0], copies), "the copy itself depends on nothing");
        T.True(TextCopies.MissingNote("a_ctb_ger", copies).Contains("매크로") && TextCopies.MissingNote("tank_x", copies) == BlockCopy.MissingNote("tank_x"), "the note names the kind of copy");

        var project = new EditProject { Edits = copies };
        var overlay = TextCopies.Overlay(project);
        T.True(overlay(F, Text).Contains("(define \"a_ctb_ger\""), "the overlay copies in that file");
        T.Eq(Text, overlay("other.inc", Text), "and leaves other files alone");
        T.True(TextCopies.Overlay(new EditProject()) == null, "no copies, no overlay");
        T.True(TextCopies.IsCopy(BlockCopy.Kind) && TextCopies.IsCopy(DefineCopy.Kind) && !TextCopies.IsCopy(MacroSwap.Kind) && !TextCopies.IsCopy(null), "both copy kinds");
        T.True(TextCopies.ApplyAll(Text, copies).Contains("(define \"a_ctb_ger\""), "ApplyAll applies define copies too");
    }
}
