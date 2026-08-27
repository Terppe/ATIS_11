
namespace ATIS.Helpers;

public static class SpeciesDisplayFormatter
{
    public static string Format(
        string? genusName,
        string? speciesName,
        string? subspecies = null,
        string? divers = null)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(genusName))
            parts.Add(genusName);

        if (!string.IsNullOrWhiteSpace(speciesName))
            parts.Add(speciesName);

        if (!string.IsNullOrWhiteSpace(subspecies))
            parts.Add(subspecies);

        if (!string.IsNullOrWhiteSpace(divers))
            parts.Add(divers);

        return string.Join(" ", parts);
    }
}
