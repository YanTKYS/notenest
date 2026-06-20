using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NestSuite.ViewModels;

namespace NestSuite.Views;

public partial class NoteNestWorkspaceView
{
    private void InitNoteFilter()
    {
        DataContextChanged += NoteFilter_DataContextChanged;
    }

    private void NoteFilter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is MainViewModel oldVm)
        {
            oldVm.Notebooks.CollectionChanged -= Notebooks_CollectionChanged;
            foreach (var nb in oldVm.Notebooks)
                CollectionViewSource.GetDefaultView(nb.Notes).Filter = null;
        }

        NoteFilterBox.Text = string.Empty;

        if (e.NewValue is MainViewModel newVm)
            newVm.Notebooks.CollectionChanged += Notebooks_CollectionChanged;
    }

    private void Notebooks_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var filterText = NoteFilterBox.Text;
        if (string.IsNullOrEmpty(filterText) || e.NewItems == null) return;
        foreach (NotebookViewModel nb in e.NewItems)
        {
            var view = CollectionViewSource.GetDefaultView(nb.Notes);
            view.Filter = obj => obj is NoteViewModel note &&
                note.Title.Contains(filterText, StringComparison.OrdinalIgnoreCase);
        }
    }

    private void NoteFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filterText = NoteFilterBox.Text;
        NoteFilterClearButton.Visibility = string.IsNullOrEmpty(filterText)
            ? Visibility.Collapsed
            : Visibility.Visible;
        ApplyNoteFilter(filterText);
    }

    private void NoteFilterClear_Click(object sender, RoutedEventArgs e)
    {
        NoteFilterBox.Text = string.Empty;
        NoteFilterBox.Focus();
    }

    private void ApplyNoteFilter(string filterText)
    {
        if (DataContext is not MainViewModel vm) return;
        foreach (var nb in vm.Notebooks)
        {
            var view = CollectionViewSource.GetDefaultView(nb.Notes);
            if (string.IsNullOrEmpty(filterText))
                view.Filter = null;
            else
                view.Filter = obj => obj is NoteViewModel note &&
                    note.Title.Contains(filterText, StringComparison.OrdinalIgnoreCase);
        }

        if (string.IsNullOrEmpty(filterText) && vm.SelectedNote != null)
            SyncTreeSelection(vm.SelectedNote);
    }
}
