using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class TokenPatcherTests
{
    static Edit E(string path, int occ, int arg, string old, string @new)
    {
        return new Edit { Unit = "u.def", File = "u.def", Path = path, Occurrence = occ, Arg = arg, Old = old, New = @new };
    }

    static string Apply(string text, out List<EditResult> results, params Edit[] edits)
    {
        results = new List<EditResult>();
        return TokenPatcher.Apply(text, edits, results);
    }

    public static void Run()
    {
        List<EditResult> r;
        string text = "{game_entity ; top comment\r\n\t{mass   46000}\t; kg\r\n\t{volume \"body\" {thickness 45 30}}\r\n\t{volume \"turret\" {thickness 100}}\r\n}\r\n";

        string one = Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "50000"));
        T.Eq(text.Replace("46000", "50000"), one, "only the value token changes; comments, tabs and CRLF stay");
        T.Eq(EditStatus.Applied, r[0].Status, "applied status");

        // 길이가 다른 수정 여러 건: 앞의 치환이 뒤의 위치를 밀지 않는다
        string many = Apply(text, out r,
            E("/game_entity/mass", 0, 0, "46000", "5"),
            E("/game_entity/volume/thickness", 0, 1, "30", "3000000"),
            E("/game_entity/volume/thickness", 1, 0, "100", "1"),
            E("/game_entity/volume", 1, 0, "\"turret\"", "\"t\""));
        T.Eq(text.Replace("46000", "5").Replace("45 30", "45 3000000").Replace("{thickness 100}", "{thickness 1}").Replace("\"turret\"", "\"t\""), many, "several edits of different lengths in one file");
        T.Eq(4, r.Count(x => x.Status == EditStatus.Applied), "all four applied");

        string same = Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "46000"));
        T.Eq(text, same, "an identity edit leaves the text byte-identical");

        T.Eq(text, Apply(text, out r, E("/game_entity/mass", 0, 0, "40000", "50000")), "old value mismatch changes nothing");
        T.Eq(EditStatus.SourceChanged, r[0].Status, "old value mismatch is SourceChanged");
        T.True(r[0].Message.Contains("46000"), "the message shows the current source value");

        Apply(text, out r, E("/game_entity/nope", 0, 0, "1", "2"), E("/game_entity/mass", 3, 0, "46000", "1"), E("/game_entity/mass", 0, 5, "46000", "1"));
        T.Eq(new[] { EditStatus.TargetMissing, EditStatus.TargetMissing, EditStatus.TargetMissing }, r.Select(x => x.Status).ToArray(), "missing path, occurrence and value index");

        string twice = Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "1"), E("/game_entity/mass", 0, 0, "46000", "2"));
        T.Eq(text.Replace("46000", "1"), twice, "the first of two edits on one token wins");
        T.Eq(EditStatus.Invalid, r[1].Status, "the second edit on the same token is Invalid");

        // 새 값 검사: 구조를 깨는 값은 거부한다
        string w;
        T.True(TokenPatcher.CheckNewValue("46000", "50000", out w) == null && w == null, "a number for a number is fine");
        foreach (string bad in new[] { "", "1 2", "1}", "{1", "(1", "1)", "1 ; x", "1;x", "\"a", "a\"b", "1\n2", "\t1" })
            T.True(TokenPatcher.CheckNewValue("46000", bad, out w) != null, "rejected new value: [" + bad.Replace("\n", "\\n").Replace("\t", "\\t") + "]");
        T.True(TokenPatcher.CheckNewValue("\"body\"", "\"hull plate\"", out w) == null, "a quoted string may contain spaces");
        T.True(TokenPatcher.CheckNewValue("\"body\"", "\"a{b}\"", out w) == null, "a quoted string may contain braces");
        T.True(TokenPatcher.CheckNewValue("\"body\"", "body", out w) != null, "a quoted value cannot become unquoted");
        T.True(TokenPatcher.CheckNewValue("46000", "\"46000\"", out w) != null, "an unquoted value cannot become quoted");
        T.True(TokenPatcher.CheckNewValue("46000", "fast", out w) == null && w != null, "a non-number for a number is a warning, not an error");
        T.True(TokenPatcher.CheckNewValue("fast", "slow", out w) == null && w == null, "a word for a word is fine");

        Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "5 {x}"));
        T.Eq(EditStatus.Invalid, r[0].Status, "Apply refuses an invalid new value");

        var unitScope = E("/game_entity/mass", 0, 0, "46000", "1"); unitScope.Scope = "unit";
        T.True(Apply(text, out r, unitScope) != text && r[0].Status == EditStatus.Applied, "scope is the builder's business; the patcher applies the edit it is given");

        var rawResults = new List<EditResult>();
        string rawText = "{a (include \"x.inc\") {k 1}}";
        var rawPatch = new RawPatch { Index = rawText.IndexOf("\"x.inc\""), Old = "\"x.inc\"", New = "\"x__copy.inc\"" };
        T.Eq("{a (include \"x__copy.inc\") {k 2}}",
            TokenPatcher.Apply(rawText, new[] { new Edit { File = "f", Path = "/a/k", Old = "1", New = "2" } }, rawResults, new[] { rawPatch }),
            "a raw patch and an edit apply together, even though the raw patch shifts the edit's position");
        T.Eq("{a (include \"x__copy.inc\") {k 1}}", TokenPatcher.Apply(rawText, new Edit[0], rawResults, new[] { rawPatch }), "a raw patch applies with no edits");
        T.Throws<InvalidOperationException>(() => TokenPatcher.Apply(rawText, new Edit[0], rawResults, new[] { new RawPatch { Index = 3, Old = "\"x.inc\"", New = "\"y.inc\"" } }), "a raw patch whose Old is not at Index throws");
        T.Throws<InvalidOperationException>(() => TokenPatcher.Apply(rawText, new Edit[0], rawResults, new[] { new RawPatch { Index = rawPatch.Index, Old = rawPatch.Old, New = "\"a\" {b}" } }), "a raw patch whose New is not one string token throws");
        T.Throws<InvalidOperationException>(() => TokenPatcher.Apply("{a (include \"x.inc\")", new Edit[0], rawResults, new[] { new RawPatch { Index = 12, Old = "\"x.inc\"", New = "\"y.inc\"" } }), "a raw patch on a file that does not parse throws");

        var off = E("/game_entity/mass", 0, 0, "46000", "1"); off.Enabled = false;
        T.Eq(text, Apply(text, out r, off), "a disabled edit is skipped");
        T.Eq(0, r.Count, "a disabled edit produces no result");

        Apply("{a {b 1}", out r, E("/a/b", 0, 0, "1", "2"));
        T.Eq(EditStatus.Invalid, r[0].Status, "a source file that does not parse cannot be edited");

        // 0x80 이상의 바이트가 있는 파일: 고친 토큰 밖의 바이트는 그대로다
        byte[] bytes = Encoding.Latin1.GetBytes("{a ; caf\u00E9 \u00A0\u0085\r\n {b 1}}");
        string decoded = TextCodec.Decode(bytes, out TextKind kind);
        byte[] patched = TextCodec.Encode(Apply(decoded, out r, E("/a/b", 0, 0, "1", "22")), kind);
        T.Eq(Encoding.Latin1.GetBytes("{a ; caf\u00E9 \u00A0\u0085\r\n {b 22}}"), patched, "high bytes outside the edited token survive");

        // 프로젝트 파일
        string path = Path.Combine(T.TempDir(), "p.json");
        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("/game_entity/volume", 1, 0, "\"turret\"", "\"t\""));
        project.Save(path);
        var loaded = EditProject.Load(path);
        T.Eq("\"turret\"", loaded.Edits[0].Old, "quotes survive the project file");
        T.Eq("shared", loaded.Edits[0].Scope, "scope defaults to shared");
        T.True(loaded.Edits[0].Enabled, "enabled defaults to true");
        T.True(File.ReadAllText(path).Contains("\"occurrence\": 1"), "the project file uses camelCase names");
    }
}
