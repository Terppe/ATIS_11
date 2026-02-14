using ATIS.Services;
using WinUI3Localizer;

namespace ATIS.DesignSystem.Services;

public class DialogService
{
    public async Task ShowMessageAsync(string titleKey, string messageKey)
    {
        var title = LocalizationManager.Get(titleKey);
        var message = LocalizationManager.Get(messageKey);

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = LocalizationManager.Get("Dialog_OK"),
            XamlRoot = App.MainRoot.XamlRoot
        };

        await dialog.ShowAsync();
    }
}

//public class DialogService(ILocalizer loc)
//{
//    //   private readonly ILocalizationService _loc;

// public async Task ShowMessageAsync(string titleKey, string messageKey)
//    {
//        var dialog = new ContentDialog
//        {
//            Title = loc.GetLocalizedString(titleKey),
//            Content = loc.GetLocalizedString(messageKey),
//            PrimaryButtonText = loc.GetLocalizedString("Dialog_OK"),
//            XamlRoot = App.MainWindow.Content.XamlRoot
//        };

//        await dialog.ShowAsync();
//    }

//    public async Task<bool> ShowOkCancelAsync(string titleKey, string messageKey)
//    {
//        var dialog = new ContentDialog
//        {
//            Title = loc.GetLocalizedString(titleKey),
//            Content = loc.GetLocalizedString(messageKey),
//            PrimaryButtonText = loc.GetLocalizedString("Dialog_OK"),
//            CloseButtonText = loc.GetLocalizedString("Dialog_Cancel"),
//            XamlRoot = App.MainWindow.Content.XamlRoot
//        };

//        var result = await dialog.ShowAsync();
//        return result == ContentDialogResult.Primary;
//    }

//    public async Task<bool> ShowYesNoAsync(string titleKey, string messageKey)
//    {
//        var dialog = new ContentDialog
//        {
//            Title = loc.GetLocalizedString(titleKey),
//            Content = loc.GetLocalizedString(messageKey),
//            PrimaryButtonText = loc.GetLocalizedString("Dialog_Yes"),
//            CloseButtonText = loc.GetLocalizedString("Dialog_No"),
//            XamlRoot = App.MainWindow.Content.XamlRoot
//        };

//        var result = await dialog.ShowAsync();
//        return result == ContentDialogResult.Primary;
//    }
//}
