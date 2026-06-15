using NoteNest.NestSuite.IdeaNest.Models;

namespace NoteNest.NestSuite.IdeaNest.Services;

// Facade for UI-level file operations (open/save dialogs, tab state wiring).
// Actual serialization is delegated to IdeaNestWorkspaceService.
// UI wiring (file dialogs, NestSuiteShellWindow integration) is deferred to v1.8.3.
public static class IdeaNestFileService
{
    public const string FileExtension = ".ideanest";

    // Single source of truth is IdeaNestSchema.CurrentVersion; expose here for callers
    // that only need the file service namespace.
    public const string SchemaVersion = IdeaNestSchema.CurrentVersion;
}
