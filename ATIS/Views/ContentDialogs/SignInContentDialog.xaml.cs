using Microsoft.UI.Xaml.Controls.Primitives;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.ContentDialogs;
public enum SignInResult
{
    SignInOK,
    SignInFail,
    SignInCancel,
    Nothing
}

public sealed partial class SignInContentDialog : ContentDialog
{
    public SignInResult Result { get; private set; }

    public SignInContentDialog()
    {
        InitializeComponent();
        Opened += SignInContentDialog_Opened;
        Closing += SignInContentDialog_Closing;
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Ensure the user name and password fields aren't empty. If a required field
        // is empty, set args.Cancel = true to keep the dialog open.
        if (string.IsNullOrEmpty(UserNameTextBox.Text))
        {
            args.Cancel = true;
            ErrorTextBlock.Text = "User name is required.";
        }
        else if (string.IsNullOrEmpty(PasswordTextBox.Password))
        {
            args.Cancel = true;
            ErrorTextBlock.Text = "Password is required.";
        }

        // If you're performing async operations in the button click handler,
        // get a deferral before you await the operation. Then, complete the
        // deferral when the async operation is complete.

        //ContentDialogButtonClickDeferral deferral = args.GetDeferral();
        //if (await SomeAsyncSignInOperation())
        //{
        //    this.Result = SignInResult.SignInOK;
        //}
        //else
        //{
        //    this.Result = SignInResult.SignInFail;
        //}
        //deferral.Complete();
    }

    private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // User clicked Cancel, ESC, or the system back button.
        this.Result = SignInResult.SignInCancel;
    }

    void SignInContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        Result = SignInResult.Nothing;

        // If the user name is saved, get it and populate the user name field.
        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        if (roamingSettings.Values.ContainsKey("userName"))
        {
            UserNameTextBox.Text = roamingSettings.Values["userName"].ToString();
            SaveUserNameCheckBox.IsChecked = true;
        }
    }

    void SignInContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        // If sign in was successful, save or clear the user name based on the user choice.
        if (Result == SignInResult.SignInOK)
        {
            if (SaveUserNameCheckBox.IsChecked == true)
            {
                SaveUserName();
            }
            else
            {
                ClearUserName();
            }
        }

        // If the user entered a name and checked or cleared the 'save user name' checkbox, then clicked the back arrow,
        // confirm if it was their intention to save or clear the user name without signing in.
        if (Result == SignInResult.Nothing && !string.IsNullOrEmpty(UserNameTextBox.Text))
        {
            if (SaveUserNameCheckBox.IsChecked == false)
            {
                args.Cancel = true;
                FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["DiscardNameFlyout"]);
                FlyoutBase.ShowAttachedFlyout(this);
            }
            else if (SaveUserNameCheckBox.IsChecked == true && !string.IsNullOrEmpty(UserNameTextBox.Text))
            {
                args.Cancel = true;
                FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["SaveNameFlyout"]);
                FlyoutBase.ShowAttachedFlyout(this);
            }
        }
    }

    private void SaveUserName()
    {
        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        roamingSettings.Values["userName"] = UserNameTextBox.Text;
    }

    private void ClearUserName()
    {
        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        roamingSettings.Values["userName"] = null;
        UserNameTextBox.Text = string.Empty;
    }

    // Handle the button clicks from the flyouts.
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveUserName();
        FlyoutBase.GetAttachedFlyout(this).Hide();
    }

    private void DiscardButton_Click(object sender, RoutedEventArgs e)
    {
        ClearUserName();
        FlyoutBase.GetAttachedFlyout(this).Hide();
    }

    // When the flyout closes, hide the sign in dialog, too.
    private void Flyout_Closed(object sender, object e)
    {
        this.Hide();
    }
}
