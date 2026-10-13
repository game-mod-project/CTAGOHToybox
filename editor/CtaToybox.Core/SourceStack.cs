// 게임이 읽을 파일을 가상 경로로 찾는다. 계층은 로드 순서대로이고 뒤의 계층이 이긴다.
// 한 계층 안에서는 pak을 경로 이름순으로 읽은 뒤 낱개 파일을 읽으므로 낱개 파일이 이긴다.
// (계층 순서와 낱개 파일 우선은 게임에서 확인했다: docs/07-stat-editor/ingame-check.md의 "일괄 확인", 2026-10-13)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class SourceLayer
    {
        public string Name { get; }
        public string ResourceDir { get; }
        public SourceLayer(string name, string resourceDir) { Name = name; ResourceDir = resourceDir; }
    }

    public sealed class SourceFile
    {
        public string VirtualPath { get; set; }
        public string Layer { get; set; }
        public string Container { get; set; }
        public string Entry { get; set; }   // pak 안의 원래 이름. 낱개 파일이면 null
    }

    public sealed class SourceStack : IDisposable
    {
        readonly List<SourceLayer> layers;
        readonly Dictionary<string, List<SourceFile>> index = new Dictionary<string, List<SourceFile>>(StringComparer.Ordinal);
        readonly Dictionary<string, ZipArchive> open = new Dictionary<string, ZipArchive>(StringComparer.OrdinalIgnoreCase);
        static readonly IReadOnlyList<SourceFile> None = new SourceFile[0];

        public SourceStack(IEnumerable<SourceLayer> layersInLoadOrder) { layers = layersInLoadOrder.ToList(); }

        public IReadOnlyList<SourceLayer> Layers { get { return layers; } }
        public IEnumerable<string> Paths { get { return index.Keys; } }

        public static string ToVirtual(string path)
        {
            return path.Replace('\\', '/').TrimStart('/').ToLowerInvariant();
        }

        public void Scan()
        {
            index.Clear();
            foreach (var layer in layers)
            {
                if (!Directory.Exists(layer.ResourceDir)) continue;
                string root = Path.GetFullPath(layer.ResourceDir);
                var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
                foreach (string pak in files.Where(f => f.EndsWith(".pak", StringComparison.OrdinalIgnoreCase)))
                {
                    string dir = Path.GetDirectoryName(Path.GetRelativePath(root, pak));
                    string prefix = string.IsNullOrEmpty(dir) ? "" : ToVirtual(dir) + "/";
                    foreach (var e in Archive(pak).Entries)
                    {
                        if (e.FullName.EndsWith("/") || e.FullName.EndsWith("\\")) continue;
                        // 폴더 밖을 가리키는 이름(.. 조각, 드라이브 문자)은 파일로 치지 않는다
                        if (e.FullName.Contains(':') || e.FullName.Split('/', '\\').Contains("..")) continue;
                        Add(new SourceFile { VirtualPath = prefix + ToVirtual(e.FullName), Layer = layer.Name, Container = pak, Entry = e.FullName });
                    }
                }
                foreach (string f in files.Where(f => !f.EndsWith(".pak", StringComparison.OrdinalIgnoreCase)))
                    Add(new SourceFile { VirtualPath = ToVirtual(Path.GetRelativePath(root, f)), Layer = layer.Name, Container = f, Entry = null });
            }
        }

        void Add(SourceFile f)
        {
            List<SourceFile> list;
            if (!index.TryGetValue(f.VirtualPath, out list)) { list = new List<SourceFile>(); index[f.VirtualPath] = list; }
            list.Add(f);
        }

        ZipArchive Archive(string pak)
        {
            ZipArchive z;
            if (!open.TryGetValue(pak, out z)) { z = ZipFile.OpenRead(pak); open[pak] = z; }
            return z;
        }

        public IReadOnlyList<SourceFile> Versions(string virtualPath)
        {
            List<SourceFile> list;
            return index.TryGetValue(ToVirtual(virtualPath), out list) ? list : None;
        }

        public SourceFile Find(string virtualPath)
        {
            var v = Versions(virtualPath);
            return v.Count == 0 ? null : v[v.Count - 1];
        }

        public byte[] Read(SourceFile file)
        {
            if (file.Entry == null) return File.ReadAllBytes(file.Container);
            var entry = Archive(file.Container).GetEntry(file.Entry);
            using (var s = entry.Open())
            using (var m = new MemoryStream())
            {
                s.CopyTo(m);
                return m.ToArray();
            }
        }

        // 지금 게임이 읽을 그 파일의 텍스트. 없으면 null
        public string ReadText(string virtualPath)
        {
            var file = Find(virtualPath);
            if (file == null) return null;
            TextKind kind;
            return TextCodec.Decode(Read(file), out kind);
        }

        public void Dispose()
        {
            foreach (var z in open.Values) z.Dispose();
            open.Clear();
        }
    }
}
