// 화면 목록이 묶는 항목과 표시 문자열. 판단(고칠 수 있는지, 범위 등)은 EditorSession이 하고, 여기서는 글자만 만든다.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public sealed class Choice
    {
        public string Label { get; set; }
        public string Value { get; set; }   // null이면 거르지 않는다
    }

    public sealed class UnitItem
    {
        public UnitEntry Entry { get; set; }
        public string Title { get; set; }   // 게임 속 이름, 없으면 파일 이름
        public string Sub { get; set; }     // 파일 이름 · 종류 · 출처
    }

    // 전체·요약 탭의 한 줄: 키 한 줄(값이 여럿이면 한 줄에 모아 보인다)
    public sealed class RowItem
    {
        public RowView Row { get; set; }
        public string Group { get; set; }
        public int GroupOrder { get; set; }
        public string Name { get; set; }       // 한글 이름(키 주석에서)
        public string Label { get; set; }      // 항목 이름
        public string Key { get; set; }        // 원본에 쓰인 그대로의 키 경로
        public string Context { get; set; }    // 어느 볼륨·부품·무장의 것인지
        public string Values { get; set; }     // 지금 값
        public string NewValues { get; set; }  // 걸린 수정
        public string Note { get; set; }       // 설명과 등급
        public string Shared { get; set; }     // 같이 쓰는 유닛 수
        public string Scope { get; set; }      // "이 유닛만" 가능 여부
        public string Tip { get; set; }        // 원본 파일과 자리
        public bool Editable { get; set; }
    }

    public sealed class ItemLinkItem
    {
        public ItemLink Link { get; set; }
        public string Title { get; set; }
        public string Sub { get; set; }
    }

    public sealed class EditItem
    {
        public Edit Edit { get; set; }
        public bool Enabled { get; set; }
        public string Group { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }
    }

    public static class Text
    {
        public const string NotEditableGroup = "고칠 수 없는 값 (치환·계산으로 만든 값, include 인자)";

        // 값 하나를 화면에 보이는 글자로: 따옴표 문자열은 따옴표를 뗀다
        public static string Value(string token) { return token != null && token.StartsWith("\"") ? token.Trim('"') : token; }

        public static string Status(EditStatus s)
        {
            switch (s)
            {
                case EditStatus.Applied: return "적용";
                case EditStatus.SourceChanged: return "원본이 바뀜";
                case EditStatus.TargetMissing: return "대상 없음";
                default: return "적용 불가";
            }
        }

        // 줄의 설명: 주석이 없는 매크로 호출 줄은 매크로 인자라는 것을 알린다
        public static string RowNote(RowView row)
        {
            string comment = (row.SourceComment == null ? "" : "\n원문 주석: " + row.SourceComment) + (row.TableComment == null ? "" : "\n표 설명: " + row.TableComment);
            if (row.Note == null && row.IsMacroCall) return "[매크로 호출] 이 인자를 쓰는 값들이 함께 바뀐다. 인자로 계산한 값은 전체 탭의 '고칠 수 없는 값'에 보인다";
            if (row.Note == null && comment.Length > 0) return "(한글 설명 없음)" + comment;
            return Note(row.Note) + comment;
        }

        // 줄의 한글 이름: 키 주석의 뜻을 짧게 줄인 것. 뜻을 모르면 "(이름 미확인)"과 원문 키의 마지막 이름
        public static string RowName(RowView row)
        {
            if (row.Note != null && row.Note.Grade != "미확인" && !string.IsNullOrWhiteSpace(row.Note.Meaning))
                return SummaryKeys.ShortLabel(row.Note.Meaning);
            var parts = KeyLabel.Split(row.RawPath ?? row.Path);
            string last = parts.Count == 0 ? row.Path : parts[parts.Count - 1];
            if (row.IsMacroCall) return "매크로 " + last.Trim('(', ')', '"') + " (뜻 미확인)";
            return "(이름 미확인) " + last.Trim('"');
        }

        // 수정 목록의 제목: 줄 이름 [원본 줄] · 값 이름: 원래 → 새 값
        public static string EditTitle(EditorSession.EditDescription d, Edit e)
        {
            string change = Value(e.Old) + " → " + Value(e.New);
            if (d == null) return KeyLabel.Readable(e.Path) + " · " + (e.Arg + 1) + "번째 값: " + change;
            var row = new RowView { Path = e.Path, Note = d.Note, IsMacroCall = d.IsMacroCall };
            string rowName = RowName(row);
            // 문자열 속 설정: 바뀐 조각만 보인다(문자열 전체는 길어 무엇이 바뀌었는지 읽히지 않는다)
            if (d.ChangedPieces != null && d.ChangedPieces.Count > 0) return rowName + " · " + string.Join(", ", d.ChangedPieces);
            string valueName = d.ValueLabel ?? (d.ValueCount == 1 && d.ValueName == null ? null : (d.ValueName ?? (e.Arg + 1) + "번째 값") + " (뜻 미확인)");
            return rowName + (d.ValueCount > 1 ? "  [" + d.RowText.Replace("\"", "") + "]" : "") + (valueName == null ? "" : " · " + valueName) + ":  " + change;
        }

        // 값 하나의 이름: 뜻이 있으면 뜻, 없으면 원문 이름이나 자리 번호에 "뜻 미확인"
        public static string ValueName(RowView row, int index)
        {
            var v = row.Values[index];
            if (v.Label != null) return v.Label;
            if (row.Values.Count == 1 && v.Name == null) return RowName(row);   // 값이 하나면 줄의 이름이 곧 값의 이름이다
            return (v.Name ?? "값 " + (index + 1)) + " (뜻 미확인)";
        }

        // 줄의 지금 값. 호출 안의 값은 원문처럼 이름(값 값)으로 묶는다: side(ger) lvl(lvl1 lvl2) cost(10.0)
        public static string ValuesLine(RowView row)
        {
            if (row.Values.Count == 1 && row.Values[0].Computed != null) return Value(row.Values[0].Trace.Text) + "  (= " + row.Values[0].Computed + ")";
            // 문자열 속 설정: 조각마다 한 줄 "뜻: 값"
            if (row.Values.Count == 1 && row.Values[0].Pieces != null)
            {
                var v0 = row.Values[0];
                return string.Join("\n", v0.Pieces.Select((p, i) => (v0.PieceLabels[i] ?? p.Name + " (뜻 미확인)") + ": " + p.Value));
            }
            // 값이 여럿이고 뜻을 아는 값이 있으면 한 줄에 하나씩 "뜻: 값"으로 보인다
            if (row.Values.Count > 1 && row.Values.Any(v => v.Label != null))
                return string.Join("\n", row.Values.Select((v, i) => v.Label != null && v.Label.StartsWith("열쇠말") ? null : ValueName(row, i) + ": " + Value(v.Trace.Text)).Where(s => s != null));
            var parts = new System.Collections.Generic.List<string>();
            for (int i = 0; i < row.Values.Count; i++)
            {
                string name = row.Values[i].Name;
                if (name == null) { parts.Add(Value(row.Values[i].Trace.Text)); continue; }
                var group = new System.Collections.Generic.List<string>();
                for (; i < row.Values.Count && row.Values[i].Name == name; i++) group.Add(Value(row.Values[i].Trace.Text));
                i--;
                parts.Add(name + "(" + string.Join(" ", group) + ")");
            }
            return string.Join("  ", parts);
        }

        // 줄의 부위와, 같은 줄을 모았으면 그 수. 둘 다 없으면 null
        public static string ContextLine(string context, int repeats, string condLabel = null)
        {
            string rep = repeats > 1 ? "같은 줄 " + repeats + "곳을 하나로 보인다. 고치면 모두 바뀐다" : null;
            // (mod ...) 조건 안의 줄은 그 조건일 때만 게임에 들어간다
            string cond = condLabel == null ? null : "조건: " + condLabel;
            string tail = string.Join("  ·  ", new[] { cond, rep }.Where(x => x != null));
            if (context == null) return tail.Length == 0 ? null : tail;
            return "↳ " + context + (tail.Length == 0 ? "" : "  ·  " + tail);
        }

        public static string Note(KeyNote note)
        {
            return note == null ? "(설명 없음)" : "[" + (note.Grade.Length == 0 ? "등급 없음" : note.Grade) + "] " + note.Meaning;
        }

        public static string UnitName(EditorSession session, string unit)
        {
            return session.Names.OfUnit(unit) ?? unit.Substring(unit.LastIndexOf('/') + 1);
        }
    }

    // 사용자가 펼치거나 접은 묶음. 값을 고쳐 목록을 다시 그려도 그대로 두고, 다른 유닛을 열면 지운다. 키는 "목록|묶음 이름"
    public static class GroupStates
    {
        static readonly Dictionary<string, bool> states = new Dictionary<string, bool>(StringComparer.Ordinal);

        static string Key(string list, object group) { return list + "|" + group; }
        public static bool? Get(string list, object group) { bool v; return states.TryGetValue(Key(list, group), out v) ? v : (bool?)null; }
        public static void Set(string list, object group, bool expanded) { states[Key(list, group)] = expanded; }
        public static void Clear() { states.Clear(); }
    }

    // 묶음을 펼칠지. 매개변수는 "목록" 또는 "목록|수". 사용자가 펼치거나 접은 묶음은 그대로, 아니면 작은 묶음만 펼친다. 고칠 수 없는 값은 접는다
    public sealed class ExpandSmallGroups : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var group = value as CollectionViewGroup;
            if (group == null) return true;
            string[] p = ((string)parameter ?? "").Split('|');
            bool? kept = GroupStates.Get(p[0], group.Name);
            if (kept.HasValue) return kept.Value;
            if (string.Equals(group.Name as string, Text.NotEditableGroup, StringComparison.Ordinal)) return false;
            int limit = p.Length < 2 ? int.MaxValue : int.Parse(p[1], CultureInfo.InvariantCulture);
            return group.ItemCount <= limit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return Binding.DoNothing; }
    }
}
