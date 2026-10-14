// 수정 목록, 프로젝트 열기, 빌드
using System;
using System.IO;
using System.Windows;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        void OpenProjectFile(string path)
        {
            try { session.OpenProject(path); }
            catch (InvalidDataException ex) { MessageBox.Show(ex.Message, "프로젝트"); }
            ProjectText.Text = session.Project == null ? "(없음)" : session.Project.Name;
        }

        public void RefreshEdits() { }
        void OpenProject_Click(object sender, RoutedEventArgs e) { }
        void Build_Click(object sender, RoutedEventArgs e) { }
        void RemoveEdit_Click(object sender, RoutedEventArgs e) { }
        void RebaseEdit_Click(object sender, RoutedEventArgs e) { }
        void OpenEditUnit_Click(object sender, RoutedEventArgs e) { }
    }
}
