using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl21Class;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl18SuperclassViewModel Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl18Superclass;

public class Tbl18SuperclassViewModel : BaseBossListViewModel<Models.Tbl18Superclass>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl18Superclass>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel SuperclassCommentViewModel { get; }
    public Tbl90ReferenceViewModel SuperclassReferenceViewModel { get; }
    public Tbl21ClassViewModel ClassViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public ObservableCollection<Models.Tbl12Subphylum> SubphylumList { get; private set; } = new();
    public Dictionary<int, string?> SubphylumLookup { get; private set; } = new();

    public async Task LoadSubphylumLookupAsync()
    {
        SubphylumList = await _lookupService.GetSubphylumListAsync();
        SubphylumLookup = await _lookupService.GetSubphylumLookupAsync();
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   

    public Tbl18SuperclassViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SuperclassCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SuperclassReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        ClassViewModel = new Tbl21ClassViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SuperclassId")) ? "Id" : loc.GetLocalizedString("HeaderSort_SuperclassId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SuperclassName")) ? "Name" : loc.GetLocalizedString("HeaderSort_SuperclassName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------               

    private int _selectedDetailPivotIndex;
    public int SelectedDetailPivotIndex
    {
        get => _selectedDetailPivotIndex;
        set
        {
            if (SetProperty(ref _selectedDetailPivotIndex, value))
            {
                UpdateButtonStates();
            }
        }
    }

    private void UpdateButtonStates()
    {
        // Pivot 0 = Boss-Modus
        IsInParentMode = SelectedDetailPivotIndex != 0;

        if (SelectedDetailPivotIndex == 2)
            _ = SuperclassReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailPivotIndex == 5)
            _ = ClassViewModel.ClassReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsClassParentSelected => ClassViewModel.SelectedItem != null;
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

        await LoadSubphylumLookupAsync();

        Items = new ObservableCollection<Models.Tbl18Superclass>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------

    public async Task LoadForSubphylumAsync(int subphylumId)
        => await LoadInternalAsync(subphylumId, Data.GetSuperclassForSubphylumAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl18Superclass>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl18Superclass>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl18Superclass>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSuperclassWithSubphylumAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl18Superclass>();
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
            SelectedDetailPivotIndex = 0;
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
        await SuperclassCommentViewModel.LoadForSuperclassAsync(entityId);
        await SuperclassReferenceViewModel.LoadForSuperclassAsync(entityId);

        await ClassViewModel.LoadSuperclassLookupAsync();
        await ClassViewModel.LoadForSuperclassAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        ClassViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        SuperclassCommentViewModel.Items?.Clear();
        SuperclassReferenceViewModel.Items?.Clear();
        ClassViewModel.Items?.Clear();

        if (IsBossMode)
        {
            SuperclassCommentViewModel.SelectedItem = null;
            SuperclassReferenceViewModel.SelectedItem = null;
            ClassViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.Tbl18Superclass item, string column)
        => column switch
        {
            "Id" => item.SuperclassId,
            "Name" => item.SuperclassName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl18Superclass CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl18Superclass
        {
            SubphylumId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl18Superclass?> SaveItemAsync(Models.Tbl18Superclass item)
    {
        if (string.IsNullOrWhiteSpace(item.SuperclassName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSuperclassAsync(item);
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl18Superclass item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.SuperclassName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl18Superclass item)
    {
        if (item.SuperclassId == 0)
            return false;

        if (await Data.SuperclassHasClassAsync(item.SuperclassId) ||
            await Data.SuperclassHasReferenceAsync(item.SuperclassId) ||
            await Data.SuperclassHasCommentAsync(item.SuperclassId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSuperclassAsync(item.SuperclassId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl18Superclass item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl18Superclass item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl18Superclass item)
        => item.SuperclassId;

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

        SelectedDetailPivotIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }



}
