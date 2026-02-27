using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Views.Modules.Tbl90RefExpert;

public abstract class Base1List1DetailViewModel<TDomain, TEdit>
    : ObservableObject
    where TDomain : class, ICopyable<TDomain>, new()
    where TEdit : class, new()
{
    // ------------------------------------------------------------
    #region [ STATE ]
    // ------------------------------------------------------------
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

    private TDomain? _selectedItem;
    public TDomain? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                // WICHTIG: Nur laden, wenn wir NICHT im Edit-Modus sind
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

    private TDomain? _backupItem;
    public TDomain? BackupItem
    {
        get => _backupItem;
        private set => SetProperty(ref _backupItem, value);
    }

    private TEdit? _editItem;
    public TEdit? EditItem
    {
        get => _editItem;
        private set => SetProperty(ref _editItem, value);
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

    protected readonly IDialogService DialogService;

    private readonly ObservableCollection<TDomain> _itemsInternal = new();
    public ObservableCollection<TDomain> Items => _itemsInternal;
    #endregion [ STATE ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ COMMANDS + KONSTRUCTOR ]
    // ------------------------------------------------------------
    public IAsyncRelayCommand LoadCommand { get; }
    public IRelayCommand NewCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }

    protected Base1List1DetailViewModel(IDialogService dialogService)
    {
        DialogService = dialogService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StartEdit, () => SelectedItem != null);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => IsInEdit);
        CancelCommand = new AsyncRelayCommand(CancelEdit);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItem != null);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
    }
    #endregion [ COMMANDS ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ ABSTRACTS ]
    // ------------------------------------------------------------
    //protected virtual Task<IEnumerable<TDomain>> LoadItemsAsync()
    //    => Task.FromResult(Enumerable.Empty<TDomain>());
    //  Enumerable.Empty<Tbl90RefExpert>();
    protected abstract Task<IEnumerable<TDomain>> LoadItemsAsync();
    protected abstract TDomain CreateNewDomainModel();
    protected abstract TDomain CloneDomain(TDomain source);
    protected abstract TEdit ConvertDomainToEdit(TDomain domain);
    protected abstract void ConvertEditToDomain(TEdit edit, TDomain domain);
    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
    protected abstract Task DeleteDomainAsync(TDomain domain);
    protected abstract int GetId(TDomain model);
    protected abstract void OnBeforeSave(TDomain domain, bool isNew);
    protected abstract Task<IEnumerable<TDomain>> SearchItemsAsync(string? searchText);
    #endregion [ ABSTRACTS ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ LOAD ]
    // ------------------------------------------------------------
    private async Task LoadAsync()
    {
        var result = await LoadItemsAsync();
        SetItems(result);
        OnPropertyChanged(nameof(Items));
    }
    protected void SetItems(IEnumerable<TDomain> items)
    {
        _itemsInternal.Clear();
        foreach (var item in items)
            _itemsInternal.Add(item);
    }
    #endregion [ LOAD ]
    // ------------------------------------------------------------
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
                SetItems(Enumerable.Empty<TDomain>());
                SelectedItem = null;
                OnPropertyChanged(nameof(Items));
                DataSetCount = 0;
                //return;
            }

            var result = await SearchItemsAsync(SearchText);

            SetItems(result);
            OnPropertyChanged(nameof(Items));
            DataSetCount = Items.Count();

            SelectedItem = Items.FirstOrDefault();
        }
        finally
        {
            _isSearching = false;
        }
    }
    #endregion [ SEARCH ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ NEW ]
    // ------------------------------------------------------------
    private void NewItem()
    {
        _previousSelectedItem = SelectedItem; // <— wichtig

        var domain = CreateNewDomainModel();

        // 1) In Items einfügen (sichtbar im Grid)
        Items.Add(domain);

        // 2) EditItem vorbereiten
        BackupItem = CloneDomain(domain);
        EditItem = ConvertDomainToEdit(BackupItem);

        // 3) Im Grid auswählen
        SelectedItem = domain;

        // 4) Edit-Modus aktivieren
        IsNew = true;
        IsInEdit = true;
    }
    //private void NewItem()
    //{
    //    var domain = CreateNewDomainModel();
    //    BackupItem = CloneDomain(domain);
    //    EditItem = ConvertDomainToEdit(BackupItem);
    //    SelectedItem = domain;
    //    DataSetCount = 0;

    //    DataSetCount = Items.Count;

    //    IsNew = true;
    //    IsInEdit = true;
    //}
    #endregion [ NEW ]
    // ------------------------------------------------------------
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
    }
    #endregion [ EDIT ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ CANCEL ]
    // ------------------------------------------------------------
    protected virtual async Task CancelEdit()
    {
        var confirm = await DialogService.ShowOkCancelAsync("Cancel_Title", "Cancel_Message");
        if (!confirm)
            return;

        if (IsNew)
        {
            // 1. Temporären Datensatz entfernen
            var removed = SelectedItem;
            Items.Remove(removed);

            // 2. zurück zum vorherigen Datensatz
            SelectedItem = _previousSelectedItem;

            // 3. Detail zurücksetzen
            EditItem = SelectedItem != null
                ? ConvertDomainToEdit(SelectedItem)
                : null;
        }
        else
        {
            // Bearbeiten → Änderungen verwerfen
            if (BackupItem != null && SelectedItem != null)
                SelectedItem.CopyFrom(BackupItem);

            EditItem = ConvertDomainToEdit(BackupItem);
        }

        IsInEdit = false;
        IsNew = false;
    }

    //protected virtual async Task CancelEdit()
    //{
    //    // 1. Bestätigung
    //    var confirm = await DialogService.ShowOkCancelAsync("Cancel_Title", "Cancel_Message");
    //    if (!confirm)
    //        return;

    //    // 2. Änderungen verwerfen
    //    if (BackupItem != null)
    //    {
    //        // Domain zurücksetzen
    //        if (SelectedItem != null)
    //            SelectedItem.CopyFrom(BackupItem);

    //        // EditItem neu erzeugen
    //        EditItem = ConvertDomainToEdit(BackupItem);
    //    }

    //    // 3. Flags zurücksetzen
    //    IsInEdit = false;
    //    IsNew = false;
    //}

    //private void CancelEdit()
    //{
    //    if (BackupItem != null && SelectedItem != null)
    //    {
    //        SelectedItem.CopyFrom(BackupItem);
    //        EditItem = ConvertDomainToEdit(BackupItem);
    //    }

    //    IsInEdit = false;
    //}
    #endregion [ CANCEL ]
    // ------------------------------------------------------------
    #region [ SAVE + VALIDATE + EXISTS ]
    // ------------------------------------------------------------
    protected async Task SaveAsync()
    {
        if (SelectedItem == null || EditItem == null)
            return;

        // 1) Vor dem Speichern: Ja/Nein
        var confirm = await DialogService.ShowYesNoAsync("Save_Title", "Save_Question");
        if (!confirm)
            return;

        // 2) Validierung
        if (!await ValidateBeforeSaveAsync())
            return;

        if (await ExistsAsync(EditItem))
        {
            await DialogService.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return;
        }

        // 3) Domain aktualisieren
        ConvertEditToDomain(EditItem, SelectedItem);
        OnBeforeSave(SelectedItem, IsNew);

        // 4) Speichern
        var saved = await SaveDomainAsync(SelectedItem);

        // 5) Nach dem Speichern: OK
        await DialogService.ShowInfoAsync("Save_Success_Title", "Save_Success_Message");

        // 6) Reload + Auswahl
        await ReloadAndSelectAsync(saved);

        IsInEdit = false;
        IsNew = false;
    }


    //private async Task SaveAsync()
    //{
    //    if (SelectedItem == null || EditItem == null)
    //        return;

    //    ConvertEditToDomain(EditItem, SelectedItem);

    //    OnBeforeSave(SelectedItem, IsNew);

    //    var saved = await SaveDomainAsync(SelectedItem);

    //    await ReloadAndSelectAsync(saved);

    //    IsInEdit = false;
    //}
    protected virtual Task<bool> ValidateBeforeSaveAsync()
    {
        // Default: keine Validierung
        return Task.FromResult(true);
    }

    protected virtual Task<bool> ExistsAsync(TEdit edit)
    {
        return Task.FromResult(false);
    }

    #endregion [  SAVE (A1) ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ DELETE ]
    // ------------------------------------------------------------
    protected virtual async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        // 1. Bestätigung
        var confirm = await DialogService.ShowYesNoAsync("Delete_Title", "Delete_Message");
        if (!confirm)
            return;

        // 2. Löschen
        await DeleteDomainAsync(SelectedItem);

        // 3. Erfolgsmeldung
        await DialogService.ShowInfoAsync("Delete_Success_Title", "Delete_Success_Message");

        // 4. Reload
        await ReloadAndSelectAsync(null);
    }

    //private async Task DeleteAsync()
    //{
    //    if (SelectedItem == null)
    //        return;

    //    await DeleteDomainAsync(SelectedItem);

    //    await ReloadAndSelectAsync(null);
    //}
    #endregion [ DELETE (A1) ]
    // ------------------------------------------------------------
    // ------------------------------------------------------------
    #region [ RELOAD +  RE-SELECT ]
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
    #endregion [ RELOAD +  RE-SELECT ]
    // ------------------------------------------------------------
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
    #endregion [ COMMAND UPDATES ]
    // ---------------------------------------------------------
    // ---------------------------------------------------------
    #region [ SORT ]
    // ---------------------------------------------------------
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
    #endregion [ SORT ]
    // ---------------------------------------------------------
}
