using System.IO;
using System.Linq;
using CtaToybox.Core;

static class UnitIndexTests
{
    public static void Run()
    {
        T.Eq("vehicle", UnitKind.Of("entity/-vehicle/ussr/tank_heavy/kv85/kv85.def"), "a vehicle def");
        T.True(UnitKind.Of("entity/-vehicle/ussr/tank_heavy/kv85/kv85.ext") == null, "a vehicle .ext is not a unit");
        T.Eq("breed", UnitKind.Of("set/breed/mp/ger/early/nco.set"), "a breed");
        T.Eq("humanskin", UnitKind.Of("entity/humanskin/ger/x/x.def"), "a humanskin def");
        T.Eq("item-entity", UnitKind.Of("entity/inventory/helmet/h.def"), "an inventory entity");
        T.Eq("stuff", UnitKind.Of("set/stuff/gun/85mm_d5t"), "a stuff file without extension");
        T.Eq("stuff", UnitKind.Of("set/stuff/shell/bulletger_105.heat.ammo"), "a stuff file with an extension");
        T.Eq("pattern", UnitKind.Of("set/stuff/gun/gun.pattern"), "a pattern");
        T.True(UnitKind.Of("set/stuff/gun/.presets") == null, "a .presets file is not a unit");
        T.True(UnitKind.Of("set/stuff/gun/common.inc") == null, "a stuff .inc is not a unit");
        T.True(UnitKind.Of("set/stuff/_settings/x.set") == null, "the _settings folder is not units");
        T.True(UnitKind.Of("set/multiplayer/units/late/units_rus.set") == null, "other set files are not units");

        string vanilla = T.TempDir(), mod = T.TempDir(), work = T.TempDir();
        T.Zip(Path.Combine(vanilla, "entity", "-vehicle.pak"),
            ("-vehicle/x/.presets", "(define \"m\" {k 1})"),
            ("-vehicle/x/a/a.def", "{game_entity (include \"s.inc\") (include \"/properties/tank.ext\")}"),
            ("-vehicle/x/a/b.def", "{game_entity (include \"s.inc\")}"),
            ("-vehicle/x/a/s.inc", "{box {item \"mine\" 24}}"),
            ("-vehicle/x/c/c.def", "{game_entity (include \"missing.inc\")}"));
        T.Zip(Path.Combine(vanilla, "properties.pak"), ("properties/tank.ext", "{armor 30}"));
        T.Put(mod, "entity/-vehicle/x/a/b.def", "{game_entity {mass 1}}");
        T.Put(mod, "set/breed/mp/n.set", "{breed}");

        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modX", mod) });
        stack.Scan();
        T.Eq("{armor 30}", stack.ReadText("properties/tank.ext"), "ReadText decodes the winning file");
        T.True(stack.ReadText("properties/none.ext") == null, "ReadText of a missing file is null");

        var index = UnitIndex.Build(stack, null);
        T.Eq(new[] { "entity/-vehicle/x/a/a.def", "entity/-vehicle/x/a/b.def", "entity/-vehicle/x/c/c.def", "set/breed/mp/n.set" },
            index.Units.Select(u => u.Path).ToArray(), "units only, sorted by path");
        var a = index.Units[0];
        T.Eq("vehicle", a.Kind, "kind");
        T.Eq("vanilla", a.Layer, "layer of the winning file");
        T.Eq(new[] { "entity/-vehicle/x/a/s.inc", "properties/tank.ext" }, a.Includes.ToArray(), "includes");
        T.Eq(new[] { "entity/-vehicle/x/.presets" }, a.Presets.ToArray(), "presets of ancestor folders");
        T.Eq("modX", index.Units[1].Layer, "an overridden unit reports the overriding layer");
        T.Eq(0, index.Units[1].Includes.Count, "the overriding file's includes are used, not the original's");
        T.Eq(1, index.Units[2].Unresolved, "unresolved includes are counted");

        T.Eq(new[] { "entity/-vehicle/x/a/a.def" }, index.UsersOf("entity/-vehicle/x/a/s.inc").ToArray(), "users of an include: b.def was overridden and no longer includes it");
        T.Eq(3, index.UsersOf("entity/-vehicle/x/.presets").Count, "users of a .presets file");
        T.Eq(new[] { "entity/-vehicle/x/a/a.def" }, index.UsersOf("ENTITY/-vehicle/x/a/a.def").ToArray(), "a unit uses itself; lookup ignores case");
        T.Eq(0, index.UsersOf("properties/unused.ext").Count, "a file nobody reads has no users");

        string csv = Path.Combine(work, "names.csv");
        File.WriteAllText(csv, "\"Context\",\"Text\",\"File\"\r\n\"desc/entity/a\",\"Tank \"\"A\"\"\",\"x.pot\"\r\n\"info/entity/a\",\"line one\nline two\",\"x.pot\"\r\n");
        var names = DisplayNames.Load(csv);
        T.Eq("Tank \"A\"", names.OfUnit("entity/-vehicle/x/a/a.def"), "an entity's name comes from desc/entity/<file name>");
        T.True(names.OfUnit("entity/-vehicle/x/a/b.def") == null, "no key, no name");
        T.True(names.OfUnit("set/breed/mp/n.set") == null, "breeds get no name");
        T.True(DisplayNames.Load(Path.Combine(work, "none.csv")).OfUnit("entity/-vehicle/x/a/a.def") == null, "a missing csv is empty");
        T.Eq(3, DisplayNames.ParseCsv("a,b\r\n\"c\nd\",e\r\nf,\"g\"\"h\"").Count, "csv rows: quoted newlines stay in one row");

        T.Eq(4, index.Search(null, null, null, null).Count(), "no criteria returns everything");
        T.Eq(new[] { "entity/-vehicle/x/c/c.def" }, index.Search("C.DEF", null, null, null).Select(u => u.Path).ToArray(), "text matches the path, ignoring case");
        T.Eq(new[] { "entity/-vehicle/x/a/a.def" }, index.Search("tank", null, null, names).Select(u => u.Path).ToArray(), "text matches the display name");
        T.Eq(new[] { "set/breed/mp/n.set" }, index.Search(null, "breed", null, null).Select(u => u.Path).ToArray(), "kind filter");
        T.Eq(2, index.Search(null, null, "modX", null).Count(), "layer filter");

        string cache = Path.Combine(work, "cache", "unit-index.json");
        bool rebuilt;
        UnitIndex.LoadOrBuild(stack, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "the first call builds");
        T.True(File.Exists(cache), "and writes the cache");
        var again = UnitIndex.LoadOrBuild(stack, cache, null, out rebuilt);
        T.Eq(false, rebuilt, "the second call loads the cache");
        T.Eq(4, again.Units.Count, "the cached index has the same units");
        T.Eq(1, again.UsersOf("entity/-vehicle/x/a/s.inc").Count, "the cached index answers UsersOf");

        T.Put(mod, "set/breed/mp/n.set", "{breed {changed 1}}");
        using var stack2 = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modX", mod) });
        stack2.Scan();
        UnitIndex.LoadOrBuild(stack2, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "a changed source file invalidates the cache");
        using var stack3 = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack3.Scan();
        UnitIndex.LoadOrBuild(stack3, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "a different set of layers invalidates the cache");
        File.WriteAllText(cache, "{ not json");
        UnitIndex.LoadOrBuild(stack3, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "a corrupt cache is rebuilt, not fatal");
    }
}
