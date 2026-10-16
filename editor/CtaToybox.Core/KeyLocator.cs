// (경로, 몇 번째 행, 몇 번째 값)으로 원본 파일의 토큰을 찾는다.
// 경로는 SdlCore.KeyRows가 만드는 것과 같다(docs/04-units/README.md의 "경로 표기").
// 값은 행의 인자에서 뽑은 잎 토큰이다: name(...)은 안으로 들어가고, 산술식은 피연산자만 센다.
using System;
using System.Collections.Generic;

namespace CtaToybox.Core
{
    public sealed class KeyRow
    {
        public string Path { get; set; }
        public string RawPath { get; set; }     // 원문의 대소문자 그대로 쓴 경로(화면 표시용)
        public string Form { get; set; }
        public int Occurrence { get; set; }      // 같은 경로를 가진 행 가운데 파일에 나온 순서
        public List<SdlNode> Leaves { get; set; }
        public List<string> LeafNames { get; set; }   // 잎마다 그것을 감싼 호출의 이름(cp(40)이면 "cp"). 호출 밖이면 null
        public int RawOccurrence { get; set; }   // 같은 원문 경로(따옴표 이름 포함, 대소문자 무시) 가운데 몇 번째인지
        public string Cond { get; set; }         // 감싼 (mod ...) 조건. 조건 밖이면 null
        public SdlNode Head { get; set; }        // 매크로 호출 행의 이름 토큰(E4). 다른 행은 null
    }

    // 한 파일의 행을 한 번만 만들어 (경로, 몇 번째)로 바로 찾는다. 경로는 대소문자를 가리지 않는다(KeyLocator.Find와 같다).
    // 같은 파일에서 여러 자리를 찾을 때 쓴다(빌드·점검의 TokenPatcher, 실제 파일 검증). Find가 부를 때마다 행 목록을 만드는 비용을 없앤다(W-E)
    public sealed class KeyIndex
    {
        readonly Dictionary<string, KeyRow> rows = new Dictionary<string, KeyRow>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, KeyRow> named = new Dictionary<string, KeyRow>(StringComparer.OrdinalIgnoreCase);

        // Rows는 몇 번째를 대소문자를 가리지 않는 경로마다 센다. 그래서 (경로, 몇 번째)는 겹치지 않는다. 원문 경로도 같다
        public KeyIndex(SdlNode root)
        {
            foreach (var row in KeyLocator.Rows(root))
            {
                rows[Key(row.Path, row.Occurrence)] = row;
                named[Key(row.RawPath, row.RawOccurrence)] = row;
            }
        }

        // 따옴표 이름이 든 원문 경로(예 /"tiger"/cost)와 그 경로 가운데 몇 번째인지로 찾는다(Q7: 모드가 항목을 끼워 넣어도 같은 이름을 찾는다)
        public KeyRow NamedRow(string rawPath, int rawOccurrence)
        {
            KeyRow row;
            return rawPath != null && named.TryGetValue(Key(rawPath, rawOccurrence), out row) ? row : null;
        }

        public SdlNode FindNamed(string rawPath, int rawOccurrence, int arg)
        {
            var row = NamedRow(rawPath, rawOccurrence);
            return row != null && arg >= 0 && arg < row.Leaves.Count ? row.Leaves[arg] : null;
        }

        static string Key(string path, int occurrence) { return occurrence + "\u0001" + path; }

        public KeyRow Row(string path, int occurrence)
        {
            KeyRow row;
            return path != null && rows.TryGetValue(Key(path, occurrence), out row) ? row : null;
        }

        public SdlNode Find(string path, int occurrence, int arg)
        {
            var row = Row(path, occurrence);
            return row != null && arg >= 0 && arg < row.Leaves.Count ? row.Leaves[arg] : null;
        }
    }

    public static class KeyLocator
    {
        // 키 앞에 ##가 붙은 블록({##damage ...}, {## pulseperiod ...}) 안의 자리인가.
        // 개발사 문서(Men of War II): ##는 꺼 둔 블록이다. 읽을 때 지우지 않지만 게임은 그 값을 쓰지 않는다
        public static bool IsDisabled(string path) { return path != null && path.Contains("/##"); }
        public const string DisabledNote = "게임이 쓰지 않는 꺼진 블록(##) 안의 값이다(개발사 문서). 쓰려면 원본에서 ##를 지운다";

        // 따옴표 이름 블록 안의 자리인가({"tiger" ...}). 이런 자리는 원문 경로의 이름으로도 찾는다(Q7)
        public static bool IsNamed(string path) { return path != null && path.Contains("\"*\""); }

        // tools/lib/SdlKeys.ps1의 $SdlQualifiedBlocks와 같아야 한다
        static readonly string[] Qualified = { "extender" };

        public static List<KeyRow> Rows(SdlNode root)
        {
            var result = new List<KeyRow>();
            var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var seenRaw = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in SdlCore.KeyRows(root.Children, Qualified))
            {
                if (row.Form != "block" && row.Form != "macro") continue;
                int n, rn;
                seen.TryGetValue(row.Path, out n);
                seen[row.Path] = n + 1;
                seenRaw.TryGetValue(row.RawPath, out rn);
                seenRaw[row.RawPath] = rn + 1;
                var leaves = new List<SdlNode>();
                var names = new List<string>();
                foreach (var a in row.ArgNodes) AddLeaves(a, leaves, names, null);
                result.Add(new KeyRow { Path = row.Path, RawPath = row.RawPath, Form = row.Form, Occurrence = n, Leaves = leaves, LeafNames = names, RawOccurrence = rn, Cond = row.Cond, Head = row.Head });
            }
            return result;
        }

        static void AddLeaves(SdlNode node, List<SdlNode> leaves, List<string> names, string name)
        {
            if (node.Kind == "atom" || node.Kind == "str") { leaves.Add(node); names.Add(name); return; }
            if (node.Kind == "brace") return;                       // 블록은 별도의 행이다
            bool expr = node.Kind == "paren" && SdlCore.IsExpr(node);
            if (node.Kind == "paren" && !expr) return;              // 산술식이 아닌 괄호 구문도 별도의 행이다
            if (node.Kind == "call") name = node.Text;
            for (int i = expr ? 1 : 0; i < node.Children.Count; i++) AddLeaves(SdlCore.Unwrap(node.Children[i]), leaves, names, name);
        }

        // 한 자리만 찾을 때. 같은 파일에서 여러 자리를 찾으면 KeyIndex를 한 번 만들어 쓴다
        public static SdlNode Find(SdlNode root, string path, int occurrence, int arg)
        {
            return new KeyIndex(root).Find(path, occurrence, arg);
        }
    }
}
