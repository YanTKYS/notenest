namespace NoteNest.Models;

public class NoteTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public bool IsCompleted { get; set; } = false;
}
