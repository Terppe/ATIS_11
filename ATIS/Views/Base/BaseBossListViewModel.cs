using System.Collections.ObjectModel;
using System.Windows.Input;
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
        //NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem, () => !IsInParentMode);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(EditItem, () => SelectedItem != null);
        SaveCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await SaveAsync(), () => SelectedItem != null);
        DeleteCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await DeleteAsync(), () => SelectedItem != null);
        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CancelEdit);
        //RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await ReloadAsync());
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() =>  SelectedItem = null );
        RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await ReloadAsync(), () => !IsInParentMode && IsRefreshEnabled);

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
    public IRelayCommand CloseEditCommand { get; }

    protected virtual bool CanSave() => true;
    protected virtual bool CanDelete() => true;

    // ---------------------------------------------------------
    // CRUD Hooks (MÜSSEN in Child-VM überschrieben werden)
    // ---------------------------------------------------------
    protected abstract override Task NewAsync();
    protected abstract TModel CreateNewItem();
    protected abstract override Task<IEnumerable<TModel>> LoadItemsAsync();
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
    public virtual Task OnSelectedItemChangedBossAsync()
    {
        SaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        EditCommand.NotifyCanExecuteChanged();

        return Task.CompletedTask;
    }

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
        UpdateRefreshState();
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
        UpdateRefreshState();
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
    // UI STATE
    // ---------------------------------------------------------

    private bool _isRefreshEnabled;
    public bool IsRefreshEnabled
    {
        get => _isRefreshEnabled;
        set => SetProperty(ref _isRefreshEnabled, value);
    }

    private bool _isInParentMode;
    public bool IsInParentMode
    {
        get => _isInParentMode;
        set
        {
            if (SetProperty(ref _isInParentMode, value))
            {
                // Buttons aktualisieren
                NewCommand.NotifyCanExecuteChanged();
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // REFRESH STATE
    // ---------------------------------------------------------

    protected void UpdateRefreshState()
    {
        IsRefreshEnabled = Items?.Any() == true;
        RefreshCommand.NotifyCanExecuteChanged();
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

            // ⭐ Nur im Boss-Modus filtern
            if (!IsInParentMode && !string.IsNullOrWhiteSpace(SearchText))
                items = items.Where(FilterBySearchText);

            Items = new ObservableCollection<TModel>(items);

            UpdateRefreshState();

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
        UpdateRefreshState(); // ⭐ hier ergänzen
    }

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected abstract int GetId(TModel item);
}
