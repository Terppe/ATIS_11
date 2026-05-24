using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using ATIS.DesignSystem.Services;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Microsoft.EntityFrameworkCore;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.ViewModels;

public partial class TblCountryAltViewModel : ObservableObject, INotifyDataErrorInfo
{
    // =========================================================
    #region [ Private Fields ]
    // =========================================================

    private readonly IDataService _dataService;
    private readonly DbContext _db;
    private readonly Dictionary<string, FieldUiInfo> _fieldUi = new();
    public FieldUiInfo NameUi => _fieldUi["Name"];
    public FieldUiInfo RegexUi => _fieldUi["Regex"];
    public FieldUiInfo ZoomLevelUi => _fieldUi["ZoomLevel"];
    public FieldUiInfo LatitudeUi => _fieldUi["Latitude"];
    public FieldUiInfo LongitudeUi => _fieldUi["Longitude"];

    private readonly DialogService _dialogService;

    private readonly Dictionary<string, List<string>> _errors = new();
    private CancellationTokenSource? _applyWidthsCts;

    private TblCountry? _backupCountry;

    #endregion
    // =========================================================

    // =========================================================
    #region [ Constructor ]
    // =========================================================

    public TblCountryAltViewModel(IDataService dataService, AtisDbContext db, DialogService dialogService)
    {
        _dataService = dataService;
        _db = db;
        _dialogService = dialogService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SearchCommand = new AsyncRelayCommand<string>(LoadSearchAsync);
        AddCountryCommand = new RelayCommand(AddCountry);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, CanRefresh);

        SaveCountryCommand = new AsyncRelayCommand(
            SaveCountryAsync,
            CanSaveCountry
        );

        DeleteCountryCommand = new AsyncRelayCommand(
            DeleteCountryAsync,
            CanDeleteCountry
        );

        CancelEditCountryCommand = new RelayCommand(
            CancelEdit,
            () => IsInEditCountry
        );

        OpenCountryCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<TblCountry>(OpenCountry);

