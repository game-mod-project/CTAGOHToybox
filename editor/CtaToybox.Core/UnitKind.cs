// 가상 경로로 유닛의 종류를 정한다. tools/lib/Common.ps1의 Get-SdlUnitKind와 같은 규칙이어야 한다.
// 편집기는 그 위에 차량을 탱크·포·차량·기타 탈것으로 나눈 '편집기 종류'를 쓴다(EditorKindOf).
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static class UnitKind
    {
        public static readonly string[] All = { "vehicle", "breed", "humanskin", "item-entity", "stuff", "pattern" };

        // 편집기가 보이는 종류, 목록 순서. 보병 외형(humanskin), 아이템 모델(item-entity), 아이템 틀(pattern)은 스탯을 고칠 대상이 아니라 뺀다.
        // 색인에는 그대로 둔다: 같이 쓰는 유닛 수는 이 종류들까지 세야 '전부' 수정이 어디에 닿는지 맞다
        public static readonly string[] Editor = { "tank", "cannon", "car", "vehicle-other", "breed", "stuff" };

        public static bool InEditor(string editorKind) { return editorKind != null && Editor.Contains(editorKind); }

        // 차량을 나누는 기준: 유닛이 include하는 properties/*.ext. 앞의 규칙이 이긴다.
        // 2026-10-14 데이터에서 _x 파생 파일(properties/vehicle_xx.ext만 읽는다)을 빼면 차량 .def 2개 말고는 모두 이 가운데 하나를 읽는다
        static readonly (string Kind, string[] Ext)[] VehicleRules =
        {
            ("cannon", new[] { "cannon.ext", "cannon_armor.ext", "cannon_tracked.ext", "cannon_armor_tracked.ext", "cannon_mgun.ext" }),
            ("tank", new[] { "tank.ext", "tank_assaultgun.ext", "tank_unarmed.ext" }),
            ("car", new[] { "car.ext", "armoredcar.ext", "halftrack.ext", "moto.ext", "bicycle.ext" }),
        };

        // 편집기 종류. 편집기에 보이지 않는 종류면 null
        public static string EditorKindOf(string kind, IEnumerable<string> includes)
        {
            if (kind == "breed" || kind == "stuff") return kind;
            if (kind != "vehicle") return null;
            var ext = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string f in includes ?? Enumerable.Empty<string>())
                if (f.StartsWith("properties/", StringComparison.OrdinalIgnoreCase) && f.IndexOf('/', "properties/".Length) < 0) ext.Add(f.Substring("properties/".Length));
            foreach (var rule in VehicleRules)
                if (rule.Ext.Any(ext.Contains)) return rule.Kind;
            return "vehicle-other";
        }

        // 화면에 보이는 이름
        public static string Label(string kind)
        {
            switch (kind)
            {
                case "vehicle": return "차량";
                case "tank": return "탱크";
                case "cannon": return "포";
                case "car": return "차량";
                case "vehicle-other": return "기타 탈것 (항공기·배·열차 등)";
                case "breed": return "보병";
                case "stuff": return "무기·탄약·장비";
                default: return "파일";
            }
        }

        // 점으로 시작하는 파일은 이름 전체, 확장자가 없으면 "(no-ext)", 그 밖에는 마지막 확장자
        static string FileKind(string virtualPath)
        {
            string leaf = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
            if (leaf.StartsWith(".")) return leaf;
            int dot = leaf.LastIndexOf('.');
            return dot < 0 ? "(no-ext)" : leaf.Substring(dot);
        }

        // 유닛이 아니면 null
        public static string Of(string virtualPath)
        {
            string kind = FileKind(virtualPath);
            if (virtualPath.StartsWith("set/breed/", StringComparison.Ordinal) && kind == ".set") return "breed";
            if (virtualPath.StartsWith("entity/humanskin/", StringComparison.Ordinal) && kind == ".def") return "humanskin";
            if (virtualPath.StartsWith("entity/-vehicle/", StringComparison.Ordinal) && kind == ".def") return "vehicle";
            if (virtualPath.StartsWith("entity/inventory/", StringComparison.Ordinal) && kind == ".def") return "item-entity";
            if (virtualPath.StartsWith("set/stuff/", StringComparison.Ordinal))
            {
                if (kind == ".pattern") return "pattern";
                // .fsm, .hand와 _settings 폴더는 아이템이 아니라 동작 설정이다
                bool support = kind == ".presets" || kind == ".inc" || kind == ".fsm" || kind == ".hand";
                if (!support && !virtualPath.StartsWith("set/stuff/_settings/", StringComparison.Ordinal)) return "stuff";
            }
            return null;
        }
    }
}
