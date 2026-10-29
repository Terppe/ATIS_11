using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using ATIS.Infrastructure;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
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
using ATIS.Views.Search;
using ATIS.Views.Settings;
using ATIS.Views.User;
using ATIS.Views.UserControls;
using QuestPDF.Infrastructure;
using WinUI3Localizer;

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
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"[Unhandled] {e.ExceptionObject}");
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"[UnobservedTask] {e.Exception}");
            e.SetObserved();
        };
    }

    // ---------------------------------------------------------
    // 3. ONLAUNCHED – HAUPTSTART
    // ---------------------------------------------------------

    // Window erstellen
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var connected = await new AtisDbContext().Database.CanConnectAsync();
        System.Diagnostics.Debug.WriteLine($"[DB] connected={connected}");


        // 1. Localizer EINMAL initialisieren
        await InitializeLocalizer();

        // 2. Culture setzen
        ApplyCulture();

        // 3. Window erstellen
        MainWindow = new Window { Title = AppName };

        MainWindow.AppWindow.Closing += (sender, e) =>
        {
            if (_closeConfirmed) return;
            e.Cancel = true;
            _ = RequestCloseAsync();
        };

        // 4. MainPage setzen
        MainPageInstance = new MainPage();
        MainWindow.Content = MainPageInstance;
        MainRoot = MainWindow.Content as FrameworkElement;

        // 5 ThemeService
        if (Services != null)
        {
            //var themeService = Services.GetService<IThemeService>();
            //themeService?.AutoInitialize(MainWindow);
        }

        // 6 Logging 
        InitializeLogging();

        // 7 Splash 
        //solange entwickelt wird // danach     await ShowSplashAsync();


        MainWindow.Activate();


        UIContext.Root = (MainWindow.Content as FrameworkElement)?.XamlRoot;

        var customPrincipal = new CustomPrincipal();

        AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

        //Update check on startup
        _ = MainPageInstance.CheckForUpdatesAsync(showStatusMessages: false);
    }

    // ---------------------------------------------------------
    // 2. CULTURE
    // ---------------------------------------------------------
    private void ApplyCulture()
    {
        Cult = ConfigurationManager.AppSettings.Get("Culture");

        Thread.CurrentThread.CurrentCulture = new CultureInfo(Cult);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cult);
    }

    public static void RaiseLanguageChanged(string lang)
        => LanguageChanged?.Invoke(lang);

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
    // Centralized app-close flow (WinUI 3)
    // ------------------------------------
    // Why this exists:
    // - We had shutdown crashes (0xC000027B / 0xC0000005) caused by multiple close paths
    //   and dialog/localization interactions during teardown.
    // - Therefore, close handling is centralized in App.xaml.cs.
    //
    // Rules:
    // 1) Exactly ONE AppWindow.Closing handler in the whole app (registered in App).
    // 2) All user-initiated close actions (menu, commands, etc.) call App.RequestCloseAsync().
    // 3) Do not show additional close dialogs in pages/viewmodels.
    // 4) Use _dialogOpen to prevent re-entrant/double dialogs.
    // 5) Use _closeConfirmed to allow the final close without re-prompt.
    //
    // Notes:
    // - Window.Close() from menu may bypass the same UX path as titlebar close.
    //   Always use RequestCloseAsync() for consistent behavior.
    // - Keep this logic in App (not MainPage) to avoid duplicate subscriptions from Loaded events.
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
        services.AddTransient<SearchViewModel>();

        services.AddTransient<SpeciesViewModel>();
        services.AddTransient<UtilityViewModel>();
        services.AddTransient<GeneralSettingViewModel>();
        services.AddTransient<ThemeSettingViewModel>();
        services.AddTransient<AppUpdateSettingViewModel>();
        services.AddTransient<GitUpdateSettingViewModel>();
        services.AddTransient<AboutUsSettingViewModel>();

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
        SimpleLog.SetLogDir("C:\\Temp", true);
        SimpleLog.SetLogFile(
            logDir: "C:\\Temp\\LogAtis",
            prefix: $"Atis_Log_{AppVersion}_",
            writeText: true);
        SimpleLog.Info("ATIS Application started");
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

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // Wir verhindern App-Absturz
        e.Handled = true;

        // Logging
        LogException(e.Exception);
    }
    private void LogException(Exception ex)
    {
        Debug.WriteLine("========================================");
        Debug.WriteLine($"🔥 UNHANDLED EXCEPTION [{DateTime.Now:HH:mm:ss}]");
        Debug.WriteLine("========================================");

        while (ex != null)
        {
            Debug.WriteLine($"➡ {ex.GetType().Name}: {ex.Message}");
            Debug.WriteLine(ex.StackTrace);
            ex = ex.InnerException;
            if (ex != null)
                Debug.WriteLine("---- INNER EXCEPTION ----");
        }

        Debug.WriteLine("========================================");
    }
    // ---------------------------------------------------------
    // 10. Helper
    // ---------------------------------------------------------

    public static T GetService<T>() where T : class
    {
        return Services.GetRequiredService<T>();
    }

    //public static IThemeService GetThemeService()
    //{
    //    return Services.GetRequiredService<IThemeService>();
    //}

}

