using System.Collections.Generic;
using System.Linq;
using NoteNest.NestSuite.IdeaNest.ViewModels;

namespace NoteNest.NestSuite.IdeaNest.Services;

public static class TagSyncService
{
    public static IReadOnlyList<TagItemViewModel> ComputeTagItems(
        IEnumerable<IdeaCardViewModel> allCards)
    {
        return allCards
            .SelectMany(c => c.Tags)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .GroupBy(t => t, StringComparer.Ordinal)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => new TagItemViewModel(g.Key, g.Count()))
            .ToList()
            .AsReadOnly();
    }
}
