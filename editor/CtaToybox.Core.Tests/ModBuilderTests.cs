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

        // 깨진 목록 파일: 이전 생성물의 기록을 잃지 않도록 빌드를 멈추고 아무것도 쓰지 않는다(docs/10-backlog.md Q3, 2026-10-15)
        File.Delete(Path.Combine(outDir, ModBuilder.ManifestName));
        foreach (var e in project.Edits) e.Enabled = true;
        builder.Build(project);
        string before = Read(outDir, "resource/set/a.set");
        File.WriteAllText(Path.Combine(outDir, ModBuilder.ManifestName), "not json");
        project.Edits.Single(e => e.Path == "/a/x").New = "11";
        report = builder.Build(project);
        T.True(report.FileErrors.Any(f => f.Contains("목록 파일")), "an unreadable manifest stops the build with an error");
        T.Eq(0, report.WrittenFiles.Count + report.RemovedFiles.Count, "nothing is written or removed");
        T.Eq(before, Read(outDir, "resource/set/a.set"), "the earlier output stays as it was");
        T.Eq("not json", File.ReadAllText(Path.Combine(outDir, ModBuilder.ManifestName)), "the broken manifest is left for the user to look at");
        File.Delete(Path.Combine(outDir, ModBuilder.ManifestName));
        foreach (string f in Directory.GetFiles(Path.Combine(outDir, "resource", "set")).Where(f => !f.EndsWith("hand.set"))) File.Delete(f);
        project.Edits.Single(e => e.Path == "/a/x").New = "10";
        report = builder.Build(project);
        T.Eq("{a {x 10} {y 20}}\r\n", Read(outDir, "resource/set/a.set"), "after the user removes the manifest and stale files the build completes");

        Sub(HandPlacedCollision);
        Sub(UndecodableAndUnencodable);
        Sub(ReportAfterFileFailure);
        Sub(CopyFailureKeepsManifest);
        Sub(PathEscape);
    }

    // 한 경우가 예외로 끝나도 나머지 경우를 돌린다
    static void Sub(System.Action run)
    {
        try { run(); }
        catch (System.Exception ex) { T.Fail++; System.Console.WriteLine("FAIL " + run.Method.Name + " threw: " + ex.GetType().Name + ": " + ex.Message); }
    }

    // 손으로 넣은 파일과 경로가 같으면 덮어쓰지 않는다
    static void HandPlacedCollision()
    {
        string vanilla = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/a.set", "{a {x 1}}"));
        T.Put(outDir, "resource/set/a.set", "mine");
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("set/a.set", "/a/x", "1", "2"));
        var report = new ModBuilder(stack, outDir).Build(project);
        T.Eq("mine", Read(outDir, "resource/set/a.set"), "a hand-placed file at the same path is not overwritten");
        T.Eq(1, report.FileErrors.Count, "the collision is a file error");
        T.Eq(0, report.Count(EditStatus.Applied), "the edit is not counted as applied");
        T.Eq(1, report.Count(EditStatus.Invalid), "the edit is reported as Invalid");
        project.Edits[0].Enabled = false;
        new ModBuilder(stack, outDir).Build(project);
        T.Eq("mine", Read(outDir, "resource/set/a.set"), "the hand-placed file survives the next build too");
    }

    // 되쓸 수 없는 원본, 그 파일의 인코딩으로 쓸 수 없는 새 값
    static void UndecodableAndUnencodable()
    {
        string vanilla = T.TempDir(), outDir = T.TempDir();
        // UTF-8 BOM 뒤에 UTF-8이 아닌 바이트(0xE9)가 있다. T.Zip은 Latin-1로 쓴다
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/d.set", "ï»¿{d {x 1}} ; café"), ("set/s.set", "{s {name \"n\"} {x 1}}"));
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("set/d.set", "/d/x", "1", "2"));
        project.Edits.Add(E("set/s.set", "/s/name", "\"n\"", "\"탱크\""));
        project.Edits.Add(E("set/s.set", "/s/x", "1", "2"));
        var report = new ModBuilder(stack, outDir).Build(project);
        T.True(!Exists(outDir, "resource/set/d.set"), "a file that does not re-encode to its own bytes is not written");
        T.True(report.FileErrors.Any(x => x.Contains("set/d.set")), "and is reported as a file error");
        T.Eq("{s {name \"n\"} {x 2}}", Read(outDir, "resource/set/s.set"), "a value the file's encoding cannot hold is not written; other edits are");
        T.Eq(2, report.Count(EditStatus.Invalid), "both refused edits are Invalid");
        T.Eq(1, report.Count(EditStatus.Applied), "one applied");
    }

    // 파일 하나가 실패해도 그 파일의 수정마다 결과가 하나씩이고 적용으로 세지 않는다
    static void ReportAfterFileFailure()
    {
        string vanilla = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/e.set", "{e {Extender \"inventory\" {x 1}} {y 1}}"));
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var project = new EditProject { Name = "t" };
        project.Edits.Add(new Edit { Unit = "set/e.set", File = "set/e.set", Path = "/e/extender[inventory]", Old = "\"inventory\"", New = "\"other\"" });   // 고치면 경로가 달라져 검증에 걸린다
        project.Edits.Add(E("set/e.set", "/e/y", "9", "2"));
        project.Edits.Add(E("set/e.set", "/e/y", "1", "2"));
        var report = new ModBuilder(stack, outDir).Build(project);
        T.Eq(3, report.Results.Count, "one result per edit");
        T.Eq(0, report.Count(EditStatus.Applied), "nothing is counted as applied when the file was not written");
        T.Eq(1, report.Count(EditStatus.SourceChanged), "the other statuses of that file are kept");
        T.Eq(2, report.Count(EditStatus.Invalid), "edits that would have applied become Invalid");
        T.Eq(1, report.FileErrors.Count, "one file error");
        T.True(!Exists(outDir, "resource/set/e.set"), "the file is not written");
    }

    // 옮기는 도중에 실패해도 예외가 새지 않고, 쓴 파일은 목록에 남아 다음 빌드가 지울 수 있다
    static void CopyFailureKeepsManifest()
    {
        string vanilla = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/a.set", "{a {x 1}}"), ("set/z.set", "{z {x 1}}"));
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var builder = new ModBuilder(stack, outDir);
        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("set/a.set", "/a/x", "1", "2"));
        builder.Build(project);
        project.Edits[0].New = "3";
        project.Edits.Add(E("set/z.set", "/z/x", "1", "2"));
        BuildReport report;
        using (new FileStream(Path.Combine(outDir, "resource", "set", "a.set"), FileMode.Open, FileAccess.Read, FileShare.None))
            report = builder.Build(project);
        T.True(report.FileErrors.Any(x => x.Contains("set/a.set")), "a locked target is a file error, not an exception");
        T.Eq("{z {x 2}}", Read(outDir, "resource/set/z.set"), "the other file is still written");
        string manifest = File.ReadAllText(Path.Combine(outDir, ModBuilder.ManifestName));
        T.True(manifest.Contains("resource/set/a.set") && manifest.Contains("resource/set/z.set"), "the manifest lists every generated file, including the one that failed");
        foreach (var e in project.Edits) e.Enabled = false;
        builder.Build(project);
        T.True(!Exists(outDir, "resource/set/a.set") && !Exists(outDir, "resource/set/z.set"), "the next build can clean up everything");
    }

    // pak 항목 이름에 ..이 있어도 출력 폴더 밖에 쓰지 않는다
    static void PathEscape()
    {
        string vanilla = T.TempDir(), parent = T.TempDir(), outDir = Path.Combine(parent, "mod");
        Directory.CreateDirectory(outDir);
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/../../../evil.set", "{a {x 1}}"));
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("set/../../../evil.set", "/a/x", "1", "2"));
        var report = new ModBuilder(stack, outDir).Build(project);
        T.True(!File.Exists(Path.Combine(parent, "evil.set")), "nothing is written outside the mod folder");
        T.Eq(0, report.Count(EditStatus.Applied), "the escaping edit is not applied");
    }
}
