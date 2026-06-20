using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
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

    // dynamische Parent-Zuweisung
    private Action<Models.Tbl93Comment>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl93Comment>>>? _loadByParent;

    public Tbl93CommentViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookup,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        IsCommentDetailVisible = false;
        _lookupService = lookup;
    }

    // ---------------------------------------------------------
    // UI STATE
    // ---------------------------------------------------------
    private bool _isCommentDetailVisible;
    public bool IsCommentDetailVisible
    {
        get => _isCommentDetailVisible;
        set => SetProperty(ref _isCommentDetailVisible, value);
    }

    // ---------------------------------------------------------
    // RESOLVE PARENT
    // ---------------------------------------------------------
    private void ResolveParent<TParent>(int parentId)
    {
        if (typeof(TParent) == typeof(Models.Tbl75Species))
        {
            _assignParentFk = c => c.SpeciesId = parentId;
            _loadByParent = Data.GetCommentsForSpeciesAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl66Genus))
        {
            _assignParentFk = c => c.GenusId = parentId;
            _loadByParent = Data.GetCommentsForGenusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl63Infratribus))
        {
            _assignParentFk = c => c.InfratribusId = parentId;
            _loadByParent = Data.GetCommentsForInfratribusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl60Subtribus))
        {
            _assignParentFk = c => c.SubtribusId = parentId;
            _loadByParent = Data.GetCommentsForSubtribusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl57Tribus))
        {
            _assignParentFk = c => c.TribusId = parentId;
            _loadByParent = Data.GetCommentsForTribusAsync;
        }

        // später:
        // else if (typeof(TParent) == typeof(Tbl54Familia)) ...
        // else if (typeof(TParent) == typeof(Tbl51Ordo)) ...
        // else if (typeof(TParent) == typeof(Tbl48Classis)) ...
        // else if (typeof(TParent) == typeof(Tbl45Phylum)) ...
        // else if (typeof(TParent) == typeof(Tbl42Regnum)) ...
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
            : Enumerable.Empty<Models.Tbl93Comment>();

        Items = new ObservableCollection<Models.Tbl93Comment>(items);
        DataSetCount = Items.Count;

        // WICHTIG: Parent-Modus
        SelectedItem = null;
        IsCommentDetailVisible = false;
    }

    // ---------------------------------------------------------
    // LOAD PARENT (UI-Modus)
    // ---------------------------------------------------------
    public override async Task LoadParentAsync(int parentId)
    {
        if (_loadByParent == null)
            return;

        var items = await _loadByParent(parentId);
        Items = new ObservableCollection<Models.Tbl93Comment>(items);

        DataSetCount = Items.Count;

        SelectedItem = null;
        IsCommentDetailVisible = false;
    }

    // ---------------------------------------------------------
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        _loadByParent = null;
        _currentParentId = 0;

        Items = new ObservableCollection<Models.Tbl93Comment>();
        SelectedItem = null;
        IsCommentDetailVisible = false;

        await ReloadAsync();
    }
    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
    {
        if (_loadByParent != null && _currentParentId > 0)
            return await _loadByParent(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchCommentAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl93Comment>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM (Boss ohne Children)
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        IsCommentDetailVisible = SelectedItem != null;

        // keine Children → nichts laden/clearen

        await base.OnSelectedItemChangedAsync();
    }

    // ---------------------------------------------------------
    // OHNE CHILDS
    // ---------------------------------------------------------

    protected override Task LoadChildrenAsync(int entityId)
    {
        // Comments haben keine Children
        return Task.CompletedTask;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        // Comments haben keine Children
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        IsCommentDetailVisible = true;
        await Task.Delay(50);
    }

    protected override Models.Tbl93Comment CreateNewItem()
    {
        var c = new Models.Tbl93Comment();

        _assignParentFk?.Invoke(c);
        return c;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
    {
        if (await Data.CommentExistsAsync(item))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null; // IMPORTANT: base soll nicht reload+select auf "unsaved"
        }

        var saved = await Data.UpsertCommentAsync(item);
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

    // ---------------------------------------------------------
    // CONFIRM
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl93Comment item)
        => item.CommentId;
}






