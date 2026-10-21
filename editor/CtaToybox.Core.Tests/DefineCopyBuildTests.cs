using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// 매크로 정의 복제를 점검·빌드·세션·명령줄에 잇는다. 인자 있는 매크로 호출도 복제본 이름으로 바꾼다
static class DefineCopyBuildTests
{
    const string F = "properties/resupply.inc";
    const string Truck = "entity/-vehicle/germany/car/truck/truck.def", Cart = "entity/-vehicle/germany/trailer/cart/cart.def";
    const string Original = "(define \"truck_ammo\"\n {extender \"supply_zone\" {radius 20} {limit 3000}}\n)\n(define \"crate\"\n {extender \"supply_zone\" {radius 10} {limit %supply}}\n)\n";
    // 매크로 정의와 '이 유닛만'으로 고칠 수 있는 값이 같은 파일에 있다(검토 2026-10-21 P5)
    const string Mixed = "properties/mixed.inc", Truck2 = "entity/-vehicle/germany/car/truck2/truck2.def";
    const string MixedText = "(define \"m\"\n {extender \"supply_zone\" {radius 5}}\n)\n{v 1}\n";

    static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "properties.pak"), ("properties/resupply.inc", Original), (Mixed, MixedText));
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            (Truck.Substring("entity/".Length), "{game_entity\n (include \"/properties/resupply.inc\")\n (\"truck_ammo\")\n}\n"),
            (Cart.Substring("entity/".Length), "{game_entity\n (include \"/properties/resupply.inc\")\n (\"crate\" supply(500))\n}\n"),
            (Truck2.Substring("entity/".Length), "{game_entity\n (include \"/properties/mixed.inc\")\n (\"m\")\n}\n"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    static Edit Value(string file, string path, string old, string next) { return new Edit { Unit = file, File = file, Path = path, Occurrence = 0, Arg = 0, Old = old, New = next, Scope = "shared" }; }
    static Edit Swap(string unit, string name, string next) { return new Edit { Kind = MacroSwap.Kind, Unit = unit, File = unit, Path = "/game_entity/(\"" + name + "\")", Occurrence = 0, Arg = 0, Old = "\"" + name + "\"", New = "\"" + next + "\"", Scope = "shared" }; }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        string projectPath = Path.Combine(repo, "editor", "projects", "supply.json");
        var project = new EditProject { Name = "supply" };
        project.Edits.Add(DefineCopy.New(F, F, "truck_ammo", "truck_ammo_x"));
        project.Edits.Add(Value(F, "/(define \"truck_ammo_x\")/extender[supply_zone]/radius", "20", "60"));
        project.Edits.Add(Swap(Truck, "truck_ammo", "truck_ammo_x"));
        project.Edits.Add(DefineCopy.New(F, F, "crate", "crate_x"));
        project.Edits.Add(Value(F, "/(define \"crate_x\")/extender[supply_zone]/limit", "%supply", "0"));
        project.Edits.Add(Swap(Cart, "crate", "crate_x"));
        project.Save(projectPath);

        using var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(projectPath);
        var copyEdit = session.Project.Edits.First(e => e.Kind == DefineCopy.Kind);

        // 세션에서 점검과 빌드
        var bad = session.CheckEdits().Where(c => c.Status != EditStatus.Applied).ToList();
        T.Eq(0, bad.Count, "every edit checks: " + string.Join("; ", bad.Select(c => c.Edit.Path + " " + c.Message)));
        var report = session.Build();
        T.Eq(6, report.Count(EditStatus.Applied), "the build applies all six: " + string.Join("; ", report.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.Path + " " + r.Message)));
        string inc = FactionPresetTests.Out(outDir, F) ?? "";
        T.True(inc.StartsWith(Original), "the original defines stay as they were: " + inc);
        T.True(inc.Contains("(define \"truck_ammo_x\"\n {extender \"supply_zone\" {radius 60} {limit 3000}}\n)") && inc.Contains("(define \"crate_x\"\n {extender \"supply_zone\" {radius 10} {limit 0}}\n)"), "the copies with their values: " + inc);
        T.True((FactionPresetTests.Out(outDir, Truck) ?? "").Contains("(\"truck_ammo_x\")"), "the truck calls the copy");
        T.True((FactionPresetTests.Out(outDir, Cart) ?? "").Contains("(\"crate_x\" supply(500))"), "a call with arguments keeps its arguments");
        var d = session.Describe(copyEdit);
        T.True(d != null && d.ValueLabel == "매크로 복제" && d.RowText == "\"truck_ammo\"", "the edit list names a define copy");

        // 복제가 꺼지면 그것에 기대는 수정을 막는다(모드 갱신으로 원본 정의가 사라진 경우와 같다)
        copyEdit.Enabled = false;
        var checks = session.CheckEdits();
        var dependents = checks.Where(c => c.Edit.File == Truck || (c.Edit.Path ?? "").Contains("truck_ammo_x")).ToList();
        T.True(dependents.Count == 2 && dependents.All(c => c.Status == EditStatus.Invalid && (c.Message ?? "").Contains("truck_ammo_x")), "the check blocks what needs the copy: " + string.Join("; ", dependents.Select(c => c.Status + " " + c.Message)));
        var offBuild = session.Build();
        T.True(offBuild.Results.Where(r => r.Edit.File == Truck || (r.Edit.Path ?? "").Contains("truck_ammo_x")).All(r => r.Status == EditStatus.Invalid), "and so does the build");
        T.True(!(FactionPresetTests.Out(outDir, Truck) ?? "").Contains("truck_ammo_x"), "the truck is not left calling a missing define");
        copyEdit.Enabled = true;

        // 덧씌우기가 없는 새 스택(명령줄 build와 같은 조건)
        using (var plain = new SourceStack(session.Plan.Layers))
        {
            plain.Scan();
            T.True(plain.Overlay == null, "a fresh stack has no overlay");
            var plainReport = new ModBuilder(plain, T.TempDir()).Build(session.Project);
            T.Eq(6, plainReport.Count(EditStatus.Applied), "a build without a session overlay still finds the copies: " + string.Join("; ", plainReport.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.Path + " " + r.Message)));
            T.True(plain.Overlay == null, "and leaves the stack as it was");
        }

        // 명령줄
        string cliProject = Path.Combine(repo, "editor", "projects", "cli.json");
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "define-copy", "--file", F, "--path", "/(define \"crate\")", "--new", "\"crate_y\"" }, w), "CLI add copies a define: " + w);
        T.Eq(DefineCopy.Kind, EditProject.Load(cliProject).Edits.Single().Kind, "with the kind");
        w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "macro-name", "--file", Cart, "--path", "/game_entity/(\"crate\")", "--new", "\"crate_y\"" }, w), "CLI renames a call with arguments to the copy: " + w);
        var w2 = new StringWriter();
        T.Eq(1, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "macro-name", "--file", Cart, "--path", "/game_entity/(\"crate\")", "--new", "\"nope\"" }, w2), "a name that is not defined is refused: " + w2);
        var w3 = new StringWriter();
        T.Eq(1, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "define-copy", "--file", F, "--path", "/(define \"missing\")", "--new", "\"m2\"" }, w3), "a define that is not there is refused: " + w3);

        UnitCopyGetsTheDefineCopy();
        ClashingCopyFileBlocksDependents();
    }

    // '이 유닛만' 사본도 복제를 넣은 본문에서 만든다. 아니면 사본을 include하는 유닛이 복제본을 찾지 못한다(define not found)
    static void UnitCopyGetsTheDefineCopy()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        string projectPath = Path.Combine(repo, "editor", "projects", "mixed.json");
        var project = new EditProject { Name = "mixed" };
        project.Edits.Add(DefineCopy.New(Mixed, Mixed, "m", "m_x"));
        project.Edits.Add(Swap(Truck2, "m", "m_x"));
        project.Edits.Add(new Edit { Unit = Truck2, File = Mixed, Path = "/v", Occurrence = 0, Arg = 0, Old = "1", New = "2", Scope = "unit" });
        project.Save(projectPath);
        using var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(projectPath);
        var report = session.Build();
        T.Eq(3, report.Count(EditStatus.Applied), "the copy, the rename and the unit-only value apply: " + string.Join("; ", report.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.Path + " " + r.Message)));
        string truck = FactionPresetTests.Out(outDir, Truck2) ?? "";
        var copies = Directory.GetFiles(Path.Combine(outDir, "resource", "properties"), "mixed__ctb_*").ToList();
        T.Eq(1, copies.Count, "the unit gets its own copy of mixed.inc");
        string unitCopy = copies.Count == 1 ? File.ReadAllText(copies[0]) : "";
        T.True(truck.Contains("(\"m_x\")") && truck.Contains(Path.GetFileName(copies.FirstOrDefault() ?? "?")), "the truck includes its copy and calls the define copy: " + truck);
        T.True(unitCopy.Contains("(define \"m_x\"") && unitCopy.Contains("{v 2}"), "the unit's copy has the define copy and the unit-only value: " + unitCopy);
    }

    // 복제가 든 파일을 쓸 수 없으면(출력 폴더에 손으로 넣은 같은 경로의 파일) 그 복제본을 부르는 수정도 막는다
    static void ClashingCopyFileBlocksDependents()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);
        string projectPath = Path.Combine(repo, "editor", "projects", "clash.json");
        var project = new EditProject { Name = "clash" };
        project.Edits.Add(DefineCopy.New(F, F, "truck_ammo", "truck_ammo_x"));
        project.Edits.Add(Swap(Truck, "truck_ammo", "truck_ammo_x"));
        project.Save(projectPath);
        Directory.CreateDirectory(Path.Combine(outDir, "resource", "properties"));
        File.WriteAllText(Path.Combine(outDir, "resource", "properties", "resupply.inc"), "; hand made\n");
        using var session = EditorSession.Open(settings, repo, null);
        session.OpenProject(projectPath);
        var checks = session.CheckEdits();
        T.True(checks.All(c => c.Status == EditStatus.Invalid), "the check refuses the copy and the call that needs it: " + string.Join("; ", checks.Select(c => c.Edit.Path + " " + c.Status + " " + c.Message)));
        var report = session.Build();
        T.True(report.Results.All(r => r.Status != EditStatus.Applied), "the build applies neither: " + string.Join("; ", report.Results.Select(r => r.Edit.Path + " " + r.Status + " " + r.Message)));
        T.True(!(FactionPresetTests.Out(outDir, Truck) ?? "").Contains("truck_ammo_x"), "the truck is not left calling a define that was not written");
    }
}
