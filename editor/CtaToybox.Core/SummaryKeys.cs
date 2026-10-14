// 요약 탭에 올릴 키와 그 이름. editor/summary/<종류>.psv(경로|이름)가 있으면 그것을 쓰고,
// 없으면 키 주석에서 등급이 "확인"인 키를 쓴다(스펙: 첫 버전은 common.psv의 "확인" 키로 시작한다).
// 경로는 주석과 같은 방식으로 맞춘다(정확한 경로, **/꼬리, */이름).
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class SummaryKeys
    {
        const int MaxLabel = 30;
        readonly Dictionary<string, string> labels = new Dictionary<string, string>(StringComparer.Ordinal);

        public string SourceFile { get; private set; }
        public int Count { get { return labels.Count; } }

        public static string ShortLabel(string meaning)
        {
            string s = meaning ?? "";
            foreach (string stop in new[] { ". ", ", ", " (", "(" })
            {
                int i = s.IndexOf(stop, StringComparison.Ordinal);
                if (i > 0) s = s.Substring(0, i);
            }
            s = s.TrimEnd('.', ' ');
            return s.Length > MaxLabel ? s.Substring(0, MaxLabel) + "…" : s;
        }

        public static SummaryKeys Load(string summaryDir, string kind, KeyAnnotations notes)
        {
            var keys = new SummaryKeys();
            string file = Path.Combine(summaryDir, kind + ".psv");
            if (File.Exists(file))
            {
                keys.SourceFile = file;
                foreach (string line in File.ReadLines(file, Encoding.UTF8))
                {
                    if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                    string[] p = line.Split('|', 2);
                    if (p.Length < 2) throw new FormatException("요약 키 줄 형식 오류: " + line);
                    keys.labels[p[0]] = p[1];
                }
                return keys;
            }
            foreach (var e in notes.Entries)
            {
                if (e.Value.Grade != "확인") continue;
                string path = e.Key.Substring(0, e.Key.IndexOf('\t'));
                if (!keys.labels.ContainsKey(path)) keys.labels[path] = ShortLabel(e.Value.Meaning);
            }
            return keys;
        }

        // 요약에 없으면 null
        public string LabelOf(string expandedPath)
        {
            foreach (string key in KeyPattern.Candidates(expandedPath, true))
            {
                string label;
                if (labels.TryGetValue(key, out label)) return label;
            }
            return null;
        }
    }
}
