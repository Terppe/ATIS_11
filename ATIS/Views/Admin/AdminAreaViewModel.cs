using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.System;
using ATIS.Helpers;
using ATIS.Infrastructure;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using RelayCommand = WinUICommunity.RelayCommand;

namespace ATIS.Views.Admin;
public partial class AdminAreaViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<TblUserProfile> UserProfileItems { get; } = [];

    #endregion [Private Data Members]           

    #region [Constructor]
    public AdminAreaViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        TblCountriesAllList ??= [];
        TblCountriesAllList = [.. _dataService.GetCountryAsync()];
    }

    public AdminAreaViewModel()
    {

    }

    #endregion [Constructor]

    //    Part 1    

    #region [Methods UserProfile]

    public async Task AddUserProfileAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEdit = true;
            IsNewUserProfile = true;

            TblUserProfilesList ??= [];
            TblUserProfilesList.Insert(index: 0, item: new TblUserProfile { Role = "User", Password = "0424"});
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
            if (IsModified)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertUserProfileAsync(viewModelUserProfileSelected);
                    IsModified = false;
                });
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
            IsInEdit = false;
            IsModified = false;
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
            IsInEdit = false;
            IsModified = false;
            IsExistingUserProfile = true;
            IsDeleting = true;
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
            var userprofile = await AppData.Data.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofileid);
            if (userprofile != null)
            {
                //AppData.Data.TblUserProfiles.Remove(userprofile);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }
    private async Task RefreshUserProfileAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            TblUserProfilesList ??= [];
            TblUserProfilesList = _dataService.GetUserProfileAsync();

            RefreshUserProfileItems();
        });
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


    #region [Methods Comment]
    public ICommand MailAccountPasswordCommand => new RelayCommand(execute: delegate { var task = MailAccountPw(); });


    private Task MailAccountPw()
    {
        _ = Crypt.EncryptString(MailAccountPassword);
        return Task.CompletedTask;
    }
    private string _mailAccountPassword;
    public string MailAccountPassword
    {
        get => _mailAccountPassword;
        set { _mailAccountPassword = value; OnPropertyChanged(); }
    }

    #endregion [Methods Comment]

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
    public async void EndEdit() => await SaveUserProfileAsync(UserProfileSelected);
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
