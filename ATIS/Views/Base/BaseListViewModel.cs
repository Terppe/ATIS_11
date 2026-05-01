using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ATIS.Interfaces;

namespace ATIS.Views.Base;

public abstract class BaseListViewModel<TModel> : INotifyPropertyChanged
{
    protected readonly IDataService _data;

    protected BaseListViewModel(IDataService data)
    {
        _data = data;

        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(New);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => SelectedItem != null);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItem != null);
        RefreshCommand = new AsyncRelayCommand(ReloadAsync);
    }

    // ---------------------------------------------------------
    // INotifyPropertyChanged
    // ---------------------------------------------------------
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------
    public IRelayCommand NewCommand { get; protected set; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    // ---------------------------------------------------------
    // Filter
    // ---------------------------------------------------------
    public string? CurrentFilter { get; protected set; }
    public bool SuppressFilter { get; set; }

    public virtual Task ApplyFilterAsync(string? filter)
    {
        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        if (SuppressFilter)
            return Task.CompletedTask;

        return ReloadAndSelectAsync(saved: default, autoSelect: false);
    }

    // ---------------------------------------------------------
    // Items
    // ---------------------------------------------------------
    private ObservableCollection<TModel> _items = new();
    public ObservableCollection<TModel> Items
    {
        get => _items;
        protected set => SetProperty(ref _items, value);
    }

    // ---------------------------------------------------------
    // Selection
    // ---------------------------------------------------------
    private bool _suppressSelectedItemChanged;

    private TModel? _selectedItem;
    public TModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value))
                return;

            SaveCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();

            if (value != null && !_suppressSelectedItemChanged)
                _ = SafeOnSelectedItemChangedAsync();
        }
    }

    private async Task SafeOnSelectedItemChangedAsync()
    {
        try
        {
            await OnSelectedItemChangedAsync();
        }
        catch
        {
            // optional: loggen, aber niemals unhandled werden lassen
        }
    }

    protected IDisposable SuppressSelectedItemChanged()
    {
        _suppressSelectedItemChanged = true;
        return new ActionOnDispose(() => _suppressSelectedItemChanged = false);
    }

    public void SelectItemSilently(TModel? item)
    {
        using (SuppressSelectedItemChanged())
            SelectedItem = item;
    }

    private sealed class ActionOnDispose : IDisposable
    {
        private readonly Action _action;
        public ActionOnDispose(Action action) => _action = action;
        public void Dispose() => _action();
    }

    // ---------------------------------------------------------
    // State
    // ---------------------------------------------------------
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        protected set => SetProperty(ref _isLoading, value);
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        protected set => SetProperty(ref _dataSetCount, value);
    }

    // ---------------------------------------------------------
    // Abstract contract
    // ---------------------------------------------------------
    protected abstract Task<IEnumerable<TModel>> LoadItemsAsync();
    protected abstract TModel CreateNewItem();

    /// <summary>Return null => not saved (blocked/duplicate/validation/cancel).</summary>
    protected abstract Task<TModel?> SaveItemAsync(TModel item);

    /// <summary>Return false => not deleted (blocked/cancel/unsaved-discard).</summary>
    protected abstract Task<bool> DeleteItemAsync(TModel item);

    public virtual Task OnSelectedItemChangedAsync() => Task.CompletedTask;

    // ---------------------------------------------------------
    // Hooks
    // ---------------------------------------------------------
    protected virtual Task<bool> ConfirmSaveAsync(TModel item) => Task.FromResult(true);
    protected virtual Task<bool> ConfirmDeleteAsync(TModel item) => Task.FromResult(true);
    protected virtual Task<bool> ValidateBeforeSaveAsync(TModel item) => Task.FromResult(true);

    protected virtual void OnAfterNew(TModel item) { }
    protected virtual bool IsUnsavedNewItem(TModel item) => false;
    protected virtual string? GetFilterForSavedItem(TModel saved) => null;

    // ---------------------------------------------------------
    // CRUD
    // ---------------------------------------------------------
    public virtual void New()
    {
        var item = CreateNewItem();
        SelectedItem = item;
        OnAfterNew(item);
    }
    protected virtual Task OnBeforeReloadAfterSaveAsync(TModel item)
        => Task.CompletedTask;

    public virtual async Task SaveAsync()
    {
        if (SelectedItem == null)
            return;

        if (!await ConfirmSaveAsync(SelectedItem))
            return;

        if (!await ValidateBeforeSaveAsync(SelectedItem))
            return;

        var saved = await SaveItemAsync(SelectedItem);
        if (saved == null)
            return;

        // Erweiterungspunkt einfügen
        await OnBeforeReloadAfterSaveAsync(saved);

        var newFilter = GetFilterForSavedItem(saved);
        if (!string.IsNullOrWhiteSpace(newFilter) && !Equals(CurrentFilter, newFilter))
        {
            // optional: Filter setzen ohne doppelten Reload
            SuppressFilter = true;
            CurrentFilter = newFilter;
            OnPropertyChanged(nameof(CurrentFilter));
            SuppressFilter = false;
        }

        await ReloadAndSelectAsync(saved, autoSelect: true);
    }

    public virtual async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        var toDelete = SelectedItem;

        if (!await ConfirmDeleteAsync(toDelete))
            return;

        var deleted = await DeleteItemAsync(toDelete);
        if (!deleted)
            return;

        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    // ---------------------------------------------------------
    // Reload
    // ---------------------------------------------------------
    public virtual async Task ReloadAsync()
    {
        if (SelectedItem is not null && IsUnsavedNewItem(SelectedItem))
            SelectedItem = default;

        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    public Task ReloadAndSelectAsync(TModel? saved)
        => ReloadAndSelectAsync(saved, autoSelect: true);

    public virtual async Task ReloadAndSelectAsync(TModel? saved, bool autoSelect)
    {
        IsLoading = true;
        try
        {
            var old = SelectedItem;

            var list = (await LoadItemsAsync()).ToList();
            Items = new ObservableCollection<TModel>(list);
            DataSetCount = Items.Count;

            if (!autoSelect)
            {
                SelectItemSilently(default);
                return;
            }

            SelectItemSilently(default);

            if (saved != null)
            {
                var savedId = GetId(saved);
                SelectedItem = Items.FirstOrDefault(x => GetId(x) == savedId);
            }
            else if (old != null)
            {
                var oldId = GetId(old);

                if (oldId > 0)
                    SelectedItem = Items.FirstOrDefault(x => GetId(x) == oldId);

                if (SelectedItem == null)
                    SelectedItem = Items.FirstOrDefault();
            }
            else
            {
                SelectedItem = Items.FirstOrDefault();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private int GetId(TModel? item)
    {
        if (item == null)
            return 0;

        var type = typeof(TModel);

        var idProp =
            type.GetProperty("Id") ??
            type.GetProperties().FirstOrDefault(p =>
                p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) &&
                p.PropertyType == typeof(int));

        if (idProp == null)
            return 0;

        return (int)(idProp.GetValue(item) ?? 0);
    }
}
