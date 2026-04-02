
using System.Collections.ObjectModel;
using ATIS.Interfaces;

namespace ATIS.Views.Base;

public abstract class Base1List1DetailViewModel<TDomain, TEdit>
    : Base1List1DetailViewModelBase
    where TDomain : class, ICopyable<TDomain>, new()
    where TEdit : BaseEditModel, new()
{
    // ------------------------------------------------------------
    #region [ EDIT ITEM ]
    // ------------------------------------------------------------

    private TEdit? _editItem;
    public TEdit? EditItem
    {
        get => _editItem;
        set => SetProperty(ref _editItem, value);
    }

    public override object? EditItemUntyped => EditItem;

    #endregion

    // ------------------------------------------------------------
    #region [ STATE ]
    // ------------------------------------------------------------

    private TDomain? _selectedItem;
    public TDomain? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                // Nur laden, wenn wir NICHT im Edit-Modus sind
                if (!IsInEdit)
                {
                    if (value != null)
                    {
                        BackupItem = CloneDomain(value);
                        EditItem = ConvertDomainToEdit(BackupItem);
                    }
                    else
                    {
                        BackupItem = default;
                        EditItem = default;
                    }
                }

                IsDetailVisible = value != null;
                UpdateCommands();
            }
        }
    }

    protected bool IsNew { get; private set; }

    private bool _isInEdit;
    public bool IsInEdit
    {
        get => _isInEdit;
        private set
        {
            if (SetProperty(ref _isInEdit, value))
                UpdateCommands();
        }
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        private set => SetProperty(ref _isDetailVisible, value);
    }

    private TDomain? _backupItem;
    public TDomain? BackupItem
    {
        get => _backupItem;
        private set => SetProperty(ref _backupItem, value);
    }

    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        private set => SetProperty(ref _dataSetCount, value);
    }

    private bool _isSearching;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private TDomain? _previousSelectedItem;

    private enum LoadMode
    {
        None,
        Search
    }

    private bool _shouldFocusFirstField;
    public bool ShouldFocusFirstField
    {
        get => _shouldFocusFirstField;
        set => SetProperty(ref _shouldFocusFirstField, value);
    }

    private bool _isReadOnlyMode = true;
    public bool IsReadOnlyMode
    {
        get => _isReadOnlyMode;
        set
        {
            if (_isReadOnlyMode != value)
            {
                _isReadOnlyMode = value;
                OnPropertyChanged();
            }
        }
    }

    private LoadMode _currentLoadMode = LoadMode.None;
    private string? _lastSearchText;

    protected readonly IDialogService _dialogService;

    private readonly ObservableCollection<TDomain> _itemsInternal = new();
    public ObservableCollection<TDomain> Items => _itemsInternal;

    #endregion

    // ------------------------------------------------------------
    #region [ COMMANDS + CTOR ]
    // ------------------------------------------------------------

    public IAsyncRelayCommand LoadCommand { get; }
    public IRelayCommand NewCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    protected Base1List1DetailViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StartEdit, () => SelectedItem != null);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => IsInEdit);
        CancelCommand = new AsyncRelayCommand(CancelEdit);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItem != null);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    #endregion

    // ------------------------------------------------------------
    #region [ ABSTRACTS ]
    // ------------------------------------------------------------

    protected abstract Task<IEnumerable<TDomain>> SearchItemsAsync(string? searchText);

    protected virtual async Task<IEnumerable<TDomain>> LoadItemsAsync()
    {
        return _currentLoadMode switch
        {
            LoadMode.Search when !string.IsNullOrWhiteSpace(_lastSearchText)
                => await SearchItemsAsync(_lastSearchText),

            _ => Enumerable.Empty<TDomain>()
        };
    }

    protected abstract TDomain CreateNewDomainModel();
    protected abstract TDomain CloneDomain(TDomain source);
    protected abstract TEdit ConvertDomainToEdit(TDomain domain);
    protected abstract void ConvertEditToDomain(TEdit edit, TDomain domain);
    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
    protected abstract Task DeleteDomainAsync(TDomain domain);
    protected abstract int GetId(TDomain model);
    protected abstract void OnBeforeSave(TDomain domain, bool isNew);

    #endregion

    // ------------------------------------------------------------
    #region [ LOAD + SETITEMS ]
    // ------------------------------------------------------------

    private async Task LoadAsync()
    {
        SetItems(Enumerable.Empty<TDomain>());
        DataSetCount = 0;

        SelectedItem = null;
        EditItem = null;

        OnPropertyChanged(nameof(Items));
    }

    protected void SetItems(IEnumerable<TDomain> items)
    {
        _itemsInternal.Clear();
        foreach (var item in items)
            _itemsInternal.Add(item);
    }

    #endregion

    // ------------------------------------------------------------
    #region [ SEARCH ]
    // ------------------------------------------------------------

    private async Task SearchAsync()
    {
        if (_isSearching)
            return;

        _isSearching = true;

        try
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                _lastSearchText = null;

                SetItems(Enumerable.Empty<TDomain>());
                DataSetCount = 0;

                SelectedItem = null;
                EditItem = null;

                OnPropertyChanged(nameof(Items));
                return;
            }

            _lastSearchText = SearchText;
            _currentLoadMode = LoadMode.Search;

            var result = await SearchItemsAsync(SearchText);

            SetItems(result);
            DataSetCount = Items.Count;

            OnPropertyChanged(nameof(Items));
        }
        finally
        {
            _isSearching = false;
        }
    }

    #endregion

    // ------------------------------------------------------------
    #region [ NEW ]
    // ------------------------------------------------------------

    private void NewItem()
    {
        _previousSelectedItem = SelectedItem;

        var domain = CreateNewDomainModel();

        Items.Add(domain);

        BackupItem = CloneDomain(domain);
        EditItem = ConvertDomainToEdit(BackupItem);

        SelectedItem = domain;

        IsNew = true;
        IsInEdit = true;
        IsReadOnlyMode = false;

        ShouldFocusFirstField = true;
        ShouldFocusFirstField = false;
    }

    #endregion

    // ------------------------------------------------------------
    #region [ EDIT ]
    // ------------------------------------------------------------

    private void StartEdit()
    {
        if (SelectedItem == null)
            return;

        BackupItem = CloneDomain(SelectedItem);
        EditItem = ConvertDomainToEdit(BackupItem);

        IsNew = false;
        IsInEdit = true;
        IsReadOnlyMode = false;

        ShouldFocusFirstField = true;
        ShouldFocusFirstField = false;
    }

    #endregion

    // ------------------------------------------------------------
    #region [ CANCEL ]
    // ------------------------------------------------------------

    protected virtual async Task CancelEdit()
    {
        var confirm = await _dialogService.ShowOkCancelAsync("Cancel_Title", "Cancel_Message");
        if (!confirm)
            return;

        if (IsNew)
        {
            var removed = SelectedItem;
            if (removed != null)
                Items.Remove(removed);

            SelectedItem = _previousSelectedItem;

            EditItem = SelectedItem != null
                ? ConvertDomainToEdit(SelectedItem)
                : null;
        }
        else
        {
            if (BackupItem != null && SelectedItem != null)
                SelectedItem.CopyFrom(BackupItem);

            EditItem = BackupItem != null
                ? ConvertDomainToEdit(BackupItem)
                : null;
        }

        IsInEdit = false;
        IsNew = false;
        IsReadOnlyMode = true;
    }

    #endregion

    // ------------------------------------------------------------
    #region [ SAVE + VALIDATE + EXISTS ]
    // ------------------------------------------------------------

    protected async Task SaveAsync()
    {
        if (SelectedItem == null || EditItem == null)
            return;

        var confirm = await _dialogService.ShowYesNoAsync("Save_Title", "Save_Question");
        if (!confirm)
            return;

        if (!await ValidateBeforeSaveAsync())
            return;

        if (await ExistsAsync(EditItem))
        {
            await _dialogService.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return;
        }

        ConvertEditToDomain(EditItem, SelectedItem);
        OnBeforeSave(SelectedItem, IsNew);

        var saved = await SaveDomainAsync(SelectedItem);

        await _dialogService.ShowInfoAsync("Save_Success_Title", "Save_Success_Message");

        _currentLoadMode = LoadMode.Search;
        await ReloadAndSelectAsync(saved);

        IsInEdit = false;
        IsNew = false;
        IsReadOnlyMode = true;
    }

    protected virtual Task<bool> ValidateBeforeSaveAsync()
        => Task.FromResult(true);

    protected virtual Task<bool> ExistsAsync(TEdit edit)
        => Task.FromResult(false);

    #endregion

    // ------------------------------------------------------------
    #region [ DELETE ]
    // ------------------------------------------------------------

    protected virtual async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        var confirm = await _dialogService.ShowYesNoAsync("Delete_Title", "Delete_Message");
        if (!confirm)
            return;

        await DeleteDomainAsync(SelectedItem);

        await _dialogService.ShowInfoAsync("Delete_Success_Title", "Delete_Success_Message");

        _currentLoadMode = LoadMode.Search;
        await ReloadAndSelectAsync(null);
    }

    #endregion

    // ------------------------------------------------------------
    #region [ RELOAD + RE-SELECT ]
    // ------------------------------------------------------------

    protected virtual async Task ReloadAndSelectAsync(TDomain? saved)
    {
        var result = await LoadItemsAsync();
        SetItems(result);
        OnPropertyChanged(nameof(Items));
        DataSetCount = Items.Count();

        if (saved != null)
        {
            SelectedItem = Items.FirstOrDefault(x => GetId(x) == GetId(saved));
        }
        else
        {
            SelectedItem = Items.FirstOrDefault();
        }
    }

    #endregion

    // ------------------------------------------------------------
    #region [ REFRESH ]
    // ------------------------------------------------------------

    private async Task RefreshAsync()
    {
        if (string.IsNullOrWhiteSpace(_lastSearchText))
            return;

        _previousSelectedItem = SelectedItem;

        var result = await SearchItemsAsync(_lastSearchText);

        SetItems(result);
        DataSetCount = Items.Count();

        if (_previousSelectedItem != null)
        {
            var match = Items.FirstOrDefault(x => GetId(x) == GetId(_previousSelectedItem));
            SelectedItem = match ?? Items.FirstOrDefault();
        }
        else
        {
            SelectedItem = Items.FirstOrDefault();
        }

        EditItem = SelectedItem != null
            ? ConvertDomainToEdit(SelectedItem)
            : null;

        OnPropertyChanged(nameof(Items));
    }

    #endregion

    // ------------------------------------------------------------
    #region [ COMMAND UPDATES ]
    // ------------------------------------------------------------

    protected virtual void UpdateCommands()
    {
        SaveCommand.NotifyCanExecuteChanged();
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        NewCommand.NotifyCanExecuteChanged();
    }

    #endregion

    // ------------------------------------------------------------
    #region [ SORT ]
    // ------------------------------------------------------------

    public void Sort(string columnName, bool ascending)
    {
        var prop = typeof(TDomain).GetProperty(columnName);
        if (prop == null)
            return;

        var sorted = ascending
            ? Items.OrderBy(x => prop.GetValue(x) ?? "").ToList()
            : Items.OrderByDescending(x => prop.GetValue(x) ?? "").ToList();

        _itemsInternal.Clear();
        foreach (var item in sorted)
            _itemsInternal.Add(item);
    }

    #endregion
}




