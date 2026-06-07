using NoteNest.ViewModels;
using Xunit;

namespace NoteNest.Tests;

public class MainViewModelCompositionTests
{
    [Fact]
    public void FacadeExposesIndependentResponsibilityOwners()
    {
        var main = new MainViewModel();

        Assert.Same(main.Notes.Notebooks, main.Notebooks);
        Assert.Same(main.Tasks.TaskGroups, main.TaskGroups);
        Assert.Same(main.MarkerPanel.Markers, main.Markers);
        Assert.Equal(main.Editor.Content, main.EditorContent);
    }

    [Fact]
    public void EditorFacadePropagatesContentAndPersistentSettings()
    {
        var main = new MainViewModel();
        var notebook = main.Notes.AddNotebook("NB");
        var note = main.Notes.AddNote(notebook, "Note")!;
        main.SelectNote(note);
        main.IsModified = false;

        main.Editor.Content = "edited";
        main.Editor.FontSize = 18;

        Assert.Equal("edited", note.Content);
        Assert.Equal(18, main.EditorFontSize);
        Assert.True(main.IsModified);
    }

    [Fact]
    public void EditorFacadePropagatesTaskComment()
    {
        var main = new MainViewModel();
        var task = main.Tasks.AddTask("today", "Task")!;
        main.SelectTask(task);
        main.IsModified = false;

        main.Editor.Content = "comment";

        Assert.Equal("comment", task.Comment);
        Assert.True(main.IsModified);
    }

    [Fact]
    public void DirectNoteChangeMarksProjectModifiedAndRefreshesMarkers()
    {
        var main = new MainViewModel();
        var notebook = main.Notes.AddNotebook("NB");
        var note = main.Notes.AddNote(notebook, "Note")!;
        var markerCountBeforeChange = main.MarkerCount;
        main.IsModified = false;

        note.Content = "[TODO] direct change";

        Assert.True(main.IsModified);
        Assert.Equal(markerCountBeforeChange + 1, main.MarkerCount);
    }
}
