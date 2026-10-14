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
    }

    public static class KeyLocator
    {
        // tools/lib/SdlKeys.ps1의 $SdlQualifiedBlocks와 같아야 한다
        static readonly string[] Qualified = { "extender" };

        public static List<KeyRow> Rows(SdlNode root)
        {
            var result = new List<KeyRow>();
            var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in SdlCore.KeyRows(root.Children, Qualified))
            {
                if (row.Form != "block" && row.Form != "macro") continue;
                int n;
                seen.TryGetValue(row.Path, out n);
                seen[row.Path] = n + 1;
                var leaves = new List<SdlNode>();
                foreach (var a in row.ArgNodes) AddLeaves(a, leaves);
                result.Add(new KeyRow { Path = row.Path, RawPath = row.RawPath, Form = row.Form, Occurrence = n, Leaves = leaves });
            }
            return result;
        }

        static void AddLeaves(SdlNode node, List<SdlNode> leaves)
        {
            if (node.Kind == "atom" || node.Kind == "str") { leaves.Add(node); return; }
            if (node.Kind == "brace") return;                       // 블록은 별도의 행이다
            bool expr = node.Kind == "paren" && SdlCore.IsExpr(node);
            if (node.Kind == "paren" && !expr) return;              // 산술식이 아닌 괄호 구문도 별도의 행이다
            for (int i = expr ? 1 : 0; i < node.Children.Count; i++) AddLeaves(SdlCore.Unwrap(node.Children[i]), leaves);
        }

        public static SdlNode Find(SdlNode root, string path, int occurrence, int arg)
        {
            foreach (var row in Rows(root))
            {
                if (row.Occurrence != occurrence || !string.Equals(row.Path, path, StringComparison.OrdinalIgnoreCase)) continue;
                return arg >= 0 && arg < row.Leaves.Count ? row.Leaves[arg] : null;
            }
            return null;
        }
    }
}
