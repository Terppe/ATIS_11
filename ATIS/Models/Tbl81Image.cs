using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Models;


public class Tbl81Image : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int ImageId { get; set; }

    // ---------------------------------------------------------
    // FIELDS
    // ---------------------------------------------------------
    public int SpeciesId { get; set; }
    public int? FoodId { get; set; }
    public int? DiseaseId { get; set; }
    public int? MiscellaneousId { get; set; }

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

    public DateTime? ShotDate { get; set; }

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
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }

    [StringLength(50)]
    public string Updater { get; set; }
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

    public byte[]? ImageData { get; set; }

    [StringLength(50)]
    public string? ImageMimeType { get; set; }

    public byte[]? Filestream { get; set; }
    public Guid FilestreamId { get; set; }

    // ---------------------------------------------------------
    // NAVIGATION
    // ---------------------------------------------------------
    [ForeignKey("SpeciesId")]
    public virtual Tbl75Species Species { get; set; }

    [NotMapped] public string? SpeciesName => Species?.SpeciesName;

    // (Food, Disease, Misc kommen später als Modelle)
    [NotMapped] public string? FoodName { get; set; }
    [NotMapped] public string? DiseaseName { get; set; }
    [NotMapped] public string? MiscName { get; set; }

    // ---------------------------------------------------------
    // ERROR DICTIONARY (wie Subtribus)
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
    // VALIDATION
    // ---------------------------------------------------------
    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));

        if (!string.IsNullOrWhiteSpace(ValidYear) && ValidYear.Length != 4)
            AddError(nameof(ValidYear), "Year must be 4 digits.");
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
    public string ValidYearError => _errors.ContainsKey(nameof(ValidYear)) ? string.Join("; ", _errors[nameof(ValidYear)]) : null;

    [NotMapped]
    public bool HasValidYearError => _errors.ContainsKey(nameof(ValidYear));

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
    public int GetEntityId() => ImageId;

    public string GetDisplayName() =>
        !string.IsNullOrWhiteSpace(Info)
            ? Info
            : $"Image {ImageId}";
}
//public class Tbl81Image
//{
//    [Key]

//    public int ImageId { get; set; }
//    public int SpeciesId { get; set; }
//    //public int FiSpeciesId { get; set; }
//    //public int PlSpeciesId { get; set; }
//    public int FoodId { get; set; }
//    public int DiseaseId { get; set; }
//    public int MiscellaneousId { get; set; }
//    [Required]
//    public int CountId { get; set; }
//    public bool? Valid { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ValidYear { get; set; }
//    public DateTime? ShotDate { get; set; }
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
//    public byte[] ImageData { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ImageMimeType { get; set; }
//    public byte[] Filestream { get; set; }
//    public Guid FilestreamId { get; set; }
//    //     public byte[] RowVersion { get; set; }

//    //[ForeignKey("FiSpeciesId")]
//    //public virtual Tbl69FiSpecies Tbl69FiSpeciesses { get; set; }

//    //[ForeignKey("PlSpeciesId")]
//    //public virtual Tbl72PlSpecies Tbl72PlSpeciesses { get; set; }

//    //[ForeignKey("FoodId")]
//    //public virtual TblFood TblFoods { get; set; }

//    //[ForeignKey("DiseaseId")]
//    //public virtual TblDisease TblDiseases { get; set; }

//    //[ForeignKey("MiscellaneousId")]
//    //public virtual TblMiscellaneous TblMiscellaneousses { get; set; }

//    //[ForeignKey("SpeciesId")]
//    //public virtual Tbl75Species Tbl75Speciesses { get; set; }


//}