        StartEditCountryCommand = new RelayCommand(
            StartEditCountry,
            () => CanEditCountry
        );
        LoadFieldLengths();

    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ CountryEdit Handling ]
    // =========================================================

    private CountryEditModel? _countryEdit;
    public CountryEditModel? CountryEdit
    {
        get => _countryEdit;
        private set => SetProperty(ref _countryEdit, value);
    }

    private void SetEditModel(TblCountry model)
    {
        if (model == null)
            return;

        _isUserEditing = false;

        CountryEdit = new CountryEditModel
        {
            Name = model.Name ?? "",
            Regex = model.Regex ?? "",
            ZoomLevel = model.ZoomLevel?.ToString(CultureInfo.InvariantCulture) ?? "",
            Latitude = model.Latitude?.ToString("G", CultureInfo.CurrentUICulture) ?? "",
            Longitude = model.Longitude?.ToString("G", CultureInfo.CurrentUICulture) ?? ""
        };

        ClearAllErrors();

        _isUserEditing = true;

        ApplyWidths(CurrentPageWidth);
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Selection / Open / Sort ]
    // =========================================================

    private TblCountry? _countrySelected;
    public TblCountry? CountrySelected
    {
        get => _countrySelected;
        set
        {
            if (_countrySelected == value)
                return; // verhindert Loop

            if (SetProperty(ref _countrySelected, value))
            {
                Debug.WriteLine($"[CountrySelected] SET → {(value == null ? "NULL" : value.CountryId.ToString())}");

                IsDetailVisible = value != null;

                if (value != null)
                {
                    SetEditModel(value);
                }
                else
                {
                    CountryEdit = null;
                }

                UpdateCommands();

                OnPropertyChanged(nameof(HasSelection));
                OnPropertyChanged(nameof(CanEditCountry));
            }
        }
    }

    public bool HasSelection => CountrySelected != null;

    private void OpenCountry(TblCountry? country)
    {
        CountrySelected = country;
        IsDetailVisible = true;
        IsInEditCountry = false;
        IsNewCountry = false;
    }

    public void Sort(string columnName, bool ascending)
    {
        var prop = typeof(TblCountry).GetProperty(columnName);
        if (prop == null) return;

        var sorted = ascending
            ? Countries.OrderBy(x => prop.GetValue(x) ?? "").ToList()
            : Countries.OrderByDescending(x => prop.GetValue(x) ?? "").ToList();

        Countries.Clear();
        foreach (var item in sorted)
            Countries.Add(item);
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Load / Search  ]
    // =========================================================

    public Task LoadAsync()
    {
        Countries.Clear();

        if (!IsInEditCountry)
            CountrySelected = null;

        IsInEditCountry = false;
        IsNewCountry = false;

        UpdateCommands();

        CountryDataSetCount = 0;
        return Task.CompletedTask;
    }

    public async Task LoadSearchAsync(string? search)
    {
        CurrentQuery = search;
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

            if (!IsInEditCountry)
                CountrySelected = null;

            UpdateCommands();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadSearchInternalAsync(string? search, int? selectId)
    {
        IsLoading = true;
        try
        {
            Countries.Clear();

            List<TblCountry> list;

            if (!string.IsNullOrWhiteSpace(search))
            {
                list = await _dataService.GetCountriesByNameOrIdAsync(search);
            }
            else
            {
                list = await _dataService.GetAllCountriesAsync();
            }

            foreach (var item in list)
                Countries.Add(item);

            CountryDataSetCount = Countries.Count;

            // Fokus setzen – unabhängig vom Edit-Modus
                if (selectId != null)
                {
                    CountrySelected = Countries.FirstOrDefault(c => c.CountryId == selectId);
                }
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Add / Edit / Save / Delete / Cancel / Refresh / LostFocus ]
    // =========================================================

    private void AddCountry()
    {
        var model = new TblCountry();

        CountrySelected = model;
        SetEditModel(model);

        _backupCountry = model.Clone();
        IsInEditCountry = true;
        IsNewCountry = true;

        UpdateCommands();

        RequestFocusName?.Invoke();
    }

    public void BeginUserEdit()
    {
        _isUserEditing = true;
    }

    public void EndUserEdit()
    {
        _isUserEditing = false;
    }

    private void StartEditCountry()
    {
        if (CountrySelected == null)
            return;
        _backupCountry = new TblCountry
        {
            CountryId = CountrySelected.CountryId,
            Name = CountrySelected.Name,
            Regex = CountrySelected.Regex,
            ZoomLevel = CountrySelected.ZoomLevel,
            Latitude = CountrySelected.Latitude,
            Longitude = CountrySelected.Longitude
        };
        _isUserEditing = false;
        SetEditModel(CountrySelected);
        IsInEditCountry = true;
        UpdateCommands();
    }

    private bool CanSaveCountry()
    {
        if (!IsInEditCountry || CountryEdit == null)
            return false;
        if (string.IsNullOrWhiteSpace(CountryEdit.Name))
            return false;
        if (HasErrors)
            return false;
        return true;
    }
    private async Task SaveCountryAsync()
    {
        if (CountryEdit == null)
            return;

        // 1️⃣ Nur prüfen – NICHT validieren
        if (HasErrors)
        {
            await ShowMessageAsync(
                LocalizationManager.Get("ErrorTitle"),
                LocalizationManager.Get("ErrorFixBeforeSave")
            );

            await ShowMessageAsync("Fehler", "Bitte korrigiere die Fehler, bevor du speicherst.");
            return;
        }

        // 2️⃣ Konvertierung für DB / Domain
        var zoomText = CountryEdit.ZoomLevel?.Replace(',', '.');
        int? zoom = int.TryParse(zoomText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var zoomVal)
            ? zoomVal
            : null;
        var latText = CountryEdit.Latitude?.Replace(',', '.');
        double? lat = double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out var latVal)
            ? latVal
            : null;
        var lonText = CountryEdit.Longitude?.Replace(',', '.');
        double? lon = double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out var lonVal)
            ? lonVal
            : null;

        // 3️⃣ Domain‑Modell für DB (korrigiert)
        var model = _backupCountry != null
            ? _backupCountry.Clone()
            : new TblCountry();

        model.Name = CountryEdit.Name;
        model.Regex = CountryEdit.Regex;
        model.ZoomLevel = zoom;
        model.Latitude = lat;
        model.Longitude = lon;

        // 4️⃣ Duplikatprüfung
        var exists = await _dataService.CountryExistsAsync(model);
        if (exists)
        {
            await ShowMessageAsync("Fehler", "Ein identischer Datensatz existiert bereits.");
            return;
        }

        // 5️⃣ Dialog
        if (RequestSaveDialog != null)
        {
            var result = await RequestSaveDialog();
            if (result != ContentDialogResult.Primary)
                return;
        }

        // 6️⃣ Speichern
        var saved = await _dataService.UpsertCountryAsync(model);

        // 7️⃣ Backup aktualisieren
        _backupCountry = saved.Clone();

        // 8️⃣ Liste neu laden und selektieren
        await LoadSearchInternalAsync(SearchText, saved.CountryId);

        // 9️⃣ Edit‑Modus verlassen
        IsInEditCountry = false;
        IsNewCountry = false;
        _isUserEditing = false;

        UpdateCommands();

        RequestFocusName?.Invoke();

        // 1️⃣0️⃣ Meldung
        //   await ShowMessageAsync("Gespeichert", "Der Datensatz wurde erfolgreich gespeichert.");
      //  await _dialogService.ShowMessageAsync("SaveSuccessTitle", "SaveSuccessMessage");

    }

    private bool CanDeleteCountry()
    {
        return !IsInEditCountry
               && !IsNewCountry
               && CountrySelected != null;
    }

    private async Task DeleteCountryAsync()
    {
        if (CountrySelected == null)
            return;

        if (RequestDeleteDialog != null)
        {
            var confirm = await RequestDeleteDialog();
            if (!confirm)
                return;
        }

        var id = CountrySelected.CountryId;

        // 1️⃣ Löschen in der DB
        await _dataService.DeleteCountryAsync(id);

        // 2️⃣ Liste neu laden
        await LoadSearchInternalAsync(SearchText, null);

        // 3️⃣ Edit-Modus verlassen
        IsInEditCountry = false;
        IsNewCountry = false;

        // 4️⃣ Commands aktualisieren
        UpdateCommands();

        // 5️⃣ Optional: Meldung
        await ShowMessageAsync("Gelöscht", "Der Datensatz wurde erfolgreich gelöscht.");
    }

    private void CancelEdit()
    {
        Debug.WriteLine("[CANCEL] START");
        Debug.WriteLine("[CANCEL] CancelEdit wurde ausgeführt");

        if (_backupCountry != null)
        {
            // 1️⃣ CountryEdit komplett ersetzen
            SetEditModel(_backupCountry);

            // 2️⃣ Auswahl wiederherstellen
            CountrySelected = Countries.FirstOrDefault(c => c.CountryId == _backupCountry.CountryId);
        }

        // 3️⃣ Edit-Modus verlassen
        IsInEditCountry = false;
        IsNewCountry = false;

        // 4️⃣ Commands aktualisieren
        UpdateCommands();

        Debug.WriteLine("[CANCEL] END");
    }

    private bool CanRefresh()
    {
        return !IsInEditCountry && CountrySelected != null;
    }

    private async Task RefreshAsync()
    {
        if (CountrySelected == null)
            return;

        var id = CountrySelected.CountryId;
        IsLoading = true;

        try
        {
            // 1️⃣ Liste neu laden
            var freshList = await _dataService.GetCountriesByNameAsync(CurrentQuery);

            Countries.Clear();
            foreach (var c in freshList)
                Countries.Add(c);

            // 2️⃣ Auswahl wiederherstellen
            CountrySelected = Countries.FirstOrDefault(c => c.CountryId == id);

            // 3️⃣ Formular aktualisieren
            if (CountrySelected != null)
            {
                _backupCountry = CountrySelected.Clone();   // Backup aktualisieren (wie vorherige Logik)
                SetEditModel(_backupCountry);               // Edit-Model neu aus Domain-Modell befüllen
            }
            else
            {
                CountryEdit = null;
            }

            // 4️⃣ Zähler aktualisieren
            CountryDataSetCount = Countries.Count;
 //           OnPropertyChanged(nameof(CountryDataSetCountText));
        }
        finally
        {
            IsLoading = false;

            // 5️⃣ Commands aktualisieren
            UpdateCommands();
        }
    }

    private bool _isUserEditing = false;

    public void OnNameLostFocus()
    {
        EndUserEdit();
        ValidateName();
    }

    public void OnRegexLostFocus()
    {
        EndUserEdit();
        ValidateRegex();
    }
    public void OnZoomLevelLostFocus()
    {
        EndUserEdit();
        ValidateZoomLevel();
    }

    public void OnLatitudeLostFocus()
    {
        EndUserEdit();
        ValidateLatitude();
    }

    public void OnLongitudeLostFocus()
    {
        EndUserEdit();
        ValidateLongitude();
    }

    private async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "OK",
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Length / FieldUiInfo / LoadFieldLengths / ApplyWidths]

    public class FieldUiInfo
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        // Optional: Wenn gesetzt, überschreibt es die automatische Berechnung
        public int? MinLengthOverride { get; set; }

        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
    }
    private void LoadFieldLengths()
    {
        var props = typeof(TblCountry).GetProperties();
        foreach (var prop in props)
        {
            if (prop.PropertyType != typeof(string)) continue;
            int max = EfMetadataHelper.GetMaxLength(_db, typeof(TblCountry), prop.Name);
            _fieldUi[prop.Name] = new FieldUiInfo
            {
                MaxLength = max,
                MinLength = max / 2 // // Standardregel: halbe EF-Länge
            };
        }

        OnPropertyChanged(nameof(NameUi));
        OnPropertyChanged(nameof(FieldUi));
    }
    public async void ApplyWidths(double pageWidth)
    {
        _applyWidthsCts?.Cancel();
        _applyWidthsCts = new CancellationTokenSource();
        var token = _applyWidthsCts.Token;

        try
        {
            // 50 ms warten – wenn in der Zeit neue Events kommen, wird abgebrochen
            await Task.Delay(50, token);

            ApplyWidthsInternal(pageWidth);
        }
        catch (TaskCanceledException)
        {
            // Debounce – ignorieren
        }
    }
    private void ApplyWidthsInternal(double pageWidth)
    {
        foreach (var kvp in _fieldUi)
        {
            var info = kvp.Value;

            int minChars = info.MinLengthOverride ?? info.MinLength;

            var widths = WidthHelper.CalculateWidths(
                info.MaxLength,
                minChars,
                pageWidth
            );

            info.MinWidth = widths.MinWidth;
            info.MaxWidth = widths.MaxWidth;
        }

        OnPropertyChanged(nameof(NameUi));
        OnPropertyChanged(nameof(RegexUi));
        OnPropertyChanged(nameof(ZoomLevelUi));
        OnPropertyChanged(nameof(LatitudeUi));
        OnPropertyChanged(nameof(LongitudeUi));
        OnPropertyChanged(nameof(FieldUi));

        Debug.WriteLine($"ApplyWidths called, pageWidth={pageWidth}");
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Validation / Service]
    public IReadOnlyDictionary<string, FieldUiInfo> FieldUi => _fieldUi;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Any();

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _errors.SelectMany(kv => kv.Value);

        return _errors.TryGetValue(propertyName, out var list)
            ? list
            : Enumerable.Empty<string>();
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public void SetErrors(string field, IEnumerable<string> errors)
    {
        if (errors.Any())
            _errors[field] = errors.ToList();
        else
            _errors.Remove(field);

        OnErrorsChanged(field);
        OnPropertyChanged($"{field}Errors");
        OnPropertyChanged($"{field}ErrorsString");
        OnPropertyChanged($"{field}HasErrors");

        SaveCountryCommand.NotifyCanExecuteChanged();
        DeleteCountryCommand.NotifyCanExecuteChanged();
    }

    private void ClearAllErrors()
    {
        _errors.Clear();
        OnErrorsChanged(string.Empty);
    }

    public void ValidateName()
    {
        // 1️⃣ Während Laden/Cancel/Refresh NICHT validieren
        if (!_isUserEditing)
            return;

        var errors = new List<string>();

        // 2️⃣ Regeln
        if (string.IsNullOrWhiteSpace(CountryEdit?.Name))
            errors.Add("Name darf nicht leer sein.");
        else if (CountryEdit.Name.Length < 3)
            errors.Add("Name muss mindestens 3 Zeichen lang sein.");

        // 3️⃣ Fehler setzen
        SetErrors(nameof(CountryEdit.Name), errors);

        // 4️⃣ UI informieren
        OnPropertyChanged(nameof(NameErrors));
        OnPropertyChanged(nameof(HasErrors));

        // 5️⃣ Save/Cancel/Commands aktualisieren
        UpdateCommands();
    }
    public void ValidateRegex()
    {
        if (!_isUserEditing)
            return;

        var errors = new List<string>();
        var value = CountryEdit?.Regex;

        // 1️⃣ Leer → erlaubt
        if (string.IsNullOrWhiteSpace(value))
        {
            SetErrors(nameof(CountryEdit.Regex), errors);
            OnPropertyChanged(nameof(RegexErrors));
            OnPropertyChanged(nameof(HasErrors));
            UpdateCommands();
            return;
        }

        // 2️⃣ Regex-Regel für internationale Vorwahlen
        const string pattern = @"^\+\d{1,4}([ -]?\d{1,4})*(\(\d{1,4}\))?$";

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
            errors.Add("Ungültige Ländervorwahl. Beispiel: +355, +1-246, +971(4)");

        // 3️⃣ Fehler setzen
        SetErrors(nameof(CountryEdit.Regex), errors);
        OnPropertyChanged(nameof(RegexErrors));
        OnPropertyChanged(nameof(HasErrors));
        UpdateCommands();
    }
    public void ValidateZoomLevel()
    {
        // 1️⃣ Während Laden/Cancel/Refresh NICHT validieren
        if (!_isUserEditing)
            return;

        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(CountryEdit?.ZoomLevel))
        {
            var text = CountryEdit.ZoomLevel.Replace(',', '.');

            if (text.Contains("e", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Wissenschaftliche Notation (E) ist nicht erlaubt.");
            }
            else if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var zoom))
            {
                errors.Add("ZoomLevel muss eine ganze Zahl sein.");
            }
            else if (zoom < 0 || zoom > 20)
            {
                errors.Add("ZoomLevel muss zwischen 0 und 20 liegen.");
            }
        }

        SetErrors(nameof(CountryEdit.ZoomLevel), errors);

        OnPropertyChanged(nameof(ZoomLevelErrors));
        OnPropertyChanged(nameof(HasErrors));

        // 5️⃣ Save/Cancel/Commands aktualisieren
        UpdateCommands();

    }
    public void ValidateLatitude()
    {
        // 1️⃣ Während Laden/Cancel/Refresh NICHT validieren
        if (!_isUserEditing)
            return;

        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(CountryEdit?.Latitude))
        {
            var text = CountryEdit.Latitude.Replace(',', '.');

            // ⭐ Zwischenzustand erlauben: nur "-"
            if (text == "-")
            {
                SetErrors(nameof(CountryEdit.Latitude), errors);
                return;
            }

            if (text.Contains("e", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Wissenschaftliche Notation (E) ist nicht erlaubt.");
            }
            else if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
            {
                errors.Add("Latitude ist keine gültige Zahl.");
            }
            else if (lat < -90 || lat > 90)
            {
                errors.Add("Latitude muss zwischen -90 und 90 liegen.");
            }
        }

        SetErrors(nameof(CountryEdit.Latitude), errors);

        OnPropertyChanged(nameof(LatitudeErrors));
        OnPropertyChanged(nameof(HasErrors));

        // 5️⃣ Save/Cancel/Commands aktualisieren
        UpdateCommands();

    }
    public void ValidateLongitude()
    {
        // 1️⃣ Während Laden/Cancel/Refresh NICHT validieren
        if (!_isUserEditing)
            return;

        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(CountryEdit?.Longitude))
        {
            var text = CountryEdit.Longitude.Replace(',', '.');

            // ⭐ Zwischenzustand erlauben
            if (text == "-")
            {
                SetErrors(nameof(CountryEdit.Longitude), errors);
                return;
            }

            if (text.Contains("e", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Wissenschaftliche Notation (E) ist nicht erlaubt.");
            }
            else if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
            {
                errors.Add("Longitude ist keine gültige Zahl.");
            }
            else if (lon < -180 || lon > 180)
            {
                errors.Add("Longitude muss zwischen -180 und 180 liegen.");
            }
        }

        SetErrors(nameof(CountryEdit.Longitude), errors);

        OnPropertyChanged(nameof(LongitudeErrors));
        OnPropertyChanged(nameof(HasErrors));

        // 5️⃣ Save/Cancel/Commands aktualisieren
        UpdateCommands();

    }

    //neu hinzu
    public string NameErrorsString =>
        string.Join("\n", GetErrors(nameof(CountryEdit.Name)).Cast<string>());

    public string RegexErrorsString =>
        string.Join("\n", GetErrors(nameof(CountryEdit.Regex)).Cast<string>());

    public string ZoomLevelErrorsString =>
        string.Join("\n", GetErrors(nameof(CountryEdit.ZoomLevel)).Cast<string>());

    public string LatitudeErrorsString =>
        string.Join("\n", GetErrors(nameof(CountryEdit.Latitude)).Cast<string>());

    public string LongitudeErrorsString =>
        string.Join("\n", GetErrors(nameof(CountryEdit.Longitude)).Cast<string>());

    public bool NameHasErrors =>
        GetErrors(nameof(CountryEdit.Name)).Cast<string>().Any();

    public bool RegexHasErrors =>
        GetErrors(nameof(CountryEdit.Regex)).Cast<string>().Any();

    public bool ZoomLevelHasErrors =>
        GetErrors(nameof(CountryEdit.ZoomLevel)).Cast<string>().Any();

    public bool LatitudeHasErrors =>
        GetErrors(nameof(CountryEdit.Latitude)).Cast<string>().Any();

    public bool LongitudeHasErrors =>
        GetErrors(nameof(CountryEdit.Longitude)).Cast<string>().Any();


    public IReadOnlyList<string> NameErrors
        => _errors.TryGetValue(nameof(CountryEdit.Name), out var list)
            ? list
            : new List<string>();

    public IReadOnlyList<string> RegexErrors
        => _errors.TryGetValue(nameof(CountryEdit.Regex), out var list)
            ? list
            : new List<string>();

    public IReadOnlyList<string> ZoomLevelErrors
        => _errors.TryGetValue(nameof(CountryEdit.ZoomLevel), out var list)
            ? list
            : new List<string>();

    public IReadOnlyList<string> LatitudeErrors
        => _errors.TryGetValue(nameof(CountryEdit.Latitude), out var list)
            ? list
            : new List<string>();

    public IReadOnlyList<string> LongitudeErrors
    {
        get =>
            _errors.TryGetValue(nameof(CountryEdit.Longitude), out var list)
                ? list
                : new List<string>();
        set => throw new NotImplementedException();
    }

    public void OnFieldChanged()
    {
        if (!_isUserEditing)
            return;

        ValidateName();
        ValidateRegex();
        ValidateZoomLevel();
        ValidateLatitude();
        ValidateLongitude();
    }


    #endregion [ Validation ]
    // =========================================================

    // =========================================================
    #region [ UI State ]
    // =========================================================
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    private bool _isNewCountry;
    public bool IsNewCountry
    {
        get => _isNewCountry;
        set => SetProperty(ref _isNewCountry, value);
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
                UpdateCommands();
            }
        }
    }

    public bool CanEditCountry => CountrySelected != null && !IsInEditCountry;

    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string? _currentQuery;
    public string? CurrentQuery
    {
        get => _currentQuery;
        set => SetProperty(ref _currentQuery, value);
    }

    private int _countryDataSetCount;
    public int CountryDataSetCount
    {
        get => _countryDataSetCount;
        set => SetProperty(ref _countryDataSetCount, value);
    }

    private double _currentPageWidth;
    public double CurrentPageWidth
    {
        get => _currentPageWidth;
        set
        {
            if (SetProperty(ref _currentPageWidth, value))
                ApplyWidths(value);
        }
    }

    private int _nameMaxChars;
    public int NameMaxChars
    {
        get => _nameMaxChars;
        private set => SetProperty(ref _nameMaxChars, value);
    }

    private int _nameMinChars = 1;
    public int NameMinChars
    {
        get => _nameMinChars;
        private set => SetProperty(ref _nameMinChars, value);
    }

    private double _nameMaxWidth;
    public double NameMaxWidth
    {
        get => _nameMaxWidth;
        private set => SetProperty(ref _nameMaxWidth, value);
    }

    private double _nameMinWidth;
    public double NameMinWidth
    {
        get => _nameMinWidth;
        private set => SetProperty(ref _nameMinWidth, value);
    }
    private int _nameMaxLength;
    public int NameMaxLength
    {
        get => _nameMaxLength;
        private set => SetProperty(ref _nameMaxLength, value);
    }

    private int _nameMinLength = 1;
    public int NameMinLength
    {
        get => _nameMinLength;
        private set => SetProperty(ref _nameMinLength, value);
    }
    public FieldUiInfo this[string propertyName]
        => _fieldUi[propertyName];


    #endregion
    // =========================================================

    // =========================================================
    #region [ Collections ]
    // =========================================================
    public ObservableCollection<TblCountry> Countries { get; } = new();

    #endregion
    // =========================================================

    // =========================================================
    #region [ Commands / Callbacks ]
    // =========================================================
    private void UpdateCommands()
    {
        StartEditCountryCommand.NotifyCanExecuteChanged();
        CancelEditCountryCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    //private void UpdateCommands()
    //{
    //    (StartEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
    //    (SaveCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    //    (DeleteCountryCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    //    (CancelEditCountryCommand as RelayCommand)?.NotifyCanExecuteChanged();
    //    (RefreshCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
    //}

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<string> SearchCommand { get; }
    public IRelayCommand AddCountryCommand { get; }
    public IRelayCommand CancelEditCountryCommand { get; }
    public IRelayCommand<TblCountry> OpenCountryCommand { get; }
    public IAsyncRelayCommand SaveCountryCommand { get; }
    public IAsyncRelayCommand DeleteCountryCommand { get; }
    public IRelayCommand StartEditCountryCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    public Func<Task<ContentDialogResult>>? RequestSaveDialog { get; set; }
    public Func<Task<bool>>? RequestDeleteDialog { get; set; }
    public Action? RequestFocusName { get; set; }
    public Action? ScrollToSelectedRequest { get; set; }
    public Action<double>? ApplyPageWidthRequest { get; set; }


    #endregion
    // =========================================================
}



