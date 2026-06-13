using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl93Comment;

public class Tbl93CommentViewModel : BaseBossListViewModel<Models.Tbl93Comment>
{
    private readonly LookupService _lookupService;

    // Der Parent wird durch genau EINE FK bestimmt
    private Action<Models.Tbl93Comment>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl93Comment>>>? _loadByParent;
    private int _currentParentId;

    public Tbl93CommentViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;
        IsCommentDetailVisible = false;
    }

    private bool _isCommentDetailVisible;
    public bool IsCommentDetailVisible
    {
        get => _isCommentDetailVisible;
        set => SetProperty(ref _isCommentDetailVisible, value);
    }

    //public override async Task LoadParentAsync(int parentId)
    //{
    //    Debug.WriteLine("LOAD PARENT COMMENTS");

    //    _currentParentId = parentId;

    //    // WICHTIG: Parent-FK setzen
    //    _assignParentFk = c => c.SubtribusId = parentId;

    //    var items = await Data.GetCommentsForSubtribusAsync(parentId);
    //    Items = new ObservableCollection<Models.Tbl93Comment>(items);

    //    DataSetCount = Items.Count;

    //    SelectedItem = null;
    //    IsCommentDetailVisible = false;
    //}
    // in Tbl93CommentViewModel


    // ---------------------------------------------------------
    // LOAD PARENT (für ALLE 20 Ebenen)
    // ---------------------------------------------------------
    public async Task LoadParentAsync<TParent>(int parentId)
    {
        Debug.WriteLine($"LOAD PARENT COMMENTS for {typeof(TParent).Name}, Id={parentId}");

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
        // … weiter für alle Ebenen …

        var items = _loadByParent != null
            ? await _loadByParent(parentId)
            : Enumerable.Empty<Models.Tbl93Comment>();

        Items = new ObservableCollection<Models.Tbl93Comment>(items);
        DataSetCount = Items.Count;

        SelectedItem = null;
        IsCommentDetailVisible = false;
    }

    private async Task LoadItemsForParent(int parentId)
    {
        if (_loadByParent == null)
        {
            Items = new ObservableCollection<Models.Tbl93Comment>();
            return;
        }

        var items = await _loadByParent(parentId);
        Items = new ObservableCollection<Models.Tbl93Comment>(items);
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return Enumerable.Empty<Models.Tbl93Comment>();

        return await Data.SearchCommentAsync(SearchText);
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override Task OnSelectedItemChangedAsync()
    {
        IsCommentDetailVisible = SelectedItem != null;
        return Task.CompletedTask;
    }

    protected override Task LoadChildrenAsync(int entityId)
        => Task.CompletedTask;

    protected override Task ClearChildCollectionsAsync()
        => Task.CompletedTask;

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
        var c = new Models.Tbl93Comment
        {
            Valid = true,
            Writer = Environment.UserName,
            WriterDate = DateTime.Now
        };

        _assignParentFk?.Invoke(c);
        return c;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
    {
        if (!item.IsValid())
            return null;

        item.Updater = Environment.UserName;
        item.UpdaterDate = DateTime.Now;

        return await Data.UpsertCommentAsync(item);
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
    {
        await Data.DeleteCommentAsync(item.CommentId);
        return true;
    }

    // ---------------------------------------------------------
    // CONFIRM
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
        => Dialog.ShowConfirmAsync("Speichern?", "Kommentar speichern?");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
        => Dialog.ShowConfirmAsync("Löschen?", "Kommentar löschen?");

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl93Comment item)
        => item.CommentId;
}


//public class Tbl93CommentViewModel : BaseBossListViewModel<Models.Tbl93Comment>
//{
//    private readonly LookupService _lookup;
//    private int _currentParentId;
//    public ICommand NewCommand { get; }
//    public ICommand SaveCommand { get; }
//    public ICommand DeleteCommand { get; }
//    public ICommand RefreshCommand { get; }
//    public ICommand CloseEditCommand { get; }
//    // ---------------------------------------------------------
//    // CONSTRUCTOR
//    // ---------------------------------------------------------
//    public Tbl93CommentViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookup,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        _lookup = lookup;

//        IsCommentDetailVisible = false;
//        IsEditVisible = false;

//        //// Sortierung
//        //SortHeader1 = "Id";
//        //SortHeader2 = "Writer";
//        //SortHeader3 = "Date";
//        //SortHeader4 = "Valid";

//     //   NewCommand = new AsyncRelayCommand(NewAsync);
//        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
//        SaveCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await SaveItemAsync(), CanSave);
//        DeleteCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await DeleteItemAsync(), CanDelete);
//        RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await RefreshAsync());
//        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);
//    }

//    // ---------------------------------------------------------
//    // VISIBILITY FLAGS
//    // ---------------------------------------------------------
//    private bool _isDetailVisible;
//    public bool IsDetailVisible
//    {
//        get => _isDetailVisible;
//        set => SetProperty(ref _isDetailVisible, value);
//    }

//    private bool _isEditVisible;
//    public bool IsEditVisible
//    {
//        get => _isEditVisible;
//        set => SetProperty(ref _isEditVisible, value);
//    }
//    private bool _isCommentDetailVisible;
//    public bool IsCommentDetailVisible
//    {
//        get => _isCommentDetailVisible;
//        set => SetProperty(ref _isCommentDetailVisible, value);
//    }

//    // ---------------------------------------------------------
//    // CREATE NEW
//    // ---------------------------------------------------------
//    //protected override async Task NewAsync()
//    //{
//    //    var item = CreateNewItem();
//    //    SelectedItem = item;

