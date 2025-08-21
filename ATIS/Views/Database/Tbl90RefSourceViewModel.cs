using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl90RefSourceViewModel Skriptdatum:   06.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl90RefSourceViewModel(IDataService _dataService) : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    public ObservableCollection<Tbl90RefSource> RefSourceItems { get; } = [];

    #endregion [Private Data Members]           

    //    Part 1    

    #region [Methods RefSource]
    public async Task AddRefSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditRefSource = true;
            IsNewRefSource = true;

            Tbl90RefSourcesList ??= [];
            Tbl90RefSourcesList.Insert(index: 0, item: new Tbl90RefSource {});
        });
    }

    public async Task SaveRefSourceAsync(Tbl90RefSource viewModelRefSourceSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewRefSource)
            {
                IsNewRefSource = false;
            }
        });

        Tbl90RefSource result = null;
        try
        {
            if (IsModifiedRefSource)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertRefSourceAsync(viewModelRefSourceSelected);
                    IsModifiedRefSource = false;
                });
            }
            else
            {
                IsModifiedRefSource = true;
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
            IsInEditRefSource = false;
            IsModifiedRefSource = false;
            IsExistingRefSource = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteRefSourceAsync(Tbl90RefSource viewModelRefSourceSelected)
    {
        Tbl90RefSource result = null;
        try
        {
            var deleteRefSourceId = viewModelRefSourceSelected.RefSourceId;
            await DeleteRefSource(deleteRefSourceId);

            result = viewModelRefSourceSelected;
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
            IsInEditRefSource = false;
            IsModifiedRefSource = false;
            IsExistingRefSource = true;
            IsDeletingRefSource = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteRefSource(int refSourceid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var refSource = await Context.Tbl90RefSources.FirstOrDefaultAsync(b => b.RefSourceId == refSourceid);
            if (refSource != null)
            {
                Context.Tbl90RefSources.Remove(refSource);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods RefSource ]  



    //    Part 2    

    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedRefSource { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => RefSourceItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingRefSource;
    public bool IsDeletingRefSource
    {
        get => _isDeletingRefSource;
        set => SetProperty(ref _isDeletingRefSource, value);
    }

    //------------IsNew---------------------------------------
    private bool _isNewRefSource;
    public bool IsNewRefSource
    {
        get => _isNewRefSource;
        set => SetProperty(ref _isNewRefSource, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingRefSource;
    public bool IsExistingRefSource
    {
        get => _isExistingRefSource;
        set => SetProperty(ref _isExistingRefSource, value);
    }


    //-----------IsInEdit----------------------------------------
    private bool _isInEditRefSource = false;
    public bool IsInEditRefSource
    {
        get => _isInEditRefSource;
        set => SetProperty(ref _isInEditRefSource, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditRefSource()
    {
        IsInEditRefSource = true;
        IsModifiedRefSource = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditRefSource() => await SaveRefSourceAsync(RefSourceSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshRefSource => !IsModifiedRefSource && IsExistingRefSource;
    //--------------CanRevert-------------------------------------
    public bool CanRevertRefSource => IsModifiedRefSource && IsExistingRefSource;

    #endregion [Propertys]           

    #region Selected Properties

    private Tbl90RefSource _refSourceSelected;
    public Tbl90RefSource RefSourceSelected
    {
         get => _refSourceSelected;
         set => SetProperty(ref _refSourceSelected, value);
    }

    #endregion Selected Properties             

    #region Public Properties

    private int _refSourceDataSetCount;

    public int RefSourceDataSetCount
    {
        get => _refSourceDataSetCount;
        set
        {
            _refSourceDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchRefSourceName = "";

    public string SearchRefSourceName
    {
        get => _searchRefSourceName;
        set
        {
            _searchRefSourceName = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl90RefSource> _tbl90RefSourcesList;
    public ObservableCollection<Tbl90RefSource> Tbl90RefSourcesList
    {
        get => _tbl90RefSourcesList;
        set
        {
            _tbl90RefSourcesList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties         

    #region Refresh Methods

    public void RefreshRefSourceItems()
    {
        RefSourceItems.Clear();
        if (Tbl90RefSourcesList != null)
        {
            foreach (var item in Tbl90RefSourcesList)
            {
                RefSourceItems.Add(item);
            }

            if (Tbl90RefSourcesList.Count == 0)
            {
                return;
            }

            if (RefSourceSelected == null && Tbl90RefSourcesList.Count != 0)
            {
                RefSourceSelected = RefSourceItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties     

}
