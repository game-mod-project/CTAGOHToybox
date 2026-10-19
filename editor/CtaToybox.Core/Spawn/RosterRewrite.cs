// 스커미시 구매 창 로스터(스펙 "스커미시: 구매 창 로스터").
// 1. 이긴 판 set/multiplayer/units/<시기>/{units,inf}_<국가>.set의 cd·ci·cp·cost·cw·cool 인자와 {cost N}·{cp N}을 0으로(값 토큰만 바꾼다).
//    차량 비용에는 승무원 단가가 squad_cost_factor로 더해지므로 inf_<국가>.set의 병종 cost도 0이어야 0이 된다(2026-10-17 조사).
// 기본으로 끈다(RosterOptions.Enabled, spawn-menu build --roster로 켠다). 차량 더하기(2)는 --roster-add.
// 2. (AddVehicles, --roster-add) 그 국가 로스터에 없는 전차·포·차량을 국가의 시기 파일마다 새 항목("vehicle2" 매크로)으로 더한다.
// 3. 횟수 제한 {unitLimits "officer=2;..."}의 수를 모두 999로.
// 4. 종류 단추: 구매 창 단추는 budget*.inc의 {buttons "inf1 inf2 empl vehicles tanks"} 순서로 나오고, 그림은 순서 번호 꼴
//    interface/scene/unit_icon/button_<번호>_<국가>_0N.tga다(바닐라 scene.pak). 새 단추 light·medium·heavy·td·air를 tanks 자리에 넣고
//    tanks 그림(5번)을 6번 이후 그림으로 복사한다. 새 단추를 게임이 받는지는 미확인이라 BasicButtons로 끌 수 있다
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.Spawn
{
    public sealed class RosterOptions
    {
        // 구매 창 로스터를 바꾼다(비용 0·횟수 999·종류 단추). 기본으로 끈다(2026-10-19 사용자 결정: 구매 창은 원래대로. 그 전 10-18에는 켰다). --roster로 켠다
        public bool Enabled { get; set; }
        // 로스터에 없던 차량을 더하는가. 게임이 판을 열 때 그 판 국가·시기의 로스터 차량을 모두 미리 읽어서
        // 더한 차량만큼 로딩이 길어진다(2026-10-17 21:59 game.log: eScenePreload 210초 넘게, 미리 읽은 차량 43종 중 38종이 더한 차량)
        public bool AddVehicles { get; set; }
        public bool KeepCost { get; set; }
        public bool BasicButtons { get; set; }
    }

    public static class RosterRewrite
    {
        public static readonly string[] ZeroArgs = { "cd", "ci", "cp", "cost", "cw", "cool" };
        public static readonly string[] ZeroBlocks = { "cost", "cp" };
        public static readonly string[] TankButtons = { "light", "medium", "heavy", "td" };
        public static readonly string[] NewButtons = { "inf1", "inf2", "empl", "vehicles", "light", "medium", "heavy", "td", "air" };

        // 국가별 기본 승무원(조사 집계에서 고른 것. AI 판단): (전차병, 전차장)
        public static readonly Dictionary<string, (string Crew, string Commander)> DefaultCrew = new Dictionary<string, (string, string)>(StringComparer.Ordinal)
        {
            ["ger"] = ("tankman", "tank_commander"), ["rus"] = ("tankman", "tank_commander"), ["usa"] = ("tankman_m3", "tank_com_m3"),
            ["eng"] = ("tankman", "tank_commander_m1928"), ["fin"] = ("tankman", "tankman_vet"), ["jap"] = ("tankman", "tank_commander"),
        };
        public const string GunCrew = "artilleryman";
        public static readonly string[] Periods = { "early", "mid", "late" };

        static readonly Regex InfSettings = new Regex(@"^set/multiplayer/units/settings_inf[^/]*\.set$", RegexOptions.CultureInvariant);
        static readonly Regex Limit = new Regex(@"=\s*\d+", RegexOptions.CultureInvariant);
        static readonly Regex Buttons = new Regex("\\{\\s*buttons\\s+\"([^\"]*)\"\\s*\\}", RegexOptions.CultureInvariant);

        public static List<SpawnFile> Build(SourceStack stack, SpawnCatalog cat, RosterOptions o, List<string> skipped, List<string> notes)
        {
            var files = new List<SpawnFile>();
            var byArmyName = cat.Units.GroupBy(u => u.Army + "|" + u.Name, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var rosterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var texts = new Dictionary<string, (string Text, TextKind Kind, SourceFile Src)>(StringComparer.Ordinal);

            foreach (string file in stack.Paths.Where(p => RosterScan.SkirmishFile(p) != null || RosterScan.InfFile(p) != null || (!o.KeepCost && InfSettings.IsMatch(p))).OrderBy(p => p, StringComparer.Ordinal))
            {
                var src = stack.Find(file);
                TextKind kind;
                string text = TextCodec.Decode(stack.Read(src), out kind);
                texts[file] = (text, kind, src);
                var sk = RosterScan.SkirmishFile(file);
                if (sk == null) continue;
                foreach (var item in RosterScan.Items(file, SdlCore.Parse(text).Root)) if (!item.IsCall) rosterNames.Add(sk.Value.Army + "|" + item.Name);
            }

            foreach (var kv in texts)
            {
                string file = kv.Key, text = kv.Value.Text;
                var parsed = SdlCore.Parse(text);
                var patches = new List<(int Index, int Length, string Text)>();
                // 보병 CP는 병종 틀(settings_inf*.set의 매크로 정의 안 {cp N})에 있다. 그 파일만 정의 안까지 0으로 한다
                if (!o.KeepCost) Zero(parsed.Root, patches, false, InfSettings.IsMatch(file));
                var sk = RosterScan.SkirmishFile(file);
                if (sk != null && !o.BasicButtons)
                    foreach (var item in RosterScan.Items(file, parsed.Root).Where(i => !i.IsCall))
                    {
                        var b = item.Args.FirstOrDefault(a => string.Equals(a.Name, "b", StringComparison.OrdinalIgnoreCase));
                        SpawnUnit unit;
                        if (b?.Value == null || !byArmyName.TryGetValue(sk.Value.Army + "|" + item.Name, out unit)) continue;
                        string want = Button(unit.Category, b.Text);
                        if (want != null && !string.Equals(want, b.Text, StringComparison.OrdinalIgnoreCase)) patches.Add((b.Value.Index, b.Value.Text.Length, want));
                    }
                string next = Patch(text, patches);
                if (sk != null && o.AddVehicles) next += Additions(stack, texts, cat, sk.Value.Army, sk.Value.Period, rosterNames, RosterScan.Items(file, parsed.Root), o, skipped, next.EndsWith("\n") ? "" : "\n");
                if (next == text) continue;
                files.Add(new SpawnFile { Path = "resource/" + file, Bytes = TextCodec.Encode(next, kv.Value.Kind), Source = kv.Value.Src.Layer });
            }

            foreach (string file in stack.Paths.Where(p => p.StartsWith("set/multiplayer/games/presets/", StringComparison.Ordinal) && Regex.IsMatch(p, @"/unitlimits[^/]*\.inc$")).OrderBy(p => p, StringComparer.Ordinal))
            {
                var src = stack.Find(file);
                TextKind kind;
                string text = TextCodec.Decode(stack.Read(src), out kind);
                string next = Regex.Replace(text, "\\{\\s*unitLimits\\s+\"[^\"]*\"", m => Limit.Replace(m.Value, "=999"));
                if (next != text) files.Add(new SpawnFile { Path = "resource/" + file, Bytes = TextCodec.Encode(next, kind), Source = src.Layer });
            }

            if (!o.BasicButtons) files.AddRange(ButtonFiles(stack, notes));
            return files;
        }

        // 그 유닛 종류의 새 단추. 바꾸지 않으면 null
        static string Button(string category, string current)
        {
            if (string.Equals(current, "tanks", StringComparison.OrdinalIgnoreCase) && TankButtons.Contains(category)) return category;
            if (category == "air" && (string.Equals(current, "tanks", StringComparison.OrdinalIgnoreCase) || string.Equals(current, "vehicles", StringComparison.OrdinalIgnoreCase))) return "air";
            if (string.Equals(current, "tanks", StringComparison.OrdinalIgnoreCase)) return "medium";
            return null;
        }

        static string NewButton(SpawnUnit u, RosterOptions o)
        {
            if (u.Category == "empl") return "empl";
            if (u.Category == "car") return "vehicles";
            if (TankButtons.Contains(u.Category)) return o.BasicButtons ? "tanks" : u.Category;
            return null;
        }

        // (define ...) 밖의 cd(…) 등 인자와 {cost N}·{cp N} 블록의 수
        static void Zero(SdlNode node, List<(int, int, string)> patches, bool inDefine, bool cpInDefine)
        {
            if (node.Children == null) return;
            if (node.Kind == "paren" && node.Children.Count > 0 && SdlCore.Unwrap(node.Children[0]).Kind == "atom" && SdlCore.Unwrap(node.Children[0]).Text == "define") inDefine = true;
            if (!inDefine)
            {
                if (node.Kind == "call" && ZeroArgs.Contains(node.Text.ToLowerInvariant()))
                {
                    var v = node.Children.Select(SdlCore.Unwrap).FirstOrDefault();
                    if (v != null && v.Kind == "atom" && SdlCore.IsNumber(v.Text) && !IsZero(v.Text)) patches.Add((v.Index, v.Text.Length, "0"));
                }
            }
            if (node.Kind == "brace" && node.Children.Count == 2 && (!inDefine || cpInDefine))
            {
                var head = SdlCore.Unwrap(node.Children[0]); var v = SdlCore.Unwrap(node.Children[1]);
                bool want = inDefine ? head.Text.Equals("cp", StringComparison.OrdinalIgnoreCase) : ZeroBlocks.Contains(head.Text.ToLowerInvariant());
                if (head.Kind == "atom" && want && v.Kind == "atom" && SdlCore.IsNumber(v.Text) && !IsZero(v.Text)) patches.Add((v.Index, v.Text.Length, "0"));
            }
            foreach (object c in node.Children) Zero(SdlCore.Unwrap(c), patches, inDefine, cpInDefine);
        }

        static bool IsZero(string s) { double d; return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d) && d == 0; }

        static string Patch(string text, List<(int Index, int Length, string Text)> patches)
        {
            if (patches.Count == 0) return text;
            var sb = new StringBuilder(text);
            foreach (var p in patches.OrderByDescending(p => p.Index)) { sb.Remove(p.Index, p.Length); sb.Insert(p.Index, p.Text); }
            return sb.ToString();
        }

        static string Additions(SourceStack stack, Dictionary<string, (string Text, TextKind Kind, SourceFile Src)> texts, SpawnCatalog cat, string army, string period, HashSet<string> rosterNames, List<RosterItem> fileItems, RosterOptions o, List<string> skipped, string lead)
        {
            var sb = new StringBuilder();
            // 그 시기·국가의 병종 로스터 전부(inf_<국가>.set과 모드의 inf_<국가>_*.set)
            string infAll = string.Concat(texts.Where(kv => RosterScan.InfFile(kv.Key) is var fi && fi != null && fi.Value.Period == period && fi.Value.Army == army).Select(kv => kv.Value.Text));
            foreach (var u in cat.Units.Where(u => u.Army == army && !rosterNames.Contains(army + "|" + u.Name)).OrderBy(u => u.Name, StringComparer.Ordinal))
            {
                string button = NewButton(u, o);
                if (button == null) continue;
                // 승무원 후보: 기본 표, 이어서 이 파일의 같은 단추 항목들이 가장 많이 쓰는 승무원 구성. 그 시기에 병종이 다 있는 첫 후보를 쓴다
                var candidates = new List<List<(string Type, int Count)>>();
                (string Crew, string Commander) c;
                if (u.Category == "empl") candidates.Add(new List<(string, int)> { (GunCrew, 2) });
                else if (DefaultCrew.TryGetValue(army, out c)) candidates.Add(new List<(string, int)> { (c.Crew, 1), (c.Commander, 1) });
                string[] sameButtons = u.Category == "empl" ? new[] { "empl" } : u.Category == "car" ? new[] { "vehicles" } : new[] { "tanks", "light", "medium", "heavy", "td" };
                candidates.AddRange(fileItems.Where(i => !i.IsCall && sameButtons.Contains((i.Arg("b") ?? "").ToLowerInvariant()))
                    .Select(i => RosterScan.Members(i, true)).Where(m => m.Count > 0)
                    .GroupBy(m => string.Join(" ", m.Select(x => x.Type + ":" + x.Count))).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).Select(g => g.First()));
                Func<string, bool> valid = type => stack.Find(SpawnCatalog.BreedFile("mp/" + army + "/" + period + "/" + type)) != null && infAll.IndexOf("\"mp/" + army + "/" + period + "/" + type + "\"", StringComparison.OrdinalIgnoreCase) >= 0;
                var crew = candidates.FirstOrDefault(cand => cand.All(x => valid(x.Type)));
                if (crew == null) { skipped.Add(u.Def + ": " + period + " 로스터에 넣지 않았다. 승무원 병종이 없다: " + (candidates.Count == 0 ? "후보 없음" : string.Join(", ", candidates[0].Where(x => !valid(x.Type)).Select(x => x.Type)))); continue; }
                sb.Append("{\"").Append(u.Name).Append("\" (\"vehicle2\" period(").Append(period).Append(") cd(0) ci(0) t() side(").Append(army).Append(")");
                for (int i = 0; i < crew.Count; i++) sb.Append(" crew").Append(i + 1).Append('(').Append(crew[i].Type).Append(':').Append(crew[i].Count).Append(')');
                sb.Append(" g(ctb_spawn) b(").Append(button).Append(") cw(0) cp(0) cost(").Append(o.KeepCost ? "100" : "0").Append("))}\n");
            }
            return sb.Length == 0 ? "" : lead + "; CTAToybox Spawn: 로스터에 없던 유닛\n" + sb;
        }

        // budget*.inc의 단추 목록과 그림. tanks가 든 목록만 바꾼다(컨퀘스트 campaign_capture_the_flag.set은 그대로)
        static IEnumerable<SpawnFile> ButtonFiles(SourceStack stack, List<string> notes)
        {
            foreach (string file in stack.Paths.Where(p => p.StartsWith("set/multiplayer/games/presets/", StringComparison.Ordinal) && Regex.IsMatch(p, @"/budget[^/]*\.inc$")).OrderBy(p => p, StringComparer.Ordinal))
            {
                var src = stack.Find(file);
                TextKind kind;
                string text = TextCodec.Decode(stack.Read(src), out kind);
                string next = Buttons.Replace(text, m => m.Groups[1].Value.Split(' ', '\t').Contains("tanks") ? "{buttons \"" + string.Join(" ", NewButtons) + "\"}" : m.Value);
                if (next != text) yield return new SpawnFile { Path = "resource/" + file, Bytes = TextCodec.Encode(next, kind), Source = src.Layer };
            }
            int tanks = 5;
            foreach (string icon in stack.Paths.Where(p => Regex.IsMatch(p, @"^interface/scene/unit_icon/button_5_[a-z]+_0\d\.tga$")).OrderBy(p => p, StringComparer.Ordinal))
            {
                var src = stack.Find(icon);
                byte[] bytes = stack.Read(src);
                for (int n = tanks + 1; n <= NewButtons.Length; n++)
                {
                    string target = icon.Replace("/button_5_", "/button_" + n + "_");
                    if (stack.Find(target) != null) continue;
                    yield return new SpawnFile { Path = "resource/" + target, Bytes = bytes, Source = src.Layer };
                }
            }
            notes.Add("미확인: 새 단추(light·medium·heavy·td·air)를 게임이 받는지, 그림 순서 번호(button_N)가 단추 순서와 같은지");
        }
    }
}
