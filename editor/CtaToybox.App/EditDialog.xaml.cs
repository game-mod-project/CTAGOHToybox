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
        readonly Dictionary<int, List<TextBox>> pieceBoxes = new Dictionary<int, List<TextBox>>();   // 문자열 속 설정: 값 자리 → 조각 칸
        readonly Dictionary<int, TextBlock> stepTexts = new Dictionary<int, TextBlock>();             // 단계표: 값 자리 → 풀이

        public EditDialog(EditorSession session, UnitView unit, RowView row, string label, string context)
        {
            InitializeComponent();
            this.session = session; this.unit = unit; this.row = row;
            var editable = row.Values.Where(v => v.Editable).ToList();
            var widest = editable.OrderByDescending(v => v.SharedBy).FirstOrDefault();
            KeyText.Text = label;
            ContextText.Text = (context == null ? "" : "부위: " + context + "\n") + "키: " + (row.RawPath ?? row.Path);
            NoteText.Text = Text.RowNote(row) + (row.Note == null ? "" : "\n출처: " + row.Note.Source);
            var files = (editable.Count > 0 ? editable : row.Values).Select(v => v.Trace.File).Where(f => f != null).Distinct().ToList();
            OriginText.Text = files.Count == 1 ? "값이 있는 파일: " + files[0] : "값마다 있는 파일이 다르다. 아래 값 옆에 적었다";

            for (int i = 0; i < row.Values.Count; i++)
            {
                var v = row.Values[i];
                var line = new DockPanel { Margin = new Thickness(0, 3, 0, 3) };
                var caption = new StackPanel { Width = 300 };
                // 첫 줄: 값의 뜻. 둘째 줄: 원문의 이름과 자리, 원래 값
                caption.Children.Add(new TextBlock { TextWrapping = TextWrapping.Wrap, FontWeight = FontWeights.SemiBold, Text = Text.ValueName(row, i) });
                caption.Children.Add(new TextBlock { TextWrapping = TextWrapping.Wrap, Foreground = Brushes.DimGray, FontSize = 12,
                    Text = (row.Values.Count > 1 ? (i + 1) + "번째 값" : "값") + (v.Name == null ? "" : " · 원문 " + v.Name + "()") + " · 원래 " + Text.Value(v.Trace.Text) + (v.Computed == null ? "" : " (= " + v.Computed + ")") +
                           (v.Edit == null ? "" : "  (수정됨: " + Text.Value(v.Edit.New) + ", " + (v.Edit.Scope == "unit" ? "이 유닛만" : "전부") + ")") });
                if (v.Editable && files.Count > 1)
                    caption.Children.Add(new TextBlock { Foreground = Brushes.Gray, FontSize = 12, TextWrapping = TextWrapping.Wrap,
                        Text = v.Trace.File.Substring(v.Trace.File.LastIndexOf('/') + 1) + (SettingFiles.InArea(v.Trace.File) ? " · 게임 설정 파일" : " · 유닛 " + v.SharedBy + "개가 같이 씀") });
                if (v.Editable && v.Pieces != null)
                {
                    // 문자열 속 설정: 조각마다 칸. 이미 수정이 있으면 그 새 값에서 시작한다(앞에 고친 조각이 남는다)
                    var start = SettingStrings.StartPieces(v.Pieces, v.Edit == null ? null : v.Edit.New);
                    var block = new StackPanel { Margin = new Thickness(0, 3, 0, 3) };
                    block.Children.Add(caption);
                    var list = new List<TextBox>();
                    for (int j = 0; j < v.Pieces.Count; j++)
                    {
                        var pieceLine = new DockPanel { Margin = new Thickness(12, 2, 0, 2) };
                        var pieceCaption = new TextBlock { Width = 288, TextWrapping = TextWrapping.Wrap, FontSize = 12,
                            Text = (v.PieceLabels[j] ?? v.Pieces[j].Name + " (뜻 미확인)") + "\n원문 " + v.Pieces[j].Name + " · 원래 " + v.Pieces[j].Value };
                        DockPanel.SetDock(pieceCaption, Dock.Left);
                        pieceLine.Children.Add(pieceCaption);
                        var pieceBox = new TextBox { Text = start[j].Value, VerticalAlignment = VerticalAlignment.Top };
                        pieceBox.TextChanged += (s, e) => Validate();
                        pieceLine.Children.Add(pieceBox);
                        block.Children.Add(pieceLine);
                        list.Add(pieceBox);
                    }
                    pieceBoxes[i] = list;
                    ValuesPanel.Children.Add(block);
                    continue;
                }
                DockPanel.SetDock(caption, Dock.Left);
                line.Children.Add(caption);
                if (v.Editable)
                {
                    var box = new TextBox { Text = Text.Value(v.Edit != null ? v.Edit.New : v.Trace.Text), VerticalAlignment = VerticalAlignment.Top };
                    box.TextChanged += (s, e) => Validate();
                    boxes[i] = box;
                    if (v.Choices != null)
                    {
                        // 매크로 호출 이름(E4): 같은 파일에 정의된 이름만 고를 수 있다. 다른 이름은 게임이 시작할 때 꺼진다
                        var column = new StackPanel();
                        column.Children.Add(box);
                        column.Children.Add(new TextBlock { Foreground = Brushes.DimGray, FontSize = 12, TextWrapping = TextWrapping.Wrap,
                            Text = "고를 수 있는 이름(같은 파일의 정의): " + string.Join(", ", v.Choices) });
                        line.Children.Add(column);
                    }
                    else if (v.Steps != null)
                    {
                        // 판 수 단계표: 칸 아래에 새 값의 풀이를 보인다
                        var column = new StackPanel();
                        column.Children.Add(box);
                        var preview = new TextBlock { Foreground = Brushes.DimGray, FontSize = 12, TextWrapping = TextWrapping.Wrap };
                        column.Children.Add(preview);
                        stepTexts[i] = preview;
                        line.Children.Add(column);
                    }
                    else line.Children.Add(box);
                }
                else line.Children.Add(new TextBlock { Text = "고칠 수 없다: " + v.Trace.Note, Foreground = Brushes.Gray, TextWrapping = TextWrapping.Wrap });
                ValuesPanel.Children.Add(line);
            }

            // 범위: 같이 쓰는 값이 하나라도 있으면 묻는다. 영향받는 유닛은 가장 많이 같이 쓰는 값의 파일 기준이다
            if (widest == null)
            {
                // 고칠 값이 없는 줄: 게임 설정 파일의 따옴표 이름 블록이면 복제만 할 수 있다(E18)
                ScopePanel.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = false;
                RemoveButton.Visibility = Visibility.Collapsed;
                if (Text.IsCopyableBlock(row)) AddCopyPanel();
                return;
            }
            var users = session.Index.UsersOf(widest.Trace.File);
            ScopePanel.Visibility = widest.SharedBy > 1 ? Visibility.Visible : Visibility.Collapsed;
            SharedRadio.Content = "전부 (이 값이 있는 파일을 쓰는 유닛 " + widest.SharedBy + "개가 모두 바뀐다)";
            UsersExpander.Header = "영향받는 유닛 " + users.Count + "개 보기";
            UsersList.ItemsSource = users.Take(MaxUsers).Select(u => Text.UnitName(session, u) + "   " + u).ToList();
            var blockers = editable.Where(v => v.UnitScopeBlocker != null).Select(v => v.UnitScopeBlocker).Distinct().ToList();
            UnitRadio.IsEnabled = blockers.Count == 0;
            // 원형에서 물려받은 줄의 '이 유닛만'은 원형 파일이 아니라 유닛 파일에 키를 덧붙인다(E14)
            if (row.From != null && UnitRadio.IsEnabled) UnitRadio.Content = "이 유닛만 (이 유닛 파일에 이 키를 덧붙인다. 원형을 물려받는 다른 유닛은 그대로)";
            BlockerText.Text = blockers.Count == 0 ? "" : "이 유닛만은 안 된다: " + string.Join(" / ", blockers);
            if (editable.Any(v => v.Edit != null && v.Edit.Scope == "unit") && UnitRadio.IsEnabled) UnitRadio.IsChecked = true;
            RemoveButton.Visibility = row.Values.Any(v => v.Edit != null) ? Visibility.Visible : Visibility.Collapsed;
            Validate();
        }

        TextBox copyBox;

        // 블록 복제 칸(E18): 새 이름을 쓰고 단추를 누르면 수정 목록에 넣는다. 복제된 블록의 값은 전체 탭에서 다른 값처럼 고친다
        void AddCopyPanel()
        {
            var panel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
            panel.Children.Add(new TextBlock { Text = "이 블록 복제", FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Foreground = Brushes.DimGray, FontSize = 12, TextWrapping = TextWrapping.Wrap,
                Text = "이 블록(" + Text.Value(row.Values[0].Trace.Text) + ")을 새 이름으로 복사해 같은 파일에 더한다. 새 이름은 빈칸·따옴표·/ 없이 쓰고, 파일에 없는 이름이어야 한다. 다른 파일이 이 새 이름으로 부르게 하면 그 블록의 값이 쓰인다" });
            var line = new DockPanel { Margin = new Thickness(0, 4, 0, 0) };
            var button = new Button { Content = "이 블록 복제", Padding = new Thickness(10, 2, 10, 2), Margin = new Thickness(8, 0, 0, 0) };
            DockPanel.SetDock(button, Dock.Right);
            copyBox = new TextBox();
            button.Click += (s, e) => { if (PressCopy()) DialogResult = true; };
            line.Children.Add(button);
            line.Children.Add(copyBox);
            panel.Children.Add(line);
            ValuesPanel.Children.Add(panel);
        }

        // 시나리오(copyblock)와 단추가 같이 쓴다
        public bool PressCopy(string name = null)
        {
            if (copyBox == null) return false;
            if (name != null) copyBox.Text = name;
            string typed = copyBox.Text.Trim().Trim('"');
            return Run(() => session.AddBlockCopy(unit, row.Values[0], "\"" + typed + "\""));
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
            {
                if (pieceBoxes.ContainsKey(i)) result.Add(SettingStrings.Join(row.Values[i].Trace.Text, pieceBoxes[i].Select(b => b.Text.Trim()).ToList()));
                else result.Add(boxes.ContainsKey(i) ? Token(row.Values[i].Trace.Text, boxes[i].Text) : null);
            }
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
            foreach (var kv in stepTexts) kv.Value.Text = "풀이: " + (SettingStrings.StepPreview(tokens[kv.Key], row.Path) ?? "꼴이 맞지 않는다");
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
        public void TypePiece(int index, string pieceName, string text)
        {
            int j = row.Values[index].Pieces.FindIndex(p => p.Name == pieceName);
            pieceBoxes[index][j].Text = text;
        }
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
