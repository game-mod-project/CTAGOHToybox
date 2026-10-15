// 파일 하나의 값 줄. SDL 파일은 KeyLocator.Rows, Lua 파일은 LuaTable의 줄(값 하나짜리 KeyRow)이다.
// 수정의 자리(경로, 몇 번째, 몇 번째 값)를 원본에서 다시 찾는 곳(점검, 설명, 명령줄)이 모두 이것을 쓴다
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static class SourceRows
    {
        // 읽지 못하면 null이고 error에 이유
        public static List<KeyRow> Of(string virtualPath, string text, out string error)
        {
            error = null;
            if (text == null) { error = "원본에 이 파일이 없다"; return null; }
            if (LuaTable.IsLua(virtualPath))
            {
                var lua = LuaTable.Read(text);
                if (lua.Errors.Count > 0) { error = lua.Errors[0]; return null; }
                return lua.Rows.Select(r => new KeyRow
                {
                    Path = r.Path, RawPath = r.RawPath, Form = "block", Occurrence = r.Occurrence,
                    Leaves = new List<SdlNode> { new SdlNode(r.Kind == "string" ? "str" : "atom", r.Value, r.Index, false) },
                    LeafNames = new List<string> { null },
                }).ToList();
            }
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0) { error = parsed.Errors[0]; return null; }
            return KeyLocator.Rows(parsed.Root);
        }
    }
}
