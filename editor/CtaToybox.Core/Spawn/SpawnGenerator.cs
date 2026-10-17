// 스폰 모드의 파일 목록을 만든다: 스커미시 로스터, 맵 사본, 메뉴(menu.mi, nation_<국가>.mi), 메뉴 글자(.pot), 화면 파일, 단추 그림
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

            var roster = RosterRewrite.Build(stack, cat, o.Roster, report.Skipped, report.Notes);
            report.NewRosterEntries = roster.Where(f => f.Path.EndsWith(".set", StringComparison.Ordinal)).Sum(f => CountOf(Encoding.Latin1.GetString(f.Bytes), "g(ctb_spawn)"));
            files.AddRange(roster);

            files.AddRange(MapPatch.Build(stack, report.Skipped, out report.CampaignMaps, out report.MultiMaps));

            var menu = MenuBuilder.Build(cat, o.Menu, n => stack.Find("interface/scene/unit_icon/" + n + "_00.tga") != null, b => stack.Find(SpawnCatalog.BreedFile(b)) != null, report.Skipped);
            report.Buttons = menu.Screens.Sum(s => s.Buttons.Count) + 1;
            report.Screens = menu.Screens.Count;
            report.Templates = menu.Templates;
            files.Add(new SpawnFile { Path = MenuDir + "menu.mi", Bytes = Encoding.UTF8.GetBytes(menu.Text) });
            var list = MenuBuilder.Armies(cat);
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
            report.Notes.Add("미확인: 증원 창 메뉴가 멀티·컨퀘스트에서 보이는지, 메뉴가 사라지는 문제가 고쳐지는지(게임 안에서만 알 수 있다)");
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
