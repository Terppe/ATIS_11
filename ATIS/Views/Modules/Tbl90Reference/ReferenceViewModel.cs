using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl90Reference;

public class Tbl90ReferenceViewModel : BaseListViewModel<Models.Tbl90Reference>
{
    private readonly IDialogService _dialog;

    public Tbl90ReferenceViewModel(IDataService data, IDialogService dialog)
        : base(data)
    {
        _dialog = dialog;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        OpenEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(OpenEdit);
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);

        NewCommand = new AsyncRelayCommand(NewAsync);

        RefAuthorsList = new ObservableCollection<Models.Tbl90RefAuthor>();
        RefExpertsList = new ObservableCollection<Models.Tbl90RefExpert>();
        RefSourcesList = new ObservableCollection<Models.Tbl90RefSource>();
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
    // FK: aktueller Subtribus
    // ---------------------------------------------------------
    public int? CurrentSubtribusId { get; private set; }
    public int? CurrentInfratribusId { get; private set; }
    public int? CurrentGenusId { get; private set; }

    //public async Task LoadForSubtribusAsync(int subtribusId)
    //{
    //    CurrentSubtribusId = subtribusId;
    //    CurrentInfratribusId = null;

    //    Items = new ObservableCollection<Models.Tbl90Reference>(
    //        await _data.GetReferencesForSubtribusAsync(subtribusId));

    //    SelectedItem = null;
    //    IsReferenceEditVisible = false;
    //}
    public async Task LoadForSubtribusAsync(int subtribusId)
    {
        if (subtribusId <= 0)
        {
            Items = new ObservableCollection<Models.Tbl90Reference>();
            SelectedItem = null;
            IsReferenceEditVisible = false;
            return;
        }

        CurrentSubtribusId = subtribusId;
        CurrentInfratribusId = null;

        Items = new ObservableCollection<Models.Tbl90Reference>(
            await _data.GetReferencesForSubtribusAsync(subtribusId));

        SelectedItem = null;
        IsReferenceEditVisible = false;
    }

    public async Task LoadForInfratribusAsync(int infratribusId)
    {
        // Modus setzen
        CurrentInfratribusId = infratribusId;
        CurrentSubtribusId = null;

        // Laden
        Items = new ObservableCollection<Models.Tbl90Reference>(
            await _data.GetReferencesForInfratribusAsync(infratribusId)
        );

        DataSetCount = Items.Count;

        // Auswahl zurücksetzen
        SelectItemSilently(null);
        SelectedItem = null;

        // Editor schließen
        IsReferenceEditVisible = false;

        // Detailbereich schließen, wenn keine Items
        IsDetailVisible = false;
    }

    public async Task LoadForGenusAsync(int genusId)
    {
        // Modus setzen
        CurrentGenusId = genusId;
        CurrentInfratribusId = null;

        // Laden
        Items = new ObservableCollection<Models.Tbl90Reference>(
            await _data.GetReferencesForGenusAsync(genusId)
        );

        DataSetCount = Items.Count;

        // Auswahl zurücksetzen
        SelectItemSilently(null);
        SelectedItem = null;

        // Editor schließen
        IsReferenceEditVisible = false;

        // Detailbereich schließen, wenn keine Items
        IsDetailVisible = false;
    }

    //public async Task LoadForInfratribusAsync(int infratribusId)
    //{
    //    CurrentInfratribusId = infratribusId;
    //    CurrentSubtribusId = null;   // ← wichtig: Modus zurücksetzen

    //    Items = new ObservableCollection<Models.Tbl90Reference>(
    //        await _data.GetReferencesForInfratribusAsync(infratribusId)
    //    );

    //    DataSetCount = Items.Count;

    //    await LoadComboDataAsync();

    //    IsReferenceEditVisible = false;
    //    SelectItemSilently(null);
    //}

    private async Task LoadComboDataAsync()
    {
        RefAuthorsList.Clear();
        RefExpertsList.Clear();
        RefSourcesList.Clear();

        foreach (var a in await _data.GetAllRefAuthorsAsync())
            RefAuthorsList.Add(a);
        foreach (var e in await _data.GetAllRefExpertsAsync())
            RefExpertsList.Add(e);
        foreach (var s in await _data.GetAllRefSourcesAsync())
            RefSourcesList.Add(s);
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
