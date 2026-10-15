// 키 의미 주석(data/annotations)을 읽고 찾는다. tools/lib/Annotations.ps1의 Read-Annotations와 같은 형식이다.
// common.psv(경로|형태|등급|의미|출처)와 그룹 전용 <그룹>.psv를 읽고, 같은 키면 그룹 쪽이 이긴다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class KeyNote
    {
        public string Grade { get; set; }
        public string Meaning { get; set; }
        public string Source { get; set; }
    }

    public sealed class KeyAnnotations
    {
        static readonly Dictionary<string, string> SourceNames = new Dictionary<string, string>
        {
            ["C"] = "게임 파일 주석",
            ["S"] = "파일 구조와 값에서 추론",
            ["D"] = "GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인)",
            ["W"] = "Best Way 옛 공식 위키 wiki.bestway.com.ua의 web.archive.org 보존본 (В тылу врага 2 기준, 2026-10-12 확인)",
            ["M"] = "mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인)",
            ["G"] = "Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인)",
        };

        readonly Dictionary<string, KeyNote> map = new Dictionary<string, KeyNote>(StringComparer.Ordinal);

        public int Count { get { return map.Count; } }
        public IEnumerable<KeyValuePair<string, KeyNote>> Entries { get { return map; } }

        // tools/lib/Common.ps1의 Get-SdlFileGroup과 같은 규칙
        public static string GroupOf(string virtualPath)
        {
            if (virtualPath.StartsWith("set/breed/", StringComparison.Ordinal)) return "human";
            if (virtualPath.StartsWith("entity/humanskin/", StringComparison.Ordinal)) return "human";
            if (virtualPath.StartsWith("entity/-vehicle/", StringComparison.Ordinal)) return "vehicle";
            if (virtualPath.StartsWith("set/stuff/", StringComparison.Ordinal)) return "weapon-ammo";
            if (virtualPath.StartsWith("entity/inventory/", StringComparison.Ordinal)) return "inventory";
            return "shared";
        }

        public static KeyAnnotations Load(string annotationDir, string group)
        {
            var result = new KeyAnnotations();
            var files = new List<string> { Path.Combine(annotationDir, "common.psv") };
            if (group != null) files.Add(Path.Combine(annotationDir, group + ".psv"));
            foreach (string file in files)
            {
                if (!File.Exists(file)) continue;
                foreach (string line in File.ReadLines(file, Encoding.UTF8))
                {
                    if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                    string[] p = line.Split('|', 5);
                    if (p.Length < 5) throw new FormatException("주석 줄 형식 오류: " + line);
                    string source;
                    result.map[p[0] + "\t" + p[1]] = new KeyNote { Grade = p[2], Meaning = p[3], Source = SourceNames.TryGetValue(p[4], out source) ? source : p[4] };
                }
            }
            return result;
        }

        // 없으면 null. rawPath를 주면 따옴표 이름 줄은 이름을 넣은 경로의 주석을 먼저 찾는다(KeyPattern.Candidates)
        public KeyNote Find(string path, string form, bool allowWildcard, string rawPath = null)
        {
            foreach (string key in KeyPattern.Candidates(path, rawPath, allowWildcard))
            {
                KeyNote note;
                if (map.TryGetValue(key + "\t" + form, out note)) return note;
            }
            return null;
        }
    }
}
