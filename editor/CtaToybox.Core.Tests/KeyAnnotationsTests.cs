using System.IO;
using System.Linq;
using CtaToybox.Core;

static class KeyAnnotationsTests
{
    public static void Run()
    {
        T.Eq(new[] { "game_entity", "extender[inventory]", "box", "item" }, KeyPattern.PlainKeyPath("/game_entity/extender[inventory]/box/item").ToArray(), "plain path keeps block names");
        T.Eq(new[] { "from", "speed" }, KeyPattern.PlainKeyPath("/from/(\"mobility\")/speed").ToArray(), "macro call segments are dropped");
        T.Eq(new[] { "a", "sensor" }, KeyPattern.PlainKeyPath("/a/off()/#sensor").ToArray(), "call-argument segments and the # marker are dropped");
        T.Eq(new[] { "/a/b/c", "**/a/b/c", "**/b/c", "**/c", "*/c" }, KeyPattern.Candidates("/a/b/c", true).ToArray(), "lookup order with wildcard");
        T.Eq(new[] { "/a/b/c", "**/a/b/c", "**/b/c", "**/c" }, KeyPattern.Candidates("/a/b/c", false).ToArray(), "lookup order without wildcard");

        T.Eq("human", KeyAnnotations.GroupOf("set/breed/mp/usa/late/x.set"), "breed group");
        T.Eq("human", KeyAnnotations.GroupOf("entity/humanskin/a/a.def"), "humanskin group");
        T.Eq("vehicle", KeyAnnotations.GroupOf("entity/-vehicle/a/a.def"), "vehicle group");
        T.Eq("weapon-ammo", KeyAnnotations.GroupOf("set/stuff/smg/m3_grease"), "stuff group");
        T.Eq("inventory", KeyAnnotations.GroupOf("entity/inventory/a/a.def"), "inventory group");
        T.Eq("shared", KeyAnnotations.GroupOf("properties/tank.ext"), "everything else is shared");

        string dir = T.TempDir();
        File.WriteAllText(Path.Combine(dir, "common.psv"),
            "# 주석\n\n" +
            "/game_entity/mass|block|확인|무게. 킬로그램|C\n" +
            "**/box/item|block|추정|적재품 한 칸|S\n" +
            "*/mass|block|추정|어디든 mass|S\n" +
            "*/speed|block|문서|속도|D\n" +
            "**/damage|block|추정|공통 damage|S\n");
        File.WriteAllText(Path.Combine(dir, "weapon-ammo.psv"), "**/damage|block|추정|무기의 damage|S\n");

        var notes = KeyAnnotations.Load(dir, "vehicle");
        T.Eq(5, notes.Count, "five entries");
        var mass = notes.Find("/game_entity/mass", "block", true);
        T.Eq("확인", mass.Grade, "an exact path wins");
        T.Eq("게임 파일 주석", mass.Source, "source abbreviations are expanded");
        T.Eq("추정", notes.Find("/x/mass", "block", true).Grade, "*/name applies with wildcard");
        T.True(notes.Find("/x/mass", "block", false) == null, "*/name does not apply without wildcard");
        T.Eq("적재품 한 칸", notes.Find("/game_entity/extender[inventory]/box/item", "block", false).Meaning, "**/tail applies anywhere");
        T.Eq("파일 구조와 값에서 추론", notes.Find("/game_entity/extender[inventory]/box/item", "block", false).Source, "S is expanded");
        T.True(notes.Find("/game_entity/mass", "macro", true) == null, "the form must match");
        T.Eq("공통 damage", notes.Find("/a/damage", "block", false).Meaning, "the common entry without the group file");
        T.Eq("무기의 damage", KeyAnnotations.Load(dir, "weapon-ammo").Find("/a/damage", "block", false).Meaning, "a group file overrides common");
        T.Eq(5, KeyAnnotations.Load(dir, null).Count, "no group reads only common");
        T.Eq(0, KeyAnnotations.Load(Path.Combine(dir, "none"), "vehicle").Count, "a missing folder is empty");

        File.WriteAllText(Path.Combine(dir, "bad.psv"), "/a|block|추정\n");
        T.Throws<System.FormatException>(() => KeyAnnotations.Load(dir, "bad"), "a line with fewer than five fields is an error");
    }
}
