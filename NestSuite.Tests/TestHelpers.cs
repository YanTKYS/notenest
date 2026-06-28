using NestSuite.Models;
using NestSuite.ViewModels;
using Xunit;

namespace NestSuite.Tests;

internal static class TestPaths
{
    internal static readonly string RepoRoot =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    internal static string ReadBacklog()
    {
        var path = Path.Combine(RepoRoot, "docs", "backlog.md");
        Assert.True(File.Exists(path), $"backlog.md not found: {path}");
        return File.ReadAllText(path);
    }

    internal static string ReadReleaseNotes()
    {
        var path = Path.Combine(RepoRoot, "docs", "release-notes.md");
        Assert.True(File.Exists(path), $"release-notes.md not found: {path}");
        return File.ReadAllText(path);
    }
}

internal static class TestFactories
{
    internal static NoteViewModel MakeNote(string title, string content = "") =>
        new(new Note { Title = title, Content = content });
}
