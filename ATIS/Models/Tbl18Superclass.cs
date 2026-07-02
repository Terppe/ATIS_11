using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ATIS.Models;

public class Tbl18Superclass : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
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
        if (_errors.Remove(property))
        {
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
    public int SuperclassId { get; set; }

    // ---------------------------------------------------------
    // FIELDS
    // ---------------------------------------------------------
    private string _superclassName;
    [Required]
    [StringLength(100)]
    public string SuperclassName
    {
        get => _superclassName;
        set
        {
            if (SetProperty(ref _superclassName, value))
                ValidateSuperclassName();
        }
    }

    [Required]
    public int SubphylumId { get; set; }

    [Required]
    public int CountId { get; set; }

    public bool Valid { get; set; }

    private string _validYear;
    [StringLength(4)]
    public string ValidYear
    {
        get => _validYear;
        set
        {
            if (SetProperty(ref _validYear, value))
                ValidateValidYear();
        }
    }

    private string _synonym;
    [StringLength(1000)]
    public string Synonym
    {
        get => _synonym;
        set
        {
            if (SetProperty(ref _synonym, value))
                ValidateSynonym();
        }
    }

    private string _author;
    [StringLength(60)]
    public string Author
    {
        get => _author;
        set
        {
            if (SetProperty(ref _author, value))
                ValidateAuthor();
        }
    }

    private string _authorYear;
    [StringLength(4)]
    public string AuthorYear
    {
        get => _authorYear;
        set
        {
            if (SetProperty(ref _authorYear, value))
                ValidateAuthorYear();
        }
    }

    private string _info;
    [StringLength(1000)]
    public string Info
    {
        get => _info;
        set
        {
            if (SetProperty(ref _info, value))
                ValidateInfo();
        }
    }

    [StringLength(200)]
    public string EngName { get; set; }

    [StringLength(200)]
    public string GerName { get; set; }

    [StringLength(200)]
    public string FraName { get; set; }

    [StringLength(200)]
    public string PorName { get; set; }

    [StringLength(50)]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }

    [StringLength(50)]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }

    private string _memo;
    [StringLength(1000)]
    public string Memo
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
    [ForeignKey("SubphylumId")]
    public virtual Tbl12Subphylum Subphylum { get; set; }

    [NotMapped]
    public string SubphylumName => Subphylum?.SubphylumName;

    // ---------------------------------------------------------
    // VALIDATION METHODS
    // ---------------------------------------------------------
    private void ValidateSuperclassName()
    {
        ClearErrors(nameof(SuperclassName));

        if (string.IsNullOrWhiteSpace(SuperclassName))
            AddError(nameof(SuperclassName), "Name is required.");

        if (SuperclassName?.Length > 100)
            AddError(nameof(SuperclassName), "Max length is 100.");
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));

        if (!string.IsNullOrWhiteSpace(ValidYear) && ValidYear.Length != 4)
            AddError(nameof(ValidYear), "Year must be 4 digits.");
    }

    private void ValidateSynonym()
    {
        ClearErrors(nameof(Synonym));

        if (!string.IsNullOrWhiteSpace(Synonym) && Synonym.Length > 1000)
            AddError(nameof(Synonym), "Max length is 1000.");
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

        if (!string.IsNullOrWhiteSpace(Info) && Info.Length > 1000)
            AddError(nameof(Info), "Max length is 1000.");
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
    [NotMapped] public string SuperclassNameError => _errors.ContainsKey(nameof(SuperclassName)) ? string.Join("; ", _errors[nameof(SuperclassName)]) : null;
    [NotMapped] public bool HasSuperclassNameError => _errors.ContainsKey(nameof(SuperclassName));

    [NotMapped] public string ValidYearError => _errors.ContainsKey(nameof(ValidYear)) ? string.Join("; ", _errors[nameof(ValidYear)]) : null;
    [NotMapped] public bool HasValidYearError => _errors.ContainsKey(nameof(ValidYear));

    [NotMapped] public string SynonymError => _errors.ContainsKey(nameof(Synonym)) ? string.Join("; ", _errors[nameof(Synonym)]) : null;
    [NotMapped] public bool HasSynonymError => _errors.ContainsKey(nameof(Synonym));

    [NotMapped] public string AuthorError => _errors.ContainsKey(nameof(Author)) ? string.Join("; ", _errors[nameof(Author)]) : null;
    [NotMapped] public bool HasAuthorError => _errors.ContainsKey(nameof(Author));

    [NotMapped] public string AuthorYearError => _errors.ContainsKey(nameof(AuthorYear)) ? string.Join("; ", _errors[nameof(AuthorYear)]) : null;
    [NotMapped] public bool HasAuthorYearError => _errors.ContainsKey(nameof(AuthorYear));

    [NotMapped] public string InfoError => _errors.ContainsKey(nameof(Info)) ? string.Join("; ", _errors[nameof(Info)]) : null;
    [NotMapped] public bool HasInfoError => _errors.ContainsKey(nameof(Info));

    [NotMapped] public string MemoError => _errors.ContainsKey(nameof(Memo)) ? string.Join("; ", _errors[nameof(Memo)]) : null;
    [NotMapped] public bool HasMemoError => _errors.ContainsKey(nameof(Memo));

    // ---------------------------------------------------------
    // IHierarchicalEntity
    // ---------------------------------------------------------
    public int GetEntityId() => SuperclassId;

    public string GetDisplayName() => SuperclassName;
}
