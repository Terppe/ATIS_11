using System.Collections.ObjectModel;
using ATIS.Interfaces;

namespace ATIS.Views.Base;

public abstract class BaseChildListViewModel<T> : BaseBossListViewModel<T>
    where T : class
{
    private readonly Func<int, Task<IEnumerable<T>>> _loadForParentAsync;

    protected BaseChildListViewModel(
        IDataService data,
        IDialogService dialog,
        Func<int, Task<IEnumerable<T>>> loadForParentAsync)
        : base(data, dialog, coordinator: null!)
    {
        _loadForParentAsync = loadForParentAsync;
    }

    public int? CurrentParentId { get; private set; }

    public async Task LoadForParentAsync(int parentId)
    {
        CurrentParentId = parentId;

        var items = await _loadForParentAsync(parentId);

        if (Items == null)
            Items = new ObservableCollection<T>(items);
        else
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        }

        SelectedItem = Items.FirstOrDefault();
    }

    // ---------------------------------------------------------
    // Child-Listen sind read-only
    // ---------------------------------------------------------
    protected override T CreateNewItem()
        => throw new NotSupportedException("Child lists cannot create items.");

    protected override Task<IEnumerable<T>> LoadItemsAsync()
    {
        if (CurrentParentId is null)
            return Task.FromResult<IEnumerable<T>>(Array.Empty<T>());

        return _loadForParentAsync(CurrentParentId.Value);
    }

    protected override Task<T?> SaveItemAsync(T item)
        => Task.FromResult<T?>(default);

    protected override Task<bool> DeleteItemAsync(T item)
        => Task.FromResult(false);

    protected override Task<bool> ConfirmSaveAsync(T item)
        => Task.FromResult(false);

    protected override Task<bool> ConfirmDeleteAsync(T item)
        => Task.FromResult(false);

    public override Task LoadParentAsync(int parentId)
        => Task.CompletedTask;

    protected override Task LoadChildrenAsync(int entityId)
        => Task.CompletedTask;

    protected override Task ClearChildCollectionsAsync()
        => Task.CompletedTask;

    protected override int GetId(T model)
        => 0; // Child-Listen brauchen keine ID-Pipeline
}
