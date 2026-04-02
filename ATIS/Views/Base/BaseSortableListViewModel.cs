using System.Collections.ObjectModel;

namespace ATIS.Views.Base;

public abstract class BaseSortableListViewModel<TDomain> : BaseListViewModel<TDomain>
    where TDomain : class
{
    public event Func<Task>? SelectedItemChangedAsync;

    private bool _suspendSelection;

    protected BaseSortableListViewModel()
    {
  //      Items = new ObservableCollection<TDomain>();

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
    }

    // ---------------------------------------------------------
    #region ITEMS + SELECTION
    // ---------------------------------------------------------
    private ObservableCollection<TDomain> _items;
    public ObservableCollection<TDomain> Items
    {
        get => _items;
        //protected set => SetProperty(ref _items, value);
    }

    private TDomain? _selectedItem;
    public TDomain? SelectedItem
    {
        get => _selectedItem;
        set
        {
            //if (SetProperty(ref _selectedItem, value))
            //{
            //    if (!_suspendSelection)
            //        _ = OnSelectedItemChangedInternalAsync();
            //}
        }
    }

    private async Task OnSelectedItemChangedInternalAsync()
    {
        if (SelectedItemChangedAsync != null)
            await SelectedItemChangedAsync.Invoke();
    }

    public void SelectItem(TDomain? item)
    {
        SelectedItem = item;
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        //set => SetProperty(ref _dataSetCount, value);
    }

    #endregion ITEMS + SELECTION

    // ---------------------------------------------------------
    #region SORT STATE
    // ---------------------------------------------------------
    private bool _sortAscending = true;
    private string _currentSortColumn = "";

    #endregion SORT STATE

    // ---------------------------------------------------------
    #region SORT COMMAND
    // ---------------------------------------------------------
    public IRelayCommand<string> SortCommand { get; }

    #endregion SORT COMMAND

    // ---------------------------------------------------------
    #region SORT LOGIC
    // ---------------------------------------------------------
    public void SortBy(string? column)
    {
        if (Items == null || Items.Count == 0 || column == null)
            return;

        var old = SelectedItem;

        if (_currentSortColumn == column)
            _sortAscending = !_sortAscending;
        else
        {
            _currentSortColumn = column;
            _sortAscending = true;
        }

        var sorted = SortItems(column, _sortAscending);

        _suspendSelection = true;

      //  Items = new ObservableCollection<TDomain>(sorted);
        Items.Clear();
        foreach (var item in sorted)
            Items.Add(item);


        if (old != null)
            SelectedItem = Items.FirstOrDefault(x => Equals(x, old));

        _suspendSelection = false;

        _ = OnSelectedItemChangedInternalAsync();
    }

    #endregion SORT LOGIC

    // ---------------------------------------------------------
    #region ABSTRACT SORTING HOOK
    // ---------------------------------------------------------
    protected abstract IEnumerable<TDomain> SortItems(string column, bool ascending);

    #endregion ABSTRACT SORTING HOOK

    // ---------------------------------------------------------
    #region DEFAULT LOAD
    // ---------------------------------------------------------
    public  async Task LoadAsync()
    {
        _suspendSelection = true;

        Items.Clear();
        SelectedItem = null;
    //    DataSetCount = 0;

        _suspendSelection = false;

        await Task.CompletedTask;
    }

    #endregion DEFAULT LOAD
}
