using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl03Regnums;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;
using QuestPDF.Fluent;
using WinUI3Localizer;

//    Tbl03RegnumDetailViewModel Skriptdatum:  24.08.2026  12:32      

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl03RegnumDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Regnum: {RegnumSelected?.RegnumName}";
    // -----------------------------
    //  SELECTED Regnum
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; }
    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl06Phylum? PhylumSelected { get; private set; }
    // -----------------------------
    //  Breadcrumb
    // -----------------------------
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToRegnumCommand { get; }
    // -----------------------------
    //  COLLECTIONS FOR LISTVIEWS
    // -----------------------------
    public ObservableCollection<TaxonomyLine<Tbl03Regnum>> RegnumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl06Phylum>> PhylumItems { get; } = new();

    // -----------------------------
    //  REFERENCES
    // -----------------------------
    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = new();
    public ObservableCollection<BreadcrumbItem> Breadcrumb { get; set; } = new();

    // -----------------------------
    //  COMMENTS
    // -----------------------------
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = new();

    // -----------------------------
    //  COMMANDS
    // -----------------------------
    public IAsyncRelayCommand ExportPdfCommand { get; }

    // ⭐ NEU: BackCommand
    public IRelayCommand BackCommand { get; }

    // -----------------------------
    //  CONSTRUCTOR
    // -----------------------------
    public Tbl03RegnumDetailViewModel(Tbl03Regnum selectedRegnum, IDataService? dataService)
    {
        RegnumSelected = selectedRegnum;
        _dataService = dataService;

        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);

        // ⭐ BackCommand initialisieren
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }
    private bool _isInitialized;
    private bool _isInitializing;
    public async Task InitializeAsync()
    {
        if (_isInitialized || _isInitializing) return;
        _isInitializing = true;
        try
        {
            await LoadTaxonomy();
            LoadDirectChildren();
            LoadReferences();
            LoadComments();
            BuildBreadcrumb();
            _isInitialized = true;
        }
        finally
        {
            _isInitializing = false;
        }
    }

    // -----------------------------
    //  ISLOADING PROPERTY
    // -----------------------------
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // -----------------------------
    //  BREADCRUMB
    // -----------------------------
    public IEnumerable<BreadcrumbItem> BreadcrumbReversed => Breadcrumb.Reverse();

    private void BuildBreadcrumb()
    {
        Breadcrumb.Clear();

        Breadcrumb.Add(new BreadcrumbItem
        {
            Label = RegnumSelected.RegnumName,
            EntityType = "Regnum",
            EntityId = RegnumSelected.RegnumId
        });


        // <<< HIER: aktuellen Knoten entfernen
        for (int i = Breadcrumb.Count - 1; i >= 0; i--)
        {
            var x = Breadcrumb[i];
            if (x.EntityType == "Regnum" && x.EntityId == RegnumSelected.RegnumId)
                Breadcrumb.RemoveAt(i);
        }
    }

    public async Task NavigateBreadcrumbAsync(BreadcrumbItem item)
    {
        switch (item.EntityType)
        {
            case "Regnum":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl03RegnumDetailPage), item.EntityId);
                break;

            case "Phylum":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl06PhylumDetailPage), item.EntityId);
                break;

        }
    }

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        var frame = App.MainPageInstance?.NavFrame;
        if (frame?.CanGoBack == true)
            frame.GoBack();
    }

    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    private static Thickness CreateHeaderMargin(int visibleIndex)
    {
        return new Thickness(8 + (visibleIndex * 8), 0, 0, 0);
    }

    // -----------------------------
    //  LOAD TAXONOMY
    // -----------------------------
    private async Task LoadTaxonomy()
    {
        RegnumItems.Clear();
        // sichtbare Reihenfolge aufbauen

        var visibleIndex = 0;

        RegnumItems.Add(new TaxonomyLine<Tbl03Regnum>
        {
            Item = RegnumSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (Phylum)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async void LoadDirectChildren()
    {
        PhylumItems.Clear();
        HasDirectChildren = false;

        var phylum = await _dataService.GetPhylumsByRegnumIdAsync(RegnumSelected.RegnumId);

        var visibleItems = phylum
            .Where(x => IsVisibleTaxonName(x.PhylumName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = RegnumItems.Count > 0 ? RegnumItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            PhylumItems.Add(new TaxonomyLine<Tbl06Phylum>
            {
                Item = item,
                HeaderMargin = childMargin,
                ShowAsHyperlink = false
            });
        }

        HasDirectChildren = true;
    }

    // -----------------------------
    //  LOAD REFERENCES
    // -----------------------------
    private async void LoadReferences()
    {
        // -------------------------
        // EXPERT REFERENCES
        // -------------------------
        var experts = await _dataService.GetExpertReferencesByRegnumIdAsync(RegnumSelected.RegnumId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesByRegnumIdAsync(RegnumSelected.RegnumId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesByRegnumIdAsync(RegnumSelected.RegnumId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsByRegnumIdAsync(RegnumSelected.RegnumId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        if (RegnumSelected == null || RegnumSelected.RegnumId == 0)
            return;

        try
        {
            var model = await Tbl03RegnumsDocumentDataSource.GetRegnumDetailsAsync(RegnumSelected.RegnumId);

            if (model == null)
            {
                var loc = Localizer.Get();

                var infoDialog = new ContentDialog
                {
                    Title = loc.GetLocalizedString("Dialog_Info"),
                    Content = loc.GetLocalizedString("PdfExport_Hidden"),
                    CloseButtonText = loc.GetLocalizedString("Dialog_Ok"),
                    XamlRoot = App.MainPageInstance.Content.XamlRoot
                };

                await infoDialog.ShowAsync();
                return;
            }

            var document = new Tbl03RegnumsDocument(model);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var safeTitle = string.IsNullOrWhiteSpace(RegnumSelected.RegnumName)
                ? $"Regnum_{RegnumSelected.RegnumId}"
                : string.Concat(RegnumSelected.RegnumName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeTitle}_{RegnumSelected.RegnumId}.pdf";
            var filePath = Path.Combine(desktopPath, fileName);

            document.GeneratePdf(filePath);

            var loc1 = Localizer.Get();

            var successDialog = new ContentDialog
            {
                Title = loc1.GetLocalizedString("PdfExport_Success_Title"),
                Content = string.Format(loc1.GetLocalizedString("PdfExport_Success"), filePath),
                CloseButtonText = loc1.GetLocalizedString("Dialog_Ok"),
                XamlRoot = App.MainPageInstance.Content.XamlRoot
            };

            await successDialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            var loc = Localizer.Get();

            var errorDialog = new ContentDialog
            {
                Title = loc.GetLocalizedString("PdfExport_Error_Title"),
                Content = string.Format(loc.GetLocalizedString("PdfExport_Error"), ex.ToString()),
                CloseButtonText = loc.GetLocalizedString("Dialog_Ok"),
                XamlRoot = App.MainPageInstance.Content.XamlRoot
            };

            await errorDialog.ShowAsync();
        }
    }
}
