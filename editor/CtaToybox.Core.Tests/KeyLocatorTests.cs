using System.Linq;
using CtaToybox.Core;

static class KeyLocatorTests
{
    static string[] Leaves(string text, string path, int occurrence)
    {
        var row = KeyLocator.Rows(SdlCore.Parse(text).Root).FirstOrDefault(r => r.Path == path && r.Occurrence == occurrence);
        return row == null ? null : row.Leaves.Select(l => l.Text).ToArray();
    }

    public static void Run()
    {
        string text = "{game_entity\n  {mass 46000}\n  {volume \"body\" {thickness 45 30}}\n  {volume \"turret\" {thickness 100}}\n" +
                      "  {Extender \"inventory\" {box {item \"ap\" 60}}}\n" +
                      "  (\"mobility\" speed(48) reverse((* 2 10)) off({sensor 0}))\n  (include \"a.ext\" scale(1.5))\n  (define \"m\" {k 7})\n}";
        var root = SdlCore.Parse(text).Root;

        T.Eq(new[] { "46000" }, Leaves(text, "/game_entity/mass", 0), "a plain block value");
        T.Eq(new[] { "\"body\"" }, Leaves(text, "/game_entity/volume", 0), "first occurrence of a repeated path");
        T.Eq(new[] { "\"turret\"" }, Leaves(text, "/game_entity/volume", 1), "second occurrence of a repeated path");
        T.Eq(new[] { "45", "30" }, Leaves(text, "/game_entity/volume/thickness", 0), "two values in one row");
        T.Eq(new[] { "100" }, Leaves(text, "/game_entity/volume/thickness", 1), "occurrences are counted per path across parents");
        T.Eq(new[] { "\"ap\"", "60" }, Leaves(text, "/game_entity/extender[inventory]/box/item", 0), "qualified extender path, lower-cased");
        T.Eq(new[] { "48", "2", "10" }, Leaves(text, "/game_entity/(\"mobility\")", 0), "macro call: named args and expression operands, not names or operators");
        T.Eq(new[] { "0" }, Leaves(text, "/game_entity/(\"mobility\")/off()/sensor", 0), "a block inside a macro argument is its own row");
        T.Eq(new[] { "7" }, Leaves(text, "/game_entity/(define \"m\")/k", 0), "a block inside a macro definition");
        T.True(Leaves(text, "/game_entity/(include)", 0) == null, "include rows are not editable rows");
        T.True(Leaves(text, "/game_entity/(define \"m\")", 0) == null, "define rows are not editable rows");

        var node = KeyLocator.Find(root, "/game_entity/volume/thickness", 0, 1);
        T.Eq("30", node.Text, "Find returns the token");
        T.Eq("30", text.Substring(node.Index, node.Text.Length), "the token's Index points into the source text");
        T.True(KeyLocator.Find(root, "/game_entity/volume/thickness", 0, 2) == null, "a value index past the end is null");
        T.True(KeyLocator.Find(root, "/game_entity/volume/thickness", 2, 0) == null, "an occurrence past the end is null");
        T.True(KeyLocator.Find(root, "/game_entity/nope", 0, 0) == null, "an unknown path is null");
        T.Eq("46000", KeyLocator.Find(root, "/GAME_ENTITY/Mass", 0, 0).Text, "paths compare case-insensitively");
    }
}
