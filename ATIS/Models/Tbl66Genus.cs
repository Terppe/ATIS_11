using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;


//    Tbl66GenusModel Skriptdatum:  13.06.2024  10:32    

namespace ATIS.Models;

public class Tbl66Genus : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------

    [Key]
    public int GenusId { get; set; }

    // ---------------------------------------------------------
    // GenusName
    // ---------------------------------------------------------

    [StringLength(100)]
    private string? _genusName;
    public string? GenusName
    {
        get => _genusName;
        set
        {
            SetProperty(ref _genusName, value);
            ValidateGenusName();
        }
    }

    private string? _genusNameError;
    [NotMapped]
    public string? GenusNameError
    {
        get => _genusNameError;
        set => SetProperty(ref _genusNameError, value);
    }

    private bool _hasGenusNameError;
    [NotMapped]
    public bool HasGenusNameError
    {
        get => _hasGenusNameError;
        set
        {
            if (_hasGenusNameError != value)
            {
                _hasGenusNameError = value;
                OnPropertyChanged(); // WICHTIG!
            }
        }
    }

    // ---------------------------------------------------------                   
    // InfratribusId (FK)
    // ---------------------------------------------------------                   

    private int? _infratribusId;
    public int? InfratribusId
    {
        get => _infratribusId;
        set => SetProperty(ref _infratribusId, value);
    }

    public Tbl63Infratribus? Infratribus { get; set; }

    [NotMapped]
    public string? InfratribusName => Infratribus?.InfratribusName;

    // ---------------------------------------------------------               
    // CountId
    // ---------------------------------------------------------               

    private int _countId;
    public int CountId
    {
        get => _countId;
        set => SetProperty(ref _countId, value);
    }

    // ---------------------------------------------------------                  
    // Valid
    // ---------------------------------------------------------                  
    private bool? _valid;
    public bool? Valid
    {
        get => _valid;
        set => SetProperty(ref _valid, value);
    }

    // ---------------------------------------------------------
    // ValidYear
    // ---------------------------------------------------------

    [StringLength(4)]
    private string? _validYear;
    public string? ValidYear
    {
        get => _validYear;
        set
        {
            if (SetProperty(ref _validYear, value))
            {
                ValidateValidYear();
            }
        }
    }

    private string? _validYearError;
    [NotMapped]
    public string? ValidYearError
    {
        get => _validYearError;
        set => SetProperty(ref _validYearError, value);
    }

    private bool _hasValidYearError;
    [NotMapped]
    public bool HasValidYearError
    {
        get => _hasValidYearError;
        set => SetProperty(ref _hasValidYearError, value);
    }

    // ---------------------------------------------------------
    // Synonym
    // ---------------------------------------------------------

    [StringLength(1000)]
    private string? _synonym;
    public string? Synonym
    {
        get => _synonym;
        set => SetProperty(ref _synonym, value);
    }

    // ---------------------------------------------------------
    // Author
    // ---------------------------------------------------------

    [StringLength(60)]
    private string? _author;
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }

    // ---------------------------------------------------------
    // AuthorYear
    // ---------------------------------------------------------

    [StringLength(4)]
    private string? _authorYear;
    public string? AuthorYear
    {
        get => _authorYear;
        set
        {
            if (SetProperty(ref _authorYear, value))
            {
                ValidateAuthorYear();
            }
        }
    }

    private string? _authorYearError;
    [NotMapped]
    public string? AuthorYearError
    {
        get => _authorYearError;
        set => SetProperty(ref _authorYearError, value);
    }

    private bool _hasAuthorYearError;
    [NotMapped]
    public bool HasAuthorYearError
    {
        get => _hasAuthorYearError;
        set => SetProperty(ref _hasAuthorYearError, value);
    }

    // ---------------------------------------------------------
    // Info
    // ---------------------------------------------------------

    [StringLength(1000)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set => SetProperty(ref _info, value);
    }

    // ---------------------------------------------------------
    // EngName
    // ---------------------------------------------------------

    [StringLength(200)]
    private string? _engName;
    public string? EngName
    {
        get => _engName;
        set => SetProperty(ref _engName, value);
    }

    // ---------------------------------------------------------
    // GerName
    // ---------------------------------------------------------

    [StringLength(200)]
    private string? _gerName;
    public string? GerName
    {
        get => _gerName;
        set => SetProperty(ref _gerName, value);
    }

    // ---------------------------------------------------------
    // FraName
    // ---------------------------------------------------------

    [StringLength(200)]
    private string? _fraName;
    public string? FraName
    {
        get => _fraName;
        set => SetProperty(ref _fraName, value);
    }

    // ---------------------------------------------------------
    // PorName
    // ---------------------------------------------------------

    [StringLength(200)]
    private string? _porName;
    public string? PorName
    {
        get => _porName;
        set => SetProperty(ref _porName, value);
    }

    // ---------------------------------------------------------
    // Writer
    // ---------------------------------------------------------

    [StringLength(50)]
    private string _writer = string.Empty;
    public string Writer
    {
        get => _writer;
        set => SetProperty(ref _writer, value);
    }

    private DateTime _writerDate;
    public DateTime WriterDate
    {
        get => _writerDate;
        set => SetProperty(ref _writerDate, value);
    }

    // ---------------------------------------------------------
    // Updater
    // ---------------------------------------------------------

    [StringLength(50)]
    private string _updater = string.Empty;
    public string Updater
    {
        get => _updater;
        set => SetProperty(ref _updater, value);
    }

    private DateTime _updaterDate;
    public DateTime UpdaterDate
    {
        get => _updaterDate;
        set => SetProperty(ref _updaterDate, value);
    }

    // ---------------------------------------------------------
    // Memo
    // ---------------------------------------------------------

    [StringLength(1000)]
    private string? _memo;
    public string? Memo
    {
        get => _memo;
        set => SetProperty(ref _memo, value);
    }

    // ---------------------------------------------------------
    // IHierarchicalEntity IMPLEMENTATION
    // ---------------------------------------------------------

    public int GetEntityId() => GenusId;
    public string GetDisplayName() => GenusName ?? $"Genus {GenusId}";

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
            case nameof(GenusName):
                GenusNameError = errorMessage;
                HasGenusNameError = hasErrors;
                break;
            case nameof(ValidYear):
                ValidYearError = errorMessage;
                HasValidYearError = hasErrors;
                break;
            case nameof(AuthorYear):
                AuthorYearError = errorMessage;
                HasAuthorYearError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    // ---------------------------------------------------------
    // Validate Name
    // ---------------------------------------------------------

    public void ValidateGenusName()
    {
        ClearErrors(nameof(GenusName));
        var loc = Localizer.Get();  // ← Direkt verwenden wie in deinem Code

        if (string.IsNullOrWhiteSpace(_genusName))
            AddError(nameof(GenusName), loc.GetLocalizedString("Error_GenusName_Empty"));

        if (_genusName?.Length > 100)
            AddError(nameof(GenusName), loc.GetLocalizedString("Error_GenusName_TooLong"));

        UpdateErrorProperty(nameof(GenusName));
    }

    // ---------------------------------------------------------
    // Validate ValidYear
    // ---------------------------------------------------------

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();  // ← Direkt verwenden

        if (string.IsNullOrWhiteSpace(ValidYear))
            goto Notify;

        if (ValidYear!.Length != 4)
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Length"));
            goto Notify;
        }

        if (!int.TryParse(ValidYear, out var year))
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Range"));

Notify:
        UpdateErrorProperty(nameof(ValidYear));
    }

    // ---------------------------------------------------------
    // Validate AuthorYear
    // ---------------------------------------------------------

    private void ValidateAuthorYear()
    {
        ClearErrors(nameof(AuthorYear));
        var loc = Localizer.Get();  // ← Direkt verwenden

        if (string.IsNullOrWhiteSpace(AuthorYear))
            goto Notify;

        if (AuthorYear!.Length != 4)
        {
            AddError(nameof(AuthorYear), loc.GetLocalizedString("Error_AuthorYear_Length"));
            goto Notify;
        }

        if (!int.TryParse(AuthorYear, out var year))
        {
            AddError(nameof(AuthorYear), loc.GetLocalizedString("Error_AuthorYear_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(AuthorYear), loc.GetLocalizedString("Error_AuthorYear_Range"));

Notify:
        UpdateErrorProperty(nameof(AuthorYear));
    }

    public bool IsValid()
    {
        ValidateGenusName();
        ValidateValidYear();
        ValidateAuthorYear();

        return !HasGenusNameError && !HasValidYearError && !HasAuthorYearError;
    }



}
