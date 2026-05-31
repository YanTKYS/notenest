using NoteNest.Models;

namespace NoteNest.ViewModels;

public class TaskViewModel : BaseViewModel
{
    private readonly NoteTask _model;

    public TaskViewModel(NoteTask model) => _model = model;

    public string Id => _model.Id;

    public string Title
    {
        get => _model.Title;
        set { _model.Title = value; OnPropertyChanged(); }
    }

    public bool IsCompleted
    {
        get => _model.IsCompleted;
        set { _model.IsCompleted = value; OnPropertyChanged(); }
    }

    public NoteTask Model => _model;
}
