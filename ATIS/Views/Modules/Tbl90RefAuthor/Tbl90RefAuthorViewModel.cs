using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public class Tbl90RefAuthorViewModel
    : Base1List1DetailViewModel<Models.Tbl90RefAuthor, RefAuthorEditModel>
{
    private readonly IDataService _data;
    private readonly IUserService _user;

    public Tbl90RefAuthorViewModel(IDataService data, IUserService user, IDialogService dialogService)
        : base(dialogService)
    {
        _data = data;
        _user = user;
    }

    // ---------------------------------------------------------
    // SEARCH
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl90RefAuthor>> SearchItemsAsync(string? text)
    {
        IsLoading = true;
        try
        {
            return await _data.SearchRefAuthorAsync(text);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (für Refresh/Reload)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl90RefAuthor>> LoadItemsAsync()
    {
        // Base1 lädt bei Refresh/Reload immer die letzte Suche
        if (!string.IsNullOrWhiteSpace(SearchText))
            return await _data.SearchRefAuthorAsync(SearchText);

        return Enumerable.Empty<Models.Tbl90RefAuthor>();
    }

    // ---------------------------------------------------------
    // CREATE NEW DOMAIN MODEL
    // ---------------------------------------------------------
    protected override Models.Tbl90RefAuthor CreateNewDomainModel()
        => new Models.Tbl90RefAuthor();

    // ---------------------------------------------------------
    // CLONE
    // ---------------------------------------------------------
    protected override Models.Tbl90RefAuthor CloneDomain(Models.Tbl90RefAuthor source)
        => source.Clone();

    // ---------------------------------------------------------
    // EXISTS?
    // ---------------------------------------------------------
    protected override Task<bool> ExistsAsync(RefAuthorEditModel edit)
    {
        var domain = new Models.Tbl90RefAuthor
        {
            RefAuthorName = edit.RefAuthorName,
            ValidYear = edit.ValidYear,
            Valid = edit.Valid,
            ISBN = edit.ISBN,
            ArticelTitle = edit.ArticelTitle,
            BookName = edit.BookName,
            Page = edit.Page,
            Page1 = edit.Page1,
            Publisher = edit.Publisher,
            PublicationPlace = edit.PublicationPlace,
            PublicationYear = edit.PublicationYear,
            Info = edit.Info,
            Notes = edit.Notes,
            Memo = edit.Memo
        };

        return _data.RefAuthorExistsAsync(domain);
    }

    // ---------------------------------------------------------
    // DOMAIN → EDITMODEL
    // ---------------------------------------------------------
    protected override RefAuthorEditModel ConvertDomainToEdit(Models.Tbl90RefAuthor d)
        => new RefAuthorEditModel
        {
            RefAuthorName = d.RefAuthorName,
            ValidYear = d.ValidYear,
            Valid = d.Valid,
            ISBN = d.ISBN,
            ArticelTitle = d.ArticelTitle,
            BookName = d.BookName,
            Page = d.Page,
            Page1 = d.Page1,
            Publisher = d.Publisher,
            PublicationPlace = d.PublicationPlace,
            PublicationYear = d.PublicationYear,
            Info = d.Info,
            Notes = d.Notes,
            Memo = d.Memo,
            CountId = d.CountId
        };

    // ---------------------------------------------------------
    // EDITMODEL → DOMAIN
    // ---------------------------------------------------------
    protected override void ConvertEditToDomain(RefAuthorEditModel e, Models.Tbl90RefAuthor d)
    {
        d.RefAuthorName = e.RefAuthorName;
        d.ValidYear = e.ValidYear;
        d.Valid = e.Valid;
        d.ISBN = e.ISBN;
        d.ArticelTitle = e.ArticelTitle;
        d.BookName = e.BookName;
        d.Page = e.Page;
        d.Page1 = e.Page1;
        d.Publisher = e.Publisher;
        d.PublicationPlace = e.PublicationPlace;
        d.PublicationYear = e.PublicationYear;
        d.Info = e.Info;
        d.Notes = e.Notes;
        d.Memo = e.Memo;
        d.CountId = e.CountId;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl90RefAuthor> SaveDomainAsync(Models.Tbl90RefAuthor domain)
        => await _data.UpsertRefAuthorAsync(domain);

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task DeleteDomainAsync(Models.Tbl90RefAuthor domain)
        => await _data.DeleteRefAuthorAsync(domain.RefAuthorId);

    // ---------------------------------------------------------
    // GET ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl90RefAuthor model)
        => model.RefAuthorId;

    // ---------------------------------------------------------
    // BEFORE SAVE
    // ---------------------------------------------------------
    protected override void OnBeforeSave(Models.Tbl90RefAuthor domain, bool isNew)
    {
        if (isNew)
        {
            domain.Writer = _user.CurrentUser;
            domain.WriterDate = DateTime.Now;
            domain.CountId = RandomHelper.Randomnumber();
        }

        domain.Updater = _user.CurrentUser;
        domain.UpdaterDate = DateTime.Now;
    }
}



//using ATIS.Helpers;
//using ATIS.Interfaces;
//using ATIS.Models;
//using ATIS.Views.Base;

//namespace ATIS.Views.Modules.Tbl90RefAuthor;

//public class Tbl90RefAuthorViewModel
//    : Base1List1DetailViewModel<Models.Tbl90RefAuthor, RefAuthorEditModel>
//{
//    private readonly IDataService _data;
//    private readonly IUserService _user;

//    public Tbl90RefAuthorViewModel(IDataService data, IUserService user, IDialogService dialogService)
//        : base(dialogService)
//    {
//        _data = data;
//        _user = user;
//    }

//    // ---------------------------------------------------------
//    // SEARCH
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl90RefAuthor>> SearchItemsAsync(string? text)
//    {
//        IsLoading = true;
//        try
//        {
//            return await _data.SearchRefAuthorAsync(text);
//        }
//        finally
//        {
//            IsLoading = false;
//        }
//    }

//    // ---------------------------------------------------------
//    // CREATE NEW DOMAIN MODEL
//    // ---------------------------------------------------------
//    protected override Models.Tbl90RefAuthor CreateNewDomainModel()
//        => new Models.Tbl90RefAuthor();

//    // ---------------------------------------------------------
//    // CLONE
//    // ---------------------------------------------------------
//    protected override Models.Tbl90RefAuthor CloneDomain(Models.Tbl90RefAuthor source)
//        => source.Clone();

//    // ---------------------------------------------------------
//    // EXISTS?
//    // ---------------------------------------------------------
//    protected override Task<bool> ExistsAsync(RefAuthorEditModel edit)
//    {
//        var domain = new Models.Tbl90RefAuthor
//        {
//            RefAuthorName = edit.RefAuthorName,
//            ValidYear = edit.ValidYear,
//            Valid = edit.Valid,
//            ISBN = edit.ISBN,
//            ArticelTitle = edit.ArticelTitle,
//            BookName = edit.BookName,
//            Page1 = edit.Page1,
//            Publisher = edit.Publisher,
//            PublicationPlace = edit.PublicationPlace,
//            PublicationYear = edit.PublicationYear,
//            Info = edit.Info,
//            Notes = edit.Notes,
//            Memo = edit.Memo
//        };

//        return _data.RefAuthorExistsAsync(domain);
//    }

//    // ---------------------------------------------------------
//    // DOMAIN → EDITMODEL
//    // ---------------------------------------------------------
//    protected override RefAuthorEditModel ConvertDomainToEdit(Models.Tbl90RefAuthor d)
//        => new RefAuthorEditModel
//        {
//            RefAuthorName = d.RefAuthorName,
//            ValidYear = d.ValidYear,
//            Valid = d.Valid,
//            ISBN = d.ISBN,
//            ArticelTitle = d.ArticelTitle,
//            BookName = d.BookName,
//            Page = d.Page,
//            Page1 = d.Page1,
//            Publisher = d.Publisher,
//            PublicationPlace = d.PublicationPlace,
//            PublicationYear = d.PublicationYear,
//            Info = d.Info,
//            Notes = d.Notes,
//            Memo = d.Memo,
//            CountId = d.CountId
//        };

//    // ---------------------------------------------------------
//    // EDITMODEL → DOMAIN
//    // ---------------------------------------------------------
//    protected override void ConvertEditToDomain(RefAuthorEditModel e, Models.Tbl90RefAuthor d)
//    {
//        d.RefAuthorName = e.RefAuthorName;
//        d.ValidYear = e.ValidYear;
//        d.Valid = e.Valid;
//        d.ISBN = e.ISBN;
//        d.ArticelTitle = e.ArticelTitle;
//        d.BookName = e.BookName;
//        d.Page = e.Page;
//        d.Page1 = e.Page1;
//        d.Publisher = e.Publisher;
//        d.PublicationPlace = e.PublicationPlace;
//        d.PublicationYear = e.PublicationYear;
//        d.Info = e.Info;
//        d.Notes = e.Notes;
//        d.Memo = e.Memo;
//        d.CountId = e.CountId;
//    }

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override async Task<Models.Tbl90RefAuthor> SaveDomainAsync(Models.Tbl90RefAuthor domain)
//        => await _data.UpsertRefAuthorAsync(domain);

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override async Task DeleteDomainAsync(Models.Tbl90RefAuthor domain)
//        => await _data.DeleteRefAuthorAsync(domain.RefAuthorId);

//    // ---------------------------------------------------------
//    // GET ID
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl90RefAuthor model)
//        => model.RefAuthorId;

//    // ---------------------------------------------------------
//    // BEFORE SAVE
//    // ---------------------------------------------------------
//    protected override void OnBeforeSave(Models.Tbl90RefAuthor domain, bool isNew)
//    {
//        if (isNew)
//        {
//            domain.Writer = _user.CurrentUser;
//            domain.WriterDate = DateTime.Now;
//            domain.CountId = RandomHelper.Randomnumber();
//        }

//        domain.Updater = _user.CurrentUser;
//        domain.UpdaterDate = DateTime.Now;
//    }
//}
