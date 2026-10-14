// 앱 진입점. 설정 파일이 없으면 설정 창을 먼저 띄운다.
using System;
using System.IO;
using System.Windows;

namespace CtaToybox.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 마지막 안전망: 처리하지 못한 예외로 창이 닫히지 않게 이유를 보인다. 원인은 그 자리에서 고친다
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show("예상하지 못한 오류: " + ex.Exception.GetType().Name + ": " + ex.Exception.Message, "CTAToybox");
                ex.Handled = true;
            };
            AppOptions options;
            try { options = AppOptions.Parse(e.Args); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message, "CTAToybox"); Shutdown(2); return; }

            if (!File.Exists(options.Settings))
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var dialog = new SettingsDialog(options.Settings, options.Repo);
                if (dialog.ShowDialog() != true) { Shutdown(1); return; }
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            var window = new MainWindow(options);
            MainWindow = window;
            window.Show();
        }
    }
}
