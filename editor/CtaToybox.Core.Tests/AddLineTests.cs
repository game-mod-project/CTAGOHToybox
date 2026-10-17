using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

// 팩션 강화: 줄 더하기(Kind add-line)
static class AddLineTests
{
    public const string Breed = "set/breed/mp/ger/mid/a.set";
    const string BreedText = "{perks (include \"ability.inc\") (\"tier3_standard\")}\n{inventory {item \"k98\" filled}}\n{cost 5}\n";

    public static void Run()
    {
        var blocks = AddLine.Blocks("{a {b 1} {b 2}}\n{c (x {d 1})}");
        T.Eq(new[] { "/a", "/a/b", "/a/b", "/c", "/c/d" }, blocks.Select(b => b.Path).ToArray(), "blocks by key path; a macro call adds no path part");
        T.Eq(1, blocks[2].Occurrence, "second /a/b");
        T.True(AddLine.CheckLine("{\"health\"") != null && AddLine.CheckLine("{a} {b}") != null && AddLine.CheckLine("") != null, "a broken or doubled line is refused");
        T.Eq(null, AddLine.CheckLine("{\"health\" \"600%\"}"), "a line in one brace pair is fine");

        var add = AddLine.New(Breed, Breed, "/perks", 0, "{\"health\" \"600%\"}");
        var results = new List<EditResult>();
        string text = AddLine.Apply(BreedText, new[] { add }, results);
        T.Eq(EditStatus.Applied, results.Single().Status, "applied");
        T.True(text.StartsWith("{perks (include \"ability.inc\") (\"tier3_standard\")\n\t{\"health\" \"600%\"}\n}"), "the line goes before the closing brace: " + text);
        T.Eq(0, SdlCore.Parse(text).Errors.Count, "and the file parses");
        var again = new List<EditResult>();
        T.Eq(text, AddLine.Apply(text, new[] { add }, again), "the same line is not added twice");
        var missing = new List<EditResult>();
        AddLine.Apply(BreedText, new[] { AddLine.New(Breed, Breed, "/nope", 0, "{x 1}") }, missing);
        T.Eq(EditStatus.TargetMissing, missing.Single().Status, "a missing block is TargetMissing");

        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), repo = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "set.pak"), (Breed, BreedText));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        using var s = EditorSession.Open(settings, repo, null);
        s.OpenProject(Path.Combine(repo, "editor", "projects", "lines.json"));
        var line = s.AddLineEdit(null, Breed, "/perks", 0, "{\"health\" \"600%\"}");
        T.True(s.AddLineEdit(null, Breed, "/perks", 0, "{\"health\" \"600%\"}") == line, "the same line returns the same edit");
        T.Throws<System.ArgumentException>(() => s.AddLineEdit(null, Breed, "/nope", 0, "{x 1}"), "a missing block is refused when adding");
        var unit = s.OpenUnit(Breed);
        var itemValue = unit.Rows.Single(r => r.Path == "/inventory/item").Values[0];
        T.Eq("/inventory", s.BlockOfValue(itemValue).Path, "a line next to an item goes into inventory, not inside the item row");
        T.Eq("/perks", s.BlockInFile(Breed, "/perks").Path, "a block found by path in the unit file");
        s.AddEdit(unit, unit.Rows.Single(r => r.Path == "/cost").Values[0], "6", "shared");
        s.Project.Edits.Add(AddLine.New(Breed, Breed, "/gone", 0, "{y 1}"));
        var checks = s.CheckEdits();
        T.Eq(EditStatus.TargetMissing, checks.Single(c => c.Edit.Path == "/gone").Status, "the check finds the missing block");
        T.Eq(2, checks.Count(c => c.Status == EditStatus.Applied), "the other two check");
        var report = s.Build();
        T.Eq(2, report.Count(EditStatus.Applied), "the line and the value apply, the missing one does not");
        string built = File.ReadAllText(Path.Combine(outDir, "resource", "set", "breed", "mp", "ger", "mid", "a.set"));
        T.True(built.Contains("\t{\"health\" \"600%\"}") && built.Contains("{cost 6}"), "the output has both: " + built);
        var reopened = EditProject.Load(Path.Combine(repo, "editor", "projects", "lines.json"));
        T.Eq("{\"health\" \"600%\"}", reopened.Edits.Single(e => e.Kind == AddLine.Kind).New, "the project keeps the line");
    }
}

// 합치기: 같은 원본 블록에서 이름이 다른 복제, 같은 블록에 다른 줄은 각각 더한다(E18 월드워-차종의 필드 복제 셋)
static class MergeKindsTests
{
    public static void Run()
    {
        var other = new EditProject();
        foreach (string n in new[] { "\"a\"", "\"b\"", "\"c\"" })
            other.Edits.Add(new Edit { Kind = BlockCopy.Kind, File = "set/vision/vision_fields.inc", Path = "/\"*\"", Occurrence = 32, Arg = 0, Old = "\"x\"", New = n, Scope = "shared" });
        other.Edits.Add(AddLine.New(null, "f.set", "/perks", 0, "{a 1}"));
        other.Edits.Add(AddLine.New(null, "f.set", "/perks", 0, "{b 1}"));
        var project = new EditProject();
        var report = project.MergeFrom(other);
        T.Eq(5, report.Added, "every copy and line is added");
        T.Eq(5, project.MergeFrom(other).Same, "and merging again finds them all the same");
    }
}
