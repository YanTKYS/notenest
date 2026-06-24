using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace NestSuite.Tests;

public class AutomationIdTests
{
    private static List<string> GetIdsFromNestedClass(Type nested) =>
        nested.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
              .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
              .Select(f => (string)f.GetRawConstantValue()!)
              .ToList();

    private static List<string> GetAllIds()
    {
        var ids = new List<string>();
        foreach (var t in typeof(AutomationIds).GetNestedTypes(BindingFlags.Public))
            ids.AddRange(GetIdsFromNestedClass(t));
        return ids;
    }

    [Fact]
    public void AllAutomationIds_MatchDotSeparatedFormat()
    {
        var pattern = new Regex(@"^[A-Za-z][A-Za-z0-9]*(\.[A-Za-z][A-Za-z0-9]*)+$");
        foreach (var id in GetAllIds())
            Assert.Matches(pattern, id);
    }

    [Fact]
    public void AllAutomationIds_ContainOnlyAsciiCharacters()
    {
        foreach (var id in GetAllIds())
            Assert.True(id.All(c => c < 128), $"'{id}' contains non-ASCII characters");
    }

    [Fact]
    public void ShellAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.Shell));

    [Fact]
    public void NoteNestAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.NoteNest));

    [Fact]
    public void EditorAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.Editor));

    [Fact]
    public void IdeaNestAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.IdeaNest));

    [Fact]
    public void ChatNestAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.ChatNest));

    [Fact]
    public void TempNestAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.TempNest));

    [Fact]
    public void DialogAutomationIds_AreUnique() =>
        AssertUniqueWithinClass(typeof(AutomationIds.Dialog));

    [Fact]
    public void AllAutomationIds_PrefixMatchesContainingClass()
    {
        foreach (var nested in typeof(AutomationIds).GetNestedTypes(BindingFlags.Public))
        {
            var className = nested.Name;
            foreach (var id in GetIdsFromNestedClass(nested))
                Assert.StartsWith(className + ".", id);
        }
    }

    private static void AssertUniqueWithinClass(Type nested)
    {
        var ids = GetIdsFromNestedClass(nested);
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }
}
