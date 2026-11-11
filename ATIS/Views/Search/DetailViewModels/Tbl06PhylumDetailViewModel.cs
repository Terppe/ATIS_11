using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl06Phylums;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;
using QuestPDF.Fluent;
using WinUI3Localizer;


//    Tbl06PhylumDetailViewModel Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl06PhylumDetailViewModel : ObservableObject
{

    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Phylum: {PhylumSelected?.PhylumName}";

    // -----------------------------
    //  SELECTED Phylum
    // -----------------------------
    public Tbl06Phylum? PhylumSelected { get; }


    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
    public Tbl12Subphylum? SubphylumSelected { get; private set; }
    // -----------------------------
    //  Breadcrumb
    // -----------------------------
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToRegnumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToPhylumCommand { get; }
    // -----------------------------
    //  COLLECTIONS FOR LISTVIEWS
    // -----------------------------
    public ObservableCollection<TaxonomyLine<Tbl03Regnum>> RegnumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl06Phylum>> PhylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl12Subphylum>> SubphylumItems { get; } = new();

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
    public Tbl06PhylumDetailViewModel(Tbl06Phylum selectedPhylum, IDataService? dataService)
    {
        PhylumSelected = selectedPhylum;
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

        if (RegnumSelected != null)
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = RegnumSelected.RegnumName,
                EntityType = "Regnum",
                EntityId = RegnumSelected.RegnumId
            });
        }

        Breadcrumb.Add(new BreadcrumbItem
        {
            Label = PhylumSelected.PhylumName,
            EntityType = "Phylum",
            EntityId = PhylumSelected.PhylumId
        });

        // <<< HIER: aktuellen Knoten entfernen
        for (int i = Breadcrumb.Count - 1; i >= 0; i--)
        {
            var x = Breadcrumb[i];
            if (x.EntityType == "Phylum" && x.EntityId == PhylumSelected.PhylumId)
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

            case "Subphylum":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl12SubphylumDetailPage), item.EntityId);
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
        PhylumItems.Clear();

        Tbl03Regnum? regnum = null;

        // technisch laden
        if (PhylumSelected.RegnumId != 0)
            regnum = await _dataService.GetRegnumByIdAsync(PhylumSelected.RegnumId);


        // sichtbare Reihenfolge aufbauen
        var visibleIndex = 0;

        if (regnum != null && IsVisibleTaxonName(regnum.RegnumName))
        {
            RegnumSelected = regnum;
            RegnumItems.Add(new TaxonomyLine<Tbl03Regnum>
            {
                Item = regnum,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        PhylumItems.Add(new TaxonomyLine<Tbl06Phylum>
        {
            Item = PhylumSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (Subphylum)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async void LoadDirectChildren()
    {
        SubphylumItems.Clear();
        HasDirectChildren = false;

        var subphylum = await _dataService.GetSubphylumsByPhylumIdAsync(PhylumSelected.PhylumId);

        var visibleItems = subphylum
            .Where(x => IsVisibleTaxonName(x.SubphylumName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = PhylumItems.Count > 0 ? PhylumItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            SubphylumItems.Add(new TaxonomyLine<Tbl12Subphylum>
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
        var experts = await _dataService.GetExpertReferencesByPhylumIdAsync(PhylumSelected.PhylumId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesByPhylumIdAsync(PhylumSelected.PhylumId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesByPhylumIdAsync(PhylumSelected.PhylumId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsByPhylumIdAsync(PhylumSelected.PhylumId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        if (PhylumSelected == null || PhylumSelected.PhylumId == 0)
            return;

        try
        {
            var model = await Tbl06PhylumsDocumentDataSource.GetPhylumDetailsAsync(PhylumSelected.PhylumId);

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

            var document = new Tbl06PhylumsDocument(model);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var safeTitle = string.IsNullOrWhiteSpace(PhylumSelected.PhylumName)
                ? $"Phylum_{PhylumSelected.PhylumId}"
                : string.Concat(PhylumSelected.PhylumName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeTitle}_{PhylumSelected.PhylumId}.pdf";
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
