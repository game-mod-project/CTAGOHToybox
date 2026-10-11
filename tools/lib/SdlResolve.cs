// include, define, 매크로 호출, 전위 산술식을 펼친다. 규칙은 파일에서 추론한 것이며 엔진에서 확인한 것이 아니다.
//   - (include "경로" 이름(값)): 그 자리에 대상 파일의 내용을 넣는다. '/'로 시작하면 루트 기준, 아니면 현재 파일 폴더 기준.
//   - (define "이름" 본문): 이름을 등록하고 출력에는 남기지 않는다.
//   - ("이름" k(v) args a b): 본문으로 바꾸고 %k, %0, %1을 인자로 치환한다.
//   - (+ a b) 등 두 수의 산술식은 계산한다.
//   - 조상 폴더의 .presets는 파일보다 먼저 읽어 define만 등록한다.
//   - (mod ...)와 {from "pattern ..."}은 펼치지 않는다.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public sealed class SdlResolveResult
{
    public List<object> Nodes { get; set; }
    public List<string> Unresolved { get; set; }   // include:<경로> | include-depth:<경로> | macro:<이름> | param:%<이름>
    public List<string> Includes { get; set; }
    public List<string> Macros { get; set; }
    public List<string> Presets { get; set; }
}

public sealed class SdlResolver
{
    const int MaxDepth = 32;
    static readonly Regex ParamInString = new Regex(@"%(\w+)", RegexOptions.Compiled);
    static readonly Encoding Latin1 = Encoding.GetEncoding(28591);

    sealed class Def { public List<object> Body; public string Dir; }

