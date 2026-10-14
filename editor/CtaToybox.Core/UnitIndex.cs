// 유닛 목록과 유닛마다 읽는 파일(include와 .presets). 값이 몇 개 유닛에 공유되는지 세는 데 쓴다.
// 유닛 전체를 펼치는 데 1분 넘게 걸리므로 결과를 파일로 캐시한다. 원본 구성이 바뀌면 다시 만든다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class UnitEntry
    {
        public string Path { get; set; }
        public string Kind { get; set; }
        public string Layer { get; set; }
        public List<string> Includes { get; set; } = new List<string>();
        public List<string> Presets { get; set; } = new List<string>();
        public int Unresolved { get; set; }
        public int Numbers { get; set; }    // 펼친 값 가운데 수의 개수. 0이면 고칠 스탯이 없는 파일이라 편집기 목록에서 뺀다
    }

    public sealed class UnitIndex
    {
        public string Signature { get; set; }
        public List<UnitEntry> Units { get; set; } = new List<UnitEntry>();

        static readonly IReadOnlyList<string> None = new string[0];
        Dictionary<string, List<string>> users;

        public static UnitIndex Build(SourceStack stack, Action<int, int> progress)
        {
            var paths = stack.Paths.Where(p => UnitKind.Of(p) != null).OrderBy(p => p, StringComparer.Ordinal).ToList();
            var resolver = new SdlResolver(stack.ReadText);
            var index = new UnitIndex { Signature = SignatureOf(stack) };
            for (int i = 0; i < paths.Count; i++)
            {
                var r = resolver.Resolve(paths[i]);
                index.Units.Add(new UnitEntry
                {
                    Path = paths[i], Kind = UnitKind.Of(paths[i]), Layer = stack.Find(paths[i]).Layer,
                    Includes = r.Includes, Presets = r.Presets, Unresolved = r.Unresolved.Count, Numbers = CountNumbers(r.Nodes)
                });
                if (progress != null && (i + 1) % 1000 == 0) progress(i + 1, paths.Count);
            }
            return index;
        }

        // 펼친 트리에서 키 행의 값 가운데 수인 것. 편집기 가운데 창이 보이는 값과 같은 범위다(KeyLocator 행의 잎)
        public static int CountNumbers(List<object> nodes)
        {
            var root = new SdlNode("root", "", 0, true);
            root.Children.AddRange(nodes);
            int count = 0;
            foreach (var row in KeyLocator.Rows(root))
                foreach (var leaf in row.Leaves) if (leaf.Kind == "atom" && SdlCore.IsNumber(leaf.Text)) count++;
            return count;
        }

        // 색인 형식이 바뀌면 올린다. 지문에 들어가 옛 캐시를 다시 만들게 한다
        const string FormatVersion = "2";

        // 계층의 구성과 원본 파일(pak, 낱개 파일)의 크기·수정 시각으로 만든 지문
        public static string SignatureOf(SourceStack stack)
        {
            var containers = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string p in stack.Paths)
                foreach (var v in stack.Versions(p)) containers.Add(v.Container);
            var sb = new StringBuilder("format " + FormatVersion + "\n");
            foreach (var l in stack.Layers) sb.Append(l.Name).Append('|').Append(l.ResourceDir).Append('\n');
            foreach (string c in containers)
            {
                var info = new FileInfo(c);
                sb.Append(c).Append('|').Append(info.Length).Append('|').Append(info.LastWriteTimeUtc.Ticks).Append('\n');
            }
            using (var sha = SHA1.Create()) return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
        }

        public static UnitIndex LoadOrBuild(SourceStack stack, string cachePath, Action<int, int> progress, out bool rebuilt)
        {
            string signature = SignatureOf(stack);
            if (File.Exists(cachePath))
            {
                try
                {
                    var cached = JsonSerializer.Deserialize<UnitIndex>(File.ReadAllText(cachePath, Encoding.UTF8));
                    if (cached != null && cached.Units != null && cached.Signature == signature) { rebuilt = false; return cached; }
                }
                catch (JsonException) { }   // 깨진 캐시는 다시 만든다
            }
            var built = Build(stack, progress);
            built.Save(cachePath);
            rebuilt = true;
            return built;
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            File.WriteAllText(path, JsonSerializer.Serialize(this), new UTF8Encoding(false));
        }

        // 그 파일을 읽는 유닛: 자기 자신, include, .presets
        public IReadOnlyList<string> UsersOf(string file)
        {
            if (users == null)
            {
                var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
                foreach (var u in Units)
                    foreach (string f in new[] { u.Path }.Concat(u.Includes).Concat(u.Presets).Distinct())
                    {
                        List<string> list;
                        if (!map.TryGetValue(f, out list)) { list = new List<string>(); map[f] = list; }
                        list.Add(u.Path);
                    }
                users = map;
            }
            List<string> found;
            return users.TryGetValue(SourceStack.ToVirtual(file), out found) ? found : None;
        }

        public IEnumerable<UnitEntry> Search(string text, string kind, string layer, DisplayNames names)
        {
            foreach (var u in Units)
            {
                if (kind != null && !string.Equals(u.Kind, kind, StringComparison.OrdinalIgnoreCase)) continue;
                if (layer != null && !string.Equals(u.Layer, layer, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrEmpty(text))
                {
                    string name = names == null ? null : names.OfUnit(u.Path);
                    bool hit = u.Path.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                        || (name != null && name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!hit) continue;
                }
                yield return u;
            }
        }
    }
}
