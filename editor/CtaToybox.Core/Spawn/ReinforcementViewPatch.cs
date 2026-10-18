// 증원 창 단추 칸을 세로 목록에서 가로 격자로 바꾼다(사용자 요청: 하위 메뉴가 원게임 구매 창처럼 가로로 펼쳐지게).
// 바닐라 interface.pak!layout/views/scene/component/GameUnitReinforcementView.xml의 단추 칸은
// <StepScrollPane id="unit_types_container" visibleElements="7"> 세로 목록이다. 같은 id의 <GridPanel cols="N">으로 바꾼다.
// 구매 창(GameUnitSpawnView.xml)이 같은 꼴(GridPanel id="unit_container" cols="3" hgap="2" vgap="2")을 쓴다.
// 2026-10-18 이 판으로 캠페인 판을 열자 게임이 판 화면(scenegameview.xml)을 만들다 EXCEPTION_ACCESS_VIOLATION으로 꺼졌다.
// 같은 판의 SceneGameView.xml은 Spawn Menu(정상 동작) 것과 조건 줄만 다르고 싱글에서는 둘 다 켜진다. 바뀐 것은 이 칸의 종류(StepScrollPane → GridPanel)뿐이라
// 게임 코드가 이 칸을 StepScrollPane으로 다룬다고 본다(스키마의 StepScrollPane에는 방향 속성이 없다). 그래서 기본에서 뺐고 --grid N으로만 쓴다
using System;
using System.Text.RegularExpressions;

namespace CtaToybox.Core.Spawn
{
    public static class ReinforcementViewPatch
    {
        public const string Path = "interface/layout/views/scene/component/gameunitreinforcementview.xml";
        static readonly Regex Pane = new Regex("<StepScrollPane\\b[^>]*\\bid=\"unit_types_container\"[^>]*>(?<body>.*?)</StepScrollPane>", RegexOptions.Singleline | RegexOptions.CultureInvariant);

        public static string Patch(string xml, int cols, out string error)
        {
            error = null;
            if (cols < 1) { error = "열 수는 1 이상"; return null; }
            var m = Pane.Match(xml);
            if (!m.Success)
            {
                if (xml.Contains("<GridPanel id=\"unit_types_container\"")) return xml;
                error = "StepScrollPane id=\"unit_types_container\"를 찾지 못했다";
                return null;
            }
            string grid = "<GridPanel id=\"unit_types_container\" cols=\"" + cols + "\" hgap=\"2\" vgap=\"2\">" + m.Groups["body"].Value + "</GridPanel>";
            return xml.Substring(0, m.Index) + grid + xml.Substring(m.Index + m.Length);
        }

        static readonly Regex PaneTag = new Regex("<StepScrollPane\\b[^>]*\\bid=\"unit_types_container\"[^>]*>", RegexOptions.Singleline | RegexOptions.CultureInvariant);
        static readonly Regex Visible = new Regex("\\bvisibleElements=\"\\d+\"", RegexOptions.CultureInvariant);

        // 세로 목록은 그대로 두고 한 번에 보이는 단추 수만 바꾼다(StepScrollPane의 visibleElements, 바닐라 7. 실행 파일에 등록된 속성 VisibleElements).
        // 칸 종류는 바꾸지 않는다. 메뉴 한 화면은 유닛 21개와 이전·다음·뒤로·처음 단추로 25개까지다(MenuBuilder PageSize)
        public static string SetVisible(string xml, int count, out string error)
        {
            error = null;
            if (count < 1) { error = "보이는 단추 수는 1 이상"; return null; }
            var m = PaneTag.Match(xml);
            if (!m.Success) { error = "StepScrollPane id=\"unit_types_container\"를 찾지 못했다"; return null; }
            string tag = Visible.IsMatch(m.Value) ? Visible.Replace(m.Value, "visibleElements=\"" + count + "\"", 1) : m.Value.Insert("<StepScrollPane".Length, " visibleElements=\"" + count + "\"");
            return xml.Substring(0, m.Index) + tag + xml.Substring(m.Index + m.Length);
        }
    }
}
