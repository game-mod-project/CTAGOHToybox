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
        const string AllChoice = "(전체)";

        readonly AppOptions options;
        EditorSession session;
        UnitView unit;

        public MainWindow(AppOptions options)
        {
            InitializeComponent();
            this.options = options;
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
                if (options.Screenshot != null) { SaveScreenshot(options.Screenshot); Close(); }
                return;
            }
            SourcesText.Text = string.Join(" → ", session.Plan.Layers.Select(l => l.Name)) + "  (출력 모드 " + session.Settings.OutputModName + (session.Plan.OutputModActive ? ", 켜져 있음" : ", 꺼져 있음") + ")";
            WarningText.Text = string.Join("\n", session.Plan.Warnings);
            KindBox.ItemsSource = new[] { AllChoice }.Concat(UnitKind.All).ToList();
            KindBox.SelectedIndex = 0;
            LayerBox.ItemsSource = new[] { AllChoice }.Concat(session.Plan.Layers.Select(l => l.Name)).ToList();
            LayerBox.SelectedIndex = 0;
            SearchBox.IsEnabled = KindBox.IsEnabled = LayerBox.IsEnabled = OpenProjectButton.IsEnabled = true;
            OpenProjectFile(options.Project);
            RunSearch();
            if (options.Unit != null) ShowUnit(options.Unit);
            if (options.Tab == "full") Tabs.SelectedItem = FullTab;
            if (options.Filter != null) FullFilter.Text = options.Filter;
            if (options.Screenshot != null)
            {
                await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                SaveScreenshot(options.Screenshot);
                Close();
            }
        }

        void Search_Changed(object sender, EventArgs e) { if (session != null) RunSearch(); }

        void RunSearch()
        {
            string text = SearchBox.Text.Trim();
            string kind = KindBox.SelectedItem as string, layer = LayerBox.SelectedItem as string;
            var found = session.Search(text.Length == 0 ? null : text, kind == AllChoice ? null : kind, layer == AllChoice ? null : layer).ToList();
            UnitList.ItemsSource = found.Take(MaxResults).Select(u =>
            {
                string name = session.Names.OfUnit(u.Path);
                return new UnitItem { Entry = u, Label = (name == null ? "" : name + "  ") + u.Path.Substring(u.Path.LastIndexOf('/') + 1) + "  [" + u.Kind + ", " + u.Layer + "]" };
            }).ToList();
            ResultCountText.Text = found.Count > MaxResults ? found.Count + "개 가운데 " + MaxResults + "개. 검색어를 좁힌다." : found.Count + "개";
        }

        void UnitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = UnitList.SelectedItem as UnitItem;
            if (item != null) ShowUnit(item.Entry.Path);
        }

        // PNG로 창 내용을 저장한다. 화면 확인을 사람 대신 할 때 쓴다(--screenshot)
        void SaveScreenshot(string path)
        {
            UpdateLayout();
            var content = (FrameworkElement)Content;
            var dpi = VisualTreeHelper.GetDpi(this);
            var bitmap = new RenderTargetBitmap((int)(content.ActualWidth * dpi.DpiScaleX), (int)(content.ActualHeight * dpi.DpiScaleY), 96 * dpi.DpiScaleX, 96 * dpi.DpiScaleY, PixelFormats.Pbgra32);
            bitmap.Render(content);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var f = File.Create(path)) encoder.Save(f);
        }
    }
}
