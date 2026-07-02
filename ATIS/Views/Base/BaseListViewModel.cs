using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;

namespace ATIS.Views.Base;

public abstract class BaseListViewModel<T> : ObservableObject
    where T : class
{
    protected readonly IDataService Data;
    protected readonly IDialogService Dialog;
    public readonly LookupService LookupService;
    public event EventHandler? ScrollRequested;

    protected BaseListViewModel(
        IDataService data,
        IDialogService dialog)
    {
        Data = data;
        Dialog = dialog;
    }

    // ---------------------------------------------------------
    // LOADING
    // ---------------------------------------------------------
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public virtual Task ReloadAsync() => Task.CompletedTask;

    // ---------------------------------------------------------
    // STATE PROPERTIES
    // ---------------------------------------------------------
    public bool IsEditMode => SelectedItem != null;
    public bool HasSelection => SelectedItem != null;
    public bool IsDetailVisible => SelectedItem != null;
    public bool HasItems => Items != null && Items.Count > 0;

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    private T? _selectedItem;
    public T? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                _ = OnSelectedItemChangedAsync();

                if (this is BaseBossListViewModel<T> boss)
                {
                    _ = boss.OnSelectedItemChangedBossAsync();
                    boss.OnScrollRequested();   // ✔ korrekt
                }

                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(IsDetailVisible));
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    // ---------------------------------------------------------
    // ITEMS + COUNT
    // ---------------------------------------------------------
    private ObservableCollection<T>? _items;
    public ObservableCollection<T>? Items
    {
        get => _items;
        set
        {
            if (SetProperty(ref _items, value))
            {
                DataSetCount = _items?.Count ?? 0;
                OnPropertyChanged(nameof(HasItems));
            }
        }
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        set => SetProperty(ref _dataSetCount, value);
    }

    // ---------------------------------------------------------
    // HOOKS
    // ---------------------------------------------------------
    protected virtual Task<IEnumerable<T>> LoadItemsAsync()
        => Task.FromResult<IEnumerable<T>>(Array.Empty<T>());

    protected virtual Task OnSelectedItemChangedAsync()
        => Task.CompletedTask;

    protected virtual bool FilterBySearchText(T item) => true;

    protected abstract Task NewAsync();
}

//public abstract class BaseListViewModel<T> : ObservableObject
//    where T : class
//{
//    protected readonly IDataService Data;
//    protected readonly IDialogService Dialog;
//    public readonly LookupService _lookupService;

//    protected BaseListViewModel(
//        IDataService data,
//        IDialogService dialog)
//    {
//        Data = data;
//        Dialog = dialog;
//    }

//    // ---------------------------------------------------------
//    // LOADING
//    // ---------------------------------------------------------

//    private bool _isLoading;
//    public bool IsLoading
//    {
//        get => _isLoading;
//        set => SetProperty(ref _isLoading, value);
//    }
//    public virtual Task ReloadAsync()
//    {
//        return Task.CompletedTask;
//    }
//    public bool IsEditMode => SelectedItem != null;
//    public bool HasSelection => SelectedItem != null;

//    // ---------------------------------------------------------
//    // DETAIL VISIBILITY
//    // ---------------------------------------------------------
//    public bool IsDetailVisible => SelectedItem != null;

//    // ---------------------------------------------------------
//    // SELECTED ITEM
//    // ---------------------------------------------------------

//    private T? _selectedItem;
//    public T? SelectedItem
//    {
//        get => _selectedItem;
//        set
//        {
//            if (SetProperty(ref _selectedItem, value))
//            {
//                _ = OnSelectedItemChangedAsync();

//                if (this is BaseBossListViewModel<T> boss)
//                    _ = boss.OnSelectedItemChangedBossAsync();

//                OnPropertyChanged(nameof(IsEditMode));
//                OnPropertyChanged(nameof(IsDetailVisible));
//            }
//        }
//    }


//    // ---------------------------------------------------------
//    // ITEMS + COUNT
//    // ---------------------------------------------------------
//    private ObservableCollection<T>? _items;
//    public ObservableCollection<T>? Items
//    {
//        get => _items;
//        set
//        {
//            if (SetProperty(ref _items, value))
//            {
//                DataSetCount = _items?.Count ?? 0;
//                OnPropertyChanged(nameof(HasItems));
//            }
//        }
//    }

//    private int _dataSetCount;
//    public int DataSetCount
//    {
//        get => _dataSetCount;
//        set => SetProperty(ref _dataSetCount, value);
//    }


//    // ---------------------------------------------------------
//    // HOOKS
//    // ---------------------------------------------------------
//    protected virtual Task<IEnumerable<T>> LoadItemsAsync()
//        => Task.FromResult<IEnumerable<T>>(Array.Empty<T>());

//    protected virtual Task OnSelectedItemChangedAsync()
//        => Task.CompletedTask;

//    // optionaler Filter-Hook
//    protected virtual bool FilterBySearchText(T item) => true;

//    protected abstract Task NewAsync();

//}
