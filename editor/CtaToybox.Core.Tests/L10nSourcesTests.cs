using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;
using CtaToybox.Core.L10n;

// 한글화 Task 2: 현지화 층 읽기
static class L10nSourcesTests
{
    public const string Pot = "msgid \"\"\nmsgstr \"\"\n\"Language: en\\n\"\n\nmsgctxt \"k1\"\nmsgid \"Tank\"\nmsgstr \"\"\n";

    // 게임(바닐라 pak), 모드 A(pak), 모드 B(낱개), 출력 모드(낱개, 원문에서 빠져야 한다)
    public static EditorSettings Fixture(out LayerPlan plan, out string outDir)
    {
        string game = T.TempDir(), work = T.TempDir();
        outDir = T.TempDir();
        T.Zip(Path.Combine(game, "localizations", "default.pak"), ("default/interface/text/tips.pot", Pot), ("default/interface/text/desc/a.pot", Pot));
        Directory.CreateDirectory(Path.Combine(game, "resource"));
        string modA = Path.Combine(game, "mods", "moda"), modB = Path.Combine(game, "mods", "modb");
        Directory.CreateDirectory(Path.Combine(modA, "resource")); Directory.CreateDirectory(Path.Combine(modB, "resource"));
        T.Zip(Path.Combine(modA, "localizations", "default.pak"), ("default/interface/text/tips.pot", Pot.Replace("Tank", "Tank A")));
        T.Put(modB, "localizations/default/interface/text/desc/b.pot", Pot);
        T.Put(outDir, "localizations/default/interface/text/tips.pot", Pot.Replace("Tank", "전차"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"moda:0\" \"modb:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        var s = new EditorSettings { GameDir = game, WorkshopDir = T.TempDir(), OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) };
        plan = GameProfile.PlanLayers(s, GameProfile.ReadModEntries(File.ReadAllText(options)));
        return s;
    }

    public static void Run()
    {
        LayerPlan plan; string outDir;
        var s = Fixture(out plan, out outDir);
        var warnings = new List<string>();
        var got = L10nSources.Collect(s, plan, warnings);
        T.Eq(new[] { "interface/text/desc/a.pot", "interface/text/desc/b.pot", "interface/text/tips.pot" }, got.Select(x => x.Path).ToArray(), "all paths, sorted");
        var tips = got.Single(x => x.Path.EndsWith("tips.pot"));
        T.Eq("moda", tips.Layer, "later mod wins; output mod is not a source");
        T.True(tips.Text.Contains("Tank A") && !tips.Text.Contains("전차"), "the winning text");
        T.Eq(new[] { "vanilla" }, tips.Shadowed.ToArray(), "shadowed layers listed");
        T.Eq("modb", got.Single(x => x.Path.EndsWith("b.pot")).Layer, "loose files read");
        T.Eq(0, warnings.Count, "no warnings");
    }
}
