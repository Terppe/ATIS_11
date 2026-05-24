using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using Windows.Storage.Pickers;
using ATIS.Infrastructure;

//    Tbl87GeographicViewModel Skriptdatum:  08.07.2025  10:32    

namespace ATIS.Views.Database;
public partial class Tbl87GeographicAltViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly IDataService _data;
    public ObservableCollection<Tbl87Geographic> GeographicItems { get; } = [];
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = [];

    private readonly FileOpenPicker _filePicker = new();

    private readonly List<Data> _fileStreamList = [];
    public struct Data
    {
        public int ImageCountId;
        public byte[] ImageFileStream;
    };

    #endregion [Private Data Members]           

    #region [Constructor]
    public Tbl87GeographicAltViewModel(IDataService dataService)
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
        TblCountriesAllList ??= [];
        // Fix: Await the Task and use the result for the collection
        TblCountriesAllList = new ObservableCollection<TblCountry>(_dataService.GetCountryAsync().GetAwaiter().GetResult());
    }
    public Tbl87GeographicAltViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1

    #region [Methods Geographic]
    public async Task AddGeographicAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditGeographic = true;
            IsNewGeographic = true;

            var speciesId = Tbl75SpeciessesAllList.First().SpeciesId;

            Tbl87GeographicsList ??= [];
            Tbl87GeographicsList.Insert(index: 0, item: new Tbl87Geographic { SpeciesId = speciesId });
        });
    }

    public async Task SaveGeographicAsync(Tbl87Geographic viewModelGeographicSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewGeographic)
            {
                IsNewGeographic = false;
            }
        });

        Tbl87Geographic result = null;
        try
        {
            if (IsModifiedGeographic)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertGeographicAsync(viewModelGeographicSelected);
                    IsModifiedGeographic = false;
                });
            }
            else
            {
                IsModifiedGeographic = true;
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
            IsInEditGeographic = false;
            IsModifiedGeographic = false;
            IsExistingGeographic = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteGeographicAsync(Tbl87Geographic viewModelGeographicSelected)
    {
        Tbl87Geographic result = null;
        try
        {
            var deleteGeographicId = viewModelGeographicSelected.GeographicId;
            await DeleteGeographic(deleteGeographicId);

            result = viewModelGeographicSelected;
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
            IsInEditGeographic = false;
            IsModifiedGeographic = false;
            IsExistingGeographic = true;
            IsDeletingGeographic = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteGeographic(int geographicid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var geographic = await AppData.Data.Tbl87Geographics.FirstOrDefaultAsync(b => b.GeographicId == geographicid);
            if (geographic != null)
            {
                //AppData.Data.Tbl87Geographics.Remove(geographic);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Geographic]  

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
    public bool IsModifiedGeographic { get; set; }
    public bool IsModifiedSpecies { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => GeographicItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingGeographic;
    public bool IsDeletingGeographic
    {
        get => _isDeletingGeographic;
        set => SetProperty(ref _isDeletingGeographic, value);
    }

    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewGeographic;
    public bool IsNewGeographic
    {
        get => _isNewGeographic;
        set => SetProperty(ref _isNewGeographic, value);
    }

    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingGeographic;
    public bool IsExistingGeographic
    {
        get => _isExistingGeographic;
        set => SetProperty(ref _isExistingGeographic, value);
    }
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditGeographic = false;
    public bool IsInEditGeographic
    {
        get => _isInEditGeographic;
        set => SetProperty(ref _isInEditGeographic, value);
    }
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditGeographic()
    {
        IsInEditGeographic = true;
        IsModifiedGeographic = false;
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
    public async void EndEditGeographic() => await SaveGeographicAsync(GeographicSelected);
    public async void EndEditSpecies() => await SaveSpeciesAsync(SpeciesSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshGeographic => !IsModifiedGeographic && IsExistingGeographic;
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    //--------------CanRevert-------------------------------------
    public bool CanRevertGeographic => IsModifiedGeographic && IsExistingGeographic;
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;

    #endregion [Propertys]     


    #region Selected Properties

    private Tbl87Geographic _geographicSelected;

    public Tbl87Geographic GeographicSelected
    {
        get => _geographicSelected;
        set => SetProperty(ref _geographicSelected, value);
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

    private int _geographicDataSetCount;

    public int GeographicDataSetCount
    {
        get => _geographicDataSetCount;
        set
        {
            _geographicDataSetCount = value; OnPropertyChanged();
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
    private ObservableCollection<Tbl87Geographic> _tbl87GeographicsList;
    public ObservableCollection<Tbl87Geographic> Tbl87GeographicsList
    {
        get => _tbl87GeographicsList;
        set
        {
            _tbl87GeographicsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl87Geographic> _tbl87GeographicsAllList;
    public ObservableCollection<Tbl87Geographic> Tbl87GeographicsAllList
    {
        get => _tbl87GeographicsAllList;
        set
        {
            _tbl87GeographicsAllList = value; OnPropertyChanged();
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
    private ObservableCollection<TblCountry> _tblCountriesAllList;
    public ObservableCollection<TblCountry> TblCountriesAllList
    {
        get => _tblCountriesAllList;
        set
        {
            _tblCountriesAllList = value; OnPropertyChanged();
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

    private ObservableCollection<TblCountry> _tblCountriesList;

    public ObservableCollection<TblCountry> TblCountriesList
    {
        get => _tblCountriesList;
        set
        {
            _tblCountriesList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    


    #region Refresh Methods

    public void RefreshGeographicItems()
    {
        GeographicItems.Clear();
        if (Tbl87GeographicsList != null)
        {
            foreach (var item in Tbl87GeographicsList)
            {
                GeographicItems.Add(item);
            }

            if (Tbl87GeographicsList.Count == 0)
            {
                return;
            }

            if (GeographicSelected == null && Tbl87GeographicsList.Count != 0)
            {
                GeographicSelected = GeographicItems.FirstOrDefault()!;
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
