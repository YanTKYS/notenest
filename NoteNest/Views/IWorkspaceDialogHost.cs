using System.Windows;
using System.Windows.Controls;
using NoteNest.ViewModels;

namespace NoteNest.Views;

public interface IWorkspaceDialogHost
{
    string? ShowInput(string title, string prompt, string initialText = "");
    bool Confirm(string message, string title = "確認", MessageBoxImage icon = MessageBoxImage.Warning);
    void ShowError(string message, string title = "エラー");
    void ShowInfo(string message, string title = "情報");
    NoteViewModel? PickNote(IEnumerable<(string NotebookTitle, NoteViewModel Note)> notes);
    void ShowFindReplace(TextBox editor, string lastSearch, string lastReplace, double? left, double? top);
    (string LastSearchText, string LastReplaceText, double? Left, double? Top) GetFindReplaceState(
        string fallbackSearch, string fallbackReplace, double? fallbackLeft, double? fallbackTop);
    void CloseFindReplace();
}
