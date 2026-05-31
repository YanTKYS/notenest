using NoteNest.Services;

namespace NoteNest.ViewModels;

public class MarkerViewModel : BaseViewModel
{
    public MarkerViewModel(MarkerInfo info)
    {
        Type = info.Type;
        LineNumber = info.LineNumber;
        NoteTitle = info.NoteTitle;
        Excerpt = info.Excerpt;
    }

    public string Type { get; }
    public int LineNumber { get; }
    public string NoteTitle { get; }
    public string Excerpt { get; }

    public string LineLabel => $"L{LineNumber}";
}
