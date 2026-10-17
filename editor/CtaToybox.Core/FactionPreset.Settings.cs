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
    }
}
