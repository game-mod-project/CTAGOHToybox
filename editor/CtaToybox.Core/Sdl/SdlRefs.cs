// 이름으로 파일 사이의 참조를 찾는다.
// 인자에 나온 문자열이 다른 파일의 이름(아이템, 엔티티, breed, 패턴)과 같으면 참조로 기록한다.
// 흔한 단어(gun, body)는 잘못 이어질 수 있다. whole은 인자 전체가 이름과 같은 경우, token은 공백으로 나눈 조각이 같은 경우다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public sealed class SdlReferenceIndex
{
    static readonly Regex ArgToken = new Regex("\"([^\"]*)\"|([^\\s\"(){}]+)", RegexOptions.Compiled);
    static readonly Regex Digits = new Regex(@"^\d+$", RegexOptions.Compiled);
    readonly Dictionary<string, HashSet<string>> names = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public int NameCount { get { return names.Count; } }
    public int ReferenceCount { get { return seen.Count; } }

    public void AddName(string name, string ns)
    {
        if (name.Length < 3 || Digits.IsMatch(name)) return;
        HashSet<string> set;
        if (!names.TryGetValue(name, out set)) { set = new HashSet<string>(StringComparer.Ordinal); names[name] = set; }
        set.Add(ns);
    }

    static string Csv(string s) { return "\"" + s.Replace("\"", "\"\"") + "\""; }

    void Match(string id, string self, string path, string candidate, string matchType, TextWriter w, ref int added)
    {
        HashSet<string> set;
        if (string.Equals(candidate, self, StringComparison.OrdinalIgnoreCase) || !names.TryGetValue(candidate, out set)) return;
        string target = candidate.ToLowerInvariant();
        foreach (string ns in set)
        {
            if (!seen.Add(id + "\t" + path + "\t" + ns + "\t" + target + "\t" + matchType)) continue;
            w.WriteLine(Csv(id) + "," + Csv(path) + "," + ns + "," + Csv(target) + "," + matchType);
            added++;
        }
    }

    // 한 파일의 행을 훑어 새로 찾은 참조를 CSV 줄로 쓴다. 반환값은 새 참조 수
    public int Scan(string id, string self, List<SdlRow> rows, TextWriter w)
    {
        int added = 0;
        foreach (var r in rows)
        {
            if (string.IsNullOrEmpty(r.Args)) continue;
            for (Match m = ArgToken.Match(r.Args); m.Success; m = m.NextMatch())
            {
                string whole = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
                Match(id, self, r.Path, whole, "whole", w, ref added);
                if (whole.IndexOf(' ') >= 0)
                    foreach (string t in whole.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        Match(id, self, r.Path, t, "token", w, ref added);
            }
        }
        return added;
    }
}
