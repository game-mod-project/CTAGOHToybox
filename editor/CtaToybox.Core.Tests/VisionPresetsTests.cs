using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// 월드워 시야 프로젝트를 만드는 VisionPresets와 명령 vision-preset(월드워 Task 2)
static class VisionPresetsTests
{
    const string FieldsText =
        "{\"extra\" {h_fov 10} (\"spotting\" r(1.0))}\n" +
        "{\"human\" {uncover 20} {h_fov 150} {v_fov 180} (\"spotting\" r(1.0))}\n" +
        "{\"vehicle_around\" {uncover 15} {h_fov 360} (\"spotting\" r(0.6))}\n" +
        "{\"tank_range_200\" {uncover 20} {h_fov 15} (\"spotting2\" r(1.5) r_human(0.66) veh_vis(50) can_vis(50) can_fire(70))}\n" +
        "{\"watchtower\" {h_fov 90} (\"spotting\" r(1))}\n" +
        "{\"recon_open\" {uncover 50} {h_fov 360} (\"spotting\" r(2.2))}\n" +
        "{\"airborne_main\" {uncover 0} {h_fov 110} (\"spotting\" r(3.5))}";

    static void PutFiles(string resource)
    {
        T.Put(resource, VisionPresets.FieldsFile, FieldsText);
        T.Put(resource, VisionPresets.VisionSet, VisionSettingsTests.Files()[0].Text);
    }

    static int Run(params string[] args) { return CliApp.Run(args, new StringWriter()); }

    public static void Run()
    {
        string vanilla = T.TempDir();
        PutFiles(vanilla);
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();

        var notes = new List<string>();
        var wot = VisionPresets.Build(stack, "wot", 4, notes);
        T.Eq("월드워", wot.Name, "the WoT project name");
        var human = wot.Edits.Single(e => e.NamedPath == "/\"human\"/h_fov");
        T.Eq("150", human.Old, "an inserted field does not shift the target");
        T.Eq("360", human.New, "h_fov becomes 360");
        T.Eq("set/vision/vision_fields.inc", human.File, "edits the fields file");
        T.Eq("set/vision/vision.set", human.Unit, "under the vision settings item");
        T.Eq("shared", human.Scope, "shared");
        T.Eq("50", wot.Edits.Single(e => e.NamedPath == "/\"human\"/uncover").New, "uncover becomes 50");
        T.True(!wot.Edits.Any(e => e.NamedPath == "/\"vehicle_around\"/h_fov"), "a field outside the ground list makes no edit");
        T.True(!wot.Edits.Any(e => e.NamedPath.StartsWith("/\"recon_open\"/")), "a ground field already at the target (h_fov 360, uncover 50) makes no edit");
        T.True(!wot.Edits.Any(e => e.NamedPath.Contains("airborne") || e.NamedPath.Contains("extra")), "fields outside the ground list are untouched");
        T.True(!wot.Edits.Any(e => e.NamedPath == "/\"watchtower\"/uncover") && notes.Any(n => n.Contains("watchtower") && n.Contains("uncover")), "missing keys and fields are noted, not invented");
        T.True(notes.Any(n => n.Contains("human_elite")), "a ground field absent from the file is noted");
        T.True(!wot.Edits.Any(e => e.NamedPath.Contains("(\"spotting")), "WoT keeps the distance multipliers");
        T.True(wot.Edits.All(e => e.File == VisionPresets.FieldsFile), "vision.set is never edited");

        notes.Clear();
        var wt = VisionPresets.Build(stack, "wt", 4, notes);
        T.Eq("월드워-워썬더", wt.Name, "the War Thunder project name");
        T.True(!wt.Edits.Any(e => e.NamedPath.EndsWith("/uncover")), "WT keeps uncover");
        var r1 = wt.Edits.Single(e => e.NamedPath == "/\"human\"/(\"spotting\")");
        T.Eq(new[] { "1.0", "4" }, new[] { r1.Old, r1.New }, "spotting r x4");
        var r2 = wt.Edits.Single(e => e.NamedPath == "/\"tank_range_200\"/(\"spotting2\")");
        T.Eq(new[] { "1.5", "6", "0" }, new[] { r2.Old, r2.New, r2.Arg.ToString() }, "spotting2 r x4, the r argument only");
        T.True(!wt.Edits.Any(e => e.NamedPath.Contains("airborne") || e.NamedPath.Contains("vehicle_around")), "only the ground list is scaled");
        T.True(wt.Edits.All(e => e.File == VisionPresets.FieldsFile), "vision.set is never edited");
        T.Throws<ArgumentException>(() => VisionPresets.Build(stack, "bf", 4, notes), "an unknown variant is refused");

        // 만든 수정이 그 파일에 그대로 들어간다
        foreach (var p in new[] { wot, wt })
        {
            var results = new List<EditResult>();
            string patched = TokenPatcher.Apply(stack.ReadText(VisionPresets.FieldsFile), p.Edits, results);
            T.True(results.All(r => r.Status == EditStatus.Applied), p.Name + ": every edit applies");
            T.True(patched.Contains("{\"airborne_main\" {uncover 0} {h_fov 110} (\"spotting\" r(3.5))}"), p.Name + ": the air field is unchanged");
        }

        // 명령줄
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        PutFiles(Path.Combine(game, "resource"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json"), proj = Path.Combine(work, "projects", "ww.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        T.Eq(2, Run("vision-preset", "--settings", settings, "--variant", "bf", "--project", proj), "an unknown variant is a usage error");
        T.Eq(2, Run("vision-preset", "--settings", settings, "--variant", "wt", "--scale", "0", "--project", proj), "a scale must be above zero");
        T.Eq(0, Run("vision-preset", "--settings", settings, "--variant", "wot", "--project", proj), "writes the WoT project");
        T.Eq("월드워", EditProject.Load(proj).Name, "with its name");
        T.Eq(1, Run("vision-preset", "--settings", settings, "--variant", "wt", "--project", proj), "an existing project is not overwritten without --force");
        T.Eq(0, Run("vision-preset", "--settings", settings, "--variant", "wt", "--scale", "2.5", "--project", proj, "--force"), "--force overwrites");
        T.True(EditProject.Load(proj).Edits.Any(e => e.NamedPath == "/\"human\"/(\"spotting\")" && e.New == "2.5"), "--scale sets the r multiplier");
    }
}
