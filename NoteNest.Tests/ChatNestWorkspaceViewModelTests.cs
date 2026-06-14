using System.Collections.Generic;
using NoteNest.NestSuite.ChatNest;
using Xunit;

namespace NoteNest.Tests;

/// <summary>
/// v1.7.0: ChatNest 統合検証用 Workspace ViewModel の最小動作確認。
/// 投稿・発言者切替・クリア・読込など、UI（MessageBox）に依存しない経路のみを検証する。
/// 発言削除は MessageBox を伴うため本テストの対象外（NestSuite 配下の暫定許容）。
/// </summary>
public class ChatNestWorkspaceViewModelTests
{
    [Fact]
    public void Post_WithText_AddsMessageWithSelectedSpeaker()
    {
        var vm = new ChatNestWorkspaceViewModel
        {
            SelectedSpeaker = Speaker.反論,
            InputText = "それは違うと思う",
        };

        vm.PostCommand.Execute(null);

        var message = Assert.Single(vm.Messages);
        Assert.Equal(Speaker.反論, message.Speaker);
        Assert.Equal("それは違うと思う", message.Text);
        Assert.Equal(string.Empty, vm.InputText);
        Assert.True(vm.IsDirty);
    }

    [Fact]
    public void Post_WithWhitespaceOnly_DoesNotAddMessage()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "   " };

        Assert.False(vm.PostCommand.CanExecute(null));
        vm.PostCommand.Execute(null);

        Assert.Empty(vm.Messages);
    }

    [Fact]
    public void Post_TrimsSurroundingWhitespace()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "  まとめ  " };

        vm.PostCommand.Execute(null);

        Assert.Equal("まとめ", Assert.Single(vm.Messages).Text);
    }

    [Fact]
    public void CycleSpeaker_Forward_AdvancesThroughAllSpeakersAndWraps()
    {
        var vm = new ChatNestWorkspaceViewModel { SelectedSpeaker = Speaker.自分 };

        vm.CycleSpeaker(forward: true);
        Assert.Equal(Speaker.反論, vm.SelectedSpeaker);

        vm.CycleSpeaker(forward: true);
        Assert.Equal(Speaker.補足, vm.SelectedSpeaker);

        vm.CycleSpeaker(forward: true);
        Assert.Equal(Speaker.結論, vm.SelectedSpeaker);

        // 末尾から先頭へ循環する
        vm.CycleSpeaker(forward: true);
        Assert.Equal(Speaker.自分, vm.SelectedSpeaker);
    }

    [Fact]
    public void CycleSpeaker_Backward_WrapsToLast()
    {
        var vm = new ChatNestWorkspaceViewModel { SelectedSpeaker = Speaker.自分 };

        vm.CycleSpeaker(forward: false);

        Assert.Equal(Speaker.結論, vm.SelectedSpeaker);
    }

    [Fact]
    public void WorkspaceModified_RaisedOnPost()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "発火確認" };
        bool raised = false;
        vm.WorkspaceModified += (_, _) => raised = true;

        vm.PostCommand.Execute(null);

        Assert.True(raised);
    }

    [Fact]
    public void Clear_RemovesMessagesAndResetsState()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "残す？" };
        vm.PostCommand.Execute(null);

        vm.Clear();

        Assert.Empty(vm.Messages);
        Assert.Equal(string.Empty, vm.InputText);
        Assert.False(vm.IsDirty);
    }

    [Fact]
    public void HasUnsavedChanges_FreshViewModel_IsFalse()
    {
        var vm = new ChatNestWorkspaceViewModel();

        Assert.False(vm.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_AfterPost_IsTrue()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "投稿する" };

        vm.PostCommand.Execute(null);

        // 投稿後は入力欄が空でも IsDirty により未保存扱いとなる
        Assert.Equal(string.Empty, vm.InputText);
        Assert.True(vm.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_WithUnpostedInputOnly_IsTrue()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "下書き中" };

        // 投稿前でも入力欄に内容があれば未保存扱いとする
        Assert.False(vm.IsDirty);
        Assert.True(vm.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_WithWhitespaceInputOnly_IsFalse()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "   " };

        Assert.False(vm.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_RaisesPropertyChanged_WhenInputTextChanges()
    {
        var vm = new ChatNestWorkspaceViewModel();
        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.InputText = "入力";

        Assert.Contains(nameof(ChatNestWorkspaceViewModel.HasUnsavedChanges), changed);
    }

    [Fact]
    public void Clear_ResetsHasUnsavedChanges()
    {
        var vm = new ChatNestWorkspaceViewModel { InputText = "残す？" };
        vm.PostCommand.Execute(null);
        Assert.True(vm.HasUnsavedChanges);

        vm.Clear();

        Assert.False(vm.HasUnsavedChanges);
    }

    [Fact]
    public void LoadMessages_ReplacesContentsAndMarksClean()
    {
        var vm = new ChatNestWorkspaceViewModel();
        var loaded = new[]
        {
            new Message { Speaker = Speaker.自分, Text = "A" },
            new Message { Speaker = Speaker.結論, Text = "B" },
        };

        vm.LoadMessages(loaded);

        Assert.Equal(2, vm.Messages.Count);
        Assert.Equal("A", vm.Messages[0].Text);
        Assert.Equal("B", vm.Messages[1].Text);
        Assert.False(vm.IsDirty);
    }
}
