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

        Tables = new() { "Species", "Class", "Superclass", "Subphylum", "Phylum", "Regnum" };
        SelectedTable = Tables.First();

        SearchCommand = new AsyncRelayCommand(ExecuteSearchAsync);
    }

    public List<string> Tables { get; }
    public string SelectedTable { get; set; }

    public string SearchText { get; set; } = string.Empty;

    public ObservableCollection<object> Results { get; } = new();

    public IAsyncRelayCommand SearchCommand { get; }

    private async Task ExecuteSearchAsync()
    {
        Results.Clear();

        switch (SelectedTable)
        {
            case "Superclass":
                var superclassResults = await _dataService.SearchSuperclassWithSubphylumAsync(SearchText);
                foreach (var item in superclassResults)
                    Results.Add(item);
                break;
            case "Class":
                var classResults = await _dataService.SearchClassWithSuperclassAsync(SearchText);
                foreach (var item in classResults)
                    Results.Add(item);
                break;

        }
    }
}
