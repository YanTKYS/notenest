using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NoteNest.Dialogs;
using NoteNest.ViewModels;

namespace NoteNest;

public partial class MainWindow
{
    private void TaskItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _taskDragStartPoint = e.GetPosition(null);
        _draggedTask = (sender as FrameworkElement)?.DataContext as TaskViewModel;
    }

    private void TaskItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _draggedTask == null) return;
        var pos = e.GetPosition(null);
        if (Math.Abs(pos.X - _taskDragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(pos.Y - _taskDragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance) return;
        if (sender is DependencyObject dep)
            DragDrop.DoDragDrop(dep, _draggedTask, DragDropEffects.Move);
        _draggedTask = null;
    }

    private void TaskItem_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void TaskItem_Drop(object sender, DragEventArgs e)
    {
        var target = (sender as FrameworkElement)?.DataContext as TaskViewModel;
        if (target != null && _draggedTask != null && _draggedTask != target)
            ViewModel.MoveTaskToGroupAt(_draggedTask, target);
        _draggedTask = null;
    }

    private void TaskGroupHeader_DragOver(object sender, DragEventArgs e)
    {
        if (_draggedTask == null) { e.Effects = DragDropEffects.None; e.Handled = true; return; }
        e.Effects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void TaskGroupHeader_Drop(object sender, DragEventArgs e)
    {
        if (_draggedTask == null) return;
        var group = (sender as FrameworkElement)?.DataContext as TaskGroupViewModel;
        if (group != null)
            ViewModel.MoveTask(_draggedTask, group.Key);
        _draggedTask = null;
    }

    private void NoteItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _noteDragStartPoint = e.GetPosition(null);
        _draggedNote = (sender as FrameworkElement)?.DataContext as NoteViewModel;
    }

    private void NoteItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _draggedNote == null) return;
        var pos = e.GetPosition(null);
        if (Math.Abs(pos.X - _noteDragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(pos.Y - _noteDragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance) return;
        if (sender is DependencyObject dep)
            DragDrop.DoDragDrop(dep, _draggedNote, DragDropEffects.Move);
        _draggedNote = null;
    }

    private void NotebookHeader_DragOver(object sender, DragEventArgs e)
    {
        if (_draggedNote == null) { e.Effects = DragDropEffects.None; e.Handled = true; return; }
        e.Effects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void NotebookHeader_Drop(object sender, DragEventArgs e)
    {
        if (_draggedNote == null) return;
        var targetNotebook = (sender as FrameworkElement)?.DataContext as NotebookViewModel;
        if (targetNotebook != null)
        {
            var note = _draggedNote;
            ViewModel.MoveNoteToNotebook(note, targetNotebook);
            Dispatcher.BeginInvoke(() => SyncTreeSelection(note),
                System.Windows.Threading.DispatcherPriority.Loaded);
        }
        _draggedNote = null;
    }
}
