using System.Configuration;
using ATIS.Helpers;
using ATIS.Infrastructure;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Auth;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Admin;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AdminAreaPage : Page
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public AdminAreaViewModel ViewModel { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;
    #endregion [Private Data Members]

    #region [Constructor]

    public AdminAreaPage()
    {
        var customPrincipal = (CustomPrincipal)Thread.CurrentPrincipal;
        if (customPrincipal == null)
        {
            return;
        }
        var name = customPrincipal.Identity.Email;
        if (name == "")
        {
            _ = AllDialogs.NobodyLoggedInSelectedWarnMessageDialogAsync();
            return;
        }

        var role = customPrincipal.Identity.Role;

        if (role == "Administrator")
        {
            InitializeComponent();
            Loaded += OnLoaded;

        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync(name).ConfigureAwait(false);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel = App.GetService<AdminAreaViewModel>();
    }

    #endregion [Constructor]

    #region [UserProfile Methods]

    private async void GetUserProfilesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblUserProfilesList?.Clear();

            ViewModel.UserProfileItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblUserProfilesList?.Clear();

                UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;

                ViewModel.IsLoading = false;
                ViewModel.IsDeleting = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            //await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblUserProfilesList ??= [];
                ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.TblUserProfiles
                        .Where(a => a.UserProfileId == id)]
                    : [.. AppData.Data.TblUserProfiles
                        .Where(a => _search != null && a.Email.StartsWith(_search))
                        .OrderBy(a => a.Email)];
            }
            //);

            UserProfileDetailExpander.Visibility = Visibility.Collapsed;

            if (ViewModel.TblUserProfilesList is { Count: 0 })
            {
                ViewModel.TblUserProfilesList?.Clear();

                AppBarButtonDeleteUserProfile.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelUserProfile.Visibility = Visibility.Visible;
                UserProfileDetailExpander.Visibility = Visibility.Collapsed;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
            }
            else
            {
                UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
                //fehler
                ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.InnerException?.ToString());
            SimpleLog.Log(e);
        }
    }

    private async void UserProfileDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await UserProfileDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task UserProfileDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.UserProfileSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        ViewModel.IsLoading = false;
    }

    private async void AddUserProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddUserProfile();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddUserProfile()
    {
        ViewModel.IsInEdit = true;
        ViewModel.IsNewUserProfile = true;

        await ViewModel.AddUserProfileAsync();

        if (ViewModel.TblUserProfilesList != null)
        {
            ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];
        }

        UserProfileDetailExpander.IsExpanded = true;
        UserProfileDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewUserProfile = false;
    }

    private void UserProfileSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UserProfileSearchBox.Visibility = Visibility.Visible;
        UserProfileSearchBox.Focus(FocusState.Programmatic);
        UserProfileSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void UserProfileDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
      //  (sender as DataGrid).Sort(e.Column, ViewModel.TblUserProfilesList.Sort);
    }

    private void AddNewUserProfileCanceled(object sender, EventArgs e) => UserProfileDetailExpander.IsExpanded = false;

    private void ViewUserProfileHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsInEdit = false;
        ViewModel.IsNewUserProfile = false;
        UserProfileDetailExpander.IsExpanded = true;
        UserProfileDetailExpander.Visibility = Visibility.Visible;
    }

    private async void DeleteUserProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
          //  await DeleteUserProfile();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    //private async Task DeleteUserProfile()
    //{
    //    var deleteDialog = new DeleteDialog
    //    {
    //        Title = StringsRes.Delete,
    //        XamlRoot = Content.XamlRoot
    //    };
    //    await deleteDialog.ShowAsync();
    //    var res = deleteDialog.Result;

    //    switch (res)
    //    {
    //        case DeleteDialogResult.Delete:
    //            await ViewModel.DeleteUserProfileAsync(ViewModel.UserProfileSelected);
    //            await Refresh();
    //            break;
    //        case DeleteDialogResult.DontDelete:
    //            await ViewModel.RevertChangesAsync();
    //            break;
    //        case DeleteDialogResult.Cancel:
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }

    //}

    private async void SaveUserProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
          //  await SaveUserProfile();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    //private async Task SaveUserProfile()
    //{
    //    var saveDialog = new SaveChangesDialog
    //    {
    //        Title = StringsRes.SaveChanges,
    //        XamlRoot = Content.XamlRoot
    //    };
    //    await saveDialog.ShowAsync();
    //    var res = saveDialog.Result;

    //    switch (res)
    //    {
    //        case SaveChangesDialogResult.Save:
    //            if (await BeforeSaveUserProfile())
    //            {
    //                return;
    //            }
    //            await ViewModel.SaveUserProfileAsync(ViewModel.UserProfileSelected);
    //            await BehindSaveUserProfile();
    //            break;
    //        case SaveChangesDialogResult.DontSave:
    //            await ViewModel.RevertChangesAsync();
    //            break;
    //        case SaveChangesDialogResult.Cancel:
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}

    private async Task<bool> BeforeSaveUserProfile()
    {
        _search = UserProfileSearchBox.Text;
      //  await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblUserProfilesList ??= [];
            ViewModel.TblUserProfilesList = [.. AppData.Data.TblUserProfiles
                .Where(a => a.Email == ViewModel.UserProfileSelected.Email)];
        }
        //);
        if (ViewModel.IsNewUserProfile)
        {
            if (ViewModel.TblUserProfilesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModified = true;
        return false;
    }

    private async Task BehindSaveUserProfile()
    {
        if (string.IsNullOrEmpty(UserProfileSearchBox.Text))
        {
       //     await _dispatcherQueue.EnqueueAsync(async () =>
            {
                ViewModel.TblUserProfilesList ??= [];
                ViewModel.TblUserProfilesList = [.. AppData.Data.TblUserProfiles
                    .Where(a => a.UserProfileId == ViewModel.UserProfileSelected.UserProfileId)];

                if (ViewModel.TblUserProfilesList.Count == 0)
                {
                    await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                    return;
                }
                UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
                ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];

                ////string cut last Char
                //var length = ViewModel.FamilySelected.FamilyName.Length;
                //var org = ViewModel.FamilySelected.FamilyName;
                //var cutorg = org.Remove(length - 1, 1);
                //FamilySearchBox.Text = cutorg;
            }
            //);
        }
        await Refresh();
    }

    private void BackUserProfile_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [UserProfile Methods]                    


    //#region [UserProfile Methods]

    //private async void GetUserProfilesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    //{
    //    try
    //    {
    //        _search = args.QueryText;

    //        ViewModel.IsLoading = true;

    //        ViewModel.TblUserProfilesList?.Clear();

    //        ViewModel.UserProfileItems.Clear();

    //        const int milliseconds = 1500;
    //        await Task.Delay(milliseconds);

    //        if (string.IsNullOrEmpty(_search))
    //        {
    //            ViewModel.TblUserProfilesList?.Clear();

    //            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
    //            UserProfileDetailExpander.Visibility = Visibility.Collapsed;

    //            ViewModel.IsLoading = false;
    //            ViewModel.IsDeleting = false;
    //            await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
    //            return;
    //        }

    //        //if _search = "" und count = 0 you see the whole Tab thousands of datasets
    //        //change to % for all Datasets in 20 Parts of sets

    //        await _dispatcherQueue.EnqueueAsync(() =>
    //        {
    //            ViewModel.TblUserProfilesList ??= [];
    //            ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
    //            ? [.. Context.TblUserProfiles
    //                .Where(e => e.UserProfileId == id)]
    //            : [.. Context.TblUserProfiles
    //                .Where(e => e.Email.StartsWith(_search))
    //                .OrderBy(a => a.Email)];
    //        });

    //        UserProfileDetailExpander.Visibility = Visibility.Collapsed;

    //        if (ViewModel.TblUserProfilesList is { Count: 0 })
    //        {
    //            ViewModel.TblUserProfilesList?.Clear();

    //            AppBarButtonDeleteUserProfile.IsEnabled = false;
    //            AppBarButtonSync.IsEnabled = false;

    //            DetailPanelUserProfile.Visibility = Visibility.Visible;
    //            UserProfileDetailExpander.Visibility = Visibility.Collapsed;
    //            await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
    //        }
    //        else
    //        {
    //            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
    //            ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];
    //        }
    //        ViewModel.IsLoading = false;
    //    }
    //    catch (Exception xe)
    //    {
    //        //hier wird Fehler ausgelöst aber ungeklärt
    //                 await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

    //private async void UserProfileDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        await UserProfileDataGridCurrentCellChanged();
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

    //private async Task UserProfileDataGridCurrentCellChanged()
    //{
    //    ViewModel.IsLoading = true;

    //    if (ViewModel.UserProfileSelected == null)
    //    {
    //        ViewModel.IsLoading = false;
    //        return;
    //    }

    //    //delay loading
    //    const int milliseconds = 1500;
    //    await Task.Delay(milliseconds);
    //    ViewModel.IsLoading = false;

    //}

    //private async void AddUserProfile_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        await AddUserProfile();
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

    //private async Task AddUserProfile()
    //{
    //    ViewModel.IsInEdit = true;
    //    ViewModel.IsNewUserProfile = true;

    //    await ViewModel.AddUserProfileAsync();

    //    if (ViewModel.TblUserProfilesList != null)
    //    {
    //        ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];
    //    }

    //    UserProfileDetailExpander.IsExpanded = true;
    //    UserProfileDetailExpander.Visibility = Visibility.Visible;

    //    ViewModel.IsNewUserProfile = false;

    //}

    //private void UserProfileSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    //{
    //    UserProfileSearchBox.Visibility = Visibility.Visible;
    //    UserProfileSearchBox.Focus(FocusState.Programmatic);
    //    UserProfileSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    //}

    //private void UserProfileDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    //{
    //    (sender as DataGrid).Sort(e.Column, ViewModel.TblUserProfilesList.Sort);
    //}

    //private void AddNewUserProfileCanceled(object sender, EventArgs e) => UserProfileDetailExpander.IsExpanded = false;

    //private void ViewUserProfileHyperlinkButton_Click(object sender, RoutedEventArgs e)
    //{
    //    ViewModel.IsInEdit = false;
    //    ViewModel.IsNewUserProfile = false;
    //    UserProfileDetailExpander.IsExpanded = true;
    //    UserProfileDetailExpander.Visibility = Visibility.Visible;
    //}

    //private async void DeleteUserProfile_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        await DeleteUserProfile();
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

    //private async Task DeleteUserProfile()
    //{
    //    var deleteDialog = new DeleteDialog
    //    {
    //        Title = StringsRes.Delete,
    //        XamlRoot = Content.XamlRoot
    //    };
    //    await deleteDialog.ShowAsync();
    //    var res = deleteDialog.Result;

    //    switch (res)
    //    {
    //        case DeleteDialogResult.Delete:
    //            await ViewModel.DeleteUserProfileAsync(ViewModel.UserProfileSelected);
    //            await Refresh();
    //            break;
    //        case DeleteDialogResult.DontDelete:
    //            await ViewModel.RevertChangesAsync();
    //            break;
    //        case DeleteDialogResult.Cancel:
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }

    //}

    //private async void SaveUserProfile_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        await SaveUserProfile();
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

    //private async Task SaveUserProfile()
    //{
    //    var saveDialog = new SaveChangesDialog
    //    {
    //        Title = StringsRes.SaveChanges,
    //        XamlRoot = Content.XamlRoot
    //    };
    //    await saveDialog.ShowAsync();
    //    var res = saveDialog.Result;

    //    switch (res)
    //    {
    //        case SaveChangesDialogResult.Save:
    //            if (await BeforeSaveUserProfile())
    //            {
    //                return;
    //            }
    //            await ViewModel.SaveUserProfileAsync(ViewModel.UserProfileSelected);
    //            await BehindSaveUserProfile();
    //            break;
    //        case SaveChangesDialogResult.DontSave:
    //            await ViewModel.RevertChangesAsync();
    //            break;
    //        case SaveChangesDialogResult.Cancel:
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }

    //}

    //private async Task<bool> BeforeSaveUserProfile()
    //{
    //    _search = UserProfileSearchBox.Text;
    //    await _dispatcherQueue.EnqueueAsync(() =>
    //    {
    //        ViewModel.TblUserProfilesList ??= [];
    //        ViewModel.TblUserProfilesList = [.. Context.TblUserProfiles
    //            .Where(a => a.Email == ViewModel.UserProfileSelected.Email &&
    //                        a.FirstName == ViewModel.UserProfileSelected.FirstName &&
    //                        a.LastName == ViewModel.UserProfileSelected.LastName)];
    //    });

    //    if (ViewModel.IsNewUserProfile)
    //    {
    //        if (ViewModel.TblUserProfilesList.Count != 0)
    //        {
    //            await AllDialogs.DatasetExistInfoMessageDialogAsync();
    //            return true;
    //        }
    //    }
    //    ViewModel.IsModified = true;
    //    return false;
    //}

    //private async Task BehindSaveUserProfile()
    //{
    //    if (string.IsNullOrEmpty(UserProfileSearchBox.Text))
    //    {
    //        await _dispatcherQueue.EnqueueAsync(async () =>
    //        {
    //            ViewModel.TblUserProfilesList ??= [];
    //            ViewModel.TblUserProfilesList = [.. Context.TblUserProfiles
    //                .Where(a => a.UserProfileId == ViewModel.UserProfileSelected.UserProfileId)];

    //            if (ViewModel.TblUserProfilesList.Count == 0)
    //            {
    //                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
    //                return;
    //            }
    //            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
    //            ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];

    //            ////string cut last Char
    //            //var length = ViewModel.UserProfileSelected.Email.Length;
    //            //var org = ViewModel.UserProfileSelected.Email;
    //            //var cutorg = org.Remove(length - 1, 1);
    //            //UserProfileSearchBox.Text = cutorg;
    //        });
    //    }
    //    await Refresh();
    //}

    //private void BackUserProfile_Click(object sender, RoutedEventArgs e)
    //{
    //    UserProfileDetailExpander.IsExpanded = false;
    //    UserProfileDetailExpander.Visibility = Visibility.Collapsed;
    //}

    //#endregion [UserProfile Methods]                    

    #region [Refresh Methods]
    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await Refresh();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task Refresh()
    {
        ViewModel.IsLoading = true;

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (string.IsNullOrEmpty(_search))
        {
            DetailPanelUserProfile.Visibility = Visibility.Visible;
            UserProfileDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("UserProfileDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.TblUserProfilesList!.Count)
            {
                ViewModel.UserProfileItems.Clear();
                foreach (var item in ViewModel.TblUserProfilesList)
                {
                    ViewModel.UserProfileItems.Add(item);
                }

                ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[iNdx];
            }
            UserProfileDataGrid.SelectedIndex = iNdx;

         //   await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblUserProfilesList ??= [];
                ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.TblUserProfiles
                            .Where(a => a.UserProfileId == id)]
                    : [.. AppData.Data.TblUserProfiles
                            .Where(a => a.Email.StartsWith(_search))
                            .OrderBy(a => a.Email)];
            }
            //);
            if (ViewModel.TblUserProfilesList is { Count: 0 })
            {
                ViewModel.TblUserProfilesList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;

            if (ViewModel.IsDeleting)
            {
                if (iNdx == 0 && ViewModel.TblUserProfilesList.Count == 0)
                {
                    ViewModel.TblUserProfilesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[iNdx];
            }
        }
        UserProfileDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeleting = false;

    }

    private void BackDetailExpanders()
    {
        UserProfileDetailExpander.IsExpanded = false;
        UserProfileDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


    private void BtnEncrypt_OnClick(object sender, RoutedEventArgs e)
    {
        if (TbMailAccountPassword != null && TbMailAccountPassword.Text != "")
        {
            TbEncryptMailAccountPassword.Text = Crypt.EncryptString(TbMailAccountPassword.Text);
        }
    }


    private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
    {
        if (await AllDialogs.SaveResultQuestionConfirmationDialogAsync())
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["EncryptMailAccountPassword"].Value = TbEncryptMailAccountPassword.Text;

            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Full);

            ConfigurationManager.RefreshSection("AppSettings");
        }
    }
}
