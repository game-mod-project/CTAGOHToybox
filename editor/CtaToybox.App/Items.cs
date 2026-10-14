// 화면 목록이 묶는 항목과 표시 문자열. 판단(고칠 수 있는지, 범위 등)은 EditorSession이 하고, 여기서는 글자만 만든다.
using System;
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
            if (row.Note == null && row.IsMacroCall) return "[매크로 호출] 이 인자를 쓰는 값들이 함께 바뀐다. 인자로 계산한 값은 전체 탭의 '고칠 수 없는 값'에 보인다";
            return Note(row.Note);
        }

        // 줄의 지금 값. 호출 안의 값은 원문처럼 이름(값 값)으로 묶는다: side(ger) lvl(lvl1 lvl2) cost(10.0)
        public static string ValuesLine(RowView row)
        {
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
        public static string ContextLine(string context, int repeats)
        {
            string rep = repeats > 1 ? "같은 줄 " + repeats + "곳을 하나로 보인다. 고치면 모두 바뀐다" : null;
            if (context == null) return rep;
            return "↳ " + context + (rep == null ? "" : "  ·  " + rep);
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

    // 묶음을 처음에 펼칠지: 작은 묶음만 펼친다. 고칠 수 없는 값은 접는다
    public sealed class ExpandSmallGroups : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var group = value as CollectionViewGroup;
            if (group == null) return true;
            if (string.Equals(group.Name as string, Text.NotEditableGroup, StringComparison.Ordinal)) return false;
            int limit = parameter == null ? int.MaxValue : int.Parse((string)parameter, CultureInfo.InvariantCulture);
            return group.ItemCount <= limit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return Binding.DoNothing; }
    }
}
