// 월드워 시야 프로젝트(스펙: docs/superpowers/specs/2026-10-16-worldwar-vision-design.md).
// 지금 이긴 판(모드 포함)의 set/vision/vision_fields.inc에서 지상 시야 필드의 수정 목록을 만든다.
//   wot(월드워): 수평 시야각 360, 숨은 병사를 찾는 거리 50
//   wt(월드워-워썬더): 수평 시야각 360, 거리 매크로("spotting"·"spotting2")의 r × scale
// vision.set의 전체 배수는 건드리지 않는다. 같은 정의를 항공·해상·청각·쌍안경도 쓴다.
// 필드는 따옴표 이름 블록이라 이름 경로(NamedPath)도 적는다. 모드가 필드를 끼워 넣어도 프로젝트를 열 때 이름으로 다시 찾는다(Q7)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CtaToybox.Core
{
    // 차종별 시야 거리 배수 r과 탐지 거리 uncover(m). 기본값은 E18 스펙의 표다(중전차는 주포 필드 r 1.5 그대로)
    public sealed class ClassVision
    {
        public double MediumR { get; set; } = 1.65;
        public double LightR { get; set; } = 1.8;
        public double VehicleR { get; set; } = 1.8;
        public int HeavyUncover { get; set; } = 100;
        public int MediumUncover { get; set; } = 115;
        public int LightUncover { get; set; } = 130;
        public int VehicleUncover { get; set; } = 145;
    }

    public sealed class ClassReport
    {
        public int Light { get; set; }
        public int Medium { get; set; }
        public int VehicleFiles { get; set; }
        public List<string> Skipped { get; } = new List<string>();   // "유닛: 이유"
    }

    public static class VisionPresets
    {
        public const string FieldsFile = "set/vision/vision_fields.inc", VisionSet = "set/vision/vision.set";

        // 사람·포·차량·전차·구조물의 지상 시야 필드. 항공·해상과 일부러 좁힌 필드(조준경을 볼 때, 탐조등)는 넣지 않는다
        public static readonly string[] GroundFields =
        {
            "human", "human_elite", "human_recon", "human_peripheral",
            "cannon_main", "cannon_mortar", "cannon_range_200", "cannon_range_210", "cannon_range_220", "cannon_range_230", "cannon_range_240", "cannon_range_250",
            "vehicle_main", "vehicle_top_mgun", "vehicle_top_turret", "recon_main", "recon_open",
            "tank_main_mg", "tank_main-l", "tank_main-m", "tank_main-h", "tank_main-aa", "tank_driver", "tank_hull-mg", "tank_rear-mg", "tank_remote-mg", "tank_shielded-mg",
            "tank_range_200", "tank_range_210", "tank_range_220", "tank_range_230", "tank_range_240", "tank_range_250",
            "watchtower",
        };

        // 파일에 이 필드가 없거나 키가 없으면 notes에 적고 건너뛴다. 파일을 읽지 못하면 InvalidDataException
        public static EditProject Build(SourceStack stack, string variant, double scale, List<string> notes)
        {
            if (variant != "wot" && variant != "wt") throw new ArgumentException("variant는 wot 또는 wt다: " + variant);
            string error;
            var rows = SourceRows.Of(FieldsFile, stack.ReadText(FieldsFile), out error);
            if (rows == null) throw new InvalidDataException(FieldsFile + "을 읽지 못했다: " + error);
            var project = new EditProject { Name = variant == "wot" ? "월드워" : "월드워-워썬더" };
            foreach (string field in GroundFields)
            {
                string head = "/\"" + field + "\"/";
                var own = rows.Where(r => r.RawPath.StartsWith(head, StringComparison.OrdinalIgnoreCase)).ToList();
                if (own.Count == 0) { notes.Add(field + ": 필드 없음"); continue; }
                Set(project, own, notes, field, "h_fov", 0, old => "360");
                if (variant == "wot") Set(project, own, notes, field, "uncover", 0, old => "50");
                else
                {
                    var macro = own.FirstOrDefault(r => Is(r, head, "(\"spotting\")") || Is(r, head, "(\"spotting2\")"));
                    int arg = macro == null ? -1 : macro.LeafNames.IndexOf("r");
                    double old;
                    if (arg < 0 || !double.TryParse(macro.Leaves[arg].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out old)) { notes.Add(field + ": spotting r 없음"); continue; }
                    Add(project, macro, arg, (old * scale).ToString("0.###", CultureInfo.InvariantCulture));
                }
            }
            return project;
        }

        // ---------- 차종별 시야·탐지(E18, docs/superpowers/specs/2026-10-16-e18-class-vision-design.md)

        public const string ClassSource = "tank_range_200", MediumField = "ctb_tank_medium", LightField = "ctb_tank_light", VehicleField = "ctb_vehicle";
        public static readonly string[] VehicleFiles = { "properties/car.ext", "properties/armoredcar.ext", "properties/moto.ext" };

        // 차종 필드 셋을 tank_range_200의 사본으로 만들고, 경·중형전차 유닛과 차량 파일의 around visor가 그 필드를 쓰게 하는 수정을 project에 더한다.
        // 전차 필드(tank_…)의 uncover는 HeavyUncover로 바꾼다(중전차의 탐지). '이 유닛만'이 안 되는 유닛, around가 없는 유닛, 없는 차량 파일은 notes에 적고 건너뛴다
        public static ClassReport AddClasses(EditProject project, SourceStack stack, IEnumerable<UnitEntry> units, UnitTracer tracer, Func<string, int> heirsOf, ClassVision values, List<string> notes)
        {
            var report = new ClassReport();
            project.Name = "월드워-차종";
            string error;
            string original = stack.ReadText(FieldsFile);
            var rows = SourceRows.Of(FieldsFile, original, out error);
            if (rows == null) throw new InvalidDataException(FieldsFile + "을 읽지 못했다: " + error);
            // 중전차의 탐지: 지상 전차 필드의 uncover. wot가 이미 만든 수정은 값을 바꾸고, 원래 값이 50이라 수정이 없던 필드는 새로 더한다
            string heavy = values.HeavyUncover.ToString(CultureInfo.InvariantCulture);
            project.Edits.RemoveAll(x => x.NamedPath != null && x.NamedPath.EndsWith("/uncover", StringComparison.Ordinal) && x.NamedPath.StartsWith("/\"tank_", StringComparison.OrdinalIgnoreCase));
            foreach (string field in GroundFields.Where(f => f.StartsWith("tank_", StringComparison.Ordinal)))
            {
                var own = rows.Where(r => r.RawPath.StartsWith("/\"" + field + "\"/", StringComparison.OrdinalIgnoreCase)).ToList();
                if (own.Count > 0) Set(project, own, notes, field, "uncover", 0, old => heavy);
            }
            var head = rows.FirstOrDefault(r => string.Equals(r.RawPath, "/\"" + ClassSource + "\"", StringComparison.OrdinalIgnoreCase));
            if (head == null) { notes.Add(ClassSource + ": 필드 없음. 차종 필드를 만들지 않았다"); return report; }
            var classes = new[] { (MediumField, values.MediumR, values.MediumUncover), (LightField, values.LightR, values.LightUncover), (VehicleField, values.VehicleR, values.VehicleUncover) };
            var copies = classes.Select(c => new Edit { Kind = BlockCopy.Kind, Unit = VisionSet, File = FieldsFile, Path = head.Path, Occurrence = head.Occurrence, Arg = 0,
                NamedPath = head.RawPath, NamedOccurrence = head.RawOccurrence, Old = head.Leaves[0].Text, New = "\"" + c.Item1 + "\"", Scope = "shared" }).ToList();
            project.Edits.AddRange(copies);
            var copied = SourceRows.Of(FieldsFile, BlockCopy.ApplyAll(original, copies), out error);
            foreach (var c in classes)
            {
                string field = c.Item1, h = "/\"" + field + "\"/";
                var own = copied.Where(r => r.RawPath.StartsWith(h, StringComparison.OrdinalIgnoreCase)).ToList();
                Set(project, own, notes, field, "h_fov", 0, old => "360");
                Set(project, own, notes, field, "uncover", 0, old => c.Item3.ToString(CultureInfo.InvariantCulture));
                var macro = own.FirstOrDefault(r => Is(r, h, "(\"spotting2\")"));
                int arg = macro == null ? -1 : macro.LeafNames.IndexOf("r");
                if (arg < 0) notes.Add(field + ": spotting2 r 없음");
                else Add(project, macro, arg, c.Item2.ToString("0.###", CultureInfo.InvariantCulture));
            }

            var fileRows = new Dictionary<string, List<KeyRow>>(StringComparer.Ordinal);
            Func<string, List<KeyRow>> rowsOf = f => { List<KeyRow> r; if (!fileRows.TryGetValue(f, out r)) { string e2; r = SourceRows.Of(f, stack.ReadText(f), out e2) ?? new List<KeyRow>(); fileRows[f] = r; } return r; };
            foreach (var u in units.Where(x => x.Kind == "vehicle").OrderBy(x => x.Path, StringComparer.Ordinal))
            {
                string cls = u.Path.Contains("/tank_light/") ? LightField : u.Path.Contains("/tank_medium/") ? MediumField : null;
                if (cls == null || u.Path.EndsWith("_x.def", StringComparison.Ordinal) || u.Path.EndsWith("_xx.def", StringComparison.Ordinal)) continue;
                var traced = tracer.Trace(u.Path);
                var slots = traced == null ? new List<TraceValue>() : AroundSlots(traced, rowsOf);
                if (slots.Count == 0) { report.Skipped.Add(u.Path + ": around visor 없음"); continue; }
                int heirs = heirsOf(u.Path);
                string blocker = slots.Select(v => UnitScope.Blocker(traced, v.File, v.KeyPath, v.Occurrence, v.Arg)).FirstOrDefault(b => b != null) ?? (heirs > 0 ? FromLevel.HeirsNote(heirs) : null);
                if (blocker != null) { report.Skipped.Add(u.Path + ": " + blocker); continue; }
                foreach (var v in slots)
                    project.Edits.Add(new Edit { Unit = u.Path, File = v.File, Path = v.KeyPath, Occurrence = v.Occurrence, Arg = v.Arg, Old = v.Text, New = "\"" + cls + "\"", Scope = "unit" });
                if (cls == LightField) report.Light++; else report.Medium++;
            }
            foreach (string f in VehicleFiles)
            {
                if (stack.Find(f) == null) { notes.Add(f + ": 파일 없음"); continue; }
                var slot = AroundSlotsIn(rowsOf(f)).FirstOrDefault();
                if (slot == null) { notes.Add(f + ": around visor 없음"); continue; }
                project.Edits.Add(new Edit { Unit = f, File = f, Path = slot.Path, Occurrence = slot.Occurrence, Arg = 0, Old = slot.Leaves[0].Text, New = "\"" + VehicleField + "\"", Scope = "shared" });
                report.VehicleFiles++;
            }
            foreach (string s in report.Skipped) notes.Add("건너뜀: " + s);
            return report;
        }

        // 파일 줄 가운데 visor "around" 블록 안의 vision 줄. 바로 앞의 visor 머리 줄 이름으로 가른다(대소문자 무시)
        static List<KeyRow> AroundSlotsIn(List<KeyRow> rows)
        {
            var found = new List<KeyRow>();
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                if (!r.Path.EndsWith("/visor/vision", StringComparison.OrdinalIgnoreCase) || r.Leaves.Count == 0) continue;
                string visor = r.Path.Substring(0, r.Path.Length - "/vision".Length);
                for (int j = i - 1; j >= 0; j--)
                    if (string.Equals(rows[j].Path, visor, StringComparison.OrdinalIgnoreCase))
                    {
                        if (rows[j].Leaves.Count > 0 && string.Equals(rows[j].Leaves[0].Text.Trim('"'), "around", StringComparison.OrdinalIgnoreCase)) found.Add(r);
                        break;
                    }
            }
            return found;
        }

        // 유닛을 펼친 줄 가운데 around visor의 vision 값(그 값이 있는 파일의 자리). 같은 자리는 한 번만
        static List<TraceValue> AroundSlots(UnitTraceResult traced, Func<string, List<KeyRow>> rowsOf)
        {
            var result = new List<TraceValue>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var row in traced.Rows.Where(r => r.From == null && r.Path.EndsWith("/visor/vision", StringComparison.OrdinalIgnoreCase)))
                foreach (var v in row.Values.Where(x => x.Arg >= 0 && x.File != null))
                {
                    var around = AroundSlotsIn(rowsOf(v.File));
                    if (around.Any(a => a.Occurrence == v.Occurrence && string.Equals(a.Path, v.KeyPath, StringComparison.OrdinalIgnoreCase)) && seen.Add(v.File + "|" + v.KeyPath + "|" + v.Occurrence))
                        result.Add(v);
                }
            return result;
        }

        static bool Is(KeyRow row, string head, string key) { return string.Equals(row.RawPath, head + key, StringComparison.OrdinalIgnoreCase); }

        static void Set(EditProject project, List<KeyRow> own, List<string> notes, string field, string key, int arg, Func<string, string> value)
        {
            var row = own.FirstOrDefault(r => Is(r, "/\"" + field + "\"/", key) && r.Leaves.Count > arg);
            if (row == null) { notes.Add(field + ": " + key + " 없음"); return; }
            Add(project, row, arg, value(row.Leaves[arg].Text));
        }

        static void Add(EditProject project, KeyRow row, int arg, string newText)
        {
            string old = row.Leaves[arg].Text;
            if (old == newText) return;
            project.Edits.Add(new Edit
            {
                Unit = VisionSet, File = FieldsFile, Path = row.Path, Occurrence = row.Occurrence, Arg = arg, Old = old, New = newText,
                NamedPath = row.RawPath, NamedOccurrence = row.RawOccurrence, Scope = "shared",
            });
        }
    }
}
