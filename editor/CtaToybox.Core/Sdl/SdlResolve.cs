// include, define, 매크로 호출, 전위 산술식을 펼친다. 규칙은 파일에서 추론한 것이며 엔진에서 확인한 것이 아니다.
//   - (include "경로" 이름(값)): 그 자리에 대상 파일의 내용을 넣는다. '/'로 시작하면 루트 기준, 아니면 현재 파일 폴더 기준.
//   - (define "이름" 본문): 이름을 등록하고 출력에는 남기지 않는다.
//   - ("이름" k(v) args a b): 본문으로 바꾸고 %k, %0, %1을 인자로 치환한다.
//   - (+ a b) 등 두 수의 산술식은 계산한다.
//   - 파일이 있는 폴더의 .presets는 파일보다 먼저 읽어 define만 등록한다. 조상 폴더의 것은 읽지 않는다(게임 확인 2026-10-16).
//   - 매크로 본문은 정의할 때 묶여 있던 값과 그 호출의 인자로만 펼친다. 부른 쪽의 인자는 넘기지 않는다(게임 확인 2026-10-16).
//     include는 바깥 인자를 물려준다(게임에서 확인하지 않음).
//   - (mod ...)와 {from "pattern ..."}은 펼치지 않는다.
// 미해결 기록: include:<경로>(대상 없음), include-cycle:, include-depth:, include-unsafe:(작업 폴더 밖),
//              include-parse-error:<경로>(대상에 괄호·따옴표 오류. 읽을 수 있는 데까지는 쓴다),
//              macro:<이름>(정의 없음), macro-cycle:, macro-depth:, param:%<이름>(채워지지 않은 매개변수)
// 물려받은 인자: 매크로나 include가 자기에게 직접 주어지지 않은 값(매크로는 정의할 때 묶인 값, include는 바깥 인자)을 쓴 경우 Inherited에 적는다.
// 출처: 읽은 노드마다 파일의 가상 경로(SdlNode.File)를 적는다. 치환·계산으로 만든 노드는 Synthetic이다.
//       include 지시문의 자리는 IncludeEdges에 적는다. .presets를 읽으며 지나간 include는 적지 않는다.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

// include 지시문 하나. Index와 Raw는 FromFile 안에 있는 경로 문자열 토큰의 자리와 원문(따옴표 포함)이다
public sealed class SdlIncludeEdge
{
    public string FromFile { get; set; }
    public int Index { get; set; }
    public string Raw { get; set; }
    public string ToFile { get; set; }
}

public sealed class SdlResolveResult
{
    public List<object> Nodes { get; set; }
    public List<string> Unresolved { get; set; }
    public List<string> Includes { get; set; }
    public List<string> Macros { get; set; }
    public List<string> Presets { get; set; }
    public List<string> Inherited { get; set; }
    public List<SdlIncludeEdge> IncludeEdges { get; set; }
}

public sealed class SdlResolver
{
    const int MaxDepth = 32;
    static readonly Regex ParamInString = new Regex(@"%(\w+)", RegexOptions.Compiled);
    static readonly Encoding Latin1 = Encoding.GetEncoding(28591);
    static readonly HashSet<string> NoOwn = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    // Bind는 정의할 때 묶여 있던 값이다(include 인자 등). 호출은 이 값과 자기 인자만으로 본문을 펼친다(E13)
    sealed class Def { public List<object> Body; public string Dir; public Dictionary<string, List<object>> Bind; }
    sealed class Parsed { public SdlNode Root; public bool HasErrors; }

