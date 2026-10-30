using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl81Image;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

namespace ATIS.Views.Modules.TblMiscellaneous;

public class TblMiscellaneousViewModel : BaseBossListViewModel<Models.TblMiscellaneous>
{
    private readonly LookupService _lookupService;
    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.TblMiscellaneous>>>? _activeLoader;

    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }
    private bool _wasEmptyBeforeSave;
    private readonly Action<string> _langHandler;
    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl81ImageViewModel ImageViewModel { get; }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public TblMiscellaneousViewModel(
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

        ImageViewModel = new Tbl81ImageViewModel(factory, dialog, coordinator, lookupService, user);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_MiscellaneousId")) ? "Id" : loc.GetLocalizedString("HeaderSort_MiscellaneousId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_MiscellaneousName")) ? "Name" : loc.GetLocalizedString("HeaderSort_MiscellaneousName"));
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

    private int _loadedImageMiscellaneousId;
    private async Task HandleSelectedTabChangedAsync(int tabIndex)
    {
        if (SelectedItem == null)
            return;

        if (tabIndex == 1)
        {
            var miscellaneousId = GetId(SelectedItem);

            if (_loadedImageMiscellaneousId != miscellaneousId)
            {
                await ImageViewModel.LoadForMiscellaneousAsync(miscellaneousId);
                ImageViewModel.IsInParentMode = true;

                _loadedImageMiscellaneousId = miscellaneousId;
            }
        }
    }

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    private void UpdateButtonStates()
    {
        // Tab 0 = Boss-Modus
        IsInParentMode = SelectedDetailTabIndex != 0;

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


    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        //await LoadGenusLookupAsync();
        //await LoadSpeciesgroupLookupAsync();

        Items = new ObservableCollection<Models.TblMiscellaneous>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }


    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.TblMiscellaneous>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchMiscellaneousAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.TblMiscellaneous>();
    }
    // ---------------------------------------------------------
    // Image_Video Loading Overlay
    // ---------------------------------------------------------

    public bool ShowImageTabLoadingOverlay =>
        SelectedDetailTabIndex == 1 && ImageViewModel?.ShowBusyOverlay == true;

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
            _loadedImageMiscellaneousId = 0;
            return;
        }

        _loadedImageMiscellaneousId = 0;

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
        //   await ImageViewModel.LoadSpeciesLookupAsync();
        await ImageViewModel.LoadForMiscellaneousAsync(entityId);
        ImageViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        ImageViewModel.Items?.Clear();

        _loadedImageMiscellaneousId = 0;

        if (IsBossMode)
        {
            ImageViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.TblMiscellaneous item, string column)
        => column switch
        {
            "Id" => item.MiscellaneousId,
            "Name" => item.MiscellaneousName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.TblMiscellaneous CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.TblMiscellaneous
        {
            MiscellaneousId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.TblMiscellaneous?> SaveItemAsync(Models.TblMiscellaneous item)
    {
        if (string.IsNullOrWhiteSpace(item.ShortInfo))
        {
            await Dialog.ShowErrorAsync("Error_MissingShortInfo_Title", "Error_MissingShortInfo_Message");
            return null;
        }

        if (string.IsNullOrWhiteSpace(item.MiscellaneousName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }


        var saved = await Data.UpsertMiscellaneousAsync(item);

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.TblMiscellaneous item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.MiscellaneousName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.TblMiscellaneous item)
    {
        if (item.MiscellaneousId == 0)
            return false;

        await Data.DeleteMiscellaneousAsync(item.MiscellaneousId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.TblMiscellaneous item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.TblMiscellaneous item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.TblMiscellaneous item)
        => item.MiscellaneousId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------

    private void ExecuteBack()
    {
        // ⭐ Back MUSS SelectedItem löschen
        SelectedItem = null;

        // ⭐ Detail schließen
        OnPropertyChanged(nameof(IsDetailVisible));

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }



}
