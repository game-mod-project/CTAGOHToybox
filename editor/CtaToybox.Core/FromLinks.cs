// {from "이름" ...}으로 시작하는 아이템 파일이 물려받는 원형 파일을 찾는다(E11).
// 이름 → 파일 규칙은 추정이다(docs/superpowers/specs/2026-10-16-from-inheritance-design.md). 바닐라 {from} 파일 2,565개와 켜 둔 모드까지 3,760개로 쟀다.
// set/stuff 전체에서 아이템 이름(ItemLinks)과 같은 규칙으로 찾는다. 원형(.pattern)과 .hand도 넣는다:
//   1. 파일 이름과 같다(k98)  2. 이름의 낱말과 파일 이름을 점으로 나눈 낱말이 같다(pattern rifle → rifle.pattern, knife pattern → knife.pattern)
//   3. 파일의 마지막 조각(확장자)을 빼면 같다(bazooka_m1 → bazooka_m1.weapon, bulletrus_100 ussr apds → bulletrus_100.ussr.apds.ammo)
// 모드는 다른 폴더의 원형을 가리킨다(MACE body/cap1_mengjiang → head/light_headgear.pattern). 그래서 폴더로 좁히지 않는다.
// 맞는 파일이 여럿이면(모드의 hmg/mgun.pattern과 바닐라 mgun/mgun.pattern, 같은 폴더의 air_bomb.ammo와 air_bomb.pattern)
// 물려받는 파일에서 가장 가까운 조상 폴더의 것, 그다음 물려받는 파일과 확장자가 같은 것, 그다음 이름순 맨 앞을 쓴다. 게임이 고르는 파일은 미확인이다
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class FromLinks
    {
        const string Area = "set/stuff";

        readonly Dictionary<string, List<string>> byLeaf = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> byWords = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> byWordsNoExt = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        static readonly List<string> None = new List<string>();

        public FromLinks(IEnumerable<string> virtualPaths)
        {
            foreach (string p in virtualPaths.Select(SourceStack.ToVirtual).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal))
            {
                if (!p.StartsWith(Area + "/", StringComparison.Ordinal) || p.StartsWith(Area + "/_settings/", StringComparison.Ordinal)) continue;
                string leaf = p.Substring(p.LastIndexOf('/') + 1);
                if (leaf.StartsWith(".")) continue;
                string[] words = leaf.Split('.');
                Add(byLeaf, leaf, p);
                Add(byWords, Key(words), p);
                if (words.Length > 1) Add(byWordsNoExt, Key(words.Take(words.Length - 1)), p);
            }
        }

        static string Key(IEnumerable<string> words) { return string.Join(" ", words.OrderBy(w => w, StringComparer.Ordinal)); }

        static void Add(Dictionary<string, List<string>> map, string key, string file)
        {
            List<string> list;
            if (!map.TryGetValue(key, out list)) { list = new List<string>(); map[key] = list; }
            list.Add(file);
        }

        // 이름에 맞는 파일들(앞의 규칙에 맞는 것이 있으면 뒤의 규칙은 보지 않는다), 이름순. 물려받는 파일 자신은 뺀다. set/stuff 밖의 파일이면 빈 목록
        public List<string> Candidates(string file, string name)
        {
            file = SourceStack.ToVirtual(file);
            if (!file.StartsWith(Area + "/", StringComparison.Ordinal)) return None;
            string lower = (name ?? "").Trim().ToLowerInvariant();
            var words = lower.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return None;
            lower = string.Join(" ", words);
            foreach (var pair in new[] { (byLeaf, lower), (byWords, Key(words)), (byWordsNoExt, Key(words)) })
            {
                List<string> files;
                if (!pair.Item1.TryGetValue(pair.Item2, out files)) continue;
                var found = files.Where(f => f != file).ToList();
                if (found.Count > 0) return found;
            }
            return None;
        }

        // 원형 파일. 찾지 못하거나 set/stuff 밖의 파일이면 null
        public string Find(string file, string name)
        {
            var candidates = Candidates(file, name);
            if (candidates.Count <= 1) return candidates.FirstOrDefault();
            file = SourceStack.ToVirtual(file);
            string leaf = file.Substring(file.LastIndexOf('/') + 1);
            int dot = leaf.LastIndexOf('.');
            string ext = dot > 0 ? leaf.Substring(dot) : null;
            var pool = candidates;
            for (string dir = file.Substring(0, file.LastIndexOf('/')); dir.Length > 0; dir = dir.Contains('/') ? dir.Substring(0, dir.LastIndexOf('/')) : "")
            {
                var here = candidates.Where(c => c.Substring(0, c.LastIndexOf('/')) == dir).ToList();
                if (here.Count > 0) { pool = here; break; }
            }
            return pool.FirstOrDefault(c => ext != null && c.EndsWith(ext, StringComparison.Ordinal)) ?? pool[0];
        }

        // 파일의 맨 위 블록이 {from "이름" ...}이면 그 이름. 아니면 null
        public static string NameOf(List<KeyRow> rows)
        {
            var top = rows.FirstOrDefault(r => r.Path == "/from" && r.Occurrence == 0);
            if (top == null || top.Leaves.Count == 0 || top.Leaves[0].Kind != "str") return null;
            return top.Leaves[0].Text.Trim('"');
        }

        // 원형의 경로를 물려받는 파일의 맨 위 블록 아래로 옮긴다(/bullet/mass → /from/mass). 원형의 맨 위 줄 자체는 null
        public static string Reroot(string path, string top)
        {
            if (path == null) return null;
            int second = path.IndexOf('/', 1);
            return second < 0 ? null : top + path.Substring(second);
        }
    }

    // 사슬의 원형 파일 하나. 그 원형의 줄이 게임에서 쓰이는지는 자식 쪽 파일(유닛 파일과 사슬에서 더 가까운 원형, 그 파일들이 include한 파일)에 같은 경로가 있는지로 정한다.
    // 게임 확인(G16, 2026-10-16): 자식 파일의 같은 키가 이기고, 자식에 없는 키는 원형의 것을 쓴다. 확인한 키(filling)는 한 번만 적는 키였다.
    // 감싼 블록만 자식에 있을 때와, 여러 번 적는 키(tag 등)가 쌓이는지는 확인하지 않았다
    public sealed class FromLevel
    {
        public const string Inherited = "inherited", Overridden = "overridden", Unconfirmed = "unconfirmed";
        public const string UnitScopeNote = "원형에서 물려받은 값이다. '이 유닛만'은 안 된다. '전부'로 고치면 이 원형을 물려받는 유닛이 모두 바뀐다";

        // 다른 유닛이 이 유닛을 원형으로 물려받을 때. 유닛 파일을 고치면(이 유닛만이라도) 물려받는 유닛도 바뀐다(게임 확인 2026-10-14 톰슨)
        public static string HeirsNote(int count)
        {
            return "이 유닛을 원형으로 물려받는 유닛이 " + count + "개 있다. 게임에서 원형의 수정은 그 유닛들에도 닿으므로 '이 유닛만'은 안 된다. '전부'만 된다";
        }

        public string File { get; set; }          // 원형 파일
        public string Name { get; set; }          // 이 파일을 가리킨 {from} 이름
        public List<string> Files { get; set; }   // 원형 파일과 그 파일이 include한 파일(매크로 호출 줄을 찾는 곳)
        public List<string> Candidates { get; set; }   // 이름에 맞는 파일들(FromLinks.Candidates). 둘 이상이면 File은 그 가운데 고른 것이다
        public Dictionary<string, int> Below { get; set; }   // 자식 쪽(유닛과 더 가까운 원형 각각) 가운데 한 단계에서 그 경로가 나온 가장 큰 수(펼친 줄과 매크로 호출 줄, 맨 위 블록은 from)
        public Dictionary<string, int> Own { get; set; }     // 이 원형 파일 쪽(원형과 그 include)의 경로마다 나온 수

        // 줄의 상태. 감싼 블록 때문에 미확인이면 block에 그 블록(맨 위 블록 아래 경로)을 준다. 여러 번 적는 키라서 미확인이면 block은 null이다
        public string StateOf(string path, out string block)
        {
            block = null;
            int below, own;
            if (Below.TryGetValue(path, out below))
            {
                // 이름 있는 부위 블록의 머리 줄({parameters "ap" ...}의 이름): 값은 블록 이름뿐이고 같은 이름의 블록이 자식에도 있다는 뜻이다
                if (path.EndsWith("\"", StringComparison.Ordinal)) { block = path.Substring(path.IndexOf('/', 1) + 1); return Unconfirmed; }
                Own.TryGetValue(path, out own);
                return below > 1 || own > 1 ? Unconfirmed : Overridden;
            }
            for (int i = path.LastIndexOf('/'); i > 0; i = path.LastIndexOf('/', i - 1))
            {
                string prefix = path.Substring(0, i);
                int second = prefix.IndexOf('/', 1);
                if (second < 0) break;   // 맨 위 블록 자체는 늘 같다
                if (Below.ContainsKey(prefix)) { block = prefix.Substring(second + 1); return Unconfirmed; }
            }
            return Inherited;
        }

        // 화면 머리에 보이는 사슬: "원형 사슬: rifle.pattern → bullet.pattern". 맞는 파일이 여럿인 이름은 그 수와 고른 파일을 덧붙인다. 사슬이 없으면 null
        public static string ChainLine(List<FromLevel> chain)
        {
            if (chain == null || chain.Count == 0) return null;
            var line = "원형 사슬: " + string.Join(" → ", chain.Select(l => l.File.Substring(l.File.LastIndexOf('/') + 1)));
            foreach (var l in chain.Where(x => x.Candidates != null && x.Candidates.Count > 1))
                line += "\n'" + l.Name + "'에 맞는 파일 " + l.Candidates.Count + "개(" + string.Join(", ", l.Candidates.Take(4)) + (l.Candidates.Count > 4 ? " 외" : "") + ") 가운데 " + l.File + "을 보인다. 게임이 고르는 파일은 미확인";
            return line;
        }

        public static void Count(Dictionary<string, int> counts, string path)
        {
            int n;
            counts.TryGetValue(path, out n);
            counts[path] = n + 1;
        }

        // 화면에 보이는 상태 글자. 원형의 줄이 아니면 null
        public static string LabelOf(string file, string state, string block)
        {
            if (file == null) return null;
            string leaf = file.Substring(file.LastIndexOf('/') + 1);
            if (state == Overridden) return "원형 " + leaf + "의 값 · 가려짐: 자식 쪽 파일에 같은 키가 있어 게임은 그 값을 쓴다";
            if (state == Unconfirmed && block == null) return "원형 " + leaf + "에서 물려받는다 · 미확인: 자식 쪽 파일에도 같은 키가 있다. 여러 번 적는 키라 쌓이는지 바뀌는지 확인하지 않았다";
            if (state == Unconfirmed) return "원형 " + leaf + "에서 물려받는다 · 미확인: 자식 쪽 파일에도 " + System.Text.RegularExpressions.Regex.Replace(block, "\"([^\"]*)\"", " \"$1\"") + " 블록이 있다. 블록째 바뀌는지 키마다 합쳐지는지 확인하지 않았다";
            return "원형 " + leaf + "에서 물려받는다";
        }
    }
}
