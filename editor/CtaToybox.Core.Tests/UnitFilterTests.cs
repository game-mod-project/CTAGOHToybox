// 편집기 목록에서 빼는 종류와 수 없는 파일, 같은 줄 모으기, 원문 대소문자 키 경로
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class UnitFilterTests
{
    const string Gun = "set/stuff/gun/g1", Helmet = "set/stuff/head/h1", Pattern = "set/stuff/gun/gun.pattern",
        Skin = "entity/humanskin/ger/s/s.def", Model = "entity/inventory/helmet/m.def", Tank = "entity/-vehicle/x/t/t.def";

    public static void Run()
    {
        T.Eq(new[] { "vehicle", "breed", "stuff" }, UnitKind.Editor, "editor kinds");
        T.True(!UnitKind.InEditor("humanskin") && !UnitKind.InEditor("item-entity") && !UnitKind.InEditor("pattern") && !UnitKind.InEditor(null), "dropped kinds");
        T.Eq("파일", UnitKind.Label("pattern"), "a dropped kind has no editor label");

        var raw = SdlCore.KeyRows(SdlCore.Parse("{from \"x\" {Parameters {RadiusTable {0 1}} {\"Lvl_1\" 4}} (\"Fire Rate\" 600)}").Root.Children, new[] { "extender" });
        T.Eq("/from/parameters/radiustable/#row", raw.First(r => r.Path.EndsWith("#row")).Path, "the path is lower case");
        T.Eq("/from/Parameters/RadiusTable/#row", raw.First(r => r.Path.EndsWith("#row")).RawPath, "the raw path keeps the original case");
        T.Eq("/from/Parameters/\"Lvl_1\"", raw.First(r => r.Path.EndsWith("\"*\"")).RawPath, "a quoted name block shows its name");
        T.Eq("/from/(\"Fire Rate\")", raw.First(r => r.Form == "macro").RawPath, "a macro call keeps its name");

        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), repo = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/stuff/gun/.presets", "(define \"acc\" {\"ap\" {Table {0 0} {%r (* %r 2)}}} {\"he\" {Table {0 0} {%r (* %r 2)}}})"),
            (Gun, "{from \"pattern gun\" {Shells (\"acc\" r(180))} {mass 10}}"),
            (Helmet, "{from \"pattern heavy_headgear\" {tag \"armor\"} {entity h1}}"),
            (Pattern, "{armor 30}"));
        T.Zip(Path.Combine(game, "resource", "entity", "humanskin.pak"), ("humanskin/ger/s/s.def", "{game_entity {mass 80}}"));
        T.Zip(Path.Combine(game, "resource", "entity", "inventory.pak"), ("inventory/helmet/m.def", "{game_entity {mass 1}}"));
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"), ("-vehicle/x/t/t.def", "{game_entity {mass 30000}}"));
        T.Zip(Path.Combine(game, "localizations", "default.pak"), ("default/interface/text/desc/v.pot", "msgctxt \"desc/entity/t\"\nmsgid \"Tank\"\nmsgstr \"\"\n"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "common.psv"), "");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);

        using var session = EditorSession.Open(settings, repo, null);
        T.True(session.Index.Units.Any(u => u.Path == Skin) && session.Index.Units.Any(u => u.Path == Pattern), "the index keeps every kind (users of shared files)");
        T.Eq(0, session.Index.Units.First(u => u.Path == Helmet).Numbers, "a file of names only has no numbers");
        T.Eq(new[] { Tank, Gun }, session.Search(null, null, null).Select(u => u.Path).OrderBy(p => p).ToArray(), "the list drops other kinds and files without numbers");
        T.Eq(0, session.Search(null, "humanskin", null).Count(), "a dropped kind finds nothing");
        T.True(session.Search(null, null, null).All(u => u.Path != Model), "item models are not listed");

        var gun = session.OpenUnit(Gun);
        var rows = gun.Rows.Where(r => r.Path.EndsWith("/table/#row")).ToList();
        T.Eq(3, rows.Count, "the macro's two tables: two {0 0} source lines stay, the two {180 360} rows from one argument collapse");
        var row180 = rows.First(r => r.Values[0].Trace.Text == "180");
        T.Eq(2, row180.Repeats, "the collapsed row counts its repeats");
        T.Eq("/from/Shells/\"ap\"/Table/#row", row180.RawPath, "the first repeat's raw path is kept");
        T.True(gun.Rows.Count(r => r.Path == "/from/mass") == 1 && gun.Rows.First(r => r.Path == "/from/mass").Repeats == 1, "a single row is not counted twice");

        var distinct = new System.Collections.Generic.List<RowView>
        {
            new RowView { Path = "/a", Form = "block", Context = "volume x" },
            new RowView { Path = "/a", Form = "block", Context = "volume y" },
            new RowView { Path = "/a", Form = "block", Context = "volume x" },
        };
        foreach (var r in distinct) r.Values.Add(new ValueView { Trace = new TraceValue { Text = "5", File = "f", KeyPath = "/a", Occurrence = 0, Arg = 0 } });
        distinct.Add(new RowView { Path = "/a", Form = "block" });
        distinct[3].Values.Add(new ValueView { Trace = new TraceValue { Text = "5", File = "f", KeyPath = "/a", Occurrence = 1, Arg = 0 } });
        EditorSession.CollapseRepeats(distinct);
        T.Eq(2, distinct.Count, "another source line with the same text is not a repeat");
        T.Eq("volume x, volume y", distinct[0].Context, "the contexts of collapsed rows are joined once each");
        T.Eq(3, distinct[0].Repeats, "repeat count");

        var many = Enumerable.Range(1, 5).Select(i => new RowView { Path = "/a", Form = "block", Context = "c" + i }).ToList();
        foreach (var r in many) r.Values.Add(new ValueView { Trace = new TraceValue { Text = "5", File = "f", KeyPath = "/a", Occurrence = 0, Arg = 0 } });
        EditorSession.CollapseRepeats(many);
        T.Eq("c1, c2, c3 외 2곳", many[0].Context, "many contexts are cut after three");

        var shellRows = KeyLocator.Rows(SdlCore.Parse("{gun {parameters \"ap2\" {spreading 1}} {parameters \"he\" {spreading 1}}}").Root);
        var shellContexts = UnitTracer.ContextsOf(shellRows);
        T.Eq(new[] { "parameters ap2", "parameters he" }, shellRows.Select((r, i) => new { r, c = shellContexts[i] }).Where(x => x.r.Path.EndsWith("/spreading")).Select(x => x.c).ToArray(),
            "a weapon's parameters block names the shell type of its rows");
    }
}
