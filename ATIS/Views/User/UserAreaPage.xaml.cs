using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Authentication;
using ATIS.Views.ContentDialogs;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.User;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class UserAreaPage : Page
{
    #region [Private Data Members]

    public UserAreaViewModel ViewModel { get; set; }
    private static readonly AtisDbContext Context = new();
    public string _search;
    private readonly string _name;

    #endregion [Private Data Members]   

    #region [Constructor]

    public UserAreaPage()
    {
        var customPrincipal = (CustomPrincipal)Thread.CurrentPrincipal;
        if (customPrincipal == null)
        {
            return;
        }
        _name = customPrincipal.Identity.Email;
        if (_name == "")
        {
            _ = AllDialogs.NobodyLoggedInSelectedWarnMessageDialogAsync();
            return;
        }

        var role = customPrincipal.Identity.Role;

        if (role == "Administrator" || role == "User" && _name == customPrincipal.Identity.Email)
        {
            ViewModel = App.GetService<UserAreaViewModel>();
            InitializeComponent();
            GetUserProfilesByName();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync(_name).ConfigureAwait(false);
        }
    }

    #endregion [Constructor]

    #region [UserProfile Methods]

    private async void GetUserProfilesByName()
    {
        try
        {

            //_search = args.QueryText;
            //ViewModel.IsLoading = true;


            ViewModel.TblUserProfilesList?.Clear();
            //    ViewModel.UserProfileItems.Clear();

            //const int milliseconds = 1500;
            //await Task.Delay(milliseconds);

            if (_name == "")
            {
                //          ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets


            ViewModel.TblUserProfilesList ??= [];
            //ViewModel.TblUserProfilesList = int.TryParse(_search, out var id)
            //    ? new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
            //        .Where(e => e.UserProfileId == id))
            //    : new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
            //        .Where(e => e.Email.StartsWith(_search))
            //        .OrderBy(a => a.Email));

            ViewModel.TblUserProfilesList = new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
                .Where(e => e.Email == _name));

            //   UserProfileDetailExpander.Visibility = Visibility.Collapsed;


            if (ViewModel.TblUserProfilesList is { Count: 0 })
            {
                ViewModel.TblUserProfilesList?.Clear();
                //AppBarButtonDeleteUserProfile.IsEnabled = false;
                //AppBarButtonSync.IsEnabled = false;

                DetailPanelUserProfile.Visibility = Visibility.Visible;
                //    UserProfileDetailExpander.Visibility = Visibility.Collapsed;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
            }
            else
            {
                //        UserProfileDataGrid.ItemsSource = ViewModel.TblUserProfilesList;
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

    private async void SaveUserProfile_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveUserProfile(ViewModel.UserProfileSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }

    private async Task SaveUserProfile(TblUserProfile userprofile)
    {
        if (ViewModel.IsNewUserProfile)
        {
            ViewModel.IsNewUserProfile = false;
        }

        TblUserProfile result = null;
        try
        {
            var current = await Context.TblUserProfiles.FirstOrDefaultAsync(a => a.UserProfileId == userprofile.UserProfileId);
            if (null == current)
            {
                userprofile.CountId = RandomHelper.Randomnumber();
                userprofile.Writer = Environment.UserName;
                userprofile.WriterDate = DateTime.Now;
                userprofile.Updater = Environment.UserName;
                userprofile.UpdaterDate = DateTime.Now;
                Context.TblUserProfiles.Add(userprofile);
            }
            else
            {
                userprofile.Updater = Environment.UserName;
                userprofile.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(userprofile);
            }
            await Context.SaveChangesAsync();

            result = userprofile;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingUserProfile = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [UserProfile Methods]
}
