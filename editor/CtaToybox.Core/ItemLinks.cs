// 유닛이 이름으로 가리키는 아이템(무기, 탄약 등)을 set/stuff의 파일로 잇는다.
// 규칙은 2026-10-14의 데이터 조사에서 나왔다(docs/02-modding-research.md의 "아이템 이름과 파일"):
//   1. 파일 이름과 같다  2. 이름의 낱말과 파일 이름을 점으로 나눈 낱말이 같다  3. 파일의 마지막 조각(확장자)을 빼면 같다
// 세 규칙으로 파일이 정확히 하나일 때만 이어졌다고 본다. 세 규칙에 맞는 파일이 없으면 이름의 낱말을 모두 가진 파일을 후보로 모으고,
// 후보가 한 폴더에 있으면 이름순 맨 앞 파일을 게임이 고르는 파일로 본다(G5). 나머지는 후보로만 보인다(미확인).
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class ItemLink
    {
        public string Name { get; set; }
        public string Rule { get; set; }
        public List<string> Files { get; set; } = new List<string>();
    }

    public sealed class ItemLinks
    {
        public const string RuleName = "이름", RuleWords = "낱말", RuleWordsNoExt = "확장자 뺀 낱말", RuleFirstByName = "후보 중 이름순 맨 앞", RuleCandidates = "후보(미확인)", RuleNone = "없음";

        readonly Dictionary<string, List<string>> byLeaf = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> byWords = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> byWordsNoExt = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly List<KeyValuePair<string, HashSet<string>>> all = new List<KeyValuePair<string, HashSet<string>>>();

        // tools/research/Measure-ItemNameLinks.ps1과 같은 범위: set/stuff 아래, _settings와 설정·틀 파일 제외
        public static bool IsItemFile(string virtualPath)
        {
            if (!virtualPath.StartsWith("set/stuff/", StringComparison.Ordinal) || virtualPath.StartsWith("set/stuff/_settings/", StringComparison.Ordinal)) return false;
            string leaf = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
            if (leaf.StartsWith(".")) return false;
            foreach (string ext in new[] { ".pattern", ".inc", ".fsm", ".hand", ".presets" })
                if (leaf.EndsWith(ext, StringComparison.Ordinal)) return false;
            return true;
        }

        static string Key(IEnumerable<string> words) { return string.Join(" ", words.OrderBy(w => w, StringComparer.Ordinal)); }

        static void Add(Dictionary<string, List<string>> map, string key, string file)
        {
            List<string> list;
            if (!map.TryGetValue(key, out list)) { list = new List<string>(); map[key] = list; }
            list.Add(file);
        }

        public static ItemLinks Build(IEnumerable<string> virtualPaths)
        {
            var links = new ItemLinks();
            foreach (string p in virtualPaths.Where(IsItemFile).OrderBy(x => x, StringComparer.Ordinal))
            {
                string leaf = p.Substring(p.LastIndexOf('/') + 1);
                string[] words = leaf.Split('.');
                Add(links.byLeaf, leaf, p);
                Add(links.byWords, Key(words), p);
                if (words.Length > 1) Add(links.byWordsNoExt, Key(words.Take(words.Length - 1)), p);
                links.all.Add(new KeyValuePair<string, HashSet<string>>(p, new HashSet<string>(words, StringComparer.Ordinal)));
            }
            return links;
        }

        public ItemLink Find(string name)
        {
            string lower = name.Trim().ToLowerInvariant();
            string[] words = lower.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string key = Key(words);
            var rules = new[]
            {
                new KeyValuePair<string, Dictionary<string, List<string>>>(RuleName, byLeaf),
                new KeyValuePair<string, Dictionary<string, List<string>>>(RuleWords, byWords),
                new KeyValuePair<string, Dictionary<string, List<string>>>(RuleWordsNoExt, byWordsNoExt),
            };
            foreach (var rule in rules)
            {
                List<string> files;
                if (!rule.Value.TryGetValue(rule.Key == RuleName ? lower : key, out files)) continue;
                return new ItemLink { Name = lower, Rule = files.Count == 1 ? rule.Key : RuleCandidates, Files = new List<string>(files) };
            }
            // 낱말을 모두 가진 파일들. 게임은 같은 폴더의 후보 가운데 이름순(점이 글자보다 앞) 맨 앞 파일을 고른다(G5, 2026-10-16 게임 확인).
            // 여러 폴더에 걸친 후보는 확인하지 않았다
            var candidates = all.Where(f => words.All(f.Value.Contains)).Select(f => f.Key).OrderBy(f => f, StringComparer.Ordinal).ToList();
            if (candidates.Count == 0) return new ItemLink { Name = lower, Rule = RuleNone, Files = candidates };
            bool oneFolder = candidates.Select(f => f.Substring(0, f.LastIndexOf('/'))).Distinct(StringComparer.Ordinal).Count() == 1;
            return new ItemLink { Name = lower, Rule = oneFolder ? RuleFirstByName : RuleCandidates, Files = candidates };
        }

        // 펼친 노드에서 아이템 이름을 모은다: item·weapon 행의 첫 문자열과 filling 뒤의 문자열
        public static List<string> ReferencedNames(IEnumerable<object> expandedNodes)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var row in SdlCore.KeyRows(expandedNodes.ToList(), new[] { "extender" }))
            {
                string seg = row.Path.Substring(row.Path.LastIndexOf('/') + 1);
                var args = row.ArgNodes;
                for (int i = 0; i < args.Count; i++)
                {
                    bool take = args[i].Kind == "str" &&
                        ((i == 0 && (seg == "item" || seg == "weapon")) || (i > 0 && args[i - 1].Kind == "atom" && args[i - 1].Text == "filling"));
                    if (!take) continue;
                    string name = args[i].Text.Trim('"').ToLowerInvariant();
                    if (seen.Add(name)) result.Add(name);
                }
            }
            return result;
        }
    }
}
