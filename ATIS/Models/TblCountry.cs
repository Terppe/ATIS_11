using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;


public class TblCountry : ObservableObject, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------

    [Key]
    public int CountryId { get; set; }

    // ---------------------------------------------------------
    // CountryName
    // ---------------------------------------------------------

    [StringLength(100)]
    private string? _name;
    public string? Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            ValidateName();
        }
    }

    private string? _nameError;
    [NotMapped]
    public string? NameError
    {
        get => _nameError;
        set => SetProperty(ref _nameError, value);
    }

    private bool _hasNameError;
    [NotMapped]
    public bool HasNameError
    {
        get => _hasNameError;
        set
        {
            if (_hasNameError != value)
            {
                _hasNameError = value;
                OnPropertyChanged(); // WICHTIG!
            }
        }
    }


    // ---------------------------------------------------------               
    // Continent
    // ---------------------------------------------------------               
    private string? _continent;

    /// Ein Erdteil oder mehrere, getrennt durch ";" (z. B. "Europe;Asia" für Türkei).
    [StringLength(100)]
    public string? Continent
    {
        get => _continent;
        set
        {
            SetProperty(ref _continent, value);
            ValidateContinent();
        }
    }

    private string? _continentError;
    [NotMapped]
    public string? ContinentError
    {
        get => _continentError;
        set => SetProperty(ref _continentError, value);
    }

    private bool _hasContinentError;
    [NotMapped]
    public bool HasContinentError
    {
        get => _hasContinentError;
        set
        {
            if (_hasContinentError != value)
            {
                _hasContinentError = value;
                OnPropertyChanged(); // WICHTIG!
            }
        }
    }

    // ---------------------------------------------------------
    // Regex
    // ---------------------------------------------------------

    [StringLength(250)]
    private string? _regex;
    public string? Regex
    {
        get => _regex;
        set
        {
            if (SetProperty(ref _regex, value))
            {
                ValidateRegex();
            }
        }
    }

    private string? _regexError;
    [NotMapped]
    public string? RegexError
    {
        get => _regexError;
        set => SetProperty(ref _regexError, value);
    }

    private bool _hasRegexError;
    [NotMapped]
    public bool HasRegexError
    {
        get => _hasRegexError;
        set => SetProperty(ref _hasRegexError, value);
    }

    // ---------------------------------------------------------
    // ZoomLevel
    // ---------------------------------------------------------

     
    private int? _zoomLevel;
    public int? ZoomLevel
    {
        get => _zoomLevel;
        set => SetProperty(ref _zoomLevel, value);
    }

    // ---------------------------------------------------------
    // Latitude, Longitude
    // ---------------------------------------------------------

    public double Latitude { get; set; }
    public double Longitude { get; set; }

 
    //// ---------------------------------------------------------
    //// Writer
    //// ---------------------------------------------------------

    //[StringLength(50)]
    //private string? _writer;
    //public string? Writer
    //{
    //    get => _writer;
    //    set => SetProperty(ref _writer, value);
    //}

    //private DateTime _writerDate;
    //public DateTime WriterDate
    //{
    //    get => _writerDate;
    //    set => SetProperty(ref _writerDate, value);
    //}

    //// ---------------------------------------------------------
    //// Updater
    //// ---------------------------------------------------------

    //[StringLength(50)]
    //private string? _updater;
    //public string? Updater
    //{
    //    get => _updater;
    //    set => SetProperty(ref _updater, value);
    //}

    //private DateTime _updaterDate;
    //public DateTime UpdaterDate
    //{
    //    get => _updaterDate;
    //    set => SetProperty(ref _updaterDate, value);
    //}


    // ---------------------------------------------------------
    // IHierarchicalEntity IMPLEMENTATION
    // ---------------------------------------------------------

    public int GetEntityId() => CountryId;
    public string GetDisplayName() => Name ?? $"Country {CountryId}";

    // ---------------------------------------------------------
    // VALIDIERUNGSMETHODEN MIT LOCALIZATION
    // ---------------------------------------------------------

    private void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
            _errors[propertyName].Clear();
    }

    private void AddError(string propertyName, string errorMessage)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if (!_errors[propertyName].Contains(errorMessage))
            _errors[propertyName].Add(errorMessage);
    }

    private void UpdateErrorProperty(string propertyName)
    {
        var hasErrors = _errors.ContainsKey(propertyName) && _errors[propertyName].Count > 0;
        var errorMessage = hasErrors ? string.Join(" | ", _errors[propertyName]) : "";

        switch (propertyName)
        {
            case nameof(Name):
                NameError = errorMessage;
                HasNameError = hasErrors;
                break;
            case nameof(Regex):
                RegexError = errorMessage;
                HasRegexError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    // ---------------------------------------------------------

    // ---------------------------------------------------------
    // Validate Name
    // ---------------------------------------------------------

    public void ValidateName()
    {
        ClearErrors(nameof(Name));
        var loc = Localizer.Get();  // ← Direkt verwenden wie in deinem Code

        if (string.IsNullOrWhiteSpace(_name))
            AddError(nameof(Name), loc.GetLocalizedString("Error_Name_Empty"));

        if (_name?.Length > 100)
            AddError(nameof(Name), loc.GetLocalizedString("Error_Name_TooLong"));

        UpdateErrorProperty(nameof(Name));
    }

    // ---------------------------------------------------------
    // Validate Continent
    // ---------------------------------------------------------

    public void ValidateContinent()
    {
        ClearErrors(nameof(Continent));
        var loc = Localizer.Get();  // ← Direkt verwenden wie in deinem Code

        if (string.IsNullOrWhiteSpace(_continent))
            AddError(nameof(Continent), loc.GetLocalizedString("Error_Continent_Empty"));

        if (_continent?.Length > 100)
            AddError(nameof(Continent), loc.GetLocalizedString("Error_Continent_TooLong"));

        UpdateErrorProperty(nameof(Continent));
    }


    // ---------------------------------------------------------
    // Validate ValidYear
    // ---------------------------------------------------------

    private void ValidateRegex()
    {
        ClearErrors(nameof(Regex));
        var loc = Localizer.Get();  // ← Direkt verwenden

        if (string.IsNullOrWhiteSpace(Regex))
            goto Notify;

        if (Regex!.Length != 250)
        {
            AddError(nameof(Regex), loc.GetLocalizedString("Error_Regex_Length"));
            goto Notify;
        }

        if (!int.TryParse(Regex, out var year))
        {
            AddError(nameof(Regex), loc.GetLocalizedString("Error_Regex_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(Regex), loc.GetLocalizedString("Error_Regex_Range"));

Notify:
        UpdateErrorProperty(nameof(Regex));
    }


    public bool IsValid()
    {
        ValidateName();
        ValidateContinent();
        ValidateRegex();

        return !HasNameError && !HasContinentError && !HasRegexError;
    }



}