//    //    // UI binden lassen
//    //    OnPropertyChanged(nameof(SelectedItem));

//    //    // Validierung aktivieren
//    // //   item.ValidateCommentInfo();

//    //    // WICHTIG: UI Zeit geben, den roten Rahmen zu rendern
//    //    await Task.Delay(50);
//    //}
//    private void NewItem()
//    {
//        SelectedItem = new Models.Tbl93Comment
//        {
//            SubtribusId = _currentParentId
//        };

//        IsCommentDetailVisible = true;
//    }

//    protected override async Task NewAsync()
//    {
//        throw new NotImplementedException();
//    }

//    protected override Models.Tbl93Comment CreateNewItem()
//        => new Models.Tbl93Comment
//        {
//            SubtribusId = _currentParentId
//        };

//    // ---------------------------------------------------------
//    // LOAD ITEMS (Boss)
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
//    {
//        if (_currentParentId is null)
//            return Enumerable.Empty<Models.Tbl93Comment>();

//        return await Data.GetCommentsForSubtribusAsync(_currentParentId.Value);
//    }

//    // ---------------------------------------------------------
//    // LOAD PARENT 
//    // ---------------------------------------------------------
//    public override async Task LoadParentAsync(int parentId)
//    {
//        Debug.WriteLine("LOAD PARENT COMMENTS");

//        _currentParentId = parentId;

//        var items = await Data.GetCommentsForSubtribusAsync(parentId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);

//        SelectedItem = null;
//        IsCommentDetailVisible = false;
//    }
//    // ---------------------------------------------------------
//    // LOAD CHILDREN 
//    // ---------------------------------------------------------
//    protected override Task LoadChildrenAsync(int entityId)
//    {
//        // Comments haben KEINE Children → NICHTS LADEN
//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // CLEAR CHILDREN 
//    // ---------------------------------------------------------
//    protected override Task ClearChildCollectionsAsync()
//    {
//      //  IsCommentDetailVisible = false;
//        return Task.CompletedTask;
//    }
//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        Debug.WriteLine($"SelectedItem = {SelectedItem}, Visible = {IsCommentDetailVisible}");

//        IsCommentDetailVisible = SelectedItem != null;

//        if (SelectedItem == null)
//            return;

//        // falls du noch etwas lädst:
//        await Task.CompletedTask;
//    }

//    //protected override async Task OnSelectedItemChangedAsync()
//    //{
//    //    Debug.WriteLine($"SelectedItem = {SelectedItem}, Visible = {IsCommentDetailVisible}");

//    //    if (SelectedItem == null)
//    //    {
//    //        IsCommentDetailVisible = false;
//    //        return;
//    //    }

//    //    IsCommentDetailVisible = true;
//    //    await LoadChildrenAsync(SelectedItem.CommentId);
//    //}

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    //private async Task SaveItemAsync()
//    //{
//    //    if (SelectedItem == null)
//    //        return;

//    //    await Data.UpsertCommentAsync(SelectedItem);

//    //    await RefreshAsync();
//    //}
//    protected override bool CanSave()
//    {
//        // Kein SelectedItem → kein Save
//        if (SelectedItem == null)
//            return false;

//        // Validierung über dein Model
//        if (!SelectedItem.IsValid())
//            return false;

//        return true;
//    }


//    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
//    {
//        if (!item.IsValid())
//            return null;

//        return await Data.UpsertCommentAsync(item);
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    //private async Task DeleteItemAsync()
//    //{
//    //    if (SelectedItem == null)
//    //        return;

//    //    await Data.DeleteCommentAsync(SelectedItem.CommentId);

//    //    await RefreshAsync();
//    //}
//    protected override bool CanDelete()
//    {
//        // Kein SelectedItem → kein Delete
//        if (SelectedItem == null)
//            return false;

//        // Neue Items (Id = 0) können nicht gelöscht werden
//        if (SelectedItem.CommentId == 0)
//            return false;

//        return true;
//    }
//    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
//    {
//        try
//        {
//            await Data.DeleteCommentAsync(item.CommentId);
//            return true;
//        }
//        catch
//        {
//            return false;
//        }
//    }
//    private async Task RefreshAsync()
//    {
//        await LoadParentAsync(_currentParentId);
//    }
//    private void CloseEdit()
//    {
//        IsCommentDetailVisible = false;
//    }

//    // ---------------------------------------------------------
//    // CONFIRM SAVE / DELETE
//    // ---------------------------------------------------------
//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
//        => Dialog.ShowConfirmAsync("Speichern?", "Möchten Sie diesen Kommentar speichern?");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
//        => Dialog.ShowConfirmAsync("Löschen?", "Möchten Sie diesen Kommentar wirklich löschen?");

//    // ---------------------------------------------------------
//    // ID SELECTOR
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl93Comment model)
//        => model.CommentId;

//    // ---------------------------------------------------------
//    // SORTING
//    // ---------------------------------------------------------
//    //public override void Sort(int columnIndex)
//    //{
//    //    switch (columnIndex)
//    //    {
//    //        case 0: Items = new ObservableCollection<Models.Tbl93Comment>(Items.OrderBy(i => i.CommentId)); break;
//    //        case 1: Items = new ObservableCollection<Models.Tbl93Comment>(Items.OrderBy(i => i.Writer)); break;
//    //        case 2: Items = new ObservableCollection<Models.Tbl93Comment>(Items.OrderBy(i => i.WriterDate)); break;
//    //        case 3: Items = new ObservableCollection<Models.Tbl93Comment>(Items.OrderBy(i => i.Valid)); break;
//    //    }
//    //}
//}




