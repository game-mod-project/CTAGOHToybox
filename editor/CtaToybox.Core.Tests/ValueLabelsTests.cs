// 값 이름(data/annotations/values.psv): 호출 이름, 자리 번호, 열쇠말 뒤 자리, 열쇠말 자체, 모든 값
using System.Linq;
using System.IO;
using CtaToybox.Core;

static class ValueLabelsTests
{
    public static void Run()
    {
        string dir = T.TempDir(), file = Path.Combine(dir, "values.psv");
        File.WriteAllText(file,
            "# 주석\n" +
            "*/(\"mobility_car\")|speed|최고 속도|C\n" +
            "**/box/item|#1|아이템 이름|A\n**/box/item|#2|수량|A\n**/box/item|filling#1|채운 것|A\n**/box/item|filling#2|채운 양|A\n" +
            "/breed/inventory/item|#1|아이템 이름|A\n/breed/inventory/item|=filled|장전된 채로|M\n" +
            "*/gears|*|전진 {n}단|A\n*/props|*|태그 {n}|A\n");
        var l = ValueLabels.Load(file);
        T.Eq(9, l.Count, "entries");
        T.Eq(new[] { "최고 속도", null }, l.Of("/game_entity/chassis/(\"mobility_car\")", new[] { "72", "14" }, new[] { "speed", "reverse" }).ToArray(), "a call argument is named by its call; unknown names stay null");
        T.Eq(new[] { "아이템 이름", "수량" }, l.Of("/game_entity/extender[inventory]/box/item", new[] { "\"mine m1a1\"", "24" }, new string[] { null, null }).ToArray(), "positions");
        T.Eq(new[] { "아이템 이름", "수량", "열쇠말 filling (뒤의 값을 묶는다)", "채운 것", "채운 양" },
            l.Of("/game_entity/extender[inventory]/box/item", new[] { "\"barrel\"", "20", "filling", "\"fuel\"", "420" }, new string[5]).ToArray(), "values after a keyword count from the keyword");
        T.Eq(new[] { "아이템 이름", "장전된 채로" }, l.Of("/breed/inventory/item", new[] { "\"luger\"", "filled" }, new string[2]).ToArray(), "a keyword can have its own name");
        T.Eq(new[] { "전진 1단", "전진 2단" }, l.Of("/game_entity/chassis/locomotion/gears", new[] { "0.5", "0.65" }, new string[2]).ToArray(), "every value with its number");
        T.Eq(new[] { "태그 1", "태그 2" }, l.Of("/game_entity/props", new[] { "steel", "vehicle" }, new string[2]).ToArray(), "bare words of a * key are not keywords");
        T.Eq(new string[] { null, null }, l.Of("/x/unknown", new[] { "a", "1" }, new string[2]).ToArray(), "unknown keys have no names");
        T.Eq(0, ValueLabels.Load(Path.Combine(dir, "none.psv")).Count, "a missing file is empty");
    }
}
