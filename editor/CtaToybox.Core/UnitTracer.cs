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
    }

    public sealed class UnitTraceResult
    {
        public string Unit { get; set; }
        public List<TraceRow> Rows { get; } = new List<TraceRow>();
        public List<string> Unresolved { get; set; }
        public string ReadError { get; set; }     // 파일을 읽지 못해 줄이 없을 때의 안내(Lua 설정). 읽었으면 null
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
        public List<object> Nodes { get; set; }   // 펼친 노드. 아이템 이름을 모을 때 쓴다
    }

    public sealed class UnitTracer
    {
        sealed class Slot { public string Path; public int Occurrence; public int Arg; public string RawPath; public int RawOccurrence; }

        readonly SourceStack stack;
        // 파일 → (토큰의 글자 위치 → 자리). 파싱되지 않는 파일은 빈 표다
        readonly Dictionary<string, Dictionary<int, Slot>> slots = new Dictionary<string, Dictionary<int, Slot>>(StringComparer.Ordinal);

        public UnitTracer(SourceStack stack) { this.stack = stack; }

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

        TraceValue ValueOf(SdlNode leaf)
        {
            var v = new TraceValue { Text = leaf.Text, File = leaf.File, Arg = -1, Synthetic = leaf.Synthetic };
            if (leaf.Synthetic) { v.Note = "치환이나 계산으로 만든 값이다. 원본의 인자나 피연산자를 고친다"; return v; }
            if (leaf.File == null) { v.Note = "출처를 알 수 없다"; return v; }
            Slot slot;
            if (!SlotsOf(leaf.File).TryGetValue(leaf.Index, out slot)) { v.Note = "편집할 수 있는 자리가 아니다(include 인자이거나 파싱되지 않는 파일)"; return v; }
            v.KeyPath = slot.Path; v.Occurrence = slot.Occurrence;
            if (KeyLocator.IsDisabled(slot.Path)) { v.Note = KeyLocator.DisabledNote; return v; }
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

        // 유닛 파일이 없으면 null
        public UnitTraceResult Trace(string unit)
        {
            unit = SourceStack.ToVirtual(unit);
            if (stack.Find(unit) == null) return null;
            if (LuaTable.IsLua(unit)) return TraceLua(unit);
            var resolved = new SdlResolver(stack.ReadText).Resolve(unit);
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
            return result;
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
