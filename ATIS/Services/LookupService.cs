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

    private Dictionary<int, string> _regnumLookupCache;
    private Dictionary<int, string?>? _phylumLookupCache;
    private Dictionary<int, string?>? _subphylumLookupCache;
    private Dictionary<int, string?>? _superclassLookupCache;
    private Dictionary<int, string?>? _classLookupCache;
    private Dictionary<int, string?>? _subclassLookupCache;
    private Dictionary<int, string?>? _infraclassLookupCache;
    private Dictionary<int, string?>? _legioLookupCache;
    private Dictionary<int, string?>? _ordoLookupCache;
    private Dictionary<int, string?>? _subordoLookupCache;
    private Dictionary<int, string?>? _infraordoLookupCache;
    private Dictionary<int, string?>? _superfamilyLookupCache;
    private Dictionary<int, string?>? _familyLookupCache;
    private Dictionary<int, string?>? _subfamilyLookupCache;
    private Dictionary<int, string?>? _infrafamilyLookupCache;
    private Dictionary<int, string?>? _supertribusLookupCache;
    private Dictionary<int, string?>? _tribusLookupCache;
    private Dictionary<int, string?>? _subtribusLookupCache;
    private Dictionary<int, string?>? _infratribusLookupCache;
    private Dictionary<int, string?>? _genusLookupCache;
    private Dictionary<int, string?>? _speciesLookupCache;

    private Dictionary<int, string?>? _refAuthorLookupCache;
    private Dictionary<int, string?>? _refExpertLookupCache;
    private Dictionary<int, string?>? _refSourceLookupCache;

    //--------------------------------------------------------------------
    private ObservableCollection<Models.Tbl03Regnum>? _regnumListCache;
    private ObservableCollection<Models.Tbl06Phylum>? _phylumListCache;
    private ObservableCollection<Models.Tbl12Subphylum>? _subphylumListCache;
    private ObservableCollection<Models.Tbl18Superclass>? _superclassListCache;
    private ObservableCollection<Models.Tbl21Class>? _classListCache;
    private ObservableCollection<Models.Tbl24Subclass>? _subclassListCache;
    private ObservableCollection<Models.Tbl27Infraclass>? _infraclassListCache;
    private ObservableCollection<Models.Tbl30Legio>? _legioListCache;
    private ObservableCollection<Models.Tbl33Ordo>? _ordoListCache;
    private ObservableCollection<Models.Tbl36Subordo>? _subordoListCache;
    private ObservableCollection<Models.Tbl39Infraordo>? _infraordoListCache;
    private ObservableCollection<Models.Tbl42Superfamily>? _superfamilyListCache;
    private ObservableCollection<Models.Tbl45Family>? _familyListCache;
    private ObservableCollection<Models.Tbl48Subfamily>? _subfamilyListCache;
    private ObservableCollection<Models.Tbl51Infrafamily>? _infrafamilyListCache;
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
    public async Task<Dictionary<int, string?>> GetRegnumLookupAsync()
    {
        if (_regnumLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRegnumAsync();
            _regnumLookupCache = list.ToDictionary(x => x.RegnumId, x => x.RegnumName);
        }
        return _regnumLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetPhylumLookupAsync()
    {
        if (_phylumLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllPhylumAsync();
            _phylumLookupCache = list.ToDictionary(x => x.PhylumId, x => x.PhylumName);
        }
        return _phylumLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSubphylumLookupAsync()
    {
        if (_subphylumLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubphylumAsync();
            _subphylumLookupCache = list.ToDictionary(x => x.SubphylumId, x => x.SubphylumName);
        }
        return _subphylumLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSuperclassLookupAsync()
    {
        if (_superclassLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSuperclassAsync();
            _superclassLookupCache = list.ToDictionary(x => x.SuperclassId, x => x.SuperclassName);
        }
        return _superclassLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetClassLookupAsync()
    {
        if (_classLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllClassAsync();
            _classLookupCache = list.ToDictionary(x => x.ClassId, x => x.ClassName);
        }
        return _classLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSubclassLookupAsync()
    {
        if (_subclassLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubclassAsync();
            _subclassLookupCache = list.ToDictionary(x => x.SubclassId, x => x.SubclassName);
        }
        return _subclassLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetInfraclassLookupAsync()
    {
        if (_infraclassLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfraclassAsync();
            _infraclassLookupCache = list.ToDictionary(x => x.InfraclassId, x => x.InfraclassName);
        }
        return _infraclassLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetLegioLookupAsync()
    {
        if (_legioLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllLegioAsync();
            _legioLookupCache = list.ToDictionary(x => x.LegioId, x => x.LegioName);
        }
        return _legioLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetOrdoLookupAsync()
    {
        if (_ordoLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllOrdoAsync();
            _ordoLookupCache = list.ToDictionary(x => x.OrdoId, x => x.OrdoName);
        }
        return _ordoLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSubordoLookupAsync()
    {
        if (_subordoLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubordoAsync();
            _subordoLookupCache = list.ToDictionary(x => x.SubordoId, x => x.SubordoName);
        }
        return _subordoLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetInfraordoLookupAsync()
    {
        if (_infraordoLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfraordoAsync();
            _infraordoLookupCache = list.ToDictionary(x => x.InfraordoId, x => x.InfraordoName);
        }
        return _infraordoLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSuperfamilyLookupAsync()
    {
        if (_superfamilyLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSuperfamilyAsync();
            _superfamilyLookupCache = list.ToDictionary(x => x.SuperfamilyId, x => x.SuperfamilyName);
        }
        return _superfamilyLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetFamilyLookupAsync()
    {
        if (_familyLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllFamilyAsync();
            _familyLookupCache = list.ToDictionary(x => x.FamilyId, x => x.FamilyName);
        }
        return _familyLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetSubfamilyLookupAsync()
    {
        if (_subfamilyLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubfamilyAsync();
            _subfamilyLookupCache = list.ToDictionary(x => x.SubfamilyId, x => x.SubfamilyName);
        }
        return _subfamilyLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetInfrafamilyLookupAsync()
    {
        if (_infrafamilyLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfrafamilyAsync();
            _infrafamilyLookupCache = list.ToDictionary(x => x.InfrafamilyId, x => x.InfrafamilyName);
        }
        return _infrafamilyLookupCache;
    }
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
            var list = await data.GetAllRefAuthorAsync();
            _refAuthorLookupCache = list.ToDictionary(x => x.RefAuthorId, x => x.RefAuthorName);
        }
        return _refAuthorLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetRefExpertLookupAsync()
    {
        if (_refExpertLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefExpertAsync();
            _refExpertLookupCache = list.ToDictionary(x => x.RefExpertId, x => x.RefExpertName);
        }
        return _refExpertLookupCache;
    }
    public async Task<Dictionary<int, string?>> GetRefSourceLookupAsync()
    {
        if (_refSourceLookupCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefSourceAsync();
            _refSourceLookupCache = list.ToDictionary(x => x.RefSourceId, x => x.RefSourceName);
        }
        return _refSourceLookupCache;
    }

    //--------------------------------------------------------------------
    public async Task<ObservableCollection<Models.Tbl03Regnum>> GetRegnumListAsync()
    {
        if (_regnumListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRegnumAsync();
            _regnumListCache = new ObservableCollection<Models.Tbl03Regnum>(list);
        }
        return _regnumListCache;
    }
    public async Task<ObservableCollection<Models.Tbl06Phylum>> GetPhylumListAsync()
    {
        if (_phylumListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllPhylumAsync();
            _phylumListCache = new ObservableCollection<Models.Tbl06Phylum>(list);
        }
        return _phylumListCache;
    }
    public async Task<ObservableCollection<Models.Tbl12Subphylum>> GetSubphylumListAsync()
    {
        if (_subphylumListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubphylumAsync();
            _subphylumListCache = new ObservableCollection<Models.Tbl12Subphylum>(list);
        }
        return _subphylumListCache;
    }
    public async Task<ObservableCollection<Models.Tbl18Superclass>> GetSuperclassListAsync()
    {
        if (_superclassListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSuperclassAsync();
            _superclassListCache = new ObservableCollection<Models.Tbl18Superclass>(list);
        }
        return _superclassListCache;
    }
    public async Task<ObservableCollection<Models.Tbl21Class>> GetClassListAsync()
    {
        if (_classListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllClassAsync();
            _classListCache = new ObservableCollection<Models.Tbl21Class>(list);
        }
        return _classListCache;
    }
    public async Task<ObservableCollection<Models.Tbl24Subclass>> GetSubclassListAsync()
    {
        if (_subclassListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubclassAsync();
            _subclassListCache = new ObservableCollection<Models.Tbl24Subclass>(list);
        }
        return _subclassListCache;
    }
    public async Task<ObservableCollection<Models.Tbl27Infraclass>> GetInfraclassListAsync()
    {
        if (_infraclassListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfraclassAsync();
            _infraclassListCache = new ObservableCollection<Models.Tbl27Infraclass>(list);
        }
        return _infraclassListCache;
    }
    public async Task<ObservableCollection<Models.Tbl30Legio>> GetLegioListAsync()
    {
        if (_legioListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllLegioAsync();
            _legioListCache = new ObservableCollection<Models.Tbl30Legio>(list);
        }
        return _legioListCache;
    }
    public async Task<ObservableCollection<Models.Tbl33Ordo>> GetOrdoListAsync()
    {
        if (_ordoListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllOrdoAsync();
            _ordoListCache = new ObservableCollection<Models.Tbl33Ordo>(list);
        }
        return _ordoListCache;
    }
    public async Task<ObservableCollection<Models.Tbl36Subordo>> GetSubordoListAsync()
    {
        if (_subordoListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubordoAsync();
            _subordoListCache = new ObservableCollection<Models.Tbl36Subordo>(list);
        }
        return _subordoListCache;
    }
    public async Task<ObservableCollection<Models.Tbl39Infraordo>> GetInfraordoListAsync()
    {
        if (_infraordoListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfraordoAsync();
            _infraordoListCache = new ObservableCollection<Models.Tbl39Infraordo>(list);
        }
        return _infraordoListCache;
    }
    public async Task<ObservableCollection<Models.Tbl42Superfamily>> GetSuperfamilyListAsync()
    {
        if (_superfamilyListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSuperfamilyAsync();
            _superfamilyListCache = new ObservableCollection<Models.Tbl42Superfamily>(list);
        }
        return _superfamilyListCache;
    }
    public async Task<ObservableCollection<Models.Tbl45Family>> GetFamilyListAsync()
    {
        if (_familyListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllFamilyAsync();
            _familyListCache = new ObservableCollection<Models.Tbl45Family>(list);
        }
        return _familyListCache;
    }
    public async Task<ObservableCollection<Models.Tbl48Subfamily>> GetSubfamilyListAsync()
    {
        if (_subfamilyListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllSubfamilyAsync();
            _subfamilyListCache = new ObservableCollection<Models.Tbl48Subfamily>(list);
        }
        return _subfamilyListCache;
    }
    public async Task<ObservableCollection<Models.Tbl51Infrafamily>> GetInfrafamilyListAsync()
    {
        if (_infrafamilyListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllInfrafamilyAsync();
            _infrafamilyListCache = new ObservableCollection<Models.Tbl51Infrafamily>(list);
        }
        return _infrafamilyListCache;
    }
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
            var list = await data.GetAllRefAuthorAsync();
            _refAuthorListCache = new ObservableCollection<Models.Tbl90RefAuthor>(list);
        }
        return _refAuthorListCache;
    }
    public async Task<ObservableCollection<Models.Tbl90RefExpert>> GetRefExpertListAsync()
    {
        if (_refExpertListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefExpertAsync();
            _refExpertListCache = new ObservableCollection<Models.Tbl90RefExpert>(list);
        }
        return _refExpertListCache;
    }
    public async Task<ObservableCollection<Models.Tbl90RefSource>> GetRefSourceListAsync()
    {
        if (_refSourceListCache == null)
        {
            var data = CreateDataService();
            var list = await data.GetAllRefSourceAsync();
            _refSourceListCache = new ObservableCollection<Models.Tbl90RefSource>(list);
        }
        return _refSourceListCache;
    }
}