//using System.Collections.ObjectModel;
//using ATIS.Interfaces;

//namespace ATIS.Views.Base;

//public abstract class Base1List1DetailViewModel<TDomain, TEdit>
//    : Base1List1DetailViewModelBase
//    where TDomain : class, ICopyable<TDomain>, new()
//    where TEdit : BaseEditModel, new()
//{
//    // ------------------------------------------------------------
//    #region [ EDIT ITEM ]
//    // ------------------------------------------------------------

//    public event EventHandler? EditStarted;
//    public event EventHandler? NewStarted;

//    private TEdit? _editItem;

//    public TEdit? EditItem
//    {
//        get => _editItem;
//        set => SetProperty(ref _editItem, value);
//    }
//    public override object? EditItemUntyped => EditItem;

//    #endregion [ EDIT ITEM ]

//    // ------------------------------------------------------------
//    #region [ STATE ]
//    // ------------------------------------------------------------

//    private TDomain? _selectedItem;
//    public TDomain? SelectedItem
//    {
//        get => _selectedItem;
//        set
//        {
//            if (SetProperty(ref _selectedItem, value))
//            {
//                // WICHTIG: Nur laden, wenn wir NICHT im Edit-Modus sind
//                if (!IsInEdit)
//                {
//                    if (value != null)
//                    {
//                        BackupItem = CloneDomain(value);
//                        EditItem = ConvertDomainToEdit(BackupItem);
//                    }
//                    else
//                    {
//                        BackupItem = default;
//                        EditItem = default;
//                    }
//                }

