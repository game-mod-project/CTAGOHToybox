using System.IO;
using System.Linq;
using CtaToybox.Core;

// 시야 파일(set/vision)을 게임 설정으로 연다(월드워 Task 1)
static class VisionSettingsTests
{
    public const string VisionSet = "set/vision/vision.set", Fields = "set/vision/vision_fields.inc";

    public static (string Name, string Text)[] Files()
    {
        return new[]
        {
            (VisionSet, "{visions\n (define \"radius\" (mod \"mp\" {radius (* 1.0 %r)}) (mod not \"mp\" {radius (* 1.0 %r)}))\n (include \"vision_fields.inc\")\n}"),
            (Fields, "{\"human\"\n {uncover 20}\n {h_fov 150}\n {v_fov 180}\n}"),
        };
    }

    static string Fixture(out string repo)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), Files());
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "settings.psv"),
            "**/h_fov|block|확인|수평 시야각(도). 360이면 모든 방향을 본다 (확인: vision_fields.inc 주석 \"Horizontal field of view arc\")|C\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        T.True(SettingFiles.IsTop(VisionSet), "vision.set is a settings top file");
        T.True(!SettingFiles.IsTop(Fields), "its include is not a top file");
        T.True(SettingFiles.InArea("set/vision/spotting.inc"), "the vision folder is a settings area");

        string repo;
        string settings = Fixture(out repo);
        using var session = EditorSession.Open(settings, repo, null);
        T.Eq("settings", session.Index.Units.Single(u => u.Path == VisionSet).EditorKind, "listed as game settings");
        var view = session.OpenUnit(VisionSet);
        var fov = view.Rows.Single(r => r.Path == "/visions/\"*\"/h_fov");
        T.Eq(Fields, fov.Values[0].Trace.File, "the value is located in vision_fields.inc");
        T.True(fov.Values[0].Editable, "and is editable");
        T.Eq(SettingFiles.UnitScopeNote, fov.Values[0].UnitScopeBlocker, "shared only");
        T.Eq("확인", fov.Note == null ? null : fov.Note.Grade, "settings.psv annotates vision keys");

        // 따옴표 이름 블록의 이름은 다른 파일이 그 이름으로 부르므로({vision "human"}) 고치지 못한다
        var name = view.Rows.Single(r => r.Path == "/visions/\"*\"").Values[0];
        T.True(!name.Editable, "a quoted block's own name is not editable");
        T.True(name.Trace.Note != null && name.Trace.Note.Contains("블록 이름"), "and says why");

        // E16: 수정 목록이 어느 필드의 값인지 보이게 따옴표 블록 이름을 건넨다
        T.Eq(new[] { "tank_range_200" }, KeyLabel.QuotedNames("/visions/\"*\"/h_fov", "/visions/\"tank_range_200\"/h_fov").ToArray(), "the quoted block name");
        T.Eq(new[] { "a/b", "c" }, KeyLabel.QuotedNames("/x/\"*\"/y/\"*\"/z", "/x/\"a/b\"/y/\"c\"/z").ToArray(), "several names, a slash inside quotes");
        T.Eq(0, KeyLabel.QuotedNames("/x/y", "/x/y").Count, "none outside quoted blocks");
        session.OpenProject(Path.Combine(repo, "editor", "projects", "e16.json"));
        var edit = session.AddEdit(view, fov.Values[0], "360", "shared");
        T.Eq(new[] { "human" }, session.Describe(edit).BlockNames.ToArray(), "the edit description carries the field name");
    }
}
