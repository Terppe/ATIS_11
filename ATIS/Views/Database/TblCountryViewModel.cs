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

    // =========================================================
    #endregion [Private Data Members]
    // =========================================================


    // =========================================================
    #region [Constructor]
    // =========================================================

    public TblCountryViewModel(IDataService dataService)
    {
        _dataService = dataService;

        Countries = new ObservableCollection<TblCountry>();


        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SearchCommand = new AsyncRelayCommand<string>(LoadSearchAsync);
        AddCountryCommand = new RelayCommand(AddCountry);
        RefreshCountryCommand = new AsyncRelayCommand(RefreshAsync);
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
        CountrySelected = null;
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

                foreach (var item in list)
                    Countries.Add(item);

                CountryDataSetCount = Countries.Count;
            }
            else
            {
                CountryDataSetCount = 0;
            }

            // Standard: keine Auswahl
            CountrySelected = null;

            // Nach Speichern: wieder auswählen
            if (selectId != null)
                CountrySelected = Countries.FirstOrDefault(c => c.CountryId == selectId);

            OnPropertyChanged(nameof(CountryDataSetCountText));
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadSearchAsync(string? search)
    {
        Debug.WriteLine($"LoadSearchAsync: {search}");

        IsLoading = true;
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
    }

    // ------------------ Refresh --------------------
    public bool CanRefresh => !IsInEditCountry;

    private async Task RefreshAsync()
    {
        await LoadSearchAsync(CurrentQuery);
    }

    // ------------------ Edit --------------------
    public bool CanEditCountry => CountrySelected != null && !IsInEditCountry;

    public bool CanStartEditCountry => CountrySelected != null && !IsInEditCountry;
    private void StartEditCountry()
    {
        if (CountrySelected == null)
            return;

        _backupCountry = CountrySelected.Clone();

        IsInEditCountry = true;
        IsNewCountry = false;
        IsDetailVisible = true;
    }

    // ------------------ Save --------------------
    public Func<Task<ContentDialogResult>>? RequestSaveDialog { get; set; }
    private bool CanSaveCountry()
    {
        return IsInEditCountry;
    }
    private async Task SaveCountryAsync()
    {
        if (CountrySelected == null)
            return;

        var saved = await _dataService.UpsertCountryAsync(CountrySelected);

        await LoadSearchInternalAsync(SearchText, saved.CountryId);

        IsInEditCountry = false;
        IsNewCountry = false;
    }

    //private async Task SaveCountryAsync()
    //{
    //    if (CountrySelected == null)
    //        return;

    //    var id = CountrySelected.CountryId;

    //    await _dataService.UpsertCountryAsync(CountrySelected);

    //    // Suche erneut laden und geänderten Datensatz wieder auswählen
    //    await LoadSearchInternalAsync(SearchText, id);

    //    IsInEditCountry = false;
    //}

    public Func<Task<bool>>? RequestDeleteDialog { get; set; }

    private bool CanDeleteCountry() =>
        CountrySelected != null;

    private async Task DeleteCountryAsync()
    {
        if (RequestDeleteDialog != null)
        {
            var confirm = await RequestDeleteDialog();

            if (!confirm)
                return;
        }
        if (CountrySelected == null) return;

        await _dataService.DeleteCountryAsync(CountrySelected.CountryId);

        Countries.Remove(CountrySelected);

        CountrySelected = Countries.FirstOrDefault();
        CountryDataSetCount = Countries.Count;
        OnPropertyChanged(nameof(CountryDataSetCountText));

        IsDetailVisible = CountrySelected != null;

        _backupCountry = null;
        IsInEditCountry = false;
        IsNewCountry = false;

    }

    // ------------------ Cancel --------------------
    private void CancelEdit()
    {
        if (_backupCountry != null && CountrySelected != null)
        {
            CountrySelected.CopyFrom(_backupCountry);
        }

        IsInEditCountry = false;
        IsNewCountry = false;
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

    // =========================================================
    #endregion [Methods Country]
    // =========================================================

    // =========================================================
    #region [All Properties]
    // =========================================================

    // ------------------ Collections --------------------
    public ObservableCollection<TblCountry> Countries { get; }

    public bool IsDetailEnabled => CountrySelected != null;

    // ------------------ Selected Country --------------------
    private TblCountry? _countrySelected;
    public TblCountry? CountrySelected
    {
        get => _countrySelected;
        set
        {
            if (SetProperty(ref _countrySelected, value))
            {
                Debug.WriteLine($"CountrySelected Type: {value?.GetType().FullName}");
                OnPropertyChanged(nameof(HasSelection));

                // 1️⃣ Erst alle Zustände setzen
                IsDetailVisible = value != null;
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
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
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
    //   public bool CanEditCountry => CountrySelected != null;

    //public bool CanEditCountry => CountrySelected != null && !IsInEditCountry;

    //   public bool DataGridHasFocus => true;

    //    private bool _dataGridHasFocus;
    //public bool DataGridHasFocus
    //{
    //    get => _dataGridHasFocus;
    //    set
    //    {
    //        _dataGridHasFocus = value;
    //        OnPropertyChanged();
    //        OnPropertyChanged(nameof(CanEditCountry));

    //        //if (SetProperty(ref _dataGridHasFocus, value))
    //        //{
    //        //    UpdateCanEditCountry();
    //        //}
    //    }
    //}

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
    //public IRelayCommand? BackToListCommand { get; }
    public IRelayCommand<TblCountry> OpenCountryCommand { get; }
    public IAsyncRelayCommand SaveCountryCommand { get; }
    public IAsyncRelayCommand DeleteCountryCommand { get; }
    public IRelayCommand StartEditCountryCommand { get; }
    public IAsyncRelayCommand RefreshCountryCommand { get; }

    #endregion [All Properties]
}



