using ATIS.Helpers;
using Windows.System;

namespace ATIS.Views.Settings;

public sealed partial class ThemeSettingPage : Page
{
    public string BreadCrumbBarItemText { get; set; }
    public ThemeSettingViewModel ViewModel { get; }
    public ThemeSettingPage()
    {
        ViewModel = App.GetService<ThemeSettingViewModel>();
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

}


