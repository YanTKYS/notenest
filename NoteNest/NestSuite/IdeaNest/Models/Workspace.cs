using System.Collections.Generic;

namespace NoteNest.NestSuite.IdeaNest.Models;

public class Workspace
{
    public string Version { get; set; } = "1.1.4";
    public string WorkspaceName { get; set; } = "無題のワークスペース";
    public List<Idea> Ideas { get; set; } = new();
    public WorkspaceSettings Settings { get; set; } = new();
}
