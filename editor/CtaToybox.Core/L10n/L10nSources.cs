// 현지화 층을 읽어 경로마다 원문을 정한다(스펙 "넣는 자리"). 순서: 바닐라 localizations/default.pak → options.set의 모드 순서마다
// <모드>/localizations/default.pak, 이어서 <모드>/localizations/default/**/*.pot(같은 모드 안에서는 낱개가 이긴다). 뒤가 이긴다.
// 출력 모드는 GameProfile이 층에서 이미 뺀다(한국어가 원문 자리에 섞이지 않게). 마운트 순서는 game.log 64·164행(2026-10-17 확인)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.L10n
{
    public sealed class L10nSource
    {
        public string Path { get; set; }      // interface/text/desc/desc_ammo_eng.pot 꼴. 비교는 대소문자 무시
        public string Layer { get; set; }
        public string Text { get; set; }
        public List<string> Shadowed { get; set; } = new List<string>();
    }

    public static class L10nSources
    {
        public static List<L10nSource> Collect(EditorSettings settings, LayerPlan plan, List<string> warnings)
        {
            var byPath = new Dictionary<string, L10nSource>(StringComparer.OrdinalIgnoreCase);
            void Put(string path, string layer, string text)
            {
                path = path.Replace('\\', '/');
                if (path.StartsWith("default/", StringComparison.OrdinalIgnoreCase)) path = path.Substring("default/".Length);
                L10nSource old;
                var src = new L10nSource { Path = path, Layer = layer, Text = text };
                if (byPath.TryGetValue(path, out old)) { src.Shadowed.AddRange(old.Shadowed); src.Shadowed.Add(old.Layer); src.Path = old.Path; }
                byPath[path] = src;
            }
            void Pak(string pak, string layer)
            {
                if (!File.Exists(pak)) return;
                try
                {
                    using (var z = ZipFile.OpenRead(pak))
                        foreach (var e in z.Entries.Where(x => x.FullName.EndsWith(".pot", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase))
                            using (var r = new StreamReader(e.Open(), Encoding.UTF8)) Put(e.FullName, layer, r.ReadToEnd());
                }
                catch (Exception ex) when (ex is InvalidDataException || ex is IOException || ex is UnauthorizedAccessException)
                {
                    warnings?.Add("현지화 pak을 읽지 못해 건너뛰었다: " + pak + ": " + ex.Message);
                }
            }
            Pak(System.IO.Path.Combine(settings.GameDir, "localizations", "default.pak"), "vanilla");
            foreach (var layer in plan.Layers.Where(l => l.Name != "vanilla"))
            {
                string root = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(layer.ResourceDir));
                Pak(System.IO.Path.Combine(root, "localizations", "default.pak"), layer.Name);
                string dir = System.IO.Path.Combine(root, "localizations", "default");
                if (!Directory.Exists(dir)) continue;
                foreach (string f in Directory.EnumerateFiles(dir, "*.pot", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    Put(System.IO.Path.GetRelativePath(dir, f), layer.Name, File.ReadAllText(f, Encoding.UTF8));
            }
            return byPath.Values.OrderBy(s => s.Path, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
