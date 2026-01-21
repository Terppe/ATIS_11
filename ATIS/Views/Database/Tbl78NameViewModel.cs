using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using ATIS.Infrastructure;

//    Tbl78NameViewModel Skriptdatum:  02.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl78NameViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly IDataService _data;
    public ObservableCollection<Tbl78Name> NameItems { get; } = [];
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = [];

    #endregion [Private Data Members]           

    #region [Constructor]
    public Tbl78NameViewModel(IDataService dataService)
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
        Tbl75SpeciessesAllList ??= [];
        Tbl75SpeciessesAllList = [.. _dataService.GetSpeciesAsync()];
    }

    public Tbl78NameViewModel()
    {
    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods Name]

    public async Task AddNameAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditName = true;
            IsNewName = true;

            var speciesId = Tbl75SpeciessesAllList.First().SpeciesId;

            Tbl78NamesList ??= [];
            Tbl78NamesList.Insert(index: 0, item: new Tbl78Name { SpeciesId = speciesId });
        });
    }
    public async Task SaveNameAsync(Tbl78Name viewModelNameSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewName)
            {
                IsNewName = false;
            }
        });

        Tbl78Name result = null;
        try
        {
            if (IsModifiedName)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertNameAsync(viewModelNameSelected);
                    IsModifiedName = false;
                });
            }
            else
            {
                IsModifiedName = true;
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
            IsInEditName = false;
            IsModifiedName = false;
            IsExistingName = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteNameAsync(Tbl78Name viewModelNameSelected)
    {
        Tbl78Name result = null;
        try
        {
            var deleteNameId = viewModelNameSelected.NameId;
            await DeleteName(deleteNameId);

            result = viewModelNameSelected;
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
            IsInEditName = false;
            IsModifiedName = false;
            IsExistingName = true;
            IsDeletingName = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteName(int nameid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var name = await AppData.Data.Tbl78Names.FirstOrDefaultAsync(b => b.NameId == nameid);
            if (name != null)
            {
                //AppData.Data.Tbl78Names.Remove(name);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Name]

    #region [Methods Species]


    public async Task SaveSpeciesAsync(Tbl75Species viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSpecies)
            {
                IsNewSpecies = false;
            }
        });

        Tbl75Species result = null;
        try
        {
            if (IsModifiedSpecies)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSpeciesAsync(viewModelSpeciesSelected);
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
    public bool IsModifiedName { get; set; }
    public bool IsModifiedSpecies { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => NameItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingName;
    public bool IsDeletingName
    {
        get => _isDeletingName;
        set => SetProperty(ref _isDeletingName, value);
    }

    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewName;
    public bool IsNewName
    {
        get => _isNewName;
        set => SetProperty(ref _isNewName, value);
    }

    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingName;
    public bool IsExistingName
    {
        get => _isExistingName;
        set => SetProperty(ref _isExistingName, value);
    }
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }
    //-----------IsInEdit----------------------------------------
    private bool _isInEditName = false;
    public bool IsInEditName
    {
        get => _isInEditName;
        set => SetProperty(ref _isInEditName, value);
    }
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditName()
    {
        IsInEditName = true;
        IsModifiedName = false;
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
    public async void EndEditName() => await SaveNameAsync(NameSelected);
    public async void EndEditSpecies() => await SaveSpeciesAsync(SpeciesSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshName => !IsModifiedName && IsExistingName;
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    //--------------CanRevert-------------------------------------
    public bool CanRevertName => IsModifiedName && IsExistingName;
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl78Name _nameSelected;
    public Tbl78Name NameSelected
    {
        get => _nameSelected;
        set => SetProperty(ref _nameSelected, value);
    }
    private Tbl66Genus _genusSelected;
    public Tbl66Genus GenusSelected
    {
        get => _genusSelected;
        set => SetProperty(ref _genusSelected, value);
    }

    private Tbl75Species _speciesSelected;
    public Tbl75Species SpeciesSelected
    {
        get => _speciesSelected;
        set => SetProperty(ref _speciesSelected, value);
    }

    #endregion Selected Properties    

    #region Public Properties

    private int _nameDataSetCount;
    public int NameDataSetCount
    {
        get => _nameDataSetCount;
        set
        {
            _nameDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _speciesDataSetCount;
    public int SpeciesDataSetCount
    {
        get => _speciesDataSetCount;
        set
        {
            _speciesDataSetCount = value; OnPropertyChanged();
        }
    }
    //---------------------------------------------------------------
    private ObservableCollection<Tbl78Name> _tbl78NamesAllList;
    public ObservableCollection<Tbl78Name> Tbl78NamesAllList
    {
        get => _tbl78NamesAllList;
        set
        {
            _tbl78NamesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciessesAllList;
    public ObservableCollection<Tbl75Species> Tbl75SpeciessesAllList
    {
        get => _tbl75SpeciessesAllList;
        set
        {
            _tbl75SpeciessesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl78Name> _tbl78NamesList;
    public ObservableCollection<Tbl78Name> Tbl78NamesList
    {
        get => _tbl78NamesList;
        set
        {
            _tbl78NamesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get => _tbl66GenussesList;
        set
        {
            _tbl66GenussesList = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsAllList;
    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsAllList
    {
        get => _tbl68SpeciesgroupsAllList;
        set
        {
            _tbl68SpeciesgroupsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl63Infratribus> _tbl63InfratribussesAllList;
    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesAllList
    {
        get => _tbl63InfratribussesAllList;
        set
        {
            _tbl63InfratribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciesList;

    public ObservableCollection<Tbl75Species> Tbl75SpeciessesList
    {
        get => _tbl75SpeciesList;
        set
        {
            _tbl75SpeciesList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshNameItems()
    {
        NameItems.Clear();
        if (Tbl78NamesList != null)
        {
            foreach (var item in Tbl78NamesList)
            {
                NameItems.Add(item);
            }

            if (Tbl78NamesList.Count == 0)
            {
                return;
            }

            if (NameSelected == null && Tbl78NamesList.Count != 0)
            {
                NameSelected = NameItems.FirstOrDefault()!;
            }
        }
    }

    public void RefreshGenusItems()
    {
        GenusItems.Clear();
        if (Tbl66GenussesList != null)
        {
            foreach (var item in Tbl66GenussesList)
            {
                GenusItems.Add(item);
            }

            if (Tbl66GenussesList.Count == 0)
            {
                return;
            }

            if (GenusSelected == null && Tbl66GenussesList.Count != 0)
            {
                GenusSelected = GenusItems.FirstOrDefault()!;
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
