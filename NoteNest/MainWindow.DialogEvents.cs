using System.Windows;
using System.Windows.Controls;
using NoteNest.ViewModels;
using NoteNest.Views;

namespace NoteNest;

/// <summary>Dialog launchers, shared notification helpers, and IWorkspaceDialogHost implementation.</summary>
public partial class MainWindow
{
    private void ShowFindReplace_Click(object sender, RoutedEventArgs e) => OpenFindReplace();

    private void ShowTutorial_Click(object sender, RoutedEventArgs e)
        => _dialogs.ShowTutorial();

    private void ShowFontSettings_Click(object sender, RoutedEventArgs e)
    {
        var settings = _dialogs.ShowFontSettings(ViewModel.EditorFontFamily, ViewModel.EditorFontSize);
        if (settings is { } value)
            ViewModel.ApplyFontSettings(value.FontFamily, value.FontSize);
    }

    private void ShowError(string message, string title = "エラー") => _dialogs.ShowError(message, title);
    private void ShowInfo(string message, string title = "情報") => _dialogs.ShowInfo(message, title);

    private bool Confirm(string message, string title = "確認",
        MessageBoxImage icon = MessageBoxImage.Warning) => _dialogs.Confirm(message, title, icon);

    // ── IWorkspaceDialogHost (explicit — keeps WorkspaceView boundary clean) ──

    string? IWorkspaceDialogHost.ShowInput(string title, string prompt, string initialText)
        => _dialogs.ShowInput(title, prompt, initialText);

    bool IWorkspaceDialogHost.Confirm(string message, string title, MessageBoxImage icon)
        => _dialogs.Confirm(message, title, icon);

    void IWorkspaceDialogHost.ShowError(string message, string title)
        => _dialogs.ShowError(message, title);

    void IWorkspaceDialogHost.ShowInfo(string message, string title)
        => _dialogs.ShowInfo(message, title);

    NoteViewModel? IWorkspaceDialogHost.PickNote(IEnumerable<(string NotebookTitle, NoteViewModel Note)> notes)
        => _dialogs.PickNote(notes);

    void IWorkspaceDialogHost.ShowFindReplace(TextBox editor, string lastSearch, string lastReplace, double? left, double? top)
        => _dialogs.ShowFindReplace(editor, lastSearch, lastReplace, left, top);

    (string LastSearchText, string LastReplaceText, double? Left, double? Top)
        IWorkspaceDialogHost.GetFindReplaceState(string fallbackSearch, string fallbackReplace, double? fallbackLeft, double? fallbackTop)
        => _dialogs.GetFindReplaceState(fallbackSearch, fallbackReplace, fallbackLeft, fallbackTop);

    void IWorkspaceDialogHost.CloseFindReplace() => _dialogs.CloseFindReplace();
}
