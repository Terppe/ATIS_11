using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;

namespace ATIS.Views.Search;

using CommunityToolkit.Mvvm.Input;

public sealed class SearchPageViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    public SearchPageViewModel(IDataService dataService)
    {
        _dataService = dataService;

        Tables = new()
        {
            "Species", "Genus", "Infratribus", "Subtribus", "Tribus", "Supertribus", "Infrafamily",
            "Subfamily", "Family", "Superfamily", "Infraordo", "Subordo", "Ordo", "Legio", "Infraclass", "Subclass", "Class",
            "Superclass", "Subphylum", "Phylum", "Regnum" 
        };

        // nur Default, wenn wirklich noch nichts gesetzt
        SelectedTable ??= "Species";

        SearchCommand = new AsyncRelayCommand(ExecuteSearchAsync);
    }

    public string? SelectedTable
    {
        get => _selectedTable;
        set => SetProperty(ref _selectedTable, value);
    }
    private string? _selectedTable;

    public List<string> Tables { get; }

    public string SearchText { get; set; } = string.Empty;

    public ObservableCollection<object> Results { get; } = new();

    public IAsyncRelayCommand SearchCommand { get; }

    private async Task ExecuteSearchAsync()
    {
        Results.Clear();

        switch (SelectedTable)
        {
            case "Regnum":
                {
                    var items = await _dataService.SearchRegnumAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.RegnumName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }

            case "Phylum":
                {
                    var items = await _dataService.SearchPhylumWithRegnumAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.PhylumName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }

            case "Subphylum":
                {
                    var items = await _dataService.SearchSubphylumWithPhylumAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.SubphylumName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }

            case "Superclass":
                {
                    var items = await _dataService.SearchSuperclassWithSubphylumAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.SuperclassName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }

            case "Class":
                {
                    var items = await _dataService.SearchClassWithSuperclassAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.ClassName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }

            case "Subclass":
                {
                    var items = await _dataService.SearchSubclassWithClassAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.SubclassName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }
            case "Infraclass":
                {
                    var items = await _dataService.SearchInfraclassWithSubclassAsync(SearchText);
                    foreach (var item in items)
                    {
                        if (item.InfraclassName.Contains('#')) continue;
                        Results.Add(item);
                    }
                    break;
                }
            case "Legio":
            {
                var items = await _dataService.SearchLegioWithInfraclassAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.LegioName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Ordo":
            {
                var items = await _dataService.SearchOrdoWithLegioAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.OrdoName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Subordo":
            {
                var items = await _dataService.SearchSubordoWithOrdoAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.SubordoName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Infraordo":
            {
                var items = await _dataService.SearchInfraordoWithSubordoAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.InfraordoName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Superfamily":
            {
                var items = await _dataService.SearchSuperfamilyWithInfraordoAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.SuperfamilyName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Family":
            {
                var items = await _dataService.SearchFamilyWithSuperfamilyAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.FamilyName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Subfamily":
            {
                var items = await _dataService.SearchSubfamilyWithFamilyAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.SubfamilyName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Infrafamily":
            {
                var items = await _dataService.SearchInfrafamilyWithSubfamilyAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.InfrafamilyName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Supertribus":
            {
                var items = await _dataService.SearchSupertribusWithInfrafamilyAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.SupertribusName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Tribus":
            {
                var items = await _dataService.SearchTribusWithSupertribusAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.TribusName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Subtribus":
            {
                var items = await _dataService.SearchSubtribusWithTribusAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.SubtribusName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Infratribus":
            {
                var items = await _dataService.SearchInfratribusWithSubtribusAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.InfratribusName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Genus":
            {
                var items = await _dataService.SearchGenusWithInfratribusAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.GenusName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }
            case "Species":
            {
                var items = await _dataService.SearchSpeciesWithGenusAsync(SearchText);
                foreach (var item in items)
                {
                    if (item.SpeciesName.Contains('#')) continue;
                    Results.Add(item);
                }
                break;
            }

        }
    }
}
