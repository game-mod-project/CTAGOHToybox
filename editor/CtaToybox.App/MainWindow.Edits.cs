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
            BuildButton.IsEnabled = MergeProjectButton.IsEnabled = session.Project != null;
            ProjectText.Text = session.Project == null ? "(없음. 오른쪽 단추로 연다)" : session.Project.Name;
            ProjectText.ToolTip = session.ProjectPath;
            RefreshEdits();
            RefreshUnitViews();
        }

        public void RefreshEdits()
        {
            if (session == null || session.Project == null) { EditList.ItemsSource = null; EditCountText.Text = ""; RefreshRules(); return; }
            var checks = session.CheckEdits().ToDictionary(c => c.Edit);
            var items = session.Project.Edits.Select(e =>
            {
                EditCheck c;
                checks.TryGetValue(e, out c);
                string status = !e.Enabled || c == null || c.Status == EditStatus.Applied ? "" :
                    c.Status == EditStatus.SourceChanged ? "원본이 바뀜: 지금 원본 값은 " + Text.Value(c.Current) + ". '원본의 지금 값에 맞추기'나 '삭제'를 고른다" :
                    Text.Status(c.Status) + ": " + c.Message;
                string unitPath = SourceStack.ToVirtual(e.Unit ?? e.File ?? "");
                string unitGroup = Text.UnitName(session, unitPath) + "   (" + unitPath.Substring(unitPath.LastIndexOf('/') + 1) + ")";
                // 규칙이 만든 수정은 규칙 이름의 묶음에 모으고, 어느 유닛의 것인지는 설명 줄에 적는다(F1)
                var rule = e.Rule == null ? null : session.RuleOf(e.Rule);
                return new EditItem
                {
                    Edit = e, Enabled = e.Enabled,
                    Group = rule != null ? "규칙 · " + rule.Name : unitGroup,
                    Title = Text.EditTitle(session.Describe(e), e),
                    Detail = (rule != null ? unitGroup.Trim() + " · " : "") +
                             (e.Scope == "unit" ? "이 유닛만" : "전부 (" + Text.SharedScope(SourceStack.ToVirtual(e.File), session.Index.UsersOf(e.File).Count) + ")") + " · " + e.File.Substring(e.File.LastIndexOf('/') + 1) + (e.Enabled ? "" : " · 꺼짐"),
                    Status = status,
                };
            }).ToList();
            RefreshRules();
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

        // 일괄 규칙 고르기 칸. 꺼진 규칙은 수정이 없어 목록에 안 보이므로 이 칸에서 고른다(F1)
        void RefreshRules()
        {
            string keep = SelectedRuleId;
            var rules = session == null ? new System.Collections.Generic.List<BulkRule>() : session.Rules.ToList();
            RuleBox.ItemsSource = rules.Select(r => new Choice
            {
                Label = r.Name + (r.Enabled ? "" : " (꺼짐)") + " · 수정 " + session.Project.Edits.Count(x => x.Rule == r.Id) + "건",
                Value = r.Id,
            }).ToList();
            RuleBox.IsEnabled = rules.Count > 0;
            SelectRule(keep ?? (rules.Count > 0 ? rules[0].Id : null));
        }

        string SelectedRuleId { get { var c = RuleBox.SelectedItem as Choice; return c == null ? null : c.Value; } }

        void SelectRule(string id)
        {
            var list = RuleBox.ItemsSource as System.Collections.Generic.List<Choice>;
            RuleBox.SelectedItem = list == null ? null : list.FirstOrDefault(c => c.Value == id) ?? list.FirstOrDefault();
        }

        void EditList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SelectedEdit;
            if (item != null && item.Edit.Rule != null) SelectRule(item.Edit.Rule);
        }

        BulkRule PickedRule(string what)
        {
            var rule = SelectedRuleId == null ? null : session.RuleOf(SelectedRuleId);
            if (rule == null) MessageBox.Show(this, "규칙이 없다. 요약·전체 탭의 줄을 오른쪽 단추로 눌러 '이 키로 일괄 규칙...'으로 만든다.", what);
            return rule;
        }

        void AfterRuleChange()
        {
            RefreshEdits();
            RefreshUnitViews();
        }

        // 요약·전체 탭 줄의 오른쪽 단추 메뉴: 지금 유닛의 종류, 그 줄의 키, 처음 고칠 수 있는 값의 자리로 채운 규칙 창(F1)
        void RuleFromRow_Click(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)((MenuItem)sender).Parent;
            var item = ((ListView)menu.PlacementTarget).SelectedItem as RowItem;
            if (item == null || unit == null) { MessageBox.Show(this, "줄을 먼저 고른다.", "일괄 규칙"); return; }
            OpenRuleDialog(RuleForRow(item.Row), null, item.Row.Values.Count, RuleWarning(item.Row), true);
        }

        BulkRule RuleForRow(RowView row)
        {
            int arg = row.Values.FindIndex(v => v.Editable);
            return new BulkRule { Kind = unit.EditorKind, Key = row.Path, Arg = arg < 0 ? 0 : arg, Op = "mul", Value = "", Scope = "auto" };
        }

        static string RuleWarning(RowView row)
        {
            if (row.Note == null) return "이 키는 뜻이 주석에 없다(미확인). 게임에서 무엇이 바뀌는지 모르는 채로 여러 유닛에 걸린다.";
            if (row.Note.Grade == "미확인") return "이 키의 뜻은 '미확인' 등급이다. 게임에서 무엇이 바뀌는지 모르는 채로 여러 유닛에 걸린다.";
            return null;
        }

        // modal이 아니면(시나리오) 창을 돌려준다
        RuleDialog OpenRuleDialog(BulkRule start, string ruleId, int valueCount, string warning, bool modal)
        {
            var dialog = new RuleDialog(session, start, ruleId, valueCount, warning) { Owner = this };
            if (!modal) { dialog.Show(); return dialog; }
            if (dialog.ShowDialog() == true) AfterRuleChange();
            return null;
        }

        void EditRule_Click(object sender, RoutedEventArgs e)
        {
            var rule = PickedRule("규칙 고치기");
            if (rule != null) OpenRuleDialog(rule, rule.Id, Math.Max(1, rule.Arg + 1), null, true);
        }

        void ReapplyRule_Click(object sender, RoutedEventArgs e)
        {
            var rule = PickedRule("규칙 다시 적용");
            if (rule == null) return;
            int before = session.Project.Edits.Count(x => x.Rule == rule.Id);
            if (Try("규칙 다시 적용", () => session.ReapplyRule(rule.Id)))
                MessageBox.Show(this, "'" + rule.Name + "': 수정 " + before + "건 → " + session.Project.Edits.Count(x => x.Rule == rule.Id) + "건", "규칙 다시 적용");
            AfterRuleChange();
        }

        void ToggleRule_Click(object sender, RoutedEventArgs e)
        {
            var rule = PickedRule("규칙 끄기/켜기");
            if (rule == null) return;
            Try("규칙 끄기/켜기", () => session.SetRuleEnabled(rule.Id, !rule.Enabled));
            AfterRuleChange();
        }

        void RemoveRule_Click(object sender, RoutedEventArgs e)
        {
            var rule = PickedRule("규칙 지우기");
            if (rule == null) return;
            int count = session.Project.Edits.Count(x => x.Rule == rule.Id);
            if (MessageBox.Show(this, "'" + rule.Name + "' 규칙과 그 수정 " + count + "건을 지울까? 규칙에서 떨어진(사람이 고친) 수정은 남는다.", "규칙 지우기",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            Try("규칙 지우기", () => session.RemoveRule(rule.Id));
            AfterRuleChange();
        }

        void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (session == null) return;
            string dir = session.ProjectPath != null ? Path.GetDirectoryName(Path.GetFullPath(session.ProjectPath)) : Path.Combine(options.Repo, "editor", "projects");
            var dialog = new OpenFileDialog { Filter = "프로젝트 (*.json)|*.json", CheckFileExists = false, InitialDirectory = Directory.Exists(dir) ? dir : options.Repo, Title = "프로젝트 열기 (없는 이름을 쓰면 새로 만든다)" };
            if (dialog.ShowDialog(this) == true) OpenProjectFile(dialog.FileName);
        }

        // 다른 프로젝트의 수정을 이 프로젝트에 더한다(E17). 같은 자리에 다른 값이 있으면 이 프로젝트의 것을 두고 알린다
        void MergeProject_Click(object sender, RoutedEventArgs e)
        {
            if (session == null || session.Project == null) return;
            string dir = Path.GetDirectoryName(Path.GetFullPath(session.ProjectPath));
            var dialog = new OpenFileDialog { Filter = "프로젝트 (*.json)|*.json", CheckFileExists = true, InitialDirectory = Directory.Exists(dir) ? dir : options.Repo, Title = "이 프로젝트에 합칠 프로젝트" };
            if (dialog.ShowDialog(this) != true) return;
            MergeReport report = null;
            if (!Try("프로젝트 합치기", () => report = session.MergeProject(dialog.FileName))) return;
            var lines = new System.Collections.Generic.List<string> { "더한 수정 " + report.Added + "건, 이미 같은 수정 " + report.Same + "건, 충돌 " + report.Conflicts.Count + "건" };
            if (report.Conflicts.Count > 0) lines.Add("충돌은 이 프로젝트의 값을 그대로 두었다:");
            lines.AddRange(report.Conflicts.Take(20).Select(c => "  " + c));
            if (report.Conflicts.Count > 20) lines.Add("  외 " + (report.Conflicts.Count - 20) + "건");
            MessageBox.Show(this, string.Join("\n", lines), "프로젝트 합치기");
            RefreshEdits();
            RefreshUnitViews();
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
