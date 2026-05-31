using NoteNest.Models;

namespace NoteNest.ViewModels;

public class NoteViewModel : BaseViewModel
{
    private readonly Note _model;

    public NoteViewModel(Note model) => _model = model;

    public string Id => _model.Id;

    public string Title
    {
        get => _model.Title;
        set { _model.Title = value; OnPropertyChanged(); }
    }

    public string Content
    {
        get => _model.Content;
        set { _model.Content = value; OnPropertyChanged(); }
    }

    public Note Model => _model;
}
