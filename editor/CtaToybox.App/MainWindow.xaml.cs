// 주 창. 세션을 백그라운드에서 열고, 유닛 찾기를 맡는다. 유닛 보기는 MainWindow.Unit.cs, 수정 목록은 MainWindow.Edits.cs에 있다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class MainWindow : Window
    {
        const int MaxResults = 500;

        readonly AppOptions options;
        EditorSession session;
        UnitView unit;

        public MainWindow(AppOptions options)
        {
            InitializeComponent();
            this.options = options;
            SummaryList.View = (GridView)FindResource("RowColumns");
            FullList.View = (GridView)FindResource("RowColumns");
            RosterList.View = (GridView)FindResource("RowColumns");
            Loaded += async (s, e) => await LoadSession();
            Closed += (s, e) => { if (session != null) session.Dispose(); };
        }

        // 사용자가 고칠 수 있는 원인(파일 잠김, 깨진 주석·프로젝트 파일 등)으로 동작이 실패하면 창을 닫지 않고 이유를 보인다
        bool Try(string what, Action action)
        {
            try { action(); return true; }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is FormatException || ex is InvalidOperationException || ex is InvalidDataException || ex is ArgumentException)
            {
                MessageBox.Show(this, what + "에 실패했다: " + ex.Message, "CTAToybox");
                return false;
            }
        }

        void Say(string message) { Dispatcher.Invoke(() => StatusText.Text = message); }

        async Task LoadSession()
        {
            try
            {
                session = await Task.Run(() => EditorSession.Open(options.Settings, options.Repo, Say));
            }
            catch (Exception ex)
            {
                WarningText.Text = "원본을 열지 못했다: " + ex.Message;
                if (options.Screenshot != null) { SaveScreenshot(this, options.Screenshot); Close(); }
                return;
            }
            SourcesText.Text = "원본 (로드 순서, 뒤가 이긴다): " + string.Join("  →  ", session.Plan.Layers.Select(l => session.LayerLabel(l.Name))) +
                "    출력 모드 " + session.Settings.OutputModName + (session.Plan.OutputModActive ? " (켜져 있음)" : " (꺼져 있음)");
            WarningText.Text = string.Join("\n", session.Plan.Warnings);
            StatusText.Text = "유닛 " + session.Index.Units.Count.ToString("N0") + "개를 읽었다.";
            KindBox.ItemsSource = new[] { new Choice { Label = "전체", Value = null } }
                .Concat(UnitKind.Editor.Select(k => new Choice { Label = UnitKind.Label(k), Value = k })).ToList();
            KindBox.SelectedIndex = 1;   // 탱크
            LayerBox.ItemsSource = new[] { new Choice { Label = "전체", Value = null } }
                .Concat(session.Plan.Layers.Select(l => new Choice { Label = session.LayerLabel(l.Name), Value = l.Name })).ToList();
            LayerBox.SelectedIndex = 0;
            SearchBox.IsEnabled = KindBox.IsEnabled = LayerBox.IsEnabled = ShowXBox.IsEnabled = OpenProjectButton.IsEnabled = true;
            OpenProjectFile(options.Project);
            RunSearch();
            SearchBox.Focus();
            if (options.Unit != null) ShowUnit(options.Unit);
            if (options.Tab == "full") Tabs.SelectedItem = FullTab;
            if (options.Tab == "items") Tabs.SelectedItem = ItemsTab;
            if (options.Filter != null) FullFilter.Text = options.Filter;
            if (options.Screenshot != null)
            {
                await Shoot(options.EditRow, options.Screenshot);
                Close();
            }
            else if (options.Shots != null)
            {
                // 화면 확인용: 한 번 실행으로 여러 화면을 찍는다. 목록 파일 한 줄: 파일 이름|유닛|탭|거르기|값 고치기 창을 띄울 항목
                foreach (string line in File.ReadAllLines(options.Shots))
                {
                    if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                    string[] p = (line + "||||").Split('|');
                    if (p[1].Length > 0) ShowUnit(p[1]);
                    Tabs.SelectedItem = p[2] == "full" ? FullTab : p[2] == "items" ? ItemsTab : p[2] == "roster" ? RosterTab : SummaryTab;
                    FullFilter.Text = p[3];
                    await Shoot(p[4].Length == 0 ? null : p[4], Path.Combine(options.Out ?? Path.GetDirectoryName(options.Shots), p[0] + ".png"));
                }
                Close();
            }
        }

        void Search_Changed(object sender, EventArgs e) { if (session != null) RunSearch(); }

        void RunSearch()
        {
            string text = SearchBox.Text.Trim();
            var kind = KindBox.SelectedItem as Choice;
            var layer = LayerBox.SelectedItem as Choice;
            bool showX = ShowXBox.IsChecked == true;
            var found = session.Search(text.Length == 0 ? null : text, kind == null ? null : kind.Value, layer == null ? null : layer.Value)
                .Where(u => showX || !IsXVariant(u.Path)).ToList();
            UnitList.ItemsSource = found.Take(MaxResults).Select(u =>
            {
                string name = session.Names.OfUnit(u.Path);
                string file = u.Path.Substring(u.Path.LastIndexOf('/') + 1);
                return new UnitItem
                {
                    Entry = u,
                    Title = name ?? file,
                    Sub = (name == null ? "" : file + " · ") + UnitKind.Label(u.EditorKind) + " · " + session.LayerLabel(u.Layer),
                };
            }).ToList();
            ResultCountText.Text = found.Count > MaxResults ? found.Count.ToString("N0") + "개 가운데 앞의 " + MaxResults + "개 (이름순). 검색어를 더 쓴다." : found.Count.ToString("N0") + "개 (이름순)";
        }

        // x 폴더의 _x, _xx 파일: 원본 차량과 이름이 같은 파생 파일이다(무엇인지는 확인하지 않았다). 기본으로 숨긴다
        static bool IsXVariant(string path)
        {
            string leaf = path.Substring(path.LastIndexOf('/') + 1);
            int dot = leaf.LastIndexOf('.');
            string stem = dot < 0 ? leaf : leaf.Substring(0, dot);
            return path.StartsWith("entity/", StringComparison.Ordinal) && System.Text.RegularExpressions.Regex.IsMatch(stem, "_x+$");
        }

        void UnitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = UnitList.SelectedItem as UnitItem;
            if (item != null) ShowUnit(item.Entry.Path);
        }

        // PNG로 창 내용을 저장한다. 화면 확인을 사람 대신 할 때 쓴다(--screenshot)
        // 지금 화면을 찍는다. editRow가 있으면 그 항목 이름이 든 첫 줄의 값 고치기 창을 띄워 찍고 닫는다
        async Task Shoot(string editRow, string path)
        {
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
            if (editRow == null || unit == null) { SaveScreenshot(this, path); return; }
            var item = fullItems.Where(x => x.Editable).FirstOrDefault(x => x.Label.Contains(editRow) || x.Row.Path.Contains(editRow));
            if (item == null) { SaveScreenshot(this, path); return; }
            var dialog = new EditDialog(session, unit, item.Row, item.Row.SummaryLabel ?? item.Label, item.Context) { Owner = this };
            dialog.Show();
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
            dialog.UpdateLayout();
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
            SaveScreenshot(dialog, path);
            dialog.Close();
        }

        static void SaveScreenshot(Window window, string path)
        {
            window.UpdateLayout();
            // 창 템플릿의 맨 위 요소를 그린다: 내용의 여백까지 들어간다(내용만 그리면 여백만큼 밀려 잘린다)
            var content = (FrameworkElement)VisualTreeHelper.GetChild(window, 0);
            var dpi = VisualTreeHelper.GetDpi(window);
            var bitmap = new RenderTargetBitmap((int)(content.ActualWidth * dpi.DpiScaleX), (int)(content.ActualHeight * dpi.DpiScaleY), 96 * dpi.DpiScaleX, 96 * dpi.DpiScaleY, PixelFormats.Pbgra32);
            bitmap.Render(content);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var f = File.Create(path)) encoder.Save(f);
        }
    }
}
