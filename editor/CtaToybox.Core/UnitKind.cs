// 가상 경로로 유닛의 종류를 정한다. tools/lib/Common.ps1의 Get-SdlUnitKind와 같은 규칙이어야 한다.
using System;
using System.Linq;

namespace CtaToybox.Core
{
    public static class UnitKind
    {
        public static readonly string[] All = { "vehicle", "breed", "humanskin", "item-entity", "stuff", "pattern" };

        // 편집기가 보이는 종류. 보병 외형(humanskin), 아이템 모델(item-entity), 아이템 틀(pattern)은 스탯을 고칠 대상이 아니라 뺀다.
        // 색인에는 그대로 둔다: 같이 쓰는 유닛 수는 이 종류들까지 세야 '전부' 수정이 어디에 닿는지 맞다
        public static readonly string[] Editor = { "vehicle", "breed", "stuff" };

        public static bool InEditor(string kind) { return kind != null && Editor.Contains(kind); }

        // 화면에 보이는 이름
        public static string Label(string kind)
        {
            switch (kind)
            {
                case "vehicle": return "차량";
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
