// 새 값 검사를 한곳에 모은다. 화면(EditorSession.ValidateNew), 명령줄(add), 빌드(ModBuilder)가 같은 규칙을 쓴다.
// 게임 설정이 있는 폴더(SettingFiles.InArea)의 파일이면 문자열 속 설정과 단계표의 꼴도 본다
namespace CtaToybox.Core
{
    public static class ValueRules
    {
        // 맞으면 null. warning은 막지 않는 경고다
        public static string CheckNewValue(string file, string oldText, string newText, out string warning)
        {
            if (LuaTable.IsLua(file)) return LuaPatcher.CheckNewValue(oldText, newText, out warning);
            string error = TokenPatcher.CheckNewValue(oldText, newText, out warning);
            if (error != null || !SettingFiles.InArea(file)) return error;
            return SettingStrings.CheckPieces(oldText, newText) ?? SettingStrings.CheckSteps(oldText, newText);
        }
    }
}
