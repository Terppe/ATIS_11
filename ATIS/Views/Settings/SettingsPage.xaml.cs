using System.Globalization;
using ATIS.Services;
using System.Configuration;
using ATIS.Models;
using Microsoft.Data.SqlClient;

namespace ATIS.Views.Settings;

public sealed partial class SettingsPage : Page
{
    private string? _testedServer;
    public SettingsPage()
    {
        this.InitializeComponent();
        LoadCurrentValues();
        LanguageBox.SelectionChanged += LanguageBox_SelectionChanged;
        ThemeBox.SelectionChanged += ThemeBox_SelectionChanged;
        TestConnectionButton.Click += TestConnectionButton_Click;
        TestConnectionButton.IsEnabled = true;
        ServerBox.TextChanged += ServerBox_TextChanged;
        SaveServerButton.Click += SaveServerButton_Click;
    }

    private void LoadCurrentValues()
    {
        var version = typeof(App).Assembly.GetName().Version;
        VersionText.Text = version is null ? "ATIS" : $"ATIS Version {version}";

        SelectByTag(LanguageBox, CultureInfo.CurrentUICulture.Name);
        var root = App.MainWindow?.Content as FrameworkElement
                   ?? throw new InvalidOperationException("Das Hauptfenster ist nicht initialisiert.");

        SelectByTag(ThemeBox, root.RequestedTheme.ToString());
        ServerBox.Text = DbServerSettings.DataSource ?? string.Empty;
    }

    private void LanguageBox_SelectionChanged(
        object sender, SelectionChangedEventArgs e)
    {
        if (LanguageBox.SelectedItem is not ComboBoxItem item ||
            item.Tag is not string language ||
            string.Equals(language, CultureInfo.CurrentUICulture.Name,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        App.SetCulture(language);
        App.RaiseLanguageChanged(language);
    }
    private static void SelectByTag(ComboBox box, string tag)
    {
        foreach (var item in box.Items)
        {
            if (item is ComboBoxItem cbi &&
                string.Equals(cbi.Tag as string, tag, StringComparison.OrdinalIgnoreCase))
            {
                box.SelectedItem = cbi;
                return;
            }
        }
    }

    private void ThemeBox_SelectionChanged(
        object sender, SelectionChangedEventArgs e)
    {
        if (ThemeBox.SelectedItem is not ComboBoxItem item ||
            item.Tag is not string theme)
        {
            return;
        }

        var root = App.MainWindow?.Content as FrameworkElement
                   ?? throw new InvalidOperationException("Das Hauptfenster ist nicht initialisiert.");

        root.RequestedTheme = theme switch
        {
            "Default" => ElementTheme.Default,
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => throw new InvalidOperationException($"Unbekanntes App-Design: {theme}")
        };
    }

    private async void TestConnectionButton_Click(object sender, RoutedEventArgs e)
    {
        _testedServer = null;
        SaveServerButton.IsEnabled = false;

        if (string.IsNullOrWhiteSpace(ServerBox.Text))
        {
            ConnectionStatusText.Text = "Bitte einen SQL-Server eingeben.";
            ConnectionStatusText.Visibility = Visibility.Visible;
            return;
        }

        var server = ServerBox.Text.Trim();
        TestConnectionButton.IsEnabled = false;
        ServerBox.IsEnabled = false;
        ConnectionStatusText.Text = "Verbindung wird getestet ...";
        ConnectionStatusText.Visibility = Visibility.Visible;

        try
        {
            var configured = ConfigurationManager.ConnectionStrings["MyDbConnection"]
                             ?? throw new ConfigurationErrorsException("MyDbConnection fehlt in App.config.");

            var builder = new SqlConnectionStringBuilder(configured.ConnectionString)
            {
                DataSource = server,
                ConnectTimeout = 5
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            _testedServer = server;
            SaveServerButton.IsEnabled = true;

            ConnectionStatusText.Text = "Verbindung zur ATIS-Datenbank erfolgreich.";
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException
                                       or ArgumentException or ConfigurationErrorsException)
        {
            SimpleLog.Error($"SQL-Verbindungstest fehlgeschlagen: {ex.Message}");
            ConnectionStatusText.Text =
                "Verbindung fehlgeschlagen. Details stehen im ATIS-Protokoll.";
        }
        finally
        {
            ServerBox.IsEnabled = true;
            TestConnectionButton.IsEnabled = true;
        }
    }

    private void ServerBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _testedServer = null;
        SaveServerButton.IsEnabled = false;
        ConnectionStatusText.Visibility = Visibility.Collapsed;
    }

    private void SaveServerButton_Click(object sender, RoutedEventArgs e)
    {
        ConnectionStatusText.Visibility = Visibility.Visible;

        if (_testedServer is null ||
            !string.Equals(_testedServer, ServerBox.Text.Trim(), StringComparison.Ordinal))
        {
            SaveServerButton.IsEnabled = false;
            ConnectionStatusText.Text = "Bitte zuerst die Verbindung erfolgreich testen.";
            return;
        }

        try
        {
            DbServerSettings.Save(_testedServer);
            SaveServerButton.IsEnabled = false;
            ConnectionStatusText.Text =
                "SQL-Server gespeichert. Bitte ATIS jetzt neu starten.";
        }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException
                                       or System.Security.SecurityException)
        {
            SimpleLog.Error($"SQL-Server konnte nicht gespeichert werden: {ex.Message}");
            ConnectionStatusText.Text =
                "Speichern fehlgeschlagen. Details stehen im ATIS-Protokoll.";
        }
    }
}
