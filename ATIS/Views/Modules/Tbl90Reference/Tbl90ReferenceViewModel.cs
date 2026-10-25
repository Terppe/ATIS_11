using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl90Reference;

public class Tbl90ReferenceViewModel : BaseBossListViewModel<Models.Tbl90Reference>
{
    #region [ Fields / state ]
    // ---------------------------------------------------------

    private readonly LookupService _lookupService;
    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.Tbl90Reference>>>? _activeLoader;
    private Action<Models.Tbl90Reference>? _assignParentFk;

    #endregion

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl90ReferenceViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;
    }

    #region [ LOOKUPS ]
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; } = new();
    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; } = new();
    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; } = new();
    public Dictionary<int, string?> RefAuthorLookup { get; private set; } = new();
    public Dictionary<int, string?> RefExpertLookup { get; private set; } = new();
    public Dictionary<int, string?> RefSourceLookup { get; private set; } = new();

    public async Task LoadReferenceLookupsAsync()
    {
        var authorListTask = _lookupService.GetRefAuthorListAsync();
        var expertListTask = _lookupService.GetRefExpertListAsync();
        var sourceListTask = _lookupService.GetRefSourceListAsync();

        var authorLookupTask = _lookupService.GetRefAuthorLookupAsync();
        var expertLookupTask = _lookupService.GetRefExpertLookupAsync();
        var sourceLookupTask = _lookupService.GetRefSourceLookupAsync();

        await Task.WhenAll(authorListTask, expertListTask, sourceListTask,
            authorLookupTask, expertLookupTask, sourceLookupTask);

        RefAuthorsList.Clear();
        foreach (var item in (await authorListTask) ?? new()) RefAuthorsList.Add(item);

        RefExpertsList.Clear();
        foreach (var item in (await expertListTask) ?? new()) RefExpertsList.Add(item);

        RefSourcesList.Clear();
        foreach (var item in (await sourceListTask) ?? new()) RefSourcesList.Add(item);

        RefAuthorLookup = (await authorLookupTask) ?? new();
        RefExpertLookup = (await expertLookupTask) ?? new();
        RefSourceLookup = (await sourceLookupTask) ?? new();

        OnPropertyChanged(nameof(RefAuthorLookup));
        OnPropertyChanged(nameof(RefExpertLookup));
        OnPropertyChanged(nameof(RefSourceLookup));
    }
    //---------------------------------------------------------

    private bool _speciesLoaded;
    private bool _genusLoaded;
    //---------------------------------------------------------
    private bool _infratribusLoaded;
    private bool _subtribusLoaded;
    private bool _tribusLoaded;
    private bool _supertribusLoaded;
    //---------------------------------------------------------
    private bool _infrafamilyLoaded;
    private bool _subfamilyLoaded;
    private bool _familyLoaded;
    private bool _superfamilyLoaded;
    //---------------------------------------------------------
    private bool _infraordoLoaded;
    private bool _subordoLoaded;
    private bool _ordoLoaded;
    private bool _legioLoaded;
    //---------------------------------------------------------
    private bool _infraclassLoaded;
    private bool _subclassLoaded;
    private bool _classLoaded;
    private bool _superclassLoaded;
    //---------------------------------------------------------
    private bool _subphylumLoaded;
    private bool _phylumLoaded;
    private bool _regnumLoaded;

    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl75Species> SpeciesList { get; } = new();
    public ObservableCollection<Models.Tbl66Genus> GenusList { get; } = new();
    public ObservableCollection<Models.Tbl63Infratribus> InfratribusList { get; } = new();
    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; } = new();
    public ObservableCollection<Models.Tbl57Tribus> TribusList { get; } = new();
    public ObservableCollection<Models.Tbl54Supertribus> SupertribusList { get; } = new();
    public ObservableCollection<Models.Tbl51Infrafamily> InfrafamilyList { get; } = new();
    public ObservableCollection<Models.Tbl48Subfamily> SubfamilyList { get; } = new();
    public ObservableCollection<Models.Tbl45Family> FamilyList { get; } = new();
    public ObservableCollection<Models.Tbl42Superfamily> SuperfamilyList { get; } = new();
    public ObservableCollection<Models.Tbl39Infraordo> InfraordoList { get; } = new();
    public ObservableCollection<Models.Tbl36Subordo> SubordoList { get; } = new();
    public ObservableCollection<Models.Tbl33Ordo> OrdoList { get; } = new();
    public ObservableCollection<Models.Tbl30Legio> LegioList { get; } = new();
    public ObservableCollection<Models.Tbl27Infraclass> InfraclassList { get; } = new();
    public ObservableCollection<Models.Tbl24Subclass> SubclassList { get; } = new();
    public ObservableCollection<Models.Tbl21Class> ClassList { get; } = new();
    public ObservableCollection<Models.Tbl18Superclass> SuperclassList { get; } = new();
    public ObservableCollection<Models.Tbl12Subphylum> SubphylumList { get; } = new();
    public ObservableCollection<Models.Tbl06Phylum> PhylumList { get; } = new();
    public ObservableCollection<Models.Tbl03Regnum> RegnumList { get; } = new();

    public async Task EnsureLookupForTabAsync(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0: // Species/Genus
                await EnsureTbl75SpeciesLookupAsync();
                await EnsureTbl66GenusLookupAsync();
                break;

            case 1: // Tribus-Tab
                await EnsureTbl63InfratribusLookupAsync();
                await EnsureTbl60SubtribusLookupAsync();
                await EnsureTbl57TribusLookupAsync();
                await EnsureTbl54SupertribusLookupAsync();
                break;

            case 2: // Family-Tab
                await EnsureTbl51InfrafamilyLookupAsync();
                await EnsureTbl48SubfamilyLookupAsync();
                await EnsureTbl45FamilyLookupAsync();
                await EnsureTbl42SuperfamilyLookupAsync();
                break;

            case 3: // Ordo-Tab
                await EnsureTbl39InfraordoLookupAsync();
                await EnsureTbl36SubordoLookupAsync();
                await EnsureTbl33OrdoLookupAsync();
                await EnsureTbl30LegioLookupAsync();
                break;

            case 4: // Class-Tab
                await EnsureTbl27InfraclassLookupAsync();
                await EnsureTbl24SubclassLookupAsync();
                await EnsureTbl21ClassLookupAsync();
                await EnsureTbl18SuperclassLookupAsync();
                break;

            case 5: // Phylum/Regnum-Tab
                await EnsureTbl12SubphylumLookupAsync();
                await EnsureTbl06PhylumLookupAsync();
                await EnsureTbl03RegnumLookupAsync();
                break;
        }
    }
    // ---------------------------------------------------------
    public async Task EnsureTbl75SpeciesLookupAsync()
    {
        if (_speciesLoaded) return;

        var list = await _lookupService.GetSpeciesListAsync();
        SpeciesList.Clear();
        foreach (var item in list)
            SpeciesList.Add(item);

        _speciesLoaded = true;
        OnPropertyChanged(nameof(SpeciesList));
    }

    public async Task EnsureTbl66GenusLookupAsync()
    {
        if (_genusLoaded) return;

        var list = await _lookupService.GetGenusListAsync();
        GenusList.Clear();
        foreach (var item in list)
            GenusList.Add(item);

        _genusLoaded = true;
        OnPropertyChanged(nameof(GenusList));
    }
    //---------------------------------------------------------
    public async Task EnsureTbl63InfratribusLookupAsync()
    {
        if (_infratribusLoaded) return;

        var list = await _lookupService.GetInfratribusListAsync();
        InfratribusList.Clear();
        foreach (var item in list)
            InfratribusList.Add(item);

        _infratribusLoaded = true;
        OnPropertyChanged(nameof(InfratribusList));
    }

    public async Task EnsureTbl60SubtribusLookupAsync()
    {
        if (_subtribusLoaded) return;

        var list = await _lookupService.GetSubtribusListAsync();
        SubtribusList.Clear();
        foreach (var item in list)
            SubtribusList.Add(item);

        _subtribusLoaded = true;
        OnPropertyChanged(nameof(SubtribusList));
    }

    public async Task EnsureTbl57TribusLookupAsync()
    {
        if (_tribusLoaded) return;

        var list = await _lookupService.GetTribusListAsync();
        TribusList.Clear();
        foreach (var item in list)
            TribusList.Add(item);

        _tribusLoaded = true;
        OnPropertyChanged(nameof(TribusList));
    }

    public async Task EnsureTbl54SupertribusLookupAsync()
    {
        if (_supertribusLoaded) return;

        var list = await _lookupService.GetSupertribusListAsync();
        SupertribusList.Clear();
        foreach (var item in list)
            SupertribusList.Add(item);

        _supertribusLoaded = true;
        OnPropertyChanged(nameof(SupertribusList));
    }

    //---------------------------------------------------------
    public async Task EnsureTbl51InfrafamilyLookupAsync()
    {
        if (_infrafamilyLoaded) return;

        var list = await _lookupService.GetInfrafamilyListAsync();
        InfrafamilyList.Clear();
        foreach (var item in list)
            InfrafamilyList.Add(item);

        _infrafamilyLoaded = true;
        OnPropertyChanged(nameof(InfrafamilyList));
    }

    public async Task EnsureTbl48SubfamilyLookupAsync()
    {
        if (_subfamilyLoaded) return;

        var list = await _lookupService.GetSubfamilyListAsync();
        SubfamilyList.Clear();
        foreach (var item in list)
            SubfamilyList.Add(item);

        _subfamilyLoaded = true;
        OnPropertyChanged(nameof(SubfamilyList));
    }

    public async Task EnsureTbl45FamilyLookupAsync()
    {
        if (_familyLoaded) return;

        var list = await _lookupService.GetFamilyListAsync();
        FamilyList.Clear();
        foreach (var item in list)
            FamilyList.Add(item);

        _familyLoaded = true;
        OnPropertyChanged(nameof(FamilyList));
    }

    public async Task EnsureTbl42SuperfamilyLookupAsync()
    {
        if (_superfamilyLoaded) return;

        var list = await _lookupService.GetSuperfamilyListAsync();
        SuperfamilyList.Clear();
        foreach (var item in list)
            SuperfamilyList.Add(item);

        _superfamilyLoaded = true;
        OnPropertyChanged(nameof(SuperfamilyList));
    }

    //---------------------------------------------------------
    public async Task EnsureTbl39InfraordoLookupAsync()
    {
        if (_infraordoLoaded) return;

        var list = await _lookupService.GetInfraordoListAsync();
        InfraordoList.Clear();
        foreach (var item in list)
            InfraordoList.Add(item);

        _infraordoLoaded = true;
        OnPropertyChanged(nameof(InfraordoList));
    }

    public async Task EnsureTbl36SubordoLookupAsync()
    {
        if (_subordoLoaded) return;

        var list = await _lookupService.GetSubordoListAsync();
        SubordoList.Clear();
        foreach (var item in list)
            SubordoList.Add(item);

        _subordoLoaded = true;
        OnPropertyChanged(nameof(SubordoList));
    }

    public async Task EnsureTbl33OrdoLookupAsync()
    {
        if (_ordoLoaded) return;

        var list = await _lookupService.GetOrdoListAsync();
        OrdoList.Clear();
        foreach (var item in list)
            OrdoList.Add(item);

        _ordoLoaded = true;
        OnPropertyChanged(nameof(OrdoList));
    }

    public async Task EnsureTbl30LegioLookupAsync()
    {
        if (_legioLoaded) return;

        var list = await _lookupService.GetLegioListAsync();
        LegioList.Clear();
        foreach (var item in list)
            LegioList.Add(item);

        _legioLoaded = true;
        OnPropertyChanged(nameof(LegioList));
    }

    //---------------------------------------------------------
    public async Task EnsureTbl27InfraclassLookupAsync()
    {
        if (_infraclassLoaded) return;

        var list = await _lookupService.GetInfraclassListAsync();
        InfraclassList.Clear();
        foreach (var item in list)
            InfraclassList.Add(item);

        _infraclassLoaded = true;
        OnPropertyChanged(nameof(InfraclassList));
    }

    public async Task EnsureTbl24SubclassLookupAsync()
    {
        if (_subclassLoaded) return;
        var list = await _lookupService.GetSubclassListAsync();
        SubclassList.Clear();
        foreach (var item in list)
            SubclassList.Add(item);

        _subclassLoaded = true;
        OnPropertyChanged(nameof(SubclassList));
    }

    public async Task EnsureTbl21ClassLookupAsync()
    {
        if (_classLoaded) return;

        var list = await _lookupService.GetClassListAsync();
        ClassList.Clear();
        foreach (var item in list)
            ClassList.Add(item);

        _classLoaded = true;
        OnPropertyChanged(nameof(ClassList));
    }

    public async Task EnsureTbl18SuperclassLookupAsync()
    {
        if (_superclassLoaded) return;

        var list = await _lookupService.GetSuperclassListAsync();
        SuperclassList.Clear();
        foreach (var item in list)
            SuperclassList.Add(item);

        _superclassLoaded = true;
        OnPropertyChanged(nameof(SuperclassList));
    }

    //---------------------------------------------------------
    public async Task EnsureTbl12SubphylumLookupAsync()
    {
        if (_subphylumLoaded) return;

        var list = await _lookupService.GetSubphylumListAsync();
        SubphylumList.Clear();
        foreach (var item in list)
            SubphylumList.Add(item);

        _subphylumLoaded = true;
        OnPropertyChanged(nameof(SubphylumList));
    }

    public async Task EnsureTbl06PhylumLookupAsync()
    {
        if (_phylumLoaded) return;

        var list = await _lookupService.GetPhylumListAsync();
        PhylumList.Clear();
        foreach (var item in list)
            PhylumList.Add(item);

        _phylumLoaded = true;
        OnPropertyChanged(nameof(PhylumList));
    }

    public async Task EnsureTbl03RegnumLookupAsync()
    {
        if (_regnumLoaded) return;

        var list = await _lookupService.GetRegnumListAsync();
        RegnumList.Clear();
        foreach (var item in list)
            RegnumList.Add(item);

        _regnumLoaded = true;
        OnPropertyChanged(nameof(RegnumList));
    }

    //---------------------------------------------------------
    public void ResetLookupLoadedFlags()
    {
        _speciesLoaded = false;
        _genusLoaded = false;
        _infratribusLoaded = false;
        _subtribusLoaded = false;
        _tribusLoaded = false;
        _supertribusLoaded = false;
        _infrafamilyLoaded = false;
        _subfamilyLoaded = false;
        _familyLoaded = false;
        _superfamilyLoaded = false;
        _infraordoLoaded = false;
        _subordoLoaded = false;
        _ordoLoaded = false;
        _legioLoaded = false;
        _infraclassLoaded = false;
        _subclassLoaded = false;
        _classLoaded = false;
        _superclassLoaded = false;
        _subphylumLoaded = false;
        _phylumLoaded = false;
        _regnumLoaded = false;
    }

    #endregion

    #region [ LOADASYNC ]
    // ---------------------------------------------------------

    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen

        _currentParentId = 0;
        _activeLoader = null;
        _assignParentFk = null;

        SelectedItem = null;

        await LoadReferenceLookupsAsync();
        await EnsureAllLookupsAsync();

        Items = new ObservableCollection<Models.Tbl90Reference>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    #endregion

    #region UI-Zustand vorbereiten
    // ---------------------------------------------------------
    public void PrepareViewState()
    {
        SelectedItem = null;
        Item = null;
    }
    public async Task ApplyFilterAsync(string? filter)
    {
        if (IsBossMode)
        {
            SearchText = filter;
            await ReloadAsync();
        }
    }
    #endregion

    #region LOAD FOR EACH PARENT TYPE
    // ---------------------------------------------------------
    public async Task LoadForRegnumAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForRegnumAsync, r => r.RegnumId = id);
    public async Task LoadForPhylumAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForPhylumAsync, r => r.PhylumId = id);
    public async Task LoadForSubphylumAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSubphylumAsync, r => r.SubphylumId = id);
    public async Task LoadForSuperclassAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSuperclassAsync, r => r.SuperclassId = id);
    public async Task LoadForClassAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForClassAsync, r => r.ClassId = id);
    public async Task LoadForSubclassAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSubclassAsync, r => r.SubclassId = id);
    public async Task LoadForInfraclassAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForInfraclassAsync, r => r.InfraclassId = id);
    public async Task LoadForLegioAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForLegioAsync, r => r.LegioId = id);
    public async Task LoadForOrdoAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForOrdoAsync, r => r.OrdoId = id);
    public async Task LoadForSubordoAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSubordoAsync, r => r.SubordoId = id);
    public async Task LoadForInfraordoAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForInfraordoAsync, r => r.InfraordoId = id);
    public async Task LoadForSuperfamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSuperfamilyAsync, r => r.SuperfamilyId = id);
    public async Task LoadForFamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForFamilyAsync, r => r.FamilyId = id);
    public async Task LoadForSubfamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSubfamilyAsync, r => r.SubfamilyId = id);
    public async Task LoadForInfrafamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForInfrafamilyAsync, r => r.InfrafamilyId = id);
    public async Task LoadForSupertribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSupertribusAsync, r => r.SupertribusId = id);
    public async Task LoadForTribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForTribusAsync, r => r.TribusId = id);

    public async Task LoadForSubtribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSubtribusAsync, r => r.SubtribusId = id);

    public async Task LoadForInfratribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForInfratribusAsync, r => r.InfratribusId = id);

    public async Task LoadForGenusAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForGenusAsync, r => r.GenusId = id);

    public async Task LoadForSpeciesAsync(int id)
        => await LoadInternalAsync(id, Data.GetReferencesForSpeciesAsync, r => r.SpeciesId = id);

    #endregion

    #region INTERNAL LOADER
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl90Reference>>> loader,
        Action<Models.Tbl90Reference> assignParent)
    {
        _currentParentId = parentId;
        _activeLoader = loader;
        _assignParentFk = assignParent;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl90Reference>(items);

        DataSetCount = Items.Count;
    }
    #endregion

    #region LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchReferenceAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl90Reference>();
    }
    #endregion

    #region CHILDREN (References haben keine Children)
    // ---------------------------------------------------------

    protected override Task LoadChildrenAsync(int entityId)
        => Task.CompletedTask;

    #endregion

    #region UI STATE
    // ---------------------------------------------------------                   

    private Models.Tbl90Reference? _item;
    public Models.Tbl90Reference? Item
    {
        get => _item;
        set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasActiveItem));
            }
        }
    }

    public bool HasActiveItem => Item != null;

    #endregion

    #region ViewModel
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem != null)
            await EnsureAllLookupsAsync();

        if (!ReferenceEquals(Item, SelectedItem))
            Item = SelectedItem;
    }
    private async Task EnsureAllLookupsAsync()
    {
        for (int i = 0; i <= 5; i++)
            await EnsureLookupForTabAsync(i);
    }
    #endregion

    #region NEW
    // ---------------------------------------------------------

    protected override Models.Tbl90Reference CreateNewItem()
    {
        var r = new Models.Tbl90Reference();

        _assignParentFk?.Invoke(r);
        return r;
    }

    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        await Task.Delay(50);
    }
    #endregion

    #region SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
    {
        if (string.IsNullOrWhiteSpace(item.Info))
        {
            await Dialog.ShowErrorAsync("Error_MissInfo_Title", "Error_MissInfo_Message");
            return null;
        }

        var saved = await Data.UpsertReferenceAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }

        if (_activeLoader != null)
        {
            var list = await _activeLoader(_currentParentId);
            Items = new ObservableCollection<Models.Tbl90Reference>(list);
        }

        SelectedItem = Items.FirstOrDefault(x => x.ReferenceId == saved.ReferenceId);
        SearchText = saved.Info;

        return saved;
    }
    #endregion

    #region DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl90Reference item)
    {
        if (item.ReferenceId == 0)
            return false;

        await Data.DeleteReferenceAsync(item.ReferenceId);
        return true;
    }

    #endregion

    #region CONFIRM
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90Reference item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90Reference item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");
    #endregion

    #region Get

    protected override int GetId(Models.Tbl90Reference item)
        => item.ReferenceId;

    protected override object GetSortKey(Models.Tbl90Reference item, string column)
    {
        return column switch
        {
            "Id" => item.ReferenceId,
            "Info" => item.Info ?? "",
            _ => ""
        };
    }
    #endregion

}




