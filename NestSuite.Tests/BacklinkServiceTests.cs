using NestSuite.Models;
using NestSuite.Services;
using NestSuite.ViewModels;
using Xunit;

namespace NestSuite.Tests;

public class BacklinkServiceTests
{
    private static NoteViewModel MakeNote(string title, string content = "") =>
        new(new Note { Title = title, Content = content });

    [Fact]
    public void NoBacklinks_ReturnsEmpty()
    {
        var notes = new[] { MakeNote("A", "no links here"), MakeNote("B") };
        var result = BacklinkService.FindBacklinks("A", notes);
        Assert.Equal(0, result.TotalLinkCount);
        Assert.Empty(result.AffectedNotes);
    }

    [Fact]
    public void OtherNoteHasLink_IsDetected()
    {
        var noteA = MakeNote("A", "some text");
        var noteB = MakeNote("B", "see [[A]] for details");
        var result = BacklinkService.FindBacklinks("A", new[] { noteA, noteB });
        Assert.Equal(1, result.TotalLinkCount);
        Assert.Single(result.AffectedNotes);
        Assert.Same(noteB, result.AffectedNotes[0]);
    }

    [Fact]
    public void MultipleNotesReferencing_AllCounted()
    {
        var noteA = MakeNote("A");
        var noteB = MakeNote("B", "[[A]]");
        var noteC = MakeNote("C", "[[A]] and [[A]] again");
        var result = BacklinkService.FindBacklinks("A", new[] { noteA, noteB, noteC });
        Assert.Equal(3, result.TotalLinkCount);
        Assert.Equal(2, result.AffectedNotes.Count);
    }

    [Fact]
    public void ExcludeNote_OwnContentIgnored()
    {
        var noteA = MakeNote("A", "self reference [[A]]");
        var result = BacklinkService.FindBacklinks("A", new[] { noteA }, excludeNote: noteA);
        Assert.Equal(0, result.TotalLinkCount);
        Assert.Empty(result.AffectedNotes);
    }

    [Fact]
    public void CaseInsensitiveMatch()
    {
        var noteA = MakeNote("Meeting");
        var noteB = MakeNote("B", "see [[meeting]] and [[MEETING]]");
        var result = BacklinkService.FindBacklinks("Meeting", new[] { noteA, noteB });
        Assert.Equal(2, result.TotalLinkCount);
        Assert.Single(result.AffectedNotes);
    }

    [Fact]
    public void RegularUrls_NotDetected()
    {
        var noteA = MakeNote("A");
        var noteB = MakeNote("B", "https://example.com and [text](url)");
        var result = BacklinkService.FindBacklinks("A", new[] { noteA, noteB });
        Assert.Equal(0, result.TotalLinkCount);
    }

    [Fact]
    public void WhitespaceOnlyLink_NotDetected()
    {
        var noteA = MakeNote("A");
        var noteB = MakeNote("B", "[[ ]]");
        var result = BacklinkService.FindBacklinks("A", new[] { noteA, noteB });
        Assert.Equal(0, result.TotalLinkCount);
    }

    [Fact]
    public void ExistingDuplicateCheck_NotAffected()
    {
        var workspace = new NoteWorkspaceViewModel();
        var nb = workspace.AddNotebook("NB");
        workspace.AddNote(nb, "Note A");
        workspace.AddNote(nb, "Note B");
        var noteA = workspace.FindNoteByTitle("Note A")!;
        // Duplicate rename must still be rejected — BacklinkService does not bypass this
        Assert.False(workspace.RenameNote(noteA, "Note B"));
        Assert.Equal("Note A", noteA.Title);
    }

    [Fact]
    public void RenameWithNoBacklinks_Succeeds()
    {
        var workspace = new NoteWorkspaceViewModel();
        var nb = workspace.AddNotebook("NB");
        workspace.AddNote(nb, "Alpha");
        var alpha = workspace.FindNoteByTitle("Alpha")!;
        // No other note references [[Alpha]], so rename should succeed directly
        Assert.True(workspace.RenameNote(alpha, "Beta"));
        Assert.Equal("Beta", alpha.Title);
    }
}
