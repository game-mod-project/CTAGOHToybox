// 설정 파일이 없을 때 경로를 받는다. 프로필의 options.set은 찾을 수 있으면 미리 채운다(offline 프로필은 뺀다).
using System;
using System.IO;
using System.Linq;
using System.Windows;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class SettingsDialog : Window
    {
        readonly string settingsPath;

        public SettingsDialog(string settingsPath, string repo)
        {
            InitializeComponent();
            this.settingsPath = settingsPath;
            string profiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "digitalmindsoft", "gates of hell", "profiles");
            if (Directory.Exists(profiles))
            {
                var found = Directory.GetDirectories(profiles).Where(d => !string.Equals(Path.GetFileName(d), "offline", StringComparison.OrdinalIgnoreCase))
                    .Select(d => Path.Combine(d, "options.set")).FirstOrDefault(File.Exists);
                if (found != null) OptionsBox.Text = found;
            }
            OutBox.Text = Path.Combine(repo, "mod", "CTAToybox");
        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            string game = GameBox.Text.Trim(), workshop = WorkshopBox.Text.Trim(), options = OptionsBox.Text.Trim(), output = OutBox.Text.Trim();
            if (!Directory.Exists(Path.Combine(game, "resource"))) { ErrorText.Text = "게임 폴더에 resource 폴더가 없다: " + game; return; }
            if (!Directory.Exists(workshop)) { ErrorText.Text = "창작마당 폴더가 없다: " + workshop; return; }
            if (!File.Exists(options)) { ErrorText.Text = "options.set이 없다: " + options; return; }
            if (!File.Exists(Path.Combine(output, "mod.info"))) { ErrorText.Text = "출력 모드 폴더에 mod.info가 없다: " + output; return; }
            string full = Path.GetFullPath(output).TrimEnd(Path.DirectorySeparatorChar);
            new EditorSettings { GameDir = Path.GetFullPath(game), WorkshopDir = Path.GetFullPath(workshop), OptionsSetPath = Path.GetFullPath(options), OutputModDir = full, OutputModName = Path.GetFileName(full) }.Save(settingsPath);
            DialogResult = true;
        }
    }
}
