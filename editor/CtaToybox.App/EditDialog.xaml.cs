// 키 한 줄의 값을 고친다. 값이 여럿이면(예: 아이템 이름과 수량) 각각 칸이 있고, 값마다 어느 파일에 있고 몇 개 유닛이 같이 쓰는지 보인다.
// 같이 쓰는 값이 있으면 "전부 / 이 유닛만"을 고르게 한다. "이 유닛만"이 안 되는 값이 하나라도 있으면 선택지를 끄고 이유를 적는다.
// 판단과 저장은 EditorSession.ApplyRow가 한 번에 한다(실패하면 그 줄의 변경을 모두 되돌린다).
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class EditDialog : Window
    {
        const int MaxUsers = 500;
        readonly EditorSession session;
        readonly UnitView unit;
        readonly RowView row;
        readonly Dictionary<int, TextBox> boxes = new Dictionary<int, TextBox>();

        public EditDialog(EditorSession session, UnitView unit, RowView row, string label, string context)
        {
            InitializeComponent();
            this.session = session; this.unit = unit; this.row = row;
            var editable = row.Values.Where(v => v.Editable).ToList();
            var widest = editable.OrderByDescending(v => v.SharedBy).First();
            KeyText.Text = label;
            ContextText.Text = (context == null ? "" : "부위: " + context + "\n") + "키: " + (row.RawPath ?? row.Path);
            NoteText.Text = Text.RowNote(row) + (row.Note == null ? "" : "\n출처: " + row.Note.Source);
            var files = editable.Select(v => v.Trace.File).Distinct().ToList();
            OriginText.Text = files.Count == 1 ? "값이 있는 파일: " + files[0] : "값마다 있는 파일이 다르다. 아래 값 옆에 적었다";

            for (int i = 0; i < row.Values.Count; i++)
            {
                var v = row.Values[i];
                var line = new DockPanel { Margin = new Thickness(0, 3, 0, 3) };
                var caption = new StackPanel { Width = 300 };
                // 첫 줄: 값의 뜻. 둘째 줄: 원문의 이름과 자리, 원래 값
                caption.Children.Add(new TextBlock { TextWrapping = TextWrapping.Wrap, FontWeight = FontWeights.SemiBold, Text = Text.ValueName(row, i) });
                caption.Children.Add(new TextBlock { TextWrapping = TextWrapping.Wrap, Foreground = Brushes.DimGray, FontSize = 12,
                    Text = (row.Values.Count > 1 ? (i + 1) + "번째 값" : "값") + (v.Name == null ? "" : " · 원문 " + v.Name + "()") + " · 원래 " + Text.Value(v.Trace.Text) +
                           (v.Edit == null ? "" : "  (수정됨: " + Text.Value(v.Edit.New) + ", " + (v.Edit.Scope == "unit" ? "이 유닛만" : "전부") + ")") });
                if (v.Editable && files.Count > 1)
                    caption.Children.Add(new TextBlock { Foreground = Brushes.Gray, FontSize = 12, TextWrapping = TextWrapping.Wrap,
                        Text = v.Trace.File.Substring(v.Trace.File.LastIndexOf('/') + 1) + " · 유닛 " + v.SharedBy + "개가 같이 씀" });
                DockPanel.SetDock(caption, Dock.Left);
                line.Children.Add(caption);
                if (v.Editable)
                {
                    var box = new TextBox { Text = Text.Value(v.Edit != null ? v.Edit.New : v.Trace.Text), VerticalAlignment = VerticalAlignment.Top };
                    box.TextChanged += (s, e) => Validate();
                    line.Children.Add(box);
                    boxes[i] = box;
                }
                else line.Children.Add(new TextBlock { Text = "고칠 수 없다: " + v.Trace.Note, Foreground = Brushes.Gray, TextWrapping = TextWrapping.Wrap });
                ValuesPanel.Children.Add(line);
            }

            // 범위: 같이 쓰는 값이 하나라도 있으면 묻는다. 영향받는 유닛은 가장 많이 같이 쓰는 값의 파일 기준이다
            var users = session.Index.UsersOf(widest.Trace.File);
            ScopePanel.Visibility = widest.SharedBy > 1 ? Visibility.Visible : Visibility.Collapsed;
            SharedRadio.Content = "전부 (이 값이 있는 파일을 쓰는 유닛 " + widest.SharedBy + "개가 모두 바뀐다)";
            UsersExpander.Header = "영향받는 유닛 " + users.Count + "개 보기";
            UsersList.ItemsSource = users.Take(MaxUsers).Select(u => Text.UnitName(session, u) + "   " + u).ToList();
            var blockers = editable.Where(v => v.UnitScopeBlocker != null).Select(v => v.UnitScopeBlocker).Distinct().ToList();
            UnitRadio.IsEnabled = blockers.Count == 0;
            BlockerText.Text = blockers.Count == 0 ? "" : "이 유닛만은 안 된다: " + string.Join(" / ", blockers);
            if (editable.Any(v => v.Edit != null && v.Edit.Scope == "unit") && UnitRadio.IsEnabled) UnitRadio.IsChecked = true;
            RemoveButton.Visibility = row.Values.Any(v => v.Edit != null) ? Visibility.Visible : Visibility.Collapsed;
            Validate();
        }

        string Scope { get { return UnitRadio.IsChecked == true ? "unit" : "shared"; } }

        // 원래 값이 따옴표 문자열인데 따옴표 없이 썼으면 따옴표를 붙인다
        static string Token(string original, string typed)
        {
            string s = typed.Trim();
            return original.StartsWith("\"") && !s.StartsWith("\"") ? "\"" + s + "\"" : s;
        }

        // 줄의 값 순서대로 새 토큰. 고칠 수 없는 값의 자리는 null
        List<string> Tokens()
        {
            var result = new List<string>();
            for (int i = 0; i < row.Values.Count; i++)
                result.Add(boxes.ContainsKey(i) ? Token(row.Values[i].Trace.Text, boxes[i].Text) : null);
            return result;
        }

        void Validate()
        {
            if (session == null || MessageText == null || OkButton == null) return;   // InitializeComponent 중에 라디오 단추의 Checked가 먼저 온다
            var tokens = Tokens();
            var errors = new List<string>();
            var warnings = new List<string>();
            bool changed = false;
            for (int i = 0; i < row.Values.Count; i++)
            {
                var v = row.Values[i];
                if (tokens[i] == null) continue;
                string current = v.Edit != null ? v.Edit.New : v.Trace.Text;
                if (tokens[i] != current || (v.Edit != null && v.Edit.Scope != Scope)) changed = true;
                if (tokens[i] == v.Trace.Text) continue;
                string warning;
                string error = session.ValidateNew(unit, v, tokens[i], Scope, out warning);
                if (error != null) errors.Add("값 " + (i + 1) + ": " + error);
                else if (warning != null) warnings.Add("값 " + (i + 1) + ": " + warning);
            }
            MessageText.Text = errors.Count > 0 ? string.Join("\n", errors) : string.Join("\n", warnings.Select(w => "경고: " + w));
            MessageText.Foreground = errors.Count > 0 ? Brushes.Firebrick : Brushes.DarkGoldenrod;
            OkButton.IsEnabled = errors.Count == 0 && changed;
        }

        void Scope_Changed(object sender, RoutedEventArgs e) { Validate(); }

        bool Run(Action action)
        {
            try { action(); return true; }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is System.IO.IOException || ex is UnauthorizedAccessException)
            {
                MessageText.Text = "저장하지 못했다(바뀐 것 없음): " + ex.Message;
                MessageText.Foreground = Brushes.Firebrick;
                return false;
            }
        }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (Run(() => session.ApplyRow(unit, row, Tokens(), Scope))) DialogResult = true;
        }

        // 화면 확인 시나리오(--scenario)가 사람 대신 누른다. 사람이 하는 것과 같은 칸과 단추를 쓴다
        public void TypeValue(int index, string text) { boxes[index].Text = text; }
        public void ChooseScope(bool unitOnly) { (unitOnly ? UnitRadio : SharedRadio).IsChecked = true; }
        public bool PressOk()
        {
            if (!OkButton.IsEnabled) return false;
            bool ok = Run(() => session.ApplyRow(unit, row, Tokens(), Scope));
            if (ok) Close();
            return ok;
        }

        // 이 줄에 보이는 수정(이 유닛에 적용되는 것)을 원래 값으로 되돌린다. 범위마다 ApplyRow로 한 번에 처리한다
        void Remove_Click(object sender, RoutedEventArgs e)
        {
            var originals = row.Values.Select(v => v.Editable ? v.Trace.Text : null).ToList();
            bool ok = Run(() =>
            {
                foreach (string scope in row.Values.Where(v => v.Edit != null).Select(v => v.Edit.Scope == "unit" ? "unit" : "shared").Distinct().ToList())
                    session.ApplyRow(unit, row, originals, scope);
            });
            if (ok) DialogResult = true;
        }
    }
}
