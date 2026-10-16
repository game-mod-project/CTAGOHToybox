// 유닛을 펼친 값마다 그 값이 원본의 어느 파일, 어느 키 경로, 몇 번째 값인지 찾는다.
// 리졸버가 노드에 적어 둔 파일과 글자 위치로 그 파일의 KeyLocator 행을 거꾸로 찾는다.
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class TraceValue
    {
        public string Text { get; set; }
        public string File { get; set; }        // 값이 있는 파일
        public string KeyPath { get; set; }     // 그 파일 안의 키 경로 (Edit.Path)
        public int Occurrence { get; set; }     // 그 파일 안에서 같은 경로 가운데 몇 번째인지 (Edit.Occurrence)
        public int Arg { get; set; }            // 그 행의 몇 번째 값인지 (Edit.Arg). -1이면 편집할 수 없는 자리다
        public bool Synthetic { get; set; }
        public string Note { get; set; }        // 편집할 수 없는 이유
        public string NamedPath { get; set; }   // 따옴표 이름 블록 안이면 이름이 든 원문 경로(Edit.NamedPath). 아니면 null
        public int NamedOccurrence { get; set; }
    }

    public sealed class TraceRow
    {
        public string Path { get; set; }        // 펼친 뒤의 키 경로
        public string RawPath { get; set; }     // 원문의 대소문자 그대로 쓴 경로(화면 표시용)
        public int Occurrence { get; set; }     // 펼친 뒤의 순번
        public string Form { get; set; }        // block | macro (KeyLocator의 행 형태). 주석을 찾을 때 쓴다
        public string Context { get; set; }     // 같은 키가 여러 번 나올 때 어느 부위의 것인지(예: volume body1). 없으면 null
        public string Cond { get; set; }        // 감싼 (mod ...) 조건 원문. 조건 밖이면 null
        public List<TraceValue> Values { get; } = new List<TraceValue>();
        public List<string> LeafNames { get; set; }   // 값마다 감싼 호출의 이름(KeyRow.LeafNames). 호출 밖이면 null
        public string Comment { get; set; }        // Lua 설정의 개발사 주석(그 줄). 없으면 null
        public string TableComment { get; set; }   // Lua 설정의 표 앞 주석. 없으면 null
        public string From { get; set; }        // 원형 상속으로 물려받은 줄이면 그 원형 파일(FromLevel.File). 유닛 자신의 줄이면 null
        public string FromState { get; set; }   // FromLevel.Inherited | Overridden | Unconfirmed. 유닛 자신의 줄이면 null
        public string FromBlock { get; set; }   // Unconfirmed일 때 자식 쪽에도 있는 감싼 블록
    }

    public sealed class UnitTraceResult
    {
        public string Unit { get; set; }
        public List<TraceRow> Rows { get; } = new List<TraceRow>();
        public List<string> Unresolved { get; set; }
        public string ReadError { get; set; }     // 파일을 읽지 못해 줄이 없을 때의 안내(Lua 설정). 읽었으면 null
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
        public List<object> Nodes { get; set; }   // 펼친 노드. 아이템 이름을 모을 때 쓴다
        public List<FromLevel> From { get; set; } = new List<FromLevel>();   // {from}으로 물려받는 원형 파일, 가까운 것부터. 물려받지 않으면 빈 목록
    }

    public sealed class UnitTracer
    {
        sealed class Slot { public string Path; public int Occurrence; public int Arg; public string RawPath; public int RawOccurrence; }

        readonly SourceStack stack;
        // 파일 → (토큰의 글자 위치 → 자리). 파싱되지 않는 파일은 빈 표다
        readonly Dictionary<string, Dictionary<int, Slot>> slots = new Dictionary<string, Dictionary<int, Slot>>(StringComparer.Ordinal);

        FromLinks links;

        public UnitTracer(SourceStack stack) { this.stack = stack; }

        FromLinks Links { get { return links ?? (links = new FromLinks(stack.Paths)); } }

        Dictionary<int, Slot> SlotsOf(string file)
        {
            Dictionary<int, Slot> map;
            if (slots.TryGetValue(file, out map)) return map;
            map = new Dictionary<int, Slot>();
            string text = stack.ReadText(file);
            if (text != null)
            {
                var parsed = SdlCore.Parse(text);
                if (parsed.Errors.Count == 0)
                    foreach (var row in KeyLocator.Rows(parsed.Root))
                        for (int i = 0; i < row.Leaves.Count; i++)
                            map[row.Leaves[i].Index] = new Slot { Path = row.Path, Occurrence = row.Occurrence, Arg = i, RawPath = row.RawPath, RawOccurrence = row.RawOccurrence };
            }
            slots[file] = map;
            return map;
        }

        // 따옴표 이름 블록의 이름 자체({"human" ...}의 "human"). 다른 파일이 이 이름으로 부른다(승무원의 {vision "tank_range_200"} 등)
        public const string BlockNameNote = "블록 이름이다. 다른 파일이 이 이름으로 부르므로(예: {vision \"이름\"}) 바꾸면 이어지지 않는다";

        TraceValue ValueOf(SdlNode leaf)
        {
            var v = new TraceValue { Text = leaf.Text, File = leaf.File, Arg = -1, Synthetic = leaf.Synthetic };
            if (leaf.Synthetic) { v.Note = "치환이나 계산으로 만든 값이다. 원본의 인자나 피연산자를 고친다"; return v; }
            if (leaf.File == null) { v.Note = "출처를 알 수 없다"; return v; }
            Slot slot;
            if (!SlotsOf(leaf.File).TryGetValue(leaf.Index, out slot)) { v.Note = "편집할 수 있는 자리가 아니다(include 인자이거나 파싱되지 않는 파일)"; return v; }
            v.KeyPath = slot.Path; v.Occurrence = slot.Occurrence;
            if (KeyLocator.IsDisabled(slot.Path)) { v.Note = KeyLocator.DisabledNote; return v; }
            if (slot.Arg == 0 && slot.Path.EndsWith("/\"*\"", StringComparison.Ordinal)) { v.Note = BlockNameNote; return v; }
            v.Arg = slot.Arg;
            if (KeyLocator.IsNamed(slot.Path)) { v.NamedPath = slot.RawPath; v.NamedOccurrence = slot.RawOccurrence; }
            return v;
        }

        // 부위를 나타내는 블록. 첫 문자열이 그 부위의 이름이다(예: {volume "body1" ...}, {place "gun" ...}).
        // parameters는 무기의 탄종별 값 묶음이다(예: {parameters "ap2" ...}). chassis "wheel"처럼 종류를 적는 블록과 extender "inventory" 같은 기능 블록은 넣지 않는다
        static readonly HashSet<string> PartBlocks = new HashSet<string>(StringComparer.Ordinal)
            { "volume", "component", "place", "bone", "visor", "track", "anm", "link", "setting", "parameters" };

        // 행마다 가장 가까운 이름 있는 부위 블록. rows와 같은 순서다. 인자 없는 블록도 거쳐 가므로 앞 형제의 이름을 물려받지 않는다
        public static List<string> ContextsOf(List<KeyRow> rows)
        {
            var result = new List<string>(rows.Count);
            var named = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var row in rows)
            {
                string context = null;
                for (int i = row.Path.LastIndexOf('/'); i > 0 && context == null; i = row.Path.LastIndexOf('/', i - 1))
                {
                    string label;
                    if (named.TryGetValue(row.Path.Substring(0, i), out label)) context = label;
                }
                result.Add(context);
                // 이 블록이 열렸으므로 같은 경로 아래에 남은 앞 형제의 이름을 지운다
                foreach (string k in named.Keys.Where(k => k.StartsWith(row.Path + "/", StringComparison.Ordinal)).ToList()) named.Remove(k);
                string seg = row.Path.Substring(row.Path.LastIndexOf('/') + 1);
                var first = row.Leaves.Count > 0 ? row.Leaves[0] : null;
                bool part = row.Path.LastIndexOf('/') > 0 && PartBlocks.Contains(seg) && first != null && first.Kind == "str";
                named[row.Path] = part ? seg + " " + first.Text.Trim('"') : null;
            }
            return result;
        }

        // 원형 상속에서 같은 키인지 가르는 경로. 이름 있는 부위 블록은 이름을 붙인다({parameters "ap2" {speed}} → /from/parameters"ap2"/speed).
        // 탄종이 다른 parameters 블록은 서로 다른 블록이다. rows와 같은 순서다
        public static List<string> KeyedPathsOf(List<KeyRow> rows)
        {
            var result = new List<string>(rows.Count);
            var latest = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in rows)
            {
                int slash = row.Path.LastIndexOf('/');
                string parent = row.Path.Substring(0, Math.Max(slash, 0)), parentKeyed;
                if (!latest.TryGetValue(parent, out parentKeyed)) parentKeyed = parent;
                string seg = row.Path.Substring(slash + 1);
                var first = row.Leaves.Count > 0 ? row.Leaves[0] : null;
                bool part = slash > 0 && PartBlocks.Contains(seg) && first != null && first.Kind == "str";
                string keyed = parentKeyed + "/" + seg + (part ? "\"" + first.Text.Trim('"').Replace('/', '|') + "\"" : "");
                // 따옴표 이름 블록({"head" 1.3})은 "*"로 묶여 있으므로 원문의 이름으로 가른다(E15)
                if (seg == "\"*\"" && row.RawPath != null)
                {
                    string raw = row.RawPath.Substring(FromLevel.LastSlash(row.RawPath) + 1);
                    if (raw.Length >= 2 && raw[0] == '"') keyed = parentKeyed + "/\"" + raw.Trim('"').ToLowerInvariant().Replace('/', '|') + "\"";
                }
                latest[row.Path] = keyed;
                result.Add(keyed);
            }
            return result;
        }

        // 유닛 파일이 없으면 null
        public UnitTraceResult Trace(string unit)
        {
            unit = SourceStack.ToVirtual(unit);
            if (stack.Find(unit) == null) return null;
            if (LuaTable.IsLua(unit)) return TraceLua(unit);
            var resolver = new SdlResolver(stack.ReadText);
            var resolved = resolver.Resolve(unit);
            var root = new SdlNode("root", "", 0, true);
            root.Children.AddRange(resolved.Nodes);
            var result = new UnitTraceResult { Unit = unit, Unresolved = resolved.Unresolved, IncludeEdges = resolved.IncludeEdges, Nodes = resolved.Nodes };
            var rows = KeyLocator.Rows(root);
            var contexts = ContextsOf(rows);
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row.Leaves.Count == 0) continue;
                var traced = new TraceRow { Path = row.Path, RawPath = row.RawPath, LeafNames = row.LeafNames, Occurrence = row.Occurrence, Form = row.Form, Context = contexts[i], Cond = row.Cond };
                foreach (var leaf in row.Leaves) traced.Values.Add(ValueOf(leaf));
                result.Rows.Add(traced);
            }
            AddInherited(result, rows, resolver);
            return result;
        }

        const string Top = "/from";

        // {from "이름" ...}으로 시작하는 유닛: 사슬의 원형 파일마다 따로 펼쳐 그 줄을 맨 위 블록을 from으로 바꿔 붙인다(E11).
        // 이름을 찾지 못하거나 같은 파일이 다시 나오면 풀리지 않은 구문으로 알리고 멈춘다
        void AddInherited(UnitTraceResult result, List<KeyRow> ownRows, SdlResolver resolver)
        {
            // 원형 상속은 아이템 파일(set/stuff)의 규칙이다. 그 밖의 {from}은 펼치지도 풀리지 않은 구문으로 알리지도 않는다(E15)
            if (!result.Unit.StartsWith("set/stuff/", StringComparison.Ordinal)) return;
            string name = FromLinks.NameOf(ownRows);
            if (name == null) return;
            string rawTop = ownRows.First(r => r.Path == Top && r.Occurrence == 0).RawPath;
            var below = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var plain = new HashSet<string>(StringComparer.OrdinalIgnoreCase);   // 조건 밖에서도 나온 경로
            var ownKeyed = KeyedPathsOf(ownRows);
            for (int i = 0; i < ownRows.Count; i++) { FromLevel.Count(below, ownKeyed[i]); if (ownRows[i].Cond == null) plain.Add(ownKeyed[i]); }
            foreach (string f in new[] { result.Unit }.Concat(result.IncludeEdges.Select(e => e.ToFile)).Distinct())
                foreach (var c in MacroCalls(f, false, null)) { FromLevel.Count(below, c.Keyed); if (c.Cond == null) plain.Add(c.Keyed); }
            var seen = new HashSet<string>(StringComparer.Ordinal) { result.Unit };
            string current = result.Unit;
            while (name != null)
            {
                string parent = Links.Find(current, name);
                if (parent == null) { result.Unresolved.Add("from-not-found: " + name + " (" + current + ")"); return; }
                if (!seen.Add(parent)) { result.Unresolved.Add("from-cycle: " + name + " (" + current + ")"); return; }
                var resolved = resolver.Resolve(parent);
                var root = new SdlNode("root", "", 0, true);
                root.Children.AddRange(resolved.Nodes);
                var rows = KeyLocator.Rows(root);
                var level = new FromLevel { File = parent, Name = name, Below = new Dictionary<string, int>(below, StringComparer.OrdinalIgnoreCase), Own = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase),
                    BelowPlain = new HashSet<string>(plain, StringComparer.OrdinalIgnoreCase), RawTop = rawTop };
                level.Files = new[] { parent }.Concat(resolved.IncludeEdges.Select(e => e.ToFile)).Distinct().ToList();
                level.Candidates = Links.Candidates(current, name);
                foreach (var kv in resolved.Defines ?? new Dictionary<string, List<object>>()) level.MacroKeys[kv.Key] = BodyKeys(kv.Value);
                var keyed = KeyedPathsOf(rows).Select(p => FromLinks.Reroot(p, Top)).ToList();
                AddIncludeAt(level, rows, keyed, rawTop);
                var ownPlain = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < rows.Count; i++)
                    if (keyed[i] != null) { FromLevel.Count(level.Own, keyed[i]); if (rows[i].Cond == null) ownPlain.Add(keyed[i]); }
                foreach (string f in level.Files)
                    foreach (var c in MacroCalls(f, f == parent, level)) { FromLevel.Count(level.Own, c.Keyed); if (c.Cond == null) ownPlain.Add(c.Keyed); }
                result.From.Add(level);
                foreach (string u in resolved.Unresolved) result.Unresolved.Add(u + " (원형 " + parent + ")");
                var contexts = ContextsOf(rows);
                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    string path = FromLinks.Reroot(row.Path, Top);
                    if (path == null || row.Leaves.Count == 0) continue;
                    string block;
                    string state = level.StateOf(keyed[i], out block);
                    var traced = new TraceRow { Path = path, RawPath = FromLinks.Reroot(row.RawPath, rawTop), LeafNames = row.LeafNames, Occurrence = row.Occurrence, Form = row.Form, Context = contexts[i], Cond = row.Cond, From = parent, FromState = state, FromBlock = block };
                    foreach (var leaf in row.Leaves) traced.Values.Add(ValueOf(leaf));
                    result.Rows.Add(traced);
                }
                // 단계마다 따로 센다: 유닛이 한 번, 가까운 원형이 한 번 적은 키는 여러 번 적는 키가 아니다
                foreach (var kv in level.Own) { int n; below.TryGetValue(kv.Key, out n); below[kv.Key] = Math.Max(n, kv.Value); }
                plain.UnionWith(ownPlain);
                name = FromLinks.NameOf(rows);
                current = parent;
            }
        }

        // 매크로 본문이 바로 쓰는 키(비교용 경로의 마지막 조각). 본문에 다른 호출·include가 있으면 다 알 수 없으므로 null
        static List<string> BodyKeys(List<object> body)
        {
            var root = new SdlNode("root", "", 0, true);
            root.Children.AddRange(body);
            var rows = KeyLocator.Rows(root);
            var keyed = KeyedPathsOf(rows);
            var keys = new List<string>();
            for (int i = 0; i < rows.Count; i++)
            {
                if (FromLevel.LastSlash(rows[i].Path) != 0) continue;   // 맨 위 줄만
                if (rows[i].Form != "block") return null;
                string k = keyed[i].Substring(1);
                if (!keys.Contains(k)) keys.Add(k);
            }
            return keys;
        }

        // 원형이 include한 파일마다 그 내용이 펼쳐진 자리(맨 위 블록을 from으로 바꾼 경로의 앞부분). 그 파일의 값이 펼친 줄에 하나도 없으면 모른다(E15)
        void AddIncludeAt(FromLevel level, List<KeyRow> rows, List<string> keyed, string rawTop)
        {
            foreach (string f in level.Files.Skip(1))
            {
                var fileRows = FileRows(f);
                if (fileRows == null) continue;
                var fileKeyed = KeyedPathsOf(fileRows);
                for (int i = 0; i < rows.Count && !level.IncludeAt.ContainsKey(f); i++)
                {
                    if (keyed[i] == null) continue;
                    var leaf = rows[i].Leaves.FirstOrDefault(l => l.File == f && !l.Synthetic);
                    Slot slot;
                    if (leaf == null || !SlotsOf(f).TryGetValue(leaf.Index, out slot)) continue;
                    int j = fileRows.FindIndex(r => r.Path == slot.Path && r.Occurrence == slot.Occurrence);
                    string path = FromLinks.Reroot(rows[i].Path, Top), raw = FromLinks.Reroot(rows[i].RawPath, rawTop);
                    if (j < 0 || path == null || raw == null || !path.EndsWith(slot.Path, StringComparison.OrdinalIgnoreCase) || !keyed[i].EndsWith(fileKeyed[j], StringComparison.OrdinalIgnoreCase)) continue;
                    level.IncludeAt[f] = new[] { path.Substring(0, path.Length - slot.Path.Length), raw.Substring(0, Math.Max(0, raw.Length - slot.RawPath.Length)), keyed[i].Substring(0, keyed[i].Length - fileKeyed[j].Length) };
                }
            }
        }

        List<KeyRow> FileRows(string file)
        {
            if (LuaTable.IsLua(file)) return null;
            string text = stack.ReadText(file);
            if (text == null) return null;
            var parsed = SdlCore.Parse(text);
            return parsed.Errors.Count > 0 ? null : KeyLocator.Rows(parsed.Root);
        }

        sealed class CallPath { public string Keyed; public string Cond; }

        // 파일 원문의 매크로 호출 줄의 비교용 경로(KeyedPathsOf, 매크로 정의 안은 뺀다)와 조건. reroot면 맨 위 블록을 from으로 바꾼다.
        // 원형이 include한 파일이면 그 내용이 펼쳐진 자리를 앞에 붙인다(level.IncludeAt)
        IEnumerable<CallPath> MacroCalls(string file, bool reroot, FromLevel level)
        {
            var rows = FileRows(file);
            if (rows == null) yield break;
            var keyed = KeyedPathsOf(rows);
            string[] at = null;
            if (level != null && !reroot) level.IncludeAt.TryGetValue(file, out at);
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].Form != "macro" || rows[i].Path.Contains("(define ")) continue;
                string p = reroot ? FromLinks.Reroot(keyed[i], Top) : at != null ? at[2] + keyed[i] : keyed[i];
                if (p != null) yield return new CallPath { Keyed = p, Cond = rows[i].Cond };
            }
        }

        // Lua 설정 파일: include·매크로가 없으므로 파일의 줄이 곧 펼친 줄이다. 읽지 못하면 줄 없이 이유만 낸다
        UnitTraceResult TraceLua(string unit)
        {
            var lua = LuaTable.Read(stack.ReadText(unit));
            var result = new UnitTraceResult { Unit = unit, Unresolved = new List<string>(), IncludeEdges = new List<SdlIncludeEdge>(), Nodes = new List<object>() };
            if (lua.Errors.Count > 0) result.ReadError = "이 Lua 파일은 읽지 못해 고칠 수 없다: " + string.Join("; ", lua.Errors);
            foreach (var r in lua.Rows)
            {
                var row = new TraceRow { Path = r.Path, RawPath = r.RawPath, Occurrence = r.Occurrence, Form = "block", LeafNames = new List<string> { null }, Comment = r.Comment, TableComment = r.TableComment };
                var v = new TraceValue { Text = r.Value, File = unit, KeyPath = r.Path, Occurrence = r.Occurrence, Arg = 0 };
                if (r.Kind == "nil") { v.Arg = -1; v.Note = "nil(값 없음)은 이번에 고칠 수 없다"; }
                row.Values.Add(v);
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
