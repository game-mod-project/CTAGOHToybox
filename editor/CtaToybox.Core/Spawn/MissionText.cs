// 미션 SDL 조각을 만드는 작은 나무. 머리 글(따옴표 이름이나 키)과 자식(글 또는 나무)으로 {머리 자식...}을 쓴다.
// 줄바꿈은 CRLF(바닐라 미션과 Spawn Menu cheat.mi가 CRLF다), 들여쓰기는 탭
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core.Spawn
{
    public sealed class MNode
    {
        public string Head { get; }
        public List<object> Items { get; } = new List<object>();

        public MNode(string head, params object[] items) { Head = head; Add(items); }

        public MNode Add(params object[] items)
        {
            foreach (var i in items)
            {
                if (i == null) continue;
                if (i is IEnumerable<MNode> many) Items.AddRange(many);
                else Items.Add(i);
            }
            return this;
        }

        public static string Q(string s) { return "\"" + s + "\""; }

        // 한 줄로 쓰기에 짧은 블록: 자식이 모두 글이거나, 글로만 된 블록
        bool Inline { get { return Items.All(i => i is string || (i is MNode s && s.Items.All(x => x is string) && s.Items.Count <= 3)); } }

        public void Write(StringBuilder sb, int depth)
        {
            string tab = new string('\t', depth);
            if (Inline) { sb.Append(tab); WriteInline(sb); sb.Append("\r\n"); return; }
            sb.Append(tab).Append('{').Append(Head).Append("\r\n");
            foreach (var i in Items)
            {
                if (i is MNode s) s.Write(sb, depth + 1);
                else sb.Append(tab).Append('\t').Append((string)i).Append("\r\n");
            }
            sb.Append(tab).Append("}\r\n");
        }

        void WriteInline(StringBuilder sb)
        {
            sb.Append('{').Append(Head);
            foreach (var i in Items)
            {
                sb.Append(' ');
                if (i is MNode s) s.WriteInline(sb); else sb.Append((string)i);
            }
            sb.Append('}');
        }

        public override string ToString() { var sb = new StringBuilder(); Write(sb, 0); return sb.ToString(); }

        // 자주 쓰는 미션 조각(꼴은 Spawn Menu cheat.mi와 바닐라 미션 원문, docs/13-spawn-menu-spike.md)
        public static MNode Tag(string tag) { return new MNode("tag", new MNode("tag", Q(tag))); }

        public static MNode Selector(string selectTag, string[] include = null, string[] exclude = null, MNode selectBlock = null)
        {
            var group = new MNode("group", new MNode("select", selectBlock ?? Tag(selectTag)));
            if (include != null && include.Length > 0) group.Add(new MNode("include", include.Select(Tag).ToArray()));
            if (exclude != null && exclude.Length > 0) group.Add(new MNode("exclude", exclude.Select(Tag).ToArray()));
            return new MNode("selector", new MNode("source", "advanced"), group);
        }

        public static MNode Event(string name, string mode) { return new MNode(Q("event"), new MNode("event", Q(name)), new MNode("mode", mode), new MNode("delay", "0")); }
        public static MNode Show(string name) { return new MNode(Q("reinforcement"), new MNode("operation", "show"), new MNode("name", Q(name))); }
        public static MNode Hide(string name) { return new MNode(Q("reinforcement"), new MNode("operation", "hide"), new MNode("name", Q(name))); }
        public static MNode Call(string function) { return new MNode(Q("call"), new MNode("function", Q(function))); }
        public static MNode SetVar(string var, int value) { return new MNode(Q("set_i"), new MNode("var", Q(var + "$")), new MNode("op", Q("=")), new MNode("value", value.ToString())); }
        public static MNode IfVar(string var, string op, int value, params object[] actions)
        {
            return new MNode(Q("if"), new MNode("condition", new MNode("type", "cmp_i"), new MNode("var", Q(var + "$")), new MNode("op", Q(op)), new MNode("value", value.ToString()))).Add(actions);
        }

        // 이벤트로 도는 트리거: 끝나면 이벤트를 되돌리고 자기를 다시 건다(cheat.mi 72434–72514의 꼴)
        public static MNode EventTrigger(string name, params object[] actions)
        {
            var acts = new MNode("actions").Add(actions).Add(Event(name, "reset"), new MNode(Q("trigger"), new MNode("name", Q(name))));
            return new MNode(Q(name), "{autoreset}", new MNode("condition", new MNode("terms", new MNode(Q("1.event"), new MNode("id", Q(name))))), acts);
        }
    }
}
