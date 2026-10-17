// 증원 창 메뉴 menu.mi(스펙 "캠페인·컨퀘스트: 증원 창 메뉴", "옵션").
// 꼴은 Spawn Menu의 cheat.mi를 따른다(docs/13-spawn-menu-spike.md, 2026-10-17에 원문을 읽어 맞춤):
//  - 견본: 맵 밖(-9600, 9700 근처)에 숨긴 {Human "<병종>" 0x<id> ...}와 {Tags "hidden" "<국가>" "<태그>" 0x<id>}. 비행기는 {Entity}+조종사+{Link}.
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
        public int GridCols { get; set; } = 7;   // 증원 창 단추 칸 열 수(가로 격자). 0이면 바닐라 세로 목록 그대로
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
    }

    public sealed class MenuBuilder
    {
        public const string Root = "ctb_root", HideAll = "ctb_s*", Place = "ctb_place", Target = "ctb_target", WaitInf = "ctb_wait_inf", WaitVeh = "ctb_wait_veh";
        public const string PotKey = "mission/ctatoybox_spawn/";
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
        readonly List<string> skipped;
        int nextScreen = 1;
        int nextUnit = 1;

        // 견본
        readonly Dictionary<string, (int Id, string Tag)> humanTemplates = new Dictionary<string, (int, string)>(StringComparer.OrdinalIgnoreCase);
        readonly StringBuilder templates = new StringBuilder();
        readonly StringBuilder tagLines = new StringBuilder();
        readonly StringBuilder linkLines = new StringBuilder();
        int nextId = 0xC100;

        MenuBuilder(SpawnCatalog cat, MenuOptions o, Func<string, bool> hasIcon, Func<string, bool> hasBreed, List<string> skipped)
        {
            this.cat = cat; this.o = o ?? new MenuOptions(); this.hasIcon = hasIcon ?? (_ => true); this.hasBreed = hasBreed ?? (_ => true); this.skipped = skipped;
        }

        public static List<string> Armies(SpawnCatalog cat)
        {
            var present = new HashSet<string>(cat.Units.Select(u => u.Army).Concat(cat.Squads.Select(s => s.Army)), StringComparer.Ordinal);
            return ArmyOrder.Where(present.Contains).Concat(present.Where(a => !ArmyOrder.Contains(a)).OrderBy(a => a, StringComparer.Ordinal)).ToList();
        }

        // hasIcon: interface/scene/unit_icon/<이름>_00.tga가 계층에 있는가. hasBreed: set/breed/<병종>.set이 있는가
        public static MenuResult Build(SpawnCatalog cat, MenuOptions o, Func<string, bool> hasIcon, Func<string, bool> hasBreed, List<string> skipped)
        {
            var b = new MenuBuilder(cat, o, hasIcon, hasBreed, skipped);
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

        void Make()
        {
            var armies = Armies(cat);
            var nationScreen = NewScreen(null, "Nation");
            var ownerScreen = NewScreen(null, "Owner");
            var supportScreen = NewScreen(null, "Support");
            var toolsScreen = NewScreen(null, "Tools");
            foreach (string army in armies) homes[army] = NewScreen(army, "Home").Id;

            foreach (string army in armies) MakeArmy(army, result.Screens.Single(s => s.Id == homes[army]), nationScreen, ownerScreen, supportScreen, toolsScreen);

            // 국가 화면: 바로 고르는 단추(토글 아님, 사용자 결정)
            foreach (string army in armies)
            {
                string ev = "ctb_e_nat_" + army;
                Add(nationScreen, Str("nation_" + army, ArmyText.TryGetValue(army, out var t) ? t : army), Icon("button_1_" + army, "button_1_ger"), ev);
                events.Add(ev);
                triggers.Add(MNode.EventTrigger(ev, MNode.SetVar("ctb_nation", armies.IndexOf(army) + 1), MNode.Hide(HideAll), MNode.Show("ctb_s" + homes[army] + "_*")));
            }
            // 소속 화면: Auto(autoassign)와 Player 0~9(바로 고름). 값 0 = autoassign, k = player k-1(cheat.mi "set player"와 같은 값)
            for (int k = 0; k <= 10; k++)
            {
                string ev = "ctb_e_own_" + k;
                Add(ownerScreen, Str("owner_" + k, k == 0 ? "Auto assign" : "Player " + (k - 1)), "back", ev);
                events.Add(ev);
                triggers.Add(MNode.EventTrigger(ev, MNode.SetVar("ctb_owner", k), MNode.Call("ctb_home")));
            }
            foreach (var shared in new[] { nationScreen, ownerScreen, supportScreen, toolsScreen }) AddNav(shared, null);
            MakeSupport(supportScreen);
            MakeTools(toolsScreen);

            // 처음 화면으로: 지금 국가의 처음 화면
            var home = new MNode(MNode.Q("ctb_home"), MNode.Hide(HideAll));
            foreach (string army in armies) home.Add(MNode.IfVar("ctb_nation", "==", armies.IndexOf(army) + 1, MNode.Show("ctb_s" + homes[army] + "_*")));
            events.Add("ctb_e_root"); events.Add("ctb_e_home");
            triggers.Add(MNode.EventTrigger("ctb_e_root", MNode.Call("ctb_home")));
            triggers.Add(MNode.EventTrigger("ctb_e_home", MNode.Call("ctb_home")));
            result.Text = Render(armies, home);
        }

        void AddNav(MenuScreen s, MenuScreen parent)
        {
            if (parent != null) Add(s, Str("back", "Back"), "back", ShowEvent(parent));
            Add(s, Str("home", "Home"), "back", "ctb_e_home");
        }

        void MakeArmy(string army, MenuScreen home, MenuScreen nation, MenuScreen owner, MenuScreen support, MenuScreen tools)
        {
            Add(home, Str("nation", "Nation"), Icon("button_1_" + army, "button_1_ger"), ShowEvent(nation));
            Add(home, Str("owner", "Owner"), Icon("doctrine(" + army + ")", "back"), ShowEvent(owner));
            var units = cat.Units.Where(u => u.Army == army).ToList();
            var squads = cat.Squads.Where(s => s.Army == army).ToList();

            if (squads.Count > 0)
            {
                var inf = NewScreen(army, "Infantry");
                Add(home, Str("infantry", "Infantry"), Icon("button_1_" + army, "button_1_ger"), ShowEvent(inf));
                foreach (var (button, label) in new[] { ("inf1", "Squads"), ("inf2", "Single soldiers") })
                {
                    var list = squads.Where(s => s.Button == button).ToList();
                    if (list.Count == 0) continue;
                    var page = Pages(army, inf, label, list.Select(s => (Group: s.Period, Item: (object)s)).ToList(), g => Str("period_" + g, char.ToUpperInvariant(g[0]) + g.Substring(1)));
                    Add(inf, Str(button == "inf1" ? "squads" : "single", label), Icon("button_" + (button == "inf1" ? 1 : 2) + "_" + army, "button_1_ger"), ShowEvent(page));
                }
                AddNav(inf, home);
            }
            foreach (var (cats, label, icon) in new[] { (new[] { "empl" }, "Guns", 3), (new[] { "car" }, "Vehicles", 4) })
            {
                var list = units.Where(u => cats.Contains(u.Category)).ToList();
                if (list.Count == 0) continue;
                var page = Pages(army, home, label, list.Select(u => (Group: u.Family, Item: (object)u)).ToList(), g => Str("family_" + g, g));
                Add(home, Str(label.ToLowerInvariant(), label), Icon("button_" + icon + "_" + army, "button_1_ger"), ShowEvent(page));
            }
            var tankCats = new[] { "light", "medium", "heavy", "td" };
            if (units.Any(u => tankCats.Contains(u.Category)))
            {
                var tanks = NewScreen(army, "Tanks");
                Add(home, Str("tanks", "Tanks"), Icon("button_5_" + army, "button_1_ger"), ShowEvent(tanks));
                foreach (string c in tankCats)
                {
                    var list = units.Where(u => u.Category == c).ToList();
                    if (list.Count == 0) continue;
                    var page = Pages(army, tanks, CategoryText[c], list.Select(u => (Group: u.Family, Item: (object)u)).ToList(), g => Str("family_" + g, g));
                    Add(tanks, Str("cat_" + c, CategoryText[c]), Icon(list[0].Name, "button_5_ger"), ShowEvent(page));
                }
                AddNav(tanks, home);
            }
            var air = units.Where(u => u.Category == "air").ToList();
            if (air.Count > 0)
            {
                var page = Pages(army, home, "Aircraft", air.Select(u => (Group: u.Family, Item: (object)u)).ToList(), g => Str("family_" + g, g));
                Add(home, Str("aircraft", "Aircraft"), Icon("air_strike_" + army, "air_strike_ger"), ShowEvent(page));
            }
            Add(home, Str("support", "Support"), "artystrike", ShowEvent(support));
            Add(home, Str("tools", "Tools"), "back", ShowEvent(tools));
        }

        // 유닛 목록 화면. 한도를 넘으면 묶음 화면을 한 단계 두고, 묶음 하나나 묶음 목록이 넘으면 Next/Prev로 나눈다
        MenuScreen Pages(string army, MenuScreen parent, string title, List<(string Group, object Item)> items, Func<string, string> groupText)
        {
            if (items.Count <= o.PageSize) return ItemPages(army, parent, title, items.Select(i => i.Item).ToList());
            var groups = items.GroupBy(i => i.Group ?? "other").OrderBy(g => g.Key, StringComparer.Ordinal).ToList();
            if (groups.Count == 1) return ItemPages(army, parent, title, items.Select(i => i.Item).ToList());
            var screens = Paged(army, parent, title, groups.Count, (screen, index) =>
            {
                var g = groups[index];
                var sub = ItemPages(army, screen, title + " / " + g.Key, g.Select(i => i.Item).ToList());
                string image = g.First().Item is SpawnUnit u ? Icon(u.Name, "button_5_ger") : Icon("button_1_" + army, "button_1_ger");
                Add(screen, groupText(g.Key), image, ShowEvent(sub));
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
                AddUnit(screen, "desc/squad/" + sq.MenuName, Icon(null, "button_" + (sq.Button == "inf2" ? 2 : 1) + "_" + army), ev, acts);
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
            AddUnit(screen, "desc/entity/" + u.Name, Icon(u.Name, CategoryIcon(u.Category, army)), evu, actions);
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
            templates.Append("\t{Human ").Append(MNode.Q(breed)).Append(" 0x").Append(id.ToString("x")).Append("\r\n")
                .Append("\t\t{Position ").Append(F(x)).Append(' ').Append(F(y)).Append("}\r\n")
                .Append("\t\t{Volume \"ram\"\r\n\t\t\t{able {visible 0}{bullet 0}{throwing 0}{obstacle 0}{contact 0}{contact_ground 0}{blast 0}{select 0}{touch 0}{blockcamera 0}}\r\n\t\t\t{disabled}\r\n\t\t}\r\n")
                .Append("\t\t{Player 0}\r\n\t}\r\n");
            tagLines.Append("\t{Tags \"hidden\" ").Append(MNode.Q(army)).Append(' ').Append(MNode.Q(tag)).Append(" 0x").Append(id.ToString("x")).Append("}\r\n");
            humanTemplates[breed] = (id, tag);
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
            templates.Append("\t{Entity ").Append(MNode.Q(u.Name)).Append(" 0x").Append(ent.ToString("x")).Append("\r\n")
                .Append("\t\t{Position ").Append(F(x)).Append(' ').Append(F(y)).Append("}\r\n\t\t{DisableObstacles}\r\n\t\t{Player 0}\r\n")
                .Append("\t\t{Chassis \"airborne\" {AirborneMode 1} {Altitude 40}}\r\n\t\t{ChassisManager {Current \"airborne\"}}\r\n\t}\r\n")
                .Append("\t{Human ").Append(MNode.Q(pilot)).Append(" 0x").Append(man.ToString("x")).Append("\r\n")
                .Append("\t\t{Position ").Append(F(x + 4)).Append(' ').Append(F(y)).Append("}\r\n")
                .Append("\t\t{Volume \"ram\"\r\n\t\t\t{able {visible 0}{bullet 0}{throwing 0}{obstacle 0}{contact 0}{contact_ground 0}{blast 0}{select 0}{touch 0}{blockcamera 0}}\r\n\t\t\t{disabled}\r\n\t\t}\r\n")
                .Append("\t\t{Player 0}\r\n\t}\r\n");
            linkLines.Append("\t{Link 0x").Append(man.ToString("x")).Append(" {0x").Append(ent.ToString("x")).Append(" \"driver\"}}\r\n");
            tagLines.Append("\t{Tags ").Append(MNode.Q(tag)).Append(" \"hidden\" ").Append(MNode.Q(army)).Append(" \"ctb_air\" 0x").Append(ent.ToString("x")).Append("}\r\n");
            var actions = new List<object>
            {
                new MNode(MNode.Q("waypoint"), new MNode("who", new MNode("type", "actor"), new MNode("actors", new MNode("tag", MNode.Q(tag)))), new MNode("action", new MNode("type", "start"), new MNode("waypoint", MNode.Q(Place)), "{clone}", new MNode("approach", "teleport"))),
                MNode.Call("ctb_plane"),
            };
            AddUnit(screen, "desc/entity/" + u.Name, Icon(u.Name, CategoryIcon("air", army)), ev, actions);
        }

        // 지원: 포격 소·중·대(곡사포 더미 1·3·6발, 바닐라 kollaa 43455 꼴), 증원 점수
        void MakeSupport(MenuScreen s)
        {
            foreach (var (key, label, shots) in new[] { ("arty_s", "Artillery (small)", 1), ("arty_m", "Artillery (medium)", 3), ("arty_l", "Artillery (large)", 6) })
            {
                string ev = "ctb_e_" + key;
                Add(s, Str(key, label), "150mm_sfh18_artillery_barrage", ev, Target, "crosshair");
                events.Add(ev);
                triggers.Add(MNode.EventTrigger(ev, new MNode(MNode.Q("loop") + " " + shots, new MNode(MNode.Q("spawn"), new MNode("entity", MNode.Q("artillery_single_150_dummy")), new MNode("waypoint", MNode.Q(Target))), new MNode(MNode.Q("delay"), new MNode("time", "1.5")))));
            }
            string pe = "ctb_e_points";
            Add(s, Str("points", "Reinforcement points +1000"), "artystrike", pe);
            events.Add(pe);
            triggers.Add(MNode.EventTrigger(pe, new MNode(MNode.Q("reinforcement"), new MNode("operation", "score_add"), new MNode("score", "1000"))));
        }

        static MNode Mine() { return new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("include", new MNode("state", new MNode("state", "user_control"))), new MNode("exclude", MNode.Tag("hidden")))); }
        static MNode Live() { return new MNode("exclude", MNode.Tag("hidden"), new MNode("state", new MNode("state", "inactive"))); }

        // 도구: 무적 켜기·끄기, 등급 최대(내 유닛 전체·찍은 자리의 한 유닛), 생성 시 최고 등급 켜기·끄기, 삭제(찍은 자리의 한 유닛·비행기 전부), 잔해 정리
        void MakeTools(MenuScreen s)
        {
            void Tool(string key, string label, string image, MNode[] actions, string waypoint = null, string preview = null)
            {
                string ev = "ctb_e_" + key;
                Add(s, Str(key, label), image, ev, waypoint, preview);
                events.Add(ev);
                triggers.Add(MNode.EventTrigger(ev, actions));
            }
            Tool("god_on", "God mode on", "back", new[] { MNode.SetVar("ctb_god", 1) });
            Tool("god_off", "God mode off", "back", new[] { MNode.SetVar("ctb_god", 0), new MNode(MNode.Q("entity_state"), Mine(), new MNode("impregnability", "disabled")) });
            Tool("vet_all", "Max veterancy (all mine)", "back", new[] { new MNode(MNode.Q("actor_state"), Mine(), new MNode("human", new MNode("veterancy", "8"))) });
            Tool("vet_one", "Max veterancy (pick unit)", "back", new[] { new MNode(MNode.Q("actor_state"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", Live()), new MNode("amount", "1"), new MNode("sort", new MNode("type", "waypoint"), new MNode("wpid", MNode.Q(Target)))), new MNode("human", new MNode("veterancy", "8"))) }, Target, "pointer_blue");
            Tool("maxvet_on", "Spawn at max veterancy on", "back", new[] { MNode.SetVar("ctb_maxvet", 1) });
            Tool("maxvet_off", "Spawn at max veterancy off", "back", new[] { MNode.SetVar("ctb_maxvet", 0) });
            Tool("del_one", "Delete unit (pick)", "back", new[] { new MNode(MNode.Q("delete"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("include", new MNode("prop", new MNode("prop", "human"))), Live()), new MNode("group", new MNode("include", new MNode("prop", new MNode("prop", "vehicle"))), Live()), new MNode("amount", "1"), new MNode("sort", new MNode("type", "waypoint"), new MNode("wpid", MNode.Q(Target))))) }, Target, "pointer");
            Tool("del_air", "Delete all spawned aircraft", "back", new[] { new MNode(MNode.Q("delete"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("ctb_air")), Live()))) });
            Tool("wrecks", "Clear wrecks", "back", new[] { new MNode(MNode.Q("delete"), new MNode("selector", new MNode("tag", "_bleedout_"), new MNode("type", "human"))), new MNode(MNode.Q("delete"), new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag("no_scavenge"), MNode.Tag("destroyed")), new MNode("include", new MNode("prop", new MNode("prop", "vehicle")))))) });
        }

        // 함수: 소속(cheat.mi "set player"), 차량 놓기("vk c"), 보병 놓기("hm c"), 비행기 놓기("fly c")
        static MNode Zone(string zone) { return new MNode("zone", new MNode("zone", MNode.Q(zone))); }

        static MNode InZoneHumans() { return new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", Zone("infantryzone")), new MNode("include", new MNode("prop", new MNode("prop", "human"))), new MNode("exclude", MNode.Tag("hidden"), new MNode("state", new MNode("state", "dead"))))); }
        static MNode Tagged(string tag) { return new MNode("selector", new MNode("source", "advanced"), new MNode("group", new MNode("select", MNode.Tag(tag)), new MNode("exclude", MNode.Tag("hidden"), MNode.Tag("un")))); }

        static MNode OwnerFunction(string[] tags)
        {
            var f = new MNode(MNode.Q("ctb_owner"));
            f.Add(MNode.IfVar("ctb_owner", "==", 0, tags.Select(t => (object)new MNode(MNode.Q("autoassign"), Tagged(t))).ToArray()));
            for (int k = 1; k <= 10; k++)
                f.Add(MNode.IfVar("ctb_owner", "==", k, tags.Select(t => (object)new MNode(MNode.Q("player"), Tagged(t), new MNode("operation", "set"), new MNode("player", MNode.Q((k - 1).ToString())))).ToArray()));
            foreach (string t in tags) f.Add(new MNode(MNode.Q("actor_state"), Tagged(t), new MNode("control", "user")));
            return f;
        }

        IEnumerable<MNode> Functions(MNode home)
        {
            yield return home;
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
            start.Add(MNode.IfVar("ctb_campaign", "==", 1, MNode.SetVar("ctb_owner", 1)));
            start.Add(MNode.Show(Root));
            start.Add(MNode.IfVar("ctb_campaign", "==", 0, MNode.Hide(HideAll), MNode.Show("ctb_s" + result.Screens.Single(s => s.Title == "Owner").Id + "_*")));
            yield return new MNode(MNode.Q("ctb_start"), "{condition }", start);
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
            var vars = new MNode("vars", new[] { "ctb_nation", "ctb_nation_side", "ctb_campaign", "ctb_owner", "ctb_god", "ctb_maxvet", "ctb_never" }.Select(v => (object)new MNode(MNode.Q(v))).ToArray());
            var funcs = new MNode("functions").Add(Functions(home).ToArray());
            var trig = new MNode("triggers").Add(Loops(armies).ToArray()).Add(triggers.ToArray());
            var zones = new[] { ("infantryzone", 77777), ("tankzone", 88888) }.Select(z => new MNode("zone " + MNode.Q("poly"), new MNode("position", z.Item2 + " " + z.Item2 + " 0"), new MNode("Name", MNode.Q(z.Item1)),
                new MNode("Point", "-105 -105"), new MNode("Point", "-105 105"), new MNode("Point", "105 105"), new MNode("Point", "105 -105"))).ToArray();
            var waypoints = new MNode("waypoints " + MNode.Q("ctb_waypoints"),
                new MNode(MNode.Q(WaitInf), new MNode("position", "77777 77777 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(WaitVeh), new MNode("position", "88888 88888 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(Place), new MNode("position", "-9553.58 7012.8 0"), new MNode("radius", "1")),
                new MNode(MNode.Q(Target), new MNode("position", "-9546.01 7248.6 0"), new MNode("radius", "1")));
            var helpers = new MNode("Helpers").Add(events.Select(e => (object)new MNode("event", MNode.Q(e))).ToArray()).Add(zones).Add(reinf, vars, funcs, trig, waypoints);
            result.Templates = humanTemplates.Count;
            result.Triggers = triggers.Count + 3;
            var sb = new StringBuilder();
            sb.Append(";{mission\r\n; CTAToybox Spawn: editor/CtaToybox.Core/Spawn/MenuBuilder.cs가 만든 파일이다. 손으로 고치지 않는다\r\n");
            sb.Append(templates).Append(linkLines).Append(tagLines);
            helpers.Write(sb, 1);
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
