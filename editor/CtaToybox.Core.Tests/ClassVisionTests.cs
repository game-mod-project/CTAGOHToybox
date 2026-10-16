using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// E18: vision-preset --classes. 차종 필드 셋과 유닛별 around 시야 바꾸기
static class ClassVisionTests
{
    const string Unarmed = "properties/tank_unarmed.ext", Car = "properties/car.ext";
    const string Light = "entity/-vehicle/x/tank_light/l1/l1.def", Medium = "entity/-vehicle/x/tank_medium/m1/m1.def", Heavy = "entity/-vehicle/x/tank_heavy/h1/h1.def", LightX = "entity/-vehicle/x/tank_light/l1/l1_x.def";

    static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            (VisionPresets.VisionSet, "{visions\n (include \"vision_fields.inc\")\n}"),
            (VisionPresets.FieldsFile, BlockCopyBuildTests.FieldsText));
        string tank = "{game_entity\n (include \"/properties/tank_unarmed.ext\")\n {mass 10}\n}";
        T.Zip(Path.Combine(game, "resource", "properties.pak"),
            (Unarmed, "{sensor\n {visor \"main\" {vision \"tank_range_200\"}}\n {Visor \"driver\" {vision \"tank_driver\"}}\n {visor \"around\" {vision \"tank_around\"}}\n}"),
            (Car, "{sensor\n {visor \"main\" {vision \"vehicle_main\"}}\n {visor \"around\" {vision \"human_peripheral\"}}\n}"));
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/tank_light/l1/l1.def", tank), ("-vehicle/x/tank_medium/m1/m1.def", tank), ("-vehicle/x/tank_heavy/h1/h1.def", tank),
            ("-vehicle/x/tank_light/l1/l1_x.def", tank));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    static string NewOf(EditProject p, string named) { return p.Edits.Single(e => e.NamedPath == named).New; }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        string projectPath = Path.Combine(repo, "editor", "projects", "classes.json");
        using (var session = EditorSession.Open(settings, repo, null))
        {
            var notes = new List<string>();
            var project = VisionPresets.Build(session.Stack, "wot", 4, notes);
            var report = VisionPresets.AddClasses(project, session.Stack, session.Index.Units, new UnitTracer(session.Stack), u => session.Index.HeirsOf(u).Count, new ClassVision(), notes);
            T.Eq("월드워-차종", project.Name, "the class project name");
            T.Eq("100", NewOf(project, "/\"tank_range_200\"/uncover"), "the heavy (tank field) detection is 100");
            T.Eq(new[] { "\"ctb_tank_medium\"", "\"ctb_tank_light\"", "\"ctb_vehicle\"" }, project.Edits.Where(e => e.Kind == BlockCopy.Kind).Select(e => e.New).ToArray(), "three copies of tank_range_200");
            T.Eq("130", NewOf(project, "/\"ctb_tank_light\"/uncover"), "light detection");
            T.Eq("1.8", NewOf(project, "/\"ctb_tank_light\"/(\"spotting2\")"), "light vision r");
            T.Eq("360", NewOf(project, "/\"ctb_tank_light\"/h_fov"), "all around");
            T.Eq("115", NewOf(project, "/\"ctb_tank_medium\"/uncover"), "medium detection");
            T.Eq("1.65", NewOf(project, "/\"ctb_tank_medium\"/(\"spotting2\")"), "medium vision r");
            T.Eq("145", NewOf(project, "/\"ctb_vehicle\"/uncover"), "vehicle detection");
            T.Eq("1.8", NewOf(project, "/\"ctb_vehicle\"/(\"spotting2\")"), "vehicle vision r");

            var unitEdits = project.Edits.Where(e => e.Scope == "unit").ToList();
            T.Eq(new[] { Light, Medium }, unitEdits.Select(e => e.Unit).OrderBy(x => x == Light ? 0 : 1).ToArray(), "one around edit for the light and the medium tank");
            T.Eq("\"ctb_tank_light\"", unitEdits.Single(e => e.Unit == Light).New, "the light tank's around field");
            T.Eq("\"tank_around\"", unitEdits.Single(e => e.Unit == Medium).Old, "the around visor, not main or driver");
            T.Eq(Unarmed, unitEdits[0].File, "in the shared tank file");
            var carEdit = project.Edits.Single(e => e.File == Car);
            T.True(carEdit.Scope == "shared" && carEdit.New == "\"ctb_vehicle\"" && carEdit.Old == "\"human_peripheral\"", "the car file's around field, shared");
            T.Eq(1, report.Light, "light count");
            T.Eq(1, report.Medium, "medium count");
            T.Eq(1, report.VehicleFiles, "vehicle file count (armoredcar and moto are missing here)");
            T.True(notes.Any(n => n.Contains("armoredcar.ext")), "a missing vehicle file is noted");

            project.Save(projectPath);
            session.OpenProject(projectPath);
            T.True(session.CheckEdits().All(c => c.Status == EditStatus.Applied), "every edit checks");
            var build = session.Build();
            T.True(build.Results.All(r => r.Status == EditStatus.Applied), "and builds");
            T.True(!File.Exists(Path.Combine(outDir, "resource", "properties", "tank_unarmed.ext")), "the shared tank file is not rewritten (the heavy tank keeps tank_around)");
            T.True(!File.Exists(Path.Combine(outDir, "resource", "entity", "-vehicle", "x", "tank_heavy", "h1", "h1.def")), "the heavy tank is not written");
            var copies = Directory.EnumerateFiles(Path.Combine(outDir, "resource", "properties")).Where(f => Path.GetFileName(f).StartsWith("tank_unarmed__ctb_")).Select(File.ReadAllText).ToList();
            T.Eq(2, copies.Count, "one tank file copy per changed unit");
            T.True(copies.Any(t => t.Contains("\"ctb_tank_light\"")) && copies.Any(t => t.Contains("\"ctb_tank_medium\"")), "each names its class field");

            var custom = VisionPresets.Build(session.Stack, "wot", 4, new List<string>());
            VisionPresets.AddClasses(custom, session.Stack, session.Index.Units, new UnitTracer(session.Stack), u => 0, new ClassVision { LightUncover = 140 }, new List<string>());
            T.Eq("140", NewOf(custom, "/\"ctb_tank_light\"/uncover"), "an option value is used");
        }

        // 명령줄
        string cliProject = Path.Combine(repo, "editor", "projects", "classes-cli.json");
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "vision-preset", "--settings", settings, "--variant", "wot", "--classes", "--project", cliProject, "--light-uncover", "135" }, w), "CLI --classes: " + w);
        T.True(w.ToString().Contains("경전차 1") && w.ToString().Contains("중형전차 1"), "it reports the counts");
        T.Eq("135", NewOf(EditProject.Load(cliProject), "/\"ctb_tank_light\"/uncover"), "with the option");
        T.Eq(2, CliApp.Run(new[] { "vision-preset", "--settings", settings, "--variant", "wt", "--classes", "--project", Path.Combine(repo, "x.json") }, new StringWriter()), "--classes needs the wot variant");
    }
}
