using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class UnitScopeBuildTests
{
    const string A = "entity/v/a/a.def", B = "entity/v/a/b.def", S = "entity/v/a/shared.inc", D = "properties/deep.ext";

    static Edit U(string unit, string file, string path, int arg, string old, string @new)
    {
        return new Edit { Unit = unit, File = file, Path = path, Occurrence = 0, Arg = arg, Old = old, New = @new, Scope = "unit" };
    }

    static Edit Sh(string file, string path, int occ, int arg, string old, string @new)
    {
        return new Edit { Unit = file, File = file, Path = path, Occurrence = occ, Arg = arg, Old = old, New = @new };
    }

    static string Vanilla()
    {
        string dir = T.TempDir();
        T.Put(dir, A, "{game_entity\r\n\t(include \"shared.inc\")\r\n\t(include \"shared.inc\")\r\n\t{mass 10}\r\n}\r\n");
        T.Put(dir, B, "{game_entity\r\n\t(include \"shared.inc\")\r\n}\r\n");
        T.Put(dir, S, "{box {item \"mine\" 24} {item \"can\" 1}}\r\n(include \"/properties/deep.ext\")\r\n");
        T.Put(dir, D, "{armor 30}\r\n(define \"m\" {k 7})\r\n");
        T.Put(dir, "properties/other.ext", "{other 1}\r\n");
        return dir;
    }

    // 빌드 결과를 게임처럼 원본 위에 얹어 유닛을 펼친 뒤, 펼친 값 하나를 읽는다
    static string Expanded(string vanilla, string outDir, string unit, string path, int occurrence, int arg)
    {
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("out", Path.Combine(outDir, "resource")) });
        stack.Scan();
        var t = new UnitTracer(stack).Trace(unit);
        return t.Rows.First(r => r.Path == path && r.Occurrence == occurrence).Values[arg].Text;
    }

    static List<string> Outputs(string outDir)
    {
        string root = Path.Combine(outDir, "resource");
        if (!Directory.Exists(root)) return new List<string>();
        return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(root, f).Replace('\\', '/')).OrderBy(x => x, System.StringComparer.Ordinal).ToList();
    }

    static BuildReport Build(string vanilla, string outDir, params Edit[] edits)
    {
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var project = new EditProject { Name = "t" };
        project.Edits.AddRange(edits);
        return new ModBuilder(stack, outDir).Build(project);
    }

    public static void Run()
    {
        string sCopy = IncludeChain.CopyName(S, A), dCopy = IncludeChain.CopyName(D, A);

        // 1. include로 공유된 값을 이 유닛만 고친다
        string vanilla = Vanilla(), outDir = T.TempDir();
        var report = Build(vanilla, outDir, U(A, S, "/box/item", 1, "24", "5"));
        T.Eq(1, report.Count(EditStatus.Applied), "the unit-scoped edit is applied");
        T.Eq(new[] { A, sCopy }.OrderBy(x => x, System.StringComparer.Ordinal).ToArray(), Outputs(outDir).ToArray(), "outputs: the unit file and one copy; the shared file and the other unit are not written");
        T.Eq("5", Expanded(vanilla, outDir, A, "/game_entity/box/item", 0, 1), "the unit sees the new value");
        T.Eq("5", Expanded(vanilla, outDir, A, "/game_entity/box/item", 2, 1), "through every include of that file (it is included twice)");
        T.Eq("24", Expanded(vanilla, outDir, B, "/game_entity/box/item", 0, 1), "the other unit keeps the original value");
        string aText = Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", A.Replace('/', Path.DirectorySeparatorChar))));
        string leaf = sCopy.Substring(sCopy.LastIndexOf('/') + 1);
        T.Eq("{game_entity\r\n\t(include \"" + leaf + "\")\r\n\t(include \"" + leaf + "\")\r\n\t{mass 10}\r\n}\r\n", aText, "only the include path tokens changed in the unit file");

        // 2. 수정을 끄고 다시 빌드하면 사본과 유닛 파일이 모두 지워진다
        var off = U(A, S, "/box/item", 1, "24", "5"); off.Enabled = false;
        report = Build(vanilla, outDir, off);
        T.Eq(0, Outputs(outDir).Count, "disabling the edit removes the copy and the rewritten unit file");
        T.Eq(2, report.RemovedFiles.Count, "both are reported as removed");

        // 3. 두 단계 사슬: 유닛 → shared.inc → deep.ext
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir, U(A, D, "/armor", 0, "30", "40"));
        T.Eq(new[] { A, sCopy, dCopy }.OrderBy(x => x, System.StringComparer.Ordinal).ToArray(), Outputs(outDir).ToArray(), "the whole chain is copied");
        T.Eq("40", Expanded(vanilla, outDir, A, "/game_entity/armor", 0, 0), "the unit sees the new value two includes deep");
        T.Eq("30", Expanded(vanilla, outDir, B, "/game_entity/armor", 0, 0), "the other unit does not");
        string sText = Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", sCopy.Replace('/', Path.DirectorySeparatorChar))));
        T.True(sText.Contains("(include \"/properties/" + dCopy.Substring(dCopy.LastIndexOf('/') + 1) + "\")"), "the intermediate copy includes the deep copy, keeping the root-relative form");

        // 4. 같은 파일에 '전부' 수정과 '이 유닛만' 수정이 함께 걸린다
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir,
            Sh(S, "/box/item", 1, 1, "1", "2"),            // can 1 → 2, 전부
            Sh(S, "/box/item", 0, 1, "24", "30"),          // mine 24 → 30, 전부
            U(A, S, "/box/item", 1, "24", "5"));           // mine 24 → 5, 이 유닛만
        T.Eq(3, report.Count(EditStatus.Applied), "each edit is reported once, as applied");
        T.Eq("5", Expanded(vanilla, outDir, A, "/game_entity/box/item", 0, 1), "on the same slot the unit-scoped edit wins for that unit");
        T.Eq("2", Expanded(vanilla, outDir, A, "/game_entity/box/item", 1, 1), "the unit's copy also carries the shared edit");
        T.Eq("30", Expanded(vanilla, outDir, B, "/game_entity/box/item", 0, 1), "other units get the shared edit");
        T.Eq("2", Expanded(vanilla, outDir, B, "/game_entity/box/item", 1, 1), "both of them");

        // 5. 유닛 자신의 파일에 있는 값은 그 파일을 그대로 고친다
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir, U(A, A, "/game_entity/mass", 0, "10", "12"));
        T.Eq(new[] { A }, Outputs(outDir).ToArray(), "no copy is needed for the unit's own file");
        T.Eq("12", Expanded(vanilla, outDir, A, "/game_entity/mass", 0, 0), "the value changed");

        // 6. 안 되는 경우는 아무것도 쓰지 않고 이유를 보고한다
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir,
            U(A, D, "/(define \"m\")/k", 0, "7", "8"),             // 매크로 본문
            U(B, "properties/other.ext", "/other", 0, "1", "2"),   // include 사슬 밖
            U(A, S, "/box/item", 1, "99", "5"),                    // 원본이 바뀜
            U("entity/v/a/gone.def", S, "/box/item", 1, "24", "5"),// 유닛이 없음
            U(A, "properties/gone.ext", "/x", 0, "1", "2"));       // 파일이 없음
        T.Eq(0, Outputs(outDir).Count, "nothing is written when no unit-scoped edit applies");
        T.Eq(2, report.Count(EditStatus.Invalid), "macro body and out-of-chain edits are Invalid");
        T.Eq(1, report.Count(EditStatus.SourceChanged), "a stale old value is SourceChanged");
        T.Eq(2, report.Count(EditStatus.TargetMissing), "a missing unit or file is TargetMissing");
        T.True(report.Results.Where(r => r.Status == EditStatus.Invalid).All(r => !string.IsNullOrEmpty(r.Message)), "Invalid results carry the reason");

        // 7. 사본을 만들 수 없으면 유닛 파일의 include도 고쳐 쓰지 않는다
        vanilla = Vanilla(); outDir = T.TempDir();
        T.Put(outDir, "resource/" + sCopy, "hand-made");          // 이 도구가 만들지 않은 같은 이름의 파일
        report = Build(vanilla, outDir, U(A, S, "/box/item", 1, "24", "5"));
        T.Eq(new[] { sCopy }, Outputs(outDir).ToArray(), "the unit file is not rewritten when its copy could not be made");
        T.Eq("hand-made", File.ReadAllText(Path.Combine(outDir, "resource", sCopy.Replace('/', Path.DirectorySeparatorChar))), "the hand-made file is untouched");
        T.Eq(0, report.Count(EditStatus.Applied), "the edit is not reported as applied");
        T.Eq(1, report.FileErrors.Count, "the failure is reported");

        // 8. 유닛 파일을 쓸 수 없으면 사본도 남기지 않는다
        vanilla = Vanilla(); outDir = T.TempDir();
        T.Put(outDir, "resource/" + A, "hand-made");
        report = Build(vanilla, outDir, U(A, S, "/box/item", 1, "24", "5"));
        T.Eq(new[] { A }, Outputs(outDir).ToArray(), "no orphan copy is left when the unit file could not be written");
        T.Eq(0, report.Count(EditStatus.Applied), "the edit is not reported as applied");
    }
}
