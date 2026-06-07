using NoteNest.Models;

namespace NoteNest.ViewModels;

public partial class MainViewModel
{
    public void ApplyFontSettings(string fontFamily, double fontSize)
    {
        EditorFontFamily = fontFamily;
        EditorFontSize   = fontSize;
        IsModified       = true;
    }

    private void ClearEditor()
    {
        _editorMode = EditorMode.NoteEdit;
        _editingTask = null;
        _editingTaskRelatedNote = null;
        SelectedNote = null;
        _isLoadingNote = true;
        _editorContent = "";
        OnPropertyChanged(nameof(EditorContent));
        OnPropertyChanged(nameof(CurrentNoteTitle));
        OnPropertyChanged(nameof(EditorTitle));
        OnPropertyChanged(nameof(IsTaskCommentMode));
        OnPropertyChanged(nameof(IsNoteEditMode));
        OnPropertyChanged(nameof(EditingTaskRelatedNote));
        OnPropertyChanged(nameof(HasEditingTaskRelatedNote));
        _isLoadingNote = false;
        CaretPositionText = "";
        Markers.Clear();
        _projectTodoCount  = 0;
        _projectFixmeCount = 0;
        _projectNoteCount  = 0;
        OnPropertyChanged(nameof(MarkerCount));
        OnPropertyChanged(nameof(ProjectMarkerSummary));
        RaiseFilteredMarkersChanged();
    }
}
