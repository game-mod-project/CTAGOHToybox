// 원본 본문에 먼저 넣는 복제 수정 두 종류(블록 복제 E18, 매크로 정의 복제)를 한곳에서 다룬다.
// 빌드·점검·화면은 복제를 넣은 본문에 값 수정을 적용하고, 펼치기(추적기)는 덧씌우기(SourceStack.Overlay)로 같은 본문을 읽는다
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static class TextCopies
    {
        public static bool IsCopy(string kind) { return kind == BlockCopy.Kind || kind == DefineCopy.Kind; }

        public static string Apply(string text, IEnumerable<Edit> fileEdits, List<EditResult> results)
        {
            var list = (fileEdits ?? Enumerable.Empty<Edit>()).ToList();
            return DefineCopy.Apply(BlockCopy.Apply(text, list, results), list, results);
        }

        public static string ApplyAll(string text, IEnumerable<Edit> fileEdits) { return Apply(text, fileEdits, new List<EditResult>()); }

        // 프로젝트의 켜진 복제를 원본 위에 덧씌우는 함수. 켜진 복제가 없으면 null
        public static Func<string, string, string> Overlay(EditProject project)
        {
            var copies = project == null || project.Edits == null ? new List<Edit>() : project.Edits.Where(e => e.Enabled && IsCopy(e.Kind)).ToList();
            if (copies.Count == 0) return null;
            return (file, text) =>
            {
                var mine = copies.Where(c => string.Equals(SourceStack.ToVirtual(c.File ?? ""), file, StringComparison.Ordinal)).ToList();
                return mine.Count == 0 ? text : ApplyAll(text, mine);
            };
        }

        public static string Dependency(Edit e, IEnumerable<Edit> all)
        {
            var list = (all ?? Enumerable.Empty<Edit>()).ToList();
            return BlockCopy.Dependency(e, list) ?? DefineCopy.Dependency(e, list);
        }

        public static string MissingNote(string name, IEnumerable<Edit> all)
        {
            bool define = (all ?? Enumerable.Empty<Edit>()).Any(x => x.Kind == DefineCopy.Kind && x.New != null && string.Equals(x.New.Trim('"'), name, StringComparison.OrdinalIgnoreCase));
            return define ? DefineCopy.MissingNote(name) : BlockCopy.MissingNote(name);
        }
    }
}
