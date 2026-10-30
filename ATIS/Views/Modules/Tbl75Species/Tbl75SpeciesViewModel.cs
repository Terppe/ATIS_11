using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl68Speciesgroup;
using ATIS.Views.Modules.Tbl78Name;
using ATIS.Views.Modules.Tbl81Image;
using ATIS.Views.Modules.Tbl84Synonym;
using ATIS.Views.Modules.Tbl87Geographic;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

//    Tbl75SpeciesViewModel Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public class Tbl75SpeciesViewModel : BaseBossListViewModel<Models.Tbl75Species>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl75Species>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel SpeciesCommentViewModel { get; }
    public Tbl90ReferenceViewModel SpeciesReferenceViewModel { get; }
    public Tbl68SpeciesgroupViewModel SpeciesgroupViewModel { get; }
    public Tbl78NameViewModel NameViewModel { get; }
    public Tbl81ImageViewModel ImageViewModel { get; }
    public Tbl84SynonymViewModel SynonymViewModel { get; }
    public Tbl87GeographicViewModel GeographicViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl66Genus> GenusList { get; private set; } = new();
    public Dictionary<int, string?> GenusLookup { get; private set; } = new();
    //---------------------------------------------------------
    public ObservableCollection<Models.Tbl68Speciesgroup> SpeciesgroupList { get; private set; } = new();
    public Dictionary<int, string?> SpeciesgroupLookup { get; private set; } = new();
    //---------------------------------------------------------

    public async Task LoadGenusLookupAsync()
    {
        var list = await _lookupService.GetGenusListAsync();
        var lookup = await _lookupService.GetGenusLookupAsync();

        GenusList.Clear();
        foreach (var item in list)
        {
            GenusList.Add(item);
        }

        GenusLookup = lookup;
        OnPropertyChanged(nameof(GenusLookup));
    }
    //---------------------------------------------------------
    public async Task LoadSpeciesgroupLookupAsync()
    {
        var list = await _lookupService.GetSpeciesgroupListAsync();
        var lookup = await _lookupService.GetSpeciesgroupLookupAsync();

        SpeciesgroupList.Clear();
        foreach (var item in list)
        {
            SpeciesgroupList.Add(item);
        }

        SpeciesgroupLookup = lookup;
        OnPropertyChanged(nameof(SpeciesgroupLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl75SpeciesViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SpeciesCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SpeciesReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SpeciesgroupViewModel = new Tbl68SpeciesgroupViewModel(factory, dialog, coordinator, lookupService, user);
        NameViewModel = new Tbl78NameViewModel(factory, dialog, coordinator, lookupService, user);
        ImageViewModel = new Tbl81ImageViewModel(factory, dialog, coordinator, lookupService, user);
        SynonymViewModel = new Tbl84SynonymViewModel(factory, dialog, coordinator, lookupService, user);
        GeographicViewModel = new Tbl87GeographicViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

        RegisterHeader("Id", "Species ID");
        RegisterHeader("Name", "Species Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();

        ImageViewModel.PropertyChanged += ImageViewModel_PropertyChanged;
    }

    // ---------------------------------------------------------                  
    // ImageViewModel_PropertyChanged
    // ---------------------------------------------------------    
    private void ImageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl81ImageViewModel.ShowBusyOverlay))
        {
            OnPropertyChanged(nameof(ShowImageTabLoadingOverlay));
        }
    }

    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SpeciesId")) ? "Id" : loc.GetLocalizedString("HeaderSort_SpeciesId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SpeciesName")) ? "Name" : loc.GetLocalizedString("HeaderSort_SpeciesName"));
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

    protected override void OnSelectedDetailTabIndexChanged()
    {
        UpdateButtonStates();
        OnPropertyChanged(nameof(ShowImageTabLoadingOverlay));
        _ = HandleSelectedTabChangedAsync(SelectedDetailTabIndex);
    }

    private int _loadedImageSpeciesId;
    private async Task HandleSelectedTabChangedAsync(int tabIndex)
    {
        if (SelectedItem == null)
            return;

        if (tabIndex == 4)
        {
            var speciesId = GetId(SelectedItem);

            if (_loadedImageSpeciesId != speciesId)
            {
                await ImageViewModel.LoadSpeciesLookupAsync();
                await ImageViewModel.LoadForSpeciesAsync(speciesId);
                ImageViewModel.IsInParentMode = true;

                _loadedImageSpeciesId = speciesId;
            }
        }
    }
    private void UpdateButtonStates()
    {
        // Tab 0 = Boss-Modus
        IsInParentMode = SelectedDetailTabIndex != 0;

        if (SelectedDetailTabIndex == 2)
            _ = SpeciesReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsNameParentSelected => NameViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadGenusLookupAsync();
        await LoadSpeciesgroupLookupAsync();

        Items = new ObservableCollection<Models.Tbl75Species>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForGenusAsync(int genusId)
        => await LoadInternalAsync(genusId, Data.GetSpeciesForGenusAsync);
    public async Task LoadForSpeciesgroupAsync(int speciesgroupId)
        => await LoadInternalAsync(speciesgroupId, Data.GetSpeciesForSpeciesgroupAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl75Species>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl75Species>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl75Species>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.GetSpeciesWithGenusAndSpeciesgroupByIdAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl75Species>();
    }

    // ---------------------------------------------------------
    // Image_Video Loading Overlay
    // ---------------------------------------------------------

    public bool ShowImageTabLoadingOverlay =>
        SelectedDetailTabIndex == 4 && ImageViewModel?.ShowBusyOverlay == true;

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        // 1. BACK → SelectedItem == null
        if (SelectedItem == null)
        {
            OnPropertyChanged(nameof(IsDetailVisible));
            await ClearChildCollectionsAsync();
            SelectedDetailTabIndex = 0;
            _loadedImageSpeciesId = 0;
            return;
        }

        _loadedImageSpeciesId = 0;

        // 2. Parent-Modus → Kinder laden, aber KEIN UI-Rebuild
        if (IsInParentMode)
        {
            await LoadChildrenAsync(GetId(SelectedItem));
            return;
        }

        // 3. Boss-Modus → volle Detail-Logik
        OnPropertyChanged(nameof(IsDetailVisible));
        await LoadChildrenAsync(GetId(SelectedItem));
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await SpeciesCommentViewModel.LoadForSpeciesAsync(entityId);
        await SpeciesReferenceViewModel.LoadForSpeciesAsync(entityId);

        await NameViewModel.LoadSpeciesLookupAsync();
        await NameViewModel.LoadForSpeciesAsync(entityId);
        NameViewModel.IsInParentMode = true;

        //seperate loading 
        //await ImageViewModel.LoadSpeciesLookupAsync();
        //await ImageViewModel.LoadForSpeciesAsync(entityId);
        //ImageViewModel.IsInParentMode = true;

        await SynonymViewModel.LoadSpeciesLookupAsync();
        await SynonymViewModel.LoadForSpeciesAsync(entityId);
        SynonymViewModel.IsInParentMode = true;

        await GeographicViewModel.LoadSpeciesLookupAsync();
        await GeographicViewModel.LoadForSpeciesAsync(entityId);
        GeographicViewModel.IsInParentMode = true;

        SpeciesgroupViewModel.IsInParentMode = true; // nur falls wirklich verwendet
    }

    protected override Task ClearChildCollectionsAsync()
    {
        SpeciesCommentViewModel.Items?.Clear();
        SpeciesReferenceViewModel.Items?.Clear();
        NameViewModel.Items?.Clear();
        ImageViewModel.Items?.Clear();
        SynonymViewModel.Items?.Clear();
        GeographicViewModel.Items?.Clear();

        _loadedImageSpeciesId = 0;

        if (IsBossMode)
        {
            SpeciesCommentViewModel.SelectedItem = null;
            SpeciesReferenceViewModel.SelectedItem = null;
            NameViewModel.SelectedItem = null;
            ImageViewModel.SelectedItem = null;
            SynonymViewModel.SelectedItem = null;
            GeographicViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl75Species item, string column)
        => column switch
        {
            "Id" => item.SpeciesId,
            "Name" => item.SpeciesName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl75Species CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl75Species
        {
            GenusId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl75Species?> SaveItemAsync(Models.Tbl75Species item)
    {
        if (string.IsNullOrWhiteSpace(item.SpeciesName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSpeciesAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl75Species item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.SpeciesName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl75Species item)
    {
        if (item.SpeciesId == 0)
            return false;

        if (await Data.SpeciesHasNameAsync(item.SpeciesId) ||
            await Data.SpeciesHasImageAsync(item.SpeciesId) ||
            await Data.SpeciesHasSynonymAsync(item.SpeciesId) ||
            await Data.SpeciesHasGeographicAsync(item.SpeciesId) ||
            await Data.SpeciesHasReferenceAsync(item.SpeciesId) ||
            await Data.SpeciesHasCommentAsync(item.SpeciesId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSpeciesAsync(item.SpeciesId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl75Species item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl75Species item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl75Species item)
        => item.SpeciesId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        // ⭐ Back MUSS SelectedItem löschen
        SelectedItem = null;

        // ⭐ Detail schließen
        OnPropertyChanged(nameof(IsDetailVisible));

        // Parent-Modus bleibt aktiv
        IsInParentMode = true;

        SelectedDetailTabIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
