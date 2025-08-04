using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl68SpeciesgroupViewModel Skriptdatum:  10.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl68SpeciesgroupViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;

    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl68Speciesgroup> SpeciesgroupItems { get; } = [];
    public ObservableCollection<Tbl69FiSpecies> SpeciesItems { get; } = [];

    #endregion [Private Data Members]           

    #region [Constructor]
    public Tbl68SpeciesgroupViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl66GenussesAllList ??= [];
        Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];
        Tbl68SpeciesgroupsAllList ??= [];
        Tbl68SpeciesgroupsAllList = [.. _dataService.GetSpeciesgroupAsync()];
    }

    public Tbl68SpeciesgroupViewModel()
    {

    }
    #endregion [Constructor]  


    //    Part 1    



    #region [Methods Speciesgroup]

    public async Task AddSpeciesgroupAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditSpeciesgroup = true;
            IsNewSpeciesgroup = true;

            Tbl68SpeciesgroupsList ??= [];
            Tbl68SpeciesgroupsList.Insert(index: 0, item: new Tbl68Speciesgroup { });
        });
    }

    public async Task SaveSpeciesgroupAsync(Tbl68Speciesgroup viewModelSpeciesgroupSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSpeciesgroup)
            {
                IsNewSpeciesgroup = false;
            }
        });

        Tbl68Speciesgroup result = null;
        try
        {
            if (IsModifiedSpeciesgroup)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSpeciesgroupAsync(viewModelSpeciesgroupSelected);
                    IsModifiedSpeciesgroup = false;
                });
            }
            else
            {
                IsModifiedSpeciesgroup = true;
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
            IsInEditSpeciesgroup = false;
            IsModifiedSpeciesgroup = false;
            IsExistingSpeciesgroup = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteSpeciesgroupAsync(Tbl68Speciesgroup viewModelSpeciesgroupSelected)
    {
        Tbl68Speciesgroup result = null;
        try
        {
            var deleteSpeciesgroupId = viewModelSpeciesgroupSelected.SpeciesgroupId;
            await DeleteSpeciesgroup(deleteSpeciesgroupId);

            result = viewModelSpeciesgroupSelected;
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
            IsInEditSpeciesgroup = false;
            IsModifiedSpeciesgroup = false;
            IsExistingSpeciesgroup = true;
            IsDeletingSpeciesgroup = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteSpeciesgroup(int speciesgroupid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var speciesgroup =
                await Context.Tbl68Speciesgroups.FirstOrDefaultAsync(b => b.SpeciesgroupId == speciesgroupid);
            if (speciesgroup != null)
            {

                Context.Tbl68Speciesgroups.Remove(speciesgroup);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Speciesgroup ]  

    #region [Methods Species]

    public async Task AddSpeciesAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl66GenussesAllList ??= [];
            Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];

            var genusid = Tbl66GenussesAllList.First().GenusId;

            var speciesgroupId = SpeciesgroupSelected.SpeciesgroupId;

            Tbl75SpeciessesList ??= [];
            Tbl75SpeciessesList.Insert(index: 0, item: new Tbl69FiSpecies { GenusId = genusid, SpeciesgroupId = speciesgroupId });
        });
    }

    public async Task SaveSpeciesAsync(Tbl69FiSpecies viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSpecies)
            {
                IsNewSpecies = false;
            }
        });

        Tbl69FiSpecies result = null;
        try
        {
            if (IsModifiedSpecies)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertFiSpeciesAsync(viewModelSpeciesSelected);
                    IsModifiedSpecies = false;
                });
            }
            else
            {
                IsModifiedSpecies = true;
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
            IsInEditSpecies = false;
            IsModifiedSpecies = false;
            IsExistingSpecies = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Species]


    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------  
    public bool IsModifiedSpeciesgroup { get; set; }
    public bool IsModifiedSpecies { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => SpeciesgroupItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingSpeciesgroup;
    public bool IsDeletingSpeciesgroup
    {
        get => _isDeletingSpeciesgroup;
        set => SetProperty(ref _isDeletingSpeciesgroup, value);
    }
    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewSpeciesgroup;
    public bool IsNewSpeciesgroup
    {
        get => _isNewSpeciesgroup;
        set => SetProperty(ref _isNewSpeciesgroup, value);
    }

    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingSpeciesgroup;
    public bool IsExistingSpeciesgroup
    {
        get => _isExistingSpeciesgroup;
        set => SetProperty(ref _isExistingSpeciesgroup, value);
    }
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }
    //-----------IsInEdit----------------------------------------
    private bool _isInEditSpeciesgroup = false;
    public bool IsInEditSpeciesgroup
    {
        get => _isInEditSpeciesgroup;
        set => SetProperty(ref _isInEditSpeciesgroup, value);
    }
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditSpeciesgroup()
    {
        IsInEditSpeciesgroup = true;
        IsModifiedSpeciesgroup = false;
    }
    public void StartEditSpecies()
    {
        IsInEditSpecies = true;
        IsModifiedSpecies = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditSpeciesgroup() => await SaveSpeciesgroupAsync(SpeciesgroupSelected);
    public async void EndEditSpecies() => await SaveSpeciesAsync(SpeciesSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshSpeciesgroup => !IsModifiedSpeciesgroup && IsExistingSpeciesgroup;
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    //--------------CanRevert-------------------------------------
    public bool CanRevertSpeciesgroup => IsModifiedSpeciesgroup && IsExistingSpeciesgroup;
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl68Speciesgroup _speciesgroupSelected;
    public Tbl68Speciesgroup SpeciesgroupSelected
    {
        get => _speciesgroupSelected;
        set => SetProperty(ref _speciesgroupSelected, value);
    }

    private Tbl69FiSpecies _speciesSelected;
    public Tbl69FiSpecies SpeciesSelected
    {
        get => _speciesSelected;
        set => SetProperty(ref _speciesSelected, value);
    }

    #endregion Selected Properties    

    #region Public Properties

    private int _speciesgroupDataSetCount;

    public int SpeciesgroupDataSetCount
    {
        get => _speciesgroupDataSetCount;
        set
        {
            _speciesgroupDataSetCount = value; OnPropertyChanged();
        }
    }

    //---------------------------------------------------
    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsList;
    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsList
    {
        get => _tbl68SpeciesgroupsList;
        set
        {
            _tbl68SpeciesgroupsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsAllList;
    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsAllList
    {
        get => _tbl68SpeciesgroupsAllList;
        set
        {
            _tbl68SpeciesgroupsAllList = value; OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl66Genus> _tbl66GenussesAllList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesAllList
    {
        get => _tbl66GenussesAllList;
        set
        {
            _tbl66GenussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl69FiSpecies> _tbl75SpeciesList;

    public ObservableCollection<Tbl69FiSpecies> Tbl75SpeciessesList
    {
        get => _tbl75SpeciesList;
        set
        {
            _tbl75SpeciesList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshSpeciesgroupItems()
    {
        SpeciesgroupItems.Clear();
        if (Tbl68SpeciesgroupsList != null)
        {
            foreach (var item in Tbl68SpeciesgroupsList)
            {
                SpeciesgroupItems.Add(item);
            }

            if (Tbl68SpeciesgroupsList.Count == 0)
            {
                return;
            }

            if (SpeciesgroupSelected == null && Tbl68SpeciesgroupsList.Count != 0)
            {
                SpeciesgroupSelected = SpeciesgroupItems.FirstOrDefault()!;
            }
        }
    }


    public void RefreshSpeciesItems()
    {
        SpeciesItems.Clear();
        if (Tbl75SpeciessesList != null)
        {
            foreach (var item in Tbl75SpeciessesList)
            {
                SpeciesItems.Add(item);
            }

            if (Tbl75SpeciessesList.Count == 0)
            {
                return;
            }

            if (SpeciesSelected == null && Tbl75SpeciessesList.Count != 0)
            {
                SpeciesSelected = SpeciesItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties 


}
