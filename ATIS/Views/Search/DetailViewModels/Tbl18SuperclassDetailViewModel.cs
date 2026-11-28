using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.Tbl18Superclasses;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;


//    Tbl18SuperclassDetailViewModel Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl18SuperclassDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;
    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Superclass: {SuperclassSelected?.SuperclassName}";
    // -----------------------------
    //  SELECTED Superclass
    // -----------------------------
    public Tbl18Superclass? SuperclassSelected { get; }
    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
    public Tbl06Phylum? PhylumSelected { get; private set; }
    public Tbl12Subphylum? SubphylumSelected { get; private set; }
    public Tbl21Class? ClassSelected { get; private set; }
    // -----------------------------
    //  Breadcrumb
    // -----------------------------
    // -----------------------------
    //  COLLECTIONS FOR LISTVIEWS
    // -----------------------------
    public ObservableCollection<TaxonomyLine<Tbl03Regnum>> RegnumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl06Phylum>> PhylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl12Subphylum>> SubphylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl18Superclass>> SuperclassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl21Class>> ClassItems { get; } = new();
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
    public Tbl18SuperclassDetailViewModel(Tbl18Superclass? selectedSuperclass, IDataService dataService)
    {
        SuperclassSelected = selectedSuperclass;
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        // ⭐ BackCommand initialisieren
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }
    private bool _isInitialized;
    private bool _isInitializing;
    public async Task InitializeAsync()
    {
        if (SuperclassSelected is not { } selected || _isInitialized || _isInitializing) return;
        _isInitializing = true;
        try
        {
            await LoadTaxonomy(selected);
            await LoadDirectChildrenAsync(selected);
            await LoadReferencesAsync(selected);
            await LoadCommentsAsync(selected);
            await OfflineTranslationFields.TranslateAsync(
                App.GetService<OfflineTranslationService>(),
                new ITranslatableTaxon?[] { SuperclassSelected, SubphylumSelected }
                    .OfType<ITranslatableTaxon>(),
                ReferenceExpertItems,
                ReferenceSourceItems,
                ReferenceAuthorItems,
                CommentItems);
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
        if (RegnumSelected != null && IsVisibleTaxonName(RegnumSelected.RegnumName))
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = RegnumSelected.RegnumName,
                EntityType = "Regnum",
                EntityId = RegnumSelected.RegnumId
            });
        }
        if (PhylumSelected != null && IsVisibleTaxonName(PhylumSelected.PhylumName))
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = PhylumSelected.PhylumName,
                EntityType = "Phylum",
                EntityId = PhylumSelected.PhylumId
            });
        }
        if (SubphylumSelected != null && IsVisibleTaxonName(SubphylumSelected.SubphylumName))
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubphylumSelected.SubphylumName,
                EntityType = "Subphylum",
                EntityId = SubphylumSelected.SubphylumId
            });
        }
    }
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
            case "Subphylum":
                frame.Navigate(typeof(Tbl12SubphylumDetailPage), item.EntityId);
                break;
            case "Superclass":
                frame.Navigate(typeof(Tbl18SuperclassDetailPage), item.EntityId);
                break;
            case "Class":
                frame.Navigate(typeof(Tbl21ClassDetailPage), item.EntityId);
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
    private async Task LoadTaxonomy(Tbl18Superclass selected)
    {
        RegnumItems.Clear();
        PhylumItems.Clear();
        SubphylumItems.Clear();
        SuperclassItems.Clear();
        Tbl12Subphylum? subphylum = null;
        Tbl06Phylum? phylum = null;
        Tbl03Regnum? regnum = null;
        // technisch laden
        if (selected.SubphylumId is int subphylumId && subphylumId != 0)
            subphylum = await _dataService.GetSubphylumByIdAsync(subphylumId);
        if (subphylum?.PhylumId is int phylumId && phylumId != 0)
            phylum = await _dataService.GetPhylumByIdAsync(phylumId);
        if (phylum?.RegnumId is int regnumId && regnumId != 0)
            regnum = await _dataService.GetRegnumByIdAsync(regnumId);
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
        if (phylum != null && IsVisibleTaxonName(phylum.PhylumName))
        {
            PhylumSelected = phylum;
            PhylumItems.Add(new TaxonomyLine<Tbl06Phylum>
            {
                Item = phylum,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }
        if (subphylum != null && IsVisibleTaxonName(subphylum.SubphylumName))
        {
            SubphylumSelected = subphylum;
            SubphylumItems.Add(new TaxonomyLine<Tbl12Subphylum>
            {
                Item = subphylum,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }
        SuperclassItems.Add(new TaxonomyLine<Tbl18Superclass>
        {
            Item = selected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }
    // -----------------------------
    //  LOAD DIRECT CHILDREN (Class)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async Task LoadDirectChildrenAsync(Tbl18Superclass selected)
    {
        ClassItems.Clear();
        HasDirectChildren = false;
        var classe = await _dataService.GetClassesBySuperclassIdAsync(selected.SuperclassId);
        var visibleItems = classe
            .Where(x => IsVisibleTaxonName(x.ClassName))
            .ToList();
        if (visibleItems.Count == 0)
            return;
        var parentLeft = SuperclassItems.Count > 0 ? SuperclassItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);
        foreach (var item in visibleItems)
        {
            ClassItems.Add(new TaxonomyLine<Tbl21Class>
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
    private async Task LoadReferencesAsync(Tbl18Superclass selected)
    {
        // -------------------------
        // EXPERT REFERENCES
        // -------------------------
        var experts = await _dataService.GetExpertReferencesBySuperclassIdAsync(selected.SuperclassId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);
        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesBySuperclassIdAsync(selected.SuperclassId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);
        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesBySuperclassIdAsync(selected.SuperclassId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }
    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async Task LoadCommentsAsync(Tbl18Superclass selected)
    {
        var comments = await _dataService.GetCommentsBySuperclassIdAsync(selected.SuperclassId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }
    // -----------------------------
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        var selected = SuperclassSelected;
        if (selected == null || selected.SuperclassId == 0)
            return;
        var name = PdfExportService.SanitizeFileNamePart(selected.SuperclassName);
        var suggestedFileName = $"Superclass_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";
        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await Tbl18SuperclassesDocumentDataSource.GetSuperclassDetailsAsync(selected.SuperclassId);
            if (model is null)
                return null;
            model.Info = selected.InfoForDisplay;
            model.Memo = selected.MemoForDisplay;
            return new Tbl18SuperclassesDocument(model);
        }, suggestedFileName);
    }
}
