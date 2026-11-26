using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.Tbl45Families;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;


//    Tbl45FamilyDetailViewModel Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl45FamilyDetailViewModel : ObservableObject
{

    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Family: {FamilySelected?.FamilyName}";

    // -----------------------------
    //  SELECTED Family
    // -----------------------------
    public Tbl45Family? FamilySelected { get; }


    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
    public Tbl06Phylum? PhylumSelected { get; private set; }
    public Tbl12Subphylum? SubphylumSelected { get; private set; }
    public Tbl18Superclass? SuperclassSelected { get; private set; }
    public Tbl21Class? ClassSelected { get; private set; }
    public Tbl24Subclass? SubclassSelected { get; private set; }
    public Tbl27Infraclass? InfraclassSelected { get; private set; }
    public Tbl30Legio? LegioSelected { get; private set; }
    public Tbl33Ordo? OrdoSelected { get; private set; }
    public Tbl36Subordo? SubordoSelected { get; private set; }
    public Tbl39Infraordo? InfraordoSelected { get; private set; }
    public Tbl42Superfamily? SuperfamilySelected { get; private set; }
    public Tbl48Subfamily? SubfamilySelected { get; private set; }
    // -----------------------------
    //  COLLECTIONS FOR LISTVIEWS
    // -----------------------------
    public ObservableCollection<TaxonomyLine<Tbl03Regnum>> RegnumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl06Phylum>> PhylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl12Subphylum>> SubphylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl18Superclass>> SuperclassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl21Class>> ClassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl24Subclass>> SubclassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl27Infraclass>> InfraclassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl30Legio>> LegioItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl33Ordo>> OrdoItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl36Subordo>> SubordoItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl39Infraordo>> InfraordoItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl42Superfamily>> SuperfamilyItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl45Family>> FamilyItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl48Subfamily>> SubfamilyItems { get; } = new();

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
    public Tbl45FamilyDetailViewModel(Tbl45Family? selectedFamily, IDataService dataService)
    {
        FamilySelected = selectedFamily;
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);

        // ⭐ BackCommand initialisieren
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }
    private bool _isInitialized;
    private bool _isInitializing;
    public async Task InitializeAsync()
    {
        if (FamilySelected is not { } selected || _isInitialized || _isInitializing) return;
        _isInitializing = true;
        try
        {
            await LoadTaxonomy(selected);
            await LoadDirectChildrenAsync(selected);
            await LoadReferencesAsync(selected);
            await LoadCommentsAsync(selected);

            await OfflineTranslationFields.TranslateAsync(
                App.GetService<OfflineTranslationService>(),
                new ITranslatableTaxon?[] { FamilySelected, SuperfamilySelected }
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

        if (SuperclassSelected != null && IsVisibleTaxonName(SuperclassSelected.SuperclassName))
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SuperclassSelected.SuperclassName,
                EntityType = "Superclass",
                EntityId = SuperclassSelected.SuperclassId
            });
        }

        if (ClassSelected != null && IsVisibleTaxonName(ClassSelected.ClassName))
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = ClassSelected.ClassName,
                EntityType = "Class",
                EntityId = ClassSelected.ClassId
            });
        }

        if (SubclassSelected != null && IsVisibleTaxonName(SubclassSelected.SubclassName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubclassSelected.SubclassName,
                EntityType = "Subclass",
                EntityId = SubclassSelected.SubclassId
            });
        }

        if (InfraclassSelected != null && IsVisibleTaxonName(InfraclassSelected.InfraclassName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = InfraclassSelected.InfraclassName,
                EntityType = "Infraclass",
                EntityId = InfraclassSelected.InfraclassId
            });
        }

        if (LegioSelected != null && IsVisibleTaxonName(LegioSelected.LegioName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = LegioSelected.LegioName,
                EntityType = "Legio",
                EntityId = LegioSelected.LegioId
            });
        }

        if (OrdoSelected != null && IsVisibleTaxonName(OrdoSelected.OrdoName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = OrdoSelected.OrdoName,
                EntityType = "Ordo",
                EntityId = OrdoSelected.OrdoId
            });
        }

        if (SubordoSelected != null && IsVisibleTaxonName(SubordoSelected.SubordoName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubordoSelected.SubordoName,
                EntityType = "Subordo",
                EntityId = SubordoSelected.SubordoId
            });
        }

        if (InfraordoSelected != null && IsVisibleTaxonName(InfraordoSelected.InfraordoName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = InfraordoSelected.InfraordoName,
                EntityType = "Infraordo",
                EntityId = InfraordoSelected.InfraordoId
            });
        }

        if (SuperfamilySelected != null && IsVisibleTaxonName(SuperfamilySelected.SuperfamilyName))
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SuperfamilySelected.SuperfamilyName,
                EntityType = "Superfamily",
                EntityId = SuperfamilySelected.SuperfamilyId
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

            case "Subclass":
                frame.Navigate(typeof(Tbl24SubclassDetailPage), item.EntityId);
                break;

            case "Infraclass":
                frame.Navigate(typeof(Tbl27InfraclassDetailPage), item.EntityId);
                break;

            case "Legio":
                frame.Navigate(typeof(Tbl30LegioDetailPage), item.EntityId);
                break;

            case "Ordo":
                frame.Navigate(typeof(Tbl33OrdoDetailPage), item.EntityId);
                break;

            case "Subordo":
                frame.Navigate(typeof(Tbl36SubordoDetailPage), item.EntityId);
                break;

            case "Infraordo":
                frame.Navigate(typeof(Tbl39InfraordoDetailPage), item.EntityId);
                break;

            case "Superfamily":
                frame.Navigate(typeof(Tbl42SuperfamilyDetailPage), item.EntityId);
                break;

            case "Family":
                frame.Navigate(typeof(Tbl45FamilyDetailPage), item.EntityId);
                break;

            case "Subfamily":
                frame.Navigate(typeof(Tbl48SubfamilyDetailPage), item.EntityId);
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
    private async Task LoadTaxonomy(Tbl45Family selected)
    {
        RegnumItems.Clear();
        PhylumItems.Clear();
        SubphylumItems.Clear();
        SuperclassItems.Clear();
        ClassItems.Clear();
        SubclassItems.Clear();
        InfraclassItems.Clear();
        LegioItems.Clear();
        OrdoItems.Clear();
        SubordoItems.Clear();
        InfraordoItems.Clear();
        SuperfamilyItems.Clear();
        FamilyItems.Clear();

        Tbl42Superfamily? superfamily = null;
        Tbl39Infraordo? infraordo = null;
        Tbl36Subordo? subordo = null;
        Tbl33Ordo? ordo = null;
        Tbl30Legio? legio = null;
        Tbl27Infraclass? infraclass = null;
        Tbl24Subclass? subclass = null;
        Tbl21Class? classe = null;
        Tbl18Superclass? superclass = null;
        Tbl12Subphylum? subphylum = null;
        Tbl06Phylum? phylum = null;
        Tbl03Regnum? regnum = null;

        // technisch laden
        if (selected.SuperfamilyId is int superfamilyId && superfamilyId != 0)
            superfamily = await _dataService.GetSuperfamilyByIdAsync(superfamilyId);

        if (superfamily != null && superfamily.InfraordoId != 0)
            infraordo = await _dataService.GetInfraordoByIdAsync(superfamily.InfraordoId);

        if (infraordo != null && infraordo.SubordoId != 0)
            subordo = await _dataService.GetSubordoByIdAsync(infraordo.SubordoId);

        if (subordo != null && subordo.OrdoId != 0)
            ordo = await _dataService.GetOrdoByIdAsync(subordo.OrdoId);

        if (ordo != null && ordo.LegioId != 0)
            legio = await _dataService.GetLegioByIdAsync(ordo.LegioId);

        if (legio != null && legio.InfraclassId != 0)
            infraclass = await _dataService.GetInfraclassByIdAsync(legio.InfraclassId);

        if (infraclass != null && infraclass.SubclassId != 0)
            subclass = await _dataService.GetSubclassByIdAsync(infraclass.SubclassId);

        if (subclass != null && subclass.ClassId != 0)
            classe = await _dataService.GetClassByIdAsync(subclass.ClassId);

        if (classe != null && classe.SuperclassId != 0)
            superclass = await _dataService.GetSuperclassByIdAsync(classe.SuperclassId);

        if (superclass != null && superclass.SubphylumId != 0)
            subphylum = await _dataService.GetSubphylumByIdAsync(superclass.SubphylumId);

        if (subphylum != null && subphylum.PhylumId != 0)
            phylum = await _dataService.GetPhylumByIdAsync(subphylum.PhylumId);

        if (phylum != null && phylum.RegnumId != 0)
            regnum = await _dataService.GetRegnumByIdAsync(phylum.RegnumId);

        // sichtbare Reihenfolge aufbauen
        int visibleIndex = 0;

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

        if (superclass != null && IsVisibleTaxonName(superclass.SuperclassName))
        {
            SuperclassSelected = superclass;
            SuperclassItems.Add(new TaxonomyLine<Tbl18Superclass>
            {
                Item = superclass,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (classe != null && IsVisibleTaxonName(classe.ClassName))
        {
            ClassSelected = classe;
            ClassItems.Add(new TaxonomyLine<Tbl21Class>
            {
                Item = classe,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (subclass != null && IsVisibleTaxonName(subclass.SubclassName))
        {
            SubclassSelected = subclass;
            SubclassItems.Add(new TaxonomyLine<Tbl24Subclass>
            {
                Item = subclass,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (infraclass != null && IsVisibleTaxonName(infraclass.InfraclassName))
        {
            InfraclassSelected = infraclass;
            InfraclassItems.Add(new TaxonomyLine<Tbl27Infraclass>
            {
                Item = infraclass,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (legio != null && IsVisibleTaxonName(legio.LegioName))
        {
            LegioSelected = legio;
            LegioItems.Add(new TaxonomyLine<Tbl30Legio>
            {
                Item = legio,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (ordo != null && IsVisibleTaxonName(ordo.OrdoName))
        {
            OrdoSelected = ordo;
            OrdoItems.Add(new TaxonomyLine<Tbl33Ordo>
            {
                Item = ordo,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (subordo != null && IsVisibleTaxonName(subordo.SubordoName))
        {
            SubordoSelected = subordo;
            SubordoItems.Add(new TaxonomyLine<Tbl36Subordo>
            {
                Item = subordo,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }


        if (infraordo != null && IsVisibleTaxonName(infraordo.InfraordoName))
        {
            InfraordoSelected = infraordo;
            InfraordoItems.Add(new TaxonomyLine<Tbl39Infraordo>
            {
                Item = infraordo,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (superfamily != null && IsVisibleTaxonName(superfamily.SuperfamilyName))
        {
            SuperfamilySelected = superfamily;
            SuperfamilyItems.Add(new TaxonomyLine<Tbl42Superfamily>
            {
                Item = superfamily,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        FamilyItems.Add(new TaxonomyLine<Tbl45Family>
        {
            Item = selected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (Subfamily)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async Task LoadDirectChildrenAsync(Tbl45Family selected)
    {
        SubfamilyItems.Clear();
        HasDirectChildren = false;

        var subfamily = await _dataService.GetSubfamiliesByFamilyIdAsync(selected.FamilyId);

        var visibleItems = subfamily
            .Where(x => IsVisibleTaxonName(x.SubfamilyName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = FamilyItems.Count > 0 ? FamilyItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            SubfamilyItems.Add(new TaxonomyLine<Tbl48Subfamily>
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
    private async Task LoadReferencesAsync(Tbl45Family selected)
    {
        // -------------------------
        // EXPERT REFERENCES
        // -------------------------
        var experts = await _dataService.GetExpertReferencesByFamilyIdAsync(selected.FamilyId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesByFamilyIdAsync(selected.FamilyId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesByFamilyIdAsync(selected.FamilyId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async Task LoadCommentsAsync(Tbl45Family selected)
    {
        var comments = await _dataService.GetCommentsByFamilyIdAsync(selected.FamilyId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        var selected = FamilySelected;
        if (selected == null || selected.FamilyId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.FamilyName);
        var suggestedFileName = $"Family_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await Tbl45FamiliesDocumentDataSource.GetFamilyDetailsAsync(selected.FamilyId);

            if (model is null)
                return null;

            model.Info = selected.InfoForDisplay;
            model.Memo = selected.MemoForDisplay;

            return model == null ? null : new Tbl45FamiliesDocument(model);
        }, suggestedFileName);
    }
}
