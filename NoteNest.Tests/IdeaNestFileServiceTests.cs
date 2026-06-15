using System.Text.Json.Serialization;
using System.Reflection;
using NoteNest.NestSuite.IdeaNest.Models;
using NoteNest.NestSuite.IdeaNest.Services;
using Xunit;

namespace NoteNest.Tests;

/// <summary>
/// v1.8.2: IdeaNestFileService 定数の確認および
/// IdeaNest モデルの [JsonPropertyName] 属性適用確認テスト。
/// JSON キー名が IdeaNest v1.1.4 の camelCase 形式と互換であることを保証する。
/// </summary>
public class IdeaNestFileServiceTests
{
    // ── IdeaNestFileService 定数 ─────────────────────────────────────────

    [Fact]
    public void FileExtension_IsExpected()
    {
        Assert.Equal(".ideanest", IdeaNestFileService.FileExtension);
    }

    [Fact]
    public void SchemaVersion_IsExpected()
    {
        Assert.Equal("1.1.4", IdeaNestFileService.SchemaVersion);
    }

    [Fact]
    public void NewWorkspace_UsesCurrentSchemaVersion()
    {
        // Workspace.Version default must stay in sync with IdeaNestFileService.SchemaVersion.
        // This catches the case where one is updated without the other.
        Assert.Equal(IdeaNestFileService.SchemaVersion, new Workspace().Version);
    }

    // ── Idea モデルの [JsonPropertyName] 属性確認 ───────────────────────

    [Theory]
    [InlineData("Id", "id")]
    [InlineData("Title", "title")]
    [InlineData("Body", "body")]
    [InlineData("Tags", "tags")]
    [InlineData("Color", "color")]
    [InlineData("IsPinned", "isPinned")]
    [InlineData("IsArchived", "isArchived")]
    [InlineData("CreatedAt", "createdAt")]
    [InlineData("UpdatedAt", "updatedAt")]
    public void Idea_Property_HasJsonPropertyNameAttribute(string propertyName, string expectedJsonName)
    {
        var prop = typeof(Idea).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(prop);
        var attr = prop!.GetCustomAttribute<JsonPropertyNameAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(expectedJsonName, attr!.Name);
    }

    // ── Workspace モデルの [JsonPropertyName] 属性確認 ───────────────────

    [Theory]
    [InlineData("Version", "version")]
    [InlineData("WorkspaceName", "workspaceName")]
    [InlineData("Ideas", "ideas")]
    [InlineData("Settings", "settings")]
    public void Workspace_Property_HasJsonPropertyNameAttribute(string propertyName, string expectedJsonName)
    {
        var prop = typeof(Workspace).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(prop);
        var attr = prop!.GetCustomAttribute<JsonPropertyNameAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(expectedJsonName, attr!.Name);
    }

    // ── WorkspaceSettings モデルの [JsonPropertyName] 属性確認 ───────────

    [Theory]
    [InlineData("SearchText", "searchText")]
    [InlineData("SelectedTag", "selectedTag")]
    [InlineData("SelectedColor", "selectedColor")]
    [InlineData("ShowArchived", "showArchived")]
    [InlineData("TagPanelOpen", "tagPanelOpen")]
    [InlineData("CardSize", "cardSize")]
    [InlineData("CardHeightMode", "cardHeightMode")]
    [InlineData("SortMode", "sortMode")]
    [InlineData("WindowWidth", "windowWidth")]
    [InlineData("WindowHeight", "windowHeight")]
    public void WorkspaceSettings_Property_HasJsonPropertyNameAttribute(string propertyName, string expectedJsonName)
    {
        var prop = typeof(WorkspaceSettings).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(prop);
        var attr = prop!.GetCustomAttribute<JsonPropertyNameAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(expectedJsonName, attr!.Name);
    }
}
