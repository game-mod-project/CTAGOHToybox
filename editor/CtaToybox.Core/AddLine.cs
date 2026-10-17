// 줄 더하기(팩션 강화, docs/superpowers/specs/2026-10-17-faction-boost-design.md).
// {…} 블록의 닫는 괄호 바로 앞에 줄 하나를 넣는다(예: 보병 perks 블록에 {"health" "600%"}, 무기 사본에 {rechargeTime 3}).
// 블록은 여는 { 다음 낱말(따옴표 포함, 소문자)로 경로를 만들고 같은 경로 가운데 몇 번째인지로 찾는다(/perks, /from/inventory).
// 빌드는 값 고치기를 다 한 본문에 넣는다. 넣은 줄이 같은 경로의 다른 줄 순번을 밀지 않게 하려는 것이다.
// 앞 줄과 같은 키를 블록 끝에 더하면 게임이 어느 쪽을 쓰는지는 미확인이다(게임 안 확인을 하지 않는다)
// 수정의 꼴: Kind "add-line", File, Path 블록 경로, Occurrence, Arg 0, Old null, New 넣을 줄 원문, Scope shared
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class BlockSpan
    {
        public string Path { get; set; }
        public int Occurrence { get; set; }
        public int Open { get; set; }    // 여는 { 의 글자 자리
        public int Close { get; set; }   // 닫는 } 의 글자 자리
    }

    public static class AddLine
    {
        public const string Kind = "add-line";

        // 넣을 줄이 괄호 짝이 맞는 한 덩이로 파싱되면 null
        public static string CheckLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return "넣을 줄이 없다";
            if (line.Contains('\n') || line.Contains('\r')) return "줄 하나만 넣는다";
            var parsed = SdlCore.Parse(line);
            if (parsed.Errors.Count > 0) return "줄이 파싱되지 않는다: " + string.Join("; ", parsed.Errors);
            var tokens = parsed.Tokens;
            if (tokens.Count < 2 || tokens[0].Kind != "open" || Match(tokens, 0) != tokens.Count - 1) return "괄호 하나로 감싼 줄이어야 한다: " + line;
            return null;
        }

        // 본문의 {…} 블록들. 파싱되지 않으면 빈 목록
        public static IReadOnlyList<BlockSpan> Blocks(string text)
        {
            var result = new List<BlockSpan>();
            var parsed = SdlCore.Parse(text ?? "");
            if (parsed.Errors.Count > 0) return result;
            var tokens = parsed.Tokens;
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            var stack = new List<KeyValuePair<string, int>>();   // (이 깊이까지의 블록 경로, 여는 토큰 자리). ( )는 경로에 넣지 않는다
            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];
                if (t.Kind == "open")
                {
                    string parent = stack.Count == 0 ? "" : stack[stack.Count - 1].Key;
                    if (t.Text == "{" && i + 1 < tokens.Count && (tokens[i + 1].Kind == "atom" || tokens[i + 1].Kind == "str"))
                    {
                        string path = parent + "/" + tokens[i + 1].Text.ToLowerInvariant();
                        int close = Match(tokens, i);
                        if (close < 0) break;
                        int n = counts.TryGetValue(path, out int c) ? c : 0;
                        counts[path] = n + 1;
                        result.Add(new BlockSpan { Path = path, Occurrence = n, Open = t.Index, Close = tokens[close].Index });
                        stack.Add(new KeyValuePair<string, int>(path, i));
                    }
                    else stack.Add(new KeyValuePair<string, int>(parent, i));
                }
                else if (t.Kind == "close" && stack.Count > 0) stack.RemoveAt(stack.Count - 1);
            }
            return result;
        }

        public static BlockSpan Find(string text, string path, int occurrence)
        {
            return Blocks(text).FirstOrDefault(b => b.Occurrence == occurrence && string.Equals(b.Path, path, StringComparison.OrdinalIgnoreCase));
        }

        // 켜진 줄 더하기를 차례로 넣은 본문. 수정마다 결과 하나를 results에 더한다
        public static string Apply(string text, IEnumerable<Edit> adds, List<EditResult> results)
        {
            foreach (var e in (adds ?? Enumerable.Empty<Edit>()).Where(x => x.Enabled && x.Kind == Kind))
            {
                string bad = CheckLine(e.New);
                if (bad != null) { results.Add(Result(e, EditStatus.Invalid, bad)); continue; }
                var block = Find(text, e.Path, e.Occurrence);
                if (block == null) { results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 블록이 없다: " + e.Path + (e.Occurrence > 0 ? " [" + e.Occurrence + "]" : ""))); continue; }
                string inside = text.Substring(block.Open, block.Close - block.Open);
                if (inside.Contains(e.New.Trim())) { results.Add(Result(e, EditStatus.Applied, null)); continue; }
                string nl = text.Contains("\r\n") ? "\r\n" : "\n";
                int at = block.Close;
                string lead = at > 0 && text[at - 1] != '\n' ? nl : "";
                string next = text.Substring(0, at) + lead + "\t" + e.New.Trim() + nl + text.Substring(at);
                if (SdlCore.Parse(next).Errors.Count > 0) { results.Add(Result(e, EditStatus.Invalid, "줄을 넣은 파일이 파싱되지 않는다")); continue; }
                text = next;
                results.Add(Result(e, EditStatus.Applied, null));
            }
            return text;
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

        public static Edit New(string unit, string file, string blockPath, int occurrence, string line)
        {
            return new Edit { Kind = Kind, Unit = SourceStack.ToVirtual(unit ?? file), File = SourceStack.ToVirtual(file), Path = blockPath, Occurrence = occurrence, Arg = 0, Old = null, New = line, Scope = "shared" };
        }

        static EditResult Result(Edit e, EditStatus status, string message) { return new EditResult { Edit = e, Status = status, Message = message }; }
    }
}
