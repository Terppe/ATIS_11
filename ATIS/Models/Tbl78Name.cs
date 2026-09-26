using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Models;

public class Tbl78Name : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int NameId { get; set; }

    // ---------------------------------------------------------
    // FIELDS
    // ---------------------------------------------------------
    private string _nameName;
    [Required]
    [StringLength(100)]
    public string NameName
    {
        get => _nameName;
        set
        {
            if (SetProperty(ref _nameName, value))
                ValidateNameName();
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
    // CountId
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

    private string? _language;
    [StringLength(3)]
    public string? Language
    {
        get => _language;
        set
        {
            if (SetProperty(ref _language, value))
                ValidateLanguage();
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

    [StringLength(50)]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }

    [StringLength(50)]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }


    // ---------------------------------------------------------
    // VALIDATION METHODS
    // ---------------------------------------------------------
    private void ValidateNameName()
    {
        ClearErrors(nameof(NameName));

        if (string.IsNullOrWhiteSpace(NameName))
            AddError(nameof(NameName), "Name is required.");

        if (NameName?.Length > 100)
            AddError(nameof(NameName), "Max length is 100.");
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));

        if (!string.IsNullOrWhiteSpace(ValidYear) && ValidYear.Length != 4)
            AddError(nameof(ValidYear), "Year must be 4 digits.");
    }

    private void ValidateLanguage()
    {
        ClearErrors(nameof(Language));

        if (!string.IsNullOrWhiteSpace(Language) && Language.Length != 3)
            AddError(nameof(Language), "Language code must be 3 letters.");
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
    // ERROR DICTIONARY 
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
    // ERROR PROPERTIES (wie Subtribus)
    // ---------------------------------------------------------
    [NotMapped]
    public string NameNameError => _errors.ContainsKey(nameof(NameName)) ? string.Join("; ", _errors[nameof(NameName)]) : null;

    [NotMapped]
    public bool HasNameNameError => _errors.ContainsKey(nameof(NameName));

    [NotMapped]
    public string ValidYearError => _errors.ContainsKey(nameof(ValidYear)) ? string.Join("; ", _errors[nameof(ValidYear)]) : null;

    [NotMapped]
    public bool HasValidYearError => _errors.ContainsKey(nameof(ValidYear));

    [NotMapped]
    public string LanguageError => _errors.ContainsKey(nameof(Language)) ? string.Join("; ", _errors[nameof(Language)]) : null;

    [NotMapped]
    public bool HasLanguageError => _errors.ContainsKey(nameof(Language));

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
    public int GetEntityId() => NameId;

    public string GetDisplayName() => NameName;
}

//public class Tbl78Name : IHierarchicalEntity
//{
//    [Key]

//    public int NameId { get; set; }
//    [Required(ErrorMessageResourceName = "RequiredNameName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string NameName { get; set; }
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
//    [StringLength(3, ErrorMessageResourceName = "StringLength_3", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Language { get; set; }
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
//    //     public byte[] RowVersion { get; set; }

//    [ForeignKey("SpeciesId")]
//    public virtual Tbl75Species Tbl75Speciesses { get; set; }


//    public int GetEntityId()
//    {
//        throw new NotImplementedException();
//    }

//    public string GetDisplayName()
//    {
//        throw new NotImplementedException();
//    }
//}
