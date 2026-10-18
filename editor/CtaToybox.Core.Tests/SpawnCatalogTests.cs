using System.Linq;
using CtaToybox.Core.Spawn;

// 스폰 모드: 소환 유닛 목록과 종류(Task 2)
static class SpawnCatalogTests
{
    public static void Run()
    {
        var c = SpawnCategories.Parse(SpawnFixture.Categories, null);
        T.Eq("td", c.Of(SpawnFixture.Stug, "tanks", "tank"), "an earlier name line beats the medium folder line");
        T.Eq("heavy", c.Of(SpawnFixture.Ht, "tanks", "tank"), "folder rule");
        T.Eq("car", c.Of("entity/-vehicle/germany/tank_light/kfz/kfz.def", "vehicles", "car"), "a non-tank button wins");
        T.Eq("medium", c.Of("entity/-vehicle/germany/odd/o1/o1.def", null, "tank"), "a tank no rule matches is medium");
        T.Eq("empl", c.Of("entity/-vehicle/germany/special/g1/g1.def", null, "cannon"), "a cannon no rule matches is a gun");
        T.Eq("tiger", SpawnCatalog.FamilyOf("entity/-vehicle/germany/tank_heavy/tiger1e/tiger1e.def"), "family from the folder");
        var warnings = new System.Collections.Generic.List<string>();
        SpawnCategories.Parse("light|folder|/x/\nbad line\n", warnings);
        T.Eq(1, warnings.Count, "a bad rule line is reported");

        string repo, stat, spawn;
        string settings = SpawnFixture.Make(out repo, out stat, out spawn);
        using var o = SpawnFixture.Open(settings);
        var cat = o.Catalog;
        T.Eq("light", cat.Units.Single(u => u.Name == "lt").Category, "light tank");
        T.Eq("td", cat.Units.Single(u => u.Name == "stug3g").Category, "name rule");
        T.Eq("rus", cat.Units.Single(u => u.Name == "rt").Army, "army by folder");
        T.Eq("empl", cat.Units.Single(u => u.Name == "pak40").Category, "gun");
        T.True(cat.Units.Single(u => u.Name == "lt").Roster != null && cat.Units.Single(u => u.Name == "ht").Roster == null, "roster entry when listed");
        T.True(cat.Units.Any(u => u.Name == "bf109"), "an other vehicle named in the roster is in");
        T.True(!cat.Units.Any(u => u.Name == "wreck"), "an other vehicle not in the roster is out");
        T.Eq("air", cat.Units.Single(u => u.Name == "ju87").Category, "an aircraft (airborne.ext) is in as air");
        var sq = cat.Squads.Single(s => s.Name == "sq_a");
        T.Eq(new[] { ("mp/ger/mid/nco", 1), ("mp/ger/mid/rifle", 3) }, sq.Members.ToArray(), "squad members from the roster");
        T.Eq("sq_a(ger)", sq.MenuName, "squad name as the template makes it");
        T.True(!cat.Squads.Any(s => s.Name == "sq_b") && o.Skipped.Any(s => s.Contains("sq_b") && s.Contains("ghost")), "a squad with a missing breed is skipped with the reason");
    }
}
