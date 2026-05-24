using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Modules.Tbl90Reference;

public class Tbl90ReferenceViewModel : BaseListViewModel<Models.Tbl90Reference>
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IDialogService _dialog;
    private readonly IDataService _data;
    private readonly LookupService _lookupService;

    public Tbl90ReferenceViewModel(
        IDataService data,
        IDialogService dialog,
        LookupService lookupService,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
        _lookupService = lookupService;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        OpenEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(OpenEdit);
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);

        NewCommand = new AsyncRelayCommand(NewAsync);

        RefAuthorsList = new ObservableCollection<Models.Tbl90RefAuthor>();
        RefExpertsList = new ObservableCollection<Models.Tbl90RefExpert>();
        RefSourcesList = new ObservableCollection<Models.Tbl90RefSource>();
    }


    // ---------------------------------------------------------
    // Lookup‑Loader
    // ---------------------------------------------------------
    public async Task LoadReferenceExpertsAsync()
    {
        RefExpertsList.Clear();
        var items = await _data.GetAllRefExpertsAsync();
        foreach (var item in items)
            RefExpertsList.Add(item);
    }

    public async Task LoadReferenceSourcesAsync()
    {
        RefSourcesList.Clear();
        var items = await _data.GetAllRefSourcesAsync();
        foreach (var item in items)
            RefSourcesList.Add(item);
    }

    public async Task LoadReferenceAuthorsAsync()
    {
        RefAuthorsList.Clear();
        var items = await _data.GetAllRefAuthorsAsync();
        foreach (var item in items)
            RefAuthorsList.Add(item);
    }

    public async Task LoadComboDataAsync()
    {
        await LoadReferenceExpertsAsync();
        await LoadReferenceSourcesAsync();
        await LoadReferenceAuthorsAsync();
    }

    // ---------------------------------------------------------
    // Combo‑Collections
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; }
    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; }
    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; }

    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    // ---------------------------------------------------------
    // FK: aktueller 
    // ---------------------------------------------------------
    public int? CurrentReferenceId { get; set; }
    public int? CurrentTribusId { get; set; }
    public int? CurrentGenusId { get; set; }
    public int? CurrentSubtribusId { get; set; }
    public int? CurrentInfratribusId { get; set; }

    // --------------------------------------------
    // Reset-Logik
    // --------------------------------------------
    public void ResetBelow()
    {
        CurrentSubtribusId = null;
        CurrentInfratribusId = null;
        CurrentGenusId = null;
    }

    // --------------------------------------------
    // LoadForXAsync – neue Pipeline
    // --------------------------------------------
    public Task LoadForTribusAsync(int id)
        => LoadForAsync(id, _data.GetReferencesForTribusAsync, ResetBelow);

    public Task LoadForSubtribusAsync(int id)
        => LoadForAsync(id, _data.GetReferencesForSubtribusAsync, ResetBelow);

    public Task LoadForInfratribusAsync(int id)
        => LoadForAsync(id, _data.GetReferencesForInfratribusAsync, ResetBelow);

    public Task LoadForGenusAsync(int id)
        => LoadForAsync(id, _data.GetReferencesForGenusAsync, ResetBelow);
    public Task LoadForSpeciesAsync(int id)
        => LoadForAsync(id, _data.GetReferencesForSpeciesAsync, ResetBelow);
    // --------------------------------------------
    // Edit schließen
    // --------------------------------------------
    protected override void OnEditClosed()
    {
        IsReferenceEditVisible = false;
    }

    // ---------------------------------------------------------
    // Base overrides
    // ---------------------------------------------------------

    public override Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem == null)
            return Task.CompletedTask;

        // Debug optional
        System.Diagnostics.Debug.WriteLine(
            $"[ReferenceVM] SelectedItem changed: " +
            $"Expert={SelectedItem.RefExpertId}, " +
            $"Source={SelectedItem.RefSourceId}, " +
            $"Author={SelectedItem.RefAuthorId}, ");

        // Keine weitere Logik nötig – das Model steuert alles
        return Task.CompletedTask;
    }

    protected override Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
        => Task.FromResult<IEnumerable<Models.Tbl90Reference>>(Enumerable.Empty<Models.Tbl90Reference>());

    protected override Models.Tbl90Reference CreateNewItem()
    {
        var item = new Models.Tbl90Reference
        {
            RefExpertId = null,
            RefSourceId = null,
            RefAuthorId = null
        };

        if (CurrentSubtribusId != null)
        {
            item.SubtribusId = CurrentSubtribusId.Value;
            item.InfratribusId = null;
            return item;
        }

        if (CurrentInfratribusId != null)
        {
            item.InfratribusId = CurrentInfratribusId.Value;
            item.SubtribusId = null;
            return item;
        }

        throw new InvalidOperationException("No parent context set.");
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90Reference item)
        => _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90Reference item)
        => _dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
    {
        // 1. Pflichtprüfung: Mindestens eine ID muss gesetzt sein
        if (item.RefExpertId == null &&
            item.RefSourceId == null &&
            item.RefAuthorId == null)
        {
            await _dialog.ShowErrorAsync("Error_NoRefComboBoxSelected_Title", "Error_NoRefComboBoxSelected_Message");
            return null;
        }

        // 2. FK korrekt setzen (Subtribus oder Infratribus)
        if (CurrentSubtribusId != null)
        {
            item.SubtribusId = CurrentSubtribusId.Value;
            item.InfratribusId = null;   // ← wichtig
        }

        if (CurrentInfratribusId != null)
        {
            item.InfratribusId = CurrentInfratribusId.Value;
            item.SubtribusId = null;     // ← wichtig
        }

        // 3. Duplicate check
        if (await _data.ReferenceExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        // 4. Speichern
        var saved = await _data.UpsertReferenceAsync(item);

        //Debug.WriteLine(
        //    $"Save Reference: Subtribus={item.SubtribusId}, Infratribus={item.InfratribusId}, " +
        //    $"Expert={item.RefExpertId}, Source={item.RefSourceId}, Author={item.RefAuthorId}"
        //);

        // 5. Reload
        return await _data.GetReferenceByIdAsync(saved.ReferenceId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl90Reference item)
    {
        if (item.ReferenceId == 0)
        {
            IsReferenceEditVisible = false;
            SelectItemSilently(null);
            return false;
        }

        await _data.DeleteReferenceAsync(item.ReferenceId);
        return true;
    }

    protected override void OnAfterNew(Models.Tbl90Reference item)
        => IsReferenceEditVisible = true;

    protected override bool IsUnsavedNewItem(Models.Tbl90Reference item)
        => item.ReferenceId == 0;

    private async Task NewAsync()
    {
        if (CurrentSubtribusId <= 0)
        {
            await _dialog.ShowErrorAsync("Hinweis", "Bitte zuerst einen Subtribus auswählen.");
            return;
        }

        base.New();
    }

    // ---------------------------------------------------------
    // Edit visibility
    // ---------------------------------------------------------
    public bool IsCommentEditVisible
    {
        get => _isCommentEditVisible;
        set { _isCommentEditVisible = value; OnPropertyChanged(); }
    }
    private bool _isCommentEditVisible;


    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set { _isDetailVisible = value; OnPropertyChanged(); }
    }
    private bool _isDetailVisible;

    private bool _isReferenceEditVisible;
    public bool IsReferenceEditVisible
    {
        get => _isReferenceEditVisible;
        set { _isReferenceEditVisible = value; OnPropertyChanged(); }
    }

    public int? CurrentSupertribusId { get; set; }

    private void OpenEdit()
    {
        if (SelectedItem != null)
            IsReferenceEditVisible = true;
    }

    private void CloseEdit()
    {
        IsReferenceEditVisible = false;
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // Reload (FK)
    // ---------------------------------------------------------
    public override async Task ReloadAndSelectAsync(Models.Tbl90Reference? saved, bool autoSelect)
    {
        IsLoading = true;
        try
        {
            var old = SelectedItem;

            IsReferenceEditVisible = false;

            // ---------------------------------------------------------
            // 1. Items laden je nach Parent-Modus
            // ---------------------------------------------------------
            if (CurrentSubtribusId != null)
            {
                Items = new ObservableCollection<Models.Tbl90Reference>(
                    await _data.GetReferencesForSubtribusAsync(CurrentSubtribusId.Value)
                );
            }
            else if (CurrentInfratribusId != null)
            {
                Items = new ObservableCollection<Models.Tbl90Reference>(
                    await _data.GetReferencesForInfratribusAsync(CurrentInfratribusId.Value)
                );
            }
            else
            {
                // Sicherheitsfallback
                Items = new ObservableCollection<Models.Tbl90Reference>();
            }

            DataSetCount = Items.Count;

            // ---------------------------------------------------------
            // 2. Combo-Daten laden
            // ---------------------------------------------------------
            await LoadComboDataAsync();

            // ---------------------------------------------------------
            // 3. Auswahl zurücksetzen
            // ---------------------------------------------------------
            SelectItemSilently(null);

            if (!autoSelect)
                return;

            // ---------------------------------------------------------
            // 4. Auswahl wiederherstellen
            // ---------------------------------------------------------
            if (saved != null)
            {
                SelectedItem = Items.FirstOrDefault(x => x.ReferenceId == saved.ReferenceId);
            }
            else if (old != null && old.ReferenceId != 0)
            {
                SelectedItem = Items.FirstOrDefault(x => x.ReferenceId == old.ReferenceId);
            }
            else
            {
                SelectedItem = Items.FirstOrDefault();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ---------------------------------------------------------
    // Sort
    // ---------------------------------------------------------
    private bool _sortAscending = true;
    private string _currentSortColumn = "";

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

        IEnumerable<Models.Tbl90Reference> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.ReferenceId)
                                   : Items.OrderByDescending(x => x.ReferenceId),

            "Info" => _sortAscending ? Items.OrderBy(x => x.Info)
                                     : Items.OrderByDescending(x => x.Info),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl90Reference>(sorted);
    }
}
