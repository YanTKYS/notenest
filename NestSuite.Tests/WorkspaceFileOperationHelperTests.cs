using System.IO;
using System.Security;
using System.Text.Json;
using NestSuite.Services;
using Xunit;

namespace NestSuite.Tests;

/// <summary>
/// v2.7.6 TD-3-1: WorkspaceFileHelper 共通化後の回帰確認テスト。
/// Shell 上の private ヘルパーは WPF ウィンドウに依存するため直接テストしない。
/// FileErrorMessages と NestSuiteOpenFilePolicy の動作が共通化後も維持されることを確認する。
/// </summary>
public class WorkspaceFileOperationHelperTests
{
    // ── FileErrorMessages 回帰 (LogAndShowLoadError / LogAndShowSaveError 内で使用) ─

    [Fact]
    public void ForLoad_FileNotFoundException_ReturnsFileNotFoundMessage()
    {
        Assert.Contains("見つかりません", FileErrorMessages.ForLoad(new FileNotFoundException()));
    }

    [Fact]
    public void ForLoad_DirectoryNotFoundException_ReturnsFileNotFoundMessage()
    {
        Assert.Contains("見つかりません", FileErrorMessages.ForLoad(new DirectoryNotFoundException()));
    }

    [Fact]
    public void ForLoad_JsonException_ReturnsFormatMessage()
    {
        Assert.Contains("形式", FileErrorMessages.ForLoad(new JsonException()));
    }

    [Fact]
    public void ForLoad_IOException_ReturnsIoMessage()
    {
        Assert.Contains("入出力エラー", FileErrorMessages.ForLoad(new IOException()));
    }

    [Fact]
    public void ForLoad_UnauthorizedAccessException_ReturnsAccessMessage()
    {
        Assert.Contains("権限", FileErrorMessages.ForLoad(new UnauthorizedAccessException()));
    }

    [Fact]
    public void ForLoad_SecurityException_ReturnsAccessMessage()
    {
        Assert.Contains("権限", FileErrorMessages.ForLoad(new SecurityException()));
    }

    [Fact]
    public void ForLoad_PathTooLongException_ReturnsPathMessage()
    {
        Assert.Contains("パス", FileErrorMessages.ForLoad(new PathTooLongException()));
    }

    [Fact]
    public void ForLoad_UnknownException_ReturnsFallbackAndNotEmpty()
    {
        Assert.NotEmpty(FileErrorMessages.ForLoad(new InvalidOperationException("unknown")));
    }

    [Fact]
    public void ForSave_IOException_ReturnsIoMessage()
    {
        Assert.Contains("入出力エラー", FileErrorMessages.ForSave(new IOException()));
    }

    [Fact]
    public void ForSave_UnauthorizedAccessException_ReturnsAccessMessage()
    {
        Assert.Contains("権限", FileErrorMessages.ForSave(new UnauthorizedAccessException()));
    }

    [Fact]
    public void ForSave_JsonException_ReturnsWriteErrorMessage()
    {
        Assert.Contains("書き込み", FileErrorMessages.ForSave(new JsonException()));
    }

    [Fact]
    public void ForSave_UnknownException_ReturnsFallbackAndNotEmpty()
    {
        Assert.NotEmpty(FileErrorMessages.ForSave(new InvalidOperationException("unknown")));
    }

    // ── NestSuiteOpenFilePolicy 回帰 (CheckAndActivateDuplicateTabForSave 内で使用) ─

    [Fact]
    public void IsSameFile_CaseInsensitive_ReturnsTrue()
    {
        Assert.True(NestSuiteOpenFilePolicy.IsSameFile(
            @"C:\Projects\notes.notenest",
            @"C:\projects\NOTES.NOTENEST"));
    }

    [Fact]
    public void IsSameFile_NullLeft_ReturnsFalse()
    {
        Assert.False(NestSuiteOpenFilePolicy.IsSameFile(null, @"C:\file.notenest"));
    }

    [Fact]
    public void IsSameFile_NullRight_ReturnsFalse()
    {
        Assert.False(NestSuiteOpenFilePolicy.IsSameFile(@"C:\file.notenest", null));
    }

    [Fact]
    public void IsSameFile_BothNull_ReturnsFalse()
    {
        Assert.False(NestSuiteOpenFilePolicy.IsSameFile(null, null));
    }

    [Fact]
    public void IsSameFile_DifferentFiles_ReturnsFalse()
    {
        Assert.False(NestSuiteOpenFilePolicy.IsSameFile(
            @"C:\Projects\notes.notenest",
            @"C:\Projects\other.notenest"));
    }

    [Fact]
    public void IsSameFile_IdenticalPaths_ReturnsTrue()
    {
        Assert.True(NestSuiteOpenFilePolicy.IsSameFile(
            @"C:\Projects\notes.notenest",
            @"C:\Projects\notes.notenest"));
    }
}
