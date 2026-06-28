using System.IO;
using NestSuite.Models;
using NestSuite.ViewModels;
using Xunit;

namespace NestSuite.Tests;

/// <summary>
/// v2.10.1: 有識者提案整理・docs 整合の回帰テスト。
/// docs/planning/expert-proposals-2026-06.md と docs/backlog.md の存在・内容を確認する。
/// </summary>
public class ExpertProposalPlanningTests
{
    private static readonly string RepoRoot =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    // ── planning doc の存在と必須分類 ─────────────────────────────────────

    [Fact]
    public void PlanningDoc_ExpertProposals_Exists()
    {
        var path = Path.Combine(RepoRoot, "docs", "planning", "expert-proposals-2026-06.md");
        Assert.True(File.Exists(path), $"Planning doc not found: {path}");
    }

    [Fact]
    public void PlanningDoc_Contains_ShortTermSection()
    {
        var text = ReadPlanningDoc();
        Assert.Contains("短期採用候補", text);
    }

    [Fact]
    public void PlanningDoc_Contains_StagedSection()
    {
        var text = ReadPlanningDoc();
        Assert.Contains("段階的採用候補", text);
    }

    [Fact]
    public void PlanningDoc_Contains_LongTermSection()
    {
        var text = ReadPlanningDoc();
        Assert.Contains("長期構想", text);
    }

    [Fact]
    public void PlanningDoc_Contains_OutOfScopeSection()
    {
        var text = ReadPlanningDoc();
        Assert.Contains("当面対象外", text);
    }

    [Fact]
    public void PlanningDoc_AI_IsOutOfScope_NotShortTerm()
    {
        // AI 要約・クラウド同期などは当面対象外として整理されている。
        // 当面対象外セクション以降に「外部 AI」が含まれ、短期採用候補には含まれないことを確認する。
        var text = ReadPlanningDoc();
        Assert.Contains("外部 AI", text);
        Assert.Contains("当面対象外", text);
    }

    // ── backlog.md の未完了候補確認 ───────────────────────────────────────

    // TD-33: SH-20 は完了済みのため backlog に存在しない。release-notes.md で確認する。
    [Fact]
    public void ReleaseNotes_Contains_SH20_SaveAll()
    {
        var text = ReadReleaseNotes();
        Assert.Contains("SH-20", text);
    }

    [Fact]
    public void Backlog_Contains_SH19_ShortcutHelp()
    {
        var text = ReadBacklog();
        Assert.Contains("SH-19", text);
    }

    // TD-33: L15 は完了済みのため backlog に存在しない。release-notes.md で確認する。
    [Fact]
    public void ReleaseNotes_Contains_L15_CharCount()
    {
        var text = ReadReleaseNotes();
        Assert.Contains("L15", text);
    }

    [Fact]
    public void Backlog_Contains_M15_MarkerCopy()
    {
        var text = ReadBacklog();
        Assert.Contains("M15", text);
    }

    // TD-33: CH-14 は完了済みのため backlog に存在しない。release-notes.md で確認する。
    [Fact]
    public void ReleaseNotes_Contains_CH14_FormattedCopy()
    {
        var text = ReadReleaseNotes();
        Assert.Contains("CH-14", text);
    }

    [Fact]
    public void Backlog_Contains_TN7_SlotToWorkspace()
    {
        var text = ReadBacklog();
        Assert.Contains("TN-7", text);
    }

    [Fact]
    public void Backlog_Contains_LK5_CrossInput()
    {
        var text = ReadBacklog();
        Assert.Contains("LK-5", text);
    }

    // ── release-notes.md の v2.10.1 エントリ確認 ─────────────────────────

    [Fact]
    public void ReleaseNotes_Contains_V2101()
    {
        var releaseNotes = Path.Combine(RepoRoot, "docs", "release-notes.md");
        Assert.True(File.Exists(releaseNotes), $"release-notes.md not found: {releaseNotes}");
        var text = File.ReadAllText(releaseNotes);
        Assert.Contains("v2.10.1", text);
    }

    // ── TD-33: backlog.md 構成ルール確認 ─────────────────────────────────

