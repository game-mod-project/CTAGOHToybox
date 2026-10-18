using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CtaToybox.Core.Spawn;

// 스폰 모드: 증원 창 메뉴와 옵션(Task 5, 6)
static class SpawnMenuTests
{
    // 메뉴 글의 짝 검사: 단추의 eventStart마다 {event} 선언과 같은 id의 트리거가 있다. 결과는 잘못된 것 목록
    public static List<string> Pairs(string menu)
    {
        var bad = new List<string>();
        var declared = new HashSet<string>(Regex.Matches(menu, "\\{event \"([^\"]+)\"\\}").Select(m => m.Groups[1].Value));
        var starts = Regex.Matches(menu, "\\{eventStart \"([^\"]+)\"\\}").Select(m => m.Groups[1].Value).ToList();
        var ids = new HashSet<string>(Regex.Matches(menu, "\\{id \"([^\"]+)\"\\}").Select(m => m.Groups[1].Value));
        foreach (string e in starts.Distinct()) { if (!declared.Contains(e)) bad.Add("not declared: " + e); if (!ids.Contains(e)) bad.Add("no trigger: " + e); }
        foreach (string id in ids) if (!declared.Contains(id)) bad.Add("trigger without event: " + id);
        var calls = Regex.Matches(menu, "\\{function \"([^\"]+)\"\\}").Select(m => m.Groups[1].Value).Distinct();
        foreach (string f in calls) if (!menu.Contains("{\"" + f + "\"\r\n") && !menu.Contains("{\"" + f + "\" ")) bad.Add("function missing: " + f);
        var shows = Regex.Matches(menu, "\\{name \"(ctb_s\\d+)_\\*\"\\}").Select(m => m.Groups[1].Value).Distinct();
        foreach (string s in shows) if (!menu.Contains("{\"" + s + "_b1\"")) bad.Add("empty screen: " + s);
        return bad;
    }

