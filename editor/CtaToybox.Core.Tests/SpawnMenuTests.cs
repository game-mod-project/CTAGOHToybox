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
        var humans = Regex.Matches(menu, "\\{Human \"([^\"]+)\"").Select(m => m.Groups[1].Value).ToList();
        T.True(humans.All(h => o.Stack.Find(SpawnCatalog.BreedFile(h)) != null), "every template breed resolves");
        var soldiers = humans.Where(h => !h.EndsWith("pilot")).ToList();
        T.Eq(soldiers.Count, soldiers.Distinct().Count(), "one template per breed");
        T.True(menu.Contains("{Human \"mp/ger/mid/rifle\"") && Regex.IsMatch(menu, "\\{\"loop\" 3\\r\\n"), "squad members cloned by count");
        T.True(menu.Contains("\"ctb_start\"") && menu.Contains("{type side}") && menu.Contains("{state \"-\"}"), "start trigger and visible root");

        // 옵션(Task 6)
        T.True(menu.Contains("{impregnability full}") && menu.Contains("{veterancy 8}") && menu.Contains("artillery_single_150_dummy") && menu.Contains("{\"loop\" 6") && menu.Contains("score_add"), "options are in the menu");
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
