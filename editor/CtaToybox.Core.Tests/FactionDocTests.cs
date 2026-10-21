using System.Collections.Generic;
using CtaToybox.Core;

// 팩션 강화 수정 내용 문서(생성물 docs/14-faction-boost/<군대>.md)의 꼴
static class FactionDocTests
{
    public static void Run()
    {
        var project = new EditProject { Name = "강화-독일", Rules = new List<BulkRule> { new BulkRule { Name = "ger 속도 ×1.5", Kind = "vehicle", Army = "ger", Key = "**/locomotion/maxspeed", Op = "mul", Value = "1.5", Scope = "auto" } } };
        var report = new FactionReport { Units = 10, Infantry = 6, Vehicles = 4 };
        report.Edits["ger 속도 ×1.5"] = 7;
        report.Edits["ger 보급 반경 ×3"] = 2;
        report.Clips.Add(new ClipChange { Weapon = "set/stuff/gun/128oto_ctb_ger", Filling = 3, GapOld = "20", GapNew = "10", ReloadOld = "18", ReloadNew = "9" });
        var supply = new SupplyChange { Macro = "truck_ammo_heavy", Copy = "truck_ammo_heavy_ctb_ger", RadiusOld = "20", RadiusNew = "60", LimitOld = "3000", LimitNew = "50000", RegenOld = "5", RegenNew = "0.0002" };
        supply.Units.Add("entity/-vehicle/germany/car/opel_maultier/opel_maultier.def");
        report.Supplies.Add(supply);
        report.Skipped.Add("entity/a.def: 이 키가 없다");
        report.Skipped.Add("entity/b.def: 이 키가 없다");
        report.Skipped.Add("entity/c.def: 다른 군대도 이 유닛 파일을 쓴다");
        report.Skipped.Add("ger 차량 탄약 ×5: 이 키가 없다 (633)");   // 일괄 규칙은 이유마다 모은 수를 괄호에 적는다(FactionPreset.AddRule)
        report.Skipped.Add("ger 장갑 차량 ×2: 이 키가 없다 (74)");
        report.Shared.Add("entity/-vehicle/germany/btr/sdkfz10_4/sdkfz10_4.def: ger, fin");

        string md = FactionDoc.Markdown(project, new FactionOptions { Army = "ger" }, report, new[] { "vanilla", "mod_2905667604", "mod_3732639687" }, "월드워-차종", "2026-10-21");
        T.True(md.StartsWith("# 강화-독일 수정 내용\n"), "title: " + md.Substring(0, System.Math.Min(60, md.Length)));
        T.True(md.Contains("- 만든 날: 2026-10-21") && md.Contains("- 베이스: 월드워-차종"), "date and base");
        T.True(md.Contains("- 원본 계층: vanilla → mod_2905667604 → mod_3732639687"), "layers in load order");
        T.True(md.Contains("재장전 ×0.5") && md.Contains("보급 반경 ×3") && md.Contains("보급 총량 50000") && md.Contains("10초에 가득") && md.Contains("클립식 기준: 탄창 2발 이상, 발 사이 1초 이상"), "the options: " + md);
        T.True(!md.Contains("무제한)"), "no longer calls a number unlimited");
        T.True(!md.Contains("무제한 보급"), "the boosted supply is large (50000), not unlimited: " + md);
        T.True(md.Contains("## 항목별 수정") && md.Contains("| ger 보급 반경 ×3 | 2 |") && md.Contains("| ger 속도 ×1.5 | 7 |"), "item counts as a table");
        T.True(md.Contains("| ger 속도 ×1.5 | vehicle | `**/locomotion/maxspeed` | ×1.5 |"), "the bulk rules as a table");
        T.True(md.Contains("## 클립식 포") && md.Contains("| `set/stuff/gun/128oto_ctb_ger` | 3 | 20 → 10 | 18 → 9 |"), "clip table");
        T.True(md.Contains("## 보급") && md.Contains("| `truck_ammo_heavy` | `truck_ammo_heavy_ctb_ger` | 20 → 60 | 3000 → 50000 | 5 → 0.0002 | `entity/-vehicle/germany/car/opel_maultier/opel_maultier.def` |"), "supply table: " + md);
        T.True(md.Contains("## 건너뛴 것") && md.Contains("모두 710건"), "the total counts what the rules folded into (N): " + md);
        T.True(md.Contains("| ger 차량 탄약 ×5: 이 키가 없다 | 633 |") && md.Contains("| ger 장갑 차량 ×2: 이 키가 없다 | 74 |"), "a rule's skip keeps its rule and its count");
        T.True(md.Contains("| 이 키가 없다 | 2 | `entity/a.def` |") && md.Contains("| 다른 군대도 이 유닛 파일을 쓴다 | 1 | `entity/c.def` |"), "unit skips are grouped by reason with an example");
        T.True(md.IndexOf("| ger 차량 탄약 ×5") < md.IndexOf("| ger 장갑 차량 ×2") && md.IndexOf("| ger 장갑 차량 ×2") < md.IndexOf("| 이 키가 없다 | 2"), "most first");
        T.True(md.Contains("## 두 군대가 같이 쓰는 유닛") && md.Contains("sdkfz10_4"), "shared units");
        T.True(md.Contains("## 한계와 미확인") && md.Contains("E19") && md.Contains("E20") && md.Contains("`limit 0`") && md.Contains("동작하지 않았다") && md.Contains("0.0002"), "limits: limit 0 does not work (user check), fractional regen unknown");
        T.True(md.Contains("보급 범위 그림이 같이 커지는지는 미확인이다") && md.Contains("노획") && md.Contains("supply(500)"), "the unknowns read as unknowns, capture and unused call arguments are listed");
        T.True(md.TrimEnd().EndsWith("이 문서는 `faction-preset`이 만든다. 손으로 고치지 않는다."), "generated note");
        string other = FactionDoc.Markdown(project, new FactionOptions { Army = "ger", SupplyLimit = "5000", ClipMinGap = 2.5 }, report, new[] { "vanilla" }, null, "2026-10-21");
        T.True(other.Contains("보급 총량 5000") && other.Contains("발 사이 2.5초 이상") && other.Contains("- 베이스: 없음"), "other options are written as they are");
    }
}
