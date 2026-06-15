namespace NoteNest.NestSuite.IdeaNest.Models;

public class WorkspaceSettings
{
    public string SearchText { get; set; } = string.Empty;
    public string SelectedTag { get; set; } = string.Empty;
    public string SelectedColor { get; set; } = string.Empty;
    public bool ShowArchived { get; set; }
    public bool TagPanelOpen { get; set; }
    public string CardSize { get; set; } = "medium";
    public string CardHeightMode { get; set; } = "fixed";
    public string SortMode { get; set; } = "UpdatedDesc";
    public double WindowWidth { get; set; } = 1100;
    public double WindowHeight { get; set; } = 720;
}
