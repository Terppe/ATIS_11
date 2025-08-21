using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    TblCountryViewModel Skriptdatum:   06.01.2025 12:32      

namespace ATIS.Views.Database;

public partial class TblCountryViewModel(IDataService _dataService) : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    public ObservableCollection<TblCountry> CountryItems { get; } = [];

    #endregion [Private Data Members]           


    //    Part 1    

    #region [Methods Country]
    public async Task AddCountryAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditCountry = true;
            IsNewCountry = true;

            TblCountriesList ??= [];
            TblCountriesList.Insert(index: 0, item: new TblCountry {});
        });
    }

    public async Task SaveCountryAsync(TblCountry viewModelCountrySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewCountry)
            {
                IsNewCountry = false;
            }
        });

        TblCountry result = null;
        try
        {
            if (IsModifiedCountry)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertCountryAsync(viewModelCountrySelected);
                    IsModifiedCountry = false;
                });
            }
            else
            {
                IsModifiedCountry = true;
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
            IsInEditCountry = false;
            IsModifiedCountry = false;
            IsExistingCountry = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteCountryAsync(TblCountry viewModelCountrySelected)
    {
        TblCountry result = null;
        try
        {
            var deleteCountryId = viewModelCountrySelected.CountryId;
            await DeleteCountry(deleteCountryId);

            result = viewModelCountrySelected;
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
            IsInEditCountry = false;
            IsModifiedCountry = false;
            IsExistingCountry = true;
            IsDeletingCountry = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteCountry(int countryid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var country = await Context.TblCountries.FirstOrDefaultAsync(b => b.CountryId == countryid);
            if (country != null)
            {
                Context.TblCountries.Remove(country);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Country ]  

    //    Part 2    

    #region All Properties

    #region [Propertys]
    //------------IsModified--------------------------------------
    public bool IsModifiedCountry { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => CountryItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingCountry;
    public bool IsDeletingCountry
    {
        get => _isDeletingCountry;
        set => SetProperty(ref _isDeletingCountry, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewCountry;
    public bool IsNewCountry
    {
        get => _isNewCountry;
        set => SetProperty(ref _isNewCountry, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingCountry;
    public bool IsExistingCountry
    {
        get => _isExistingCountry;
        set => SetProperty(ref _isExistingCountry, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditCountry = false;
    public bool IsInEditCountry
    {
        get => _isInEditCountry;
        set => SetProperty(ref _isInEditCountry, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditCountry()
    {
        IsInEditCountry = true;
        IsModifiedCountry = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditCountry() => await SaveCountryAsync(CountrySelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshCountry => !IsModifiedCountry && IsExistingCountry;
    //--------------CanRevert-------------------------------------
    public bool CanRevertCountry => IsModifiedCountry && IsExistingCountry;

    #endregion [Propertys]

    #region Selected Properties

    private TblCountry _countrySelected;

    public TblCountry CountrySelected
    {
        get => _countrySelected;
        set => SetProperty(ref _countrySelected, value);
    }


    #endregion Selected Properties     

    #region Public Properties  

    private int _countryDataSetCount;
    public int CountryDataSetCount
    {
        get => _countryDataSetCount;
        set
        {
            _countryDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchCountryName = "";

    public string SearchCountryName
    {
        get => _searchCountryName;
        set
        {
            _searchCountryName = value; OnPropertyChanged();
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

    public void RefreshCountryItems()
    {
        CountryItems.Clear();
        if (TblCountriesList != null)
        {
            foreach (var item in TblCountriesList)
            {
                CountryItems.Add(item);
            }

            if (TblCountriesList.Count == 0)
            {
                return;
            }

            if (CountrySelected == null && TblCountriesList.Count != 0)
            {
                CountrySelected = CountryItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties

}
