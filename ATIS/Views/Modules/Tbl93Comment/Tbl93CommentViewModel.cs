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

    //public override Task LoadLookupsAsync()
    //    => Task.CompletedTask;

    public async Task LoadAsync()
    {
        _currentParentId = 0;
        _activeLoader = null;
        _assignParentFk = null;

  //      kein Lookupawait 

        SelectedItem = null;
             //   IsDetailVisible = false;
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
        => await LoadInternalAsync(id, Data.GetCommentsForClassAsync, c => c.SuperclassId = id);



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

        //if (Items.Any())
        //{
        //    SelectedItem = Items.First();
        //    IsDetailVisible = true;
        //}

        // ❗ Parent-Comments: KEINE Auto-Auswahl
        SelectedItem = null;
        Item = null;
       // IsDetailVisible = false;
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
        set => SetProperty(ref _item, value);
    }
    protected override async Task OnSelectedItemChangedAsync()
    {
        Item = SelectedItem;   // ⭐ EditControl bekommt Daten
        //  IsDetailVisible = SelectedItem != null;

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
        //  IsDetailVisible = true;
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


//public class Tbl93CommentViewModel : BaseHierarchyBossViewModel<Models.Tbl93Comment>
//{
//    private readonly LookupService _lookupService;
//    private int _currentParentId;

//    public Tbl93CommentViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookupService,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        _lookupService = lookupService;
//    }

//    // ---------------------------------------------------------
//    // LOOKUPS (falls benötigt)
//    // ---------------------------------------------------------
//    public override Task LoadLookupsAsync()
//        => Task.CompletedTask; // Comments brauchen keine Lookups

//    // ---------------------------------------------------------
//    // BOSS LOAD (optional)
//    // ---------------------------------------------------------
//    public override async Task LoadAsync()
//    {
//        _currentParentId = 0;

//        await base.LoadAsync(); // ruft LoadLookupsAsync + ReloadAsync

//        SelectedItem = null;
//        IsDetailVisible = false;
//    }
//    // ---------------------------------------------------------
//    // LOAD FOR EACH PARENT TYPE
//    // ---------------------------------------------------------

//    //public async Task LoadForRegnumAsync(int id)
//    //    => await LoadInternalAsync(id, Data.GetCommentsForRegnumAsync);

//    //public async Task LoadForPhylumAsync(int id)
//    //    => await LoadInternalAsync(id, Data.GetCommentsForPhylumAsync);

//    //public async Task LoadForClassAsync(int id)
//    //    => await LoadInternalAsync(id, Data.GetCommentsForClassAsync);

//    //public async Task LoadForOrdoAsync(int id)
//    //    => await LoadInternalAsync(id, Data.GetCommentsForOrdoAsync);

//    //public async Task LoadForFamilyAsync(int id)
//    //    => await LoadInternalAsync(id, Data.GetCommentsForFamilyAsync);

//    public async Task LoadForTribusAsync(int id)
//        => await LoadInternalAsync(id, Data.GetCommentsForTribusAsync);

//    public async Task LoadForSubtribusAsync(int id)
//        => await LoadInternalAsync(id, Data.GetCommentsForSubtribusAsync);

//    public async Task LoadForInfratribusAsync(int id)
//        => await LoadInternalAsync(id, Data.GetCommentsForInfratribusAsync);

//    public async Task LoadForGenusAsync(int id)
//        => await LoadInternalAsync(id, Data.GetCommentsForGenusAsync);

//    public async Task LoadForSpeciesAsync(int id)
//        => await LoadInternalAsync(id, Data.GetCommentsForSpeciesAsync);

//    // ---------------------------------------------------------
//    // INTERNAL LOADER
//    // ---------------------------------------------------------

//    private async Task LoadInternalAsync(
//        int parentId,
//        Func<int, Task<IEnumerable<Models.Tbl93Comment>>> loader)
//    {
//        _currentParentId = parentId;

//        var items = await loader(parentId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);

//        DataSetCount = Items.Count;
//        SelectedItem = null;
//        IsDetailVisible = false;
//    }

//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        throw new NotImplementedException();
//    }

//    // ---------------------------------------------------------
//    // NEW
//    // ---------------------------------------------------------

//    protected override async Task NewAsync()
//    {
//        throw new NotImplementedException();
//    }

//    protected override Models.Tbl93Comment CreateNewItem()
//        => new() { SubtribusId = _currentParentId }; // oder SubtribusId etc.

//    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
//    {
//        throw new NotImplementedException();
//    }

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------

//    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
//    {
//        if (string.IsNullOrWhiteSpace(item.Info))
//        {
//            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return null;
//        }

//        var saved = await Data.UpsertCommentAsync(item);

//        // Liste neu laden
//        var items = await Data.GetCommentsForSubtribusAsync(_currentParentId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);

//        SelectedItem = Items.FirstOrDefault(x => x.CommentId == saved.CommentId);
//        SearchText = saved.Info;

//        return saved;
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------

//    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
//    {
//        if (item.CommentId == 0)
//            return false;

//        await Data.DeleteCommentAsync(item.CommentId);
//        return true;
//    }

//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
//        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
//        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");


//    protected override int GetId(Models.Tbl93Comment item)
//        => item.CommentId;
//}


// ---------------------------------------------------------
// LOAD FOR SUBTRIBUS (Parent)
// ---------------------------------------------------------
//public async Task LoadForSubtribusAsync(int subtribusId)
//{
//    _currentParentId = subtribusId;

//    var items =  await Data.GetCommentsForSubtribusAsync(subtribusId);
//    Items = new ObservableCollection<Models.Tbl93Comment>(items);

//    DataSetCount = Items.Count;
//    SelectedItem = null;
//    IsDetailVisible = false;
//}

// ---------------------------------------------------------
// LOAD ITEMS (Boss)
// ---------------------------------------------------------
//protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
//{
//    if (_currentParentId > 0)
//        return await Data.GetCommentsForSubtribusAsync(_currentParentId);

//    if (!string.IsNullOrWhiteSpace(SearchText))
//        return await Tbl75SpeciesAltViewModel.Data.SearchCommentAsync(SearchText);

//    return Items ?? Enumerable.Empty<Models.Tbl93Comment>();
//}

// ---------------------------------------------------------
// SELECTED ITEM
// ---------------------------------------------------------
//protected override Task OnSelectedItemChangedAsync()
//{
//    IsDetailVisible = SelectedItem != null;
//    return Task.CompletedTask;
//}

// ---------------------------------------------------------
// NEW
// ---------------------------------------------------------
//protected override Models.Tbl93Comment CreateNewItem()
//    => new() { SubtribusId = _currentParentId };

//protected override async Task NewAsync()
//{
//    SelectedItem = CreateNewItem();
//    IsDetailVisible = true;
//    await Task.Delay(50);
//}

// ---------------------------------------------------------
// SAVE
// ---------------------------------------------------------
//protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
//{
//    if (string.IsNullOrWhiteSpace(item.Info))
//    {
//        await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//        return null;
//    }

//    var saved = await Tbl75SpeciesAltViewModel.Data.UpsertCommentAsync(item);

//    // Parent-Liste neu laden
//    var list = await Tbl75SpeciesAltViewModel.Data.GetCommentsForSubtribusAsync(_currentParentId);
//    Items = new ObservableCollection<Models.Tbl93Comment>(list);

//    SelectedItem = Items.FirstOrDefault(x => x.CommentId == saved.CommentId);
//    SearchText = saved.Info;

//    return saved;
//}

// ---------------------------------------------------------
// DELETE
// ---------------------------------------------------------
//    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
//    {
//        if (item.CommentId == 0)
//            return false;

//        await Tbl75SpeciesAltViewModel.Data.DeleteCommentAsync(item.CommentId);
//        return true;
//    }

//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
//        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
//        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        throw new NotImplementedException();
//    }

//    protected override int GetId(Models.Tbl93Comment item)
//        => item.CommentId;
//}



//public class Tbl93CommentViewModel : BaseBossListViewModel<Models.Tbl93Comment>
//{
//    private readonly LookupService _lookupService;
//    private int _currentParentId;

//    // dynamische Parent-Zuweisung
//    private Action<Models.Tbl93Comment>? _assignParentFk;
//    private Func<int, Task<IEnumerable<Models.Tbl93Comment>>>? _loadByParent;

//    public Tbl93CommentViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookup,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        IsCommentDetailVisible = false;
//        _lookupService = lookup;
//    }

//    // ---------------------------------------------------------
//    // UI STATE
//    // ---------------------------------------------------------
//    private bool _isCommentDetailVisible;
//    public bool IsCommentDetailVisible
//    {
//        get => _isCommentDetailVisible;
//        set => SetProperty(ref _isCommentDetailVisible, value);
//    }

//    // ---------------------------------------------------------
//    // RESOLVE PARENT
//    // ---------------------------------------------------------
//    private void ResolveParent<TParent>(int parentId)
//    {
//        if (typeof(TParent) == typeof(Models.Tbl75Species))
//        {
//            _assignParentFk = c => c.SpeciesId = parentId;
//            _loadByParent = Data.GetCommentsForSpeciesAsync;
//        }
//        else if (typeof(TParent) == typeof(Models.Tbl66Genus))
//        {
//            _assignParentFk = c => c.GenusId = parentId;
//            _loadByParent = Data.GetCommentsForGenusAsync;
//        }
//        else if (typeof(TParent) == typeof(Models.Tbl63Infratribus))
//        {
//            _assignParentFk = c => c.InfratribusId = parentId;
//            _loadByParent = Data.GetCommentsForInfratribusAsync;
//        }
//        else if (typeof(TParent) == typeof(Models.Tbl60Subtribus))
//        {
//            _assignParentFk = c => c.SubtribusId = parentId;
//            _loadByParent = Data.GetCommentsForSubtribusAsync;
//        }
//        else if (typeof(TParent) == typeof(Models.Tbl57Tribus))
//        {
//            _assignParentFk = c => c.TribusId = parentId;
//            _loadByParent = Data.GetCommentsForTribusAsync;
//        }

//        // später:
//        // else if (typeof(TParent) == typeof(Tbl54Familia)) ...
//        // else if (typeof(TParent) == typeof(Tbl51Ordo)) ...
//        // else if (typeof(TParent) == typeof(Tbl48Classis)) ...
//        // else if (typeof(TParent) == typeof(Tbl45Phylum)) ...
//        // else if (typeof(TParent) == typeof(Tbl42Regnum)) ...
//    }

//    // ---------------------------------------------------------
//    // LOAD PARENT (Boss-Konfiguration)
//    // ---------------------------------------------------------
//    public async Task LoadParentAsync<TParent>(int parentId)
//    {
//        _currentParentId = parentId;

//        ResolveParent<TParent>(parentId);

//        var items = _loadByParent != null
//            ? await _loadByParent(parentId)
//            : Enumerable.Empty<Models.Tbl93Comment>();

//        Items = new ObservableCollection<Models.Tbl93Comment>(items);
//        DataSetCount = Items.Count;

//        // WICHTIG: Parent-Modus
//        SelectedItem = null;
//        IsCommentDetailVisible = false;
//    }

//    // ---------------------------------------------------------
//    // LOAD PARENT (UI-Modus)
//    // ---------------------------------------------------------
//    public override async Task LoadParentAsync(int parentId)
//    {
//        if (_loadByParent == null)
//            return;

//        var items = await _loadByParent(parentId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);

//        DataSetCount = Items.Count;

//        SelectedItem = null;
//        IsCommentDetailVisible = false;
//    }

//    // ---------------------------------------------------------
//    // LOAD ASYNC (Boss)
//    // ---------------------------------------------------------
//    public async Task LoadAsync()
//    {
//        _loadByParent = null;
//        _currentParentId = 0;

//        Items = new ObservableCollection<Models.Tbl93Comment>();
//        SelectedItem = null;
//        IsCommentDetailVisible = false;

//        await ReloadAsync();
//    }
//    // ---------------------------------------------------------
//    // LOAD ITEMS (Boss)
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
//    {
//        if (_loadByParent != null && _currentParentId > 0)
//            return await _loadByParent(_currentParentId);

//        if (!string.IsNullOrWhiteSpace(SearchText))
//            return await Data.SearchCommentAsync(SearchText);

//        return Items ?? Enumerable.Empty<Models.Tbl93Comment>();
//    }

//    // ---------------------------------------------------------
//    // SELECTED ITEM (Boss ohne Children)
//    // ---------------------------------------------------------
//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        IsCommentDetailVisible = SelectedItem != null;

//        // keine Children → nichts laden/clearen

//        await base.OnSelectedItemChangedAsync();
//    }

//    // ---------------------------------------------------------
//    // OHNE CHILDS
//    // ---------------------------------------------------------
//    public async Task LoadForSubtribusAsync(int subtribusId)
//    {
//        _currentParentId = subtribusId;

//        var items = await Data.GetCommentsForSubtribusAsync(subtribusId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);

//        DataSetCount = Items.Count;
//        SelectedItem = null;
//    }

//    protected override Task LoadChildrenAsync(int entityId)
//    {
//        // Comments haben keine Children
//        return Task.CompletedTask;
//    }

//    protected override Task ClearChildCollectionsAsync()
//    {
//        // Comments haben keine Children
//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // NEW
//    // ---------------------------------------------------------
//    protected override async Task NewAsync()
//    {
//        SelectedItem = CreateNewItem();
//        IsCommentDetailVisible = true;
//        await Task.Delay(50);
//    }

//    protected override Models.Tbl93Comment CreateNewItem()
//    {
//        var c = new Models.Tbl93Comment();

//        _assignParentFk?.Invoke(c);
//        return c;
//    }

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
//    {
//        if (await Data.CommentExistsAsync(item))
//        {
//            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return null; // IMPORTANT: base soll nicht reload+select auf "unsaved"
//        }

//        var saved = await Data.UpsertCommentAsync(item);
//        return saved;
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
//    {
//        if (item.CommentId == 0)
//            return false;

//        await Data.DeleteCommentAsync(item.CommentId);
//        return true;
//    }

//    // ---------------------------------------------------------
//    // CONFIRM
//    // ---------------------------------------------------------
//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
//        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
//        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

//    // ---------------------------------------------------------
//    // ID
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl93Comment item)
//        => item.CommentId;
//}






