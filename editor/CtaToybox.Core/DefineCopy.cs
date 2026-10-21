// 매크로 정의 복제(docs/superpowers/specs/2026-10-21-faction-boost-clip-supply-design.md).
// 파일 맨 위의 (define "이름" …) 하나를 원문 그대로 복사해 이름만 바꾸고 파일 끝에 붙인다.
// 끝에 붙이므로 원본 토큰의 자리와 다른 줄의 순번이 밀리지 않는다. 복제본 안의 값은 /(define "새 이름")/… 경로로 다른 수정처럼 고친다.
// 수정의 꼴: File 정의가 든 파일, Path PathOf(원래 이름), Arg 0, Old 원래 이름(따옴표 포함), New 새 이름(따옴표 포함), Scope shared
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static class DefineCopy
    {
        public const string Kind = "define-copy";

        public static string PathOf(string name) { return "/(define \"" + (name ?? "").ToLowerInvariant() + "\")"; }

        // 새 이름 토큰이 맞으면 null. 블록 복제와 같은 규칙이다
        public static string CheckName(string newToken)
        {
            return BlockCopy.CheckName(newToken) == null ? null : "새 매크로 이름은 따옴표로 감싼 이름 하나다(빈칸·따옴표·/ 없이): " + newToken;
        }

        public static Edit New(string unit, string file, string name, string newName)
        {
            return new Edit { Kind = Kind, Unit = unit, File = file, Path = PathOf(name), Occurrence = 0, Arg = 0, Old = "\"" + name + "\"", New = "\"" + newName + "\"", Scope = "shared" };
        }

        // 켜진 복제를 차례로 넣은 본문. 복제마다 결과 하나를 results에 더한다
        public static string Apply(string text, IEnumerable<Edit> copies, List<EditResult> results)
        {
            var active = (copies ?? Enumerable.Empty<Edit>()).Where(e => e.Enabled && e.Kind == Kind).ToList();
            if (active.Count == 0 || text == null) return text;
            if (SdlCore.Parse(text).Errors.Count > 0)
            {
                foreach (var e in active) results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "원본 파일이 파싱되지 않는다" });
                return text;
            }
            foreach (var e in active)
            {
                string message;
                EditStatus status;
                text = ApplyOne(text, e, out message, out status);
                results.Add(new EditResult { Edit = e, Status = status, Message = message });
            }
            return text;
        }

        // 맨 위 깊이의 (define "이름" …): 여는 괄호 토큰 자리와 이름 토큰 자리
        sealed class Found { public int Open; public int Name; }

        static List<Found> Defines(List<SdlToken> tokens)
        {
            var list = new List<Found>();
            int depth = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];
                if (t.Kind == "open")
                {
                    if (depth == 0 && t.Text == "(" && i + 2 < tokens.Count && tokens[i + 1].Kind == "atom"
                        && string.Equals(tokens[i + 1].Text, "define", StringComparison.OrdinalIgnoreCase) && tokens[i + 2].Kind == "str")
                        list.Add(new Found { Open = i, Name = i + 2 });
                    depth++;
                }
                else if (t.Kind == "close") depth--;
            }
            return list;
        }

        static string ApplyOne(string text, Edit e, out string message, out EditStatus status)
        {
            message = null;
            string nameError = CheckName(e.New);
            if (nameError != null) { status = EditStatus.Invalid; message = nameError; return text; }
            var tokens = SdlCore.Parse(text).Tokens;
            var defines = Defines(tokens);
            var original = defines.FirstOrDefault(d => string.Equals(tokens[d.Name].Text, e.Old, StringComparison.OrdinalIgnoreCase));
            if (original == null) { status = EditStatus.TargetMissing; message = "원본에 이 매크로 정의가 없다: " + e.Old; return text; }
            if (defines.Any(d => string.Equals(tokens[d.Name].Text, e.New, StringComparison.OrdinalIgnoreCase)))
            { status = EditStatus.Invalid; message = "이미 있는 매크로 이름이다: " + e.New; return text; }
            int close = Match(tokens, original.Open);
            if (close < 0) { status = EditStatus.Invalid; message = "정의의 닫는 괄호를 찾지 못했다"; return text; }
            int start = tokens[original.Open].Index;
            string body = text.Substring(start, tokens[close].Index + 1 - start);
            int rel = tokens[original.Name].Index - start;
            body = body.Substring(0, rel) + e.New + body.Substring(rel + tokens[original.Name].Text.Length);
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            string lead = text.Length > 0 && !text.EndsWith("\n", StringComparison.Ordinal) ? nl : "";
            status = EditStatus.Applied;
            return text + lead + body + nl;
        }

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

        // e가 기대는 복제본 이름(따옴표 없이). 복제본 안의 값이거나, 호출을 복제본 이름으로 바꾸는 수정이면 그 이름. 아니면 null
        public static string Dependency(Edit e, IEnumerable<Edit> copies)
        {
            if (e.Kind == Kind) return null;
            foreach (var c in copies.Where(x => x.Kind == Kind && x.New != null))
            {
                string name = c.New.Trim('"');
                string prefix = PathOf(name);
                if (e.Path != null && (string.Equals(e.Path, prefix, StringComparison.OrdinalIgnoreCase) || e.Path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase))) return name;
                if (e.Kind == MacroSwap.Kind && e.New != null && string.Equals(e.New.Trim(), "\"" + name + "\"", StringComparison.OrdinalIgnoreCase)) return name;
            }
            return null;
        }

        public static string MissingNote(string name) { return "매크로 " + name + "가 없다(매크로 복제가 꺼졌거나 적용되지 않았다)"; }
    }
}
