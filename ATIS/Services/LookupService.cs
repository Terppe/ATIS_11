using System.Collections.ObjectModel;
using ATIS.Interfaces;

namespace ATIS.Services;

public class LookupService
{
    private readonly IDataService _data;

    public LookupService(IDataService data)
    {
        _data = data;
    }

    private Dictionary<int, string?>? _tribusLookupCache;
    private Dictionary<int, string?>? _subtribusLookupCache;
    private Dictionary<int, string?>? _infratribusLookupCache;

    private ObservableCollection<Models.Tbl57Tribus>? _tribusListCache;
    private ObservableCollection<Models.Tbl60Subtribus>? _subtribusListCache;
    private ObservableCollection<Models.Tbl63Infratribus>? _infratribusListCache;

    public async Task<Dictionary<int, string?>> GetTribusLookupAsync()
    {
        if (_tribusLookupCache == null)
        {
            var list = await _data.GetAllTribusAsync();
            _tribusLookupCache = list.ToDictionary(x => x.TribusId, x => x.TribusName);
        }
        return _tribusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSubtribusLookupAsync()
    {
        if (_subtribusLookupCache == null)
        {
            var list = await _data.GetAllSubtribusAsync();
            _subtribusLookupCache = list.ToDictionary(x => x.SubtribusId, x => x.SubtribusName);
        }
        return _subtribusLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetInfratribusLookupAsync()
    {
        if (_infratribusLookupCache == null)
        {
            var list = await _data.GetAllInfratribusAsync();
            _infratribusLookupCache = list.ToDictionary(x => x.InfratribusId, x => x.InfratribusName);
        }
        return _infratribusLookupCache;
    }
    //---------------------------------------------------------------
    public async Task<ObservableCollection<Models.Tbl57Tribus>> GetTribusListAsync()
    {
        if (_tribusListCache == null)
        {
            var list = await _data.GetAllTribusAsync();
            _tribusListCache = new ObservableCollection<Models.Tbl57Tribus>(list);
        }
        return _tribusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl60Subtribus>> GetSubtribusListAsync()
    {
        if (_subtribusListCache == null)
        {
            var list = await _data.GetAllSubtribusAsync();
            _subtribusListCache = new ObservableCollection<Models.Tbl60Subtribus>(list);
        }
        return _subtribusListCache;
    }
    public async Task<ObservableCollection<Models.Tbl63Infratribus>> GetInfratribusListAsync()
    {
        if (_infratribusListCache == null)
        {
            var list = await _data.GetAllInfratribusAsync();
            _infratribusListCache = new ObservableCollection<Models.Tbl63Infratribus>(list);
        }
        return _infratribusListCache;
    }
}
