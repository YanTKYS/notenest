using System.IO;
using System.Text;
using NoteNest.Models;

namespace NoteNest.Services;

public class ExportService
{
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    public void ExportProjectToText(Project project, string outputPath)
        => File.WriteAllText(outputPath, BuildProjectText(project), Encoding.UTF8);

    public void ExportNotebooksToTextFiles(Project project, string outputDirectory)
    {
        foreach (var notebook in project.Notebooks)
        {
            var safeName = SanitizeFileName(notebook.Title);
            var filePath  = GetUniqueFilePath(outputDirectory, safeName, ".txt");
            File.WriteAllText(filePath, BuildNotebookText(project, notebook), Encoding.UTF8);
        }
    }

    public static string BuildProjectText(Project project)
    {
        var sb = new StringBuilder();
        sb.AppendLine("NoteNest Export");
        sb.AppendLine($"Project: {project.ProjectName}");
        sb.AppendLine($"ExportedAt: {DateTime.Now:yyyy-MM-dd HH:mm}");

        foreach (var notebook in project.Notebooks)
        {
            sb.AppendLine();
            sb.AppendLine(new string('=', 60));
            sb.AppendLine($"Notebook: {notebook.Title}");
            sb.AppendLine(new string('=', 60));

            foreach (var note in notebook.Notes)
                AppendNote(sb, note);
        }

        return sb.ToString();
    }

    public static string BuildNotebookText(Project project, Notebook notebook)
    {
        var sb = new StringBuilder();
        sb.AppendLine("NoteNest Export");
        sb.AppendLine($"Project: {project.ProjectName}");
        sb.AppendLine($"Notebook: {notebook.Title}");
        sb.AppendLine($"ExportedAt: {DateTime.Now:yyyy-MM-dd HH:mm}");

        foreach (var note in notebook.Notes)
            AppendNote(sb, note);

        return sb.ToString();
    }

    public static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "notebook";

        var safe = new string(name.Select(c => InvalidFileNameChars.Contains(c) ? '_' : c).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(safe) ? "notebook" : safe;
    }

    public static string GetUniqueFilePath(string directory, string baseName, string extension)
    {
        var path = Path.Combine(directory, baseName + extension);
        if (!File.Exists(path)) return path;

        for (int i = 2; ; i++)
        {
            path = Path.Combine(directory, $"{baseName}_{i}{extension}");
            if (!File.Exists(path)) return path;
        }
    }

    private static void AppendNote(StringBuilder sb, Note note)
    {
        sb.AppendLine();
        sb.AppendLine(new string('-', 60));
        sb.AppendLine($"Note: {note.Title}");
        sb.AppendLine(new string('-', 60));
        sb.AppendLine();
        sb.AppendLine(note.Content.TrimEnd('\r', '\n'));
    }
}
