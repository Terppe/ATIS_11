using ATIS.Interfaces;
using WinUI3Localizer;

namespace ATIS.Services;

public class DialogService() : IDialogService
{
    private readonly XamlRoot _xamlRoot;

    public DialogService(XamlRoot xamlRoot) : this()
    {
        _xamlRoot = xamlRoot ?? throw new ArgumentNullException(nameof(xamlRoot));
    }

    public async Task ShowOkDialogAsync(string titleKey, string messageKey)
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

    public static async Task ShowErrorDialogAsync(string titleKey, string messageKey)
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

    public async Task<bool> ShowConfirmationDialogAsync(string titleKey, string messageKey)
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

    public async Task<bool> ShowActionDialogAsync(string titleKey, string messageKey)
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

    public async Task ShowInfoAsync(string titleKey, string messageKey)
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


    public async Task<bool> ShowConfirmAsync(string titleKey, string messageKey)
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
    public async Task<T?> ShowDetailDialogWithResultAsync<T>(string titleKey, FrameworkElement content, Func<T> onSave)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            PrimaryButtonText = loc.GetLocalizedString("Dialog_Save"),
            CloseButtonText = loc.GetLocalizedString("Dialog_Cancel"),
            DefaultButton = ContentDialogButton.Primary,
            Content = content,
            XamlRoot = _xamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? onSave() : default;
    }
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
    public async Task<bool> ShowConfirmWithTimeoutAsync(string titleKey, string messageKey, int timeoutMilliseconds)
    {
        var loc = Localizer.Get();

        var dialog = new ContentDialog
        {
            Title = loc.GetLocalizedString(titleKey),
            Content = loc.GetLocalizedString(messageKey),
            PrimaryButtonText = loc.GetLocalizedString("Dialog_Yes"),
            CloseButtonText = loc.GetLocalizedString("Dialog_No"),
            XamlRoot = _xamlRoot
        };

        using var cts = new CancellationTokenSource(timeoutMilliseconds);
        var task = dialog.ShowAsync().AsTask(cts.Token);

        try
        {
            var result = await task;
            return result == ContentDialogResult.Primary; // `true`, falls Yes gedrückt
        }
        catch (TaskCanceledException)
        {
            return false; // Timeout
        }
    }
}
