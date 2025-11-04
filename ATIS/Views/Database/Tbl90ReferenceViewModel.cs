using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl90ReferenceViewModel Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl90ReferenceViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public int ZRef;
    public int ZCom;
    #endregion [Private Data Members]

    #region [Constructor]
    public Tbl90ReferenceViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }
    private void GetAllCollections()
    {
        Tbl03RegnumsAllList ??= [];
        Tbl03RegnumsAllList = _dataService.GetRegnumAsync();

        Tbl06PhylumsAllList ??= [];
        Tbl06PhylumsAllList = _dataService.GetPhylumAsync();

        Tbl09DivisionsAllList ??= [];
        Tbl09DivisionsAllList = _dataService.GetDivisionAsync();

        Tbl12SubphylumsAllList ??= [];
        Tbl12SubphylumsAllList = _dataService.GetSubphylumAsync();

        Tbl15SubdivisionsAllList ??= [];
        Tbl15SubdivisionsAllList = _dataService.GetSubdivisionAsync();

        Tbl18SuperclassesAllList ??= [];
        Tbl18SuperclassesAllList = _dataService.GetSuperclassAsync();

        Tbl21ClassesAllList ??= [];
        Tbl21ClassesAllList = _dataService.GetClassAsync();

        Tbl24SubclassesAllList ??= [];
        Tbl24SubclassesAllList = _dataService.GetSubclassAsync();

        Tbl27InfraclassesAllList ??= [];
        Tbl27InfraclassesAllList = _dataService.GetInfraclassAsync();

        Tbl30LegiosAllList ??= [];
        Tbl30LegiosAllList = _dataService.GetLegioAsync();

        Tbl33OrdosAllList ??= [];
        Tbl33OrdosAllList = _dataService.GetOrdoAsync();

        Tbl36SubordosAllList ??= [];
        Tbl36SubordosAllList = _dataService.GetSubordoAsync();

        Tbl39InfraordosAllList ??= [];
        Tbl39InfraordosAllList = _dataService.GetInfraordoAsync();

        Tbl42SuperfamiliesAllList ??= [];
        Tbl42SuperfamiliesAllList = _dataService.GetSuperfamilyAsync();

        Tbl45FamiliesAllList ??= [];
        Tbl45FamiliesAllList = _dataService.GetFamilyAsync();

        Tbl48SubfamiliesAllList ??= [];
        Tbl48SubfamiliesAllList = _dataService.GetSubfamilyAsync();

        Tbl51InfrafamiliesAllList ??= [];
        Tbl51InfrafamiliesAllList = _dataService.GetInfrafamilyAsync();

        Tbl54SupertribussesAllList ??= [];
        Tbl54SupertribussesAllList = _dataService.GetSupertribusAsync();

        Tbl57TribussesAllList ??= [];
        Tbl57TribussesAllList = _dataService.GetTribusAsync();

        Tbl60SubtribussesAllList ??= [];
        Tbl60SubtribussesAllList = _dataService.GetSubtribusAsync();

        Tbl63InfratribussesAllList ??= [];
        Tbl63InfratribussesAllList = _dataService.GetInfratribusAsync();

        Tbl66GenussesAllList ??= [];
        Tbl66GenussesAllList = _dataService.GetGenusAsync();

        //Tbl69FiSpeciessesAllList ??= [];
        //Tbl69FiSpeciessesAllList = _dataService.GetSpeciesAsync();

        Tbl75SpeciessesAllList ??= [];
        Tbl75SpeciessesAllList = _dataService.GetSpeciesAsync();

        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = _dataService.GetRefExpertAsync();
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = _dataService.GetRefSourceAsync();
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = _dataService.GetRefAuthorAsync();
    }

    #endregion [Constructor]

    //    Part 1    

    #region [Methods Reference]
    public async Task AddReferenceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditReference = true;
            IsNewReference = true;

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference {});
        });
    }

    public async Task SaveReferenceAsync(Tbl90Reference viewModelReferenceSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewReference)
            {
                IsNewReference = false;
            }
        });

        Tbl90Reference result = null;
        try
        {
            if (IsModifiedReference)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertReferenceAsync(viewModelReferenceSelected);
                    IsModifiedReference = false;
                });
            }
            else
            {
                IsModifiedReference = true;
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
            IsInEditReference = false;
            IsModifiedReference = false;
            IsExistingReference = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteReferenceAsync(Tbl90Reference viewModelReferenceSelected)
    {
        Tbl90Reference result = null;
        try
        {
            var deleteReferenceId = viewModelReferenceSelected.ReferenceId;
            await DeleteReference(deleteReferenceId);

            result = viewModelReferenceSelected;
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
            IsInEditReference = false;
            IsModifiedReference = false;
            IsExistingReference = true;
            IsDeletingReference = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteReference(int referenceid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var reference = await Context.Tbl90References.FirstOrDefaultAsync(b => b.ReferenceId == referenceid);
            if (reference != null)
            {
                Context.Tbl90References.Remove(reference);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Reference]

    //    Part 2    

    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedReference { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => ReferenceItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingReference;
    public bool IsDeletingReference
    {
        get => _isDeletingReference;
        set => SetProperty(ref _isDeletingReference, value);
    }

    //------------IsNew---------------------------------------
    private bool _isNewReference;
    public bool IsNewReference
    {
        get => _isNewReference;
        set => SetProperty(ref _isNewReference, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingReference;
    public bool IsExistingReference
    {
        get => _isExistingReference;
        set => SetProperty(ref _isExistingReference, value);
    }


    //-----------IsInEdit----------------------------------------
    private bool _isInEditReference = false;
    public bool IsInEditReference
    {
        get => _isInEditReference;
        set => SetProperty(ref _isInEditReference, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditReference()
    {
        IsInEditReference = true;
        IsModifiedReference = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditReference() => await SaveReferenceAsync(ReferenceSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshReference => !IsModifiedReference && IsExistingReference;
    //--------------CanRevert-------------------------------------
    public bool CanRevertReference => IsModifiedReference && IsExistingReference;

    #endregion [Propertys]           

    #region Selected Properties

    private Tbl90Reference _referenceSelected;
    public Tbl90Reference ReferenceSelected
    {
         get => _referenceSelected;
         set => SetProperty(ref _referenceSelected, value);
    }

    #endregion Selected Properties             

    #region Public Properties

    private int _referenceDataSetCount;
    public int ReferenceDataSetCount
    {
        get => _referenceDataSetCount;
        set
        {
            _referenceDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _referenceExpertDataSetCount;
    public int ReferenceExpertDataSetCount
    {
        get => _referenceExpertDataSetCount;
        set
        {
            _referenceExpertDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _referenceSourceDataSetCount;
    public int ReferenceSourceDataSetCount
    {
        get => _referenceSourceDataSetCount;
        set
        {
            _referenceSourceDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _referenceAuthorDataSetCount;
    public int ReferenceAuthorDataSetCount
    {
        get => _referenceAuthorDataSetCount;
        set
        {
            _referenceAuthorDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchReferenceInfoOrId = "";

    public string SearchReferenceInfoOrId
    {
        get => _searchReferenceInfoOrId;
        set
        {
            _searchReferenceInfoOrId = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferencesList;

    public ObservableCollection<Tbl90Reference> Tbl90ReferencesList
    {
        get => _tbl90ReferencesList;
        set
        {
            _tbl90ReferencesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsAllList = null!;
    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get => _tbl03RegnumsAllList;
        set
        {
            _tbl03RegnumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsAllList = null!;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsAllList
    {
        get => _tbl06PhylumsAllList;
        set
        {
            _tbl06PhylumsAllList = value; OnPropertyChanged(nameof(Tbl06PhylumsAllList));
        }
    }

    private ObservableCollection<Tbl09Division> _tbl09DivisionsAllList = null!;
    public ObservableCollection<Tbl09Division> Tbl09DivisionsAllList
    {
        get => _tbl09DivisionsAllList;
        set
        {
            _tbl09DivisionsAllList = value; OnPropertyChanged(nameof(Tbl09DivisionsAllList));
        }
    }

    private ObservableCollection<Tbl12Subphylum> _tbl12SubphylumsAllList = null!;
    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsAllList
    {
        get => _tbl12SubphylumsAllList;
        set
        {
            _tbl12SubphylumsAllList = value; OnPropertyChanged(nameof(Tbl12SubphylumsAllList));
        }
    }

    private ObservableCollection<Tbl15Subdivision> _tbl15SubdivisionsAllList = null!;
    public ObservableCollection<Tbl15Subdivision> Tbl15SubdivisionsAllList
    {
        get => _tbl15SubdivisionsAllList;
        set
        {
            _tbl15SubdivisionsAllList = value; OnPropertyChanged(nameof(Tbl15SubdivisionsAllList));
        }
    }

    private ObservableCollection<Tbl18Superclass> _tbl18SuperclassesAllList = null!;
    public ObservableCollection<Tbl18Superclass> Tbl18SuperclassesAllList
    {
        get => _tbl18SuperclassesAllList;
        set
        {
            _tbl18SuperclassesAllList = value; OnPropertyChanged(nameof(Tbl18SuperclassesAllList));
        }
    }

    private ObservableCollection<Tbl21Class> _tbl21ClassesAllList = null!;
    public ObservableCollection<Tbl21Class> Tbl21ClassesAllList
    {
        get => _tbl21ClassesAllList;
        set
        {
            _tbl21ClassesAllList = value; OnPropertyChanged(nameof(Tbl21ClassesAllList));
        }
    }

    private ObservableCollection<Tbl24Subclass> _tbl24SubclassesAllList = null!;
    public ObservableCollection<Tbl24Subclass> Tbl24SubclassesAllList
    {
        get => _tbl24SubclassesAllList;
        set
        {
            _tbl24SubclassesAllList = value; OnPropertyChanged(nameof(Tbl24SubclassesAllList));
        }
    }

    private ObservableCollection<Tbl27Infraclass> _tbl27InfraclassesAllList = null!;
    public ObservableCollection<Tbl27Infraclass> Tbl27InfraclassesAllList
    {
        get => _tbl27InfraclassesAllList;
        set
        {
            _tbl27InfraclassesAllList = value; OnPropertyChanged(nameof(Tbl27InfraclassesAllList));
        }
    }

    private ObservableCollection<Tbl30Legio> _tbl30LegiosAllList = null!;
    public ObservableCollection<Tbl30Legio> Tbl30LegiosAllList
    {
        get => _tbl30LegiosAllList;
        set
        {
            _tbl30LegiosAllList = value; OnPropertyChanged(nameof(Tbl30LegiosAllList));
        }
    }

    private ObservableCollection<Tbl33Ordo> _tbl33OrdosAllList = null!;
    public ObservableCollection<Tbl33Ordo> Tbl33OrdosAllList
    {
        get => _tbl33OrdosAllList;
        set
        {
            _tbl33OrdosAllList = value; OnPropertyChanged(nameof(Tbl33OrdosAllList));
        }
    }

    private ObservableCollection<Tbl36Subordo> _tbl36SubordosAllList = null!;
    public ObservableCollection<Tbl36Subordo> Tbl36SubordosAllList
    {
        get => _tbl36SubordosAllList;
        set
        {
            _tbl36SubordosAllList = value; OnPropertyChanged(nameof(Tbl36SubordosAllList));
        }
    }

    private ObservableCollection<Tbl39Infraordo> _tbl39InfraordosAllList = null!;
    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosAllList
    {
        get => _tbl39InfraordosAllList;
        set
        {
            _tbl39InfraordosAllList = value; OnPropertyChanged(nameof(Tbl39InfraordosAllList));
        }
    }

    private ObservableCollection<Tbl42Superfamily> _tbl42SuperfamiliesAllList = null!;
    public ObservableCollection<Tbl42Superfamily> Tbl42SuperfamiliesAllList
    {
        get => _tbl42SuperfamiliesAllList;
        set
        {
            _tbl42SuperfamiliesAllList = value; OnPropertyChanged(nameof(Tbl42SuperfamiliesAllList));
        }
    }

    private ObservableCollection<Tbl45Family> _tbl45FamiliesAllList = null!;
    public ObservableCollection<Tbl45Family> Tbl45FamiliesAllList
    {
        get => _tbl45FamiliesAllList;
        set
        {
            _tbl45FamiliesAllList = value; OnPropertyChanged(nameof(Tbl45FamiliesAllList));
        }
    }

    private ObservableCollection<Tbl48Subfamily> _tbl48SubfamiliesAllList = null!;
    public ObservableCollection<Tbl48Subfamily> Tbl48SubfamiliesAllList
    {
        get => _tbl48SubfamiliesAllList;
        set
        {
            _tbl48SubfamiliesAllList = value; OnPropertyChanged(nameof(Tbl48SubfamiliesAllList));
        }
    }

    private ObservableCollection<Tbl51Infrafamily> _tbl51InfrafamiliesAllList = null!;
    public ObservableCollection<Tbl51Infrafamily> Tbl51InfrafamiliesAllList
    {
        get => _tbl51InfrafamiliesAllList;
        set
        {
            _tbl51InfrafamiliesAllList = value; OnPropertyChanged(nameof(Tbl51InfrafamiliesAllList));
        }
    }

    private ObservableCollection<Tbl54Supertribus> _tbl54SupertribussesAllList = null!;
    public ObservableCollection<Tbl54Supertribus> Tbl54SupertribussesAllList
    {
        get => _tbl54SupertribussesAllList;
        set
        {
            _tbl54SupertribussesAllList = value; OnPropertyChanged(nameof(Tbl54SupertribussesAllList));
        }
    }

    private ObservableCollection<Tbl57Tribus> _tbl57TribussesAllList = null!;
    public ObservableCollection<Tbl57Tribus> Tbl57TribussesAllList
    {
        get => _tbl57TribussesAllList;
        set
        {
            _tbl57TribussesAllList = value; OnPropertyChanged(nameof(Tbl57TribussesAllList));
        }
    }

    private ObservableCollection<Tbl60Subtribus> _tbl60SubtribussesAllList = null!;
    public ObservableCollection<Tbl60Subtribus> Tbl60SubtribussesAllList
    {
        get => _tbl60SubtribussesAllList;
        set
        {
            _tbl60SubtribussesAllList = value; OnPropertyChanged(nameof(Tbl60SubtribussesAllList));
        }
    }

    private ObservableCollection<Tbl63Infratribus> _tbl63InfratribussesAllList = null!;
    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesAllList
    {
        get => _tbl63InfratribussesAllList;
        set
        {
            _tbl63InfratribussesAllList = value; OnPropertyChanged(nameof(Tbl63InfratribussesAllList));
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesAllList = null!;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesAllList
    {
        get => _tbl66GenussesAllList;
        set
        {
            _tbl66GenussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciessesAllList = null!;
    public ObservableCollection<Tbl75Species> Tbl75SpeciessesAllList
    {
        get => _tbl75SpeciessesAllList;
        set
        {
            _tbl75SpeciessesAllList = value; OnPropertyChanged();
        }
    }

    //private ObservableCollection<Tbl69FiSpecies> _tbl69FiSpeciessesAllList = null!;
    //public ObservableCollection<Tbl69FiSpecies> Tbl69FiSpeciessesAllList
    //{
    //    get => _tbl69FiSpeciessesAllList;
    //    set
    //    {
    //        _tbl69FiSpeciessesAllList = value; OnPropertyChanged(nameof(Tbl69FiSpeciessesAllList));
    //    }
    //}

    //private ObservableCollection<Tbl72PlSpecies> _tbl72PlSpeciessesAllList = null!;
    //public ObservableCollection<Tbl72PlSpecies> Tbl72PlSpeciessesAllList
    //{
    //    get => _tbl72PlSpeciessesAllList;
    //    set
    //    {
    //        _tbl72PlSpeciessesAllList = value; OnPropertyChanged(nameof(Tbl72PlSpeciessesAllList));
    //    }
    //}

    private ObservableCollection<Tbl90RefExpert> _tbl90ExpertsAllList = null!;
    public ObservableCollection<Tbl90RefExpert> Tbl90ExpertsAllList
    {
        get => _tbl90ExpertsAllList;
        set
        {
            _tbl90ExpertsAllList = value; OnPropertyChanged(nameof(Tbl90ExpertsAllList));
        }
    }

    private ObservableCollection<Tbl90RefSource> _tbl90SourcesAllList = null!;
    public ObservableCollection<Tbl90RefSource> Tbl90SourcesAllList
    {
        get => _tbl90SourcesAllList;
        set
        {
            _tbl90SourcesAllList = value; OnPropertyChanged(nameof(Tbl90SourcesAllList));
        }
    }
    private ObservableCollection<Tbl90RefAuthor> _tbl90AuthorsAllList = null!;
    public ObservableCollection<Tbl90RefAuthor> Tbl90AuthorsAllList
    {
        get => _tbl90AuthorsAllList;
        set
        {
            _tbl90AuthorsAllList = value; OnPropertyChanged(nameof(Tbl90AuthorsAllList));
        }
    }
    private ObservableCollection<Tbl90RefExpert> _tbl90RefExpertsList = null!;
    public ObservableCollection<Tbl90RefExpert> Tbl90RefExpertsList
    {
        get => _tbl90RefExpertsList;
        set
        {
            _tbl90RefExpertsList = value; OnPropertyChanged(nameof(Tbl90RefExpertsList));
        }
    }


    private ObservableCollection<Tbl90RefSource> _tbl90RefSourcesList = null!;
    public ObservableCollection<Tbl90RefSource> Tbl90RefSourcesList
    {
        get => _tbl90RefSourcesList;
        set
        {
            _tbl90RefSourcesList = value; OnPropertyChanged(nameof(Tbl90RefSourcesList));
        }
    }

    private ObservableCollection<Tbl90RefAuthor> _tbl90RefAuthorsList = null!;
    public ObservableCollection<Tbl90RefAuthor> Tbl90RefAuthorsList
    {
        get => _tbl90RefAuthorsList;
        set
        {
            _tbl90RefAuthorsList = value; OnPropertyChanged(nameof(Tbl90RefAuthorsList));
        }
    }

    #endregion Public Properties  

    #region Refresh Methods

    public void RefreshReferenceItems()
    {
        ReferenceItems.Clear();
        if (Tbl90ReferencesList != null)
        {
            foreach (var item in Tbl90ReferencesList)
            {
                ReferenceItems.Add(item);
            }

            if (Tbl90ReferencesList.Count == 0)
            {
                return;
            }

            if (ReferenceSelected == null && Tbl90ReferencesList.Count != 0)
            {
                ReferenceSelected = ReferenceItems.FirstOrDefault();
            }
        }
    }

    #endregion Refresh Methods


    #endregion All Properties

}
