using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;


//    Tbl90RefExpertViewModel Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl90RefExpert;

public class Tbl90RefExpertViewModel : BaseBossListViewModel<Models.Tbl90RefExpert>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl90RefExpertViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "RefExpert ID");
        RegisterHeader("Name", "RefExpert Name");
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
        _currentParentId = 0;

        Items = new ObservableCollection<Models.Tbl90RefExpert>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl90RefExpert>>> loader)
    {
        _currentParentId = parentId;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl90RefExpert>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl90RefExpert>> LoadItemsAsync()
    {

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchRefExpertAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl90RefExpert>();
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
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.Tbl90RefExpert item, string column)
        => column switch
        {
            "Id" => item.RefExpertId,
            "Name" => item.RefExpertName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl90RefExpert CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl90RefExpert
        {
            RefExpertId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl90RefExpert?> SaveItemAsync(Models.Tbl90RefExpert item)
    {
        if (string.IsNullOrWhiteSpace(item.RefExpertName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertRefExpertAsync(item);
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl90RefExpert item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.RefExpertName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl90RefExpert item)
    {
        if (item.RefExpertId == 0)
            return false;

        await Data.DeleteRefExpertAsync(item.RefExpertId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90RefExpert item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90RefExpert item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl90RefExpert item)
        => item.RefExpertId;

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
