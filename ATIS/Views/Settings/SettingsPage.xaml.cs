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
        SaveServerButton.Click += SaveServerButton_Click;
    }

    private void LoadCurrentValues()
    {
        var version = typeof(App).Assembly.GetName().Version;
        VersionText.Text = version is null ? "ATIS" : $"ATIS Version {version}";

        SelectByTag(LanguageBox, CultureInfo.CurrentUICulture.Name);

        var root = App.MainWindow?.Content as FrameworkElement
                   ?? throw new InvalidOperationException(
                       "Das Hauptfenster ist nicht initialisiert.");

        SelectByTag(ThemeBox, root.RequestedTheme.ToString());

        RefreshServerList();
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

        if (ServerBox.SelectedItem is not string server ||
            string.IsNullOrWhiteSpace(server))
        {
            ConnectionStatusText.Text = "Bitte einen SQL-Server eingeben.";
            ConnectionStatusText.Visibility = Visibility.Visible;
            return;
        }

        TestConnectionButton.IsEnabled = false;
        ServerBox.IsEnabled = false;
        ConnectionStatusText.Text = "Verbindung wird getestet ...";
        ConnectionStatusText.Visibility = Visibility.Visible;

        try
        {
            //var configured = ConfigurationManager.ConnectionStrings["MyDbConnection"]
            //                 ?? throw new ConfigurationErrorsException("MyDbConnection fehlt in App.config.");

            //var builder = new SqlConnectionStringBuilder(configured.ConnectionString)
            //{
            //    DataSource = server,
            //    InitialCatalog = "ATIS34",
            //    ConnectTimeout = 5
            //};

            var builder = new SqlConnectionStringBuilder(
                DbServerSettings.GetConnectionString(server))
            {
                ConnectTimeout = 5
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT CONVERT(nvarchar(128), SERVERPROPERTY('ServerName')), DB_NAME();";
            command.CommandTimeout = 5;

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                throw new InvalidOperationException(
                    "Keine Verbindungsinformationen erhalten.");
            }

            var actualServer = reader.GetString(0);
            var actualDatabase = reader.GetString(1);

            SimpleLog.Info(
                $"Auswahl: {server}; verbunden: {actualServer}; Datenbank: {actualDatabase}");

            if (!string.Equals(actualDatabase, "ATIS34",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Falsche Datenbank: {actualDatabase}");
            }

            _testedServer = server;
            SaveServerButton.IsEnabled = true;

            ConnectionStatusText.Text = $"Verbindung erfolgreich: {actualServer} / {actualDatabase}";
        }
        catch (SqlException ex) when (ex.Number == 4060)
        {
            SimpleLog.Error($"ATIS34 auf {server} nicht zugänglich: {ex.Message}");
            ConnectionStatusText.Text =
                "ATIS34 fehlt auf diesem SQL-Server oder die Zugriffsberechtigung fehlt.";
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

    private void SaveServerButton_Click(object sender, RoutedEventArgs e)
    {
        ConnectionStatusText.Visibility = Visibility.Visible;

        if (_testedServer is null ||
            ServerBox.SelectedItem is not string selectedServer ||
            !string.Equals(_testedServer, selectedServer,
                StringComparison.OrdinalIgnoreCase))
        {
            SaveServerButton.IsEnabled = false;
            ConnectionStatusText.Text =
                "Bitte zuerst die ausgewählte Verbindung erfolgreich testen.";
            return;
        }

        try
        {
            DbServerSettings.Save(
                DbServerSettings.NormalizeDataSource(selectedServer));

            SaveServerButton.IsEnabled = false;
            ConnectionStatusText.Text =
                "SQL-Server gespeichert. Bitte ATIS neu starten.";
        }
        catch (Exception ex) when (ex is System.IO.IOException
                                       or UnauthorizedAccessException
                                       or System.Security.SecurityException)
        {
            SimpleLog.Error($"SQL-Server konnte nicht gespeichert werden: {ex.Message}");
            ConnectionStatusText.Text =
                "Speichern fehlgeschlagen. Details stehen im ATIS-Protokoll.";
        }
    }
    private void ServerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _testedServer = null;
        SaveServerButton.IsEnabled = false;
        ConnectionStatusText.Visibility = Visibility.Collapsed;
    }

    private void RefreshServersButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshServerList();
        SelectByTag(ServerBox, DbServerSettings.DataSource ?? string.Empty);
    }

    private void RefreshServerList()
    {
        try
        {
            var servers = DbServerSettings.GetInstalledDataSources();
            ServerBox.ItemsSource = servers;

            var savedServer = DbServerSettings.DataSource;

            ServerBox.SelectedItem = string.IsNullOrWhiteSpace(savedServer)
                ? null
                : servers.FirstOrDefault(server =>
                    string.Equals(
                        DbServerSettings.NormalizeDataSource(server),
                        DbServerSettings.NormalizeDataSource(savedServer),
                        StringComparison.OrdinalIgnoreCase));

            if (servers.Count == 0)
            {
                ConnectionStatusText.Text =
                    "Auf diesem Rechner wurden keine SQL-Server-Instanzen gefunden.";
                ConnectionStatusText.Visibility = Visibility.Visible;
            }
            else
            {
                ConnectionStatusText.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex) when (ex is IOException
                                       or UnauthorizedAccessException
                                       or System.Security.SecurityException)
        {
            SimpleLog.Error(
                $"Installierte SQL-Server konnten nicht ermittelt werden: {ex.Message}");
            ConnectionStatusText.Text =
                "Die installierten SQL-Server konnten nicht ermittelt werden.";
            ConnectionStatusText.Visibility = Visibility.Visible;
        }
    }
}
