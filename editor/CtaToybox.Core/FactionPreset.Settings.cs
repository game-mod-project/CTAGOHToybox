// 팩션 강화: 컨퀘스트 자원(플레이어 몫만, docs/superpowers/specs/2026-10-17-faction-boost-design.md).
// set/dynamic_campaign/resources_{low,standard,high,very_high}.set의 맨 위 블록 아래 Resources와 StageCP·GlobalMaxCP를 고친다. Bots(AI 몫)는 그대로 둔다.
// 파일의 맨 위 블록 이름은 등급마다 다르고 모드가 바꾸기도 한다(MACE Wunderwaffe는 high와 very_high의 이름이 서로 바뀌어 있다). 그래서 맨 위 이름은 보지 않는다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class ConquestValues
    {
        public string MainStart { get; set; } = "99999";   // MP·AP StartVal
        public string SmallStart { get; set; } = "999";    // SP·RP StartVal
        public string Stage { get; set; } = "9999";        // StageCP 값마다
        public string GlobalMax { get; set; } = "9999";    // GlobalMaxCP
        public string MainGain { get; set; } = "1:99999";  // MP·AP WinGain·LoseGain(판 수 : 얻는 양)
        public string SmallGain { get; set; } = "999";     // SP·RP WinGain·LoseGain
        public double Skirmish { get; set; } = 50;         // 스커미시 플레이어 MP 시작·끝·CP 배수 K. 봇 배율은 ÷K(사용자 결정 2026-10-17). 1이면 바꾸지 않는다
    }

    public static partial class FactionPreset
    {
        public static readonly string[] ResourceFiles =
        {
            "set/dynamic_campaign/resources_low.set", "set/dynamic_campaign/resources_standard.set",
            "set/dynamic_campaign/resources_high.set", "set/dynamic_campaign/resources_very_high.set",
        };

        static readonly Regex StartMain = new Regex("^/[^/]+/resources/(mp|ap)/startval$"), StartSmall = new Regex("^/[^/]+/resources/(sp|rp)/startval$"),
            GainMain = new Regex("^/[^/]+/resources/(mp|ap)/(wingain|losegain)$"), GainSmall = new Regex("^/[^/]+/resources/(sp|rp)/(wingain|losegain)$"),
            StageRow = new Regex("^/[^/]+/stagecp$"), GlobalRow = new Regex("^/[^/]+/globalmaxcp$");

        public static void AddConquest(EditProject project, FactionContext c, ConquestValues v, FactionReport report)
        {
            int made = 0;
            foreach (string file in ResourceFiles)
            {
                string text = c.Stack.ReadText(file);
                if (text == null) { report.Skipped.Add(file + ": 파일이 없다"); continue; }
                string error;
                var rows = SourceRows.Of(file, text, out error);
                if (rows == null) { report.Skipped.Add(file + ": 읽지 못했다: " + error); continue; }
                int before = made;
                foreach (var row in rows)
                {
                    string p = row.Path.ToLowerInvariant(), value = null;
                    bool all = false;
                    if (StartMain.IsMatch(p)) value = v.MainStart;
                    else if (StartSmall.IsMatch(p)) value = v.SmallStart;
                    else if (GainMain.IsMatch(p)) value = v.MainGain;
                    else if (GainSmall.IsMatch(p)) value = v.SmallGain;
                    else if (StageRow.IsMatch(p)) { value = v.Stage; all = true; }
                    else if (GlobalRow.IsMatch(p)) value = v.GlobalMax;
                    if (value == null) continue;
                    for (int i = 0; i < (all ? row.Leaves.Count : Math.Min(1, row.Leaves.Count)); i++)
                    {
                        string old = row.Leaves[i].Text;
                        // 문자열이던 자리는 문자열로("1:200 4:250" → "1:99999"), 수였던 자리에 판 수 표를 넣지 않는다
                        string next = old.StartsWith("\"") ? "\"" + value.Trim('"') + "\"" : value.Contains(":") ? value.Substring(value.LastIndexOf(':') + 1) : value;
                        if (next == old) continue;
                        if (project.Edits.Any(e => e.File == file && e.Path == row.Path && e.Occurrence == row.Occurrence && e.Arg == i)) continue;
                        project.Edits.Add(new Edit { Unit = file, File = file, Path = row.Path, Occurrence = row.Occurrence, Arg = i, Old = old, New = next, Scope = "shared" });
                        made++;
                    }
                }
                if (made == before) report.Skipped.Add(file + ": 고칠 자원 줄을 찾지 못했다");
            }
            report.Edits["컨퀘스트 자원(플레이어)"] = made;
        }

        // 스커미시(사용자 결정 2026-10-17): 배틀존·탄약전 프리셋 호출의 start·finish·cp를 ×K, 봇 배율(bots_generic.inc의 mpStart·mpFinish·cp)을 ÷K.
        // 봇 배율 파일 하나를 모든 프리셋이 같이 쓰므로, 고정 최대값 대신 배수로 해야 모든 프리셋에서 봇의 결과 값이 바닐라와 같다.
        // 피난(laststand)은 팀 a("PLAYER DEFENDER TEAM")의 start_a·finish_a·cp_a만 ×K. 프런트라인과 DP(doctrine)는 그대로 둔다
        public const string BotsFile = "set/multiplayer/games/presets/global_parameters/bots_generic.inc";
        static readonly Regex BotRow = new Regex("^/[^/]+/balancingfactorbonus/(mpstart|mpfinish|cp)$");
        static readonly Regex CpPair = new Regex(@"(\d+(?:\.\d+)?):(\d+(?:\.\d+)?)");

        public static void AddSkirmish(EditProject project, FactionContext c, double k, FactionReport report)
        {
            if (k == 1) return;
            int made = 0;
            var presets = c.Stack.Paths.Where(p => p.StartsWith("set/multiplayer/games/presets/", StringComparison.Ordinal) && !p.Contains("/global_parameters/")).OrderBy(p => p, StringComparer.Ordinal).ToList();
            foreach (string file in presets)
            {
                string leaf = file.Substring(file.LastIndexOf('/') + 1);
                string[] names = leaf.StartsWith("preset_") && leaf.EndsWith(".inc") ? new[] { "start", "finish", "cp" }
                    : leaf.StartsWith("laststand_") && leaf.EndsWith(".inc") ? new[] { "start_a", "finish_a", "cp_a" } : null;
                if (names == null) continue;
                made += Scale(project, c, file, (row, i) => row.LeafNames != null && i < row.LeafNames.Count && names.Contains(row.LeafNames[i]), k, report);
            }
            int bots = Scale(project, c, BotsFile, (row, i) => i == 0 && BotRow.IsMatch(row.Path.ToLowerInvariant()), 1 / k, report);
            if (bots == 0) report.Skipped.Add(BotsFile + ": 봇 배율을 찾지 못했다. 스커미시 AI가 바닐라와 달라진다");
            report.Edits["스커미시 플레이어 ×" + Num(k)] = made;
            report.Edits["스커미시 봇 배율 ÷" + Num(k)] = bots;
        }

        static int Scale(EditProject project, FactionContext c, string file, Func<KeyRow, int, bool> pick, double factor, FactionReport report)
        {
            string text = c.Stack.ReadText(file);
            if (text == null) return 0;
            string error;
            var rows = SourceRows.Of(file, text, out error);
            if (rows == null) { report.Skipped.Add(file + ": 읽지 못했다: " + error); return 0; }
            int made = 0;
            foreach (var row in rows)
                for (int i = 0; i < row.Leaves.Count; i++)
                {
                    if (!pick(row, i)) continue;
                    string old = row.Leaves[i].Text, next;
                    if (SdlCore.IsNumber(old)) { string err; next = BulkRules.Compute("mul", old, Num(factor), out err); if (next == null) continue; }
                    else if (CpPair.IsMatch(old)) next = CpPair.Replace(old, m => m.Groups[1].Value + ":" + BulkRules.Compute("mul", m.Groups[2].Value, Num(factor), out string e2));
                    else continue;   // 매크로 정의 안의 %start 같은 자리
                    if (next == old || project.Edits.Any(e => e.File == file && e.Path == row.Path && e.Occurrence == row.Occurrence && e.Arg == i)) continue;
                    project.Edits.Add(new Edit { Unit = file, File = file, Path = row.Path, Occurrence = row.Occurrence, Arg = i, Old = old, New = next, Scope = "shared" });
                    made++;
                }
            return made;
        }
    }
}
