// 문자열 속 설정("이름=값;…")과 판 수 단계표("1:200 4:250"), 새 값 검사의 한 입구
using System.Linq;
using CtaToybox.Core;

static class SettingStringsTests
{
    public static void Run()
    {
        const string p = "\"flag_capture_time=60;scoreStart=250\"";
        var pieces = SettingStrings.Split(p);
        T.Eq(new[] { "flag_capture_time", "scoreStart" }, pieces.Select(x => x.Name).ToArray(), "piece names");
        T.Eq(new[] { "60", "250" }, pieces.Select(x => x.Value).ToArray(), "piece values");
        T.Eq(p, SettingStrings.Join(p, new[] { "60", "250" }), "split and join give the same bytes");
        T.Eq("\"flag_capture_time=60;scoreStart=777\"", SettingStrings.Join(p, new[] { "60", "777" }), "join changes only values");
        T.Eq("\"a=1;b=2;\"", SettingStrings.Join("\"a=1;b=2;\"", new[] { "1", "2" }), "a trailing ; is kept");
        T.Eq(2, SettingStrings.Split("\"points_table_player=0/0.000,0.333/0.277;flag_capture_time=75\"").Count, "a piece value may hold a list");
        T.True(SettingStrings.Split("\"*=1.00\"") == null, "one piece is not a piece string");
        // timers_frontlines.inc: 마지막 조각 이름이 *(나머지 모두)다
        const string timers = "\"infantry=0.66;engineer=1;*=1.00\"";
        T.Eq(new[] { "infantry", "engineer", "*" }, SettingStrings.Split(timers).Select(x => x.Name).ToArray(), "a * piece name is allowed");
        T.Eq(timers, SettingStrings.Join(timers, new[] { "0.66", "1", "1.00" }), "and joins back");
        T.True(SettingStrings.Split("\"a b=1;c=2\"") == null && SettingStrings.Split("\"a=1;x*=2\"") == null, "other non-word names are not piece strings");

        // 값 고치기 창의 시작 조각: 기존 수정의 새 값이 지금 원본과 같은 조각(이름·순서·개수)일 때만 그 값에서 시작한다
        var now = SettingStrings.Split("\"a=1;b=2\"");
        T.Eq(new[] { "1", "9" }, SettingStrings.StartPieces(now, "\"a=1;b=9\"").Select(x => x.Value).ToArray(), "start from the edit when the pieces match");
        T.Eq(new[] { "1", "2" }, SettingStrings.StartPieces(now, "\"a=5\"").Select(x => x.Value).ToArray(), "fewer pieces in the edit: start from the source");
        T.Eq(new[] { "1", "2" }, SettingStrings.StartPieces(now, "\"b=5;a=6\"").Select(x => x.Value).ToArray(), "other names or order: start from the source");
        T.Eq(new[] { "1", "2" }, SettingStrings.StartPieces(now, null).Select(x => x.Value).ToArray(), "no edit: the source");
        T.True(SettingStrings.Split("\"0:500\"") == null && SettingStrings.Split("250") == null, "other values are not piece strings");

        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=777\"") == null, "a changed value is fine");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scorestart=777\"") != null, "a renamed piece is refused");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=7;x=1\"") != null, "an extra piece (a ; typed into a value) is refused");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=7=1\"") != null, "an = typed into a value is refused");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=\"") != null, "an empty value is refused");
        T.True(SettingStrings.CheckPieces("\"a=1;b=2;\"", "\"a=1;b=3\"") != null, "the trailing ; stays");
        T.True(SettingStrings.CheckPieces("250", "300") == null, "not a piece string: nothing to check");

        T.True(SettingStrings.IsStepTable("\"1:200 4:250 6:300\"") && SettingStrings.IsStepTable("\"0:500\"") && SettingStrings.IsStepTable("\"2:5 2:5 3:5\""), "step tables");
        T.True(!SettingStrings.IsStepTable("\"0:1-4\"") && !SettingStrings.IsStepTable("\"a:1\"") && !SettingStrings.IsStepTable("200"), "not step tables");
        T.True(SettingStrings.CheckSteps("\"1:200 4:250\"", "\"1:300 4:350 8:400\"") == null, "more entries of the same shape are fine");
        T.True(SettingStrings.CheckSteps("\"1:200 4:250\"", "\"1:300,4:350\"") != null, "commas are refused");
        T.True(SettingStrings.CheckSteps("\"2:5 2:5\"", "\"2:5:1 2:5\"") != null, "each entry keeps its count of numbers");
        T.Eq("1 → 200, 4 → 250", SettingStrings.StepPreview("\"1:200 4:250\""), "preview");
        T.True(SettingStrings.StepPreview("250") == null, "no preview for other values");

        const string area = "set/multiplayer/games/battle_zones.set", unit = "set/breed/mp/ger/mid/medic.set";
        string warning;
        T.True(ValueRules.CheckNewValue(area, p, "\"flag_capture_time=60;scoreStart=7;x=1\"", out warning) != null, "ValueRules checks pieces in the settings area");
        T.True(ValueRules.CheckNewValue(area, "\"1:200\"", "\"1:200,4:5\"", out warning) != null, "and step tables");
        T.True(ValueRules.CheckNewValue(unit, "\"a=1;b=2\"", "\"a=1;b=2;c=3\"", out warning) == null, "not outside the settings area");
        T.True(ValueRules.CheckNewValue(area, "250", "2 0", out warning) != null, "the token rules still apply");
    }
}
