namespace NoteNest.NestSuite;

/// <summary>
/// NestSuite に登録された内蔵ツールの一覧と統合状態を管理する。
/// v1.6.2 では NoteNest のみ統合済み。IdeaNest・ChatNest は将来統合予定。
/// </summary>
public static class NestSuiteToolRegistry
{
    public const string NoteNestToolId = "NoteNest";
    public const string IdeaNestToolId = "IdeaNest";
    public const string ChatNestToolId  = "ChatNest";

    private static readonly string[] AllToolValues =
        [NoteNestToolId, IdeaNestToolId, ChatNestToolId];

    private static readonly HashSet<string> IntegratedToolIds =
        new(StringComparer.Ordinal) { NoteNestToolId };

    /// <summary>NestSuite が将来搭載予定のツール一覧（統合済み・未統合を含む）。</summary>
    public static IReadOnlyList<string> AllTools => AllToolValues;

    /// <summary>指定ツールが現バージョンで統合済みかどうかを返す。</summary>
    public static bool IsIntegrated(string toolId) =>
        IntegratedToolIds.Contains(toolId);
}
