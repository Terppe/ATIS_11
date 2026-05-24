
namespace ATIS.Core.Taxonomy;

public static class TaxonomyNavigator
{
    public static Task NavigateToAsync(string levelName, int parentId)
    {
        var level = TaxonomyDefinition.Levels
            .FirstOrDefault(x => x.Name.Equals(levelName, StringComparison.OrdinalIgnoreCase));

        if (level == null)
            throw new InvalidOperationException($"Unknown taxon level: {levelName}");

        // Reset lower levels
        level.ResetBelow?.Invoke();

        // Load the level
        return level.Load(parentId);
    }
}
