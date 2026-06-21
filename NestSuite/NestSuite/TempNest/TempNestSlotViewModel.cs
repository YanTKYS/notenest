using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NestSuite.ViewModels;

namespace NestSuite.TempNest;

public class TempNestSlotViewModel : BaseViewModel
{
    private string _title = "";
    private string _body  = "";
    private bool _showCopyFeedback;
    private readonly DispatcherTimer _feedbackTimer;

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

    public bool ShowCopyFeedback
    {
        get => _showCopyFeedback;
        private set => SetProperty(ref _showCopyFeedback, value);
    }

    public event Action? Changed;

    public ICommand CopyBodyCommand { get; }
    public ICommand ClearCommand    { get; }

    public TempNestSlotViewModel()
    {
        _feedbackTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
        _feedbackTimer.Tick += (_, _) =>
        {
            _feedbackTimer.Stop();
            ShowCopyFeedback = false;
        };

        CopyBodyCommand = new RelayCommand(
            _ =>
            {
                if (!string.IsNullOrEmpty(Body))
                {
                    Clipboard.SetText(Body);
                    StartFeedback();
                }
            },
            _ => !string.IsNullOrEmpty(Body));
        ClearCommand = new RelayCommand(
            _ => Clear(),
            _ => !string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Body));
    }

    private void StartFeedback()
    {
        _feedbackTimer.Stop();
        ShowCopyFeedback = true;
        _feedbackTimer.Start();
    }

    public void StopFeedback()
    {
        _feedbackTimer.Stop();
        ShowCopyFeedback = false;
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
