// 컨퀘스트 AI 설정(script/multiplayer/modes/conquest/runtime_config.lua)을 읽는다. Lua 해석기가 아니라 이 파일에 나오는 꼴만 읽는다:
//   문장: 이름(.이름)* = 값. 맨 위의 X = X or {}는 줄을 만들지 않는다
//   값: 수, 수식(수와 * / + - ( )), 문자열, true, false, nil, 표 { 이름 = 값, ["문자열"] = 값, 순서 값 }
//   주석: -- 줄 주석, --[[ ]] 블록 주석. 필드 줄 끝 주석(없으면 바로 위 주석 줄)은 그 줄에, 표 앞의 주석 줄은 그 표의 줄들에 붙인다
// 이 밖의 문법을 만나면 Errors에 적고 줄을 하나도 내지 않는다(파일 전체를 읽지 못함으로 본다). 추측해서 읽지 않는다.
// 줄 경로는 KeyLocator와 같은 꼴이다: /conquestconfig.startspawntime/attackmin, 따옴표 키는 "*", 순서 값은 #1, #2
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class LuaRow
    {
        public string Path { get; set; }          // 소문자 경로. 비교에 쓴다
        public string RawPath { get; set; }       // 원문 그대로의 경로. 따옴표 키는 "0.9"처럼 원문 이름
        public int Occurrence { get; set; }       // 같은 경로 가운데 몇 번째
        public string Value { get; set; }         // 값의 원문. 예: 2 * 60000, "escalating", true
        public int Index { get; set; }            // 값이 시작하는 글자 위치
        public string Kind { get; set; }          // number | expr | string | bool | nil
        public string Comment { get; set; }       // 그 줄 끝 주석. 없으면 바로 위 주석 줄들. 없으면 null
        public string TableComment { get; set; }  // 값을 감싼 가장 가까운 표 앞의 주석. 없으면 null
    }

    public sealed class LuaParseResult
    {
        public List<LuaRow> Rows { get; } = new List<LuaRow>();
        public List<string> Errors { get; } = new List<string>();
    }

    public static class LuaTable
    {
        public static bool IsLua(string virtualPath) { return virtualPath != null && virtualPath.EndsWith(".lua", StringComparison.OrdinalIgnoreCase); }

        public static LuaParseResult Read(string text)
        {
            var result = new LuaParseResult();
            try { new Parser(text ?? "", result).Chunk(); }
            catch (FormatException ex) { result.Rows.Clear(); result.Errors.Add(ex.Message); }
            return result;
        }

        // 값 하나의 종류. 값으로 읽을 수 없으면(이름, 표, 호출 등) null
        public static string KindOf(string valueText)
        {
            if (string.IsNullOrWhiteSpace(valueText)) return null;
            var r = Read("x = " + valueText.Trim());
            return r.Errors.Count == 0 && r.Rows.Count == 1 && r.Rows[0].Path == "/x" ? r.Rows[0].Kind : null;
        }

        // 수식 계산: 수, + - * /, 괄호, 단항 -. 계산이 안 되거나 무한·NaN이면 false
        public static bool TryEvaluate(string expr, out double value)
        {
            value = 0;
            if (expr == null) return false;
            try
            {
                int i = 0;
                value = Sum(expr, ref i);
                Skip(expr, ref i);
                return i == expr.Length && !double.IsNaN(value) && !double.IsInfinity(value);
            }
            catch (FormatException) { return false; }
        }

        static void Skip(string s, ref int i) { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }

        static double Sum(string s, ref int i)
        {
            double v = Product(s, ref i);
            for (;;)
            {
                Skip(s, ref i);
                if (i < s.Length && (s[i] == '+' || s[i] == '-')) { char op = s[i++]; double r = Product(s, ref i); v = op == '+' ? v + r : v - r; }
                else return v;
            }
        }

        static double Product(string s, ref int i)
        {
            double v = Unary(s, ref i);
            for (;;)
            {
                Skip(s, ref i);
                if (i < s.Length && (s[i] == '*' || s[i] == '/')) { char op = s[i++]; double r = Unary(s, ref i); v = op == '*' ? v * r : v / r; }
                else return v;
            }
        }

        static double Unary(string s, ref int i)
        {
            Skip(s, ref i);
            if (i < s.Length && s[i] == '-') { i++; return -Unary(s, ref i); }
            if (i < s.Length && s[i] == '(')
            {
                i++;
                double v = Sum(s, ref i);
                Skip(s, ref i);
                if (i >= s.Length || s[i] != ')') throw new FormatException("')'가 없다");
                i++;
                return v;
            }
            int start = i;
            while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
            if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
            {
                i++;
                if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
                while (i < s.Length && char.IsDigit(s[i])) i++;
            }
            double d;
            if (i == start || !double.TryParse(s.Substring(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture, out d)) throw new FormatException("수가 아니다");
            return d;
        }

        sealed class Tok
        {
            public string Kind;   // name | number | string | sym | comment
            public string Text;
            public int Index;
            public int Line;
        }

        static List<Tok> Tokenize(string s)
        {
            var list = new List<Tok>();
            int i = 0, line = 1;
            while (i < s.Length)
            {
                char c = s[i];
                if (c == '\n') { line++; i++; continue; }
                if (char.IsWhiteSpace(c)) { i++; continue; }
                int start = i;
                if (c == '-' && i + 1 < s.Length && s[i + 1] == '-')
                {
                    if (i + 3 < s.Length && s[i + 2] == '[' && s[i + 3] == '[')
                    {
                        int close = s.IndexOf("]]", i + 4, StringComparison.Ordinal);
                        if (close < 0) throw new FormatException(line + "줄: 닫히지 않은 블록 주석");
                        string body = s.Substring(i + 4, close - i - 4);
                        list.Add(new Tok { Kind = "comment", Text = body.Trim(), Index = start, Line = line });
                        line += body.Count(ch => ch == '\n');
                        i = close + 2;
                        continue;
                    }
                    int end = s.IndexOf('\n', i);
                    if (end < 0) end = s.Length;
                    list.Add(new Tok { Kind = "comment", Text = s.Substring(i + 2, end - i - 2).Trim(), Index = start, Line = line });
                    i = end;
                    continue;
                }
                if (char.IsLetter(c) || c == '_')
                {
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_')) i++;
                    list.Add(new Tok { Kind = "name", Text = s.Substring(start, i - start), Index = start, Line = line });
                    continue;
                }
                if (char.IsDigit(c) || (c == '.' && i + 1 < s.Length && char.IsDigit(s[i + 1])))
                {
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
                    if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
                    {
                        i++;
                        if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
                        while (i < s.Length && char.IsDigit(s[i])) i++;
                    }
                    string number = s.Substring(start, i - start);
                    double d;
                    if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) throw new FormatException(line + "줄: 수가 아니다 (" + number + ")");
                    list.Add(new Tok { Kind = "number", Text = number, Index = start, Line = line });
                    continue;
                }
                if (c == '"' || c == '\'')
                {
                    i++;
                    while (i < s.Length && s[i] != c)
                    {
                        if (s[i] == '\n') throw new FormatException(line + "줄: 줄이 바뀌는 문자열");
                        if (s[i] == '\\') i++;
                        i++;
                    }
                    if (i >= s.Length) throw new FormatException(line + "줄: 닫히지 않은 문자열");
                    i++;
                    list.Add(new Tok { Kind = "string", Text = s.Substring(start, i - start), Index = start, Line = line });
                    continue;
                }
                if ("{}[]=,;.*/+-()".IndexOf(c) >= 0)
                {
                    i++;
                    list.Add(new Tok { Kind = "sym", Text = c.ToString(), Index = start, Line = line });
                    continue;
                }
                throw new FormatException(line + "줄: 읽을 수 없는 글자 '" + c + "'");
            }
            return list;
        }

        sealed class Parser
        {
            readonly LuaParseResult result;
            readonly List<Tok> t = new List<Tok>();
            readonly Dictionary<int, Tok> comments = new Dictionary<int, Tok>();
            readonly HashSet<int> codeLines = new HashSet<int>();
            readonly Dictionary<string, int> seen = new Dictionary<string, int>(StringComparer.Ordinal);
            readonly string text;
            int p;

            public Parser(string text, LuaParseResult result)
            {
                this.text = text;
                this.result = result;
                foreach (var k in Tokenize(text))
                {
                    if (k.Kind == "comment") comments[k.Line] = k;
                    else { t.Add(k); codeLines.Add(k.Line); }
                }
            }

            Tok Peek(int ahead = 0) { return p + ahead < t.Count ? t[p + ahead] : null; }
            static bool IsSym(Tok k, string s) { return k != null && k.Kind == "sym" && k.Text == s; }
            static FormatException Error(Tok k, string message)
            {
                return new FormatException((k == null ? "파일 끝" : k.Line + "줄") + ": " + message + (k == null ? "" : " (" + k.Text + ")"));
            }
            void Expect(string sym) { if (!IsSym(Peek(), sym)) throw Error(Peek(), "'" + sym + "'가 와야 한다"); p++; }

            // 바로 위의 주석 줄들(코드가 없는 줄만, 빈 줄에서 멈춘다). 없으면 null
            string Preceding(int line)
            {
                var parts = new List<string>();
                for (int l = line - 1; l > 0; l--)
                {
                    Tok c;
                    if (!comments.TryGetValue(l, out c) || codeLines.Contains(l)) break;
                    parts.Insert(0, c.Text);
                }
                return parts.Count == 0 ? null : string.Join(" ", parts);
            }

            public void Chunk()
            {
                while (Peek() != null)
                {
                    var first = Peek();
                    if (first.Kind != "name") throw Error(first, "문장은 이름으로 시작해야 한다");
                    var names = new List<string> { first.Text };
                    p++;
                    while (IsSym(Peek(), "."))
                    {
                        p++;
                        if (Peek() == null || Peek().Kind != "name") throw Error(Peek(), "'.' 뒤에 이름이 와야 한다");
                        names.Add(Peek().Text);
                        p++;
                    }
                    Expect("=");
                    string target = string.Join(".", names);
                    // X = X or {}: 표를 만들 뿐 값이 없다
                    if (Peek() != null && Peek().Kind == "name" && Peek().Text == target && Peek(1) != null && Peek(1).Kind == "name" && Peek(1).Text == "or"
                        && IsSym(Peek(2), "{") && IsSym(Peek(3), "}"))
                        p += 4;
                    else Value("/" + target.ToLowerInvariant(), "/" + target, Preceding(first.Line), null);
                    if (IsSym(Peek(), ";")) p++;
                }
            }

            void Value(string path, string raw, string fieldComment, string tableComment)
            {
                if (IsSym(Peek(), "{")) { Table(path, raw, fieldComment ?? tableComment); return; }
                int start = p, depth = 0;
                bool expectOperand = true;
                while (Peek() != null)
                {
                    var k = Peek();
                    if (depth == 0 && (IsSym(k, ",") || IsSym(k, ";") || IsSym(k, "}"))) break;
                    if (k.Kind == "number" || k.Kind == "string" || k.Kind == "name")
                    {
                        if (!expectOperand) break;   // 다음 문장의 시작(맨 위의 값은 구분자 없이 줄이 바뀐다)
                        expectOperand = false; p++; continue;
                    }
                    if (IsSym(k, "(")) { if (!expectOperand) throw Error(k, "호출은 읽지 않는다"); depth++; p++; continue; }
                    if (IsSym(k, ")")) { if (expectOperand || depth == 0) throw Error(k, "짝이 맞지 않는 ')'"); depth--; p++; continue; }
                    if (IsSym(k, "-")) { if (!expectOperand) expectOperand = true; p++; continue; }
                    if (IsSym(k, "*") || IsSym(k, "/") || IsSym(k, "+")) { if (expectOperand) throw Error(k, "연산자 앞에 값이 없다"); expectOperand = true; p++; continue; }
                    throw Error(k, "값 자리에 올 수 없다");
                }
                if (p == start) throw Error(Peek(), "값이 없다");
                if (expectOperand || depth != 0) throw Error(Peek(), "식이 끝나지 않았다");

                var first = t[start];
                var last = t[p - 1];
                int count = p - start;
                string kind;
                if (count == 1 && first.Kind == "string") kind = "string";
                else if (count == 1 && first.Kind == "name")
                {
                    if (first.Text == "true" || first.Text == "false") kind = "bool";
                    else if (first.Text == "nil") kind = "nil";
                    else throw Error(first, "다른 값을 가리키는 이름은 읽지 않는다");
                }
                else
                {
                    for (int i = start; i < p; i++)
                        if (t[i].Kind == "string" || t[i].Kind == "name") throw Error(t[i], "문자열·이름이 든 식은 읽지 않는다");
                    kind = count == 1 || (count == 2 && IsSym(first, "-")) ? "number" : "expr";
                }
                int end = last.Index + last.Text.Length;
                string valueText = text.Substring(first.Index, end - first.Index);
                double ignored;
                if (kind == "expr" && !TryEvaluate(valueText, out ignored)) throw Error(first, "계산되지 않는 식");

                Tok trailing;
                string comment = comments.TryGetValue(last.Line, out trailing) && trailing.Index > last.Index ? trailing.Text : fieldComment;
                int occ;
                seen.TryGetValue(path, out occ);
                seen[path] = occ + 1;
                result.Rows.Add(new LuaRow { Path = path, RawPath = raw, Occurrence = occ, Value = valueText, Index = first.Index, Kind = kind, Comment = comment, TableComment = tableComment });
            }

            void Table(string path, string raw, string tableComment)
            {
                Expect("{");
                int position = 0;
                while (!IsSym(Peek(), "}"))
                {
                    var k = Peek();
                    if (k == null) throw Error(null, "닫히지 않은 표");
                    string fieldComment = Preceding(k.Line);
                    if (k.Kind == "name" && IsSym(Peek(1), "="))
                    {
                        p += 2;
                        Value(path + "/" + k.Text.ToLowerInvariant(), raw + "/" + k.Text, fieldComment, tableComment);
                    }
                    else if (IsSym(k, "["))
                    {
                        p++;
                        var key = Peek();
                        if (key == null || key.Kind != "string") throw Error(key, "[ ] 안에는 문자열 키만 읽는다");
                        p++;
                        Expect("]");
                        Expect("=");
                        Value(path + "/\"*\"", raw + "/" + key.Text, fieldComment, tableComment);
                    }
                    else
                    {
                        position++;
                        Value(path + "/#" + position, raw + "/#" + position, fieldComment, tableComment);
                    }
                    if (IsSym(Peek(), ",") || IsSym(Peek(), ";")) p++;
                    else if (!IsSym(Peek(), "}")) throw Error(Peek(), "',' 또는 '}'가 와야 한다");
                }
                p++;
            }
        }
    }
}
