using System.Text;
using System.Windows;
using NestSuite.Services;
using NestSuite.ViewModels;

namespace NestSuite;

public partial class NestSuiteShellWindow
{
    private void MenuExportNoteMarkdownCopy_Click(object sender, RoutedEventArgs e)
        => ExportCurrentNoteMarkdownToClipboard();

    private void MenuExportNoteMarkdownSave_Click(object sender, RoutedEventArgs e)
        => ExportCurrentNoteMarkdownToFile();

    private void MenuExportAllNotesMarkdownSave_Click(object sender, RoutedEventArgs e)
        => ExportAllNotesMarkdownToFile();

    /// <summary>v2.10.5 M10: 選択中ノートを Markdown としてクリップボードにコピーする。</summary>
    private void ExportCurrentNoteMarkdownToClipboard()
    {
        var (_, note) = GetSelectedNoteNestNote();
        if (note == null)
        {
            ShowStatusNotification("  |  ノートが選択されていません");
            return;
        }
        var markdown = NoteNestMarkdownExportService.BuildCurrentNoteMarkdown(note);
        try
        {
            Clipboard.SetText(markdown);
            ShowStatusNotification("  |  Markdown をコピーしました");
        }
        catch (Exception ex)
        {
            ErrorLogService.Log("MarkdownCopyToClipboard", ex, "NoteNest");
            _dialogs.ShowError("クリップボードへのコピーに失敗しました。", "コピーエラー");
        }
    }

    /// <summary>v2.10.5 M10: 選択中ノートを Markdown ファイルとして保存する。</summary>
    private void ExportCurrentNoteMarkdownToFile()
    {
        var (_, note) = GetSelectedNoteNestNote();
        if (note == null)
        {
            ShowStatusNotification("  |  ノートが選択されていません");
            return;
        }
        var defaultName = BuildMarkdownDefaultFileName(note.Title);
        var path = _dialogs.SelectMarkdownExportPath(defaultName);
        if (path == null) return;
        var markdown = NoteNestMarkdownExportService.BuildCurrentNoteMarkdown(note);
        SaveMarkdownFile(path, markdown);
    }

    /// <summary>v2.10.5 M10: 全ノートを 1 つの Markdown ファイルとして保存する。</summary>
    private void ExportAllNotesMarkdownToFile()
    {
        if (_selectedTab?.WorkspaceKind != NestSuiteWorkspaceKind.NoteNest) return;
        if (!_sessionManager.TryGet(_selectedTab.Id, out var session) || session == null) return;
        var vm = (MainViewModel)session.WorkspaceViewModel;
        var defaultName = BuildMarkdownDefaultFileName(vm.ProjectName);
        var path = _dialogs.SelectMarkdownExportPath(defaultName);
        if (path == null) return;
        var markdown = NoteNestMarkdownExportService.BuildAllNotesMarkdown(vm.ProjectName, vm.AllNotes);
        SaveMarkdownFile(path, markdown);
    }

    private (MainViewModel? vm, NoteViewModel? note) GetSelectedNoteNestNote()
    {
        if (_selectedTab?.WorkspaceKind != NestSuiteWorkspaceKind.NoteNest) return (null, null);
        if (!_sessionManager.TryGet(_selectedTab.Id, out var session) || session == null) return (null, null);
        var vm = (MainViewModel)session.WorkspaceViewModel;
        return (vm, vm.SelectedNote);
    }

    private void SaveMarkdownFile(string path, string content)
    {
        try
        {
            AtomicFileWriter.WriteAllText(path, content, Encoding.UTF8);
            ShowStatusNotification("  |  Markdown を保存しました");
        }
        catch (Exception ex)
        {
            LogAndShowSaveError("MarkdownExport", "NoteNest", "Markdown ファイルの保存に失敗しました。", ex, path);
        }
    }

    private static string BuildMarkdownDefaultFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "note.md";
        var safe = ExportService.SanitizeFileName(name);
        return string.IsNullOrWhiteSpace(safe) ? "note.md" : safe + ".md";
    }
}
