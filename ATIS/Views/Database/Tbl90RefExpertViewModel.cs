using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using ATIS.Infrastructure;

//    Tbl90RefExpertViewModel Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl90RefExpertViewModel(IDataService _dataService) : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _data;
    public ObservableCollection<Tbl90RefExpert> RefExpertItems { get; } = [];

    #endregion [Private Data Members]           


    //    Part 1    

    #region [Methods RefExpert]
    public async Task AddRefExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditRefExpert = true;
            IsNewRefExpert = true;

            Tbl90RefExpertsList ??= [];
            Tbl90RefExpertsList.Insert(index: 0, item: new Tbl90RefExpert { });
        });
    }

    public async Task SaveRefExpertAsync(Tbl90RefExpert viewModelRefExpertSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewRefExpert)
            {
                IsNewRefExpert = false;
            }
        });

        Tbl90RefExpert result = null;
        try
        {
            if (IsModifiedRefExpert)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertRefExpertAsync(viewModelRefExpertSelected);
                    IsModifiedRefExpert = false;
                });
            }
            else
            {
                IsModifiedRefExpert = true;
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
            IsInEditRefExpert = false;
            IsModifiedRefExpert = false;
            IsExistingRefExpert = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteRefExpertAsync(Tbl90RefExpert viewModelRefExpertSelected)
    {
        Tbl90RefExpert result = null;
        try
        {
            var deleteRefExpertId = viewModelRefExpertSelected.RefExpertId;
            await DeleteRefExpert(deleteRefExpertId);

            result = viewModelRefExpertSelected;
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
            IsInEditRefExpert = false;
            IsModifiedRefExpert = false;
            IsExistingRefExpert = true;
            IsDeletingRefExpert = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteRefExpert(int refExpertid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var refExpert = await AppData.Data.Tbl90RefExperts.FirstOrDefaultAsync(b => b.RefExpertId == refExpertid);
            if (refExpert != null)
            {
                //AppData.Data.Tbl90RefExperts.Remove(refExpert);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods RefExpert ]  

    //    Part 2    

    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedRefExpert { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => RefExpertItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingRefExpert;
    public bool IsDeletingRefExpert
    {
        get => _isDeletingRefExpert;
        set => SetProperty(ref _isDeletingRefExpert, value);
    }

    //------------IsNew---------------------------------------
    private bool _isNewRefExpert;
    public bool IsNewRefExpert
    {
        get => _isNewRefExpert;
        set => SetProperty(ref _isNewRefExpert, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingRefExpert;
    public bool IsExistingRefExpert
    {
        get => _isExistingRefExpert;
        set => SetProperty(ref _isExistingRefExpert, value);
    }


    //-----------IsInEdit----------------------------------------
    private bool _isInEditRefExpert = false;
    public bool IsInEditRefExpert
    {
        get => _isInEditRefExpert;
        set => SetProperty(ref _isInEditRefExpert, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditRefExpert()
    {
        IsInEditRefExpert = true;
        IsModifiedRefExpert = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditRefExpert() => await SaveRefExpertAsync(RefExpertSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshRefExpert => !IsModifiedRefExpert && IsExistingRefExpert;
    //--------------CanRevert-------------------------------------
    public bool CanRevertRefExpert => IsModifiedRefExpert && IsExistingRefExpert;

    #endregion [Propertys]           

    #region Selected Properties

    private Tbl90RefExpert _refExpertSelected;
    public Tbl90RefExpert RefExpertSelected
    {
         get => _refExpertSelected;
         set => SetProperty(ref _refExpertSelected, value);
    }

    #endregion Selected Properties             

    #region Public Properties

    private int _refExpertDataSetCount;

    public int RefExpertDataSetCount
    {
        get => _refExpertDataSetCount;
        set
        {
            _refExpertDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchRefExpertName = "";

    public string SearchRefExpertName
    {
        get => _searchRefExpertName;
        set
        {
            _searchRefExpertName = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl90RefExpert> _tbl90RefExpertsList;
    public ObservableCollection<Tbl90RefExpert> Tbl90RefExpertsList
    {
        get => _tbl90RefExpertsList;
        set
        {
            _tbl90RefExpertsList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties         

    #region Refresh Methods

    public void RefreshRefExpertItems()
    {
        RefExpertItems.Clear();
        if (Tbl90RefExpertsList != null)
        {
            foreach (var item in Tbl90RefExpertsList)
            {
                RefExpertItems.Add(item);
            }

            if (Tbl90RefExpertsList.Count == 0)
            {
                return;
            }

            if (RefExpertSelected == null && Tbl90RefExpertsList.Count != 0)
            {
                RefExpertSelected = RefExpertItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties     

}