    readonly string[] workDirs;   // 앞에 있는 폴더가 우선한다
    readonly Dictionary<string, SdlNode> cache = new Dictionary<string, SdlNode>(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, Def> defines;
    List<string> unresolved, includes, macros;

    public SdlResolver(string[] workDirs) { this.workDirs = workDirs; }

    public static string ReadText(string path)
    {
        byte[] b = File.ReadAllBytes(path);
        if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) return Encoding.Unicode.GetString(b, 2, b.Length - 2);
        if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) return Encoding.UTF8.GetString(b, 3, b.Length - 3);
        return Latin1.GetString(b);
    }

    string Find(string virtualPath)
    {
        string rel = virtualPath.Replace('/', '\\');
        foreach (string d in workDirs)
        {
            string full = Path.Combine(d, rel);
            if (File.Exists(full)) return full;
        }
        return null;
    }

    public SdlNode GetParsed(string virtualPath, bool useCache)
    {
        SdlNode root;
        if (cache.TryGetValue(virtualPath, out root)) return root;
        string full = Find(virtualPath);
        root = full == null ? null : SdlCore.Parse(ReadText(full)).Root;
        if (useCache) cache[virtualPath] = root;
        return root;
    }

    public static string IncludePath(string raw, string fromDir)
    {
        string p = raw.Trim('"').Replace('\\', '/');
        string joined = p.StartsWith("/") ? p.TrimStart('/') : (string.IsNullOrEmpty(fromDir) ? p : fromDir + "/" + p);
        var parts = new List<string>();
        foreach (string s in joined.Split('/'))
        {
            if (s == "..") { if (parts.Count > 0) parts.RemoveAt(parts.Count - 1); }
            else if (s.Length > 0 && s != ".") parts.Add(s);
        }
        return string.Join("/", parts).ToLowerInvariant();
    }

    static string DirOf(string virtualPath)
    {
        int i = virtualPath.LastIndexOf('/');
        return i < 0 ? "" : virtualPath.Substring(0, i);
    }

    // 두 피연산자의 전위 산술식. 계산할 수 없으면 null
    static double? Eval(SdlNode n)
    {
        var k = n.Children;
        if (k.Count != 3) return null;
        var vals = new double[2];
        for (int i = 0; i < 2; i++)
        {
            var o = SdlCore.Unwrap(k[i + 1]);
            if (o.Kind == "atom")
            {
                double d;
                if (!double.TryParse(o.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) return null;
                vals[i] = d;
            }
            else if (SdlCore.IsExpr(o))
            {
                double? r = Eval(o);
                if (!r.HasValue) return null;
                vals[i] = r.Value;
            }
            else return null;
        }
        switch (SdlCore.Unwrap(k[0]).Text)
        {
            case "+": return vals[0] + vals[1];
            case "-": return vals[0] - vals[1];
            case "*": return vals[0] * vals[1];
            default: return vals[1] == 0 ? (double?)null : vals[0] / vals[1];
        }
    }

    static Dictionary<string, List<object>> NewBind(Dictionary<string, List<object>> from)
    {
        return from == null
            ? new Dictionary<string, List<object>>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, List<object>>(from, StringComparer.OrdinalIgnoreCase);
    }

    List<object> Expand(List<object> nodes, string dir, Dictionary<string, List<object>> bind, int depth)
    {
        var o = new List<object>();
        foreach (object x in nodes)
        {
            var n = SdlCore.Unwrap(x);
            List<object> bound;
            if (n.Kind == "atom")
            {
                if (n.Text.Length > 1 && n.Text[0] == '%')
                {
                    if (bind.TryGetValue(n.Text.Substring(1), out bound)) o.AddRange(bound);
                    else { unresolved.Add("param:" + n.Text); o.Add(n); }
                }
                else o.Add(n);
                continue;
            }
            if (n.Kind == "str")
            {
                if (n.Text.IndexOf('%') >= 0)
                {
                    string text = ParamInString.Replace(n.Text, m =>
                    {
                        List<object> b;
                        return bind.TryGetValue(m.Groups[1].Value, out b) ? SdlCore.NodesText(b).Trim('"') : m.Value;
                    });
                    o.Add(new SdlNode("str", text, n.Index, false));
                }
                else o.Add(n);
                continue;
            }
            if (n.Kind == "brace" || n.Kind == "call")
            {
                var c = new SdlNode(n.Kind, n.Text, n.Index, true);
                c.Children.AddRange(Expand(n.Children, dir, bind, depth));
                o.Add(c);
                continue;
            }

            // paren
            var kids = n.Children;
            SdlNode head = kids.Count > 0 ? SdlCore.Unwrap(kids[0]) : null;
            string word = head != null && head.Kind == "atom" ? head.Text.ToLowerInvariant() : "";

            if (word == "include" && kids.Count >= 2 && SdlCore.Unwrap(kids[1]).Kind == "str")
            {
                string vp = IncludePath(SdlCore.Unwrap(kids[1]).Text, dir);
                if (depth >= MaxDepth) { unresolved.Add("include-depth:" + vp); o.Add(n); continue; }
                SdlNode root = GetParsed(vp, true);
                if (root == null) { unresolved.Add("include:" + vp); o.Add(n); continue; }
                includes.Add(vp);
                var b = NewBind(bind);
                for (int i = 2; i < kids.Count; i++)
                {
                    var k = SdlCore.Unwrap(kids[i]);
                    if (k.Kind == "call") b[k.Text] = Expand(k.Children, dir, bind, depth);
                }
                o.AddRange(Expand(root.Children, DirOf(vp), b, depth + 1));
                continue;
            }
            if (word == "define" && kids.Count >= 2)
            {
                defines[SdlCore.Unwrap(kids[1]).Text.Trim('"')] = new Def { Body = kids.GetRange(2, kids.Count - 2), Dir = dir };
                continue;
            }
            if (head != null && head.Kind == "str")
            {
                string name = head.Text.Trim('"');
                Def def;
                if (depth >= MaxDepth || !defines.TryGetValue(name, out def)) { unresolved.Add("macro:" + name); o.Add(n); continue; }
                macros.Add(name);
                var b = NewBind(bind);
                int pos = -1;
                for (int i = 1; i < kids.Count; i++)
                {
                    var k = SdlCore.Unwrap(kids[i]);
                    if (k.Kind == "call") b[k.Text] = Expand(k.Children, dir, bind, depth);
                    else if (pos < 0 && k.Kind == "atom" && string.Equals(k.Text, "args", StringComparison.OrdinalIgnoreCase)) pos = 0;
                    else if (pos >= 0) { b[pos.ToString(CultureInfo.InvariantCulture)] = Expand(new List<object> { k }, dir, bind, depth); pos++; }
                }
                o.AddRange(Expand(def.Body, def.Dir, b, depth + 1));
                continue;
            }

            var p = new SdlNode("paren", "", n.Index, true);
            p.Children.AddRange(Expand(kids, dir, bind, depth));
            if (SdlCore.IsExpr(p))
            {
                double? v = Eval(p);
                if (v.HasValue) { o.Add(new SdlNode("atom", v.Value.ToString("0.######", CultureInfo.InvariantCulture), n.Index, false)); continue; }
            }
            o.Add(p);
        }
        return o;
    }

    static List<string> SortedDistinct(List<string> items)
    {
        var r = new List<string>(new HashSet<string>(items, StringComparer.Ordinal));
        r.Sort(StringComparer.Ordinal);
        return r;
    }

    public SdlResolveResult Resolve(string virtualPath)
    {
        defines = new Dictionary<string, Def>(StringComparer.OrdinalIgnoreCase);
        unresolved = new List<string>(); includes = new List<string>(); macros = new List<string>();
        var presets = new List<string>();

        string dir = DirOf(virtualPath);
        string acc = "";
        foreach (string seg in dir.Split('/'))
        {
            if (seg.Length == 0) continue;
            acc = acc.Length > 0 ? acc + "/" + seg : seg;
            string pv = acc + "/.presets";
            if (string.Equals(pv, virtualPath, StringComparison.OrdinalIgnoreCase)) continue;
            SdlNode proot = GetParsed(pv, true);
            if (proot != null) { presets.Add(pv); Expand(proot.Children, acc, NewBind(null), 1); }
        }
        // .presets를 읽으며 쌓인 include/매크로 기록은 파일 자체의 것이 아니다
        includes.Clear(); macros.Clear();

        SdlNode root = GetParsed(virtualPath, false);
        var nodes = root == null ? new List<object>() : Expand(root.Children, dir, NewBind(null), 0);
        return new SdlResolveResult
        {
            Nodes = nodes, Unresolved = SortedDistinct(unresolved), Includes = SortedDistinct(includes),
            Macros = SortedDistinct(macros), Presets = presets
        };
    }
}