    public static void Run()
    {
        string repo, stat, spawn;
        var extra = new List<(string, string)>();
        for (int i = 0; i < 50; i++)
        {
            string fam = i < 30 ? "pz" : "stu";
            extra.Add(("entity/-vehicle/germany/tank_medium/" + fam + i.ToString("00") + "/m" + i.ToString("00") + ".def", "{game_entity\n (include \"/properties/tank.ext\")\n}\n"));
        }
        string settings = SpawnFixture.Make(out repo, out stat, out spawn, extra.ToArray());
        using var o = SpawnFixture.Open(settings);
        var cat = o.Catalog;
        T.True(cat.Units.Count(u => u.Army == "ger" && u.Category == "medium") >= 50, "fixture has many medium tanks: " + cat.Units.Count(u => u.Category == "medium"));
        var skipped = new List<string>();
        var r = MenuBuilder.Build(cat, new MenuOptions(), null, b => o.Stack.Find(SpawnCatalog.BreedFile(b)) != null, skipped);

        var unitTexts = r.Screens.Where(s => s.Army == "ger").SelectMany(s => s.Buttons).Where(b => b.Event.StartsWith("ctb_e_u") || b.Event.StartsWith("ctb_e_a")).Select(b => b.Text).ToList();
        var ger = cat.Units.Where(u => u.Army == "ger").Select(u => "desc/entity/" + u.Name).ToList();
        T.Eq(ger.OrderBy(x => x).ToArray(), unitTexts.OrderBy(x => x).ToArray(), "menu_pages_cover_all: every German unit once: missing " + string.Join(",", ger.Except(unitTexts)) + " skipped " + string.Join("; ", skipped));
        T.True(r.Screens.All(s => s.Buttons.Count <= 21 + 4), "page size plus navigation: " + r.Screens.Max(s => s.Buttons.Count));
        T.True(r.Screens.Any(s => s.Title == "Medium tanks") && r.Screens.Any(s => s.Title.StartsWith("Medium tanks / pz")), "an over-full list gets a family screen");
        T.True(r.Screens.Where(s => s.Army != null && s.Title != "Home").All(s => s.Buttons.Any(b => b.Event == "ctb_e_home")), "every screen has Home");
        T.True(r.Screens.Where(s => s.Army != null && s.Title != "Home").All(s => s.Buttons.Count(b => b.Event.StartsWith("ctb_e_s")) >= 1), "every screen has Back");
        var squad = r.Screens.SelectMany(s => s.Buttons).Single(b => b.Text == "desc/squad/sq_a(ger)");
        T.Eq("ctb_place", squad.Waypoint, "a unit button picks the place");

        string menu = r.Text;
        T.True(menu.StartsWith(";{mission\r\n"), "a fragment for the mission block");
        var parsed = SdlCore.Parse(menu);
        T.Eq(0, parsed.Errors.Count, "menu parses: " + string.Join("; ", parsed.Errors.Take(3)));
        T.Eq(0, Pairs(menu).Count, "buttons, events, triggers and functions pair up: " + string.Join("; ", Pairs(menu).Take(5)));
        var entities = Regex.Matches(menu, "\\{entity \"([^\"]+)\"\\}").Select(m => m.Groups[1].Value).Distinct().ToList();
        var known = new HashSet<string>(cat.Units.Select(u => u.Name).Concat(new[] { "artillery_single_150_dummy" }));
        T.True(entities.All(known.Contains), "every spawned entity is a known unit: " + string.Join(",", entities.Where(e => !known.Contains(e))));
        // 견본은 국가 파일에만 있다
        T.True(!menu.Contains("{Human ") && !menu.Contains("{Entity "), "no templates in menu.mi");
        T.Eq(MenuBuilder.Armies(cat).ToArray(), r.TemplateFiles.Keys.ToArray(), "a template file per menu nation");
        string templates = string.Concat(r.TemplateFiles.Values);
        var humans = Regex.Matches(templates, "\\{Human \"([^\"]+)\"").Select(m => m.Groups[1].Value).ToList();
        T.True(humans.All(h => o.Stack.Find(SpawnCatalog.BreedFile(h)) != null), "every template breed resolves");
        var soldiers = humans.Where(h => !h.EndsWith("pilot")).ToList();
        T.Eq(soldiers.Count, soldiers.Distinct().Count(), "one template per breed");
        foreach (var kv in r.TemplateFiles)
        {
            T.Eq(0, SdlCore.Parse(kv.Value).Errors.Count, "template file parses: " + kv.Key);
            T.True(Regex.Matches(kv.Value, "\\{Human \"mp/([a-z]+)/").Cast<Match>().All(m => m.Groups[1].Value == kv.Key), "a nation's file holds only its soldiers: " + kv.Key);
            T.True(kv.Value.Contains("\"ctb_has_" + kv.Key + "$\"") && kv.Value.Contains("{value 1}"), "the file marks its nation as present: " + kv.Key);
            T.True(menu.Contains("{\"ctb_has_" + kv.Key + "\"}"), "the menu declares the presence variable: " + kv.Key);
        }
        T.True(r.TemplateFiles["ger"].Contains("{Human \"mp/ger/mid/rifle\"") && Regex.IsMatch(menu, "\\{\"loop\" 3\\r\\n"), "squad members cloned by count");
        // 국가 화면을 열면 견본이 없는 국가 단추를 숨긴다
        var nationScreen = r.Screens.Single(s => s.Title == "Nation");
        string natTrigger = menu.Substring(menu.IndexOf("{id \"ctb_e_s" + nationScreen.Id + "\"}"));
        natTrigger = natTrigger.Substring(0, natTrigger.IndexOf("{id ", 5) < 0 ? natTrigger.Length : natTrigger.IndexOf("{id ", 5));
        foreach (var b in nationScreen.Buttons.Where(b => b.Event.StartsWith("ctb_e_nat_")))
            T.True(natTrigger.Contains("\"ctb_has_" + b.Event.Substring("ctb_e_nat_".Length) + "$\"") && natTrigger.Contains("\"" + b.Name + "\""), "nation button hidden without templates: " + b.Name);
        T.True(menu.Contains("\"ctb_start\"") && menu.Contains("{type side}") && menu.Contains("{state \"-\"}"), "start trigger and visible root");

        // 옵션(Task 6)
        T.True(menu.Contains("{impregnability full}") && menu.Contains("{veterancy 8}") && menu.Contains("artillery_single_150_dummy") && menu.Contains("score_add"), "options are in the menu");
        // 포격: 크기마다 5~20발, 크기마다 반경이 다른 웨이포인트에 떨어진다(사용자 요청 2026-10-18)
        foreach (var b in MenuBuilder.Barrages)
        {
            T.True(b.Shots >= 5 && b.Shots <= 20, "barrage shot count in 5..20: " + b.Key);
            string trig = menu.Substring(menu.IndexOf("{\"1.event\" {id \"ctb_e_" + b.Key + "\"}}"));
            trig = trig.Substring(0, trig.IndexOf("{\"trigger\""));
            T.True(trig.Contains("{\"loop\" " + b.Shots + "\r\n") && trig.Contains("{waypoint \"ctb_wp_" + b.Key + "\"}"), "barrage loops its shots on its own waypoint: " + b.Key);
            T.True(menu.Contains("{\"ctb_wp_" + b.Key + "\" {position -9546.01 7248.6 0} {radius " + b.Radius + "}}"), "barrage waypoint has its spread radius: " + b.Key);
            T.Eq("ctb_wp_" + b.Key, r.Screens.SelectMany(s => s.Buttons).Single(x => x.Event == "ctb_e_" + b.Key).Waypoint, "the barrage button picks its own waypoint: " + b.Key);
        }
        T.True(MenuBuilder.Barrages.Select(b => b.Shots).SequenceEqual(MenuBuilder.Barrages.Select(b => b.Shots).OrderBy(x => x)) && MenuBuilder.Barrages.First().Shots == 5 && MenuBuilder.Barrages.Last().Shots == 20, "small 5 to large 20");
        // 맨 위 Spawn 단추는 여닫기
        string root = menu.Substring(menu.IndexOf("{\"1.event\" {id \"ctb_e_root\"}}"));
        root = root.Substring(0, root.IndexOf("{\"trigger\""));
        T.True(root.IndexOf("{value 1}") < root.IndexOf("{name \"ctb_s*\"}") && root.Contains("\"ctb_open$\"") && root.Contains("{function \"ctb_home\"}"), "root closes an open menu and opens a closed one: " + root);
        T.True(menu.Contains("{\"ctb_open\"}"), "open flag declared");

        // 처음엔 Spawn만, 소속 기본은 나(설계 "첫 화면"·"국가와 소속", 사용자 지적 2026-10-18)
        string start = menu.Substring(menu.IndexOf("{\"ctb_start\""));
        start = start.Substring(0, start.IndexOf("{\"ctb_find_me\""));
        var ownerScreen = r.Screens.Single(s => s.Title == "Owner");
        T.True(!start.Contains("{name \"ctb_s" + ownerScreen.Id + "_*\"}") && start.Contains("{name \"ctb_s*\"}") && start.Contains("{name \"ctb_root\"}"), "start shows only the Spawn button: " + start);
        T.True(Regex.IsMatch(start, "\"ctb_owner\\$\"\\} \\{op \"=\"\\} \\{value " + MenuBuilder.MeOwner + "\\}"), "start owner is me");
        T.True(menu.Contains("{\"ctb_find_me\"") && menu.Contains("{state user_control}") && menu.Contains("{type entities}"), "my number is looked up in the match");
        // 소속 화면: 나·아군 AI·적 AI
        var ownerTexts = ownerScreen.Buttons.Select(b => r.Strings.TryGetValue(b.Text, out var t) ? t : b.Text).ToList();
        T.True(ownerTexts.Take(3).SequenceEqual(new[] { "Me", "Ally AI", "Enemy AI" }), "owner screen starts with me, ally AI, enemy AI: " + string.Join(",", ownerTexts));
        T.True(ownerTexts.Contains("Player 2 (you)"), "my number is marked");
        T.True(menu.Contains("{relation ally}") && menu.Contains("{relation enemy}"), "ally and enemy AI found by relation to me");
        // 첫 화면에 지금 국가·소속
        var gerHome = r.Screens.First(s => s.Army == "ger" && s.Title == "Home");
        var homeTexts = gerHome.Buttons.Select(b => r.Strings.TryGetValue(b.Text, out var t) ? t : b.Text).ToList();
        T.True(homeTexts.Contains("Nation: Germany") && MenuBuilder.OwnerChoices.All(c => homeTexts.Contains("Owner: " + c.Label)), "home shows the nation and an owner label per choice: " + string.Join(",", homeTexts.Take(6)));
        string homeFn = menu.Substring(menu.IndexOf("{\"ctb_home\""));
        homeFn = homeFn.Substring(0, homeFn.IndexOf("{\"ctb_owner\""));
        var meLabel = gerHome.Buttons[homeTexts.IndexOf("Owner: Me")].Name;
        T.True(homeFn.Contains("{name \"" + meLabel + "\"}") && homeFn.Contains("\"ctb_owner$\"} {op \"==\"} {value " + MenuBuilder.MeOwner + "}"), "home shows only the current owner label");
        // 등급 최대(찍은 자리): 가까운 내 병사
        T.True(menu.Contains("{amount " + MenuBuilder.VetAround + "}"), "veterancy around the point takes my nearest soldiers");
        // 항공 폭격
        if (cat.Units.Any(u => u.Category == "air"))
            foreach (var a in MenuBuilder.AirStrikes)
            {
                string at = menu.Substring(menu.IndexOf("{\"1.event\" {id \"ctb_e_" + a.Key + "\"}}"));
                at = at.Substring(0, at.IndexOf("{\"trigger\""));
                T.True(at.Contains("{\"loop\" " + a.Planes + "\r\n") && at.Contains("{\"air_attack\"") && at.Contains("{waypoint_to_attack \"" + MenuBuilder.AirTarget + "\"}"), "air strike clones planes and attacks the point: " + a.Key);
            }
        else T.True(false, "fixture has an aircraft for the air strike test");
        T.True(menu.Contains("{amount 1}") && menu.Contains("_bleedout_"), "pick-one and wreck cleanup");

        // 캠페인 국가 조각과 .pot
        string nation = MenuBuilder.NationFile("ger", MenuBuilder.Armies(cat));
        T.Eq(0, SdlCore.Parse(nation).Errors.Count, "nation file parses");
        T.True(nation.Contains("\"ctb_campaign$\"") && nation.Contains("{value 1}"), "nation file marks a campaign");
        string pot = MenuBuilder.Pot(r);
        T.True(pot.Contains("msgctxt \"mission/ctatoybox_spawn/tanks\"\nmsgid \"Tanks\""), "menu words in the pot");
        T.True(Regex.Matches(menu, "\\{text \"mission/ctatoybox_spawn/([^\"]+)\"\\}").Select(m => "mission/ctatoybox_spawn/" + m.Groups[1].Value).All(r.Strings.ContainsKey), "every menu text key is in the pot");
    }
}
