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

namespace ATIS.Views.Modules.TblDisease;

public class TblDiseaseViewModel : BaseBossListViewModel<Models.TblDisease>
{
    private readonly LookupService _lookupService;
    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.TblDisease>>>? _activeLoader;

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
    public TblDiseaseViewModel(
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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_DiseaseId")) ? "Id" : loc.GetLocalizedString("HeaderSort_DiseaseId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_DiseaseName")) ? "Name" : loc.GetLocalizedString("HeaderSort_DiseaseName"));
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
                UpdateButtonStates();
                OnPropertyChanged(nameof(ShowImageTabLoadingOverlay));
                _ = HandleSelectedTabChangedAsync(value);
            }
        }
    }

    private int _loadedImageDiseaseId;
    private async Task HandleSelectedTabChangedAsync(int tabIndex)
    {
        if (SelectedItem == null)
            return;

        if (tabIndex == 1)
        {
            var diseaseId = GetId(SelectedItem);

            if (_loadedImageDiseaseId != diseaseId)
            {
                await ImageViewModel.LoadForDiseaseAsync(diseaseId);
                ImageViewModel.IsInParentMode = true;

                _loadedImageDiseaseId = diseaseId;
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

    public bool IsDetailVisible => SelectedItem != null;
    public bool IsEditMode => SelectedItem != null;
    public bool HasItems => Items != null && Items.Count > 0;

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

        Items = new ObservableCollection<Models.TblDisease>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }



    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.TblDisease>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchDiseaseAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.TblDisease>();
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
            _loadedImageDiseaseId = 0;
            return;
        }

        _loadedImageDiseaseId = 0;

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
        await ImageViewModel.LoadForDiseaseAsync(entityId);
        ImageViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        ImageViewModel.Items?.Clear();

        _loadedImageDiseaseId = 0;

        if (IsBossMode)
        {
            ImageViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.TblDisease item, string column)
        => column switch
        {
            "Id" => item.DiseaseId,
            "Name" => item.DiseaseName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.TblDisease CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.TblDisease
        {
            DiseaseId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.TblDisease?> SaveItemAsync(Models.TblDisease item)
    {
        if (string.IsNullOrWhiteSpace(item.ShortInfo))
        {
            await Dialog.ShowErrorAsync("Error_MissingShortInfo_Title", "Error_MissingShortInfo_Message");
            return null;
        }

        if (string.IsNullOrWhiteSpace(item.DiseaseName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertDiseaseAsync(item);

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.TblDisease item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.DiseaseName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.TblDisease item)
    {
        if (item.DiseaseId == 0)
            return false;

        await Data.DeleteDiseaseAsync(item.DiseaseId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.TblDisease item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.TblDisease item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.TblDisease item)
        => item.DiseaseId;

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
