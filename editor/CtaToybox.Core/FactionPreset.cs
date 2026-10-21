// 팩션별 강화 프로젝트(docs/superpowers/specs/2026-10-17-faction-boost-design.md).
// 한 군대(ArmyIndex)의 유닛에만 강화 수정을 넣는다. 다른 군대가 같이 쓰는 파일은 '이 유닛만'(include 사본)이나 파일 사본으로 피한다.
//   인벤토리·장갑·속도·연료: F1 일괄 규칙(군대 거르기)으로 만들어 project.Rules에 둔다. 앱에서 규칙으로 보고 다시 적용할 수 있다
//   연료의 항속(range) 인자: 연료 규칙이 고친 mobility 호출마다 같은 범위로 직접 고친다(rate = 10 × fuel / range라 둘 다 바꿔야 오래 간다)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class FactionOptions
    {
        public string Army { get; set; }
        public double Inventory { get; set; } = 2;
        public double Ammo { get; set; } = 5;
        public double Reload { get; set; } = 0.5;
        public double Health { get; set; } = 2;
        public double ArmorCar { get; set; } = 2;
        public double ArmorLight { get; set; } = 2;
        public double ArmorMedium { get; set; } = 2.5;
        public double ArmorHeavy { get; set; } = 3;
        public double ArmorSuper { get; set; } = 5;
        public double Speed { get; set; } = 1.5;
        public double Fuel { get; set; } = 10;
        public List<string> SuperHeavy { get; set; } = new List<string>();   // 초중전차 유닛 폴더(가상 경로 앞부분). editor/presets/super-heavy.txt
        // 클립식 포(set/stuff/gun, 탄창 2발 이상, 발 사이 ClipMinGap초 이상)는 발 사이 간격도 줄인다. ClipGap이 0이면 Reload와 같은 배수
        public double ClipMinGap { get; set; } = 1;
        public double ClipGap { get; set; } = 0;
    }

    // 클립식 포 하나의 원래·강화 값(초. 발 사이는 "fire rate"면 60/rpm)
    public sealed class ClipChange
    {
        public string Weapon;
        public int Filling;
        public string GapOld;
        public string GapNew;
        public string ReloadOld;
        public string ReloadNew;
    }

    public sealed class FactionReport
    {
        public int Units { get; set; }
        public int Infantry { get; set; }
        public int Vehicles { get; set; }
        public Dictionary<string, int> Edits { get; } = new Dictionary<string, int>(StringComparer.Ordinal);   // 항목 → 수정 수
        public List<string> Skipped { get; } = new List<string>();   // "유닛: 이유"
        public List<string> Shared { get; } = new List<string>();
        public List<string> Notes { get; } = new List<string>();
        public List<ClipChange> Clips { get; } = new List<ClipChange>();
    }

    // 프리셋이 쓰는 색인들. 세션 없이(명령줄·실제 검사) 만든다
    public sealed class FactionContext
    {
        public SourceStack Stack { get; set; }
        public UnitIndex Index { get; set; }
        public UnitTracer Tracer { get; set; }
        public ArmyIndex Armies { get; set; }
        public ItemLinks Items { get; set; }
    }

    public static partial class FactionPreset
    {
        public static readonly string[] CarFolders = { "/car/", "/btr/" };
        public static readonly string[] LightFolders = { "/tank_light/", "/light/" };
        public static readonly string[] MediumFolders = { "/tank_medium/" };
        public static readonly string[] HeavyFolders = { "/tank_heavy/", "/heavy/" };

        const string ArmorKeys = "**/thickness, **/thickness/front, **/thickness/top, **/thickness/rear, **/thickness/bottom, **/thickness/left, **/thickness/right";
        const string ItemKeys = "**/inventory/item, **/box/item";

        // 차종: car | light | medium | heavy | super. 차량·전차 폴더 밖이면 null
        public static string ClassOf(string unitPath, ICollection<string> superHeavy)
        {
            string p = SourceStack.ToVirtual(unitPath ?? "");
            if (!p.StartsWith("entity/-vehicle/", StringComparison.Ordinal)) return null;
            if (superHeavy != null && superHeavy.Any(s => s.Length > 0 && p.StartsWith(SourceStack.ToVirtual(s), StringComparison.Ordinal))) return "super";
            if (HeavyFolders.Any(f => p.Contains(f))) return "heavy";
            if (MediumFolders.Any(f => p.Contains(f))) return "medium";
            if (LightFolders.Any(f => p.Contains(f))) return "light";
            if (CarFolders.Any(f => p.Contains(f))) return "car";
            return null;
        }

        // 초중전차 목록 파일: 한 줄에 유닛 폴더 하나(entity/-vehicle/+germany/tank_heavy/mausking/), # 뒤는 주석
        public static List<string> LoadSuperHeavy(string path)
        {
            if (path == null || !File.Exists(path)) return new List<string>();
            return File.ReadAllLines(path).Select(l => { int h = l.IndexOf('#'); return (h < 0 ? l : l.Substring(0, h)).Trim(); })
                .Where(l => l.Length > 0).Select(l => SourceStack.ToVirtual(l).TrimEnd('/') + "/").Distinct().ToList();
        }

        static string Num(double v) { return v.ToString("0.####", CultureInfo.InvariantCulture); }

        public static void AddUnitStats(EditProject project, FactionContext c, FactionOptions o, FactionReport report)
        {
            var supers = o.SuperHeavy ?? new List<string>();
            var vehicleFolders = CarFolders.Concat(LightFolders).Concat(MediumFolders).Concat(HeavyFolders).ToList();
            var rules = new List<BulkRule>
            {
                Rule(o, "보병 탄약 ×" + Num(o.Ammo), "breed", ItemKeys, -1, o.Ammo, items: "ammo"),
                Rule(o, "보병 소지품 ×" + Num(o.Inventory), "breed", ItemKeys, -1, o.Inventory, items: "other"),
                Rule(o, "차량 탄약 ×" + Num(o.Ammo), "vehicle", ItemKeys, -1, o.Ammo, items: "ammo"),
                Rule(o, "차량 소지품 ×" + Num(o.Inventory), "vehicle", ItemKeys, -1, o.Inventory, items: "other"),
                Rule(o, "장갑 차량 ×" + Num(o.ArmorCar), "vehicle", ArmorKeys, -1, o.ArmorCar, any: CarFolders, exclude: supers),
                Rule(o, "장갑 경전차 ×" + Num(o.ArmorLight), "vehicle", ArmorKeys, -1, o.ArmorLight, any: LightFolders, exclude: supers),
                Rule(o, "장갑 중형전차 ×" + Num(o.ArmorMedium), "vehicle", ArmorKeys, -1, o.ArmorMedium, any: MediumFolders, exclude: supers),
                Rule(o, "장갑 중전차 ×" + Num(o.ArmorHeavy), "vehicle", ArmorKeys, -1, o.ArmorHeavy, any: HeavyFolders, exclude: supers),
                Rule(o, "속도 ×" + Num(o.Speed), "vehicle", "**/locomotion/maxspeed", 0, o.Speed, any: vehicleFolders),
                Rule(o, "연료 ×" + Num(o.Fuel), "vehicle", "**/fuelbag/volume, **/fuelbag/remain", 0, o.Fuel, any: vehicleFolders),
            };
            if (supers.Count > 0) rules.Insert(8, Rule(o, "장갑 초중전차 ×" + Num(o.ArmorSuper), "vehicle", ArmorKeys, -1, o.ArmorSuper, any: supers));
            foreach (var rule in rules) AddRule(project, c, rule, report);
            AddRange(project, c, rules.Last(), o.Fuel, report);
        }

        static BulkRule Rule(FactionOptions o, string name, string kind, string key, int arg, double factor, string items = null, IEnumerable<string> any = null, IEnumerable<string> exclude = null)
        {
            var ex = exclude == null ? null : exclude.ToList();
            return new BulkRule
            {
                Name = o.Army + " " + name, Kind = kind, Army = o.Army, Key = key, Arg = arg, Op = "mul", Value = Num(factor), Scope = "auto", Items = items,
                Any = any == null ? null : any.ToList(), Exclude = ex == null || ex.Count == 0 ? null : ex,
            };
        }

        // 규칙을 프로젝트 끝에 붙이고 그 수정을 만든다(EditorSession.AddRule과 같은 순서 규칙: 앞 규칙과 사람 수정이 있는 자리는 건너뛴다)
        public static BulkRule AddRule(EditProject project, FactionContext c, BulkRule rule, FactionReport report)
        {
            if (project.Rules == null) project.Rules = new List<BulkRule>();
            var added = rule.Copy();
            added.Id = EditorSession.NewRuleId(project.Rules);
            var preview = BulkRules.Preview(added, project, c.Index.Units, c.Tracer, f => c.Index.UsersOf(f), u => c.Index.HeirsOf(u).Count, p => c.Armies.ArmyOf(p), n => c.Items.IsAmmo(n));
            project.Rules.Add(added);
            project.Edits.AddRange(preview.Edits);
            report.Edits[added.Name] = preview.Edits.Count;
            foreach (var g in preview.Skipped.GroupBy(k => k.Reason)) report.Skipped.Add(added.Name + ": " + g.Key + " (" + g.Count() + ")");
            return added;
        }

        // 연료 규칙이 고친 mobility 호출의 range 인자도 같은 배수로(같은 범위·유닛). 규칙 밖의 수정이다
        static void AddRange(EditProject project, FactionContext c, BulkRule fuelRule, double factor, FactionReport report)
        {
            var fuelId = project.Rules.Last(r => r.Name == fuelRule.Name).Id;
            int made = 0;
            var rowsByFile = new Dictionary<string, List<KeyRow>>(StringComparer.Ordinal);
            foreach (var e in project.Edits.Where(x => x.Rule == fuelId && x.Path != null && x.Path.Contains("(\"mobility_")).ToList())
            {
                List<KeyRow> rows;
                if (!rowsByFile.TryGetValue(e.File, out rows))
                {
                    string error;
                    rows = SourceRows.Of(e.File, c.Stack.ReadText(e.File), out error) ?? new List<KeyRow>();
                    rowsByFile[e.File] = rows;
                }
                var row = rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
                int at = row == null || row.LeafNames == null ? -1 : row.LeafNames.IndexOf("range");
                if (at < 0 || at >= row.Leaves.Count) { report.Skipped.Add(e.File + ": mobility 호출에 range 인자가 없다"); continue; }
                if (project.Edits.Any(x => x.File == e.File && x.Path == e.Path && x.Occurrence == e.Occurrence && x.Arg == at && x.Scope == e.Scope && (e.Scope != "unit" || x.Unit == e.Unit))) continue;
                string error2;
                string old = row.Leaves[at].Text, next = BulkRules.Compute("mul", old, Num(factor), out error2);
                if (next == null) { report.Skipped.Add(e.File + ": range " + error2); continue; }
                project.Edits.Add(new Edit { Unit = e.Unit, File = e.File, Path = e.Path, Occurrence = e.Occurrence, Arg = at, Old = old, New = next, Scope = e.Scope });
                made++;
            }
            report.Edits[fuelRule.Army + " 항속(range) ×" + Num(factor)] = made;
        }
    }
}
