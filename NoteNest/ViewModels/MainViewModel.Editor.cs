namespace NoteNest.ViewModels;

public partial class MainViewModel
{
    public void ApplyFontSettings(string fontFamily, double fontSize)
    {
        EditorFontFamily = fontFamily;
        EditorFontSize   = fontSize;
        IsModified       = true;
    }

    private void ClearEditor()
    {
        _editor.Clear();
        _markers.Refresh(Array.Empty<NoteViewModel>());
    }
}
