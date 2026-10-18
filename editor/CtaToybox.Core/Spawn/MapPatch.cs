// 맵 .mi 사본: 지금 계층에서 이긴 판의 바이트 그대로에, 맨 위 {mission ...} 블록의 닫는 괄호 앞에 include 줄만 붙인다(스펙 "붙이는 방식").
// Spawn Menu도 같은 방식이다(spike: 사본 끝 (include "/map/cheat/cheat.mi")). 다른 줄은 바꾸지 않고 맵의 다른 파일은 복사하지 않는다.
// 바이트는 Latin-1로 읽어 글자 하나 = 바이트 하나로 다룬다(인코딩을 몰라도 원문이 그대로 남는다)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.Spawn
{
    public static class MapPatch
    {
        public const string MenuInclude = "(include \"/map/ctatoybox_spawn/menu.mi\")";
        // 컨퀘스트 맵(dcg_vars.inc를 include)에만 넣는 내 번호 추정 조각(MenuBuilder.ConquestFile)
        public const string ConquestFile = "conquest_me.mi";
        public const string ConquestInclude = "(include \"/map/ctatoybox_spawn/" + ConquestFile + "\")";
        public static string NationInclude(string nation) { return "(include \"/map/ctatoybox_spawn/nation_" + nation + ".mi\")"; }
        // 멀티 맵이 읽는 견본 파일. 국가 모드로 나누면 바탕 모드에는 빈 조각, 국가 모드에 진짜 견본이 있다(2026-10-18 국가별 모드 설계)
        public static string TemplateFile(string army) { return "templates_" + army + ".mi"; }
        // 캠페인 맵이 읽는 견본 파일. 나눌 때도 바탕 모드에 진짜 견본으로 있다(캠페인은 국가 모드와 상관없다)
        public static string CampaignTemplateFile(string army) { return "campaign_templates_" + army + ".mi"; }
        public static string TemplateInclude(string army) { return TemplateInclude(army, false); }
        public static string TemplateInclude(string army, bool campaign) { return "(include \"/map/ctatoybox_spawn/" + (campaign ? CampaignTemplateFile(army) : TemplateFile(army)) + "\")"; }
        static readonly Regex MapArmy = new Regex("\"mp/([a-z]+)/", RegexOptions.CultureInvariant);

        // 캠페인 맵에 나오는 국가: 맵 글의 병종 경로 "mp/<국가>/…"와 캠페인 폴더의 국가. 견본 국가 목록(armies)에 있는 것만, 그 순서로
        public static List<string> ArmiesIn(string text, string nation, IList<string> armies)
        {
            var found = new HashSet<string>(MapArmy.Matches(text).Cast<Match>().Select(m => m.Groups[1].Value), StringComparer.Ordinal);
            if (nation != null) found.Add(nation);
            return armies.Where(found.Contains).ToList();
        }
        static readonly string[] Nations = { "rus", "ger", "fin", "usa", "eng", "jap" };
        static readonly Encoding Latin1 = Encoding.Latin1;

        public static bool IsMap(string virtualPath)
        {
            string p = SourceStack.ToVirtual(virtualPath);
            if (!p.EndsWith(".mi", StringComparison.Ordinal)) return false;
            if (p.StartsWith("map/single/", StringComparison.Ordinal)) return p.EndsWith("/0.mi", StringComparison.Ordinal);
            return p.StartsWith("map/multi/", StringComparison.Ordinal);
        }

        // 캠페인 폴더 map/single/<NN-국가…>/의 국가. 모르면 null
        public static string NationOf(string virtualPath)
        {
            var parts = SourceStack.ToVirtual(virtualPath).Split('/');
            if (parts.Length < 4 || parts[1] != "single") return null;
            string folder = parts[2];
            int dash = folder.IndexOf('-');
            if (dash < 0) return null;
            string word = new string(folder.Substring(dash + 1).TakeWhile(char.IsLetter).ToArray());
            return Nations.Contains(word) ? word : null;
        }

        public static byte[] Patch(byte[] original, string nation, out string error) { return Patch(original, nation, null, out error); }

        // 넣는 줄: nation_<국가>.mi(캠페인), templates_<국가>.mi(견본 국가마다), menu.mi 순서
        public static byte[] Patch(byte[] original, string nation, IEnumerable<string> templateArmies, out string error) { return Patch(original, nation, templateArmies, false, out error); }

        // campaignTemplates: 견본 줄을 campaign_templates_<국가>.mi로 넣는다
        public static byte[] Patch(byte[] original, string nation, IEnumerable<string> templateArmies, bool campaignTemplates, out string error)
        {
            error = null;
            string text = Latin1.GetString(original);
            if (text.Contains(MenuInclude)) return original;
            var parsed = SdlCore.Parse(text);
            var top = parsed.Root.Children.Select(SdlCore.Unwrap).Where(n => n.Kind == "brace").ToList();
            var mission = top.Count == 1 && top[0].Children.Count > 0 && SdlCore.Unwrap(top[0].Children[0]).Kind == "atom" && string.Equals(SdlCore.Unwrap(top[0].Children[0]).Text, "mission", StringComparison.OrdinalIgnoreCase) ? top[0] : null;
            if (mission == null) { error = "맨 위 블록이 {mission ...} 하나가 아니다"; return null; }
            if (parsed.Errors.Count > 0) { error = "읽지 못했다: " + parsed.Errors[0]; return null; }
            int close = CloseOf(parsed.Tokens, mission.Index);
            if (close < 0) { error = "{mission의 닫는 괄호를 찾지 못했다"; return null; }
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            var lines = new List<string>();
            if (nation != null) lines.Add(NationInclude(nation));
            if (templateArmies != null) lines.AddRange(templateArmies.Select(a => TemplateInclude(a, campaignTemplates)));
            if (text.IndexOf("dcg_vars.inc", StringComparison.OrdinalIgnoreCase) >= 0) lines.Add(ConquestInclude);
            lines.Add(MenuInclude);
            int lineStart = text.LastIndexOf('\n', close - 1) + 1;
            bool onlySpace = text.Substring(lineStart, close - lineStart).All(ch => ch == ' ' || ch == '\t');
            string insert = onlySpace
                ? string.Concat(lines.Select(l => "\t" + l + nl))
                : nl + string.Concat(lines.Select(l => "\t" + l + nl));
            int at = onlySpace ? lineStart : close;
            return Latin1.GetBytes(text.Substring(0, at) + insert + text.Substring(at));
        }

        static int CloseOf(List<SdlToken> tokens, int openIndex)
        {
            int depth = 0;
            bool started = false;
            foreach (var t in tokens)
            {
                if (!started) { if (t.Kind == "open" && t.Index == openIndex) { started = true; depth = 1; } continue; }
                if (t.Kind == "open") depth++;
                else if (t.Kind == "close" && --depth == 0) return t.Index;
            }
            return -1;
        }

        public static List<SpawnFile> Build(SourceStack stack, List<string> skipped, out int campaign, out int multi)
        {
            int armies;
            return Build(stack, skipped, null, false, out campaign, out multi, out armies);
        }

        // armies: 견본 파일이 있는 국가(null이면 견본 줄을 넣지 않는다). 멀티 맵은 판의 국가를 맵을 만들 때 알 수 없어서 모두 넣는다.
        // 캠페인 맵은 allNations가 아니면 그 맵에 나오는 국가만(못 찾으면 모두). campaignArmies는 캠페인 맵마다 넣은 국가 수의 합
        public static List<SpawnFile> Build(SourceStack stack, List<string> skipped, IList<string> armies, bool allNations, out int campaign, out int multi, out int campaignArmies)
        {
            return Build(stack, skipped, armies, allNations, false, out campaign, out multi, out campaignArmies);
        }

        // campaignFiles: 캠페인 맵은 campaign_templates_<국가>.mi를 읽는다(국가 모드로 나눌 때)
        public static List<SpawnFile> Build(SourceStack stack, List<string> skipped, IList<string> armies, bool allNations, bool campaignFiles, out int campaign, out int multi, out int campaignArmies)
        {
            var files = new List<SpawnFile>();
            campaign = 0; multi = 0; campaignArmies = 0;
            foreach (string path in stack.Paths.Where(IsMap).OrderBy(p => p, StringComparer.Ordinal))
            {
                var src = stack.Find(path);
                byte[] original = stack.Read(src);
                bool single = path.StartsWith("map/single/", StringComparison.Ordinal);
                string nation = NationOf(path);
                IList<string> use = armies;
                if (armies != null && single && !allNations)
                {
                    var inMap = ArmiesIn(Latin1.GetString(original), nation, armies);
                    if (inMap.Count > 0) use = inMap;
                }
                string error;
                var bytes = Patch(original, nation, use, single && campaignFiles, out error);
                if (bytes == null) { skipped.Add(path + " (" + src.Layer + "): " + error); continue; }
                files.Add(new SpawnFile { Path = "resource/" + path, Bytes = bytes, Source = src.Layer });
                if (single) { campaign++; campaignArmies += use?.Count ?? 0; } else multi++;
            }
            return files;
        }
    }
}
