using System.Windows;

namespace NoteNest;

public partial class App : Application
{
    private void App_Startup(object sender, StartupEventArgs e)
    {
        // 第1引数があればファイルパスとして MainWindow に渡す。
        // スペースを含むパスはシェル/Explorerが "" で包んで渡すため、Args[0] はそのまま使える。
        var startupPath = e.Args.Length > 0 ? e.Args[0] : null;
        var window = new MainWindow(startupPath);
        window.Show();
    }
}
