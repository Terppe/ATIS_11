using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

//    Tbl90RefExpertViewModel Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefExpert;

public class Tbl90RefExpertViewModel : BaseBossListViewModel<Models.Tbl90RefExpert>
{
    public ICommand SortCommand { get; }
    private bool _wasEmptyBeforeSave;
    private readonly Action<string> _langHandler;

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_RefExpertId")) ? "Id" : loc.GetLocalizedString("HeaderSort_RefExpertId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_RefExpertName")) ? "Name" : loc.GetLocalizedString("HeaderSort_RefExpertName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------               

    protected override void OnSelectedDetailTabIndexChanged()
    {
        UpdateButtonStates();
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


    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        Items = new ObservableCollection<Models.Tbl90RefExpert>();
        DataSetCount = Items.Count;

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
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
           // RefExpertId = _currentParentId
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

    protected override int GetId(Models.Tbl90RefExpert item)
        => item.RefExpertId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------

    protected override void ExecuteBack()
    {
        // ⭐ Back MUSS SelectedItem löschen
        SelectedItem = null;

        // ⭐ Detail schließen
        OnPropertyChanged(nameof(IsDetailVisible));

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }



}
