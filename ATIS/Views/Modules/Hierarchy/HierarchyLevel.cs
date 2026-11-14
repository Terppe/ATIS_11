namespace ATIS.Views.Modules.Hierarchy;

/// <summary>
/// Definiert die 5 implementierten Hierarchie-Ebenen
/// </summary>
public enum HierarchyLevel
{
    Unknown = 0,
    Regnum = 1,
    Phylum = 2,
    Subphylum = 3,
    Superclass = 4,
    Class = 5,
    Subclass = 6,
    Infraclass = 7,
    Superordo = 8,
    Ordo = 9,
    Subordo = 10,
    Infraordo = 11,
    Superfamily = 12,
    Family = 13,
    Subfamily = 14,
    Infrafamily = 15,
    Supertribus = 16,
    Tribus = 17,
    Subtribus = 18,
    Infratribus = 19,
    Genus = 20,
    Species = 21,
    Comment = 22,  // ← Hinzufügen
    Reference = 23 // ← Optional
}

/// <summary>
/// Args für HierarchyCoordinator.LevelChanged Event
/// </summary>
public class HierarchyLevelChangedArgs : EventArgs
{
    public HierarchyLevel Level { get; set; }
    public int? SelectedId { get; set; }
}

/// <summary>
/// Helper Extensions für HierarchyLevel
/// </summary>
public static class HierarchyLevelExtensions
{
    /// <summary>
    /// Holt den Namen der Ebene als String (für UI/Debug)
    /// </summary>
    public static string GetDisplayName(this HierarchyLevel level) => level switch
    {
        HierarchyLevel.Comment => "Comment",
        HierarchyLevel.Reference => "Reference",
        HierarchyLevel.Regnum => "Regnum",
        HierarchyLevel.Phylum => "Phylum",
        HierarchyLevel.Subphylum => "Subphylum",
        HierarchyLevel.Superclass => "Superclass",
        HierarchyLevel.Class => "Class",
        HierarchyLevel.Subclass => "Subclass",
        HierarchyLevel.Infraclass => "Infraclass",
        HierarchyLevel.Superordo => "Superordo",
        HierarchyLevel.Ordo => "Ordo",
        HierarchyLevel.Subordo => "Subordo",
        HierarchyLevel.Infraordo => "Infraordo",
        HierarchyLevel.Superfamily => "Superfamily",
        HierarchyLevel.Family => "Family",
        HierarchyLevel.Subfamily => "Subfamily",
        HierarchyLevel.Infrafamily => "Infrafamily",
        HierarchyLevel.Supertribus => "Supertribus",
        HierarchyLevel.Tribus => "Tribus",
        HierarchyLevel.Subtribus => "Subtribus",
        HierarchyLevel.Infratribus => "Infratribus",
        HierarchyLevel.Genus => "Genus",
        HierarchyLevel.Species => "Species",
        _ => "Unbekannt"
    };

    /// <summary>
    /// Holt die Parent-Ebene (eine Ebene höher)
    /// </summary>
    public static HierarchyLevel? GetParentLevel(this HierarchyLevel level)
    {
        var parentValue = (int)level - 1;
        if (parentValue < 0)
            return null;

        return (HierarchyLevel)parentValue;
    }

    /// <summary>
    /// Holt die nächste Child-Ebene (eine Ebene tiefer)
    /// </summary>
    public static HierarchyLevel? GetChildLevel(this HierarchyLevel level)
    {
        var childValue = (int)level + 1;
        if (childValue > 3)
            return null;

        return (HierarchyLevel)childValue;
    }
}
