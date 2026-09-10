using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Models;

public class Tbl87Geographic : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
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
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int GeographicId { get; set; }

    // ---------------------------------------------------------
    // FIELDS
    // ---------------------------------------------------------
    private string? _address;
    [StringLength(100)]
    public string? Address
    {
        get => _address;
        set
        {
            if (SetProperty(ref _address, value))
                ValidateAddress();
        }
    }

    private string? _country;
    [StringLength(100)]
    public string? Country
    {
        get => _country;
        set
        {
            if (SetProperty(ref _country, value))
                ValidateCountry();
        }
    }

    private string? _continent;
    [StringLength(100)]
    public string? Continent
    {
        get => _continent;
        set
        {
            if (SetProperty(ref _continent, value))
                ValidateContinent();
        }
    }

    public int SpeciesId { get; set; }

    [Required]
    public int CountId { get; set; }

    private string? _jurisdiction;
    [StringLength(100)]
    public string? Jurisdiction
    {
        get => _jurisdiction;
        set
        {
            if (SetProperty(ref _jurisdiction, value))
                ValidateJurisdiction();
        }
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Latitude1 { get; set; }
    public double Longitude1 { get; set; }
    public double Latitude2 { get; set; }
    public double Longitude2 { get; set; }
    public double Latitude3 { get; set; }
    public double Longitude3 { get; set; }
    public double ZoomLevel { get; set; }

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

    private string? _http;
    [StringLength(100)]
    public string? Http
    {
        get => _http;
        set
        {
            if (SetProperty(ref _http, value))
                ValidateHttp();
        }
    }

    // ---------------------------------------------------------
    // NAVIGATION
    // ---------------------------------------------------------
    [ForeignKey("SpeciesId")]
    public virtual Tbl75Species Species { get; set; }

    [NotMapped]
    public string SpeciesName => Species?.SpeciesName;

    // ---------------------------------------------------------
    // VALIDATION METHODS
    // ---------------------------------------------------------
    private void ValidateAddress()
    {
        ClearErrors(nameof(Address));
        if (!string.IsNullOrWhiteSpace(Address) && Address.Length > 100)
            AddError(nameof(Address), "Max length is 100.");
    }

    private void ValidateCountry()
    {
        ClearErrors(nameof(Country));
        if (!string.IsNullOrWhiteSpace(Country) && Country.Length > 100)
            AddError(nameof(Country), "Max length is 100.");
    }

    private void ValidateContinent()
    {
        ClearErrors(nameof(Continent));
        if (!string.IsNullOrWhiteSpace(Continent) && Continent.Length > 100)
            AddError(nameof(Continent), "Max length is 100.");
    }

    private void ValidateJurisdiction()
    {
        ClearErrors(nameof(Jurisdiction));
        if (!string.IsNullOrWhiteSpace(Jurisdiction) && Jurisdiction.Length > 100)
            AddError(nameof(Jurisdiction), "Max length is 100.");
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

    private void ValidateHttp()
    {
        ClearErrors(nameof(Http));
        if (!string.IsNullOrWhiteSpace(Http) && Http.Length > 100)
            AddError(nameof(Http), "Max length is 100.");
    }

    // ---------------------------------------------------------
    // ERROR PROPERTIES
    // ---------------------------------------------------------
    [NotMapped] public string AddressError => _errors.ContainsKey(nameof(Address)) ? string.Join("; ", _errors[nameof(Address)]) : null;
    [NotMapped] public bool HasAddressError => _errors.ContainsKey(nameof(Address));

    [NotMapped] public string CountryError => _errors.ContainsKey(nameof(Country)) ? string.Join("; ", _errors[nameof(Country)]) : null;
    [NotMapped] public bool HasCountryError => _errors.ContainsKey(nameof(Country));

    [NotMapped] public string ContinentError => _errors.ContainsKey(nameof(Continent)) ? string.Join("; ", _errors[nameof(Continent)]) : null;
    [NotMapped] public bool HasContinentError => _errors.ContainsKey(nameof(Continent));

    [NotMapped] public string JurisdictionError => _errors.ContainsKey(nameof(Jurisdiction)) ? string.Join("; ", _errors[nameof(Jurisdiction)]) : null;
    [NotMapped] public bool HasJurisdictionError => _errors.ContainsKey(nameof(Jurisdiction));

    [NotMapped] public string ValidYearError => _errors.ContainsKey(nameof(ValidYear)) ? string.Join("; ", _errors[nameof(ValidYear)]) : null;
    [NotMapped] public bool HasValidYearError => _errors.ContainsKey(nameof(ValidYear));

    [NotMapped] public string AuthorError => _errors.ContainsKey(nameof(Author)) ? string.Join("; ", _errors[nameof(Author)]) : null;
    [NotMapped] public bool HasAuthorError => _errors.ContainsKey(nameof(Author));

    [NotMapped] public string AuthorYearError => _errors.ContainsKey(nameof(AuthorYear)) ? string.Join("; ", _errors[nameof(AuthorYear)]) : null;
    [NotMapped] public bool HasAuthorYearError => _errors.ContainsKey(nameof(AuthorYear));

    [NotMapped] public string InfoError => _errors.ContainsKey(nameof(Info)) ? string.Join("; ", _errors[nameof(Info)]) : null;
    [NotMapped] public bool HasInfoError => _errors.ContainsKey(nameof(Info));

    [NotMapped] public string MemoError => _errors.ContainsKey(nameof(Memo)) ? string.Join("; ", _errors[nameof(Memo)]) : null;
    [NotMapped] public bool HasMemoError => _errors.ContainsKey(nameof(Memo));

    [NotMapped] public string HttpError => _errors.ContainsKey(nameof(Http)) ? string.Join("; ", _errors[nameof(Http)]) : null;
    [NotMapped] public bool HasHttpError => _errors.ContainsKey(nameof(Http));

    // ---------------------------------------------------------
    // IHierarchicalEntity
    // ---------------------------------------------------------
    public int GetEntityId() => GeographicId;

    public string GetDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(Address))
            return Address;

        if (!string.IsNullOrWhiteSpace(Country))
            return Country;

        return $"Geographic {GeographicId}";
    }
}
//public class Tbl87Geographic
//{
//   [Key]

//    public int GeographicId { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Address { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Country { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Continent { get; set; }
//    public int SpeciesId { get; set; }
//    //[Required]
//    //public int FiSpeciesId { get; set; }
//    //[Required]
//    //public int PlSpeciesId { get; set; }
//    [Required]
//    public int CountId { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Jurisdiction { get; set; }
//    public double Latitude { get; set; }
//    public double Longitude { get; set; }
//    public double Latitude1 { get; set; }
//    public double Longitude1 { get; set; }
//    public double Latitude2 { get; set; }
//    public double Longitude2 { get; set; }
//    public double Latitude3 { get; set; }
//    public double Longitude3 { get; set; }
//    public double ZoomLevel { get; set; }
//    public bool? Valid { get; set; }
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
//    public string Memo { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Http { get; set; }
//    //     public byte[] RowVersion { get; set; }

//    [ForeignKey("SpeciesId")]
//    public virtual Tbl75Species Tbl75Speciesses { get; set; }


//}
