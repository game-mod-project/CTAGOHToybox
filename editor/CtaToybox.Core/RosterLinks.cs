// 유닛을 가격·로스터 파일(set/multiplayer/units, set/dynamic_campaign/unit_research_*)의 항목에 잇는다.
// 항목은 따옴표 이름 블록이다: {"panzer5g" ("vehicle2" ... cp(40)) {cost 1220}}, {"mp/ger/mid/nco" ("inf_tier2" ... cost(10.0))}, {"pz35t" requires "panzer2c" costs 1 ...}.
// 항목 이름이 차량 .def의 파일 이름(확장자 뺀 것)이나 breed의 set/breed 아래 경로(.set 뺀 것)와 같으면, 또는 항목 안의 vehicle(이름)이 같으면 그 유닛의 항목이다.
// 2026-10-14 바닐라에서 inf_*.set 항목은 거의 모두(예: conquest/inf_ger.set 331개 가운데 330개), units_*.set 항목도 대부분 이렇게 이어진다. docs/08-unit-files-check.md
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class RosterEntry
    {
        public string File { get; set; }
        public string Name { get; set; }
        public bool IsCall { get; set; }   // 따옴표 이름 블록이 아니라 맨 위의 분대 매크로 호출이다. 이름은 name() 인자에서 온다
        public List<KeyRow> Rows { get; } = new List<KeyRow>();   // 항목 블록 줄과 그 아래 줄. 파일의 원본 그대로(펼치지 않은 것)
    }

    public sealed class RosterLinks
    {
        readonly Dictionary<string, List<RosterEntry>> byKey = new Dictionary<string, List<RosterEntry>>(StringComparer.OrdinalIgnoreCase);

        public int EntryCount { get; private set; }

        public static bool IsRosterFile(string virtualPath)
        {
            if (!virtualPath.EndsWith(".set", StringComparison.OrdinalIgnoreCase)) return false;
            return virtualPath.StartsWith("set/multiplayer/units/", StringComparison.OrdinalIgnoreCase)
                || virtualPath.StartsWith("set/dynamic_campaign/unit_research_", StringComparison.OrdinalIgnoreCase);
        }

        // 유닛이 로스터에서 불리는 이름. 이을 수 없는 종류면 null
        public static string KeyOf(string unit)
        {
            string kind = UnitKind.Of(unit);
            if (kind == "vehicle")
            {
                string leaf = unit.Substring(unit.LastIndexOf('/') + 1);
                return leaf.Substring(0, leaf.Length - ".def".Length);
            }
            if (kind == "breed") return unit.Substring("set/breed/".Length, unit.Length - "set/breed/".Length - ".set".Length);
            return null;
        }

        // readText는 가상 경로의 글자를 준다(없으면 null). 파싱되지 않는 파일은 건너뛴다
        public static RosterLinks Build(IEnumerable<string> virtualPaths, Func<string, string> readText)
        {
            var links = new RosterLinks();
            foreach (string file in virtualPaths.Where(IsRosterFile).OrderBy(p => p, StringComparer.Ordinal))
            {
                string text = readText(file);
                if (text == null) continue;
                var parsed = SdlCore.Parse(text);
                if (parsed.Errors.Count > 0) continue;
                var entries = new List<RosterEntry>();
                RosterEntry entry = null;
                string entryPath = null;
                foreach (var row in KeyLocator.Rows(parsed.Root))
                {
                    if (entry != null && row.Path.StartsWith(entryPath + "/", StringComparison.Ordinal)) { entry.Rows.Add(row); continue; }
                    entry = null;
                    // 매크로 정의 안의 이름 블록과 호출은 틀이지 항목이 아니다
                    if (row.Path.Contains("(define ")) continue;
                    if (IsTopCall(row) && MemberKeys(row).Count > 0) entry = new RosterEntry { File = file, Name = CallName(row), IsCall = true };
                    else if (row.Path.EndsWith("/\"*\"", StringComparison.Ordinal) && row.Leaves.Count > 0 && row.Leaves[0].Kind == "str")
                    {
                        entry = new RosterEntry { File = file, Name = row.Leaves[0].Text.Trim('"') };
                        links.Add(entry.Name, entry);
                    }
                    else continue;
                    entryPath = row.Path;
                    entry.Rows.Add(row);
                    entries.Add(entry);
                    links.EntryCount++;
                }
                // 항목 안의 인자로도 잇는다: vehicle(이름)(배틀존 분대 {"doctrine_pz35t" ("squad_vehicle2_doctrine" ... vehicle(pz35t) ...)}), 분대의 병종·차량 인자
                foreach (var e in entries)
                    foreach (var r in e.Rows)
                    {
                        for (int i = 0; i < r.Leaves.Count; i++)
                            if (string.Equals(r.LeafNames[i], "vehicle", StringComparison.OrdinalIgnoreCase)) links.Add(r.Leaves[i].Text.Trim('"'), e);
                        foreach (string key in MemberKeys(r)) links.Add(key, e);
                    }
            }
            return links;
        }

        // 맨 위(파일 바로 아래)의 매크로 호출: ("squad_with3types_doc" side(ger) name(squad_ersatz_rifle_mid_all) ... c1(ersatz_sl:1))
        static bool IsTopCall(KeyRow row)
        {
            return row.Form == "macro" && row.Path.LastIndexOf('/') == 0;
        }

        static string Arg(KeyRow row, string name)
        {
            for (int i = 0; i < row.Leaves.Count; i++)
                if (string.Equals(row.LeafNames[i], name, StringComparison.OrdinalIgnoreCase)) return row.Leaves[i].Text.Trim('"');
            return null;
        }

        // 분대 틀(set/multiplayer/units/settings.set)은 병종을 content "mp/%side/%period/%c1 ..."(crew·crewN도 같은 꼴)로, 차량을 %vN으로 만든다.
        // 인자 값은 "이름:수"다(c1(ersatz_sl:1)). 2026-10-16 바닐라의 content 틀은 모두 이 꼴이었다
        static readonly Regex MemberArg = new Regex(@"^(c\d+|crew\d*)$", RegexOptions.IgnoreCase);
        static readonly Regex VehicleArg = new Regex(@"^v\d+$", RegexOptions.IgnoreCase);

        static List<string> MemberKeys(KeyRow row)
        {
            var keys = new List<string>();
            if (row.Form != "macro") return keys;
            string side = Arg(row, "side"), period = Arg(row, "period");
            for (int i = 0; i < row.Leaves.Count; i++)
            {
                string name = row.LeafNames[i];
                if (name == null) continue;
                string value = row.Leaves[i].Text.Trim('"');
                int colon = value.IndexOf(':');
                if (colon >= 0) value = value.Substring(0, colon);
                if (value.Length == 0) continue;
                if (MemberArg.IsMatch(name) && side != null && period != null) keys.Add("mp/" + side + "/" + period + "/" + value);
                else if (VehicleArg.IsMatch(name)) keys.Add(value);
            }
            return keys;
        }

        // 분대 호출의 이름: 틀이 만드는 {"%name(%side)"}와 같게. name()이 없으면 매크로 이름
        static string CallName(KeyRow row)
        {
            string name = Arg(row, "name"), side = Arg(row, "side");
            if (name != null) return side == null ? name : name + "(" + side + ")";
            return row.Path.Substring(1).Trim('(', ')').Trim('"');
        }

        void Add(string key, RosterEntry entry)
        {
            List<RosterEntry> list;
            if (!byKey.TryGetValue(key, out list)) { list = new List<RosterEntry>(); byKey[key] = list; }
            if (!list.Contains(entry)) list.Add(entry);
        }

        public IReadOnlyList<RosterEntry> EntriesOf(string unit)
        {
            string key = KeyOf(SourceStack.ToVirtual(unit));
            List<RosterEntry> list;
            return key != null && byKey.TryGetValue(key, out list) ? list : (IReadOnlyList<RosterEntry>)new RosterEntry[0];
        }
    }
}
