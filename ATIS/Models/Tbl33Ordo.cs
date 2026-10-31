using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;


//    Tbl33OrdoModel Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Models;

public class Tbl33Ordo : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------

    [Key]
    public int OrdoId { get; set; }

    // ---------------------------------------------------------
    // OrdoName
    // ---------------------------------------------------------

    [StringLength(100)]
    private string? _ordoName;
    public string? OrdoName
    {
        get => _ordoName;
        set
        {
            SetProperty(ref _ordoName, value);
            ValidateOrdoName();
        }
    }

    private string? _ordoNameError;
    [NotMapped]
    public string? OrdoNameError
    {
        get => _ordoNameError;
        set => SetProperty(ref _ordoNameError, value);
    }

    private bool _hasOrdoNameError;
    [NotMapped]
    public bool HasOrdoNameError
    {
        get => _hasOrdoNameError;
        set
        {
            if (_hasOrdoNameError != value)
            {
                _hasOrdoNameError = value;
                OnPropertyChanged(); // WICHTIG!
            }
        }
    }

    // ---------------------------------------------------------                   
    // LegioId (FK)
    // ---------------------------------------------------------                   

    private int _legioId;
    public int LegioId
    {
        get => _legioId;
        set => SetProperty(ref _legioId, value);
    }

    public Tbl30Legio? Legio { get; set; }

    [NotMapped]
    public string? LegioName => Legio?.LegioName;

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

    public int GetEntityId() => OrdoId;
    public string GetDisplayName() => OrdoName ?? $"Ordo {OrdoId}";

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
            case nameof(OrdoName):
                OrdoNameError = errorMessage;
                HasOrdoNameError = hasErrors;
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

    public void ValidateOrdoName()
    {
        ClearErrors(nameof(OrdoName));
        var loc = Localizer.Get();  // ← Direkt verwenden wie in deinem Code

        if (string.IsNullOrWhiteSpace(_ordoName))
            AddError(nameof(OrdoName), loc.GetLocalizedString("Error_OrdoName_Empty"));

        if (_ordoName?.Length > 100)
            AddError(nameof(OrdoName), loc.GetLocalizedString("Error_OrdoName_TooLong"));

        UpdateErrorProperty(nameof(OrdoName));
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
        ValidateOrdoName();
        ValidateValidYear();
        ValidateAuthorYear();

        return !HasOrdoNameError && !HasValidYearError && !HasAuthorYearError;
    }



}
