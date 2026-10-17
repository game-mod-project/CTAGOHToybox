// 한글화 빌드(스펙 "도구" l10n build): 원문 .pot마다 출력 모드 localizations/default/<경로>를 쓰고, 글꼴 .font를 resource/interface/font/에 쓴다.
// 원문 구조(항목 순서, 주석, msgctxt)를 지키고 msgid만 번역으로 바꾼다(게임은 msgid를 보인다). 비었거나 fuzzy면 원문 그대로.
// 자기 목록 .ctatoybox-l10n.json으로 지난 생성물만 지운다. 편집기 빌드 목록(.ctatoybox-build.json)의 경로와 목록 밖의 있는 파일은 덮지 않는다
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core.L10n
{
    public sealed class L10nBuildReport
    {
        public int Files, Translated, FellBack;
        public List<string> Written { get; } = new List<string>();
        public List<string> Removed { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
    }

    public static class L10nBuilder
    {
        public const string ManifestName = ".ctatoybox-l10n.json";
        sealed class Entry { public string Path { get; set; } }
        sealed class Manifest { public List<Entry> Files { get; set; } = new List<Entry>(); }
        static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        static List<(string Key, int Nth, PoEntry Entry)> Numbered(IEnumerable<PoEntry> entries)
        {
            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            var list = new List<(string, int, PoEntry)>();
            foreach (var e in entries) { int n = seen.TryGetValue(e.Key, out int c) ? c + 1 : 0; seen[e.Key] = n; list.Add((e.Key, n, e)); }
            return list;
        }

        public static string Translate(PoFile source, PoFile catalog, ref int translated, ref int fellBack)
        {
            var tr = new Dictionary<string, PoEntry>(StringComparer.Ordinal);
            if (catalog != null) foreach (var (k, n, e) in Numbered(catalog.Entries)) tr[k + "\u0001" + n] = e;
            var result = new PoFile { Header = source.Header == null ? null : PoFile.HeaderWithLanguage(source.Header, "ko"), HeaderComments = source.HeaderComments, HeaderFuzzy = source.HeaderFuzzy, Trailing = source.Trailing };
            foreach (var (k, n, e) in Numbered(source.Entries))
            {
                var t = tr.TryGetValue(k + "\u0001" + n, out var x) ? x : null;
                bool use = t != null && t.Str.Length > 0 && !t.Fuzzy && t.Id == e.Id;
                if (use) translated++; else fellBack++;
                result.Entries.Add(new PoEntry { Context = e.Context, Id = use ? t.Str : e.Id, Str = e.Str, Fuzzy = e.Fuzzy, PreviousId = e.PreviousId, Comments = e.Comments });
            }
            return result.Write();
        }

        public static L10nBuildReport Build(List<L10nSource> sources, string catalogDir, List<FontOverride> fonts, string outputModDir)
        {
            var report = new L10nBuildReport();
            var files = new List<(string Rel, string Text)>();
            foreach (var s in sources)
            {
                string po = L10nCatalog.PoPath(catalogDir, s.Path);
                var catalog = File.Exists(po) ? PoFile.Parse(File.ReadAllText(po, Encoding.UTF8)) : null;
                files.Add(("localizations/default/" + s.Path.Replace('\\', '/'), Translate(PoFile.Parse(s.Text), catalog, ref report.Translated, ref report.FellBack)));
                report.Files++;
            }
            foreach (var f in fonts ?? new List<FontOverride>()) files.Add(("resource/" + f.FontPath, f.Text));

            var previous = new HashSet<string>(ReadOwn(outputModDir, report), StringComparer.OrdinalIgnoreCase);
            var editor = new HashSet<string>(ReadEditor(outputModDir, report).Select(p => "resource/" + p), StringComparer.OrdinalIgnoreCase);
            var kept = new List<string>();
            foreach (var (rel, text) in files)
            {
                if (editor.Contains(rel)) { report.Errors.Add(rel + ": 편집기 빌드가 같은 파일을 쓴다. 쓰지 않았다"); continue; }
                string full = Path.Combine(outputModDir, rel.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(full) && !previous.Contains(rel)) { report.Errors.Add(rel + ": 한글화 목록에 없는 파일이 이미 있다. 덮지 않았다"); continue; }
                kept.Add(rel);
                byte[] bytes = new UTF8Encoding(false).GetBytes(text.Replace("\r\n", "\n"));
                if (File.Exists(full) && File.ReadAllBytes(full).AsSpan().SequenceEqual(bytes)) continue;
                Directory.CreateDirectory(Path.GetDirectoryName(full));
                File.WriteAllBytes(full, bytes);
                report.Written.Add(rel);
            }
            var now = new HashSet<string>(kept, StringComparer.OrdinalIgnoreCase);
            foreach (string old in previous.Where(p => !now.Contains(p)))
            {
                string full = Path.Combine(outputModDir, old.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(full)) { File.Delete(full); report.Removed.Add(old); }
            }
            var manifest = new Manifest { Files = kept.OrderBy(p => p, StringComparer.Ordinal).Select(p => new Entry { Path = p }).ToList() };
            File.WriteAllText(Path.Combine(outputModDir, ManifestName), JsonSerializer.Serialize(manifest, Json) + "\n", new UTF8Encoding(false));
            return report;
        }

        static IEnumerable<string> ReadOwn(string modDir, L10nBuildReport report)
        {
            string path = Path.Combine(modDir, ManifestName);
            if (!File.Exists(path)) return Enumerable.Empty<string>();
            try { return (JsonSerializer.Deserialize<Manifest>(File.ReadAllText(path), Json)?.Files ?? new List<Entry>()).Select(e => e.Path).Where(p => p != null).ToList(); }
            catch (JsonException ex) { report.Errors.Add(ManifestName + ": 읽지 못했다: " + ex.Message); return Enumerable.Empty<string>(); }
        }

        static IEnumerable<string> ReadEditor(string modDir, L10nBuildReport report)
        {
            string path = Path.Combine(modDir, ModBuilder.ManifestName);
            if (!File.Exists(path)) return Enumerable.Empty<string>();
            try { return (JsonSerializer.Deserialize<BuildManifest>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Files ?? new List<BuildManifestFile>()).Select(f => f.Path.Replace('\\', '/')).ToList(); }
            catch (JsonException ex) { report.Errors.Add(ModBuilder.ManifestName + ": 읽지 못했다: " + ex.Message); return Enumerable.Empty<string>(); }
        }
    }
}
