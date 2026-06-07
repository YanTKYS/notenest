using NoteNest.Models;

namespace NoteNest.ViewModels;

public partial class MainViewModel
{
    public void SelectNote(NoteViewModel note)
    {
        _editorMode = EditorMode.NoteEdit;
        _editingTask = null;
        _isLoadingNote = true;
        SelectedNote = note;
        _editorContent = note.Content;
        OnPropertyChanged(nameof(EditorContent));
        OnPropertyChanged(nameof(CurrentNoteTitle));
        OnPropertyChanged(nameof(EditorTitle));
        OnPropertyChanged(nameof(IsTaskCommentMode));
        OnPropertyChanged(nameof(IsNoteEditMode));
        _isLoadingNote = false;
        RefreshMarkers();
    }

    public void AddNotebookWithTitle(string title)
    {
        var model = new Notebook { Title = title };
        var vm = new NotebookViewModel(model);
        Notebooks.Add(vm);
        IsModified = true;
        StatusMessage = $"ノートブック「{title}」を追加しました。";
    }

    public void RenameNotebook(NotebookViewModel nb, string newTitle)
    {
        nb.Title = newTitle;
        IsModified = true;
    }

    public void DeleteNotebook(NotebookViewModel nb)
    {
        if (SelectedNote != null && nb.Notes.Contains(SelectedNote))
        {
            SelectedNote = null;
            ClearEditor();
        }
        ClearTaskLinksToNoteIds(nb.Notes.Select(n => n.Id));
        Notebooks.Remove(nb);
        IsModified = true;
        RefreshMarkers();
    }

    public bool AddNoteToNotebook(NotebookViewModel notebook, string title)
    {
        if (NoteNameExists(title)) return false;
        var model = new Note { Title = title };
        var vm = new NoteViewModel(model);
        notebook.Notes.Add(vm);
        notebook.Model.Notes.Add(model);
        IsModified = true;
        OnPropertyChanged(nameof(RelatedNoteChoices));
        SelectNote(vm);
        StatusMessage = $"ノート「{title}」を追加しました。";
        return true;
    }

    public bool RenameNote(NoteViewModel note, string newTitle)
    {
        if (NoteNameExists(newTitle, excludeSelf: note)) return false;
        note.Title = newTitle;
        if (SelectedNote == note)
        {
            OnPropertyChanged(nameof(CurrentNoteTitle));
            OnPropertyChanged(nameof(EditorTitle));
            RefreshMarkers();
        }
        IsModified = true;
        return true;
    }

    public void DeleteNote(NoteViewModel note)
    {
        var nb = FindNotebookOf(note);
        if (nb == null) return;
        nb.Notes.Remove(note);
        nb.Model.Notes.Remove(note.Model);
        ClearTaskLinksToNoteIds(new[] { note.Id });
        if (SelectedNote == note) ClearEditor();
        IsModified = true;
        OnPropertyChanged(nameof(RelatedNoteChoices));
        RefreshMarkers();
    }

    public void MoveNoteUp(NoteViewModel note)
    {
        var nb = FindNotebookOf(note);
        if (nb == null) return;
        var idx = nb.Notes.IndexOf(note);
        if (idx > 0) { nb.Notes.Move(idx, idx - 1); IsModified = true; }
    }

    public void MoveNoteDown(NoteViewModel note)
    {
        var nb = FindNotebookOf(note);
        if (nb == null) return;
        var idx = nb.Notes.IndexOf(note);
        if (idx >= 0 && idx < nb.Notes.Count - 1) { nb.Notes.Move(idx, idx + 1); IsModified = true; }
    }

    public void MoveNotebookUp(NotebookViewModel nb)
    {
        var idx = Notebooks.IndexOf(nb);
        if (idx > 0) { Notebooks.Move(idx, idx - 1); IsModified = true; }
    }

    public void MoveNotebookDown(NotebookViewModel nb)
    {
        var idx = Notebooks.IndexOf(nb);
        if (idx >= 0 && idx < Notebooks.Count - 1) { Notebooks.Move(idx, idx + 1); IsModified = true; }
    }

    public void MoveNoteToNotebook(NoteViewModel note, NotebookViewModel targetNotebook)
    {
        var sourceNotebook = FindNotebookOf(note);
        if (sourceNotebook == null || sourceNotebook == targetNotebook) return;
        sourceNotebook.Notes.Remove(note);
        sourceNotebook.Model.Notes.Remove(note.Model);
        targetNotebook.Notes.Add(note);
        targetNotebook.Model.Notes.Add(note.Model);
        IsModified = true;
        StatusMessage = $"ノート「{note.Title}」を「{targetNotebook.Title}」に移動しました。";
    }

    public NoteViewModel? FindNoteById(string? id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return AllNotes.FirstOrDefault(n => n.Id == id);
    }

    public NoteViewModel? FindNoteByTitle(string title) =>
        AllNotes.FirstOrDefault(n =>
            string.Equals(n.Title, title, StringComparison.OrdinalIgnoreCase));

    public bool NoteNameExists(string title, NoteViewModel? excludeSelf = null) =>
        AllNotes.Any(n => n != excludeSelf &&
            string.Equals(n.Title, title, StringComparison.OrdinalIgnoreCase));

    public NotebookViewModel? FindNotebookOf(NoteViewModel note) =>
        Notebooks.FirstOrDefault(nb => nb.Notes.Contains(note));

    public void NavigateToNote(NoteViewModel note)
    {
        SelectNote(note);
        SyncTreeSelectionCallback?.Invoke(note);
    }

    private void ClearTaskLinksToNoteIds(IEnumerable<string> deletedNoteIds)
    {
        var ids = deletedNoteIds.ToHashSet();

        foreach (var group in TaskGroups)
        foreach (var task in group.Tasks)
        {
            if (task.LinkedNoteId != null && ids.Contains(task.LinkedNoteId))
                task.LinkedNoteId = null;
        }

        if (_editingTaskRelatedNote != null && ids.Contains(_editingTaskRelatedNote.Id))
        {
            _editingTaskRelatedNote = null;
            OnPropertyChanged(nameof(EditingTaskRelatedNote));
            OnPropertyChanged(nameof(HasEditingTaskRelatedNote));
        }
    }

    private void AddNotebook()
    {
        var title = ShowInputDialog?.Invoke("ノートブック追加", "ノートブック名を入力してください:");
        if (!string.IsNullOrWhiteSpace(title))
            AddNotebookWithTitle(title.Trim());
    }
}
