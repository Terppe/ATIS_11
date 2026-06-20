using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;

namespace ATIS.Views.Base;

public abstract class BaseListViewModel<T> : ObservableObject
    where T : class
{
    protected readonly IDataService Data;
    protected readonly IDialogService Dialog;
    public readonly LookupService _lookupService;

    protected BaseListViewModel(
        IDataService data,
        IDialogService dialog)
    {
        Data = data;
        Dialog = dialog;

      //  RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await ReloadAsync());
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
                DataSetCount = _items?.Count ?? 0;
        }
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        set => SetProperty(ref _dataSetCount, value);
    }

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

                // WICHTIG: Boss-VM informieren
                if (this is BaseBossListViewModel<T> boss)
                    _ = boss.OnSelectedItemChangedBossAsync();
            }
        }
    }

    // ---------------------------------------------------------
    // HOOKS
    // ---------------------------------------------------------
    protected virtual Task<IEnumerable<T>> LoadItemsAsync()
        => Task.FromResult<IEnumerable<T>>(Array.Empty<T>());

    protected virtual Task OnSelectedItemChangedAsync()
        => Task.CompletedTask;

    // optionaler Filter-Hook
    protected virtual bool FilterBySearchText(T item) => true;

    protected abstract Task NewAsync();

}
