using System.Configuration;
using System.Reflection;
using ATIS.Views.Search;
using System.Collections.ObjectModel;
using System.Text.Json;
using ATIS.Services;
using WinUI3Localizer;

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

        Loaded += HomePage_Loaded;

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
    private bool _loadingExternalLinks;

    private async void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_loadingExternalLinks)
            return;

        _loadingExternalLinks = true;
        try
        {
            await LoadExternalLinksAsync();
        }
        finally
        {
            _loadingExternalLinks = false;
        }
    }

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

    private async Task LoadExternalLinksAsync()
    {
        var localizer = Localizer.Get();
        try
        {
            var result = await WebsiteSettings.LoadAsync(
                WebsiteSettings.FilePath, WebsiteSettings.DefaultFilePath);

            ExternalLinks.Clear();
            foreach (var link in result.Links)
                ExternalLinks.Add(new ExternalLink(link.Name, link.Uri));

            WebsiteSettingsError.Text = result.InvalidEntries.Count == 0
                ? string.Empty
                : string.Format(
                    localizer.GetLocalizedString("Home_Websites_Invalid"),
                    string.Join(", ", result.InvalidEntries),
                    WebsiteSettings.FilePath);
            WebsiteSettingsError.Visibility = result.InvalidEntries.Count == 0
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            ExternalLinks.Clear();
            WebsiteSettingsError.Text = string.Format(
                localizer.GetLocalizedString("Home_Websites_LoadError"),
                WebsiteSettings.FilePath, ex.Message);
            WebsiteSettingsError.Visibility = Visibility.Visible;
        }
    }
}
