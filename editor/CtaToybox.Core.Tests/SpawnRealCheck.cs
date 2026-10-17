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
    }
}
