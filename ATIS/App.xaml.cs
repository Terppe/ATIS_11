using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using ATIS.Infrastructure;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using System.Text.Json;
using ATIS.Views.Admin;
using ATIS.Views.Auth;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl03Regnum;
using ATIS.Views.Modules.Tbl06Phylum;
using ATIS.Views.Modules.Tbl12Subphylum;
using ATIS.Views.Modules.Tbl18Superclass;
using ATIS.Views.Modules.Tbl21Class;
using ATIS.Views.Modules.Tbl24Subclass;
using ATIS.Views.Modules.Tbl27Infraclass;
using ATIS.Views.Modules.Tbl30Legio;
using ATIS.Views.Modules.Tbl33Ordo;
using ATIS.Views.Modules.Tbl36Subordo;
using ATIS.Views.Modules.Tbl39Infraordo;
using ATIS.Views.Modules.Tbl42Superfamily;
using ATIS.Views.Modules.Tbl45Family;
using ATIS.Views.Modules.Tbl48Subfamily;
using ATIS.Views.Modules.Tbl51Infrafamily;
using ATIS.Views.Modules.Tbl54Supertribus;
using ATIS.Views.Modules.Tbl57Tribus;
using ATIS.Views.Modules.Tbl60Subtribus;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl68Speciesgroup;
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl78Name;
using ATIS.Views.Modules.Tbl81Image;
using ATIS.Views.Modules.Tbl84Synonym;
using ATIS.Views.Modules.Tbl87Geographic;
using ATIS.Views.Modules.Tbl90RefAuthor;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl90RefExpert;
using ATIS.Views.Modules.Tbl90RefSource;
using ATIS.Views.Modules.Tbl93Comment;
using ATIS.Views.Modules.TblCountry;
using ATIS.Views.Modules.TblDisease;
using ATIS.Views.Modules.TblFood;
using ATIS.Views.Modules.TblMiscellaneous;
using ATIS.Views.Modules.TblUserProfile;
using ATIS.Views.Report;
using ATIS.Views.Settings;
using ATIS.Views.User;
using QuestPDF.Infrastructure;
using WinUI3Localizer;
using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;

namespace ATIS;

public partial class App : Application
{
    // ---------------------------------------------------------
    // 1. FELDER & PROPERTIES
    // ---------------------------------------------------------

    public static Window? MainWindow { get; private set; }
    public static IServiceProvider? Services { get; private set; }
    public static FrameworkElement? MainRoot { get; private set; }
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } =
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
        ?? "unknown";

    public string? AppName { get; set; } = ConfigurationManager.AppSettings.Get("ApplicationName");
    public string? Copyright { get; set; } = ConfigurationManager.AppSettings.Get("Copyright");
    public static string? Cult = "";
    private SplashWindow splash;
    public static event Action<string>? LanguageChanged;
    private static bool _closeConfirmed;
    private static bool _dialogOpen;

    public static MainPage? MainPageInstance { get; private set; }
    public static string AppConfigFolderPath { get; private set; } = string.Empty;
    public static string StringsFolderPath { get; private set; } = string.Empty;
    public static string? SearchQuickString { get; set; } // from MainPage filled
    public static int WindowsCounter { get; set; } = 0;// Count the created and open windows

    // ---------------------------------------------------------
    // 2. KONSTRUKTOR
    // ---------------------------------------------------------

    public App()
    {
        InitializeComponent();

        Services = ConfigureServices();
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.ThrowOnMissingTextGlyphs = false;
        QuestPDF.Settings.UseSystemFonts = false;
        QuestPDF.Settings.EnableDetailedLayoutErrors = false;

        UnhandledException += App_UnhandledException;

#if DEBUG
        AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
        {
            if (e.Exception is System.Net.Sockets.SocketException)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[FirstChance] {e.Exception.GetType().Name}: {e.Exception.Message}");
            }
        };
