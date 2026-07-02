using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using ATIS.Views.Report;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl66Genus;

public class Tbl66GenusViewModel : BaseBossListViewModel<Models.Tbl66Genus>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.Tbl66Genus>>>? _activeLoader;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel GenusCommentViewModel { get; }
    public Tbl90ReferenceViewModel GenusReferenceViewModel { get; }
    public Tbl75SpeciesViewModel SpeciesViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public ObservableCollection<Models.Tbl63Infratribus> InfratribusList { get; private set; } = new();

    public async Task LoadInfratribusLookupAsync()
    {
        InfratribusList = await _lookupService.GetInfratribusListAsync();
    }

    // ---------------------------------------------------------
    // CTOR
    // ---------------------------------------------------------

    public Tbl66GenusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        GenusCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        GenusReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SpeciesViewModel = new Tbl75SpeciesViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
      //  BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Genus ID");
        RegisterHeader("Name", "Genus Name");
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
        IsInParentMode = SelectedDetailPivotIndex != 0;

        if (SelectedDetailPivotIndex == 2)
            _ = GenusReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailPivotIndex == 5)
            _ = SpeciesViewModel.SpeciesReferenceViewModel.LoadReferenceLookupsAsync();

        UpdateRefreshState();
        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
    public async Task ApplyFilterAsync(string? filter)
    {
        SearchText = filter;
        await ReloadAsync();
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    //public override async Task LoadLookupsAsync()
    //{
    //    await LoadInfratribusLookupAsync();
    //}

    // ---------------------------------------------------------
    // BOSS LOAD
    // ---------------------------------------------------------

    public  async Task LoadAsync()
    {
        _currentParentId = 0;
        _activeLoader = null;

        await LoadInfratribusLookupAsync();

        SelectedItem = null;
          //  IsDetailVisible = false;
    }

    // ---------------------------------------------------------
    // LOAD FOR INFRATRIBUS (Parent)
    // ---------------------------------------------------------

    public async Task LoadForInfratribusAsync(int infratribusId)
        => await LoadInternalAsync(infratribusId, Data.GetGenusForInfratribusAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (universell)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl66Genus>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl66Genus>(items);

        DataSetCount = Items.Count;

        if (Items.Any())
        {
            SelectedItem = Items.First();
          //  IsDetailVisible = true;
        }
        else
        {
            SelectedItem = null;
          //  IsDetailVisible = false;
        }
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl66Genus>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchGenusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl66Genus>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------

    protected override async Task OnSelectedItemChangedAsync()
    {
        //  IsDetailVisible = SelectedItem != null;

        if (SelectedItem != null)
            await LoadChildrenAsync(SelectedItem.GenusId);
        else
            await ClearChildCollectionsAsync();
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------

    protected override async Task LoadChildrenAsync(int genusId)
    {
        await GenusCommentViewModel.LoadForGenusAsync(genusId);
        await GenusReferenceViewModel.LoadForGenusAsync(genusId);

        await SpeciesViewModel.LoadGenusLookupAsync();
        await SpeciesViewModel.LoadForGenusAsync(genusId);
    }

    protected override Task ClearChildCollectionsAsync()
    {
        GenusCommentViewModel.Items?.Clear();
        GenusReferenceViewModel.Items?.Clear();
        SpeciesViewModel.Items?.Clear();

        GenusCommentViewModel.SelectedItem = null;
        GenusReferenceViewModel.SelectedItem = null;
        SpeciesViewModel.SelectedItem = null;

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Models.Tbl66Genus CreateNewItem()
        => new() { InfratribusId = _currentParentId };

    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        //  IsDetailVisible = true;
        await Task.Delay(50);
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl66Genus?> SaveItemAsync(Models.Tbl66Genus item)
    {
        if (string.IsNullOrWhiteSpace(item.GenusName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertGenusAsync(item);

        if (_activeLoader != null)
        {
            var list = await _activeLoader(_currentParentId);
            Items = new ObservableCollection<Models.Tbl66Genus>(list);
        }

        SelectedItem = Items.FirstOrDefault(x => x.GenusId == saved.GenusId);
        SearchText = saved.GenusName;

        return saved;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    {
        if (item.GenusId == 0)
            return false;

        if (await Data.GenusHasSpeciesAsync(item.GenusId) ||
            await Data.GenusHasReferenceAsync(item.GenusId) ||
            await Data.GenusHasCommentAsync(item.GenusId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteGenusAsync(item.GenusId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl66Genus item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl66Genus item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl66Genus item)
        => item.GenusId;


    // ---------------------------------------------------------
    // Sorting (in-memory)
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl66Genus item, string column)
        => column switch
        {
            "Id" => item.GenusId,
            "Name" => item.GenusName ?? "",
            _ => ""
        };

}


