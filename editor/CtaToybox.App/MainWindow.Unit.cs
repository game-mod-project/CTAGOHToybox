// 유닛 보기: 요약 탭, 전체 탭, 참조하는 아이템, 값 고치기
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        const string NotEditableGroup = "(고칠 수 없는 값: 치환·계산으로 만든 값, include 인자)";
        List<ValueItem> fullItems = new List<ValueItem>();

        public void ShowUnit(string path)
        {
            unit = session.OpenUnit(path);
            if (unit == null)
            {
                UnitTitle.Text = "원본에 이 파일이 없다: " + path;
                UnitInfo.Text = "";
                SummaryList.ItemsSource = FullList.ItemsSource = ItemList.ItemsSource = null;
                return;
            }
            UnitTitle.Text = (unit.DisplayName == null ? "" : unit.DisplayName + "  ") + unit.Unit;
            UnitInfo.Text = "종류 " + (unit.Kind ?? "유닛 아님") + ", 계층 " + unit.Layer + ", 값 " + unit.Rows.Sum(r => r.Values.Count) + "개" +
                (unit.Unresolved.Count == 0 ? "" : ", 풀리지 않은 구문 " + unit.Unresolved.Count + "개: " + string.Join(", ", unit.Unresolved.Take(5)));
            ItemList.ItemsSource = unit.Items.Select(i => new ItemLinkItem
            {
                Link = i,
                Label = i.Name + "  →  " + (i.Files.Count == 0 ? "" : string.Join(", ", i.Files.Take(3)) + (i.Files.Count > 3 ? " 외 " + (i.Files.Count - 3) + "개" : "")) + "  (" + i.Rule + ")"
            }).ToList();
            RefreshUnitViews();
        }

        static ValueItem Item(RowView row, ValueView v, int index, bool summary)
        {
            var t = v.Trace;
            string key = summary ? row.SummaryLabel : row.Path + " [" + row.Occurrence + "]";
            if (row.Values.Count > 1) key += " #" + index;
            return new ValueItem
            {
                Row = row, Value = v, Key = key, Text = t.Text,
                Group = v.Editable ? t.File : NotEditableGroup,
                NewText = v.Edit == null ? "" : "→ " + v.Edit.New + (v.Edit.Scope == "unit" ? " (이 유닛만)" : " (전부)") + (v.Edit.Enabled ? "" : " 꺼짐"),
                Origin = v.Editable ? t.KeyPath + " [" + t.Occurrence + "] 값 " + t.Arg : t.Note,
                Shared = v.Editable ? v.SharedBy.ToString() : "",
                Scope = !v.Editable ? "" : v.UnitScopeBlocker == null ? "가능" : "불가",
                Note = row.Note == null ? "" : row.Note.Meaning + " [" + row.Note.Grade + "]",
            };
        }

        public void RefreshUnitViews()
        {
            if (unit == null) return;
            session.RefreshEdits(unit);
            SummaryList.ItemsSource = unit.Rows.Where(r => r.SummaryLabel != null)
                .SelectMany(r => r.Values.Select((v, i) => Item(r, v, i, true))).ToList();
            // 묶음 순서: 유닛 파일, 나머지 파일(처음 나온 순서), 고칠 수 없는 값. ListView는 항목 순서대로 묶음을 만든다
            var all = unit.Rows.SelectMany(r => r.Values.Select((v, i) => Item(r, v, i, false))).ToList();
            var firstSeen = all.Select(x => x.Group).Distinct().Select((g, i) => new { g, i }).ToDictionary(x => x.g, x => x.i);
            fullItems = all.OrderBy(x => x.Group == unit.Unit ? -1 : x.Group == NotEditableGroup ? int.MaxValue : firstSeen[x.Group]).ToList();
            ApplyFullFilter();
        }

        void ApplyFullFilter()
        {
            string f = FullFilter.Text.Trim();
            var shown = f.Length == 0 ? fullItems : fullItems.Where(x =>
                x.Key.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 || x.Text.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 ||
                x.Group.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 || x.Note.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            var view = new ListCollectionView(shown);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            FullList.ItemsSource = view;
        }

        void FullFilter_Changed(object sender, TextChangedEventArgs e) { if (unit != null) ApplyFullFilter(); }

        void ItemList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ItemList.SelectedItem as ItemLinkItem;
            if (item == null) return;
            if (item.Link.Files.Count != 1 || item.Link.Rule == ItemLinks.RuleCandidates)
            {
                MessageBox.Show("이 이름은 파일 하나로 이어지지 않는다(" + item.Link.Rule + "). 게임이 어느 파일을 쓰는지 확인하지 않았다.\n\n" + string.Join("\n", item.Link.Files.Take(20)), "아이템");
                return;
            }
            ShowUnit(item.Link.Files[0]);
        }

        void Value_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListView)sender).SelectedItem as ValueItem;
            if (item == null || unit == null) return;
            if (!item.Value.Editable) { MessageBox.Show(item.Value.Trace.Note ?? "고칠 수 없는 값이다", "값 고치기"); return; }
            var dialog = new EditDialog(session, unit, item.Row, item.Value) { Owner = this };
            if (dialog.ShowDialog() == true) { RefreshUnitViews(); RefreshEdits(); }
        }
    }
}
