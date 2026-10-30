using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl60SubtribusViewModel Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl60Subtribus;

public class Tbl60SubtribusViewModel : BaseBossListViewModel<Models.Tbl60Subtribus>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl60Subtribus>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel SubtribusCommentViewModel { get; }
    public Tbl90ReferenceViewModel SubtribusReferenceViewModel { get; }
    public Tbl63InfratribusViewModel InfratribusViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl57Tribus> TribusList { get; private set; } = new();
    public Dictionary<int, string?> TribusLookup { get; private set; } = new();

    public async Task LoadTribusLookupAsync()
    {
        var list = await _lookupService.GetTribusListAsync();
        var lookup = await _lookupService.GetTribusLookupAsync();

        TribusList.Clear();
        foreach (var item in list)
        {
            TribusList.Add(item);
        }

        TribusLookup = lookup;
        OnPropertyChanged(nameof(TribusLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl60SubtribusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SubtribusCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SubtribusReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        InfratribusViewModel = new Tbl63InfratribusViewModel(factory, dialog, coordinator, lookupService, user);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SubtribusId")) ? "Id" : loc.GetLocalizedString("HeaderSort_SubtribusId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SubtribusName")) ? "Name" : loc.GetLocalizedString("HeaderSort_SubtribusName"));
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
            _ = SubtribusReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailTabIndex == 5)
            _ = InfratribusViewModel.InfratribusReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsInfratribusParentSelected => InfratribusViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadTribusLookupAsync();

        Items = new ObservableCollection<Models.Tbl60Subtribus>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForTribusAsync(int tribusId)
        => await LoadInternalAsync(tribusId, Data.GetSubtribusForTribusAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl60Subtribus>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSubtribusWithTribusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl60Subtribus>();
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
        await SubtribusCommentViewModel.LoadForSubtribusAsync(entityId);
        await SubtribusReferenceViewModel.LoadForSubtribusAsync(entityId);

        await InfratribusViewModel.LoadSubtribusLookupAsync();
        await InfratribusViewModel.LoadForSubtribusAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        InfratribusViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        SubtribusCommentViewModel.Items?.Clear();
        SubtribusReferenceViewModel.Items?.Clear();
        InfratribusViewModel.Items?.Clear();

        if (IsBossMode)
        {
            SubtribusCommentViewModel.SelectedItem = null;
            SubtribusReferenceViewModel.SelectedItem = null;
            InfratribusViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl60Subtribus item, string column)
        => column switch
        {
            "Id" => item.SubtribusId,
            "Name" => item.SubtribusName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl60Subtribus CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl60Subtribus
        {
            TribusId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
    {
        if (string.IsNullOrWhiteSpace(item.SubtribusName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSubtribusAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl60Subtribus item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.SubtribusName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
    {
        if (item.SubtribusId == 0)
            return false;

        if (await Data.SubtribusHasInfratribusAsync(item.SubtribusId) ||
            await Data.SubtribusHasReferenceAsync(item.SubtribusId) ||
            await Data.SubtribusHasCommentAsync(item.SubtribusId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSubtribusAsync(item.SubtribusId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl60Subtribus item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl60Subtribus item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl60Subtribus item)
        => item.SubtribusId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
}
