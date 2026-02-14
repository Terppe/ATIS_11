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
using WinUI3Localizer;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.ViewModels;

public partial class TblCountryViewModel : ObservableObject, INotifyDataErrorInfo
{
    // =========================================================
    #region [ Private Fields ]
    // =========================================================

    private readonly IDataService _dataService;
    private readonly AtisDbContext _db;
    private readonly Dictionary<string, FieldUiInfo> _fieldUi = new();
    public FieldUiInfo NameUi => _fieldUi["Name"];
    public FieldUiInfo RegexUi => _fieldUi["Regex"];
    public FieldUiInfo ZoomLevelUi => _fieldUi["ZoomLevel"];
    public FieldUiInfo LatitudeUi => _fieldUi["Latitude"];
    public FieldUiInfo LongitudeUi => _fieldUi["Longitude"];
    public ILocalizer Loc => Localizer.Get();

    private readonly Dictionary<string, List<string>> _errors = new();

    private TblCountry? _backupCountry;
    private double _lastPageWidth = -1;

    private bool _isDetailVisible;
    private bool _isNewCountry;
    private string? _currentQuery;
    private int _countryDataSetCount;
    private double _currentPageWidth;
    private int _nameMaxChars;
    private int _nameMinChars = 1;
    private double _nameMaxWidth;
    private double _nameMinWidth;
    private int _nameMaxLength;
    private int _nameMinLength = 1;
    private readonly DialogService _dialog;
    #endregion
    // =========================================================

    #region [ Constructor ]
    // =========================================================

    public TblCountryViewModel(IDataService dataService, AtisDbContext db, DialogService dialog)
    {
        _dataService = dataService;
        _db = db;
        _dialog = dialog;

        Countries.CollectionChanged += (_, __) =>
        {
            Debug.WriteLine($"[CountryPageVM] IsLoading Default: {IsLoading}");
        };

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
    #region [ SetEditModel Handling ]
    // =========================================================

    private void SetEditModel(TblCountry? entity)
    {
        if (entity == null)
        {
            CountryEdit = new CountryEditModel();
            return;
        }

        CountryEdit = new CountryEditModel
        {
            Name = entity.Name ?? string.Empty,
            Regex = entity.Regex ?? string.Empty,

            ZoomLevel = entity.ZoomLevel?.ToString() ?? string.Empty,
            Latitude = entity.Latitude?.ToString() ?? string.Empty,
            Longitude = entity.Longitude?.ToString() ?? string.Empty
        };
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Selection / Open / Sort ]
    // =========================================================
    private bool _suppressSelection;

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
        Countries = new ObservableCollection<TblCountry>();

        SetEditModel(null);

        CountrySelected = null;
        IsInEditCountry = false;
        IsNewCountry = false;
        CountryDataSetCount = 0;

        UpdateCommands();

        return Task.CompletedTask;
    }

    public async Task LoadSearchAsync(string? search)
    {
        try
        {
            IsLoading = true;
            _suppressSelection = true;

            var list = await _dataService.GetCountriesByNameOrIdAsync(search);
            Countries = new ObservableCollection<TblCountry>(list);

            CountrySelected = null;
            IsInEditCountry = false;
            IsNewCountry = false;

            UpdateCommands();
        }
        finally
        {
            _suppressSelection = false;
            IsLoading = false;
        }
    }

    //public async Task LoadSearchAsync(string? search)
    //{
    //    CurrentQuery = search;
    //    IsLoading = true;
    //    try
    //    {
    //        Countries.Clear();
    //        if (!string.IsNullOrWhiteSpace(search))
    //        {
    //            var list = await _dataService.GetCountriesByNameOrIdAsync(search);

    //            foreach (var item in list)
    //                Countries.Add(item);

    //            CountryDataSetCount = Countries.Count;
    //        }
    //        else
    //        {
    //            CountryDataSetCount = 0;
    //        }

