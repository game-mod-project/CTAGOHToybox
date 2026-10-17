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
        var roster = RosterRewrite.Build(stack, cat, new RosterOptions(), rosterSkipped, notes);
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
        var maps = MapPatch.Build(stack, mapSkipped, out campaign, out multi);
        int differ = 0;
        foreach (var f in maps)
        {
            string v = f.Path.Substring("resource/".Length);
            string orig = System.Text.Encoding.Latin1.GetString(stack.Read(stack.Find(v)));
            string back = System.Text.Encoding.Latin1.GetString(f.Bytes);
            foreach (string line in new[] { MapPatch.MenuInclude, MapPatch.NationInclude(MapPatch.NationOf(v) ?? "x") })
                foreach (string nl in new[] { "\r\n", "\n" }) back = back.Replace("\t" + line + nl, "");
            if (back != orig && back.Replace("\n", "") != orig.Replace("\n", "")) { differ++; if (differ <= 3) Console.WriteLine("  spawn map differs: " + v); }
        }
        Console.WriteLine("spawn-maps campaign=" + campaign + " multi=" + multi + " skipped=" + mapSkipped.Count + " layers=" + string.Join(",", maps.GroupBy(m => m.Source).Select(g => g.Key + ":" + g.Count())));
        foreach (string ms in mapSkipped.Take(8)) Console.WriteLine("  spawn map skipped: " + ms);
        // Spawn Menu of MACE는 멀티 389개, 원래 Spawn Menu는 388개(spike). 하나는 그 모드에만 있는 맵이라 계층에 없다
        T.True(campaign >= 91 && multi >= 388, "at least the maps Spawn Menu covered");
        T.Eq(0, differ, "map copies are the winning file plus the include lines only");
    }
}
