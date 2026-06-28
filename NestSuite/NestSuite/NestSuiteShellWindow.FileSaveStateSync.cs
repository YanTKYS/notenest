using NestSuite.ChatNest;

namespace NestSuite;

public partial class NestSuiteShellWindow
{
    /// <summary>v2.7.15: 保存成功後の IdeaNest タブ・Session 更新を共通経路へ委譲する。</summary>
    private void UpdateIdeaNestTabPath(NestSuiteWorkspaceSession session, string path) =>
        ApplySavedWorkspaceState(session, path, isModifiedAfterSave: false);

    /// <summary>
    /// v1.9.2: 指定 Session に対応する ChatNest タブのファイルパスを更新し、タブモデルを最新化する。
    /// 保存成功時に <see cref="ChatNestWorkspaceViewModel.MarkSaved"/> の後で呼ぶ。
    ///
    /// <para>案A: IsModified は MarkSaved() 後の HasUnsavedChanges を引き継ぐ。
    /// IsDirty は解消されるが InputText が残っている場合は HasUnsavedChanges が true のままになるため、
    /// IsModified = false を固定せず vm.HasUnsavedChanges を参照する。</para>
    /// </summary>
    private void UpdateChatNestTabPath(NestSuiteWorkspaceSession session, string path)
    {
        var vm = (ChatNestWorkspaceViewModel)session.WorkspaceViewModel;
        ApplySavedWorkspaceState(session, path, vm.HasUnsavedChanges);
    }

    /// <summary>v2.7.15: 保存成功後の NoteNest タブ・Session 更新を共通経路へ委譲する。</summary>
    private void UpdateNoteNestTabPath(NestSuiteWorkspaceSession session, string path) =>
        ApplySavedWorkspaceState(session, path, isModifiedAfterSave: false);
}
