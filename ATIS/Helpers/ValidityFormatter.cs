using WinUI3Localizer;

namespace ATIS.Helpers;

public static class ValidityFormatter
{
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

    public static string Format(bool? valid, string? validYear)
    {
        string status = valid switch
        {
            true => Loc("Valid"),
            false => Loc("Invalid"),
            null => string.Empty
        };

        if (string.IsNullOrWhiteSpace(status))
            return string.IsNullOrWhiteSpace(validYear) ? string.Empty : validYear;

        return string.IsNullOrWhiteSpace(validYear)
            ? status
            : $"{status} {Loc("Since")} {validYear}";
    }
}
