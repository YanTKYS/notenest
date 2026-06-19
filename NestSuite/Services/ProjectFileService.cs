using System.IO;
using System.Text;
using System.Text.Json;
using NestSuite.Models;

namespace NestSuite.Services;

public class ProjectFileService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public Project Load(string path)
    {
        var json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<Project>(json, Options)
            ?? throw new InvalidDataException("プロジェクトデータが無効です。");
    }

    public void Save(string path, Project project)
    {
        var json = JsonSerializer.Serialize(project, Options);
        var tempPath   = path + ".tmp";
        var backupPath = path + ".bak";
        File.WriteAllText(tempPath, json, Encoding.UTF8);
        if (File.Exists(path))
            File.Replace(tempPath, path, backupPath);
        else
            File.Move(tempPath, path);
    }
}
