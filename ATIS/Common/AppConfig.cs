using Nucs.JsonSettings;
using Nucs.JsonSettings.Modulation;

namespace ATIS.Common;
public class AppConfig : JsonSettings, IVersionable
{

    public virtual string? LastUpdateCheck { get; set; }

    // Docs: https://github.com/Nucs/JsonSettings
    public override string FileName { get; set; } = CreateSettingsPath();

    private static string CreateSettingsPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ATIS");

        Directory.CreateDirectory(folder);

        return Path.Combine(folder, "settings.json");
    }
    public Version? Version { get; set; }

    public virtual int? MainWindowX { get; set; }
    public virtual int? MainWindowY { get; set; }
    public virtual int? MainWindowWidth { get; set; }
    public virtual int? MainWindowHeight { get; set; }
}
