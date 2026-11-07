using System.Collections.ObjectModel;
using System.Diagnostics;
using ATIS.Core.Taxonomy;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search;
using ATIS.Views.Search.DirectChildren;
using Microsoft.EntityFrameworkCore;
using static ATIS.Interfaces.IDataService;
using static ATIS.Views.Report.PdfModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static SkiaSharp.HarfBuzz.SKShaper;
//https://www.reddit.com/r/reactjs/comments/m4mhp7/how_to_update_state_in_useeffect_when_getting/

namespace ATIS.Services;

public partial class DataService : ObservableObject, IDataService
{
    private const string Value = "#";
    private readonly IUserService _user;
    private readonly AtisDbContext? _context;
    private readonly IDbContextFactory<AtisDbContext>? _factory;

    public DataService(IDbContextFactory<AtisDbContext> factory, IUserService user)
    {
        _factory = factory;
        _user = user;
    }

    private void ApplyAuditInfo(IAuditable entity, bool isNew)
    {
        var now = DateTime.Now;

        if (isNew)
        {
            entity.Writer = Environment.UserName; 
            entity.WriterDate = now;
            entity.CountId = RandomHelper.Randomnumber();
        }

        entity.Updater = Environment.UserName; 
        entity.UpdaterDate = now;
    }

    private static void CopyProperties<T>(T source, T target, params string[] exclude)
    {
        var props = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            .Where(p => !exclude.Contains(p.Name));

        foreach (var prop in props)
        {
            var value = prop.GetValue(source);
            prop.SetValue(target, value);
        }
    }

    //Base-Logik für Upsert-Operationen, die in allen Entitäten verwendet werden kann
    protected async Task<TEntity> UpsertAsync<TEntity, TKey>(
        TEntity uiEntity,
        Func<DbContext, DbSet<TEntity>> setSelector,
        Func<TEntity, TKey> keySelector,
        Func<DbSet<TEntity>, TKey, Task<TEntity?>> findByKeyAsync,
        Action<TEntity, TEntity> mapBusinessFields)
        where TEntity : class, IDataService.IAuditable, new()
    {
        await using var context = _factory.CreateDbContext();
        var set = setSelector(context);

        var key = keySelector(uiEntity);
        var dbEntity = await findByKeyAsync(set, key);

        var isNew = dbEntity is null;
        dbEntity ??= new TEntity();

        if (isNew)
            set.Add(dbEntity);

        mapBusinessFields(uiEntity, dbEntity);
        ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();
        return dbEntity;
    }

    #region CRUD

    #region Regnum

