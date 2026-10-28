using System.Configuration;
using System.Globalization;
using System.Reflection;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Strings;
using ATIS.Views.Admin;
using ATIS.Views.Auth;
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
using ATIS.Views.Search;
using ATIS.Views.Settings;
using ATIS.Views.User;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using WinRT.Interop;
using WinUI3Localizer;

namespace ATIS.Views.Main;

public sealed partial class MainPage : Page
{
    private AppWindow _appWindow;
    public MainViewModel ViewModel { get; set; }
    private string _currentPageTag { get; set; }
    private IDialogService dialog;
    public string? WinAppSdkVersion { get; set; } = ConfigurationManager.AppSettings.Get("WinAppSDK");

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        dialog = App.GetService<IDialogService>();

        InitializeComponent();
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
        App.MainWindow?.ExtendsContentIntoTitleBar = true;
        App.MainWindow?.SetTitleBar(AppTitleBar);

        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        await ViewModel.InitAsync();

        //Version
        TbVersion.Text = BuildInfo.Text;
    }
    private void AppTitleBar_OnBackRequested(Microsoft.UI.Xaml.Controls.TitleBar sender, object args)
    {
        if (NavFrame.CanGoBack)
        {
            NavFrame.GoBack();
        }
    }

    private void AppTitleBar_OnPaneToggleRequested(Microsoft.UI.Xaml.Controls.TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void NavFrame_Navigated(object sender, NavigationEventArgs e)
    {
        AppTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        //ThemeService.ChangeThemeWithoutSave(App.MainWindow);
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        _ = args.QueryText;

        App.SearchQuickString = args.QueryText;

        NavFrame.Navigate(typeof(SearchPage));
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        //var viewModel = NavFrame.GetPageViewModel();
   //     if (viewModel != null && viewModel is ITitleBarAutoSuggestBoxAware titleBarAutoSuggestBoxAware)
        //{
        //    titleBarAutoSuggestBoxAware.OnAutoSuggestBoxTextChanged(sender, args);
        //}
    }

    private void SearchSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SearchSearchBox.Visibility = Visibility.Visible;
        SearchSearchBox.Focus(FocusState.Programmatic);
        SearchSearchBox.PlaceholderText = "SearchName";
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
                case "register":
                    await Register();
                    break;
                case "changepw":
                    await ChangePassword();
                    break;
                case "forgotpw":
                    await ForgotPassword();
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
                    _ = App.RequestCloseAsync();
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

        App.RaiseLanguageChanged(languageOverride); // alle VMs informieren

        // 3. UI neu laden color in header
        ChangeLanguageLoginReset();
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

    //----------------------------------------------------

    private async Task LogIn()
    {
        var loc = Localizer.Get();

        var loginView = new LoginView();
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = loc.GetLocalizedString("Login"),
            Content = loginView,
            CloseButtonText = loc.GetLocalizedString("Close")
        };

        loginView.ViewModel.LoginSucceeded += (_, _) => dialog.Hide();

        await dialog.ShowAsync();

        EnforceCurrentPagePermission();

        var userService = App.GetService<IUserService>();
        if (!string.IsNullOrWhiteSpace(userService.CurrentUser))
        {
            TbUser.Foreground = new SolidColorBrush(Colors.Green);
            TbUser.Text = userService.CurrentUser + " " + loc.GetLocalizedString("Status_Login_User_Successful");
        }
        else
        {
            TbUser.Text = loc.GetLocalizedString("Status_No_User_Logged_In");
        }
    }
    private async Task Register()
    {
        var loc = Localizer.Get();

        var registerView = new RegisterView();
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = loc.GetLocalizedString("Register"),
            Content = registerView,
            CloseButtonText = loc.GetLocalizedString("Close")
        };

        registerView.ViewModel.RegisterSucceeded += (_, _) => dialog.Hide();

        await dialog.ShowAsync();
    }

    private async Task ForgotPassword()
    {
        var loc = Localizer.Get();

        var view = new ForgotPasswordView();
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = loc.GetLocalizedString("ForgotPassword"),
            Content = view,
            CloseButtonText = loc.GetLocalizedString("Close")
        };

        await dialog.ShowAsync();
    }

    private async Task ChangePassword()
    {
        var loc = Localizer.Get();

        var view = new ChangePasswordView();
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = loc.GetLocalizedString("ChangePassword"),
            Content = view,
            CloseButtonText = loc.GetLocalizedString("Close")
        };

        view.ViewModel.PasswordChangedSucceeded += (_, _) => dialog.Hide();

        await dialog.ShowAsync();
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
        App.GetService<IUserService>().Clear();
        TbUser.Text = "keiner angemeldet";

        EnforceCurrentPagePermission();

        return Task.CompletedTask;
    }

    private static void SaveCulture(string cult)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings["Culture"].Value = CultureInfo.GetCultureInfoByIetfLanguageTag(cult).ToString();
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("AppSettings");
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
            NavFrame.Navigate(typeof(HomePage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavSearch.Content)
        {
            NavFrame.Navigate(typeof(SearchPage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavSpecies.Content)
        {
            NavFrame.Navigate(typeof(Tbl75SpeciesPage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavGenus.Content)
        {
            NavFrame.Navigate(typeof(Tbl66GenusPage));
            return;
        }

        if ((string)args.ChosenSuggestion == (string)NavSpeciesgroup.Content)
        {
            NavFrame.Navigate(typeof(Tbl68SpeciesgroupPage));
            return;
        }

        //-------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfratribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl63InfratribusPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubtribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl60SubtribusPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavTribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl57TribusPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSupertribus.Content)
        {
            NavFrame.Navigate(typeof(Tbl54SupertribusPage));
        }

        //-------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfrafamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl51InfrafamilyPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl48SubfamilyPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavFamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl45FamilyPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSuperfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl42SuperfamilyPage));
        }
        //-------------------------------------------

        if ((string)args.ChosenSuggestion == (string)NavInfrafamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl51InfrafamilyPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl48SubfamilyPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavFamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl45FamilyPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSuperfamily.Content)
        {
            NavFrame.Navigate(typeof(Tbl42SuperfamilyPage));
        }

        //----------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfraordo.Content)
        {
            NavFrame.Navigate(typeof(Tbl39InfraordoPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubordo.Content)
        {
            NavFrame.Navigate(typeof(Tbl36SubordoPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavOrdo.Content)
        {
            NavFrame.Navigate(typeof(Tbl33OrdoPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavLegio.Content)
        {
            NavFrame.Navigate(typeof(Tbl30LegioPage));
        }

        //--------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavInfraclass.Content)
        {
            NavFrame.Navigate(typeof(Tbl27InfraclassPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSubclass.Content)
        {
            NavFrame.Navigate(typeof(Tbl24SubclassPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavClass.Content)
        {
            NavFrame.Navigate(typeof(Tbl21ClassPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSuperclass.Content)
        {
            NavFrame.Navigate(typeof(Tbl18SuperclassPage));
        }

        //-------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavSubphylum.Content)
        {
            NavFrame.Navigate(typeof(Tbl12SubphylumPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavPhylum.Content)
        {
            NavFrame.Navigate(typeof(Tbl06PhylumPage));
        }

        //---------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavRegnum.Content)
        {
            NavFrame.Navigate(typeof(Tbl03RegnumPage));
        }

        //---------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavNames.Content)
        {
            NavFrame.Navigate(typeof(Tbl78NamePage));
        }

        if ((string)args.ChosenSuggestion == (string)NavImages.Content)
        {
            NavFrame.Navigate(typeof(Tbl81ImagePage));
        }

        if ((string)args.ChosenSuggestion == (string)NavSynonyms.Content)
        {
            NavFrame.Navigate(typeof(Tbl84SynonymPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavGeographics.Content)
        {
            NavFrame.Navigate(typeof(Tbl87GeographicPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavReferences.Content)
        {
           NavFrame.Navigate(typeof(Tbl90ReferencePage));
        }

        //------------------------------------------
        if ((string)args.ChosenSuggestion == (string)NavRefAuthors.Content)
        {
            NavFrame.Navigate(typeof(Tbl90RefAuthorPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavRefExperts.Content)
        {
            NavFrame.Navigate(typeof(Tbl90RefExpertPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavRefSources.Content)
        {
            NavFrame.Navigate(typeof(Tbl90RefSourcePage));
        }

        if ((string)args.ChosenSuggestion == (string)NavComments.Content)
        {
            NavFrame.Navigate(typeof(Tbl93CommentPage));
        }

        //-----------------------------------------------

        if ((string)args.ChosenSuggestion == (string)NavCountries.Content)
        {
            NavFrame.Navigate(typeof(TblCountryPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavUserProfiles.Content)
        {
            NavFrame.Navigate(typeof(TblUserProfilePage));
        }

        if ((string)args.ChosenSuggestion == (string)NavDiseases1.Content)
        {
            NavFrame.Navigate(typeof(TblDiseasePage));
        }

        if ((string)args.ChosenSuggestion == (string)NavFoods1.Content)
        {
            NavFrame.Navigate(typeof(TblFoodPage));
        }

        if ((string)args.ChosenSuggestion == (string)NavMiscellaneousses1.Content)
        {
            NavFrame.Navigate(typeof(TblMiscellaneousPage));
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

    #region [ PERMISSION ROLES ]

    // zentral: Tag -> erlaubte Rollen
    private static readonly Dictionary<string, string[]> PageRoleRules = new(StringComparer.OrdinalIgnoreCase)
    {
        // Beispiel: nur Admin + Developer
        ["ATIS.Views.Modules.Tbl75Species.Tbl75SpeciesPage"] = new[] { "Administrator", "Developer" },

        // weitere Ausnahmen...
        // ["ATIS.Views.Modules.XYZ.SomePage"] = new[] { "Administrator" },
    };

    // Standardrollen für alle Module-Seiten ohne Sonderregel
    private static readonly string[] DefaultModuleRoles =
    {
        "Administrator", "Developer", "Zoologist", "Biologist"
    };

    private async void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            NavFrame.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            return;
        }

        if (args.InvokedItemContainer?.Tag is not string tag || string.IsNullOrWhiteSpace(tag))
            return;

        var newPage = Type.GetType(tag);
        if (newPage is null) return;

        var isModulePage = tag.Contains(".Views.Modules.", StringComparison.OrdinalIgnoreCase);

        if (isModulePage)
        {
            // hat diese Seite eine Sonderregel?
            var allowedRoles = PageRoleRules.TryGetValue(tag, out var roles)
                ? roles
                : DefaultModuleRoles;

            if (!Permission.HasAccess(allowedRoles))
            {
                await dialog.ShowErrorAsync("Error_NoPermission_Title", "Error_NoPermission_Message");
                NavFrame.Navigate(typeof(HomePage));
                return;
            }
        }

        NavFrame.Navigate(newPage, null, args.RecommendedNavigationTransitionInfo);
    }

    private void EnforceCurrentPagePermission()
    {
        if (NavFrame.Content is not Page currentPage) return;

        var pageType = currentPage.GetType();
        var fullName = pageType.FullName ?? string.Empty;

        var isModulePage = fullName.Contains(".Views.Modules.", StringComparison.OrdinalIgnoreCase);
        if (!isModulePage) return;

        var allowedRoles = PageRoleRules.TryGetValue(fullName, out var roles)
            ? roles
            : DefaultModuleRoles;

        if (!Permission.HasAccess(allowedRoles))
        {
            // sichere Zielseite ohne Spezialrechte:
            NavFrame.Navigate(typeof(HomePage)); // oder StartPage/Dashboard
        }
    }
    private void NavigationView_Loaded(object sender, RoutedEventArgs e)
    {
        //  NavFrame.Navigate(typeof(HomePage));
        // NICHT navigieren!
        NavView.SelectedItem = NavHome;
    }
    #endregion
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
