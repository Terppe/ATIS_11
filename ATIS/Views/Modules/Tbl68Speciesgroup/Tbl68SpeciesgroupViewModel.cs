using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl68SpeciesgroupViewModel Skriptdatum:  10.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl68Speciesgroup;

public class Tbl68SpeciesgroupViewModel : BaseBossListViewModel<Models.Tbl68Speciesgroup>
{
    public ICommand SortCommand { get; }
    private bool _wasEmptyBeforeSave;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl68SpeciesgroupViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SpeciesgroupId")) ? "Id" : loc.GetLocalizedString("HeaderSort_SpeciesgroupId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SpeciesgroupName")) ? "Name" : loc.GetLocalizedString("HeaderSort_SpeciesgroupName"));
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

        Items = new ObservableCollection<Models.Tbl68Speciesgroup>();

        DataSetCount = Items.Count;

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl68Speciesgroup>> LoadItemsAsync()
    {
        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSpeciesgroupAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl68Speciesgroup>();
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
    protected override object GetSortKey(Models.Tbl68Speciesgroup item, string column)
        => column switch
        {
            "Id" => item.SpeciesgroupId,
            "Name" => item.SpeciesgroupName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl68Speciesgroup CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl68Speciesgroup
        {
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl68Speciesgroup?> SaveItemAsync(Models.Tbl68Speciesgroup item)
    {
        if (string.IsNullOrWhiteSpace(item.SpeciesgroupName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSpeciesgroupAsync(item);

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl68Speciesgroup item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.SpeciesgroupName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl68Speciesgroup item)
    {
        if (item.SpeciesgroupId == 0)
            return false;

        await Data.DeleteSpeciesgroupAsync(item.SpeciesgroupId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl68Speciesgroup item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl68Speciesgroup item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl68Speciesgroup item)
        => item.SpeciesgroupId;

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
