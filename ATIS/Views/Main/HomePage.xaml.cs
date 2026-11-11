using System.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using WinUI3Localizer;

namespace ATIS.Views.Main;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    public string AppInfo { get; set; }
    public HomeViewModel ViewModel { get; }
    public string? WinAppSdkVersion { get; set; } = ConfigurationManager.AppSettings.Get("WinAppSDK");
    public ObservableCollection<ExternalLink> ExternalLinks { get; } = new();
    private bool _externalLinksLoaded;

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();
        Loaded += HomePage_Loaded;

        var asm = typeof(App).Assembly;
        var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var fileVer = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location).ProductVersion;

        LbVersion.Text = $"{App.Current.Copyright} " +
                         " | SDK Version: " + WinAppSdkVersion +
                         " | .NET Core Version: " + Environment.Version +
                         " + OS-Version " + Environment.OSVersion +
                         " | ATIS-Version " + (info ?? "null") + " / " + (fileVer ?? "null");
    }

    private static string WebsitesPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ATIS",
        "websites.json");

    private async void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_externalLinksLoaded)
            return;

        await LoadExternalLinksAsync();
    }

    private async Task LoadExternalLinksAsync()
    {
        try
        {
            var directory = Path.GetDirectoryName(WebsitesPath)
                ?? throw new InvalidOperationException("The website settings directory could not be determined.");
            Directory.CreateDirectory(directory);

            if (!File.Exists(WebsitesPath))
            {
                var defaultsPath = Path.Combine(AppContext.BaseDirectory, "Defaults", "websites.json");
                if (!File.Exists(defaultsPath))
                    throw new FileNotFoundException("The default website settings file was not found.", defaultsPath);

                File.Copy(defaultsPath, WebsitesPath);
            }

            var json = await File.ReadAllTextAsync(WebsitesPath);
            var entries = JsonSerializer.Deserialize<Dictionary<string, string?>>(json)
                ?? throw new JsonException("The website settings file must contain a JSON object.");

            var links = new List<ExternalLink>();
            var invalidEntries = new List<string>();
            foreach (var (key, value) in entries)
            {
                const string prefix = "Website.";
                if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                var name = key[prefix.Length..].Trim();
                if (string.IsNullOrWhiteSpace(name) ||
                    !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    invalidEntries.Add(key);
                    continue;
                }

                links.Add(new ExternalLink(name, uri));
            }

            ExternalLinks.Clear();
            foreach (var link in links)
                ExternalLinks.Add(link);

            _externalLinksLoaded = true;

            if (invalidEntries.Count > 0)
            {
                await ShowWebsiteSettingsErrorAsync(
                    "Home_WebsiteSettings_Invalid",
                    string.Join("\n", invalidEntries),
                    WebsitesPath);
            }
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException or JsonException)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await ShowWebsiteSettingsErrorAsync(
                "Home_WebsiteSettings_LoadError",
                ex.Message,
                WebsitesPath);
        }
    }

    private async Task ShowWebsiteSettingsErrorAsync(
        string messageKey,
        params object[] formatArgs)
    {
        var localizer = Localizer.Get();
        var dialog = new ContentDialog
        {
            Title = localizer.GetLocalizedString("Home_WebsiteSettings_Title"),
            Content = string.Format(localizer.GetLocalizedString(messageKey), formatArgs),
            CloseButtonText = localizer.GetLocalizedString("Dialog_Ok"),
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();
    }

    public sealed class ExternalLink(string name, Uri uri)
    {
        public string Name { get; set; } = name;
        public Uri Uri { get; set; } = uri;
    }
}
