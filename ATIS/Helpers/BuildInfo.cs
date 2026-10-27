using System.Reflection;

namespace ATIS.Helpers;

public static class BuildInfo
{
    public static string Text
    {
        get
        {
            var info = typeof(App).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
#if DEBUG
            var type = "Debug";
#else
            var type = info.EndsWith("-local", StringComparison.OrdinalIgnoreCase) ? "Release lokal" : "CI";
#endif
            return $"ATIS-Version {info} ({type})";
        }
    }
}
