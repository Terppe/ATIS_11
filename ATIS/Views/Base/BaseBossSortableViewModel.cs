
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Base;

public abstract class BaseBossSortableViewModel<TDomain>
    : BaseSortableListViewModel<TDomain>
    where TDomain : class
{
    public IRelayCommand NewCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    protected string _currentFilter = "";
    private bool _suspendSelection;

    public bool IsSelectionSuspended => _suspendSelection;
    public Action? ClearFocusRequest { get; set; }
    public Func<Task>? ReloadCompleted { get; set; }

    protected BaseBossSortableViewModel()
    {
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        SaveCommand = new AsyncRelayCommand(SaveItemAsync, () => SelectedItem != null);
        DeleteCommand = new AsyncRelayCommand(DeleteItemAsync, () => SelectedItem != null);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);

        SelectedItemChangedAsync += async () =>
        {
            if (_suspendSelection)
                return;

            await OnSelectedItemChangedAsync();

            // Commands aktualisieren
            SaveCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        };
    }

    // Module überschreiben nur diese Methode
    protected virtual Task OnSelectedItemChangedAsync()
    {
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    #region ABSTRACT CRUD HOOKS
    // ---------------------------------------------------------

    protected abstract TDomain CreateNewDomain();
    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
    protected abstract Task DeleteDomainAsync(TDomain domain);
 //   protected abstract Task<IEnumerable<TDomain>> LoadItemsAsync();
    protected abstract Task<IEnumerable<TDomain>> SearchAsync(string filter);
    protected abstract int GetId(TDomain domain);

    #endregion ABSTRACT CRUD HOOKS

    // ---------------------------------------------------------
    #region NEW
    // ---------------------------------------------------------
    private void NewItem()
    {
        var domain = CreateNewDomain();
        Items.Add(domain);
        SelectedItem = domain;
    }

    #endregion NEW
    // ---------------------------------------------------------
    #region SAVE
    // ---------------------------------------------------------
    private async Task SaveItemAsync()
    {
        if (SelectedItem == null)
            return;

        var saved = await SaveDomainAsync(SelectedItem);

        await ReloadAndSelectAsync(saved);
    }

    #endregion SAVE

    // ---------------------------------------------------------
    #region  DELETE
    // ---------------------------------------------------------
    private async Task DeleteItemAsync()
    {
        if (SelectedItem == null)
            return;

        await DeleteDomainAsync(SelectedItem);

        await ReloadAndSelectAsync(null);
    }

    #endregion  DELETE

    // ---------------------------------------------------------
    #region REFRESH
    // ---------------------------------------------------------
    private async Task RefreshAsync()
    {
        await ReloadAndSelectAsync(SelectedItem);
    }

    #endregion  REFRESH

    // ---------------------------------------------------------
    #region RELOAD + RESELECT (FILTER-FÄHIG, FOKUS-SICHER)
    // ---------------------------------------------------------


    protected async Task ReloadAndSelectAsync(TDomain? saved)
    {
        _suspendSelection = true;

        // Alte Auswahl merken
        var oldSelected = SelectedItem;
        var oldIndex = oldSelected != null ? Items.IndexOf(oldSelected) : -1;

        // Neue Liste laden
        var list =
            !string.IsNullOrWhiteSpace(_currentFilter)
                ? await SearchAsync(_currentFilter)
                : await LoadItemsAsync();

        Items.Clear();
        foreach (var item in list)
            Items.Add(item);


        // Neue Auswahl bestimmen
        TDomain? newSelection = null;

        if (saved != null)
        {
            var id = GetId(saved);
            newSelection = Items.FirstOrDefault(x => GetId(x) == id);
        }

        if (newSelection == null && oldIndex >= 0 && oldIndex < Items.Count)
            newSelection = Items[oldIndex];

        if (newSelection == null)
            newSelection = Items.FirstOrDefault();

        // UI fertig bauen lassen
        await Task.Yield();

        // 🔥 WICHTIG: Fokus neutralisieren + Scrollposition wiederherstellen
        if (ReloadCompleted != null)
            await ReloadCompleted.Invoke();

        // Jetzt erst SelectedItem setzen
        SelectedItem = newSelection;

        _suspendSelection = false;

        await OnSelectedItemChangedAsync();

        SaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }


    //protected async Task ReloadAndSelectAsync(TDomain? saved)
    //{
    //    _suspendSelection = true;

    //    // Merke dir vorher:
    //    var oldSelected = SelectedItem;
    //    var oldIndex = oldSelected != null ? Items.IndexOf(oldSelected) : -1;

    //    var list =
    //        !string.IsNullOrWhiteSpace(_currentFilter)
    //            ? await SearchAsync(_currentFilter)
    //            : await LoadItemsAsync();

    //    Items.Clear();
    //    foreach (var item in list)
    //        Items.Add(item);

    //    TDomain? newSelection = null;

    //    // 1) Wenn ein "saved" übergeben wurde → versuche, denselben Datensatz per ID zu finden
    //    if (saved != null)
    //    {
    //        var id = GetId(saved);
    //        newSelection = Items.FirstOrDefault(x => GetId(x) == id);
    //    }

    //    // 2) Wenn nichts gefunden → versuche, die alte Position zu halten
    //    if (newSelection == null && oldIndex >= 0 && oldIndex < Items.Count)
    //    {
    //        newSelection = Items[oldIndex];
    //    }

    //    // 3) Fallback → erste Zeile
    //    if (newSelection == null)
    //    {
    //        newSelection = Items.FirstOrDefault();
    //    }

    //    SelectedItem = newSelection;

    //    _suspendSelection = false;

    //    await OnSelectedItemChangedAsync();

    //    SaveCommand.NotifyCanExecuteChanged();
    //    DeleteCommand.NotifyCanExecuteChanged();


    //}

    //protected async Task ReloadAndSelectAsync(TDomain? saved)
    //{
    //    _suspendSelection = true;

    //    IEnumerable<TDomain> list =
    //        !string.IsNullOrWhiteSpace(_currentFilter)
    //            ? await SearchAsync(_currentFilter)
    //            : await LoadItemsAsync();

    //    Items.Clear();
    //    foreach (var item in list)
    //        Items.Add(item);

    //    if (saved == null)
    //    {
    //        // gleiche Position halten
    //        SelectedItem = Items.FirstOrDefault();
    //    }
    //    else
    //    {
    //        // gleichen Datensatz wieder auswählen
    //        int id = GetId(saved);
    //        SelectedItem = Items.FirstOrDefault(x => GetId(x) == id);

    //    }

    //    _suspendSelection = false;

    //    // Jetzt bewusst EINMAL sauber ausführen
    //    await OnSelectedItemChangedAsync();

    //    // Commands aktualisieren
    //    (SaveCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    //    (DeleteCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    //}

    #endregion RELOAD + RESELECT (FILTER-FÄHIG, FOKUS-SICHER)

}
