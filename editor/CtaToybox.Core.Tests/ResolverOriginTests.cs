using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class ResolverOriginTests
{
    static readonly Dictionary<string, string> Files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["entity/v/.presets"] = "(define \"mob\" {speed %speed} {mass 10})",
        ["entity/v/a/a.def"] = "{game_entity\n (include \"a.inc\" scale(1.5))\n (include \"/properties/tank.ext\")\n (\"mob\" speed(40))\n {half (/ 10 4)}\n}",
        ["entity/v/a/a.inc"] = "{box {item \"mine\" 24}}\n{size %scale}\n{name \"x_%scale\"}\n(include \"deep.inc\")",
        ["entity/v/a/deep.inc"] = "{deep 1}",
        ["properties/tank.ext"] = "{armor 30}",
    };

    static SdlNode Leaf(SdlResolveResult r, string path, int arg)
    {
        var root = new SdlNode("root", "", 0, true);
        root.Children.AddRange(r.Nodes);
        return KeyLocator.Rows(root).First(x => x.Path == path).Leaves[arg];
    }

    static void Origin(SdlResolveResult r, string path, int arg, string file, string text, string name)
    {
        var leaf = Leaf(r, path, arg);
        T.Eq(file, leaf.File, name + ": file");
        T.Eq(false, leaf.Synthetic, name + ": not synthetic");
        T.Eq(text, Files[file].Substring(leaf.Index, text.Length), name + ": Index points at the token in that file");
    }

    public static void Run()
    {
        var r = new SdlResolver(vp => Files.TryGetValue(vp, out string t) ? t : null).Resolve("entity/v/a/a.def");
        T.Eq(0, r.Unresolved.Count, "the fixture resolves completely");

        Origin(r, "/game_entity/box/item", 1, "entity/v/a/a.inc", "24", "a value in an included file");
        Origin(r, "/game_entity/deep", 0, "entity/v/a/deep.inc", "1", "a value two includes deep");
        Origin(r, "/game_entity/armor", 0, "properties/tank.ext", "30", "a value from a root-relative include");
        Origin(r, "/game_entity/size", 0, "entity/v/a/a.def", "1.5", "an include argument comes from the including file");
        Origin(r, "/game_entity/speed", 0, "entity/v/a/a.def", "40", "a macro argument comes from the calling file");
        Origin(r, "/game_entity/mass", 0, "entity/v/.presets", "10", "a value fixed in a macro body comes from the defining file");

        var half = Leaf(r, "/game_entity/half", 0);
        T.Eq("2.5", half.Text, "arithmetic is evaluated");
        T.Eq(true, half.Synthetic, "a computed value is synthetic");
        var name = Leaf(r, "/game_entity/name", 0);
        T.Eq("\"x_1.5\"", name.Text, "a partial substitution is expanded");
        T.Eq(true, name.Synthetic, "a partially substituted value is synthetic");

        var edges = r.IncludeEdges.Select(e => e.FromFile + " -> " + e.ToFile + " " + e.Raw).ToArray();
        T.Eq(new[]
        {
            "entity/v/a/a.def -> entity/v/a/a.inc \"a.inc\"",
            "entity/v/a/a.inc -> entity/v/a/deep.inc \"deep.inc\"",
            "entity/v/a/a.def -> properties/tank.ext \"/properties/tank.ext\"",
        }, edges, "include edges in expansion order, with the raw path token");
        foreach (var e in r.IncludeEdges)
            T.Eq(e.Raw, Files[e.FromFile].Substring(e.Index, e.Raw.Length), "an edge's Index points at its path token: " + e.Raw);

        // 폴더에서 읽는 기존 생성자도 같은 결과를 낸다
        string dir = T.TempDir();
        foreach (var f in Files) T.Put(dir, f.Key, f.Value);
        var fromDisk = new SdlResolver(new[] { dir }).Resolve("entity/v/a/a.def");
        T.Eq(SdlCore.NodesText(r.Nodes), SdlCore.NodesText(fromDisk.Nodes), "the folder-based constructor expands to the same text");
        T.Eq("entity/v/a/a.inc", Leaf(fromDisk, "/game_entity/box/item", 1).File, "the folder-based constructor stamps files too");
    }
}
