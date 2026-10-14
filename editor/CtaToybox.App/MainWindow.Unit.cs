// 유닛 보기: 요약 탭, 전체 탭, 참조하는 아이템, 값 고치기
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        public void ShowUnit(string path)
        {
            unit = session.OpenUnit(path);
            UnitTitle.Text = unit == null ? "원본에 이 파일이 없다: " + path : unit.Unit;
        }

        public void RefreshUnitViews() { }
        void FullFilter_Changed(object sender, TextChangedEventArgs e) { }
        void ItemList_DoubleClick(object sender, MouseButtonEventArgs e) { }
        void Value_DoubleClick(object sender, MouseButtonEventArgs e) { }
    }
}
