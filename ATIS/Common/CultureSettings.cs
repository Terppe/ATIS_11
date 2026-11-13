using System.Globalization;

namespace ATIS.Common;

public static class CultureSettings
{
    public static string LoadOrMigrate(AppConfig settings, string? legacyCulture)
    {
        if (settings.Culture is not null)
            return Validate(settings.Culture);

        var culture = Validate(legacyCulture ?? "de-DE");
        Save(settings, culture);
        return culture;
    }

    public static void Save(AppConfig settings, string culture)
    {
        var validated = Validate(culture);
        var previous = settings.Culture;
        settings.Culture = validated;
        try
        {
            settings.Save();
        }
        catch
        {
            settings.Culture = previous;
            throw;
        }
    }

    private static string Validate(string culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
            throw new InvalidOperationException("Die gespeicherte Sprache darf nicht leer sein.");

        try
        {
            return CultureInfo.GetCultureInfo(culture).Name;
        }
        catch (CultureNotFoundException ex)
        {
            throw new InvalidOperationException($"Die Sprache '{culture}' ist ungültig.", ex);
        }
    }
}
