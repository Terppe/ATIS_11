using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class Tbl75Species : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private readonly Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key] public int SpeciesId { get; set; }

    // ---------------------------------------------------------
    // SpeciesName (Pflicht)
    // ---------------------------------------------------------
    [StringLength(100)] private string? _speciesName;

    public string? SpeciesName
    {
        get => _speciesName;
        set
        {
            SetProperty(ref _speciesName, value);
            ValidateSpeciesName();
        }
    }

    [NotMapped] public string? SpeciesNameError { get; set; }
    [NotMapped] public bool HasSpeciesNameError { get; set; }

    // ---------------------------------------------------------
    // Subspecies
    // ---------------------------------------------------------
    [StringLength(100)] private string? _subspecies;

    public string? Subspecies
    {
        get => _subspecies;
        set
        {
            SetProperty(ref _subspecies, value);
            ValidateSubspecies();
        }
    }

    [NotMapped] public string? SubspeciesError { get; set; }
    [NotMapped] public bool HasSubspeciesError { get; set; }

    // ---------------------------------------------------------
    // Divers
    // ---------------------------------------------------------
    [StringLength(100)] private string? _divers;

    public string? Divers
    {
        get => _divers;
        set
        {
            SetProperty(ref _divers, value);
            ValidateDivers();
        }
    }

    [NotMapped] public string? DiversError { get; set; }
    [NotMapped] public bool HasDiversError { get; set; }

    // ---------------------------------------------------------
    // SpeciesFullName (computed)
    // ---------------------------------------------------------
    [NotMapped]
    public bool IsSubspecies => !string.IsNullOrWhiteSpace(Subspecies);

    [NotMapped]
    public string SpeciesFullName => string.Join(" ",
        new[] { SpeciesName, Subspecies, Divers }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    [NotMapped]
    public string GenusSpeciesName => string.Join(" ",
        new[] { Genus?.GenusName, SpeciesName }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    [NotMapped]
    public string GenusSpeciesFullName => string.Join(" ",
        new[] { Genus?.GenusName, SpeciesName, Subspecies, Divers }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    [NotMapped]
    public string SubspeciesDiversName => string.Join(" ",
        new[] { Subspecies, Divers }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    // ---------------------------------------------------------
    // GenusId (Pflicht)
    // ---------------------------------------------------------
    private int _genusId;

    public int GenusId
    {
        get => _genusId;
        set
        {
            SetProperty(ref _genusId, value);
            ValidateGenusId();
        }
    }

    public Tbl66Genus? Genus { get; set; }

    [NotMapped]
    public string? GenusName => Genus?.GenusName;



    [NotMapped] public string? GenusIdError { get; set; }
    [NotMapped] public bool HasGenusIdError { get; set; }

    // ---------------------------------------------------------
    // SpeciesgroupId
    // ---------------------------------------------------------
    public int? SpeciesgroupId { get; set; }
    public Tbl68Speciesgroup? Speciesgroup { get; set; }

    // ---------------------------------------------------------
    // CountId (Pflicht)
    // ---------------------------------------------------------
    private int _countId;
    public int CountId
    {
        get => _countId;
        set => SetProperty(ref _countId, value);
    }

    [NotMapped] public string? CountIdError { get; set; }
    [NotMapped] public bool HasCountIdError { get; set; }

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
            SetProperty(ref _validYear, value);
            ValidateValidYear();
        }
    }

    [NotMapped] public string? ValidYearError { get; set; }
    [NotMapped] public bool HasValidYearError { get; set; }

    // ---------------------------------------------------------
    // MemoSpecies
    // ---------------------------------------------------------
    [StringLength(1000)] public string? MemoSpecies { get; set; }

    // ---------------------------------------------------------
    // TradeName
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _tradeName;

    public string? TradeName
    {
        get => _tradeName;
        set
        {
            SetProperty(ref _tradeName, value);
            ValidateTradeName();
        }
    }

    [NotMapped] public string? TradeNameError { get; set; }
    [NotMapped] public bool HasTradeNameError { get; set; }

    // ---------------------------------------------------------
    // Author
    // ---------------------------------------------------------
    [StringLength(60)]
    public string? Author { get; set; }

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
            SetProperty(ref _authorYear, value);
            ValidateAuthorYear();
        }
    }

    [NotMapped] public string? AuthorYearError { get; set; }
    [NotMapped] public bool HasAuthorYearError { get; set; }

    // ---------------------------------------------------------
    // Importer
    // ---------------------------------------------------------
    [StringLength(60)]
    private string? _importer;

    public string? Importer
    {
        get => _importer;
        set
        {
            SetProperty(ref _importer, value);
            ValidateImporter();
        }
    }

    [NotMapped] public string? ImporterError { get; set; }
    [NotMapped] public bool HasImporterError { get; set; }

    // ---------------------------------------------------------
    // ImportingYear
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _importingYear;

    public string? ImportingYear
    {
        get => _importingYear;
        set
        {
            SetProperty(ref _importingYear, value);
            ValidateImportingYear();
        }
    }

    [NotMapped] public string? ImportingYearError { get; set; }
    [NotMapped] public bool HasImportingYearError { get; set; }

    // ---------------------------------------------------------
    // TypeSpecies
    // ---------------------------------------------------------
    public bool? TypeSpecies { get; set; }

    // ---------------------------------------------------------
    // LNumber (nur Ziffern)
    // ---------------------------------------------------------
    [StringLength(10)] private string? _lNumber;

    public string? LNumber
    {
        get => _lNumber;
        set
        {
            SetProperty(ref _lNumber, value);
            ValidateLNumber();
        }
    }

    [NotMapped] public string? LNumberError { get; set; }
    [NotMapped] public bool HasLNumberError { get; set; }

    // ---------------------------------------------------------
    // LOrigin
    // ---------------------------------------------------------
    [StringLength(50)] private string? _lOrigin;

    public string? LOrigin
    {
        get => _lOrigin;
        set
        {
            SetProperty(ref _lOrigin, value);
            ValidateLOrigin();
        }
    }

    [NotMapped] public string? LOriginError { get; set; }
    [NotMapped] public bool HasLOriginError { get; set; }

    // ---------------------------------------------------------
    // LdaNumber (nur Ziffern)
    // ---------------------------------------------------------
    [StringLength(10)] private string? _ldaNumber;

    public string? LDANumber
    {
        get => _ldaNumber;
        set
        {
            SetProperty(ref _ldaNumber, value);
            ValidateLdaNumber();
        }
    }

    [NotMapped] public string? LDANumberError { get; set; }
    [NotMapped] public bool HasLDANumberError { get; set; }

    // ---------------------------------------------------------
    // LdaOrigin
    // ---------------------------------------------------------
    [StringLength(50)] private string? _ldaOrigin;

    public string? LDAOrigin
    {
        get => _ldaOrigin;
        set
        {
            SetProperty(ref _ldaOrigin, value);
            ValidateLdaOrigin();
        }
    }

    [NotMapped] public string? LDAOriginError { get; set; }
    [NotMapped] public bool HasLDAOriginError { get; set; }

    // ---------------------------------------------------------
    // BasinLength (> 0)
    // ---------------------------------------------------------
    public int? BasinLength { get; set; }

    // ---------------------------------------------------------
    // FishLength (> 0)
    // ---------------------------------------------------------
    public decimal? FishLength { get; set; }

    // ---------------------------------------------------------
    // Ernährungsformen
    // ---------------------------------------------------------
    public bool? Karnivore { get; set; }
    public bool? Herbivore { get; set; }
    public bool? Limnivore { get; set; }
    public bool? Omnivore { get; set; }

    // ---------------------------------------------------------
    // MemoFoods
    // ---------------------------------------------------------
    [StringLength(1000)]
    public string? MemoFoods { get; set; }

    // ---------------------------------------------------------
    // Difficult1–4
    // ---------------------------------------------------------
    public bool? Difficult1 { get; set; }
    public bool? Difficult2 { get; set; }
    public bool? Difficult3 { get; set; }
    public bool? Difficult4 { get; set; }

    // ---------------------------------------------------------
    // Region
    // ---------------------------------------------------------
    public bool? RegionTop { get; set; }
    public bool? RegionMiddle { get; set; }
    public bool? RegionBottom { get; set; }

    [StringLength(1000)] public string? MemoRegion { get; set; }

    [StringLength(1000)] public string? MemoTech { get; set; }

    // ---------------------------------------------------------
    // Wasserwerte (Range)
    // ---------------------------------------------------------
    public decimal? Ph1 { get; set; }
    public decimal? Ph2 { get; set; }

    public int? Temp1 { get; set; }
    public int? Temp2 { get; set; }

    public int? Hardness1 { get; set; }
    public int? Hardness2 { get; set; }

    public int? CarboHardness1 { get; set; }
    public int? CarboHardness2 { get; set; }

    // ---------------------------------------------------------
    // Audit
    // ---------------------------------------------------------
    [StringLength(50)] public string? Writer { get; set; }
    public DateTime WriterDate { get; set; }

    [StringLength(50)] public string? Updater { get; set; }
    public DateTime UpdaterDate { get; set; }

    // ---------------------------------------------------------
    // Memo-Felder
    // ---------------------------------------------------------
    [StringLength(1000)] public string? MemoHusbandry { get; set; }
    [StringLength(1000)] public string? MemoBreeding { get; set; }
    [StringLength(1000)] public string? MemoBuilt { get; set; }
    [StringLength(1000)] public string? MemoColor { get; set; }
    [StringLength(1000)] public string? MemoSozial { get; set; }
    [StringLength(1000)] public string? MemoDomorphism { get; set; }
    [StringLength(1000)] public string? MemoSpecial { get; set; }

    public int? BasinHeight { get; set; }
    public decimal? PlantLength { get; set; }

    [StringLength(1000)] public string? MemoReproduction { get; set; }
    [StringLength(1000)] public string? MemoCulture { get; set; }
    [StringLength(1000)] public string? MemoGlobal { get; set; }

    // ---------------------------------------------------------
    // Creature (Pflicht)
    // ---------------------------------------------------------
    [StringLength(100)]
    private string _creature = string.Empty;
    public string? Creature
    {
        get => _creature;
        set { SetProperty(ref _creature, value); ValidateCreature(); }
    }
    [NotMapped] public string? CreatureError { get; set; }
    [NotMapped] public bool HasCreatureError { get; set; }

    // ---------------------------------------------------------
    // IHierarchicalEntity
    // ---------------------------------------------------------
    public int GetEntityId() => SpeciesId;
    public string GetDisplayName() => SpeciesName ?? $"Species {SpeciesId}";

    // ---------------------------------------------------------
    // VALIDATION SYSTEM
    // ---------------------------------------------------------

    private void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
            _errors[propertyName].Clear();
    }

    private void AddError(string propertyName, string message)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if (!_errors[propertyName].Contains(message))
            _errors[propertyName].Add(message);
    }

    private void UpdateErrorProperty(string propertyName)
    {
        var hasErrors = _errors.ContainsKey(propertyName) && _errors[propertyName].Any();
        var message = hasErrors ? string.Join(" | ", _errors[propertyName]) : "";

        switch (propertyName)
        {
            case nameof(SpeciesName):
                SpeciesNameError = message;
                HasSpeciesNameError = hasErrors;
                break;

            case nameof(Subspecies):
                SubspeciesError = message;
                HasSubspeciesError = hasErrors;
                break;

            case nameof(Divers):
                DiversError = message;
                HasDiversError = hasErrors;
                break;

            case nameof(GenusId):
                GenusIdError = message;
                HasGenusIdError = hasErrors;
                break;

            case nameof(CountId):
                CountIdError = message;
                HasCountIdError = hasErrors;
                break;

            case nameof(ValidYear):
                ValidYearError = message;
                HasValidYearError = hasErrors;
                break;

            case nameof(AuthorYear):
                AuthorYearError = message;
                HasAuthorYearError = hasErrors;
                break;

            case nameof(Importer):
                ImporterError = message;
                HasImporterError = hasErrors;
                break;

            case nameof(ImportingYear):
                ImportingYearError = message;
                HasImportingYearError = hasErrors;
                break;

            case nameof(LNumber):
                LNumberError = message;
                HasLNumberError = hasErrors;
                break;

            case nameof(LOrigin):
                LOriginError = message;
                HasLOriginError = hasErrors;
                break;

            case nameof(LDANumber):
                LDANumberError = message;
                HasLDANumberError = hasErrors;
                break;

            case nameof(LDAOrigin):
                LDAOriginError = message;
                HasLDAOriginError = hasErrors;
                break;

            case nameof(Creature):
                CreatureError = message;
                HasCreatureError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    // ---------------------------------------------------------
    // VALIDATION METHODS
    // ---------------------------------------------------------

    private void ValidateSpeciesName()
    {
        ClearErrors(nameof(SpeciesName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(SpeciesName))
            AddError(nameof(SpeciesName), loc.GetLocalizedString("Error_SpeciesName_Empty"));

        if (SpeciesName?.Length > 100)
            AddError(nameof(SpeciesName), loc.GetLocalizedString("Error_SpeciesName_TooLong"));

        UpdateErrorProperty(nameof(SpeciesName));
    }

    private void ValidateSubspecies()
    {
        ClearErrors(nameof(Subspecies));
        var loc = Localizer.Get();

        if (Subspecies?.Length > 100)
            AddError(nameof(Subspecies), loc.GetLocalizedString("Error_Subspecies_TooLong"));

        UpdateErrorProperty(nameof(Subspecies));
    }

    private void ValidateDivers()
    {
        ClearErrors(nameof(Divers));
        var loc = Localizer.Get();

        if (Divers?.Length > 100)
            AddError(nameof(Divers), loc.GetLocalizedString("Error_Divers_TooLong"));

        UpdateErrorProperty(nameof(Divers));
    }

    private void ValidateGenusId()
    {
        ClearErrors(nameof(GenusId));
        var loc = Localizer.Get();

        if (GenusId == null || GenusId <= 0)
            AddError(nameof(GenusId), loc.GetLocalizedString("Error_GenusId_Required"));

        UpdateErrorProperty(nameof(GenusId));
    }
    private void ValidateTradeName()
    {
        ClearErrors(nameof(TradeName));
        var loc = Localizer.Get();

        if (TradeName?.Length > 100)
            AddError(nameof(TradeName), loc.GetLocalizedString("Error_TradeName_TooLong"));

        UpdateErrorProperty(nameof(TradeName));
    }

    private void ValidateCountId()
    {
        ClearErrors(nameof(CountId));
        var loc = Localizer.Get();

        if (CountId == null || CountId <= 0)
            AddError(nameof(CountId), loc.GetLocalizedString("Error_CountId_Required"));

        UpdateErrorProperty(nameof(CountId));
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();

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

    private void ValidateAuthorYear()
    {
        ClearErrors(nameof(AuthorYear));
        var loc = Localizer.Get();

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

    private void ValidateImporter()
    {
        ClearErrors(nameof(Importer));
        var loc = Localizer.Get();

        if (Importer?.Length > 60)
            AddError(nameof(Importer), loc.GetLocalizedString("Error_Importer_TooLong"));

        UpdateErrorProperty(nameof(Importer));
    }

    private void ValidateImportingYear()
    {
        ClearErrors(nameof(ImportingYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ImportingYear))
            goto Notify;

        if (ImportingYear!.Length != 4)
        {
            AddError(nameof(ImportingYear), loc.GetLocalizedString("Error_ImportingYear_Length"));
            goto Notify;
        }

        if (!int.TryParse(ImportingYear, out var year))
        {
            AddError(nameof(ImportingYear), loc.GetLocalizedString("Error_ImportingYear_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(ImportingYear), loc.GetLocalizedString("Error_ImportingYear_Range"));

Notify:
        UpdateErrorProperty(nameof(ImportingYear));
    }

    private void ValidateLNumber()
    {
        ClearErrors(nameof(LNumber));
        var loc = Localizer.Get();

        if (!string.IsNullOrWhiteSpace(LNumber) && !LNumber.All(char.IsDigit))
            AddError(nameof(LNumber), loc.GetLocalizedString("Error_LNumber_NotNumeric"));

        UpdateErrorProperty(nameof(LNumber));
    }

    private void ValidateLOrigin()
    {
        ClearErrors(nameof(LOrigin));
        var loc = Localizer.Get();

        if (LOrigin?.Length > 50)
            AddError(nameof(LOrigin), loc.GetLocalizedString("Error_LOrigin_TooLong"));

        UpdateErrorProperty(nameof(LOrigin));
    }

    private void ValidateLdaNumber()
    {
        ClearErrors(nameof(LDANumber));
        var loc = Localizer.Get();

        if (!string.IsNullOrWhiteSpace(LDANumber) && !LDANumber.All(char.IsDigit))
            AddError(nameof(LDANumber), loc.GetLocalizedString("Error_LDANumber_NotNumeric"));

        UpdateErrorProperty(nameof(LDANumber));
    }

    private void ValidateLdaOrigin()
    {
        ClearErrors(nameof(LDAOrigin));
        var loc = Localizer.Get();

        if (LDAOrigin?.Length > 50)
            AddError(nameof(LDAOrigin), loc.GetLocalizedString("Error_LDAOrigin_TooLong"));

        UpdateErrorProperty(nameof(LDAOrigin));
    }

    private void ValidateCreature()
    {
        ClearErrors(nameof(Creature));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(Creature))
            AddError(nameof(Creature), loc.GetLocalizedString("Error_Creature_Empty"));

        if (Creature?.Length > 100)
            AddError(nameof(Creature), loc.GetLocalizedString("Error_Creature_TooLong"));

        UpdateErrorProperty(nameof(Creature));
    }

    // ---------------------------------------------------------
    // RANGE-VALIDIERUNGEN (Wasserwerte / Längen)
    // ---------------------------------------------------------

    private void ValidateWaterAndSizes()
    {
        var loc = Localizer.Get();

        // pH
        if (Ph1 is < 0 or > 14)
            AddError(nameof(Ph1), loc.GetLocalizedString("Error_Ph_Range"));
        if (Ph2 is < 0 or > 14)
            AddError(nameof(Ph2), loc.GetLocalizedString("Error_Ph_Range"));

        // Temperatur
        if (Temp1 is < 0 or > 40)
            AddError(nameof(Temp1), loc.GetLocalizedString("Error_Temp_Range"));
        if (Temp2 is < 0 or > 40)
            AddError(nameof(Temp2), loc.GetLocalizedString("Error_Temp_Range"));

        // Härte
        if (Hardness1 is < 0 or > 50)
            AddError(nameof(Hardness1), loc.GetLocalizedString("Error_Hardness_Range"));
        if (Hardness2 is < 0 or > 50)
            AddError(nameof(Hardness2), loc.GetLocalizedString("Error_Hardness_Range"));

        if (CarboHardness1 is < 0 or > 50)
            AddError(nameof(CarboHardness1), loc.GetLocalizedString("Error_CarboHardness_Range"));
        if (CarboHardness2 is < 0 or > 50)
            AddError(nameof(CarboHardness2), loc.GetLocalizedString("Error_CarboHardness_Range"));

        // Becken / Längen
        if (BasinLength != null && BasinLength <= 0)
            AddError(nameof(BasinLength), loc.GetLocalizedString("Error_BasinLength_Positive"));

        if (BasinHeight != null && BasinHeight <= 0)
            AddError(nameof(BasinHeight), loc.GetLocalizedString("Error_BasinHeight_Positive"));

        if (FishLength != null && FishLength <= 0)
            AddError(nameof(FishLength), loc.GetLocalizedString("Error_FishLength_Positive"));

        if (PlantLength != null && PlantLength <= 0)
            AddError(nameof(PlantLength), loc.GetLocalizedString("Error_PlantLength_Positive"));

        // Nur PropertyChanged für die betroffenen Properties auslösen
        UpdateErrorProperty(nameof(Ph1));
        UpdateErrorProperty(nameof(Ph2));
        UpdateErrorProperty(nameof(Temp1));
        UpdateErrorProperty(nameof(Temp2));
        UpdateErrorProperty(nameof(Hardness1));
        UpdateErrorProperty(nameof(Hardness2));
        UpdateErrorProperty(nameof(CarboHardness1));
        UpdateErrorProperty(nameof(CarboHardness2));
        UpdateErrorProperty(nameof(BasinLength));
        UpdateErrorProperty(nameof(BasinHeight));
        UpdateErrorProperty(nameof(FishLength));
        UpdateErrorProperty(nameof(PlantLength));
    }

    // ---------------------------------------------------------
    // GESAMTVALIDIERUNG
    // ---------------------------------------------------------

    public bool IsValid()
    {
        // Pflicht / Taxonomie
        ValidateSpeciesName();
        ValidateGenusId();
        ValidateCountId();
        ValidateCreature();

        // Jahre
        ValidateValidYear();
        ValidateAuthorYear();
        ValidateImportingYear();

        // Namen / Nummern
        ValidateSubspecies();
        ValidateDivers();
        ValidateTradeName();
        ValidateImporter();
        ValidateLNumber();
        ValidateLOrigin();
        ValidateLdaNumber();
        ValidateLdaOrigin();

        // Wasser / Längen
        ValidateWaterAndSizes();

        return !_errors.Values.Any(list => list.Count > 0);
    }
}



//public class Tbl75Species : IHierarchicalEntity
//{
//    [Key]
//    public int SpeciesId { get; set; }
//    [Required(ErrorMessageResourceName = "RequiredSpeciesName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string SpeciesName { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Subspecies { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Divers { get; set; }
//    public string SpeciesFullName => $"{SpeciesName} {Subspecies} {Divers}";
//    public string GenusSpeciesFullName => $"{Tbl66Genusses.GenusName} {SpeciesName} {Subspecies} {Divers}";

//    public Tbl66Genus? Genus { get; set; }

//    [Required]
//    public int GenusId { get; set; }
//    public int SpeciesgroupId { get; set; }
//    [Required]
//    public int CountId { get; set; }
//    public bool Valid { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ValidYear { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoSpecies { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string TradeName { get; set; }
//    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Author { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string AuthorYear { get; set; }
//    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Importer { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ImportingYear { get; set; }
//    public bool TypeSpecies { get; set; }
//    [StringLength(10, ErrorMessageResourceName = "StringLength_10", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string LNumber { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string LOrigin { get; set; }
//    [StringLength(10, ErrorMessageResourceName = "StringLength_10", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string LdaNumber { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string LdaOrigin { get; set; }
//    public int BasinLength { get; set; }
//    public decimal FishLength { get; set; }
//    public bool Karnivore { get; set; }
//    public bool Herbivore { get; set; }
//    public bool Limnivore { get; set; }
//    public bool Omnivore { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoFoods { get; set; }
//    public bool Difficult1 { get; set; }
//    public bool Difficult2 { get; set; }
//    public bool Difficult3 { get; set; }
//    public bool Difficult4 { get; set; }
//    public bool RegionTop { get; set; }
//    public bool RegionMiddle { get; set; }
//    public bool RegionBottom { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoRegion { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoTech { get; set; }
//    public decimal Ph1 { get; set; }
//    public decimal Ph2 { get; set; }
//    public int Temp1 { get; set; }
//    public int Temp2 { get; set; }
//    public int Hardness1 { get; set; }
//    public int Hardness2 { get; set; }
//    public int CarboHardness1 { get; set; }
//    public int CarboHardness2 { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Writer { get; set; }
//    public DateTime WriterDate { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Updater { get; set; }
//    public DateTime UpdaterDate { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoHusbandry { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoBreeding { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoBuilt { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoColor { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoSozial { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoDomorphism { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoSpecial { get; set; }
//    public int BasinHeight { get; set; }
//    public decimal PlantLength { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoReproduction { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoCulture { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MemoGlobal { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Creature { get; set; }

//    //     public byte[] RowVersion { get; set; }

//    [ForeignKey("GenusId")]
//    public virtual Tbl66Genus Tbl66Genusses { get; set; }
//    // public virtual ICollection<Tbl66Genus> Tbl66Genusses { get; set; }


//    [ForeignKey("SpeciesgroupId")]
//    public virtual Tbl68Speciesgroup Tbl68Speciesgroups { get; set; }

//    //public virtual ICollection<Tbl78Name> Tbl78Names { get; set; }
//    //public virtual ICollection<Tbl81Image> Tbl81Images { get; set; }
//    //public virtual ICollection<Tbl84Synonym> Tbl84Synonyms { get; set; }
//    //public virtual ICollection<Tbl87Geographic> Tbl87Geographics { get; set; }
//    //public virtual ICollection<Tbl90Reference> Tbl90References { get; set; }
//    //public virtual ICollection<Tbl93Comment> Tbl93Comments { get; set; }

//    public void SetSpeciesgroupId(object obj)
//    {
//        SpeciesgroupId = (int)obj;
//    }


//    public int GetEntityId()
//    {
//        throw new NotImplementedException();
//    }

//    public string GetDisplayName()
//    {
//        throw new NotImplementedException();
//    }
//}
