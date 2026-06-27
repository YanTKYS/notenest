using System.IO;
using Xunit;

namespace NestSuite.Tests;

/// <summary>
/// v2.10.14 TD-28: テストクラス分類・整理方針の一次分析 docs の固定テスト。
/// </summary>
public class TestClassificationAnalysisTests
{
    private static string RepoRoot => Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    [Fact]
    public void AnalysisDocument_Exists()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "test-classification-analysis.md");

        Assert.True(File.Exists(path), $"analysis document not found: {path}");
    }

    [Fact]
    public void AnalysisDocument_ContainsFiveClassifications()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "test-classification-analysis.md");
        var text = File.ReadAllText(path);

        Assert.Contains("クラス単位テスト", text);
        Assert.Contains("機能単位テスト", text);
        Assert.Contains("シナリオ / 回帰テスト", text);
        Assert.Contains("ドキュメント / ルール固定テスト", text);
        Assert.Contains("不要テスト候補", text);
    }

    [Fact]
    public void AnalysisDocument_ContainsClassificationTable()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "test-classification-analysis.md");
        var text = File.ReadAllText(path);

        Assert.Contains("| テストクラス | テストメソッド | 分類 | 対象クラス / 対象機能 | 関連ID | 備考 |", text);
        Assert.Contains("| `ApplicationVersionTests` | `ApplicationVersion_UsesAssemblyInformationalVersion` |", text);
    }

    [Fact]
    public void DevelopmentGuidelines_ContainTestClassNamingPolicy()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "nestsuite-development-guidelines.md");
        var text = File.ReadAllText(path);

        Assert.Contains("テストクラス命名・分類方針", text);
        Assert.Contains("対象クラス名 + Tests", text);
        Assert.Contains("backlog ID、version番号、実装時期だけをテストクラス名にしない", text);
    }

    [Fact]
    public void Backlog_ContainsTD28Completion()
    {
        var path = Path.Combine(RepoRoot, "docs", "backlog.md");
        var text = File.ReadAllText(path);

        Assert.Contains("TD-28", text);
        Assert.Contains("テストクラス分類・整理方針の一次分析", text);
        Assert.Contains("v2.10.14 完了", text);
    }

    [Fact]
    public void ReleaseNotes_ContainV21014()
    {
        var path = Path.Combine(RepoRoot, "docs", "release-notes.md");
        var text = File.ReadAllText(path);

        Assert.Contains("v2.10.14", text);
        Assert.Contains("TD-28", text);
        Assert.Contains("NoteNest schema `1.4.1` 維持", text);
    }

    // TD-30: テストクラス乱立抑制・集約方針の明文化 (v2.10.16)

    [Fact]
    public void DevelopmentGuidelines_ContainTestClassConsolidationPolicy()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "nestsuite-development-guidelines.md");
        var text = File.ReadAllText(path);

        Assert.Contains("テストクラス乱立抑制・集約方針", text);
        Assert.Contains("backlog ID、version番号、実装時期だけを理由に新しいテストクラスを作成しない", text);
        Assert.Contains("既存テストクラスへ追加できないか確認する", text);
    }

    [Fact]
    public void DevelopmentGuidelines_ContainBacklogIdTrackingPolicy()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "nestsuite-development-guidelines.md");
        var text = File.ReadAllText(path);

        Assert.Contains("Trait", text);
        Assert.Contains("メソッドコメント", text);
    }

    [Fact]
    public void AnalysisDocument_ContainsTD30Supplement()
    {
        var path = Path.Combine(RepoRoot, "docs", "development", "test-classification-analysis.md");
        var text = File.ReadAllText(path);

        Assert.Contains("TD-30", text);
        Assert.Contains("v2.10.16", text);
    }

    [Fact]
    public void Backlog_ContainsTD30Completion()
    {
        var path = Path.Combine(RepoRoot, "docs", "backlog.md");
        var text = File.ReadAllText(path);

        Assert.Contains("TD-30", text);
        Assert.Contains("テストクラス乱立抑制", text);
    }

    [Fact]
    public void ReleaseNotes_ContainV21016()
    {
        var path = Path.Combine(RepoRoot, "docs", "release-notes.md");
        var text = File.ReadAllText(path);

        Assert.Contains("v2.10.16", text);
        Assert.Contains("TD-30", text);
    }
}
