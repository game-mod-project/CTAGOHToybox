using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class ModBuilderTests
{
    static Edit E(string file, string path, string old, string @new)
    {
        return new Edit { Unit = file, File = file, Path = path, Occurrence = 0, Arg = 0, Old = old, New = @new };
    }

    static string Read(string modDir, string relative) { return Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(modDir, relative.Replace('/', Path.DirectorySeparatorChar)))); }
    static bool Exists(string modDir, string relative) { return File.Exists(Path.Combine(modDir, relative.Replace('/', Path.DirectorySeparatorChar))); }

    public static void Run()
    {
        string vanilla = T.TempDir(), mod = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/a.set", "{a {x 1} {y 2}}\r\n"), ("set/b.set", "{b {x 1}}"), ("set/bad.set", "{c {x 1}"));
        T.Put(mod, "set/b.set", "{b {x 5}} ; modded");
        T.Put(outDir, "mod.info", "{mod}");
        T.Put(outDir, "resource/.gitkeep", "");
        T.Put(outDir, "resource/set/hand.set", "hand-made");

        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modX", mod) });
        stack.Scan();
        var builder = new ModBuilder(stack, outDir);

        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("set/a.set", "/a/x", "1", "10"));
        project.Edits.Add(E("set/a.set", "/a/y", "2", "20"));
        project.Edits.Add(E("set/b.set", "/b/x", "5", "50"));
        project.Edits.Add(E("set/b.set", "/b/x", "1", "99"));          // 바닐라 값 기준이라 모드 값과 다르다
        project.Edits.Add(E("set/gone.set", "/z", "1", "2"));
        project.Edits.Add(E("set/bad.set", "/c/x", "1", "2"));

        var report = builder.Build(project);
        T.Eq("{a {x 10} {y 20}}\r\n", Read(outDir, "resource/set/a.set"), "two edits land in one output file");
        T.Eq("{b {x 50}} ; modded", Read(outDir, "resource/set/b.set"), "the edit applies to the winning layer's file");
        T.Eq(new[] { "resource/set/a.set", "resource/set/b.set" }, report.WrittenFiles.OrderBy(x => x).ToArray(), "written files, relative to the mod folder");
        T.Eq(3, report.Count(EditStatus.Applied), "applied count");
        T.Eq(1, report.Count(EditStatus.SourceChanged), "an edit made against another base is skipped");
        T.Eq(1, report.Count(EditStatus.TargetMissing), "a file that no longer exists is TargetMissing");
        T.Eq(1, report.Count(EditStatus.Invalid), "a file that does not parse is Invalid");
        T.True(!Exists(outDir, "resource/set/bad.set") && !Exists(outDir, "resource/set/gone.set"), "files with no applied edit are not written");
        T.True(Exists(outDir, ModBuilder.ManifestName), "the manifest is written");
        T.True(File.ReadAllText(Path.Combine(outDir, ModBuilder.ManifestName)).Contains("\"path\": \"resource/set/a.set\""), "the manifest lists written files");
        T.Eq("hand-made", Read(outDir, "resource/set/hand.set"), "a hand-placed file is left alone");
        T.True(Exists(outDir, "mod.info") && Exists(outDir, "resource/.gitkeep"), "mod.info and .gitkeep are left alone");

        // 다시 빌드: 이전 빌드의 파일 가운데 이번에 안 쓰는 것은 지운다
        project.Edits.RemoveAll(e => e.File == "set/b.set");
        report = builder.Build(project);
        T.True(!Exists(outDir, "resource/set/b.set"), "a file from the previous build that is no longer produced is removed");
        T.Eq(new[] { "resource/set/b.set" }, report.RemovedFiles.ToArray(), "removed files are reported");
        T.True(Exists(outDir, "resource/set/a.set"), "a file still produced stays");
        T.Eq("hand-made", Read(outDir, "resource/set/hand.set"), "the hand-placed file survives a rebuild");

        // 수정을 모두 끄면 생성물이 남지 않는다
        foreach (var e in project.Edits) e.Enabled = false;
        report = builder.Build(project);
        T.True(!Exists(outDir, "resource/set/a.set"), "with every edit disabled no generated file remains");
        T.Eq(0, report.WrittenFiles.Count, "nothing written");
        T.True(Exists(outDir, "resource/.gitkeep") && Exists(outDir, "resource/set/hand.set"), "non-generated files remain");

        // 목록 파일이 출력 폴더 밖이나 손대면 안 되는 파일을 가리켜도 지우지 않는다
        string outside = Path.Combine(T.TempDir(), "victim.txt");
        File.WriteAllText(outside, "keep");
        string manifest = "{ \"project\": \"t\", \"builtAt\": \"x\", \"files\": [ { \"path\": \"../" + Path.GetFileName(Path.GetDirectoryName(outside)) + "/victim.txt\", \"layer\": \"v\", \"edits\": 1 }, " +
                          "{ \"path\": \"" + outside.Replace("\\", "\\\\") + "\", \"layer\": \"v\", \"edits\": 1 }, { \"path\": \"mod.info\", \"layer\": \"v\", \"edits\": 1 } ] }";
        File.WriteAllText(Path.Combine(outDir, ModBuilder.ManifestName), manifest);
        report = builder.Build(project);
        T.True(File.Exists(outside), "a manifest entry pointing outside the mod folder is not deleted");
        T.True(Exists(outDir, "mod.info"), "a manifest entry outside resource/ is not deleted");
        T.Eq(3, report.Warnings.Count(w => w.Contains("목록 파일")), "each refused manifest entry is warned about");

        // 깨진 목록 파일
        File.WriteAllText(Path.Combine(outDir, ModBuilder.ManifestName), "not json");
        foreach (var e in project.Edits) e.Enabled = true;
        report = builder.Build(project);
        T.True(report.Warnings.Any(w => w.Contains("목록 파일")), "an unreadable manifest is warned about, not fatal");
        T.Eq("{a {x 10} {y 20}}\r\n", Read(outDir, "resource/set/a.set"), "the build still completes");
    }
}
