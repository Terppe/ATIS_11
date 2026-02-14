using System.Configuration;
using System.Globalization;
using System.Net.Sockets;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.ViewModels;
using ATIS.Views.Admin;
using ATIS.Views.Authentication;
using ATIS.Views.Database;
using ATIS.Views.Main.Disease;
using ATIS.Views.Search;
using ATIS.Views.Settings;
using ATIS.Views.User;
using Microsoft.Data.SqlClient;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using WinUI3Localizer;
using TitleBar = Microsoft.UI.Xaml.Controls.TitleBar;

namespace ATIS.Views.Main;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; set; }
    private string _currentPageTag { get; set; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        //DataContext = ViewModel;
        System.Diagnostics.Debug.WriteLine($"DataContext ({this.GetType().Name}): {this.DataContext?.GetType().Name}");

        NavFrame.Navigate(typeof(HomePage)); // ← garantiert sofort sichtbar

        _currentPageTag = "ATIS.Views.Main.HomePage";

        NavView.SelectedItem = NavView.MenuItems
            .OfType<NavigationViewItem>()
            .First(i => (string)i.Tag == _currentPageTag);

        // Cursor im NavigationView setzen
        var homeItem = NavView.MenuItems
            .OfType<NavigationViewItem>()
            .FirstOrDefault(i => (string)i.Tag == "ATIS.Views.Main.HomePage");
        if (homeItem != null)
            NavView.SelectedItem = homeItem;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {

        ViewModel = App.GetService<MainViewModel>();
        System.Diagnostics.Debug.WriteLine($"DataContext: {this.DataContext}");

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);

        ////Check Server and Database
        //CheckSql_Db();

        //CloseApp();
        //// Write info message to log
        //SimpleLog.Info("MainWindow logging started.");

        //ViewModel.IsLoading = false;
        // Call ViewModel initialization
        await ViewModel.InitAsync();
    }

  //  private void CheckSql_Db()
    //{
    //    if (!CheckForServer(ConfigurationManager.AppSettings.Get("ServerName"),
    //            Convert.ToInt32(ConfigurationManager.AppSettings.Get("Port"))))
    //    {
    //        TbDataBase.Text = StringsRes.SqlServerNotRunning;
    //    }
    //    else
    //    {
    //        if (!CheckDatabaseExist(ConfigurationManager.AppSettings.Get("ServerName")))
    //        {
    //            TbDataBase.Text = StringsRes.DatabaseNotFound;
    //            ;
    //        }
    //    }

    //}

    //------------------------Check Server and Database-------------------------------
    //private static bool CheckForServer(string address, int port)
    //{
    //    var timeout = 500;
    //    if (ConfigurationManager.AppSettings.Get("RemoteTestTimeout") != null)
    //    {
    //        timeout = int.Parse(ConfigurationManager.AppSettings.Get("RemoteTestTimeout") ?? string.Empty);
    //    }

    //    try
    //    {
    //        var result = false;
    //        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //        if (address != null)
    //        {
    //            var asyncResult = socket.BeginConnect(address, port, null, null);
    //            result = asyncResult.AsyncWaitHandle.WaitOne(timeout, true);
    //        }

    //        socket.Close();

    //        return result;
    //    }
    //    catch (SqlException e)
    //    {
    //        _ = AllDialogs.ErrorMessageDialogAsync(e.ToString());

    //        _ = AllDialogs.NoServerConnectWarnMessageDialogAsync();
    //        //MessageBox.Show(CultRes.StringsRes.NoConnectServer, e.Message,
    //        //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

    //        SimpleLog.Error("Server not connected" + e.Message);

    //        return false;
    //    }
    //}

    //Check if database exist or SQLServer not running
    //private bool CheckDatabaseExist(string db)
    //{
    //    var ret = false;

    //    try
    //    {
    //        using var context = new AtisDbContext();
    //        var dbExists = context.Database.CanConnect();

    //        if (!dbExists)
    //        {
    //            // MessageBox.Show(CultRes.StringsRes.NoConnectServer + " " + db, $"CultRes.StringsRes.NoConnectServer: {db}");
    //            if (db != null)
    //            {
    //                _ = AllDialogs.WarningNoDatabaseWithSqlServerSelectedMessageDialogAsync(db);
    //            }

    //            //MessageBox.Show($"Database does not exist", "SQL-Server ok " + db,
    //            //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
    //            if (true)
    //            {
    //                ret = false;
    //            }

    //            SimpleLog.Error("Database does not exist - SQL - Server ok");
    //        }
    //        else
    //        {
    //            TbDataBase.Foreground = new SolidColorBrush(Colors.Green);
    //            ret = true;
    //        }
    //    }
    //    catch (SqlException e)
    //    {
    //        _ = AllDialogs.ErrorMessageDialogAsync(e.ToString());

    //        _ = AllDialogs.NoServerConnectWarnMessageDialogAsync();
    //        //MessageBox.Show($"CultRes.StringsRes.NoConnectServer: {db}", e.Message,
    //        //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

    //        SimpleLog.Error($"Server not connected: {db}");
    //        ret = false;
    //    }

    //    return ret;
    //}
    //--------------------------------------------------------------------

    private void CloseApp()
    {
        //Close Application
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
                XamlRoot = Content.XamlRoot,
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

    }

    //--------------------------------------------------------------------
    private void AppTitleBar_OnBackRequested(TitleBar sender, object args)
    {
        if (NavFrame.CanGoBack)
        {
            NavFrame.GoBack();
        }
    }

    private void AppTitleBar_OnPaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }


    private void NavFrame_Navigated(object sender, NavigationEventArgs e)
    {
        AppTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ChangeThemeWithoutSave(App.MainWindow);
    }


    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        _ = args.QueryText;

        App.SearchQuickString = args.QueryText;

        NavFrame.Navigate(typeof(SearchQuickPage));
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        var viewModel = NavFrame.GetPageViewModel();
        if (viewModel != null && viewModel is ITitleBarAutoSuggestBoxAware titleBarAutoSuggestBoxAware)
        {
            titleBarAutoSuggestBoxAware.OnAutoSuggestBoxTextChanged(sender, args);
        }
    }

    private void SearchSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SearchSearchBox.Visibility = Visibility.Visible;
        SearchSearchBox.Focus(FocusState.Programmatic);
        SearchSearchBox.PlaceholderText = StringsRes.SearchName;
    }

    private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem selectedItem)
        {
            var sortOption = selectedItem.Tag.ToString();
            switch (sortOption)
            {
                case "login":
                    await LogIn();
                    break;
                //case "register":
                //    await ContentDialogRegister.ShowAsync();
                //    break;
                case "register":
                    await Register();
                    break;
                case "changepw":
                    await ContentDialogPasswordChange.ShowAsync();
                    break;
                case "forgotpw":
                    await ContentDialogPasswordForgot.ShowAsync();
                    break;
                case "update":
                    await CheckForUpdatesButton_Click(sender, e);
                    break;
                case "logout":
                    await LogOut();
                    break;
                case "showlogfile":
                    SimpleLog.ShowLogFile();
                    break;
                case "close":
                    await Close();
                    break;
            }
        }
    }

    //--------------------------------------------------------------------
    private void MenuFlyoutLanguage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.Tag is string languageCode)
        {
            ReloadLanguage(languageCode);
        }
    }

    private void ReloadLanguage(string languageOverride)
    {
        // 1. Globale Culture setzen
        Thread.CurrentThread.CurrentCulture = new CultureInfo(languageOverride);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageOverride);

        // 2. Dein Localizer
        Localizer.Get().SetLanguage(languageOverride);
        App.Cult = languageOverride;

        // 3. UI neu laden / Texte aktualisieren
  //      ChangeLanguageLoginReset();

        // 4. 🔥 ALLE ViewModels aktualisieren
        ApplyCultureToAllViewModels();
    }

    private void ChangeLanguageLoginReset()
    {
        if (Thread.CurrentPrincipal is not CustomPrincipal customPrincipal)
        {
            return;
        }

        var name = customPrincipal.Identity.Email;
        if (name != "")
        {
            TbUser.Foreground = new SolidColorBrush(Colors.Green);
            TbUser.Text = name + " " + StringsRes.LoggedIn;
        }
    }

    private void ApplyCultureToAllViewModels()
    {
        if (App.GetService<TblCountryViewModel>() is { } countryVm)
            countryVm.ApplyCulture();
        //if (App.GetService<TblDiseaseViewModel>() is { } diseaseVm)
        //    diseaseVm.ApplyCulture();
        //if (App.GetService<TblFoodViewModel>() is { } foodVm)
        //    foodVm.ApplyCulture();
        // usw. für alle ViewModels
        }
    //----------------------------------------------------
    private async Task LogIn()
    {
        await ContentDialogLogIn.ShowAsync();

        if (Thread.CurrentPrincipal is not CustomPrincipal customPrincipal)
        {
            return;
        }

        var name = customPrincipal.Identity.Email;
        if (name != "")
        {
            TbUser.Foreground = new SolidColorBrush(Colors.Green);
            TbUser.Text = name + " " + StringsRes.LoggedIn;
        }
    }

    private async Task Register() //register and login
    {
        await ContentDialogRegister.ShowAsync();

        if (Thread.CurrentPrincipal is not CustomPrincipal customPrincipal)
        {
            return;
        }

        var name = customPrincipal.Identity.Email;
        if (name != "")
        {
            TbUser.Foreground = new SolidColorBrush(Colors.Green);
            TbUser.Text = name + " " + StringsRes.LoggedIn;
        }
    }

    private static async Task CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the current app's package for the current user.
        var pm = new PackageManager();
        var currentPackage = pm.FindPackageForUser(string.Empty, Package.Current.Id.FullName);

        var result = await currentPackage.CheckUpdateAvailabilityAsync();
        switch (result.Availability)
        {
            case PackageUpdateAvailability.Available:
                //         GoToUpdateAvailableUIView();
                break;
            case PackageUpdateAvailability.Required:
                //          GoToUpdateRequiredUIView();
                break;
            case PackageUpdateAvailability.NoUpdates:
                // Dismissable ‘Ok’ dialog.
                //          ShowNoUpdateAvailableDialog();
                break;
            case PackageUpdateAvailability.Unknown:
            default:
                // Log and ignore error.
                //         Logger.Log($"No update information associated with app {Package.Current.DisplayName}");
                // Dismissable ‘Ok’ dialog.
                //         ShowNoUpdateAvailableDialog();
                break;
        }
    }

    private Task LogOut()
    {
        if (Thread.CurrentPrincipal is CustomPrincipal customPrincipal)
        {
            customPrincipal.Identity = new AnonymousIdentity();
        }

        TbUser.Foreground = new SolidColorBrush(Colors.Red);
        TbUser.Text = StringsRes.NobodyLoggedIn;
        return Task.CompletedTask;
    }

    private static void SaveCulture(string cult)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings["Culture"].Value = CultureInfo.GetCultureInfoByIetfLanguageTag(cult).ToString();
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("AppSettings");
    }

    private async Task Close()
    {
        var dialog = new ContentDialog
        {
            XamlRoot = Content.XamlRoot,
            Title = StringsRes.WantToCloseApp,
            PrimaryButtonText = StringsRes.Yes,
            CloseButtonText = StringsRes.No,
            DefaultButton = ContentDialogButton.Close
        };
        //    dialog.PrimaryButtonClick += (se, ee) => DispatcherQueue.TryEnqueue(dialog.Hide);
        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.None)
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
    }

    private void ControlsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // User selected an item from the suggestion list, take an action on it here.

        if (args.ChosenSuggestion == null)
        {
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavHome.Content)
        {
            NavFrame.Navigate(typeof(TblCountryListTestPage));
            return;
        }
        if ((string)args.ChosenSuggestion == (string)NavHome.Content)
        {
            NavFrame.Navigate(typeof(HomePage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavSearch.Content)
        {
            NavFrame.Navigate(typeof(SearchQuickPage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavSpecies.Content)
        {
            NavFrame.Navigate(typeof(Tbl75SpeciesListPage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavGenus.Content)
        {
            NavFrame.Navigate(typeof(Tbl66GenusListPage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavSpeciesgroup.Content)
        {
            NavFrame.Navigate(typeof(Tbl68SpeciesgroupListPage));
            return;
        }

        //-------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfratribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl63InfratribusListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubtribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl60SubtribusListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavTribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl57TribusListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSupertribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl54SupertribusListPage));
        }

        //-------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfrafamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl51InfrafamilyListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl48SubfamilyListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavFamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl45FamilyListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSuperfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl42SuperfamilyListPage));
        }
        //-------------------------------------------

        if ((string)args.ChosenSuggestion == (string)NavInfrafamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl51InfrafamilyListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl48SubfamilyListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavFamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl45FamilyListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSuperfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl42SuperfamilyListPage));
        }

        //----------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfraordo.Content)
        {
            NavFrame.Navigate(typeof(Tbl39InfraordoListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubordo.Content)
        {
            NavFrame.Navigate(typeof(Tbl36SubordoListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavOrdo.Content)
        {
            NavFrame.Navigate(typeof(Tbl33OrdoListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavLegio.Content)
        {
            NavFrame.Navigate(typeof(Tbl30LegioListPage));
        }

        //--------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfraclass.Content)
        {
            NavFrame.Navigate(typeof(Tbl27InfraclassListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubclass.Content)
        {
            NavFrame.Navigate(typeof(Tbl24SubclassListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavClass.Content)
        {
            NavFrame.Navigate(typeof(Tbl21ClassListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSuperclass.Content)
        {
            NavFrame.Navigate(typeof(Tbl18SuperclassListPage));
        }

        //-------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavSubphylum.Content)
        {
            NavFrame.Navigate(typeof(Tbl12SubphylumListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavPhylum.Content)
        {
            NavFrame.Navigate(typeof(Tbl06PhylumListPage));
        }

        //---------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavRegnum.Content)
        {
            NavFrame.Navigate(typeof(Tbl03RegnumListPage));
        }

        //---------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavNames.Content)
        {
            NavFrame.Navigate(typeof(Tbl78NameListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavImages.Content)
        {
            NavFrame.Navigate(typeof(Tbl81ImageListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSynonyms.Content)
        {
            NavFrame.Navigate(typeof(Tbl84SynonymListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavGeographics.Content)
        {
            NavFrame.Navigate(typeof(Tbl87GeographicListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavReferences.Content)
        {
            NavFrame.Navigate(typeof(Tbl90ReferenceListPage));
        }

        //------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavRefAuthors.Content)
        {
            NavFrame.Navigate(typeof(Tbl90RefAuthorListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavRefExperts.Content)
        {
            NavFrame.Navigate(typeof(Tbl90RefExpertListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavRefSources.Content)
        {
            NavFrame.Navigate(typeof(Tbl90RefSourceListPage));
        }

        //-----------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavComments.Content)
        {
            NavFrame.Navigate(typeof(Tbl93CommentListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavCountries.Content)
        {
            NavFrame.Navigate(typeof(TblCountryListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavUserProfiles.Content)
        {
            NavFrame.Navigate(typeof(TblUserProfileListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavDiseases1.Content)
        {
            NavFrame.Navigate(typeof(TblDiseaseListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavFoods1.Content)
        {
            NavFrame.Navigate(typeof(TblFoodListPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavMiscellaneousses1.Content)
        {
            NavFrame.Navigate(typeof(TblMiscellaneousListPage));
        }

        //--------------------------------------------

        if ((string)args.ChosenSuggestion == (string)NavAdmin_Area.Content)
        {
            NavFrame.Navigate(typeof(AdminAreaPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavUser_Area.Content)
        {
            NavFrame.Navigate(typeof(UserAreaPage));
        }
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {

        if (args.InvokedItemContainer != null &&
            (string)args.InvokedItemContainer.Tag == "ATIS.Views.Database.TblCountryListTestPage")
        {
            NavFrame.Navigate(typeof(TblCountryListTestPage), null, args.RecommendedNavigationTransitionInfo);
            return;
        }
        if (args.InvokedItemContainer != null &&
            (string)args.InvokedItemContainer.Tag == "ATIS.Views.Main.Disease.DiseasePage")
        {
            NavFrame.Navigate(typeof(DiseasePage), null, args.RecommendedNavigationTransitionInfo);
            return;
        }

        if (args.InvokedItemContainer != null &&
            (string)args.InvokedItemContainer.Tag == "ATIS.Views.Main.Food.FoodPage")
        {
            NavFrame.Navigate(typeof(Food.FoodPage), null, args.RecommendedNavigationTransitionInfo);
            return;
        }

        if (args.InvokedItemContainer != null &&
            (string)args.InvokedItemContainer.Tag == "ATIS.Views.Main.Miscellaneous.MiscellaneousPage")
        {
            NavFrame.Navigate(typeof(Miscellaneous.MiscellaneousPage), null, args.RecommendedNavigationTransitionInfo);
            return;
        }

        if (args.IsSettingsInvoked)
        {
            NavFrame.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            return;
        }

        if (args.InvokedItemContainer != null && args.InvokedItemContainer.Tag != null)
        {
            var newPage = Type.GetType(args.InvokedItemContainer.Tag.ToString()!);
            NavFrame.Navigate(
                newPage,
                null,
                args.RecommendedNavigationTransitionInfo
            );
        }
    }

    private void NavigationView_Loaded(object sender, RoutedEventArgs e)
    {
        //  NavFrame.Navigate(typeof(HomePage));
        // NICHT navigieren!
        NavView.SelectedItem = NavHome;
    }

    //private async void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    //{
    //    var nvi = args.SelectedItemContainer as NavigationViewItem;
    //    if (nvi == null) return;

    //    var tag = (string)nvi.Tag;
    //    if (tag == null) return;


    //    // 1. Seiten ohne Permission-Prüfung
    //    if (tag == "ATIS.Views.Search.SearchQuickPage")
    //    {
    //        NavFrame.Navigate(typeof(SearchQuickPage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    if (tag == "ATIS.Views.Main.HomePage")
    //    {
    //        NavFrame.Navigate(typeof(HomePage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    // ⬅️ Während der Entwicklung, da keine Berechtigung!
    //    if (tag == "ATIS.Views.Database.TblCountryListPage")
    //    {
    //        NavFrame.Navigate(typeof(TblCountryListPage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    if (tag == "ATIS.Views.Main.Disease.DiseasePage")
    //    {
    //        NavFrame.Navigate(typeof(Disease.DiseasePage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    if (tag == "ATIS.Views.Main.Food.FoodPage")
    //    {
    //        NavFrame.Navigate(typeof(Food.FoodPage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    if (tag == "ATIS.Views.Main.Miscellaneous.MiscellaneousPage")
    //    {
    //        NavFrame.Navigate(typeof(Miscellaneous.MiscellaneousPage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    if (tag == "Settings")
    //    {
    //        NavFrame.Navigate(typeof(SettingsPage));
    //        PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //        return;
    //    }

    //    // 2.🔥 PermissionHelper einsetzen
    //    if (!await PermissionHelper.CheckOrGoBackAsync(NavView, NavFrame, tag))
    //        return;


    //    // 3. Permission OK → Page laden
    //    var type = Type.GetType(tag);
    //    NavFrame.Navigate(type);

    //    // 4. Und jetzt WICHTIG:
    //    PermissionHelper.UpdateLastPage(tag); // ⬅️ WICHTIG!
    //}





    private void ControlsSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // Only get results when it was a user typing, 
        // otherwise assume the value got filled in by TextMemberPath 
        // or the handler for SuggestionChosen.
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            //Set the ItemsSource to be your filtered dataset
            //sender.ItemsSource = dataset;

            var searchText = sender.Text;

            var resultList = new List<string>();

            var itemlist = NavView.MenuItems.ToList();

            CheckItems(itemlist, resultList, searchText);

            sender.ItemsSource = resultList;
        }
    }

    public static List<string> CheckItems(List<object> sourceList, List<string> resultList, string searchText)
    {
        foreach (var item in sourceList)
        {
            if (item.GetType() == typeof(NavigationViewItem))
            {
                // get first level item
                var viewItem = item as NavigationViewItem;

                //check if contains second level items
                if (viewItem != null)
                {
                    var level2Items = viewItem.MenuItems.ToList();

                    if (level2Items.Count > 0)
                    {
                        CheckItems(level2Items, resultList, searchText);
                    }
                    else
                    {
                        // this is the first level item
                        var str = viewItem.Content.ToString()!.ToLowerInvariant();
                        if (str.Contains(searchText))
                        {
                            resultList.Add(viewItem.Content.ToString());
                        }
                    }
                }
            }
        }

        return resultList;
    }

}
