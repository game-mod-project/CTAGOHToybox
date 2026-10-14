// 화면 목록이 묶는 항목. 표시 문자열만 만들고 판단은 하지 않는다(판단은 EditorSession이 한다).
using CtaToybox.Core;

namespace CtaToybox.App
{
    public sealed class UnitItem
    {
        public UnitEntry Entry { get; set; }
        public string Label { get; set; }
    }

    public sealed class ValueItem
    {
        public RowView Row { get; set; }
        public ValueView Value { get; set; }
        public string Group { get; set; }      // 전체 탭의 묶음: 값이 있는 파일
        public string Key { get; set; }        // 펼친 키 경로 [순번] 또는 요약 이름
        public string Text { get; set; }      // 원본 값
        public string NewText { get; set; }    // 걸린 수정의 새 값과 범위. 없으면 빈칸
        public string Origin { get; set; }     // 파일 안의 자리
        public string Shared { get; set; }     // 공유 수
        public string Scope { get; set; }      // "이 유닛만" 가능 여부
        public string Note { get; set; }       // 키 설명과 등급
    }

    public sealed class ItemLinkItem
    {
        public ItemLink Link { get; set; }
        public string Label { get; set; }
    }

    public sealed class EditItem
    {
        public Edit Edit { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }
    }
}
