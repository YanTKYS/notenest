namespace NestSuite.NoteNest.Editor;

public enum LineHighlightKind { Todo, Fixme, Note, NoteLink }

public sealed record LineHighlightInfo(int LogicalIndex, LineHighlightKind Kind);

public static class MarkerLineDetector
{
    // Returns one LineHighlightInfo per logical line that contains a recognised marker.
    // Priority when multiple markers appear on the same line: FIXME > TODO > NOTE > NoteLink.
    public static IReadOnlyList<LineHighlightInfo> Detect(string text)
    {
        if (string.IsNullOrEmpty(text)) return Array.Empty<LineHighlightInfo>();

        var result = new List<LineHighlightInfo>();
        int lineIndex = 0;
        int start = 0;
        while (start <= text.Length)
        {
            int end = text.IndexOf('\n', start);
            int lineEnd = end < 0 ? text.Length : end;
            var kind = ClassifyLine(text, start, lineEnd - start);
            if (kind.HasValue)
                result.Add(new LineHighlightInfo(lineIndex, kind.Value));
            if (end < 0) break;
            start = end + 1;
            lineIndex++;
        }
        return result;
    }

    private static LineHighlightKind? ClassifyLine(string text, int offset, int length)
    {
        ReadOnlySpan<char> span = text.AsSpan(offset, length);
        if (span.IndexOf("FIXME", StringComparison.OrdinalIgnoreCase) >= 0) return LineHighlightKind.Fixme;
        if (span.IndexOf("TODO",  StringComparison.OrdinalIgnoreCase) >= 0) return LineHighlightKind.Todo;
        if (span.IndexOf("NOTE",  StringComparison.OrdinalIgnoreCase) >= 0) return LineHighlightKind.Note;
        if (span.IndexOf("[[",    StringComparison.Ordinal)            >= 0) return LineHighlightKind.NoteLink;
        return null;
    }
}