//                IsDetailVisible = value != null;
//                UpdateCommands();
//            }
//        }
//    }

//    protected bool IsNew { get; private set; }

//    private bool _isInEdit;
//    public bool IsInEdit
//    {
//        get => _isInEdit;
//        private set
//        {
//            if (SetProperty(ref _isInEdit, value))
//                UpdateCommands();
//        }
//    }


//    private bool _isDetailVisible;
//    public bool IsDetailVisible
//    {
//        get => _isDetailVisible;
//        private set => SetProperty(ref _isDetailVisible, value);
//    }


//    private TDomain? _backupItem;
//    public TDomain? BackupItem
//    {
//        get => _backupItem;
//        private set => SetProperty(ref _backupItem, value);
//    }

//    private string? _searchText;
//    public string? SearchText
//    {
//        get => _searchText;
//        set => SetProperty(ref _searchText, value);
//    }

//    private int _dataSetCount;
//    public int DataSetCount
//    {
//        get => _dataSetCount;
//        private set => SetProperty(ref _dataSetCount, value);
//    }

//    private bool _isSearching;

//    private bool _isLoading;
//    public bool IsLoading
//    {
//        get => _isLoading;
//        set => SetProperty(ref _isLoading, value);
//    }
//    private TDomain? _previousSelectedItem;
//    private enum LoadMode
//    {
//        None, // Startzustand
//        Search // letzte Suche
//    }

