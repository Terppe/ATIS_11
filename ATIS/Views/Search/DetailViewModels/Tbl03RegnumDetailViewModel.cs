using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.Tbl03Regnums;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;

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
    //// -----------------------------
    ////  Breadcrumb
    //// -----------------------------
    //public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToRegnumCommand { get; }
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
    public Tbl03RegnumDetailViewModel(Tbl03Regnum? selectedRegnum, IDataService dataService)
    {
        RegnumSelected = selectedRegnum;
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);

        // ⭐ BackCommand initialisieren
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }
    private bool _isInitialized;
    private bool _isInitializing;
    public async Task InitializeAsync()
    {
        if (RegnumSelected is not { } selected || _isInitialized || _isInitializing)
            return;
        _isInitializing = true;
        try
        {
            await LoadTaxonomy(selected);
            await LoadDirectChildrenAsync(selected);
            await LoadReferencesAsync(selected);
            await LoadCommentsAsync(selected);

            await OfflineTranslationFields.TranslateAsync(
                App.GetService<OfflineTranslationService>(),
                new ITranslatableTaxon?[] { RegnumSelected }
                    .OfType<ITranslatableTaxon>(),
                ReferenceExpertItems,
                ReferenceSourceItems,
                ReferenceAuthorItems,
                CommentItems);

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

    public async Task NavigateBreadcrumbAsync(BreadcrumbItem item)
    {
        var frame = App.MainPageInstance?.NavFrame
                    ?? throw new InvalidOperationException("Navigation frame is not available.");

        switch (item.EntityType)
        {
            case "Regnum":
                frame.Navigate(typeof(Tbl03RegnumDetailPage), item.EntityId);
                break;

            case "Phylum":
                frame.Navigate(typeof(Tbl06PhylumDetailPage), item.EntityId);
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

    private static bool IsVisibleTaxonName([NotNullWhen(true)] string? name)
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
    private async Task LoadTaxonomy(Tbl03Regnum selected)
    {
        RegnumItems.Clear();
        // sichtbare Reihenfolge aufbauen

        var visibleIndex = 0;

        RegnumItems.Add(new TaxonomyLine<Tbl03Regnum>
        {
            Item = selected,
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
    private async Task LoadDirectChildrenAsync(Tbl03Regnum selected)
    {
        PhylumItems.Clear();
        HasDirectChildren = false;

        var phylum = await _dataService.GetPhylumsByRegnumIdAsync(selected.RegnumId);

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
    private async Task LoadReferencesAsync(Tbl03Regnum selected)
    {
        // -------------------------
        // EXPERT REFERENCES
        // -------------------------
        var experts = await _dataService.GetExpertReferencesByRegnumIdAsync(selected.RegnumId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesByRegnumIdAsync(selected.RegnumId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesByRegnumIdAsync(selected.RegnumId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async Task LoadCommentsAsync(Tbl03Regnum selected)
    {
        var comments = await _dataService.GetCommentsByRegnumIdAsync(selected.RegnumId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        var selected = RegnumSelected;
        if (selected == null || selected.RegnumId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.RegnumName);
        var suggestedFileName = $"Regnum_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await Tbl03RegnumsDocumentDataSource.GetRegnumDetailsAsync(selected.RegnumId);

            if (model is null)
                return null;

            model.Info = selected.InfoForDisplay;
            model.Memo = selected.MemoForDisplay;

            return model == null ? null : new Tbl03RegnumsDocument(model);
        }, suggestedFileName);
    }
}
