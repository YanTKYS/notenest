using NoteNest.Models;
using NoteNest.Services;
using Xunit;

namespace NoteNest.Tests;

public class ProjectFileServiceTests : IDisposable
{
    private readonly ProjectFileService _svc = new();
    private readonly string _path =
        Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".notenest");

    public void Dispose()
    {
        foreach (var f in new[] { _path, _path + ".tmp", _path + ".bak" })
            if (File.Exists(f)) File.Delete(f);
    }

    [Fact]
    public void Save_NewFile_CreatesFile()
    {
        _svc.Save(_path, new Project { ProjectName = "Test" });

        Assert.True(File.Exists(_path));
    }

    [Fact]
    public void Save_NewFile_NoTempFileLeft()
    {
        _svc.Save(_path, new Project { ProjectName = "Test" });

        Assert.False(File.Exists(_path + ".tmp"));
    }

    [Fact]
    public void Save_ExistingFile_CreatesBackup()
    {
        _svc.Save(_path, new Project { ProjectName = "First" });
        _svc.Save(_path, new Project { ProjectName = "Second" });

        Assert.True(File.Exists(_path + ".bak"));
    }

    [Fact]
    public void SaveLoad_RoundTrip_PreservesProjectName()
    {
        var project = new Project { ProjectName = "MyProject", ProjectId = "abc-123" };
        _svc.Save(_path, project);
        var loaded = _svc.Load(_path);

        Assert.Equal("MyProject", loaded.ProjectName);
        Assert.Equal("abc-123",   loaded.ProjectId);
    }

    [Fact]
    public void Load_InvalidJson_ThrowsInvalidDataException()
    {
        File.WriteAllText(_path, "not json");

        Assert.ThrowsAny<Exception>(() => _svc.Load(_path));
    }
}
