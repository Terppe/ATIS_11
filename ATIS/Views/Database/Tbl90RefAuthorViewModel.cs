using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl90RefAuthorViewModel Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl90RefAuthorViewModel(IDataService _dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    public ObservableCollection<Tbl90RefAuthor> RefAuthorItems { get; } = [];
    #endregion [Private Data Members]


    //    Part 1    

    #region [Methods RefAuthor]
    public async Task AddRefAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditRefAuthor = true;
            IsNewRefAuthor = true;

            Tbl90RefAuthorsList ??= [];
            Tbl90RefAuthorsList.Insert(index: 0, item: new Tbl90RefAuthor { });
        });
    }

    public async Task SaveRefAuthorAsync(Tbl90RefAuthor viewModelRefAuthorSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewRefAuthor)
            {
                IsNewRefAuthor = false;
            }
        });

        Tbl90RefAuthor result = null;
        try
        {
            if (IsModifiedRefAuthor)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertRefAuthorAsync(viewModelRefAuthorSelected);
                    IsModifiedRefAuthor = false;
                });
            }
            else
            {
                IsModifiedRefAuthor = true;
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
            IsInEditRefAuthor = false;
            IsModifiedRefAuthor = false;
            IsExistingRefAuthor = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteRefAuthorAsync(Tbl90RefAuthor viewModelRefAuthorSelected)
    {
        Tbl90RefAuthor result = null;
        try
        {
            var deleteRefAuthorId = viewModelRefAuthorSelected.RefAuthorId;
            await DeleteRefAuthor(deleteRefAuthorId);

            result = viewModelRefAuthorSelected;
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
            IsInEditRefAuthor = false;
            IsModifiedRefAuthor = false;
            IsExistingRefAuthor = true;
            IsDeletingRefAuthor = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteRefAuthor(int refAuthorid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var refAuthor = await Context.Tbl90RefAuthors.FirstOrDefaultAsync(b => b.RefAuthorId == refAuthorid);
            if (refAuthor != null)
            {
                Context.Tbl90RefAuthors.Remove(refAuthor);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods RefAuthor]

    //    Part 2    

    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedRefAuthor { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => RefAuthorItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingRefAuthor;
    public bool IsDeletingRefAuthor
    {
        get => _isDeletingRefAuthor;
        set => SetProperty(ref _isDeletingRefAuthor, value);
    }

    //------------IsNew---------------------------------------
    private bool _isNewRefAuthor;

    public bool IsNewRefAuthor
    {
        get => _isNewRefAuthor;
        set => SetProperty(ref _isNewRefAuthor, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingRefAuthor;
    public bool IsExistingRefAuthor
    {
        get => _isExistingRefAuthor;
        set => SetProperty(ref _isExistingRefAuthor, value);
    }
    //-----------IsInEdit----------------------------------------
    private bool _isInEditRefAuthor = false;
    public bool IsInEditRefAuthor
    {
        get => _isInEditRefAuthor;
        set => SetProperty(ref _isInEditRefAuthor, value);
    }

    //----------StartEdit-----------------------------------------
    public void StartEditRefAuthor()
    {
        IsInEditRefAuthor = true;
        IsModifiedRefAuthor = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditRefAuthor() => await SaveRefAuthorAsync(RefAuthorSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshRefAuthor => !IsModifiedRefAuthor && IsExistingRefAuthor;
    //--------------CanRevert-------------------------------------
    public bool CanRevertRefAuthor => IsModifiedRefAuthor && IsExistingRefAuthor;

    #endregion [Propertys]     


    #region Selected Properties

    private Tbl90RefAuthor _refAuthorSelected;

    public Tbl90RefAuthor RefAuthorSelected
    {
        get => _refAuthorSelected;
        set => SetProperty(ref _refAuthorSelected, value);
    }

    #endregion Selected Properties       

    #region Public Properties

    private int _refAuthorDataSetCount;
    public int RefAuthorDataSetCount
    {
        get => _refAuthorDataSetCount;
        set
        {
            _refAuthorDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchRefAuthorName = "";

    public string SearchRefAuthorName
    {
        get => _searchRefAuthorName;
        set
        {
            _searchRefAuthorName = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefAuthor> _tbl90RefAuthorsList;

    public ObservableCollection<Tbl90RefAuthor> Tbl90RefAuthorsList
    {
        get => _tbl90RefAuthorsList;
        set
        {
            _tbl90RefAuthorsList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods


    public void RefreshRefAuthorItems()
    {
        RefAuthorItems.Clear();
        if (Tbl90RefAuthorsList != null)
        {
            foreach (var item in Tbl90RefAuthorsList)
            {
                RefAuthorItems.Add(item);
            }

            if (Tbl90RefAuthorsList.Count == 0)
            {
                return;
            }

            if (RefAuthorSelected == null && Tbl90RefAuthorsList.Count != 0)
            {
                RefAuthorSelected = RefAuthorItems.FirstOrDefault()!;
            }
        }
    }
    #endregion Refresh Methods


    #endregion All Properties     

}
