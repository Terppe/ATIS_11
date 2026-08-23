using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Search.DetailPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using WinRT.Interop;
using Windows.Storage.Pickers;

namespace ATIS.Views.Search;

public sealed partial class SearchPage : Page
{
    private readonly IDataService _dataService;

    public SearchPageViewModel ViewModel { get; }

    public SearchPage()
    {
        InitializeComponent();
        DataContext = this;

        var factory = App.Services.GetRequiredService<IDbContextFactory<AtisDbContext>>();
        var user = App.Services.GetRequiredService<IUserService>();
        _dataService = new DataService(factory, user);

        // ⭐ WICHTIG: ViewModel erzeugen und DataService übergeben
        ViewModel = new SearchPageViewModel(_dataService);
        Loaded += SearchPage_Loaded;

    }
    private bool _isLoaded;

    private async void SearchPage_Loaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
        await Task.Delay(50);
        TableCombo.Focus(FocusState.Keyboard);
    }

    private void OnTableSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // alte Trefferliste löschen, damit z.B. Subclass -> Phylum sauber ist
        ViewModel.Results?.Clear();

        // optional: Suchfeld leeren
        // ViewModel.SearchText = string.Empty;

        SearchBox.Focus(FocusState.Programmatic);
    }
    private async void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
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
                App.MainPageInstance.NavFrame.Navigate(
                    typeof(Tbl75SpeciesDetailPage), species.SpeciesId);
                break;
        }
        System.Diagnostics.Debug.WriteLine(e.AddedItems[0]?.GetType().FullName);
        ResultList.SelectedItem = null;
    }

    //private async void OnPrintListClick(object sender, RoutedEventArgs e)
    //{
    //    var visibleItems = ResultList.Items.Cast<object>().ToList();
    //    await ViewModel.PrintListAsync(visibleItems);
    //}

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

        var document = ViewModel.CreateSearchListDocument(visibleItems);
        if (document == null)
            return;

        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

        var tableName = string.IsNullOrWhiteSpace(ViewModel.SelectedTable)
            ? "Search"
            : ViewModel.SelectedTable;

        var searchText = SanitizeFileNamePart(ViewModel.SearchText);

        savePicker.SuggestedFileName = $"{tableName}_List_{searchText}_{DateTime.Now:yyyyMMdd_HHmmss}";
        savePicker.FileTypeChoices.Add("PDF Document", new List<string> { ".pdf" });

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();
        if (file == null)
            return;

        document.GeneratePdf(file.Path);
    }
}
