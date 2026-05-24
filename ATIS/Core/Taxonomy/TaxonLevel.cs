namespace ATIS.Core.Taxonomy;

public record TaxonLevel(
    string Name,
    Func<int, Task> Load,     // Wie lade ich diese Ebene?
    Action ResetBelow         // Was muss unterhalb zurückgesetzt werden?
);
