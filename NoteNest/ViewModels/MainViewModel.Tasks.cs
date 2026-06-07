using NoteNest.Models;

namespace NoteNest.ViewModels;

public partial class MainViewModel
{
    public void SelectTask(TaskViewModel task)
    {
        _editorMode = EditorMode.TaskComment;
        _editingTask = task;
        _isLoadingNote = true;
        _editorContent = task.Comment;
        OnPropertyChanged(nameof(EditorContent));
        OnPropertyChanged(nameof(EditorTitle));
        OnPropertyChanged(nameof(IsTaskCommentMode));
        OnPropertyChanged(nameof(IsNoteEditMode));
        _isLoadingNote = false;

        _editingTaskRelatedNote = FindNoteById(task.LinkedNoteId);
        OnPropertyChanged(nameof(EditingTaskRelatedNote));
        OnPropertyChanged(nameof(HasEditingTaskRelatedNote));

        RefreshMarkers();
    }

    public void MoveTaskToGroupAt(TaskViewModel source, TaskViewModel target)
    {
        var sourceGroup = TaskGroups.FirstOrDefault(g => g.Tasks.Contains(source));
        var targetGroup = TaskGroups.FirstOrDefault(g => g.Tasks.Contains(target));
        if (sourceGroup == null || targetGroup == null || source == target) return;

        if (sourceGroup == targetGroup)
        {
            var srcIdx = sourceGroup.Tasks.IndexOf(source);
            var tgtIdx = targetGroup.Tasks.IndexOf(target);
            sourceGroup.Tasks.Move(srcIdx, tgtIdx);
        }
        else
        {
            sourceGroup.RemoveTask(source);
            var tgtIdx = targetGroup.Tasks.IndexOf(target);
            targetGroup.InsertTask(tgtIdx, source);
            StatusMessage = $"タスクを「{targetGroup.Title}」に移動しました。";
        }
        IsModified = true;
    }

    public void MoveTask(TaskViewModel task, string targetGroupKey)
    {
        var sourceGroup = TaskGroups.FirstOrDefault(g => g.Tasks.Contains(task));
        var targetGroup = TaskGroups.FirstOrDefault(g => g.Key == targetGroupKey);
        if (sourceGroup == null || targetGroup == null || sourceGroup == targetGroup) return;

        sourceGroup.RemoveTask(task);
        targetGroup.AddTask(task);
        IsModified = true;
        StatusMessage = $"タスクを「{targetGroup.Title}」に移動しました。";
    }

    public void RenameTask(TaskViewModel task, string newTitle)
    {
        task.Title = newTitle;
        if (_editingTask == task)
            OnPropertyChanged(nameof(EditorTitle));
        IsModified = true;
    }

    public void SetTaskRelatedNote(TaskViewModel task, NoteViewModel note)
    {
        task.LinkedNoteId = note.Id;
        if (_editingTask == task)
        {
            _editingTaskRelatedNote = note;
            OnPropertyChanged(nameof(EditingTaskRelatedNote));
            OnPropertyChanged(nameof(HasEditingTaskRelatedNote));
        }
        IsModified = true;
        StatusMessage = $"タスク「{task.Title}」に関連ノート「{note.Title}」を設定しました。";
    }

    public void ClearTaskRelatedNote(TaskViewModel task)
    {
        task.LinkedNoteId = null;
        if (_editingTask == task)
        {
            _editingTaskRelatedNote = null;
            OnPropertyChanged(nameof(EditingTaskRelatedNote));
            OnPropertyChanged(nameof(HasEditingTaskRelatedNote));
        }
        IsModified = true;
    }

    private void AddTask(string groupKey)
    {
        var title = ShowInputDialog?.Invoke("タスク追加", "タスク名を入力してください:");
        if (string.IsNullOrWhiteSpace(title)) return;

        var group = TaskGroups.FirstOrDefault(g => g.Key == groupKey);
        if (group == null) return;

        var task = new TaskViewModel(new NoteTask { Title = title.Trim() });
        TrackTaskCompletion(task);
        group.AddTask(task);
        IsModified = true;
        StatusMessage = $"タスク「{title.Trim()}」を追加しました。";
    }

    private void TrackTaskCompletion(TaskViewModel task)
    {
        task.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TaskViewModel.IsCompleted))
                IsModified = true;
        };
    }

    private void DeleteTask(TaskViewModel task)
    {
        if (_editingTask == task)
        {
            _editorMode = EditorMode.NoteEdit;
            _editingTask = null;
            _isLoadingNote = true;
            _editorContent = _selectedNote?.Content ?? "";
            OnPropertyChanged(nameof(EditorContent));
            OnPropertyChanged(nameof(EditorTitle));
            OnPropertyChanged(nameof(IsTaskCommentMode));
            OnPropertyChanged(nameof(IsNoteEditMode));
            _isLoadingNote = false;
            RefreshMarkers();
        }

        foreach (var group in TaskGroups)
        {
            if (group.RemoveTask(task))
            {
                IsModified = true;
                return;
            }
        }
    }
}
