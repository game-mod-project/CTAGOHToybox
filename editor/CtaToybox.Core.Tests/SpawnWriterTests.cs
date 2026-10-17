using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;
using CtaToybox.Core.Spawn;

// 스폰 모드: 계층(Spawn Menu 제외)과 출력 쓰기(Task 1)
static class SpawnWriterTests
{
    static SpawnFile F(string path, string text) { return new SpawnFile { Path = path, Bytes = Encoding.UTF8.GetBytes(text) }; }

    public static void Run()
    {
        string dir = T.TempDir(), stat = T.TempDir();
        T.Put(dir, "resource/user.txt", "mine");
        File.WriteAllText(Path.Combine(stat, ModBuilder.ManifestName), "{\"files\":[{\"path\":\"map/x.mi\"}]}");
        var r = SpawnWriter.Write(dir, new[] { F("resource/map/a.mi", "A"), F("resource/map/x.mi", "X") }, stat);
        T.True(File.Exists(Path.Combine(dir, "resource", "map", "a.mi")), "written");
        T.True(r.Errors.Any(e => e.Contains("map/x.mi")) && !File.Exists(Path.Combine(dir, "resource", "map", "x.mi")), "stat overlap refused");
        T.True(File.Exists(Path.Combine(dir, SpawnWriter.ManifestName)), "own list written");
        r = SpawnWriter.Write(dir, new[] { F("resource/map/a.mi", "A") }, stat);
        T.Eq(1, r.Unchanged.Count, "same bytes are not written again");
        r = SpawnWriter.Write(dir, new[] { F("resource/map/b.mi", "B"), F("resource/user.txt", "over") }, stat);
        T.True(!File.Exists(Path.Combine(dir, "resource", "map", "a.mi")) && r.Removed.Contains("resource/map/a.mi"), "stale own file removed");
        T.Eq("mine", File.ReadAllText(Path.Combine(dir, "resource", "user.txt")), "writer_keeps_foreign_files");
        T.True(r.Errors.Any(e => e.Contains("user.txt")), "a foreign file is reported, not overwritten");

        // 계층: Spawn Menu of MACE와 스폰 모드 자신은 빠지고 스탯 출력 모드는 들어간다
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "map.pak"), ("map/a.mi", "{mission}"));
        T.Put(Path.Combine(workshop, "3617250510"), "resource/map/a.mi", "{mission smm}");
        T.Put(Path.Combine(workshop, "2905667604"), "resource/set/x.set", "{x}");
        T.Put(outDir, "resource/set/y.set", "{y}");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_2905667604:0\" \"mod_3617250510:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        var settings = new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) };
        var plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(File.ReadAllText(options)));
        var warnings = new System.Collections.Generic.List<string>();
        using var stack = SpawnSources.Open(settings, plan, warnings);
        T.Eq(new[] { "vanilla", "mod_2905667604", "output" }, stack.Layers.Select(l => l.Name).ToArray(), "spawn layers");
        T.Eq("{mission}", stack.ReadText("map/a.mi"), "the Spawn Menu copy does not win");
        T.True(warnings.Any(w => w.Contains("mod_3617250510")), "the dropped layer is reported");
        T.Eq(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(outDir)), "CTAToyboxSpawn"), SpawnSources.SpawnModDirOf(settings), "default spawn folder next to the output mod");
    }
}
