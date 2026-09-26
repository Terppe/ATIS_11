using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

//    Tbl78NameViewModel Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl78Name;

public class Tbl78NameViewModel : BaseBossListViewModel<Models.Tbl78Name>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }
    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl78Name>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl78NameViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

        _lookupService = lookupService;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();
    }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl75Species> SpeciesList { get; private set; } = new();
    public Dictionary<int, string?> SpeciesLookup { get; private set; } = new();

    public async Task LoadSpeciesLookupAsync()
    {
        try
        {
            var list = await _lookupService.GetSpeciesListAsync();
            var lookup = await _lookupService.GetSpeciesLookupAsync();

            SpeciesList.Clear();
            foreach (var item in list)
                SpeciesList.Add(item);

            SpeciesLookup = lookup;
            OnPropertyChanged(nameof(SpeciesLookup));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadSpeciesLookupAsync] {ex}");

            SpeciesList.Clear();
            SpeciesLookup = new Dictionary<int, string?>();
            OnPropertyChanged(nameof(SpeciesLookup));
        }
    }


    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_NameId")) ? "Id" : loc.GetLocalizedString("HeaderSort_NameId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_NameName")) ? "Name" : loc.GetLocalizedString("HeaderSort_NameName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------                   

    public ICommand SelectDetailTabCommand => new CommunityToolkit.Mvvm.Input.RelayCommand<int>(index =>
    {
        SelectedDetailTabIndex = index;
    });

    private int _selectedDetailTabIndex;
    public int SelectedDetailTabIndex
    {
        get => _selectedDetailTabIndex;
        set
        {
            if (SetProperty(ref _selectedDetailTabIndex, value))
            {
                _ = UpdateButtonStatesAsync();
            }
        }
    }

    private async Task UpdateButtonStatesAsync()
    {
        UpdateRefreshState();

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    public async Task ApplyFilterAsync(string? filter)
    {
        if (IsBossMode)
        {
            SearchText = filter;
            await ReloadAsync();
        }
    }


    public bool IsDetailVisible => SelectedItem != null;
    public bool IsEditMode => SelectedItem != null;
    public bool HasItems => Items != null && Items.Count > 0;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        // Für Simple-Page sinnvoll: Boss-Modus aktiv
        IsInParentMode = false;

        // Lookup nur versuchen, Fehler darf UI nicht stoppen
        try
        {
            await LoadSpeciesLookupAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> LoadSpeciesLookupAsync] {ex}");
            // absichtlich weiterlaufen
        }

        Items = new ObservableCollection<Models.Tbl78Name>();
        OnPropertyChanged(nameof(HasItems));

        // Hauptdaten trotzdem laden
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> ReloadAsync] {ex}");
            // optional: Dialog zeigen
             await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
    }
    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForSpeciesAsync(int speciesId)
        => await LoadInternalAsync(speciesId, Data.GetNameForSpeciesAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl78Name>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl78Name>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;

        // Liste anzeigen, Editor erst nach Auswahl eines Eintrags.
        SelectedItem = null;
        Item = null;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl78Name>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchNameWithSpeciesAndGenusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl78Name>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override Task OnSelectedItemChangedAsync()
    {
        // Das EditControl arbeitet mit Item,
        // die Liste mit SelectedItem.
        Item = SelectedItem;
        // Tbl78Name hat keine eigenen Children.

        return Task.CompletedTask;
    }

    //protected override async Task OnSelectedItemChangedAsync()
    //{
    //    // 1. BACK → SelectedItem == null
    //    if (SelectedItem == null)
    //    {
    //        OnPropertyChanged(nameof(IsDetailVisible));
    //        await ClearChildCollectionsAsync();
    //        SelectedDetailTabIndex = 0;
    //        return;
    //    }

    //    // 2. Parent-Modus → Kinder laden, aber KEIN UI-Rebuild
    //    if (IsInParentMode)
    //    {
    //        await LoadChildrenAsync(GetId(SelectedItem));
    //        return;
    //    }

    //    // 3. Boss-Modus → volle Detail-Logik
    //    OnPropertyChanged(nameof(IsDetailVisible));
    //    await LoadChildrenAsync(GetId(SelectedItem));
    //}

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override Task LoadChildrenAsync(int entityId)
    {
        return Task.CompletedTask;
    }
    protected override Task ClearChildCollectionsAsync()
    {
        return Task.CompletedTask;
    }
    private Models.Tbl78Name? _item;

    public Models.Tbl78Name? Item
    {
        get => _item;
        set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasActiveItem));
            }
        }
    }

    public bool HasActiveItem => Item != null;
    public void PrepareViewState()
    {
        // Keine Auswahl: EditControl schließen
        if (SelectedItem == null)
        {
            Item = null;
            SelectedDetailTabIndex = 0;
            OnPropertyChanged(nameof(IsDetailVisible));
        }
        else
        {
            // Falls bereits ein Datensatz ausgewählt ist,
            // EditControl mit diesem Datensatz synchronisieren.
            Item = SelectedItem;
        }

        UpdateRefreshState();
        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
    //public void PrepareViewState()
    //{
    //    // Bei null Auswahl auf Edit-Tab zurück
    //    if (SelectedItem == null)
    //    {
    //        SelectedDetailTabIndex = 0;
    //        OnPropertyChanged(nameof(IsDetailVisible));
    //    }

    //    // Commands/UI-State auffrischen
    //    UpdateRefreshState();
    //    NewCommand.NotifyCanExecuteChanged();
    //    RefreshCommand.NotifyCanExecuteChanged();
    //}
    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl78Name item, string column)
        => column switch
        {
            "Id" => item.NameId,
            "Name" => item.NameName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl78Name CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl78Name
        {
            SpeciesId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl78Name?> SaveItemAsync(Models.Tbl78Name item)
    {
        if (string.IsNullOrWhiteSpace(item.NameName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertNameAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl78Name item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.NameName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl78Name item)
    {
        if (item.NameId == 0)
            return false;

        await Data.DeleteNameAsync(item.NameId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl78Name item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl78Name item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl78Name item)
        => item.NameId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        // SelectedItem zurücksetzen -> Detail schließen
        SelectedItem = null;
        OnPropertyChanged(nameof(IsDetailVisible));

        // Bei Simple-Page NICHT hart auf true setzen
        // IsInParentMode = true;

        SelectedDetailTabIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
