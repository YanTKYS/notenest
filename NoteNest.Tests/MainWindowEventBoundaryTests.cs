using System.Reflection;
using NoteNest.Views;
using Xunit;

namespace NoteNest.Tests;

/// <summary>
/// v1.5.5: 共有ヘルパーの検索先を MainWindow → NoteNestWorkspaceView へ更新。
/// DragDrop・ContextMenu ヘルパーは WorkspaceView のコードビハインドに移動済み。
/// </summary>
public class MainWindowEventBoundaryTests
{
    private static readonly BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly BindingFlags PrivateStatic = BindingFlags.Static | BindingFlags.NonPublic;

    [Theory]
    [InlineData("Window_PreviewKeyDown")]
    [InlineData("Window_Closing")]
    [InlineData("Export_Click")]
    [InlineData("ClearRecentFiles_Click")]
    [InlineData("ShowFindReplace_Click")]
    public void SemanticEventEntryPointsRemainAvailable(string methodName)
    {
        Assert.NotNull(typeof(NoteNest.MainWindow).GetMethod(methodName, PrivateInstance));
    }

    [Fact]
    public void ContextMenuResolutionUsesExplicitlyNamedSharedHelper()
    {
        // v1.5.5: helper moved to NoteNestWorkspaceView
        Assert.NotNull(typeof(NoteNestWorkspaceView).GetMethod("GetContextMenuDataContext", PrivateStatic));
        Assert.Null(typeof(NoteNestWorkspaceView).GetMethod("GetDataContext", PrivateStatic));
    }

    [Fact]
    public void DragDropUsesSharedThresholdAndEffectHelpers()
    {
        // v1.5.5: helpers moved to NoteNestWorkspaceView
        Assert.NotNull(typeof(NoteNestWorkspaceView).GetMethod("HasExceededDragThreshold", PrivateStatic));
        Assert.NotNull(typeof(NoteNestWorkspaceView).GetMethod("SetDragOverEffect", PrivateStatic));
    }
}
