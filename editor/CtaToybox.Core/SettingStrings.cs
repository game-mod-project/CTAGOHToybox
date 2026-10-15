// 게임 설정 문자열의 두 꼴. 스펙: docs/superpowers/specs/2026-10-15-settings-editor-design.md
//  - 문자열 속 설정 "이름=값;이름=값": 게임 모드의 parameters, unitLimits 등. 조각의 이름·개수·순서는 두고 값만 고친다
//  - 판 수 단계표 "1:200 4:250": WinGain, FlagCount 등. 칸은 공백으로 나누고 칸마다 ':'로 이은 수의 개수가 같다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class SettingPiece
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public static class SettingStrings
    {
        // 조각 이름: 낱말, 또는 *(나머지 모두. timers_frontlines.inc의 "…;*=1.00")
        static readonly Regex PieceName = new Regex(@"^([A-Za-z_][A-Za-z0-9_]*|\*)$", RegexOptions.Compiled);
        static readonly Regex StepEntry = new Regex(@"^\d+(\.\d+)?(:\d+(\.\d+)?)+$", RegexOptions.Compiled);

        static string Body(string token)
        {
            if (token == null || token.Length < 2 || token[0] != '"' || token[token.Length - 1] != '"') return null;
            return token.Substring(1, token.Length - 2);
        }

        // 따옴표 문자열이 "이름=값;…"(조각 둘 이상, 이름은 낱말) 꼴이면 조각들. 아니면 null. 끝의 ;는 조각으로 치지 않는다
        public static List<SettingPiece> Split(string token)
        {
            string body = Body(token);
            if (body == null) return null;
            if (body.EndsWith(";")) body = body.Substring(0, body.Length - 1);
            var result = new List<SettingPiece>();
            foreach (string part in body.Split(';'))
            {
                int eq = part.IndexOf('=');
                if (eq <= 0 || !PieceName.IsMatch(part.Substring(0, eq))) return null;
                result.Add(new SettingPiece { Name = part.Substring(0, eq), Value = part.Substring(eq + 1) });
            }
            return result.Count >= 2 ? result : null;
        }

        // 값 고치기 창이 시작할 조각: 기존 수정의 새 값(editNew)이 지금 원본의 조각과 이름·순서·개수가 같으면 그것, 아니면 원본.
        // 수정을 만든 뒤 모드나 패치로 원본의 조각이 바뀌면 옛 값을 새 칸에 넣지 않는다(수정 목록에는 "원본이 바뀜"으로 보인다)
        public static List<SettingPiece> StartPieces(List<SettingPiece> source, string editNew)
        {
            var edited = editNew == null ? null : Split(editNew);
            if (edited == null || edited.Count != source.Count) return source;
            for (int i = 0; i < source.Count; i++) if (edited[i].Name != source[i].Name) return source;
            return edited;
        }

        // 원래 문자열의 조각 이름·순서·끝의 ;를 두고 값만 바꿔 잇는다. values는 조각 순서대로다
        public static string Join(string originalToken, IList<string> values)
        {
            var pieces = Split(originalToken);
            if (pieces == null) throw new ArgumentException("문자열 속 설정이 아니다: " + originalToken);
            if (values.Count != pieces.Count) throw new ArgumentException("조각 수가 다르다: " + values.Count + " / " + pieces.Count);
            bool trailing = Body(originalToken).EndsWith(";");
            return "\"" + string.Join(";", pieces.Select((p, i) => p.Name + "=" + values[i])) + (trailing ? ";" : "") + "\"";
        }

        // 문자열 속 설정의 새 값 검사. 원래 값이 문자열 속 설정이 아니면 검사할 것이 없다(null). 맞으면 null
        public static string CheckPieces(string oldToken, string newToken)
        {
            var before = Split(oldToken);
            if (before == null) return null;
            var after = Split(newToken);
            if (after == null || after.Count != before.Count)
                return "조각(이름=값)의 개수와 꼴은 원래와 같아야 한다. 값에 ; 를 쓸 수 없다: " + newToken;
            for (int i = 0; i < before.Count; i++)
            {
                if (after[i].Name != before[i].Name) return (i + 1) + "번째 조각의 이름은 " + before[i].Name + "이어야 한다(지금 " + after[i].Name + ")";
                if (after[i].Value.Length == 0) return before[i].Name + "의 값이 비어 있다";
                if (after[i].Value.Contains('=') && !before[i].Value.Contains('=')) return before[i].Name + "의 값에 = 를 쓸 수 없다";
            }
            if (Body(oldToken).EndsWith(";") != Body(newToken).EndsWith(";")) return "끝의 ; 는 원래대로 둔다";
            return null;
        }

        // 칸마다 ':'로 이은 수의 개수. 단계표가 아니거나 칸마다 개수가 다르면 null
        static int[] StepShape(string token)
        {
            string body = Body(token);
            if (body == null || body.Trim().Length == 0) return null;
            var entries = Regex.Split(body.Trim(), @"\s+");
            if (!entries.All(e => StepEntry.IsMatch(e))) return null;
            var shape = entries.Select(e => e.Split(':').Length).ToArray();
            return shape.Distinct().Count() == 1 ? shape : null;
        }

        public static bool IsStepTable(string token) { return StepShape(token) != null; }

        // 단계표의 새 값 검사: 같은 꼴(칸마다 수의 개수가 같다)이어야 한다. 칸 수는 달라도 된다. 맞으면 null
        public static string CheckSteps(string oldToken, string newToken)
        {
            var before = StepShape(oldToken);
            if (before == null) return null;
            var after = StepShape(newToken);
            if (after == null) return "\"수:수 수:수\" 꼴이어야 한다(칸은 공백으로 나눈다): " + newToken;
            if (after[0] != before[0]) return "칸마다 ':'로 이은 수가 " + before[0] + "개여야 한다";
            return null;
        }

        // 읽기용 풀이: "1:200 4:250" → "1 → 200, 4 → 250". 단계표가 아니면 null
        public static string StepPreview(string token)
        {
            if (StepShape(token) == null) return null;
            return string.Join(", ", Regex.Split(Body(token).Trim(), @"\s+").Select(e =>
            {
                var p = e.Split(':');
                return p[0] + " → " + string.Join(" : ", p.Skip(1));
            }));
        }
    }
}
