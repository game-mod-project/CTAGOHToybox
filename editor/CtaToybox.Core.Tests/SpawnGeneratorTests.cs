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
        foreach (string p in new[] { "resource/map/ctatoybox_spawn/menu.mi", "resource/map/ctatoybox_spawn/templates_ger.mi", "resource/map/ctatoybox_spawn/nation_ger.mi", "resource/map/ctatoybox_spawn/nation_jap.mi", SpawnGenerator.PotPath,
            "resource/" + SpawnFixture.Campaign, "resource/" + SpawnFixture.Conquest, "resource/interface/layout/views/scene/scenegameview.xml" })
            T.True(paths.Contains(p), "generated: " + p);
        T.Eq(paths.Count, paths.Distinct().Count(), "each path once");
        // 메뉴 글자: 영어 원문과 게임이 읽는 .pot. 번역이 없으면 둘이 같다(S2, 2026-10-18)
        string SpawnText(List<SpawnFile> fs, string p) { return Encoding.UTF8.GetString(fs.Single(f => f.Path == p).Bytes); }
        T.Eq(SpawnText(files, SpawnGenerator.SourcePotPath), SpawnText(files, SpawnGenerator.PotPath), "without a translation the menu words stay English");
        T.True(report.TextEnglish > 0 && report.TextTranslated == 0, "all English counted");
        var tr = CtaToybox.Core.L10n.PoFile.Parse("msgid \"\"\nmsgstr \"\"\n\nmsgctxt \"mission/ctatoybox_spawn/tanks\"\nmsgid \"Tanks\"\nmsgstr \"전차\"\n\nmsgctxt \"mission/ctatoybox_spawn/back\"\nmsgid \"Old back\"\nmsgstr \"뒤로\"\n");
        var kr = new SpawnReport();
        var korean = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions { Translation = tr }, kr);
        string shown = SpawnText(korean, SpawnGenerator.PotPath), source = SpawnText(korean, SpawnGenerator.SourcePotPath);
        T.True(shown.Contains("msgctxt \"mission/ctatoybox_spawn/tanks\"\nmsgid \"전차\"") && shown.Contains("Language: ko"), "translated msgid in the game's pot: " + shown.Substring(0, 300));
        T.True(shown.Contains("msgctxt \"mission/ctatoybox_spawn/back\"\nmsgid \"Back\""), "a translation of a changed source stays English");
        T.True(source.Contains("msgid \"Tanks\"") && !source.Contains("전차"), "the source keeps English");
        T.True(kr.TextTranslated == 1 && kr.TextEnglish == report.TextEnglish - 1 && kr.Notes.Any(n => n.Contains("영어로 둔다")), "counts and a note");
        T.True(!shown.Contains("\r"), "LF like l10n build");
        T.True(report.Maps == 2 && report.Units > 0 && report.Squads == 1 && report.NewRosterEntries == 0 && report.Buttons > 10, "report counts");
        // 구매 창 로스터는 기본으로 바꾸고(비용 0 등) 차량은 더하지 않는다(2026-10-18 사용자 결정)
        T.True(paths.Contains("resource/" + SpawnFixture.UnitsGer), "roster rewritten by default");
        var noRoster = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions { Roster = new RosterOptions { Enabled = false } }, new SpawnReport());
        T.True(!noRoster.Any(f => f.Path.StartsWith("resource/set/multiplayer/") || f.Path.Contains("/unit_icon/")), "--no-roster writes no roster files");
        string campaign = Encoding.UTF8.GetString(files.Single(f => f.Path == "resource/" + SpawnFixture.Campaign).Bytes);
        T.True(campaign.Contains("nation_ger.mi") && campaign.Contains("templates_ger.mi") && campaign.Contains("menu.mi"), "the campaign map includes its nation file, its templates and the menu");
        T.True(campaign.IndexOf("templates_ger.mi") < campaign.IndexOf("menu.mi"), "templates before the menu");

        // --roster는 구매 창을 바꾸고, --roster-add만 차량을 더한다
        var withRoster = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions { Roster = new RosterOptions { Enabled = true } }, report = new SpawnReport());
        T.True(withRoster.Any(f => f.Path == "resource/" + SpawnFixture.UnitsGer) && report.NewRosterEntries == 0, "--roster rewrites rosters without adding vehicles");
        SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions { Roster = new RosterOptions { Enabled = true, AddVehicles = true } }, report = new SpawnReport());
        T.True(report.NewRosterEntries > 0, "--roster-add adds vehicles");
    }
}
