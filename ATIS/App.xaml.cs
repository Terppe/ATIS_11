using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using ATIS.Infrastructure;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Admin;
using ATIS.Views.Authentication;
using ATIS.Views.Database;
using ATIS.Views.Main;
using ATIS.Views.Main.Disease;
using ATIS.Views.Main.Food;
using ATIS.Views.Main.Miscellaneous;
using ATIS.Views.Report;
using ATIS.Views.Search;
using ATIS.Views.Settings;
using ATIS.Views.User;
using ATIS.Views.UserControls;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Documents;
using QuestPDF.Infrastructure;
using WinUI3Localizer;
using WinUIEx;
using DiseasePage = ATIS.Views.Main.Disease.DiseasePage;
using FoodPage = ATIS.Views.Main.Food.FoodPage;
using MiscellaneousPage = ATIS.Views.Main.Miscellaneous.MiscellaneousPage;

namespace ATIS;

public partial class App : Application
{
    // ---------------------------------------------------------
    // 1. FELDER & PROPERTIES
    // ---------------------------------------------------------

    public static Window MainWindow { get; private set; }
    public static IServiceProvider Services { get; private set; }
    public static FrameworkElement MainRoot { get; private set; }
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    public string AppName { get; set; } = ConfigurationManager.AppSettings.Get("ApplicationName");
    public string Copyright { get; set; } = ConfigurationManager.AppSettings.Get("Copyright");

    public static string Cult = "";

