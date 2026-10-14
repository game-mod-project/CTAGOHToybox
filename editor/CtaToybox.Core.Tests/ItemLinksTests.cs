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
        T.Eq(ItemLinks.RuleCandidates, family.Rule, "a family name has candidates");
        T.Eq(new[] { "set/stuff/mgun/mgun_ger.belt.ammo", "set/stuff/mgun/mgun_ger.drum.ammo" }, family.Files.ToArray(), "every file whose words contain the name's words, sorted");
        T.Eq(ItemLinks.RuleCandidates, links.Find("ammo mgun_usa").Rule, "one subset candidate is still only a candidate");
        T.Eq(ItemLinks.RuleCandidates, links.Find("dup").Rule, "the same file name in two folders is a candidate set");
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
