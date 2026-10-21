// 팩션 강화 수정 내용 문서(생성물 docs/14-faction-boost/<군대>.md, docs/superpowers/specs/2026-10-21-faction-boost-clip-supply-design.md).
// faction-preset이 프로젝트를 만들 때 같이 쓴다. 사람이 무엇이 바뀌었는지 확인하는 용도이고, 빌드는 이 문서를 읽지 않는다
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static class FactionDoc
    {
        static string N(double v) { return v.ToString("0.####", CultureInfo.InvariantCulture); }

        static string Cell(string s) { return (s ?? "").Replace("|", "\\|").Replace("\r", " ").Replace("\n", " "); }

        public static string LimitText(string limit) { return limit == "0" ? "0(무제한)" : limit; }

        static readonly Regex Folded = new Regex("^(.*) \\((\\d+)\\)$", RegexOptions.CultureInvariant);

        sealed class Skip { public string Key; public int Count; public string Example; }

        // 건너뜀 한 줄: "유닛 경로: 이유"는 이유로 묶고 경로를 예로 든다. 일괄 규칙 줄("규칙 이름: 이유 (N)")은 규칙과 이유를 그대로 두고 N을 센다(FactionPreset.AddRule)
        static Skip SkipOf(string line)
        {
            int at = line.IndexOf(": ", StringComparison.Ordinal);
            string head = at < 0 ? "" : line.Substring(0, at);
            if (head.Contains('/')) return new Skip { Key = line.Substring(at + 2), Count = 1, Example = head };
            var m = Folded.Match(line);
            return m.Success ? new Skip { Key = m.Groups[1].Value, Count = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) } : new Skip { Key = line, Count = 1 };
        }

        public static string Markdown(EditProject project, FactionOptions o, FactionReport report, IReadOnlyList<string> layers, string baseName, string date)
        {
            var sb = new StringBuilder();
            double gapFactor = o.ClipGap > 0 ? o.ClipGap : o.Reload;
            sb.Append("# ").Append(project.Name).Append(" 수정 내용\n\n");
            sb.Append("- 만든 날: ").Append(date).Append('\n');
            sb.Append("- 베이스: ").Append(string.IsNullOrEmpty(baseName) ? "없음" : baseName).Append('\n');
            sb.Append("- 군대: ").Append(o.Army).Append("(유닛 ").Append(report.Units).Append(": 보병 ").Append(report.Infantry).Append(", 차량 ").Append(report.Vehicles).Append(")\n");
            sb.Append("- 원본 계층: ").Append(string.Join(" → ", layers ?? new List<string>())).Append('\n');
            sb.Append("- 배수: 인벤토리 ×").Append(N(o.Inventory)).Append(", 탄약 ×").Append(N(o.Ammo)).Append(", 재장전 ×").Append(N(o.Reload))
              .Append(", 보병 체력 ×").Append(N(o.Health)).Append(", 장갑 차량 ×").Append(N(o.ArmorCar)).Append("·경전차 ×").Append(N(o.ArmorLight))
              .Append("·중형전차 ×").Append(N(o.ArmorMedium)).Append("·중전차 ×").Append(N(o.ArmorHeavy)).Append("·초중전차 ×").Append(N(o.ArmorSuper))
              .Append(", 속도 ×").Append(N(o.Speed)).Append(", 연료·항속 ×").Append(N(o.Fuel))
              .Append(", 보급 반경 ×").Append(N(o.SupplyRadius)).Append(", 보급 총량 ").Append(LimitText(o.SupplyLimit)).Append('\n');
            sb.Append("- 클립식 기준: 탄창 2발 이상, 발 사이 ").Append(N(o.ClipMinGap)).Append("초 이상인 `set/stuff/gun` 무기 사본. 발 사이 ×").Append(N(gapFactor)).Append(", 소수 셋째 자리까지\n");
            sb.Append("- 수정 ").Append(project.Edits.Count).Append("건, 일괄 규칙 ").Append(project.Rules == null ? 0 : project.Rules.Count).Append("개\n\n");

            sb.Append("## 항목별 수정\n\n| 항목 | 수정 수 |\n|---|---|\n");
            foreach (var kv in report.Edits) sb.Append("| ").Append(Cell(kv.Key)).Append(" | ").Append(kv.Value).Append(" |\n");
            if (project.Rules != null && project.Rules.Count > 0)
            {
                sb.Append("\n일괄 규칙(앱의 \"일괄 규칙\"에서 다시 적용할 수 있다):\n\n| 규칙 | 대상 | 키 | 바꾸기 |\n|---|---|---|---|\n");
                foreach (var r in project.Rules)
                    sb.Append("| ").Append(Cell(r.Name)).Append(" | ").Append(Cell(r.Kind)).Append(" | `").Append(Cell(r.Key)).Append("` | ").Append(r.Op == "mul" ? "×" : r.Op == "add" ? "+" : "= ").Append(Cell(r.Value)).Append(" |\n");
            }

            sb.Append("\n## 클립식 포\n\n탄창 안 발 사이 간격과 탄창 전체 장전을 같은 배수로 줄여 원래 짧은/긴 비율을 지킨다.\n\n");
            if (report.Clips.Count == 0) sb.Append("없음.\n");
            else
            {
                sb.Append("| 무기 사본 | 탄창 | 발 사이(초) | 탄창 장전(초) |\n|---|---|---|---|\n");
                foreach (var c in report.Clips.OrderBy(x => x.Weapon, StringComparer.Ordinal))
                    sb.Append("| `").Append(c.Weapon).Append("` | ").Append(c.Filling).Append(" | ").Append(c.GapOld).Append(" → ").Append(c.GapNew).Append(" | ").Append(c.ReloadOld).Append(" → ").Append(c.ReloadNew).Append(" |\n");
            }

            sb.Append("\n## 보급\n\n그 군대 보급 유닛이 부르는 `properties/resupply.inc` 매크로를 복제해 값을 바꾸고, 그 유닛의 호출 이름만 복제본으로 바꾼다. 원래 매크로와 보급 건물(깃발 탄약점, 탄약 진지 등)은 그대로다.\n\n");
            if (report.Supplies.Count == 0) sb.Append("없음.\n");
            else
            {
                sb.Append("| 매크로 | 복제본 | 반경(m) | 총량 | 바꾼 유닛 |\n|---|---|---|---|---|\n");
                foreach (var s in report.Supplies.OrderBy(x => x.Macro, StringComparer.Ordinal))
                    sb.Append("| `").Append(s.Macro).Append("` | `").Append(s.Copy).Append("` | ").Append(s.RadiusOld).Append(" → ").Append(s.RadiusNew).Append(" | ").Append(s.LimitOld).Append(" → ").Append(s.LimitNew)
                      .Append(" | ").Append(string.Join(", ", s.Units.Select(u => "`" + u + "`"))).Append(" |\n");
            }

            sb.Append("\n## 건너뛴 것\n\n");
            if (report.Skipped.Count == 0) sb.Append("없음.\n");
            else
            {
                var groups = report.Skipped.Select(SkipOf).GroupBy(x => x.Key)
                    .Select(g => new { g.Key, Count = g.Sum(x => x.Count), Example = g.Select(x => x.Example).FirstOrDefault(x => x != null) })
                    .OrderByDescending(g => g.Count).ThenBy(g => g.Key, StringComparer.Ordinal).ToList();
                sb.Append("모두 ").Append(groups.Sum(g => g.Count)).Append("건. 이유별 수(많은 순 ").Append(Math.Min(30, groups.Count)).Append("개). 전체 목록은 `faction-preset --report <파일>`이 쓴다.\n\n| 이유 | 수 | 예 |\n|---|---|---|\n");
                foreach (var g in groups.Take(30)) sb.Append("| ").Append(Cell(g.Key)).Append(" | ").Append(g.Count).Append(" | ").Append(g.Example == null ? "" : "`" + g.Example + "`").Append(" |\n");
            }

            sb.Append("\n## 두 군대가 같이 쓰는 유닛\n\n고치면 다른 군대도 바뀌므로 건너뛴다.\n\n");
            if (report.Shared.Count == 0) sb.Append("없음.\n");
            else foreach (string x in report.Shared) sb.Append("- `").Append(x).Append("`\n");

            sb.Append("\n## 한계와 미확인\n\n");
            sb.Append("- 백로그 E19: 장전 시간이 원형의 탄종별 `{parameters …}` 블록 안에 있는 무기, 매크로 본문에서 무기 이름을 부르는 차량은 강화가 빠진다.\n");
            sb.Append("- 백로그 E20: 모든 군대가 같이 쓰는 매크로 본문 안의 장갑(`sandbags` 등)은 바꾸지 않았다.\n");
            sb.Append("- 보급 `limit 0`이 무제한이라는 것은 파일 주석으로만 안다. 반경을 키우면 보급 범위 그림이 같이 커지는지는 미확인이다. 컨퀘스트의 보급 차량 AP 비용(로스터의 `supply_ap`)은 그대로다.\n");
            sb.Append("- 보급 매크로 복제본은 `limit`을 직접 적으므로 호출에 남은 총량 인자(`supply(500)`, `liters(2500)`)는 쓰이지 않는다. 쓰이지 않는 인자를 게임이 문제없이 넘기는지는 미확인이다.\n");
            sb.Append("- 강화한 무기 사본·보급 차량을 적이 노획하면 그 적도 빠른 재장전·넓은 무제한 보급을 쓴다(받아들임).\n");
            sb.Append("- 사본에 더한 `rechargeTime`·`recoveryTime` 줄과 보병 perks에 더한 `health` 줄이 원형·매크로 값을 이기는지 게임 안에서 확인하지 않았다.\n");
            sb.Append("- 같은 블록에 `recoveryTime`이 여러 줄인 무기는 모두 곱했다. 게임이 어느 줄을 쓰는지 미확인이다.\n");
            sb.Append("\n이 문서는 `faction-preset`이 만든다. 손으로 고치지 않는다.\n");
            return sb.ToString();
        }
    }
}
