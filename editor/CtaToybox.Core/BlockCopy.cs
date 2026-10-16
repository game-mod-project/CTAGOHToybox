// 게임 설정 파일의 따옴표 이름 블록 복제(E18, docs/superpowers/specs/2026-10-16-e18-class-vision-design.md).
// 원본 블록의 원문을 그대로 복사해 이름만 바꾸고, 같은 부모 블록의 마지막 형제 뒤(부모가 닫히기 전, 맨 위면 파일 끝)에 넣는다.
// 그 자리에 넣으면 같은 경로의 기존 줄 순번(Occurrence)이 밀리지 않는다. 그래서 같은 파일의 다른 수정은 복제를 넣은 본문에 그대로 적용된다.
// 복제 수정의 꼴: File 설정 파일, Path·Occurrence 원본 블록 이름 줄(/"*" 꼴), NamedPath 그 원문 경로(/"tank_range_200"), Arg 0,
// Old 원본 이름(따옴표 포함), New 새 이름(따옴표 포함), Scope shared
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static class BlockCopy
    {
        public const string Kind = "block-copy";

        static readonly Regex NameRegex = new Regex("^\"[^\"/\\s]+\"$", RegexOptions.CultureInvariant);

        // 새 이름 토큰이 맞으면 null
        public static string CheckName(string newToken)
        {
            if (newToken == null || !NameRegex.IsMatch(newToken)) return "새 블록 이름은 따옴표로 감싼 이름 하나다(빈칸·따옴표·/ 없이): " + newToken;
            return null;
        }

        public static string ApplyAll(string text, IEnumerable<Edit> copies)
        {
            return Apply(text, copies, new List<EditResult>());
        }

        // 켜진 복제를 차례로 넣은 본문. 복제마다 결과 하나를 results에 더한다
        public static string Apply(string text, IEnumerable<Edit> copies, List<EditResult> results)
        {
            var active = (copies ?? Enumerable.Empty<Edit>()).Where(e => e.Enabled && e.Kind == Kind).ToList();
            if (active.Count == 0 || text == null) return text;
            if (SdlCore.Parse(text).Errors.Count > 0)
            {
                foreach (var e in active) results.Add(Result(e, EditStatus.Invalid, "원본 파일이 파싱되지 않는다"));
                return text;
            }
            foreach (var e in active)
            {
                string error;
                text = ApplyOne(text, e, out error, out EditStatus status);
                results.Add(Result(e, status, error));
            }
            return text;
        }

        static string ApplyOne(string text, Edit e, out string message, out EditStatus status)
        {
            message = null;
            string nameError = CheckName(e.New);
            if (nameError != null) { status = EditStatus.Invalid; message = nameError; return text; }
            var parsed = SdlCore.Parse(text);
            var index = new KeyIndex(parsed.Root);
            var row = e.NamedPath != null ? index.NamedRow(e.NamedPath, e.NamedOccurrence) : index.Row(e.Path, e.Occurrence);
            if (row == null || row.Leaves.Count == 0 || !row.Path.EndsWith("/\"*\"", StringComparison.Ordinal))
            { status = EditStatus.TargetMissing; message = "원본에 이 블록이 없다: " + (e.NamedPath ?? e.Path); return text; }
            var name = row.Leaves[0];
            if (!string.Equals(name.Text, e.Old, StringComparison.OrdinalIgnoreCase))
            { status = EditStatus.SourceChanged; message = "원본의 블록 이름이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + name.Text; return text; }
            string newName = e.New.Trim('"');
            if (KeyLocator.Rows(parsed.Root).Any(r => r.Path == row.Path && r.Leaves.Count > 0 && string.Equals(r.Leaves[0].Text.Trim('"'), newName, StringComparison.OrdinalIgnoreCase)))
            { status = EditStatus.Invalid; message = "이미 있는 이름이다: " + e.New; return text; }

            var tokens = parsed.Tokens;
            int ti = tokens.FindIndex(t => t.Index == name.Index);
            if (ti < 1 || tokens[ti - 1].Text != "{") { status = EditStatus.Invalid; message = "블록의 여는 괄호를 찾지 못했다"; return text; }
            int open = ti - 1, close = Match(tokens, open);
            if (close < 0) { status = EditStatus.Invalid; message = "블록의 닫는 괄호를 찾지 못했다"; return text; }
            string block = text.Substring(tokens[open].Index, tokens[close].Index + 1 - tokens[open].Index);
            int rel = name.Index - tokens[open].Index;
            block = block.Substring(0, rel) + e.New + block.Substring(rel + name.Text.Length);

            // 부모 블록: 여는 괄호에서 거꾸로 깊이를 세어 감싼 괄호를 찾는다. 없으면 파일 끝
            int depth = 0, parent = -1;
            for (int i = open - 1; i >= 0 && parent < 0; i--)
            {
                if (tokens[i].Kind == "close") depth++;
                else if (tokens[i].Kind == "open") { if (depth == 0) parent = i; else depth--; }
            }
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            int at = parent < 0 ? text.Length : tokens[Match(tokens, parent)].Index;
            string lead = at > 0 && text[at - 1] != '\n' ? nl : "";
            status = EditStatus.Applied;
            return text.Substring(0, at) + lead + block + nl + text.Substring(at);
        }

        // 여는 괄호 토큰의 짝 닫는 괄호 토큰 자리. 없으면 -1
        static int Match(List<SdlToken> tokens, int open)
        {
            int depth = 0;
            for (int j = open; j < tokens.Count; j++)
            {
                if (tokens[j].Kind == "open") depth++;
                else if (tokens[j].Kind == "close" && --depth == 0) return j;
            }
            return -1;
        }

        // e가 기대는 복제 이름(따옴표 없이). 복제 블록 안의 값이거나, 값을 복제 이름으로 바꾸는 수정이면 그 이름. 아니면 null
        public static string Dependency(Edit e, IEnumerable<Edit> copies)
        {
            if (e.Kind == Kind) return null;
            foreach (var c in copies.Where(x => x.Kind == Kind && x.New != null))
            {
                string name = c.New.Trim('"');
                if (e.NamedPath != null && e.NamedPath.IndexOf("/\"" + name + "\"", StringComparison.OrdinalIgnoreCase) >= 0) return name;
                if (e.Kind == null && e.New != null && string.Equals(e.New.Trim(), "\"" + name + "\"", StringComparison.OrdinalIgnoreCase)) return name;
            }
            return null;
        }

        public static string MissingNote(string name) { return "필드 " + name + "가 없다(블록 복제가 꺼졌거나 적용되지 않았다)"; }

        static EditResult Result(Edit e, EditStatus status, string message)
        {
            return new EditResult { Edit = e, Status = status, Message = message };
        }
    }
}