    private SplashWindow splash;
    // ---------------------------------------------------------
    // 2. KONSTRUKTOR
    // ---------------------------------------------------------

    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
        QuestPDF.Settings.License = LicenseType.Community;
        UnhandledException += App_UnhandledException;
    }

    // ---------------------------------------------------------
    // 3. ONLAUNCHED – HAUPTSTART
    // ---------------------------------------------------------

    // Window erstellen
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // 1. Localizer EINMAL initialisieren
        await InitializeLocalizer();

        // 2. Culture setzen
        ApplyCulture();

        // 2. Window erstellen
        MainWindow = new Window
        {
            Title = AppName
        };

        // 3. MainPage setzen
        MainWindow.Content = new MainPage();
        MainRoot = MainWindow.Content as FrameworkElement;

        // 4 ThemeService
        var themeService = Services.GetService<IThemeService>();
        themeService?.AutoInitialize(MainWindow);

        // 5  


        // 6 Logging 
        InitializeLogging();

        // 7 Splash 
        await ShowSplashAsync();

        // 8 Window aktivieren
        MainWindow.Activate();


        UIContext.Root = (MainWindow.Content as FrameworkElement)?.XamlRoot;

        var customPrincipal = new CustomPrincipal();
        AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);


    }


    // ---------------------------------------------------------
    // 4. DI KONFIGURATION
    // ---------------------------------------------------------

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // THEME SERVICE (KORREKT)
        services.AddSingleton<WinUICommunity.IThemeService, WinUICommunity.ThemeService>();

        // DATABASE + DATA SERVICE
        services.AddDbContext<AtisDbContext>();
        services.AddScoped<IDataService, DataService>();

        // VIEWMODELS
        services.AddTransient<MainViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<AuthenticationViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<DatabaseViewModel>();
        services.AddTransient<SpeciesViewModel>();
        services.AddTransient<UtilityViewModel>();
        services.AddTransient<GeneralSettingViewModel>();
        services.AddTransient<ThemeSettingViewModel>();
        services.AddTransient<AppUpdateSettingViewModel>();
        services.AddTransient<GitUpdateSettingViewModel>();
        services.AddTransient<AboutUsSettingViewModel>();
        services.AddTransient<BreadCrumbBarViewModel>();

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
 //       services.AddTransient<TblCountryViewModel>();
        services.AddSingleton<TblCountryViewModel>();
        services.AddTransient<TblUserProfileViewModel>();
        services.AddTransient<FoodViewModel>();
        services.AddTransient<TblFoodViewModel>();
        services.AddTransient<TblDiseaseViewModel>();
        services.AddTransient<DiseaseViewModel>();
        services.AddTransient<TblMiscellaneousViewModel>();
        services.AddTransient<MiscellaneousViewModel>();
        services.AddTransient<UserAreaViewModel>();
        services.AddTransient<AdminAreaViewModel>();
        services.AddTransient<ReportViewModel>();

        Services = services.BuildServiceProvider();
        return (ServiceProvider)Services;
    }

    // ---------------------------------------------------------
    // 5. LOCALIZATION
    // ---------------------------------------------------------

    private static async Task InitializeLocalizer()
    {
        // Initialize a "Strings" folder in the executables folder.
        StringsFolderPath = Path.Combine(AppContext.BaseDirectory, "Strings");
        _ = await new LocalizerBuilder()
            .AddStringResourcesFolderForLanguageDictionaries(StringsFolderPath)
            //later to use for DataGridTemplateColumn, DataGridTextColumn, DataGridComboBoxColumn, DataGridCheckBoxColumn
            .AddLocalizationAction(
                new LocalizationActions.ActionItem(
                    typeof(DataGridColumn),
                    arguments =>
                    {
                        if (arguments.DependencyObject is DataGridColumn target)
                        {
                            target.Header = arguments.Value;
                        }
                    }))

            .AddLocalizationAction(new LocalizationActions.ActionItem(
                typeof(DataGridTemplateColumn),
                arguments =>
                {
                    if (arguments.DependencyObject is DataGridTemplateColumn target)
                    {
                        target.Header = arguments.Value;
                    }
                }))
            .AddLocalizationAction(new LocalizationActions.ActionItem(
                typeof(DataGridTextColumn),
                arguments =>
                {
                    if (arguments.DependencyObject is DataGridTextColumn target)
                    {
                        target.Header = arguments.Value;
                    }
                }))
            .AddLocalizationAction(new LocalizationActions.ActionItem(
                typeof(DataGridComboBoxColumn),
                arguments =>
                {
                    if (arguments.DependencyObject is DataGridComboBoxColumn target)
                    {
                        target.Header = arguments.Value;
                    }
                }))
            .AddLocalizationAction(new LocalizationActions.ActionItem(
                typeof(DataGridCheckBoxColumn),
                arguments =>
                {
                    if (arguments.DependencyObject is DataGridCheckBoxColumn target)
                    {
                        target.Header = arguments.Value;
                    }
                }))
            .AddLocalizationAction(
                new LocalizationActions.ActionItem(
                    typeof(Run),
                    arguments =>
                    {
                        if (arguments.DependencyObject is Run target)
                        {
                            target.Text = arguments.Value;
                        }
                    }))
            .AddLocalizationAction(
                new LocalizationActions.ActionItem(
                    typeof(Hyperlink),
                    arguments =>
                    {
                        if (arguments.DependencyObject is Hyperlink target && target.Inlines.Count is 0)
                        {
                            target.Inlines.Clear();
                            target.Inlines.Add(new Run() { Text = arguments.Value });
                        }
                    }))
            .SetOptions(options =>
            {
                //    options.DefaultLanguage = "de-DE";
                options.DefaultLanguage = ConfigurationManager.AppSettings.Get("Culture")!;

            })
            .Build();
    }

    //private static async Task InitializeLocalizer()  //WinUI3Localizer
    //{
    //    // Initialize a "Strings" folder in the executables folder.
    //    StringsFolderPath = Path.Combine(AppContext.BaseDirectory, "Strings");
    //    _ = await new LocalizerBuilder()
    //        .AddStringResourcesFolderForLanguageDictionaries(StringsFolderPath)
    //    //later to use for DataGridTemplateColumn, DataGridTextColumn, DataGridComboBoxColumn, DataGridCheckBoxColumn
    //.AddLocalizationAction(
    //new LocalizationActions.ActionItem(
    //    typeof(DataGridColumn),
    //    arguments =>
    //    {
    //        if (arguments.DependencyObject is DataGridColumn target)
    //        {
    //            target.Header = arguments.Value;
    //        }
    //    }))
    //        .AddLocalizationAction(new LocalizationActions.ActionItem(
    //            typeof(DataGridTemplateColumn),
    //            arguments =>
    //            {
    //                if (arguments.DependencyObject is DataGridTemplateColumn target)
    //                {
    //                    target.Header = arguments.Value;
    //                }
    //            }))
    //        .AddLocalizationAction(new LocalizationActions.ActionItem(
    //            typeof(DataGridTextColumn),
    //            arguments =>
    //            {
    //                if (arguments.DependencyObject is DataGridTextColumn target)
    //                {
    //                    target.Header = arguments.Value;
    //                }
    //            }))
    //        .AddLocalizationAction(new LocalizationActions.ActionItem(
    //            typeof(DataGridComboBoxColumn),
    //            arguments =>
    //            {
    //                if (arguments.DependencyObject is DataGridComboBoxColumn target)
    //                {
    //                    target.Header = arguments.Value;
    //                }
    //            }))
    //        .AddLocalizationAction(new LocalizationActions.ActionItem(
    //            typeof(DataGridCheckBoxColumn),
    //            arguments =>
    //            {
    //                if (arguments.DependencyObject is DataGridCheckBoxColumn target)
    //                {
    //                    target.Header = arguments.Value;
    //                }
    //            }))
    //        .AddLocalizationAction(
    //            new LocalizationActions.ActionItem(
    //                typeof(Run),
    //                arguments =>
    //                {
    //                    if (arguments.DependencyObject is Run target)
    //                    {
    //                        target.Text = arguments.Value;
    //                    }
    //                }))
    //        .AddLocalizationAction(
    //            new LocalizationActions.ActionItem(
    //                typeof(Hyperlink),
    //                arguments =>
    //                {
    //                    if (arguments.DependencyObject is Hyperlink target && target.Inlines.Count is 0)
    //                    {
    //                        target.Inlines.Clear();
    //                        target.Inlines.Add(new Run() { Text = arguments.Value });
    //                    }
    //                }))
    //        .SetOptions(options =>
    //        {
    //            //    options.DefaultLanguage = "de-DE";
    //            options.DefaultLanguage = ConfigurationManager.AppSettings.Get("Culture")!;

    //        })
    //        .Build();
    //}

    // ---------------------------------------------------------
    // 6. SPLASHSCREEN
    // ---------------------------------------------------------
    private async Task ShowSplashAsync()
    {
        splash = new SplashWindow();
        splash.CenterOnScreen();
     //   splash.IsAlwaysOnTop = true;
        splash.Activate();
        await Task.Delay(5000);
        splash.Close();
    }

    // ---------------------------------------------------------
    // 7. CULTURE
    // ---------------------------------------------------------
    private void ApplyCulture()
    {
        Cult = ConfigurationManager.AppSettings.Get("Culture");

        Thread.CurrentThread.CurrentCulture = new CultureInfo(Cult);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cult);
    }

    // ---------------------------------------------------------
    // 8. LOGGING
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
    // 9. UNHANDLED EXCEPTIONS
    // ---------------------------------------------------------

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;

        Debug.WriteLine("🔥 UNHANDLED EXCEPTION:");
        Debug.WriteLine(e.Exception?.Message);
        Debug.WriteLine(e.Exception?.StackTrace);
        System.Diagnostics.Debug.WriteLine("UNHANDLED: " + e.Exception);
    }

    // ---------------------------------------------------------
    // 10. Helper
    // ---------------------------------------------------------

    public static T GetService<T>() where T : class
    {
        return Services.GetRequiredService<T>();
    }

    public static IThemeService GetThemeService()
    {
        return Services.GetRequiredService<IThemeService>();
    }

    public static void SetCulture(string culture)
    {
        Cult = culture;

        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        Localizer.Get().SetLanguage(culture);
    }



    public static string AppConfigFolderPath { get; private set; } = string.Empty;
    public static string StringsFolderPath { get; private set; } = string.Empty;
    public static string? SearchQuickString { get; set; } // from MainPage filled
    public static int WindowsCounter { get; set; } = 0;// Count the created and open windows

    //public static IServiceProvider? Services { get; private set; }
    //public static WinUICommunity.IThemeService GetThemeService
    //    => GetService<WinUICommunity.IThemeService>();

    //public static IDataService? StaticDataService => Services.GetService<IDataService>();


    //public static WindowEx MainWindow = new();
    //public static Window MainWindow { get; private set; }

    //public new static App Current => (App)Application.Current;
    //public string AppVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    //public string AppName { get; set; } = ConfigurationManager.AppSettings.Get("ApplicationName");
    //public string Copyright { get; set; } = ConfigurationManager.AppSettings.Get("Copyright");


    //public static string? SearchQuickString { get; set; } // from MainPage filled
    ////public static FrameworkElement MainRoot { get; set; }
    //public static int WindowsCounter { get; set; } = 0;// Count the created and open windows





    //  private static readonly AtisDbContext Context = new();


    //protected override async void OnLaunched(LaunchActivatedEventArgs args)
    //{
    //    try
    //    {
    //        // 1️⃣ DI INITIALISIEREN – GANZ OBEN!
    //        Services = ConfigureServices();

    //        // 2️⃣ Fenster erstellen
    //        //MainWindow = new WindowEx();
    //        //MainWindow.IsContentPresenter = true;

    //        //System.Diagnostics.Debug.WriteLine("CONTENT TYPE: " + MainWindow.Content?.GetType().Name);

    //        //MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
    //        //MainWindow.AppWindow.SetIcon("Assets/icon.ico");

    //        //MainWindow.Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MainHeight"));
    //        //MainWindow.Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MainWidth"));
    //        //MainWindow.CenterOnScreen();
    //        //MainWindow.IsAlwaysOnTop = false;
    //        //MainWindow.IsTitleBarVisible = true;

    //        MainWindow = new Window();
    //        MainWindow.Title = ProcessInfoHelper.ProductNameAndVersion;

    //        MainWindow.Content = new MainPage();

    //        await InitializeLocalizer();

    //        SimpleLog.SetLogDir("C:\\Temp", true);
    //        SimpleLog.SetLogFile(logDir: "C:\\Temp\\LogAtis", prefix: "Atis_Log_" + AppVersion + "_", writeText: true);
    //        SimpleLog.Info(" Started ATIS Application ");



    //        // 3️⃣ Jetzt ist DI bereit → ThemeService funktioniert
    //        GetThemeService?.AutoInitialize(MainWindow);

    //        MainRoot = MainWindow.Content as FrameworkElement;




    //        splash_screen = new SplashWindow();
    //        splash_screen.CenterOnScreen();
    //        splash_screen.IsAlwaysOnTop = true;
    //        splash_screen.Activate();

    //        await Task.Delay(7000);
    //        MainWindow.Activate();

    //        splash_screen.Close();

    //        var customPrincipal = new CustomPrincipal();
    //        AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

    //        Cult = ConfigurationManager.AppSettings.Get("Culture");
    //        Thread.CurrentThread.CurrentCulture = new CultureInfo(Cult ?? string.Empty);
    //        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cult ?? string.Empty);
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

    //public static class AppData
    //{
    //    public static IDataService Data => App.StaticDataService;
    //}

    //public XamlRoot XamlRoot { get; set; }





}

