using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
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

    // dynamische Parent-Zuweisung
    private Action<Models.Tbl90Reference>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl90Reference>>>? _loadByParent;

    public Tbl90ReferenceViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;
        IsReferenceDetailVisible = false;
    }
    // ---------------------------------------------------------
    // UI STATE
    // ---------------------------------------------------------

    private bool _isReferenceDetailVisible;
    public bool IsReferenceDetailVisible
    {
        get => _isReferenceDetailVisible;
        set => SetProperty(ref _isReferenceDetailVisible, value);
    }

    // ---------------------------------------------------------
    // RESOLVE PARENT
    // ---------------------------------------------------------

    private void ResolveParent<TParent>(int parentId)
    {
        if (typeof(TParent) == typeof(Models.Tbl75Species))
        {
            _assignParentFk = r => r.SpeciesId = parentId;
            _loadByParent = Data.GetReferencesForSpeciesAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl66Genus))
        {
            _assignParentFk = r => r.GenusId = parentId;
            _loadByParent = Data.GetReferencesForGenusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl63Infratribus))
        {
            _assignParentFk = r => r.InfratribusId = parentId;
            _loadByParent = Data.GetReferencesForInfratribusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl60Subtribus))
        {
            _assignParentFk = r => r.SubtribusId = parentId;
            _loadByParent = Data.GetReferencesForSubtribusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl57Tribus))
        {
            _assignParentFk = r => r.TribusId = parentId;
            _loadByParent = Data.GetReferencesForTribusAsync;
        }

        // … und so weiter für alle 20 Ebenen …
    }

    // ---------------------------------------------------------
    // LOAD PARENT (Boss-Konfiguration)
    // ---------------------------------------------------------
    public async Task LoadParentAsync<TParent>(int parentId)
    {
        _currentParentId = parentId;

        ResolveParent<TParent>(parentId);

        var items = _loadByParent != null
            ? await _loadByParent(parentId)
            : Enumerable.Empty<Models.Tbl90Reference>();

        Items = new ObservableCollection<Models.Tbl90Reference>(items);
        DataSetCount = Items.Count;

        SelectedItem = null;
        IsReferenceDetailVisible = false;
    }
    // ---------------------------------------------------------
    // LOAD PARENT (UI-Modus)
    // ---------------------------------------------------------
    public override async Task LoadParentAsync(int parentId)
    {
        if (_loadByParent == null)
            return;

        var items = await _loadByParent(parentId);
        Items = new ObservableCollection<Models.Tbl90Reference>(items);

        DataSetCount = Items.Count;

        SelectedItem = null;
        IsReferenceDetailVisible = false;
    }
    // ---------------------------------------------------------
    // LOAD LOOKUOPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; } = new();
    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; } = new();
    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; } = new();

    public async Task LoadReferenceLookupsAsync()
    {
        RefAuthorsList.Clear();
        RefExpertsList.Clear();
        RefSourcesList.Clear();

        var authors = await _lookupService.GetRefAuthorListAsync();
        var experts = await _lookupService.GetRefExpertListAsync();
        var sources = await _lookupService.GetRefSourceListAsync();

        foreach (var a in authors) RefAuthorsList.Add(a);
        foreach (var e in experts) RefExpertsList.Add(e);
        foreach (var s in sources) RefSourcesList.Add(s);
    }

    // ---------------------------------------------------------
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        _loadByParent = null;
        _currentParentId = 0;

        await LoadReferenceLookupsAsync();

        Items = new ObservableCollection<Models.Tbl90Reference>();
        SelectedItem = null;
        IsReferenceDetailVisible = false;

        await ReloadAsync();
    }
    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
    {
        if (_loadByParent != null && _currentParentId > 0)
            return await _loadByParent(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchReferenceAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl90Reference>();
    }
    // ---------------------------------------------------------
    // SELECTED ITEM (Boss ohne Children)
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        IsReferenceDetailVisible = SelectedItem != null;

        await base.OnSelectedItemChangedAsync();
    }

    // ---------------------------------------------------------
    // OHNE CHILDREN
    // ---------------------------------------------------------
    protected override Task LoadChildrenAsync(int entityId) => Task.CompletedTask;

    protected override Task ClearChildCollectionsAsync() => Task.CompletedTask;
    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        IsReferenceDetailVisible = true;
        await Task.Delay(50);
    }

    protected override Models.Tbl90Reference CreateNewItem()
    {
        var c = new Models.Tbl90Reference();

        _assignParentFk?.Invoke(c);
        return c;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
    {
        if (await Data.ReferenceExistsAsync(item))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null; // IMPORTANT: base soll nicht reload+select auf "unsaved"
        }

        var saved = await Data.UpsertReferenceAsync(item);
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

    // ---------------------------------------------------------
    // CONFIRM
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90Reference item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90Reference item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl90Reference item)
        => item.ReferenceId;
}




