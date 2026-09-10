using System.Collections.ObjectModel;
using System.Diagnostics;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl90Reference;

public class Tbl90ReferenceViewModel : BaseBossListViewModel<Models.Tbl90Reference>
{
    private readonly LookupService _lookupService;

    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.Tbl90Reference>>>? _activeLoader;
    private Action<Models.Tbl90Reference>? _assignParentFk;

    // ---------------------------------------------------------
    // LOOKUPS
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

    //public async Task LoadReferenceLookupsAsync()
    //{
    //    var listrefauthor = await _lookupService.GetRefAuthorListAsync();
    //    var lookuprefauthor = await _lookupService.GetRefAuthorLookupAsync();

    //    RefAuthorsList.Clear();
    //    foreach (var item in listrefauthor)
    //    {
    //        RefAuthorsList.Add(item);
    //    }

    //    RefAuthorLookup = lookuprefauthor;

    //    OnPropertyChanged(nameof(RefAuthorLookup));
    //    //---------------------------------------------
    //    var listrefexpert = await _lookupService.GetRefExpertListAsync();
    //    var lookuprefexpert = await _lookupService.GetRefExpertLookupAsync();

    //    RefExpertsList.Clear();
    //    foreach (var item in listrefexpert)
    //    {
    //        RefExpertsList.Add(item);
    //    }

    //    RefExpertLookup = lookuprefexpert;

    //    OnPropertyChanged(nameof(RefExpertLookup));
    //    //---------------------------------------------
    //    var listrefsource = await _lookupService.GetRefSourceListAsync();
    //    var lookuprefsource = await _lookupService.GetRefSourceLookupAsync();

    //    RefSourcesList.Clear();
    //    foreach (var item in listrefsource)
    //    {
    //        RefSourcesList.Add(item);
    //    }

    //    RefSourceLookup = lookuprefsource;

    //    OnPropertyChanged(nameof(RefSourceLookup));
    //}

    //public async Task LoadReferenceLookupsAsync()
    //{
    //    RefAuthorsList.Clear();
    //    RefExpertsList.Clear();
    //    RefSourcesList.Clear();

    //    var authorslist = await _lookupService.GetRefAuthorListAsync();
    //    var expertslist = await _lookupService.GetRefExpertListAsync();
    //    var sourceslist = await _lookupService.GetRefSourceListAsync();

    //    var lookupauthor = await _lookupService.GetRefAuthorLookupAsync();
    //    var lookupexpert = await _lookupService.GetRefExpertLookupAsync();
    //    var lookupsource = await _lookupService.GetRefSourceLookupAsync();

    //    foreach (var a in authorslist) RefAuthorsList.Add(a);
    //    foreach (var e in expertslist) RefExpertsList.Add(e);
    //    foreach (var s in sourceslist) RefSourcesList.Add(s);

    //    RefAuthorLookup = lookupauthor;
    //    RefExpertLookup = lookupexpert;
    //    RefSourceLookup = lookupsource;

    //    OnPropertyChanged(nameof(RefAuthorsList));
    //    OnPropertyChanged(nameof(RefExpertsList));
    //    OnPropertyChanged(nameof(RefSourcesList));

    //    OnPropertyChanged(nameof(RefAuthorLookup));
    //    OnPropertyChanged(nameof(RefExpertLookup));
    //    OnPropertyChanged(nameof(RefSourceLookup));
    //}

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

    public async Task LoadAsync()
    {
        _currentParentId = 0;
        _activeLoader = null;
        _assignParentFk = null;

        await LoadReferenceLookupsAsync();

        SelectedItem = null;


    }

    // ---------------------------------------------------------
    // LOAD FOR EACH PARENT TYPE
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

    // ---------------------------------------------------------
    // INTERNAL LOADER
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

        SelectedItem = null;
        Item = null;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchReferenceAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl90Reference>();
    }

    // ---------------------------------------------------------
    // CHILDREN (References haben keine Children)
    // ---------------------------------------------------------

    protected override Task LoadChildrenAsync(int entityId)
        => Task.CompletedTask;


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

    public void PrepareViewState()
    {
        SelectedItem = null;
        Item = null;
    }

    protected override async Task OnSelectedItemChangedAsync()
    {
        Item = SelectedItem;   // ⭐ EditControl bekommt Daten

        // References haben keine Children
        await Task.CompletedTask;
    }


    // ---------------------------------------------------------
    // NEW
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
        //  IsDetailVisible = true;
        await Task.Delay(50);
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
    {
        if (string.IsNullOrWhiteSpace(item.Info))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
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

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl90Reference item)
    {
        if (item.ReferenceId == 0)
            return false;

        await Data.DeleteReferenceAsync(item.ReferenceId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90Reference item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90Reference item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

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
}




