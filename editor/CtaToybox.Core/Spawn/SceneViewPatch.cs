// 증원 창(gui_gc_reinforcement)을 모든 판에서 보이게 한다(스펙 "메뉴가 사라지는 문제" 대책 1).
// 바닐라 SceneGameView.xml(interface.pak!layout/views/scene/SceneGameView.xml)의 unitspawn_layer 아래 세 FlowPanel(멀티, 싱글·협동, 컨퀘스트)
// 가운데 싱글 패널에만 증원 창이 있다. Spawn Menu는 나머지 둘에도 넣었다(멀티는 enableif="gameclient.master"). 여기서는 셋 모두 조건 없이 둔다.
// 이긴 판의 글자를 그대로 두고 그 줄만 넣거나 enableif 속성만 지운다
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.Spawn
{
    public static class SceneViewPatch
    {
        public const string Path = "interface/layout/views/scene/scenegameview.xml";
        public static readonly string[] Panels = { "game.multiplayer and", "gui.editor or game.singleplayer", "game.campaign and" };
        static readonly Regex Ref = new Regex("<DialogRef\\s+refid=\"gui_gc_reinforcement\"([^>]*?)(/?)>", RegexOptions.CultureInvariant);
        static readonly Regex EnableIf = new Regex("\\s+enableif=\"[^\"]*\"", RegexOptions.CultureInvariant);

        public static string Patch(string xml, out string error)
        {
            error = null;
            int layer = xml.IndexOf("id=\"unitspawn_layer\"", StringComparison.Ordinal);
            if (layer < 0) { error = "unitspawn_layer를 찾지 못했다"; return null; }
            string nl = xml.Contains("\r\n") ? "\r\n" : "\n";
            string text = xml;
            foreach (string panel in Panels)
            {
                int open = FindPanel(text, layer, panel);
                if (open < 0) { error = "FlowPanel(enableif=\"" + panel + " ...\")를 찾지 못했다"; return null; }
                int children = text.IndexOf("<Children>", open, StringComparison.Ordinal);
                int end = children < 0 ? -1 : text.IndexOf("</Children>", children, StringComparison.Ordinal);
                if (children < 0 || end < 0) { error = panel + " 패널의 <Children>을 찾지 못했다"; return null; }
                string inside = text.Substring(children, end - children);
                var m = Ref.Match(inside);
                if (m.Success)
                {
                    string fixedRef = EnableIf.Replace(m.Value, "");
                    text = text.Substring(0, children + m.Index) + fixedRef + text.Substring(children + m.Index + m.Length);
                    continue;
                }
                int lineEnd = text.IndexOf('\n', children) + 1;
                int nextLineEnd = text.IndexOf('\n', lineEnd);
                string next = text.Substring(lineEnd, nextLineEnd - lineEnd);
                string indent = new string(next.TakeWhile(c => c == ' ' || c == '\t').ToArray());
                text = text.Substring(0, lineEnd) + indent + "<DialogRef refid=\"gui_gc_reinforcement\"/>" + nl + text.Substring(lineEnd);
            }
            return text;
        }

        static int FindPanel(string text, int from, string enableIfStart)
        {
            int i = from;
            while ((i = text.IndexOf("<FlowPanel", i + 1, StringComparison.Ordinal)) >= 0)
            {
                int close = text.IndexOf('>', i);
                string tag = text.Substring(i, close - i);
                var m = Regex.Match(tag, "enableif=\"([^\"]*)\"");
                if (m.Success && m.Groups[1].Value.StartsWith(enableIfStart, StringComparison.Ordinal)) return i;
            }
            return -1;
        }
    }
}
