using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NoteNest.NestSuite.ChatNest;

/// <summary>
/// ChatNest Workspace の ViewModel。参照ソース ChatNest v0.4.1
/// ViewModels/ChatNestWorkspaceViewModel.cs より、Workspace 部分を中心に取り込み。
///
/// <para><b>v1.7.0 の位置づけ</b><br/>
/// NestSuite 上で 2 つ目の Workspace として ChatNest を表示できるか検証するための最小実装。
/// メッセージ一覧・入力欄・発言者切替・投稿・削除を提供する。
/// 保存／読込（.chatnest ファイル）はメモリ内のみとし、永続化は次段階へ回す。</para>
///
/// <para><b>MessageBox 暫定許容</b><br/>
/// 発言削除確認に <c>MessageBox</c> を直接使用する。これは ChatNest 参照ソースの挙動を
/// 維持した暫定対応であり、本ファイルは ArchitectureBoundaryTests の走査対象外
/// （NestSuite 配下）に置く。IWorkspaceDialogHost 相当への委譲は次段階の課題。
/// 詳細は docs/design-decisions.md を参照。</para>
/// </summary>
public class ChatNestWorkspaceViewModel : INotifyPropertyChanged
{
    private string _inputText = string.Empty;
    private Speaker _selectedSpeaker = Speaker.自分;
    private bool _isDirty;

    private readonly ChatNestRelayCommand _postCommand;

    public ObservableCollection<Message> Messages { get; } = new();
    public Speaker[] Speakers { get; } = Enum.GetValues<Speaker>();

    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            OnPropertyChanged();
            _postCommand.RaiseCanExecuteChanged();
        }
    }

    public Speaker SelectedSpeaker
    {
        get => _selectedSpeaker;
        set { _selectedSpeaker = value; OnPropertyChanged(); }
    }

    public bool IsDirty
    {
        get => _isDirty;
        private set { _isDirty = value; OnPropertyChanged(); }
    }

    public ICommand PostCommand => _postCommand;
    public ICommand DeleteMessageCommand { get; }

    public event EventHandler? WorkspaceModified;

    public ChatNestWorkspaceViewModel()
    {
        _postCommand = new ChatNestRelayCommand(Post, () => !string.IsNullOrWhiteSpace(InputText));
        DeleteMessageCommand = new ChatNestRelayCommand<Message>(DeleteMessage);
    }

    public void CycleSpeaker(bool forward)
    {
        var speakers = Enum.GetValues<Speaker>();
        int idx = Array.IndexOf(speakers, SelectedSpeaker);
        idx = forward
            ? (idx + 1) % speakers.Length
            : (idx - 1 + speakers.Length) % speakers.Length;
        SelectedSpeaker = speakers[idx];
    }

    public void MarkSaved() => IsDirty = false;

    public void Clear()
    {
        Messages.Clear();
        InputText = string.Empty;
        IsDirty = false;
    }

    public void LoadMessages(IEnumerable<Message> messages)
    {
        Messages.Clear();
        InputText = string.Empty;
        foreach (var m in messages)
            Messages.Add(m);
        IsDirty = false;
    }

    private void Post()
    {
        var text = InputText.Trim();
        if (string.IsNullOrEmpty(text)) return;
        Messages.Add(new Message { Speaker = SelectedSpeaker, Text = text });
        InputText = string.Empty;
        IsDirty = true;
        WorkspaceModified?.Invoke(this, EventArgs.Empty);
    }

    private void DeleteMessage(Message? message)
    {
        if (message == null) return;
        // v1.7.0 暫定許容: 発言削除確認に MessageBox を直接使用（NestSuite 配下のため境界テスト対象外）
        if (MessageBox.Show("この発言を削除しますか？", "削除の確認",
                MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
        {
            Messages.Remove(message);
            IsDirty = true;
            WorkspaceModified?.Invoke(this, EventArgs.Empty);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
