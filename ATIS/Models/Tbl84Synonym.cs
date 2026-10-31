using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Models;


public class Tbl84Synonym : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int SynonymId { get; set; }

    // ---------------------------------------------------------
    // FIELDS
    // ---------------------------------------------------------
    private string? _synonymName;
    [Required]
    [StringLength(100)]
    public string? SynonymName
    {
        get => _synonymName;
        set
        {
            if (SetProperty(ref _synonymName, value))
                ValidateSynonymName();
        }
    }

    // ---------------------------------------------------------
    // SpeciesId (Pflicht)
    // ---------------------------------------------------------
    private int? _speciesId;
    public int? SpeciesId
    {
        get => _speciesId;
        set => SetProperty(ref _speciesId, value);
    }

    public Tbl75Species? Species { get; set; }

    [NotMapped]
    public string? GenusName => Species?.GenusName;

    [NotMapped]
    public string? SpeciesName => Species?.SpeciesName;

    [NotMapped]
    public string? Subspecies => Species?.Subspecies;

    [NotMapped]
    public string? Divers => Species?.Divers;

    [NotMapped]
    public string GenusSpeciesFullName => string.Join(" ",
        new[] { GenusName, SpeciesName, Subspecies, Divers }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    // ---------------------------------------------------------
    // CountId (Pflicht)
    // ---------------------------------------------------------

    [Required]
    public int CountId { get; set; }

    // ---------------------------------------------------------                  
    // Valid
    // ---------------------------------------------------------                  
    private bool? _valid;
    public bool? Valid
    {
        get => _valid;
        set => SetProperty(ref _valid, value);
    }

    private string? _validYear;
    [StringLength(4)]
    public string? ValidYear
    {
        get => _validYear;
        set
        {
            if (SetProperty(ref _validYear, value))
                ValidateValidYear();
        }
    }

    private string? _author;
    [StringLength(60)]
    public string? Author
    {
        get => _author;
        set
        {
            if (SetProperty(ref _author, value))
                ValidateAuthor();
        }
    }

    private string? _authorYear;
    [StringLength(4)]
    public string? AuthorYear
    {
        get => _authorYear;
        set
        {
            if (SetProperty(ref _authorYear, value))
                ValidateAuthorYear();
        }
    }

    private string? _info;
    [StringLength(100)]
    public string? Info
    {
        get => _info;
        set
        {
            if (SetProperty(ref _info, value))
                ValidateInfo();
        }
    }

    [StringLength(50)]
    public string Writer { get; set; } = string.Empty;
    public DateTime WriterDate { get; set; }

    [StringLength(50)]
    public string Updater { get; set; } = string.Empty;
    public DateTime UpdaterDate { get; set; }

    private string? _memo;
    [StringLength(1000)]
    public string? Memo
    {
        get => _memo;
        set
        {
            if (SetProperty(ref _memo, value))
                ValidateMemo();
        }
    }

    // ---------------------------------------------------------
    // NAVIGATION
    // ---------------------------------------------------------

    private readonly Dictionary<string, List<string>> _errors = new();

    public IReadOnlyDictionary<string, List<string>> Errors => _errors;

    private void AddError(string property, string message)
    {
        if (!_errors.ContainsKey(property))
            _errors[property] = new List<string>();

        if (!_errors[property].Contains(message))
            _errors[property].Add(message);

        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged($"{property}Error");
        OnPropertyChanged($"Has{property}Error");
    }

    private void ClearErrors(string property)
    {
        if (_errors.ContainsKey(property))
        {
            _errors.Remove(property);
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged($"{property}Error");
            OnPropertyChanged($"Has{property}Error");
        }
    }

    public bool HasErrors => _errors.Count > 0;

    // ---------------------------------------------------------
    // VALIDATION METHODS
    // ---------------------------------------------------------
    private void ValidateSynonymName()
    {
        ClearErrors(nameof(SynonymName));

        if (string.IsNullOrWhiteSpace(SynonymName))
            AddError(nameof(SynonymName), "Synonym is required.");

        if (SynonymName?.Length > 100)
            AddError(nameof(SynonymName), "Max length is 100.");
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));

        if (!string.IsNullOrWhiteSpace(ValidYear) && ValidYear.Length != 4)
            AddError(nameof(ValidYear), "Year must be 4 digits.");
    }

    private void ValidateAuthor()
    {
        ClearErrors(nameof(Author));

        if (!string.IsNullOrWhiteSpace(Author) && Author.Length > 60)
            AddError(nameof(Author), "Max length is 60.");
    }

    private void ValidateAuthorYear()
    {
        ClearErrors(nameof(AuthorYear));

        if (!string.IsNullOrWhiteSpace(AuthorYear) && AuthorYear.Length != 4)
            AddError(nameof(AuthorYear), "Year must be 4 digits.");
    }

    private void ValidateInfo()
    {
        ClearErrors(nameof(Info));

        if (!string.IsNullOrWhiteSpace(Info) && Info.Length > 100)
            AddError(nameof(Info), "Max length is 100.");
    }

    private void ValidateMemo()
    {
        ClearErrors(nameof(Memo));

        if (!string.IsNullOrWhiteSpace(Memo) && Memo.Length > 1000)
            AddError(nameof(Memo), "Max length is 1000.");
    }

    // ---------------------------------------------------------
    // ERROR PROPERTIES
    // ---------------------------------------------------------
    [NotMapped]
    public string SynonymNameError => _errors.ContainsKey(nameof(SynonymName)) ? string.Join("; ", _errors[nameof(SynonymName)]) : null;

    [NotMapped]
    public bool HasSynonymNameError => _errors.ContainsKey(nameof(SynonymName));

    [NotMapped]
    public string ValidYearError => _errors.ContainsKey(nameof(ValidYear)) ? string.Join("; ", _errors[nameof(ValidYear)]) : null;

    [NotMapped]
    public bool HasValidYearError => _errors.ContainsKey(nameof(ValidYear));

    [NotMapped]
    public string AuthorError => _errors.ContainsKey(nameof(Author)) ? string.Join("; ", _errors[nameof(Author)]) : null;

    [NotMapped]
    public bool HasAuthorError => _errors.ContainsKey(nameof(Author));

    [NotMapped]
    public string AuthorYearError => _errors.ContainsKey(nameof(AuthorYear)) ? string.Join("; ", _errors[nameof(AuthorYear)]) : null;

    [NotMapped]
    public bool HasAuthorYearError => _errors.ContainsKey(nameof(AuthorYear));

    [NotMapped]
    public string InfoError => _errors.ContainsKey(nameof(Info)) ? string.Join("; ", _errors[nameof(Info)]) : null;

    [NotMapped]
    public bool HasInfoError => _errors.ContainsKey(nameof(Info));

    [NotMapped]
    public string MemoError => _errors.ContainsKey(nameof(Memo)) ? string.Join("; ", _errors[nameof(Memo)]) : null;

    [NotMapped]
    public bool HasMemoError => _errors.ContainsKey(nameof(Memo));

    // ---------------------------------------------------------
    // IHierarchicalEntity
    // ---------------------------------------------------------
    public int GetEntityId() => SynonymId;

    public string GetDisplayName() => SynonymName;
}
//public class Tbl84Synonym
//{
//    [Key]

//    public int SynonymId { get; set; }
//    [Required(ErrorMessageResourceName = "RequiredSynonymName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string SynonymName { get; set; }
//    public int SpeciesId { get; set; }
//    //[Required]
//    //public int FiSpeciesId { get; set; }
//    //[Required]
//    //public int PlSpeciesId { get; set; }
//    [Required]
//    public int CountId { get; set; }
//    public bool Valid { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ValidYear { get; set; }
//    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Author { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string AuthorYear { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Info { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Writer { get; set; }
//    public DateTime WriterDate { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Updater { get; set; }
//    public DateTime UpdaterDate { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Memo { get; set; }
//    //   public byte[] RowVersion { get; set; }

//    [ForeignKey("SpeciesId")]
//    public virtual Tbl75Species Tbl75Speciesses { get; set; }


//}