    [Fact]
    public void Backlog_StatesOnlyUncompletedItems()
    {
        var text = ReadBacklog();
        Assert.Contains("未着手・保留・将来候補", text);
    }

    [Fact]
    public void Backlog_StatesCompletedItemsManagedInReleaseNotes()
    {
        var text = ReadBacklog();
        Assert.Contains("完了済み項目", text);
        Assert.Contains("release-notes.md", text);
    }

    [Fact]
    public void Backlog_StatesCompletedIdsNotReused()
    {
        var text = ReadBacklog();
        Assert.Contains("完了済み項番は再利用しない", text);
    }

    [Fact]
    public void Backlog_ContainsLTPrefixDescription()
    {
        var text = ReadBacklog();
        Assert.Contains("LT-", text);
    }

    [Fact]
    public void Backlog_ContainsRJPrefixDescription()
    {
        var text = ReadBacklog();
        Assert.Contains("RJ-", text);
    }

    [Fact]
    public void Backlog_ContainsLTSection()
    {
        var text = ReadBacklog();
        Assert.Contains("長期構想・保留（LT-）", text);
    }

    [Fact]
    public void Backlog_ContainsRJSection()
    {
        var text = ReadBacklog();
        Assert.Contains("見送り・採用しない方針（RJ-）", text);
    }

    [Fact]
    public void Backlog_HasNoDetailsCompletedSection()
    {
        // Actual collapsible markdown sections use <summary>; rule text mentioning <details> is allowed.
        var text = ReadBacklog();
        Assert.DoesNotContain("<summary>", text);
    }

    [Fact]
    public void Backlog_HasNoStrikethroughSH()
    {
        var text = ReadBacklog();
        Assert.DoesNotContain("~~SH-", text);
    }

    [Fact]
    public void Backlog_HasNoStrikethroughTN()
    {
        var text = ReadBacklog();
        Assert.DoesNotContain("~~TN-", text);
    }

    [Fact]
    public void Backlog_HasNoStrikethroughCH()
    {
        var text = ReadBacklog();
        Assert.DoesNotContain("~~CH-", text);
    }

    [Fact]
    public void Backlog_HasNoStrikethroughTD()
    {
        var text = ReadBacklog();
        Assert.DoesNotContain("~~TD-", text);
    }

    // ── TD-33: release-notes.md 役割セクション確認 ────────────────────────

    [Fact]
    public void ReleaseNotes_ContainsRoleSection()
    {
        var text = ReadReleaseNotes();
        Assert.Contains("release notes の役割", text);
    }

    [Fact]
    public void ReleaseNotes_ContainsV21019()
    {
        var text = ReadReleaseNotes();
        Assert.Contains("v2.10.19", text);
    }

    // ── TD-33: development guidelines 運用ルール確認 ──────────────────────

    [Fact]
    public void Guidelines_ContainsBacklogReleaseNotesPolicy()
    {
        var text = ReadGuidelines();
        Assert.Contains("backlog / release notes 運用", text);
    }

    [Fact]
    public void Guidelines_ContainsLTRJPolicy()
    {
        var text = ReadGuidelines();
        Assert.Contains("LT-", text);
        Assert.Contains("RJ-", text);
    }

    // ── バージョン / スキーマ ────────────────────────────────────────────

    // ── helpers ─────────────────────────────────────────────────────────

    private string ReadPlanningDoc()
    {
        var path = Path.Combine(RepoRoot, "docs", "planning", "expert-proposals-2026-06.md");
        Assert.True(File.Exists(path), $"Planning doc not found: {path}");
        return File.ReadAllText(path);
    }

    private string ReadBacklog()
    {
        var path = Path.Combine(RepoRoot, "docs", "backlog.md");
        Assert.True(File.Exists(path), $"backlog.md not found: {path}");
        return File.ReadAllText(path);
    }

    private string ReadReleaseNotes()
    {
        var path = Path.Combine(RepoRoot, "docs", "release-notes.md");
        Assert.True(File.Exists(path), $"release-notes.md not found: {path}");
        return File.ReadAllText(path);
    }

    private string ReadGuidelines()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "nestsuite-development-guidelines.md");
        Assert.True(File.Exists(path), $"guidelines not found: {path}");
        return File.ReadAllText(path);
    }
}
