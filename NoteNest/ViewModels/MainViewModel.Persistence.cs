using NoteNest.Models;

namespace NoteNest.ViewModels;

public partial class MainViewModel
{
    public void ExportProjectToText(string outputPath)
        => _exportService.ExportProjectToText(BuildProject(), outputPath);

    public int ExportNotebooksToTextFiles(string outputDirectory)
    {
        var project = BuildProject();
        _exportService.ExportNotebooksToTextFiles(project, outputDirectory);
        return project.Notebooks.Count;
    }

    public bool OpenFileAtStartup(string path)
    {
        try
        {
            var project = _fileService.Load(path);
            LoadProject(project, path);
            StatusMessage = $"プロジェクトを開きました: {System.IO.Path.GetFileName(path)}";
            return true;
        }
        catch (Exception ex)
        {
            ShowErrorDialog?.Invoke("エラー", $"ファイルを開けませんでした。\n{ex.Message}");
            return false;
        }
    }

    public bool ConfirmCloseIfModified()
    {
        if (!IsModified) return true;
        return ShowConfirmDialog?.Invoke("未保存の変更", "保存されていない変更があります。終了しますか？") ?? true;
    }

    private bool EnsureCanDiscardChanges(string question)
    {
        if (!IsModified) return true;
        return ShowConfirmDialog?.Invoke("未保存の変更", question) ?? true;
    }

    private void NewProject()
    {
        if (!EnsureCanDiscardChanges("保存されていない変更があります。新規プロジェクトを作成しますか？"))
            return;
        LoadProject(_sampleService.Create(), null);
        StatusMessage = "新規プロジェクトを作成しました。";
    }

    private void OpenProject()
    {
        if (!EnsureCanDiscardChanges("保存されていない変更があります。続けますか？")) return;

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "NoteNest プロジェクト (*.notenest)|*.notenest|すべてのファイル (*.*)|*.*",
            DefaultExt = ".notenest"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            var project = _fileService.Load(dialog.FileName);
            LoadProject(project, dialog.FileName);
            StatusMessage = $"プロジェクトを開きました: {System.IO.Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception ex)
        {
            ShowErrorDialog?.Invoke("エラー", $"ファイルを開けませんでした。\n{ex.Message}");
        }
    }

    private void SaveProject()
    {
        if (_currentFilePath == null) { SaveProjectAs(); return; }
        DoSave(_currentFilePath);
    }

    private void SaveProjectAs()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "NoteNest プロジェクト (*.notenest)|*.notenest",
            DefaultExt = ".notenest",
            FileName = ProjectName
        };

        if (dialog.ShowDialog() != true) return;
        if (DoSave(dialog.FileName))
        {
            _currentFilePath = dialog.FileName;
            OnPropertyChanged(nameof(WindowTitle));
            OnPropertyChanged(nameof(ProjectDisplayName));
        }
    }

    private bool DoSave(string path)
    {
        try
        {
            _fileService.Save(path, BuildProject());
            IsModified = false;
            IsSampleProject = false;
            StatusMessage = $"保存しました: {System.IO.Path.GetFileName(path)}";
            RecordRecentFile(path);
            return true;
        }
        catch (Exception ex)
        {
            ShowErrorDialog?.Invoke("エラー", $"保存に失敗しました。\n{ex.Message}");
            return false;
        }
    }

    private void Exit()
    {
        if (ConfirmCloseIfModified()) RequestClose?.Invoke();
    }

    private void OpenRecentFile(string path)
    {
        if (!EnsureCanDiscardChanges("保存されていない変更があります。続けますか？")) return;
        try
        {
            var project = _fileService.Load(path);
            LoadProject(project, path);
            StatusMessage = $"プロジェクトを開きました: {System.IO.Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            ShowErrorDialog?.Invoke("エラー", $"ファイルを開けませんでした。\n{ex.Message}");
        }
    }

    private void RecordRecentFile(string path)
    {
        _recentFilesService.Add(path);
        RecentFiles.Clear();
        foreach (var p in _recentFilesService.Load())
            RecentFiles.Add(new RecentFileViewModel(p));
    }

    private void LoadProject(Project project, string? filePath)
    {
        _editorMode = EditorMode.NoteEdit;
        _editingTask = null;
        _currentProjectId = project.ProjectId;
        ProjectName = project.ProjectName;
        _currentFilePath = filePath;
        IsSampleProject = filePath == null;
        if (filePath != null) RecordRecentFile(filePath);

        Notebooks.Clear();
        foreach (var nb in project.Notebooks)
            Notebooks.Add(new NotebookViewModel(nb));

        foreach (var group in TaskGroups)
            group.Tasks.Clear();

        var taskMap = new Dictionary<string, List<NoteTask>>
        {
            { "today",   project.Tasks.Today   },
            { "week",    project.Tasks.Week     },
            { "backlog", project.Tasks.Backlog  }
        };

        foreach (var group in TaskGroups)
        {
            if (taskMap.TryGetValue(group.Key, out var tasks))
                foreach (var t in tasks)
                {
                    var taskVm = new TaskViewModel(t);
                    TrackTaskCompletion(taskVm);
                    group.AddTask(taskVm);
                }
        }

        EditorFontFamily = project.Settings.FontFamily;
        EditorFontSize   = project.Settings.FontSize;

        // Restore last opened note
        var lastNote = FindNoteById(project.Settings.LastOpenedNoteId);
        if (lastNote == null && Notebooks.Count > 0 && Notebooks[0].Notes.Count > 0)
            lastNote = Notebooks[0].Notes[0];

        if (lastNote != null)
            SelectNote(lastNote);
        else
            ClearEditor();

        IsModified = false;
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(ProjectDisplayName));
        OnPropertyChanged(nameof(RelatedNoteChoices));
    }

    private Project BuildProject()
    {
        if (_editorMode == EditorMode.NoteEdit && _selectedNote != null)
            _selectedNote.Content = _editorContent;

        return new Project
        {
            Version = Project.CurrentSchemaVersion,
            ProjectId = _currentProjectId,
            ProjectName = ProjectName,
            Notebooks = Notebooks.Select(nb => new Notebook
            {
                Id    = nb.Id,
                Title = nb.Title,
                Notes = nb.Notes.Select(n => new Note
                {
                    Id      = n.Id,
                    Title   = n.Title,
                    Content = n.Content
                }).ToList()
            }).ToList(),
            Tasks = new TaskCollection
            {
                Today   = TaskGroups.First(g => g.Key == "today")  .Tasks.Select(t => t.Model).ToList(),
                Week    = TaskGroups.First(g => g.Key == "week")   .Tasks.Select(t => t.Model).ToList(),
                Backlog = TaskGroups.First(g => g.Key == "backlog").Tasks.Select(t => t.Model).ToList()
            },
            Settings = new AppSettings
            {
                LastOpenedNoteId = SelectedNote?.Id ?? "",
                FontFamily       = EditorFontFamily,
                FontSize         = EditorFontSize
            }
        };
    }
}
