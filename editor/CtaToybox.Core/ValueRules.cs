// 새 값 검사를 한곳에 모은다. 화면(EditorSession.ValidateNew), 명령줄(add), 빌드(ModBuilder)가 같은 규칙을 쓴다.
// 게임 설정이 있는 폴더(SettingFiles.InArea)의 파일이면 문자열 속 설정과 단계표의 꼴도 본다
namespace CtaToybox.Core
{
    public static class ValueRules
    {
        // 맞으면 null. warning은 막지 않는 경고다
        public static string CheckNewValue(string file, string oldText, string newText, out string warning)
        {
            string error = LuaTable.IsLua(file) ? LuaPatcher.CheckNewValue(oldText, newText, out warning) : TokenPatcher.CheckNewValue(oldText, newText, out warning);
            if (error != null) return error;
            warning = warning ?? RangeWarning(oldText, newText);
            if (LuaTable.IsLua(file) || !SettingFiles.InArea(file)) return null;
            return SettingStrings.CheckPieces(oldText, newText) ?? SettingStrings.CheckSteps(oldText, newText);
        }

        // 범위 경고(막지 않는다): 0보다 큰 값을 음수로 바꾸면 실수일 수 있다. 수식이나 문자열은 보지 않는다.
        // 몇 배인지는 보지 않는다(gravity 2→60 같은 정상 변경이 있다). 정수를 소수로 바꾸는 것도 보지 않는다({item "m24 grenade" 1.25 0.5}처럼 개수도 소수가 된다)
        static string RangeWarning(string oldText, string newText)
        {
            string a = oldText == null ? null : oldText.Trim(), b = newText == null ? null : newText.Trim();
            if (a == null || b == null || !SdlCore.IsNumber(a) || !SdlCore.IsNumber(b)) return null;
            double before, after;
            if (!double.TryParse(a, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out before) ||
                !double.TryParse(b, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out after)) return null;
            if (before > 0 && after < 0) return "원래 값은 0보다 큰데 새 값이 음수다: " + b;
            return null;
        }

        // 수정 한 건을 원본 텍스트에 미리 적용해 본다. 되면 null, 안 되면 이유.
        // 새 값 규칙을 지켜도 블록 한정자나 숫자 행의 첫 값을 바꾸면 키 경로가 바뀌어 빌드가 그 파일을 쓰지 못한다(CheckNewValue로는 보이지 않는다)
        public static string CheckApplies(string file, string text, Edit edit)
        {
            var results = new System.Collections.Generic.List<EditResult>();
            var one = new[] { new Edit { Unit = edit.Unit, File = edit.File, Path = edit.Path, Occurrence = edit.Occurrence, Arg = edit.Arg, Old = edit.Old, New = edit.New, Scope = edit.Scope } };
            try
            {
                if (LuaTable.IsLua(file)) LuaPatcher.Apply(text, one, results);
                else TokenPatcher.Apply(text, one, results);
            }
            catch (System.InvalidOperationException ex) { return "적용해 보니 안 된다: " + ex.Message; }
            if (results.Count == 0) return "적용해 보니 결과가 없다";
            return results[0].Status == EditStatus.Applied ? null : results[0].Message;
        }
    }
}
