using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NestSuite.ChatNest;

/// <summary>
/// ChatNest Workspace ビュー。参照ソース ChatNest v0.4.1
/// Views/ChatNestWorkspaceView.xaml(.cs) より、Workspace 部分を中心に取り込み。
/// メッセージ追加時の自動スクロールと、入力欄のショートカット（Ctrl/Shift+Enter 投稿、
/// Ctrl/Shift+←→ 発言者切替）を処理する。AppShell（NestSuiteShellWindow）は移植しない。
///
/// <para><b>v2.3.0 変更点</b><br/>
/// CH-3: 最下部付近なら自動スクロール。遡り閲覧中は「最新へ」ボタンを表示する。<br/>
/// CH-6: EditBox の PreviewKeyDown / IsVisibleChanged でインライン編集キー操作を処理する。</para>
/// </summary>
public partial class ChatNestWorkspaceView : UserControl
{
    private const double NearBottomThreshold = 100.0;

    public ChatNestWorkspaceView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatNestWorkspaceViewModel oldVm)
            oldVm.Messages.CollectionChanged -= OnMessagesChanged;
        if (e.NewValue is ChatNestWorkspaceViewModel newVm)
            newVm.Messages.CollectionChanged += OnMessagesChanged;
    }

    private bool IsNearBottom()
    {
        var sv = ChatScrollViewer;
        return sv.ScrollableHeight <= 0 || sv.ScrollableHeight - sv.VerticalOffset <= NearBottomThreshold;
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add) return;
        bool wasNearBottom = IsNearBottom();
        Dispatcher.InvokeAsync(() =>
        {
            if (wasNearBottom)
                ChatScrollViewer.ScrollToBottom();
            else
                ScrollToBottomButton.Visibility = Visibility.Visible;
        }, DispatcherPriority.Background);
    }

    private void ChatScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (IsNearBottom())
            ScrollToBottomButton.Visibility = Visibility.Collapsed;
    }

    private void ScrollToBottomButton_Click(object sender, RoutedEventArgs e)
    {
        ChatScrollViewer.ScrollToBottom();
    }

    private void EditBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is not MessageViewModel vm) return;
        if (e.Key == Key.Escape)
        {
            if (vm.CancelEditCommand.CanExecute(null)) vm.CancelEditCommand.Execute(null);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (vm.CommitEditCommand.CanExecute(null)) vm.CommitEditCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void EditBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true && sender is TextBox tb)
        {
            tb.Focus();
            tb.SelectAll();
        }
    }

    private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not ChatNestWorkspaceViewModel vm) return;
        var mods = Keyboard.Modifiers;

        if (e.Key == Key.Enter &&
            (mods == ModifierKeys.Control || mods == ModifierKeys.Shift))
        {
            if (vm.PostCommand.CanExecute(null))
                vm.PostCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if ((e.Key == Key.Right || e.Key == Key.Left) &&
            (mods == ModifierKeys.Control || mods == ModifierKeys.Shift))
        {
            vm.CycleSpeaker(e.Key == Key.Right);
            e.Handled = true;
        }
    }
}
