// 값 하나를 고친다. 공유되는 값이면 영향받는 유닛을 보이고 "전부 / 이 유닛만"을 고르게 한다.
// "이 유닛만"이 안 되는 자리는 선택지를 끄고 이유를 적는다. 판단은 모두 EditorSession이 한다.
using System;
using System.Linq;
using System.Windows;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class EditDialog : Window
    {
        const int MaxUsers = 300;
        readonly EditorSession session;
        readonly UnitView unit;
        readonly ValueView value;

        public EditDialog(EditorSession session, UnitView unit, RowView row, ValueView value)
        {
            InitializeComponent();
            this.session = session; this.unit = unit; this.value = value;
            var t = value.Trace;
            KeyText.Text = row.Path + " [" + row.Occurrence + "]" + (row.SummaryLabel == null ? "" : "  " + row.SummaryLabel);
            NoteText.Text = row.Note == null ? "설명 없음 (미확인)" : row.Note.Meaning + "  [" + row.Note.Grade + ", " + row.Note.Source + "]";
            OriginText.Text = "원본: " + t.File + "  " + t.KeyPath + " [" + t.Occurrence + "] 값 " + t.Arg;
            CurrentText.Text = "지금 값: " + t.Text;
            var users = session.Index.UsersOf(t.File);
            SharedText.Text = "이 파일을 쓰는 유닛 " + users.Count + "개" + (users.Count > MaxUsers ? " (앞의 " + MaxUsers + "개를 보인다)" : "");
            UsersList.ItemsSource = users.Take(MaxUsers).ToList();
            bool shared = users.Count > 1;
            ScopePanel.Visibility = shared ? Visibility.Visible : Visibility.Collapsed;
            UnitRadio.IsEnabled = value.UnitScopeBlocker == null;
            BlockerText.Text = value.UnitScopeBlocker == null ? "" : "이 유닛만: " + value.UnitScopeBlocker;
            if (value.Edit != null)
            {
                NewBox.Text = value.Edit.New;
                if (value.Edit.Scope == "unit" && UnitRadio.IsEnabled) UnitRadio.IsChecked = true;
            }
            else NewBox.Text = t.Text;
            RemoveButton.Visibility = value.Edit == null ? Visibility.Collapsed : Visibility.Visible;
            Validate();
        }

        string Scope { get { return UnitRadio.IsChecked == true ? "unit" : "shared"; } }

        void Validate()
        {
            if (session == null || MessageText == null || OkButton == null) return;   // InitializeComponent 중에 라디오 단추의 Checked가 먼저 온다
            string warning;
            string error = session.ValidateNew(unit, value, NewBox.Text.Trim(), Scope, out warning);
            MessageText.Text = error ?? (warning == null ? "" : "경고: " + warning);
            MessageText.Foreground = error != null ? System.Windows.Media.Brushes.Firebrick : System.Windows.Media.Brushes.DarkGoldenrod;
            OkButton.IsEnabled = error == null && NewBox.Text.Trim() != value.Trace.Text;
        }

        void NewBox_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e) { Validate(); }
        void Scope_Changed(object sender, RoutedEventArgs e) { Validate(); }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            try { session.AddEdit(unit, value, NewBox.Text.Trim(), Scope); }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is System.IO.IOException || ex is UnauthorizedAccessException)
            { MessageText.Text = ex.Message; MessageText.Foreground = System.Windows.Media.Brushes.Firebrick; return; }
            DialogResult = true;
        }

        void Remove_Click(object sender, RoutedEventArgs e)
        {
            try { session.RemoveEdit(value.Edit); }
            catch (Exception ex) when (ex is InvalidOperationException || ex is System.IO.IOException || ex is UnauthorizedAccessException)
            { MessageText.Text = ex.Message; MessageText.Foreground = System.Windows.Media.Brushes.Firebrick; return; }
            DialogResult = true;
        }
    }
}
