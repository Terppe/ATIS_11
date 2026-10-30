using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl12Subphylum;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl06PhylumViewModel Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl06Phylum;

public class Tbl06PhylumViewModel : BaseBossListViewModel<Models.Tbl06Phylum>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl06Phylum>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel PhylumCommentViewModel { get; }
    public Tbl90ReferenceViewModel PhylumReferenceViewModel { get; }
    public Tbl12SubphylumViewModel SubphylumViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl03Regnum> RegnumList { get; private set; } = new();
    public Dictionary<int, string?> RegnumLookup { get; private set; } = new();

    public async Task LoadRegnumLookupAsync()
    {
        var list = await _lookupService.GetRegnumListAsync();
        var lookup = await _lookupService.GetRegnumLookupAsync();

        RegnumList.Clear();
        foreach (var item in list)
        {
            RegnumList.Add(item);
        }

        RegnumLookup = lookup;
        OnPropertyChanged(nameof(RegnumLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl06PhylumViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        PhylumCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        PhylumReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SubphylumViewModel = new Tbl12SubphylumViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();
    }

    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_PhylumId")) ? "Id" : loc.GetLocalizedString("HeaderSort_PhylumId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_PhylumName")) ? "Name" : loc.GetLocalizedString("HeaderSort_PhylumName"));
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
    }

    private void UpdateButtonStates()
    {
        // Tab 0 = Boss-Modus
        IsInParentMode = SelectedDetailTabIndex != 0;

        if (SelectedDetailTabIndex == 2)
            _ = PhylumReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailTabIndex == 5)
            _ = SubphylumViewModel.SubphylumReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsSubphylumParentSelected => SubphylumViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadRegnumLookupAsync();

        Items = new ObservableCollection<Models.Tbl06Phylum>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForRegnumAsync(int regnumId)
        => await LoadInternalAsync(regnumId, Data.GetPhylumForRegnumAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl06Phylum>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl06Phylum>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl06Phylum>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchPhylumWithRegnumAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl06Phylum>();
    }

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
            return;
        }

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
        await PhylumCommentViewModel.LoadForPhylumAsync(entityId);
        await PhylumReferenceViewModel.LoadForPhylumAsync(entityId);

        await SubphylumViewModel.LoadPhylumLookupAsync();
        await SubphylumViewModel.LoadForPhylumAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        SubphylumViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        PhylumCommentViewModel.Items?.Clear();
        PhylumReferenceViewModel.Items?.Clear();
        SubphylumViewModel.Items?.Clear();

        if (IsBossMode)
        {
            PhylumCommentViewModel.SelectedItem = null;
            PhylumReferenceViewModel.SelectedItem = null;
            SubphylumViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl06Phylum item, string column)
        => column switch
        {
            "Id" => item.PhylumId,
            "Name" => item.PhylumName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl06Phylum CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl06Phylum
        {
            RegnumId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl06Phylum?> SaveItemAsync(Models.Tbl06Phylum item)
    {
        if (string.IsNullOrWhiteSpace(item.PhylumName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertPhylumAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl06Phylum item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.PhylumName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl06Phylum item)
    {
        if (item.PhylumId == 0)
            return false;

        if (await Data.PhylumHasSubphylumAsync(item.PhylumId) ||
            await Data.PhylumHasReferenceAsync(item.PhylumId) ||
            await Data.PhylumHasCommentAsync(item.PhylumId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeletePhylumAsync(item.PhylumId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl06Phylum item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl06Phylum item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl06Phylum item)
        => item.PhylumId;

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
