using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using ATIS.Infrastructure;

//    Tbl84SynonymViewModel Skriptdatum:  25.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl84SynonymViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly IDataService _data;
    public ObservableCollection<Tbl84Synonym> SynonymItems { get; } = [];
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = [];

    #endregion [Private Data Members]           

    #region [Constructor]
    public Tbl84SynonymViewModel(IDataService dataService)
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
    public Tbl84SynonymViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1    

    #region [Methods Synonym]

    public async Task AddSynonymAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditSynonym = true;
            IsNewSynonym = true;

            var speciesId = Tbl75SpeciessesAllList.First().SpeciesId;

            Tbl84SynonymsList ??= [];
            Tbl84SynonymsList.Insert(index: 0, item: new Tbl84Synonym { SpeciesId = speciesId});
        });
    }

    public async Task SaveSynonymAsync(Tbl84Synonym viewModelSynonymSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSynonym)
            {
                IsNewSynonym = false;
            }
        });

        Tbl84Synonym result = null;
        try
        {
            if (IsModifiedSynonym)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSynonymAsync(viewModelSynonymSelected);
                    IsModifiedSynonym = false;
                });
            }
            else
            {
                IsModifiedSynonym = true;
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
            IsInEditSynonym = false;
            IsModifiedSynonym = false;
            IsExistingSynonym = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteSynonymAsync(Tbl84Synonym viewModelSynonymSelected)
    {
        Tbl84Synonym result = null;
        try
        {
            var deleteSynonymId = viewModelSynonymSelected.SynonymId;
            await DeleteSynonym(deleteSynonymId);

            result = viewModelSynonymSelected;
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
            IsInEditSynonym = false;
            IsModifiedSynonym = false;
            IsExistingSynonym = true;
            IsDeletingSynonym = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteSynonym(int synonymid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var synonym = await AppData.Data.Tbl84Synonyms.FirstOrDefaultAsync(b => b.SynonymId == synonymid);
            if (synonym != null)
            {
                //AppData.Data.Tbl84Synonyms.Remove(synonym);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }


    #endregion [Methods Synonym]  

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
    public bool IsModifiedSynonym { get; set; }
    public bool IsModifiedSpecies { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => SynonymItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingSynonym;
    public bool IsDeletingSynonym
    {
        get => _isDeletingSynonym;
        set => SetProperty(ref _isDeletingSynonym, value);
    }

    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewSynonym;
    public bool IsNewSynonym
    {
        get => _isNewSynonym;
        set => SetProperty(ref _isNewSynonym, value);
    }

    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingSynonym;
    public bool IsExistingSynonym
    {
        get => _isExistingSynonym;
        set => SetProperty(ref _isExistingSynonym, value);
    }
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditSynonym = false;
    public bool IsInEditSynonym
    {
        get => _isInEditSynonym;
        set => SetProperty(ref _isInEditSynonym, value);
    }
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditSynonym()
    {
        IsInEditSynonym = true;
        IsModifiedSynonym = false;
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
    public async void EndEditSynonym() => await SaveSynonymAsync(SynonymSelected);
    public async void EndEditSpecies() => await SaveSpeciesAsync(SpeciesSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshSynonym => !IsModifiedSynonym && IsExistingSynonym;
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    //--------------CanRevert-------------------------------------
    public bool CanRevertSynonym => IsModifiedSynonym && IsExistingSynonym;
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;

    #endregion [Propertys]     


    #region Selected Properties

    private Tbl84Synonym _synonymSelected;

    public Tbl84Synonym SynonymSelected
    {
        get => _synonymSelected;
        set => SetProperty(ref _synonymSelected, value);
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

    private int _synonymDataSetCount;

    public int SynonymDataSetCount
    {
        get => _synonymDataSetCount;
        set
        {
            _synonymDataSetCount = value; OnPropertyChanged();
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
    private ObservableCollection<Tbl84Synonym> _tbl84SynonymsList;
    public ObservableCollection<Tbl84Synonym> Tbl84SynonymsList
    {
        get => _tbl84SynonymsList;
        set
        {
            _tbl84SynonymsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl84Synonym> _tbl84SynonymsAllList;
    public ObservableCollection<Tbl84Synonym> Tbl84SynonymsAllList
    {
        get => _tbl84SynonymsAllList;
        set
        {
            _tbl84SynonymsAllList = value; OnPropertyChanged();
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

    public void RefreshSynonymItems()
    {
        SynonymItems.Clear();
        if (Tbl84SynonymsList != null)
        {
            foreach (var item in Tbl84SynonymsList)
            {
                SynonymItems.Add(item);
            }

            if (Tbl84SynonymsList.Count == 0)
            {
                return;
            }

            if (SynonymSelected == null && Tbl84SynonymsList.Count != 0)
            {
                SynonymSelected = SynonymItems.FirstOrDefault()!;
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
