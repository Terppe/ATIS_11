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

        }
    }
}
