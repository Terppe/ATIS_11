using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Base;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using CommunityToolkit.Mvvm.Input;

namespace ATIS.Views.Modules.Tbl66Genus;

public class Tbl66GenusViewModel : BaseListViewModel<Models.Tbl66Genus>
{
    private readonly IDialogService _dialog;

    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
    public Tbl93CommentViewModel CommentViewModel { get; }

    public Tbl66GenusViewModel(
        IDataService data,
        IDialogService dialog,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel
    ) : base(data)
    {
        _dialog = dialog;
        ReferenceViewModel = referenceViewModel;
        CommentViewModel = commentViewModel;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        OpenEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(OpenEdit);
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);
        NewCommand = new AsyncRelayCommand(NewAsync);
    }

    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }
    public IAsyncRelayCommand NewCommand { get; }

    // ---------------------------------------------------------
    // Lookup (Infratribus ComboBox)
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl63Infratribus> InfratribusList { get; } = new();
    public Dictionary<int, string?> InfratribusLookup { get; } = new();

    public async Task LoadInfratribusAsync()
    {
        InfratribusList.Clear();
        InfratribusLookup.Clear();

        var list = await _data.GetAllInfratribusAsync();
        foreach (var item in list)
        {
            InfratribusList.Add(item);
            InfratribusLookup[item.InfratribusId] = item.InfratribusName;
        }
    }

    // ---------------------------------------------------------
    // Load entrypoint
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        await LoadInfratribusAsync();

        if (string.IsNullOrWhiteSpace(CurrentFilter))
            CurrentFilter = "";
        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    public override async Task ApplyFilterAsync(string? filter)
    {
        if (SuppressFilter)
            return;

        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        await LoadInfratribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: false);
    }

    // ---------------------------------------------------------
    // FK Navigation (Infratribus → Genus)
    // ---------------------------------------------------------
    public int? CurrentInfratribusId { get; private set; }

    public async Task LoadForInfratribusAsync(int infratribusId)
    {
        CurrentInfratribusId = infratribusId;

        Items = new ObservableCollection<Models.Tbl66Genus>(
            await _data.GetGenusForInfratribusAsync(infratribusId));

        DataSetCount = Items.Count;

        IsGenusEditVisible = false;
        SelectItemSilently(null);
    }

    protected override async Task<IEnumerable<Models.Tbl66Genus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl66Genus>();

        return await _data.SearchGenusAsync(CurrentFilter);
    }

    // ---------------------------------------------------------
    // Selection → load children
    // ---------------------------------------------------------
    public override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem == null)
        {
            IsDetailVisible = false;
            IsGenusEditVisible = false;

            await CommentViewModel.LoadForGenusAsync(0);
            await ReferenceViewModel.LoadForGenusAsync(0);

            IsCommentEditVisible = false;
            IsReferenceEditVisible = false;
            return;
        }

        IsDetailVisible = true;
        IsGenusEditVisible = true;

        await CommentViewModel.LoadForGenusAsync(SelectedItem.GenusId);
        await ReferenceViewModel.LoadForGenusAsync(SelectedItem.GenusId);

        IsCommentEditVisible = false;
        IsReferenceEditVisible = false;
    }

    // ---------------------------------------------------------
    // Visibility Flags
    // ---------------------------------------------------------
    private string _searchText = string.Empty;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    private bool _isGenusEditVisible;
    public bool IsGenusEditVisible
    {
        get => _isGenusEditVisible;
        set => SetProperty(ref _isGenusEditVisible, value);
    }

    private bool _isCommentEditVisible;
    public bool IsCommentEditVisible
    {
        get => _isCommentEditVisible;
        set => SetProperty(ref _isCommentEditVisible, value);
    }

    private bool _isReferenceEditVisible;
    public bool IsReferenceEditVisible
    {
        get => _isReferenceEditVisible;
        set => SetProperty(ref _isReferenceEditVisible, value);
    }

    // ---------------------------------------------------------
    // Edit visibility
    // ---------------------------------------------------------
    private void OpenEdit()
    {
        if (SelectedItem != null)
            IsGenusEditVisible = true;
    }

    private void CloseEdit()
    {
        IsGenusEditVisible = false;
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // New
    // ---------------------------------------------------------
    protected override Models.Tbl66Genus CreateNewItem()
        => new()
        {
            GenusId = 0,
            InfratribusId = CurrentInfratribusId ?? 0,
            Writer = Environment.UserName,
            WriterDate = DateTime.Now
        };

    protected override void OnAfterNew(Models.Tbl66Genus item)
        => IsGenusEditVisible = true;

    protected override bool IsUnsavedNewItem(Models.Tbl66Genus item)
        => item.GenusId == 0;

    private async Task NewAsync()
    {
        if (CurrentInfratribusId <= 0)
        {
            await _dialog.ShowErrorAsync("Hinweis", "Bitte zuerst einen Infratribus auswählen.");
            return;
        }

        base.New();

        await CommentViewModel.LoadForGenusAsync(0);
        await ReferenceViewModel.LoadForGenusAsync(0);

        IsCommentEditVisible = false;
        IsReferenceEditVisible = false;
    }

    // ---------------------------------------------------------
    // Save
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl66Genus?> SaveItemAsync(Models.Tbl66Genus item)
    {
        if (await _data.GenusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertGenusAsync(item);
        return await _data.GetGenusByIdAsync(saved.GenusId);
    }

    // ---------------------------------------------------------
    // Delete
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    {
        if (item.GenusId == 0)
        {
            SelectedItem = null;
            return false;
        }

        if (await _data.GenusHasSpeciesAsync(item.GenusId) ||
            await _data.GenusHasReferenceAsync(item.GenusId) ||
            await _data.GenusHasCommentAsync(item.GenusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await _data.DeleteGenusAsync(item.GenusId);
        return true;
    }

    // ---------------------------------------------------------
    // Sorting
    // ---------------------------------------------------------
    private bool _sortAscending = true;
    private string _currentSortColumn = "";

    private string _idHeaderText = "ID";
    public string IdHeaderText
    {
        get => _idHeaderText;
        set { _idHeaderText = value; OnPropertyChanged(); }
    }

    private string _nameHeaderText = "Name";
    public string NameHeaderText
    {
        get => _nameHeaderText;
        set { _nameHeaderText = value; OnPropertyChanged(); }
    }

    public void SortBy(string column)
    {
        if (Items.Count == 0)
            return;

        if (_currentSortColumn == column)
            _sortAscending = !_sortAscending;
        else
        {
            _currentSortColumn = column;
            _sortAscending = true;
        }

        IEnumerable<Models.Tbl66Genus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.GenusId)
                                   : Items.OrderByDescending(x => x.GenusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.GenusName)
                                       : Items.OrderByDescending(x => x.GenusName),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl66Genus>(sorted);

        SelectedItem = null;
        UpdateHeaderTexts();
    }

    private void UpdateHeaderTexts()
    {
        const string asc = " ▲";
        const string desc = " ▼";

        IdHeaderText = "ID";
        NameHeaderText = "Name";

        var suffix = _sortAscending ? asc : desc;

        switch (_currentSortColumn)
        {
            case "Id":
                IdHeaderText += suffix;
                break;
            case "Extra1":
                NameHeaderText += suffix;
                break;
        }
    }
}
