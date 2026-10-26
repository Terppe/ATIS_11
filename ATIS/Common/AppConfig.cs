using Nucs.JsonSettings;
using Nucs.JsonSettings.Modulation;

namespace ATIS.Common;
public class AppConfig : JsonSettings, IVersionable
{
  //  [EnforcedVersion("10.6.0.2610")]
  //  public virtual Version Version { get; set; } = new Version(10, 6, 0, 2610);

  //  public override string FileName { get; set; } = Constants.AppConfigPath;

    public virtual string LastUpdateCheck { get; set; }

    // Docs: https://github.com/Nucs/JsonSettings
    public override string FileName { get; set; }
    public Version Version { get; set; }
}
