using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;
using static ATIS.Views.Report.PdfModels;


//    Tbl54SupertribusDetailViewModel Skriptdatum:  18.18.2026  10:32    

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl54SupertribusDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Supertribus: {SupertribusSelected?.SupertribusName}";

    // -----------------------------
    //  SELECTED Supertribus
    // -----------------------------
    public Tbl54Supertribus SupertribusSelected { get; }

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
    public Tbl45Family? FamilySelected { get; private set; }
    public Tbl48Subfamily? SubfamilySelected { get; private set; }
    public Tbl51Infrafamily? InfrafamilySelected { get; private set; }
    // -----------------------------
    //  Breadcrumb
    // -----------------------------
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToRegnumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToPhylumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSubphylumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSuperclassCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToClassCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSubclassCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToInfraclassCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToLegioCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToOrdoCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSubordoCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToInfraordoCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSuperfamilyCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToFamilyCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSubfamilyCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToInfrafamilyCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSupertribusCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToTribusCommand { get; }

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
    public ObservableCollection<TaxonomyLine<Tbl51Infrafamily>> InfrafamilyItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl54Supertribus>> SupertribusItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl57Tribus>> TribusItems { get; } = new();

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
    public IRelayCommand ExportPdfCommand { get; }

    // ⭐ NEU: BackCommand
    public IRelayCommand BackCommand { get; }

    // -----------------------------
    //  CONSTRUCTOR
    // -----------------------------
    public Tbl54SupertribusDetailViewModel(Tbl54Supertribus selectedSupertribus, IDataService? dataService)
    {
        SupertribusSelected = selectedSupertribus;
        _dataService = dataService;

        ExportPdfCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExportPdf);

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
    public IEnumerable<BreadcrumbItem> BreadcrumbReversed => Breadcrumb.Reverse();

    private void BuildBreadcrumb()
    {
        //Initialisierung der Breadcrumb-Navigation
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

        if (PhylumSelected != null)
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = PhylumSelected.PhylumName,
                EntityType = "Phylum",
                EntityId = PhylumSelected.PhylumId
            });
        }

        if (SubphylumSelected != null)
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubphylumSelected.SubphylumName,
                EntityType = "Subphylum",
                EntityId = SubphylumSelected.SubphylumId
            });
        }

        if (SuperclassSelected != null)
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SuperclassSelected.SuperclassName,
                EntityType = "Superclass",
                EntityId = SuperclassSelected.SuperclassId
            });
        }

        if (ClassSelected != null)
        {
            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = ClassSelected.ClassName,
                EntityType = "Class",
                EntityId = ClassSelected.ClassId
            });
        }

        if (SubclassSelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubclassSelected.SubclassName,
                EntityType = "Subclass",
                EntityId = SubclassSelected.SubclassId
            });
        }

        if (InfraclassSelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = InfraclassSelected.InfraclassName,
                EntityType = "Infraclass",
                EntityId = InfraclassSelected.InfraclassId
            });
        }

        if (LegioSelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = LegioSelected.LegioName,
                EntityType = "Legio",
                EntityId = LegioSelected.LegioId
            });
        }

        if (OrdoSelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = OrdoSelected.OrdoName,
                EntityType = "Ordo",
                EntityId = OrdoSelected.OrdoId
            });
        }

        if (SubordoSelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubordoSelected.SubordoName,
                EntityType = "Subordo",
                EntityId = SubordoSelected.SubordoId
            });
        }

        if (InfraordoSelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = InfraordoSelected.InfraordoName,
                EntityType = "Infraordo",
                EntityId = InfraordoSelected.InfraordoId
            });
        }

        if (SuperfamilySelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SuperfamilySelected.SuperfamilyName,
                EntityType = "Superfamily",
                EntityId = SuperfamilySelected.SuperfamilyId
            });
        }

        if (FamilySelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = FamilySelected.FamilyName,
                EntityType = "Family",
                EntityId = FamilySelected.FamilyId
            });
        }

        if (SubfamilySelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = SubfamilySelected.SubfamilyName,
                EntityType = "Subfamily",
                EntityId = SubfamilySelected.SubfamilyId
            });
        }

        if (InfrafamilySelected != null)
        {

            Breadcrumb.Add(new BreadcrumbItem
            {
                Label = InfrafamilySelected.InfrafamilyName,
                EntityType = "Infrafamily",
                EntityId = InfrafamilySelected.InfrafamilyId
            });
        }

        Breadcrumb.Add(new BreadcrumbItem
        {
            Label = SupertribusSelected.SupertribusName,
            EntityType = "Supertribus",
            EntityId = SupertribusSelected.SupertribusId
        });


        for (int i = Breadcrumb.Count - 1; i >= 0; i--)
        {
            var x = Breadcrumb[i];
            if (x.EntityType == "Supertribus" && x.EntityId == SupertribusSelected.SupertribusId)
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

            case "Superclass":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl18SuperclassDetailPage), item.EntityId);
                break;

            case "Class":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl21ClassDetailPage), item.EntityId);
                break;

            case "Subclass":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl24SubclassDetailPage), item.EntityId);
                break;

            case "Infraclass":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl27InfraclassDetailPage), item.EntityId);
                break;

            case "Legio":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl30LegioDetailPage), item.EntityId);
                break;

            case "Ordo":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl33OrdoDetailPage), item.EntityId);
                break;

            case "Subordo":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl36SubordoDetailPage), item.EntityId);
                break;

            case "Infraordo":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl39InfraordoDetailPage), item.EntityId);
                break;

            case "Superfamily":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl42SuperfamilyDetailPage), item.EntityId);
                break;

            case "Family":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl45FamilyDetailPage), item.EntityId);
                break;

            case "Subfamily":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl48SubfamilyDetailPage), item.EntityId);
                break;

            case "Infrafamily":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl51InfrafamilyDetailPage), item.EntityId);
                break;

            case "Supertribus":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl54SupertribusDetailPage), item.EntityId);
                break;
        }
    }

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        var frame = App.MainPageInstance.NavFrame;

        if (frame.CanGoBack)
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
        SubfamilyItems.Clear();
        InfrafamilyItems.Clear();
        SupertribusItems.Clear();

        Tbl54Supertribus? supertribus = null;
        Tbl51Infrafamily? infrafamily = null;
        Tbl48Subfamily? subfamily = null;
        Tbl45Family? family = null;
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
        if (SupertribusSelected.SupertribusId != 0)
            infrafamily = await _dataService.GetInfrafamilyByIdAsync(SupertribusSelected.InfrafamilyId);

        if (infrafamily.SubfamilyId != 0)
            subfamily = await _dataService.GetSubfamilyByIdAsync(infrafamily.SubfamilyId);

        if (subfamily.FamilyId != 0)
            family = await _dataService.GetFamilyByIdAsync(subfamily.FamilyId);

        if (family.SuperfamilyId != 0)
            superfamily = await _dataService.GetSuperfamilyByIdAsync(family.SuperfamilyId);

        if (superfamily.InfraordoId != 0)
            infraordo = await _dataService.GetInfraordoByIdAsync(superfamily.InfraordoId);

        if (infraordo.SubordoId != 0)
            subordo = await _dataService.GetSubordoByIdAsync(infraordo.SubordoId);

        if (subordo.OrdoId != 0)
            ordo = await _dataService.GetOrdoByIdAsync(subordo.OrdoId);

        if (ordo.LegioId != 0)
            legio = await _dataService.GetLegioByIdAsync(ordo.LegioId);

        if (legio.InfraclassId != 0)
            infraclass = await _dataService.GetInfraclassByIdAsync(legio.InfraclassId);

        if (infraclass.SubclassId != 0)
            subclass = await _dataService.GetSubclassByIdAsync(infraclass.SubclassId);

        if (subclass.ClassId != 0)
            classe = await _dataService.GetClassByIdAsync(subclass.ClassId);

        if (classe?.SuperclassId != 0)
            superclass = await _dataService.GetSuperclassByIdAsync(classe.SuperclassId);

        if (superclass?.SubphylumId != 0)
            subphylum = await _dataService.GetSubphylumByIdAsync(superclass.SubphylumId);

        if (subphylum?.PhylumId != 0)
            phylum = await _dataService.GetPhylumByIdAsync(subphylum.PhylumId);

        if (phylum?.RegnumId != 0)
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

        if (family != null && IsVisibleTaxonName(family.FamilyName))
        {
            FamilySelected = family;
            FamilyItems.Add(new TaxonomyLine<Tbl45Family>
            {
                Item = family,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (subfamily != null && IsVisibleTaxonName(subfamily.SubfamilyName))
        {
            SubfamilySelected = subfamily;
            SubfamilyItems.Add(new TaxonomyLine<Tbl48Subfamily>
            {
                Item = subfamily,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        if (infrafamily != null && IsVisibleTaxonName(infrafamily.InfrafamilyName))
        {
            InfrafamilySelected = infrafamily;
            InfrafamilyItems.Add(new TaxonomyLine<Tbl51Infrafamily>
            {
                Item = infrafamily,
                HeaderMargin = CreateHeaderMargin(visibleIndex++),
                ShowAsHyperlink = true
            });
        }

        SupertribusItems.Add(new TaxonomyLine<Tbl54Supertribus>
        {
            Item = SupertribusSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });

    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (Tribus)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async void LoadDirectChildren()
    {
        TribusItems.Clear();
        HasDirectChildren = false;

        var tribus = await _dataService.GetTribussesBySupertribusIdAsync(SupertribusSelected.SupertribusId);

        var visibleItems = tribus
            .Where(x => IsVisibleTaxonName(x.TribusName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = SupertribusItems.Count > 0 ? SupertribusItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            TribusItems.Add(new TaxonomyLine<Tbl57Tribus>
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
        var experts = await _dataService.GetExpertReferencesBySupertribusIdAsync(SupertribusSelected.SupertribusId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesBySupertribusIdAsync(SupertribusSelected.SupertribusId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesBySupertribusIdAsync(SupertribusSelected.SupertribusId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsBySupertribusIdAsync(SupertribusSelected.SupertribusId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------
    //  PDF EXPORT
    // -----------------------------
    private void ExportPdf()
    {
        // Hier später QuestPDF oder Syncfusion einbauen
    }

}