//    private bool _shouldFocusFirstField;
//    public bool ShouldFocusFirstField
//    {
//        get => _shouldFocusFirstField;
//        set => SetProperty(ref _shouldFocusFirstField, value);
//    }

//    private bool _isReadOnlyMode = true;
//    public bool IsReadOnlyMode
//    {
//        get => _isReadOnlyMode;
//        set
//        {
//            if (_isReadOnlyMode != value)
//            {
//                _isReadOnlyMode = value;
//                OnPropertyChanged();
//            }
//        }
//    }



//    private LoadMode _currentLoadMode = LoadMode.None;
//    private string? _lastSearchText;

//    protected readonly IDialogService _dialogService;

//    private readonly ObservableCollection<TDomain> _itemsInternal = new();
//    public ObservableCollection<TDomain> Items => _itemsInternal;
//    #endregion [ STATE ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ COMMANDS + KONSTRUCTOR ]
//    // ------------------------------------------------------------
//    public IAsyncRelayCommand LoadCommand { get; }
//    public IRelayCommand NewCommand { get; }
//    public IRelayCommand EditCommand { get; }
//    public IAsyncRelayCommand SaveCommand { get; }
//    public IRelayCommand CancelCommand { get; }
//    public IAsyncRelayCommand DeleteCommand { get; }
//    public IAsyncRelayCommand SearchCommand { get; }
//    public IAsyncRelayCommand RefreshCommand { get; }

//    protected Base1List1DetailViewModel(IDialogService dialogService)
//    {
//        _dialogService = dialogService;

