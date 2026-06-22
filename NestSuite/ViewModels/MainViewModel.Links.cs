namespace NestSuite.ViewModels;

public partial class MainViewModel
{
    private void RefreshLinks() => _links.Refresh(_editor.SelectedNote, _notes.AllNotes);
}
