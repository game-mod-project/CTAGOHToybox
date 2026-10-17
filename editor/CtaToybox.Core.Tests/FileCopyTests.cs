using System.IO;
using System.Linq;
using CtaToybox.Core;

// 팩션 강화: 파일 사본(Kind file-copy)
static class FileCopyTests
{
    public const string K98 = "set/stuff/rifle/k98", G43 = "set/stuff/rifle/g43", Pattern = "set/stuff/rifle/rifle.pattern";

    public static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "set.pak"),
            (K98, "{from \"rifle.pattern\"\n {weight 4}\n}\n"), (G43, "{from \"rifle.pattern\"\n {weight 5}\n}\n"), (Pattern, "{weapon\n {rechargeTime 6}\n {range 100}\n}\n"),
            ("set/stuff/mgun/mg34.weapon", "{weight 11}\n"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        T.Eq("set/stuff/rifle/k98_ctb_ger", FileCopy.NameFor(K98, "ger"), "the army tail goes on the file name");
        T.Eq("set/stuff/mgun/mg34_ctb_ger.weapon", FileCopy.NameFor("set/stuff/mgun/mg34.weapon", "ger"), "before the extension, on the first word");

        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using var s = EditorSession.Open(settings, repo, null);
        s.OpenProject(Path.Combine(repo, "editor", "projects", "copy.json"));
        string copy = FileCopy.NameFor(K98, "ger");
        var edit = s.AddFileCopy(null, K98, copy);
        T.Eq(FileCopy.Kind, edit.Kind, "a file-copy edit");
        T.Eq(s.Stack.ReadText(K98), s.Stack.ReadText(copy), "the copy reads as the source");
        T.Eq(new[] { K98 }, s.Items.Find("k98").Files.ToArray(), "the old name still finds the original");
        T.Eq(new[] { copy }, s.Items.Find("k98_ctb_ger").Files.ToArray(), "the copy name finds the copy");
        T.True(FileCopy.CheckTarget(s.Stack, K98, G43) != null, "a copy onto an existing file is refused");
        T.True(FileCopy.CheckTarget(s.Stack, K98, "set/stuff/gun/k98_x") != null, "a copy into another folder is refused");
        T.Throws<System.ArgumentException>(() => s.AddFileCopy(null, K98, copy), "the same copy twice is refused");

        var view = s.OpenUnit(copy);
        var weight = view.Rows.Single(r => r.Path.EndsWith("/weight")).Values[0];
        T.Eq(copy, SourceStack.ToVirtual(weight.Trace.File), "the copy's own row lives in the copy");
        s.AddEdit(view, weight, "8", "shared");
        var inherited = view.Rows.Single(r => r.Path.EndsWith("/rechargetime")).Values[0];
        T.Eq(Pattern, SourceStack.ToVirtual(inherited.Trace.File), "the inherited row still comes from the shared pattern");
        s.AddEdit(view, inherited, "3", "unit");   // 물려받은 값의 '이 유닛만'(E14): 사본 맨 위 블록에 덧붙인다
        s.AddFileCopy(null, G43, FileCopy.NameFor(G43, "ger"));
        T.True(s.CheckEdits().All(c => c.Status == EditStatus.Applied), "copies and the copy's edit check");

        var report = s.Build();
        T.Eq(0, report.Results.Count(r => r.Status != EditStatus.Applied), "everything applies: " + string.Join("; ", report.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Message)));
        string outCopy = Path.Combine(outDir, "resource", "set", "stuff", "rifle", "k98_ctb_ger");
        T.True(File.Exists(outCopy) && File.ReadAllText(outCopy).Contains("{weight 8}") && File.ReadAllText(outCopy).Contains("{from \"rifle.pattern\""), "the copy is written with its edit");
        T.True(File.ReadAllText(outCopy).Contains("{rechargeTime 3}"), "the inherited reload time is appended to the copy: " + File.ReadAllText(outCopy));
        T.True(File.Exists(Path.Combine(outDir, "resource", "set", "stuff", "rifle", "g43_ctb_ger")), "a copy without edits is written too");
        T.True(!File.Exists(Path.Combine(outDir, "resource", "set", "stuff", "rifle", "k98")) && !File.Exists(Path.Combine(outDir, "resource", "set", "stuff", "rifle", "rifle.pattern")), "the originals are not written");

        s.SetEnabled(edit, false);
        var weightCheck = s.CheckEdits().Single(c => c.Edit.File == copy);
        T.Eq(EditStatus.TargetMissing, weightCheck.Status, "with the copy off, its edits have no file");
        T.Eq(null, s.Stack.Find(copy), "and the copy is gone from the stack");
        var reopened = EditProject.Load(Path.Combine(repo, "editor", "projects", "copy.json"));
        T.Eq(2, reopened.Edits.Count(e => e.Kind == FileCopy.Kind), "the project keeps the copies");
    }
}
