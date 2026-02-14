using System.Globalization;
using Microsoft.Extensions.Localization;

namespace ATIS.Services;

public static class LocalizationManager
{
    public static event Action? LanguageChanged;

    public static IStringLocalizer? Localizer { get; private set; }

    public static void Initialize(IStringLocalizer localizer)
    {
        Localizer = localizer;
    }

    public static string Get(string key)
        => Localizer?[key] ?? key;

    public static void ChangeLanguage(string culture)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        LanguageChanged?.Invoke();
    }
}
