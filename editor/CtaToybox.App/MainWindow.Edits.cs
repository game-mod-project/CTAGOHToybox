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
            catch (InvalidDataException ex) { MessageBox.Show(this, ex.Message, "프로젝트"); }
            BuildButton.IsEnabled = session.Project != null;
            ProjectText.Text = session.Project == null ? "(없음. 오른쪽 단추로 연다)" : session.Project.Name;
            ProjectText.ToolTip = session.ProjectPath;
            RefreshEdits();
            RefreshUnitViews();
        }

        public void RefreshEdits()
        {
            if (session == null || session.Project == null) { EditList.ItemsSource = null; EditCountText.Text = ""; return; }
            var checks = session.CheckEdits().ToDictionary(c => c.Edit);
            var items = session.Project.Edits.Select(e =>
            {
                EditCheck c;
                checks.TryGetValue(e, out c);
                string status = !e.Enabled || c == null || c.Status == EditStatus.Applied ? "" :
                    c.Status == EditStatus.SourceChanged ? "원본이 바뀜: 지금 원본 값은 " + Text.Value(c.Current) + ". '원본의 지금 값에 맞추기'나 '삭제'를 고른다" :
                    Text.Status(c.Status) + ": " + c.Message;
                string unitPath = SourceStack.ToVirtual(e.Unit ?? e.File ?? "");
                return new EditItem
                {
                    Edit = e, Enabled = e.Enabled,
                    Group = Text.UnitName(session, unitPath) + "   (" + unitPath.Substring(unitPath.LastIndexOf('/') + 1) + ")",
                    Title = Text.EditTitle(session.Describe(e), e),
                    Detail = (e.Scope == "unit" ? "이 유닛만" : "전부 (유닛 " + session.Index.UsersOf(e.File).Count + "개가 바뀐다)") + " · " + e.File.Substring(e.File.LastIndexOf('/') + 1) + (e.Enabled ? "" : " · 꺼짐"),
                    Status = status,
                };
            }).ToList();
            var view = new ListCollectionView(items);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            EditList.ItemsSource = view;
            int problems = items.Count(i => i.Status.Length > 0);
            EditCountText.Text = "수정 " + items.Count + "건" + (problems > 0 ? " · 문제 " + problems + "건" : "");
            EditHelp.Visibility = Visibility.Visible;
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
            if (item == null) { MessageBox.Show(this, "수정 목록에서 지울 수정을 먼저 고른다.", "삭제"); return; }
            Try("수정 삭제", () => session.RemoveEdit(item.Edit));
            RefreshEdits();
            RefreshUnitViews();
        }

        void RebaseEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item == null) { MessageBox.Show(this, "수정 목록에서 '원본이 바뀜'인 수정을 먼저 고른다.", "원본의 지금 값에 맞추기"); return; }
            Try("원본의 지금 값에 맞추기", () => session.Rebase(item.Edit));
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
            var dialog = new OpenFileDialog { Filter = "프로젝트 (*.json)|*.json", CheckFileExists = false, InitialDirectory = Directory.Exists(dir) ? dir : options.Repo, Title = "프로젝트 열기 (없는 이름을 쓰면 새로 만든다)" };
            if (dialog.ShowDialog(this) == true) OpenProjectFile(dialog.FileName);
        }

        void Build_Click(object sender, RoutedEventArgs e)
        {
            if (session == null || session.Project == null) return;
            // 실행 중인 게임은 켤 때 본 모드 파일을 나중에 다시 읽는다. 그 사이에 파일을 바꾸거나 지우면 꺼질 수 있다(2026-10-14)
            if (EditorSession.GameRunning() &&
                MessageBox.Show(this, "게임이나 에디터가 실행 중이다.\n\n실행 중에 모드 파일을 바꾸거나 지우면, 게임이 그 파일을 다시 읽을 때(예: 전투를 새로 시작할 때) 꺼질 수 있다. " +
                                "게임을 끄고 빌드하는 것이 안전하다.\n\n그래도 빌드할까? 빌드했으면 전투를 새로 시작하지 말고 게임을 다시 시작한다.",
                                "빌드", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            BuildReport report = null;
            if (!Try("빌드", () => report = session.Build())) return;
            var lines = new System.Collections.Generic.List<string>
            {
                "적용 " + report.Count(EditStatus.Applied) + "건, 원본이 바뀜 " + report.Count(EditStatus.SourceChanged) + "건, 대상 없음 " +
                report.Count(EditStatus.TargetMissing) + "건, 적용 불가 " + report.Count(EditStatus.Invalid) + "건, 파일 오류 " + report.FileErrors.Count + "건",
                "모드 폴더에 쓴 파일 " + report.WrittenFiles.Count + "개, 지운 파일 " + report.RemovedFiles.Count + "개",
            };
            lines.AddRange(session.Plan.Warnings.Select(w => "경고: " + w));
            lines.AddRange(report.Warnings.Select(w => "경고: " + w));
            lines.AddRange(report.FileErrors.Select(x => "오류: " + x));
            lines.AddRange(report.Results.Where(r => r.Status != EditStatus.Applied).Take(20).Select(r =>
                Text.Status(r.Status) + ": " + KeyLabel.Readable(r.Edit.Path) + " (" + r.Edit.File + ")" + (r.Message == null ? "" : " — " + r.Message)));
            lines.Add("");
            // 목록 파일이 깨져 멈춘 빌드처럼 아무것도 바뀌지 않았으면 반영된다고 하지 않는다
            lines.Add(report.WrittenFiles.Count + report.RemovedFiles.Count > 0 ? "게임을 다시 시작하면 반영된다." : "모드 폴더에 바뀐 것이 없다.");
            MessageBox.Show(this, string.Join("\n", lines), "빌드 결과");
            RefreshEdits();
        }
    }
}
