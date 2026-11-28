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
            return $"v{info}";
        }
    }
}
