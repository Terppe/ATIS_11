using System.Configuration;
using System.Reflection;
using ATIS.Views.Search;

namespace ATIS.Views.Main;


public sealed partial class HomePage : Page
{
    public string AppInfo { get; set; }
    public HomeViewModel ViewModel { get; }
    public string? WinAppSdkVersion { get; set; } = ConfigurationManager.AppSettings.Get("WinAppSDK");

    private void HomeSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        NavigateToSearch(args.QueryText);
    }

    private void HomeSearchButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToSearch(HomeSearchBox.Text);
    }

    private void CategoryCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string table })
            Frame.Navigate(typeof(SearchPage), new SearchPageNavigationRequest(table));
    }

    private void NavigateToSearch(string? query)
    {
        Frame.Navigate(typeof(SearchPage), query);
    }

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
