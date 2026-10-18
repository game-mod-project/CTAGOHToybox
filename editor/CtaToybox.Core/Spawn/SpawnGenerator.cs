// 스폰 모드의 파일 목록을 만든다: 맵 사본, 메뉴(menu.mi, templates_<국가>.mi, nation_<국가>.mi), 메뉴 글자(.pot), 화면 파일.
// 스커미시 로스터와 단추 그림은 --no-roster면 내지 않는다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.Spawn
{
    public sealed class SpawnOptions
    {
        public RosterOptions Roster { get; set; } = new RosterOptions();
        public MenuOptions Menu { get; set; } = new MenuOptions();
    }

    public sealed class SpawnReport
    {
        public int CampaignMaps, MultiMaps, Units, Squads, NewRosterEntries, Buttons, Screens, Templates;
        public int CampaignTemplateArmies;   // 캠페인 맵마다 넣은 견본 국가 수의 합
        public List<string> Skipped { get; } = new List<string>();
        public List<string> Notes { get; } = new List<string>();
        public int Maps { get { return CampaignMaps + MultiMaps; } }
    }

    public static class SpawnGenerator
    {
        public const string MenuDir = "resource/map/ctatoybox_spawn/";
        public const string PotPath = "localizations/default/interface/text/mission/ctatoybox_spawn.pot";

        public static List<SpawnFile> Generate(SourceStack stack, UnitIndex index, ArmyIndex armies, SpawnCategories categories, SpawnOptions o, SpawnReport report)
        {
            o = o ?? new SpawnOptions();
            var files = new List<SpawnFile>();
            var cat = SpawnCatalog.Build(stack, index, armies, categories, report.Skipped);
            report.Units = cat.Units.Count;
            report.Squads = cat.Squads.Count;

            if (o.Roster.Enabled)
            {
                var roster = RosterRewrite.Build(stack, cat, o.Roster, report.Skipped, report.Notes);
                report.NewRosterEntries = roster.Where(f => f.Path.EndsWith(".set", StringComparison.Ordinal)).Sum(f => CountOf(Encoding.Latin1.GetString(f.Bytes), "g(ctb_spawn)"));
                files.AddRange(roster);
            }

            var list = MenuBuilder.Armies(cat);
            files.AddRange(MapPatch.Build(stack, report.Skipped, list, o.Menu.AllNations, out report.CampaignMaps, out report.MultiMaps, out report.CampaignTemplateArmies));

            var menu = MenuBuilder.Build(cat, o.Menu, n => stack.Find("interface/scene/unit_icon/" + n + "_00.tga") != null, b => stack.Find(SpawnCatalog.BreedFile(b)) != null, report.Skipped);
            report.Buttons = menu.Screens.Sum(s => s.Buttons.Count) + 1;
            report.Screens = menu.Screens.Count;
            report.Templates = menu.Templates;
            files.Add(new SpawnFile { Path = MenuDir + "menu.mi", Bytes = Encoding.UTF8.GetBytes(menu.Text) });
            foreach (var kv in menu.TemplateFiles) files.Add(new SpawnFile { Path = MenuDir + MapPatch.TemplateFile(kv.Key), Bytes = Encoding.UTF8.GetBytes(kv.Value) });
            foreach (string army in list) files.Add(new SpawnFile { Path = MenuDir + "nation_" + army + ".mi", Bytes = Encoding.UTF8.GetBytes(MenuBuilder.NationFile(army, list)) });
            // 맵이 부르는 국가 조각이 다 있게: 캠페인 폴더의 국가가 메뉴 국가 목록에 없으면 독일 값으로 둔다
            foreach (string army in new[] { "rus", "ger", "fin", "usa", "eng", "jap" }.Where(a => !list.Contains(a)))
                files.Add(new SpawnFile { Path = MenuDir + "nation_" + army + ".mi", Bytes = Encoding.UTF8.GetBytes(MenuBuilder.NationFile(army, list)) });
            files.Add(new SpawnFile { Path = PotPath, Bytes = Encoding.UTF8.GetBytes(MenuBuilder.Pot(menu)) });

            var view = stack.Find(SceneViewPatch.Path);
            if (view == null) report.Skipped.Add(SceneViewPatch.Path + ": 계층에 없다");
            else
            {
                string xml = Encoding.UTF8.GetString(stack.Read(view));
                string error;
                string patched = SceneViewPatch.Patch(xml, out error);
                if (patched == null) report.Skipped.Add(SceneViewPatch.Path + ": " + error);
                else files.Add(new SpawnFile { Path = "resource/" + view.VirtualPath, Bytes = Encoding.UTF8.GetBytes(patched), Source = view.Layer });
            }
            if (o.Menu.GridCols > 0)
            {
                var rv = stack.Find(ReinforcementViewPatch.Path);
                string rerr = null;
                string rx = rv == null ? null : ReinforcementViewPatch.Patch(Encoding.UTF8.GetString(stack.Read(rv)), o.Menu.GridCols, out rerr);
                if (rv == null) report.Skipped.Add(ReinforcementViewPatch.Path + ": 계층에 없다");
                else if (rx == null) report.Skipped.Add(ReinforcementViewPatch.Path + ": " + rerr);
                else files.Add(new SpawnFile { Path = "resource/" + rv.VirtualPath, Bytes = Encoding.UTF8.GetBytes(rx), Source = rv.Layer });
                report.Notes.Add("주의: 증원 창 가로 격자(" + o.Menu.GridCols + "열)는 2026-10-18 게임이 판 화면을 만들다 꺼진 판이다");
            }
            else if (o.Menu.VisibleButtons > 0)
            {
                var rv = stack.Find(ReinforcementViewPatch.Path);
                string rerr = null;
                string rx = rv == null ? null : ReinforcementViewPatch.SetVisible(Encoding.UTF8.GetString(stack.Read(rv)), o.Menu.VisibleButtons, out rerr);
                if (rv == null) report.Skipped.Add(ReinforcementViewPatch.Path + ": 계층에 없다");
                else if (rx == null) report.Skipped.Add(ReinforcementViewPatch.Path + ": " + rerr);
                else files.Add(new SpawnFile { Path = "resource/" + rv.VirtualPath, Bytes = Encoding.UTF8.GetBytes(rx), Source = rv.Layer });
                report.Notes.Add("미확인: 증원 창에 단추 " + o.Menu.VisibleButtons + "개가 화면 높이 안에 다 보이는지. 잘리면 spawn-menu build --visible <더 작은 수>");
            }
            report.Notes.Add("미확인: 증원 창 메뉴가 멀티·컨퀘스트에서 보이는지, 메뉴가 사라지는 문제가 고쳐지는지(게임 안에서만 알 수 있다)");
            if (!o.Menu.AllNations) report.Notes.Add("미확인: 견본 파일(templates_<국가>.mi)의 트리거가 ctb_has_<국가>를 켜서 국가 화면이 그 맵의 국가만 보이는지. 안 되면 spawn-menu build --all-nations");
            return files;
        }

        static int CountOf(string text, string what)
        {
            int n = 0, i = 0;
            while ((i = text.IndexOf(what, i, StringComparison.Ordinal)) >= 0) { n++; i += what.Length; }
            return n;
        }
    }
}
