
namespace ATIS.Helpers;

public static class NamesFormatter
{
    public static string Format(string? engName, string? gerName, string? fraName, string? porName)
    {
        var names = new List<string>();

        if (!string.IsNullOrWhiteSpace(engName))
            names.Add($"{engName} [ENG]");

        if (!string.IsNullOrWhiteSpace(gerName))
            names.Add($"{gerName} [GER]");

        if (!string.IsNullOrWhiteSpace(fraName))
            names.Add($"{fraName} [FRA]");

        if (!string.IsNullOrWhiteSpace(porName))
            names.Add($"{porName} [SPA]");

        return string.Join("; ", names);
    }
}
