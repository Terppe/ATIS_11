using System.Configuration;
using System.Globalization;
using System.Reflection;
using ATIS.Helpers;
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

namespace ATIS;

public partial class App : Application
{
    public static string Cult = "";
    public static string AppConfigFolderPath { get; private set; } = string.Empty;
    public static string StringsFolderPath { get; private set; } = string.Empty;

    private static async Task InitializeLocalizer()  //WinUI3Localizer
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

    public static WindowEx MainWindow = new();
    public IServiceProvider Services { get; }
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    public string AppName { get; set; } = ConfigurationManager.AppSettings.Get("ApplicationName");
    public string Copyright { get; set; } = ConfigurationManager.AppSettings.Get("Copyright");

    public static IJsonNavigationViewService GetJsonNavigationViewService => GetService<IJsonNavigationViewService>();
    public static IThemeService GetThemeService => GetService<IThemeService>();
    public static string SearchQuickString { get; set; } // from MainPage filled
    public static FrameworkElement MainRoot { get; set; }
    public static int WindowsCounter { get; set; } = 0;// Count the created and open windows

    private static readonly AtisDbContext Context = new();

    private SimpleSplashScreen Fss { get; set; }
    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    private SplashWindow splash_screen;


    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
        QuestPDF.Settings.License = LicenseType.Community;
    }
    public XamlRoot XamlRoot { get; set; }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IDataService, DataService>();
        services.AddSingleton<IJsonNavigationViewService>(factory =>
        {
            var json = new JsonNavigationViewService();
            
            json.ConfigDefaultPage(typeof(HomeLandingPage));
            json.ConfigSectionPage(typeof(SearchQuickPage));
            json.ConfigSectionPage(typeof(DatabasePage));
            json.ConfigSectionPage(typeof(SpeciesPage));
            json.ConfigSectionPage(typeof(UtilityPage));
            json.ConfigSectionPage(typeof(Tbl03RegnumListPage));
            json.ConfigSectionPage(typeof(Tbl06PhylumListPage));
            json.ConfigSectionPage(typeof(Tbl12SubphylumListPage));
            json.ConfigSectionPage(typeof(Tbl18SuperclassListPage));
            json.ConfigSectionPage(typeof(Tbl21ClassListPage));
            json.ConfigSectionPage(typeof(Tbl24SubclassListPage));
            json.ConfigSectionPage(typeof(Tbl27InfraclassListPage));
            json.ConfigSectionPage(typeof(Tbl30LegioListPage));
            json.ConfigSectionPage(typeof(Tbl33OrdoListPage));
            json.ConfigSectionPage(typeof(Tbl36SubordoListPage));
            json.ConfigSectionPage(typeof(Tbl39InfraordoListPage));
            json.ConfigSectionPage(typeof(Tbl42SuperfamilyListPage));
            json.ConfigSectionPage(typeof(Tbl45FamilyListPage));
            json.ConfigSectionPage(typeof(Tbl48SubfamilyListPage));
            json.ConfigSectionPage(typeof(Tbl51InfrafamilyListPage));
            json.ConfigSectionPage(typeof(Tbl54SupertribusListPage));
            json.ConfigSectionPage(typeof(Tbl57TribusListPage));
            json.ConfigSectionPage(typeof(Tbl60SubtribusListPage));
            json.ConfigSectionPage(typeof(Tbl63InfratribusListPage));
            json.ConfigSectionPage(typeof(Tbl66GenusListPage));
            json.ConfigSectionPage(typeof(Tbl68SpeciesgroupListPage));
            json.ConfigSectionPage(typeof(Tbl75SpeciesListPage));

            json.ConfigSectionPage(typeof(Tbl78NameListPage));
            json.ConfigSectionPage(typeof(Tbl81ImageListPage));
            json.ConfigSectionPage(typeof(Tbl84SynonymListPage));
            json.ConfigSectionPage(typeof(Tbl87GeographicListPage));

            json.ConfigSectionPage(typeof(Tbl90ReferenceListPage));
            json.ConfigSectionPage(typeof(Tbl90RefAuthorListPage));
            json.ConfigSectionPage(typeof(Tbl90RefExpertListPage));
            json.ConfigSectionPage(typeof(Tbl90RefSourceListPage));
            json.ConfigSectionPage(typeof(Tbl93CommentListPage));
            json.ConfigSectionPage(typeof(TblCountryListPage));
            json.ConfigSectionPage(typeof(TblUserProfileListPage));
            json.ConfigSectionPage(typeof(TblFoodListPage));
            json.ConfigSectionPage(typeof(FoodPage));
            json.ConfigSectionPage(typeof(TblDiseaseListPage));
            json.ConfigSectionPage(typeof(DiseasePage));
            json.ConfigSectionPage(typeof(TblMiscellaneousListPage));
            json.ConfigSectionPage(typeof(MiscellaneousPage));
            json.ConfigSectionPage(typeof(UserAreaPage));
            json.ConfigSectionPage(typeof(AdminAreaPage));
            json.ConfigSettingsPage(typeof(GeneralSettingPage));
            json.ConfigSettingsPage(typeof(SettingsPage));
            return json;
        });

        services.AddTransient<MainViewModel>();
        services.AddTransient<AuthenticationViewModel>();
        services.AddTransient<HomeLandingViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<DatabaseViewModel>();
        services.AddTransient<SpeciesViewModel>();
        services.AddTransient<UtilityViewModel>();
        services.AddTransient<GeneralSettingViewModel>();
        services.AddTransient<ThemeSettingViewModel>();
        services.AddTransient<AppUpdateSettingViewModel>();
        services.AddTransient<GitUpdateSettingViewModel>();
        services.AddTransient<AboutUsSettingViewModel>();
        //services.AddTransient<SettingsViewModel>();
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
        services.AddTransient<TblCountryViewModel>();
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

        return services.BuildServiceProvider();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            MainWindow = new WindowEx();
            ;
            MainWindow.Title = MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
            MainWindow.AppWindow.SetIcon("Assets/icon.ico");

            MainWindow.Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MainHeight"));
            MainWindow.Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MainWidth"));
            MainWindow.CenterOnScreen();
            MainWindow.IsAlwaysOnTop = false;
            MainWindow.IsTitleBarVisible = true;

            await InitializeLocalizer();

            //---------------------------------------------------------------------------
            //    Loggen Sie in ein Unterverzeichnis 'Log' des aktuellen Arbeitsverzeichnisses.
            //    Stellen Sie der Protokolldatei das Präfix "MyLog_" voran.
            //    Schreiben Sie eine XML-Datei, keinen Nur-Text.
            //    Dies ist ein optionaler Aufruf und muss nur einmal durchgeführt werden,
            //    vor dem ersten Log-Eintrag, bevor der erste Log-Eintrag geschrieben wird.

            SimpleLog.SetLogDir("C:\\Temp", true);
            //SimpleLog.SetLogFile(logDir: ".\\Log", prefix: "Atis_Log_", writeText: true);
            SimpleLog.SetLogFile(logDir: "C:\\Temp\\LogAtis", prefix: "Atis_Log_" + AppVersion + "_", writeText: true);

            SimpleLog.Info(" Started ATIS Application ");
            //---------------------------------------------------------------------------

            if (MainWindow.Content is not Frame rootFrame)
            {
                MainWindow.Content = rootFrame = new Frame();
            }

            GetThemeService?.AutoInitialize(MainWindow);

            MainRoot = MainWindow.Content as FrameworkElement;  //for ContentDialog

            rootFrame.Navigate(typeof(MainPage));

            splash_screen = new SplashWindow();
            splash_screen.CenterOnScreen();
            splash_screen.IsAlwaysOnTop = true;
            splash_screen.Activate();

            await Task.Delay(7000);
            MainWindow.Activate();

            splash_screen.Close();

            //Create a custom principal with an anonymous identity at startup
            var customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);
            //---------------------------------------------------------------------------

            Cult = ConfigurationManager.AppSettings.Get("Culture");
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Cult ?? string.Empty);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cult ?? string.Empty);

            //ThemeManager.Current.ChangeThemeBaseColor(Current, ConfigurationManager.AppSettings.Get("Theme1")!);
            //ThemeManager.Current.ChangeThemeColorScheme(Current, ConfigurationManager.AppSettings.Get("Accent1")!);



            //MainWindow.Closed += async ( sender,  args1) =>
            //{


            //    //  args1.Cancel = true;


            //    //var closeWindow = new CloseWindow();
            //    //closeWindow.CenterOnScreen();
            //    //closeWindow.IsAlwaysOnTop = true;
            //    //closeWindow.Activate();

            //    //// Call app specific code to subscribe to the service. For example:
            //    ContentDialogResult cdResult;
            //    ContentDialog cd = null;
            //    cd = new ContentDialog
            //    {
            //        Title = "Question",
            //        Content = "Do you want to quit ?",
            //        PrimaryButtonText = "OK",
            //    };
            //    cd.PrimaryButtonText = "Yes";
            //    cd.SecondaryButtonText = "No";
            //    cd.XamlRoot = MainRoot.XamlRoot;
            //    cdResult = await cd.ShowAsync();
            //    if (cdResult == ContentDialogResult.Primary)
            //        cd.Hide();
            //    else
            //        cd = null;
            //    if (true)
            //    {
            //        Process.GetCurrentProcess().Kill();
            //     //   Application.Current.Exit();
            //    }
            //    else
            //    {
            //        //  Nichts tun
            //    }
            //};


        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }
    //-----------------------------------------------------
    public static TblFood SingleFood(int id)
    {
        return Context.TblFoods.SingleOrDefault(a => a.FoodId == id);
    }
    public static TblDisease SingleDisease(int id)
    {
        return Context.TblDiseases.SingleOrDefault(a => a.DiseaseId == id);
    }
    public static TblMiscellaneous SingleMiscellaneous(int id)
    {
        return Context.TblMiscellaneousses.SingleOrDefault(a => a.MiscellaneousId == id);
    }


    //-----------------------------------------------------
    public static Tbl78Name SingleName(int id)
    {
        return Context.Tbl78Names.SingleOrDefault(a => a.NameId == id);
    }
    public static Tbl81Image SingleImage(int id)
    {
        return Context.Tbl81Images.SingleOrDefault(a => a.ImageId == id);
    }
    public static Tbl84Synonym SingleSynonym(int id)
    {
        return Context.Tbl84Synonyms.SingleOrDefault(a => a.SynonymId == id);
    }
    public static Tbl87Geographic SingleGeographic(int id)
    {
        return Context.Tbl87Geographics.SingleOrDefault(a => a.GeographicId == id);
    }
    //-------------------------------------------------------------------------------
    public static TblCountry SingleCountry(int id)
    {
        return Context.TblCountries.SingleOrDefault(a => a.CountryId == id);
    }
    public static TblUserProfile SingleUserProfile(int id)
    {
        return Context.TblUserProfiles.SingleOrDefault(a => a.UserProfileId == id);
    }

    //--------------------------------------------------------------------
    public static Tbl90RefAuthor SingleRefAuthor(int id)
    {
        return Context.Tbl90RefAuthors.SingleOrDefault(a => a.RefAuthorId == id);
    }
    public static Tbl90RefExpert SingleRefExpert(int id)
    {
        return Context.Tbl90RefExperts.SingleOrDefault(a => a.RefExpertId == id);
    }
    public static Tbl90RefSource SingleRefSource(int id)
    {
        return Context.Tbl90RefSources.SingleOrDefault(a => a.RefSourceId == id);
    }
    public static Tbl90Reference SingleReference(int id)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == id);
    }

    public static Tbl93Comment SingleComment(int id)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == id);
    }


    //--------------------------------------------------------------------
    public static Tbl81Image SingleImageFood(int imageId, int? foodId)
    {
        return Context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.FoodId == foodId);
    }
    public static Tbl81Image SingleImageDisease(int imageId, int? diseaseId)
    {
        return Context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.DiseaseId == diseaseId);
    }
    public static Tbl81Image SingleImageMiscellaneous(int imageId, int? miscellaneousId)
    {
        return Context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.MiscellaneousId == miscellaneousId);
    }

    //------------------------------------------------------------------
    public static Tbl75Species SingleSpecies(int id)
    {
        return Context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == id);
    }
    public static Tbl90Reference SingleReferenceSpecies(int referenceid, int? speciesId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SpeciesId == speciesId);
    }
    public static Tbl93Comment SingleCommentSpecies(int commentid, int? speciesid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SpeciesId == speciesid);
    }
    public static Tbl78Name SingleNameSpecies(int nameId, int? speciesId)
    {
        return Context.Tbl78Names.SingleOrDefault(a => a.NameId == nameId & a.SpeciesId == speciesId);
    }
    public static Tbl81Image SingleImageSpecies(int imageId, int? speciesId)
    {
        return Context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.SpeciesId == speciesId);
    }
    public static Tbl84Synonym SingleSynonymSpecies(int synonymId, int? speciesId)
    {
        return Context.Tbl84Synonyms.SingleOrDefault(a => a.SynonymId == synonymId & a.SpeciesId == speciesId);
    }
    public static Tbl87Geographic SingleGeographicSpecies(int geographicId, int? speciesId)
    {
        return Context.Tbl87Geographics.SingleOrDefault(a => a.GeographicId == geographicId & a.SpeciesId == speciesId);
    }
    public static Tbl75Species SingleSpeciesGenus(int speciesid, int? genusid)
    {
        return Context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == speciesid & a.GenusId == genusid);
    }
    public static Tbl78Name SingleNameSpecies(int nameid, int speciesid)
    {
        return Context.Tbl78Names.SingleOrDefault(a => a.SpeciesId == speciesid & a.NameId == nameid);
    }
    
    //------------------------------------------------------------------
    public static Tbl68Speciesgroup SingleSpeciesgroup(int id)
    {
        return Context.Tbl68Speciesgroups.SingleOrDefault(a => a.SpeciesgroupId == id);
    }
    public static Tbl75Species SingleSpeciesSpeciesgroup(int speciesId, int? speciesgroupId)
    {
        return Context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == speciesId & a.SpeciesgroupId == speciesgroupId);
    }
    //public static Tbl75Species SingleSpeciesSpeciesgroup(int speciesid, int? speciesgroupid)
    //{
    //    return Context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == speciesid & a.SpeciesgroupId == speciesgroupid);
    //}
    
    //------------------------------------------------------------------

    public static Tbl66Genus SingleGenus(int id)
    {
        return Context.Tbl66Genusses.SingleOrDefault(a => a.GenusId == id);
    }
    public static Tbl90Reference SingleReferenceGenus(int referenceid, int? genusId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.GenusId == genusId);
    }
    public static Tbl93Comment SingleCommentGenus(int commentid, int? genusid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.GenusId == genusid);
    }
    //public static Tbl75Species SingleSpeciesGenus(int speciesId, int? genusId)
    //{
    //    return Context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == speciesId & a.GenusId == genusId);
    //}
    //---------------------------------------------------
    public static Tbl63Infratribus SingleInfratribus(int id)
    {
        return Context.Tbl63Infratribusses.SingleOrDefault(a => a.InfratribusId == id);
    }
    public static Tbl90Reference SingleReferenceInfratribus(int referenceid, int? infratribusId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfratribusId == infratribusId);
    }
    public static Tbl93Comment SingleCommentInfratribus(int commentid, int? infratribusid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfratribusId == infratribusid);
    }
    public static Tbl66Genus SingleGenusInfratribus(int genusId, int? infratribusId)
    {
        return Context.Tbl66Genusses.SingleOrDefault(a => a.GenusId == genusId & a.InfratribusId == infratribusId);
    }
    //------------------------------------------------------------------

    //---------------------------------------------

    public static Tbl60Subtribus SingleSubtribus(int id)
    {
        return Context.Tbl60Subtribusses.SingleOrDefault(a => a.SubtribusId == id);
    }
    public static Tbl90Reference SingleReferenceSubtribus(int referenceid, int? subtribusId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubtribusId == subtribusId);
    }
    public static Tbl93Comment SingleCommentSubtribus(int commentid, int? subtribusid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubtribusId == subtribusid);
    }
    
    public static Tbl63Infratribus SingleInfratribusSubtribus(int infratribusId, int? subtribusId)
    {
        return Context.Tbl63Infratribusses.SingleOrDefault(a => a.InfratribusId == infratribusId & a.SubtribusId == subtribusId);
    }
    //---------------------------------------------------
    public static Tbl57Tribus SingleTribus(int id)
    {
        return Context.Tbl57Tribusses.SingleOrDefault(a => a.TribusId == id);
    }
    public static Tbl90Reference SingleReferenceTribus(int referenceid, int? tribusId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.TribusId == tribusId);
    }
    public static Tbl93Comment SingleCommentTribus(int commentid, int? tribusid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.TribusId == tribusid);
    }
    public static Tbl60Subtribus SingleSubtribusTribus(int subtribusId, int? tribusId)
    {
        return Context.Tbl60Subtribusses.SingleOrDefault(a => a.SubtribusId == subtribusId & a.TribusId == tribusId);
    }
    //------------------------------------------------------------------
    public static Tbl54Supertribus SingleSupertribus(int id)
    {
        return Context.Tbl54Supertribusses.SingleOrDefault(a => a.SupertribusId == id);
    }
    public static Tbl90Reference SingleReferenceSupertribus(int referenceid, int? supertribusId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SupertribusId == supertribusId);
    }
    public static Tbl93Comment SingleCommentSupertribus(int commentid, int? supertribusid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SupertribusId == supertribusid);
    }
    public static Tbl57Tribus SingleTribusSupertribus(int tribusId, int? supertribusId)
    {
        return Context.Tbl57Tribusses.SingleOrDefault(a => a.TribusId == tribusId & a.SupertribusId == supertribusId);
    }
    //------------------------------------------------------------------
    public static Tbl51Infrafamily SingleInfrafamily(int id)
    {
        return Context.Tbl51Infrafamilies.SingleOrDefault(a => a.InfrafamilyId == id);
    }
    public static Tbl90Reference SingleReferenceInfrafamily(int referenceid, int? infrafamilyId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfrafamilyId == infrafamilyId);
    }
    public static Tbl93Comment SingleCommentInfrafamily(int commentid, int? infrafamilyid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfrafamilyId == infrafamilyid);
    }
    public static Tbl54Supertribus SingleSupertribusInfrafamily(int supertribusId, int? infrafamilyId)
    {
        return Context.Tbl54Supertribusses.SingleOrDefault(a => a.SupertribusId == supertribusId & a.InfrafamilyId == infrafamilyId);
    }
    //------------------------------------------------------------------
    public static Tbl48Subfamily SingleSubfamily(int id)
    {
        return Context.Tbl48Subfamilies.SingleOrDefault(a => a.SubfamilyId == id);
    }
    public static Tbl90Reference SingleReferenceSubfamily(int referenceid, int? subfamilyId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubfamilyId == subfamilyId);
    }
    public static Tbl93Comment SingleCommentSubfamily(int commentid, int? subfamilyid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubfamilyId == subfamilyid);
    }
    public static Tbl51Infrafamily SingleInfrafamilySubfamily(int infrafamilyId, int? subfamilyId)
    {
        return Context.Tbl51Infrafamilies.SingleOrDefault(a => a.InfrafamilyId == infrafamilyId & a.SubfamilyId == subfamilyId);
    }
    //------------------------------------------------------------------
    public static Tbl45Family SingleFamily(int id)
    {
        return Context.Tbl45Families.SingleOrDefault(a => a.FamilyId == id);
    }
    public static Tbl90Reference SingleReferenceFamily(int referenceid, int? familyId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.FamilyId == familyId);
    }
    public static Tbl93Comment SingleCommentFamily(int commentid, int? familyid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.FamilyId == familyid);
    }
    public static Tbl48Subfamily SingleSubfamilyFamily(int subfamilyId, int? familyId)
    {
        return Context.Tbl48Subfamilies.SingleOrDefault(a => a.SubfamilyId == subfamilyId & a.FamilyId == familyId);
    }
    //------------------------------------------------------------------
    public static Tbl42Superfamily SingleSuperfamily(int id)
    {
        return Context.Tbl42Superfamilies.SingleOrDefault(a => a.SuperfamilyId == id);
    }
    public static Tbl90Reference SingleReferenceSuperfamily(int referenceid, int? superfamilyId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SuperfamilyId == superfamilyId);
    }
    public static Tbl93Comment SingleCommentSuperfamily(int commentid, int? superfamilyid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SuperfamilyId == superfamilyid);
    }
    public static Tbl45Family SingleFamilySuperfamily(int familyId, int? superfamilyId)
    {
        return Context.Tbl45Families.SingleOrDefault(a => a.FamilyId == familyId & a.SuperfamilyId == superfamilyId);
    }
    //------------------------------------------------------------------
    public static Tbl39Infraordo SingleInfraordo(int id)
    {
        return Context.Tbl39Infraordos.SingleOrDefault(a => a.InfraordoId == id);
    }
    public static Tbl90Reference SingleReferenceInfraordo(int referenceid, int? infraordoId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfraordoId == infraordoId);
    }
    public static Tbl93Comment SingleCommentInfraordo(int commentid, int? infraordoid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfraordoId == infraordoid);
    }
    public static Tbl42Superfamily SingleSuperfamilyInfraordo(int superfamilyId, int? infraordoId)
    {
        return Context.Tbl42Superfamilies.SingleOrDefault(a => a.SuperfamilyId == superfamilyId & a.InfraordoId == infraordoId);
    }
    //------------------------------------------------------------------
    public static Tbl36Subordo SingleSubordo(int id)
    {
        return Context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == id);
    }
    public static Tbl90Reference SingleReferenceSubordo(int referenceid, int? subordoId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubordoId == subordoId);
    }
    public static Tbl93Comment SingleCommentSubordo(int commentid, int? subordoid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubordoId == subordoid);
    }
    public static Tbl39Infraordo SingleInfraordoSubordo(int infraordoId, int? subordoId)
    {
        return Context.Tbl39Infraordos.SingleOrDefault(a => a.InfraordoId == infraordoId & a.SubordoId == subordoId);
    }
    //------------------------------------------------------------------
    public static Tbl33Ordo SingleOrdo(int id)
    {
        return Context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == id);
    }
    public static Tbl90Reference SingleReferenceOrdo(int referenceid, int? ordoId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.OrdoId == ordoId);
    }
    public static Tbl93Comment SingleCommentOrdo(int commentid, int? ordoid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.OrdoId == ordoid);
    }
    public static Tbl36Subordo SingleSubordoOrdo(int subordoId, int? ordoId)
    {
        return Context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == subordoId & a.OrdoId == ordoId);
    }
    //------------------------------------------------------------------
    public static Tbl30Legio SingleLegio(int id)
    {
        return Context.Tbl30Legios.SingleOrDefault(a => a.LegioId == id);
    }
    public static Tbl90Reference SingleReferenceLegio(int referenceid, int? legioId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.LegioId == legioId);
    }
    public static Tbl93Comment SingleCommentLegio(int commentid, int? legioid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.LegioId == legioid);
    }
    public static Tbl33Ordo SingleOrdoLegio(int ordoId, int? legioId)
    {
        return Context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == ordoId & a.LegioId == legioId);
    }
    //------------------------------------------------------------------
    public static Tbl27Infraclass SingleInfraclass(int id)
    {
        return Context.Tbl27Infraclasses.SingleOrDefault(a => a.InfraclassId == id);
    }
    public static Tbl90Reference SingleReferenceInfraclass(int referenceid, int? infraclassId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfraclassId == infraclassId);
    }
    public static Tbl93Comment SingleCommentInfraclass(int commentid, int? infraclassid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfraclassId == infraclassid);
    }
    public static Tbl30Legio SingleLegioInfraclass(int legioId, int? infraclassId)
    {
        return Context.Tbl30Legios.SingleOrDefault(a => a.LegioId == legioId & a.InfraclassId == infraclassId);
    }
    //------------------------------------------------------------------
    public static Tbl24Subclass SingleSubclass(int id)
    {
        return Context.Tbl24Subclasses.SingleOrDefault(a => a.SubclassId == id);
    }
    public static Tbl90Reference SingleReferenceSubclass(int referenceid, int? subclassId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubclassId == subclassId);
    }
    public static Tbl93Comment SingleCommentSubclass(int commentid, int? subclassid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubclassId == subclassid);
    }
    public static Tbl27Infraclass SingleInfraclassSubclass(int infraclassId, int? subclassId)
    {
        return Context.Tbl27Infraclasses.SingleOrDefault(a => a.InfraclassId == infraclassId & a.SubclassId == subclassId);
    }
    //------------------------------------------------------------------
    public static Tbl21Class SingleClass(int id)
    {
        return Context.Tbl21Classes.SingleOrDefault(a => a.ClassId == id);
    }
    public static Tbl90Reference SingleReferenceClass(int referenceid, int? classId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.ClassId == classId);
    }
    public static Tbl93Comment SingleCommentClass(int commentid, int? classid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.ClassId == classid);
    }
    public static Tbl24Subclass SingleSubclassClass(int subclassId, int? classId)
    {
        return Context.Tbl24Subclasses.SingleOrDefault(a => a.SubclassId == subclassId & a.ClassId == classId);
    }
    //------------------------------------------------------------------
    public static Tbl18Superclass SingleSuperclass(int id)
    {
        return Context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == id);
    }
    public static Tbl90Reference SingleReferenceSuperclass(int referenceid, int? superclassId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SuperclassId == superclassId);
    }
    public static Tbl93Comment SingleCommentSuperclass(int commentid, int? superclassid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SuperclassId == superclassid);
    }
    public static Tbl21Class SingleClassSuperclass(int classeId, int? superclassId)
    {
        return Context.Tbl21Classes.SingleOrDefault(a => a.ClassId == classeId & a.SuperclassId == superclassId);
    }
    //------------------------------------------------------------------
    //public static Tbl15Subdivision SingleSubdivision(int id)
    //{
    //    return Context.Tbl15Subdivisions.SingleOrDefault(a => a.SubdivisionId == id);
    //}
    //public static Tbl90Reference SingleReferenceSubdivision(int referenceid)
    //{
    //    return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid );
    //}
    //public static Tbl93Comment SingleCommentSubdivision(int commentid)
    //{
    //    return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid );
    //}
    //public static Tbl18Superclass SingleSuperclassSubdivision(int superclassId)
    //{
    //    return Context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == superclassId );
    //}
    //------------------------------------------------------------------
    public static Tbl12Subphylum SingleSubphylum(int id)
    {
        return Context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id);
    }
    public static Tbl90Reference SingleReferenceSubphylum(int referenceid, int? subphylumId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubphylumId == subphylumId);
    }
    public static Tbl93Comment SingleCommentSubphylum(int commentid, int? subphylumid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubphylumId == subphylumid);
    }
    public static Tbl18Superclass SingleSuperclassSubphylum(int superclassId, int? subphylumId)
    {
        return Context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == superclassId & a.SubphylumId == subphylumId);
    }
    //------------------------------------------------------------------
    //public static Tbl09Division SingleDivision(int id)
    //{
    //    return Context.Tbl09Divisions.SingleOrDefault(a => a.DivisionId == id);
    //}
    //public static Tbl90Reference SingleReferenceDivision(int referenceid, int? divisionId)
    //{
    //    return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.DivisionId == divisionId);
    //}
    //public static Tbl93Comment SingleCommentDivision(int commentid, int? divisionid)
    //{
    //    return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.DivisionId == divisionid);
    //}
    //public static Tbl15Subdivision SingleSubdivisionDivision(int subdivisionId, int? divisionId)
    //{
    //    return Context.Tbl15Subdivisions.SingleOrDefault(a => a.SubdivisionId == subdivisionId & a.DivisionId == divisionId);
    //}
    //------------------------------------------------------------------
    public static Tbl06Phylum SinglePhylum(int id)
    {
        return Context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id);
    }
    public static Tbl90Reference SingleReferencePhylum(int referenceid, int? phylumId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.PhylumId == phylumId);
    }
    public static Tbl93Comment SingleCommentPhylum(int commentid, int? phylumid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.PhylumId == phylumid);
    }
    public static Tbl12Subphylum SingleSubphylumPhylum(int subphylumId, int? phylumId)
    {
        return Context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == subphylumId & a.PhylumId == phylumId);
    }
    //------------------------------------------------------------------
    public static Tbl03Regnum SingleRegnum(int id)
    {
        return Context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id);
    }
    public static Tbl90Reference SingleReferenceRegnum(int referenceid, int? regnumId)
    {
        return Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.RegnumId == regnumId);
    }
    public static Tbl93Comment SingleCommentRegnum(int commentid, int? regnumid)
    {
        return Context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.RegnumId == regnumid);
    }
    public static Tbl06Phylum SinglePhylumRegnum(int phylumId, int? regnumId)
    {
        return Context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == phylumId & a.RegnumId == regnumId);
    }
    public static Tbl09Division SingleDivisionRegnum(int divisionId, int? regnumId)
    {
        return Context.Tbl09Divisions.SingleOrDefault(a => a.DivisionId == divisionId & a.RegnumId == regnumId);
    }

}

