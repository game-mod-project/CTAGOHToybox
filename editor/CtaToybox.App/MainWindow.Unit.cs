// 유닛 보기: 요약 탭, 전체 탭, 무기·탄약 탭, 값 고치기
using System;
using System.Collections.Generic;
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
        List<RowItem> fullItems = new List<RowItem>();

        public void ShowUnit(string path)
        {
            if (session == null) return;
            UnitView opened = null;
            if (!Try("유닛 열기", () => opened = session.OpenUnit(path))) return;
            unit = opened;
            if (unit == null)
            {
                UnitTitle.Text = "원본에 이 파일이 없다: " + path;
                UnitInfo.Text = "";
                SummaryList.ItemsSource = FullList.ItemsSource = ItemList.ItemsSource = null;
                return;
            }
            UnitTitle.Text = unit.DisplayName ?? unit.Unit.Substring(unit.Unit.LastIndexOf('/') + 1);
            UnitInfo.Text = UnitKind.Label(unit.Kind) + " · 출처 " + session.LayerLabel(unit.Layer) + " · " + unit.Unit +
                (unit.Unresolved.Count == 0 ? "" : "\n풀리지 않은 구문 " + unit.Unresolved.Count + "개: " + string.Join(", ", unit.Unresolved.Take(5)));
            ItemList.ItemsSource = unit.Items.Select(ItemLinkItemOf).ToList();
            ItemsTab.Header = "무기·탄약 (" + unit.Items.Count + ")";
            RefreshUnitViews();
        }

        ItemLinkItem ItemLinkItemOf(ItemLink link)
        {
            string name = link.Files.Count == 1 ? session.Names.OfUnit(link.Files[0]) : null;
            string where;
            if (link.Rule == ItemLinks.RuleNone) where = "이어지는 파일 없음 (미확인)";
            else if (link.Rule == ItemLinks.RuleCandidates) where = "후보 " + link.Files.Count + "개 (미확인): " + string.Join(", ", link.Files.Take(3).Select(f => f.Substring(f.LastIndexOf('/') + 1))) + (link.Files.Count > 3 ? " 외" : "");
            else where = link.Files[0] + "  (두 번 눌러 연다)";
            return new ItemLinkItem { Link = link, Title = (name == null ? "" : name + "  ·  ") + link.Name, Sub = where };
        }

        RowItem Item(RowView row, bool summary, Dictionary<string, int> groupOrder)
        {
            var editable = row.Values.Where(v => v.Editable).ToList();
            var first = editable.FirstOrDefault();
            string group;
            if (first == null) group = Text.NotEditableGroup;
            else if (summary) group = row.SummaryLabel;
            else if (row.IsMacroCall) group = "매크로 호출 · " + first.Trace.File;
            else group = (first.Trace.File == unit.Unit ? "이 유닛의 파일 · " : "") + first.Trace.File + (first.SharedBy > 1 ? "  (유닛 " + first.SharedBy + "개가 같이 씀)" : "");
            int order;
            if (!groupOrder.TryGetValue(group, out order)) { order = groupOrder.Count; groupOrder[group] = order; }

            var edits = row.Values.Where(v => v.Edit != null).ToList();
            string newValues = string.Join("  ", edits.Select(v =>
                (row.Values.Count > 1 ? Text.Value(v.Trace.Text) + " → " : "→ ") + Text.Value(v.Edit.New) +
                (v.Edit.Scope == "unit" ? " (이 유닛만)" : " (전부)") + (v.Edit.Enabled ? "" : " [꺼짐]")));
            return new RowItem
            {
                Row = row, Group = group, GroupOrder = order,
                // 요약에서는 묶음 머리가 항목 이름이므로 줄에는 어느 부위의 것인지만 보인다
                Label = summary ? (row.Context ?? "이 유닛") : KeyLabel.Readable(row.Path),
                Context = summary || row.Context == null ? null : "↳ " + row.Context,
                Values = string.Join("  ", row.Values.Select(v => Text.Value(v.Trace.Text))),
                NewValues = newValues,
                Note = Text.RowNote(row),
                Shared = first == null ? "" : editable.Max(v => v.SharedBy) + "개",
                Scope = first == null ? "" : editable.All(v => v.UnitScopeBlocker == null) ? "가능" : "불가",
                Tip = first == null ? (row.Values.Count > 0 ? row.Values[0].Trace.Note : null) : first.Trace.File + "\n" + first.Trace.KeyPath + " [" + first.Trace.Occurrence + "]" + (first.UnitScopeBlocker == null ? "" : "\n이 유닛만 불가: " + first.UnitScopeBlocker),
                Editable = first != null,
            };
        }

        public void RefreshUnitViews()
        {
            if (unit == null) return;
            session.RefreshEdits(unit);
            var summary = session.SummaryOf(unit);
            var summaryOrder = new Dictionary<string, int>();
            var summaryItems = unit.Rows.Where(r => r.SummaryLabel != null).Select(r => Item(r, true, summaryOrder)).ToList();
            // 요약 묶음은 요약 파일에 적힌 순서로
            foreach (var x in summaryItems) { int i = summary.Labels.IndexOf(x.Group); x.GroupOrder = i < 0 ? int.MaxValue : i; }
            var sview = new ListCollectionView(summaryItems.OrderBy(x => x.GroupOrder).ToList());
            sview.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            SummaryList.ItemsSource = sview;
            SummaryTab.Header = summaryItems.Count == 0 ? "요약 (없음)" : "요약";

            // 전체: 유닛 파일, 나머지 파일(처음 나온 순서), 매크로 호출, 고칠 수 없는 값
            var order = new Dictionary<string, int>();
            var all = unit.Rows.Select(r => Item(r, false, order)).ToList();
            fullItems = all.OrderBy(x => x.Group == Text.NotEditableGroup ? 3 : x.Row.IsMacroCall ? 2 : x.Group.StartsWith("이 유닛의 파일") ? 0 : 1).ThenBy(x => x.GroupOrder).ToList();
            ApplyFullFilter();
        }

        void ApplyFullFilter()
        {
            string f = FullFilter.Text.Trim();
            var shown = f.Length == 0 ? fullItems : fullItems.Where(x =>
                Has(x.Label, f) || Has(x.Values, f) || Has(x.Group, f) || Has(x.Note, f) || Has(x.Context, f) || Has(x.Row.Path, f)).ToList();
            var view = new ListCollectionView(shown);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            FullList.ItemsSource = view;
            FullTab.Header = "전체 (" + fullItems.Count + "줄)";
        }

        static bool Has(string s, string f) { return s != null && s.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0; }

        void FullFilter_Changed(object sender, TextChangedEventArgs e) { if (unit != null) ApplyFullFilter(); }

        void ItemList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ItemList.SelectedItem as ItemLinkItem;
            if (item == null) return;
            if (item.Link.Files.Count != 1 || item.Link.Rule == ItemLinks.RuleCandidates || item.Link.Rule == ItemLinks.RuleNone)
            {
                MessageBox.Show(this, "'" + item.Link.Name + "'은 파일 하나로 이어지지 않는다. 게임이 어느 파일을 쓰는지 확인하지 않았다.\n\n" +
                    (item.Link.Files.Count == 0 ? "후보 파일 없음" : "후보:\n" + string.Join("\n", item.Link.Files.Take(20))), "무기·탄약");
                return;
            }
            ShowUnit(item.Link.Files[0]);
        }

        void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListView)sender).SelectedItem as RowItem;
            if (item == null || unit == null) return;
            if (!item.Editable)
            {
                MessageBox.Show(this, "이 줄은 고칠 수 없다: " + (item.Tip ?? "치환이나 계산으로 만든 값이다") + "\n\n계산에 쓰인 원래 값은 전체 탭의 '매크로 호출' 묶음이나 그 값이 있는 파일에서 고친다.", "값 고치기");
                return;
            }
            var dialog = new EditDialog(session, unit, item.Row, item.Label, item.Context) { Owner = this };
            dialog.ShowDialog();
            RefreshUnitViews();   // 저장이 실패해 창을 닫아도 지금 상태를 다시 그린다
            RefreshEdits();
        }
    }
}
