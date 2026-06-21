using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace NestSuite.NoteNest.Editor;

public partial class NoteEditorHost : UserControl
{
    private ScrollViewer? _editorScrollViewer;
    private ScrollViewer? _lineNumberScrollViewer;

    public ITextEditorAdapter Editor { get; private set; } = null!;

    public event RoutedEventHandler? OpenNoteLinkClicked;
    public event RoutedEventHandler? InsertNoteLinkClicked;
    public event EventHandler? EditorReady;

    public NoteEditorHost()
    {
        InitializeComponent();
        IsVisibleChanged += (_, _) => { if (IsVisible) UpdateCurrentLineHighlight(); };
    }

    private void EditorBox_Loaded(object sender, RoutedEventArgs e)
    {
        _editorScrollViewer     = GetDescendant<ScrollViewer>(EditorBox);
        _lineNumberScrollViewer = GetDescendant<ScrollViewer>(LineNumberBox);
        if (_editorScrollViewer != null)
            _editorScrollViewer.ScrollChanged += EditorScrollViewer_ScrollChanged;
        Editor = new TextBoxEditorAdapter(EditorBox);
        Editor.SelectionChanged += (_, _) => UpdateCurrentLineHighlight();
        UpdateLineNumbers();
        UpdateCurrentLineHighlight();
        EditorReady?.Invoke(this, EventArgs.Empty);
    }

    private void EditorBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateLineNumbers();
        UpdateCurrentLineHighlight();
    }

    private void EditorScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        _lineNumberScrollViewer?.ScrollToVerticalOffset(e.VerticalOffset);
        Dispatcher.InvokeAsync(UpdateCurrentLineHighlight, DispatcherPriority.Render);
    }

    private void UpdateLineNumbers()
    {
        var count = EditorBox.Text.Count(c => c == '\n') + 1;
        LineNumberBox.Text = string.Join("\n", Enumerable.Range(1, count));
    }

    private void UpdateCurrentLineHighlight()
    {
        if (Editor == null || LineNumberBox.LineCount == 0)
        {
            CurrentLineHighlight.Visibility = Visibility.Collapsed;
            return;
        }
        var lineIndex = Editor.GetLineIndexFromCharacterIndex(Editor.CaretIndex);
        if (lineIndex < 0) lineIndex = 0;
        if (lineIndex >= LineNumberBox.LineCount)
        {
            CurrentLineHighlight.Visibility = Visibility.Collapsed;
            return;
        }
        var charIdx = LineNumberBox.GetCharacterIndexFromLineIndex(lineIndex);
        if (charIdx < 0)
        {
            CurrentLineHighlight.Visibility = Visibility.Collapsed;
            return;
        }
        Rect rect;
        try { rect = LineNumberBox.GetRectFromCharacterIndex(charIdx); }
        catch { CurrentLineHighlight.Visibility = Visibility.Collapsed; return; }
        if (rect.IsEmpty || rect.Height <= 0)
        {
            CurrentLineHighlight.Visibility = Visibility.Collapsed;
            return;
        }
        if (rect.Bottom <= 0 || rect.Top >= LineNumberBox.ActualHeight)
        {
            CurrentLineHighlight.Visibility = Visibility.Collapsed;
            return;
        }
        Canvas.SetTop(CurrentLineHighlight, rect.Top);
        CurrentLineHighlight.Height = rect.Height;
        CurrentLineHighlight.Width = LineHighlightCanvas.ActualWidth > 0
            ? LineHighlightCanvas.ActualWidth
            : LineNumberBox.ActualWidth;
        CurrentLineHighlight.Visibility = Visibility.Visible;
    }

    private void OpenNoteLink_ItemClick(object sender, RoutedEventArgs e) =>
        OpenNoteLinkClicked?.Invoke(this, e);

    private void InsertNoteLink_ItemClick(object sender, RoutedEventArgs e) =>
        InsertNoteLinkClicked?.Invoke(this, e);

    private static T? GetDescendant<T>(DependencyObject obj) where T : DependencyObject
    {
        if (obj is T t) return t;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var result = GetDescendant<T>(VisualTreeHelper.GetChild(obj, i));
            if (result != null) return result;
        }
        return null;
    }
}
