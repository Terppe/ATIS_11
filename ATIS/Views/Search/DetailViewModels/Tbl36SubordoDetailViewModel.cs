using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl36Subordos;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;
using QuestPDF.Fluent;
using WinUI3Localizer;


//    Tbl36SubordoDetailViewModel Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl36SubordoDetailViewModel : ObservableObject
{

    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Subordo: {SubordoSelected?.SubordoName}";

    // -----------------------------
    //  SELECTED Subordo
    // -----------------------------
    public Tbl36Subordo? SubordoSelected { get; }


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
    public Tbl39Infraordo? InfraordoSelected { get; private set; }
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
    public Tbl36SubordoDetailViewModel(Tbl36Subordo selectedSubordo, IDataService? dataService)
    {
        SubordoSelected = selectedSubordo;
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

        Breadcrumb.Add(new BreadcrumbItem
        {
            Label = SubordoSelected.SubordoName,
            EntityType = "Subordo",
            EntityId = SubordoSelected.SubordoId
        });

        // <<< HIER: aktuellen Knoten entfernen
        for (int i = Breadcrumb.Count - 1; i >= 0; i--)
        {
            var x = Breadcrumb[i];
            if (x.EntityType == "Subordo" && x.EntityId == SubordoSelected.SubordoId)
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
        SubphylumItems.Clear();
        SuperclassItems.Clear();
        ClassItems.Clear();
        SubclassItems.Clear();
        InfraclassItems.Clear();
        LegioItems.Clear();
        OrdoItems.Clear();
        SubordoItems.Clear();

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
        if (SubordoSelected.OrdoId != 0)
            ordo = await _dataService.GetOrdoByIdAsync(SubordoSelected.OrdoId);

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

        SubordoItems.Add(new TaxonomyLine<Tbl36Subordo>
        {
            Item = SubordoSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (Infraordo)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async void LoadDirectChildren()
    {
        InfraordoItems.Clear();
        HasDirectChildren = false;

        var infraordo = await _dataService.GetInfraordosBySubordoIdAsync(SubordoSelected.SubordoId);

        var visibleItems = infraordo
            .Where(x => IsVisibleTaxonName(x.InfraordoName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = SubordoItems.Count > 0 ? SubordoItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            InfraordoItems.Add(new TaxonomyLine<Tbl39Infraordo>
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
        var experts = await _dataService.GetExpertReferencesBySubordoIdAsync(SubordoSelected.SubordoId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesBySubordoIdAsync(SubordoSelected.SubordoId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesBySubordoIdAsync(SubordoSelected.SubordoId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsBySubordoIdAsync(SubordoSelected.SubordoId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        if (SubordoSelected == null || SubordoSelected.SubordoId == 0)
            return;

        try
        {
            var model = await Tbl36SubordosDocumentDataSource.GetSubordoDetailsAsync(SubordoSelected.SubordoId);

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

            var document = new Tbl36SubordosDocument(model);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var safeTitle = string.IsNullOrWhiteSpace(SubordoSelected.SubordoName)
                ? $"Subordo_{SubordoSelected.SubordoId}"
                : string.Concat(SubordoSelected.SubordoName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeTitle}_{SubordoSelected.SubordoId}.pdf";
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
