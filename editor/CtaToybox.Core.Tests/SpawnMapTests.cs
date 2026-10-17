using System.Collections.Generic;
using System.Linq;
using System.Text;
using CtaToybox.Core.Spawn;

// 스폰 모드: 맵 사본(Task 4)
static class SpawnMapTests
{
    public static void Run()
    {
        byte[] src = Encoding.UTF8.GetBytes(SpawnFixture.MissionText);
        string err;
        T.Eq("{mission\r\n\t{a 1}\r\n\t(include \"/map/ctatoybox_spawn/nation_ger.mi\")\r\n\t(include \"/map/ctatoybox_spawn/menu.mi\")\r\n}\r\n", Encoding.UTF8.GetString(MapPatch.Patch(src, "ger", out err)), "two lines before the close");
        byte[] once = MapPatch.Patch(src, null, out err);
        T.Eq(Encoding.UTF8.GetString(once), Encoding.UTF8.GetString(MapPatch.Patch(once, null, out err)), "an include already there is not added twice");
        T.Eq("{mission\n\t{b 2}\n\t(include \"/map/ctatoybox_spawn/menu.mi\")\n}\n", Encoding.UTF8.GetString(MapPatch.Patch(Encoding.UTF8.GetBytes("{mission\n\t{b 2}\n}\n"), null, out err)), "LF kept");
        T.Eq("{mission {b 2}\n\t(include \"/map/ctatoybox_spawn/menu.mi\")\n}", Encoding.UTF8.GetString(MapPatch.Patch(Encoding.UTF8.GetBytes("{mission {b 2}}"), null, out err)), "a close on a code line gets its own lines");
        byte[] latin = { (byte)'{', (byte)'m', (byte)'i', (byte)'s', (byte)'s', (byte)'i', (byte)'o', (byte)'n', (byte)' ', (byte)'"', 0xC4, 0xFC, (byte)'"', (byte)'\n', (byte)'}', (byte)'\n' };
        var patched = MapPatch.Patch(latin, null, out err);
        T.True(patched[10] == 0xC4 && patched[11] == 0xFC, "bytes other than the added line stay the same");
        T.Eq(null, MapPatch.Patch(Encoding.UTF8.GetBytes("{scene}"), null, out err), "map_unparsable_skipped: not a mission");
        T.True(err != null, "with a reason");
        T.Eq(null, MapPatch.Patch(Encoding.UTF8.GetBytes("{mission {a 1}"), null, out err), "map_unparsable_skipped: unclosed");
        T.Eq("ger", MapPatch.NationOf("map/single/051-ger_finesthour/x/0.mi"), "nation from folder");
        T.Eq("usa", MapPatch.NationOf("map/single/041-usa_airborne/x/0.mi"), "nation before an underscore");
        T.Eq("ger", MapPatch.NationOf("map/single/02-ger-west/x/0.mi"), "nation before a dash");
        T.Eq(null, MapPatch.NationOf("map/single/00-bootcamp/x/0.mi"), "no nation for bootcamp");
        T.True(MapPatch.IsMap("map/multi/dcg_x/campaign_capture_the_flag.mi") && !MapPatch.IsMap("map/multi/dcg_x/dcg.inc") && !MapPatch.IsMap("map/single/02-ger/m1/1.mi"), "which files are maps");

        string repo, stat, spawn;
        string settings = SpawnFixture.Make(out repo, out stat, out spawn);
        using var o = SpawnFixture.Open(settings);
        var skipped = new List<string>();
        int campaign, multi;
        var files = MapPatch.Build(o.Stack, skipped, out campaign, out multi);
        T.Eq(1, campaign, "one campaign map");
        T.Eq(1, multi, "one multiplayer map");
        string c = Encoding.UTF8.GetString(files.Single(f => f.Path == "resource/" + SpawnFixture.Campaign).Bytes);
        T.True(c.StartsWith("{mission\r\n\t{a 1}\r\n") && !c.Contains("smm") && !c.Contains("cheat.mi"), "the vanilla map wins, not the Spawn Menu copy: " + c);
        T.True(!files.Any(f => f.Path.EndsWith("dcg.inc")), "other map files are not copied");

        // 견본 국가: 멀티는 모두, 캠페인은 맵에 나오는 국가와 캠페인 국가만
        T.Eq("{mission\r\n\t{a 1}\r\n\t(include \"/map/ctatoybox_spawn/nation_ger.mi\")\r\n\t(include \"/map/ctatoybox_spawn/templates_ger.mi\")\r\n\t(include \"/map/ctatoybox_spawn/templates_usa.mi\")\r\n\t(include \"/map/ctatoybox_spawn/menu.mi\")\r\n}\r\n",
            Encoding.UTF8.GetString(MapPatch.Patch(src, "ger", new[] { "ger", "usa" }, out err)), "template lines between the nation file and the menu");
        var all = new[] { "ger", "rus", "usa", "jap" };
        T.Eq(new[] { "ger", "usa" }, MapPatch.ArmiesIn("{Human \"mp/usa/late/rifleman\" 0x1}{Human \"mp/pol/late/x\" 0x2}", "ger", all).ToArray(), "map nations: soldiers and the campaign nation, menu order, unknown dropped");
        T.Eq(new string[0], MapPatch.ArmiesIn("{mission}", null, all).ToArray(), "no nation found");
        files = MapPatch.Build(o.Stack, skipped, all, false, out campaign, out multi, out int campaignArmies);
        c = Encoding.UTF8.GetString(files.Single(f => f.Path == "resource/" + SpawnFixture.Campaign).Bytes);
        string m = Encoding.UTF8.GetString(files.Single(f => f.Path == "resource/" + SpawnFixture.Conquest).Bytes);
        T.True(c.Contains("templates_ger.mi") && !c.Contains("templates_rus.mi") && !c.Contains("templates_jap.mi"), "campaign map: only its nations: " + c);
        T.True(all.All(a => m.Contains("templates_" + a + ".mi")), "multiplayer map: every nation");
        T.Eq(1, campaignArmies, "campaign nations counted");
        files = MapPatch.Build(o.Stack, skipped, all, true, out campaign, out multi, out campaignArmies);
        c = Encoding.UTF8.GetString(files.Single(f => f.Path == "resource/" + SpawnFixture.Campaign).Bytes);
        T.True(all.All(a => c.Contains("templates_" + a + ".mi")), "--all-nations: every nation in a campaign map too");
    }
}
