namespace NoteNest.NestSuite;

/// <summary>
/// v1.6.4 NestSuite に登録された内蔵ツールの定義・一覧・統合状態を管理する。
/// ツール定義（<see cref="NestSuiteTool"/>）と文字列 ID ベースの既存 API を両方提供する。
/// NoteNest のみ統合済み。IdeaNest・ChatNest は将来統合予定。
/// </summary>
public static class NestSuiteToolRegistry
{
    public const string NoteNestToolId = "NoteNest";
    public const string IdeaNestToolId = "IdeaNest";
    public const string ChatNestToolId  = "ChatNest";

    // ── ツール定義（v1.6.4） ──────────────────────────────────────────────

    public static readonly NestSuiteTool NoteNestDef = new(
        NoteNestToolId, "NoteNest", "ノート・タスク・マーカー統合管理",
        IsIntegrated: true,  StatusText: "統合済み");

    public static readonly NestSuiteTool IdeaNestDef = new(
        IdeaNestToolId, "IdeaNest", "アイデア整理ツール（将来統合予定）",
        IsIntegrated: false, StatusText: "未統合");

    public static readonly NestSuiteTool ChatNestDef = new(
        ChatNestToolId, "ChatNest", "チャットツール（将来統合予定）",
        IsIntegrated: false, StatusText: "未統合");

    /// <summary>全ツール定義一覧（統合済み・未統合を含む）。先頭が最初の統合済みツール。</summary>
    public static IReadOnlyList<NestSuiteTool> ToolDefinitions { get; } =
        Array.AsReadOnly(new[] { NoteNestDef, IdeaNestDef, ChatNestDef });

    // ── 文字列 ID ベース API（既存互換） ─────────────────────────────────

    private static readonly IReadOnlyList<string> AllToolValues =
        Array.AsReadOnly(new[] { NoteNestToolId, IdeaNestToolId, ChatNestToolId });

    private static readonly HashSet<string> IntegratedToolIds =
        new(StringComparer.Ordinal) { NoteNestToolId };

    /// <summary>NestSuite が将来搭載予定のツール ID 一覧（統合済み・未統合を含む）。</summary>
    public static IReadOnlyList<string> AllTools => AllToolValues;

    /// <summary>指定ツールが現バージョンで統合済みかどうかを返す。</summary>
    public static bool IsIntegrated(string toolId) =>
        IntegratedToolIds.Contains(toolId);
}
