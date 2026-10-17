// 맵 .mi 사본: 지금 계층에서 이긴 판의 바이트 그대로에, 맨 위 {mission ...} 블록의 닫는 괄호 앞에 include 줄만 붙인다(스펙 "붙이는 방식").
// Spawn Menu도 같은 방식이다(spike: 사본 끝 (include "/map/cheat/cheat.mi")). 다른 줄은 바꾸지 않고 맵의 다른 파일은 복사하지 않는다.
// 바이트는 Latin-1로 읽어 글자 하나 = 바이트 하나로 다룬다(인코딩을 몰라도 원문이 그대로 남는다)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.Spawn
{
    public static class MapPatch
    {
        public const string MenuInclude = "(include \"/map/ctatoybox_spawn/menu.mi\")";
        public static string NationInclude(string nation) { return "(include \"/map/ctatoybox_spawn/nation_" + nation + ".mi\")"; }
        static readonly string[] Nations = { "rus", "ger", "fin", "usa", "eng", "jap" };
        static readonly Encoding Latin1 = Encoding.Latin1;

        public static bool IsMap(string virtualPath)
        {
            string p = SourceStack.ToVirtual(virtualPath);
            if (!p.EndsWith(".mi", StringComparison.Ordinal)) return false;
            if (p.StartsWith("map/single/", StringComparison.Ordinal)) return p.EndsWith("/0.mi", StringComparison.Ordinal);
            return p.StartsWith("map/multi/", StringComparison.Ordinal);
        }

        // 캠페인 폴더 map/single/<NN-국가…>/의 국가. 모르면 null
        public static string NationOf(string virtualPath)
        {
            var parts = SourceStack.ToVirtual(virtualPath).Split('/');
            if (parts.Length < 4 || parts[1] != "single") return null;
            string folder = parts[2];
            int dash = folder.IndexOf('-');
            if (dash < 0) return null;
            string word = new string(folder.Substring(dash + 1).TakeWhile(char.IsLetter).ToArray());
            return Nations.Contains(word) ? word : null;
        }

        public static byte[] Patch(byte[] original, string nation, out string error)
        {
            error = null;
            string text = Latin1.GetString(original);
            if (text.Contains(MenuInclude)) return original;
            var parsed = SdlCore.Parse(text);
            var top = parsed.Root.Children.Select(SdlCore.Unwrap).Where(n => n.Kind == "brace").ToList();
            var mission = top.Count == 1 && top[0].Children.Count > 0 && SdlCore.Unwrap(top[0].Children[0]).Kind == "atom" && string.Equals(SdlCore.Unwrap(top[0].Children[0]).Text, "mission", StringComparison.OrdinalIgnoreCase) ? top[0] : null;
            if (mission == null) { error = "맨 위 블록이 {mission ...} 하나가 아니다"; return null; }
            if (parsed.Errors.Count > 0) { error = "읽지 못했다: " + parsed.Errors[0]; return null; }
            int close = CloseOf(parsed.Tokens, mission.Index);
            if (close < 0) { error = "{mission의 닫는 괄호를 찾지 못했다"; return null; }
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            var lines = new List<string>();
            if (nation != null) lines.Add(NationInclude(nation));
            lines.Add(MenuInclude);
            int lineStart = text.LastIndexOf('\n', close - 1) + 1;
            bool onlySpace = text.Substring(lineStart, close - lineStart).All(ch => ch == ' ' || ch == '\t');
            string insert = onlySpace
                ? string.Concat(lines.Select(l => "\t" + l + nl))
                : nl + string.Concat(lines.Select(l => "\t" + l + nl));
            int at = onlySpace ? lineStart : close;
            return Latin1.GetBytes(text.Substring(0, at) + insert + text.Substring(at));
        }

        static int CloseOf(List<SdlToken> tokens, int openIndex)
        {
            int depth = 0;
            bool started = false;
            foreach (var t in tokens)
            {
                if (!started) { if (t.Kind == "open" && t.Index == openIndex) { started = true; depth = 1; } continue; }
                if (t.Kind == "open") depth++;
                else if (t.Kind == "close" && --depth == 0) return t.Index;
            }
            return -1;
        }

        public static List<SpawnFile> Build(SourceStack stack, List<string> skipped, out int campaign, out int multi)
        {
            var files = new List<SpawnFile>();
            campaign = 0; multi = 0;
            foreach (string path in stack.Paths.Where(IsMap).OrderBy(p => p, StringComparer.Ordinal))
            {
                var src = stack.Find(path);
                string error;
                var bytes = Patch(stack.Read(src), NationOf(path), out error);
                if (bytes == null) { skipped.Add(path + " (" + src.Layer + "): " + error); continue; }
                files.Add(new SpawnFile { Path = "resource/" + path, Bytes = bytes, Source = src.Layer });
                if (path.StartsWith("map/single/", StringComparison.Ordinal)) campaign++; else multi++;
            }
            return files;
        }
    }
}
