// 게임 블록 문법(SDL)의 토크나이저, 파서, 키 행 추출.
// .mdl 파일이 텍스트의 80%(약 78MB, 대부분 숫자)를 차지해 PowerShell 객체로는 너무 느리므로 C#으로 둔다.
// PowerShell 쪽 함수(SdlTokenizer.ps1, SdlParser.ps1, SdlKeys.ps1)가 이 클래스를 감싼다.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public sealed class SdlToken
{
    public string Kind { get; set; }   // open | close | str | badstr | atom
    public string Text { get; set; }
    public int Index { get; set; }
    public bool Glued { get; set; }    // 원자 바로 뒤에 붙은 '('
}

public sealed class SdlNode
{
    public string Kind { get; set; }   // root | brace | paren | call | str | atom
    public string Text { get; set; }
    public int Index { get; set; }
    public List<object> Children { get; set; }   // 잎은 null

    public SdlNode(string kind, string text, int index, bool container)
    {
        Kind = kind; Text = text; Index = index;
        Children = container ? new List<object>() : null;
    }
}

public sealed class SdlParseResult
{
    public SdlNode Root { get; set; }
    public List<string> Errors { get; set; }
    public List<SdlToken> Tokens { get; set; }
}

public sealed class SdlRow
{
    public string Path { get; set; }
    public string Form { get; set; }   // block | include | define | mod | macro | paren-other | bare
    public string Shape { get; set; }
    public string Args { get; set; }
}

public static partial class SdlCore
{
    static readonly Regex TokenRegex = new Regex(
        @"(?<comment>;[^\r\n]*)|(?<str>""[^""]*"")|(?<badstr>""[^""]*\z)|(?<open>[{(])|(?<close>[})])|(?<atom>[^\s{}()"";]+)",
        RegexOptions.Compiled);
    static readonly Regex NumberRegex = new Regex(@"^[+-]?(\d+\.?\d*|\.\d+)$", RegexOptions.Compiled);

    public static List<SdlToken> Tokenize(string text)
    {
        var tokens = new List<SdlToken>();
        int prevEnd = -1;
        string prevKind = "";
        for (Match m = TokenRegex.Match(text); m.Success; m = m.NextMatch())
        {
            if (m.Groups["comment"].Success) continue;
            string kind = m.Groups["str"].Success ? "str"
                : m.Groups["badstr"].Success ? "badstr"
                : m.Groups["open"].Success ? "open"
                : m.Groups["close"].Success ? "close" : "atom";
            bool glued = kind == "open" && m.Value == "(" && prevKind == "atom" && m.Index == prevEnd;
            tokens.Add(new SdlToken { Kind = kind, Text = m.Value, Index = m.Index, Glued = glued });
            prevEnd = m.Index + m.Length;
            prevKind = kind;
        }
        return tokens;
    }

    public static SdlParseResult Parse(string text)
    {
        var tokens = Tokenize(text);
        var errors = new List<string>();
        var root = new SdlNode("root", "", 0, true);
        var stack = new Stack<SdlNode>();
        stack.Push(root);
        foreach (var t in tokens)
        {
            var cur = stack.Peek();
            if (t.Kind == "open")
            {
                SdlNode node;
                if (t.Glued)
                {
                    var head = Unwrap(cur.Children[cur.Children.Count - 1]);
                    cur.Children.RemoveAt(cur.Children.Count - 1);
                    node = new SdlNode("call", head.Text, head.Index, true);
                }
                else node = new SdlNode(t.Text == "{" ? "brace" : "paren", "", t.Index, true);
                cur.Children.Add(node);
                stack.Push(node);
            }
            else if (t.Kind == "close")
            {
                if (stack.Count == 1) errors.Add("unexpected '" + t.Text + "' at " + t.Index);
                else
                {
                    string want = cur.Kind == "brace" ? "}" : ")";
                    if (t.Text != want) errors.Add("mismatched '" + t.Text + "' at " + t.Index + ", expected '" + want + "'");
                    stack.Pop();
                }
            }
            else if (t.Kind == "badstr")
            {
                errors.Add("unterminated string at " + t.Index);
                cur.Children.Add(new SdlNode("str", t.Text, t.Index, false));
            }
            else cur.Children.Add(new SdlNode(t.Kind, t.Text, t.Index, false));
        }
        if (stack.Count > 1) errors.Add("unclosed block opened at " + stack.Peek().Index);
        return new SdlParseResult { Root = root, Errors = errors, Tokens = tokens };
    }

    public static void Flat(SdlNode n, List<string> o)
    {
        switch (n.Kind)
        {
            case "root": foreach (object c in n.Children) Flat(Unwrap(c), o); break;
            case "brace": o.Add("{"); foreach (object c in n.Children) Flat(Unwrap(c), o); o.Add("}"); break;
            case "paren": o.Add("("); foreach (object c in n.Children) Flat(Unwrap(c), o); o.Add(")"); break;
            case "call": o.Add(n.Text); o.Add("("); foreach (object c in n.Children) Flat(Unwrap(c), o); o.Add(")"); break;
            default: o.Add(n.Text); break;
        }
    }

    public static string NodesText(IEnumerable nodes)
    {
        var o = new List<string>();
        if (nodes != null) foreach (object x in nodes) Flat(Unwrap(x), o);
        return string.Join(" ", o);
    }

    // PowerShell이 넘긴 PSObject 래퍼를 벗긴다
    public static SdlNode Unwrap(object x)
    {
        var n = x as SdlNode;
        if (n != null) return n;
        var ps = x as System.Management.Automation.PSObject;
        if (ps != null) return (SdlNode)ps.BaseObject;
        throw new ArgumentException("not an SdlNode: " + (x == null ? "null" : x.GetType().FullName));
    }

    public static bool RoundTrip(SdlParseResult p)
    {
        if (p.Errors.Count != 0) return false;
        var flat = new List<string>();
        Flat(p.Root, flat);
        if (flat.Count != p.Tokens.Count) return false;
        for (int i = 0; i < flat.Count; i++) if (!string.Equals(flat[i], p.Tokens[i].Text, StringComparison.Ordinal)) return false;
        return true;
    }
}
