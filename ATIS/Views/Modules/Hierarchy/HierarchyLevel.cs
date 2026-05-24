namespace ATIS.Views.Modules.Hierarchy;

/// <summary>
/// Definiert die 5 implementierten Hierarchie-Ebenen
/// </summary>
public enum HierarchyLevel
{
    Unknown = 0,
    Supertribus = 1,
    Tribus = 2,
    Subtribus = 3,
    Infratribus = 4,
    Genus = 5,
    Species = 6,
    Comment = 7,  // ← Hinzufügen
    Reference = 8 // ← Optional
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

//namespace ATIS.Views.Modules.Hierarchy;

///// <summary>
///// Definiert ALLE 25 Taxonomischen Hierarchie-Ebenen
///// Linnäisches System: Regnum → Species
///// </summary>
//public enum HierarchyLevel
//{
//    Regnum = 0,
//    Phylum = 1,
//    Subphylum = 2,
//    Superclass = 3,
//    Class = 4,
//    Subclass = 5,
//    Infraclass = 6,
//    Legio = 7,
//    Ordo = 8,
//    Subordo = 9,
//    Infraordo = 10,
//    Superfamily = 11,
//    Family = 12,
//    Subfamily = 13,
//    Infrafamily = 14,
//    Supertribus = 15,
//    Tribus = 16,
//    Subtribus = 17,
//    Infratribus = 18,
//    Genus = 19,
//    Species = 20
//}

///// <summary>
///// Args für HierarchyCoordinator.LevelChanged Event
///// </summary>
//public class HierarchyLevelChangedArgs : EventArgs
//{
//    public HierarchyLevel Level { get; set; }
//    public int? SelectedId { get; set; }
//}

///// <summary>
///// Helper Extensions für HierarchyLevel
///// </summary>
//public static class HierarchyLevelExtensions
//{
//    /// <summary>
//    /// Holt den Namen der Ebene als String
//    /// </summary>
//    public static string GetDisplayName(this HierarchyLevel level) => level switch
//    {
//        HierarchyLevel.Regnum => "Regnum (Reich)",
//        HierarchyLevel.Phylum => "Phylum (Stamm)",
//        HierarchyLevel.Subphylum => "Subphylum",
//        HierarchyLevel.Superclass => "Superclass",
//        HierarchyLevel.Class => "Class (Klasse)",
//        HierarchyLevel.Subclass => "Subclass",
//        HierarchyLevel.Infraclass => "Infraclass",
//        HierarchyLevel.Legio => "Legio",
//        HierarchyLevel.Ordo => "Ordo (Ordnung)",
//        HierarchyLevel.Subordo => "Subordo",
//        HierarchyLevel.Infraordo => "Infraordo",
//        HierarchyLevel.Superfamily => "Superfamily",
//        HierarchyLevel.Family => "Family (Familie)",
//        HierarchyLevel.Subfamily => "Subfamily",
//        HierarchyLevel.Infrafamily => "Infrafamily",
//        HierarchyLevel.Supertribus => "Supertribus",
//        HierarchyLevel.Tribus => "Tribus",
//        HierarchyLevel.Subtribus => "Subtribus",
//        HierarchyLevel.Infratribus => "Infratribus",
//        HierarchyLevel.Genus => "Genus",
//        HierarchyLevel.Species => "Species (Art)",
//        _ => "Unbekannt"
//    };

//    /// <summary>
//    /// Holt die Parent-Ebene (eine Ebene höher)
//    /// </summary>
//    public static HierarchyLevel? GetParentLevel(this HierarchyLevel level)
//    {
//        var parentValue = (int)level - 1;
//        if (parentValue < 0)
//            return null;

//        return (HierarchyLevel)parentValue;
//    }

//    /// <summary>
//    /// Holt die nächste Child-Ebene (eine Ebene tiefer)
//    /// </summary>
//    public static HierarchyLevel? GetChildLevel(this HierarchyLevel level)
//    {
//        var childValue = (int)level + 1;
//        if (childValue > 20)
//            return null;

//        return (HierarchyLevel)childValue;
//    }

//    /// <summary>
//    /// Gibt alle Ebenen von dieser bis zur Zieleben zurück
//    /// Für UI: "Sie sind hier: Ordo → Family → Genus"
//    /// </summary>
//    public static IEnumerable<HierarchyLevel> GetHierarchyPath(this HierarchyLevel from, HierarchyLevel to)
//    {
//        var start = Math.Min((int)from, (int)to);
//        var end = Math.Max((int)from, (int)to);

//        for (int i = start; i <= end; i++)
//            yield return (HierarchyLevel)i;
//    }
//}