    readonly string[] workDirs;   // 앞에 있는 폴더가 우선한다
    readonly Func<string, string> reader;
    readonly Dictionary<string, Parsed> cache = new Dictionary<string, Parsed>(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, Def> defines;
    List<string> unresolved, includes, macros, inherited;
    List<SdlIncludeEdge> edges;
    // 지금 펼치는 중인 include와 매크로. 다시 들어오면 순환이다
    readonly HashSet<string> activeIncludes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> activeMacros = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public SdlResolver(string[] workDirs) { this.workDirs = workDirs; reader = ReadFromDirs; }

    // 가상 경로를 주면 텍스트를, 없으면 null을 돌려주는 함수로 읽는다 (편집기의 원본 계층)
    public SdlResolver(Func<string, string> readText) { workDirs = new string[0]; reader = readText; }

    string ReadFromDirs(string virtualPath)
    {
        string full = Find(virtualPath);
        return full == null ? null : ReadText(full);
    }

    static void Stamp(SdlNode n, string file)
    {
        n.File = file;
        if (n.Children != null) foreach (object c in n.Children) Stamp(SdlCore.Unwrap(c), file);
    }

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

    Parsed Load(string virtualPath, bool useCache)
    {
        Parsed p;
        if (cache.TryGetValue(virtualPath, out p)) return p;
        string text = reader(virtualPath);
        if (text == null) p = null;
        else
        {
            var r = SdlCore.Parse(text);
            Stamp(r.Root, virtualPath);
            p = new Parsed { Root = r.Root, HasErrors = r.Errors.Count > 0 };
        }
        if (useCache) cache[virtualPath] = p;
        return p;
    }

    public SdlNode GetParsed(string virtualPath, bool useCache)
    {
        var p = Load(virtualPath, useCache);
        return p == null ? null : p.Root;
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

    // 작업 폴더 안을 가리키는 경로인지. 드라이브 문자나 UNC가 들어가면 Path.Combine이 폴더 밖으로 나간다
    static bool IsSafeVirtualPath(string vp)
    {
        return vp.Length > 0 && vp.IndexOf(':') < 0 && !vp.StartsWith("/") && !vp.StartsWith("\\");
    }

    static string DirOf(string virtualPath)
    {
        int i = virtualPath.LastIndexOf('/');
        return i < 0 ? "" : virtualPath.Substring(0, i);
    }

    // 묶인 값을 찾는다. 이 호출에 직접 주어진 인자가 아니면(바깥에서 물려받았으면) 기록한다
    bool TryBind(string key, Dictionary<string, List<object>> bind, HashSet<string> own, out List<object> value)
    {
        if (!bind.TryGetValue(key, out value)) return false;
        if (!own.Contains(key)) inherited.Add("%" + key);
        return true;
    }

    // 문자열이나 낱말 안의 %이름을 치환한다.
    // 이름 있는 매개변수는 낱말 전체가 같아야 한다(%door가 묶여 있어도 %door2는 다른 매개변수다).
    // 숫자로 시작하면 위치 인자이므로 앞의 숫자만 이름으로 본다("%0_left"에서 0).
    // 묶인 이름이 없으면 그대로 두고 미해결로 기록한다
    string Substitute(string text, Dictionary<string, List<object>> bind, HashSet<string> own)
    {
        return ParamInString.Replace(text, m =>
        {
            string w = m.Groups[1].Value;
            List<object> b;
            if (TryBind(w, bind, own, out b)) return SdlCore.NodesText(b).Trim('"');
            int digits = 0;
            while (digits < w.Length && w[digits] >= '0' && w[digits] <= '9') digits++;
            if (digits > 0 && digits < w.Length && TryBind(w.Substring(0, digits), bind, own, out b))
                return SdlCore.NodesText(b).Trim('"') + w.Substring(digits);
            unresolved.Add("param:%" + w);
            return m.Value;
        });
    }

    // 계산 결과를 적는다. 소수 15자리까지, 지수 표기 없이. -0은 0으로
    static string FormatNumber(double v)
    {
        string s = v.ToString("0.###############", CultureInfo.InvariantCulture);
        return s == "-0" ? "0" : s;
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

    // own: 지금 펼치는 본문(매크로나 include)에 직접 주어진 인자 이름
    List<object> Expand(List<object> nodes, string dir, Dictionary<string, List<object>> bind, HashSet<string> own, int depth)
    {
        var o = new List<object>();
        foreach (object x in nodes)
        {
            var n = SdlCore.Unwrap(x);
            List<object> bound;
            if (n.Kind == "atom")
            {
                if (n.Text.Length > 1 && n.Text.IndexOf('%') >= 0)
                {
                    if (n.Text[0] == '%' && TryBind(n.Text.Substring(1), bind, own, out bound)) o.AddRange(bound);
                    else
                    {
                        string s = Substitute(n.Text, bind, own);
                        o.Add(s == n.Text ? n : new SdlNode("atom", s, n.Index, false) { File = n.File, Synthetic = true });
                    }
                }
                else o.Add(n);
                continue;
            }
            if (n.Kind == "str")
            {
                if (n.Text.IndexOf('%') >= 0)
                {
                    string text = Substitute(n.Text, bind, own);
                    o.Add(text == n.Text ? n : new SdlNode("str", text, n.Index, false) { File = n.File, Synthetic = true });
                }
                else o.Add(n);
                continue;
            }
            if (n.Kind == "brace" || n.Kind == "call")
            {
                var c = new SdlNode(n.Kind, n.Text, n.Index, true) { File = n.File };
                c.Children.AddRange(Expand(n.Children, dir, bind, own, depth));
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
                if (!IsSafeVirtualPath(vp)) { unresolved.Add("include-unsafe:" + vp); o.Add(n); continue; }
                if (activeIncludes.Contains(vp)) { unresolved.Add("include-cycle:" + vp); o.Add(n); continue; }
                if (depth >= MaxDepth) { unresolved.Add("include-depth:" + vp); o.Add(n); continue; }
                Parsed target = Load(vp, true);
                if (target == null) { unresolved.Add("include:" + vp); o.Add(n); continue; }
                if (target.HasErrors) unresolved.Add("include-parse-error:" + vp);
                includes.Add(vp);
                var pathNode = SdlCore.Unwrap(kids[1]);
                edges.Add(new SdlIncludeEdge { FromFile = n.File, Index = pathNode.Index, Raw = pathNode.Text, ToFile = vp });
                var b = NewBind(bind);
                var ownHere = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 2; i < kids.Count; i++)
                {
                    var k = SdlCore.Unwrap(kids[i]);
                    if (k.Kind == "call") { b[k.Text] = Expand(k.Children, dir, bind, own, depth); ownHere.Add(k.Text); }
                }
                activeIncludes.Add(vp);
                try { o.AddRange(Expand(target.Root.Children, DirOf(vp), b, ownHere, depth + 1)); }
                finally { activeIncludes.Remove(vp); }
                continue;
            }
            if (word == "define" && kids.Count >= 2)
            {
                defines[SdlCore.Unwrap(kids[1]).Text.Trim('"')] = new Def { Body = kids.GetRange(2, kids.Count - 2), Dir = dir, Bind = bind };
                continue;
            }
            if (head != null && head.Kind == "str")
            {
                string name = head.Text.Trim('"');
                Def def;
                if (activeMacros.Contains(name)) { unresolved.Add("macro-cycle:" + name); o.Add(n); continue; }
                if (depth >= MaxDepth) { unresolved.Add("macro-depth:" + name); o.Add(n); continue; }
                if (!defines.TryGetValue(name, out def)) { unresolved.Add("macro:" + name); o.Add(n); continue; }
                macros.Add(name);
                // 부른 쪽의 인자는 넘기지 않는다: 게임에서 매크로 안의 매크로가 바깥 인자를 받지 못해 꺼졌다(2026-10-16). 인자 값은 부른 쪽에서 계산한다
                var b = NewBind(def.Bind);
                var ownHere = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int pos = -1;
                for (int i = 1; i < kids.Count; i++)
                {
                    var k = SdlCore.Unwrap(kids[i]);
                    if (k.Kind == "call") { b[k.Text] = Expand(k.Children, dir, bind, own, depth); ownHere.Add(k.Text); }
                    else if (pos < 0 && k.Kind == "atom" && string.Equals(k.Text, "args", StringComparison.OrdinalIgnoreCase)) pos = 0;
                    else if (pos >= 0)
                    {
                        string key = pos.ToString(CultureInfo.InvariantCulture);
                        b[key] = Expand(new List<object> { k }, dir, bind, own, depth);
                        ownHere.Add(key);
                        pos++;
                    }
                }
                activeMacros.Add(name);
                try { o.AddRange(Expand(def.Body, def.Dir, b, ownHere, depth + 1)); }
                finally { activeMacros.Remove(name); }
                continue;
            }

            var p = new SdlNode("paren", "", n.Index, true) { File = n.File };
            p.Children.AddRange(Expand(kids, dir, bind, own, depth));
            if (SdlCore.IsExpr(p))
            {
                double? v = Eval(p);
                if (v.HasValue) { o.Add(new SdlNode("atom", FormatNumber(v.Value), n.Index, false) { File = n.File, Synthetic = true }); continue; }
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
        unresolved = new List<string>(); includes = new List<string>(); macros = new List<string>(); inherited = new List<string>();
        edges = new List<SdlIncludeEdge>();
        var presets = new List<string>();
        activeIncludes.Clear(); activeMacros.Clear();

        string dir = DirOf(virtualPath);
        // 파일이 있는 폴더의 .presets만 읽는다. 조상 폴더의 것은 게임이 쓰지 않았다(2026-10-16 "define not found")
        if (dir.Length > 0)
        {
            string pv = dir + "/.presets";
            SdlNode proot = string.Equals(pv, virtualPath, StringComparison.OrdinalIgnoreCase) ? null : GetParsed(pv, true);
            if (proot != null) { presets.Add(pv); Expand(proot.Children, dir, NewBind(null), NoOwn, 1); }
        }
        // .presets를 읽으며 쌓인 기록은 파일 자체의 것이 아니다
        includes.Clear(); macros.Clear(); inherited.Clear(); edges.Clear();

        activeIncludes.Add(virtualPath);   // 자기 자신을 다시 include하면 순환이다
        SdlNode root = GetParsed(virtualPath, false);
        var nodes = root == null ? new List<object>() : Expand(root.Children, dir, NewBind(null), NoOwn, 0);
        return new SdlResolveResult
        {
            Nodes = nodes, Unresolved = SortedDistinct(unresolved), Includes = SortedDistinct(includes),
            Macros = SortedDistinct(macros), Presets = presets, Inherited = SortedDistinct(inherited), IncludeEdges = edges
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
