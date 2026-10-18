// 증원 창 메뉴 menu.mi(스펙 "캠페인·컨퀘스트: 증원 창 메뉴", "옵션").
// 꼴은 Spawn Menu의 cheat.mi를 따른다(docs/13-spawn-menu-spike.md, 2026-10-17에 원문을 읽어 맞춤):
//  - 견본: 맵 밖(-9600, 9700 근처)에 숨긴 {Human "<병종>" 0x<id> ...}와 {Tags "hidden" "<국가>" "<태그>" 0x<id>}. 비행기는 {Entity}+조종사+{Link}.
//    견본은 국가마다 templates_<국가>.mi에 따로 쓴다. 멀티 맵은 모두, 캠페인 맵은 그 맵에 나오는 국가 것만 넣는다(MapPatch).
//    견본 파일은 ctb_has_<국가>를 1로 켜고, 국가 화면은 견본이 없는 국가 단추를 숨긴다.
//  - 단추를 누르면 트리거가 견본을 대기 구역(77777, 88888)으로 복제하고(actor_to_waypoint {clone}), 차량은 "spawn"으로 만든 뒤
//    함수가 소속을 정하고 놓을 자리(ctb_place)로 옮긴다("placement", 보병은 "waypoint" {make_squad}). 차량에는 "linker"로 승무원을 태운다.
//  - 화면은 단추 이름의 와일드카드로 보이고 숨긴다: 화면 N의 단추는 ctb_sN_bK, 모두 숨기기는 ctb_s*.
// 메뉴 글자는 영어이고 .pot 키(mission/ctatoybox_spawn/<키>)로 쓴다. 유닛 이름은 게임의 desc/entity·desc/squad 키 그대로
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.Spawn
{
    public sealed class MenuOptions
    {
        public int PageSize { get; set; } = 21;
        // 증원 창 단추 칸 열 수(가로 격자). 0이면 바닐라 세로 목록 그대로. 기본 0: 격자 판으로 게임이 판 화면을 만들다 꺼졌다(2026-10-18 game.log)
        public int GridCols { get; set; }
        // 증원 창 세로 목록에 한 번에 보이는 단추 수(바닐라 7). 0이면 화면 파일을 쓰지 않는다. 25는 화면을 넘어 아래 단추를 누를 수 없었다(2026-10-18 사용자).
        // 기본 10: 단추 그림 stub.tga 144x72 + gap 2 → 10칸 740 + 위아래 스크롤 단추 16x2 = 772px로 1080 화면에서 점수 창과 위쪽 여백이 남는다
        public int VisibleButtons { get; set; } = 10;
        public bool AllNations { get; set; }     // 캠페인 맵에도 모든 국가 견본을 넣는다(기본은 그 맵에 나오는 국가만)
    }

    public sealed class MenuButton
    {
        public string Name { get; set; }
        public string Text { get; set; }      // .pot 키
        public string Image { get; set; }
        public string Event { get; set; }
        public string Waypoint { get; set; }
        public string Preview { get; set; }
    }

    public sealed class MenuScreen
    {
        public int Id { get; set; }
        public string Army { get; set; }      // 국가 화면이면 그 국가, 함께 쓰는 화면이면 null
        public string Title { get; set; }
        public List<MenuButton> Buttons { get; } = new List<MenuButton>();
    }

    public sealed class MenuResult
    {
        public string Text { get; set; }
        public Dictionary<string, string> Strings { get; } = new Dictionary<string, string>(StringComparer.Ordinal);   // .pot 키 → 영어
        public List<MenuScreen> Screens { get; } = new List<MenuScreen>();
        public int Templates { get; set; }
        public int Triggers { get; set; }
        // 국가 → templates_<국가>.mi 글. 그 국가의 숨긴 견본(사람·비행기)과 ctb_has_<국가>를 켜는 트리거
        public Dictionary<string, string> TemplateFiles { get; } = new Dictionary<string, string>(StringComparer.Ordinal);
    }

    public sealed class MenuBuilder
    {
        public const string Root = "ctb_root", HideAll = "ctb_s*", Place = "ctb_place", Target = "ctb_target", WaitInf = "ctb_wait_inf", WaitVeh = "ctb_wait_veh";
        public const string PotKey = "mission/ctatoybox_spawn/";
        public const string Open = "ctb_open";   // 메뉴가 열려 있으면 1
        public const string Me = "ctb_me";       // 내 플레이어 번호 + 1. 0이면 아직 못 찾음
        public const string MineVar = "ctb_mine"; // 쓸 내 번호 + 1: ctb_me, 모르면 ctb_guess, 둘 다 0이면 0(player 0으로 둔다)
        public const string Guess = "ctb_guess";  // 컨퀘스트 Lua가 넣는 id_1st_player + 1(conquest_me.mi). 미확인: 그 번호가 이 PC 사용자인지
        public const int MeOwner = 11, AllyOwner = 12, EnemyOwner = 13;   // ctb_owner 값: 나, 아군 AI, 적 AI
        public const string Found = "ctb_found";
        public const int VetAround = 12;   // 등급 최대(찍은 자리): 가까운 내 병사 수. 분대 하나와 전차 승무원 둘쯤
        // 소속 값과 글자: 첫 화면의 "Owner: …" 표시와 소속 화면이 같이 쓴다
        public static readonly (int Value, string Key, string Label)[] OwnerChoices =
            new[] { (MeOwner, "me", "Me"), (AllyOwner, "ally", "Ally AI"), (EnemyOwner, "enemy", "Enemy AI"), (0, "0", "Auto assign") }
            .Concat(Enumerable.Range(1, 10).Select(k => (k, k.ToString(), "Player " + (k - 1)))).ToArray();
        public static readonly string[] NavEvents = { "ctb_e_root", "ctb_e_home" };
        static readonly string[] ArmyOrder = { "ger", "rus", "usa", "eng", "fin", "jap" };
        static readonly Dictionary<string, string> ArmyText = new Dictionary<string, string> { ["ger"] = "Germany", ["rus"] = "Soviet Union", ["usa"] = "United States", ["eng"] = "Britain", ["fin"] = "Finland", ["jap"] = "Japan" };
        static readonly Dictionary<string, string> CategoryText = new Dictionary<string, string> { ["light"] = "Light tanks", ["medium"] = "Medium tanks", ["heavy"] = "Heavy tanks", ["td"] = "Tank destroyers / SPG", ["car"] = "Vehicles", ["empl"] = "Guns", ["air"] = "Aircraft" };

        readonly SpawnCatalog cat;
        readonly MenuOptions o;
        readonly Func<string, bool> hasIcon;
        readonly Func<string, bool> hasBreed;
        readonly MenuResult result = new MenuResult();
        readonly List<MNode> triggers = new List<MNode>();
        readonly List<string> events = new List<string>();
        readonly Dictionary<string, int> homes = new Dictionary<string, int>(StringComparer.Ordinal);
        readonly Dictionary<string, List<(int Value, string Name)>> ownerLabels = new Dictionary<string, List<(int, string)>>(StringComparer.Ordinal);
        readonly List<string> skipped;
        int nextScreen = 1;
        int nextUnit = 1;

        // 견본. 국가마다 따로 모아 templates_<국가>.mi로 쓴다(맵이 필요한 국가 파일만 include한다)
        readonly Dictionary<string, (int Id, string Tag)> humanTemplates = new Dictionary<string, (int, string)>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, (StringBuilder Objects, StringBuilder Links, StringBuilder Tags)> byArmy = new Dictionary<string, (StringBuilder, StringBuilder, StringBuilder)>(StringComparer.Ordinal);
        int nextId = 0xC100;

        (StringBuilder Objects, StringBuilder Links, StringBuilder Tags) Of(string army)
        {
            if (!byArmy.TryGetValue(army, out var t)) { t = (new StringBuilder(), new StringBuilder(), new StringBuilder()); byArmy[army] = t; }
            return t;
        }

        public static string HasVar(string army) { return "ctb_has_" + army; }

        readonly List<string> iconNames;
        readonly Func<string, string> unitName;
        readonly Dictionary<string, string> squadIcons = new Dictionary<string, string>(StringComparer.Ordinal);
        // 국가 → 그 국가 견본 하나의 태그. 판 안에서 이 태그의 개체가 있으면 그 국가 견본 파일(국가 모드)이 읽힌 것이다
        readonly Dictionary<string, string> probeTag = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, int> owners = new Dictionary<string, int>(StringComparer.Ordinal), supports = new Dictionary<string, int>(StringComparer.Ordinal), tools = new Dictionary<string, int>(StringComparer.Ordinal);
        MenuScreen noNation;

        MenuBuilder(SpawnCatalog cat, MenuOptions o, Func<string, bool> hasIcon, Func<string, bool> hasBreed, List<string> skipped, IEnumerable<string> iconNames, Func<string, string> unitName)
        {
            this.cat = cat; this.o = o ?? new MenuOptions(); this.hasIcon = hasIcon ?? (_ => true); this.hasBreed = hasBreed ?? (_ => true); this.skipped = skipped;
            this.iconNames = (iconNames ?? Enumerable.Empty<string>()).Distinct(StringComparer.Ordinal).OrderBy(n => n, StringComparer.Ordinal).ToList();
            this.unitName = unitName;
        }

        public static List<string> Armies(SpawnCatalog cat)
        {
            var present = new HashSet<string>(cat.Units.Select(u => u.Army).Concat(cat.Squads.Select(s => s.Army)), StringComparer.Ordinal);
            return ArmyOrder.Where(present.Contains).Concat(present.Where(a => !ArmyOrder.Contains(a)).OrderBy(a => a, StringComparer.Ordinal)).ToList();
        }

        // hasIcon: interface/scene/unit_icon/<이름>_00.tga가 계층에 있는가. hasBreed: set/breed/<병종>.set이 있는가
        // iconNames: 계층에 있는 단추 그림 이름 전부(분대 그림 고르기). unitName: 유닛 정의 경로 → 게임 속 이름(계열 묶음 글자)
        public static MenuResult Build(SpawnCatalog cat, MenuOptions o, Func<string, bool> hasIcon, Func<string, bool> hasBreed, List<string> skipped, IEnumerable<string> iconNames = null, Func<string, string> unitName = null)
        {
            var b = new MenuBuilder(cat, o, hasIcon, hasBreed, skipped, iconNames, unitName);
            b.Make();
            return b.result;
        }

        string Str(string key, string english) { result.Strings[PotKey + key] = english; return PotKey + key; }

        MenuScreen NewScreen(string army, string title)
        {
            var s = new MenuScreen { Id = nextScreen++, Army = army, Title = title };
            result.Screens.Add(s);
            return s;
        }

        MenuButton Add(MenuScreen s, string textKey, string image, string ev, string waypoint = null, string preview = null)
        {
            var b = new MenuButton { Name = "ctb_s" + s.Id + "_b" + (s.Buttons.Count + 1), Text = textKey, Image = image, Event = ev, Waypoint = waypoint, Preview = preview };
            s.Buttons.Add(b);
            return b;
        }

        string ShowEvent(MenuScreen s)
        {
            string ev = "ctb_e_s" + s.Id;
            if (!events.Contains(ev))
            {
                events.Add(ev);
                triggers.Add(MNode.EventTrigger(ev, MNode.Hide(HideAll), MNode.Show("ctb_s" + s.Id + "_*")));
            }
            return ev;
        }

        string Icon(string name, string fallback) { return name != null && hasIcon(name) ? name : fallback; }

        // 국가마다 다른 그림(없으면 독일 그림, 그것도 없으면 fallback)
        string ArmyIcon(string pattern, string army, string fallback) { return Icon(string.Format(pattern, army), Icon(string.Format(pattern, "ger"), fallback)); }

        static double Cost(SpawnUnit u)
        {
            double v;
            return double.TryParse(u.Roster?.Arg("cost"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v) ? v : -1;
        }

        // 묶음의 대표 유닛(사용자 요청 2026-10-18: 가장 비싸거나 능력치가 높은 유닛의 아이콘). 아이콘이 있는 유닛 가운데 로스터 cost()가 가장 큰 것,
        // 같으면 이름이 뒤인 것(후기형이 뒤에 오는 이름이 많다). 능력치는 색인에 없어 쓰지 않는다
        SpawnUnit Rep(IEnumerable<SpawnUnit> units)
        {
            return units.Where(u => hasIcon(u.Name)).OrderByDescending(Cost).ThenByDescending(u => u.Name, StringComparer.Ordinal).FirstOrDefault();
        }

        string RepIcon(IEnumerable<SpawnUnit> units, string fallback) { var r = Rep(units); return r != null ? r.Name : fallback; }

        static readonly HashSet<string> NoiseWords = new HashSet<string>(StringComparer.Ordinal) { "squad", "single", "doctrine", "seq", "def", "all", "new" };
        static readonly string[] Periods = { "early", "mid", "late" };

        static List<string> Words(string name)
        {
            int p = name.IndexOf('(');
            if (p >= 0) name = name.Substring(0, p);
            return name.ToLowerInvariant().Split('_').Where(w => w.Length > 1 && !NoiseWords.Contains(w) && !w.All(char.IsDigit)).Distinct().ToList();
        }

        // 분대 그림: 분대 이름 그림이 있으면 그것. 없으면 같은 국가(side)의 분대·낱개 그림 가운데 이름 낱말이 가장 많이 겹치는 것(시기 낱말은 셈에서 빼고 같으면 앞세움),
        // 그것도 없으면 역할 낱말(sniper·medic·engineer…)의 낱개 그림. 모두 없으면 null(지금 그림)
        string SquadIcon(SpawnSquad sq)
        {
            string cached;
            if (squadIcons.TryGetValue(sq.MenuName, out cached)) return cached;
            string found = hasIcon(sq.MenuName) ? sq.MenuName : null;
            if (found == null)
            {
                string suffix = "(" + sq.Side + ")";
                var mine = Words(sq.Name);
                string period = mine.FirstOrDefault(Periods.Contains);
                var core = mine.Where(w => !Periods.Contains(w)).ToList();
                found = iconNames.Where(n => n.EndsWith(suffix, StringComparison.Ordinal) && (n.StartsWith("squad_", StringComparison.Ordinal) || n.StartsWith("single_", StringComparison.Ordinal) || n.StartsWith("doctrine_squad_", StringComparison.Ordinal)))
                    .Select(n => (Name: n, Words: Words(n)))
                    .Select(c => (c.Name, Score: c.Words.Count(core.Contains), SamePeriod: period != null && c.Words.Contains(period), Extra: c.Words.Count(w => !core.Contains(w) && !Periods.Contains(w))))
                    .Where(c => c.Score > 0)
                    .OrderByDescending(c => c.Score).ThenByDescending(c => c.SamePeriod).ThenBy(c => c.Extra).ThenBy(c => c.Name, StringComparer.Ordinal)
                    .Select(c => c.Name).FirstOrDefault();
            }
            if (found == null)
            {
                string all = string.Join("_", Words(sq.Name));
                foreach (var (words, role) in new[] { (new[] { "sniper", "marksm" }, "single_sniper"), (new[] { "medic" }, "single_medic"), (new[] { "engineer", "pio", "sap", "miner", "pioneer" }, "single_engineer"), (new[] { "officer", "command", "leader" }, "single_officer"), (new[] { "signal", "radio" }, "single_signaller") })
                    if (words.Any(w => all.Contains(w)) && hasIcon(role + "(" + sq.Side + ")")) { found = role + "(" + sq.Side + ")"; break; }
            }
            squadIcons[sq.MenuName] = found;
            return found;
        }

        // 분대 묶음의 대표 그림: 이름 그대로의 그림이 있는 분대 → 낱말로 고른 그림, 시기는 late → mid → early
        string RepSquadIcon(IEnumerable<SpawnSquad> squads, string fallback)
        {
            var ordered = squads.OrderByDescending(s => Array.IndexOf(Periods, s.Period)).ThenBy(s => s.Name, StringComparer.Ordinal).ToList();
            return ordered.Where(s => hasIcon(s.MenuName)).Select(s => s.MenuName).FirstOrDefault() ?? ordered.Select(SquadIcon).FirstOrDefault(i => i != null) ?? fallback;
        }

        void Make()
        {
            var armies = Armies(cat);
            var nationScreen = NewScreen(null, "Nation");
            foreach (string army in armies)
            {
                homes[army] = NewScreen(army, "Home").Id;
                owners[army] = NewScreen(army, "Owner").Id;
                supports[army] = NewScreen(army, "Support").Id;
                tools[army] = NewScreen(army, "Tools").Id;
            }
            // 견본이 있는 국가가 하나도 없을 때(국가 모드를 하나도 켜지 않음) 보이는 화면
            noNation = NewScreen(null, "No nation");
            Add(noNation, Str("no_nation", "No nation spawn mod is enabled"), "back", "ctb_e_root");

            // 국가 화면: 바로 고르는 단추(토글 아님, 사용자 결정). 견본이 실제로 있는 국가 단추만 보인다(ctb_check)
            var hides = new List<object> { MNode.Call("ctb_check"), MNode.Hide(HideAll), MNode.Show("ctb_s" + nationScreen.Id + "_*") };
            foreach (string army in armies)
            {
                string ev = "ctb_e_nat_" + army;
                var b = Add(nationScreen, Str("nation_" + army, ArmyText.TryGetValue(army, out var t) ? t : army), Icon("button_1_" + army, "button_1_ger"), ev);
                events.Add(ev);
                triggers.Add(MNode.EventTrigger(ev, MNode.SetVar("ctb_nation", armies.IndexOf(army) + 1), MNode.Hide(HideAll), MNode.Show("ctb_s" + homes[army] + "_*")));
                // 이 맵이 그 국가 견본 파일을 넣지 않았으면(캠페인) 그 국가 단추를 숨긴다. 견본 없이는 보병·승무원을 만들 수 없다
                hides.Add(MNode.IfVar(HasVar(army), "==", 0, MNode.Hide(b.Name)));
            }
            string nationEv = "ctb_e_s" + nationScreen.Id;
            events.Add(nationEv);
            triggers.Add(MNode.EventTrigger(nationEv, hides.ToArray()));

            MenuScreen ScreenOf(int id) { return result.Screens.Single(s => s.Id == id); }
            foreach (string army in armies) MakeOwner(army, ScreenOf(owners[army]), ScreenOf(homes[army]));
            foreach (string army in armies) MakeArmy(army, ScreenOf(homes[army]), nationScreen, ScreenOf(owners[army]), ScreenOf(supports[army]), ScreenOf(tools[army]));
            AddNav(nationScreen, null);
            foreach (string army in armies)
            {
                MakeSupport(ScreenOf(supports[army]), army);
                MakeTools(ScreenOf(tools[army]), army);
                foreach (int id in new[] { owners[army], supports[army], tools[army] }) AddNav(ScreenOf(id), ScreenOf(homes[army]));
            }

            // 처음 화면으로: 지금 국가의 처음 화면. 국가가 없으면(견본 있는 국가가 없음) 안내 화면
            var home = new MNode(MNode.Q("ctb_home"), MNode.Hide(HideAll));
            home.Add(MNode.IfVar("ctb_nation", "==", 0, MNode.Show("ctb_s" + noNation.Id + "_*")));
            foreach (string army in armies)
            {
                var show = new List<object> { MNode.Show("ctb_s" + homes[army] + "_*") };
                foreach (var v in ownerLabels[army]) show.Add(MNode.Hide(v.Name));
                foreach (var v in ownerLabels[army]) show.Add(MNode.IfVar("ctb_owner", "==", v.Value, MNode.Show(v.Name)));
                home.Add(MNode.IfVar("ctb_nation", "==", armies.IndexOf(army) + 1, show.ToArray()));
            }
            home.Add(MNode.SetVar(Open, 1));
            events.Add("ctb_e_root"); events.Add("ctb_e_home");
            // 맨 위 Spawn 단추는 여닫기다(사용자 지적 2026-10-18: 메뉴가 완전히 접히지 않는다). 열려 있으면(1) 모두 숨기고 2를 거쳐 0으로, 닫혀 있으면(0) 처음 화면
            triggers.Add(MNode.EventTrigger("ctb_e_root",
                MNode.IfVar(Open, "==", 1, MNode.Hide(HideAll), MNode.SetVar(Open, 2)),
                MNode.IfVar(Open, "==", 0, MNode.Call("ctb_home")),
                MNode.IfVar(Open, "==", 2, MNode.SetVar(Open, 0))));
            triggers.Add(MNode.EventTrigger("ctb_e_home", MNode.Call("ctb_home")));
            result.Text = Render(armies, home);
        }

        void AddNav(MenuScreen s, MenuScreen parent)
        {
            if (parent != null) Add(s, Str("back", "Back"), "back", ShowEvent(parent));
            Add(s, Str("home", "Home"), "back", "ctb_e_home");
        }

        // 소속 화면(국가마다, 국가 그림을 쓰려고): 나·아군 AI·적 AI(설계 "소속 바꾸기"), Auto, Player 0~9. 값은 OwnerChoices.
        // 내 번호(ctb_mine = 번호+1)는 판 안에서 찾는다(Mine). 알면 내 번호 단추가 "Player N (you)"로 바뀐다(사용자 지적 2026-10-18: 내가 몇 번인지 모른다)
        void MakeOwner(string army, MenuScreen ownerScreen, MenuScreen home)
        {
            var ownerShow = new List<object>();
            ownerShow.AddRange(FindMe());
            ownerShow.Add(MNode.Call("ctb_mine"));
            ownerShow.Add(MNode.Hide(HideAll));
            ownerShow.Add(MNode.Show("ctb_s" + ownerScreen.Id + "_*"));
            string officer = ArmyIcon("single_officer({0})", army, "back");
            var images = new Dictionary<string, string>
            {
                ["me"] = officer, ["ally"] = ArmyIcon("signaller({0})", army, "back"), ["enemy"] = ArmyIcon("single_sniper({0})", army, "back"), ["0"] = ArmyIcon("doctrine({0})", army, "back"),
            };
            string Owner(int k, string key, string label, string image)
            {
                string ev = "ctb_e_own_" + k;
                var b = Add(ownerScreen, Str(key, label), image, ev);
                if (!events.Contains(ev))
                {
                    events.Add(ev);
                    triggers.Add(MNode.EventTrigger(ev, MNode.SetVar("ctb_owner", k), MNode.Call("ctb_home")));
                }
                return b.Name;
            }
            foreach (var c in OwnerChoices.Where(c => c.Value == 0 || c.Value > 10)) Owner(c.Value, "owner_" + c.Key, c.Label, images[c.Key]);
            string flag = ArmyIcon("button_1_{0}", army, "back");
            for (int k = 1; k <= 10; k++)
            {
                string plain = Owner(k, "owner_" + k, "Player " + (k - 1), flag);
                string you = Owner(k, "owner_you_" + k, "Player " + (k - 1) + " (you)", officer);
                ownerShow.Add(MNode.Hide(you));
                ownerShow.Add(MNode.IfVar(MineVar, "==", k, MNode.Hide(plain), MNode.Show(you)));
            }
            ownerShow.Add(MNode.SetVar(Open, 1));
            string ownerEv = "ctb_e_s" + ownerScreen.Id;
            events.Add(ownerEv);
            triggers.Add(MNode.EventTrigger(ownerEv, ownerShow.ToArray()));
        }

        void MakeArmy(string army, MenuScreen home, MenuScreen nation, MenuScreen owner, MenuScreen support, MenuScreen tools)
        {
            // 첫 화면에 지금 국가와 지금 소속을 보인다(설계 "첫 화면": 국가: <지금 국가> · 소속: <지금 소속>). 소속은 값마다 단추를 두고 ctb_home이 맞는 하나만 보인다
            Add(home, Str("nation_is_" + army, "Nation: " + (ArmyText.TryGetValue(army, out var an) ? an : army)), Icon("button_1_" + army, "button_1_ger"), ShowEvent(nation));
            var variants = new List<(int Value, string Name)>();
            foreach (var c in OwnerChoices) variants.Add((c.Value, Add(home, Str("owner_is_" + c.Key, "Owner: " + c.Label), ArmyIcon("single_officer({0})", army, "back"), ShowEvent(owner)).Name));
            ownerLabels[army] = variants;
            var units = cat.Units.Where(u => u.Army == army).ToList();
            var squads = cat.Squads.Where(s => s.Army == army).ToList();

            // 단추 그림은 묶음의 대표(사용자 요청 2026-10-18: 국기·같은 그림이라 구분이 어렵다)
            if (squads.Count > 0)
            {
                var inf = NewScreen(army, "Infantry");
                Add(home, Str("infantry", "Infantry"), RepSquadIcon(squads.Where(s => s.Button == "inf1"), ArmyIcon("button_1_{0}", army, "back")), ShowEvent(inf));
                foreach (var (button, label) in new[] { ("inf1", "Squads"), ("inf2", "Single soldiers") })
                {
                    var list = squads.Where(s => s.Button == button).ToList();
                    if (list.Count == 0) continue;
                    string fallback = ArmyIcon("button_" + (button == "inf1" ? 1 : 2) + "_{0}", army, "back");
                    var page = Pages(army, inf, label, list.Select(s => (Group: s.Period, Item: (object)s)).ToList(),
                        (g, items) => Str("period_" + g, char.ToUpperInvariant(g[0]) + g.Substring(1)), items => RepSquadIcon(items.Cast<SpawnSquad>(), fallback));
                    Add(inf, Str(button == "inf1" ? "squads" : "single", label), RepSquadIcon(list, fallback), ShowEvent(page));
                }
                AddNav(inf, home);
            }
            foreach (var (cats, label, icon) in new[] { (new[] { "empl" }, "Guns", 3), (new[] { "car" }, "Vehicles", 4) })
            {
                var list = units.Where(u => cats.Contains(u.Category)).ToList();
                if (list.Count == 0) continue;
                var page = UnitPages(army, home, label, list);
                Add(home, Str(label.ToLowerInvariant(), label), RepIcon(list, ArmyIcon("button_" + icon + "_{0}", army, "back")), ShowEvent(page));
            }
            var tankCats = new[] { "light", "medium", "heavy", "td" };
            if (units.Any(u => tankCats.Contains(u.Category)))
            {
                var tanks = NewScreen(army, "Tanks");
                Add(home, Str("tanks", "Tanks"), RepIcon(units.Where(u => tankCats.Contains(u.Category)), ArmyIcon("button_5_{0}", army, "back")), ShowEvent(tanks));
                foreach (string c in tankCats)
                {
                    var list = units.Where(u => u.Category == c).ToList();
                    if (list.Count == 0) continue;
                    var page = UnitPages(army, tanks, CategoryText[c], list);
                    Add(tanks, Str("cat_" + c, CategoryText[c]), RepIcon(list, ArmyIcon("button_5_{0}", army, "back")), ShowEvent(page));
                }
                AddNav(tanks, home);
            }
            var air = units.Where(u => u.Category == "air").ToList();
            if (air.Count > 0)
            {
                var page = UnitPages(army, home, "Aircraft", air);
                Add(home, Str("aircraft", "Aircraft"), RepIcon(air, ArmyIcon("air_strike_{0}", army, "back")), ShowEvent(page));
            }
            Add(home, Str("support", "Support"), Icon("150mm_sfh18_artillery_barrage", "artystrike"), ShowEvent(support));
            Add(home, Str("tools", "Tools"), ArmyIcon("single_engineer({0})", army, "back"), ShowEvent(tools));
        }

        // 유닛 목록을 계열로 묶는 화면. 묶음 단추 글자는 대표 유닛의 게임 속 이름 + " (+나머지 수)"(사용자 요청 2026-10-18 가독성: 폴더 앞말 b·c·pzkpfw는 읽기 어렵다).
        // 키는 국가·화면·계열마다 따로(같은 계열 이름이 화면마다 다른 글자가 되므로)
        MenuScreen UnitPages(string army, MenuScreen parent, string title, List<SpawnUnit> list)
        {
            string slug = new string(title.ToLowerInvariant().Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
            return Pages(army, parent, title, list.Select(u => (Group: u.Family, Item: (object)u)).ToList(), (g, items) =>
            {
                var us = items.Cast<SpawnUnit>().ToList();
                var rep = Rep(us) ?? us.OrderByDescending(Cost).ThenByDescending(u => u.Name, StringComparer.Ordinal).First();
                string name = unitName?.Invoke(rep.Def);
                string label = string.IsNullOrWhiteSpace(name) ? g : name + (us.Count > 1 ? " (+" + (us.Count - 1) + ")" : "");
                return Str("group_" + army + "_" + slug + "_" + g, label);
            }, items => RepIcon(items.Cast<SpawnUnit>(), CategoryIcon(((SpawnUnit)items[0]).Category, army)));
        }

        // 유닛 목록 화면. 한도를 넘으면 묶음 화면을 한 단계 두고, 묶음 하나나 묶음 목록이 넘으면 Next/Prev로 나눈다
        MenuScreen Pages(string army, MenuScreen parent, string title, List<(string Group, object Item)> items, Func<string, List<object>, string> groupText, Func<List<object>, string> groupImage)
        {
            if (items.Count <= o.PageSize) return ItemPages(army, parent, title, items.Select(i => i.Item).ToList());
            var groups = items.GroupBy(i => i.Group ?? "other").OrderBy(g => g.Key, StringComparer.Ordinal).ToList();
            if (groups.Count == 1) return ItemPages(army, parent, title, items.Select(i => i.Item).ToList());
            var screens = Paged(army, parent, title, groups.Count, (screen, index) =>
            {
                var g = groups[index];
                var members = g.Select(i => i.Item).ToList();
                var sub = ItemPages(army, screen, title + " / " + g.Key, members);
                Add(screen, groupText(g.Key, members), groupImage(members), ShowEvent(sub));
            });
            return screens[0];
        }

        MenuScreen ItemPages(string army, MenuScreen parent, string title, List<object> items)
        {
            return Paged(army, parent, title, items.Count, (screen, index) => UnitButton(screen, army, items[index]))[0];
        }

        // count개를 PageSize씩 화면에 나눈다. 화면마다 Prev/Next, Back(parent), Home
        List<MenuScreen> Paged(string army, MenuScreen parent, string title, int count, Action<MenuScreen, int> addItem)
        {
            var pages = new List<MenuScreen>();
            int n = Math.Max(1, (count + o.PageSize - 1) / o.PageSize);
            for (int p = 0; p < n; p++) pages.Add(NewScreen(army, title + (n > 1 ? " " + (p + 1) : "")));
            for (int p = 0; p < n; p++)
            {
                for (int i = p * o.PageSize; i < Math.Min(count, (p + 1) * o.PageSize); i++) addItem(pages[p], i);
                if (p > 0) Add(pages[p], Str("prev", "Previous"), "back", ShowEvent(pages[p - 1]));
                if (p < n - 1) Add(pages[p], Str("next", "Next"), "back", ShowEvent(pages[p + 1]));
                AddNav(pages[p], parent);
            }
            return pages;
        }

        void UnitButton(MenuScreen screen, string army, object item)
        {
            if (item is SpawnSquad sq)
            {
                string ev = "ctb_e_q" + nextUnit++;
                var acts = new List<object>();
                foreach (var m in sq.Members) acts.Add(Clone(Human(m.Breed, army), m.Count));
                acts.Add(MNode.Call("ctb_infantry"));
                AddUnit(screen, "desc/squad/" + sq.MenuName, SquadIcon(sq) ?? ArmyIcon("button_" + (sq.Button == "inf2" ? 2 : 1) + "_{0}", army, "back"), ev, acts);
                return;
            }
            var u = (SpawnUnit)item;
            if (u.Category == "air") { AirButton(screen, army, u); return; }
            var crew = Crew(u);
            if (crew == null) { skipped?.Add(u.Def + ": 메뉴에 넣지 않았다. 승무원 병종을 찾지 못했다"); return; }
            string evu = "ctb_e_u" + nextUnit++;
            var actions = new List<object>();
            foreach (var c in crew) actions.Add(Clone(Human(c.Breed, army), c.Count));
            actions.Add(new MNode(MNode.Q("spawn"), new MNode("entity", MNode.Q(u.Name)), new MNode("waypoint", MNode.Q(WaitVeh))));
            actions.Add(MNode.Call("ctb_vehicle"));
            AddUnit(screen, "desc/entity/" + u.Name, UnitIcon(u), evu, actions);
        }

        // 유닛 단추 그림: 자기 아이콘, 없으면 같은 국가·같은 계열에서 아이콘 있는 대표의 것(모드 유닛은 아이콘이 없는 경우가 많다), 그것도 없으면 종류 그림
        string UnitIcon(SpawnUnit u)
        {
            if (hasIcon(u.Name)) return u.Name;
            var kin = Rep(cat.Units.Where(x => x.Army == u.Army && x.Category == u.Category && x.Family != null && x.Family == u.Family));
            return kin != null ? kin.Name : CategoryIcon(u.Category, u.Army);
        }

        string CategoryIcon(string category, string army)
        {
            switch (category)
            {
                case "empl": return Icon("button_3_" + army, "button_3_ger");
                case "car": return Icon("button_4_" + army, "button_4_ger");
                case "air": return Icon("air_strike_" + army, "air_strike_ger");
                default: return Icon("button_5_" + army, "button_5_ger");
            }
        }

        void AddUnit(MenuScreen screen, string text, string image, string ev, List<object> actions)
        {
            Add(screen, text, image, ev, Place, "pointer_green");
            events.Add(ev);
            triggers.Add(MNode.EventTrigger(ev, actions.ToArray()));
        }

        // 차량 승무원: 그 국가 로스터 항목의 crewN, 없으면 국가 기본 승무원(거치·포는 artilleryman 2). 시기는 항목의 period, 없으면 late → mid → early 가운데 병종이 있는 것
        List<(string Breed, int Count)> Crew(SpawnUnit u)
        {
            List<(string Type, int Count)> types;
            string period = u.Roster?.Arg("period");
            if (u.Roster != null && RosterScan.Members(u.Roster, true).Count > 0) types = RosterScan.Members(u.Roster, true);
            else if (u.Category == "empl") types = new List<(string, int)> { (RosterRewrite.GunCrew, 2) };
            else if (RosterRewrite.DefaultCrew.TryGetValue(u.Army, out var c)) types = new List<(string, int)> { (c.Crew, 1), (c.Commander, 1) };
            else types = null;
            // 후보: 위의 것, 이어서 같은 국가·같은 종류의 로스터 항목들이 가장 많이 쓰는 승무원 구성(RosterRewrite와 같은 법)
            var candidates = new List<List<(string Type, int Count)>>();
            if (types != null) candidates.Add(types);
            candidates.AddRange(cat.Units.Where(x => x.Army == u.Army && x.Category == u.Category && x.Roster != null).Select(x => RosterScan.Members(x.Roster, true)).Where(m => m.Count > 0)
                .GroupBy(m => string.Join(" ", m.Select(t => t.Type + ":" + t.Count))).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).Select(g => g.First()));
            foreach (var cand in candidates)
                foreach (string p in (period != null ? new[] { period } : new string[0]).Concat(new[] { "late", "mid", "early" }))
                {
                    var breeds = cand.Select(t => ("mp/" + u.Army + "/" + p + "/" + t.Type, t.Count)).ToList();
                    if (breeds.All(b => hasBreed(b.Item1))) return breeds;
                }
            return null;
        }

        (int Id, string Tag) Human(string breed, string army)
        {
            if (humanTemplates.TryGetValue(breed, out var t)) return t;
            int id = nextId++;
            string tag = "ctb_h" + (humanTemplates.Count + 1);
            int i = humanTemplates.Count;
            double x = -9700 + (i % 60) * 3, y = 9600 + (i / 60) * 3;
            var to = Of(army);
            to.Objects.Append("\t{Human ").Append(MNode.Q(breed)).Append(" 0x").Append(id.ToString("x")).Append("\r\n")
                .Append("\t\t{Position ").Append(F(x)).Append(' ').Append(F(y)).Append("}\r\n")
                .Append("\t\t{Volume \"ram\"\r\n\t\t\t{able {visible 0}{bullet 0}{throwing 0}{obstacle 0}{contact 0}{contact_ground 0}{blast 0}{select 0}{touch 0}{blockcamera 0}}\r\n\t\t\t{disabled}\r\n\t\t}\r\n")
                .Append("\t\t{Player 0}\r\n\t}\r\n");
            to.Tags.Append("\t{Tags \"hidden\" ").Append(MNode.Q(army)).Append(' ').Append(MNode.Q(tag)).Append(" 0x").Append(id.ToString("x")).Append("}\r\n");
            humanTemplates[breed] = (id, tag);
            if (!probeTag.ContainsKey(army)) probeTag[army] = tag;
            return (id, tag);
        }

        static string F(double v) { return v.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture); }

        static MNode Clone((int Id, string Tag) template, int count)
        {
            var move = new MNode(MNode.Q("actor_to_waypoint"), MNode.Selector(template.Tag, null, new[] { "un" }), new MNode("waypoint", MNode.Q(WaitInf)), "{clone}", new MNode("approach", "teleport"));
            return new MNode(MNode.Q("loop") + " " + count, move);
        }

        // 비행기: 조종사를 태운 {Entity} 견본을 놓을 자리로 복제한다(cheat.mi 204행 bf109_g6 견본, 트리거 "飞机/ger/9.2" 꼴)
        void AirButton(MenuScreen screen, string army, SpawnUnit u)
        {
            // 조종사 병종: pilot, 일본은 ijn_pilot(2026-10-17 색인)
            string pilot = new[] { "late", "mid", "early" }.SelectMany(p => new[] { "pilot", "ijn_pilot" }.Select(n => "mp/" + army + "/" + p + "/" + n)).FirstOrDefault(hasBreed);
            if (pilot == null) { skipped?.Add(u.Def + ": 메뉴에 넣지 않았다. 조종사 병종이 없다"); return; }
            int index = nextUnit++;
            string tag = "ctb_a" + index, ev = "ctb_e_a" + index;
            int ent = nextId++, man = nextId++;
            double x = -9500 + (index % 40) * 40, y = 8300;
            var to = Of(army);
            to.Objects.Append("\t{Entity ").Append(MNode.Q(u.Name)).Append(" 0x").Append(ent.ToString("x")).Append("\r\n")
                .Append("\t\t{Position ").Append(F(x)).Append(' ').Append(F(y)).Append("}\r\n\t\t{DisableObstacles}\r\n\t\t{Player 0}\r\n")
                .Append("\t\t{Chassis \"airborne\" {AirborneMode 1} {Altitude 40}}\r\n\t\t{ChassisManager {Current \"airborne\"}}\r\n\t}\r\n")
                .Append("\t{Human ").Append(MNode.Q(pilot)).Append(" 0x").Append(man.ToString("x")).Append("\r\n")
                .Append("\t\t{Position ").Append(F(x + 4)).Append(' ').Append(F(y)).Append("}\r\n")
                .Append("\t\t{Volume \"ram\"\r\n\t\t\t{able {visible 0}{bullet 0}{throwing 0}{obstacle 0}{contact 0}{contact_ground 0}{blast 0}{select 0}{touch 0}{blockcamera 0}}\r\n\t\t\t{disabled}\r\n\t\t}\r\n")
                .Append("\t\t{Player 0}\r\n\t}\r\n");
            to.Links.Append("\t{Link 0x").Append(man.ToString("x")).Append(" {0x").Append(ent.ToString("x")).Append(" \"driver\"}}\r\n");
            to.Tags.Append("\t{Tags ").Append(MNode.Q(tag)).Append(" \"hidden\" ").Append(MNode.Q(army)).Append(" \"ctb_air\" 0x").Append(ent.ToString("x")).Append("}\r\n");
            if (!firstPlane.ContainsKey(army)) firstPlane[army] = tag;
            if (!probeTag.ContainsKey(army)) probeTag[army] = tag;
            if (!firstStrike.ContainsKey(army) && u.Name.IndexOf("strike", StringComparison.OrdinalIgnoreCase) >= 0) firstStrike[army] = tag;
            var actions = new List<object>
            {
                new MNode(MNode.Q("waypoint"), new MNode("who", new MNode("type", "actor"), new MNode("actors", new MNode("tag", MNode.Q(tag)))), new MNode("action", new MNode("type", "start"), new MNode("waypoint", MNode.Q(Place)), "{clone}", new MNode("approach", "teleport"))),
                MNode.Call("ctb_plane"),
            };
            AddUnit(screen, "desc/entity/" + u.Name, UnitIcon(u), ev, actions);
        }

        // 포격 소·중·대: (키, 글자, 발 수, 흩어지는 반경). 사용자 요청(2026-10-18): 찍은 지점 둘레에 최소 5발에서 최대 20발.
        // 바닐라 포격은 반경이 큰 웨이포인트에 spawn을 되풀이한다(01-rus/1942_05_kerch "part2/enemy/main/art/global": loop 12, 웨이포인트 1659~1661 radius 1400,
        // 실행 파일에 disable_random_spawn_position 명령이 있다 → spawn은 웨이포인트 반경 안 아무 데나 놓는다고 본다). 그래서 크기마다 반경이 다른 웨이포인트를 둔다.
        // 반경 값의 거리 단위는 미확인(바닐라 캠페인 포격 지점 반경 2~2000, 평균 462)
        public static readonly (string Key, string Label, int Shots, int Radius)[] Barrages =
        {
            ("arty_s", "Artillery (small)", 5, 20), ("arty_m", "Artillery (medium)", 12, 40), ("arty_l", "Artillery (large)", 20, 70),
        };

        public static readonly (string Key, string Label, int Planes)[] AirStrikes = { ("air_s", "Air strike (small)", 1), ("air_m", "Air strike (medium)", 2), ("air_l", "Air strike (large)", 4) };
        public const string AirTarget = "ctb_wp_air";
        // 국가 → 첫 항공기 견본 태그, 이름에 strike가 든 첫 항공기 견본 태그
        readonly Dictionary<string, string> firstPlane = new Dictionary<string, string>(StringComparer.Ordinal), firstStrike = new Dictionary<string, string>(StringComparer.Ordinal);

        // 이벤트 트리거는 한 번만 둔다(지원·도구·소속 화면이 국가마다 있어 같은 이벤트 단추가 여럿이다)
        void Once(string ev, params object[] actions)
        {
            if (events.Contains(ev)) return;
            events.Add(ev);
            triggers.Add(MNode.EventTrigger(ev, actions));
        }

        // 포격 그림: 소 81mm 박격포, 중 120mm 박격포, 대 150mm 곡사포(scene.pak 그림, 크기를 그림으로 구분)
        static readonly Dictionary<string, string> BarrageIcon = new Dictionary<string, string> { ["arty_s"] = "81mm_krh36_mortar_barrage", ["arty_m"] = "120mm_krh40_mortar_barrage", ["arty_l"] = "150mm_sfh18_artillery_barrage" };

        void MakeSupport(MenuScreen s, string army)
        {
            foreach (var (key, label, shots, _) in Barrages)
            {
                string ev = "ctb_e_" + key, wp = "ctb_wp_" + key;
                Add(s, Str(key, label), Icon(BarrageIcon[key], "artystrike"), ev, wp, "crosshair");
                Once(ev, new MNode(MNode.Q("loop") + " " + shots, new MNode(MNode.Q("spawn"), new MNode("entity", MNode.Q("artillery_single_150_dummy")), new MNode("waypoint", MNode.Q(wp))), new MNode(MNode.Q("delay"), new MNode("time", "0.4"))));
            }
            // 항공 폭격 소·중·대(설계 "옵션" 폭격: 항공·곡사포·자주포, 소·중·대): 지금 국가의 공격기 견본을 찍은 자리로 복제하고 내 소속으로 한 뒤
            // "air_attack" {waypoint_to_attack}으로 그 자리를 치게 한다(바닐라 00-bootcamp/04_reinforcements-offmap-support "air_attack" 꼴, 바닐라·모드 맵 116회).
            // 공격기는 국가의 항공기 가운데 이름에 strike가 든 첫 것, 없으면 첫 항공기. 게임 안 동작은 미확인
            var airStrike = firstPlane.ToDictionary(kv => kv.Key, kv => firstStrike.TryGetValue(kv.Key, out var st) ? st : kv.Value, StringComparer.Ordinal);
            if (airStrike.ContainsKey(army))
                foreach (var (key, label, planes) in AirStrikes)
                {
                    string ev = "ctb_e_" + key;
                    Add(s, Str(key, label), ArmyIcon("air_strike_{0}", army, "artystrike"), ev, AirTarget, "crosshair");
                    if (events.Contains(ev)) continue;
                    var acts = new List<object>();
                    var armies = Armies(cat);
                    foreach (var kv in airStrike)
                        acts.Add(MNode.IfVar("ctb_nation", "==", armies.IndexOf(kv.Key) + 1, new MNode(MNode.Q("loop") + " " + planes,
                            new MNode(MNode.Q("waypoint"), new MNode("who", new MNode("type", "actor"), new MNode("actors", new MNode("tag", MNode.Q(kv.Value)))), new MNode("action", new MNode("type", "start"), new MNode("waypoint", MNode.Q(AirTarget)), "{clone}", new MNode("approach", "teleport"))),
                            new MNode(MNode.Q("delay"), new MNode("time", "0.3")))));
                    acts.Add(new MNode(MNode.Q("entity_state"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("ctb_air")), new MNode("exclude", MNode.Tag("hidden"), MNode.Tag("un"), MNode.Tag("ctb_air_new"), MNode.Tag("ctb_strike")))), new MNode("tag_add", "ctb_strike")));
                    acts.AddRange(FindMe());
                    acts.Add(MNode.Call("ctb_mine"));
                    acts.Add(MNode.IfVar(MineVar, "==", 0, SetPlayer("ctb_strike", 0)));
                    for (int m = 0; m <= 9; m++) acts.Add(MNode.IfVar(MineVar, "==", m + 1, SetPlayer("ctb_strike", m)));
                    acts.Add(new MNode(MNode.Q("air_attack"), Tagged("ctb_strike"), new MNode("drop", MNode.Q("orders sensor senseless")), new MNode("enemies", new MNode("ignore_captured_by_user", "0")), new MNode("waypoint_to_attack", MNode.Q(AirTarget)), new MNode("attack_altitude", "30")));
                    acts.Add(Done("ctb_strike"));
                    Once(ev, acts.ToArray());
                }
            string pe = "ctb_e_points";
            Add(s, Str("points", "Reinforcement points +1000"), ArmyIcon("inf_crate_{0}", army, "artystrike"), pe);
            Once(pe, new MNode(MNode.Q("reinforcement"), new MNode("operation", "score_add"), new MNode("score", "1000")));
        }

        static MNode Mine() { return new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("include", new MNode("state", new MNode("state", "user_control"))), new MNode("exclude", MNode.Tag("hidden")))); }
        static MNode Live() { return new MNode("exclude", MNode.Tag("hidden"), new MNode("state", new MNode("state", "inactive"))); }

        // 도구: 무적 켜기·끄기, 등급 최대(내 유닛 전체·찍은 자리의 한 유닛), 생성 시 최고 등급 켜기·끄기, 삭제(찍은 자리의 한 유닛·비행기 전부), 잔해 정리
        void MakeTools(MenuScreen s, string army)
        {
            // 그림(AI 판단, 2026-10-18 가독성): 무적 위생병, 등급 장교, 생성 시 등급 교리, 삭제·잔해 공병, 비행기 삭제 항공 폭격
            string medic = ArmyIcon("single_medic({0})", army, "back"), officer = ArmyIcon("single_officer({0})", army, "back"), doctrine = ArmyIcon("doctrine({0})", army, "back");
            string engineer = ArmyIcon("single_engineer({0})", army, "back"), plane = ArmyIcon("air_strike_{0}", army, "back");
            void Tool(string key, string label, string image, MNode[] actions, string waypoint = null, string preview = null)
            {
                string ev = "ctb_e_" + key;
                Add(s, Str(key, label), image, ev, waypoint, preview);
                Once(ev, actions);
            }
            Tool("god_on", "God mode on", medic, new[] { MNode.SetVar("ctb_god", 1) });
            Tool("god_off", "God mode off", medic, new[] { MNode.SetVar("ctb_god", 0), new MNode(MNode.Q("entity_state"), Mine(), new MNode("impregnability", "disabled")) });
            Tool("vet_all", "Max veterancy (all mine)", officer, new[] { new MNode(MNode.Q("actor_state"), Mine(), new MNode("human", new MNode("veterancy", "8"))) });
            // 찍은 자리 둘레의 내 병사(탄 승무원 포함): 미션 선택자에 반경 거르기가 없어(바닐라 include 종류 15가지에 없음) 찍은 자리에서 가까운 내 병사 VetAround명으로 한다
            Tool("vet_one", "Max veterancy (my soldiers around point)", officer, new[] { new MNode(MNode.Q("actor_state"), new MNode("selector", new MNode("source", "advanced"),
                new MNode("group", new MNode("include", new MNode("state", new MNode("state", "user_control")), new MNode("prop", new MNode("prop", "human"))), new MNode("exclude", MNode.Tag("hidden"), new MNode("state", new MNode("state", "dead")))),
                new MNode("amount", VetAround.ToString()), new MNode("sort", new MNode("type", "waypoint"), new MNode("wpid", MNode.Q(Target)))), new MNode("human", new MNode("veterancy", "8"))) }, Target, "pointer_blue");
            Tool("maxvet_on", "Spawn at max veterancy on", doctrine, new[] { MNode.SetVar("ctb_maxvet", 1) });
            Tool("maxvet_off", "Spawn at max veterancy off", doctrine, new[] { MNode.SetVar("ctb_maxvet", 0) });
            Tool("del_one", "Delete unit (pick)", engineer, new[] { new MNode(MNode.Q("delete"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("include", new MNode("prop", new MNode("prop", "human"))), Live()), new MNode("group", new MNode("include", new MNode("prop", new MNode("prop", "vehicle"))), Live()), new MNode("amount", "1"), new MNode("sort", new MNode("type", "waypoint"), new MNode("wpid", MNode.Q(Target))))) }, Target, "pointer");
            Tool("del_air", "Delete all spawned aircraft", plane, new[] { new MNode(MNode.Q("delete"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("ctb_air")), Live()))) });
            Tool("wrecks", "Clear wrecks", engineer, new[] { new MNode(MNode.Q("delete"), new MNode("selector", new MNode("tag", "_bleedout_"), new MNode("type", "human"))), new MNode(MNode.Q("delete"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("no_scavenge"), MNode.Tag("destroyed")), new MNode("include", new MNode("prop", new MNode("prop", "vehicle")))))) });
        }

        // 함수: 소속(cheat.mi "set player"), 차량 놓기("vk c"), 보병 놓기("hm c"), 비행기 놓기("fly c")
        static MNode Zone(string zone) { return new MNode("zone", new MNode("zone", MNode.Q(zone))); }

        static MNode InZoneHumans() { return new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", Zone("infantryzone")), new MNode("include", new MNode("prop", new MNode("prop", "human"))), new MNode("exclude", MNode.Tag("hidden"), new MNode("state", new MNode("state", "dead"))))); }
        static MNode Tagged(string tag) { return new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag(tag)), new MNode("exclude", MNode.Tag("hidden"), MNode.Tag("un")))); }

        // 소속 정하기. 새로 만든 유닛에 ctb_own 태그를 잠시 붙여 한 번에 정한다.
        // 나(11): 판 안에서 찾은 내 번호. 내 번호는 "player k의 유닛 가운데 user_control 상태인 것이 있다"로 찾는다
        //   (고급 선택자 {include {player {player "k"}}}와 {state {state user_control}}, 바닐라 미션 1,882·여러 회. "if" {condition {type entities}}는 바닐라 3,288회).
        //   못 찾으면 player 0(바닐라 캠페인의 사용자 번호, {player "0"} 2,774회).
        // 아군 AI(12)·적 AI(13): 내 번호 m에 대해 {relation {relation ally|enemy} {player "m"}}인 유닛이 있는 첫 번호 j(j ≠ m, 바닐라 732회). 못 찾으면 autoassign.
        // 사용자 지적(2026-10-18): Player 0~9로는 내가 몇 번인지 모른다. 설계도 "내 소속은 Auto Assign이나 고정 번호로 정하지 않는다"였다
        static MNode SetPlayer(string tag, int player) { return new MNode(MNode.Q("player"), Tagged(tag), new MNode("operation", "set"), new MNode("player", MNode.Q(player.ToString()))); }

        static MNode IfAny(MNode group, params object[] actions)
        {
            return new MNode(MNode.Q("if"), new MNode("condition", new MNode("type", "entities"), new MNode("selector", new MNode("source", "advanced"), new MNode("ignore_captured_by_user", "0"), group))).Add(actions);
        }

        static MNode PlayerUnits(int k, MNode filter)
        {
            var include = new MNode("include", new MNode("player", new MNode("player", MNode.Q(k.ToString()))));
            if (filter != null) include.Add(filter);
            return new MNode("group", include, new MNode("exclude", MNode.Tag("hidden"), new MNode("state", new MNode("state", "dead"))));
        }

        // 내 번호를 ctb_me에 적는다(이미 알면 그대로)
        static IEnumerable<object> FindMe()
        {
            for (int k = 0; k <= 9; k++)
                yield return MNode.IfVar(Me, "==", 0, IfAny(PlayerUnits(k, new MNode("state", new MNode("state", "user_control"))), MNode.SetVar(Me, k + 1)));
        }

        static IEnumerable<object> Relative(string tag, string relation)
        {
            for (int m = 0; m <= 9; m++)
            {
                var block = new List<object> { MNode.SetVar(Found, 0) };
                for (int j = 0; j <= 9; j++)
                    if (j != m) block.Add(MNode.IfVar(Found, "==", 0, IfAny(PlayerUnits(j, new MNode("relation", new MNode("relation", relation), new MNode("player", MNode.Q(m.ToString())))), SetPlayer(tag, j), MNode.SetVar(Found, 1))));
                block.Add(MNode.IfVar(Found, "==", 0, new MNode(MNode.Q("autoassign"), Tagged(tag))));
                yield return MNode.IfVar(MineVar, "==", m + 1, block.ToArray());
            }
            yield return MNode.IfVar(MineVar, "==", 0, new MNode(MNode.Q("autoassign"), Tagged(tag)));
        }

        static MNode OwnerFunction(string[] tags)
        {
            const string own = "ctb_own";
            var f = new MNode(MNode.Q("ctb_owner"));
            foreach (string t in tags) f.Add(new MNode(MNode.Q("entity_state"), Tagged(t), new MNode("tag_add", own)));
            f.Add(FindMe().ToArray());
            f.Add(MNode.Call("ctb_mine"));
            f.Add(MNode.IfVar("ctb_owner", "==", 0, new MNode(MNode.Q("autoassign"), Tagged(own))));
            for (int k = 1; k <= 10; k++) f.Add(MNode.IfVar("ctb_owner", "==", k, SetPlayer(own, k - 1)));
            var me = new List<object> { MNode.IfVar(MineVar, "==", 0, SetPlayer(own, 0)) };
            for (int m = 0; m <= 9; m++) me.Add(MNode.IfVar(MineVar, "==", m + 1, SetPlayer(own, m)));
            f.Add(MNode.IfVar("ctb_owner", "==", MeOwner, me.ToArray()));
            f.Add(MNode.IfVar("ctb_owner", "==", AllyOwner, Relative(own, "ally").ToArray()));
            f.Add(MNode.IfVar("ctb_owner", "==", EnemyOwner, Relative(own, "enemy").ToArray()));
            f.Add(new MNode(MNode.Q("actor_state"), Tagged(own), new MNode("control", "user")));
            f.Add(new MNode(MNode.Q("entity_state"), Tagged(own), new MNode("tag_remove", own)));
            return f;
        }

        // ctb_mine = ctb_me, 0이면 ctb_guess(2026-10-18 조사: 멀티·컨퀘스트 맵에는 시작 때 내 유닛이 없어 ctb_me를 못 찾는다. 멀티 사본 388개에 {control user} 0건)
        static MNode MineFunction()
        {
            var f = new MNode(MNode.Q("ctb_mine"), MNode.SetVar(MineVar, 0));
            for (int k = 1; k <= 10; k++) f.Add(MNode.IfVar(Me, "==", k, MNode.SetVar(MineVar, k)));
            for (int k = 1; k <= 10; k++) f.Add(MNode.IfVar(Me, "==", 0, MNode.IfVar(Guess, "==", k, MNode.SetVar(MineVar, k))));
            return f;
        }

        // ctb_has_<국가> = 그 국가 견본이 판에 있는가(국가 모드가 켜졌거나 캠페인 맵이 넣었다). 견본 하나의 태그로 개체를 직접 본다:
        // 견본 파일 안 트리거가 켜는 방식은 따로 include한 {Helpers}가 합쳐지는지 미확인이라서(2026-10-18 국가별 모드 설계 "함께 켜기")
        MNode CheckFunction()
        {
            var f = new MNode(MNode.Q("ctb_check"));
            foreach (var kv in probeTag.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                f.Add(MNode.SetVar(HasVar(kv.Key), 0));
                f.Add(IfAny(new MNode("group", new MNode("select", MNode.Tag(kv.Value))), MNode.SetVar(HasVar(kv.Key), 1)));
            }
            return f;
        }

        IEnumerable<MNode> Functions(MNode home)
        {
            yield return home;
            yield return MineFunction();
            yield return CheckFunction();
            yield return OwnerFunction(new[] { "ctb_v", "ctb_c", "ctb_air_new" });
            yield return new MNode(MNode.Q("ctb_vehicle"),
                new MNode(MNode.Q("entity_state"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", Zone("tankzone")), new MNode("exclude", new MNode("prop", new MNode("prop", "human")), new MNode("state", new MNode("state", "operatable")), new MNode("state", new MNode("state", "inhabited")), MNode.Tag("hidden")))), new MNode("tag_add", "ctb_v")),
                new MNode(MNode.Q("entity_state"), InZoneHumans(), new MNode("tag_add", "ctb_c")),
                MNode.Call("ctb_owner"),
                new MNode(MNode.Q("placement"), Tagged("ctb_v"), new MNode("target_waypoint", MNode.Q(Place))),
                new MNode(MNode.Q("placement"), Tagged("ctb_c"), new MNode("target_waypoint", MNode.Q(Place))),
                new MNode(MNode.Q("delay"), new MNode("time", "0.5")),
                new MNode(MNode.Q("linker"), Tagged("ctb_v"), new MNode("entities", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("ctb_c")), new MNode("exclude", MNode.Tag("hidden"), MNode.Tag("un"))))),
                MaxVet("ctb_c"),
                Done("ctb_v"), Done("ctb_c"));
            yield return new MNode(MNode.Q("ctb_infantry"),
                new MNode(MNode.Q("squad_create"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("include", Zone("infantryzone")), new MNode("exclude", MNode.Tag("hidden"))))),
                new MNode(MNode.Q("entity_state"), InZoneHumans(), new MNode("tag_add", "ctb_c")),
                MNode.Call("ctb_owner"),
                new MNode(MNode.Q("waypoint"), new MNode("who", new MNode("type", "actor"), new MNode("actors", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("ctb_c")), new MNode("include", Zone("infantryzone"))))), new MNode("action", new MNode("type", "start"), new MNode("waypoint", MNode.Q(Place)), "{make_squad}", new MNode("approach", "teleport"))),
                MaxVet("ctb_c"),
                Done("ctb_c"));
            yield return new MNode(MNode.Q("ctb_plane"),
                new MNode(MNode.Q("entity_state"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("ctb_air")), new MNode("exclude", MNode.Tag("hidden"), MNode.Tag("un"), MNode.Tag("ctb_air_new")))), new MNode("tag_add", "ctb_air_new")),
                MNode.Call("ctb_owner"),
                new MNode(MNode.Q("actor_state"), Tagged("ctb_air_new"), new MNode("move_mode", "hold")),
                Done("ctb_air_new"));
        }

        static MNode MaxVet(string tag) { return MNode.IfVar("ctb_maxvet", "==", 1, new MNode(MNode.Q("actor_state"), Tagged(tag), new MNode("human", new MNode("veterancy", "8")))); }
        static MNode Done(string tag) { return new MNode(MNode.Q("entity_state"), Tagged(tag), new MNode("tag_add", "un"), new MNode("tag_remove", tag)); }

        // 늘 도는 트리거: 시작(국가·소속 기본값), 메뉴 유지(30초마다 맨 위 단추를 다시 보인다), 무적(5초마다 다시 건다)
        IEnumerable<MNode> Loops(List<string> armies)
        {
            var start = new MNode("actions", new MNode(MNode.Q("delay"), new MNode("time", "1")));
            var sw = new MNode(MNode.Q("switch"));
            foreach (string army in armies)
                sw.Add(new MNode(MNode.Q("case"), new MNode("condition", new MNode("type", "side"), new MNode("side", army)), MNode.SetVar("ctb_nation_side", armies.IndexOf(army) + 1)));
            sw.Add(new MNode(MNode.Q("default"), MNode.SetVar("ctb_nation_side", 1)));
            start.Add(MNode.IfVar("ctb_campaign", "==", 0, sw, MNode.SetVar("ctb_nation", 0)));
            // ctb_nation_side를 ctb_nation으로 옮긴다(캠페인은 nation_<국가>.mi가 이미 정했다)
            foreach (string army in armies) start.Add(MNode.IfVar("ctb_campaign", "==", 0, MNode.IfVar("ctb_nation_side", "==", armies.IndexOf(army) + 1, MNode.SetVar("ctb_nation", armies.IndexOf(army) + 1))));
            // 그 국가 견본이 없으면(국가 모드가 꺼짐) 견본이 있는 첫 국가로, 하나도 없으면 0(안내 화면). 2026-10-18 국가별 모드 설계 "함께 켜기"
            start.Add(MNode.Call("ctb_check"));
            foreach (string army in armies) start.Add(MNode.IfVar("ctb_nation", "==", armies.IndexOf(army) + 1, MNode.IfVar(HasVar(army), "==", 0, MNode.SetVar("ctb_nation", 0))));
            foreach (string army in armies) start.Add(MNode.IfVar("ctb_nation", "==", 0, MNode.IfVar(HasVar(army), "==", 1, MNode.SetVar("ctb_nation", armies.IndexOf(army) + 1))));
            // 소속 기본값은 나(설계: 처음에 자동으로 정한다). 처음엔 맨 위 Spawn 단추만 보인다(설계 "첫 화면", 사용자 지적 2026-10-18: 처음부터 다 펼쳐져 있다)
            start.Add(MNode.SetVar("ctb_owner", MeOwner));
            start.Add(MNode.Hide(HideAll));
            start.Add(MNode.SetVar(Open, 0));
            start.Add(MNode.Show(Root));
            yield return new MNode(MNode.Q("ctb_start"), "{condition }", start);
            // 내 번호를 판 안에서 찾을 때까지 3초마다 본다(스커미시는 처음에 내 유닛이 없을 수 있다)
            var find = new MNode("actions", new MNode(MNode.Q("delay"), new MNode("time", "3")));
            find.Add(FindMe().ToArray());
            yield return new MNode(MNode.Q("ctb_find_me"), "{autoreset}", new MNode("condition", new MNode("terms", new MNode(MNode.Q("1.cmp_i"), new MNode("var", MNode.Q(Me + "$")), new MNode("op", MNode.Q("==")), new MNode("value", "0")))), find);
            yield return new MNode(MNode.Q("ctb_keep_menu"), "{autoreset}", new MNode("condition", new MNode("terms", new MNode(MNode.Q("1.cmp_i"), new MNode("var", MNode.Q("ctb_never$")), new MNode("op", MNode.Q("==")), new MNode("value", "0")))),
                new MNode("actions", new MNode(MNode.Q("delay"), new MNode("time", "30")), MNode.Show(Root)));
            yield return new MNode(MNode.Q("ctb_god_loop"), "{autoreset}", new MNode("condition", new MNode("terms", new MNode(MNode.Q("1.cmp_i"), new MNode("var", MNode.Q("ctb_god$")), new MNode("op", MNode.Q("==")), new MNode("value", "1")))),
                new MNode("actions", new MNode(MNode.Q("entity_state"), Mine(), new MNode("impregnability", "full")), new MNode(MNode.Q("delay"), new MNode("time", "5"))));
        }

        string Render(List<string> armies, MNode home)
        {
            var reinf = new MNode("reinforcements");
            reinf.Add(Button(new MenuButton { Name = Root, Text = Str("spawn", "Spawn"), Image = "button_1_ger", Event = "ctb_e_root" }, true));
            foreach (var s in result.Screens) foreach (var b in s.Buttons) reinf.Add(Button(b, false));
            var vars = new MNode("vars", new[] { "ctb_nation", "ctb_nation_side", "ctb_campaign", "ctb_owner", "ctb_god", "ctb_maxvet", "ctb_never", Open, Me, MineVar, Guess, Found }.Concat(armies.Select(HasVar)).Select(v => (object)new MNode(MNode.Q(v))).ToArray());
            var funcs = new MNode("functions").Add(Functions(home).ToArray());
            var trig = new MNode("triggers").Add(Loops(armies).ToArray()).Add(triggers.ToArray());
            var zones = new[] { ("infantryzone", 77777), ("tankzone", 88888) }.Select(z => new MNode("zone " + MNode.Q("poly"), new MNode("position", z.Item2 + " " + z.Item2 + " 0"), new MNode("Name", MNode.Q(z.Item1)),
                new MNode("Point", "-105 -105"), new MNode("Point", "-105 105"), new MNode("Point", "105 105"), new MNode("Point", "105 -105"))).ToArray();
            var waypoints = new MNode("waypoints " + MNode.Q("ctb_waypoints"),
                new MNode(MNode.Q(WaitInf), new MNode("position", "77777 77777 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(WaitVeh), new MNode("position", "88888 88888 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(Place), new MNode("position", "-9553.58 7012.8 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(Target), new MNode("position", "-9546.01 7248.6 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(AirTarget), new MNode("position", "-9546.01 7248.6 0"), new MNode("radius", "1")));
            foreach (var b in Barrages) waypoints.Add(new MNode(MNode.Q("ctb_wp_" + b.Key), new MNode("position", "-9546.01 7248.6 0"), new MNode("radius", b.Radius.ToString())));
            var helpers = new MNode("Helpers").Add(events.Select(e => (object)new MNode("event", MNode.Q(e))).ToArray()).Add(zones).Add(reinf, vars, funcs, trig, waypoints);
            result.Templates = humanTemplates.Count;
            result.Triggers = triggers.Count + 3;
            var sb = new StringBuilder();
            sb.Append(";{mission\r\n; CTAToybox Spawn: editor/CtaToybox.Core/Spawn/MenuBuilder.cs가 만든 파일이다. 손으로 고치지 않는다\r\n");
            helpers.Write(sb, 1);
            foreach (string army in armies) result.TemplateFiles[army] = TemplateFile(army);
            return sb.ToString();
        }

        // 국가 견본 파일: 맵이 menu.mi 앞에 읽는다. 견본과 함께 "이 맵에 이 국가 견본이 있다"는 변수를 켠다
        string TemplateFile(string army)
        {
            var t = Of(army);
            var sb = new StringBuilder(";{mission\r\n; CTAToybox Spawn: " + army + " 견본. editor/CtaToybox.Core/Spawn/MenuBuilder.cs가 만든 파일이다. 손으로 고치지 않는다\r\n");
            sb.Append(t.Objects).Append(t.Links).Append(t.Tags);
            new MNode("Helpers", new MNode("triggers", new MNode(MNode.Q("ctb_has_" + army), "{condition }", new MNode("actions", MNode.SetVar(HasVar(army), 1))))).Write(sb, 1);
            return sb.ToString();
        }

        static MNode Button(MenuButton b, bool visible)
        {
            var n = new MNode(MNode.Q(b.Name), new MNode("state", visible ? MNode.Q("-") : "hidden"), new MNode("text", MNode.Q(b.Text)), new MNode("image", MNode.Q(b.Image)), new MNode("hint", MNode.Q(b.Text)),
                new MNode("count", "-1"), new MNode("score", "0"), new MNode("delay", "0"), new MNode("eventStart", MNode.Q(b.Event)));
            if (b.Waypoint != null) n.Add(new MNode("waypoint", MNode.Q(b.Waypoint)));
            if (b.Preview != null) n.Add(new MNode("previewEntity", b.Preview));
            return n;
        }

        // 컨퀘스트 맵(dcg_vars.inc를 읽는 맵)이 menu.mi 앞에 읽는 조각: 바닐라 Lua가 판 시작 때 넣는 id_1st_player
        // (script/multiplayer/modes/conquest/runtime_context.lua 32행 SetVar("id_1st_player", BotApi.Conquest.FirstPlayerId), 선언 multi/dcg_vars.inc 35행)를
        // ctb_guess(번호 + 1)에 옮긴다. ctb_me를 찾기 전까지 3초마다. 미확인: FirstPlayerId가 이 PC 사용자인지(2026-10-18 조사)
        public static string ConquestFile()
        {
            var acts = new MNode("actions", new MNode(MNode.Q("delay"), new MNode("time", "3")));
            for (int k = 0; k <= 9; k++) acts.Add(MNode.IfVar("id_1st_player", "==", k, MNode.SetVar(Guess, k + 1)));
            var trig = new MNode(MNode.Q("ctb_guess_me"), "{autoreset}", new MNode("condition", new MNode("terms", new MNode(MNode.Q("1.cmp_i"), new MNode("var", MNode.Q(Me + "$")), new MNode("op", MNode.Q("==")), new MNode("value", "0")))), acts);
            var sb = new StringBuilder(";{mission\r\n; CTAToybox Spawn: 컨퀘스트 내 번호 추정. editor/CtaToybox.Core/Spawn/MenuBuilder.cs가 만든 파일이다\r\n");
            new MNode("Helpers", new MNode("triggers", trig)).Write(sb, 1);
            return sb.ToString();
        }

        // 캠페인 맵이 menu.mi 앞에 읽는 작은 조각: 그 캠페인의 국가를 정한다
        public static string NationFile(string army, List<string> armies)
        {
            int index = armies.IndexOf(army) + 1;
            var trig = new MNode(MNode.Q("ctb_campaign_nation"), "{condition }", new MNode("actions", MNode.SetVar("ctb_campaign", 1), MNode.SetVar("ctb_nation", index <= 0 ? 1 : index)));
            var sb = new StringBuilder(";{mission\r\n");
            new MNode("Helpers", new MNode("triggers", trig)).Write(sb, 1);
            return sb.ToString();
        }

        public static string Pot(MenuResult r)
        {
            var sb = new StringBuilder();
            sb.Append("msgid \"\"\nmsgstr \"\"\n\"Project-Id-Version: CTAToybox Spawn\\n\"\n\"Language: en\\n\"\n\"MIME-Version: 1.0\\n\"\n\"Content-Type: text/plain; charset=UTF-8\\n\"\n\"Content-Transfer-Encoding: 8bit\\n\"\n");
            foreach (var kv in r.Strings.OrderBy(k => k.Key, StringComparer.Ordinal))
                sb.Append("\nmsgctxt \"").Append(kv.Key).Append("\"\nmsgid \"").Append(kv.Value.Replace("\"", "\\\"")).Append("\"\nmsgstr \"\"\n");
            return sb.ToString();
        }
    }
}
