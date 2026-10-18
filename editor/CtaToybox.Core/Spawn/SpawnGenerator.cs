// 스폰 모드의 파일 목록을 만든다: 맵 사본, 메뉴(menu.mi, templates_<국가>.mi, nation_<국가>.mi), 메뉴 글자(.pot), 화면 파일.
// 스커미시 로스터와 단추 그림은 --no-roster면 내지 않는다.
// 메뉴 글자는 영어 원문(SourcePotPath, 게임은 읽지 않는다)과 게임이 읽는 .pot(PotPath)을 함께 낸다. Translation이 있으면 .pot의 msgid를 번역으로 바꾼다(l10n build와 같은 규칙)
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
        public L10n.PoFile Translation { get; set; }   // l10n/ko의 메뉴 글자 번역. null이면 영어 그대로
        // 국가별 모드로 나눈다(2026-10-18 사용자 요청): 멀티 맵 견본은 국가 모드(NationFiles)에, 바탕 모드에는 빈 조각. false면 예전처럼 한 모드에 모두
        public bool NationMods { get; set; } = true;
        public Func<string, string> UnitName { get; set; }   // 유닛 정의 경로 → 게임 속 이름(영어). 계열 묶음 단추 글자. null이면 폴더 앞말
    }

    public sealed class SpawnReport
    {
        public int CampaignMaps, MultiMaps, Units, Squads, NewRosterEntries, Buttons, Screens, Templates;
        public int CampaignTemplateArmies;   // 캠페인 맵마다 넣은 견본 국가 수의 합
        public int TextTranslated, TextEnglish;   // 메뉴 글자 가운데 번역한 것과 영어로 둔 것
        public List<string> Skipped { get; } = new List<string>();
        public List<string> Notes { get; } = new List<string>();
        public int Maps { get { return CampaignMaps + MultiMaps; } }
    }

    public static class SpawnGenerator
    {
        public const string MenuDir = "resource/map/ctatoybox_spawn/";
        public const string PotPath = "localizations/default/interface/text/mission/ctatoybox_spawn.pot";
        public const string L10nPath = "interface/text/mission/ctatoybox_spawn.pot";   // 한글화 층의 경로(L10nSources, l10n/ko)
        // 메뉴 글자 영어 원문. localizations 밖이라 게임은 읽지 않고, l10n extract가 스폰 모드의 원문으로 읽는다(.pot이 한국어가 되어도 원문이 남게)
        public const string SourcePotPath = ".ctatoybox-spawn-text.pot";

        public static List<SpawnFile> Generate(SourceStack stack, UnitIndex index, ArmyIndex armies, SpawnCategories categories, SpawnOptions o, SpawnReport report)
        {
            Dictionary<string, List<SpawnFile>> nationFiles;
            return Generate(stack, index, armies, categories, o, report, out nationFiles);
        }

        // 국가 모드 안의 견본 파일 경로(바탕 모드의 빈 조각과 같은 경로라 뒤에 로드되면 이긴다)
        public static string NationTemplatePath(string army) { return MenuDir + MapPatch.TemplateFile(army); }

        // 빈 견본 조각: 국가 모드가 꺼져 있으면 멀티 맵이 이것을 읽는다
        public static string EmptyTemplate(string army)
        {
            return ";{mission\r\n; CTAToybox Spawn: " + army + " 견본 자리(빈 조각). 국가 모드 CTAToybox Spawn - " + army + "를 켜면 그 모드의 같은 경로 파일이 이긴다\r\n";
        }

        // nationFiles: 국가 → 국가 모드에 쓸 파일(NationMods일 때만, 아니면 비었다)
        public static List<SpawnFile> Generate(SourceStack stack, UnitIndex index, ArmyIndex armies, SpawnCategories categories, SpawnOptions o, SpawnReport report, out Dictionary<string, List<SpawnFile>> nationFiles)
        {
            o = o ?? new SpawnOptions();
            nationFiles = new Dictionary<string, List<SpawnFile>>(StringComparer.Ordinal);
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
            files.AddRange(MapPatch.Build(stack, report.Skipped, list, o.Menu.AllNations, o.NationMods, out report.CampaignMaps, out report.MultiMaps, out report.CampaignTemplateArmies));

            const string iconDir = "interface/scene/unit_icon/";
            var icons = stack.Paths.Where(p => p.StartsWith(iconDir, StringComparison.Ordinal) && p.EndsWith("_00.tga", StringComparison.Ordinal) && p.IndexOf('/', iconDir.Length) < 0)
                .Select(p => p.Substring(iconDir.Length, p.Length - iconDir.Length - "_00.tga".Length)).ToList();
            var menu = MenuBuilder.Build(cat, o.Menu, n => stack.Find(iconDir + n + "_00.tga") != null, b => stack.Find(SpawnCatalog.BreedFile(b)) != null, report.Skipped, icons, o.UnitName);
            report.Buttons = menu.Screens.Sum(s => s.Buttons.Count) + 1;
            report.Screens = menu.Screens.Count;
            report.Templates = menu.Templates;
            files.Add(new SpawnFile { Path = MenuDir + "menu.mi", Bytes = Encoding.UTF8.GetBytes(menu.Text) });
            foreach (var kv in menu.TemplateFiles)
            {
                byte[] real = Encoding.UTF8.GetBytes(kv.Value);
                if (!o.NationMods) { files.Add(new SpawnFile { Path = NationTemplatePath(kv.Key), Bytes = real }); continue; }
                files.Add(new SpawnFile { Path = NationTemplatePath(kv.Key), Bytes = Encoding.UTF8.GetBytes(EmptyTemplate(kv.Key)) });
                files.Add(new SpawnFile { Path = MenuDir + MapPatch.CampaignTemplateFile(kv.Key), Bytes = real });
                nationFiles[kv.Key] = new List<SpawnFile> { new SpawnFile { Path = NationTemplatePath(kv.Key), Bytes = real } };
            }
            files.Add(new SpawnFile { Path = MenuDir + MapPatch.ConquestFile, Bytes = Encoding.UTF8.GetBytes(MenuBuilder.ConquestFile()) });
            foreach (string army in list) files.Add(new SpawnFile { Path = MenuDir + "nation_" + army + ".mi", Bytes = Encoding.UTF8.GetBytes(MenuBuilder.NationFile(army, list)) });
            // 맵이 부르는 국가 조각이 다 있게: 캠페인 폴더의 국가가 메뉴 국가 목록에 없으면 독일 값으로 둔다
            foreach (string army in new[] { "rus", "ger", "fin", "usa", "eng", "jap" }.Where(a => !list.Contains(a)))
                files.Add(new SpawnFile { Path = MenuDir + "nation_" + army + ".mi", Bytes = Encoding.UTF8.GetBytes(MenuBuilder.NationFile(army, list)) });
            string pot = MenuBuilder.Pot(menu);
            files.Add(new SpawnFile { Path = SourcePotPath, Bytes = Encoding.UTF8.GetBytes(pot) });
            string shown = pot;
            if (o.Translation != null) shown = L10n.L10nBuilder.Translate(L10n.PoFile.Parse(pot), o.Translation, ref report.TextTranslated, ref report.TextEnglish).Replace("\r\n", "\n");
            else report.TextEnglish = menu.Strings.Count;
            files.Add(new SpawnFile { Path = PotPath, Bytes = Encoding.UTF8.GetBytes(shown) });
            if (o.Translation != null && report.TextEnglish > 0)
                report.Notes.Add("메뉴 글자 " + report.TextEnglish + "개는 영어로 둔다(번역이 비었거나 원문이 바뀌었다). l10n extract 다음 l10n/ko/interface/text/mission/ctatoybox_spawn.po를 채우고 다시 만든다");

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
