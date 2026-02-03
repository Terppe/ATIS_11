using System.Collections.ObjectModel;
using System.Diagnostics;
using ATIS.Interfaces;
using ATIS.Models;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Views.Database;

public partial class TblCountryViewModel : ObservableObject
{
    // =========================================================
    #region [Private Data Members]
    // =========================================================

    private readonly IDataService _dataService;
    private TblCountry? _backupCountry;
    public event Action? RequestFocusName;
    //public ObservableCollection<TblCountry> Countries { get; }
    //    = new ObservableCollection<TblCountry>();


    // =========================================================
    #endregion [Private Data Members]
    // =========================================================


    // =========================================================
    #region [Constructor]
    // =========================================================

    public TblCountryViewModel(IDataService dataService)
    {
        Debug.WriteLine($"CTOR: _isLoading (privat) = {_isLoading}");
        Debug.WriteLine($"CTOR: IsLoading (public) = {IsLoading}");

        _dataService = dataService;

        Countries.CollectionChanged += (s, e) =>
        {
            //Debug.WriteLine($"[Countries] Action={e.Action}, Count={Countries.Count}");
            //Debug.WriteLine(Environment.StackTrace);
            Debug.WriteLine($"[CountryPageVM] IsLoading Default: {IsLoading}");
        };
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SearchCommand = new AsyncRelayCommand<string>(LoadSearchAsync);
        AddCountryCommand = new RelayCommand(AddCountry);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => CanRefresh);
        SaveCountryCommand = new AsyncRelayCommand(SaveCountryAsync, CanSaveCountry);
        DeleteCountryCommand = new AsyncRelayCommand(DeleteCountryAsync, CanDeleteCountry);
        CancelEditCountryCommand = new RelayCommand(CancelEdit, () => IsInEditCountry);
        OpenCountryCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<TblCountry>(OpenCountry);
        StartEditCountryCommand = new RelayCommand(StartEditCountry);


    }

    // =========================================================
    #endregion [Constructor]
    // =========================================================


    // =========================================================
    #region [Methods Country]
    // =========================================================
    // ------------------ Load all countries --------------------
    //   private bool _isInitialized;

    public async Task LoadAsync()
    {
        Countries.Clear();
//        CountrySelected = null;
        CountryDataSetCount = 0;
    }

    private async Task LoadSearchInternalAsync(string? search, int? selectId)
    {
        IsLoading = true;
        try
        {
            Countries.Clear();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var list = await _dataService.GetCountriesByNameOrIdAsync(search);

                Debug.WriteLine($"Search returned {list.Count} items for '{search}'");
                foreach (var item in list)
                    Countries.Add(item);

                CountryDataSetCount = Countries.Count;
            }
            else
            {
                Debug.WriteLine("Search skipped because search text is empty");
                CountryDataSetCount = 0;
            }


            // ❗ Nur auf null setzen, wenn KEINE Wieder-Auswahl gewünscht ist
            if (selectId == null)
            {
                CountrySelected = null;
            }

            // Nach Speichern: wieder auswählen
            if (selectId != null)
            {
                CountrySelected = Countries.FirstOrDefault(c => c.CountryId == selectId);
                Debug.WriteLine($"Re-select result: {(CountrySelected == null ? "NOT FOUND" : "FOUND")}");
                OnPropertyChanged(nameof(CountrySelected));   // 🔥 zwingt NumberBox zum Refresh
            }

            OnPropertyChanged(nameof(CountryDataSetCountText));
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadSearchAsync(string? search)
    {
        //      Debug.WriteLine($"LoadSearchAsync: {search}");
        CurrentQuery = search;

        IsLoading = true;
        await Task.Yield();
        try
        {
            Countries.Clear();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var list = await _dataService.GetCountriesByNameOrIdAsync(search);

                foreach (var item in list)
                    Countries.Add(item);

                CountryDataSetCount = Countries.Count;
            }
            else
            {
                CountryDataSetCount = 0;
            }

            CountrySelected = null;
            //    CountrySelected = Countries.FirstOrDefault();
            OnPropertyChanged(nameof(CountryDataSetCountText));
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchAsync(string? query)
    {
        IsLoading = true;
        try
        {
            Countries.Clear();

            if (string.IsNullOrWhiteSpace(query))
            {
                CountryDataSetCount = 0;
                OnPropertyChanged(nameof(CountryDataSetCountText));
                return;
            }

            var result = await _dataService.GetCountriesByNameOrIdAsync(query);

            foreach (var item in result)
                Countries.Add(item);

            CountryDataSetCount = Countries.Count;
            OnPropertyChanged(nameof(CountryDataSetCountText));

            CountrySelected = Countries.FirstOrDefault();
        }
        finally
        {
            IsLoading = false;
        }
    }

    //------------ Add --------------------
    private void AddCountry()
    {
        var newCountry = new TblCountry();

        CountrySelected = newCountry;   // Detailbereich zeigt leeres Formular
        IsInEditCountry = true;         // Editmodus aktiv
        IsNewCountry = true;            // Speichern weiß: INSERT statt UPDATE

        RequestFocusName?.Invoke();
    }

    // ------------------ Refresh --------------------
    public bool CanRefresh => !IsInEditCountry && CountrySelected != null;

    private async Task RefreshAsync()
    {
        Debug.WriteLine("=== REFRESH START ===");

        if (CountrySelected == null)
            return;

        var id = CountrySelected.CountryId;

        IsLoading = true;

        try
        {
            // 1) Liste neu laden (mit Query)
            var freshList = await _dataService.GetCountriesByNameAsync(CurrentQuery);

            // 2) Countries neu befüllen (kein Replace → DataGrid bleibt stabil)
            Countries.Clear();
            foreach (var c in freshList)
                Countries.Add(c);

            // 3) Selektion wiederherstellen
            CountrySelected = Countries.FirstOrDefault(c => c.CountryId == id);

            // 4) Count aktualisieren
            CountryDataSetCount = Countries.Count;
            OnPropertyChanged(nameof(CountryDataSetCountText));
        }
        finally
        {
            IsLoading = false;
        }
    }

    //private async Task RefreshAsync()
    //{
    //    Debug.WriteLine("=== REFRESH START ===");

    //    if (CountrySelected == null)
    //        return;

    //    var id = CountrySelected.CountryId;

    //    // komplette Liste neu laden
    //    var freshList = await _dataService.GetCountriesByNameAsync(CurrentQuery);

    //    Countries = new ObservableCollection<TblCountry>(freshList);

    //    // Selektion wiederherstellen
    //    CountrySelected = Countries.FirstOrDefault(c => c.CountryId == id);

    //    CountryDataSetCount = Countries.Count;
    //    OnPropertyChanged(nameof(CountryDataSetCountText));
    //}

    //private async Task RefreshAsync()
    //{
    //    Debug.WriteLine("=== REFRESH START ===");
    //    Debug.WriteLine($"CurrentQuery = '{CurrentQuery}'");
    //    Debug.WriteLine($"CurrentId = {CountrySelected?.CountryId}");

    //        // Wenn nichts ausgewählt ist → gar nichts tun
    //        if (CountrySelected == null)
    //            return;

    //        var id = CountrySelected.CountryId;

    //        // Frische Version aus der DB holen
    //        var fresh = await _dataService.GetCountryByIdAsync(id);
    //        if (fresh == null)
    //            return; // optional: oder Auswahl löschen

    //        // Den Eintrag in der bestehenden Liste ersetzen
    //        var index = Countries.IndexOf(CountrySelected);
    //        if (index >= 0)
    //        {
    //            Countries[index] = fresh;
    //            CountrySelected = fresh;
    //        }

    //        CountryDataSetCount = Countries.Count;
    //        OnPropertyChanged(nameof(CountryDataSetCountText));


    //}

    // ------------------ Edit --------------------
    public bool CanEditCountry => CountrySelected != null && !IsInEditCountry;

    public bool CanStartEditCountry => CountrySelected != null && !IsInEditCountry;
    private void StartEditCountry()
    {
        if (CountrySelected == null)
            return;

        // Backup für Cancel
        _backupCountry = CountrySelected.Clone();

        IsInEditCountry = true;
        IsNewCountry = false;
        IsDetailVisible = true;

        // Commands aktualisieren
        (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
        (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    }

    // ------------------ Save --------------------
    public Func<Task<ContentDialogResult>>? RequestSaveDialog { get; set; }
    private bool CanSaveCountry()
    {
        return IsInEditCountry && CountrySelected != null;
    }
    private async Task SaveCountryAsync()
    {
        if (CountrySelected == null)
            return;

        Debug.WriteLine("=== SAVE START ===");
        Debug.WriteLine($"CountryId: {CountrySelected.CountryId}");
        Debug.WriteLine($"Name: {CountrySelected.Name}");
        Debug.WriteLine($"Latitude: {CountrySelected.Latitude}");
        Debug.WriteLine($"Longitude: {CountrySelected.Longitude}");
        Debug.WriteLine($"ZoomLevel: {CountrySelected.ZoomLevel}");

        // 1️⃣ Dialog anzeigen
        if (RequestSaveDialog != null)
        {
            var result = await RequestSaveDialog();
            if (result != ContentDialogResult.Primary)
                return;
        }

        Debug.WriteLine("=== CALLING UPSERT ===");

        // 2️⃣ Speichern
        var saved = await _dataService.UpsertCountryAsync(CountrySelected);

        // Wenn kein SearchText gesetzt ist → direkt nur den neuen Datensatz laden
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Countries.Clear();
            Countries.Add(saved);
            CountrySelected = saved;
            return;
        }

        Debug.WriteLine("=== UPSERT RETURNED ===");
        Debug.WriteLine($"Saved CountryId: {saved.CountryId}");

        // 3️⃣ Liste neu laden und gespeicherten Datensatz wieder auswählen
        Debug.WriteLine("=== RELOAD LIST ===");
        Debug.WriteLine($"SearchText: {SearchText}");
        Debug.WriteLine($"SelectId: {saved.CountryId}");

        await LoadSearchInternalAsync(SearchText, saved.CountryId);

        Debug.WriteLine("=== RELOAD DONE ===");

        IsInEditCountry = false;
        IsNewCountry = false;

        // 4️⃣ Commands aktualisieren
        (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
        (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    }

    public Func<Task<bool>>? RequestDeleteDialog { get; set; }

    private bool CanDeleteCountry() =>
        CountrySelected != null;

    private async Task DeleteCountryAsync()
    {
        // 1️⃣ Dialog anzeigen
        if (RequestDeleteDialog != null)
        {
            var confirm = await RequestDeleteDialog();
            if (!confirm)
                return;
        }

        // 2️⃣ Safety: Kein SelectedItem → nichts zu löschen
        if (CountrySelected == null)
            return;

        var idToDelete = CountrySelected.CountryId;

        // 3️⃣ Löschen
        await _dataService.DeleteCountryAsync(idToDelete);

        // 4️⃣ Aus Liste entfernen
        Countries.Remove(CountrySelected);

        // 5️⃣ Neuen Datensatz auswählen (falls vorhanden)
        CountrySelected = Countries.FirstOrDefault();

        // 6️⃣ Dataset Count aktualisieren
        CountryDataSetCount = Countries.Count;
        OnPropertyChanged(nameof(CountryDataSetCountText));

        // 7️⃣ UI‑Zustände zurücksetzen
        _backupCountry = null;
        IsInEditCountry = false;
        IsNewCountry = false;
        IsDetailVisible = CountrySelected != null;

        // 8️⃣ Commands aktualisieren
        (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
        (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    }

    // ------------------ Cancel --------------------
    private void CancelEdit()
    {
        // 1️⃣ Änderungen zurücksetzen
        if (_backupCountry != null && CountrySelected != null)
        {
            CountrySelected.CopyFrom(_backupCountry);
        }

        // 2️⃣ Editmodus beenden
        IsInEditCountry = false;
        IsNewCountry = false;

        // 3️⃣ Commands aktualisieren
        (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
        (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    }
    public bool CanCancelCountry => IsInEditCountry;
    public Task CancelAsync()
    {
        if (_backupCountry != null)
        {
            CountrySelected?.CopyFrom(_backupCountry);
        }

        IsInEditCountry = false;
        return Task.CompletedTask;
    }

    // ------------------ Open from DataGrid --------------------
    private void OpenCountry(TblCountry? country)
    {
        if (country == null) return;

        CountrySelected = country;
        IsDetailVisible = true;
        IsInEditCountry = false;
        IsNewCountry = false;
    }

    public void Sort(string columnName, bool ascending)
    {
        var prop = typeof(TblCountry).GetProperty(columnName);
        if (prop == null)
            return;

        var sorted = ascending
            ? Countries.OrderBy(x => prop.GetValue(x) ?? "").ToList()
            : Countries.OrderByDescending(x => prop.GetValue(x) ?? "").ToList();

        Countries.Clear();

        foreach (var item in sorted)
            Countries.Add(item);
    }
    //=========================================================
    #endregion [Methods Country]
    // =========================================================

    // =========================================================
    #region [All Properties]
    // =========================================================
    private ObservableCollection<TblCountry> _countries = new();
    public ObservableCollection<TblCountry> Countries
    {
        get => _countries;
        set
        {
            _countries = value;
            OnPropertyChanged();
        }
    }

    // ------------------ Collections --------------------

    public bool IsDetailEnabled => CountrySelected != null;

    // ------------------ Selected Country --------------------
    private TblCountry? _countrySelected;
    public TblCountry? CountrySelected
    {
        get => _countrySelected;
        set
        {
            Debug.WriteLine($"[CountrySelected] SET → {(value == null ? "NULL" : value.CountryId.ToString())}");
            // 1️⃣ Null-Guard
            if (value == null)
            {
                if (SetProperty(ref _countrySelected, null))
                {
                    IsDetailVisible = false;
                    IsNewCountry = false;
                    _isInEditCountry = false;
                    OnPropertyChanged(nameof(HasSelection));
                    OnPropertyChanged(nameof(IsInEditCountry));
                    OnPropertyChanged(nameof(CanEditCountry));
                    (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
                    (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
                    (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (RefreshCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
                return; // ⛔ WICHTIG: NICHT WEITER MACHEN
            }

            if (SetProperty(ref _countrySelected, value))
            {
                //Debug.WriteLine($"[CountrySelected] SET → {(value == null ? "NULL" : value.CountryId.ToString())}");
                //Debug.WriteLine(Environment.StackTrace);

                OnPropertyChanged(nameof(HasSelection));

                // 1️⃣ Erst alle Zustände setzen
                //IsDetailVisible = value != null; vorher
                IsDetailVisible = true;
                IsNewCountry = false;

                // Wichtig: IsInEditCountry löst selbst CanEditCountry aus
                // aber das passiert zu früh → also temporär unterdrücken
                _isInEditCountry = false;
                OnPropertyChanged(nameof(IsInEditCountry));

                // 2️⃣ Jetzt EINMAL CanEditCountry auslösen
                OnPropertyChanged(nameof(CanEditCountry));

                // 3️⃣ Commands aktualisieren
                (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
                (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
                (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RefreshCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    // ------------------ Search --------------------
    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public string SearchPlaceholder => "Suche nach Ländern…";

    public UIElement DetailHeaderButtons => new StackPanel
    {
        Orientation = Orientation.Horizontal,
        Spacing = 8,
        Children =
        {
            new AppBarButton
            {
                Command = SaveCountryCommand,
                Tag = "\uE74E",
                Visibility = IsInEditCountry ? Visibility.Visible : Visibility.Collapsed
            },
            new AppBarButton
            {
                Command = CancelEditCountryCommand,
                Tag = "\uE8BB",
                Visibility = IsInEditCountry ? Visibility.Visible : Visibility.Collapsed
            },
            new AppBarButton
            {
                Command = StartEditCountryCommand,
                IsEnabled = CanEditCountry,
                Tag = "\uE70F"
            }
        }
    };

    // ------------------ UI State --------------------

    private bool _isLoading = false; // GANZ WICHTIG!
    public bool IsLoading
    {
        get => _isLoading;
        set { SetProperty(ref _isLoading, value); Debug.WriteLine("IsLoading SET: " + value); }
    }

    private string? _currentQuery;

    public string? CurrentQuery
    {
        get => _currentQuery;
        set => SetProperty(ref _currentQuery, value);
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    private bool _isInEditCountry;
    public bool IsInEditCountry
    {
        get => _isInEditCountry;
        set
        {
            if (SetProperty(ref _isInEditCountry, value))
            {
                OnPropertyChanged(nameof(CanEditCountry));

                (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
                (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RefreshCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }
    public bool HasSelection => CountrySelected != null;

    private bool _isNewCountry;
    public bool IsNewCountry
    {
        get => _isNewCountry;
        set => SetProperty(ref _isNewCountry, value);
    }


    // ------------------ Dataset Count --------------------
    private int _countryDataSetCount;
    public int CountryDataSetCount
    {
        get => _countryDataSetCount;
        set => SetProperty(ref _countryDataSetCount, value);
    }

    public string CountryDataSetCountText =>
        $"{CountryDataSetCount} Datensätze gefunden";


    // ------------------ Commands --------------------
    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<string> SearchCommand { get; }
    public IRelayCommand AddCountryCommand { get; }
    public IRelayCommand SyncCountryCommand { get; } = null!;
    public IRelayCommand CancelEditCountryCommand { get; }
    public IRelayCommand<TblCountry> OpenCountryCommand { get; }
    public IAsyncRelayCommand SaveCountryCommand { get; }
    public IAsyncRelayCommand DeleteCountryCommand { get; }
    public IRelayCommand StartEditCountryCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    #endregion [All Properties]
}




