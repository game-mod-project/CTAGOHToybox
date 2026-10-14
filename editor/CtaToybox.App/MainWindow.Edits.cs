// 수정 목록, 프로젝트 열기, 빌드
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CtaToybox.Core;
using Microsoft.Win32;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        void OpenProjectFile(string path)
        {
            try { session.OpenProject(path); }
            catch (InvalidDataException ex) { MessageBox.Show(ex.Message, "프로젝트"); }
            BuildButton.IsEnabled = session.Project != null;
            ProjectText.Text = session.Project == null ? "(없음. 위의 열기...로 연다)" : session.Project.Name + "  (" + session.ProjectPath + ")";
            RefreshEdits();
            RefreshUnitViews();
        }

        public void RefreshEdits()
        {
            if (session == null || session.Project == null) return;
            var checks = session.CheckEdits().ToDictionary(c => c.Edit);
            var items = session.Project.Edits.Select(e =>
            {
                EditCheck c;
                checks.TryGetValue(e, out c);
                string status = !e.Enabled ? "" : c == null ? "" : c.Status == EditStatus.Applied ? "" :
                    (c.Status == EditStatus.SourceChanged ? "원본이 바뀜: 지금 " + c.Current : c.Status == EditStatus.TargetMissing ? "대상 없음: " + c.Message : "적용 불가: " + c.Message);
                return new EditItem
                {
                    Edit = e, Enabled = e.Enabled,
                    Title = e.Path + " [" + e.Occurrence + "] 값 " + e.Arg + ":  " + e.Old + " → " + e.New,
                    Detail = (e.Scope == "unit" ? "이 유닛만" : "전부(유닛 " + session.Index.UsersOf(e.File).Count + "개)") + "  ·  " + e.File,
                    Status = status,
                };
            }).ToList();
            var view = new ListCollectionView(items);
            view.GroupDescriptions.Add(new PropertyGroupDescription(null, new UnitGroupConverter(session)));
            EditList.ItemsSource = view;
            int problems = items.Count(i => i.Status.Length > 0);
            EditCountText.Text = "수정 " + items.Count + "건" + (problems > 0 ? ", 문제 " + problems + "건" : "");
        }

        // 수정 목록을 유닛별로 묶는다
        sealed class UnitGroupConverter : IValueConverter
        {
            readonly EditorSession session;
            public UnitGroupConverter(EditorSession session) { this.session = session; }
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var e = ((EditItem)value).Edit;
                string unit = SourceStack.ToVirtual(e.Unit ?? "");
                string name = session.Names.OfUnit(unit);
                return (name == null ? "" : name + "  ") + unit;
            }
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { throw new NotSupportedException(); }
        }

        EditItem SelectedEdit { get { return EditList.SelectedItem as EditItem; } }

        void EditEnabled_Click(object sender, RoutedEventArgs e)
        {
            var item = (EditItem)((CheckBox)sender).Tag;
            Try("수정 켜고 끄기", () => session.SetEnabled(item.Edit, ((CheckBox)sender).IsChecked == true));
            RefreshEdits();
            RefreshUnitViews();
        }

        void RemoveEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item == null) return;
            Try("수정 삭제", () => session.RemoveEdit(item.Edit));
            RefreshEdits();
            RefreshUnitViews();
        }

        void RebaseEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item == null) return;
            Try("지금 값에 맞추기", () => session.Rebase(item.Edit));
            RefreshEdits();
        }

        void OpenEditUnit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item != null) ShowUnit(SourceStack.ToVirtual(item.Edit.Unit ?? item.Edit.File));
        }

        void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (session == null) return;
            string dir = session.ProjectPath != null ? Path.GetDirectoryName(Path.GetFullPath(session.ProjectPath)) : Path.Combine(options.Repo, "editor", "projects");
            var dialog = new OpenFileDialog { Filter = "프로젝트 (*.json)|*.json", CheckFileExists = false, InitialDirectory = Directory.Exists(dir) ? dir : options.Repo };
            if (dialog.ShowDialog(this) == true) OpenProjectFile(dialog.FileName);
        }

        void Build_Click(object sender, RoutedEventArgs e)
        {
            if (EditorSession.GameRunning() &&
                MessageBox.Show("게임이나 에디터가 실행 중이다. 빌드해도 다시 시작해야 반영된다. 계속할까?", "빌드", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            if (session == null || session.Project == null) return;
            BuildReport report = null;
            if (!Try("빌드", () => report = session.Build())) return;
            var lines = new System.Collections.Generic.List<string>
            {
                "적용 " + report.Count(EditStatus.Applied) + "건, 원본이 바뀜 " + report.Count(EditStatus.SourceChanged) + "건, 대상 없음 " +
                report.Count(EditStatus.TargetMissing) + "건, 적용 불가 " + report.Count(EditStatus.Invalid) + "건, 파일 오류 " + report.FileErrors.Count + "건",
            };
            lines.AddRange(session.Plan.Warnings.Select(w => "경고: " + w));
            lines.AddRange(report.Warnings.Select(w => "경고: " + w));
            lines.AddRange(report.FileErrors.Select(x => "오류: " + x));
            lines.AddRange(report.Results.Where(r => r.Status != EditStatus.Applied).Take(20).Select(r => r.Status + ": " + r.Edit.File + " " + r.Edit.Path + (r.Message == null ? "" : " : " + r.Message)));
            lines.Add("씀 " + report.WrittenFiles.Count + "개, 지움 " + report.RemovedFiles.Count + "개");
            MessageBox.Show(string.Join("\n", lines), "빌드 결과");
            RefreshEdits();
        }
    }
}
