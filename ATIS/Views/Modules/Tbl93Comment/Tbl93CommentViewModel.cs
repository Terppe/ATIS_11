using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;


namespace ATIS.Views.Modules.Tbl93Comment;

public class Tbl93CommentViewModel : BaseBossListViewModel<Models.Tbl93Comment>
{
    private readonly LookupService _lookupService;

    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.Tbl93Comment>>>? _activeLoader;
    private Action<Models.Tbl93Comment>? _assignParentFk;

    public Tbl93CommentViewModel(
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

        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // UI-Zustand vorbereiten
    // ---------------------------------------------------------
    public void PrepareViewState()
    {
        SelectedItem = null;
        Item = null;
    }

    // ---------------------------------------------------------
    // LOAD FOR EACH PARENT TYPE
    // ---------------------------------------------------------
    public async Task LoadForRegnumAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForRegnumAsync, c => c.RegnumId = id);
    public async Task LoadForPhylumAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForPhylumAsync, c => c.PhylumId = id);
    public async Task LoadForSubphylumAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSubphylumAsync, c => c.SubphylumId = id);
    public async Task LoadForSuperclassAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSuperclassAsync, c => c.SuperclassId = id);
    public async Task LoadForClassAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForClassAsync, c => c.ClassId = id);
    public async Task LoadForSubclassAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSubclassAsync, c => c.SubclassId = id);
    public async Task LoadForInfraclassAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForInfraclassAsync, c => c.InfraclassId = id);
    public async Task LoadForLegioAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForLegioAsync, c => c.LegioId = id);
    public async Task LoadForOrdoAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForOrdoAsync, c => c.OrdoId = id);
    public async Task LoadForSubordoAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSubordoAsync, c => c.SubordoId = id);
    public async Task LoadForInfraordoAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForInfraordoAsync, c => c.InfraordoId = id);
    public async Task LoadForSuperfamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSuperfamilyAsync, c => c.SuperfamilyId = id);
    public async Task LoadForFamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForFamilyAsync, c => c.FamilyId = id);
    public async Task LoadForSubfamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSubfamilyAsync, c => c.SubfamilyId = id);
    public async Task LoadForInfrafamilyAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForInfrafamilyAsync, c => c.InfrafamilyId = id);
    public async Task LoadForSupertribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSupertribusAsync, c => c.SupertribusId = id);

    public async Task LoadForTribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForTribusAsync, c => c.TribusId = id);

    public async Task LoadForSubtribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSubtribusAsync, c => c.SubtribusId = id);

    public async Task LoadForInfratribusAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForInfratribusAsync, c => c.InfratribusId = id);

    public async Task LoadForGenusAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForGenusAsync, c => c.GenusId = id);

    public async Task LoadForSpeciesAsync(int id)
        => await LoadInternalAsync(id, Data.GetCommentsForSpeciesAsync, c => c.SpeciesId = id);

    // ---------------------------------------------------------
    // INTERNAL LOADER
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl93Comment>>> loader,
        Action<Models.Tbl93Comment> assignParent)
    {
        _currentParentId = parentId;
        _activeLoader = loader;
        _assignParentFk = assignParent;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl93Comment>(items);

        DataSetCount = Items.Count;

        // ❗ Parent-Comments: KEINE Auto-Auswahl
        SelectedItem = null;
        Item = null;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchCommentAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl93Comment>();
    }

    // ---------------------------------------------------------
    // CHILDREN (Comments haben keine Children)
    // ---------------------------------------------------------

    protected override Task LoadChildrenAsync(int entityId)
        => Task.CompletedTask;
    protected override Task ClearChildCollectionsAsync()
        => Task.CompletedTask;

    private Models.Tbl93Comment? _item;
    public Models.Tbl93Comment? Item
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

    protected override async Task OnSelectedItemChangedAsync()
    {
        Item = SelectedItem;   // ⭐ EditControl bekommt Daten

        // Comments haben keine Children
        await Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Models.Tbl93Comment CreateNewItem()
    {
        var c = new Models.Tbl93Comment();

        _assignParentFk?.Invoke(c);
        return c;
    }

    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        await Task.Delay(50);
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
    {
        if (string.IsNullOrWhiteSpace(item.Info))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertCommentAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();  // Nötig??
        }

        if (_activeLoader != null)
        {
            var list = await _activeLoader(_currentParentId);
            Items = new ObservableCollection<Models.Tbl93Comment>(list);
        }

        SelectedItem = Items.FirstOrDefault(x => x.CommentId == saved.CommentId);
        SearchText = saved.Info;

        return saved;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
    {
        if (item.CommentId == 0)
            return false;

        await Data.DeleteCommentAsync(item.CommentId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl93Comment item)
        => item.CommentId;

    protected override object GetSortKey(Models.Tbl93Comment item, string column)
    {
        return column switch
        {
            "Id" => item.CommentId,
            "Info" => item.Info ?? "",
            _ => ""
        };
    }
}






