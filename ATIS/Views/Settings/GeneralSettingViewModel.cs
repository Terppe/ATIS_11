using Microsoft.UI.Xaml.Media.Animation;

namespace ATIS.Views.Settings;
public partial class GeneralSettingViewModel(IJsonNavigationViewService jsonNavigationViewService) : ObservableObject
{
    public readonly IJsonNavigationViewService JsonNavigationViewService = jsonNavigationViewService;

    [RelayCommand]
    private void GoToGeneralSettingPage(object sender)
    {
        var item = sender as SettingsCard;
        if (item.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"ATIS.Views.Settings.{item.Tag}");

            if (pageType != null)
            {
                var entranceNavigation = new SlideNavigationTransitionInfo
                {
                    Effect = SlideNavigationTransitionEffect.FromRight
                };
                JsonNavigationViewService.NavigateTo(pageType, item.Header, false, entranceNavigation);
            }
        }
    }

}
