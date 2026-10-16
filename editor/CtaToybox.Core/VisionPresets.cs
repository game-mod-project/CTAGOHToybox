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
