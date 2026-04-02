using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;

namespace ATIS.Views.Base;

public abstract class Base4List4DetailViewModel<TDomain, TEdit>
    : Base1List1DetailViewModelBase
    where TDomain : class, ICopyable<TDomain>, new()
    where TEdit : BaseEditModel, new()
{
    // ------------------------------------------------------------
    #region [ STATE ]
    // ------------------------------------------------------------

    private readonly ObservableCollection<TDomain> _itemsLevel1Internal = new();
    public ObservableCollection<TDomain> ItemsLevel1 => _itemsLevel1Internal;


    protected virtual Task OnSelectedItemChangedAsync(TDomain? item)
    {
        return Task.CompletedTask;
    }

    private TDomain? _selectedItemLevel1;
    public TDomain? SelectedItemLevel1
    {
        get => _selectedItemLevel1;
        set
        {
            if (SetProperty(ref _selectedItemLevel1, value))
            {
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

                if (value != null)
                    _ = LoadDetailLevelsAsync();

                UpdateCommands();

                // ⭐ NEU: Preview‑Hook
                _ = OnSelectedItemChangedAsync(value);
            }
        }
    }

    private TEdit? _editItem;
    public TEdit? EditItem
    {
        get => _editItem;
        set => SetProperty(ref _editItem, value);
    }
    public override object? EditItemUntyped => EditItem;

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
    private string? _lastSearchText;
    private TDomain? _previousSelectedItem;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
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

    private bool _shouldFocusFirstField;
    public bool ShouldFocusFirstField
    {
        get => _shouldFocusFirstField;
        set => SetProperty(ref _shouldFocusFirstField, value);
    }

    protected readonly IDialogService _dialogService;

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

    protected Base4List4DetailViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StartEdit, () => SelectedItemLevel1 != null);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => IsInEdit);
        CancelCommand = new AsyncRelayCommand(CancelEdit);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItemLevel1 != null);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    #endregion

    // ------------------------------------------------------------
    #region [ ABSTRACTS + DETAIL LOAD ]
    // ------------------------------------------------------------

    protected abstract Task<IEnumerable<TDomain>> SearchItemsAsync(string? searchText);
    protected abstract Task<IEnumerable<TDomain>> LoadDataAsync();

    protected abstract TDomain CreateNewDomainModel();
    protected abstract TDomain CloneDomain(TDomain source);
    protected abstract TEdit ConvertDomainToEdit(TDomain domain);
    protected abstract void ConvertEditToDomain(TEdit edit, TDomain domain);
    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
    protected abstract Task DeleteDomainAsync(TDomain domain);
    protected abstract int GetId(TDomain model);
    protected abstract void OnBeforeSave(TDomain domain, bool isNew);

    protected virtual Task LoadForeignKeyListsAsync() => Task.CompletedTask;
    protected virtual Task LoadLevel2Async() => Task.CompletedTask;
    protected virtual Task LoadLevel3Async() => Task.CompletedTask;
    protected virtual Task LoadLevel4Async() => Task.CompletedTask;

    private async Task LoadDetailLevelsAsync()
    {
        await LoadForeignKeyListsAsync();
        await LoadLevel2Async();
        await LoadLevel3Async();
        await LoadLevel4Async();
    }

    #endregion

    // ------------------------------------------------------------
    #region [ LOAD + SETITEMS ]
    // ------------------------------------------------------------

    private async Task LoadAsync()
    {
        SetItemsLevel1(Enumerable.Empty<TDomain>());
        DataSetCount = 0;

        SelectedItemLevel1 = null;
        EditItem = null;

        OnPropertyChanged(nameof(ItemsLevel1));
    }

    protected void SetItemsLevel1(IEnumerable<TDomain> items)
    {
        _itemsLevel1Internal.Clear();
        foreach (var item in items)
            _itemsLevel1Internal.Add(item);
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

                SetItemsLevel1(Enumerable.Empty<TDomain>());
                DataSetCount = 0;

                SelectedItemLevel1 = null;
                EditItem = null;

                OnPropertyChanged(nameof(ItemsLevel1));
                return;
            }

            _lastSearchText = SearchText;

            IsLoading = true;
            var result = await SearchItemsAsync(SearchText);
            IsLoading = false;

            SetItemsLevel1(result);

            DataSetCount = ItemsLevel1.Count;

            await LoadForeignKeyListsAsync();


            OnPropertyChanged(nameof(ItemsLevel1));
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
        _previousSelectedItem = SelectedItemLevel1;

        var domain = CreateNewDomainModel();

        ItemsLevel1.Add(domain);

        BackupItem = CloneDomain(domain);
        EditItem = ConvertDomainToEdit(BackupItem);

        SelectedItemLevel1 = domain;

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
        if (SelectedItemLevel1 == null)
            return;

        BackupItem = CloneDomain(SelectedItemLevel1);
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
            var removed = SelectedItemLevel1;
            if (removed != null)
                ItemsLevel1.Remove(removed);

            SelectedItemLevel1 = _previousSelectedItem;

            EditItem = SelectedItemLevel1 != null
                ? ConvertDomainToEdit(SelectedItemLevel1)
                : null;
        }
        else
        {
            if (BackupItem != null && SelectedItemLevel1 != null)
                SelectedItemLevel1.CopyFrom(BackupItem);

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
        if (SelectedItemLevel1 == null || EditItem == null)
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

        ConvertEditToDomain(EditItem, SelectedItemLevel1);
        OnBeforeSave(SelectedItemLevel1, IsNew);

        var saved = await SaveDomainAsync(SelectedItemLevel1);

        await _dialogService.ShowInfoAsync("Save_Success_Title", "Save_Success_Message");

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
        if (SelectedItemLevel1 == null)
            return;

        var confirm = await _dialogService.ShowYesNoAsync("Delete_Title", "Delete_Message");
        if (!confirm)
            return;

        await DeleteDomainAsync(SelectedItemLevel1);

        await _dialogService.ShowInfoAsync("Delete_Success_Title", "Delete_Success_Message");

        await ReloadAndSelectAsync(null);
    }

    #endregion

    // ------------------------------------------------------------
    #region [ RELOAD + RE-SELECT ]
    // ------------------------------------------------------------

    protected virtual async Task ReloadAndSelectAsync(TDomain? saved)
    {
        var result = await LoadDataAsync();
        SetItemsLevel1(result);
        OnPropertyChanged(nameof(ItemsLevel1));
        DataSetCount = ItemsLevel1.Count;

        if (saved != null)
        {
            SelectedItemLevel1 = ItemsLevel1.FirstOrDefault(x => GetId(x) == GetId(saved));
        }
        else
        {
            SelectedItemLevel1 = ItemsLevel1.FirstOrDefault();
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

        _previousSelectedItem = SelectedItemLevel1;

        var result = await SearchItemsAsync(_lastSearchText);

        SetItemsLevel1(result);
        DataSetCount = ItemsLevel1.Count;

        if (_previousSelectedItem != null)
        {
            var match = ItemsLevel1.FirstOrDefault(x => GetId(x) == GetId(_previousSelectedItem));
            SelectedItemLevel1 = match ?? ItemsLevel1.FirstOrDefault();
        }
        else
        {
            SelectedItemLevel1 = ItemsLevel1.FirstOrDefault();
        }

        EditItem = SelectedItemLevel1 != null
            ? ConvertDomainToEdit(SelectedItemLevel1)
            : null;

        OnPropertyChanged(nameof(ItemsLevel1));
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
            ? ItemsLevel1.OrderBy(x => prop.GetValue(x) ?? "").ToList()
            : ItemsLevel1.OrderByDescending(x => prop.GetValue(x) ?? "").ToList();

        _itemsLevel1Internal.Clear();
        foreach (var item in sorted)
            _itemsLevel1Internal.Add(item);
    }

    #endregion
}




