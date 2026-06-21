using System.Windows;
using System.Windows.Input;
using NestSuite.ViewModels;

namespace NestSuite.TempNest;

public class TempNestSlotViewModel : BaseViewModel
{
    private string _title = "";
    private string _body  = "";

    public string Title
    {
        get => _title;
        set { if (SetProperty(ref _title, value)) Changed?.Invoke(); }
    }

    public string Body
    {
        get => _body;
        set { if (SetProperty(ref _body, value)) Changed?.Invoke(); }
    }

    public event Action? Changed;

    public ICommand CopyBodyCommand { get; }
    public ICommand ClearCommand    { get; }

    public TempNestSlotViewModel()
    {
        CopyBodyCommand = new RelayCommand(
            _ => { if (!string.IsNullOrEmpty(Body)) Clipboard.SetText(Body); },
            _ => !string.IsNullOrEmpty(Body));
        ClearCommand = new RelayCommand(_ => Clear());
    }

    private void Clear()
    {
        Title = "";
        Body  = "";
    }

    public TempNestSlot ToSlot()
        => new() { Title = Title, Body = Body, UpdatedAt = DateTimeOffset.Now };

    public void LoadFromSlot(TempNestSlot slot)
    {
        _title = slot.Title;
        _body  = slot.Body;
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Body));
    }
}
