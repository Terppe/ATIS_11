using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Models;
using QuestPDF.Infrastructure;
using ReportPdf = ATIS.Views.Report.Pdf;
using SearchPdf = ATIS.Views.Search.Pdf;

namespace ATIS.Views.Search;

public sealed class SearchPageViewModel : ObservableObject
{
    private readonly IDataService _dataService;
    private int _searchVersion;

    public SearchPageViewModel(IDataService dataService)
    {
        _dataService = dataService;

        Tables = new()
        {
            "Species", "Genus", "Infratribus", "Subtribus", "Tribus", "Supertribus", "Infrafamily",
            "Subfamily", "Family", "Superfamily", "Infraordo", "Subordo", "Ordo", "Legio", "Infraclass", "Subclass", "Class",
            "Superclass", "Subphylum", "Phylum", "Regnum", "Disease", "Food", "Miscellaneous"
        };

        // nur Default, wenn wirklich noch nichts gesetzt
        SelectedTable ??= "Genus";

        SearchCommand = new AsyncRelayCommand(ExecuteSearchAsync);
        Results.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CanPrintList));
    }

    private string? _selectedTable;
    public string? SelectedTable
    {
        get => _selectedTable;
        set => SetProperty(ref _selectedTable, value);
    }

    public bool CanPrintList => Results.Count > 0;

    public List<string> Tables { get; }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public ObservableCollection<object> Results { get; } = new();

    public IAsyncRelayCommand SearchCommand { get; }

    public void ClearResults()
    {
        _searchVersion++;
        Results.Clear();
    }

    private async Task ExecuteSearchAsync()
    {
        ClearResults();
        var searchVersion = _searchVersion;
        var searchText = SearchText;
        var results = new List<object>();

        switch (SelectedTable)
        {
            case "Regnum":
                {
                    var items = await _dataService.SearchRegnumAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.RegnumName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Phylum":
                {
                    var items = await _dataService.SearchPhylumWithRegnumAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.PhylumName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Subphylum":
                {
                    var items = await _dataService.SearchSubphylumWithPhylumAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SubphylumName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Superclass":
                {
                    var items = await _dataService.SearchSuperclassWithSubphylumAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SuperclassName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Class":
                {
                    var items = await _dataService.SearchClassWithSuperclassAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.ClassName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Subclass":
                {
                    var items = await _dataService.SearchSubclassWithClassAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SubclassName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Infraclass":
                {
                    var items = await _dataService.SearchInfraclassWithSubclassAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.InfraclassName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Legio":
                {
                    var items = await _dataService.SearchLegioWithInfraclassAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.LegioName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Ordo":
                {
                    var items = await _dataService.SearchOrdoWithLegioAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.OrdoName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Subordo":
                {
                    var items = await _dataService.SearchSubordoWithOrdoAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SubordoName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Infraordo":
                {
                    var items = await _dataService.SearchInfraordoWithSubordoAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.InfraordoName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Superfamily":
                {
                    var items = await _dataService.SearchSuperfamilyWithInfraordoAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SuperfamilyName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Family":
                {
                    var items = await _dataService.SearchFamilyWithSuperfamilyAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.FamilyName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Subfamily":
                {
                    var items = await _dataService.SearchSubfamilyWithFamilyAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SubfamilyName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Infrafamily":
                {
                    var items = await _dataService.SearchInfrafamilyWithSubfamilyAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.InfrafamilyName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Supertribus":
                {
                    var items = await _dataService.SearchSupertribusWithInfrafamilyAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SupertribusName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Tribus":
                {
                    var items = await _dataService.SearchTribusWithSupertribusAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.TribusName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Subtribus":
                {
                    var items = await _dataService.SearchSubtribusWithTribusAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SubtribusName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Infratribus":
                {
                    var items = await _dataService.SearchInfratribusWithSubtribusAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.InfratribusName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Genus":
                {
                    var items = await _dataService.SearchGenusWithInfratribusAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.GenusName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Species":
                {
                    var items = await _dataService.GetSpeciesWithGenusAndSpeciesgroupByIdAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.SpeciesName.Contains('#')) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Disease":
                {
                    var items = await _dataService.SearchDiseaseAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.DiseaseName?.Contains('#') == true) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Food":
                {
                    var items = await _dataService.SearchFoodAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.FoodName?.Contains('#') == true) continue;
                        results.Add(item);
                    }
                    break;
                }
            case "Miscellaneous":
                {
                    var items = await _dataService.SearchMiscellaneousAsync(searchText);
                    foreach (var item in items)
                    {
                        if (item.MiscellaneousName?.Contains('#') == true) continue;
                        results.Add(item);
                    }
                    break;
                }
        }

        if (searchVersion != _searchVersion)
            return;

        foreach (var item in results)
            Results.Add(item);
    }

    public async Task PrintListAsync(IList<object> visibleItems)
    {
        ArgumentNullException.ThrowIfNull(visibleItems);
        if (visibleItems.Count == 0)
            return;

        var items = visibleItems.ToList();
        var selectedTable = SelectedTable;
        var tableName = PdfExportService.SanitizeFileNamePart(SelectedTable, "Search");
        var searchText = PdfExportService.SanitizeFileNamePart(SearchText, "All");
        var suggestedFileName = $"{tableName}_List_{searchText}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(
            () => Task.FromResult(CreateSearchListDocument(items, selectedTable)),
            suggestedFileName);
    }

    public IDocument? CreateSearchListDocument(IList<object> visibleItems)
        => CreateSearchListDocument(visibleItems, SelectedTable);

    private static IDocument? CreateSearchListDocument(
        IList<object> visibleItems,
        string? selectedTable)
    {
        ArgumentNullException.ThrowIfNull(visibleItems);
        if (visibleItems.Count == 0)
            return null;

        return selectedTable switch
        {
            "Regnum" => CreateListDocument<Tbl03Regnum>(visibleItems, items =>
                new SearchPdf.Tbl03Regnums.Tbl03RegnumsListDocument(new ReportPdf.Tbl03Regnums.Tbl03RegnumsPdfModel(), items)),
            "Phylum" => CreateListDocument<Tbl06Phylum>(visibleItems, items =>
                new SearchPdf.Tbl06Phylums.Tbl06PhylumsListDocument(new ReportPdf.Tbl06Phylums.Tbl06PhylumsPdfModel(), items)),
            "Subphylum" => CreateListDocument<Tbl12Subphylum>(visibleItems, items =>
                new SearchPdf.Tbl12Subphylums.Tbl12SubphylumsListDocument(new ReportPdf.Tbl12Subphylums.Tbl12SubphylumsPdfModel(), items)),
            "Superclass" => CreateListDocument<Tbl18Superclass>(visibleItems, items =>
                new SearchPdf.Tbl18Superclasses.Tbl18SuperclassesListDocument(new ReportPdf.Tbl18Superclasses.Tbl18SuperclassesPdfModel(), items)),
            "Class" => CreateListDocument<Tbl21Class>(visibleItems, items =>
                new SearchPdf.Tbl21Classes.Tbl21ClassesListDocument(items)),
            "Subclass" => CreateListDocument<Tbl24Subclass>(visibleItems, items =>
                new SearchPdf.Tbl24Subclasses.Tbl24SubclassesListDocument(new ReportPdf.Tbl24Subclasses.Tbl24SubclassesPdfModel(), items)),
            "Infraclass" => CreateListDocument<Tbl27Infraclass>(visibleItems, items =>
                new SearchPdf.Tbl27Infraclasses.Tbl27InfraclassesListDocument(new ReportPdf.Tbl27Infraclasses.Tbl27InfraclassesPdfModel(), items)),
            "Legio" => CreateListDocument<Tbl30Legio>(visibleItems, items =>
                new SearchPdf.Tbl30Legios.Tbl30LegiosListDocument(new ReportPdf.Tbl30Legios.Tbl30LegiosPdfModel(), items)),
            "Ordo" => CreateListDocument<Tbl33Ordo>(visibleItems, items =>
                new SearchPdf.Tbl33Ordos.Tbl33OrdosListDocument(new ReportPdf.Tbl33Ordos.Tbl33OrdosPdfModel(), items)),
            "Subordo" => CreateListDocument<Tbl36Subordo>(visibleItems, items =>
                new SearchPdf.Tbl36Subordos.Tbl36SubordosListDocument(new ReportPdf.Tbl36Subordos.Tbl36SubordosPdfModel(), items)),
            "Infraordo" => CreateListDocument<Tbl39Infraordo>(visibleItems, items =>
                new SearchPdf.Tbl39Infraordos.Tbl39InfraordosListDocument(new ReportPdf.Tbl39Infraordos.Tbl39InfraordosPdfModel(), items)),
            "Superfamily" => CreateListDocument<Tbl42Superfamily>(visibleItems, items =>
                new SearchPdf.Tbl42Superfamilies.Tbl42SuperfamiliesListDocument(new ReportPdf.Tbl42Superfamilies.Tbl42SuperfamiliesPdfModel(), items)),
            "Family" => CreateListDocument<Tbl45Family>(visibleItems, items =>
                new SearchPdf.Tbl45Families.Tbl45FamiliesListDocument(new ReportPdf.Tbl45Families.Tbl45FamiliesPdfModel(), items)),
            "Subfamily" => CreateListDocument<Tbl48Subfamily>(visibleItems, items =>
                new SearchPdf.Tbl48Subfamilies.Tbl48SubfamiliesListDocument(new ReportPdf.Tbl48Subfamilies.Tbl48SubfamiliesPdfModel(), items)),
            "Infrafamily" => CreateListDocument<Tbl51Infrafamily>(visibleItems, items =>
                new SearchPdf.Tbl51Infrafamilies.Tbl51InfrafamiliesListDocument(new ReportPdf.Tbl51Infrafamilies.Tbl51InfrafamiliesPdfModel(), items)),
            "Supertribus" => CreateListDocument<Tbl54Supertribus>(visibleItems, items =>
                new SearchPdf.Tbl54Supertribusses.Tbl54SupertribussesListDocument(new ReportPdf.Tbl54Supertribusses.Tbl54SupertribussesPdfModel(), items)),
            "Tribus" => CreateListDocument<Tbl57Tribus>(visibleItems, items =>
                new SearchPdf.Tbl57Tribusses.Tbl57TribussesListDocument(new ReportPdf.Tbl57Tribusses.Tbl57TribussesPdfModel(), items)),
            "Subtribus" => CreateListDocument<Tbl60Subtribus>(visibleItems, items =>
                new SearchPdf.Tbl60Subtribusses.Tbl60SubtribussesListDocument(new ReportPdf.Tbl60Subtribusses.Tbl60SubtribussesPdfModel(), items)),
            "Infratribus" => CreateListDocument<Tbl63Infratribus>(visibleItems, items =>
                new SearchPdf.Tbl63Infratribusses.Tbl63InfratribussesListDocument(new ReportPdf.Tbl63Infratribusses.Tbl63InfratribussesPdfModel(), items)),
            "Genus" => CreateListDocument<Tbl66Genus>(visibleItems, items =>
                new SearchPdf.Tbl66Genusses.Tbl66GenussesListDocument(new ReportPdf.Tbl66Genusses.Tbl66GenussesPdfModel(), items)),
            "Species" => CreateListDocument<Tbl75Species>(visibleItems, items =>
                new SearchPdf.Tbl75Speciesses.Tbl75SpeciessesListDocument(new ReportPdf.Tbl75Speciesses.Tbl75SpeciessesPdfModel(), items)),
            "Disease" => CreateListDocument<TblDisease>(visibleItems, items =>
                new SearchPdf.TblDisease.TblDiseasesListDocument(new ReportPdf.TblDisease.TblDiseasesPdfModel(), items)),
            "Food" => CreateListDocument<TblFood>(visibleItems, items =>
                new SearchPdf.TblFoods.TblFoodsListDocument(new ReportPdf.TblFoods.TblFoodsPdfModel(), items)),
            "Miscellaneous" => CreateListDocument<TblMiscellaneous>(visibleItems, items =>
                new SearchPdf.TblMiscellaneous.TblMiscellaneoussesListDocument(new ReportPdf.TblMiscellaneous.TblMiscellaneoussesPdfModel(), items)),
            _ => throw new InvalidOperationException($"Unsupported search table: {selectedTable}")
        };
    }

    private static IDocument CreateListDocument<T>(
        IEnumerable<object> visibleItems,
        Func<ObservableCollection<T>, IDocument> createDocument)
    {
        return createDocument(new ObservableCollection<T>(visibleItems.Cast<T>()));
    }
}
