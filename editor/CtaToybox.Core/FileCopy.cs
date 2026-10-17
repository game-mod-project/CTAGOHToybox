// 파일 사본(팩션 강화, docs/superpowers/specs/2026-10-17-faction-boost-design.md).
// 원본 파일(예 set/stuff/rifle/k98)을 같은 폴더의 새 이름으로 출력 모드에 쓴다. 편집기는 사본 경로를 원본 내용으로 보이게 해서
// (SourceStack.SetCopies) 사본을 다른 파일처럼 펼치고 고친다. 유닛이 그 사본을 쓰게 하려면 이름 값을 사본 이름으로 바꾸는 수정을 따로 넣는다.
// 수정의 꼴: Kind "file-copy", File 원본 가상 경로, New 사본 가상 경로(따옴표 없음), Path "", Arg 0, Scope shared
// 사본 이름은 파일 이름의 첫 낱말(점 앞)에 꼬리를 붙인다. 아이템 이름은 점으로 나눈 낱말 집합으로 풀리므로(ItemLinks),
// 첫 낱말이 새 낱말이면 원래 이름("k98", "ammo mgun_ger" 같은 계열 이름)은 사본으로 풀리지 않는다
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static class FileCopy
    {
        public const string Kind = "file-copy";

        // "set/stuff/rifle/k98" + ger → "set/stuff/rifle/k98_ctb_ger", "set/stuff/mgun/mg34.weapon" → "set/stuff/mgun/mg34_ctb_ger.weapon"
        public static string NameFor(string source, string army)
        {
            string path = SourceStack.ToVirtual(source);
            int slash = path.LastIndexOf('/');
            string dir = path.Substring(0, slash + 1), leaf = path.Substring(slash + 1);
            int dot = leaf.IndexOf('.');
            string first = dot < 0 ? leaf : leaf.Substring(0, dot), rest = dot < 0 ? "" : leaf.Substring(dot);
            return dir + first + "_ctb_" + army + rest;
        }

        // 사본을 만들 수 있으면 null. 원본이 없거나, 같은 경로의 실제 파일이 있거나, 폴더가 다르거나, 사본의 첫 낱말이 이미 다른 아이템 파일의 낱말이면 이유
        public static string CheckTarget(SourceStack stack, string source, string target)
        {
            string src = SourceStack.ToVirtual(source ?? ""), dst = SourceStack.ToVirtual(target ?? "");
            if (dst.Length == 0 || dst.EndsWith("/")) return "사본 경로가 없다";
            if (src == dst) return "사본 경로가 원본과 같다";
            if (stack.FindReal(src) == null) return "원본에 이 파일이 없다: " + src;
            if (stack.FindReal(dst) != null) return "이미 있는 파일이다: " + dst;
            if (src.Substring(0, src.LastIndexOf('/') + 1) != dst.Substring(0, Math.Max(0, dst.LastIndexOf('/') + 1))) return "사본은 원본과 같은 폴더에 둔다: " + dst;
            if (ItemLinks.IsItemFile(dst))
            {
                string leaf = dst.Substring(dst.LastIndexOf('/') + 1);
                string first = leaf.Split('.')[0];
                if (stack.Paths.Where(p => p != dst && ItemLinks.IsItemFile(p)).Any(p => p.Substring(p.LastIndexOf('/') + 1).Split('.').Contains(first)))
                    return "사본의 이름 낱말 " + first + "을 다른 아이템 파일이 이미 쓴다. 이름이 섞여 풀릴 수 있다";
            }
            return null;
        }

        // 프로젝트의 켜진 사본: 사본 경로 → 원본 경로
        public static List<KeyValuePair<string, string>> Pairs(EditProject project)
        {
            if (project == null) return new List<KeyValuePair<string, string>>();
            return project.Edits.Where(e => e.Enabled && e.Kind == Kind && e.File != null && e.New != null)
                .Select(e => new KeyValuePair<string, string>(SourceStack.ToVirtual(e.New), SourceStack.ToVirtual(e.File))).ToList();
        }

        public static Edit New(string unit, string source, string target)
        {
            return new Edit { Kind = Kind, Unit = SourceStack.ToVirtual(unit ?? source), File = SourceStack.ToVirtual(source), Path = "", Arg = 0, Old = null, New = SourceStack.ToVirtual(target), Scope = "shared" };
        }
    }
}
