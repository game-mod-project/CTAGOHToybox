// 팩션 강화 프로젝트 한 벌: 베이스(월드워-차종) 수정을 가져오고 유닛 강화, 재장전·체력, 컨퀘스트 자원을 차례로 더한다
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static partial class FactionPreset
    {
        public static readonly Dictionary<string, string> ArmyNames = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["ger"] = "독일", ["rus"] = "소련", ["usa"] = "미국", ["eng"] = "영국", ["fin"] = "핀란드", ["jap"] = "일본",
        };

        public static EditProject Build(FactionContext c, EditProject baseProject, FactionOptions o, ConquestValues conquest, FactionReport report)
        {
            if (!ArmyIndex.IsArmy(o.Army)) throw new ArgumentException("군대는 " + string.Join(", ", ArmyIndex.Armies) + " 가운데 하나다: " + o.Army);
            var project = new EditProject { Name = "강화-" + ArmyNames[o.Army] };
            // 보급 단계가 매크로 복제를 덧씌운다. 끝나면 스택을 원래대로 둔다(같은 스택으로 다른 검사를 이어 한다)
            var oldOverlay = c.Stack.Overlay;
            try
            {
                if (baseProject != null)
                {
                    var merged = project.MergeFrom(baseProject);
                    report.Edits["베이스 수정"] = merged.Added;
                    if (baseProject.Rules != null && baseProject.Rules.Count > 0) report.Notes.Add("베이스의 일괄 규칙 " + baseProject.Rules.Count + "개는 가져오지 않았다(규칙이 만든 수정도)");
                }
                c.Stack.SetCopies(FileCopy.Pairs(project));
                var members = c.Armies.Members(o.Army);
                report.Units = members.Count;
                report.Infantry = members.Count(m => m.StartsWith("set/breed/", StringComparison.Ordinal));
                report.Vehicles = members.Count(m => m.StartsWith("entity/-vehicle/", StringComparison.Ordinal));
                report.Shared.AddRange(c.Armies.Shared);
                AddUnitStats(project, c, o, report);
                AddWeaponsAndHealth(project, c, o, report);
                AddSupply(project, c, o, report);
                AddConquest(project, c, conquest ?? new ConquestValues(), report);
                AddSkirmish(project, c, (conquest ?? new ConquestValues()).Skirmish, report);
                return project;
            }
            finally { c.Stack.Overlay = oldOverlay; }
        }
    }
}