//        LoadCommand = new AsyncRelayCommand(LoadAsync);
//        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
//        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StartEdit, () => SelectedItem != null);
//        SaveCommand = new AsyncRelayCommand(SaveAsync, () => IsInEdit);
//        CancelCommand = new AsyncRelayCommand(CancelEdit);
//        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItem != null);
//        SearchCommand = new AsyncRelayCommand(SearchAsync);
//        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
//    }
//    #endregion [ COMMANDS ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ ABSTRACTS ]
//    // ------------------------------------------------------------
//    protected abstract Task<IEnumerable<TDomain>> SearchItemsAsync(string? searchText);

//    protected virtual async Task<IEnumerable<TDomain>> LoadItemsAsync()
//    {
//        return _currentLoadMode switch
//        {
//            LoadMode.Search when !string.IsNullOrWhiteSpace(_lastSearchText)
//                => await SearchItemsAsync(_lastSearchText),

//            _ => Enumerable.Empty<TDomain>()
//        };
//    }

//    protected abstract TDomain CreateNewDomainModel();
//    protected abstract TDomain CloneDomain(TDomain source);
//    protected abstract TEdit ConvertDomainToEdit(TDomain domain);
//    protected abstract void ConvertEditToDomain(TEdit edit, TDomain domain);
//    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
//    protected abstract Task DeleteDomainAsync(TDomain domain);
//    protected abstract int GetId(TDomain model);
//    protected abstract void OnBeforeSave(TDomain domain, bool isNew);
//    #endregion [ ABSTRACTS ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ LOAD + SETITEMS ]
//    // ------------------------------------------------------------
//    private async Task LoadAsync()
//    {
//        // Startzustand: keine Daten
//        SetItems(Enumerable.Empty<TDomain>());
//        DataSetCount = 0;

//        SelectedItem = null;
//        EditItem = null;

//        OnPropertyChanged(nameof(Items));
//    }

//    protected void SetItems(IEnumerable<TDomain> items)
//    {
//        _itemsInternal.Clear();
//        foreach (var item in items)
//            _itemsInternal.Add(item);
//    }
//    #endregion [ LOAD + SETITEMS ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ SEARCH ]
//    // ------------------------------------------------------------

//    private async Task SearchAsync()
//    {
//        if (_isSearching)
//            return;

//        _isSearching = true;

//        try
//        {
//            // 1) Leeres Suchfeld → Liste leeren
//            if (string.IsNullOrWhiteSpace(SearchText))
//            {
//                _lastSearchText = null;

//                SetItems(Enumerable.Empty<TDomain>());
//                DataSetCount = 0;

//                SelectedItem = null;
//                EditItem = null;

//                OnPropertyChanged(nameof(Items));
//                return;
//            }

//            // 2) Suchtext merken (für Refresh)
//            _lastSearchText = SearchText;

//            // 3) Suche ausführen
//            var result = await SearchItemsAsync(SearchText);

//            SetItems(result);
//            DataSetCount = Items.Count;

//            OnPropertyChanged(nameof(Items));
//        }
//        finally
//        {
//            _isSearching = false;
//        }
//    }

//    #endregion [ SEARCH ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ NEW ]
//    // ------------------------------------------------------------
//    private void NewItem()
//    {
//        _previousSelectedItem = SelectedItem; // <— wichtig

//        var domain = CreateNewDomainModel();

//        // 1) In Items einfügen (sichtbar im Grid)
//        Items.Add(domain);

//        // 2) EditItem vorbereiten
//        BackupItem = CloneDomain(domain);
//        EditItem = ConvertDomainToEdit(BackupItem);

//        // 3) Im Grid auswählen
//        SelectedItem = domain;

//        // 4) Edit-Modus aktivieren
//        IsNew = true;
//        IsInEdit = true;
//        IsReadOnlyMode = false;

//        // 5) Fokus setzen
//        ShouldFocusFirstField = true;
//        ShouldFocusFirstField = false; // Reset
//    }
//    #endregion [ NEW ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ EDIT ]
//    // ------------------------------------------------------------
//    private void StartEdit()
//    {
//        if (SelectedItem == null)
//            return;