#endif
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogException("Nicht behandelte Prozess-Ausnahme", exception);
            }
            else
            {
                Debug.WriteLine(
                    $"Nicht behandelte Prozess-Ausnahme: {e.ExceptionObject}");
            }
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            LogException("Nicht beobachtete Task-Ausnahme", e.Exception);
            e.SetObserved();
        };
    }

    // ---------------------------------------------------------
    // 3. ONLAUNCHED – HAUPTSTART
    // ---------------------------------------------------------

    // Window erstellen
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Sprache und Protokollierung initialisieren
        await InitializeLocalizer();
        ApplyCulture();
        InitializeLogging();

        // Startansicht für Datenbankprüfung und Dialoge anzeigen
        MainWindow = new Window { Title = AppName };

        var startupRoot = new Grid();
        startupRoot.Children.Add(new TextBlock
        {
            Text = "SQL-Server und ATIS34 werden geprüft …",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        var loaded = new TaskCompletionSource();
        startupRoot.Loaded += (_, _) => loaded.TrySetResult();

        MainWindow.Content = startupRoot;
        MainWindow.Activate();
        await loaded.Task;

        // Gespeicherten Server prüfen oder eine erreichbare Instanz einrichten
        while (true)
        {
            var savedServer = DbServerSettings.DataSource;

            if (!string.IsNullOrWhiteSpace(savedServer) &&
                await SqlInstanceFinder.CanConnectAsync(savedServer, "ATIS34"))
            {
                break;
            }

            var found = await SqlInstanceFinder.FindReachableAsync("ATIS34");

            if (found.Count == 1)
            {
                DbServerSettings.Save(found[0]);
                break;
            }

            if (found.Count > 1)
            {
                if (await ChooseDbServerAsync(found))
                {
                    break;
                }

                Exit();
                return;
            }

            var result = await new ContentDialog
            {
                XamlRoot = startupRoot.XamlRoot,
                Title = "ATIS34 nicht erreichbar",
                Content =
                    "Bitte prüfen, ob der SQL-Server-Dienst läuft und " +
                    "die Datenbank ATIS34 vorhanden ist. " +
                    "Details stehen im ATIS-Protokoll.",
                PrimaryButtonText = "Erneut prüfen",
                CloseButtonText = "Beenden"
            }.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                Exit();
                return;
            }
        }

        // Benutzerkontext vor dem Erstellen der Hauptansicht setzen
        var customPrincipal = new CustomPrincipal();
        AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

        // Schließen ausschließlich über den zentralen Ablauf behandeln
        MainWindow.AppWindow.Closing += (sender, e) =>
        {
            if (_closeConfirmed)
            {
                return;
            }

            e.Cancel = true;
            _ = RequestCloseAsync();
        };

        // Hauptansicht erst mit eingerichteter Datenbankverbindung laden
        MainPageInstance = new MainPage();
        MainPageInstance.Loaded += (_, _) =>
        {
            UIContext.Root = MainPageInstance.XamlRoot;
        };

        MainWindow.Content = MainPageInstance;
        MainRoot = MainPageInstance;

        // Automatische Updateprüfung nur im Release-Build
#if !DEBUG
    _ = MainPageInstance.CheckForUpdatesAsync(showStatusMessages: false);
#endif

#if !DEBUG
    try
    {
        if (UpdateSettings.Load())
        {
            _ = MainPageInstance.CheckForUpdatesAsync(
                showStatusMessages: false);
        }
    }
    catch (Exception ex) when (ex is IOException
                                   or UnauthorizedAccessException
                                   or System.Security.SecurityException
                                   or JsonException)
    {
        SimpleLog.Error(
            $"Automatische Updateprüfung nicht gestartet: {ex.Message}");
    }
#endif
    }
    // ---------------------------------------------------------
    // 2. CHOOSE SERVER
    // ---------------------------------------------------------

    private async Task<bool> ChooseDbServerAsync(List<string> servers)
    {
        if (MainWindow?.Content is not FrameworkElement root ||
            root.XamlRoot is null)
        {
            throw new InvalidOperationException(
                "Das Fenster für die Serverauswahl ist nicht bereit.");
        }

        var radios = new RadioButtons
        {
            ItemsSource = servers,
            SelectedIndex = 0
        };

        var dialog = new ContentDialog
        {
            XamlRoot = root.XamlRoot,
            Title = "SQL-Server mit ATIS34 auswählen",
            Content = radios,
            PrimaryButtonText = "Übernehmen",
            CloseButtonText = "Abbrechen",
            DefaultButton = ContentDialogButton.Primary
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary ||
            radios.SelectedItem is not string selected)
        {
            return false;
        }

        // Vor dem Speichern die ausgewählte Datenbank erneut prüfen.
        if (!await SqlInstanceFinder.CanConnectAsync(selected, "ATIS34"))
        {
            await new ContentDialog
            {
                XamlRoot = root.XamlRoot,
                Title = "Verbindung fehlgeschlagen",
                Content =
                    $"Die Datenbank ATIS34 auf {selected} ist nicht erreichbar. " +
                    "Der Server wurde nicht gespeichert.",
                CloseButtonText = "Beenden"
            }.ShowAsync();

            return false;
        }

        DbServerSettings.Save(selected);
        return true;
    }

    private void ApplyCulture()
    {
        Cult = ConfigurationManager.AppSettings.Get("Culture");

        if (Cult != null)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Cult);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cult);
        }
    }

    public static void RaiseLanguageChanged(string lang) => LanguageChanged?.Invoke(lang);

    public static void SetCulture(string culture)
    {
        Cult = culture;

        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        Localizer.Get().SetLanguage(culture);
    }

    //---------------------------------------------------------
    //3. Window erstellen
    //---------------------------------------------------------
    public static async Task<bool> ConfirmCloseAsync()
    {
        if (_dialogOpen) return false;
        _dialogOpen = true;
        try
        {
            var loc = Localizer.Get();
            var dialog = GetService<IDialogService>();
            return await dialog.ShowConfirmAsync(
                loc.GetLocalizedString("ConfirmExitTitle"),
                loc.GetLocalizedString("ConfirmExitMessage"));
        }
        catch (InvalidOperationException ex)
        {
            SimpleLog.Error($"Beenden-Dialog nicht anzeigbar: {ex.Message}");
            return false;
        }
        finally
        {
            _dialogOpen = false;
        }
    }

    public static async Task RequestCloseAsync()
    {
        if (!await ConfirmCloseAsync()) return;

        SimpleLog.Info("AppClosed");
        _closeConfirmed = true;
        MainWindow?.DispatcherQueue.TryEnqueue(() => MainWindow.Close());
    }

    // ---------------------------------------------------------
    // 5. CONFICURE SERVICES
    // ---------------------------------------------------------
    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // THEME SERVICE (KORREKT)
     //   services.AddSingleton<IThemeService, ThemeService>();

        // HIERARCHY COORDINATOR - SINGLETON!
        services.AddSingleton<HierarchyCoordinator>();

        // DATA SERVICE
        services.AddDbContextFactory<AtisDbContext>();
        services.AddScoped<IDataService, DataService>();

        // DIALOG SYSTEM SERVICES
        services.AddSingleton<IDialogService, DialogService>();

        // LOOKUP SERVICES
        services.AddScoped<LookupService>();

        // USER SERVICES
        services.AddSingleton<IUserService, UserService>();

        // AUTHENTICATION SERVICES
        services.AddTransient<ATIS.Interfaces.IAuthenticationService, ATIS.Services.AuthenticationService>();

        // VIEWMODELS
        services.AddTransient<MainViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<AuthenticationViewModel>();

        services.AddTransient<Tbl03RegnumViewModel>();
        services.AddTransient<Tbl06PhylumViewModel>();
        services.AddTransient<Tbl12SubphylumViewModel>();
        services.AddTransient<Tbl18SuperclassViewModel>();
        services.AddTransient<Tbl21ClassViewModel>();
        services.AddTransient<Tbl24SubclassViewModel>();
        services.AddTransient<Tbl27InfraclassViewModel>();
        services.AddTransient<Tbl30LegioViewModel>();
        services.AddTransient<Tbl33OrdoViewModel>();
        services.AddTransient<Tbl36SubordoViewModel>();
        services.AddTransient<Tbl39InfraordoViewModel>();
        services.AddTransient<Tbl42SuperfamilyViewModel>();
        services.AddTransient<Tbl45FamilyViewModel>();
        services.AddTransient<Tbl48SubfamilyViewModel>();
        services.AddTransient<Tbl51InfrafamilyViewModel>();
        services.AddTransient<Tbl54SupertribusViewModel>();
        services.AddTransient<Tbl57TribusViewModel>();
        services.AddTransient<Tbl60SubtribusViewModel>();
        services.AddTransient<Tbl63InfratribusViewModel>();
        services.AddTransient<Tbl66GenusViewModel>();
        services.AddTransient<Tbl68SpeciesgroupViewModel>();
        services.AddTransient<Tbl75SpeciesViewModel>();
        services.AddTransient<Tbl78NameViewModel>();
        services.AddTransient<Tbl81ImageViewModel>();
        services.AddTransient<Tbl84SynonymViewModel>();
        services.AddTransient<Tbl87GeographicViewModel>();

        services.AddTransient<Tbl90ReferenceViewModel>();
        services.AddTransient<Tbl90RefAuthorViewModel>();
        services.AddTransient<Tbl90RefExpertViewModel>();
        services.AddTransient<Tbl90RefSourceViewModel>();
        services.AddTransient<Tbl93CommentViewModel>();

        services.AddTransient<TblCountryViewModel>();
        services.AddTransient<TblUserProfileViewModel>();

        services.AddTransient<TblFoodViewModel>();
        services.AddTransient<TblDiseaseViewModel>();
        services.AddTransient<TblMiscellaneousViewModel>();

        services.AddTransient<UserAreaViewModel>();
        services.AddTransient<AdminAreaViewModel>();

        services.AddTransient<ReportViewModel>();


        Services = services.BuildServiceProvider();
        //try
        //{
        //    return (ServiceProvider)Services;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Debug.WriteLine("DI ERROR: " + ex);
        //    throw;
        //}

        return (ServiceProvider)Services;
    }

    // ---------------------------------------------------------
    // 5. LOCALIZATION
    // ---------------------------------------------------------

    private static async Task InitializeLocalizer()
    {
        // Initialize a "Strings" folder in the executables folder.
        StringsFolderPath = Path.Combine(AppContext.BaseDirectory, "Strings");

        if (!Directory.Exists(StringsFolderPath))
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Localizer] Pfad fehlt: {StringsFolderPath}");
            return;
        }

        await new LocalizerBuilder()
            .AddStringResourcesFolderForLanguageDictionaries(StringsFolderPath)

            .SetOptions(options =>
            {
                //    options.DefaultLanguage = "de-DE";
                options.DefaultLanguage = ConfigurationManager.AppSettings.Get("Culture")!;

            })
            .Build();
    }

    // ---------------------------------------------------------
    // 6. LOGGING
    // ---------------------------------------------------------

    private void InitializeLogging()
    {
        var localAppData =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(localAppData))
            throw new InvalidOperationException("Der lokale App-Datenordner ist nicht verfügbar.");

        var logDirectory = Path.Combine(localAppData, "ATIS", "Logs");

        var directoryError = SimpleLog.SetLogDir(logDirectory, createIfNotExisting: true);
        if (directoryError is not null)
            throw directoryError;

        SimpleLog.SetLogFile(
            logDir: logDirectory,
            prefix: $"Atis_Log_{AppVersion}_",
            writeText: true,
            check: false);

        var writeError = SimpleLog.Info(
            "ATIS-Protokollierung initialisiert.",
            useBackgroundTask: false);

        if (writeError is not null)
            throw writeError;
    }

    // ---------------------------------------------------------
    // 7. Splash
    // ---------------------------------------------------------
    private async Task ShowSplashAsync()
    {
        splash = new SplashWindow();
     //   splash.CenterOnScreen();
     //   splash.IsAlwaysOnTop = true;
        splash.Activate();
        await Task.Delay(5000);
        splash.Close();
    }

    // ---------------------------------------------------------
    // 9. UNHANDLED EXCEPTIONS
    // ---------------------------------------------------------

    private void App_UnhandledException(
        object sender,
        Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        LogException("Nicht behandelte UI-Ausnahme", e.Exception);

        // e.Handled bleibt false; der unerwartete Fehler wird nicht verschluckt.
    }

    private static void LogException(string context, Exception exception)
    {
        try
        {
            SimpleLog.Error(context, useBackgroundTask: false);

            var loggingError = SimpleLog.Log(exception, useBackgroundTask: false);

            if (loggingError is not null)
            {
                Debug.WriteLine(
                    $"Protokollieren fehlgeschlagen ({context}): {loggingError}");
            }
        }
        catch (Exception loggingException)
        {
            Debug.WriteLine(
                $"Protokollieren fehlgeschlagen ({context}): {loggingException}");
        }

        Debug.WriteLine($"[{context}] {exception}");
    }    // ---------------------------------------------------------
    // 10. Helper
    // ---------------------------------------------------------

    public static T GetService<T>() where T : class
    {
        if (Services != null)
        {
            return Services.GetRequiredService<T>();
        }
        return null;
    }


}

