using System.Collections.ObjectModel;
using ATIS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Services;

public class LookupService
{
    private readonly IDbContextFactory<AtisDbContext> _factory;
    private readonly IUserService _user;

    public LookupService(IDbContextFactory<AtisDbContext> factory, IUserService user)
    {
        _factory = factory;
        _user = user;
    }

    private IDataService CreateDataService()
        => new DataService(_factory, _user);

    private Dictionary<int, string?>? _supertribusLookupCache;
    private Dictionary<int, string?>? _tribusLookupCache;
    private Dictionary<int, string?>? _subtribusLookupCache;
    private Dictionary<int, string?>? _infratribusLookupCache;
    private Dictionary<int, string?>? _genusLookupCache;
    private Dictionary<int, string?>? _speciesLookupCache;

    private Dictionary<int, string?>? _refAuthorLookupCache;
    private Dictionary<int, string?>? _refExpertLookupCache;
    private Dictionary<int, string?>? _refSourceLookupCache;


    private ObservableCollection<Models.Tbl54Supertribus>? _supertribusListCache;
    private ObservableCollection<Models.Tbl57Tribus>? _tribusListCache;
    private ObservableCollection<Models.Tbl60Subtribus>? _subtribusListCache;
    private ObservableCollection<Models.Tbl63Infratribus>? _infratribusListCache;
    private ObservableCollection<Models.Tbl66Genus>? _genusListCache;
    private ObservableCollection<Models.Tbl75Species>? _speciesListCache;

    private ObservableCollection<Models.Tbl90RefAuthor>? _refAuthorListCache;
    private ObservableCollection<Models.Tbl90RefExpert>? _refExpertListCache;
    private ObservableCollection<Models.Tbl90RefSource>? _refSourceListCache;

    //--------------------------------------------------------------------
    public async Task<Dictionary<int, string?>> GetSupertribusLookupAsync()
    {
        if (_supertribusLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSupertribusAsync();
            _supertribusLookupCache = list.ToDictionary(x => x.SupertribusId, x => x.SupertribusName);
        }
        return _supertribusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetTribusLookupAsync()
    {
        if (_tribusLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllTribusAsync();
            _tribusLookupCache = list.ToDictionary(x => x.TribusId, x => x.TribusName);
        }
        return _tribusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSubtribusLookupAsync()
    {
        if (_subtribusLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubtribusAsync();
            _subtribusLookupCache = list.ToDictionary(x => x.SubtribusId, x => x.SubtribusName);
        }
        return _subtribusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetInfratribusLookupAsync()
    {
        if (_infratribusLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfratribusAsync();
            _infratribusLookupCache = list.ToDictionary(x => x.InfratribusId, x => x.InfratribusName);
        }
        return _infratribusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetGenusLookupAsync()
    {
        if (_genusLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllGenusAsync();
            _genusLookupCache = list.ToDictionary(x => x.GenusId, x => x.GenusName);
        }
        return _genusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSpeciesLookupAsync()
    {
        if (_speciesLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSpeciesAsync();
            _speciesLookupCache = list.ToDictionary(x => x.SpeciesId, x => x.SpeciesName);
        }
        return _speciesLookupCache;
    }


    public async Task<Dictionary<int, string?>> GetRefAuthorLookupAsync()
    {
        if (_refAuthorLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefAuthorsAsync();
            _refAuthorLookupCache = list.ToDictionary(x => x.RefAuthorId, x => x.RefAuthorName);
        }
        return _refAuthorLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetRefExpertLookupAsync()
    {
        if (_refExpertLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefExpertsAsync();
            _refExpertLookupCache = list.ToDictionary(x => x.RefExpertId, x => x.RefExpertName);
        }
        return _refExpertLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetRefSourceLookupAsync()
    {
        if (_refSourceLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefSourcesAsync();
            _refSourceLookupCache = list.ToDictionary(x => x.RefSourceId, x => x.RefSourceName);
        }
        return _refSourceLookupCache;
    }

    //--------------------------------------------------------------------
    public async Task<ObservableCollection<Models.Tbl54Supertribus>> GetSupertribusListAsync()
    {
        if (_supertribusListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSupertribusAsync();
            _supertribusListCache = new ObservableCollection<Models.Tbl54Supertribus>(list);
        }
        return _supertribusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl57Tribus>> GetTribusListAsync()
    {
        if (_tribusListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllTribusAsync();
            _tribusListCache = new ObservableCollection<Models.Tbl57Tribus>(list);
        }
        return _tribusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl60Subtribus>> GetSubtribusListAsync()
    {
        if (_subtribusListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubtribusAsync();
            _subtribusListCache = new ObservableCollection<Models.Tbl60Subtribus>(list);
        }
        return _subtribusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl63Infratribus>> GetInfratribusListAsync()
    {
        if (_infratribusListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfratribusAsync();
            _infratribusListCache = new ObservableCollection<Models.Tbl63Infratribus>(list);
        }
        return _infratribusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl66Genus>> GetGenusListAsync()
    {
        if (_genusListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllGenusAsync();
            _genusListCache = new ObservableCollection<Models.Tbl66Genus>(list);
        }
        return _genusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl75Species>> GetSpeciesListAsync()
    {
        if (_speciesListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSpeciesAsync();
            _speciesListCache = new ObservableCollection<Models.Tbl75Species>(list);
        }
        return _speciesListCache;
    }


    public async Task<ObservableCollection<Models.Tbl90RefAuthor>> GetRefAuthorListAsync()
    {
        if (_refAuthorListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefAuthorsAsync();
            _refAuthorListCache = new ObservableCollection<Models.Tbl90RefAuthor>(list);
        }
        return _refAuthorListCache;
    }
    public async Task<ObservableCollection<Models.Tbl90RefExpert>> GetRefExpertListAsync()
    {
        if (_refExpertListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefExpertsAsync();
            _refExpertListCache = new ObservableCollection<Models.Tbl90RefExpert>(list);
        }
        return _refExpertListCache;
    }
    public async Task<ObservableCollection<Models.Tbl90RefSource>> GetRefSourceListAsync()
    {
        if (_refSourceListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefSourcesAsync();
            _refSourceListCache = new ObservableCollection<Models.Tbl90RefSource>(list);
        }
        return _refSourceListCache;
    }
}