//        BackupItem = CloneDomain(SelectedItem);
//        EditItem = ConvertDomainToEdit(BackupItem);

//        IsNew = false;
//        IsInEdit = true;
//        IsReadOnlyMode = false;

//        // 5) Fokus setzen
//        ShouldFocusFirstField = true;
//        ShouldFocusFirstField = false; // Reset
//    }
//    #endregion [ EDIT ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ CANCEL ]
//    // ------------------------------------------------------------
//    protected virtual async Task CancelEdit()
//    {
//        var confirm = await _dialogService.ShowOkCancelAsync("Cancel_Title", "Cancel_Message");
//        if (!confirm)
//            return;

//        if (IsNew)
//        {
//            // 1. Temporären Datensatz entfernen
//            var removed = SelectedItem;
//            Items.Remove(removed);

//            // 2. zurück zum vorherigen Datensatz
//            SelectedItem = _previousSelectedItem;

//            // 3. Detail zurücksetzen
//            EditItem = SelectedItem != null
//                ? ConvertDomainToEdit(SelectedItem)
//                : null;
//        }
//        else
//        {
//            // Bearbeiten → Änderungen verwerfen
//            if (BackupItem != null && SelectedItem != null)
//                SelectedItem.CopyFrom(BackupItem);

//            EditItem = ConvertDomainToEdit(BackupItem);
//        }

//        IsInEdit = false;
//        IsNew = false;
//        IsReadOnlyMode = true;

//    }

//    //protected virtual async Task CancelEdit()
//    //{
//    //    // 1. Bestätigung
//    //    var confirm = await DialogService.ShowOkCancelAsync("Cancel_Title", "Cancel_Message");
//    //    if (!confirm)
//    //        return;

//    //    // 2. Änderungen verwerfen
//    //    if (BackupItem != null)
//    //    {
//    //        // Domain zurücksetzen
//    //        if (SelectedItem != null)
//    //            SelectedItem.CopyFrom(BackupItem);

//    //        // EditItem neu erzeugen
//    //        EditItem = ConvertDomainToEdit(BackupItem);
//    //    }

//    //    // 3. Flags zurücksetzen
//    //    IsInEdit = false;
//    //    IsNew = false;
//    //}

//    //private void CancelEdit()
//    //{
//    //    if (BackupItem != null && SelectedItem != null)
//    //    {
//    //        SelectedItem.CopyFrom(BackupItem);
//    //        EditItem = ConvertDomainToEdit(BackupItem);
//    //    }

//    //    IsInEdit = false;
//    //}
//    #endregion [ CANCEL ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ SAVE + VALIDATE + EXISTS ]
//    // ------------------------------------------------------------
//    protected async Task SaveAsync()
//    {
//        if (SelectedItem == null || EditItem == null)
//            return;

//        // 1) Vor dem Speichern: Ja/Nein
//        var confirm = await _dialogService.ShowYesNoAsync("Save_Title", "Save_Question");
//        if (!confirm)
//            return;

//        // 2) Validierung
//        if (!await ValidateBeforeSaveAsync())
//            return;

//        if (await ExistsAsync(EditItem))
//        {
//            await _dialogService.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return;
//        }

//        // 3) Domain aktualisieren
//        ConvertEditToDomain(EditItem, SelectedItem);
//        OnBeforeSave(SelectedItem, IsNew);

//        // 4) Speichern
//        var saved = await SaveDomainAsync(SelectedItem);

//        // 5) Nach dem Speichern: OK
//        await _dialogService.ShowInfoAsync("Save_Success_Title", "Save_Success_Message");

//        // 6) Reload + Auswahl
//        _currentLoadMode = LoadMode.Search;
//        await ReloadAndSelectAsync(saved);

//        IsInEdit = false;
//        IsNew = false;
//        IsReadOnlyMode = true;
//    }

