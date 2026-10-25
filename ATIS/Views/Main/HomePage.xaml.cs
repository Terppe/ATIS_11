using System.Configuration;

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

        LbVersion.Text = $"{App.Current.Copyright} " +
                     " | SDK Version: " + WinAppSdkVersion +
                     " | .NET Core Version: " + Environment.Version +
                     " + OS-Version " + Environment.OSVersion +
                     " | ATIS-Version " + $"{App.Current.AppVersion}";

    }


}
