namespace NoteNest.NestSuite.IdeaNest.Services;

// Facade for UI-level file operations (open/save dialogs, tab state wiring).
// Actual serialization is delegated to IdeaNestWorkspaceService.
// UI wiring (file dialogs, NestSuiteShellWindow integration) is deferred to v1.8.3.
public static class IdeaNestFileService
{
    public const string FileExtension = ".ideanest";

    // Version string written to the "version" field when NestSuite creates a new .ideanest file.
    // Matches IdeaNest v1.1.4 (the reference version this port is based on).
    public const string SchemaVersion = "1.1.4";
}
