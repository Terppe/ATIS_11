using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    TblUserProfileViewModel Skriptdatum:   06.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class TblUserProfileViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<TblUserProfile> UserProfileItems { get; } = [];

    #endregion [Private Data Members]           

    #region [Constructor]
    public TblUserProfileViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        TblCountriesAllList ??= [];
        TblCountriesAllList = [.. _dataService.GetCountryAsync()];
    }

    #endregion [Constructor]

    //    Part 1    

    #region [Methods UserProfile]

    public async Task AddUserProfileAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditUserProfile = true;
            IsNewUserProfile = true;

            TblUserProfilesList ??= [];
            TblUserProfilesList.Insert(index: 0, item: new TblUserProfile {});
        });
    }

    public async Task SaveUserProfileAsync(TblUserProfile viewModelUserProfileSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewUserProfile)
            {
                IsNewUserProfile = false;
            }
        });

        TblUserProfile result = null;
        try
        {
            if (IsModifiedUserProfile)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertUserProfileAsync(viewModelUserProfileSelected);
                    IsModifiedUserProfile = false;
                });
            }
            else
            {
                IsModifiedUserProfile = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }

        if (result != null)
        {
            IsInEditUserProfile = false;
            IsModifiedUserProfile = false;
            IsExistingUserProfile = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteUserProfileAsync(TblUserProfile viewModelUserProfileSelected)
    {
        TblUserProfile result = null;
        try
        {
            var deleteUserProfileId = viewModelUserProfileSelected.UserProfileId;
            await DeleteUserProfile(deleteUserProfileId);

            result = viewModelUserProfileSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            IsInEditUserProfile = false;
            IsModifiedUserProfile = false;
            IsExistingUserProfile = true;
            IsDeletingUserProfile = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteUserProfile(int userprofileid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var userprofile = await Context.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofileid);
            if (userprofile != null)
            {
                Context.TblUserProfiles.Remove(userprofile);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods UserProfile ]  

    //    Part 2    

    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedUserProfile { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => UserProfileItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingUserProfile;
    public bool IsDeletingUserProfile
    {
        get => _isDeletingUserProfile;
        set => SetProperty(ref _isDeletingUserProfile, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewUserProfile;
    public bool IsNewUserProfile
    {
        get => _isNewUserProfile;
        set => SetProperty(ref _isNewUserProfile, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingUserProfile;
    public bool IsExistingUserProfile
    {
        get => _isExistingUserProfile;
        set => SetProperty(ref _isExistingUserProfile, value);
    }


    //-----------IsInEdit----------------------------------------
    private bool _isInEditUserProfile = false;
    public bool IsInEditUserProfile
    {
        get => _isInEditUserProfile;
        set => SetProperty(ref _isInEditUserProfile, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditUserProfile()
    {
        IsInEditUserProfile = true;
        IsModifiedUserProfile = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditUserProfile() => await SaveUserProfileAsync(UserProfileSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshUserProfile => !IsModifiedUserProfile && IsExistingUserProfile;
    //--------------CanRevert-------------------------------------
    public bool CanRevertUserProfile => IsModifiedUserProfile && IsExistingUserProfile;

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

    private int _userProfileDataSetCount;
    public int UserProfileDataSetCount
    {
        get => _userProfileDataSetCount;
        set
        {
            _userProfileDataSetCount = value; OnPropertyChanged();
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
    private ObservableCollection<TblUserProfile> _tblUserProfilesAllList;

    public ObservableCollection<TblUserProfile> TblUserProfilesAllList
    {
        get => _tblUserProfilesAllList;
        set
        {
            _tblUserProfilesAllList = value; OnPropertyChanged();
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
