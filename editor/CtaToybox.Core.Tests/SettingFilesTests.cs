// 게임 설정 파일(하위 프로젝트 B)의 판정, 이름 표, 유닛 색인
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class SettingFilesTests
{
    public const string Res = "set/dynamic_campaign/resources_standard.set", Dur = "set/dynamic_campaign/duration_normal.set",
        Ctf = "set/multiplayer/games/campaign_capture_the_flag.set", Bz = "set/multiplayer/games/battle_zones.set",
        Limits = "set/multiplayer/games/presets/global_parameters/unitlimits.inc", Dl = "set/difficulty/normal.dl", DlInc = "set/difficulty/difficulty_generic.inc";

    // 바닐라 꼴을 줄인 게임 설정 파일들
    public static (string Name, string Text)[] Files()
    {
        return new[]
        {
            (Res, "{Standard\n\t{GlobalMaxCP 550}\n\t{Bots {StartMP 1.00 min 99999 attackerMultiplier 1.00}}\n\t{Resources\n\t\t{MP {StartVal 900} {WinGain \"1:200 4:250\"}}\n\t\t{RP {StartVal 6}}\n\t}\n}"),
            (Dur, "{Normal\n\t{FlagCount \"2:5 2:5 3:5\"}\n}"),
            (Ctf, "{game\n\t{parameters \"flag_capture_time=75;scoreStart=250\"}\n\t{settings {scoreFinal 1000}}\n}"),
            (Bz, "{game\n\t{parameters \"flag_capture_time=60;scoreStart=250\"}\n\t(include \"presets/global_parameters/unitlimits.inc\")\n}"),
            (Limits, "{unitLimits \"officer=2;sniper=5\"}"),
            (Dl, "{difficulty\n\t(include \"difficulty_generic.inc\")\n\t{enemy {health 0.50}}\n}"),
            (DlInc, "\t{any {burst 1}}"),
            ("set/dynamic_campaign/values.set", "{GameModes \"campaign_capture_the_flag\"}"),
            ("set/dynamic_campaign/unit_research_ger.set", "{tech \"a\" costs 5}"),
        };
    }

    public static void Run()
    {
        T.True(SettingFiles.IsTop(Res) && SettingFiles.IsTop(Dur) && SettingFiles.IsTop(Ctf) && SettingFiles.IsTop(Bz) && SettingFiles.IsTop(Dl), "top setting files");
        T.True(SettingFiles.IsTop("SET/Dynamic_Campaign/Resources_High.set"), "case does not matter");
        T.True(!SettingFiles.IsTop(Limits) && !SettingFiles.IsTop(DlInc), "included files are not list entries");
        T.True(!SettingFiles.IsTop("set/dynamic_campaign/values.set") && !SettingFiles.IsTop("set/dynamic_campaign/unit_research_ger.set") && !SettingFiles.IsTop("set/dynamic_campaign/map_points.set"), "values, research and map list are out of scope");
        T.True(!SettingFiles.IsTop("set/multiplayer/games/gamemode.template"), "only .set game modes");
        T.True(SettingFiles.InArea(Limits) && SettingFiles.InArea(DlInc) && SettingFiles.InArea("script/multiplayer/modes/conquest/waves.lua"), "the settings area holds included files too");
        T.True(!SettingFiles.InArea("set/breed/mp/ger/mid/medic.set"), "units are not in the settings area");

        T.Eq("settings", UnitKind.OfEditor(Res), "OfEditor adds the settings kind");
        T.Eq("breed", UnitKind.OfEditor("set/breed/mp/ger/mid/medic.set"), "OfEditor keeps the unit kinds");
        T.True(UnitKind.Of(Res) == null, "Of stays the same as Common.ps1");
        T.Eq("settings", UnitKind.EditorKindOf("settings", new string[0]), "editor kind");
        T.True(UnitKind.InEditor("settings"), "settings is shown in the editor");
        T.Eq("게임 설정", UnitKind.Label("settings"), "label");

        string dir = T.TempDir(), psv = Path.Combine(dir, "files.psv");
        File.WriteAllText(psv, "# 주석\n" + Res + "|컨퀘스트 자원|컨퀘스트 자원 — standard\n\n" + Dl + "|난이도|난이도 — normal\n");
        var names = SettingFiles.LoadNames(psv);
        T.Eq(2, names.Count, "names");
        T.Eq("컨퀘스트 자원", names[Res.ToUpperInvariant()].Group, "group, case-insensitive key");
        T.Eq("난이도 — normal", names[Dl].Name, "name");
        T.Eq(0, SettingFiles.LoadNames(Path.Combine(dir, "none.psv")).Count, "a missing file is empty");
        File.WriteAllText(psv, Res + "|두 칸\n");
        T.Throws<System.FormatException>(() => SettingFiles.LoadNames(psv), "a short line is a format error");

        var display = DisplayNames.Load(Path.Combine(dir, "none.csv"));
        display.AddPathName(Res, "컨퀘스트 자원 — standard");
        T.Eq("컨퀘스트 자원 — standard", display.OfUnit(Res), "a path name");

        string game = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), Files());
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", Path.Combine(game, "resource")) });
        stack.Scan();
        var index = UnitIndex.Build(stack, null);
        var settings = index.Units.Where(u => u.Kind == "settings").Select(u => u.Path).OrderBy(p => p).ToArray();
        T.Eq(new[] { Dl, Dur, Res, Bz, Ctf }.OrderBy(p => p).ToArray(), settings, "the index lists the top setting files");
        T.True(index.Units.Where(u => u.Kind == "settings").All(u => u.EditorKind == "settings"), "with the settings editor kind");
        T.True(index.Units.Single(u => u.Path == Res).Numbers > 0, "numbers are counted");
        T.Eq(0, index.Units.Single(u => u.Path == Dur).Numbers, "a duration file holds only step-table strings (the list must not hide it, Task 2)");
        T.Eq(new[] { Bz }, index.UsersOf(Limits).ToArray(), "an included preset counts its users");
    }
}
