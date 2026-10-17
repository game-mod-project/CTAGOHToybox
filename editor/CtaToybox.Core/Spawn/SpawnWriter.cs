// 스폰 모드 폴더를 갱신한다. 자기 목록(.ctatoybox-spawn.json)에 있는 지난 생성물만 지우고, 목록 밖의 파일은 덮지도 지우지도 않는다.
// 스탯 출력 모드의 목록(.ctatoybox-build.json)과 같은 resource 경로는 쓰지 않는다(두 모드가 같은 경로를 서로 덮지 않게).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core.Spawn
{
    public sealed class SpawnFile
    {
        public string Path { get; set; }     // 모드 폴더 기준. resource/map/… 꼴, '/'
        public byte[] Bytes { get; set; }
        public string Source { get; set; }   // 바탕이 된 계층 이름. 새로 만든 파일이면 null
    }

    public sealed class SpawnWriteReport
    {
        public List<string> Written { get; } = new List<string>();
        public List<string> Unchanged { get; } = new List<string>();
        public List<string> Removed { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
    }

    public static class SpawnWriter
    {
        public const string ManifestName = ".ctatoybox-spawn.json";

        sealed class Entry { public string Path { get; set; } }
        sealed class Manifest { public string BuiltAt { get; set; } public List<Entry> Files { get; set; } = new List<Entry>(); }

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static SpawnWriteReport Write(string modDir, IReadOnlyList<SpawnFile> files, string statModDir)
        {
            var report = new SpawnWriteReport();
            Directory.CreateDirectory(modDir);
            var previous = new HashSet<string>(ReadOwn(modDir, report), StringComparer.OrdinalIgnoreCase);
            var stat = new HashSet<string>(ReadStat(statModDir, report).Select(p => "resource/" + p), StringComparer.OrdinalIgnoreCase);
            var kept = new List<string>();
            foreach (var f in files)
            {
                string rel = f.Path.Replace('\\', '/').TrimStart('/');
                if (rel.Split('/').Contains("..")) { report.Errors.Add(rel + ": 폴더 밖 경로"); continue; }
                if (stat.Contains(rel)) { report.Errors.Add(rel + ": 스탯 출력 모드가 같은 경로를 쓴다. 쓰지 않았다"); continue; }
                string full = Path.Combine(modDir, rel.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(full) && !previous.Contains(rel)) { report.Errors.Add(rel + ": 생성물 목록에 없는 파일이 이미 있다. 덮지 않았다"); continue; }
                kept.Add(rel);
                if (File.Exists(full) && File.ReadAllBytes(full).AsSpan().SequenceEqual(f.Bytes)) { report.Unchanged.Add(rel); continue; }
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(full));
                    File.WriteAllBytes(full, f.Bytes);
                    report.Written.Add(rel);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { report.Errors.Add(rel + ": 쓰지 못했다: " + ex.Message); }
            }
            var now = new HashSet<string>(kept, StringComparer.OrdinalIgnoreCase);
            foreach (string old in previous.Where(p => !now.Contains(p)))
            {
                string full = Path.Combine(modDir, old.Replace('/', Path.DirectorySeparatorChar));
                try { if (File.Exists(full)) { File.Delete(full); report.Removed.Add(old); } }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { report.Errors.Add(old + ": 지우지 못했다: " + ex.Message); kept.Add(old); }
            }
            var manifest = new Manifest { BuiltAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), Files = kept.OrderBy(p => p, StringComparer.Ordinal).Select(p => new Entry { Path = p }).ToList() };
            File.WriteAllText(Path.Combine(modDir, ManifestName), JsonSerializer.Serialize(manifest, Json) + "\n", new UTF8Encoding(false));
            return report;
        }

        public static DateTime? BuiltAt(string modDir)
        {
            string path = Path.Combine(modDir, ManifestName);
            return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : (DateTime?)null;
        }

        static IEnumerable<string> ReadOwn(string modDir, SpawnWriteReport report)
        {
            string path = Path.Combine(modDir, ManifestName);
            if (!File.Exists(path)) return Enumerable.Empty<string>();
            try { return (JsonSerializer.Deserialize<Manifest>(File.ReadAllText(path), Json)?.Files ?? new List<Entry>()).Select(e => e.Path).Where(p => p != null).ToList(); }
            catch (JsonException ex) { report.Errors.Add(ManifestName + ": 읽지 못했다: " + ex.Message); return Enumerable.Empty<string>(); }
        }

        static IEnumerable<string> ReadStat(string statModDir, SpawnWriteReport report)
        {
            if (string.IsNullOrEmpty(statModDir)) return Enumerable.Empty<string>();
            string path = Path.Combine(statModDir, ModBuilder.ManifestName);
            if (!File.Exists(path)) return Enumerable.Empty<string>();
            try
            {
                var m = JsonSerializer.Deserialize<BuildManifest>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (m?.Files ?? new List<BuildManifestFile>()).Select(f => f.Path.Replace('\\', '/')).ToList();
            }
            catch (JsonException ex) { report.Errors.Add(ModBuilder.ManifestName + ": 읽지 못했다: " + ex.Message); return Enumerable.Empty<string>(); }
        }
    }
}
