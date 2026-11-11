using Nucs.JsonSettings;
using Nucs.JsonSettings.Modulation.Recovery;

namespace ATIS.Common;

public static partial class AppHelper
{
    public static AppConfig Settings = JsonSettings.Configure<AppConfig>()
        .WithRecovery(RecoveryAction.RenameAndLoadDefault)
        .LoadNow();
}
