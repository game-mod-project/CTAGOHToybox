using System.Linq;
using CtaToybox.Core;

static class ItemLinksTests
{
    public static void Run()
    {
        T.True(ItemLinks.IsItemFile("set/stuff/smg/m3_grease"), "a stuff file without extension");
        T.True(ItemLinks.IsItemFile("set/stuff/shell/bulletrus_85.aphebc.ammo"), "an ammo file");
        T.True(!ItemLinks.IsItemFile("set/stuff/smg/.presets"), ".presets is not an item");
        T.True(!ItemLinks.IsItemFile("set/stuff/smg/smg.pattern"), "a pattern is not an item");
        T.True(!ItemLinks.IsItemFile("set/stuff/smg/x.inc"), "an .inc is not an item");
        T.True(!ItemLinks.IsItemFile("set/stuff/_settings/a"), "_settings is not items");
        T.True(!ItemLinks.IsItemFile("entity/-vehicle/a/a.def"), "outside set/stuff is not an item file");

        var links = ItemLinks.Build(new[]
        {
            "set/stuff/smg/m3_grease", "set/stuff/smg/m3_grease_mag.ammo", "set/stuff/shell/bulletrus_85.aphebc.ammo",
            "set/stuff/mgun/mgun_usa.belt.ammo", "set/stuff/mgun/mgun_ger.belt.ammo", "set/stuff/mgun/mgun_ger.drum.ammo",
            "set/stuff/a/dup", "set/stuff/b/dup", "set/stuff/smg/.presets", "entity/-vehicle/m3_grease/m3_grease.def",
        });

        var l = links.Find("m3_grease");
        T.Eq(ItemLinks.RuleName, l.Rule, "a name equal to a file name");
        T.Eq(new[] { "set/stuff/smg/m3_grease" }, l.Files.ToArray(), "links to that file, not to the entity def");
        T.Eq(ItemLinks.RuleWords, links.Find("m3_grease_mag ammo").Rule, "same words in another order");
        T.Eq(ItemLinks.RuleWords, links.Find("ammo m3_grease_mag").Rule, "same words, other order");
        T.Eq(new[] { "set/stuff/smg/m3_grease_mag.ammo" }, links.Find("ammo m3_grease_mag").Files.ToArray(), "to the ammo file");
        T.Eq(ItemLinks.RuleWordsNoExt, links.Find("bulletrus_85 aphebc").Rule, "same words without the extension");
        T.Eq(ItemLinks.RuleWordsNoExt, links.Find("BULLETRUS_85  APHEBC").Rule, "case and extra spaces do not matter");
        var family = links.Find("ammo mgun_ger");
        T.Eq(ItemLinks.RuleFirstByName, family.Rule, "a family name in one folder goes to the first file by name (G5, in game 2026-10-16)");
        T.Eq(new[] { "set/stuff/mgun/mgun_ger.belt.ammo", "set/stuff/mgun/mgun_ger.drum.ammo" }, family.Files.ToArray(), "every file whose words contain the name's words, the game's pick first");
        T.Eq(ItemLinks.RuleFirstByName, links.Find("ammo mgun_usa").Rule, "one subset candidate is the game's pick too");
        T.Eq(ItemLinks.RuleCandidates, links.Find("dup").Rule, "the same file name in two folders is a candidate set");

        // 이름순은 점이 글자보다 앞인 글자 순서(ASCII)다: aa.zz가 aaa와 belt보다 앞이다
        var g5 = ItemLinks.Build(new[] { "set/stuff/mgun/mgun_ger.zz.ammo", "set/stuff/mgun/mgun_ger.belt.ammo", "set/stuff/mgun/mgun_ger.aaa.ammo", "set/stuff/mgun/mgun_ger.aa.zz.ammo" });
        T.Eq("set/stuff/mgun/mgun_ger.aa.zz.ammo", g5.Find("ammo mgun_ger").Files[0], "more words but first by name wins");
        // 후보가 여러 폴더에 걸치면 게임이 무엇을 고르는지 확인하지 않았다
        var spread = ItemLinks.Build(new[] { "set/stuff/a/bazooka.heat.ammo", "set/stuff/b/bazooka.he.ammo" });
        T.Eq(ItemLinks.RuleCandidates, spread.Find("bazooka").Rule, "candidates in two folders stay unconfirmed");
        T.Eq(2, links.Find("dup").Files.Count, "with both files");
        var none = links.Find("kordr");
        T.Eq(ItemLinks.RuleNone, none.Rule, "no file at all");
        T.Eq(0, none.Files.Count, "and no files");

        var nodes = SdlCore.Parse(
            "{game_entity {Weaponry {place \"gun\" {weapon \"85mm_d5t\" filling \"bulletrus_85 aphebc\" 1}}}" +
            " {extender \"inventory\" {box {item \"m3_grease\" filled} {item \"m3_grease_mag ammo\" 90} {item \"m3_grease\" filled}}}" +
            " {weapon \"mgun\" {mask \"mountable\"}} {name \"not an item\"}}").Root.Children;
        T.Eq(new[] { "85mm_d5t", "bulletrus_85 aphebc", "m3_grease", "m3_grease_mag ammo", "mgun" }, ItemLinks.ReferencedNames(nodes).ToArray(),
            "names from item and weapon rows and after filling, lower-cased, distinct, in order");
    }
}
