using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CtaToybox.Core;

static class IncludeChainTests
{
    static SdlIncludeEdge Edge(string from, string to) { return new SdlIncludeEdge { FromFile = from, ToFile = to, Index = 0, Raw = "\"x\"" }; }

    public static void Run()
    {
        // u → a → f,  u → f (두 경로),  u → g,  a → h,  p(.presets)에서 나온 include → q
        var edges = new List<SdlIncludeEdge>
        {
            Edge("u.def", "d/a.inc"), Edge("d/a.inc", "d/f.inc"), Edge("u.def", "d/f.inc"),
            Edge("u.def", "d/g.inc"), Edge("d/a.inc", "d/h.inc"), Edge("d/.presets", "d/q.inc"),
        };

        T.Eq(new[] { "d/a.inc", "d/f.inc", "d/g.inc", "d/h.inc" }, IncludeChain.Reachable(edges, "u.def").OrderBy(x => x).ToArray(), "reachable from the unit");
        T.Eq(0, IncludeChain.Reachable(edges, "d/g.inc").Count, "a leaf reaches nothing");

        T.Eq(new[] { "d/a.inc", "d/f.inc" }, IncludeChain.CopySet(edges, "u.def", "d/f.inc").OrderBy(x => x).ToArray(), "every file on any path to the target, and the target");
        T.Eq(new[] { "d/g.inc" }, IncludeChain.CopySet(edges, "u.def", "d/g.inc").ToArray(), "a directly included target is copied alone");
        T.Eq(new[] { "d/a.inc", "d/h.inc" }, IncludeChain.CopySet(edges, "u.def", "d/h.inc").OrderBy(x => x).ToArray(), "siblings off the path are not copied");
        T.Eq(0, IncludeChain.CopySet(edges, "u.def", "d/q.inc").Count, "a file reached only through .presets is not in the chain");
        T.Eq(0, IncludeChain.CopySet(edges, "u.def", "d/none.inc").Count, "an unknown file is not in the chain");

        var cycle = new List<SdlIncludeEdge> { Edge("u.def", "d/a.inc"), Edge("d/a.inc", "u.def") };
        T.Eq(new[] { "d/a.inc" }, IncludeChain.CopySet(cycle, "u.def", "d/a.inc").ToArray(), "the unit itself is never copied, even in a cycle");

        string copy = IncludeChain.CopyName("entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc", "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def");
        T.True(Regex.IsMatch(copy, @"^entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack__ctb_m3a1_halftrack_[0-9a-f]{8}\.inc$"), "copy name: same folder, same extension, unit name and hash: " + copy);
        T.Eq(copy, IncludeChain.CopyName("entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc", "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def"), "copy names are stable");
        T.True(IncludeChain.CopyName("properties/tank.ext", "entity/a/x.def") != IncludeChain.CopyName("properties/tank.ext", "entity/b/x.def"), "units with the same file name in different folders get different copies");
        T.True(Regex.IsMatch(IncludeChain.CopyName("properties/tank.ext", "set/breed/mp/ger-a b.set"), @"^properties/tank__ctb_ger_a_b_[0-9a-f]{8}\.ext$"), "characters outside a-z, 0-9 and _ become _");
        T.True(Regex.IsMatch(IncludeChain.CopyName("set/stuff/gun/common", "set/stuff/gun/85mm_d5t"), @"^set/stuff/gun/common__ctb_85mm_d5t_[0-9a-f]{8}$"), "files without an extension stay without one");

        T.Eq("\"kv85__ctb_x_0a1b2c3d.ext\"", IncludeChain.RewriteRaw("\"kv85.ext\"", "entity/v/kv85/kv85__ctb_x_0a1b2c3d.ext"), "a same-folder include keeps its form");
        T.Eq("\"/properties/tank__ctb_x_0a1b2c3d.ext\"", IncludeChain.RewriteRaw("\"/properties/tank.ext\"", "properties/tank__ctb_x_0a1b2c3d.ext"), "a root-relative include keeps its folder part");
        T.Eq("\"../base/base__ctb_x_0a1b2c3d.inc\"", IncludeChain.RewriteRaw("\"../base/base.inc\"", "entity/v/base/base__ctb_x_0a1b2c3d.inc"), "a parent-relative include keeps its folder part");
        T.Eq("\"..\\base\\base__ctb_x_0a1b2c3d.inc\"", IncludeChain.RewriteRaw("\"..\\base\\base.inc\"", "entity/v/base/base__ctb_x_0a1b2c3d.inc"), "backslashes are kept");

        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "u.def", "/a/(define \"m\")/k") == null, "the unit's own file is always possible");
        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "d/f.inc", "/box/item") == null, "an included file is possible");
        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "d/f.inc", "/(define \"m\")/k") != null, "a value inside a macro body is not");
        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "d/q.inc", "/box/item") != null, "a file outside the include chain is not");
    }
}
