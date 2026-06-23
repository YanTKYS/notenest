using System.Text.Json;
using NestSuite.IdeaNest.Models;
using NestSuite.IdeaNest.Services;
using NestSuite.IdeaNest.ViewModels;
using Xunit;

namespace NestSuite.Tests;

/// <summary>
/// v2.7.12 ID-9: IdeaNest カードの hover/focus 視覚フィードバック強化の回帰確認。
/// 表示状態（hover/focus）は .ideanest ファイルに保存されないこと、
/// 既存のカード操作コマンドが維持されること、カードサイズ切替が引き続き機能することを検証する。
/// </summary>
public class IdeaNestHoverFocusTests
{
    // ── .ideanest 保存形式への混入なし ───────────────────────────────────────

    [Fact]
    public void SavedFile_DoesNotContainHoverOrFocusState()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ideanest");
        try
        {
            var workspace = new Workspace
            {
                Ideas = new()
                {
                    new Idea { Id = "card1", Title = "タイトル", Body = "本文", Tags = new() { "tagA" } },
                    new Idea { Id = "card2", Body = "2枚目" },
                }
            };
            IdeaNestFileService.Save(path, workspace);

            var json = File.ReadAllText(path).ToLowerInvariant();
            Assert.DoesNotContain("hover", json);
            Assert.DoesNotContain("focus", json);
            Assert.DoesNotContain("isselected", json);
            Assert.DoesNotContain("ismouseover", json);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void BuildWorkspaceForSave_DoesNotContainHoverOrFocusFields()
    {
        var vm = new IdeaNestWorkspaceViewModel();
        vm.LoadFromWorkspace(new Workspace
        {
            Ideas = new()
            {
                new Idea { Id = "a", Title = "A", Body = "本文A" },
                new Idea { Id = "b", Body = "本文B" },
            }
        });

        var saved = vm.BuildWorkspaceForSave();
        var json = JsonSerializer.Serialize(saved).ToLowerInvariant();

        Assert.DoesNotContain("hover", json);
        Assert.DoesNotContain("focus", json);
        Assert.DoesNotContain("isselected", json);
    }

    [Fact]
    public void RoundTrip_LoadAndSave_PreservesCardData()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ideanest");
        try
        {
            var workspace = new Workspace
            {
                Ideas = new()
                {
                    new Idea { Id = "x1", Title = "ホバーテスト", Body = "本文", Tags = new() { "t1", "t2" } },
                }
            };
            IdeaNestFileService.Save(path, workspace);
            var loaded = IdeaNestFileService.Load(path);

            Assert.Single(loaded.Ideas);
            Assert.Equal("x1", loaded.Ideas[0].Id);
            Assert.Equal("ホバーテスト", loaded.Ideas[0].Title);
            Assert.Equal(new[] { "t1", "t2" }, loaded.Ideas[0].Tags);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // ── カードサイズ切替の回帰 ────────────────────────────────────────────────

    [Theory]
    [InlineData("small",  184)]
    [InlineData("medium", 252)]
    [InlineData("large",  340)]
    public void CardSize_SwitchPreservesCardWidths(string size, double expectedWidth)
    {
        var vm = new CardDisplayViewModel(() => { }, () => { });
        vm.CardSize = size;
        Assert.Equal(expectedWidth, vm.CardWidth);
    }

    [Fact]
    public void CardSize_AfterSwitch_BodyPreviewMaxLinesIsCorrect()
    {
        var vm = new CardDisplayViewModel(() => { }, () => { });

        vm.CardSize = "small";
        Assert.Equal(3, vm.BodyPreviewMaxLines);

        vm.CardSize = "large";
        Assert.Equal(10, vm.BodyPreviewMaxLines);

        vm.CardSize = "medium";
        Assert.Equal(5, vm.BodyPreviewMaxLines);
    }

    // ── 既存コマンドの回帰 ────────────────────────────────────────────────────

    [Fact]
    public void WorkspaceViewModel_CardOperationCommandsExist()
    {
        var vm = new IdeaNestWorkspaceViewModel();

        Assert.NotNull(vm.AddIdeaCommand);
        Assert.NotNull(vm.DeleteIdeaCommand);
        Assert.NotNull(vm.TogglePinCommand);
        Assert.NotNull(vm.ToggleArchiveCommand);
        Assert.NotNull(vm.PreviewIdeaCommand);
        Assert.NotNull(vm.CopyCardMarkdownCommand);
    }
}
