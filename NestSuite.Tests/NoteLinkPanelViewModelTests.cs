using NestSuite.ViewModels;
using Xunit;

namespace NestSuite.Tests;

public class NoteLinkPanelViewModelTests
{
    private static NoteViewModel MakeNote(string title, string content = "") =>
        new NoteViewModel(new NestSuite.Models.Note { Title = title, Content = content });

    [Fact]
    public void Refresh_WithNull_ClearsAllAndHasNoteIsFalse()
    {
        var vm = new NoteLinkPanelViewModel();
        var note = MakeNote("A", "[[B]]");
        vm.Refresh(note, [note]);

        vm.Refresh(null, []);

        Assert.False(vm.HasNote);
        Assert.Empty(vm.OutboundLinks);
        Assert.Empty(vm.Backlinks);
    }

    [Fact]
    public void Refresh_WithNote_SetsHasNoteTrue()
    {
        var vm = new NoteLinkPanelViewModel();
        var note = MakeNote("A");

        vm.Refresh(note, [note]);

        Assert.True(vm.HasNote);
    }

    [Fact]
    public void Refresh_ExtractsOutboundLinksFromContent()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A", "See [[B]] and [[C]]");
        var noteB = MakeNote("B");
        var noteC = MakeNote("C");

        vm.Refresh(noteA, [noteA, noteB, noteC]);

        Assert.Equal(2, vm.OutboundLinks.Count);
        Assert.Contains(vm.OutboundLinks, e => e.LinkName == "B");
        Assert.Contains(vm.OutboundLinks, e => e.LinkName == "C");
    }

    [Fact]
    public void Refresh_ResolvedOutboundLink_IsNotBroken()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A", "[[B]]");
        var noteB = MakeNote("B");

        vm.Refresh(noteA, [noteA, noteB]);

        var entry = vm.OutboundLinks.Single();
        Assert.False(entry.IsBroken);
        Assert.Equal(noteB, entry.Target);
    }

    [Fact]
    public void Refresh_UnresolvedOutboundLink_IsBroken()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A", "[[NoSuchNote]]");

        vm.Refresh(noteA, [noteA]);

        var entry = vm.OutboundLinks.Single();
        Assert.True(entry.IsBroken);
        Assert.Null(entry.Target);
    }

    [Fact]
    public void Refresh_LinkResolutionIsCaseInsensitive()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A", "[[meeting notes]]");
        var noteB = MakeNote("Meeting Notes");

        vm.Refresh(noteA, [noteA, noteB]);

        var entry = vm.OutboundLinks.Single();
        Assert.False(entry.IsBroken);
        Assert.Equal(noteB, entry.Target);
    }

    [Fact]
    public void Refresh_BuildsBacklinksFromOtherNotes()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A");
        var noteB = MakeNote("B", "Links to [[A]] here");
        var noteC = MakeNote("C", "Also [[A]]");

        vm.Refresh(noteA, [noteA, noteB, noteC]);

        Assert.Equal(2, vm.Backlinks.Count);
        Assert.Contains(vm.Backlinks, bl => bl.SourceNote == noteB);
        Assert.Contains(vm.Backlinks, bl => bl.SourceNote == noteC);
    }

    [Fact]
    public void Refresh_SelectedNoteExcludedFromBacklinks()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A", "Self ref [[A]]");

        vm.Refresh(noteA, [noteA]);

        Assert.Empty(vm.Backlinks);
    }

    [Fact]
    public void HasNoOutboundLinks_TrueWhenNoteSelectedButNoLinks()
    {
        var vm = new NoteLinkPanelViewModel();
        var note = MakeNote("A", "No links here");

        vm.Refresh(note, [note]);

        Assert.True(vm.HasNoOutboundLinks);
    }

    [Fact]
    public void HasNoBacklinks_TrueWhenNoteSelectedButNoBacklinks()
    {
        var vm = new NoteLinkPanelViewModel();
        var note = MakeNote("A");

        vm.Refresh(note, [note]);

        Assert.True(vm.HasNoBacklinks);
    }

    [Fact]
    public void HasNoNote_TrueInitially()
    {
        var vm = new NoteLinkPanelViewModel();

        Assert.True(vm.HasNoNote);
        Assert.False(vm.HasNote);
    }

    [Fact]
    public void CountTexts_ReflectLinkCounts()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("A", "[[B]]");
        var noteB = MakeNote("B", "[[A]]");

        vm.Refresh(noteA, [noteA, noteB]);

        Assert.Equal("1 件", vm.OutboundCountText);
        Assert.Equal("1 件", vm.BacklinkCountText);
    }

    [Fact]
    public void BacklinkEntry_DisplayText_IsSourceNoteTitle()
    {
        var vm = new NoteLinkPanelViewModel();
        var noteA = MakeNote("Alpha");
        var noteB = MakeNote("Beta", "See [[Alpha]]");

        vm.Refresh(noteA, [noteA, noteB]);

        Assert.Equal("Beta", vm.Backlinks.Single().DisplayText);
    }
}
