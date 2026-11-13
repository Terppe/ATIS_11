using ATIS.Common;
using Nucs.JsonSettings;

var directory = Path.Combine(Path.GetTempPath(), $"atis-culture-{Guid.NewGuid():N}");
Directory.CreateDirectory(directory);
try
{
    var file = Path.Combine(directory, "settings.json");
    File.WriteAllText(file, """{"MainWindowX":42,"MainWindowWidth":1200}""");
    AppConfig Load() => JsonSettings.Configure<AppConfig>(file).LoadNow();
    void Check(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }

    var settings = Load();
    Check(CultureSettings.LoadOrMigrate(settings, "en-US") == "en-US", "Legacy culture not migrated");
    settings = Load();
    Check(settings.Culture == "en-US", "Migrated culture not persisted");
    Check(settings.MainWindowX == 42 && settings.MainWindowWidth == 1200, "Window settings changed");
    Check(CultureSettings.LoadOrMigrate(settings, "de-DE") == "en-US", "JSON must take precedence");
    CultureSettings.Save(settings, "fr-FR");
    settings = Load();
    Check(CultureSettings.LoadOrMigrate(settings, null) == "fr-FR", "Changed culture not persisted");
    settings.MainWindowHeight = 800;
    settings.Save();
    settings = Load();
    Check(settings.Culture == "fr-FR" && settings.MainWindowHeight == 800, "Window save lost culture");

    var defaultFile = Path.Combine(directory, "default.json");
    var defaults = JsonSettings.Configure<AppConfig>(defaultFile).LoadNow();
    Check(CultureSettings.LoadOrMigrate(defaults, null) == "de-DE", "Default culture incorrect");
    Check(JsonSettings.Configure<AppConfig>(defaultFile).LoadNow().Culture == "de-DE", "Default not persisted");

    try
    {
        CultureSettings.Save(settings, "");
        throw new Exception("Invalid culture accepted");
    }
    catch (InvalidOperationException)
    {
        Check(Load().Culture == "fr-FR", "Invalid culture changed saved value");
    }

    settings.Culture = "";
    try
    {
        CultureSettings.LoadOrMigrate(settings, "de-DE");
        throw new Exception("Invalid saved culture accepted");
    }
    catch (InvalidOperationException)
    {
        Check(Load().Culture == "fr-FR", "Invalid saved culture was overwritten");
    }

    settings.Culture = "fr-FR";
    settings.FileName = directory;
    try
    {
        CultureSettings.Save(settings, "en-US");
        throw new Exception("Failed save was not reported");
    }
    catch (JsonSettingsException)
    {
        Check(settings.Culture == "fr-FR", "Failed save changed in-memory culture");
    }

    Console.WriteLine("CultureSettings checks passed.");
}
finally
{
    Directory.Delete(directory, recursive: true);
}
