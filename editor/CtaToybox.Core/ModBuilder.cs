// 프로젝트의 수정을 원본에 적용해 출력 모드 폴더의 resource 아래에 낱개 파일로 쓴다.
// 지우는 것은 이전 빌드의 목록 파일에 적혀 있고 resource 아래에 있는 파일뿐이다.
// 새 파일은 임시 폴더에 다 만든 뒤 옮기므로, 적용 도중에 실패하면 이전 빌드 결과가 남는다.
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
            try
            {
                // 1. 임시 폴더에 고친 파일을 만든다
                foreach (var group in project.Edits.Where(e => e.Enabled).GroupBy(e => SourceStack.ToVirtual(e.File)))
                {
                    var source = stack.Find(group.Key);
                    if (source == null)
                    {
                        foreach (var e in group) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + group.Key });
                        continue;
                    }
                    var results = new List<EditResult>();
                    try
                    {
                        TextKind kind;
                        string text = TextCodec.Decode(stack.Read(source), out kind);
                        string patched = TokenPatcher.Apply(text, group.ToList(), results);
                        report.Results.AddRange(results);
                        int applied = results.Count(r => r.Status == EditStatus.Applied);
                        if (applied == 0) continue;
                        string relative = "resource/" + group.Key;
                        string target = Path.Combine(stage, relative.Replace('/', Path.DirectorySeparatorChar));
                        Directory.CreateDirectory(Path.GetDirectoryName(target));
                        File.WriteAllBytes(target, TextCodec.Encode(patched, kind));
                        manifest.Files.Add(new BuildManifestFile { Path = relative, Layer = source.Layer, Edits = applied });
                    }
                    catch (Exception ex)
                    {
                        report.FileErrors.Add(group.Key + ": " + ex.Message);
                        foreach (var e in group)
                            if (!results.Any(r => r.Edit == e && r.Status != EditStatus.Applied))
                                report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "파일을 만들지 못했다: " + ex.Message });
                    }
                }

                // 2. 이전 빌드의 파일을 지운다
                RemovePrevious(report);

                // 3. 새 파일을 옮기고 목록 파일을 쓴다
                foreach (var f in manifest.Files)
                {
                    string from = Path.Combine(stage, f.Path.Replace('/', Path.DirectorySeparatorChar));
                    string to = Path.Combine(modDir, f.Path.Replace('/', Path.DirectorySeparatorChar));
                    Directory.CreateDirectory(Path.GetDirectoryName(to));
                    File.Copy(from, to, true);
                    report.WrittenFiles.Add(f.Path);
                    report.RemovedFiles.Remove(f.Path);
                }
                Directory.CreateDirectory(modDir);
                File.WriteAllText(Path.Combine(modDir, ManifestName), JsonSerializer.Serialize(manifest, Json) + "\n", new UTF8Encoding(false));
            }
            finally
            {
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
            }
            return report;
        }

        void RemovePrevious(BuildReport report)
        {
            string manifestPath = Path.Combine(modDir, ManifestName);
            if (!File.Exists(manifestPath)) return;
            BuildManifest old;
            try { old = JsonSerializer.Deserialize<BuildManifest>(File.ReadAllText(manifestPath, Encoding.UTF8), Json); }
            catch (JsonException)
            {
                report.Warnings.Add("이전 빌드의 목록 파일을 읽지 못했다. 이전 생성물을 지우지 않는다: " + manifestPath);
                return;
            }
            if (old == null || old.Files == null) return;
            string root = resourceDir + Path.DirectorySeparatorChar;
            foreach (var f in old.Files)
            {
                // 목록에 있어도 출력 모드의 resource 아래가 아니면 지우지 않는다
                string full = f.Path == null || Path.IsPathRooted(f.Path) ? null : Path.GetFullPath(Path.Combine(modDir, f.Path.Replace('/', Path.DirectorySeparatorChar)));
                if (full == null || !full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                {
                    report.Warnings.Add("목록 파일의 항목이 출력 폴더의 resource 밖을 가리킨다. 지우지 않는다: " + f.Path);
                    continue;
                }
                if (!File.Exists(full)) continue;
                File.Delete(full);
                report.RemovedFiles.Add(f.Path);
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
}
