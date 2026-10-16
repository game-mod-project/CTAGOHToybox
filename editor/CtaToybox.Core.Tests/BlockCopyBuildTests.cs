using System.IO;
using System.Linq;
using CtaToybox.Core;

// E18: 수정 점검과 빌드가 블록 복제를 먼저 반영하고, 복제가 없으면 그 이름을 쓰는 수정을 막는다
static class BlockCopyBuildTests
{
    public const string Fields = "set/vision/vision_fields.inc", VisionSet = "set/vision/vision.set", Unarmed = "properties/tank_unarmed.ext";

    public static string FieldsText = "{\"tank_range_200\"\n {uncover 50}\n {h_fov 15}\n (\"spotting2\" r(1.5) r_human(0.66) veh_vis(50) can_vis(50) can_fire(70))\n}\n{\"human\"\n {uncover 20}\n {h_fov 150}\n}\n";

    public static string Fixture(out string repo, out string outDir, string fields = null)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            (VisionSet, "{visions\n (include \"vision_fields.inc\")\n}"),
            (Fields, fields ?? FieldsText));
        T.Zip(Path.Combine(game, "resource", "properties.pak"),
            (Unarmed, "{sensor\n {visor \"main\" {vision \"tank_range_200\"}}\n {visor \"around\" {vision \"tank_around\"}}\n}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    static Edit Value(string file, string path, int occurrence, int arg, string old, string @new, string named)
    {
        return new Edit { Unit = file == Fields ? VisionSet : file, File = file, Path = path, Occurrence = occurrence, Arg = arg, Old = old, New = @new, NamedPath = named, Scope = "shared" };
    }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using (var session = EditorSession.Open(settings, repo, null))
        {
            session.OpenProject(Path.Combine(repo, "editor", "projects", "e18.json"));
            var copy = BlockCopyTests.Copy("tank_range_200", "ctb_tank_light");
            // 복제 블록은 파일 끝에 들어가므로 /"*"/uncover의 셋째(0 원본, 1 human, 2 복제)다
            var inCopy = Value(Fields, "/\"*\"/uncover", 2, 0, "50", "130", "/\"ctb_tank_light\"/uncover");
            var human = Value(Fields, "/\"*\"/h_fov", 1, 0, "150", "360", "/\"human\"/h_fov");
            var around = Value(Unarmed, "/sensor/visor/vision", 1, 0, "\"tank_around\"", "\"ctb_tank_light\"", null);
            session.Project.Edits.AddRange(new[] { copy, inCopy, human, around });
            T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "the check applies copies first");

            var report = session.Build();
            T.Eq(4, report.Count(EditStatus.Applied), "the build applies all four");
            string text = File.ReadAllText(Path.Combine(outDir, "resource", "set", "vision", "vision_fields.inc"));
            string error;
            var rows = SourceRows.Of(Fields, text, out error);
            T.True(rows != null, "the output parses");
            T.Eq("130", rows.Single(r => r.RawPath == "/\"ctb_tank_light\"/uncover").Leaves[0].Text, "the value inside the copy");
            T.Eq("360", rows.Single(r => r.RawPath == "/\"human\"/h_fov").Leaves[0].Text, "an edit of another field keeps its slot");
            T.Eq("50", rows.Single(r => r.RawPath == "/\"tank_range_200\"/uncover").Leaves[0].Text, "the source block is unchanged");
            T.True(File.ReadAllText(Path.Combine(outDir, "resource", "properties", "tank_unarmed.ext")).Contains("\"ctb_tank_light\""), "the unit file names the new field");

            copy.Enabled = false;
            var checks = session.CheckEdits();
            foreach (var dependent in new[] { inCopy, around })
            {
                var c = checks.Single(x => x.Edit == dependent);
                T.Eq(EditStatus.Invalid, c.Status, "a dependent edit is refused when the copy is off");
                T.True(c.Message.Contains("필드 ctb_tank_light가 없다"), "with the reason");
            }
            T.Eq(EditStatus.Applied, checks.Single(x => x.Edit == human).Status, "an unrelated edit still applies");
            report = session.Build();
            T.Eq(EditStatus.Invalid, report.Results.Single(r => r.Edit == around).Status, "the build refuses it too");
            T.True(report.Results.Single(r => r.Edit == inCopy).Message.Contains("필드 ctb_tank_light가 없다"), "with the same reason");
            T.True(!File.Exists(Path.Combine(outDir, "resource", "properties", "tank_unarmed.ext")), "the unit file is not written");
        }

        // 원본 블록이 없는 판
        settings = Fixture(out repo, out outDir, "{\"human\"\n {uncover 20}\n}\n");
        using (var session = EditorSession.Open(settings, repo, null))
        {
            session.OpenProject(Path.Combine(repo, "editor", "projects", "e18b.json"));
            var copy = BlockCopyTests.Copy("tank_range_200", "ctb_tank_light");
            var around = Value(Unarmed, "/sensor/visor/vision", 1, 0, "\"tank_around\"", "\"ctb_tank_light\"", null);
            session.Project.Edits.AddRange(new[] { copy, around });
            var checks = session.CheckEdits();
            T.Eq(EditStatus.TargetMissing, checks.Single(c => c.Edit == copy).Status, "a copy of a missing block");
            T.Eq(EditStatus.Invalid, checks.Single(c => c.Edit == around).Status, "makes its dependents invalid");
            var report = session.Build();
            T.Eq(EditStatus.TargetMissing, report.Results.Single(r => r.Edit == copy).Status, "the build agrees on the copy");
            T.Eq(EditStatus.Invalid, report.Results.Single(r => r.Edit == around).Status, "and on the dependent");
        }
    }
}