    // ---------------------------------------------------------                  
    // GetAllRegnumAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl03Regnum>> GetAllRegnumAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums
            .AsNoTracking()
            .OrderBy(e => e.RegnumName)
            .ThenBy(a => a.Subregnum)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchRegnumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl03Regnum>> SearchRegnumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl03Regnum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl03Regnums
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RegnumId == id || e.RegnumName.StartsWith(search))
                .OrderBy(e => e.RegnumName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RegnumName.StartsWith(search))
            .OrderBy(e => e.RegnumName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertRegnumAsync(Tbl03Regnum uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl03Regnum> UpsertRegnumAsync(Tbl03Regnum uiEntity)
    {
        return UpsertAsync<Tbl03Regnum, int>(
            uiEntity,
            ctx => ctx.Set<Tbl03Regnum>(),
            x => x.RegnumId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RegnumId == id),
            MapRegnumFields
        );
    }

    // ---------------------------------------------------------                  
    // MapRegnumFields(Tbl03Regnum source, Tbl03Regnum target)
    // ---------------------------------------------------------                 
    private static void MapRegnumFields(Tbl03Regnum source, Tbl03Regnum target)
    {
        target.RegnumName = source.RegnumName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetRegnumByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl03Regnum?> GetRegnumByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RegnumId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteRegnumAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRegnumAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl03Regnums.FindAsync(id);
        if (entity != null)
        {
            context.Tbl03Regnums.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl03Regnum entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumExistsAsync(Tbl03Regnum entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums.AnyAsync(x =>
            x.RegnumName == entity.RegnumName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.RegnumId != entity.RegnumId
        );
    }

    // ---------------------------------------------------------                  
    // HasPhylumAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumHasPhylumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl06Phylums
            .AnyAsync(x => x.RegnumId == regnumId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumHasReferenceAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.RegnumId == regnumId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumHasCommentAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.RegnumId == regnumId);
    }

    // ---------------------------------------------------------                  
    // GetPhylumsByRegnumIdAsync(int regnumId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl06Phylum>> GetPhylumsByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .Where(x => x.RegnumId == regnumId)
            .OrderBy(x => x.PhylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.RegnumId == regnumId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.RegnumId == regnumId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.RegnumId == regnumId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.RegnumId == regnumId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Regnum

    #region Phylum

    // ---------------------------------------------------------                  
    // GetAllPhylumAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl06Phylum>> GetAllPhylumAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .AsNoTracking()
            .OrderBy(e => e.PhylumName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchPhylumWithRegnumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl06Phylum>> SearchPhylumWithRegnumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl06Phylum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl06Phylums
            .Include(x => x.Regnum)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.PhylumId == id || e.PhylumName.StartsWith(search))
                .OrderBy(e => e.PhylumName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.PhylumName.StartsWith(search))
            .OrderBy(e => e.PhylumName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertPhylumAsync(Tbl06Phylum uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl06Phylum> UpsertPhylumAsync(Tbl06Phylum uiEntity)
    {
        return UpsertAsync<Tbl06Phylum, int>(
            uiEntity,
            ctx => ctx.Set<Tbl06Phylum>(),
            x => x.PhylumId,
            (set, id) => set.FirstOrDefaultAsync(x => x.PhylumId == id),
            MapPhylumFields
        );
    }

    // ---------------------------------------------------------                  
    // MapPhylumFields(Tbl06Phylum source, Tbl06Phylum target)
    // ---------------------------------------------------------                 
    private static void MapPhylumFields(Tbl06Phylum source, Tbl06Phylum target)
    {
        target.PhylumName = source.PhylumName;
        target.RegnumId = source.RegnumId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetPhylumByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl06Phylum?> GetPhylumByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PhylumId == id);
    }

    // ---------------------------------------------------------                  
    // DeletePhylumAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeletePhylumAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl06Phylums.FindAsync(id);
        if (entity != null)
        {
            context.Tbl06Phylums.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl06Phylum entity)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumExistsAsync(Tbl06Phylum entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums.AnyAsync(x =>
            x.PhylumName == entity.PhylumName &&
            x.RegnumId == entity.RegnumId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.PhylumId != entity.PhylumId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubphylumAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumHasSubphylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl12Subphylums
            .AnyAsync(x => x.PhylumId == phylumId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumHasReferenceAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.PhylumId == phylumId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumHasCommentAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.PhylumId == phylumId);
    }

    // ---------------------------------------------------------                  
    // GetPhylumForRegnumAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl06Phylum>> GetPhylumForRegnumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .Where(e => e.RegnumId == regnumId)
            .Include(e => e.Regnum)
            .OrderBy(e => e.PhylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubphylumsByPhylumIdAsync(int phylumId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl12Subphylum>> GetSubphylumsByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .Where(x => x.PhylumId == phylumId)
            .OrderBy(x => x.SubphylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.PhylumId == phylumId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.PhylumId == phylumId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.PhylumId == phylumId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.PhylumId == phylumId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Phylum

    #region Subphylum

    // ---------------------------------------------------------                  
    // GetAllSubphylumAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl12Subphylum>> GetAllSubphylumAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .AsNoTracking()
            .OrderBy(e => e.SubphylumName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSubphylumWithPhylumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl12Subphylum>> SearchSubphylumWithPhylumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl12Subphylum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl12Subphylums
            .Include(x => x.Phylum)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubphylumId == id || e.SubphylumName.StartsWith(search))
                .OrderBy(e => e.SubphylumName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubphylumName.StartsWith(search))
            .OrderBy(e => e.SubphylumName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSubphylumAsync(Tbl12Subphylum uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl12Subphylum> UpsertSubphylumAsync(Tbl12Subphylum uiEntity)
    {
        return UpsertAsync<Tbl12Subphylum, int>(
            uiEntity,
            ctx => ctx.Set<Tbl12Subphylum>(),
            x => x.SubphylumId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubphylumId == id),
            MapSubphylumFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSubphylumFields(Tbl12Subphylum source, Tbl12Subphylum target)
    // ---------------------------------------------------------                 
    private static void MapSubphylumFields(Tbl12Subphylum source, Tbl12Subphylum target)
    {
        target.SubphylumName = source.SubphylumName;
        target.PhylumId = source.PhylumId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSubphylumByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl12Subphylum?> GetSubphylumByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubphylumId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSubphylumAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubphylumAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl12Subphylums.FindAsync(id);
        if (entity != null)
        {
            context.Tbl12Subphylums.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl12Subphylum entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumExistsAsync(Tbl12Subphylum entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums.AnyAsync(x =>
            x.SubphylumName == entity.SubphylumName &&
            x.PhylumId == entity.PhylumId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubphylumId != entity.SubphylumId
        );
    }

    // ---------------------------------------------------------                  
    // HasSuperclassAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumHasSuperclassAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl18Superclasses
            .AnyAsync(x => x.SubphylumId == subphylumId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumHasReferenceAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubphylumId == subphylumId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumHasCommentAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubphylumId == subphylumId);
    }

    // ---------------------------------------------------------                  
    // GetSubphylumForPhylumAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl12Subphylum>> GetSubphylumForPhylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .Where(e => e.PhylumId == phylumId)
            .Include(e => e.Phylum)
            .OrderBy(e => e.SubphylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSuperclassesBySubphylumIdAsync(int subphylumId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl18Superclass>> GetSuperclassesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .Where(x => x.SubphylumId == subphylumId)
            .OrderBy(x => x.SuperclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubphylumId == subphylumId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubphylumId == subphylumId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubphylumId == subphylumId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubphylumId == subphylumId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subphylum

    #region Superclass

    // ---------------------------------------------------------                  
    // GetAllSuperclassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl18Superclass>> GetAllSuperclassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .AsNoTracking()
            .OrderBy(e => e.SuperclassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSuperclassWithSubphylumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl18Superclass>> SearchSuperclassWithSubphylumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl18Superclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl18Superclasses
            .Include(x => x.Subphylum)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SuperclassId == id || e.SuperclassName.StartsWith(search))
                .OrderBy(e => e.SuperclassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SuperclassName.StartsWith(search))
            .OrderBy(e => e.SuperclassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSuperclassAsync(Tbl18Superclass uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl18Superclass> UpsertSuperclassAsync(Tbl18Superclass uiEntity)
    {
        return UpsertAsync<Tbl18Superclass, int>(
            uiEntity,
            ctx => ctx.Set<Tbl18Superclass>(),
            x => x.SuperclassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SuperclassId == id),
            MapSuperclassFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSuperclassFields(Tbl18Superclass source, Tbl18Superclass target)
    // ---------------------------------------------------------                 
    private static void MapSuperclassFields(Tbl18Superclass source, Tbl18Superclass target)
    {
        target.SuperclassName = source.SuperclassName;
        target.SubphylumId = source.SubphylumId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSuperclassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl18Superclass?> GetSuperclassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SuperclassId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSuperclassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSuperclassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl18Superclasses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl18Superclasses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl18Superclass entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassExistsAsync(Tbl18Superclass entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses.AnyAsync(x =>
            x.SuperclassName == entity.SuperclassName &&
            x.SubphylumId == entity.SubphylumId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SuperclassId != entity.SuperclassId
        );
    }

    // ---------------------------------------------------------                  
    // HasClassAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassHasClassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl21Classes
            .AnyAsync(x => x.SuperclassId == superclassId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassHasReferenceAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SuperclassId == superclassId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassHasCommentAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SuperclassId == superclassId);
    }

    // ---------------------------------------------------------                  
    // GetSuperclassForSubphylumAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl18Superclass>> GetSuperclassForSubphylumAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .Where(e => e.SubphylumId == subphylumId)
            .Include(e => e.Subphylum)
            .OrderBy(e => e.SuperclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetClassesBySuperclassIdAsync(int superclassId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl21Class>> GetClassesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .Where(x => x.SuperclassId == superclassId)
            .OrderBy(x => x.ClassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SuperclassId == superclassId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SuperclassId == superclassId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SuperclassId == superclassId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SuperclassId == superclassId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Superclass

    #region Class

    // ---------------------------------------------------------                  
    // GetAllClassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl21Class>> GetAllClassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .AsNoTracking()
            .OrderBy(e => e.ClassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchClassWithSuperclassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl21Class>> SearchClassWithSuperclassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl21Class>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl21Classes
            .Include(x => x.Superclass)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.ClassId == id || e.ClassName.StartsWith(search))
                .OrderBy(e => e.ClassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.ClassName.StartsWith(search))
            .OrderBy(e => e.ClassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertClassAsync(Tbl21Class uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl21Class> UpsertClassAsync(Tbl21Class uiEntity)
    {
        return UpsertAsync<Tbl21Class, int>(
            uiEntity,
            ctx => ctx.Set<Tbl21Class>(),
            x => x.ClassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.ClassId == id),
            MapClassFields
        );
    }

    // ---------------------------------------------------------                  
    // MapClassFields(Tbl21Class source, Tbl21Class target)
    // ---------------------------------------------------------                 
    private static void MapClassFields(Tbl21Class source, Tbl21Class target)
    {
        target.ClassName = source.ClassName;
        target.SuperclassId = source.SuperclassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetClassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl21Class?> GetClassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClassId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteClassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteClassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl21Classes.FindAsync(id);
        if (entity != null)
        {
            context.Tbl21Classes.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl21Class entity)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassExistsAsync(Tbl21Class entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes.AnyAsync(x =>
            x.ClassName == entity.ClassName &&
            x.SuperclassId == entity.SuperclassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.ClassId != entity.ClassId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubclassAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassHasSubclassAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl24Subclasses
            .AnyAsync(x => x.ClassId == classId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassHasReferenceAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.ClassId == classId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassHasCommentAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.ClassId == classId);
    }

    // ---------------------------------------------------------                  
    // GetClassForSuperclassAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl21Class>> GetClassForSuperclassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .Where(e => e.SuperclassId == superclassId)
            .Include(e => e.Superclass)
            .OrderBy(e => e.ClassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubclassesByClassIdAsync(int classId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl24Subclass>> GetSubclassesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .Where(x => x.ClassId == classId)
            .OrderBy(x => x.SubclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.ClassId == classId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.ClassId == classId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.ClassId == classId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.ClassId == classId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Class

    #region Subclass

    // ---------------------------------------------------------                  
    // GetAllSubclassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl24Subclass>> GetAllSubclassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .AsNoTracking()
            .OrderBy(e => e.SubclassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSubclassWithClassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl24Subclass>> SearchSubclassWithClassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl24Subclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl24Subclasses
            .Include(x => x.Class)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubclassId == id || e.SubclassName.StartsWith(search))
                .OrderBy(e => e.SubclassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubclassName.StartsWith(search))
            .OrderBy(e => e.SubclassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSubclassAsync(Tbl24Subclass uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl24Subclass> UpsertSubclassAsync(Tbl24Subclass uiEntity)
    {
        return UpsertAsync<Tbl24Subclass, int>(
            uiEntity,
            ctx => ctx.Set<Tbl24Subclass>(),
            x => x.SubclassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubclassId == id),
            MapSubclassFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSubclassFields(Tbl24Subclass source, Tbl24Subclass target)
    // ---------------------------------------------------------                 
    private static void MapSubclassFields(Tbl24Subclass source, Tbl24Subclass target)
    {
        target.SubclassName = source.SubclassName;
        target.ClassId = source.ClassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSubclassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl24Subclass?> GetSubclassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubclassId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSubclassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubclassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl24Subclasses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl24Subclasses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl24Subclass entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassExistsAsync(Tbl24Subclass entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses.AnyAsync(x =>
            x.SubclassName == entity.SubclassName &&
            x.ClassId == entity.ClassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubclassId != entity.SubclassId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfraclassAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassHasInfraclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl27Infraclasses
            .AnyAsync(x => x.SubclassId == subclassId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassHasReferenceAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubclassId == subclassId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassHasCommentAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubclassId == subclassId);
    }

    // ---------------------------------------------------------                  
    // GetSubclassForClassAsync(int classeId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl24Subclass>> GetSubclassForClassAsync(int classeId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .Where(e => e.ClassId == classeId)
            .Include(e => e.Class)
            .OrderBy(e => e.SubclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetInfraclassesBySubclassIdAsync(int subclassId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl27Infraclass>> GetInfraclassesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .Where(x => x.SubclassId == subclassId)
            .OrderBy(x => x.InfraclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubclassIdAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubclassId == subclassId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubclassIdAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubclassId == subclassId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubclassIdAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubclassId == subclassId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubclassIdAsync(int subclassId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubclassId == subclassId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subclass

    #region Infraclass

    // ---------------------------------------------------------                  
    // GetAllInfraclassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl27Infraclass>> GetAllInfraclassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .AsNoTracking()
            .OrderBy(e => e.InfraclassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchInfraclassWithSubclassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl27Infraclass>> SearchInfraclassWithSubclassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl27Infraclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl27Infraclasses
            .Include(x => x.Subclass)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfraclassId == id || e.InfraclassName.StartsWith(search))
                .OrderBy(e => e.InfraclassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfraclassName.StartsWith(search))
            .OrderBy(e => e.InfraclassName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertInfraclassAsync(Tbl27Infraclass uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl27Infraclass> UpsertInfraclassAsync(Tbl27Infraclass uiEntity)
    {
        return UpsertAsync<Tbl27Infraclass, int>(
            uiEntity,
            ctx => ctx.Set<Tbl27Infraclass>(),
            x => x.InfraclassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfraclassId == id),
            MapInfraclassFields
        );
    }

    // ---------------------------------------------------------                  
    // MapInfraclassFields(Tbl27Infraclass source, Tbl27Infraclass target)
    // ---------------------------------------------------------                 
    private static void MapInfraclassFields(Tbl27Infraclass source, Tbl27Infraclass target)
    {
        target.InfraclassName = source.InfraclassName;
        target.SubclassId = source.SubclassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetInfraclassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl27Infraclass?> GetInfraclassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfraclassId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteInfraclassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfraclassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl27Infraclasses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl27Infraclasses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl27Infraclass entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassExistsAsync(Tbl27Infraclass entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses.AnyAsync(x =>
            x.InfraclassName == entity.InfraclassName &&
            x.SubclassId == entity.SubclassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfraclassId != entity.InfraclassId
        );
    }

    // ---------------------------------------------------------                  
    // HasLegioAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassHasLegioAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl30Legios
            .AnyAsync(x => x.InfraclassId == infraclassId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassHasReferenceAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfraclassId == infraclassId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassHasCommentAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfraclassId == infraclassId);
    }

    // ---------------------------------------------------------                  
    // GetInfraclassForSubclassAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl27Infraclass>> GetInfraclassForSubclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .Where(e => e.SubclassId == subclassId)
            .Include(e => e.Subclass)
            .OrderBy(e => e.InfraclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetLegiosByInfraclassIdAsync(int infraclassId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl30Legio>> GetLegiosByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .Where(x => x.InfraclassId == infraclassId)
            .OrderBy(x => x.LegioName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByInfraclassIdAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.InfraclassId == infraclassId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByInfraclassIdAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.InfraclassId == infraclassId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByInfraclassIdAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.InfraclassId == infraclassId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByInfraclassIdAsync(int infraclassId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.InfraclassId == infraclassId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Infraclass

    #region Legio

    // ---------------------------------------------------------                  
    // GetAllLegioAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl30Legio>> GetAllLegioAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .AsNoTracking()
            .OrderBy(e => e.LegioName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchLegioWithInfraclassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl30Legio>> SearchLegioWithInfraclassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl30Legio>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl30Legios
            .Include(x => x.Infraclass)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.LegioId == id || e.LegioName.StartsWith(search))
                .OrderBy(e => e.LegioName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.LegioName.StartsWith(search))
            .OrderBy(e => e.LegioName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertLegioAsync(Tbl30Legio uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl30Legio> UpsertLegioAsync(Tbl30Legio uiEntity)
    {
        return UpsertAsync<Tbl30Legio, int>(
            uiEntity,
            ctx => ctx.Set<Tbl30Legio>(),
            x => x.LegioId,
            (set, id) => set.FirstOrDefaultAsync(x => x.LegioId == id),
            MapLegioFields
        );
    }

    // ---------------------------------------------------------                  
    // MapLegioFields(Tbl30Legio source, Tbl30Legio target)
    // ---------------------------------------------------------                 
    private static void MapLegioFields(Tbl30Legio source, Tbl30Legio target)
    {
        target.LegioName = source.LegioName;
        target.InfraclassId = source.InfraclassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetLegioByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl30Legio> GetLegioByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LegioId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteLegioAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteLegioAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl30Legios.FindAsync(id);
        if (entity != null)
        {
            context.Tbl30Legios.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl30Legio entity)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioExistsAsync(Tbl30Legio entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios.AnyAsync(x =>
            x.LegioName == entity.LegioName &&
            x.InfraclassId == entity.InfraclassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.LegioId != entity.LegioId
        );
    }

    // ---------------------------------------------------------                  
    // HasOrdoAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioHasOrdoAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl33Ordos
            .AnyAsync(x => x.LegioId == legioId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioHasReferenceAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.LegioId == legioId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioHasCommentAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.LegioId == legioId);
    }

    // ---------------------------------------------------------                  
    // GetLegioForInfraclassAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl30Legio>> GetLegioForInfraclassAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .Where(e => e.InfraclassId == infraclassId)
            .Include(e => e.Infraclass)
            .OrderBy(e => e.LegioName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetOrdosByLegioIdAsync(int legioId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl33Ordo>> GetOrdosByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .Where(x => x.LegioId == legioId)
            .OrderBy(x => x.OrdoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByLegioIdAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.LegioId == legioId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByLegioIdAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.LegioId == legioId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByLegioIdAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.LegioId == legioId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByLegioIdAsync(int legioId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.LegioId == legioId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Legio

    #region Ordo

    // ---------------------------------------------------------                  
    // GetAllOrdoAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl33Ordo>> GetAllOrdoAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .AsNoTracking()
            .OrderBy(e => e.OrdoName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchOrdoWithLegioAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl33Ordo>> SearchOrdoWithLegioAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl33Ordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl33Ordos
            .Include(x => x.Legio)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.OrdoId == id || e.OrdoName.StartsWith(search))
                .OrderBy(e => e.OrdoName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.OrdoName.StartsWith(search))
            .OrderBy(e => e.OrdoName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertOrdoAsync(Tbl33Ordo uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl33Ordo> UpsertOrdoAsync(Tbl33Ordo uiEntity)
    {
        return UpsertAsync<Tbl33Ordo, int>(
            uiEntity,
            ctx => ctx.Set<Tbl33Ordo>(),
            x => x.OrdoId,
            (set, id) => set.FirstOrDefaultAsync(x => x.OrdoId == id),
            MapOrdoFields
        );
    }

    // ---------------------------------------------------------                  
    // MapOrdoFields(Tbl33Ordo source, Tbl33Ordo target)
    // ---------------------------------------------------------                 
    private static void MapOrdoFields(Tbl33Ordo source, Tbl33Ordo target)
    {
        target.OrdoName = source.OrdoName;
        target.LegioId = source.LegioId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetOrdoByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl33Ordo> GetOrdoByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrdoId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteOrdoAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteOrdoAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl33Ordos.FindAsync(id);
        if (entity != null)
        {
            context.Tbl33Ordos.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl33Ordo entity)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoExistsAsync(Tbl33Ordo entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos.AnyAsync(x =>
            x.OrdoName == entity.OrdoName &&
            x.LegioId == entity.LegioId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.OrdoId != entity.OrdoId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubordoAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoHasSubordoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl36Subordos
            .AnyAsync(x => x.OrdoId == ordoId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoHasReferenceAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.OrdoId == ordoId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoHasCommentAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.OrdoId == ordoId);
    }

    // ---------------------------------------------------------                  
    // GetOrdoForLegioAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl33Ordo>> GetOrdoForLegioAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .Where(e => e.LegioId == legioId)
            .Include(e => e.Legio)
            .OrderBy(e => e.OrdoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubordosByOrdoIdAsync(int ordoId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl36Subordo>> GetSubordosByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .Where(x => x.OrdoId == ordoId)
            .OrderBy(x => x.SubordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByOrdoIdAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.OrdoId == ordoId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByOrdoIdAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.OrdoId == ordoId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByOrdoIdAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.OrdoId == ordoId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByOrdoIdAsync(int ordoId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.OrdoId == ordoId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Ordo

    #region Subordo

    // ---------------------------------------------------------                  
    // GetAllSubordoAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl36Subordo>> GetAllSubordoAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .AsNoTracking()
            .OrderBy(e => e.SubordoName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSubordoWithOrdoAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl36Subordo>> SearchSubordoWithOrdoAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl36Subordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl36Subordos
            .Include(x => x.Ordo)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubordoId == id || e.SubordoName.StartsWith(search))
                .OrderBy(e => e.SubordoName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubordoName.StartsWith(search))
            .OrderBy(e => e.SubordoName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSubordoAsync(Tbl36Subordo uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl36Subordo> UpsertSubordoAsync(Tbl36Subordo uiEntity)
    {
        return UpsertAsync<Tbl36Subordo, int>(
            uiEntity,
            ctx => ctx.Set<Tbl36Subordo>(),
            x => x.SubordoId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubordoId == id),
            MapSubordoFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSubordoFields(Tbl36Subordo source, Tbl36Subordo target)
    // ---------------------------------------------------------                 
    private static void MapSubordoFields(Tbl36Subordo source, Tbl36Subordo target)
    {
        target.SubordoName = source.SubordoName;
        target.OrdoId = source.OrdoId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSubordoByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl36Subordo> GetSubordoByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();
                return await context.Tbl36Subordos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubordoId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSubordoAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubordoAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl36Subordos.FindAsync(id);
        if (entity != null)
        {
            context.Tbl36Subordos.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl36Subordo entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoExistsAsync(Tbl36Subordo entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos.AnyAsync(x =>
            x.SubordoName == entity.SubordoName &&
            x.OrdoId == entity.OrdoId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubordoId != entity.SubordoId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfraordoAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoHasInfraordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl39Infraordos
            .AnyAsync(x => x.SubordoId == subordoId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoHasReferenceAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubordoId == subordoId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoHasCommentAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubordoId == subordoId);
    }

    // ---------------------------------------------------------                  
    // GetSubordoForOrdoAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl36Subordo>> GetSubordoForOrdoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .Where(e => e.OrdoId == ordoId)
            .Include(e => e.Ordo)
            .OrderBy(e => e.SubordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetInfraordosBySubordoIdAsync(int subordoId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl39Infraordo>> GetInfraordosBySubordoIdAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .Where(x => x.SubordoId == subordoId)
            .OrderBy(x => x.InfraordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubordoIdAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubordoIdAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubordoId == subordoId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubordoIdAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubordoIdAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubordoId == subordoId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubordoIdAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubordoIdAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubordoId == subordoId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubordoIdAsync(int subordoId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubordoIdAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubordoId == subordoId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subordo

    #region Infraordo

    // ---------------------------------------------------------                  
    // GetAllInfraordoAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl39Infraordo>> GetAllInfraordoAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .AsNoTracking()
            .OrderBy(e => e.InfraordoName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchInfraordoWithSubordoAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl39Infraordo>> SearchInfraordoWithSubordoAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl39Infraordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl39Infraordos
            .Include(x => x.Subordo)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfraordoId == id || e.InfraordoName.StartsWith(search))
                .OrderBy(e => e.InfraordoName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfraordoName.StartsWith(search))
            .OrderBy(e => e.InfraordoName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertInfraordoAsync(Tbl39Infraordo uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl39Infraordo> UpsertInfraordoAsync(Tbl39Infraordo uiEntity)
    {
        return UpsertAsync<Tbl39Infraordo, int>(
            uiEntity,
            ctx => ctx.Set<Tbl39Infraordo>(),
            x => x.InfraordoId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfraordoId == id),
            MapInfraordoFields
        );
    }

    // ---------------------------------------------------------                  
    // MapInfraordoFields(Tbl39Infraordo source, Tbl39Infraordo target)
    // ---------------------------------------------------------                 
    private static void MapInfraordoFields(Tbl39Infraordo source, Tbl39Infraordo target)
    {
        target.InfraordoName = source.InfraordoName;
        target.SubordoId = source.SubordoId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetInfraordoByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl39Infraordo> GetInfraordoByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfraordoId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteInfraordoAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfraordoAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl39Infraordos.FindAsync(id);
        if (entity != null)
        {
            context.Tbl39Infraordos.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl39Infraordo entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoExistsAsync(Tbl39Infraordo entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos.AnyAsync(x =>
            x.InfraordoName == entity.InfraordoName &&
            x.SubordoId == entity.SubordoId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfraordoId != entity.InfraordoId
        );
    }

    // ---------------------------------------------------------                  
    // HasSuperfamilyAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoHasSuperfamilyAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl42Superfamilies
            .AnyAsync(x => x.InfraordoId == infraordoId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoHasReferenceAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfraordoId == infraordoId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoHasCommentAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfraordoId == infraordoId);
    }

    // ---------------------------------------------------------                  
    // GetInfraordoForSubordoAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl39Infraordo>> GetInfraordoForSubordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .Where(e => e.SubordoId == subordoId)
            .Include(e => e.Subordo)
            .OrderBy(e => e.InfraordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSuperfamiliesByInfraordoIdAsync(int infraordoId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl42Superfamily>> GetSuperfamiliesByInfraordoIdAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .Where(x => x.InfraordoId == infraordoId)
            .OrderBy(x => x.SuperfamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByInfraordoIdAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfraordoIdAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.InfraordoId == infraordoId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByInfraordoIdAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfraordoIdAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.InfraordoId == infraordoId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByInfraordoIdAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfraordoIdAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.InfraordoId == infraordoId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByInfraordoIdAsync(int infraordoId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByInfraordoIdAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.InfraordoId == infraordoId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Infraordo

    #region Superfamily

    // ---------------------------------------------------------                  
    // GetAllSuperfamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl42Superfamily>> GetAllSuperfamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .AsNoTracking()
            .OrderBy(e => e.SuperfamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSuperfamilyWithInfraordoAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl42Superfamily>> SearchSuperfamilyWithInfraordoAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl42Superfamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl42Superfamilies
            .Include(x => x.Infraordo)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SuperfamilyId == id || e.SuperfamilyName.StartsWith(search))
                .OrderBy(e => e.SuperfamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SuperfamilyName.StartsWith(search))
            .OrderBy(e => e.SuperfamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSuperfamilyAsync(Tbl42Superfamily uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl42Superfamily> UpsertSuperfamilyAsync(Tbl42Superfamily uiEntity)
    {
        return UpsertAsync<Tbl42Superfamily, int>(
            uiEntity,
            ctx => ctx.Set<Tbl42Superfamily>(),
            x => x.SuperfamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SuperfamilyId == id),
            MapSuperfamilyFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSuperfamilyFields(Tbl42Superfamily source, Tbl42Superfamily target)
    // ---------------------------------------------------------                 
    private static void MapSuperfamilyFields(Tbl42Superfamily source, Tbl42Superfamily target)
    {
        target.SuperfamilyName = source.SuperfamilyName;
        target.InfraordoId = source.InfraordoId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSuperfamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl42Superfamily> GetSuperfamilyByIdAsync(int? id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SuperfamilyId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSuperfamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSuperfamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl42Superfamilies.FindAsync(id);
        if (entity != null)
        {
            context.Tbl42Superfamilies.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl42Superfamily entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyExistsAsync(Tbl42Superfamily entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies.AnyAsync(x =>
            x.SuperfamilyName == entity.SuperfamilyName &&
            x.InfraordoId == entity.InfraordoId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SuperfamilyId != entity.SuperfamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasFamilyAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyHasFamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl45Families
            .AnyAsync(x => x.SuperfamilyId == superfamilyId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyHasReferenceAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SuperfamilyId == superfamilyId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyHasCommentAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SuperfamilyId == superfamilyId);
    }

    // ---------------------------------------------------------                  
    // GetSuperfamilyForInfraordoAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl42Superfamily>> GetSuperfamilyForInfraordoAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .Where(e => e.InfraordoId == infraordoId)
            .Include(e => e.Infraordo)
            .OrderBy(e => e.SuperfamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetFamiliesBySuperfamilyIdAsync(int superfamilyId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl45Family>> GetFamiliesBySuperfamilyIdAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .Where(x => x.SuperfamilyId == superfamilyId)
            .OrderBy(x => x.FamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySuperfamilyIdAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySuperfamilyIdAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SuperfamilyId == superfamilyId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySuperfamilyIdAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySuperfamilyIdAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SuperfamilyId == superfamilyId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySuperfamilyIdAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySuperfamilyIdAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SuperfamilyId == superfamilyId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySuperfamilyIdAsync(int superfamilyId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySuperfamilyIdAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SuperfamilyId == superfamilyId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Superfamily

    #region Family

    // ---------------------------------------------------------                  
    // GetAllFamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl45Family>> GetAllFamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .AsNoTracking()
            .OrderBy(e => e.FamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchFamilyWithSuperfamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl45Family>> SearchFamilyWithSuperfamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl45Family>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl45Families
            .Include(x => x.Superfamily)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.FamilyId == id || e.FamilyName.StartsWith(search))
                .OrderBy(e => e.FamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.FamilyName.StartsWith(search))
            .OrderBy(e => e.FamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertFamilyAsync(Tbl45Family uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl45Family> UpsertFamilyAsync(Tbl45Family uiEntity)
    {
        return UpsertAsync<Tbl45Family, int>(
            uiEntity,
            ctx => ctx.Set<Tbl45Family>(),
            x => x.FamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.FamilyId == id),
            MapFamilyFields
        );
    }

    // ---------------------------------------------------------                  
    // MapFamilyFields(Tbl45Family source, Tbl45Family target)
    // ---------------------------------------------------------                 
    private static void MapFamilyFields(Tbl45Family source, Tbl45Family target)
    {
        target.FamilyName = source.FamilyName;
        target.SuperfamilyId = source.SuperfamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetFamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl45Family> GetFamilyByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FamilyId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteFamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteFamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl45Families.FindAsync(id);
        if (entity != null)
        {
            context.Tbl45Families.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl45Family entity)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyExistsAsync(Tbl45Family entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families.AnyAsync(x =>
            x.FamilyName == entity.FamilyName &&
            x.SuperfamilyId == entity.SuperfamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.FamilyId != entity.FamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubfamilyAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyHasSubfamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl48Subfamilies
            .AnyAsync(x => x.FamilyId == familyId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyHasReferenceAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.FamilyId == familyId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyHasCommentAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.FamilyId == familyId);
    }

    // ---------------------------------------------------------                  
    // GetFamilyForSuperfamilyAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl45Family>> GetFamilyForSuperfamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .Where(e => e.SuperfamilyId == superfamilyId)
            .Include(e => e.Superfamily)
            .OrderBy(e => e.FamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubfamiliesByFamilyIdAsync(int familyId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl48Subfamily>> GetSubfamiliesByFamilyIdAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .Where(x => x.FamilyId == familyId)
            .OrderBy(x => x.SubfamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByFamilyIdAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByFamilyIdAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.FamilyId == familyId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByFamilyIdAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByFamilyIdAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.FamilyId == familyId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByFamilyIdAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByFamilyIdAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.FamilyId == familyId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByFamilyIdAsync(int familyId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByFamilyIdAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.FamilyId == familyId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Family

    #region Subfamily

    // ---------------------------------------------------------                  
    // GetAllSubfamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl48Subfamily>> GetAllSubfamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .AsNoTracking()
            .OrderBy(e => e.SubfamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSubfamilyWithFamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl48Subfamily>> SearchSubfamilyWithFamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl48Subfamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl48Subfamilies
            .Include(x => x.Family)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubfamilyId == id || e.SubfamilyName.StartsWith(search))
                .OrderBy(e => e.SubfamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubfamilyName.StartsWith(search))
            .OrderBy(e => e.SubfamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSubfamilyAsync(Tbl48Subfamily uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl48Subfamily> UpsertSubfamilyAsync(Tbl48Subfamily uiEntity)
    {
        return UpsertAsync<Tbl48Subfamily, int>(
            uiEntity,
            ctx => ctx.Set<Tbl48Subfamily>(),
            x => x.SubfamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubfamilyId == id),
            MapSubfamilyFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSubfamilyFields(Tbl48Subfamily source, Tbl48Subfamily target)
    // ---------------------------------------------------------                 
    private static void MapSubfamilyFields(Tbl48Subfamily source, Tbl48Subfamily target)
    {
        target.SubfamilyName = source.SubfamilyName;
        target.FamilyId = source.FamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSubfamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl48Subfamily> GetSubfamilyByIdAsync(int? id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubfamilyId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSubfamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubfamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl48Subfamilies.FindAsync(id);
        if (entity != null)
        {
            context.Tbl48Subfamilies.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl48Subfamily entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyExistsAsync(Tbl48Subfamily entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies.AnyAsync(x =>
            x.SubfamilyName == entity.SubfamilyName &&
            x.FamilyId == entity.FamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubfamilyId != entity.SubfamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfrafamilyAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyHasInfrafamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl51Infrafamilies
            .AnyAsync(x => x.SubfamilyId == subfamilyId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyHasReferenceAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubfamilyId == subfamilyId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyHasCommentAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubfamilyId == subfamilyId);
    }

    // ---------------------------------------------------------                  
    // GetSubfamilyForFamilyAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl48Subfamily>> GetSubfamilyForFamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .Where(e => e.FamilyId == familyId)
            .Include(e => e.Family)
            .OrderBy(e => e.SubfamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetInfrafamiliesBySubfamilyIdAsync(int subfamilyId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl51Infrafamily>> GetInfrafamiliesBySubfamilyIdAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .Where(x => x.SubfamilyId == subfamilyId)
            .OrderBy(x => x.InfrafamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubfamilyIdAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubfamilyIdAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubfamilyId == subfamilyId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubfamilyIdAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubfamilyIdAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubfamilyId == subfamilyId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubfamilyIdAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubfamilyIdAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubfamilyId == subfamilyId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubfamilyIdAsync(int subfamilyId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubfamilyIdAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubfamilyId == subfamilyId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subfamily

    #region Infrafamily

    // ---------------------------------------------------------                  
    // GetAllInfrafamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl51Infrafamily>> GetAllInfrafamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .AsNoTracking()
            .OrderBy(e => e.InfrafamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchInfrafamilyWithSubfamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl51Infrafamily>> SearchInfrafamilyWithSubfamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl51Infrafamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl51Infrafamilies
            .Include(x => x.Subfamily)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfrafamilyId == id || e.InfrafamilyName.StartsWith(search))
                .OrderBy(e => e.InfrafamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfrafamilyName.StartsWith(search))
            .OrderBy(e => e.InfrafamilyName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertInfrafamilyAsync(Tbl51Infrafamily uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl51Infrafamily> UpsertInfrafamilyAsync(Tbl51Infrafamily uiEntity)
    {
        return UpsertAsync<Tbl51Infrafamily, int>(
            uiEntity,
            ctx => ctx.Set<Tbl51Infrafamily>(),
            x => x.InfrafamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfrafamilyId == id),
            MapInfrafamilyFields
        );
    }

    // ---------------------------------------------------------                  
    // MapInfrafamilyFields(Tbl51Infrafamily source, Tbl51Infrafamily target)
    // ---------------------------------------------------------                 
    private static void MapInfrafamilyFields(Tbl51Infrafamily source, Tbl51Infrafamily target)
    {
        target.InfrafamilyName = source.InfrafamilyName;
        target.SubfamilyId = source.SubfamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetInfrafamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl51Infrafamily> GetInfrafamilyByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfrafamilyId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteInfrafamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfrafamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl51Infrafamilies.FindAsync(id);
        if (entity != null)
        {
            context.Tbl51Infrafamilies.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl51Infrafamily entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyExistsAsync(Tbl51Infrafamily entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies.AnyAsync(x =>
            x.InfrafamilyName == entity.InfrafamilyName &&
            x.SubfamilyId == entity.SubfamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfrafamilyId != entity.InfrafamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasSupertribusAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyHasSupertribusAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl54Supertribusses
            .AnyAsync(x => x.InfrafamilyId == infrafamilyId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyHasReferenceAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfrafamilyId == infrafamilyId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyHasCommentAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfrafamilyId == infrafamilyId);
    }

    // ---------------------------------------------------------                  
    // GetInfrafamilyForSubfamilyAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl51Infrafamily>> GetInfrafamilyForSubfamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .Where(e => e.SubfamilyId == subfamilyId)
            .Include(e => e.Subfamily)
            .OrderBy(e => e.InfrafamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSupertribussesByInfrafamilyIdAsync(int infrafamilyId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl54Supertribus>> GetSupertribussesByInfrafamilyIdAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .Where(x => x.InfrafamilyId == infrafamilyId)
            .OrderBy(x => x.SupertribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByInfrafamilyIdAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfrafamilyIdAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.InfrafamilyId == infrafamilyId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByInfrafamilyIdAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfrafamilyIdAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.InfrafamilyId == infrafamilyId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByInfrafamilyIdAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfrafamilyIdAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.InfrafamilyId == infrafamilyId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByInfrafamilyIdAsync(int infrafamilyId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByInfrafamilyIdAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.InfrafamilyId == infrafamilyId)
            .AsNoTracking()
            .ToListAsync();
    }


    #endregion Infrafamily

    #region Supertribus

    // ---------------------------------------------------------                  
    // GetAllSupertribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl54Supertribus>> GetAllSupertribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .AsNoTracking()
            .OrderBy(e => e.SupertribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSupertribusWithInfrafamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl54Supertribus>> SearchSupertribusWithInfrafamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl54Supertribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl54Supertribusses
            .Include(x => x.Infrafamily)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SupertribusId == id || e.SupertribusName.StartsWith(search))
                .OrderBy(e => e.SupertribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SupertribusName.StartsWith(search))
            .OrderBy(e => e.SupertribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSupertribusAsync(Tbl54Supertribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl54Supertribus> UpsertSupertribusAsync(Tbl54Supertribus uiEntity)
    {
        return UpsertAsync<Tbl54Supertribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl54Supertribus>(),
            x => x.SupertribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SupertribusId == id),
            MapSupertribusFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSupertribusFields(Tbl54Supertribus source, Tbl54Supertribus target)
    // ---------------------------------------------------------                 
    private static void MapSupertribusFields(Tbl54Supertribus source, Tbl54Supertribus target)
    {
        target.SupertribusName = source.SupertribusName;
        target.InfrafamilyId = source.InfrafamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSupertribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl54Supertribus> GetSupertribusByIdAsync(int? id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SupertribusId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSupertribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSupertribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl54Supertribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl54Supertribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl54Supertribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusExistsAsync(Tbl54Supertribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses.AnyAsync(x =>
            x.SupertribusName == entity.SupertribusName &&
            x.InfrafamilyId == entity.InfrafamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SupertribusId != entity.SupertribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasTribusAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusHasTribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl57Tribusses
            .AnyAsync(x => x.SupertribusId == supertribusId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusHasReferenceAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SupertribusId == supertribusId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusHasCommentAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SupertribusId == supertribusId);
    }

    // ---------------------------------------------------------                  
    // GetSupertribusForInfrafamilyAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl54Supertribus>> GetSupertribusForInfrafamilyAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .Where(e => e.InfrafamilyId == infrafamilyId)
            .Include(e => e.Infrafamily)
            .OrderBy(e => e.SupertribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetTribussesBySupertribusIdAsync(int supertribusId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl57Tribus>> GetTribussesBySupertribusIdAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .Where(x => x.SupertribusId == supertribusId)
            .OrderBy(x => x.TribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySupertribusIdAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySupertribusIdAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SupertribusId == supertribusId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySupertribusIdAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySupertribusIdAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SupertribusId == supertribusId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySupertribusIdAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySupertribusIdAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SupertribusId == supertribusId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySupertribusIdAsync(int supertribusId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySupertribusIdAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SupertribusId == supertribusId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Supertribus

    #region Tribus

    // ---------------------------------------------------------                  
    // GetAllTribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl57Tribus>> GetAllTribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .AsNoTracking()
            .OrderBy(e => e.TribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchTribusWithSupertribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl57Tribus>> SearchTribusWithSupertribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl57Tribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl57Tribusses
            .Include(x => x.Supertribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.TribusId == id || e.TribusName.StartsWith(search))
                .OrderBy(e => e.TribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.TribusName.StartsWith(search))
            .OrderBy(e => e.TribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertTribusAsync(Tbl57Tribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl57Tribus> UpsertTribusAsync(Tbl57Tribus uiEntity)
    {
        return UpsertAsync<Tbl57Tribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl57Tribus>(),
            x => x.TribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.TribusId == id),
            MapTribusFields
        );
    }

    // ---------------------------------------------------------                  
    // MapTribusFields(Tbl57Tribus source, Tbl57Tribus target)
    // ---------------------------------------------------------                 
    private static void MapTribusFields(Tbl57Tribus source, Tbl57Tribus target)
    {
        target.TribusName = source.TribusName;
        target.SupertribusId = source.SupertribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetTribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl57Tribus> GetTribusByIdAsync(int? id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TribusId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteTribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteTribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl57Tribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl57Tribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl57Tribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusExistsAsync(Tbl57Tribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses.AnyAsync(x =>
            x.TribusName == entity.TribusName &&
            x.SupertribusId == entity.SupertribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.TribusId != entity.TribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubtribusAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusHasSubtribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl60Subtribusses
            .AnyAsync(x => x.TribusId == tribusId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusHasReferenceAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.TribusId == tribusId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusHasCommentAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.TribusId == tribusId);
    }

    // ---------------------------------------------------------                  
    // GetTribusForSupertribusAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl57Tribus>> GetTribusForSupertribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .Where(e => e.SupertribusId == supertribusId)
            .Include(e => e.Supertribus)
            .OrderBy(e => e.TribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubtribussesByTribusIdAsync(int tribusId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl60Subtribus>> GetSubtribussesByTribusIdAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .Where(x => x.TribusId == tribusId)
            .OrderBy(x => x.SubtribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByTribusIdAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByTribusIdAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.TribusId == tribusId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByTribusIdAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByTribusIdAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.TribusId == tribusId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByTribusIdAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByTribusIdAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.TribusId == tribusId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByTribusIdAsync(int tribusId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByTribusIdAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.TribusId == tribusId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Tribus

    #region Subtribus

    // ---------------------------------------------------------                  
    // GetAllSubtribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl60Subtribus>> GetAllSubtribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .AsNoTracking()
            .OrderBy(e => e.SubtribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSubtribusWithTribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusWithTribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl60Subtribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl60Subtribusses
            .Include(x => x.Tribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubtribusId == id || e.SubtribusName.StartsWith(search))
                .OrderBy(e => e.SubtribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubtribusName.StartsWith(search))
            .OrderBy(e => e.SubtribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSubtribusAsync(Tbl60Subtribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl60Subtribus> UpsertSubtribusAsync(Tbl60Subtribus uiEntity)
    {
        return UpsertAsync<Tbl60Subtribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl60Subtribus>(),
            x => x.SubtribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubtribusId == id),
            MapSubtribusFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSubtribusFields(Tbl60Subtribus source, Tbl60Subtribus target)
    // ---------------------------------------------------------                 
    private static void MapSubtribusFields(Tbl60Subtribus source, Tbl60Subtribus target)
    {
        target.SubtribusName = source.SubtribusName;
        target.TribusId = source.TribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSubtribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl60Subtribus> GetSubtribusByIdAsync(int? id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubtribusId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSubtribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubtribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl60Subtribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl60Subtribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl60Subtribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusExistsAsync(Tbl60Subtribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses.AnyAsync(x =>
            x.SubtribusName == entity.SubtribusName &&
            x.TribusId == entity.TribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubtribusId != entity.SubtribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfratribusAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusHasInfratribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl63Infratribusses
            .AnyAsync(x => x.SubtribusId == subtribusId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusHasReferenceAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubtribusId == subtribusId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusHasCommentAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubtribusId == subtribusId);
    }

    // ---------------------------------------------------------                  
    // GetSubtribusForTribusAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl60Subtribus>> GetSubtribusForTribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .Where(e => e.TribusId == tribusId)
            .Include(e => e.Tribus)
            .OrderBy(e => e.SubtribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetInfratribussesBySubtribusIdAsync(int subtribusId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl63Infratribus>> GetInfratribussesBySubtribusIdAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .Where(x => x.SubtribusId == subtribusId)
            .OrderBy(x => x.InfratribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubtribusIdAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubtribusIdAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubtribusId == subtribusId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubtribusIdAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubtribusIdAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubtribusId == subtribusId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubtribusIdAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubtribusIdAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubtribusId == subtribusId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubtribusIdAsync(int subtribusId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubtribusIdAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubtribusId == subtribusId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subtribus

    #region Infratribus

    // ---------------------------------------------------------                  
    // GetAllInfratribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl63Infratribus>> GetAllInfratribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .AsNoTracking()
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchInfratribusWithSubtribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusWithSubtribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl63Infratribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl63Infratribusses
            .Include(x => x.Subtribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfratribusId == id || e.InfratribusName.StartsWith(search))
                .OrderBy(e => e.InfratribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfratribusName.StartsWith(search))
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertInfratribusAsync(Tbl63Infratribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus uiEntity)
    {
        return UpsertAsync<Tbl63Infratribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl63Infratribus>(),
            x => x.InfratribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfratribusId == id),
            MapInfratribusFields
        );
    }

    // ---------------------------------------------------------                  
    // MapInfratribusFields(Tbl63Infratribus source, Tbl63Infratribus target)
    // ---------------------------------------------------------                 
    private static void MapInfratribusFields(Tbl63Infratribus source, Tbl63Infratribus target)
    {
        target.InfratribusName = source.InfratribusName;
        target.SubtribusId = source.SubtribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetInfratribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl63Infratribus> GetInfratribusByIdAsync(int? id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfratribusId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteInfratribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfratribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl63Infratribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl63Infratribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl63Infratribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusExistsAsync(Tbl63Infratribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses.AnyAsync(x =>
            x.InfratribusName == entity.InfratribusName &&
            x.SubtribusId == entity.SubtribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfratribusId != entity.InfratribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasGenusAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusHasGenusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl66Genusses
            .AnyAsync(x => x.InfratribusId == infratribusId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusHasReferenceAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfratribusId == infratribusId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusHasCommentAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfratribusId == infratribusId);
    }


    // ---------------------------------------------------------                  
    // GetInfratribusForSubtribusAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl63Infratribus>> GetInfratribusForSubtribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .Where(e => e.SubtribusId == subtribusId)
            .Include(e => e.Subtribus)
            .OrderBy(e => e.InfratribusName)
            .AsNoTracking()
            .ToListAsync();
    }


    // ---------------------------------------------------------                  
    // GetGenussesByInfratribusIdAsync(int infratribusId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl66Genus>> GetGenussesByInfratribusIdAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .Where(x => x.InfratribusId == infratribusId)
            .OrderBy(x => x.GenusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByInfratribusIdAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfratribusIdAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.InfratribusId == infratribusId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByInfratribusIdAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfratribusIdAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.InfratribusId == infratribusId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByInfratribusIdAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfratribusIdAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.InfratribusId == infratribusId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByInfratribusIdAsync(int infratribusId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByInfratribusIdAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.InfratribusId == infratribusId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Infratribus

    #region Genus

    // ---------------------------------------------------------                  
    // GetAllGenusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl66Genus>> GetAllGenusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .AsNoTracking()
            .OrderBy(e => e.GenusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchGenusWithInfratribusAsync(string? search)
    // ---------------------------------------------------------

    public async Task<IEnumerable<Tbl66Genus>> SearchGenusWithInfratribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl66Genus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl66Genusses
            .Include(x => x.Infratribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.GenusId == id || e.GenusName.StartsWith(search))
                .OrderBy(e => e.GenusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.GenusName.StartsWith(search))
            .OrderBy(e => e.GenusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertGenusAsync(Tbl66Genus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus uiEntity)
    {
        return UpsertAsync<Tbl66Genus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl66Genus>(),
            x => x.GenusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.GenusId == id),
            MapGenusFields
        );
    }

    // ---------------------------------------------------------                  
    // MapGenusFields(Tbl66Genus source, Tbl66Genus target)
    // ---------------------------------------------------------                 
    private static void MapGenusFields(Tbl66Genus source, Tbl66Genus target)
    {
        target.GenusName = source.GenusName;
        target.InfratribusId = source.InfratribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetGenusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl66Genus> GetGenusByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GenusId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteGenusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteGenusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl66Genusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl66Genusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl66Genus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusExistsAsync(Tbl66Genus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses.AnyAsync(x =>
            x.GenusName == entity.GenusName &&
            x.InfratribusId == entity.InfratribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.GenusId != entity.GenusId
        );
    }

    // ---------------------------------------------------------                  
    // HasSpeciesAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusHasSpeciesAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl75Speciesses
            .AnyAsync(x => x.GenusId == genusId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusHasReferenceAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.GenusId == genusId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusHasCommentAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.GenusId == genusId);
    }

    // ---------------------------------------------------------                  
    // GetGenusForInfratribusAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl66Genus>> GetGenusForInfratribusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .Where(e => e.InfratribusId == infratribusId)
            .Include(e => e.Infratribus)
            .OrderBy(e => e.GenusName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSpeciessesByGenusIdAsync(int genusId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl75Species>> GetSpeciessesByGenusIdAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .Where(x => x.GenusId == genusId)
            .OrderBy(x => x.SpeciesName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByGenusIdAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByGenusIdAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.GenusId == genusId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByGenusIdAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByGenusIdAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.GenusId == genusId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByGenusIdAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByGenusIdAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.GenusId == genusId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByGenusIdAsync(int genusId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByGenusIdAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.GenusId == genusId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Genus

    #region Speciesgroup

    // ---------------------------------------------------------                  
    // GetAllSpeciesgroupAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl68Speciesgroup>> GetAllSpeciesgroupAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl68Speciesgroups
            .AsNoTracking()
            .OrderBy(e => e.SpeciesgroupName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSpeciesgroupWithGenusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl68Speciesgroup>> SearchSpeciesgroupAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl68Speciesgroup>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl68Speciesgroups
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SpeciesgroupId == id || e.SpeciesgroupName.StartsWith(search))
                .OrderBy(e => e.SpeciesgroupName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SpeciesgroupName.StartsWith(search))
            .OrderBy(e => e.SpeciesgroupName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSpeciesgroupAsync(Tbl68Speciesgroup uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl68Speciesgroup> UpsertSpeciesgroupAsync(Tbl68Speciesgroup uiEntity)
    {
        return UpsertAsync<Tbl68Speciesgroup, int>(
            uiEntity,
            ctx => ctx.Set<Tbl68Speciesgroup>(),
            x => x.SpeciesgroupId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SpeciesgroupId == id),
            MapSpeciesgroupFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSpeciesgroupFields(Tbl68Speciesgroup source, Tbl68Speciesgroup target)
    // ---------------------------------------------------------                 
    private static void MapSpeciesgroupFields(Tbl68Speciesgroup source, Tbl68Speciesgroup target)
    {
        target.SpeciesgroupName = source.SpeciesgroupName;
        target.Subspeciesgroup = source.Subspeciesgroup;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSpeciesgroupByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl68Speciesgroup?> GetSpeciesgroupByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl68Speciesgroups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SpeciesgroupId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSpeciesgroupAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSpeciesgroupAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl68Speciesgroups.FindAsync(id);
        if (entity != null)
        {
            context.Tbl68Speciesgroups.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl68Speciesgroup entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesgroupExistsAsync(Tbl68Speciesgroup entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl68Speciesgroups.AnyAsync(x =>
            x.SpeciesgroupName == entity.SpeciesgroupName &&
            x.Subspeciesgroup == entity.Subspeciesgroup &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SpeciesgroupId != entity.SpeciesgroupId
        );
    }

    // ---------------------------------------------------------                  
    // HasSpeciesAsync(int speciesgroupId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesgroupHasSpeciesAsync(int speciesgroupId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl75Speciesses
            .AnyAsync(x => x.SpeciesgroupId == speciesgroupId);
    }

    #endregion Speciesgroup

    #region Species

    // ---------------------------------------------------------                  
    // GetAllSpeciesAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl75Species>> GetAllSpeciesAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .AsNoTracking()
            .OrderBy(e => e.SpeciesName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSpeciesWithGenusAndSpeciesgroupByIdAsync(string? search)
    // ---------------------------------------------------------

    public async Task<IEnumerable<Tbl75Species>> GetSpeciesWithGenusAndSpeciesgroupByIdAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl75Species>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl75Speciesses
            .Include(x => x.Genus)
            .Include(x => x.Speciesgroup)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SpeciesId == id || e.SpeciesName.StartsWith(search))
                .OrderBy(e => e.SpeciesName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SpeciesName.StartsWith(search))
            .OrderBy(e => e.SpeciesName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSpeciesAsync(Tbl75Species uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl75Species> UpsertSpeciesAsync(Tbl75Species uiEntity)
    {
        return UpsertAsync<Tbl75Species, int>(
            uiEntity,
            ctx => ctx.Set<Tbl75Species>(),
            x => x.SpeciesId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SpeciesId == id),
            MapSpeciesFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSpeciesFields(Tbl75Species source, Tbl75Species target)
    // ---------------------------------------------------------                 
    private static void MapSpeciesFields(Tbl75Species source, Tbl75Species target)
    {
        target.SpeciesName = source.SpeciesName;
        target.Subspecies = source.Subspecies;
        target.Divers = source.Divers;
        target.GenusId = source.GenusId;
        target.SpeciesgroupId = source.SpeciesgroupId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.MemoSpecies = source.MemoSpecies;
        target.TradeName = source.TradeName;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Importer = source.Importer;
        target.ImportingYear = source.ImportingYear;
        target.TypeSpecies = source.TypeSpecies;
        target.LNumber = source.LNumber;
        target.LOrigin = source.LOrigin;
        target.LDANumber = source.LDANumber;
        target.LDAOrigin = source.LDAOrigin;
        target.BasinLength = source.BasinLength;
        target.FishLength = source.FishLength;
        target.Karnivore = source.Karnivore;
        target.Herbivore = source.Herbivore;
        target.Limnivore = source.Limnivore;
        target.Omnivore = source.Omnivore;
        target.MemoFoods = source.MemoFoods;
        target.Difficult1 = source.Difficult1;
        target.Difficult2 = source.Difficult2;
        target.Difficult3 = source.Difficult3;
        target.Difficult4 = source.Difficult4;
        target.RegionTop = source.RegionTop;
        target.RegionMiddle = source.RegionMiddle;
        target.RegionBottom = source.RegionBottom;
        target.MemoRegion = source.MemoRegion;
        target.MemoTech = source.MemoTech;
        target.Ph1 = source.Ph1;
        target.Ph2 = source.Ph2;
        target.Temp1 = source.Temp1;
        target.Temp2 = source.Temp2;
        target.Hardness1 = source.Hardness1;
        target.Hardness2 = source.Hardness2;
        target.CarboHardness1 = source.CarboHardness1;
        target.CarboHardness2 = source.CarboHardness2;
        target.MemoHusbandry = source.MemoHusbandry;
        target.MemoBreeding = source.MemoBreeding;
        target.MemoBuilt = source.MemoBuilt;
        target.MemoColor = source.MemoColor;
        target.MemoSozial = source.MemoSozial;
        target.MemoDomorphism = source.MemoDomorphism;
        target.MemoSpecial = source.MemoSpecial;
        target.BasinHeight = source.BasinHeight;
        target.PlantLength = source.PlantLength;
        target.MemoReproduction = source.MemoReproduction;
        target.MemoCulture = source.MemoCulture;
        target.MemoGlobal = source.MemoGlobal;
        target.Creature = source.Creature;
    }

    // ---------------------------------------------------------                  
    // GetSpeciesByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl75Species> GetSpeciesByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SpeciesId == id);
    }
    // ---------------------------------------------------------                  
    // GetSpeciesWithGenusAndSpeciesgroupByIdAsync(int id)
    // ---------------------------------------------------------                 

    public async Task<Tbl75Species> GetSpeciesWithGenusAndSpeciesgroupByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .AsNoTracking()
            .Include(x => x.Genus)
            .Include(x => x.Speciesgroup)
            .FirstOrDefaultAsync(x => x.SpeciesId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSpeciesAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSpeciesAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl75Speciesses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl75Speciesses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl75Species entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesExistsAsync(Tbl75Species entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses.AnyAsync(x =>
            x.SpeciesName == entity.SpeciesName &&
            x.GenusId == entity.GenusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.SpeciesId != entity.SpeciesId
        );
    }

    // ---------------------------------------------------------                  
    // HasNameAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasNameAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl78Names
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // HasImageAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasImageAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl81Images
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // HasSynonymAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasSynonymAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl84Synonyms
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // HasGeographicAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasGeographicAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl87Geographics
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // HasReferenceAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasReferenceAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // HasCommentAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasCommentAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // GetSpeciesForGenusAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl75Species>> GetSpeciesForGenusAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .Where(e => e.GenusId == genusId)
            .Include(e => e.Genus)
            .OrderBy(e => e.SpeciesName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSpeciesForSpeciesgroupAsync(int speciesgroupId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl75Species>> GetSpeciesForSpeciesgroupAsync(int speciesgroupId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .Where(e => e.SpeciesgroupId == speciesgroupId)
            .Include(e => e.Speciesgroup)
            .OrderBy(e => e.SpeciesName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetNamesBySpeciesIdAsync(int speciesId) FürReportPDF
    // ---------------------------------------------------------                 

    public async Task<IEnumerable<Tbl78Name>> GetNamesBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl78Names
            .Where(x => x.SpeciesId == speciesId)
            .OrderBy(x => x.NameName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl81Image>> GetImagesBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .Where(x => x.SpeciesId == speciesId)
            .OrderBy(x => x.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl84Synonym>> GetSynonymsBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl84Synonyms
            .Where(x => x.SpeciesId == speciesId)
            .OrderBy(x => x.SynonymName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl87Geographic>> GetGeographicsBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl87Geographics
            .Where(x => x.SpeciesId == speciesId)
            .OrderBy(x => x.Country)
            .ThenBy(x => x.Address)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySpeciesIdAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SpeciesId == speciesId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySpeciesIdAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SpeciesId == speciesId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySpeciesIdAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SpeciesId == speciesId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySpeciesIdAsync(int speciesId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySpeciesIdAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SpeciesId == speciesId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Species

    #region Name

    // ---------------------------------------------------------                  
    // GetAllNameAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl78Name>> GetAllNameAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl78Names
            .AsNoTracking()
            .OrderBy(e => e.NameName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchNameWithSpeciesAndGenusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl78Name>> SearchNameWithSpeciesAndGenusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl78Name>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl78Names
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.NameId == id || e.NameName.StartsWith(search))
                .OrderBy(e => e.NameName)
                .ToListAsync();
        }

        var result = await query
            .Where(e => e.NameName.StartsWith(search))
            .OrderBy(e => e.NameName)
            .ToListAsync();

        return result;
    }

    // ---------------------------------------------------------                  
    // UpsertNameAsync(Tbl78Name uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl78Name> UpsertNameAsync(Tbl78Name uiEntity)
    {
        return UpsertAsync<Tbl78Name, int>(
            uiEntity,
            ctx => ctx.Set<Tbl78Name>(),
            x => x.NameId,
            (set, id) => set.FirstOrDefaultAsync(x => x.NameId == id),
            MapNameFields
        );
    }

    // ---------------------------------------------------------                  
    // MapNameFields(Tbl78Name source, Tbl78Name target)
    // ---------------------------------------------------------                 
    private static void MapNameFields(Tbl78Name source, Tbl78Name target)
    {
        target.NameName = source.NameName;
        target.SpeciesId = source.SpeciesId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Language = source.Language;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetNameByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl78Name?> GetNameByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl78Names
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NameId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteNameAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteNameAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl78Names.FindAsync(id);
        if (entity != null)
        {
            context.Tbl78Names.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl78Name entity)
    // ---------------------------------------------------------                 
    public async Task<bool> NameExistsAsync(Tbl78Name entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl78Names.AnyAsync(x =>
            x.NameName == entity.NameName &&
            x.SpeciesId == entity.SpeciesId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Language == entity.Language &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.NameId != entity.NameId
        );
    }

    // ---------------------------------------------------------                  
    // GetNameForSpeciesAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl78Name>> GetNameForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl78Names
            .Where(e => e.SpeciesId == speciesId)
            .Include(e => e.Species)
            .ThenInclude(s => s.Genus)
            .OrderBy(e => e.NameName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Name

    #region Image
    // ---------------------------------------------------------                  
    // GetAllImageAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl81Image>> GetAllImageAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .AsNoTracking()
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchImageWithSpeciesAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl81Image>> SearchImageWithSpeciesAndGenusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl81Image>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl81Images
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.ImageId == id || e.Info.StartsWith(search))
                .OrderBy(e => e.Info)
                .ToListAsync();
        }

        return await query
            .Where(e => e.Info.StartsWith(search))
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertImageAsync(Tbl81Image uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl81Image> UpsertImageAsync(Tbl81Image uiEntity)
    {
        return UpsertAsync<Tbl81Image, int>(
            uiEntity,
            ctx => ctx.Set<Tbl81Image>(),
            x => x.ImageId,
            (set, id) => set.FirstOrDefaultAsync(x => x.ImageId == id),
            MapImageFields
        );
    }

    // ---------------------------------------------------------                  
    // MapImageFields(Tbl81Image source, Tbl81Image target)
    // ---------------------------------------------------------
    private static void MapImageFields(Tbl81Image source, Tbl81Image target)
    {
        target.SpeciesId = source.SpeciesId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.FilestreamId = source.FilestreamId == Guid.Empty ? Guid.NewGuid() : source.FilestreamId;
        target.Filestream = source.Filestream;
        target.ShotDate = source.ShotDate;
        target.ImageMimeType = source.ImageMimeType;
        target.Info = source.Info;
        target.Memo = source.Memo;
        target.FoodId = source.FoodId;
        target.DiseaseId = source.DiseaseId;
        target.MiscellaneousId = source.MiscellaneousId;
    }

    // ---------------------------------------------------------                  
    // GetImageByIdAsync(int id)
    // ---------------------------------------------------------

    public async Task<Tbl81Image?> GetImageByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ImageId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteImageAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteImageAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl81Images.FindAsync(id);
        if (entity != null)
        {
            context.Tbl81Images.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl81Image entity)
    // ---------------------------------------------------------
    public async Task<bool> ImageExistsAsync(Tbl81Image entity)
    {
        await using var context = _factory.CreateDbContext();

        var candidates = await context.Tbl81Images
            .Where(x =>
                x.SpeciesId == entity.SpeciesId &&
                x.ImageMimeType == entity.ImageMimeType &&
                x.ImageId != entity.ImageId)
            .ToListAsync();

        return candidates.Any(x =>
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.ShotDate == entity.ShotDate &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.FoodId == entity.FoodId &&
            x.DiseaseId == entity.DiseaseId &&
            x.MiscellaneousId == entity.MiscellaneousId &&
            ((x.Filestream == null && entity.Filestream == null) ||
             (x.Filestream != null && entity.Filestream != null &&
              x.Filestream.SequenceEqual(entity.Filestream)))
        );
    }

    public async Task<IEnumerable<Tbl81Image>> GetImageForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .Where(e => e.SpeciesId == speciesId)
            .Include(e => e.Species)
            .ThenInclude(s => s.Genus)
            .OrderBy(e => e.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetImageForFoodAsync(int foodId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl81Image>> GetImageForFoodAsync(int foodId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .Where(e => e.FoodId == foodId)
            .OrderBy(e => e.FoodId)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetImageForDiseaseAsync(int diseaseId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl81Image>> GetImageForDiseaseAsync(int diseaseId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .Where(e => e.DiseaseId == diseaseId)
            .OrderBy(e => e.DiseaseId)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetImageForMiscellaneousAsync(int miscellaneousId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl81Image>> GetImageForMiscellaneousAsync(int miscellaneousId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl81Images
            .Where(e => e.MiscellaneousId == miscellaneousId)
            .OrderBy(e => e.MiscellaneousId)
            .AsNoTracking()
            .ToListAsync();
    }


    #endregion Image

    #region Synonym

    // ---------------------------------------------------------                  
    // GetAllSynonymAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl84Synonym>> GetAllSynonymAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl84Synonyms
            .AsNoTracking()
            .OrderBy(e => e.SynonymName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchSynonymWithSpeciesAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl84Synonym>> SearchSynonymWithSpeciesAndGenusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl84Synonym>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl84Synonyms
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SynonymId == id || e.SynonymName.StartsWith(search))
                .OrderBy(e => e.SynonymName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SynonymName.StartsWith(search))
            .OrderBy(e => e.SynonymName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertSynonymAsync(Tbl84Synonym uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym uiEntity)
    {
        return UpsertAsync<Tbl84Synonym, int>(
            uiEntity,
            ctx => ctx.Set<Tbl84Synonym>(),
            x => x.SynonymId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SynonymId == id),
            MapSynonymFields
        );
    }

    // ---------------------------------------------------------                  
    // MapSynonymFields(Tbl84Synonym source, Tbl84Synonym target)
    // ---------------------------------------------------------                 
    private static void MapSynonymFields(Tbl84Synonym source, Tbl84Synonym target)
    {
        target.SynonymName = source.SynonymName;
        target.SpeciesId = source.SpeciesId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetSynonymByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl84Synonym?> GetSynonymByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl84Synonyms
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SynonymId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteSynonymAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSynonymAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl84Synonyms.FindAsync(id);
        if (entity != null)
        {
            context.Tbl84Synonyms.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl84Synonym entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SynonymExistsAsync(Tbl84Synonym entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl84Synonyms.AnyAsync(x =>
            x.SynonymName == entity.SynonymName &&
            x.SpeciesId == entity.SpeciesId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.SynonymId != entity.SynonymId
        );
    }

    // ---------------------------------------------------------                  
    // GetSynonymForSpeciesAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl84Synonym>> GetSynonymForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl84Synonyms
            .Where(e => e.SpeciesId == speciesId)
            .Include(e => e.Species)
            .ThenInclude(s => s.Genus)
            .OrderBy(e => e.SynonymName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Name

    #region Geographic

    // ---------------------------------------------------------                  
    // GetAllGeographicAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl87Geographic>> GetAllGeographicAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl87Geographics
            .AsNoTracking()
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchGeographicWithSpeciesAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl87Geographic>> SearchGeographicWithSpeciesAndGenusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl87Geographic>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl87Geographics
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.GeographicId == id || e.Info.StartsWith(search))
                .OrderBy(e => e.Info)
                .ToListAsync();
        }

        return await query
            .Where(e => e.Info.StartsWith(search))
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertGeographicAsync(Tbl87Geographic uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl87Geographic> UpsertGeographicAsync(Tbl87Geographic uiEntity)
    {
        return UpsertAsync<Tbl87Geographic, int>(
            uiEntity,
            ctx => ctx.Set<Tbl87Geographic>(),
            x => x.GeographicId,
            (set, id) => set.FirstOrDefaultAsync(x => x.GeographicId == id),
            MapGeographicFields
        );
    }

    // ---------------------------------------------------------                  
    // MapGeographicFields(Tbl87Geographic source, Tbl87Geographic target)
    // ---------------------------------------------------------                 
    private static void MapGeographicFields(Tbl87Geographic source, Tbl87Geographic target)
    {
        target.SpeciesId = source.SpeciesId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Continent = source.Continent;
        target.Country = source.Country;
        target.Address = source.Address;
        target.Jurisdiction = source.Jurisdiction;
        target.Http = source.Http;
        target.Latitude = source.Latitude;
        target.Latitude1 = source.Latitude1;
        target.Latitude2 = source.Latitude2;
        target.Latitude3 = source.Latitude3;
        target.Longitude = source.Longitude;
        target.Longitude1 = source.Longitude1;
        target.Longitude2 = source.Longitude2;
        target.Longitude3 = source.Longitude3;
        target.ZoomLevel = source.ZoomLevel;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetGeographicByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl87Geographic?> GetGeographicByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl87Geographics
            .Include(x => x.Species)
            .ThenInclude(s => s.Genus)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographicId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteGeographicAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteGeographicAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl87Geographics.FindAsync(id);
        if (entity != null)
        {
            context.Tbl87Geographics.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl87Geographic entity)
    // ---------------------------------------------------------
    //
    public async Task<bool> GeographicExistsAsync(Tbl87Geographic entity)
    {
        System.Diagnostics.Debug.WriteLine(
            $"GeographicExistsAsync START: HasLocalGeometry={HasLocalGeometry(entity)}, " +
            $"GeoId={entity.GeographicId}, Continent='{entity.Continent}', Country='{entity.Country}', " +
            $"Lat1={entity.Latitude1}, Lon1={entity.Longitude1}, Lat2={entity.Latitude2}, Lon2={entity.Longitude2}, Lat3={entity.Latitude3}, Lon3={entity.Longitude3}");

        if (!HasLocalGeometry(entity))
        {
            System.Diagnostics.Debug.WriteLine("GeographicExistsAsync RETURN FALSE: no local geometry");
            return false;
        }

        await using var context = _factory.CreateDbContext();

        var candidates = await context.Tbl87Geographics
            .Where(x =>
                x.SpeciesId == entity.SpeciesId &&
                x.Valid == entity.Valid &&
                x.ValidYear == entity.ValidYear &&
                x.Author == entity.Author &&
                x.AuthorYear == entity.AuthorYear &&
                x.Continent == entity.Continent &&
                x.Country == entity.Country &&
                x.Address == entity.Address &&
                x.Jurisdiction == entity.Jurisdiction &&
                x.GeographicId != entity.GeographicId)
            .ToListAsync();

        var result = candidates
            .Where(HasLocalGeometry)
            .Any(x => HaveSameLocalGeometry(x, entity));

        System.Diagnostics.Debug.WriteLine(
            $"GeographicExistsAsync END: candidates={candidates.Count}, result={result}");

        return result;
    }

    private static bool HaveSameLocalGeometry(Tbl87Geographic a, Tbl87Geographic b)
    {
        return
            NearlyEqual(a.Latitude1, b.Latitude1) &&
            NearlyEqual(a.Longitude1, b.Longitude1) &&
            NearlyEqual(a.Latitude2, b.Latitude2) &&
            NearlyEqual(a.Longitude2, b.Longitude2) &&
            NearlyEqual(a.Latitude3, b.Latitude3) &&
            NearlyEqual(a.Longitude3, b.Longitude3);
    }

    private static bool HasLocalGeometry(Tbl87Geographic x)
    {
        return
            !IsZeroCoordinatePair(x.Latitude1, x.Longitude1) &&
            !IsZeroCoordinatePair(x.Latitude2, x.Longitude2) &&
            !IsZeroCoordinatePair(x.Latitude3, x.Longitude3);
    }

    private static bool IsZeroCoordinatePair(double latitude, double longitude)
    {
        return NearlyEqual(latitude, 0d) && NearlyEqual(longitude, 0d);
    }

    private static bool NearlyEqual(double a, double b, double epsilon = 0.000001d)
    {
        return Math.Abs(a - b) < epsilon;
    }

   public async Task<IEnumerable<Tbl87Geographic>> GetGeographicForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl87Geographics
            .Where(e => e.SpeciesId == speciesId)
            .Include(e => e.Species)
            .ThenInclude(s => s.Genus)
            .OrderBy(e => e.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Image

    #region Reference

    //---------------------------------------------------------------
    public async Task<IEnumerable<Tbl90Reference>> GetAllReferenceAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .AsNoTracking()
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> SearchReferenceAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90Reference>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Where(e =>
                e.Info!.StartsWith(search) ||
                e.ReferenceId.ToString() == search)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<Tbl90Reference> GetReferenceByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References.FirstOrDefaultAsync(x => x.ReferenceId == id);
    }

    public async Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference uiEntity)
    {
        await using var context = _factory.CreateDbContext();

        Tbl90Reference dbEntity;

        if (uiEntity.ReferenceId == 0)
        {
            dbEntity = new Tbl90Reference();
            context.Tbl90References.Add(dbEntity);
        }
        else
        {
            dbEntity = await context.Tbl90References
                           .FirstOrDefaultAsync(x => x.ReferenceId == uiEntity.ReferenceId)
                       ?? throw new InvalidOperationException("Entity not found in database.");
        }

        CopyProperties(uiEntity, dbEntity,
            nameof(Tbl90Reference.ReferenceId),
            nameof(Tbl90Reference.Writer),
            nameof(Tbl90Reference.WriterDate),
            nameof(Tbl90Reference.Updater),
            nameof(Tbl90Reference.UpdaterDate),
            nameof(Tbl90Reference.CountId)
        );

        var isNew = dbEntity.ReferenceId == 0;
        ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();

        return dbEntity;
    }

    public async Task DeleteReferenceAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90References.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90References.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ReferenceExistsAsync(Tbl90Reference entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References.AnyAsync(x =>
            x.GenusId == entity.GenusId &&
            x.InfratribusId == entity.InfratribusId &&
            x.SubtribusId == entity.SubtribusId &&
            x.TribusId == entity.TribusId &&
            x.SupertribusId == entity.SupertribusId &&
            x.InfrafamilyId == entity.InfrafamilyId &&
            x.SubfamilyId == entity.SubfamilyId &&
            x.FamilyId == entity.FamilyId &&
            x.SuperfamilyId == entity.SuperfamilyId &&
            x.InfraordoId == entity.InfraordoId &&
            x.SubordoId == entity.SubordoId &&
            x.OrdoId == entity.OrdoId &&
            x.LegioId == entity.LegioId &&
            x.InfraclassId == entity.InfraclassId &&
            x.SubclassId == entity.SubclassId &&
            x.ClassId == entity.ClassId &&
            x.SuperclassId == entity.SuperclassId &&
            x.SubdivisionId == entity.SubdivisionId &&
            x.SubphylumId == entity.SubphylumId &&
            x.DivisionId == entity.DivisionId &&
            x.PhylumId == entity.PhylumId &&
            x.RegnumId == entity.RegnumId &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&

            x.ReferenceId != entity.ReferenceId
        );
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForRegnumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.RegnumId == regnumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForPhylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.PhylumId == phylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubphylumAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubphylumId == subphylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSuperclassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SuperclassId == superclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForClassAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.ClassId == classId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubclassId == subclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfraclassAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfraclassId == infraclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForLegioAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.LegioId == legioId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForOrdoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.OrdoId == ordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubordoId == subordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfraordoAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfraordoId == infraordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSuperfamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SuperfamilyId == superfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForFamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.FamilyId == familyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubfamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubfamilyId == subfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfrafamilyAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfrafamilyId == infrafamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSupertribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SupertribusId == supertribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubtribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubtribusId == subtribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForTribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.TribusId == tribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfratribusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfratribusId == infratribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForGenusAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.GenusId == genusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SpeciesId == speciesId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    //---------------------------------------------------------
    public async Task DeleteReferencesAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        var refs = context.Tbl90References
            .Where(r => r.InfratribusId == infratribusId);

        context.Tbl90References.RemoveRange(refs);
        await context.SaveChangesAsync();
    }

    #endregion Reference

    #region RefAuthor

    // ---------------------------------------------------------                  
    // GetAllRefAuthorAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl90RefAuthor>> GetAllRefAuthorAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefAuthors
            .AsNoTracking()
            .OrderBy(e => e.RefAuthorName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchRefAuthorAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90RefAuthor>> SearchRefAuthorAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefAuthor>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl90RefAuthors
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RefAuthorId == id || e.RefAuthorName.StartsWith(search))
                .OrderBy(e => e.RefAuthorName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RefAuthorName.StartsWith(search))
            .OrderBy(e => e.RefAuthorName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertRefAuthorAsync(Tbl90RefAuthor uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor uiEntity)
    {
        return UpsertAsync<Tbl90RefAuthor, int>(
            uiEntity,
            ctx => ctx.Set<Tbl90RefAuthor>(),
            x => x.RefAuthorId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RefAuthorId == id),
            MapRefAuthorFields
        );
    }

    // ---------------------------------------------------------                  
    // MapRefAuthorFields(Tbl90RefAuthor source, Tbl90RefAuthor target)
    // ---------------------------------------------------------                 
    private static void MapRefAuthorFields(Tbl90RefAuthor source, Tbl90RefAuthor target)
    {
        target.RefAuthorName = source.RefAuthorName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Publisher = source.Publisher;
        target.PublicationYear = source.PublicationYear;
        target.PublicationPlace = source.PublicationPlace;
        target.Page1 = source.Page1;
        target.ArticelTitle = source.ArticelTitle;
        target.BookName = source.BookName;
        target.ISBN = source.ISBN;
        target.Notes = source.Notes;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetRefAuthorByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl90RefAuthor> GetRefAuthorByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefAuthors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefAuthorId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteRefAuthorAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRefAuthorAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90RefAuthors.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90RefAuthors.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl90RefAuthor entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RefAuthorExistsAsync(Tbl90RefAuthor entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefAuthors.AnyAsync(x =>
            x.RefAuthorName == entity.RefAuthorName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.RefAuthorId != entity.RefAuthorId
        );
    }


    public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
            .OrderBy(k => k.RefAuthorName));
        return collection;
    }

    #endregion RefAuthor

    #region RefSource

    // ---------------------------------------------------------                  
    // GetAllRefSourceAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl90RefSource>> GetAllRefSourceAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefSources
            .AsNoTracking()
            .OrderBy(e => e.RefSourceName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchRefSourceWithNULLAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90RefSource>> SearchRefSourceAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefSource>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl90RefSources
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RefSourceId == id || e.RefSourceName.StartsWith(search))
                .OrderBy(e => e.RefSourceName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RefSourceName.StartsWith(search))
            .OrderBy(e => e.RefSourceName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertRefSourceAsync(Tbl90RefSource uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl90RefSource> UpsertRefSourceAsync(Tbl90RefSource uiEntity)
    {
        return UpsertAsync<Tbl90RefSource, int>(
            uiEntity,
            ctx => ctx.Set<Tbl90RefSource>(),
            x => x.RefSourceId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RefSourceId == id),
            MapRefSourceFields
        );
    }

    // ---------------------------------------------------------                  
    // MapRefSourceFields(Tbl90RefSource source, Tbl90RefSource target)
    // ---------------------------------------------------------                 
    private static void MapRefSourceFields(Tbl90RefSource source, Tbl90RefSource target)
    {
        target.RefSourceName = source.RefSourceName;
        target.Valid = source.Valid;
        target.Author = source.Author;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetRefSourceByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl90RefSource> GetRefSourceByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefSources
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefSourceId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteRefSourceAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRefSourceAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90RefSources.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90RefSources.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl90RefSource entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RefSourceExistsAsync(Tbl90RefSource entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefSources.AnyAsync(x =>
            x.RefSourceName == entity.RefSourceName &&
            x.Valid == entity.Valid &&
            x.Author == entity.Author &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.RefSourceId != entity.RefSourceId
        );
    }

    #endregion

    #region RefExpert
    // ---------------------------------------------------------                  
    // GetAllRefExpertAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl90RefExpert>> GetAllRefExpertAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefExperts
            .AsNoTracking()
            .OrderBy(e => e.RefExpertName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchRefExpertAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90RefExpert>> SearchRefExpertAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefExpert>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl90RefExperts
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RefExpertId == id || e.RefExpertName.StartsWith(search))
                .OrderBy(e => e.RefExpertName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RefExpertName.StartsWith(search))
            .OrderBy(e => e.RefExpertName)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertRefExpertAsync(Tbl90RefExpert uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert uiEntity)
    {
        return UpsertAsync<Tbl90RefExpert, int>(
            uiEntity,
            ctx => ctx.Set<Tbl90RefExpert>(),
            x => x.RefExpertId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RefExpertId == id),
            MapRefExpertFields
        );
    }

    // ---------------------------------------------------------                  
    // MapRefExpertFields(Tbl90RefExpert source, Tbl90RefExpert target)
    // ---------------------------------------------------------                 
    private static void MapRefExpertFields(Tbl90RefExpert source, Tbl90RefExpert target)
    {
        target.RefExpertName = source.RefExpertName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }

    // ---------------------------------------------------------                  
    // GetRefExpertByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl90RefExpert> GetRefExpertByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefExperts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefExpertId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteRefExpertAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRefExpertAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90RefExperts.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90RefExperts.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl90RefExpert entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RefExpertExistsAsync(Tbl90RefExpert entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefExperts.AnyAsync(x =>
            x.RefExpertName == entity.RefExpertName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.RefExpertId != entity.RefExpertId
        );
    }

    #endregion RefExpert

    #region Comment
    //--------------------------------------------------------
    public async Task<IEnumerable<Tbl93Comment>> GetAllCommentAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .AsNoTracking()
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> SearchCommentAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl93Comment>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .AsNoTracking()
            .Where(e =>
                e.Info.StartsWith(search) ||
                e.CommentId.ToString() == search)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<Tbl93Comment> GetCommentByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments.FirstOrDefaultAsync(x => x.CommentId == id);
    }

    public async Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment uiEntity)
    {
        await using var context = _factory.CreateDbContext();

        Tbl93Comment dbEntity;

        if (uiEntity.CommentId == 0)
        {
            dbEntity = new Tbl93Comment();
            context.Tbl93Comments.Add(dbEntity);
        }
        else
        {
            dbEntity = await context.Tbl93Comments
                           .FirstOrDefaultAsync(x => x.CommentId == uiEntity.CommentId)
                       ?? throw new InvalidOperationException("Entity not found.");
        }

        CopyProperties(uiEntity, dbEntity,
            nameof(Tbl93Comment.CommentId),
            nameof(Tbl93Comment.Writer),
            nameof(Tbl93Comment.WriterDate),
            nameof(Tbl93Comment.Updater),
            nameof(Tbl93Comment.UpdaterDate)
        );

        var isNew = dbEntity.CommentId == 0;
        ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();

        return dbEntity;
    }

    public async Task DeleteCommentAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl93Comments.FindAsync(id);
        if (entity != null)
        {
            context.Tbl93Comments.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> CommentExistsAsync(Tbl93Comment entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments.AnyAsync(x =>
            x.SpeciesId == entity.SpeciesId &&
            x.GenusId == entity.GenusId &&
            x.InfratribusId == entity.InfratribusId &&
            x.SubtribusId == entity.SubtribusId &&
            x.TribusId == entity.TribusId &&
            x.SupertribusId == entity.SupertribusId &&
            x.InfrafamilyId == entity.InfrafamilyId &&
            x.SubfamilyId == entity.SubfamilyId &&
            x.FamilyId == entity.FamilyId &&
            x.SuperfamilyId == entity.SuperfamilyId &&
            x.InfraordoId == entity.InfraordoId &&
            x.SubordoId == entity.SubordoId &&
            x.OrdoId == entity.OrdoId &&
            x.LegioId == entity.LegioId &&
            x.InfraclassId == entity.InfraclassId &&
            x.SubclassId == entity.SubclassId &&
            x.ClassId == entity.ClassId &&
            x.SuperclassId == entity.SuperclassId &&
            x.SubdivisionId == entity.SubdivisionId &&
            x.SubphylumId == entity.SubphylumId &&
            x.DivisionId == entity.DivisionId &&
            x.PhylumId == entity.PhylumId &&
            x.RegnumId == entity.RegnumId &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&

            x.CommentId != entity.CommentId
        );
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForRegnumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.RegnumId == regnumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForPhylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.PhylumId == phylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubphylumAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubphylumId == subphylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSuperclassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SuperclassId == superclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForClassAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.ClassId == classId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubclassId == subclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfraclassAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.InfraclassId == infraclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForLegioAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.LegioId == legioId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForOrdoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.OrdoId == ordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubordoId == subordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfraordoAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.InfraordoId == infraordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSuperfamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SuperfamilyId == superfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForFamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.FamilyId == familyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubfamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubfamilyId == subfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfrafamilyAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.InfrafamilyId == infrafamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSupertribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SupertribusId == supertribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForTribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.TribusId == tribusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubtribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.SubtribusId == subtribusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfratribusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.InfratribusId == infratribusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForGenusAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.GenusId == genusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.SpeciesId == speciesId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeleteCommentsAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        var comments = context.Tbl93Comments
            .Where(c => c.InfratribusId == infratribusId);

        context.Tbl93Comments.RemoveRange(comments);
        await context.SaveChangesAsync();
    }

    #endregion Comment

    #region Country

    // ---------------------------------------------------------                  
    // GetAllCountryAsync()
    // // ---------------------------------------------------------
    public async Task<IEnumerable<TblCountry>> GetAllCountryAsync()
    {
        var started = System.Diagnostics.Stopwatch.StartNew();
        await using var context = await _factory.CreateDbContextAsync();

        var list = await context.TblCountries
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync();

        System.Diagnostics.Debug.WriteLine(
            $"[DataService.GetAllCountryAsync] count = {list.Count}, elapsed = {started.ElapsedMilliseconds} ms");

        return list;
    }
    // ---------------------------------------------------------                  
    // SearchCountryAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<TblCountry>> SearchCountryAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<TblCountry>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.TblCountries
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.CountryId == id || e.Name.StartsWith(search))
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        return await query
            .Where(e => e.Name.StartsWith(search))
            .OrderBy(e => e.Name)
            .ToListAsync();

    }

    // ---------------------------------------------------------                  
    // UpsertCountryAsync(TblCountry uiEntity)
    // ---------------------------------------------------------
    public async Task<TblCountry> UpsertCountryAsync(TblCountry uiEntity)
    {
        await using var context = _factory.CreateDbContext();

        TblCountry dbEntity;

        if (uiEntity.CountryId == 0)
        {
            dbEntity = new TblCountry();
            context.TblCountries.Add(dbEntity);
        }
        else
        {
            dbEntity = await context.TblCountries
                           .FirstOrDefaultAsync(x => x.CountryId == uiEntity.CountryId)
                       ?? throw new InvalidOperationException("Entity not found.");
        }

        CopyProperties(uiEntity, dbEntity,
            nameof(TblCountry.CountryId)
            //nameof(TblCountry.Writer),
            //nameof(TblCountry.WriterDate),
            //nameof(TblCountry.Updater),
            //nameof(TblCountry.UpdaterDate)
        );

        var isNew = dbEntity.CountryId == 0;
        //     ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();

        return dbEntity;
    }

    // ---------------------------------------------------------                  
    // MapCountryFields(TblCountry source, TblCountry target)
    // ---------------------------------------------------------                 
    private static void MapCountryFields(TblCountry source, TblCountry target)
    {
        target.Name = source.Name;
        target.Regex = source.Regex;
        target.ZoomLevel = source.ZoomLevel;
        target.Latitude = source.Latitude;
        target.Longitude = source.Longitude;
    }

    // ---------------------------------------------------------                  
    // GetCountryByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<TblCountry?> GetCountryByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblCountries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CountryId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteCountryAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteCountryAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.TblCountries.FindAsync(id);
        if (entity != null)
        {
            context.TblCountries.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(TblCountry entity)
    // ---------------------------------------------------------                 
    public async Task<bool> CountryExistsAsync(TblCountry entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblCountries.AnyAsync(x =>
            x.Name == entity.Name &&
            x.Regex == entity.Regex &&
            x.ZoomLevel == entity.ZoomLevel &&
            x.Latitude == entity.Latitude &&
            x.Longitude == entity.Longitude &&
            x.CountryId != entity.CountryId
        );
    }

    #endregion Country

    #region UserProfile

    // ---------------------------------------------------------                  
    // GetAllUserProfileAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<TblUserProfile>> GetAllUserProfileAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblUserProfiles
            .AsNoTracking()
            .OrderBy(e => e.Email)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // SearchUserProfileAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<TblUserProfile>> SearchUserProfileAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<TblUserProfile>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.TblUserProfiles
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.UserProfileId == id || e.Email.StartsWith(search))
                .OrderBy(e => e.Email)
                .ToListAsync();
        }

        return await query
            .Where(e => e.Email.StartsWith(search))
            .OrderBy(e => e.Email)
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // UpsertUserProfileAsync(TblUserProfile uiEntity)
    // ---------------------------------------------------------                 
    public Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile uiEntity)
    {
        return UpsertAsync<TblUserProfile, int>(
            uiEntity,
            ctx => ctx.Set<TblUserProfile>(),
            x => x.UserProfileId,
            (set, id) => set.FirstOrDefaultAsync(x => x.UserProfileId == id),
            MapUserProfileFields
        );
    }

    // ---------------------------------------------------------                  
    // MapUserProfileFields(TblUserProfile source, TblUserProfile target)
    // ---------------------------------------------------------                 
    private static void MapUserProfileFields(TblUserProfile source, TblUserProfile target)
    {
        target.Email = source.Email;
        target.LastName = source.LastName;
        target.FirstName = source.FirstName;
        target.Title = source.Title;
        target.Password = source.Password;
        target.Role = source.Role;
        target.Flag = source.Flag;
        target.StartDate = source.StartDate;
        target.EndDate = source.EndDate;
        target.Notes = source.Notes;
        target.Colour = source.Colour;
        target.BirthDate = source.BirthDate;
        target.Gender = source.Gender;
        target.Country = source.Country;
        target.Postcode = source.Postcode;
        target.City = source.City;
        target.Street1 = source.Street1;
        target.Street2 = source.Street2;
        target.Tel = source.Tel;
        target.Mobil = source.Mobil;
        target.Fax = source.Fax;
        target.HomePageUrl = source.HomePageUrl;
        target.Business = source.Business;
        target.Company = source.Company;
        target.Memo = source.Memo;
        target.Filestream = source.Filestream;
        target.ImageMimeType = source.ImageMimeType;
        target.FilestreamId = source.FilestreamId;
        target.Signature = source.Signature;
        target.MailNewsletter = source.MailNewsletter;
        target.MaulHtml = source.MaulHtml;
        target.Known = source.Known;
        target.PasswordResetToken = source.PasswordResetToken;
        target.PasswordResetTokenExpiryUtc = source.PasswordResetTokenExpiryUtc;
    }

    // ---------------------------------------------------------                  
    // GetUserProfileByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<TblUserProfile?> GetUserProfileByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblUserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserProfileId == id);
    }

    // ---------------------------------------------------------                  
    // DeleteUserProfileAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteUserProfileAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.TblUserProfiles.FindAsync(id);
        if (entity != null)
        {
            context.TblUserProfiles.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------                  
    // ExistsAsync(TblUserProfile entity)
    // ---------------------------------------------------------                 
    public async Task<bool> UserProfileExistsAsync(TblUserProfile entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblUserProfiles.AnyAsync(x =>
            x.Email == entity.Email &&
            x.LastName == entity.LastName &&
            x.FirstName == entity.FirstName &&
            x.Title == entity.Title &&
            x.Password == entity.Password &&
            x.Role == entity.Role &&
            x.Flag == entity.Flag &&
            x.StartDate == entity.StartDate &&
            x.EndDate == entity.EndDate &&
            x.Notes == entity.Notes &&
            x.Colour == entity.Colour &&
            x.BirthDate == entity.BirthDate &&
            x.Gender == entity.Gender &&
            x.Country == entity.Country &&
            x.Postcode == entity.Postcode &&
            x.City == entity.City &&
            x.Street1 == entity.Street1 &&
            x.Street2 == entity.Street2 &&
            x.Tel == entity.Tel &&
            x.Mobil == entity.Mobil &&
            x.Fax == entity.Fax &&
            x.HomePageUrl == entity.HomePageUrl &&
            x.Business == entity.Business &&
            x.Company == entity.Company &&
            x.Memo == entity.Memo &&
            x.Filestream == entity.Filestream &&
            x.ImageMimeType == entity.ImageMimeType &&
            x.FilestreamId == entity.FilestreamId &&
            x.Signature == entity.Signature &&
            x.MailNewsletter == entity.MailNewsletter &&
            x.MaulHtml == entity.MaulHtml &&
            x.Known == entity.Known &&
            x.PasswordResetToken == entity.PasswordResetToken &&
            x.PasswordResetTokenExpiryUtc == entity.PasswordResetTokenExpiryUtc &&
            x.UserProfileId != entity.UserProfileId
        );
    }

    #endregion UserProfile

    #region Food
    // ---------------------------------------------------------                  
    // GetAllFoodAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<TblFood>> GetAllFoodAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblFoods
            .AsNoTracking()
            .OrderBy(e => e.FoodName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchFoodAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<TblFood>> SearchFoodAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<TblFood>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.TblFoods
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.FoodId == id || e.FoodName.StartsWith(search))
                .OrderBy(e => e.FoodName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.FoodName.StartsWith(search))
            .OrderBy(e => e.FoodName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertFoodAsync(TblFood uiEntity)
    // ---------------------------------------------------------                 
    public Task<TblFood> UpsertFoodAsync(TblFood uiEntity)
    {
        return UpsertAsync<TblFood, int>(
            uiEntity,
            ctx => ctx.Set<TblFood>(),
            x => x.FoodId,
            (set, id) => set.FirstOrDefaultAsync(x => x.FoodId == id),
            MapFoodFields
        );
    }
    // ---------------------------------------------------------                  
    // MapFoodFields(TblFood source, TblFood target)
    // ---------------------------------------------------------                 
    private static void MapFoodFields(TblFood source, TblFood target)
    {
        target.FoodName = source.FoodName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.ShortInfo = source.ShortInfo;
        target.FoodVolume = source.FoodVolume;
        target.Info = source.Info;
        target.Symptom = source.Symptom;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetFoodByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<TblFood?> GetFoodByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblFoods
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FoodId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteFoodAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteFoodAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.TblFoods.FindAsync(id);
        if (entity != null)
        {
            context.TblFoods.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(TblFood entity)
    // ---------------------------------------------------------                 
    public async Task<bool> FoodExistsAsync(TblFood entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblFoods.AnyAsync(x =>
            x.FoodName == entity.FoodName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.ShortInfo == entity.ShortInfo &&
            x.FoodVolume == entity.FoodVolume &&
            x.Info == entity.Info &&
            x.Symptom == entity.Symptom &&
            x.Memo == entity.Memo &&
            x.FoodId != entity.FoodId
        );
    }

    #endregion Food

    #region Disease
    // ---------------------------------------------------------                  
    // GetAllDiseaseAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<TblDisease>> GetAllDiseaseAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblDiseases
            .AsNoTracking()
            .OrderBy(e => e.DiseaseName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchDiseaseWithNULLAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<TblDisease>> SearchDiseaseAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<TblDisease>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.TblDiseases
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.DiseaseId == id || e.DiseaseName.StartsWith(search))
                .OrderBy(e => e.DiseaseName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.DiseaseName.StartsWith(search))
            .OrderBy(e => e.DiseaseName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertDiseaseAsync(TblDisease uiEntity)
    // ---------------------------------------------------------                 
    public Task<TblDisease> UpsertDiseaseAsync(TblDisease uiEntity)
    {
        return UpsertAsync<TblDisease, int>(
            uiEntity,
            ctx => ctx.Set<TblDisease>(),
            x => x.DiseaseId,
            (set, id) => set.FirstOrDefaultAsync(x => x.DiseaseId == id),
            MapDiseaseFields
        );
    }
    // ---------------------------------------------------------                  
    // MapDiseaseFields(TblDisease source, TblDisease target)
    // ---------------------------------------------------------                 
    private static void MapDiseaseFields(TblDisease source, TblDisease target)
    {
        target.DiseaseName = source.DiseaseName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.ShortInfo = source.ShortInfo;
        target.Symptom = source.Symptom;
        target.Reason = source.Reason;
        target.Therapie = source.Therapie;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetDiseaseByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<TblDisease?> GetDiseaseByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblDiseases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DiseaseId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteDiseaseAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteDiseaseAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.TblDiseases.FindAsync(id);
        if (entity != null)
        {
            context.TblDiseases.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(TblDisease entity)
    // ---------------------------------------------------------                 
    public async Task<bool> DiseaseExistsAsync(TblDisease entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblDiseases.AnyAsync(x =>
            x.DiseaseName == entity.DiseaseName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.ShortInfo == entity.ShortInfo &&
            x.Symptom == entity.Symptom &&
            x.Reason == entity.Reason &&
            x.Therapie == entity.Therapie &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.DiseaseId != entity.DiseaseId
        );
    }

    #endregion Disease

    #region Miscellaneous
    // ---------------------------------------------------------                  
    // GetAllMiscellaneousAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<TblMiscellaneous>> GetAllMiscellaneousAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblMiscellaneousses
            .AsNoTracking()
            .OrderBy(e => e.MiscellaneousName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchMiscellaneousAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<TblMiscellaneous>> SearchMiscellaneousAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<TblMiscellaneous>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.TblMiscellaneousses
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.MiscellaneousId == id || e.MiscellaneousName.StartsWith(search))
                .OrderBy(e => e.MiscellaneousName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.MiscellaneousName.StartsWith(search))
            .OrderBy(e => e.MiscellaneousName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertMiscellaneousAsync(TblMiscellaneous uiEntity)
    // ---------------------------------------------------------                 
    public Task<TblMiscellaneous> UpsertMiscellaneousAsync(TblMiscellaneous uiEntity)
    {
        return UpsertAsync<TblMiscellaneous, int>(
            uiEntity,
            ctx => ctx.Set<TblMiscellaneous>(),
            x => x.MiscellaneousId,
            (set, id) => set.FirstOrDefaultAsync(x => x.MiscellaneousId == id),
            MapMiscellaneousFields
        );
    }
    // ---------------------------------------------------------                  
    // MapMiscellaneousFields(TblMiscellaneous source, TblMiscellaneous target)
    // ---------------------------------------------------------                 
    private static void MapMiscellaneousFields(TblMiscellaneous source, TblMiscellaneous target)
    {
        target.MiscellaneousName = source.MiscellaneousName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.ShortInfo = source.ShortInfo;
        target.Thema1 = source.Thema1;
        target.Thema2 = source.Thema2;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetMiscellaneousByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<TblMiscellaneous?> GetMiscellaneousByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblMiscellaneousses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MiscellaneousId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteMiscellaneousAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteMiscellaneousAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.TblMiscellaneousses.FindAsync(id);
        if (entity != null)
        {
            context.TblMiscellaneousses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(TblMiscellaneous entity)
    // ---------------------------------------------------------                 
    public async Task<bool> MiscellaneousExistsAsync(TblMiscellaneous entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.TblMiscellaneousses.AnyAsync(x =>
            x.MiscellaneousName == entity.MiscellaneousName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.ShortInfo == entity.ShortInfo &&
            x.Thema1 == entity.Thema1 &&
            x.Thema2 == entity.Thema2 &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.MiscellaneousId != entity.MiscellaneousId
        );
    }

    #endregion Miscellaneous

    #endregion CRUD

    #region REPORT
    public async Task<IEnumerable<Tbl03Regnum>> SearchRegnumForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl03Regnum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums
            .AsNoTracking()
            .Where(e => e.RegnumName != null && !e.RegnumName.Contains("#"))
            .Where(e =>
                e.RegnumName.StartsWith(search) ||
                (e.Subregnum != null && e.Subregnum.Contains(search)) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.RegnumName)
            .ThenBy(e => e.Subregnum)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl06Phylum>> SearchPhylumForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl06Phylum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .AsNoTracking()
            .Where(e => e.PhylumName != null && !e.PhylumName.Contains("#"))
            .Where(e =>
                e.PhylumName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.PhylumName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl12Subphylum>> SearchSubphylumForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl12Subphylum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .AsNoTracking()
            .Where(e => e.SubphylumName != null && !e.SubphylumName.Contains("#"))
            .Where(e =>
                e.SubphylumName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SubphylumName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl18Superclass>> SearchSuperclassForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl18Superclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .AsNoTracking()
            .Where(e => e.SuperclassName != null && !e.SuperclassName.Contains("#"))
            .Where(e =>
                e.SuperclassName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SuperclassName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl21Class>> SearchClassForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl21Class>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .AsNoTracking()
            .Where(e => e.ClassName != null && !e.ClassName.Contains("#"))
            .Where(e =>
                e.ClassName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.ClassName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl24Subclass>> SearchSubclassForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl24Subclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .AsNoTracking()
            .Where(e => e.SubclassName != null && !e.SubclassName.Contains("#"))
            .Where(e =>
                e.SubclassName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SubclassName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl27Infraclass>> SearchInfraclassForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl27Infraclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .AsNoTracking()
            .Where(e => e.InfraclassName != null && !e.InfraclassName.Contains("#"))
            .Where(e =>
                e.InfraclassName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.InfraclassName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl30Legio>> SearchLegioForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl30Legio>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .AsNoTracking()
            .Where(e => e.LegioName != null && !e.LegioName.Contains("#"))
            .Where(e =>
                e.LegioName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.LegioName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl33Ordo>> SearchOrdoForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl33Ordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .AsNoTracking()
            .Where(e => e.OrdoName != null && !e.OrdoName.Contains("#"))
            .Where(e =>
                e.OrdoName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.OrdoName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl36Subordo>> SearchSubordoForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl36Subordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .AsNoTracking()
            .Where(e => e.SubordoName != null && !e.SubordoName.Contains("#"))
            .Where(e =>
                e.SubordoName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SubordoName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl39Infraordo>> SearchInfraordoForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl39Infraordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .AsNoTracking()
            .Where(e => e.InfraordoName != null && !e.InfraordoName.Contains("#"))
            .Where(e =>
                e.InfraordoName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.InfraordoName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl42Superfamily>> SearchSuperfamilyForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl42Superfamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .AsNoTracking()
            .Where(e => e.SuperfamilyName != null && !e.SuperfamilyName.Contains("#"))
            .Where(e =>
                e.SuperfamilyName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SuperfamilyName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl45Family>> SearchFamilyForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl45Family>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .AsNoTracking()
            .Where(e => e.FamilyName != null && !e.FamilyName.Contains("#"))
            .Where(e =>
                e.FamilyName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.FamilyName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl48Subfamily>> SearchSubfamilyForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl48Subfamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .AsNoTracking()
            .Where(e => e.SubfamilyName != null && !e.SubfamilyName.Contains("#"))
            .Where(e =>
                e.SubfamilyName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SubfamilyName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl51Infrafamily>> SearchInfrafamilyForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl51Infrafamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .AsNoTracking()
            .Where(e => e.InfrafamilyName != null && !e.InfrafamilyName.Contains("#"))
            .Where(e =>
                e.InfrafamilyName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.InfrafamilyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl54Supertribus>> SearchSupertribusForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl54Supertribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .AsNoTracking()
            .Where(e => e.SupertribusName != null && !e.SupertribusName.Contains("#"))
            .Where(e =>
                e.SupertribusName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SupertribusName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl57Tribus>> SearchTribusForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl57Tribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .AsNoTracking()
            .Where(e => e.TribusName != null && !e.TribusName.Contains("#"))
            .Where(e =>
                e.TribusName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.TribusName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl60Subtribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .AsNoTracking()
            .Where(e => e.SubtribusName != null && !e.SubtribusName.Contains("#"))
            .Where(e =>
                e.SubtribusName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.SubtribusName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl63Infratribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .AsNoTracking()
            .Where(e => e.InfratribusName != null && !e.InfratribusName.Contains("#"))
            .Where(e =>
                e.InfratribusName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl66Genus>> SearchGenusForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl66Genus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .Include(e => e.Infratribus)
            .AsNoTracking()
            .Where(e => e.GenusName != null && !e.GenusName.Contains("#"))
            .Where(e =>
                e.GenusName.StartsWith(search) ||
                (e.EngName != null && e.EngName.Contains(search)) ||
                (e.GerName != null && e.GerName.Contains(search)) ||
                (e.FraName != null && e.FraName.Contains(search)) ||
                (e.PorName != null && e.PorName.Contains(search)))
            .OrderBy(e => e.GenusName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl75Species>> SearchSpeciesForReportAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl75Species>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .Include(e => e.Genus)
            .Include(e => e.Speciesgroup)
            .AsNoTracking()
            .Where(e => e.SpeciesName != null && !e.SpeciesName.Contains("#"))
            .Where(e =>
                e.SpeciesName.StartsWith(search) ||
                (e.Genus != null && e.Genus.GenusName != null &&
                 e.Genus.GenusName.Contains(search)) ||
                (e.Subspecies != null && e.Subspecies.Contains(search)) ||
                (e.Divers != null && e.Divers.Contains(search)) ||
                (e.TradeName != null && e.TradeName.Contains(search)) ||
                (e.Author != null && e.Author.Contains(search)))
            .OrderBy(e => e.Genus!.GenusName)
            .ThenBy(e => e.SpeciesName)
            .ThenBy(e => e.Subspecies)
            .ThenBy(e => e.Divers)
            .ToListAsync();
    }

    #endregion REPORT

}
