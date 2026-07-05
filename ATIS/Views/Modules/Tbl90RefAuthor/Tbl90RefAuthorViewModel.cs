using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;


//    Tbl90RefAuthorViewModel Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public class Tbl90RefAuthorViewModel : BaseBossListViewModel<Models.Tbl90RefAuthor>
{
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
 //   private Func<int, Task<IEnumerable<Models.Tbl90RefAuthor>>>? _activeLoader;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   

    public Tbl90RefAuthorViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "RefAuthor ID");
        RegisterHeader("Name", "RefAuthor Name");
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

        Items = new ObservableCollection<Models.Tbl90RefAuthor>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl90RefAuthor>>> loader)
    {
        _currentParentId = parentId;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl90RefAuthor>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl90RefAuthor>> LoadItemsAsync()
    {
        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchRefAuthorAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl90RefAuthor>();
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
            return;
        }

        // 3. Boss-Modus → volle Detail-Logik
        OnPropertyChanged(nameof(IsDetailVisible));
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.Tbl90RefAuthor item, string column)
        => column switch
        {
            "Id" => item.RefAuthorId,
            "Name" => item.RefAuthorName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl90RefAuthor CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl90RefAuthor
        {
            RefAuthorId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl90RefAuthor?> SaveItemAsync(Models.Tbl90RefAuthor item)
    {
        if (string.IsNullOrWhiteSpace(item.RefAuthorName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertRefAuthorAsync(item);
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl90RefAuthor item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.RefAuthorName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl90RefAuthor item)
    {
        if (item.RefAuthorId == 0)
            return false;

        await Data.DeleteRefAuthorAsync(item.RefAuthorId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90RefAuthor item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90RefAuthor item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl90RefAuthor item)
        => item.RefAuthorId;

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

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

}
