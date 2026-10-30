using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl24Subclass;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl21ClassViewModel Skriptdatum:  26.08.2026   18:32    

namespace ATIS.Views.Modules.Tbl21Class;

public class Tbl21ClassViewModel : BaseBossListViewModel<Models.Tbl21Class>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl21Class>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel ClassCommentViewModel { get; }
    public Tbl90ReferenceViewModel ClassReferenceViewModel { get; }
    public Tbl24SubclassViewModel SubclassViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl18Superclass> SuperclassList { get; private set; } = new();
    public Dictionary<int, string?> SuperclassLookup { get; private set; } = new();

    public async Task LoadSuperclassLookupAsync()
    {
        var list = await _lookupService.GetSuperclassListAsync();
        var lookup = await _lookupService.GetSuperclassLookupAsync();

        SuperclassList.Clear();
        foreach (var item in list)
        {
            SuperclassList.Add(item);
        }

        SuperclassLookup = lookup;
        OnPropertyChanged(nameof(SuperclassLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl21ClassViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        ClassCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        ClassReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SubclassViewModel = new Tbl24SubclassViewModel(factory, dialog, coordinator, lookupService, user);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_ClassId")) ? "Id" : loc.GetLocalizedString("HeaderSort_ClassId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_ClassName")) ? "Name" : loc.GetLocalizedString("HeaderSort_ClassName"));
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
            _ = ClassReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailTabIndex == 5)
            _ = SubclassViewModel.SubclassReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsSubclassParentSelected => SubclassViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadSuperclassLookupAsync();

        Items = new ObservableCollection<Models.Tbl21Class>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForSuperclassAsync(int superclassId)
        => await LoadInternalAsync(superclassId, Data.GetClassForSuperclassAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl21Class>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl21Class>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl21Class>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchClassWithSuperclassAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl21Class>();
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
        await ClassCommentViewModel.LoadForClassAsync(entityId);
        await ClassReferenceViewModel.LoadForClassAsync(entityId);

        await SubclassViewModel.LoadClassLookupAsync();
        await SubclassViewModel.LoadForClassAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        SubclassViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        ClassCommentViewModel.Items?.Clear();
        ClassReferenceViewModel.Items?.Clear();
        SubclassViewModel.Items?.Clear();

        if (IsBossMode)
        {
            ClassCommentViewModel.SelectedItem = null;
            ClassReferenceViewModel.SelectedItem = null;
            SubclassViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl21Class item, string column)
        => column switch
        {
            "Id" => item.ClassId,
            "Name" => item.ClassName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl21Class CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl21Class
        {
            SuperclassId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl21Class?> SaveItemAsync(Models.Tbl21Class item)
    {
        if (string.IsNullOrWhiteSpace(item.ClassName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertClassAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl21Class item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.ClassName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl21Class item)
    {
        if (item.ClassId == 0)
            return false;

        if (await Data.ClassHasSubclassAsync(item.ClassId) ||
            await Data.ClassHasReferenceAsync(item.ClassId) ||
            await Data.ClassHasCommentAsync(item.ClassId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteClassAsync(item.ClassId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl21Class item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl21Class item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl21Class item)
        => item.ClassId;

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
