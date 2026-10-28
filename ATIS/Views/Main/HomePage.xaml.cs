using System.Configuration;
using System.Reflection;
using Mapsui.Utilities;

namespace ATIS.Views.Main;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    public string AppInfo { get; set; }
    public HomeViewModel ViewModel { get; }
    public string? WinAppSdkVersion { get; set; } = ConfigurationManager.AppSettings.Get("WinAppSDK");

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();

        var asm = typeof(App).Assembly;
        var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

        string buildType;
        #if DEBUG
            buildType = "Debug";
        #else
            buildType = info.EndsWith("-local", StringComparison.OrdinalIgnoreCase) ? "Release lokal" : "CI";
        #endif

        LbVersion.Text = $"{App.Current.Copyright} " +
                         " | SDK Version: " + WinAppSdkVersion +
                         " | .NET Core Version: " + Environment.Version +
                         " + OS-Version " + Environment.OSVersion +
                         " | ATIS-Version " + $"v{info}";
    }


}
