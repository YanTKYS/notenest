using System.IO;

namespace NestSuite.ViewModels;

public sealed class RecentFileViewModel
{
    public RecentFileViewModel(string path) => FullPath = path;
    public string FullPath { get; }
    public string FileName => Path.GetFileName(FullPath);
    public string DisplayName => Path.GetFileNameWithoutExtension(FullPath);
}
