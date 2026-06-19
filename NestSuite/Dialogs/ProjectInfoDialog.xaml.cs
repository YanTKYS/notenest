using System.Windows;

namespace NestSuite.Dialogs;

public partial class ProjectInfoDialog : Window
{
    public ProjectInfoDialog(string information)
    {
        InitializeComponent();
        InfoBox.Text = information;
    }
}
