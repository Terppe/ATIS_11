using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Base;

public abstract class BaseBossListViewModel<TModel> : BaseListViewModel<TModel>
    where TModel : class
{
    protected readonly HierarchyCoordinator Coordinator;
    protected readonly IDataService Data;
    protected readonly IDialogService Dialog;

    protected BaseBossListViewModel(
        IDataService data,
        IDialogService dialog,
        HierarchyCoordinator coordinator)
        : base(data, dialog)
    {
        Data = data;
        Dialog = dialog;
        Coordinator = coordinator;

        // EINHEITLICHE COMMANDS
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(EditItem, () => SelectedItem != null);
        SaveCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await SaveAsync(), () => SelectedItem != null);
        DeleteCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await DeleteAsync(), () => SelectedItem != null);
        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CancelEdit);
        RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await ReloadAsync());
    }

    // ---------------------------------------------------------
    // Commands (NICHT in Child-VM überschreiben!)
    // ---------------------------------------------------------
    public IRelayCommand NewCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand DeleteCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand RefreshCommand { get; }

    protected virtual bool CanSave() => true;
    protected virtual bool CanDelete() => true;

    // ---------------------------------------------------------
    // CRUD Hooks (MÜSSEN in Child-VM überschrieben werden)
    // ---------------------------------------------------------
    protected abstract override Task NewAsync();
    protected abstract TModel CreateNewItem();
    protected abstract Task<IEnumerable<TModel>> LoadItemsAsync();
    protected abstract Task<TModel?> SaveItemAsync(TModel item);
    protected abstract Task<bool> DeleteItemAsync(TModel item);
    protected abstract Task<bool> ConfirmSaveAsync(TModel item);
    protected abstract Task<bool> ConfirmDeleteAsync(TModel item);

    protected virtual Task<bool> ValidateBeforeSaveAsync(TModel item)
        => Task.FromResult(true);

    // ---------------------------------------------------------
    // Parent / Children
    // ---------------------------------------------------------
    public virtual Task LoadParentAsync(int parentId) => Task.CompletedTask;
    protected abstract Task LoadChildrenAsync(int entityId);
    protected virtual Task ClearChildCollectionsAsync() => Task.CompletedTask;

    // ---------------------------------------------------------
    // CRUD Pipeline (NICHT überschreiben!)
    // ---------------------------------------------------------
    private void NewItem()
    {
        SelectedItem = CreateNewItem();
    }

    private void EditItem() { }

    private async Task SaveAsync()
    {
        if (SelectedItem == null)
            return;

        if (!await ConfirmSaveAsync(SelectedItem))
            return;

        if (!await ValidateBeforeSaveAsync(SelectedItem))
            return;

        var saved = await SaveItemAsync(SelectedItem);
        if (saved != null)
        {
            await OnBeforeReloadAfterSaveAsync(saved);

            var id = GetId(saved);
            await ReloadAndSelectAsync(id, GetId);
            await LoadChildrenAsync(id);
        }
    }

    protected virtual Task OnBeforeReloadAfterSaveAsync(TModel item)
        => Task.CompletedTask;

    private async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        if (!await ConfirmDeleteAsync(SelectedItem))
            return;

        if (await DeleteItemAsync(SelectedItem))
        {
            await ReloadAsync();
            await ClearChildCollectionsAsync();

            SelectedItem = Items?.FirstOrDefault();
        }
    }

    private void CancelEdit()
    {
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // SEARCH
    // ---------------------------------------------------------
    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    // ---------------------------------------------------------
    // RELOAD
    // ---------------------------------------------------------
    public async Task ReloadAsync()
    {
        IsLoading = true;
        try
        {
            var oldId = SelectedItem != null ? GetId(SelectedItem) : 0;

            var items = await LoadItemsAsync();

            if (!string.IsNullOrWhiteSpace(SearchText))
                items = items.Where(FilterBySearchText);

            Items = new ObservableCollection<TModel>(items);

            if (Items.Any())
            {
                SelectedItem = Items.FirstOrDefault(x => GetId(x) == oldId)
                             ?? Items.First();
            }
            else
            {
                SelectedItem = null;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ---------------------------------------------------------
    // Reload + Select
    // ---------------------------------------------------------
    public async Task ReloadAndSelectAsync(int id, Func<TModel, int> idSelector)
    {
        var list = (await LoadItemsAsync()).ToList();
        Items = new ObservableCollection<TModel>(list);

        SelectedItem = default;

        if (id > 0)
            SelectedItem = Items.FirstOrDefault(x => idSelector(x) == id);
        else
            SelectedItem = Items.FirstOrDefault();

        OnPropertyChanged(nameof(SelectedItem));
    }

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected abstract int GetId(TModel item);
}

//public abstract class BaseBossListViewModel<TModel> : BaseListViewModel<TModel>
//    where TModel : class
//{
//    protected readonly HierarchyCoordinator Coordinator;
//    protected readonly IDataService Data;
//    protected readonly IDialogService Dialog;

//    protected BaseBossListViewModel(
//        IDataService data,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator)
//        : base(data, dialog)
//    {
//        Data = data;
//        Dialog = dialog;
//        Coordinator = coordinator;

//        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
//        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(EditItem, () => SelectedItem != null);
//        SaveCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await SaveAsync(), () => SelectedItem != null);
//        DeleteCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await DeleteAsync(), () => SelectedItem != null);
//        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CancelEdit);
//        RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await ReloadAsync());
//    }

//    // ---------------------------------------------------------
//    // Commands
//    // ---------------------------------------------------------
//    public IRelayCommand NewCommand { get; set; }
//    public IRelayCommand EditCommand { get; }
//    public IRelayCommand SaveCommand { get; }
//    public IRelayCommand DeleteCommand { get; }
//    public IRelayCommand CancelCommand { get; }
//    public IRelayCommand RefreshCommand { get; }
//    protected virtual bool CanSave() => true;
//    protected virtual bool CanDelete() => true;

//    // ---------------------------------------------------------
//    // CRUD Hooks
//    // ---------------------------------------------------------
//    protected abstract override Task NewAsync();

//    protected abstract TModel CreateNewItem();
//    protected abstract Task<IEnumerable<TModel>> LoadItemsAsync();
//    protected abstract Task<TModel?> SaveItemAsync(TModel item);
//    protected abstract Task<bool> DeleteItemAsync(TModel item);
//    protected abstract Task<bool> ConfirmSaveAsync(TModel item);
//    protected abstract Task<bool> ConfirmDeleteAsync(TModel item);

//    // optionaler Validierungs‑Hook vor Save
//    protected virtual Task<bool> ValidateBeforeSaveAsync(TModel item)
//        => Task.FromResult(true);

//    // ---------------------------------------------------------
//    // Parent / Children
//    // ---------------------------------------------------------
//    public virtual Task LoadParentAsync(int parentId) => Task.CompletedTask;
//    protected abstract Task LoadChildrenAsync(int entityId);
//    protected virtual Task ClearChildCollectionsAsync() => Task.CompletedTask;

//    // ---------------------------------------------------------
//    // CRUD Pipeline
//    // ---------------------------------------------------------
//    private void NewItem()
//    {
//        SelectedItem = CreateNewItem();
//    }

//    private void EditItem() { }

//    private async Task SaveAsync()
//    {
//        if (SelectedItem == null)
//            return;

//        if (!await ConfirmSaveAsync(SelectedItem))
//            return;

//        if (!await ValidateBeforeSaveAsync(SelectedItem))
//            return;

//        var saved = await SaveItemAsync(SelectedItem);
//        if (saved != null)
//        {
//            // NEU: Hook aufrufen
//            await OnBeforeReloadAfterSaveAsync(saved);

//            var id = GetId(saved);
//            await ReloadAndSelectAsync(id, GetId);
//            await LoadChildrenAsync(id);
//        }
//    }
//    protected virtual Task OnBeforeReloadAfterSaveAsync(TModel item)
//    {
//        return Task.CompletedTask;
//    }

//    private async Task DeleteAsync()
//    {
//        if (SelectedItem == null)
//            return;

//        if (!await ConfirmDeleteAsync(SelectedItem))
//            return;

//        if (await DeleteItemAsync(SelectedItem))
//        {
//            await ReloadAsync();               // Liste neu laden
//            await ClearChildCollectionsAsync(); // Nur Child-Listen löschen

//            if (Items != null && Items.Any())  // Auf ersten Datensatz springen
//                SelectedItem = Items.First();
//            else
//                SelectedItem = null;
//        }
//    }

//    private void CancelEdit()
//    {
//        SelectedItem = null;
//    }
//    // ---------------------------------------------------------
//    // SEARCH
//    // ---------------------------------------------------------
//    private string? _searchText;
//    public string? SearchText
//    {
//        get => _searchText;
//        set
//        {
//            if (SetProperty(ref _searchText, value))
//                _ = ReloadAsync(); // optional: Live-Filter
//        }
//    }

//    // ---------------------------------------------------------
//    // RELOAD
//    // ---------------------------------------------------------
//    public async Task ReloadAsync()
//    {
//        IsLoading = true;
//        try
//        {
//            // 1) Aktuelle ID merken
//            var oldId = GetId(SelectedItem);

//            // 2) Items laden
//            var items = await LoadItemsAsync();

//            // 3) Filter anwenden
//            if (!string.IsNullOrWhiteSpace(SearchText))
//                items = items.Where(FilterBySearchText);

//            // 4) Items setzen
//            Items = new ObservableCollection<TModel>(items);

//            // 5) Wiederherstellen des alten Datensatzes
//            if (Items.Any())
//            {
//                var restored = Items.FirstOrDefault(x => GetId(x) == oldId);

//                SelectedItem = restored ?? Items.First();
//            }
//            else
//            {
//                SelectedItem = null;
//            }
//        }
//        finally
//        {
//            IsLoading = false;
//        }
//    }

//    // ---------------------------------------------------------
//    // Reload + Select
//    // ---------------------------------------------------------
//    public async Task ReloadAndSelectAsync(int id, Func<TModel, int> idSelector)
//    {
//        var list = (await LoadItemsAsync()).ToList();
//        Items = new ObservableCollection<TModel>(list);

//        // WICHTIG: Reset erzwingt UI-Refresh
//        SelectedItem = default;

//        if (id > 0)
//        {
//            SelectedItem = Items.FirstOrDefault(x => idSelector(x) == id);
//        }
//        else
//        {
//            SelectedItem = Items.FirstOrDefault();
//        }

//        // WICHTIG: Bindings refreshen
//        OnPropertyChanged(nameof(SelectedItem));
//    }

//    protected abstract int GetId(TModel item);
//    //protected int GetId(TModel model)
//    //{
//    //    if (model == null)
//    //        return 0;

//    //    var type = typeof(TModel);

//    //    var idProp =
//    //        type.GetProperty("Id") ??
//    //        type.GetProperties().FirstOrDefault(p =>
//    //            p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) &&
//    //            p.PropertyType == typeof(int));

//    //    if (idProp == null)
//    //        return 0;

//    //    return (int)(idProp.GetValue(model) ?? 0);
//    //}

//}
