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

        // 행 색인(W-E): 파일 하나의 행을 한 번 만들어 (경로, 몇 번째)로 찾는다. Find와 같은 노드를 돌려준다
        var index = new KeyIndex(root);
        foreach (var row in KeyLocator.Rows(root))
            for (int arg = 0; arg <= row.Leaves.Count; arg++)
            {
                // 행 목록을 처음부터 훑어 찾는 정의(색인 전의 Find)와 같은 노드여야 한다
                var linear = KeyLocator.Rows(root).First(r => r.Occurrence == row.Occurrence && string.Equals(r.Path, row.Path, System.StringComparison.OrdinalIgnoreCase));
                var expected = arg < linear.Leaves.Count ? linear.Leaves[arg] : null;
                T.True(ReferenceEquals(expected, index.Find(row.Path, row.Occurrence, arg)) && ReferenceEquals(expected, KeyLocator.Find(root, row.Path, row.Occurrence, arg)),
                    "the index and Find give the node a scan of the rows gives: " + row.Path + " [" + row.Occurrence + "] " + arg);
            }
        T.Eq("100", index.Find("/game_entity/volume/thickness", 1, 0).Text, "the second occurrence");
        T.Eq("46000", index.Find("/GAME_ENTITY/Mass", 0, 0).Text, "the index compares paths case-insensitively");
        T.True(index.Find("/game_entity/volume/thickness", 0, 2) == null, "index: a value index past the end is null");
        T.True(index.Find("/game_entity/volume/thickness", 0, -1) == null, "index: a negative value index is null");
        T.True(index.Find("/game_entity/volume/thickness", 2, 0) == null, "index: an occurrence past the end is null");
        T.True(index.Find("/game_entity/nope", 0, 0) == null, "index: an unknown path is null");
        T.Eq("/game_entity/volume", index.Row("/game_entity/volume", 1).Path, "the row itself");
        T.True(index.Row("/game_entity/volume", 2) == null, "no such row");

        // (mod ...) 조건(E11): 안의 줄은 부모 경로에 실리고, 어느 조건 안인지를 따로 싣는다
        var cond = KeyLocator.Rows(SdlCore.Parse(
            "{weapon {limit 10} (mod \"mp\" {limit 50} (mod not \"coop\" {x 1})) (mod not \"mp\" {limit 200})}").Root);
        T.Eq(new[] { null, null, "(mod \"mp\")", "(mod \"mp\") (mod not \"coop\")", "(mod not \"mp\")" }, cond.Select(r => r.Cond).ToArray(), "each row carries the conditions around it");
        T.Eq(new[] { "/weapon", "/weapon/limit", "/weapon/limit", "/weapon/x", "/weapon/limit" }, cond.Select(r => r.Path).ToArray(), "condition rows keep the parent path");

        // 따옴표 이름 블록(Q7): 원문 경로에는 이름이 있고, 같은 원문 경로 가운데 몇 번째인지를 따로 센다
        var named = KeyLocator.Rows(SdlCore.Parse("{\"tiger\" {cost 1}} {\"panther\" {cost 2}} {\"Tiger\" {cost 3}}").Root);
        T.Eq(new[] { 0, 0, 1, 1, 2, 2 }, named.Select(r => r.Occurrence).ToArray(), "occurrence counts the * path");
        T.Eq(new[] { 0, 0, 0, 0, 1, 1 }, named.Select(r => r.RawOccurrence).ToArray(), "raw occurrence counts the named path, ignoring case");
        var namedIndex = new KeyIndex(SdlCore.Parse("{\"tiger\" {cost 1}} {\"panther\" {cost 2}} {\"Tiger\" {cost 3}}").Root);
        T.Eq("3", namedIndex.FindNamed("/\"tiger\"/cost", 1, 0).Text, "find by the named path and its own occurrence");
        T.True(namedIndex.NamedRow("/\"leopard\"/cost", 0) == null, "an unknown name is null");
    }
}
