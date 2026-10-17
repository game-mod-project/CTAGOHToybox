using System.Linq;
using CtaToybox.Core.Spawn;

// 스폰 모드: 증원 창 가로 격자(사용자 요청 2026-10-17), 풀리지 않는 차량 빼기(2026-10-17 tog2 "define not found")
static class SpawnGridTests
{
    public const string View = "<Layout>\n\t<FlowPanel id=\"container\" direction=\"vertical\">\n\t\t<Children>\n\t\t\t<StackPanel>\n\t\t\t\t<Children>\n\t\t\t\t\t<StepScrollPane gap=\"2\"\n\t\t\t\t\t\tvisibleElements=\"7\"\n\t\t\t\t\t\tid=\"unit_types_container\">\n\t\t\t\t\t\t<Children>\n\t\t\t\t\t\t\t<!-- buttons here -->\n\t\t\t\t\t\t</Children>\n\t\t\t\t\t</StepScrollPane>\n\t\t\t\t</Children>\n\t\t\t</StackPanel>\n\t\t</Children>\n\t</FlowPanel>\n</Layout>\n";

    public static void Run()
    {
        string err;
        string g = ReinforcementViewPatch.Patch(View, 7, out err);
        T.True(g != null && g.Contains("<GridPanel id=\"unit_types_container\" cols=\"7\" hgap=\"2\" vgap=\"2\">") && !g.Contains("StepScrollPane"), "list becomes a 7-column grid: " + err);
        T.True(g.Contains("<!-- buttons here -->") && g.Contains("</GridPanel>"), "children kept");
        T.Eq(g, ReinforcementViewPatch.Patch(g, 7, out err), "patching twice changes nothing");
        T.Eq(null, ReinforcementViewPatch.Patch("<Layout/>", 7, out err), "no list is an error");

        // 보이는 단추 수: 세로 목록 그대로, visibleElements만 바뀐다
        string v = ReinforcementViewPatch.SetVisible(View, 25, out err);
        T.Eq(View.Replace("visibleElements=\"7\"", "visibleElements=\"25\""), v, "only visibleElements changes: " + err);
        T.Eq(v, ReinforcementViewPatch.SetVisible(v, 25, out err), "setting twice changes nothing");
        T.True(ReinforcementViewPatch.SetVisible(View.Replace("\n\t\t\t\t\t\tvisibleElements=\"7\"", ""), 25, out err).Contains("<StepScrollPane visibleElements=\"25\" gap=\"2\""), "added when missing");
        T.Eq(null, ReinforcementViewPatch.SetVisible("<Layout/>", 25, out err), "no list is an error for the count too");

        // 생성기: 격자 판과 세로 판, 풀리지 않는 차량
        string repo, stat, spawn;
        string settings = SpawnFixture.Make(out repo, out stat, out spawn,
            ("interface/layout/views/scene/component/GameUnitReinforcementView.xml", View),
            ("entity/-vehicle/eng/tank_heavy/broken/broken.def", "{game_entity\n (include \"/properties/tank.ext\")\n (\"no_such_macro\")\n}\n"));
        using var o = SpawnFixture.Open(settings);
        T.True(!o.Catalog.Units.Any(u => u.Name == "broken") && o.Skipped.Any(s => s.Contains("broken.def") && s.Contains("풀리지 않는다")), "a vehicle whose definition does not resolve and is in no roster is left out");
        // 기본은 바닐라 세로 목록(2026-10-18 게임이 격자 판 화면을 만들다 꺼졌다: ld:create scenegameview.xml EXCEPTION_ACCESS_VIOLATION)
        var list = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions(), new SpawnReport());
        var tall = list.SingleOrDefault(f => f.Path.EndsWith("gameunitreinforcementview.xml", System.StringComparison.OrdinalIgnoreCase));
        string tallText = tall == null ? "" : System.Text.Encoding.UTF8.GetString(tall.Bytes);
        T.True(tallText.Contains("<StepScrollPane") && tallText.Contains("visibleElements=\"25\"") && !tallText.Contains("GridPanel"), "the default is the vertical list showing 25 buttons");
        var vanilla = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions { Menu = new MenuOptions { VisibleButtons = 0 } }, new SpawnReport());
        T.True(!vanilla.Any(f => f.Path.EndsWith("gameunitreinforcementview.xml", System.StringComparison.OrdinalIgnoreCase)), "--visible 0 keeps the vanilla view file");
        var files = SpawnGenerator.Generate(o.Stack, o.Index, o.Armies, SpawnCategories.Parse(SpawnFixture.Categories, null), new SpawnOptions { Menu = new MenuOptions { GridCols = 7 } }, new SpawnReport());
        var view = files.SingleOrDefault(f => f.Path.EndsWith("gameunitreinforcementview.xml", System.StringComparison.OrdinalIgnoreCase));
        T.True(view != null && System.Text.Encoding.UTF8.GetString(view.Bytes).Contains("cols=\"7\""), "--grid 7 writes the grid view");
    }
}
