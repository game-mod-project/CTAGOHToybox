// 스커미시 로스터(set/multiplayer/units/<시기>/units_<국가>.set) 한 파일의 항목을 읽는다.
// 꼴(2026-10-17 조사, gamelogic.pak!set/multiplayer/units/mid/units_ger.set):
//   차량·거치 화기: {"panzer1b" ("vehicle2" period(mid) cd(150) ... b(tanks) cw(4) cp(8) cost(105))}  (789행)
//   보병 분대·단독: ("squad_with3types_doc" side(ger) name(...) period(mid) ... c1(ersatz_sl:1) ... b(inf1) t())  (403행)
// 맨 위의 따옴표 이름 블록과 맨 위의 매크로 호출만 항목이다. 매크로 정의((define ...)) 안은 틀이라 보지 않는다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.Spawn
{
    public sealed class RosterArg
    {
        public string Name { get; set; }
        public SdlNode Value { get; set; }   // 첫 값 토큰. 인자 안이 비면 null
        public SdlNode Call { get; set; }
        public string Text { get { return Value == null ? "" : Value.Text.Trim('"'); } }
    }

    public sealed class RosterItem
    {
        public string File { get; set; }
        public string Name { get; set; }       // 차량 항목은 블록 이름, 분대 호출은 name() 값
        public bool IsCall { get; set; }       // 맨 위 분대 호출
        public string Macro { get; set; }
        public SdlNode Block { get; set; }     // 차량 항목의 블록. 분대 호출이면 null
        public SdlNode Paren { get; set; }     // 매크로 호출 괄호
        public List<RosterArg> Args { get; } = new List<RosterArg>();

        public string Arg(string name)
        {
            var a = Args.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            return a == null ? null : a.Text;
        }
    }

    public static class RosterScan
    {
        static readonly Regex Skirmish = new Regex(@"^set/multiplayer/units/(early|mid|late)/units_([a-z]+)(?:_[a-z0-9_]+)?\.set$", RegexOptions.CultureInvariant);
        static readonly Regex Inf = new Regex(@"^set/multiplayer/units/(early|mid|late)/inf_([a-z]+)(?:_[a-z0-9_]+)?\.set$", RegexOptions.CultureInvariant);

        // 스커미시 시기 로스터면 (시기, 국가), 아니면 null. 모드가 더한 꼬리도 같은 국가다(MACE late/inf_jap_mace_mp.set)
        public static (string Period, string Army)? SkirmishFile(string virtualPath)
        {
            var m = Skirmish.Match(SourceStack.ToVirtual(virtualPath));
            return m.Success ? (m.Groups[1].Value, m.Groups[2].Value) : ((string, string)?)null;
        }

        public static (string Period, string Army)? InfFile(string virtualPath)
        {
            var m = Inf.Match(SourceStack.ToVirtual(virtualPath));
            return m.Success ? (m.Groups[1].Value, m.Groups[2].Value) : ((string, string)?)null;
        }

        public static List<RosterItem> Items(string file, SdlNode root)
        {
            var items = new List<RosterItem>();
            foreach (object o in root.Children)
            {
                var n = SdlCore.Unwrap(o);
                if (n.Kind == "brace" && n.Children.Count > 0 && SdlCore.Unwrap(n.Children[0]).Kind == "str")
                {
                    var paren = n.Children.Skip(1).Select(SdlCore.Unwrap).FirstOrDefault(IsMacro);
                    if (paren == null) continue;
                    var item = Read(file, paren);
                    item.Name = SdlCore.Unwrap(n.Children[0]).Text.Trim('"');
                    item.Block = n;
                    items.Add(item);
                }
                else if (IsMacro(n))
                {
                    var item = Read(file, n);
                    item.IsCall = true;
                    item.Name = item.Arg("name") ?? item.Macro;
                    items.Add(item);
                }
            }
            return items;
        }

        static bool IsMacro(SdlNode n)
        {
            return n.Kind == "paren" && n.Children.Count > 0 && SdlCore.Unwrap(n.Children[0]).Kind == "str";
        }

        static RosterItem Read(string file, SdlNode paren)
        {
            var item = new RosterItem { File = file, Paren = paren, Macro = SdlCore.Unwrap(paren.Children[0]).Text.Trim('"') };
            foreach (object c in paren.Children.Skip(1))
            {
                var a = SdlCore.Unwrap(c);
                if (a.Kind != "call") continue;
                var value = a.Children.Select(SdlCore.Unwrap).FirstOrDefault(v => v.Kind == "atom" || v.Kind == "str");
                item.Args.Add(new RosterArg { Name = a.Text, Value = value, Call = a });
            }
            return item;
        }

        static readonly Regex Member = new Regex(@"^c\d+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        static readonly Regex Crew = new Regex(@"^crew\d*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        // 분대 구성원(c1(x:n) ...)이나 승무원(crewN(x:n)). 값 "이름:수", 수가 없으면 1
        public static List<(string Type, int Count)> Members(RosterItem item, bool crew)
        {
            var list = new List<(string, int)>();
            foreach (var a in item.Args)
            {
                if (!(crew ? Crew : Member).IsMatch(a.Name)) continue;
                string v = a.Text;
                if (v.Length == 0) continue;
                int colon = v.IndexOf(':');
                int count = 1;
                if (colon >= 0) { int.TryParse(v.Substring(colon + 1), out count); v = v.Substring(0, colon); }
                if (v.Length == 0 || count <= 0) continue;
                list.Add((v, count));
            }
            return list;
        }
    }
}
