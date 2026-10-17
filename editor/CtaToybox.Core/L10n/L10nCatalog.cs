// 번역 보관 l10n/ko/<원래 경로>.po의 갱신(스펙 "번역 보관"). 원문 .pot마다 .po를 만들거나 고친다:
// 원문 순서대로 내고, 같은 키가 여러 번이면 몇 번째인지로 짝짓는다. 원문이 같으면 번역을 그대로 두고, 바뀌면 #, fuzzy와 #| msgid(예전 원문)를 남긴다.
// 새 키는 msgstr "", 원문에서 사라진 키는 버린다. 원문 경로가 사라진 .po는 지운다(glossary.psv는 건드리지 않는다)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.L10n
{
    public sealed class ExtractReport
    {
        public int Files, Entries, Added, Changed, Removed, Untranslated, Fuzzy;
        public List<string> RemovedFiles { get; } = new List<string>();
    }

    public static class L10nCatalog
    {
        public static string PoPath(string catalogDir, string sourcePath)
        {
            string rel = sourcePath.Replace('/', Path.DirectorySeparatorChar);
            if (rel.EndsWith(".pot", StringComparison.OrdinalIgnoreCase)) rel = rel.Substring(0, rel.Length - 1);
            return Path.Combine(catalogDir, rel);
        }

        static List<(string Key, int Nth, PoEntry Entry)> Numbered(IEnumerable<PoEntry> entries)
        {
            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            var list = new List<(string, int, PoEntry)>();
            foreach (var e in entries)
            {
                int n = seen.TryGetValue(e.Key, out int c) ? c + 1 : 0;
                seen[e.Key] = n;
                list.Add((e.Key, n, e));
            }
            return list;
        }

        public static PoFile Merge(PoFile source, PoFile existing) { int a, c, r; return Merge(source, existing, out a, out c, out r); }

        public static PoFile Merge(PoFile source, PoFile existing, out int added, out int changed, out int removed)
        {
            added = changed = removed = 0;
            var old = new Dictionary<string, PoEntry>(StringComparer.Ordinal);
            if (existing != null) foreach (var (k, n, e) in Numbered(existing.Entries)) old[k + "\u0001" + n] = e;
            var result = new PoFile { Header = PoFile.HeaderWithLanguage(source.Header, "ko"), HeaderFuzzy = false };
            var used = new HashSet<string>(StringComparer.Ordinal);
            foreach (var (k, n, e) in Numbered(source.Entries))
            {
                string id = k + "\u0001" + n;
                var o = old.TryGetValue(id, out var x) ? x : null;
                var entry = new PoEntry { Context = e.Context, Id = e.Id, Comments = new List<string>(e.Comments) };
                if (o == null) { entry.Str = ""; added++; }
                else
                {
                    used.Add(id);
                    entry.Str = o.Str;
                    if (o.Id == e.Id) { entry.Fuzzy = o.Fuzzy; entry.PreviousId = o.PreviousId; }
                    else
                    {
                        entry.Fuzzy = o.Str.Length > 0 || o.Fuzzy;
                        entry.PreviousId = o.Str.Length > 0 || o.Fuzzy ? (o.PreviousId ?? o.Id) : null;
                        changed++;
                    }
                }
                result.Entries.Add(entry);
            }
            removed = old.Keys.Count(k => !used.Contains(k));
            return result;
        }

        public static ExtractReport Extract(List<L10nSource> sources, string catalogDir, string gameVersion)
        {
            var report = new ExtractReport();
            Directory.CreateDirectory(catalogDir);
            var wanted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in sources)
            {
                string path = PoPath(catalogDir, s.Path);
                wanted.Add(Path.GetFullPath(path));
                var source = PoFile.Parse(s.Text);
                var existing = File.Exists(path) ? PoFile.Parse(File.ReadAllText(path, Encoding.UTF8)) : null;
                int a, c, r;
                var merged = Merge(source, existing, out a, out c, out r);
                merged.HeaderComments = new List<string> { "# source: " + s.Layer + " " + s.Path };
                if (!string.IsNullOrEmpty(gameVersion)) merged.HeaderComments.Add("# game-version: " + gameVersion);
                string text = merged.Write();
                if (!File.Exists(path) || File.ReadAllText(path, Encoding.UTF8) != text)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, text, new UTF8Encoding(false));
                }
                report.Files++; report.Entries += merged.Entries.Count; report.Added += a; report.Changed += c; report.Removed += r;
                report.Untranslated += merged.Entries.Count(e => e.Str.Length == 0);
                report.Fuzzy += merged.Entries.Count(e => e.Fuzzy);
            }
            foreach (string po in Directory.EnumerateFiles(catalogDir, "*.po", SearchOption.AllDirectories).ToList())
                if (!wanted.Contains(Path.GetFullPath(po)))
                {
                    File.Delete(po);
                    report.RemovedFiles.Add(Path.GetRelativePath(catalogDir, po).Replace('\\', '/'));
                }
            return report;
        }

        // 보관 전체의 .po(경로는 보관 기준 상대, '/')
        public static IEnumerable<(string Path, PoFile Po)> All(string catalogDir)
        {
            if (!Directory.Exists(catalogDir)) yield break;
            foreach (string f in Directory.EnumerateFiles(catalogDir, "*.po", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                yield return (Path.GetRelativePath(catalogDir, f).Replace('\\', '/'), PoFile.Parse(File.ReadAllText(f, Encoding.UTF8)));
        }
    }
}
