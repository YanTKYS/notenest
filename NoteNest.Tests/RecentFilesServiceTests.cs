using NoteNest.Services;
using Xunit;

namespace NoteNest.Tests;

// Integration tests: write to %AppData%\NoteNest\recent-files.json.
// Tests are additive — they do not reset the file beforehand.
public class RecentFilesServiceTests
{
    private readonly RecentFilesService _svc = new();

    [Fact]
    public void Add_DuplicatePath_MovedToFront()
    {
        _svc.Add("test/path/a");
        _svc.Add("test/path/b");
        _svc.Add("test/path/a");

        var list = _svc.Load();
        Assert.Equal("test/path/a", list[0]);
    }

    [Fact]
    public void Add_ExceedsMaxFive_ListTrimmed()
    {
        for (int i = 0; i < 7; i++)
            _svc.Add($"test/path/{i}");

        var list = _svc.Load();
        Assert.True(list.Count <= 5);
    }

    [Fact]
    public void Add_NewPath_AppearsAtFront()
    {
        _svc.Add("test/existing");
        _svc.Add("test/newest");

        var list = _svc.Load();
        Assert.Equal("test/newest", list[0]);
    }
}
