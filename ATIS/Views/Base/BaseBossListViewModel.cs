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
    public event EventHandler? ScrollRequested;

    // ---------------------------------------------------------
    // MODUS: Boss / Parent
    // ---------------------------------------------------------

    public bool IsBossMode { get; private set; } = true;
    public bool IsParentMode => !IsBossMode;

    public void SetBossMode() => IsBossMode = true;
    public void SetParentMode() => IsBossMode = false;

    // ---------------------------------------------------------
    // CTOR
    // ---------------------------------------------------------

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
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem, () => IsBossMode);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(EditItem, () => SelectedItem != null);
        SaveCommand =
            new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await SaveAsync(), () => SelectedItem != null);
        DeleteCommand =
            new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await DeleteAsync(), () => SelectedItem != null);
        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CancelEdit);
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => SelectedItem = null);

        RefreshCommand =
            new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await ReloadAsync(),
                () => IsBossMode && IsRefreshEnabled);
    }

    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------

    public IRelayCommand NewCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand DeleteCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand RefreshCommand { get; }
    public IRelayCommand CloseEditCommand { get; }
    public IRelayCommand BackCommand { get; }

    // ---------------------------------------------------------
    // CRUD Hooks
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

    public bool HasParentSelection => SelectedItem != null;

    // ---------------------------------------------------------
    // SelectedItem Changed (Boss + Parent)
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
        if (IsBossMode)
            SelectedItem = CreateNewItem();
    }

    private void EditItem() { }
    private void CancelEdit()
    {
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // SCROLL REQUESTED EVENT
    // ---------------------------------------------------------

    public virtual void OnScrollRequested()
    {
        ScrollRequested?.Invoke(this, EventArgs.Empty);
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected virtual Task OnBeforeSaveAsync(TModel item)
    {
        return Task.CompletedTask;
    }
    private async Task SaveAsync()
    {
        if (SelectedItem == null)
            return;

        if (!await ConfirmSaveAsync(SelectedItem))
            return;

        if (!await ValidateBeforeSaveAsync(SelectedItem))
            return;

        await RunWithLoadingAsync(async () =>
        {
            await OnBeforeSaveAsync(SelectedItem);

            var saved = await SaveItemAsync(SelectedItem);
            if (saved != null)
            {
                await OnBeforeReloadAfterSaveAsync(saved);

                var id = GetId(saved);

                await ReloadCoreAsync();

                SelectedItem = Items?.FirstOrDefault(i => GetId(i) == id);
            }
        });
    }
    protected virtual Task OnBeforeReloadAfterSaveAsync(TModel item)
        => Task.CompletedTask;

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    private async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        if (!await ConfirmDeleteAsync(SelectedItem))
            return;

        await RunWithLoadingAsync(async () =>
        {
            if (await DeleteItemAsync(SelectedItem))
            {
                await ReloadCoreAsync();

                SelectedItem = null;

                if (this is BaseBossListViewModel<TModel> boss)
                    boss.OnScrollRequested();
            }
        });
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
                NewCommand.NotifyCanExecuteChanged();
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }
    }


    // ---------------------------------------------------------
    // RELOAD (Boss + Parent)
    // ---------------------------------------------------------

    public override async Task ReloadAsync()
    {
        await RunWithLoadingAsync(ReloadCoreAsync);
    }

    protected virtual async Task ReloadCoreAsync()
    {
        var oldId = SelectedItem != null ? GetId(SelectedItem) : 0;

        var items = await LoadItemsAsync();

        if (IsBossMode && !string.IsNullOrWhiteSpace(SearchText))
            items = items.Where(FilterBySearchText);

        Items = new ObservableCollection<TModel>(items);

        UpdateRefreshState();

        if (IsBossMode)
        {
            if (Items.Any())
            {
                SelectedItem = Items.FirstOrDefault(x => GetId(x) == oldId)
                               ?? (string.IsNullOrWhiteSpace(SearchText)
                                   ? Items.FirstOrDefault()
                                   : null);
            }
            else
            {
                SelectedItem = null;
            }
        }
    }

    protected abstract int GetId(TModel item);

    // ---------------------------------------------------------
    // Sorting + Header (kompakt)
    // ---------------------------------------------------------
    private string _currentSortColumn = "";
    private bool _sortAscending = true;

    private readonly Dictionary<string, string> _headerDefaults = new();
    private readonly Dictionary<string, string> _headerTexts = new();

    protected void RegisterHeader(string key, string header)
    {
        _headerDefaults[key] = header;
        _headerTexts[key] = header;
        OnPropertyChanged($"{key}HeaderText");
    }

    protected void UpdateHeader(string key, string header)
    {
        var value = string.IsNullOrWhiteSpace(header) ? key : header;
        _headerDefaults[key] = value; // lokalisierter Basistext
        _headerTexts[key] = value; // aktueller sichtbarer Text
        OnPropertyChanged($"{key}HeaderText");
    }

    protected abstract object? GetSortKey(TModel item, string column);

    protected void SortBy(string column)
    {
        if (Items is null || Items.Count == 0) return;

        _sortAscending = _currentSortColumn == column ? !_sortAscending : true;
        _currentSortColumn = column;

        var sorted = _sortAscending
            ? Items.OrderBy(x => GetSortKey(x, column))
            : Items.OrderByDescending(x => GetSortKey(x, column));

        Items = new ObservableCollection<TModel>(sorted);
        RefreshHeaderTextsWithSortArrow();
    }

    private void RefreshHeaderTextsWithSortArrow()
    {
        foreach (var key in _headerDefaults.Keys)
        {
            var text = _headerDefaults[key];
            if (key == _currentSortColumn)
                text += _sortAscending ? " ▲" : " ▼";

            _headerTexts[key] = text;
            OnPropertyChanged($"{key}HeaderText");
        }
    }

    protected string GetHeaderText(string key)
        => _headerTexts.TryGetValue(key, out var v) ? v : key;

    // Konkrete Header-Properties:
    public string IdHeaderText => GetHeaderText("Id");
    public string NameHeaderText => GetHeaderText("Name");



}
