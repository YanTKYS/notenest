using System.ComponentModel;
using NoteNest.ViewModels;

namespace NoteNest.Services;

/// <summary>責務別 ViewModel 間の変更伝播を集約し、MainViewModel へ意味的な変更通知を提供します。</summary>
public sealed class WorkspaceChangeCoordinator
{
    private readonly NoteWorkspaceViewModel _notes;
    private readonly TaskBoardViewModel _tasks;
    private readonly MarkerPanelViewModel _markers;
    private readonly EditorStateViewModel _editor;

    public WorkspaceChangeCoordinator(
        NoteWorkspaceViewModel notes,
        TaskBoardViewModel tasks,
        MarkerPanelViewModel markers,
        EditorStateViewModel editor)
    {
        _notes = notes;
        _tasks = tasks;
        _markers = markers;
        _editor = editor;

        _notes.Changed += NotesChanged;
        _tasks.Changed += TasksChanged;
        _editor.ContentEdited += EditorContentEdited;
        _editor.RelatedNoteChanged += EditorRelatedNoteChanged;
        _editor.SettingsChanged += EditorSettingsChanged;
        _editor.PropertyChanged += EditorPropertyChanged;
        _markers.PropertyChanged += MarkerPropertyChanged;
    }

    public event EventHandler<WorkspaceChangeEventArgs>? Changed;

    private void NotesChanged(object? sender, EventArgs e)
    {
        _markers.Refresh(_notes.AllNotes);
        Publish(true, nameof(MainViewModel.RelatedNoteChoices), nameof(MainViewModel.CurrentNoteTitle), nameof(MainViewModel.EditorTitle));
    }

    private void TasksChanged(object? sender, EventArgs e) =>
        Publish(true, nameof(MainViewModel.EditorTitle));

    private void EditorContentEdited(object? sender, EventArgs e)
    {
        if (_editor.IsTaskCommentMode && _editor.EditingTask != null)
            _tasks.UpdateComment(_editor.EditingTask, _editor.Content);
        else if (_editor.IsNoteEditMode && _editor.SelectedNote != null)
            _notes.UpdateContent(_editor.SelectedNote, _editor.Content);
    }

    private void EditorRelatedNoteChanged(object? sender, EventArgs e)
    {
        if (_editor.EditingTask != null)
            _tasks.SetRelatedNote(_editor.EditingTask, _editor.EditingTaskRelatedNote);
    }

    private void EditorSettingsChanged(object? sender, EventArgs e) => Publish(true);

    private void EditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var facadeProperty = e.PropertyName switch
        {
            nameof(EditorStateViewModel.Content) => nameof(MainViewModel.EditorContent),
            nameof(EditorStateViewModel.FontFamily) => nameof(MainViewModel.EditorFontFamily),
            nameof(EditorStateViewModel.FontSize) => nameof(MainViewModel.EditorFontSize),
            nameof(EditorStateViewModel.CaretPositionText) => nameof(MainViewModel.CaretPositionText),
            nameof(EditorStateViewModel.ShowLineNumbers) => nameof(MainViewModel.ShowLineNumbers),
            _ => e.PropertyName,
        };
        Publish(false, facadeProperty);
    }

    private void MarkerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var facadeProperty = e.PropertyName == nameof(MarkerPanelViewModel.SortOrderIndex)
            ? nameof(MainViewModel.MarkerSortOrderIndex)
            : e.PropertyName;
        Publish(false, facadeProperty);
    }

    private void Publish(bool isDataChanged, params string?[] propertyNames) =>
        Changed?.Invoke(this, new WorkspaceChangeEventArgs(
            isDataChanged,
            propertyNames.OfType<string>().Where(name => !string.IsNullOrWhiteSpace(name)).Distinct().ToArray()));
}

public sealed record WorkspaceChangeEventArgs(bool IsDataChanged, IReadOnlyList<string> PropertyNames);
