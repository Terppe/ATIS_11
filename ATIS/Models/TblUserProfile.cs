using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class TblUserProfile : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int UserProfileId { get; set; }

    // ---------------------------------------------------------
    // Email
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _email;
    public string? Email        
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            ValidateEmail();
        }
    }

    private string? _emailError;
    [NotMapped]
    public string? EmailError
    {
        get => _emailError;
        set => SetProperty(ref _emailError, value);
    }

    private bool _hasEmailError;
    [NotMapped]
    public bool HasEmailError
    {
        get => _hasEmailError;
        set
        {
            if (_hasEmailError != value)
            {
                _hasEmailError = value;
                OnPropertyChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // FirstName
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _firstName;
    public string? FirstName
    {
        get => _firstName;
        set
        {
            if (SetProperty(ref _firstName, value))
                ValidateFirstName();
        }
    }

    private string? _firstNameError;
    [NotMapped]
    public string? FirstNameError
    {
        get => _firstNameError;
        set => SetProperty(ref _firstNameError, value);
    }

    private bool _hasFirstNameError;
    [NotMapped]
    public bool HasFirstNameError
    {
        get => _hasFirstNameError;
        set
        {
            if (_hasFirstNameError != value)
            {
                _hasFirstNameError = value;
                OnPropertyChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // LastName
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _lastName;
    public string? LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value))
                ValidateLastName();
        }
    }

    private string? _lastNameError;
    [NotMapped]
    public string? LastNameError
    {
        get => _lastNameError;
        set => SetProperty(ref _lastNameError, value);
    }

    private bool _hasLastNameError;
    [NotMapped]
    public bool HasLastNameError
    {
        get => _hasLastNameError;
        set
        {
            if (_hasLastNameError != value)
            {
                _hasLastNameError = value;
                OnPropertyChanged();
            }
        }
    }


    // ---------------------------------------------------------
    // Title
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _title;
    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // ---------------------------------------------------------
    // Password
    // ---------------------------------------------------------
    [StringLength(250)]
    private string? _password;
    public string? Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    // ---------------------------------------------------------
    // Role
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _role;
    public string? Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    // ---------------------------------------------------------
    // Flag
    // ---------------------------------------------------------
    private bool? _flag;
    public bool? Flag
    {
        get => _flag;
        set => SetProperty(ref _flag, value);
    }

    // ---------------------------------------------------------
    // StartDate
    // ---------------------------------------------------------
    private DateTime _startDate;
    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    // ---------------------------------------------------------
    // EndDate
    // ---------------------------------------------------------
    private DateTime _endDate;
    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    // ---------------------------------------------------------
    // Colour
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _colour;
    public string? Colour
    {
        get => _colour;
        set => SetProperty(ref _colour, value);
    }

    // ---------------------------------------------------------
    // Notes
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

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
    // BirthDate
    // ---------------------------------------------------------
    private DateTime _birthDate;
    public DateTime BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }

    // ---------------------------------------------------------
    // Gender
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _gender;
    public string? Gender
    {
        get => _gender;
        set => SetProperty(ref _gender, value);
    }
    // ---------------------------------------------------------
    // Country
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _country;
    public string? Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }
    // ---------------------------------------------------------
    // Postcode
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _postcode;
    public string? Postcode
    {
        get => _postcode;
        set => SetProperty(ref _postcode, value);
    }
    // ---------------------------------------------------------
    // City
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _city;
    public string? City
    {
        get => _city;
        set => SetProperty(ref _city, value);
    }
    // ---------------------------------------------------------
    // Street1
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _street1;
    public string? Street1
    {
        get => _street1;
        set => SetProperty(ref _street1, value);
    }
    // ---------------------------------------------------------
    // Street2
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _street2;
    public string? Street2
    {
        get => _street2;
        set => SetProperty(ref _street2, value);
    }
    // ---------------------------------------------------------
    // Tel
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _tel;
    public string? Tel      
    {
        get => _tel;
        set => SetProperty(ref _tel, value);
    }
    // ---------------------------------------------------------
    // Mobil
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _mobil;
    public string? Mobil
    {
        get => _mobil;
        set => SetProperty(ref _mobil, value);
    }

    // ---------------------------------------------------------
    // Fax
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _fax;
    public string? Fax
    {
        get => _fax;
        set => SetProperty(ref _fax, value);
    }
    // ---------------------------------------------------------
    // HomePageUrl
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _homePageUrl;
    public string? HomePageUrl
    {
        get => _homePageUrl;
        set => SetProperty(ref _homePageUrl, value);
    }
    // ---------------------------------------------------------
    // Business
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _business;
    public string? Business
    {
        get => _business;
        set => SetProperty(ref _business, value);
    }
    // ---------------------------------------------------------
    // Company
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _company;
    public string? Company
    {
        get => _company;
        set => SetProperty(ref _company, value);
    }
    // ---------------------------------------------------------
    // Writer
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _writer;
    public string? Writer
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

    [StringLength(50)]
    private string? _updater;
    public string? Updater
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
    // Filestream
    // ---------------------------------------------------------
    private byte[]? _filestream;
    public byte[]? Filestream
    {
        get => _filestream;
        set => SetProperty(ref _filestream, value);
    }
    // ---------------------------------------------------------
    // ImageMimeType
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _imageMimeType;
    [StringLength(50)]
    public string? ImageMimeType
    {
        get => _imageMimeType;
        set => SetProperty(ref _imageMimeType, value);
    }
    // ---------------------------------------------------------
    // FilestreamId
    // ---------------------------------------------------------
    private Guid _filestreamId;
    public Guid FilestreamId
    {
        get => _filestreamId;
        set => SetProperty(ref _filestreamId, value);
    }

    // ---------------------------------------------------------
    // Signature
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _signature;
    public string? Signature            
    {
        get => _signature;
        set => SetProperty(ref _signature, value);
    }

    // ---------------------------------------------------------
    // MailNewsletter
    // ---------------------------------------------------------
    private bool? _mailNewsletter;
    public bool? MailNewsletter
    {
        get => _mailNewsletter;
        set => SetProperty(ref _mailNewsletter, value);
    }
    // ---------------------------------------------------------
    // MaulHtml
    // ---------------------------------------------------------
    private bool? _maulHtml;
    public bool? MaulHtml
    {
        get => _maulHtml;
        set => SetProperty(ref _maulHtml, value);
    }

    // ---------------------------------------------------------
    // Known
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _known;
    public string? Known
    {
        get => _known;
        set => SetProperty(ref _known, value);
    }

    // ---------------------------------------------------------
    // IHierarchicalEntity
    // ---------------------------------------------------------
    public int GetEntityId() => UserProfileId;
    public string GetDisplayName() => Email ?? $"User {UserProfileId}";

    // ---------------------------------------------------------
    // VALIDIERUNG
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
            case nameof(Email):
                EmailError = errorMessage;
                HasEmailError = hasErrors;
                break;

            case nameof(FirstName):
                FirstNameError = errorMessage;
                HasFirstNameError = hasErrors;
                break;
            case nameof(LastName):
                LastNameError = errorMessage;
                HasLastNameError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    private void ValidateEmail()
    {
        ClearErrors(nameof(Email));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(Email))
            AddError(nameof(Email), loc.GetLocalizedString("Error_Email_Empty"));

        if (Email?.Length > 200)
            AddError(nameof(Email), loc.GetLocalizedString("Error_Email_TooLong"));

        UpdateErrorProperty(nameof(Email));
    }


    // ---------------------------------------------------------
    // Validate FirstName
    // ---------------------------------------------------------

    private void ValidateFirstName()
    {
        ClearErrors(nameof(FirstName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(FirstName))
            AddError(nameof(FirstName), loc.GetLocalizedString("Error_FirstName_Empty"));

        if (FirstName?.Length > 100)
            AddError(nameof(FirstName), loc.GetLocalizedString("Error_FirstName_TooLong"));

        UpdateErrorProperty(nameof(FirstName));
    }

    // ---------------------------------------------------------
    // Validate LastName
    // ---------------------------------------------------------

    private void ValidateLastName()
    {
        ClearErrors(nameof(LastName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(LastName))
            AddError(nameof(LastName), loc.GetLocalizedString("Error_LastName_Empty"));

        if (LastName?.Length > 100)
            AddError(nameof(LastName), loc.GetLocalizedString("Error_LastName_TooLong"));

        UpdateErrorProperty(nameof(LastName));
    }

    public bool IsValid()
    {
        ValidateEmail();
        ValidateFirstName();
        ValidateLastName();

        return !HasEmailError && !HasFirstNameError && !HasLastNameError;
    }


}


    //[Key]
    //public int UserProfileId { get; set; }
    //[Required(ErrorMessageResourceName = "RequiredEmail", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    ////  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //// [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Email { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string LastName { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string FirstName { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Title { get; set; }
    //[Required(ErrorMessageResourceName = "RequiredPassword", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //[StringLength(250, ErrorMessageResourceName = "StringLength_250", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Password { get; set; }
    //[Required(ErrorMessageResourceName = "RequiredRole", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Role { get; set; }
    //public bool Flag { get; set; }
    //public DateTime StartDate { get; set; }
    //public DateTime EndDate { get; set; }
    //[StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Notes { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Colour { get; set; }
    //public int CountId { get; set; }
    //public DateTime BirthDate { get; set; }
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Gender { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Country { get; set; }
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Postcode { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string City { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Street1 { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Street2 { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Tel { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Mobil { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Fax { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string HomePageUrl { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Business { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Company { get; set; }
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Writer { get; set; }
    //public DateTime WriterDate { get; set; }
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Updater { get; set; }
    //public DateTime UpdaterDate { get; set; }
    //[StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Memo { get; set; }
    //public byte[] Filestream { get; set; }
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string ImageMimeType { get; set; }
    //public Guid FilestreamId { get; set; }
    //[StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Signature { get; set; }
    //public bool MailNewsletter { get; set; }
    //public bool MaulHtml { get; set; }
    //[StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //public string Known { get; set; }
    ////    public byte[] RowVersion { get; set; }
    ////   public EntityState EntityState { get; set; }

