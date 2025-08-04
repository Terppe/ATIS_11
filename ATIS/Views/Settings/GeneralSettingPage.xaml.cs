using System.Globalization;
using System.Windows.Input;
using ATIS.Helpers;
using Windows.System;
using WinUI3Localizer;
using RelayCommand = WinUICommunity.RelayCommand;

namespace ATIS.Views.Settings;

public sealed partial class GeneralSettingPage : Page
{
    public string BreadCrumbBarItemText { get; set; }

    public GeneralSettingViewModel ViewModel { get; }
    public GeneralSettingPage()
    {
        ViewModel = App.GetService<GeneralSettingViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }

    private void CmbBackdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.GetThemeService.OnBackdropComboBoxSelectionChanged(sender);
    }

    private void CmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.GetThemeService.OnThemeComboBoxSelectionChanged(sender);
    }
    private async void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }
        catch (Exception ex)
        {
            //throw; // TODO handle exception
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
        }
    }
    //-------------------------------
    private RelayCommand _changeCulUsaCommand;
    public ICommand ChangeCulUsaCommand => _changeCulUsaCommand ??= new RelayCommand(ChangeCultureUsa);

    private static void ChangeCultureUsa()
    {
        App.Cult = "en-US";
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        Localizer.Get().SetLanguage("en-US");
    }
    //----------------------------------------------------------------
    private RelayCommand _changeCulGermanCommand;
    public ICommand ChangeCulGermanCommand => _changeCulGermanCommand ??= new RelayCommand(ChangeCultureGerman);

    private static void ChangeCultureGerman()
    {
        App.Cult = "de-DE";
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        Localizer.Get().SetLanguage("de-DE");
    }

    //-----------------------------------------------------------

    private RelayCommand _changeCulFrenchCommand;
    public ICommand ChangeCulFrenchCommand => _changeCulFrenchCommand ??= new RelayCommand(ChangeCultureFrench);

    private static void ChangeCultureFrench()
    {
        App.Cult = "fr-FR";
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        Localizer.Get().SetLanguage("fr-FR");
    }
    //----------------------------------------------------------------
    private RelayCommand _changeCulSpainCommand;
    public ICommand ChangeCulSpainCommand => _changeCulSpainCommand ??= new RelayCommand(ChangeCultureSpain);


    private static void ChangeCultureSpain()
    {
        App.Cult = "sp-SP";
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        Localizer.Get().SetLanguage("sp-SP");
    }
    //----------------------------------------------------------------
    //private RelayCommand _changeCulPortuguiseCommand;
    //public ICommand ChangeCulPortuguiseCommand => _changeCulPortuguiseCommand ??= new RelayCommand(ChangeCulturePortuguise);


    //private static void ChangeCulturePortuguise()
    //{
    //  //  App.ChangeLanguage("pt-PT");
    //}


}


