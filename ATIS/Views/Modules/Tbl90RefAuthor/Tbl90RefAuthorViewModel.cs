using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;


using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

//    Tbl90RefAuthorViewModel Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public class Tbl90RefAuthorViewModel : BaseBossListViewModel<Models.Tbl90RefAuthor>
{

    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }
    private bool _wasEmptyBeforeSave;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl90RefAuthorViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_RefAuthorId")) ? "Id" : loc.GetLocalizedString("HeaderSort_RefAuthorId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_RefAuthorName")) ? "Name" : loc.GetLocalizedString("HeaderSort_RefAuthorName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------                   

    private int _selectedDetailTabIndex;
    public int SelectedDetailTabIndex
    {
        get => _selectedDetailTabIndex;
        set
        {
            if (SetProperty(ref _selectedDetailTabIndex, value))
            {
                UpdateButtonStates();
            }
        }
    }

    private void UpdateButtonStates()
    {


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

        Items = new ObservableCollection<Models.Tbl90RefAuthor>();
        DataSetCount = Items.Count;

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
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
        }
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override Task LoadChildrenAsync(int entityId)
    {
        return Task.CompletedTask;
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
            //  NULLId = _currentParentId
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

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
