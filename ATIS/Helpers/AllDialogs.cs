using System.Data;
using ATIS.Strings;

namespace ATIS.Helpers;

public class AllDialogs
{
    public AllDialogs()
    {

    }

    #region [MessageDialogs]

    public static async Task NoDatasetFoundInfoMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.NoDatasetFound, "OK");
    }
    public static async Task ApplicationRunningInfoMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.ApplicationAlreadyRunning, "OK");
    }
    public static async Task SaveSuccessInfoMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.SaveSuccessful, "OK");
    }
    public static async Task LogInSuccessInfoMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.LoginSuccsessful, "OK");
    }
    public static async Task DatasetExistInfoMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.DatasetExist, "OK");
    }
    public static async Task NameRequiredWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NameRequired, "OK");
    }
    public static async Task ReferenceIncorrectWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.ReverencesDecisionNotCorrect, "OK");
    }
    public static async Task TooMuchIdsSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.SelectOneId, "OK");
    }
    public static async Task DatasetNotExistInfoMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.DatasetNotExist, "OK");
    }
    public static async Task NoDatasetSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoDatasetSelected, "OK");
    }
    public static async Task NoComboItemSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoComboboxItemSelected, "OK");
    }
    public static async Task NoCreatureItemSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoCreatureSelected, "OK");
    }
    public static async Task NoColumnSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoColumnSelected, "OK");
    }

    public static async Task NoServerConnectWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoServerSelected, "OK");
    }

    public static async Task NoPermissionWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoPermission, "OK");
    }

    public static async void NoDatasetFoundWarnMessageDialog()
    {
        await App.MainRoot.MessageDialogAsync(StringsRes.Warning,
            StringsRes.NoDatasetFound, "OK");
    }
    public static async void NoFileFoundInfoWarnDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(StringsRes.Warning,
            StringsRes.NoFileFound, "OK");
    }
    public static async Task NoImagePathSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoImagePathSelected, "OK");
    }

    public static async Task NobodyLoggedInSelectedWarnMessageDialogAsync()
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NobodyLoggedIn, "OK");
    }

    //-------------------------------------------------
    public static async Task WarningNoPermissionMessageDialogAsync(string datasetName)
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.NoPermissionForArea + " " + datasetName, "OK");
    }
    public static async Task WarningNoDatasetSelectedMessageDialogAsync(string datasetName)
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.DoNotDeleteDataset + " " + datasetName, "OK");
    }
    public static async Task InfoSuccessfulDeleteMessageDialogAsync(string datasetName)
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.Dataset + " " + datasetName + " " + StringsRes.DeleteSuccessful, "OK");
    }
    public static async Task InfoSuccessfulSaveMessageDialogAsync(string datasetName)
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Information,
            StringsRes.Dataset + datasetName + " " + StringsRes.SaveSuccessful, "OK");
    }
    public static async Task WarningNoDatabaseWithSqlServerSelectedMessageDialogAsync(string database)
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            StringsRes.DatabaseNotExistSQLServerExist + " " + database, "OK");
    }

    public static async Task ErrorMessageDialogAsync(string? message)
    {
        await App.MainRoot.MessageDialogAsync(
            StringsRes.Warning,
            message, "OK");
    }

    //-----------------------------------------------------------

    public static async Task<bool> DeleteDatasetQuestionConfirmationDialogAsync(string datasetName)
    {
        var result = await App.MainRoot.ConfirmationDialogAsync(StringsRes.Question,
            StringsRes.DeleteQuestion + " " + datasetName + " ?", StringsRes.Yes, StringsRes.No, "");
        return result != null && (bool)result;
    }

    //-----------------------------------------------------------

    public static async Task<bool> SaveDatasetQuestionConfirmationDialogAsync(string datasetName)
    {
        var result = await App.MainRoot.ConfirmationDialogAsync("",
            StringsRes.WantToSaveDataset + " " + datasetName + " ?", StringsRes.Yes, StringsRes.No, "");
        return result != null && (bool)result;
    }

    public static async Task<bool> SaveResultQuestionConfirmationDialogAsync()
    {
        var result = await App.MainRoot.ConfirmationDialogAsync("",
            StringsRes.WantToSave, StringsRes.Yes, StringsRes.No, "");
        return result != null && (bool)result;
    }

    public static async Task<bool> CloseAppQuestionConfirmationDialogAsync(string appName)
    {
        var result = await App.MainRoot.ConfirmationDialogAsync("",
            StringsRes.Cancel + " " + appName + " ?", StringsRes.Yes, StringsRes.No, "");
        return result != null && (bool)result;
    }

    //-----------------------------------------------------------
    public static async Task<bool> SaveQuestionConfirmationDialogAsync(string name)
    {
        var result = await App.MainRoot.ConfirmationDialogAsync("Save changes?",
            StringsRes.WantToSaveDataset + " " + name + " ?", StringsRes.Yes, "Don't save", "Cancel");
        return result != null && (bool)result;
    }

    public static async Task<bool> DeleteQuestionConfirmationDialogAsync(string dataset)
    {
        var result = await App.MainRoot.ConfirmationDialogAsync("Delete?",
            StringsRes.WantToSaveDataset + " " + dataset + " ?", StringsRes.Yes, "Don't delete", "Cancel");
        return result != null && (bool)result;
    }

    //-----------------------------------------------------------

    //public static void ErrorMessageBox(string message, string caption)
    //{
    //    //   MessageBoxEx.SetErrorDelegate(this.errorDelegate, true);
    //    //MessageBoxEx.Show("Show an error message. The icon is clickable.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    //    //var ext = new MsgBoxExtendedFunctionality()
    //    //{
    //    //    MessageDelegate = _errorDelegate
    //    //    ,
    //    //    ExitAfterAction = true
    //    //};
    //    //    MessageBoxEx.ShowEx("Show an error message. The icon is clickable.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, ext);

    //    //// show clickable error icon
    //    //MessageBoxEx.SetErrorDelegate(this._errorDelegate);
    //    //WpfMessageBox.Show(message + "Show an error message. The icon is clickable.",
    //    //    caption,
    //    //    MessageBoxButton.OK,
    //    //    System.Windows.MessageBoxImage.Error);
    //}

    //public static void DetailErrorMessageBox(string message, string caption)
    //{
    //    //if (_checkboxData != null && !_checkboxData.CheckBoxIsChecked)
    //    //{
    //    //    // show box with checkbox and details 
    //    //    //MessageBoxEx.ShowWithCheckBoxAndDetails("This is possibly pointless and can be permenantly dismissed by clicking the checkbox below." 
    //    //    //										, this.checkboxData, this.Testmsg(), "Checkbox and Details Sample"
    //    //    //										, MessageBoxButton.OK, MessageBoxImage.Information);

    //    //    var ext = new MsgBoxExtendedFunctionality()
    //    //    {
    //    //        CheckBoxData = _checkboxData
    //    //        ,
    //    //        DetailsText = Testmsg()
    //    //    };
    //    //    MessageBoxEx.ShowEx("This is possibly pointless and can be permenantly dismissed by clicking the checkbox below."
    //    //        , "Checkbox and Details Sample"
    //    //        , MessageBoxButton.OK, MessageBoxImage.Information, ext);
    //    //}
    //    //else
    //    //{
    //    //    WPFMessageBox.Show("But you said not to show the message anymore. Make up your mind.", "Hypocrite Notice");
    //    //}
    //}

    //public static void UrlMessageBox(string message, string caption)
    //{
    //    //_checkboxData.CheckBoxIsChecked = false;
    //    //var ext = new MsgBoxExtendedFunctionality()
    //    //{
    //    //    URL = new MsgBoxUrl()
    //    //    {
    //    //        URL = new Uri("http://www.google.com")
    //    //        ,
    //    //        DisplayName = "Google"
    //    //        ,
    //    //        Foreground = Colors.LightBlue
    //    //    }
    //    //};
    //    //MessageBoxEx.ShowEx("Example showing a clickable URL.", "URL Example", ext);

    //}

    #endregion [MessageDialogs]

}
