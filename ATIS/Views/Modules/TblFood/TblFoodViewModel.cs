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

namespace ATIS.Views.Modules.TblFood;

public class TblFoodViewModel : BaseBossListViewModel<Models.TblFood>
{
    private readonly LookupService _lookupService;
    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.TblFood>>>? _activeLoader;

    public ICommand SortCommand { get; }
    private bool _wasEmptyBeforeSave;
    private readonly Action<string> _langHandler;
    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl81ImageViewModel ImageViewModel { get; }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public TblFoodViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_FoodId")) ? "Id" : loc.GetLocalizedString("HeaderSort_FoodId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_FoodName")) ? "Name" : loc.GetLocalizedString("HeaderSort_FoodName"));
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

    private int _loadedImageFoodId;
    private async Task HandleSelectedTabChangedAsync(int tabIndex)
    {
        if (SelectedItem == null)
            return;

        if (tabIndex == 1)
        {
            var foodId = GetId(SelectedItem);

            if (_loadedImageFoodId != foodId)
            {
                await ImageViewModel.LoadForFoodAsync(foodId);
                ImageViewModel.IsInParentMode = true;

                _loadedImageFoodId = foodId;
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

        Items = new ObservableCollection<Models.TblFood>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }


    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.TblFood>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchFoodAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.TblFood>();
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
            _loadedImageFoodId = 0;
            return;
        }

        _loadedImageFoodId = 0;

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
        await ImageViewModel.LoadForFoodAsync(entityId);
        ImageViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        ImageViewModel.Items?.Clear();

        _loadedImageFoodId = 0;

        if (IsBossMode)
        {
            ImageViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.TblFood item, string column)
        => column switch
        {
            "Id" => item.FoodId,
            "Name" => item.FoodName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.TblFood CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.TblFood
        {
            FoodId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.TblFood?> SaveItemAsync(Models.TblFood item)
    {
        if (string.IsNullOrWhiteSpace(item.ShortInfo))
        {
            await Dialog.ShowErrorAsync("Error_MissingShortInfo_Title", "Error_MissingShortInfo_Message");
            return null;
        }

        if (string.IsNullOrWhiteSpace(item.FoodName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertFoodAsync(item);

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.TblFood item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.FoodName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.TblFood item)
    {
        if (item.FoodId == 0)
            return false;

        await Data.DeleteFoodAsync(item.FoodId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.TblFood item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.TblFood item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.TblFood item)
        => item.FoodId;

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
