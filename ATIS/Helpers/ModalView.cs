using Microsoft.UI.Xaml.Data;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Helpers;

public static class ModalView
{
    private static async void ButtonShowMessageDialog_Click(object sender, RoutedEventArgs e)
    {

        var dialog = new Windows.UI.Popups.MessageDialog(
            "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
            "Morbi iaculis augue vel elementum volutpat.",
            "Lorem Ipsum");

        dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
        dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

        if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
        {
            // Adding a 3rd command will crash the app when running on Mobile !!!
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Maybe later") { Id = 2 });
        }

        dialog.DefaultCommandIndex = 0;
        dialog.CancelCommandIndex = 1;

        var result = await dialog.ShowAsync();

        var btn = sender as Button;
        btn!.Content = $"Result: {result.Label} ({result.Id})";
    }

    private static async void ButtonShowContentDialog1_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        var dialog = new ContentDialog()
        {
            Title = "Lorem Ipsum",
            //RequestedTheme = ElementTheme.Dark,
            //FullSizeDesired = true,
            //     MaxWidth = ActualWidth // Required for Mobile!
        };

        // Setup Content
        var panel = new StackPanel();

        panel.Children.Add(new TextBlock
        {
            Text = "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
                   "Morbi iaculis augue vel elementum volutpat.",
            TextWrapping = TextWrapping.Wrap,
        });

        var cb = new CheckBox
        {
            Content = "Agree"
        };

        cb.SetBinding(CheckBox.IsCheckedProperty, new Binding
        {
            Source = dialog,
            Path = new PropertyPath("IsPrimaryButtonEnabled"),
            Mode = BindingMode.TwoWay,
        });

        panel.Children.Add(cb);
        dialog.Content = panel;

        // Add Buttons
        dialog.PrimaryButtonText = "OK";
        dialog.IsPrimaryButtonEnabled = false;
        dialog.PrimaryButtonClick += delegate {
            btn!.Content = "Result: OK";
        };

        dialog.SecondaryButtonText = "Cancel";
        dialog.SecondaryButtonClick += delegate {
            btn!.Content = "Result: Cancel";
        };

        // Show Dialog
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.None)
        {
            btn!.Content = "Result: NONE";
        }
    }

    private static async void ButtonShowContentDialog2_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        var dialog = new ContentDialog()
        {
            Title = "Lorem Ipsum",
            //RequestedTheme = ElementTheme.Dark,
            //FullSizeDesired = true,
            //  MaxWidth = this.ActualWidth // Required for Mobile!
        };

        var panel = new StackPanel();

        panel.Children.Add(new TextBlock
        {
            Text = "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
                   "Morbi iaculis augue vel elementum volutpat.",
            TextWrapping = TextWrapping.Wrap,
        });

        var cb = new CheckBox
        {
            Content = "Agree"
        };
        panel.Children.Add(cb);
        dialog.Content = panel;

        // The CanExecute of the Command does not enable/disable the button :-(
        dialog.PrimaryButtonText = "OK";
        var cmd = new RelayCommand(() => {
            btn!.Content = "Result: OK";
        }, () => cb.IsChecked ?? false);

        dialog.PrimaryButtonCommand = cmd;

        dialog.SecondaryButtonText = "Cancel";
        dialog.SecondaryButtonCommand = new RelayCommand(() => {
            btn!.Content = "Result: Cancel";
        });

        cb.Click += delegate {
            cmd.NotifyCanExecuteChanged();
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.None)
        {
            btn!.Content = "Result: NONE";
        }
    }

    //---------------------------------------------
    public static async Task MessageDialogAsync(this FrameworkElement element, string title, string message)
    {
        await MessageDialogAsync(element, title, message, "OK");
    }

    public static async Task MessageDialogAsync(this FrameworkElement element, string title, string message, string buttonText)
    {
        if (element == null)
        {
            // throw new ArgumentNullException(nameof(element));
            return;
        }

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = buttonText,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = element!.XamlRoot,
            RequestedTheme = element.ActualTheme
        };

        await dialog.ShowAsync();
    }

    public static async Task<bool?> MessageDialogAsync(this FrameworkElement element, string title, string message, string yesButtonText, string noButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            DefaultButton = ContentDialogButton.Secondary,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return null;
        }

        return (result == ContentDialogResult.Primary);
    }

    public static async Task<bool?> ConfirmationDialogAsync(this FrameworkElement element, string title)
    {
        return await ConfirmationDialogAsync(element, title, "OK", string.Empty, "Cancel");
    }

    public static async Task<bool?> ConfirmationDialogAsync(this FrameworkElement element, string title, string yesButtonText, string noButtonText)
    {
        return (await ConfirmationDialogAsync(element, title, yesButtonText, noButtonText, string.Empty))!.Value;
    }

    public static async Task<bool?> ConfirmationDialogAsync(this FrameworkElement element, string title, string message, string yesButtonText, string noButtonText, string cancelButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            DefaultButton = ContentDialogButton.Secondary,
            CloseButtonText = string.Empty,
            XamlRoot = element!.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return null;
        }

        return result == ContentDialogResult.Primary;
    }

    public static async Task<bool?> ConfirmationDialogAsync(this FrameworkElement element, string title, string yesButtonText, string noButtonText, string cancelButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            DefaultButton = ContentDialogButton.Secondary,
            CloseButtonText = cancelButtonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return null;
        }

        return result == ContentDialogResult.Primary;
    }


    public static async Task<string> InputStringDialogAsync(this FrameworkElement element, string title)
    {
        return await element.InputStringDialogAsync(title, string.Empty);
    }

    public static async Task<string> InputStringDialogAsync(this FrameworkElement element, string title, string defaultText)
    {
        return await element.InputStringDialogAsync(title, defaultText, "OK", "Cancel");
    }

    public static async Task<string> InputStringDialogAsync(this FrameworkElement element, string title, string defaultText, string okButtonText, string cancelButtonText)
    {
        var inputTextBox = new TextBox
        {
            AcceptsReturn = false,
            Height = 32,
            Text = defaultText,
            SelectionStart = defaultText.Length
        };
        var dialog = new ContentDialog
        {
            Content = inputTextBox,
            Title = title,
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = okButtonText,
            SecondaryButtonText = cancelButtonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            return inputTextBox.Text;
        }
        else
        {
            return string.Empty;
        }
    }

    public static async Task<string> InputTextDialogAsync(this FrameworkElement element, string title)
    {
        return await element.InputTextDialogAsync(title, string.Empty);
    }

    public static async Task<string> InputTextDialogAsync(this FrameworkElement element, string title, string defaultText)
    {
        var inputTextBox = new TextBox
        {
            AcceptsReturn = true,
            Height = 32 * 6,
            Text = defaultText,
            TextWrapping = TextWrapping.Wrap,
            SelectionStart = defaultText.Length
        };
        var dialog = new ContentDialog
        {
            Content = inputTextBox,
            Title = title,
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = "Ok",
            SecondaryButtonText = "Cancel",
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            return inputTextBox.Text;
        }
        else
        {
            return string.Empty;
        }
    }
}
