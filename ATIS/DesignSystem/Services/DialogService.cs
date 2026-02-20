using ATIS;
using WinUI3Localizer;

public class DialogService
{
    public async Task ShowMessageAsync(string titleKey, string messageKey)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            Content = loc.GetLocalizedString(messageKey),
            CloseButtonText = loc.GetLocalizedString("Dialog_Ok"),
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };

        await dialog.ShowAsync();
    }

    public async Task ShowErrorAsync(string titleKey, string messageKey)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            Content = loc.GetLocalizedString(messageKey),
            CloseButtonText = loc.GetLocalizedString("Dialog_Ok"),
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    public async Task<bool> ShowYesNoAsync(string titleKey, string messageKey)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            Content = loc.GetLocalizedString(messageKey),
            PrimaryButtonText = loc.GetLocalizedString("Dialog_Yes"),
            CloseButtonText = loc.GetLocalizedString("Dialog_No"),
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<bool> ShowOkCancelAsync(string titleKey, string messageKey)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            Content = loc.GetLocalizedString(messageKey),
            PrimaryButtonText = loc.GetLocalizedString("Dialog_Ok"),
            CloseButtonText = loc.GetLocalizedString("Dialog_Cancel"),
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<bool> ShowConfirmAsync(string messageKey)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString("ConfirmTitle"),
            Content = loc.GetLocalizedString(messageKey),
            PrimaryButtonText = loc.GetLocalizedString("Dialog_Yes"),
            CloseButtonText = loc.GetLocalizedString("Dialog_No"),
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    // 🔥 WICHTIG: Generischer Dialog für DetailControls
    public async Task<bool> ShowDetailDialogAsync(FrameworkElement content, string titleKey)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            PrimaryButtonText = loc.GetLocalizedString("Dialog_Save"),
            CloseButtonText = loc.GetLocalizedString("Dialog_Cancel"),
            DefaultButton = ContentDialogButton.Primary,
            Content = content,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}
