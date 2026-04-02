
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl90RefSource;

public class Tbl90RefSourceViewModel
    : Base1List1DetailViewModel<Models.Tbl90RefSource, RefSourceEditModel>
{
    private readonly IDataService _data;
    private readonly IUserService _user;

    public Tbl90RefSourceViewModel(IDataService data, IUserService user, IDialogService dialogService)
        : base(dialogService)
    {
        _data = data;
        _user = user;
    }

    // ---------------------------------------------------------
    // SEARCH
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl90RefSource>> SearchItemsAsync(string? text)
    {
        IsLoading = true;
        try
        {
            return await _data.SearchRefSourceAsync(text);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (für Refresh/Reload)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl90RefSource>> LoadItemsAsync()
    {
        // Base1 lädt bei Refresh/Reload immer die letzte Suche
        if (!string.IsNullOrWhiteSpace(SearchText))
            return await _data.SearchRefSourceAsync(SearchText);

        return Enumerable.Empty<Models.Tbl90RefSource>();
    }

    // ---------------------------------------------------------
    // CREATE NEW DOMAIN MODEL
    // ---------------------------------------------------------
    protected override Models.Tbl90RefSource CreateNewDomainModel()
        => new Models.Tbl90RefSource();

    // ---------------------------------------------------------
    // CLONE
    // ---------------------------------------------------------
    protected override Models.Tbl90RefSource CloneDomain(Models.Tbl90RefSource source)
        => source.Clone();

    // ---------------------------------------------------------
    // EXISTS?
    // ---------------------------------------------------------
    protected override Task<bool> ExistsAsync(RefSourceEditModel edit)
    {
        var domain = new Models.Tbl90RefSource
        {
            RefSourceName = edit.RefSourceName,
            SourceYear = edit.SourceYear,
            Author = edit.Author,
            Valid = edit.Valid,
            ValidYear = edit.ValidYear,
            Info = edit.Info,
            Notes = edit.Notes,
            Memo = edit.Memo
        };

        return _data.RefSourceExistsAsync(domain);
    }

    // ---------------------------------------------------------
    // DOMAIN → EDITMODEL
    // ---------------------------------------------------------
    protected override RefSourceEditModel ConvertDomainToEdit(Models.Tbl90RefSource d)
        => new RefSourceEditModel
        {
            RefSourceName = d.RefSourceName,
            SourceYear = d.SourceYear,
            Author = d.Author,
            ValidYear = d.ValidYear,
            Valid = d.Valid,
            Notes = d.Notes,
            Info = d.Info,
            Memo = d.Memo,
            CountId = d.CountId
        };

    // ---------------------------------------------------------
    // EDITMODEL → DOMAIN
    // ---------------------------------------------------------
    protected override void ConvertEditToDomain(RefSourceEditModel e, Models.Tbl90RefSource d)
    {
        d.RefSourceName = e.RefSourceName;
        d.SourceYear = e.SourceYear;
        d.Author = e.Author;
        d.ValidYear = e.ValidYear;
        d.Valid = e.Valid;
        d.Notes = e.Notes;
        d.Info = e.Info;
        d.Memo = e.Memo;
        d.CountId = e.CountId;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl90RefSource> SaveDomainAsync(Models.Tbl90RefSource domain)
        => await _data.UpsertRefSourceAsync(domain);

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task DeleteDomainAsync(Models.Tbl90RefSource domain)
        => await _data.DeleteRefSourceAsync(domain.RefSourceId);

    // ---------------------------------------------------------
    // GET ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl90RefSource model)
        => model.RefSourceId;

    // ---------------------------------------------------------
    // BEFORE SAVE
    // ---------------------------------------------------------
    protected override void OnBeforeSave(Models.Tbl90RefSource domain, bool isNew)
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
//using ATIS.Views.Base;

//namespace ATIS.Views.Modules.Tbl90RefSource;

//public class Tbl90RefSourceViewModel
//    : Base1List1DetailViewModel<Models.Tbl90RefSource, RefSourceEditModel>
//{
//    private readonly IDataService _data;
//    private readonly IUserService _user;

//    public Tbl90RefSourceViewModel(IDataService data, IUserService user, IDialogService dialogService)
//        : base(dialogService)
//    {
//        _data = data;
//        _user = user;
//    }

//    // ---------------------------------------------------------
//    // SEARCH
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl90RefSource>> SearchItemsAsync(string? text)
//    {
//        IsLoading = true;
//        try
//        {
//            return await _data.SearchRefSourceAsync(text);
//        }
//        finally
//        {
//            IsLoading = false;
//        }
//    }

//    // ---------------------------------------------------------
//    // CREATE NEW DOMAIN MODEL
//    // ---------------------------------------------------------
//    protected override Models.Tbl90RefSource CreateNewDomainModel()
//        => new Models.Tbl90RefSource();

//    // ---------------------------------------------------------
//    // CLONE
//    // ---------------------------------------------------------
//    protected override Models.Tbl90RefSource CloneDomain(Models.Tbl90RefSource source)
//        => source.Clone();

//    // ---------------------------------------------------------
//    // EXISTS?
//    // ---------------------------------------------------------
//    protected override Task<bool> ExistsAsync(RefSourceEditModel edit)
//    {
//        var domain = new Models.Tbl90RefSource
//        {
//            RefSourceName = edit.RefSourceName,
//            SourceYear = edit.SourceYear,
//            Author = edit.Author,
//            Valid = edit.Valid,
//            ValidYear = edit.ValidYear,
//            Info = edit.Info,
//            Notes = edit.Notes,
//            Memo = edit.Memo
//        };

//        return _data.RefSourceExistsAsync(domain);
//    }

//    // ---------------------------------------------------------
//    // DOMAIN → EDITMODEL
//    // ---------------------------------------------------------
//    protected override RefSourceEditModel ConvertDomainToEdit(Models.Tbl90RefSource d)
//        => new RefSourceEditModel
//        {
//            RefSourceName = d.RefSourceName,
//            SourceYear = d.SourceYear,
//            Author = d.Author,
//            ValidYear = d.ValidYear,
//            Valid = d.Valid,
//            Notes = d.Notes,
//            Info = d.Info,
//            Memo = d.Memo,
//            CountId = d.CountId
//        };

//    // ---------------------------------------------------------
//    // EDITMODEL → DOMAIN
//    // ---------------------------------------------------------
//    protected override void ConvertEditToDomain(RefSourceEditModel e, Models.Tbl90RefSource d)
//    {
//        d.RefSourceName = e.RefSourceName;
//        d.SourceYear = e.SourceYear;
//        d.Author = e.Author;
//        d.ValidYear = e.ValidYear;
//        d.Valid = e.Valid;
//        d.Notes = e.Notes;
//        d.Info = e.Info;
//        d.Memo = e.Memo;
//        d.CountId = e.CountId;
//    }

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override async Task<Models.Tbl90RefSource> SaveDomainAsync(Models.Tbl90RefSource domain)
//        => await _data.UpsertRefSourceAsync(domain);

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override async Task DeleteDomainAsync(Models.Tbl90RefSource domain)
//        => await _data.DeleteRefSourceAsync(domain.RefSourceId);

//    // ---------------------------------------------------------
//    // GET ID
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl90RefSource model)
//        => model.RefSourceId;

//    // ---------------------------------------------------------
//    // BEFORE SAVE
//    // ---------------------------------------------------------
//    protected override void OnBeforeSave(Models.Tbl90RefSource domain, bool isNew)
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