// 키 행을 (범위, 경로, 형태)별로 모은다
public sealed class SdlKeyAggregate
{
    public string Scope { get; set; }
    public string Path { get; set; }
    public string Form { get; set; }
    public int Count { get; set; }
    public int FileCount { get; set; }
    public string ArgShapes { get; set; }
    public double? NumMin { get; set; }
    public double? NumMax { get; set; }
    public string Values { get; set; }
    public string ExampleFile { get; set; }
    public string ExampleArgs { get; set; }
    internal string LastFile = "";
    internal Dictionary<string, int> Shapes = new Dictionary<string, int>(StringComparer.Ordinal);
    internal Dictionary<string, int> ValueCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
}

public sealed class SdlKeyAggregator
{
    const int MaxValues = 12;
    readonly Dictionary<string, SdlKeyAggregate> map = new Dictionary<string, SdlKeyAggregate>(StringComparer.OrdinalIgnoreCase);

    public void Add(string scope, SdlRow row, string file)
    {
        string key = scope + "\t" + row.Path + "\t" + row.Form;
        SdlKeyAggregate a;
        if (!map.TryGetValue(key, out a))
        {
            a = new SdlKeyAggregate { Scope = scope, Path = row.Path, Form = row.Form, ExampleFile = file, ExampleArgs = row.Args };
            map[key] = a;
        }
        a.Count++;
        if (a.LastFile != file) { a.FileCount++; a.LastFile = file; }
        int c;
        a.Shapes[row.Shape] = a.Shapes.TryGetValue(row.Shape, out c) ? c + 1 : 1;
        if (a.ValueCounts.Count <= MaxValues) a.ValueCounts[row.Args] = a.ValueCounts.TryGetValue(row.Args, out c) ? c + 1 : 1;
        if (row.Shape == "num")
        {
            double d = double.Parse(row.Args, NumberStyles.Float, CultureInfo.InvariantCulture);
            if (!a.NumMin.HasValue || d < a.NumMin.Value) a.NumMin = d;
            if (!a.NumMax.HasValue || d > a.NumMax.Value) a.NumMax = d;
        }
    }

    public void AddRows(string scope, List<SdlRow> rows, string file)
    {
        foreach (var r in rows) Add(scope, r, file);
    }

    public List<SdlKeyAggregate> Export()
    {
        var result = new List<SdlKeyAggregate>(map.Count);
        foreach (var a in map.Values)
        {
            var shapes = new List<KeyValuePair<string, int>>(a.Shapes);
            shapes.Sort((x, y) => y.Value.CompareTo(x.Value));
            var sb = new StringBuilder();
            for (int i = 0; i < shapes.Count && i < 5; i++) { if (i > 0) sb.Append(' '); sb.Append('[').Append(shapes[i].Key).Append("]x").Append(shapes[i].Value); }
            a.ArgShapes = sb.ToString();
            if (a.ValueCounts.Count > MaxValues) a.Values = "(13종 이상)";
            else
            {
                var vals = new List<KeyValuePair<string, int>>(a.ValueCounts);
                vals.Sort((x, y) => y.Value.CompareTo(x.Value));
                var names = new List<string>();
                foreach (var v in vals) names.Add(v.Key);
                a.Values = string.Join(" ¦ ", names);
            }
            result.Add(a);
        }
        return result;
    }
}
