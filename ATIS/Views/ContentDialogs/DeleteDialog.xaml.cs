

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.


namespace ATIS.Views.ContentDialogs;
public sealed partial class DeleteDialog : ContentDialog
{
    public DeleteDialog()
    {
        InitializeComponent();
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
    }
    /// <summary>
    /// Gets or sets the user's choice. 
    /// </summary>
    public DeleteDialogResult Result { get; private set; } = DeleteDialogResult.Cancel;

    /// <summary>
    /// Occurs when the user chooses to save. 
    /// </summary>
    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Result = DeleteDialogResult.Delete;
        Hide();
    }

    /// <summary>
    /// Occurs when the user chooses to discard changes.
    /// </summary>
    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Result = DeleteDialogResult.DontDelete;
        Hide();
    }

    /// <summary>
    /// Occurs when the user chooses to cancel the operation that triggered the event.
    /// </summary>
    private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Result = DeleteDialogResult.Cancel;
        Hide();
    }
}

/// <summary>
/// Defines the choices available to the user. 
/// </summary>
public enum DeleteDialogResult
{
    Delete,
    DontDelete,
    Cancel
}
