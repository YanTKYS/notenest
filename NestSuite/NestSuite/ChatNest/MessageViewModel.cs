using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NestSuite.ChatNest;

/// <summary>
/// v2.3.0 CH-6: Message の UI ラッパー。インライン編集状態（IsEditing / EditingText）を管理する。
/// 編集開始・削除依頼は callback を通じて <see cref="ChatNestWorkspaceViewModel"/> へ委譲する。
/// </summary>
public class MessageViewModel : INotifyPropertyChanged
{
    private bool _isEditing;
    private string _editingText = string.Empty;

    private readonly Action<MessageViewModel> _onBeginEditRequested;
    private readonly Action<MessageViewModel> _onRequestDelete;
    private readonly Action<MessageViewModel> _onEditCommitted;

    public Message Model { get; }

    public Speaker Speaker => Model.Speaker;
    public DateTimeOffset CreatedAt => Model.CreatedAt;
    public string Text => Model.Text;

    public bool IsEditing
    {
        get => _isEditing;
        private set { _isEditing = value; OnPropertyChanged(); }
    }

    public string EditingText
    {
        get => _editingText;
        set
        {
            _editingText = value;
            OnPropertyChanged();
            CommitEditCommand.RaiseCanExecuteChanged();
        }
    }

    public ChatNestRelayCommand BeginEditCommand { get; }
    public ChatNestRelayCommand CommitEditCommand { get; }
    public ChatNestRelayCommand CancelEditCommand { get; }
    public ChatNestRelayCommand RequestDeleteCommand { get; }

    public MessageViewModel(
        Message model,
        Action<MessageViewModel> onBeginEditRequested,
        Action<MessageViewModel> onRequestDelete,
        Action<MessageViewModel> onEditCommitted)
    {
        Model = model;
        _onBeginEditRequested = onBeginEditRequested;
        _onRequestDelete = onRequestDelete;
        _onEditCommitted = onEditCommitted;

        BeginEditCommand     = new ChatNestRelayCommand(() => _onBeginEditRequested(this));
        CommitEditCommand    = new ChatNestRelayCommand(CommitEdit, () => !string.IsNullOrWhiteSpace(EditingText));
        CancelEditCommand    = new ChatNestRelayCommand(CancelEdit);
        RequestDeleteCommand = new ChatNestRelayCommand(() => _onRequestDelete(this));
    }

    internal void BeginEditInternal()
    {
        EditingText = Model.Text;
        IsEditing = true;
    }

    private void CommitEdit()
    {
        var trimmed = EditingText.Trim();
        if (string.IsNullOrEmpty(trimmed)) return;
        Model.Text = trimmed;
        IsEditing = false;
        OnPropertyChanged(nameof(Text));
        _onEditCommitted(this);
    }

    private void CancelEdit()
    {
        IsEditing = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
