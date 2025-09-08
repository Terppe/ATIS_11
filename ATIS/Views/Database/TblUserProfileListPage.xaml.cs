
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  TblUserProfileListPage.xaml.cs Skriptdatum:   06.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class TblUserProfileListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public TblUserProfileViewModel ViewModel { get; }
    private TblUserProfile SingleUserProfile { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public TblUserProfileListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<TblUserProfileViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
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
                UserProfileDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingUserProfile = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblUserProfilesList ??= [];
                ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
                ? [.. Context.TblUserProfiles
                .Where(e => e.UserProfileId == id)]
                : [.. Context.TblUserProfiles
                .Where(e => e.Email.StartsWith(_search))
                .OrderBy(a => a.Email)];
            });

            UserProfileDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.UserProfileDataSetCount = ViewModel.TblUserProfilesList.Count;

            if (ViewModel.TblUserProfilesList is { Count: 0 })
            {
                ViewModel.TblUserProfilesList?.Clear();

                AppBarButtonDeleteUserProfile.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelUserProfile.Visibility = Visibility.Visible;
                UserProfileDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(TblUserProfileListPage));
            }
            else
            {
                UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
                ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
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
        UserProfileEmail.Loaded += (s, e) =>
        {
            UserProfileEmail.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditUserProfile = true;
        ViewModel.IsNewUserProfile = true;

        await ViewModel.AddUserProfileAsync();

        if (ViewModel.TblUserProfilesList != null)
        {
            ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[0];
        }

        UserProfileDetailExpander.IsExpanded = true;
        UserProfileDetailExpander.Visibility = Visibility.Visible;

        UserProfileEmail.PlaceholderText = StringsRes.EnterText;
    }

    private void UserProfileSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UserProfileSearchBox.Visibility = Visibility.Visible;
        UserProfileSearchBox.Focus(FocusState.Programmatic);
        UserProfileSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void UserProfileDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblUserProfilesList.Sort);
    }

    private void AddNewUserProfileCanceled(object sender, EventArgs e) => UserProfileDetailExpander.IsExpanded = false;

    private async void ViewUserProfileHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewUserProfileHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewUserProfileHyperlink()
    {
        ViewModel.IsInEditUserProfile = false;
        ViewModel.IsNewUserProfile = false;
        UserProfileDetailExpander.IsExpanded = true;
        UserProfileDetailExpander.Visibility = Visibility.Visible;

        if (UserProfileDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleUserProfile = App.SingleUserProfile(ViewModel.UserProfileSelected.UserProfileId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsUserProfileAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewUserProfile)
        {
            UserProfileDetailExpander.IsExpanded = false;
            UserProfileDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelUserProfileEdits();
        }
    }

    private void CancelUserProfileEdits()
    {
        var x = (DataGrid)FindName("UserProfileDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.UserProfileSelected = ViewModel.TblUserProfilesList[iNdx];
            SingleUserProfile = App.SingleUserProfile(ViewModel.UserProfileSelected.UserProfileId);
            UserProfileSingles();
        }
        else
        {
            UserProfileDetailExpander.IsExpanded = false;
            UserProfileDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditUserProfile = false;
        ViewModel.IsNewUserProfile = false;
    }

    private void UserProfileSingles()
    {
        ViewModel.UserProfileSelected.Country = SingleUserProfile.Country;
        ViewModel.UserProfileSelected.City = SingleUserProfile.City;
        ViewModel.UserProfileSelected.Role = SingleUserProfile.Role;
        ViewModel.UserProfileSelected.Memo = SingleUserProfile.Memo;
        ViewModel.UserProfileSelected.BirthDate = SingleUserProfile.BirthDate;
        ViewModel.UserProfileSelected.Business = SingleUserProfile.Business;
        ViewModel.UserProfileSelected.Colour = SingleUserProfile.Colour;
        ViewModel.UserProfileSelected.Company = SingleUserProfile.Company;
        ViewModel.UserProfileSelected.Email = SingleUserProfile.Email;
        ViewModel.UserProfileSelected.EndDate = SingleUserProfile.EndDate;
        ViewModel.UserProfileSelected.Fax = SingleUserProfile.Fax;
        ViewModel.UserProfileSelected.Filestream = SingleUserProfile.Filestream;
        ViewModel.UserProfileSelected.FirstName = SingleUserProfile.FirstName;
        ViewModel.UserProfileSelected.LastName = SingleUserProfile.LastName;
        ViewModel.UserProfileSelected.FilestreamId = SingleUserProfile.FilestreamId;
        ViewModel.UserProfileSelected.Flag = SingleUserProfile.Flag;
        ViewModel.UserProfileSelected.Gender = SingleUserProfile.Gender;
        ViewModel.UserProfileSelected.HomePageUrl = SingleUserProfile.HomePageUrl;
        ViewModel.UserProfileSelected.ImageMimeType = SingleUserProfile.ImageMimeType;
        ViewModel.UserProfileSelected.MailNewsletter = SingleUserProfile.MailNewsletter;
        ViewModel.UserProfileSelected.Known = SingleUserProfile.Known;
        ViewModel.UserProfileSelected.MaulHtml = SingleUserProfile.MaulHtml;
        ViewModel.UserProfileSelected.Mobil = SingleUserProfile.Mobil;
        ViewModel.UserProfileSelected.Signature = SingleUserProfile.Signature;
        ViewModel.UserProfileSelected.Notes = SingleUserProfile.Notes;
        ViewModel.UserProfileSelected.Street1 = SingleUserProfile.Street1;
        ViewModel.UserProfileSelected.Street2 = SingleUserProfile.Street2;
        ViewModel.UserProfileSelected.Title = SingleUserProfile.Title;
        ViewModel.UserProfileSelected.Tel = SingleUserProfile.Tel;
    }
    private async Task RevertChangesUserProfileAsync()
    {
        ViewModel.IsInEditUserProfile = false;
        if (ViewModel.IsModifiedUserProfile)
        {
            await RefreshUserProfileAsync();
            ViewModel.IsModifiedUserProfile = false;
        }
    }
    private Task RefreshUserProfileAsync()
    {
        if (UserProfileDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleUserProfile = App.SingleUserProfile(ViewModel.UserProfileSelected.UserProfileId);
            UserProfileSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteUserProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteUserProfile();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteUserProfile()
    {
        var deleteDialog = new DeleteDialog
        {
            PrimaryButtonText = StringsRes.Delete,
            SecondaryButtonText = StringsRes.DoNotDelete,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await ViewModel.DeleteUserProfileAsync(ViewModel.UserProfileSelected);
                await Refresh();
                UserProfileDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesUserProfileAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveUserProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveUserProfile();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveUserProfile()
    {
        var saveDialog = new SaveChangesDialog
        {
            PrimaryButtonText = StringsRes.Save,
            SecondaryButtonText = StringsRes.DoNotSave,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Save,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                if (await BeforeSaveUserProfile())
                {
                    return;
                }
                await ViewModel.SaveUserProfileAsync(ViewModel.UserProfileSelected);
                await BehindSaveUserProfile(ViewModel.UserProfileSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedUserProfile = true;
                await RevertChangesUserProfileAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveUserProfile()
    {
        _search = UserProfileSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblUserProfilesList ??= [];
            ViewModel.TblUserProfilesList = [.. Context.TblUserProfiles
            .Where(a => a.Email == ViewModel.UserProfileSelected.Email &&
                        a.FirstName == ViewModel.UserProfileSelected.FirstName &&
                        a.LastName == ViewModel.UserProfileSelected.LastName)];
        });

        if (ViewModel.IsNewUserProfile)
        {
            if (ViewModel.TblUserProfilesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedUserProfile = true;
        return false;
    }

    private async Task BehindSaveUserProfile(TblUserProfile viewModelUserProfileSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblUserProfilesList ??= [];
            if (UserProfileSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
                    ? [.. Context.TblUserProfiles
                    .Where(a => a.UserProfileId == id)]
                    : [.. Context.TblUserProfiles
                    .Where(a => _search != null && a.Email.StartsWith(_search))
                    .OrderBy(a => a.Email)];
            }
        });

        ViewModel.RefreshUserProfileItems();

        if (ViewModel.TblUserProfilesList.Count == 0)
        {
            ViewModel.TblUserProfilesList = [.. Context.TblUserProfiles
                .Where(a => a.UserProfileId == viewModelUserProfileSelected.UserProfileId)];
            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
            await Refresh();
            UserProfileDataGrid.SelectedIndex = 0;
        }
        else
        {
            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
            var index = ViewModel.UserProfileItems.ToList().FindIndex(item => item.UserProfileId == viewModelUserProfileSelected.UserProfileId);
            UserProfileDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.UserProfileDataSetCount = ViewModel.TblUserProfilesList.Count;
    }

    private void BackUserProfile_Click(object sender, RoutedEventArgs e)
    {
        UserProfileDetailExpander.IsExpanded = false;
        UserProfileDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [UserProfile Methods]                    

    private void BackDetailExpanders()
    {
        UserProfileDetailExpander.IsExpanded = false;
        UserProfileDetailExpander.Visibility = Visibility.Collapsed;
    }

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
            //DetailPanelUserProfile.Visibility = Visibility.Visible;
            //UserProfileDetailExpander.Visibility = Visibility.Visible;
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

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblUserProfilesList ??= [];
                ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
                ? [.. Context.TblUserProfiles
                .Where(a => a.UserProfileId == id)]
                : [.. Context.TblUserProfiles
                    .Where(a => a.Email.StartsWith(_search))
                    .OrderBy(a => a.Email)];
            });
            if (ViewModel.TblUserProfilesList is { Count: 0 })
            {
                ViewModel.TblUserProfilesList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;

            if (ViewModel.IsDeletingUserProfile)
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
        ViewModel.IsDeletingUserProfile = false;
    }

    #endregion [Refresh Methods] 


}

