// 트리에서 키 행을 뽑는다. Sdl.cs의 SdlCore에 이어지는 부분이다.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static partial class SdlCore
{
    static bool IsOperator(string s) { return s == "+" || s == "-" || s == "*" || s == "/"; }

    public static bool IsExpr(SdlNode n)
    {
        if (n.Kind != "paren" || n.Children.Count == 0) return false;
        var h = Unwrap(n.Children[0]);
        return h.Kind == "atom" && IsOperator(h.Text);
    }

    static string ArgShape(SdlNode n)
    {
        switch (n.Kind)
        {
            case "str": return "str";
            case "atom":
                if (NumberRegex.IsMatch(n.Text)) return "num";
                return n.Text.StartsWith("%") ? "param" : "atom";
            case "call": return "call:" + n.Text.ToLowerInvariant();
            case "paren": return "expr";
            default: return n.Kind;
        }
    }

    static SdlRow Row(string path, string form, List<SdlNode> args)
    {
        var shapes = new List<string>(args.Count);
        var flat = new List<string>();
        foreach (var a in args) { shapes.Add(ArgShape(a)); Flat(a, flat); }
        return new SdlRow { Path = path, Form = form, Shape = string.Join(" ", shapes), Args = string.Join(" ", flat) };
    }

    static bool IsStatement(SdlNode k) { return k.Kind == "brace" || (k.Kind == "paren" && !IsExpr(k)); }

    // 자식을 인자(잎, call, 산술식)와 하위 구문(블록, 괄호 구문)으로 나눈다
    static void Split(List<object> kids, int start, List<SdlNode> args, List<SdlNode> nested)
    {
        for (int i = start; i < kids.Count; i++)
        {
            var k = Unwrap(kids[i]);
            if (IsStatement(k)) nested.Add(k); else args.Add(k);
        }
    }

    static List<SdlNode> Range(List<object> kids, int start)
    {
        var r = new List<SdlNode>();
        for (int i = start; i < kids.Count; i++) r.Add(Unwrap(kids[i]));
        return r;
    }

    static void AddRows(List<SdlNode> nodes, string parent, List<SdlRow> rows, HashSet<string> qualified)
    {
        foreach (var n in nodes)
        {
            if (n.Kind == "brace") AddBrace(n, parent, rows, qualified);
            else if (n.Kind == "paren" && !IsExpr(n)) AddParen(n, parent, rows, qualified);
            else rows.Add(Row(parent + "/<bare>", "bare", new List<SdlNode> { n }));
        }
    }

    static void AddBrace(SdlNode node, string parent, List<SdlRow> rows, HashSet<string> qualified)
    {
        var kids = node.Children;
        string seg = "{}";
        int start = 0;
        if (kids.Count > 0)
        {
            var first = Unwrap(kids[0]);
            if (first.Kind == "atom" || first.Kind == "str")
            {
                string name = first.Text.ToLowerInvariant();
                SdlNode second = kids.Count > 1 ? Unwrap(kids[1]) : null;
                // 따옴표로 된 이름은 데이터로 본다: 경로를 "*"로 묶고 이름은 인자로 남긴다
                if (first.Kind == "str") seg = "\"*\"";
                else if (NumberRegex.IsMatch(name)) seg = "#row";
                else if (qualified.Contains(name) && second != null && (second.Kind == "atom" || second.Kind == "str"))
                {
                    seg = name + "[" + second.Text.Trim('"').ToLowerInvariant() + "]";
                    start = 1;
                }
                else { seg = name; start = 1; }
            }
        }
        string path = parent + "/" + seg;
        var args = new List<SdlNode>(); var nested = new List<SdlNode>();
        Split(kids, start, args, nested);
        rows.Add(Row(path, "block", args));
        AddRows(nested, path, rows, qualified);
    }

    static void AddParen(SdlNode node, string parent, List<SdlRow> rows, HashSet<string> qualified)
    {
        var kids = node.Children;
        if (kids.Count == 0) { rows.Add(Row(parent + "/()", "paren-other", new List<SdlNode>())); return; }
        var head = Unwrap(kids[0]);
        if (head.Kind == "str")
        {
            rows.Add(Row(parent + "/(" + head.Text.ToLowerInvariant() + ")", "macro", Range(kids, 1)));
            return;
        }
        string word = head.Kind == "atom" ? head.Text.ToLowerInvariant() : "";
        if (word == "include") rows.Add(Row(parent + "/(include)", "include", Range(kids, 1)));
        else if (word == "define" && kids.Count >= 2)
        {
            string path = parent + "/(define " + (Unwrap(kids[1])).Text.ToLowerInvariant() + ")";
            rows.Add(Row(path, "define", new List<SdlNode>()));
            AddRows(Range(kids, 2), path, rows, qualified);
        }
        else if (word == "mod")
        {
            var args = new List<SdlNode>(); var nested = new List<SdlNode>();
            Split(kids, 1, args, nested);
            rows.Add(Row(parent + "/(mod)", "mod", args));
            AddRows(nested, parent, rows, qualified);
        }
        else rows.Add(Row(parent + "/" + (word != "" ? "(" + word + ")" : "()"), "paren-other", Range(kids, 1)));
    }

    public static List<SdlRow> KeyRows(IEnumerable topLevel, string[] qualifiedBlocks)
    {
        var rows = new List<SdlRow>();
        var q = new HashSet<string>(qualifiedBlocks ?? new string[0], StringComparer.OrdinalIgnoreCase);
        var nodes = new List<SdlNode>();
        if (topLevel != null) foreach (object x in topLevel) nodes.Add(Unwrap(x));
        AddRows(nodes, "", rows, q);
        return rows;
    }

    // JSON 문자열 리터럴. 제어 문자는 \u00XX로 쓴다
    public static string JsonString(string s)
    {
        var sb = new System.Text.StringBuilder(s.Length + 2);
        sb.Append('"');
        foreach (char ch in s)
        {
            if (ch == '"') sb.Append("\\\"");
            else if (ch == '\\') sb.Append("\\\\");
            else if (ch == '\t') sb.Append("\\t");
            else if (ch == '\n') sb.Append("\\n");
            else if (ch == '\r') sb.Append("\\r");
            else if (ch < ' ') sb.Append("\\u").Append(((int)ch).ToString("x4"));
            else sb.Append(ch);
        }
        sb.Append('"');
        return sb.ToString();
    }

    // 행 목록을 [[Path,Form,Args],...] JSON으로 쓴다
    public static string RowsJson(List<SdlRow> rows)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append('[');
        for (int i = 0; i < rows.Count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append('[').Append(JsonString(rows[i].Path)).Append(',').Append(JsonString(rows[i].Form)).Append(',').Append(JsonString(rows[i].Args)).Append(']');
        }
        sb.Append(']');
        return sb.ToString();
    }
}