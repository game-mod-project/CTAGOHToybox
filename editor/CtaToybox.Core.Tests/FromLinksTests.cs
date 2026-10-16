using System.Linq;
using CtaToybox.Core;

static class FromLinksTests
{
    public static void Run()
    {
        var links = new FromLinks(new[]
        {
            "set/stuff/rifle/rifle.pattern", "set/stuff/rifle/k98", "set/stuff/bullet.pattern", "set/bullet.pattern",
            "set/stuff/bombing/air_bomb.ammo", "set/stuff/bombing/air_bomb.pattern", "set/stuff/bombing/boom.ammo",
            "set/stuff/melee/knife.pattern", "set/stuff/melee/knife_stab.pattern", "set/stuff/melee/shovel",
            "set/stuff/bazooka/bazooka_m1.weapon", "set/stuff/bazooka/bazooka_m1a1.weapon",
            "set/stuff/grenade/at/g.grenade", "set/stuff/throwable.pattern", "set/stuff/grenade/throwable.pattern",
            "set/stuff/two/x.a", "set/stuff/two/x.b", "set/stuff/two/y", "set/stuff/thrower.hand", "set/stuff/thrower_v", "set/only.pattern",
            "set/stuff/head/light_headgear.pattern", "set/stuff/body/cap", "set/stuff/mgun/mgun.pattern", "set/stuff/hmg/mgun.pattern", "set/stuff/shell/bulletrus_100.ussr.apds.ammo",
        });
        T.Eq("set/stuff/rifle/rifle.pattern", links.Find("set/stuff/rifle/k98", "pattern rifle"), "words reversed and joined with a dot, in the same folder");
        T.Eq("set/stuff/thrower.hand", links.Find("set/stuff/thrower_v", "hand thrower"), "any two words");
        T.Eq("set/stuff/bullet.pattern", links.Find("set/stuff/rifle/rifle.pattern", "pattern bullet"), "an ancestor folder");
        T.Eq("set/stuff/grenade/throwable.pattern", links.Find("set/stuff/grenade/at/g.grenade", "pattern throwable"), "the nearest folder wins");
        T.Eq("set/stuff/melee/knife.pattern", links.Find("set/stuff/melee/k", "knife pattern"), "two words in written order when the reversed name is missing");
        T.Eq("set/stuff/rifle/k98", links.Find("set/stuff/rifle/k98_30", "k98"), "one word: the file without an extension");
        T.Eq("set/stuff/bombing/air_bomb.ammo", links.Find("set/stuff/bombing/boom.ammo", "air_bomb"), "one word: the child's own extension before other files");
        T.Eq("set/stuff/bazooka/bazooka_m1.weapon", links.Find("set/stuff/bazooka/bazooka_m1a1.weapon", "bazooka_m1"), "one word: the child's own extension");
        T.Eq("set/stuff/melee/knife_stab.pattern", links.Find("set/stuff/melee/shovel", "knife_stab"), "one word: the only file with that name and one extension");
        T.Eq("set/stuff/two/x.a", links.Find("set/stuff/two/y", "x"), "one word: two files with that name and other extensions: the first by name");
        T.Eq(new[] { "set/stuff/two/x.a", "set/stuff/two/x.b" }, links.Candidates("set/stuff/two/y", "x").ToArray(), "and both are candidates");
        T.Eq("set/stuff/head/light_headgear.pattern", links.Find("set/stuff/body/cap", "pattern light_headgear"), "a file in another folder (mods rely on this)");
        T.Eq("set/stuff/shell/bulletrus_100.ussr.apds.ammo", links.Find("set/stuff/shell/bulletrus_100.apds.ammo", "bulletrus_100 ussr apds"), "three words without the extension");
        T.Eq("set/stuff/hmg/mgun.pattern", links.Find("set/stuff/hmg/x", "pattern mgun"), "a name in two folders: the nearest one");
        T.Eq("set/stuff/mgun/mgun.pattern", links.Find("set/stuff/mgun/besa", "pattern mgun"), "from the other folder, the other one");
        T.Eq(2, links.Candidates("set/stuff/mgun/besa", "pattern mgun").Count, "and both are candidates");
        T.Eq(1, links.Candidates("set/stuff/rifle/k98", "pattern rifle").Count, "a unique name has one candidate");
        T.Eq("set/stuff/rifle/rifle.pattern", links.Find("SET/stuff/rifle/k98", "  Pattern   RIFLE "), "case and spacing are ignored");
        T.True(links.Find("set/stuff/rifle/k98", "pattern none") == null, "a missing name is null");
        T.True(links.Find("set/stuff/rifle/k98", "pattern only") == null, "the search stops at set/stuff");
        T.True(links.Find("set/other/k", "pattern bullet") == null, "files outside set/stuff do not inherit");
        T.True(links.Find("set/stuff/rifle/k98", "") == null, "an empty name is null");

        var root = SdlCore.Parse("; head\n{from \"pattern rifle\" {mass 4}}").Root;
        T.Eq("pattern rifle", FromLinks.NameOf(KeyLocator.Rows(root)), "the from name of a file whose top block is from");
        T.True(FromLinks.NameOf(KeyLocator.Rows(SdlCore.Parse("{bullet {from \"x\"}}").Root)) == null, "a nested from is not inheritance");
        T.True(FromLinks.NameOf(KeyLocator.Rows(SdlCore.Parse("{from x}").Root)) == null, "an unquoted name is not inheritance");

        T.Eq("/from/inventory/size", FromLinks.Reroot("/bullet/inventory/size", "/from"), "the parent's top block takes the child's name");
        T.Eq("/from/(\"fire rate\")", FromLinks.Reroot("/weapon_bullet/(\"fire rate\")", "/from"), "macro rows too");
        T.True(FromLinks.Reroot("/bullet", "/from") == null, "the parent's top row itself has no place in the child");
    }
}
