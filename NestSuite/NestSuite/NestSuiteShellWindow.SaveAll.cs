using System.Windows.Input;
using NestSuite.ChatNest;
using NestSuite.IdeaNest.Services;
using NestSuite.IdeaNest.ViewModels;
using NestSuite.ViewModels;

namespace NestSuite;

public partial class NestSuiteShellWindow
{
    public static readonly RoutedCommand SaveAllCommand = new RoutedCommand(
        "SaveAll", typeof(NestSuiteShellWindow),
        new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });

    private enum SaveAllTabResult { Saved, Cancelled, Failed }

    private void CommandSaveAll_Executed(object sender, ExecutedRoutedEventArgs e) => SaveAllTabs();

    /// <summary>
    /// v2.10.4 SH-20: 未保存の全タブ（NoteNest/IdeaNest/ChatNest）を順に保存する。
    /// TempNest は CanClose=false のため対象外。SaveAs キャンセル・保存失敗時は中断する。
    /// </summary>
    private void SaveAllTabs()
    {
        var targets = _tabs.Where(t => t.IsModified && t.CanClose).ToList();
        if (targets.Count == 0)
        {
            ShowStatusNotification("  |  保存が必要なタブはありません");
            return;
        }
        int savedCount = 0;
        foreach (var tab in targets)
        {
            var result = TrySaveTabForSaveAll(tab);
            if (result == SaveAllTabResult.Cancelled || result == SaveAllTabResult.Failed)
            {
                ShowStatusNotification("  |  すべて保存を中断しました");
                return;
            }
            savedCount++;
        }
        ShowStatusNotification($"  |  {savedCount}件保存しました");
    }

    /// <summary>
    /// v2.10.4 SH-20: 指定タブを SaveAll 用に保存する。
    /// 既存パスがある場合は上書き保存、ない場合は SaveAs ダイアログを表示する。
    /// キャンセル・保存失敗時は Cancelled/Failed を返す。
    /// </summary>
    private SaveAllTabResult TrySaveTabForSaveAll(NestSuiteDocumentTab tab)
    {
        if (!_sessionManager.TryGet(tab.Id, out var session) || session == null)
            return SaveAllTabResult.Failed;

        return tab.WorkspaceKind switch
        {
            NestSuiteWorkspaceKind.NoteNest  => TrySaveNoteNestForSaveAll(tab, session),
            NestSuiteWorkspaceKind.IdeaNest  => TrySaveIdeaNestForSaveAll(tab, session),
            NestSuiteWorkspaceKind.ChatNest  => TrySaveChatNestForSaveAll(tab, session),
            _                                => SaveAllTabResult.Failed,
        };
    }

    private SaveAllTabResult TrySaveNoteNestForSaveAll(NestSuiteDocumentTab tab, NestSuiteWorkspaceSession session)
    {
        var vm = (MainViewModel)session.WorkspaceViewModel;
        if (tab.FilePath != null)
        {
            var path = NormalizeFilePath(tab.FilePath);
            if (!vm.SaveToPath(path)) return SaveAllTabResult.Failed;
            ApplySavedWorkspaceState(session, path, isModifiedAfterSave: false, showNotification: false);
            return SaveAllTabResult.Saved;
        }
        else
        {
            var rawPath = _dialogs.SelectProjectSavePath(vm.ProjectName);
            if (rawPath == null) return SaveAllTabResult.Cancelled;
            var normalizedPath = NormalizeFilePath(rawPath);
            if (CheckAndActivateDuplicateTabForSave(NestSuiteWorkspaceKind.NoteNest, normalizedPath, tab.Id))
                return SaveAllTabResult.Cancelled;
            if (!vm.SaveToPath(normalizedPath)) return SaveAllTabResult.Failed;
            ApplySavedWorkspaceState(session, normalizedPath, isModifiedAfterSave: false, showNotification: false);
            return SaveAllTabResult.Saved;
        }
    }

    private SaveAllTabResult TrySaveIdeaNestForSaveAll(NestSuiteDocumentTab tab, NestSuiteWorkspaceSession session)
    {
        var targetPath = tab.FilePath != null ? NormalizeFilePath(tab.FilePath) : null;
        if (targetPath == null)
        {
            var rawPath = _dialogs.SelectIdeaNestSavePath(DefaultIdeaNestFileName);
            if (rawPath == null) return SaveAllTabResult.Cancelled;
            targetPath = NormalizeFilePath(rawPath);
            if (CheckAndActivateDuplicateTabForSave(NestSuiteWorkspaceKind.IdeaNest, targetPath, tab.Id))
                return SaveAllTabResult.Cancelled;
        }
        var vm = (IdeaNestWorkspaceViewModel)session.WorkspaceViewModel;
        try
        {
            IdeaNestFileService.Save(targetPath, vm.BuildWorkspaceForSave());
            vm.MarkSaved();
            ApplySavedWorkspaceState(session, targetPath, isModifiedAfterSave: false, showNotification: false);
            return SaveAllTabResult.Saved;
        }
        catch (Exception ex)
        {
            LogAndShowSaveError("IdeaNestSave", "IdeaNest", "IdeaNest ファイルの保存に失敗しました。", ex, targetPath);
            return SaveAllTabResult.Failed;
        }
    }

    private SaveAllTabResult TrySaveChatNestForSaveAll(NestSuiteDocumentTab tab, NestSuiteWorkspaceSession session)
    {
        var targetPath = tab.FilePath != null ? NormalizeFilePath(tab.FilePath) : null;
        if (targetPath == null)
        {
            var rawPath = _dialogs.SelectChatNestSavePath(DefaultChatNestFileName);
            if (rawPath == null) return SaveAllTabResult.Cancelled;
            targetPath = NormalizeFilePath(rawPath);
            if (CheckAndActivateDuplicateTabForSave(NestSuiteWorkspaceKind.ChatNest, targetPath, tab.Id))
                return SaveAllTabResult.Cancelled;
        }
        var vm = (ChatNestWorkspaceViewModel)session.WorkspaceViewModel;
        try
        {
            ChatNestFileService.Save(targetPath, vm.MessageModels);
            vm.MarkSaved();
            ApplySavedWorkspaceState(session, targetPath, vm.HasUnsavedChanges, showNotification: false);
            return SaveAllTabResult.Saved;
        }
        catch (Exception ex)
        {
            LogAndShowSaveError("ChatNestSave", "ChatNest", "ChatNest ファイルの保存に失敗しました。", ex, targetPath);
            return SaveAllTabResult.Failed;
        }
    }
}
