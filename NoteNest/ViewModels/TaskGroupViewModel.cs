using System.Collections.ObjectModel;

namespace NoteNest.ViewModels;

public class TaskGroupViewModel : BaseViewModel
{
    private bool _isExpanded = true;

    public TaskGroupViewModel(string title, string key)
    {
        Title = title;
        Key = key;
        Tasks = new ObservableCollection<TaskViewModel>();
        Tasks.CollectionChanged += (_, _) => RefreshCount();
    }

    public string Title { get; }
    public string Key { get; }
    public ObservableCollection<TaskViewModel> Tasks { get; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    public string CountText => $"{Tasks.Count(t => !t.IsCompleted)}/{Tasks.Count}";

    public void AddTask(TaskViewModel task)
    {
        task.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TaskViewModel.IsCompleted))
                RefreshCount();
        };
        Tasks.Add(task);
    }

    public void RefreshCount() => OnPropertyChanged(nameof(CountText));
}