    //        if (!IsInEditCountry)
    //            CountrySelected = null;

    //        OnPropertyChanged(nameof(CountryDataSetCountText));
    //    }
    //    finally
    //    {
    //        IsLoading = false;
    //    }
    //}

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

            OnPropertyChanged(nameof(CountryDataSetCountText));
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
        // Nur für "Neuen Datensatz hinzufügen" – NICHT beim Laden!
        var model = new TblCountry();

        IsNewCountry = true;
        IsInEditCountry = true;

        CountrySelected = model;
        SetEditModel(model);

        _backupCountry = model.Clone();

        ValidateAll();
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
        if (!IsInEditCountry)
            return false;

        if (CountryEdit == null)
            return false;

        return !HasErrors;
    }
    private async Task SaveCountryAsync()
    {
        if (CountryEdit == null)
            return;

        // 1️⃣ Nur prüfen – NICHT validieren
        if (HasErrors)
        {
        //    await ShowMessageAsync("Fehler", "Bitte korrigiere die Fehler, bevor du speicherst.");
            await _dialog.ShowMessageAsync("Dialog_Error_Title", "Dialog_Error_FixBeforeSave");
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
        var model = CountrySelected != null
            ? CountrySelected.Clone()
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
            await _dialog.ShowMessageAsync("Dialog_Error_Title", "Dialog_Error_Duplicate");
      //      await ShowMessageAsync("Fehler", "Ein identischer Datensatz existiert bereits.");
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

        CountrySelected = saved; // Domain aktualisieren
        SetEditModel(saved); // UI aktualisieren

        // 9️⃣ Edit‑Modus verlassen
        IsInEditCountry = false;
        IsNewCountry = false;
        _isUserEditing = false;

        UpdateCommands();

        // 1️⃣0️⃣ Meldung
        await _dialog.ShowMessageAsync("Dialog_Save_Title", "Dialog_Save_Success");
 //       await ShowMessageAsync("Gespeichert", "Der Datensatz wurde erfolgreich gespeichert.");
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
    //    await ShowMessageAsync("Gelöscht", "Der Datensatz wurde erfolgreich gelöscht.");
        await _dialog.ShowMessageAsync("Dialog_Delete_Title", "Dialog_Delete_Success");
    }

    private void CancelEdit()
    {
        Debug.WriteLine("[CANCEL] START");

        if (_backupCountry != null)
        {
            // 1️⃣ Domain wiederherstellen
            CountrySelected = _backupCountry.Clone();

            // 2️⃣ UI wiederherstellen
            SetEditModel(CountrySelected);
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
                _backupCountry = CountrySelected.Clone();   // Backup aktualisieren
                SetEditModel(CountrySelected);              // ⭐ KORREKT
            }
            else
            {
                // Kein Datensatz mehr vorhanden
                CountryEdit = null;
            }

            // 4️⃣ Zähler aktualisieren
            CountryDataSetCount = Countries.Count;
            OnPropertyChanged(nameof(CountryDataSetCountText));
        }
        finally
        {
            IsLoading = false;

            // 5️⃣ Commands aktualisieren
            UpdateCommands();
        }
    }

    private bool _isUserEditing = false;
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
        //
        // ⭐ HIER kommen deine individuellen Overrides hin
        //
        // Beispiel: Name soll 2/3 der EF-Länge als Mindestlänge haben
        // _fieldUi["Name"].MinLengthOverride = (int)(_fieldUi["Name"].MaxLength * 0.66);
        // Beispiel: Regex soll 80% der EF-Länge als Mindestlänge haben
        // _fieldUi["Regex"].MinLengthOverride = (int)(_fieldUi["Regex"].MaxLength * 0.8);
        // Beispiel: ZoomLevel soll immer mindestens 3 Zeichen breit sein
        // _fieldUi["ZoomLevel"].MinLengthOverride = 3;

        OnPropertyChanged(nameof(NameUi));
        OnPropertyChanged(nameof(FieldUi));

    }

    private void ApplyWidths(double pageWidth)
    {
        // 1. Schutz: Wenn PageWidth sich nicht geändert hat → NICHT neu berechnen
        if (Math.Abs(pageWidth - _lastPageWidth) < 1)
            return;

        _lastPageWidth = pageWidth;

        // 2. Breiten für alle dynamischen Felder berechnen
        foreach (var kvp in _fieldUi)
        {
            var info = kvp.Value;

            var widths = WidthHelper.CalculateWidths(
                info.MaxLength,
                info.MinLengthOverride ?? info.MinLength,
                pageWidth
            );

            info.MinWidth = widths.MinWidth;
            info.MaxWidth = widths.MaxWidth;
        }

        // 3. Debug-Ausgabe für Diagnose
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

    private void ValidateAll()
    {
        if (CountryEdit == null)
            return;

        // 🔒 Während Laden/Cancel/Refresh keine Validierung
        if (!_isUserEditing)
            return;

        ValidateName();
        ValidateRegex();
        ValidateZoomLevel();
        ValidateLatitude();
        ValidateLongitude();

        // 🔁 UI & Commands aktualisieren
        OnPropertyChanged(nameof(HasErrors));
        UpdateCommands();
    }

    //public IReadOnlyList<string> NameErrors
    //    => _errors.TryGetValue(nameof(CountryEdit.Name), out var list)
    //        ? list
    //        : new List<string>();
    //public IReadOnlyList<string> RegexErrors
    //    => _errors.TryGetValue(nameof(CountryEdit.Regex), out var list)
    //        ? list
    //        : new List<string>();

    //public IReadOnlyList<string> ZoomLevelErrors
    //    => _errors.TryGetValue(nameof(CountryEdit.ZoomLevel), out var list)
    //        ? list
    //        : new List<string>();

    //public IReadOnlyList<string> LatitudeErrors
    //    => _errors.TryGetValue(nameof(CountryEdit.Latitude), out var list)
    //        ? list
    //        : new List<string>();

    //public IReadOnlyList<string> LongitudeErrors
    //{
    //    get =>
    //        _errors.TryGetValue(nameof(CountryEdit.Longitude), out var list)
    //            ? list
    //            : new List<string>();
    //    set => throw new NotImplementedException();
    //}

    //private string _countryNamePlaceholder;
    //public string CountryNamePlaceholder
    //{
    //    get => _countryNamePlaceholder;
    //    set => SetProperty(ref _countryNamePlaceholder, value);
    //}

    //private string _countryRegexPlaceholder;
    //public string CountryRegexPlaceholder
    //{
    //    get => _countryRegexPlaceholder;
    //    set => SetProperty(ref _countryRegexPlaceholder, value);
    //}

    //private string _colCountryName;
    //public string ColCountryName
    //{
    //    get => _colCountryName;
    //    set => SetProperty(ref _colCountryName, value);
    //}

    //private string _colCountryRegex;
    //public string ColCountryRegex
    //{
    //    get => _colCountryRegex;
    //    set => SetProperty(ref _colCountryRegex, value);
    //}

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

    private string _name = "";

    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    public bool IsNewCountry
    {
        get => _isNewCountry;
        set => SetProperty(ref _isNewCountry, value);
    }



    public string? CurrentQuery
    {
        get => _currentQuery;
        set => SetProperty(ref _currentQuery, value);
    }

    public int CountryDataSetCount
    {
        get => _countryDataSetCount;
        set => SetProperty(ref _countryDataSetCount, value);
    }

    public string CountryDataSetCountText => $"{CountryDataSetCount} Datensätze gefunden";

    public double CurrentPageWidth
    {
        get => _currentPageWidth;
        set
        {
            if (SetProperty(ref _currentPageWidth, value))
                ApplyWidths(value);
        }
    }

    public int NameMaxChars
    {
        get => _nameMaxChars;
        private set => SetProperty(ref _nameMaxChars, value);
    }

    public int NameMinChars
    {
        get => _nameMinChars;
        private set => SetProperty(ref _nameMinChars, value);
    }

    public double NameMaxWidth
    {
        get => _nameMaxWidth;
        private set => SetProperty(ref _nameMaxWidth, value);
    }

    public double NameMinWidth
    {
        get => _nameMinWidth;
        private set => SetProperty(ref _nameMinWidth, value);
    }
    public int NameMaxLength
    {
        get => _nameMaxLength;
        private set => SetProperty(ref _nameMaxLength, value);
    }

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
    #region [CULTURE APPLYCULTURE + PROPERTIES (Labels, Titles, Static Text)]
    // =========================================================
    public void ApplyCulture()
    {
        Debug.WriteLine($"🔥 ApplyCulture wurde ausgeführt!\r\n");

        // ⭐ Page‑Titel
        CountryPageTitle = Localizer.Get().GetLocalizedString("Country_Page_Title");
        DataSetCountLabel = Localizer.Get().GetLocalizedString("DataSetCount_Label");

        // ⭐ Detail‑Titel
        CountryDetailTitle = Localizer.Get().GetLocalizedString("Country_Detail_Title");

        // ⭐ Buttons (Page‑Level)
        ButtonAddLabel = Localizer.Get().GetLocalizedString("Button_Add");
        ButtonSaveLabel = Localizer.Get().GetLocalizedString("Button_Save");
        ButtonDeleteLabel = Localizer.Get().GetLocalizedString("Button_Delete");
        ButtonRefreshLabel = Localizer.Get().GetLocalizedString("Button_Refresh");

        // ⭐ Buttons (Detail‑Bereich)
        ButtonCancelLabel = Localizer.Get().GetLocalizedString("Button_Cancel");
        ButtonEditLabel = Localizer.Get().GetLocalizedString("Button_Edit");

        // ⭐ SearchBox
        SearchPlaceholder = Localizer.Get().GetLocalizedString("Search_Placeholder");

        // ⭐ DataGrid‑Header
        ColCountryId = Localizer.Get().GetLocalizedString("Col_CountryId");
        ColCountryName = Localizer.Get().GetLocalizedString("Col_CountryName");
        ColCountryRegex = Localizer.Get().GetLocalizedString("Col_CountryRegex");
        ColCountryZoomLevel = Localizer.Get().GetLocalizedString("Col_CountryZoomLevel");
        ColCountryLatitude = Localizer.Get().GetLocalizedString("Col_CountryLatitude");
        ColCountryLongitude = Localizer.Get().GetLocalizedString("Col_CountryLongitude");

        // ⭐ ContextFlyout‑Labels
        CtxAddLabel = Localizer.Get().GetLocalizedString("Ctx_Add");
        CtxEditLabel = Localizer.Get().GetLocalizedString("Ctx_Edit");
        CtxSaveLabel = Localizer.Get().GetLocalizedString("Ctx_Save");
        CtxDeleteLabel = Localizer.Get().GetLocalizedString("Ctx_Delete");
        CtxRefreshLabel = Localizer.Get().GetLocalizedString("Ctx_Refresh");

        // ⭐ Placeholder im Detailbereich
        CountryNamePlaceholder = Localizer.Get().GetLocalizedString("CountryName_Placeholder");
        CountryRegexPlaceholder = Localizer.Get().GetLocalizedString("CountryRegex_Placeholder");
        CountryZoomLevelPlaceholder = Localizer.Get().GetLocalizedString("CountryZoomLevel_Placeholder");
        CountryLatitudePlaceholder = Localizer.Get().GetLocalizedString("CountryLatitude_Placeholder");
        CountryLongitudePlaceholder = Localizer.Get().GetLocalizedString("CountryLongitude_Placeholder");
    }

    //------ Page‑Titel & Header--------
    private string _countryPageTitle;
    private string _dataSetCountLabel;
    private string _countryDetailTitle;

    public string CountryPageTitle
    {
        get => _countryPageTitle;
        set => SetProperty(ref _countryPageTitle, value);
    }

    public string DataSetCountLabel
    {
        get => _dataSetCountLabel;
        set => SetProperty(ref _dataSetCountLabel, value);
    }

    public string CountryDetailTitle
    {
        get => _countryDetailTitle;
        set => SetProperty(ref _countryDetailTitle, value);
    }
    //------ Buttons (Page‑Level)--------
    private string _buttonAddLabel;
    public string ButtonAddLabel
    {
        get => _buttonAddLabel;
        set => SetProperty(ref _buttonAddLabel, value);
    }

    private string _buttonSaveLabel;
    public string ButtonSaveLabel
    {
        get => _buttonSaveLabel;
        set => SetProperty(ref _buttonSaveLabel, value);
    }

    private string _buttonDeleteLabel;
    public string ButtonDeleteLabel
    {
        get => _buttonDeleteLabel;
        set => SetProperty(ref _buttonDeleteLabel, value);
    }

    private string _buttonRefreshLabel;
    public string ButtonRefreshLabel
    {
        get => _buttonRefreshLabel;
        set => SetProperty(ref _buttonRefreshLabel, value);
    }

    //------ Buttons (Detail‑Bereich) --------
    private string _buttonCancelLabel;
    public string ButtonCancelLabel
    {
        get => _buttonCancelLabel;
        set => SetProperty(ref _buttonCancelLabel, value);
    }

    private string _buttonEditLabel;
    public string ButtonEditLabel
    {
        get => _buttonEditLabel;
        set => SetProperty(ref _buttonEditLabel, value);
    }

    //------ SearchBox --------
    private string _searchPlaceholder;
    public string SearchPlaceholder
    {
        get => _searchPlaceholder;
        set => SetProperty(ref _searchPlaceholder, value);
    }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }
    //------ DataGrid‑Header --------
    private string _colCountryId;
    public string ColCountryId
    {
        get => _colCountryId;
        set => SetProperty(ref _colCountryId, value);
    }

    private string _colCountryName;
    public string ColCountryName
    {
        get => _colCountryName;
        set
        {
            _colCountryName = value;
            OnPropertyChanged();
        }
    }

    private string _colCountryRegex;
    public string ColCountryRegex
    {
        get => _colCountryRegex;
        set => SetProperty(ref _colCountryRegex, value);
    }

    private string _colCountryZoomLevel;
    public string ColCountryZoomLevel
    {
        get => _colCountryZoomLevel;
        set => SetProperty(ref _colCountryZoomLevel, value);
    }

    private string _colCountryLatitude;
    public string ColCountryLatitude
    {
        get => _colCountryLatitude;
        set => SetProperty(ref _colCountryLatitude, value);
    }

    private string _colCountryLongitude;
    public string ColCountryLongitude
    {
        get => _colCountryLongitude;
        set => SetProperty(ref _colCountryLongitude, value);
    }
    //------ ContextFlyout‑Labels --------
    private string _ctxAddLabel;
    public string CtxAddLabel
    {
        get => _ctxAddLabel;
        set => SetProperty(ref _ctxAddLabel, value);
    }

    private string _ctxEditLabel;
    public string CtxEditLabel
    {
        get => _ctxEditLabel;
        set => SetProperty(ref _ctxEditLabel, value);
    }

    private string _ctxSaveLabel;
    public string CtxSaveLabel
    {
        get => _ctxSaveLabel;
        set => SetProperty(ref _ctxSaveLabel, value);
    }

    private string _ctxDeleteLabel;
    public string CtxDeleteLabel
    {
        get => _ctxDeleteLabel;
        set => SetProperty(ref _ctxDeleteLabel, value);
    }

    private string _ctxRefreshLabel;
    public string CtxRefreshLabel
    {
        get => _ctxRefreshLabel;
        set => SetProperty(ref _ctxRefreshLabel, value);
    }
    //------ Placeholder im Detailbereich --------
    private string _countryNamePlaceholder;
    public string CountryNamePlaceholder
    {
        get => _countryNamePlaceholder;
        set => SetProperty(ref _countryNamePlaceholder, value);
    }

    private string _countryRegexPlaceholder;
    public string CountryRegexPlaceholder
    {
        get => _countryRegexPlaceholder;
        set => SetProperty(ref _countryRegexPlaceholder, value);
    }

    private string _countryZoomLevelPlaceholder;
    public string CountryZoomLevelPlaceholder
    {
        get => _countryZoomLevelPlaceholder;
        set => SetProperty(ref _countryZoomLevelPlaceholder, value);
    }

    private string _countryLatitudePlaceholder;
    public string CountryLatitudePlaceholder
    {
        get => _countryLatitudePlaceholder;
        set => SetProperty(ref _countryLatitudePlaceholder, value);
    }

    private string _countryLongitudePlaceholder;
    public string CountryLongitudePlaceholder
    {
        get => _countryLongitudePlaceholder;
        set => SetProperty(ref _countryLongitudePlaceholder, value);
    }
    //------ Edit‑State / Auswahl --------
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

    private bool _canEditCountry;
    public bool CanEditCountry
    {
        get => _canEditCountry;
        set => SetProperty(ref _canEditCountry, value);
    }

    private TblCountry _countrySelected;
    public TblCountry? CountrySelected
    {
        get => _countrySelected;
        set
        {
            if (_suppressSelection)
                return;

            if (SetProperty(ref _countrySelected, value))
            {
                SetEditModel(value);   // ← WICHTIG: auch bei null!
                UpdateCommands();
            }
        }
    }

    private CountryEditModel? _countryEdit;

    public CountryEditModel? CountryEdit
    {
        get => _countryEdit;
        private set => SetProperty(ref _countryEdit, value);
    }

    //------ Fehlerlisten --------
    private List<string> _nameErrors;
    public List<string> NameErrors
    {
        get => _nameErrors;
        set => SetProperty(ref _nameErrors, value);
    }

    private List<string> _regexErrors;
    public List<string> RegexErrors
    {
        get => _regexErrors;
        set => SetProperty(ref _regexErrors, value);
    }

    private List<string> _zoomLevelErrors;
    public List<string> ZoomLevelErrors
    {
        get => _zoomLevelErrors;
        set => SetProperty(ref _zoomLevelErrors, value);
    }

    private List<string> _latitudeErrors;
    public List<string> LatitudeErrors
    {
        get => _latitudeErrors;
        set => SetProperty(ref _latitudeErrors, value);
    }

    private List<string> _longitudeErrors;
    public List<string> LongitudeErrors
    {
        get => _longitudeErrors;
        set => SetProperty(ref _longitudeErrors, value);
    }
    //------ Ladezustand --------

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    #endregion [CULTURE PROPERTIES]
    // =========================================================

    // =========================================================
    #region [ Collections ]
    // =========================================================

    private ObservableCollection<TblCountry> _countries = new();

    public ObservableCollection<TblCountry> Countries
    {
        get => _countries;
        set => SetProperty(ref _countries, value);
    }

    #endregion
    // =========================================================

    // =========================================================
    #region [ Commands / Callbacks ]
    // =========================================================

    private IEnumerable<object> AllCommands => new object[]
    {
        StartEditCountryCommand,
        SaveCountryCommand,
        DeleteCountryCommand,
        CancelEditCountryCommand,
        RefreshCommand
    };

    private void UpdateCommands()
    {
        foreach (var cmd in AllCommands)
            Notify(cmd);
    }

    private void Notify(object command)
    {
        if (command is IRelayCommand relay)
            relay.NotifyCanExecuteChanged();
    }
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



