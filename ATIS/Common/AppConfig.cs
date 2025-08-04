using Nucs.JsonSettings;
using Nucs.JsonSettings.Modulation;

namespace ATIS.Common;
public class AppConfig : JsonSettings, IVersionable
{
    [EnforcedVersion("9.1.3.2412")]
    public virtual Version Version { get; set; } = new Version(9, 1, 3, 2412);

    public override string FileName { get; set; } = Constants.AppConfigPath;

    public virtual string LastUpdateCheck { get; set; }

    // Docs: https://github.com/Nucs/JsonSettings
}
