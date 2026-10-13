// 프로젝트의 수정을 원본에 적용해 출력 모드 폴더의 resource 아래에 낱개 파일로 쓴다.
// 지우는 것은 이전 빌드의 목록 파일에 적혀 있고 resource 아래에 있는 파일뿐이다. 손으로 넣은 파일은 덮어쓰지도 않는다.
// 새 파일은 임시 폴더에 다 만든 뒤 옮긴다. 옮기기 전에 목록 파일에 이전 생성물과 이번 생성물을 모두 적어 두므로,
// 옮기다 실패해도 다음 빌드가 남은 파일을 지울 수 있다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class BuildReport
    {
        public List<EditResult> Results { get; } = new List<EditResult>();
        public List<string> WrittenFiles { get; } = new List<string>();
        public List<string> RemovedFiles { get; } = new List<string>();
        public List<string> FileErrors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public int Count(EditStatus status) { return Results.Count(r => r.Status == status); }
    }

    // 목록 파일(.ctatoybox-build.json)의 형식
    public sealed class BuildManifestFile { public string Path { get; set; } public string Layer { get; set; } public int Edits { get; set; } }
    public sealed class BuildManifest { public string Project { get; set; } public string BuiltAt { get; set; } public List<BuildManifestFile> Files { get; set; } = new List<BuildManifestFile>(); }

    public sealed class ModBuilder
    {
        public const string ManifestName = ".ctatoybox-build.json";

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        readonly SourceStack stack;
        readonly string modDir, resourceDir;

        public ModBuilder(SourceStack stack, string outputModDir)
        {
            this.stack = stack;
            modDir = Path.GetFullPath(outputModDir);
            resourceDir = Path.Combine(modDir, "resource");
        }

        public BuildReport Build(EditProject project)
        {
            var report = new BuildReport();
            string stage = Path.Combine(Path.GetTempPath(), "ctatoybox-build-" + Guid.NewGuid().ToString("N"));
            var manifest = new BuildManifest { Project = project.Name, BuiltAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") };
            var previous = ReadPrevious(report);
            try
            {
                // 1. 임시 폴더에 고친 파일을 만든다
                foreach (var group in project.Edits.Where(e => e.Enabled).GroupBy(e => SourceStack.ToVirtual(e.File ?? "")))
                {
                    var edits = group.ToList();
                    var source = stack.Find(group.Key);
                    if (source == null)
                    {
                        foreach (var e in edits) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + group.Key });
                        continue;
                    }
                    var results = new List<EditResult>();
                    string error;
                    try { error = StageFile(source, edits, stage, previous, results, manifest); }
                    catch (Exception ex) { error = ex.Message; }
                    if (error != null)
                    {
                        // 파일을 만들지 못했으면 적용으로 세지 않는다. 다른 상태의 결과는 그대로 둔다
                        report.FileErrors.Add(group.Key + ": " + error);
                        foreach (var r in results.Where(x => x.Status == EditStatus.Applied)) { r.Status = EditStatus.Invalid; r.Message = "파일을 만들지 못했다: " + error; }
                        foreach (var e in edits.Where(x => !results.Any(r => r.Edit == x)))
                            results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "파일을 만들지 못했다: " + error });
                    }
                    report.Results.AddRange(results);
                }

                // 2. 이전 생성물과 이번 생성물을 모두 적은 목록 파일을 먼저 쓴다
                var fresh = new HashSet<string>(manifest.Files.Select(x => x.Path), StringComparer.OrdinalIgnoreCase);
                var stale = previous.Values.Where(x => !fresh.Contains(x.Path)).ToList();
                if (!WriteManifest(manifest, manifest.Files.Concat(stale), report)) return report;

                // 3. 새 파일을 옮긴다
                foreach (var f in manifest.Files)
                {
                    try
                    {
                        string to = FullPath(f.Path);
                        Directory.CreateDirectory(Path.GetDirectoryName(to));
                        File.Copy(Path.Combine(stage, f.Path.Replace('/', Path.DirectorySeparatorChar)), to, true);
                        report.WrittenFiles.Add(f.Path);
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        string virtualPath = f.Path.Substring("resource/".Length);
                        report.FileErrors.Add(virtualPath + ": " + ex.Message);
                        foreach (var r in report.Results.Where(x => x.Status == EditStatus.Applied && SourceStack.ToVirtual(x.Edit.File) == virtualPath))
                        { r.Status = EditStatus.Invalid; r.Message = "파일을 쓰지 못했다: " + ex.Message; }
                    }
                }

                // 4. 이번에 만들지 않은 이전 생성물을 지우고 목록 파일을 마무리한다. 지우지 못한 것은 목록에 남긴다
                var kept = new List<BuildManifestFile>(manifest.Files);
                foreach (var f in stale)
                {
                    try { Remove(f.Path, report); }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        report.FileErrors.Add(f.Path + ": 지우지 못했다: " + ex.Message);
                        kept.Add(f);
                    }
                }
                WriteManifest(manifest, kept, report);
            }
            finally
            {
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
            }
            return report;
        }

        // 원본 하나에 수정을 적용해 임시 폴더에 쓴다. 파일을 만들 수 없으면 이유를 돌려준다
        string StageFile(SourceFile source, List<Edit> edits, string stage, Dictionary<string, BuildManifestFile> previous, List<EditResult> results, BuildManifest manifest)
        {
            byte[] original = stack.Read(source);
            TextKind kind;
            string text = TextCodec.Decode(original, out kind);
            if (!original.AsSpan().SequenceEqual(TextCodec.Encode(text, kind)))
                return "원본을 읽고 다시 쓰면 바이트가 달라진다(인코딩이 깨진 파일). 고치지 않는다";

            var usable = new List<Edit>();
            foreach (var e in edits)
            {
                if (e.New != null && !TextCodec.CanEncode(e.New, kind))
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                else usable.Add(e);
            }
            string patched = TokenPatcher.Apply(text, usable, results);
            int applied = results.Count(r => r.Status == EditStatus.Applied);
            if (applied == 0) return null;

            string relative = "resource/" + source.VirtualPath;
            if (FullPath(relative) == null) return "출력 폴더의 resource 밖을 가리키는 경로다";
            if (File.Exists(FullPath(relative)) && !previous.ContainsKey(relative)) return "출력 폴더에 이 도구가 만들지 않은 같은 경로의 파일이 있다. 덮어쓰지 않는다: " + relative;
            string target = Path.Combine(stage, relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.WriteAllBytes(target, TextCodec.Encode(patched, kind));
            manifest.Files.Add(new BuildManifestFile { Path = relative, Layer = source.Layer, Edits = applied });
            return null;
        }

        // 출력 모드 폴더 기준 상대 경로의 전체 경로. resource 아래가 아니면 null
        string FullPath(string relative)
        {
            if (string.IsNullOrEmpty(relative) || Path.IsPathRooted(relative)) return null;
            string full;
            try { full = Path.GetFullPath(Path.Combine(modDir, relative.Replace('/', Path.DirectorySeparatorChar))); }
            catch (ArgumentException) { return null; }
            return full.StartsWith(resourceDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? full : null;
        }

        bool WriteManifest(BuildManifest manifest, IEnumerable<BuildManifestFile> files, BuildReport report)
        {
            var copy = new BuildManifest { Project = manifest.Project, BuiltAt = manifest.BuiltAt, Files = files.ToList() };
            try
            {
                Directory.CreateDirectory(modDir);
                File.WriteAllText(Path.Combine(modDir, ManifestName), JsonSerializer.Serialize(copy, Json) + "\n", new UTF8Encoding(false));
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                report.FileErrors.Add(ManifestName + ": 목록 파일을 쓰지 못했다: " + ex.Message);
                return false;
            }
        }

        // 이전 빌드의 목록 파일에서 지워도 되는 항목(출력 모드의 resource 아래)만 읽는다
        Dictionary<string, BuildManifestFile> ReadPrevious(BuildReport report)
        {
            var result = new Dictionary<string, BuildManifestFile>(StringComparer.OrdinalIgnoreCase);
            string manifestPath = Path.Combine(modDir, ManifestName);
            if (!File.Exists(manifestPath)) return result;
            BuildManifest old;
            try { old = JsonSerializer.Deserialize<BuildManifest>(File.ReadAllText(manifestPath, Encoding.UTF8), Json); }
            catch (JsonException)
            {
                report.Warnings.Add("이전 빌드의 목록 파일을 읽지 못했다. 이전 생성물을 지우지 않는다: " + manifestPath);
                return result;
            }
            if (old == null || old.Files == null) return result;
            foreach (var f in old.Files)
            {
                if (f == null || FullPath(f.Path) == null)
                {
                    report.Warnings.Add("목록 파일의 항목이 출력 폴더의 resource 밖을 가리킨다. 지우지 않는다: " + (f == null ? "null" : f.Path));
                    continue;
                }
                f.Path = f.Path.Replace('\\', '/');
                result[f.Path] = f;
            }
            return result;
        }

        void Remove(string relative, BuildReport report)
        {
            string full = FullPath(relative);
            if (full == null || !File.Exists(full)) return;
            File.Delete(full);
            report.RemovedFiles.Add(relative);
            // 비게 된 폴더를 resource 바로 아래까지 지운다
            string dir = Path.GetDirectoryName(full);
            while (dir.Length > resourceDir.Length && Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
            {
                Directory.Delete(dir);
                dir = Path.GetDirectoryName(dir);
            }
        }
    }
}
