// 스폰 메뉴·로스터에 넣을 유닛 목록(스펙 "소환할 유닛 고르기").
// 차량: 편집기 종류 tank·cannon·car 정의, 항공기(airborne.ext), 그리고 vehicle-other 가운데 스커미시 로스터에 이름이 나오는 정의.
// 국가는 ArmyIndex(로스터 이름, 없으면 진영 폴더)로 정하고, 정하지 못하거나 두 국가에 걸친 유닛은 넣지 않고 건너뛴 목록에 적는다.
// 보병: 스커미시 로스터의 b(inf1)·b(inf2) 항목 그대로(사용자 결정: 분대 정보는 로스터 것을 그대로). 구성원 병종이 정의로 풀리지 않으면 건너뛴다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.Spawn
{
    public sealed class SpawnUnit
    {
        public string Def { get; set; }
        public string Name { get; set; }
        public string Army { get; set; }
        public string Category { get; set; }
        public string Family { get; set; }
        public string EditorKind { get; set; }
        public RosterItem Roster { get; set; }   // 그 국가 로스터의 첫 항목. 없으면 null
    }

    public sealed class SpawnSquad
    {
        public string Army { get; set; }
        public string Period { get; set; }
        public string Name { get; set; }
        public string Side { get; set; }
        public string Button { get; set; }
        public string File { get; set; }
        public List<(string Breed, int Count)> Members { get; set; } = new List<(string, int)>();
        public string MenuName { get { return Name + "(" + Side + ")"; } }   // 틀이 만드는 이름(desc/squad/<이름>(<side>))
    }

    public sealed class SpawnCatalog
    {
        public List<SpawnUnit> Units { get; } = new List<SpawnUnit>();
        public List<SpawnSquad> Squads { get; } = new List<SpawnSquad>();
        public List<string> RosterFiles { get; } = new List<string>();

        static readonly string[] VehicleKinds = { "tank", "cannon", "car" };
        static readonly Regex FamilyPart = new Regex("[a-z]+", RegexOptions.CultureInvariant);

        public static string BreedFile(string breed) { return "set/breed/" + breed + ".set"; }

        public static string FamilyOf(string def)
        {
            var parts = SourceStack.ToVirtual(def).Split('/');
            string folder = parts.Length >= 2 ? parts[parts.Length - 2] : parts[0];
            var m = FamilyPart.Match(folder);
            return m.Success ? m.Value : folder;
        }

        public static SpawnCatalog Build(SourceStack stack, UnitIndex index, ArmyIndex armies, SpawnCategories categories, List<string> skipped)
        {
            var cat = new SpawnCatalog();
            // 국가 로스터 항목: 이름 → (국가, 항목). 같은 국가의 여러 시기에 있으면 처음 것
            var byName = new Dictionary<string, Dictionary<string, RosterItem>>(StringComparer.OrdinalIgnoreCase);
            var squadKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string file in stack.Paths.Where(p => RosterScan.SkirmishFile(p) != null).OrderBy(p => p, StringComparer.Ordinal))
            {
                var fa = RosterScan.SkirmishFile(file).Value;
                string text = stack.ReadText(file);
                if (text == null) continue;
                var parsed = SdlCore.Parse(text);
                // 바닐라 early/units_eng.set처럼 남는 닫는 괄호가 있어도 파서는 그것만 버리고 나머지를 읽는다. 알리기만 한다
                if (parsed.Errors.Count > 0) skipped.Add(file + ": 로스터 문법 경고(나머지는 읽었다): " + parsed.Errors[0]);
                cat.RosterFiles.Add(file);
                foreach (var item in RosterScan.Items(file, parsed.Root))
                {
                    string button = item.Arg("b");
                    if (item.IsCall && (string.Equals(button, "inf1", StringComparison.OrdinalIgnoreCase) || string.Equals(button, "inf2", StringComparison.OrdinalIgnoreCase)))
                    {
                        string side = item.Arg("side") ?? fa.Army, period = item.Arg("period") ?? fa.Period;
                        var sq = new SpawnSquad { Army = fa.Army, Period = period, Name = item.Name, Side = side, Button = button.ToLowerInvariant(), File = file };
                        foreach (var m in RosterScan.Members(item, false)) sq.Members.Add(("mp/" + side + "/" + period + "/" + m.Type, m.Count));
                        string key = sq.Army + "|" + sq.Period + "|" + sq.MenuName;
                        if (!squadKeys.Add(key)) continue;
                        var missing = sq.Members.Where(m => stack.Find(BreedFile(m.Breed)) == null).Select(m => m.Breed).ToList();
                        if (sq.Members.Count == 0) skipped.Add(file + " " + sq.MenuName + ": 분대 구성원이 없다");
                        else if (missing.Count > 0) skipped.Add(file + " " + sq.MenuName + ": 병종 정의가 없다: " + string.Join(", ", missing));
                        else cat.Squads.Add(sq);
                        continue;
                    }
                    if (item.IsCall) continue;
                    Dictionary<string, RosterItem> armiesOf;
                    if (!byName.TryGetValue(item.Name, out armiesOf)) { armiesOf = new Dictionary<string, RosterItem>(StringComparer.Ordinal); byName[item.Name] = armiesOf; }
                    if (!armiesOf.ContainsKey(fa.Army)) armiesOf[fa.Army] = item;
                }
            }

            var shared = new HashSet<string>(armies.Shared.Select(s => s.Substring(0, s.IndexOf(':') < 0 ? s.Length : s.IndexOf(':'))), StringComparer.Ordinal);
            foreach (var u in index.Units.OrderBy(u => u.Path, StringComparer.Ordinal))
            {
                string path = SourceStack.ToVirtual(u.Path);
                if (!path.EndsWith(".def", StringComparison.Ordinal)) continue;
                string name = path.Substring(path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - ".def".Length);
                Dictionary<string, RosterItem> inRoster;
                byName.TryGetValue(name, out inRoster);
                bool vehicle = VehicleKinds.Contains(u.EditorKind);
                // 항공기는 properties/airborne.ext를 읽는 정의다(2026-10-17 색인: entity/-vehicle/*/airborne/). x 폴더는 잔해다
                bool air = u.EditorKind == "vehicle-other" && u.Includes.Contains("properties/airborne.ext") && !path.Split('/').Contains("x");
                if (!vehicle && !air && !(u.EditorKind == "vehicle-other" && inRoster != null)) continue;
                if (shared.Contains(path)) { skipped.Add(path + ": 두 국가 이상의 로스터에 있다"); continue; }
                string army = armies.ArmyOf(path);
                if (army == null && inRoster != null && inRoster.Count == 1) army = inRoster.Keys.First();
                if (army == null) { skipped.Add(path + ": 국가를 정하지 못했다"); continue; }
                RosterItem roster = null;
                if (inRoster != null) inRoster.TryGetValue(army, out roster);
                cat.Units.Add(new SpawnUnit
                {
                    Def = path, Name = name, Army = army, EditorKind = u.EditorKind, Roster = roster,
                    Category = categories.Of(path, roster?.Arg("b"), u.EditorKind),
                    Family = FamilyOf(path),
                });
            }
            return cat;
        }
    }
}
