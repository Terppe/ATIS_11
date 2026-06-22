using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Base;

public abstract class BaseSortableBossViewModel<TModel>
    : BaseBossListViewModel<TModel>
    where TModel : class
{
    public IRelayCommand SortCommand { get; }

    private bool _ascending = true;
    private string _currentColumn = "";

    protected BaseSortableBossViewModel(
        IDataService data,
        IDialogService dialog,
        HierarchyCoordinator coordinator)
        : base(data, dialog, coordinator)
    {
        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
    }
    protected bool SortAscending => _ascending;
    protected string CurrentSortColumn => _currentColumn;

    protected virtual void OnSorted() { }

    private void SortBy(string column)
    {
        if (Items == null || Items.Count == 0)
            return;

        if (_currentColumn == column)
            _ascending = !_ascending;
        else
        {
            _currentColumn = column;
            _ascending = true;
        }

        var sorted = _ascending
            ? Items.OrderBy(x => GetSortKey(x, column))
            : Items.OrderByDescending(x => GetSortKey(x, column));

        Items = new ObservableCollection<TModel>(sorted);

        SelectedItem = null;

        OnSorted();   // ⭐ WICHTIG
    }

    protected abstract object GetSortKey(TModel item, string column);

}
