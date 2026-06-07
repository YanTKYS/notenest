using NoteNest.Models;

namespace NoteNest.ViewModels;

public partial class MainViewModel
{
    private static int MarkerTypeOrder(string type) => type switch
    {
        "TODO"  => 0,
        "FIXME" => 1,
        "NOTE"  => 2,
        _       => 99,
    };

    private void RaiseFilteredMarkersChanged()
    {
        OnPropertyChanged(nameof(FilteredMarkers));
        OnPropertyChanged(nameof(FilteredMarkerCountText));
    }

    private void RefreshMarkers()
    {
        Markers.Clear();
        int todo = 0, fixme = 0, noteCount = 0;
        foreach (var n in AllNotes)
        {
            foreach (var m in _markerService.Extract(n.Content, n.Title))
            {
                Markers.Add(new MarkerViewModel(m, n));
                if (m.Type == "TODO")       todo++;
                else if (m.Type == "FIXME") fixme++;
                else if (m.Type == "NOTE")  noteCount++;
            }
        }
        _projectTodoCount  = todo;
        _projectFixmeCount = fixme;
        _projectNoteCount  = noteCount;
        OnPropertyChanged(nameof(MarkerCount));
        OnPropertyChanged(nameof(ProjectMarkerSummary));
        RaiseFilteredMarkersChanged();
    }
}
