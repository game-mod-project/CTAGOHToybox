using System.Linq;
using System.IO;
using CtaToybox.Core;

static class SummaryKeysTests
{
    public static void Run()
    {
        T.Eq("무게", SummaryKeys.ShortLabel("무게. 킬로그램"), "the label stops at the first period");
        T.Eq("최고 속도", SummaryKeys.ShortLabel("최고 속도, km/h"), "or at a comma");
        T.Eq("관통 피해 기준", SummaryKeys.ShortLabel("관통 피해 기준 (값은 mm)"), "or at a parenthesis");
        T.Eq("가나다라마바사아자차카타파하가나다라마바사아자차카타파하가나…", SummaryKeys.ShortLabel("가나다라마바사아자차카타파하가나다라마바사아자차카타파하가나다라마바사"), "long labels are cut at 30 characters");

        string ann = T.TempDir(), summary = T.TempDir();
        File.WriteAllText(Path.Combine(ann, "common.psv"),
            "*/maxspeed|block|확인|최고 속도, km/h|C\n" +
            "**/armor/front|block|확인|전면 장갑. mm|C\n" +
            "*/mass|block|추정|무게|S\n");
        var notes = KeyAnnotations.Load(ann, "vehicle");

        var fromNotes = SummaryKeys.Load(summary, "vehicle", notes);
        T.True(fromNotes.SourceFile == null, "without a summary file the keys come from annotations");
        T.Eq(2, fromNotes.Count, "only 확인 entries");
        T.Eq("최고 속도", fromNotes.LabelOf("/game_entity/chassis/(\"mobility_tank\")/maxspeed"), "*/name matches expanded paths");
        T.Eq("전면 장갑", fromNotes.LabelOf("/game_entity/props/armor/front"), "**/tail matches");
        T.True(fromNotes.LabelOf("/game_entity/mass") == null, "a 추정 key is not in the summary");

        File.WriteAllText(Path.Combine(summary, "vehicle.psv"), "# 요약 키\n**/box/item|적재품\n/game_entity/mass|무게(kg)\n");
        var fromFile = SummaryKeys.Load(summary, "vehicle", notes);
        T.Eq(Path.Combine(summary, "vehicle.psv"), fromFile.SourceFile, "a summary file wins");
        T.Eq(2, fromFile.Count, "its entries");
        T.Eq(new[] { "적재품", "무게(kg)" }, fromFile.Labels.ToArray(), "labels keep the file order for grouping");
        T.Eq("무게(kg)", fromFile.LabelOf("/game_entity/mass"), "an exact path");
        T.Eq("적재품", fromFile.LabelOf("/game_entity/extender[inventory]/box/item"), "a tail");
        T.True(fromFile.LabelOf("/game_entity/chassis/maxspeed") == null, "annotation keys are not added when the file exists");
        T.Eq(2, SummaryKeys.Load(summary, "breed", notes).Count, "another kind without a file falls back to annotations");

        // 따옴표 이름 줄: 펼친 경로는 "*"로 묶이지만 요약 파일은 이름을 넣은 경로로 줄을 나눌 수 있다
        File.WriteAllText(Path.Combine(summary, "breed.psv"), "/breed/perks/\"health\"|체력\n**/perks/\"speed\"|이동 속도\n/breed/perks/\"*\"|그 밖의 기술\n");
        var named = SummaryKeys.Load(summary, "breed", notes);
        T.Eq("체력", named.LabelOf("/breed/perks/\"*\"", "/breed/perks/\"health\""), "a named exact path splits the quoted-name rows");
        T.Eq("체력", named.LabelOf("/breed/perks/\"*\"", "/Breed/Perks/\"Health\""), "names match without case");
        T.Eq("이동 속도", named.LabelOf("/breed/perks/\"*\"", "/breed/perks/\"speed\""), "a named tail");
        T.Eq("그 밖의 기술", named.LabelOf("/breed/perks/\"*\"", "/breed/perks/\"rifle\""), "other names fall back to the \"*\" line");
        T.Eq("그 밖의 기술", named.LabelOf("/breed/perks/\"*\""), "without a raw path the \"*\" line is used");
        T.Eq("그 밖의 기술", named.LabelOf("/breed/perks/\"*\"", "/breed/perks/\"a/b\""), "a name with a slash does not line up and falls back");
        T.Eq(new[] { "/breed/perks/\"health\"", "**/perks/\"speed\"", "/breed/perks/\"*\"" }, named.Keys.ToArray(), "Keys in file order");
        T.Eq("/breed/perks/\"health\"", named.KeyOf("/breed/perks/\"*\"", "/breed/perks/\"health\""), "KeyOf names the matching key");
        T.True(named.KeyOf("/x/y") == null, "KeyOf is null outside the summary");
    }
}
