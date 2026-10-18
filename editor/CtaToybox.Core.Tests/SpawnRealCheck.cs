using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;
using CtaToybox.Core.Spawn;

// 스폰 모드 실제 파일 검사: 지금 계층(Spawn Menu 제외)으로 목록을 만들어 수를 본다. `-- real`과 `-- real-spawn`이 부른다
static class SpawnRealCheck
{
    public static void Run(string settingsPath)
    {
        var s = EditorSettings.Load(settingsPath);
        var plan = GameProfile.PlanLayers(s, GameProfile.ReadModEntries(File.ReadAllText(s.OptionsSetPath)));
        var warnings = new List<string>();
        using var stack = SpawnSources.Open(s, plan, warnings);
        T.True(!stack.Layers.Any(l => SpawnSources.ExcludedLayers.Contains(l.Name)), "spawn layers drop the Spawn Menu mods");
        var index = UnitIndex.Build(stack, null);
        var armies = ArmyIndex.Build(stack, index.Units);
        var skipped = new List<string>();
        var cat = SpawnCatalog.Build(stack, index, armies, SpawnCategories.Load(Path.Combine(T.RepoRoot(), "editor", "spawn", "categories.psv"), warnings), skipped);
        Console.WriteLine("spawn-layers=" + string.Join(",", stack.Layers.Select(l => l.Name)) + " spawn-units=" + cat.Units.Count + " spawn-squads=" + cat.Squads.Count + " spawn-skipped=" + skipped.Count + " spawn-rosters=" + cat.RosterFiles.Count);
        foreach (var g in cat.Units.GroupBy(u => u.Army + "/" + u.Category).OrderBy(g => g.Key)) Console.Write(g.Key + "=" + g.Count() + " ");
        Console.WriteLine();
        foreach (var g in skipped.GroupBy(x => x.Substring(x.LastIndexOf(':') + 1).Trim().Split(':')[0]).OrderByDescending(g => g.Count()).Take(8)) Console.WriteLine("  spawn skipped " + g.Count() + "x: " + g.First());
        T.True(cat.Units.Any(u => u.Def.StartsWith("entity/-vehicle/") && index.Units.Any(x => x.Path == u.Def && x.Layer == "mod_3732639687")), "Wunderwaffe vehicles are in the spawn list");
        T.True(cat.Squads.Count > 500, "the skirmish rosters give squads");

        // 스커미시 로스터
        var rosterSkipped = new List<string>(); var notes = new List<string>();
        // 기본 생성은 로스터를 바꾸지 않지만 --roster-add로 켰을 때의 글도 문법을 본다
        var roster = RosterRewrite.Build(stack, cat, new RosterOptions { Enabled = true, AddVehicles = true }, rosterSkipped, notes);
        int bad = 0, added = 0;
        foreach (var f in roster.Where(f => f.Path.EndsWith(".set") || f.Path.EndsWith(".inc")))
        {
            TextKind kind;
            string text = TextCodec.Decode(f.Bytes, out kind);
            var before = stack.ReadText(f.Path.Substring("resource/".Length));
            if (SdlCore.Parse(text).Errors.Count > SdlCore.Parse(before ?? "").Errors.Count) { bad++; Console.WriteLine("  spawn roster parse: " + f.Path); }
            added += System.Text.RegularExpressions.Regex.Matches(text, "g\\(ctb_spawn\\)").Count;
        }
        Console.WriteLine("spawn-roster-files=" + roster.Count + " spawn-roster-new-entries=" + added + " spawn-roster-skipped=" + rosterSkipped.Count + " icons=" + roster.Count(f => f.Path.EndsWith(".tga")));
        foreach (var g in rosterSkipped.GroupBy(x => x.Substring(x.IndexOf(':') + 1).Trim().Split(':')[0]).OrderByDescending(g => g.Count()).Take(5)) Console.WriteLine("  spawn roster skipped " + g.Count() + "x: " + g.First());
        T.Eq(0, bad, "rewritten rosters parse as well as the originals");
        T.True(added > 0, "missing vehicles are added to the rosters");

        // 맵 사본: 넣은 줄을 빼면 이긴 판 바이트와 같다
        var mapSkipped = new List<string>();
        int campaign, multi;
        var menuArmies = MenuBuilder.Armies(cat);
        int campaignArmies;
        var maps = MapPatch.Build(stack, mapSkipped, menuArmies, false, true, out campaign, out multi, out campaignArmies);
        int differ = 0;
        foreach (var f in maps)
        {
            string v = f.Path.Substring("resource/".Length);
            string orig = System.Text.Encoding.Latin1.GetString(stack.Read(stack.Find(v)));
            string back = System.Text.Encoding.Latin1.GetString(f.Bytes);
            foreach (string line in new[] { MapPatch.MenuInclude, MapPatch.ConquestInclude, MapPatch.NationInclude(MapPatch.NationOf(v) ?? "x") }.Concat(menuArmies.Select(MapPatch.TemplateInclude)).Concat(menuArmies.Select(a => MapPatch.TemplateInclude(a, true))))
                foreach (string nl in new[] { "\r\n", "\n" }) back = back.Replace("\t" + line + nl, "");
            if (back != orig && back.Replace("\n", "") != orig.Replace("\n", "")) { differ++; if (differ <= 3) Console.WriteLine("  spawn map differs: " + v); }
        }
        Console.WriteLine("spawn-maps campaign=" + campaign + " multi=" + multi + " campaign-template-nations=" + campaignArmies + " skipped=" + mapSkipped.Count + " layers=" + string.Join(",", maps.GroupBy(m => m.Source).Select(g => g.Key + ":" + g.Count())));
        foreach (string ms in mapSkipped.Take(8)) Console.WriteLine("  spawn map skipped: " + ms);
        // Spawn Menu of MACE는 멀티 389개, 원래 Spawn Menu는 388개(spike). 하나는 그 모드에만 있는 맵이라 계층에 없다
        T.True(campaign >= 91 && multi >= 388, "at least the maps Spawn Menu covered");
        T.Eq(0, differ, "map copies are the winning file plus the include lines only");

        // 메뉴
        var menuSkipped = new List<string>();
        var menu = MenuBuilder.Build(cat, new MenuOptions(), n => stack.Find("interface/scene/unit_icon/" + n + "_00.tga") != null, b => stack.Find(SpawnCatalog.BreedFile(b)) != null, menuSkipped,
            stack.Paths.Where(x => x.StartsWith("interface/scene/unit_icon/") && x.EndsWith("_00.tga")).Select(x => x.Substring(26, x.Length - 33)));
        var menuParse = SdlCore.Parse(menu.Text);
        var pairs = SpawnMenuTests.Pairs(menu.Text);
        T.True(campaignArmies < campaign * menuArmies.Count, "campaign maps get fewer template nations than all");
        foreach (var kv in menu.TemplateFiles) T.Eq(0, SdlCore.Parse(kv.Value).Errors.Count, "the real template file parses: " + kv.Key);
        var images = menu.Screens.SelectMany(sc => sc.Buttons).Select(b => b.Image).ToList();
        Console.WriteLine("spawn-icons distinct=" + images.Distinct().Count() + " back=" + images.Count(i => i == "back") + " button_N=" + images.Count(i => i.StartsWith("button_")) + " squad-buttons-with-squad-icon=" + menu.Screens.SelectMany(sc => sc.Buttons).Count(b => b.Text.StartsWith("desc/squad/") && !b.Image.StartsWith("button_")) + "/" + menu.Screens.SelectMany(sc => sc.Buttons).Count(b => b.Text.StartsWith("desc/squad/")));
        Console.WriteLine("spawn-templates " + string.Join(" ", menu.TemplateFiles.Select(kv => kv.Key + "=" + System.Text.RegularExpressions.Regex.Matches(kv.Value, "\\{Human ").Count)));
        int maxId = System.Text.RegularExpressions.Regex.Matches(string.Concat(menu.TemplateFiles.Values), " 0x([0-9a-f]+)\\r\\n").Select(m => Convert.ToInt32(m.Groups[1].Value, 16)).DefaultIfEmpty(0).Max();
        Console.WriteLine("spawn-menu bytes=" + menu.Text.Length + " screens=" + menu.Screens.Count + " buttons=" + menu.Screens.Sum(s => s.Buttons.Count) + " templates=" + menu.Templates + " triggers=" + menu.Triggers + " skipped=" + menuSkipped.Count + " max-id=0x" + maxId.ToString("x") + " max-buttons=" + menu.Screens.Max(s => s.Buttons.Count));
        foreach (var g in menuSkipped.GroupBy(x => x.Substring(x.IndexOf(':') + 1).Trim()).OrderByDescending(g => g.Count()).Take(5)) Console.WriteLine("  spawn menu skipped " + g.Count() + "x: " + g.First());
        foreach (string p in pairs.Take(5)) Console.WriteLine("  spawn menu pair: " + p);
        T.Eq(0, menuParse.Errors.Count, "the real menu parses");
        T.Eq(0, pairs.Count, "the real menu's buttons, events and triggers pair up");
        T.True(maxId <= 0xFFFF, "template ids stay in the 16-bit range the missions use");
    }
}
