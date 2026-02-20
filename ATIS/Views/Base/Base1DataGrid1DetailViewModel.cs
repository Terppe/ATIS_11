using ATIS.Interfaces;

public abstract class Base1DataGrid1DetailViewModel<TDomain, TEdit>
    : ObservableObject
    where TDomain : class, ICopyable<TDomain>, new()
    where TEdit : class, new()
{
    private TDomain? _selectedItem;
    private TDomain? _backupItem;
    private TEdit? _editItem;

    private bool _isInEdit;
    private bool _isDetailVisible;

    protected bool IsNew { get; private set; }

    public bool IsInEdit
    {
        get => _isInEdit;
        private set
        {
            if (SetProperty(ref _isInEdit, value))
                UpdateCommands();
        }
    }

    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        private set => SetProperty(ref _isDetailVisible, value);
    }

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

    public TDomain? BackupItem
    {
        get => _backupItem;
        private set => SetProperty(ref _backupItem, value);
    }

    public TEdit? EditItem
    {
        get => _editItem;
        private set => SetProperty(ref _editItem, value);
    }

    public IEnumerable<TDomain>? Items { get; private set; }

    // Commands
    public IAsyncRelayCommand LoadCommand { get; }
    public IRelayCommand NewCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }

    protected Base1DataGrid1DetailViewModel()
    {
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        EditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StartEdit, () => SelectedItem != null);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => IsInEdit);
        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CancelEdit);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItem != null);
    }

    // Abstrakte Methoden
    protected abstract Task<IEnumerable<TDomain>> LoadItemsAsync();
    protected abstract TDomain CreateNewDomainModel();
    protected abstract TDomain CloneDomain(TDomain source);
    protected abstract TEdit ConvertDomainToEdit(TDomain domain);
    protected abstract void ConvertEditToDomain(TEdit edit, TDomain domain);
    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
    protected abstract Task DeleteDomainAsync(TDomain domain);
    protected abstract int GetId(TDomain model);
    protected abstract void OnBeforeSave(TDomain domain, bool isNew);

    // Laden
    private async Task LoadAsync()
    {
        Items = await LoadItemsAsync();
        OnPropertyChanged(nameof(Items));
    }

    // Neu
    private void NewItem()
    {
        var domain = CreateNewDomainModel();
        BackupItem = CloneDomain(domain);
        EditItem = ConvertDomainToEdit(BackupItem);
        SelectedItem = domain;

        IsNew = true;
        IsInEdit = true;
    }

    // Edit
    private void StartEdit()
    {
        if (SelectedItem == null)
            return;

        BackupItem = CloneDomain(SelectedItem);
        EditItem = ConvertDomainToEdit(BackupItem);

        IsNew = false;
        IsInEdit = true;
    }

    // Cancel
    private void CancelEdit()
    {
        if (BackupItem != null && SelectedItem != null)
        {
            SelectedItem.CopyFrom(BackupItem);
            EditItem = ConvertDomainToEdit(BackupItem);
        }

        IsInEdit = false;
    }

    // Save
    private async Task SaveAsync()
    {
        if (SelectedItem == null || EditItem == null)
            return;

        ConvertEditToDomain(EditItem, SelectedItem);

        OnBeforeSave(SelectedItem, IsNew);

        var saved = await SaveDomainAsync(SelectedItem);

        await ReloadAndSelectAsync(saved);

        IsInEdit = false;
    }

    // Delete
    private async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        await DeleteDomainAsync(SelectedItem);

        await ReloadAndSelectAsync(null);
    }

    // Reload + Re-Select
    protected virtual async Task ReloadAndSelectAsync(TDomain? saved)
    {
        Items = await LoadItemsAsync();
        OnPropertyChanged(nameof(Items));

        if (saved != null)
        {
            SelectedItem = Items.FirstOrDefault(x => GetId(x) == GetId(saved));
        }
        else
        {
            SelectedItem = Items.FirstOrDefault();
        }
    }

    protected virtual void UpdateCommands()
    {
        SaveCommand.NotifyCanExecuteChanged();
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        NewCommand.NotifyCanExecuteChanged();
    }
}