//    protected virtual Task<bool> ValidateBeforeSaveAsync()
//    {
//        // Default: keine Validierung
//        return Task.FromResult(true);
//    }

//    protected virtual Task<bool> ExistsAsync(TEdit edit)
//    {
//        return Task.FromResult(false);
//    }

//    #endregion [  SAVE (A1) ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ DELETE ]
//    // ------------------------------------------------------------
//    protected virtual async Task DeleteAsync()
//    {
//        if (SelectedItem == null)
//            return;

//        // 1. Bestätigung
//        var confirm = await _dialogService.ShowYesNoAsync("Delete_Title", "Delete_Message");
//        if (!confirm)
//            return;

//        // 2. Löschen
//        await DeleteDomainAsync(SelectedItem);

//        // 3. Erfolgsmeldung
//        await _dialogService.ShowInfoAsync("Delete_Success_Title", "Delete_Success_Message");

//        // 4. Reload
//        _currentLoadMode = LoadMode.Search;
//        await ReloadAndSelectAsync(null);
//    }

//    #endregion [ DELETE (A1) ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ RELOAD +  RE-SELECT ]
//    // ------------------------------------------------------------

//    protected virtual async Task ReloadAndSelectAsync(TDomain? saved)
//    {
//        var result = await LoadItemsAsync();
//        SetItems(result);
//        OnPropertyChanged(nameof(Items));
//        DataSetCount = Items.Count();

//        if (saved != null)
//        {
//            SelectedItem = Items.FirstOrDefault(x => GetId(x) == GetId(saved));
//        }
//        else
//        {
//            SelectedItem = Items.FirstOrDefault();
//        }
//    }
//    #endregion [ RELOAD +  RE-SELECT ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ REFRESH ]
//    // ------------------------------------------------------------
//    private async Task RefreshAsync()
//    {
//        if (string.IsNullOrWhiteSpace(_lastSearchText))
//            return;

//        // 1. vorherigen Datensatz merken
//        _previousSelectedItem = SelectedItem;

//        // 2. Suche erneut ausführen
//        var result = await SearchItemsAsync(_lastSearchText);

//        SetItems(result);
//        DataSetCount = Items.Count();

//        // 3. vorherigen Datensatz wieder auswählen
//        if (_previousSelectedItem != null)
//        {
//            var match = Items.FirstOrDefault(x => GetId(x) == GetId(_previousSelectedItem));
//            SelectedItem = match ?? Items.FirstOrDefault();
//        }
//        else
//        {
//            SelectedItem = Items.FirstOrDefault();
//        }

//        // 4. Detail aktualisieren
//        EditItem = SelectedItem != null
//            ? ConvertDomainToEdit(SelectedItem)
//            : null;

//        OnPropertyChanged(nameof(Items));
//    }
//    #endregion [ REFRESH ]
//    // ------------------------------------------------------------
//    // ------------------------------------------------------------
//    #region [ COMMAND UPDATES ]
//    // ------------------------------------------------------------
//    protected virtual void UpdateCommands()
//    {
//        SaveCommand.NotifyCanExecuteChanged();
//        EditCommand.NotifyCanExecuteChanged();
//        DeleteCommand.NotifyCanExecuteChanged();
//        NewCommand.NotifyCanExecuteChanged();
//    }
//    #endregion [ COMMAND UPDATES ]
//    // ---------------------------------------------------------
//    // ---------------------------------------------------------
//    #region [ SORT ]
//    // ---------------------------------------------------------
//    public void Sort(string columnName, bool ascending)
//    {
//        var prop = typeof(TDomain).GetProperty(columnName);
//        if (prop == null)
//            return;

//        var sorted = ascending
//            ? Items.OrderBy(x => prop.GetValue(x) ?? "").ToList()
//            : Items.OrderByDescending(x => prop.GetValue(x) ?? "").ToList();

//        _itemsInternal.Clear();
//        foreach (var item in sorted)
//            _itemsInternal.Add(item);
//    }
//    #endregion [ SORT ]
//    // ---------------------------------------------------------
//}
