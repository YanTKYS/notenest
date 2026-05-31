namespace NoteNest.Models;

public class Project
{
    public string Version { get; set; } = "0.1.0";
    public string ProjectId { get; set; } = Guid.NewGuid().ToString();
    public string ProjectName { get; set; } = "新しいプロジェクト";
    public List<Notebook> Notebooks { get; set; } = new();
    public TaskCollection Tasks { get; set; } = new();
    public AppSettings Settings { get; set; } = new();
}
