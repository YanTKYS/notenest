using System.Windows;
using System.Windows.Input;
using NoteNest.ViewModels;

namespace NoteNest.Dialogs;

public record NotePickerItem(string NotebookTitle, NoteViewModel Note)
{
    public string DisplayText => $"{NotebookTitle} / {Note.Title}";
}

public partial class NotePickerDialog : Window
{
    public NoteViewModel? SelectedNote { get; private set; }

    public NotePickerDialog(IEnumerable<NotePickerItem> items)
    {
        InitializeComponent();
        NoteList.ItemsSource = items.ToList();
        Loaded += (_, _) =>
        {
            NoteList.Focus();
            if (NoteList.Items.Count > 0)
                NoteList.SelectedIndex = 0;
        };
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        if (NoteList.SelectedItem is NotePickerItem item)
        {
            SelectedNote = item.Note;
            DialogResult = true;
        }
    }

    private void NoteList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (NoteList.SelectedItem != null)
            OK_Click(sender, e);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
