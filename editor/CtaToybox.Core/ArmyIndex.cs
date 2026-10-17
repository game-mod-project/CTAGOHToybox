// 유닛이 어느 군대(플레이할 수 있는 진영) 것인지(팩션 강화, docs/superpowers/specs/2026-10-17-faction-boost-design.md).
// 군대는 set/multiplayer/armies의 6개다. 로스터 파일(set/multiplayer/units/**/inf_<군대>*.set, units_<군대>*.set)에 나오는 side(x)를
// 가장 많이 나오는 군대에 묶는다(eng 파일의 can·pol·fra·aus·nzl은 eng). 보병은 폴더(set/breed/mp|single/<side>/)의 side로,
// 차량은 로스터 항목 이름으로 정하고, 로스터에 없는 차량은 진영 폴더로 정한다.
// 두 군대 이상의 로스터에 이름이 나오는 유닛은 공유로 두고 어느 군대에도 넣지 않는다(고치면 다른 군대도 바뀐다)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class ArmyIndex
    {
        public static readonly string[] Armies = { "ger", "rus", "usa", "eng", "fin", "jap" };

        // 로스터에 없는 차량의 진영 폴더(entity/-vehicle/<폴더>/). 모드는 앞에 +를 붙인다
        static readonly Dictionary<string, string> VehicleFolders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["germany"] = "ger", ["ussr"] = "rus", ["usa"] = "usa", ["eng"] = "eng", ["finland"] = "fin", ["japan"] = "jap",
        };

        static readonly Regex FileArmy = new Regex(@"(?:^|/)(?:inf|units)_(ger|rus|usa|eng|fin|jap)(?:[_.])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        static readonly Regex Side = new Regex(@"\bside\(\s*([a-z]+)\s*\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        static readonly Regex EntryName = new Regex("\\{\\s*\"([^\"]+)\"\\s*\\(", RegexOptions.CultureInvariant);

        readonly Dictionary<string, string> armyOf = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> members = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> sides = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly List<string> shared = new List<string>();

        public IReadOnlyList<string> Shared { get { return shared; } }

        public string ArmyOf(string unitPath)
        {
            string a;
            return unitPath != null && armyOf.TryGetValue(SourceStack.ToVirtual(unitPath), out a) ? a : null;
        }

        public IReadOnlyList<string> Members(string army)
        {
            List<string> list;
            return army != null && members.TryGetValue(army, out list) ? list : new List<string>();
        }

        public IReadOnlyList<string> SidesOf(string army)
        {
            List<string> list;
            return army != null && sides.TryGetValue(army, out list) ? list : new List<string>();
        }

        public static bool IsArmy(string army) { return army != null && Armies.Contains(army); }

        public static ArmyIndex Build(SourceStack stack, IEnumerable<UnitEntry> units)
        {
            var result = new ArmyIndex();
            var sideCounts = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
            var registered = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);   // 로스터 이름 → 군대들
            foreach (string file in stack.Paths.Where(p => p.StartsWith("set/multiplayer/units/", StringComparison.Ordinal) && p.EndsWith(".set", StringComparison.Ordinal)).OrderBy(p => p, StringComparer.Ordinal))
            {
                var m = FileArmy.Match(file);
                if (!m.Success) continue;
                string army = m.Groups[1].Value.ToLowerInvariant();
                string text = stack.ReadText(file);
                if (text == null) continue;
                foreach (Match s in Side.Matches(text))
                {
                    string side = s.Groups[1].Value.ToLowerInvariant();
                    Dictionary<string, int> counts;
                    if (!sideCounts.TryGetValue(side, out counts)) { counts = new Dictionary<string, int>(StringComparer.Ordinal); sideCounts[side] = counts; }
                    counts[army] = (counts.TryGetValue(army, out int n) ? n : 0) + 1;
                }
                foreach (Match e in EntryName.Matches(text))
                {
                    string name = e.Groups[1].Value.ToLowerInvariant();
                    HashSet<string> set;
                    if (!registered.TryGetValue(name, out set)) { set = new HashSet<string>(StringComparer.Ordinal); registered[name] = set; }
                    set.Add(army);
                }
            }
            var sideArmy = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in sideCounts)
                sideArmy[kv.Key] = kv.Value.OrderByDescending(x => x.Value).ThenBy(x => Array.IndexOf(Armies, x.Key)).First().Key;
            foreach (string army in Armies) result.sides[army] = sideArmy.Where(kv => kv.Value == army).Select(kv => kv.Key).OrderBy(x => x, StringComparer.Ordinal).ToList();

            foreach (var u in units)
            {
                string path = SourceStack.ToVirtual(u.Path);
                string own = null;
                var parts = path.Split('/');
                if (path.StartsWith("set/breed/", StringComparison.Ordinal) && parts.Length > 4 && (parts[2] == "mp" || parts[2] == "single"))
                {
                    string a;
                    if (!sideArmy.TryGetValue(parts[3], out a) && IsArmy(parts[3])) a = parts[3];
                    own = a;
                }
                else if (path.StartsWith("entity/-vehicle/", StringComparison.Ordinal) && path.EndsWith(".def", StringComparison.Ordinal) && parts.Length > 3)
                {
                    string a;
                    if (VehicleFolders.TryGetValue(parts[2].TrimStart('+'), out a)) own = a;
                }
                else continue;

                string key = RosterLinks.KeyOf(path);
                HashSet<string> regs;
                var armies = key != null && registered.TryGetValue(key, out regs) ? new HashSet<string>(regs, StringComparer.Ordinal) : new HashSet<string>(StringComparer.Ordinal);
                // 보병은 폴더의 군대를 기본으로 하고, 다른 군대 로스터에도 이름이 있으면 공유다
                if (own != null && path.StartsWith("set/breed/", StringComparison.Ordinal)) armies.Add(own);
                if (armies.Count == 0 && own != null) armies.Add(own);
                if (armies.Count > 1)
                {
                    result.shared.Add(path + ": " + string.Join(", ", armies.OrderBy(x => Array.IndexOf(Armies, x))));
                    continue;
                }
                if (armies.Count == 0) continue;
                string army = armies.First();
                result.armyOf[path] = army;
                List<string> list;
                if (!result.members.TryGetValue(army, out list)) { list = new List<string>(); result.members[army] = list; }
                list.Add(path);
            }
            return result;
        }
    }
}
