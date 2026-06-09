using System.IO;
using System.Text.Json;

namespace NoteNest.Services;

public class RecentFilesService
{
    private static readonly string DefaultDataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                     "NoteNest", "recent-files.json");
    private const int MaxItems = 5;

    private readonly string _dataPath;

    public RecentFilesService(string? dataPath = null)
    {
        _dataPath = dataPath ?? DefaultDataPath;
    }

    public List<string> Load()
    {
        try
        {
            if (!File.Exists(_dataPath)) return [];
            return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_dataPath)) ?? [];
        }
        catch { return []; }
    }

    public IReadOnlyList<string> Add(string filePath)
    {
        var persisted = Load();
        var updated = persisted.ToList();
        updated.Remove(filePath);
        updated.Insert(0, filePath);
        if (updated.Count > MaxItems) updated = updated.Take(MaxItems).ToList();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
            File.WriteAllText(_dataPath, JsonSerializer.Serialize(updated));
            return updated;
        }
        catch
        {
            return persisted;
        }
    }

    public IReadOnlyList<string> ClearAndGetUpdatedList()
    {
        var persisted = Load();
        try
        {
            if (File.Exists(_dataPath)) File.Delete(_dataPath);
            return [];
        }
        catch
        {
            return persisted;
        }
    }

    public void Remove(string filePath)
    {
        var list = Load();
        if (!list.Remove(filePath)) return;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
            File.WriteAllText(_dataPath, JsonSerializer.Serialize(list));
        }
        catch { }
    }
}
