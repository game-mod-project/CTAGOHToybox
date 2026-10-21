using System.Collections.Generic;
using System.Linq;
using CtaToybox.Core;

// 팩션 강화: 클립식 포(탄창 2발 이상, 발 사이 1초 이상)는 발 사이 간격도 재장전 배수로 줄인다(2026-10-21 설계)
static class FactionClipTests
{
    static (string, string) Vehicle(string weapon) { return ("entity/-vehicle/germany/tank_heavy/" + weapon + "/" + weapon + ".def", "{game_entity\n {weaponry {place {weapon \"" + weapon + "\"}}}\n}\n"); }

    public static void Run()
    {
        var set = new[]
        {
            ("set/stuff/gun/clip_own", "{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 6}\n {recoveryTime 1.5}\n {recoveryTime 5.00}\n {rechargeTime 35}\n}\n"),
            ("set/stuff/gun/clip_rpm", "{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 3}\n (\"fire rate\" rpm(3))\n (\"reload time\" seconds(18))\n}\n"),
            ("set/stuff/gun/.presets", "(define \"fire rate\" {recoveryTime (/ 60 %rpm)})\n(define \"reload time\" {rechargeTime %seconds})\n"),
            ("set/stuff/gun/clip_pattern", "{from \"clip.pattern\"\n {filling \"bulletger_88_l56\" 4}\n {rechargeTime 25}\n}\n"),
            ("set/stuff/gun/clip.pattern", "{weapon\n {recoveryTime 2.5}\n {rechargeTime 5}\n}\n"),
            ("set/stuff/gun/autocannon", "{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 20}\n {recoveryTime 0.25}\n {rechargeTime 8}\n}\n"),
            ("set/stuff/rocket/salvo.weapon", "{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 6}\n {recoveryTime 2}\n {rechargeTime 60}\n}\n"),
        };
        var vehicles = new[] { Vehicle("clip_own"), Vehicle("clip_rpm"), Vehicle("clip_pattern"), Vehicle("autocannon"), Vehicle("salvo") };
        string repo, outDir;
        string settings = FactionPresetTests.FixtureWith(out repo, out outDir, set, vehicles, new (string, string)[0]);
        using var s = EditorSession.Open(settings, repo, null);
        var c = FactionPresetTests.Context(s);
        var project = new EditProject { Name = "ger" };
        var report = new FactionReport();
        FactionPreset.AddWeaponsAndHealth(project, c, new FactionOptions { Army = "ger" }, report);
        var build = new ModBuilder(s.Stack, outDir).Build(project);
        T.Eq(0, build.Results.Count(r => r.Status != EditStatus.Applied), "everything applies: " + string.Join("; ", build.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.File + " " + r.Message)));
        string own = FactionPresetTests.Out(outDir, "set/stuff/gun/clip_own_ctb_ger") ?? "";
        T.True(own.Contains("{recoveryTime 0.75}") && own.Contains("{recoveryTime 2.5}") && own.Contains("{rechargeTime 17.5}"), "both gap lines and the clip reload are halved with decimals: " + own);
        string rpm = FactionPresetTests.Out(outDir, "set/stuff/gun/clip_rpm_ctb_ger") ?? "";
        T.True(rpm.Contains("rpm(6)") && rpm.Contains("seconds(9)"), "the fire-rate rpm doubles (gap ×0.5): " + rpm);
        // 원형에서 오는 발 사이는 사본에 줄을 더한다
        string pat = FactionPresetTests.Out(outDir, "set/stuff/gun/clip_pattern_ctb_ger") ?? "";
        T.True(pat.Contains("{recoveryTime 1.25}") && pat.Contains("{rechargeTime 12.5}"), "a gap from the pattern becomes a line in the copy: " + pat);
        string auto = FactionPresetTests.Out(outDir, "set/stuff/gun/autocannon_ctb_ger") ?? "";
        T.True(!auto.Contains("0.125") && auto.Contains("{recoveryTime 0.25}") && auto.Contains("{rechargeTime 4}"), "a gap under one second is a fire rate and stays; its reload keeps integer rounding: " + auto);
        string salvo = FactionPresetTests.Out(outDir, "set/stuff/rocket/salvo_ctb_ger.weapon") ?? "";
        T.True(salvo.Contains("{recoveryTime 2}") && salvo.Contains("{rechargeTime 30}"), "outside set/stuff/gun the gap stays: " + salvo);
        T.Eq(new[] { "set/stuff/gun/clip_own_ctb_ger", "set/stuff/gun/clip_pattern_ctb_ger", "set/stuff/gun/clip_rpm_ctb_ger" }, report.Clips.Select(x => x.Weapon).OrderBy(x => x).ToArray(), "the report lists the clip guns");
        var clipOwn = report.Clips.Single(x => x.Weapon.EndsWith("clip_own_ctb_ger"));
        T.True(clipOwn.Filling == 6 && clipOwn.GapOld == "5.00" && clipOwn.GapNew == "2.5" && clipOwn.ReloadOld == "35" && clipOwn.ReloadNew == "17.5", "with old and new values: " + clipOwn.Filling + " " + clipOwn.GapOld + "→" + clipOwn.GapNew + " " + clipOwn.ReloadOld + "→" + clipOwn.ReloadNew);
        var clipRpm = report.Clips.Single(x => x.Weapon.EndsWith("clip_rpm_ctb_ger"));
        T.True(clipRpm.Filling == 3 && clipRpm.GapOld == "20" && clipRpm.GapNew == "10" && clipRpm.ReloadOld == "18" && clipRpm.ReloadNew == "9", "fire-rate values in seconds: " + clipRpm.GapOld + "→" + clipRpm.GapNew);
        T.Eq(4, report.Edits["ger 클립식 발 사이 ×0.5"], "gap edits: two lines, one rpm, one added line");

        int before = project.Edits.Count, clips = report.Clips.Count;
        FactionPreset.AddWeaponsAndHealth(project, c, new FactionOptions { Army = "ger" }, report);
        T.Eq(before, project.Edits.Count, "running it again adds nothing");
        T.Eq(clips, report.Clips.Count, "and lists each clip gun once");

        var p2 = new EditProject { Name = "g2" };
        FactionPreset.AddWeaponsAndHealth(p2, c, new FactionOptions { Army = "ger", ClipMinGap = 30 }, new FactionReport());   // 시험 무기의 발 사이는 최대 20초(clip_rpm)
        var gapEdits = p2.Edits.Where(e => (e.Path ?? "").EndsWith("/recoverytime") || (e.Path ?? "").EndsWith("(\"fire rate\")") || (e.New ?? "").StartsWith("{recoveryTime")).ToList();
        T.True(gapEdits.Count == 0, "--clip-min-gap raises the bar: " + string.Join("; ", gapEdits.Select(e => e.File + " " + e.Path + " " + e.Old + "→" + e.New)));
        var p3 = new EditProject { Name = "g3" };
        FactionPreset.AddWeaponsAndHealth(p3, c, new FactionOptions { Army = "ger", ClipGap = 0.8 }, new FactionReport());
        T.True(p3.Edits.Any(e => e.File == "set/stuff/gun/clip_own_ctb_ger" && e.New == "4") && p3.Edits.Any(e => e.File == "set/stuff/gun/clip_own_ctb_ger" && e.New == "17.5"), "--clip-gap sets the gap factor apart from the reload");
    }
}
