using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using Mapsui.Utilities;
using Microsoft.Data.SqlClient;
using Windows.ApplicationModel.DataTransfer;

namespace ATIS.Views.Settings;

public sealed partial class SettingsPage : Page
{
    private string? _testedServer;
    private bool _savedAutomaticUpdates;
    private bool _restoringUpdateSetting;

    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentValues();
        LanguageBox.SelectionChanged += LanguageBox_SelectionChanged;
        ThemeBox.SelectionChanged += ThemeBox_SelectionChanged;
        TestConnectionButton.Click += TestConnectionButton_Click;
        TestConnectionButton.IsEnabled = true;
        SaveServerButton.Click += SaveServerButton_Click;
        LoadUpdateSettings();
    }

    private void LoadCurrentValues()
    {
        var version = typeof(App).Assembly.GetName().Version;
      //  VersionText.Text = version is null ? "ATIS" : $"ATIS Version {version}";
        VersionText.Text = $"ATIS Version " + BuildInfo.Text;
        Info.Text = $"ATIS = Aquaristik Taxonomie Information System mit Bildern und  Videos in 4 Sprachen.\r\n       " +
                    $"Auf der Main Page soll ein Suchfeld sein, in dem man suchen  kann ohne angemeldet zu sein.\r\n  " +
                    $"Um Eingaben in der vorhandenen Datenbank ATIS34, oder Einstellungen zu machen,\r\n              " +
                    $"muss man sich einloggen als Administrator, Developer, Zoologe oder Biologe.\r\n                 " +
                    $"MSSQLSERVER 2025 Als User kann man sich auch anmelden,\r\n                           " +
                    $"kann aber nicht auf die wichtigen Bereiche zugreifen.\r\n                           " +
                    $"Das Ergebnis der Suche holt Daten aus fast allen Tabellen der Datenbank und zeigt sie mit   " +
                    $"Bildern und Videos an.\r\n                           " +
                    $"Dies kann in PDF ausgedruckt werden. Entwurfsmuster MVVM zur Trennung von " +
                    $"Darstellung und Logik der UI (aber nicht generell)\r\n                           " +
                    $"Weitere Informationen und Links anzeigen über alles Wissenswerte rund um Lebewesen und    " +
                    $"Aquaristik.\r\n                           Moderne Oberfläche ähnlich WinUi3 wie Fluent. " +
                    $"Bilder und Videos werden im Ordner DB_FS_ATIS34 als File stream gespeichert.\r\n                 " +
                    $"  http://www.terppe.com  "  ;

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

            DbServerSettings.Save(_testedServer);
            RefreshRestartNotice();

            SaveServerButton.IsEnabled = false;
            ConnectionStatusText.Text = "SQL-Server gespeichert.";
            ConnectionStatusText.Visibility = Visibility.Visible;
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

    private void RefreshRestartNotice()
    {
        RestartRequiredText.Visibility = DbServerSettings.RestartRequired
            ? Visibility.Visible
            : Visibility.Collapsed;
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
    private void LoadUpdateSettings()
    {
        try
        {
            _savedAutomaticUpdates = UpdateSettings.Load();
            AutomaticUpdates.IsOn = _savedAutomaticUpdates;
            AutomaticUpdates.Toggled += AutomaticUpdates_Toggled;
        }
        catch (Exception ex) when (ex is IOException
                                       or UnauthorizedAccessException
                                       or System.Security.SecurityException
                                       or JsonException)
        {
            AutomaticUpdates.IsEnabled = false;
            SimpleLog.Error($"Update-Einstellungen nicht lesbar: {ex.Message}");
            UpdateStatusText.Text =
                "Update-Einstellungen konnten nicht geladen werden.";
            UpdateStatusText.Visibility = Visibility.Visible;
        }
    }

    private void AutomaticUpdates_Toggled(object sender, RoutedEventArgs e)
    {
        if (_restoringUpdateSetting)
            return;

        try
        {
            UpdateSettings.Save(AutomaticUpdates.IsOn);
            _savedAutomaticUpdates = AutomaticUpdates.IsOn;
            UpdateStatusText.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex) when (ex is IOException
                                       or UnauthorizedAccessException
                                       or System.Security.SecurityException)
        {
            _restoringUpdateSetting = true;
            try
            {
                AutomaticUpdates.IsOn = _savedAutomaticUpdates;
            }
            finally
            {
                _restoringUpdateSetting = false;
            }

            SimpleLog.Error($"Update-Einstellungen nicht gespeichert: {ex.Message}");
            UpdateStatusText.Text =
                "Die Einstellung konnte nicht gespeichert werden.";
            UpdateStatusText.Visibility = Visibility.Visible;
        }
    }

    private async void CheckUpdatesButton_Click(object sender, RoutedEventArgs e)
    {
        if (App.MainPageInstance is not { } mainPage)
        {
            UpdateStatusText.Text = "Die Hauptansicht ist nicht verfügbar.";
            UpdateStatusText.Visibility = Visibility.Visible;
            return;
        }

        CheckUpdatesButton.IsEnabled = false;

        try
        {
            await mainPage.CheckForUpdatesAsync(showStatusMessages: true);
        }
        finally
        {
            CheckUpdatesButton.IsEnabled = true;
        }
    }

    private void ShowLogFileButton_Click(object sender, RoutedEventArgs e)
    {
        DiagnosticsStatusText.Visibility = Visibility.Visible;

        try
        {
            SimpleLog.Flush();
            var date = DateTime.Now;

            if (!SimpleLog.LogFileExists(date))
            {
                DiagnosticsStatusText.Text =
                    "Für heute ist keine Protokolldatei vorhanden.";
                return;
            }

            SimpleLog.ShowLogFile(date);
            DiagnosticsStatusText.Text =
                "Das Öffnen des aktuellen Protokolls wurde angefordert.";
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException or
                System.Security.SecurityException or
                System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            SimpleLog.Error("Protokolldatei konnte nicht geöffnet werden.");
            SimpleLog.Log(ex);

            DiagnosticsStatusText.Text =
                "Das Protokoll konnte nicht geöffnet werden. " +
                "Bitte prüfen Sie das Standardprogramm für Protokolldateien.";
        }
    }
    private void CopyDiagnosticsButton_Click(object sender, RoutedEventArgs e)
    {
        DiagnosticsStatusText.Visibility = Visibility.Visible;

        try
        {
            var assembly = typeof(App).Assembly;
            var version = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion
                ?? assembly.GetName().Version?.ToString()
                ?? "Unbekannt";

            var activeServer = DbServerSettings.ActiveDataSource;
            var savedServer = DbServerSettings.DataSource;

            var restartRequired = activeServer is not null &&
                !string.Equals(
                    activeServer,
                    savedServer,
                    StringComparison.OrdinalIgnoreCase);

            var text = string.Join(Environment.NewLine, new[]
            {
            "ATIS – Diagnoseinformationen",
            $"Zeitpunkt: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}",
            $"ATIS-Version: {version}",
            $"Betriebssystem: {RuntimeInformation.OSDescription}",
            $"Prozessarchitektur: {RuntimeInformation.ProcessArchitecture}",
            $".NET: {RuntimeInformation.FrameworkDescription}",
            $"Aktiver SQL-Server: {activeServer ?? "Nicht festgelegt"}",
            $"Gespeicherter SQL-Server: {savedServer ?? "Nicht festgelegt"}",
            "Datenbank: ATIS34",
            $"Serverwechsel nach Neustart: {(restartRequired ? "Ja" : "Nein")}",
            "Verbindungsstatus: Nicht geprüft"
        });

            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);

            DiagnosticsStatusText.Text =
                "Diagnoseinformationen kopiert. Sie können diese jetzt beispielsweise in eine E-Mail einfügen.";
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException or
                System.Security.SecurityException or JsonException or
                COMException or InvalidOperationException or ArgumentException)
        {
            SimpleLog.Error("Diagnoseinformationen kopieren fehlgeschlagen.");
            SimpleLog.Log(ex);

            DiagnosticsStatusText.Text =
                "Diagnoseinformationen konnten nicht kopiert werden. Details stehen im ATIS-Protokoll.";
        }
    }
}
