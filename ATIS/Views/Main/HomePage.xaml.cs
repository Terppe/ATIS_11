using System.Configuration;
using System.Reflection;
using ATIS.Views.Search;
using System.Collections.ObjectModel;
using System.Configuration;

namespace ATIS.Views.Main;

public sealed partial class HomePage : Page
{
    public string AppInfo { get; set; }
    public HomeViewModel ViewModel { get; }
    public string? WinAppSdkVersion { get; set; } = ConfigurationManager.AppSettings.Get("WinAppSDK");

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();

        LoadExternalLinks();

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
    public ObservableCollection<ExternalLink> ExternalLinks { get; } = new();

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

    public sealed class ExternalLink
    {
        public ExternalLink(string name, Uri uri)
        {
            Name = name;
            Uri = uri;
        }

        public string Name { get; set; }
        public Uri Uri { get; set; }
    }

    private void LoadExternalLinks()
    {
        ExternalLinks.Clear();

        AddExternalLink("ITIS USA", "ItisUSA");
        AddExternalLink("ITIS Canada", "ItisCanada");
        AddExternalLink("Catalogue of Life", "CoLife");
        AddExternalLink("EOL", "EOL");
        AddExternalLink("Mexico", "Mexico");
        AddExternalLink("GBIF", "GlobalBio");
    }

    private void AddExternalLink(string name, string configKey)
    {
        var value = ConfigurationManager.AppSettings[configKey];

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"Die Adresse für '{configKey}' in der App.config fehlt oder ist ungültig.");
        }

        ExternalLinks.Add(new ExternalLink(name, uri));
    }
}
