using System.Collections.Generic;
using System.Linq;
using System.Text;
using CtaToybox.Core.Spawn;

// 스폰 모드: 화면 파일과 생성기 묶기(Task 7)
static class SpawnGeneratorTests
{
    public const string View =
        "<Layout>\n\t<FlowPanel id=\"unitspawn_layer\" halign=\"right\">\n\t\t<Children>\n" +
        "\t\t\t<FlowPanel direction=\"vertical\" enableif=\"game.multiplayer and !game.cooperative and !game.campaign\">\n\t\t\t\t<Children>\n\t\t\t\t\t<DialogRef refid=\"gui_gc_unitspawn\"/>\n\t\t\t\t</Children>\n\t\t\t</FlowPanel>\n" +
        "\t\t\t<FlowPanel direction=\"vertical\" enableif=\"gui.editor or game.singleplayer or game.cooperative\">\n\t\t\t\t<Children>\n\t\t\t\t\t<DialogRef refid=\"gui_gc_reinforcement\" enableif=\"gui.editor\"/>\n\t\t\t\t</Children>\n\t\t\t</FlowPanel>\n" +
        "\t\t\t<FlowPanel direction=\"vertical\" enableif=\"game.campaign and gameclient.master\">\n\t\t\t\t<Children>\n\t\t\t\t\t<DialogRef refid=\"gui_campaign_spawn\"/>\n\t\t\t\t</Children>\n\t\t\t</FlowPanel>\n" +
        "\t\t</Children>\n\t</FlowPanel>\n</Layout>\n";

    public static void Run()
    {
        string err;
        string patched = SceneViewPatch.Patch(View, out err);
        T.Eq(3, System.Text.RegularExpressions.Regex.Matches(patched, "gui_gc_reinforcement").Count, "reinforcement dialog in all three panels: " + err);
        T.True(patched.Contains("\t\t\t\t\t<DialogRef refid=\"gui_gc_reinforcement\"/>\n\t\t\t\t\t<DialogRef refid=\"gui_gc_unitspawn\"/>"), "inserted with the panel's indent: " + patched);
        T.True(!patched.Contains("enableif=\"gui.editor\"/>"), "the existing dialog loses its condition");
        string Strip(string x) { return System.Text.RegularExpressions.Regex.Replace(x, "\t*<DialogRef refid=\"gui_gc_reinforcement\"[^>]*>\n", ""); }
        T.Eq(Strip(View), Strip(patched), "nothing else changes");
        T.Eq(patched, SceneViewPatch.Patch(patched, out err), "patching twice changes nothing");
        T.Eq(null, SceneViewPatch.Patch(View.Replace("game.campaign and", "x"), out err), "a missing panel is an error");
        T.True(err != null && err.Contains("game.campaign"), "naming the panel");

        string repo, stat, spawn;
        string settings = SpawnFixture.Make(out repo, out stat, out spawn, ("interface/layout/views/scene/SceneGameView.xml", View));
        using var o = SpawnFixture.Open(settings);
        var report = new SpawnReport();
        var files = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions(), report);
        var paths = files.Select(f => f.Path).ToList();
        foreach (string p in new[] { "resource/map/ctatoybox_spawn/menu.mi", "resource/map/ctatoybox_spawn/nation_ger.mi", "resource/map/ctatoybox_spawn/nation_jap.mi", SpawnGenerator.PotPath,
            "resource/" + SpawnFixture.Campaign, "resource/" + SpawnFixture.Conquest, "resource/" + SpawnFixture.UnitsGer, "resource/interface/layout/views/scene/scenegameview.xml" })
            T.True(paths.Contains(p), "generated: " + p);
        T.Eq(paths.Count, paths.Distinct().Count(), "each path once");
        T.True(report.Maps == 2 && report.Units > 0 && report.Squads == 1 && report.NewRosterEntries > 0 && report.Buttons > 10, "report counts");
        string campaign = Encoding.UTF8.GetString(files.Single(f => f.Path == "resource/" + SpawnFixture.Campaign).Bytes);
        T.True(campaign.Contains("nation_ger.mi") && campaign.Contains("menu.mi"), "the campaign map includes its nation file and the menu");
    }
}
