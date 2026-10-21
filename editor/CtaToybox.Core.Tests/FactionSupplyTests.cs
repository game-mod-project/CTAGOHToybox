using System.Linq;
using CtaToybox.Core;

// 팩션 강화: 그 군대 보급 유닛이 부르는 보급 매크로를 복제해 반경 ×3, 총량 0(무제한)으로 바꾸고 호출 이름만 복제본으로(2026-10-21 설계)
static class FactionSupplyTests
{
    const string Resupply = "(define \"truck_ammo\"\n {extender \"supply_zone\" {radius 20} {limit 3000}}\n)\n(define \"crate\"\n {extender \"supply_zone\" {radius 10} {limit %supply}}\n)\n"
        + "(define \"flag_heavy\"\n {extender \"supply_zone\" {radius 15} {limit 50000}}\n)\n(define \"items\" {x 1})\n";
    const string Truck = "entity/-vehicle/germany/car/truck/truck.def", Cart = "entity/-vehicle/germany/trailer/cart/cart.def";
    const string Eng = "entity/-vehicle/germany/car/eng/eng.def", EngInc = "entity/-vehicle/germany/car/eng/eng.inc";
    const string Soviet = "entity/-vehicle/ussr/car/truck_r/truck_r.def", Flag = "entity/service/-multiplayer/flag_point/flagpoint_ammo/flagpoint_ammo.def";

    static string Calls(string call) { return "{game_entity\n (include \"/properties/resupply.inc\")\n " + call + "\n}\n"; }

    public static void Run()
    {
        var vehicles = new[]
        {
            (Truck, Calls("(\"truck_ammo\")")), (Cart, Calls("(\"crate\" supply(500))")),
            (Eng, Calls("(include \"eng.inc\")")), (EngInc, "(\"truck_ammo\")\n"),
            (Soviet, Calls("(\"truck_ammo\")")), (Flag, Calls("(\"flag_heavy\")")),
        };
        string repo, outDir;
        string settings = FactionPresetTests.FixtureWith(out repo, out outDir, new (string, string)[0], vehicles, new[] { (FactionPreset.SupplyFile, Resupply) });
        using var s = EditorSession.Open(settings, repo, null);
        var c = FactionPresetTests.Context(s);
        var project = new EditProject { Name = "ger" };
        var report = new FactionReport();
        FactionPreset.AddSupply(project, c, new FactionOptions { Army = "ger" }, report);
        T.Eq(new[] { "\"crate_ctb_ger\"", "\"truck_ammo_ctb_ger\"" }, project.Edits.Where(e => e.Kind == DefineCopy.Kind).Select(e => e.New).OrderBy(x => x).ToArray(), "one copy per macro the army calls");
        var build = new ModBuilder(s.Stack, outDir).Build(project);
        T.Eq(0, build.Results.Count(r => r.Status != EditStatus.Applied), "everything applies: " + string.Join("; ", build.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.File + " " + r.Message)));
        string inc = FactionPresetTests.Out(outDir, FactionPreset.SupplyFile) ?? "";
        T.True(inc.StartsWith(Resupply), "the originals and the building macro stay as they were: " + inc);
        T.True(inc.Contains("(define \"truck_ammo_ctb_ger\"\n {extender \"supply_zone\" {radius 60} {limit 0}}") && inc.Contains("(define \"crate_ctb_ger\"\n {extender \"supply_zone\" {radius 30} {limit 0}}"), "the copies: radius ×3, limit 0 (also over %supply): " + inc);
        T.True(!inc.Contains("flag_heavy_ctb_ger"), "the building macro is not copied");
        T.True((FactionPresetTests.Out(outDir, Truck) ?? "").Contains("(\"truck_ammo_ctb_ger\")"), "the German truck calls the copy");
        T.True((FactionPresetTests.Out(outDir, Cart) ?? "").Contains("(\"crate_ctb_ger\" supply(500))"), "the cart keeps its argument");
        T.Eq(null, FactionPresetTests.Out(outDir, Soviet), "the Soviet truck is untouched");
        T.Eq(null, FactionPresetTests.Out(outDir, Flag), "the building is untouched");
        // include 파일 안에서 부르는 호출은 바꾸지 않고 알린다
        T.Eq(null, FactionPresetTests.Out(outDir, EngInc), "a call inside an include is not renamed");
        T.True(report.Skipped.Any(x => x.StartsWith(Eng) && x.Contains("유닛 파일 밖")), "and is reported: " + string.Join("; ", report.Skipped));
        var truck = report.Supplies.Single(x => x.Macro == "truck_ammo");
        T.True(truck.Copy == "truck_ammo_ctb_ger" && truck.RadiusOld == "20" && truck.RadiusNew == "60" && truck.LimitOld == "3000" && truck.LimitNew == "0" && truck.Units.SequenceEqual(new[] { Truck }), "the report row: " + truck.Copy + " " + truck.RadiusOld + "→" + truck.RadiusNew + " " + truck.LimitOld + "→" + truck.LimitNew + " " + string.Join(",", truck.Units));
        var crate = report.Supplies.Single(x => x.Macro == "crate");
        T.True(crate.LimitOld == "%supply" && crate.LimitNew == "0" && crate.Units.SequenceEqual(new[] { Cart }), "the crate row");
        T.True(report.Edits["ger 보급 매크로 복제"] == 2 && report.Edits["ger 보급 반경 ×3"] == 2 && report.Edits["ger 보급 총량 → 0"] == 2 && report.Edits["ger 보급 호출 → 복제본"] == 2, "report counts: " + string.Join(", ", report.Edits.Select(kv => kv.Key + "=" + kv.Value)));

        int before = project.Edits.Count, rows = report.Supplies.Count;
        FactionPreset.AddSupply(project, c, new FactionOptions { Army = "ger" }, report);
        T.Eq(before, project.Edits.Count, "running it again adds nothing");
        T.Eq(rows, report.Supplies.Count, "and reports each macro once");

        string repo2, outDir2;
        string plain = FactionPresetTests.Fixture(out repo2, out outDir2);
        using var s2 = EditorSession.Open(plain, repo2, null);
        var none = new EditProject { Name = "none" };
        FactionPreset.AddSupply(none, FactionPresetTests.Context(s2), new FactionOptions { Army = "ger" }, new FactionReport());
        T.Eq(0, none.Edits.Count, "no resupply.inc, nothing to do");

        // 프리셋 전체: 끝나면 스택의 덧씌우기를 되돌린다
        var overlayBefore = s.Stack.Overlay = null;
        FactionPreset.Build(c, null, new FactionOptions { Army = "ger" }, new ConquestValues(), new FactionReport());
        T.True(s.Stack.Overlay == overlayBefore, "FactionPreset.Build leaves the stack overlay as it was");
    }
}
