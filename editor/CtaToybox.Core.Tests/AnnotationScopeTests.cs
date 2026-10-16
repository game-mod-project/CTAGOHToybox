using System.IO;
using System.Linq;
using CtaToybox.Core;

// Q8: */이름 주석은 유닛 파일과 펼친 키에만 붙는다(tools/lib/Annotations.ps1 Get-Annotation과 같은 규칙).
// 게임 설정은 settings.psv의 */이름만 쓴다. 요약 키(주석 대체)는 종류와 주석 묶음으로 나눠 기억한다
static class AnnotationScopeTests
{
    const string Unit = "entity/-vehicle/x/a/a.def", Inc = "entity/-vehicle/x/a/s.inc", Stuff = "set/stuff/smg/x.inc";

    static string Fixture(out string repo)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity\n (include \"s.inc\")\n {mass 10}\n}"),
            ("-vehicle/x/a/s.inc", "{position 3 (\"mobility_tank\" speed(9))}"));
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/stuff/smg/x.inc", "{weight 4}"),
            (VisionSettingsTests.VisionSet, "{visions\n (include \"vision_fields.inc\")\n}"),
            (VisionSettingsTests.Fields, "{\"human\"\n {uncover 20}\n {h_fov 150}\n {mass 3}\n}"));
        string ann = Path.Combine(repo, "data", "annotations");
        Directory.CreateDirectory(ann);
        File.WriteAllText(Path.Combine(ann, "common.psv"),
            "*/mass|block|확인|무게. 킬로그램|C\n*/position|block|확인|자리|C\n*/(\"mobility_tank\")|macro|확인|전차 기동|C\n");
        File.WriteAllText(Path.Combine(ann, "vehicle.psv"), "*/flag|block|확인|차량 깃발|C\n");
        File.WriteAllText(Path.Combine(ann, "weapon-ammo.psv"), "*/weight|block|확인|아이템 무게|C\n");
        File.WriteAllText(Path.Combine(ann, "settings.psv"), "*/uncover|block|확인|근접 발견 거리|C\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        string dir = T.TempDir();
        File.WriteAllText(Path.Combine(dir, "common.psv"), "*/mass|block|확인|무게|C\n**/box/item|block|추정|칸|S\n");
        File.WriteAllText(Path.Combine(dir, "settings.psv"), "*/uncover|block|확인|근접|C\n");
        var plain = KeyAnnotations.Load(dir, "settings", false);
        T.True(plain.Find("/a/mass", "block", true) == null, "common */name is left out when asked");
        T.Eq("칸", plain.Find("/a/box/item", "block", true).Meaning, "common **/tail is still read");
        T.Eq("근접", plain.Find("/a/uncover", "block", true).Meaning, "the group file's */name stays");

        string repo;
        string settings = Fixture(out repo);
        using var session = EditorSession.Open(settings, repo, null);

        var unit = session.OpenUnit(Unit);
        T.Eq("확인", unit.Rows.Single(r => r.Path == "/game_entity/mass").Note?.Grade, "*/name applies to a unit's expanded key");
        T.Eq("자리", unit.Rows.Single(r => r.Path == "/game_entity/position").Note?.Meaning, "and to a key spliced in from an include");
        var call = unit.Rows.Single(r => r.IsMacroCall && r.Values.Any(v => v.Trace.File == Inc));
        T.True(call.Note == null, "a macro call row read from the include file itself takes no */name note");

        var vision = session.OpenUnit(VisionSettingsTests.VisionSet);
        T.True(vision.Rows.Single(r => r.Path.EndsWith("/mass")).Note == null, "common */name does not reach a settings file");
        T.Eq("근접 발견 거리", vision.Rows.Single(r => r.Path.EndsWith("/uncover")).Note?.Meaning, "settings.psv */name does");

        // 같은 '종류 없음'이라도 주석 묶음(vehicle, weapon-ammo)이 다르면 요약 키가 섞이지 않는다
        var inc = session.OpenUnit(Inc);
        var stuff = session.OpenUnit(Stuff);
        T.True(inc != null && stuff != null, "files without a unit kind still open");
        T.True(session.SummaryOf(inc).Labels.Contains("차량 깃발"), "the vehicle file's summary comes from vehicle.psv");
        T.True(!session.SummaryOf(stuff).Labels.Contains("차량 깃발"), "the stuff file's summary is not the cached vehicle one");
        T.True(session.SummaryOf(stuff).Labels.Contains("아이템 무게"), "it comes from weapon-ammo.psv");

        var edit = session.Describe(new Edit { Unit = Unit, File = Inc, Path = "/position", Occurrence = 0, Arg = 0, Old = "3", New = "4" });
        T.True(edit != null && edit.Note == null, "the edit list does not put */name on a row of a non-unit file");
    }
}
