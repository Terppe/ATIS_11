using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using ATIS.Infrastructure;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Admin;
using ATIS.Views.Authentication;
using ATIS.Views.Database;
using ATIS.Views.Main;
using ATIS.Views.Main.Disease;
using ATIS.Views.Main.Food;
using ATIS.Views.Main.Miscellaneous;
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
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl78Name;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl90RefExpert;
using ATIS.Views.Modules.Tbl90RefSource;
using ATIS.Views.Modules.Tbl93Comment;
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
using Tbl60SubtribusViewModel = ATIS.Views.Modules.Tbl60Subtribus.Tbl60SubtribusViewModel;
using Tbl63InfratribusViewModel = ATIS.Views.Modules.Tbl63Infratribus.Tbl63InfratribusViewModel;
using Tbl90RefAuthorViewModel = ATIS.Views.Modules.Tbl90RefAuthor.Tbl90RefAuthorViewModel;
using Tbl90ReferenceViewModel = ATIS.Views.Modules.Tbl90Reference.Tbl90ReferenceViewModel;
using Tbl93CommentViewModel = ATIS.Views.Modules.Tbl93Comment.Tbl93CommentViewModel;
using TblCountryAltViewModel = ATIS.ViewModels.TblCountryAltViewModel;

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

    public static string? Cult = "";

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
        //solange entwickelt wird     await ShowSplashAsync();

        // 8 Close Application
        Close();

        MainWindow.Activate();


        UIContext.Root = (MainWindow.Content as FrameworkElement)?.XamlRoot;

        var customPrincipal = new CustomPrincipal();
        AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);



    }

    //---------------------------------------------------------
    // 3. CLOSE
    //---------------------------------------------------------
    private static void Close()
    {
        var closing = false;

        App.MainWindow.Closed += async (se, ee) =>
        {
            if (closing)
            {
                return;
            }

            closing = true;
            ee.Handled = true;

            var dialog = new ContentDialog
            {
                XamlRoot = MainWindow.Content.XamlRoot,
                Title = StringsRes.WantToCloseApp,
                PrimaryButtonText = StringsRes.Yes,
                CloseButtonText = StringsRes.No,
                DefaultButton = ContentDialogButton.Close
            };
            //      dialog.PrimaryButtonClick += (s, e) => DispatcherQueue.TryEnqueue(dialog.Hide);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
            {
                closing = false;
            }
            else
            {
                dialog.Hide();
                // to write in App.config not possible in unpacked version find solution
                //SaveCulture(App.Cult);

                // Write info message to log
                SimpleLog.Info(StringsRes.AppClosed);

                // Close this Application
                App.MainWindow.Close();

                //   Process.GetCurrentProcess().Kill();
                Application.Current.Exit();
            }
        };
        //return closing;
    }

    //private static void Close()
    //{
    //    var closing = false;

    //    App.MainWindow.Closed += async (se, ee) =>
    //    {
    //        if (closing)
    //        {
    //            return;
    //        }

    //        closing = true;
    //        ee.Handled = true;

    //        var dialog = new ContentDialog
    //        {
    //            XamlRoot = MainWindow.Content.XamlRoot,
    //            Title = "StringsRes.WantToCloseApp",
    //            PrimaryButtonText = StringsRes.Yes,
    //            CloseButtonText = StringsRes.No,
    //            DefaultButton = ContentDialogButton.Close
    //        };
    //        //      dialog.PrimaryButtonClick += (s, e) => DispatcherQueue.TryEnqueue(dialog.Hide);
    //        var result = await dialog.ShowAsync();
    //        if (result == ContentDialogResult.None)
    //        {
    //            closing = false;
    //        }
    //        else
    //        {
    //            dialog.Hide();
    //            // to write in App.config not possible in unpacked version find solution
    //            //SaveCulture(App.Cult);

    //            // Write info message to log
    //            SimpleLog.Info(StringsRes.AppClosed);

    //            // Close this Application
    //            App.MainWindow.Close();

    //            //   Process.GetCurrentProcess().Kill();
    //            Application.Current.Exit();
    //        }
    //    };
    //    //return closing;
    //}

    // ---------------------------------------------------------
    // 4. DI KONFIGURATION
    // ---------------------------------------------------------

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // THEME SERVICE (KORREKT)
        services.AddSingleton<WinUICommunity.IThemeService, WinUICommunity.ThemeService>();

        // DATABASE + DATA SERVICE
      //  services.AddDbContext<AtisDbContext>();

        // HIERARCHY COORDINATOR - SINGLETON!
        services.AddSingleton<HierarchyCoordinator>();

        // DATA SERVICE
        services.AddDbContextFactory<AtisDbContext>();
        //   services.AddScoped<IDataService, DataService>();

        // DIALOG SYSTEM SERVICES
        services.AddSingleton<IDialogService, DialogService>();

        // LOOKUP SERVICES
        services.AddScoped<LookupService>();

        // USER SERVICES
        services.AddScoped<IUserService, UserService>();

        // VIEWMODELS
        services.AddTransient<TblCountryListTestPage>();
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

        services.AddTransient<Tbl03RegnumAltViewModel>();
        services.AddTransient<Tbl06PhylumAltViewModel>();
        services.AddTransient<Tbl12SubphylumAltViewModel>();
        services.AddTransient<Tbl18SuperclassAltViewModel>();
        services.AddTransient<Tbl21ClassAltViewModel>();
        services.AddTransient<Tbl24SubclassAltViewModel>();
        services.AddTransient<Tbl27InfraclassAltViewModel>();
        services.AddTransient<Tbl30LegioAltViewModel>();
        services.AddTransient<Tbl33OrdoAltViewModel>();
        services.AddTransient<Tbl36SubordoAltViewModel>();
        services.AddTransient<Tbl39InfraordoAltViewModel>();
        services.AddTransient<Tbl42SuperfamilyAltViewModel>();
        services.AddTransient<Tbl45FamilyAltViewModel>();
        services.AddTransient<Tbl48SubfamilyAltViewModel>();
        services.AddTransient<Tbl51InfrafamilyAltViewModel>();
        services.AddTransient<Tbl54SupertribusAltViewModel>();
        services.AddTransient<Tbl57TribusAltViewModel>();

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
        services.AddTransient<Tbl75SpeciesViewModel>();
        services.AddTransient<Tbl78NameViewModel>();

        services.AddTransient<Tbl66GenusAltViewModel>();
        services.AddTransient<Tbl68SpeciesgroupAltViewModel>();
        services.AddTransient<Tbl75SpeciesAltViewModel>();
        services.AddTransient<Tbl78NameAltViewModel>();
        services.AddTransient<Tbl81ImageAltViewModel>();
        services.AddTransient<Tbl84SynonymAltViewModel>();
        services.AddTransient<Tbl87GeographicAltViewModel>();

        services.AddTransient<Tbl90ReferenceViewModel>();
        services.AddTransient<Tbl90RefAuthorViewModel>();
        services.AddTransient<Tbl90RefExpertViewModel>();
        services.AddTransient<Tbl90RefSourceViewModel>();
        services.AddTransient<Tbl93CommentViewModel>();

        services.AddTransient<TblCountryAltViewModel>();
        services.AddTransient<TblUserProfileAltViewModel>();
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
            //.AddLocalizationAction(new LocalizationActions.ActionItem(
            //    typeof(ComboBox),
            //    arguments =>
            //    {
            //        if (arguments.DependencyObject is ComboBox target)
            //        {
            //            target.Header = arguments.Value;
            //        }
            //    }))
            .SetOptions(options =>
            {
                //    options.DefaultLanguage = "de-DE";
                options.DefaultLanguage = ConfigurationManager.AppSettings.Get("Culture")!;

            })
            .Build();
    }

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

    // MainWindow.xaml.cs

    public static string AppConfigFolderPath { get; private set; } = string.Empty;
    public static string StringsFolderPath { get; private set; } = string.Empty;
    public static string? SearchQuickString { get; set; } // from MainPage filled
    public static int WindowsCounter { get; set; } = 0;// Count the created and open windows

}

