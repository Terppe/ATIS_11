using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Search.DetailPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Search;

public sealed record SearchPageNavigationRequest(
    string Table,
    string? SearchText = null);

public sealed partial class SearchPage : Page
{
    private static readonly (string Header, string DefaultTable, string[] Tables)[] TableGroups =
    [
        ("Species", "Species", ["Species"]),
        ("Genus", "Genus", ["Genus"]),
        ("Tribus", "Tribus", ["Infratribus", "Subtribus", "Tribus", "Supertribus"]),
        ("Families", "Family", ["Infrafamily", "Subfamily", "Family", "Superfamily"]),
        ("Order", "Ordo", ["Infraordo", "Subordo", "Ordo", "Legio"]),
        ("Class", "Class", ["Infraclass", "Subclass", "Class", "Superclass"]),
        ("Phylum + Regnum", "Phylum", ["Subphylum", "Phylum", "Regnum"]),
        ("Disease", "Disease", ["Disease"]),
        ("Food", "Food", ["Food"]),
        ("Miscellaneous", "Miscellaneous", ["Miscellaneous"])
    ];
    private readonly IDataService _dataService;
    private readonly List<(Button Button, Border Underline)> _groupTabs = [];
    private readonly List<(Button Button, Border Underline)> _tableTabs = [];
    private int _selectedGroupIndex = 1;
    private bool _focusSearchBox;
    public SearchPageViewModel ViewModel { get; }
    public SearchPage()
    {
        var factory = App.Services.GetRequiredService<IDbContextFactory<AtisDbContext>>();
        var user = App.Services.GetRequiredService<IUserService>();
        _dataService = new DataService(factory, user);
        ViewModel = new SearchPageViewModel(_dataService);
        InitializeComponent();
        DataContext = this;
        CreateGroupTabs();
        SelectGroup(_selectedGroupIndex);
        Loaded += SearchPage_Loaded;
    }
    private void SearchPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_focusSearchBox)
            SearchBox.Focus(FocusState.Programmatic);
        else
            _groupTabs[_selectedGroupIndex].Button.Focus(FocusState.Keyboard);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var request = e.Parameter as SearchPageNavigationRequest;

        _focusSearchBox = e.NavigationMode == NavigationMode.New &&
                          (request is not null
                              ? string.IsNullOrWhiteSpace(request.SearchText)
                              : e.Parameter is not string);

        // Beim Zurücknavigieren bleiben Suchgruppe und Ergebnisse erhalten.
        if (e.NavigationMode != NavigationMode.New)
            return;

        var selectedGroupIndex = request is null
            ? 1
            : Array.FindIndex(TableGroups, group =>
                group.Tables.Contains(
                    request.Table,
                    StringComparer.OrdinalIgnoreCase));

        if (selectedGroupIndex < 0)
            selectedGroupIndex = 1;

        SelectGroup(selectedGroupIndex);

        if (request is not null)
        {
            ViewModel.SelectedTable =
                TableGroups[selectedGroupIndex].Tables.FirstOrDefault(table =>
                    string.Equals(
                        table,
                        request.Table,
                        StringComparison.OrdinalIgnoreCase))
                ?? TableGroups[selectedGroupIndex].DefaultTable;

            ViewModel.SearchText = request.SearchText ?? string.Empty;
            UpdateTableTabStates();
        }
        else if (e.Parameter is string searchText)
        {
            ViewModel.SearchText = searchText;
        }

        await SearchSelectedTableAsync();
    }

    private (Button Button, Border Underline) CreateTab(string header, string tag, RoutedEventHandler onClick)
    {
        var underline = new Border
        {
            Height = 2,
            Background = new SolidColorBrush(Colors.Transparent)
        };
        var content = new StackPanel { Spacing = 4 };
        content.Children.Add(new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = header
        });
        content.Children.Add(underline);
        var button = new Button
        {
            Content = content,
            Style = (Style)Application.Current.Resources["TabButtonStyle"],
            Tag = tag
        };
        button.Click += onClick;
        return (button, underline);
    }
    private void CreateGroupTabs()
    {
        foreach (var group in TableGroups)
        {
            var tab = CreateTab(group.Header, group.Header, OnGroupTabClick);
            _groupTabs.Add(tab);
            GroupTabsPanel.Children.Add(tab.Button);
        }
    }
    private void SelectGroup(int index)
    {
        _selectedGroupIndex = index;
        var group = TableGroups[index];
        ViewModel.ClearResults();
        ViewModel.SelectedTable = group.DefaultTable;
        _tableTabs.Clear();
        TableTabsPanel.Children.Clear();
        TableTabsScrollViewer.Visibility = group.Tables.Length > 1 ? Visibility.Visible : Visibility.Collapsed;
        foreach (var table in group.Tables)
        {
            var tab = CreateTab(table, table, OnTableTabClick);
            _tableTabs.Add(tab);
            TableTabsPanel.Children.Add(tab.Button);
        }
        UpdateTableTabStates();
    }
    private async void OnGroupTabClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string header })
            return;
        SelectGroup(Array.FindIndex(TableGroups, group => group.Header == header));
        await SearchSelectedTableAsync();
    }
    private async void OnTableTabClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string table })
            return;
        ViewModel.ClearResults();
        ViewModel.SelectedTable = table;
        UpdateTableTabStates();
        await SearchSelectedTableAsync();
    }
    private void UpdateTableTabStates()
    {
        foreach (var (button, underline) in _groupTabs)
            SetTabButtonStyle(button, underline, Equals(button.Tag, TableGroups[_selectedGroupIndex].Header));
        foreach (var (button, underline) in _tableTabs)
            SetTabButtonStyle(button, underline, Equals(button.Tag, ViewModel.SelectedTable));
    }
    private static void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    {
        button.Background = new SolidColorBrush(Colors.Transparent);
        button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
        underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    }
    private async Task SearchSelectedTableAsync()
    {
        if (!string.IsNullOrWhiteSpace(ViewModel.SearchText))
            await ViewModel.SearchCommand.ExecuteAsync(null);
    }
    private async void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel.SearchText = args.QueryText;
        await ViewModel.SearchCommand.ExecuteAsync(null);
    }
    private void OnRowSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0)
            return;
        var item = e.AddedItems[0];
        switch (item)
        {
            case Tbl03Regnum regnum:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl03RegnumDetailPage), regnum.RegnumId);
                break;
            case Tbl06Phylum phylum:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl06PhylumDetailPage), phylum.PhylumId);
                break;
            case Tbl12Subphylum subphylum:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl12SubphylumDetailPage), subphylum.SubphylumId);
                break;
            case Tbl18Superclass superclass:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl18SuperclassDetailPage), superclass.SuperclassId);
                break;
            case Tbl21Class class1:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl21ClassDetailPage), class1.ClassId);
                break;
            case Tbl24Subclass subclass:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl24SubclassDetailPage), subclass.SubclassId);
                break;
            case Tbl27Infraclass infraclass:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl27InfraclassDetailPage), infraclass.InfraclassId);
                break;
            case Tbl30Legio legio:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl30LegioDetailPage), legio.LegioId);
                break;
            case Tbl33Ordo ordo:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl33OrdoDetailPage), ordo.OrdoId);
                break;
            case Tbl36Subordo subordo:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl36SubordoDetailPage), subordo.SubordoId);
                break;
            case Tbl39Infraordo infraordo:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl39InfraordoDetailPage), infraordo.InfraordoId);
                break;
            case Tbl42Superfamily superfamily:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl42SuperfamilyDetailPage), superfamily.SuperfamilyId);
                break;
            case Tbl45Family family:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl45FamilyDetailPage), family.FamilyId);
                break;
            case Tbl48Subfamily subfamily:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl48SubfamilyDetailPage), subfamily.SubfamilyId);
                break;
            case Tbl51Infrafamily infrafamily:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl51InfrafamilyDetailPage), infrafamily.InfrafamilyId);
                break;
            case Tbl54Supertribus supertribus:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl54SupertribusDetailPage), supertribus.SupertribusId);
                break;
            case Tbl57Tribus tribus:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl57TribusDetailPage), tribus.TribusId);
                break;
            case Tbl60Subtribus subtribus:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl60SubtribusDetailPage), subtribus.SubtribusId);
                break;
            case Tbl63Infratribus infratribus:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl63InfratribusDetailPage), infratribus.InfratribusId);
                break;
            case Tbl66Genus genus:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl66GenusDetailPage), genus.GenusId);
                break;
            case Tbl75Species species:
                if (species.IsSubspecies)
                {
                    App.MainPageInstance.NavFrame.Navigate(
                        typeof(Tbl75SpeciesSubDetailPage), species.SpeciesId);
                }
                else
                {
                    App.MainPageInstance.NavFrame.Navigate(
                        typeof(Tbl75SpeciesDetailPage), species.SpeciesId);
                }
                break;
            case TblDisease disease:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(TblDiseaseDetailPage), disease.DiseaseId);
                break;

            case TblFood food:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(TblFoodDetailPage), food.FoodId);
                break;

            case TblMiscellaneous miscellaneous:
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(TblMiscellaneousDetailPage), miscellaneous.MiscellaneousId);
                break;
        }
        System.Diagnostics.Debug.WriteLine(e.AddedItems[0]?.GetType().FullName);
        ResultList.SelectedItem = null;
    }
    private static string SanitizeFileNamePart(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "All";
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value
            .Where(c => !invalidChars.Contains(c))
            .ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "All" : cleaned.Trim();
    }
    private async void OnPrintListClick(object sender, RoutedEventArgs e)
    {
        var visibleItems = ResultList.Items.Cast<object>().ToList();

        await ViewModel.PrintListAsync(visibleItems);
    }
}
