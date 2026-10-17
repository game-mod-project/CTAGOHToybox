// gettext .po/.pot 한 파일(한글화, docs/superpowers/specs/2026-10-17-korean-localization-design.md).
// 문자열은 파일 속 이스케이프 그대로 둔다(\n, \"를 풀지 않는다). 이어진 줄("..." 다음 줄의 "...")은 이어 붙인다.
// 항목 순서와 같은 msgctxt의 되풀이, msgctxt 없는 항목을 모두 그대로 지킨다. 쓰기는 LF, 항목 사이 빈 줄 하나
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.L10n
{
    public sealed class PoEntry
    {
        public string Context { get; set; }
        public string Id { get; set; } = "";
        public string Str { get; set; } = "";
        public bool Fuzzy { get; set; }
        public string PreviousId { get; set; }
        public List<string> Comments { get; set; } = new List<string>();
        public string Key { get { return Context ?? ("\u0004" + Id); } }
    }

    public sealed class PoFile
    {
        public string Header { get; set; }                       // 머리 msgstr의 이어 붙인 원문. 머리가 없으면 null
        public bool HeaderFuzzy { get; set; }
        public List<string> HeaderComments { get; set; } = new List<string>();
        public List<PoEntry> Entries { get; set; } = new List<PoEntry>();
        public List<string> Trailing { get; set; } = new List<string>();   // 마지막 항목 뒤의 주석 줄(#~ 등)

        static string Quoted(string line, int start)
        {
            int a = line.IndexOf('"', start), b = line.LastIndexOf('"');
            return a < 0 || b <= a ? "" : line.Substring(a + 1, b - a - 1);
        }

        public static PoFile Parse(string text)
        {
            var po = new PoFile();
            var lines = text.Replace("\r\n", "\n").Split('\n');
            PoEntry cur = null;
            var pending = new List<string>();
            bool fuzzy = false;
            string prev = null;
            string field = null;            // 이어진 줄을 붙일 칸: ctx, id, str, prev
            bool sawStr = false;

            void Finish()
            {
                if (cur == null) return;
                if (po.Header == null && po.Entries.Count == 0 && cur.Context == null && cur.Id.Length == 0)
                {
                    po.Header = cur.Str; po.HeaderFuzzy = cur.Fuzzy; po.HeaderComments = cur.Comments;
                }
                else po.Entries.Add(cur);
                cur = null; sawStr = false; field = null;
            }

            void Start()
            {
                cur = new PoEntry { Comments = pending, Fuzzy = fuzzy, PreviousId = prev };
                pending = new List<string>(); fuzzy = false; prev = null;
            }

            foreach (string raw in lines)
            {
                string line = raw.TrimEnd();
                if (line.Length == 0) { Finish(); continue; }
                if (line.StartsWith("#,"))
                {
                    if (cur != null && sawStr) Finish();
                    var flags = line.Substring(2).Split(',').Select(f => f.Trim()).Where(f => f.Length > 0).ToList();
                    if (flags.Remove("fuzzy")) fuzzy = true;
                    if (flags.Count > 0) pending.Add("#, " + string.Join(", ", flags));
                    field = null;
                    continue;
                }
                if (line.StartsWith("#| msgid "))
                {
                    if (cur != null && sawStr) Finish();
                    prev = Quoted(line, 8); field = "prev"; continue;
                }
                if (line.StartsWith("#| \"") && field == "prev") { prev += Quoted(line, 2); continue; }
                if (line.StartsWith("#"))
                {
                    if (cur != null && sawStr) Finish();
                    pending.Add(raw.TrimEnd()); field = null; continue;
                }
                if (line.StartsWith("msgctxt "))
                {
                    if (cur != null) Finish();
                    Start(); cur.Context = Quoted(line, 7); field = "ctx"; continue;
                }
                if (line.StartsWith("msgid "))
                {
                    if (cur != null && (sawStr || cur.Id.Length > 0 || field == "id")) Finish();
                    if (cur == null) Start();
                    cur.Id = Quoted(line, 5); field = "id"; continue;
                }
                if (line.StartsWith("msgstr "))
                {
                    if (cur == null) Start();
                    cur.Str = Quoted(line, 6); field = "str"; sawStr = true; continue;
                }
                if (line.StartsWith("\"") && cur != null)
                {
                    string part = Quoted(line, 0);
                    if (field == "ctx") cur.Context += part;
                    else if (field == "id") cur.Id += part;
                    else if (field == "str") cur.Str += part;
                }
            }
            Finish();
            po.Trailing = pending;
            return po;
        }

        public string Write()
        {
            var sb = new StringBuilder();
            bool first = true;
            if (Header != null)
            {
                foreach (string c in HeaderComments) sb.Append(c).Append('\n');
                if (HeaderFuzzy) sb.Append("#, fuzzy\n");
                sb.Append("msgid \"\"\nmsgstr \"\"\n");
                foreach (string part in SplitHeader(Header)) sb.Append('"').Append(part).Append("\"\n");
                first = false;
            }
            foreach (var e in Entries)
            {
                if (!first) sb.Append('\n');
                first = false;
                foreach (string c in e.Comments) sb.Append(c).Append('\n');
                if (e.Fuzzy) sb.Append("#, fuzzy\n");
                if (e.PreviousId != null) sb.Append("#| msgid \"").Append(e.PreviousId).Append("\"\n");
                if (e.Context != null) sb.Append("msgctxt \"").Append(e.Context).Append("\"\n");
                sb.Append("msgid \"").Append(e.Id).Append("\"\n");
                sb.Append("msgstr \"").Append(e.Str).Append("\"\n");
            }
            if (Trailing.Count > 0) { sb.Append('\n'); foreach (string c in Trailing) sb.Append(c).Append('\n'); }
            return sb.ToString();
        }

        // 머리 원문을 \n(두 글자) 뒤에서 나눈다
        static IEnumerable<string> SplitHeader(string header)
        {
            int start = 0;
            for (int i = 0; i + 1 < header.Length; i++)
                if (header[i] == '\\' && header[i + 1] == 'n' && (i == 0 || header[i - 1] != '\\')) { yield return header.Substring(start, i + 2 - start); start = i + 2; i++; }
            if (start < header.Length) yield return header.Substring(start);
        }

        public static string HeaderWithLanguage(string header, string lang)
        {
            header = header ?? "";
            var parts = SplitHeader(header).ToList();
            int i = parts.FindIndex(p => p.StartsWith("Language:", StringComparison.Ordinal));
            string line = "Language: " + lang + "\\n";
            if (i >= 0) parts[i] = line; else parts.Add(line);
            return string.Concat(parts);
        }
    }
}
