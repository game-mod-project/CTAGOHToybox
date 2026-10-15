using System.IO;
using System.Linq;
using CtaToybox.Core;

static class GameProfileTests
{
    public static void Run()
    {
        string options = "{options\n\t{sound {volume 0.5}}\n\t{mods\n\t\t\"mod_111:0\"\n\t\t\"CTAToybox:0\"\n\t\t\"mod_222:2097245\"\n\t\t\"local_one\"\n\t}\n}\n";
        T.Eq(new[] { "mod_111:0", "CTAToybox:0", "mod_222:2097245", "local_one" }, GameProfile.ReadModEntries(options).ToArray(), "mod entries are read in file order");
        T.Eq(0, GameProfile.ReadModEntries("{options {sound {volume 1}}}").Count, "no mods block means no mods");
        T.Eq(0, GameProfile.ReadModEntries("{options {mods}}").Count, "an empty mods block means no mods");

        string game = T.TempDir(), workshop = T.TempDir(), repoMod = T.TempDir();
        Directory.CreateDirectory(Path.Combine(game, "resource"));
        Directory.CreateDirectory(Path.Combine(workshop, "111", "resource"));
        Directory.CreateDirectory(Path.Combine(game, "mods", "local_one", "resource"));
        var settings = new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = "unused", OutputModDir = repoMod, OutputModName = "ctatoybox" };

        var plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(options));
        T.Eq(new[] { "vanilla", "mod_111", "mod_222", "local_one" }, plan.Layers.Select(l => l.Name).ToArray(), "vanilla first, then mods in order, without the output mod");
        T.Eq(Path.Combine(game, "resource"), plan.Layers[0].ResourceDir, "vanilla resource folder");
        T.Eq(Path.Combine(workshop, "111", "resource"), plan.Layers[1].ResourceDir, "workshop mod resource folder");
        T.Eq(Path.Combine(game, "mods", "local_one", "resource"), plan.Layers[3].ResourceDir, "local mod resource folder");
        T.True(plan.OutputModActive, "the output mod is recognised case-insensitively");
        T.Eq(new[] { "mod_222", "local_one" }, plan.LayersAfterOutput.ToArray(), "layers loaded after the output mod are reported");
        T.True(plan.Warnings.Any(w => w.Contains("mod_222") && w.Contains("폴더")), "a mod whose folder is missing is warned about");
        T.True(plan.Warnings.Any(w => w.Contains("뒤에 로드")), "being loaded before other mods is warned about");

        var off = GameProfile.PlanLayers(settings, new[] { "mod_111:0" });
        T.True(!off.OutputModActive, "output mod absent from the list");
        T.True(off.Warnings.Any(w => w.Contains("켜져 있지 않")), "an inactive output mod is warned about");

        // 출력 모드는 이름이 아니라 폴더로도 알아본다: 이름이 options.set의 표기와 달라도 원본 계층에 섞이지 않는다(Q6)
        Directory.CreateDirectory(Path.Combine(game, "mods", "mymod", "resource"));
        var renamed = new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = "unused", OutputModDir = Path.Combine(game, "mods", "mymod"), OutputModName = "Other" };
        var byDir = GameProfile.PlanLayers(renamed, new[] { "mod_111:0", "mymod:0" });
        T.Eq(new[] { "vanilla", "mod_111" }, byDir.Layers.Select(l => l.Name).ToArray(), "the output folder is not read as a source layer");
        T.True(byDir.OutputModActive, "and counts as the active output mod");
        T.True(byDir.Warnings.Any(w => w.Contains("mymod") && w.Contains("Other")), "the name mismatch is warned about");
        var later = GameProfile.PlanLayers(renamed, new[] { "mymod:0", "mod_111:0" });
        T.True(later.Warnings.Any(w => w.Contains("뒤에 로드") && w.Contains("mymod 줄")) && !later.Warnings.Any(w => w.Contains("Other 줄")), "the load-order advice names the entry that is in options.set");

        // 게임의 mods 아래 정션이 출력 폴더를 가리켜도 출력 모드다(실제 배치: mods\CTAToybox → 저장소 mod\CTAToybox)
        string target = T.TempDir();
        string junction = Path.Combine(game, "mods", "linked");
        var mklink = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c mklink /J \"" + junction + "\" \"" + target + "\"") { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true });
        mklink.WaitForExit();
        T.True(Directory.Exists(junction), "the test junction exists");
        var linked = GameProfile.PlanLayers(new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = "unused", OutputModDir = target, OutputModName = "Other" }, new[] { "linked:0" });
        T.True(linked.OutputModActive && linked.Layers.Count == 1, "a junction to the output folder is the output mod");

        string path = Path.Combine(T.TempDir(), "settings.json");
        settings.Save(path);
        var loaded = EditorSettings.Load(path);
        T.Eq(game, loaded.GameDir, "settings round-trip");
        T.Eq("ctatoybox", loaded.OutputModName, "settings round-trip (name)");
    }
}
