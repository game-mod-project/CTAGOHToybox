using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// E4: 인자 없는 매크로 호출(병종의 tier 등)을 같은 파일에 정의된 다른 매크로로 바꾼다
static class MacroSwapTests
{
    const string Breed = "set/breed/mp/e4/late/rifle.set";

    static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/breed/presets/perks/health.inc", "(define \"tier1_standard\" {\"health\" \"100%\"})\n(define \"tier2_standard\" {\"health\" \"200%\"})\n(define \"tier2_recon\" {\"health\" \"150%\"})"),
            ("set/breed/presets/perks/ability.inc", "(define \"stealth\" {\"stealth\" 1})"),
            ("set/breed/mp/e4/late/.presets", "(include \"/set/breed/presets/perks/health.inc\")\n(include \"/set/breed/presets/perks/ability.inc\")"),
            (Breed, "{breed\n {perks\n  (\"Tier2_Standard\")\n  (\"stealth\")\n }\n {mass 80}\n}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        T.Eq("Tier2_Standard", MacroSwap.NameOf("/breed/perks/(\"Tier2_Standard\")"), "the call name from the raw path");
        T.True(MacroSwap.NameOf("/breed/mass") == null, "not a call row");
        T.True(MacroSwap.SameFamily("tier2_standard", "tier4_standard") && !MacroSwap.SameFamily("tier2_standard", "tier2_recon"), "a family differs only in numbers");

        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        using var session = EditorSession.Open(settings, repo, null);
        string projectPath = Path.Combine(repo, "editor", "projects", "e4.json");
        session.OpenProject(projectPath);
        var view = session.OpenUnit(Breed);

        var tierRow = view.Rows.Single(r => r.IsMacroCall && r.Path.EndsWith("(\"tier2_standard\")"));
        var tier = tierRow.Values.Single();
        T.True(tier.Trace.MacroName && tier.Editable, "an argument-less call shows its name as an editable value");
        T.Eq("\"Tier2_Standard\"", tier.Trace.Text, "the value is the quoted name as written");
        T.Eq(new[] { "tier1_standard", "tier2_standard", "tier2_recon" }, tier.Choices.ToArray(), "choices are the names defined in the same file, same family first");
        var stealth = view.Rows.Single(r => r.IsMacroCall && r.Path.EndsWith("(\"stealth\")")).Values.Single();
        T.True(!stealth.Editable && stealth.Trace.Note == MacroSwap.NoChoiceNote, "a macro alone in its file has nothing to swap to");

        string warning;
        T.True(session.ValidateNew(view, tier, "\"tier9_standard\"", "shared", out warning) != null, "a name outside the file is refused");
        T.True(session.ValidateNew(view, tier, "tier1_standard", "shared", out warning) != null, "the name must be quoted");
        T.True(session.ValidateNew(view, tier, "\"tier2_recon\"", "shared", out warning) == null && warning != null && warning.Contains("갈래"), "another family is allowed with a warning");
        T.True(session.ValidateNew(view, tier, "\"tier1_standard\"", "shared", out warning) == null && warning == null, "the same family is fine");

        var edit = session.AddEdit(view, tier, "\"tier1_standard\"", "shared");
        T.Eq(MacroSwap.Kind, edit.Kind, "the edit records that it renames a call");
        T.True(File.ReadAllText(projectPath).Contains("\"kind\": \"macro-name\""), "and the project file keeps it");
        session.RefreshEdits(view);
        T.True(tier.Edit == edit, "the value shows its edit");
        T.Eq(EditStatus.Applied, session.CheckEdits().Single().Status, "the check accepts it");
        var d = session.Describe(edit);
        T.True(d != null && d.ValueLabel == "매크로 이름" && d.IsMacroCall, "the edit list names it");

        var report = session.Build();
        T.Eq(1, report.Count(EditStatus.Applied), "the build applies it");
        string written = File.ReadAllText(Path.Combine(outDir, "resource", "set", "breed", "mp", "e4", "late", "rifle.set"));
        T.True(written.Contains("(\"tier1_standard\")") && !written.Contains("Tier2_Standard") && written.Contains("(\"stealth\")"), "only the call name changes");

        // 예전 프로젝트처럼 목록 밖의 이름을 직접 넣어도 점검과 빌드가 막는다
        edit.New = "\"nope\"";
        T.Eq(EditStatus.Invalid, session.CheckEdits().Single().Status, "the check refuses a name that is not defined there");
        T.Eq(EditStatus.Invalid, session.Build().Results.Single().Status, "and so does the build");

        // 값 수정은 kind 없이 저장된다(지금까지의 프로젝트 파일과 같다)
        edit.New = "\"tier1_standard\"";
        var mass = session.AddEdit(view, view.Rows.Single(r => r.Path == "/breed/mass").Values[0], "90", "shared");
        T.True(mass.Kind == null, "a value edit has no kind");
        T.Eq(1, System.Text.RegularExpressions.Regex.Matches(File.ReadAllText(projectPath), "\"kind\"").Count, "only the macro edit writes kind");

        // 명령줄 add --kind macro-name
        string cliProject = Path.Combine(repo, "editor", "projects", "e4-cli.json");
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Breed, "--path", "/breed/perks/(\"tier2_standard\")", "--new", "\"tier1_standard\"", "--kind", "macro-name" }, w), "CLI add renames a call");
        T.Eq(MacroSwap.Kind, EditProject.Load(cliProject).Edits.Single().Kind, "with the kind");
        w = new StringWriter();
        T.Eq(1, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--file", Breed, "--path", "/breed/perks/(\"tier2_standard\")", "--new", "\"stealth\"", "--kind", "macro-name" }, w), "CLI add refuses a name from another file");
        T.True(w.ToString().Contains("같은 파일"), "with the reason");
    }
}
