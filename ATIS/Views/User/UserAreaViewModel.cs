using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.User;
public partial class UserAreaViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly IDataService _dataService;
    public ObservableCollection<TblUserProfile> UserProfileItems { get; } = [];
    #endregion [Private Data Members]

    #region [Constructor]
    public UserAreaViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        TblCountriesAllList ??= [];
        // Fix: Await the Task and use the result for the collection
        TblCountriesAllList = new ObservableCollection<TblCountry>(_dataService.GetCountryAsync().GetAwaiter().GetResult());
    }


    #endregion [Constructor]

    #region [Methods UserProfile]


    public async Task SaveUserProfileAsync()
    {
        if (IsNewUserProfile)
        {
            IsNewUserProfile = false;
        }

        TblUserProfile result = null;
        try
        {
            if (IsModified)
            {
                result = await _dataService.UpsertUserProfileAsync(UserProfileSelected);
                IsModified = false;
            }
            else
            {
                IsModified = true;
            }
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
            IsInEdit = false;
            IsModified = false;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    //public static async Task DeleteUserProfileAsync(int userprofileid)
    //{
    //    var userprofile = await Context.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofileid);
    //    if (userprofile != null)
    //    {
    //        Context.TblUserProfiles.Remove(userprofile);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    public Task RefreshUserProfileAsync()
    {
        TblUserProfilesList ??= [];
        TblUserProfilesList = _dataService.GetUserProfileAsync();

        RefreshUserProfileItems();
        return Task.CompletedTask;
    }

    public async Task RevertChangesAsync()
    {
        IsInEdit = false;
        if (IsModified)
        {
            await RefreshUserProfileAsync();
            IsModified = false;
        }
    }

    public void CancelEditsAsync()
    {
        if (IsNewUserProfile)
        {
            AddNewUserProfileCanceled?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            RevertChangesAsync().ConfigureAwait(false);
        }
    }

    #endregion [Methods UserProfile ]  

    //    Part 2    

    #region All Properties

    #region [Propertys]

    public bool IsModified { get; set; }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isDeleting;
    public bool IsDeleting
    {
        get => _isDeleting;
        set => SetProperty(ref _isDeleting, value);
    }

    private bool IsLoaded => UserProfileItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isNewUserProfile;

    public bool IsNewUserProfile
    {
        get => _isNewUserProfile;
        set => SetProperty(ref _isNewUserProfile, value);
    }
    private bool _isExistingUserProfile;

    public bool IsExistingUserProfile
    {
        get => _isExistingUserProfile;
        set => SetProperty(ref _isExistingUserProfile, value);
    }

    private bool _isInEdit = false;
    public bool IsInEdit
    {
        get => _isInEdit;
        set => SetProperty(ref _isInEdit, value);
    }
    public void StartEdit()
    {
        IsInEdit = true;
        IsModified = false;
    }
    public static void BeginEdit()
    {
        // Not used.
    }
    public void CancelEdit() => CancelEditsAsync();
    public async void EndEdit() => await SaveUserProfileAsync();
    public event EventHandler AddNewUserProfileCanceled;
    public bool CanRefresh => !IsModified && IsExistingUserProfile;
    public bool CanRevert => IsModified && IsExistingUserProfile;

    #endregion [Propertys]

    #region Selected Properties

    private TblUserProfile _userprofileSelected;

    public TblUserProfile UserProfileSelected
    {
        get => _userprofileSelected;
        set => SetProperty(ref _userprofileSelected, value);
    }

    #endregion Selected Properties     


    #region Public Properties  

    private int _userprofileDataSetCount;

    public int UserProfileDataSetCount
    {
        get => _userprofileDataSetCount;
        set
        {
            _userprofileDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchUserProfileName = "";

    public string SearchUserProfileName
    {
        get => _searchUserProfileName;
        set
        {
            _searchUserProfileName = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblUserProfile> _tblUserProfilesList;

    public ObservableCollection<TblUserProfile> TblUserProfilesList
    {
        get => _tblUserProfilesList;
        set
        {
            _tblUserProfilesList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<TblCountry> _tblCountriesAllList;
    public ObservableCollection<TblCountry> TblCountriesAllList
    {
        get => _tblCountriesAllList;
        set
        {
            _tblCountriesAllList = value; OnPropertyChanged();
        }
    }


    #endregion Public Properties

    #region Refresh Methods


    public void RefreshUserProfileItems()
    {
        UserProfileItems.Clear();
        if (TblUserProfilesList != null)
        {
            foreach (var item in TblUserProfilesList)
            {
                UserProfileItems.Add(item);
            }

            if (TblUserProfilesList.Count == 0)
            {
                return;
            }

            if (UserProfileSelected == null && TblUserProfilesList.Count != 0)
            {
                UserProfileSelected = UserProfileItems.FirstOrDefault();
            }
        }
    }
    #endregion Refresh Methods

    #endregion All Properties

}
