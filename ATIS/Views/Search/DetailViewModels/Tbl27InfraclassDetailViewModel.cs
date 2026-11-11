using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl27Infraclasses;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;
using QuestPDF.Fluent;
using WinUI3Localizer;


//    Tbl27InfraclassDetailViewModel Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl27InfraclassDetailViewModel : ObservableObject
{

    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Infraclass: {InfraclassSelected?.InfraclassName}";

    // -----------------------------
    //  SELECTED Infraclass
    // -----------------------------
    public Tbl27Infraclass? InfraclassSelected { get; }


    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
    public Tbl06Phylum? PhylumSelected { get; private set; }
    public Tbl12Subphylum? SubphylumSelected { get; private set; }
    public Tbl18Superclass? SuperclassSelected { get; private set; }
    public Tbl21Class? ClassSelected { get; private set; }
    public Tbl24Subclass? SubclassSelected { get; private set; }
    public Tbl30Legio? LegioSelected { get; private set; }
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
    public Tbl27InfraclassDetailViewModel(Tbl27Infraclass selectedInfraclass, IDataService? dataService)
    {
        InfraclassSelected = selectedInfraclass;
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

        Breadcrumb.Add(new BreadcrumbItem
        {
            Label = InfraclassSelected.InfraclassName,
            EntityType = "Infraclass",
            EntityId = InfraclassSelected.InfraclassId
        });

        // <<< HIER: aktuellen Knoten entfernen
        for (int i = Breadcrumb.Count - 1; i >= 0; i--)
        {
            var x = Breadcrumb[i];
            if (x.EntityType == "Infraclass" && x.EntityId == InfraclassSelected.InfraclassId)
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

        Tbl24Subclass? subclass = null;
        Tbl21Class? classe = null;
        Tbl18Superclass? superclass = null;
        Tbl12Subphylum? subphylum = null;
        Tbl06Phylum? phylum = null;
        Tbl03Regnum? regnum = null;

        // technisch laden
        if (InfraclassSelected.SubclassId != 0)
            subclass = await _dataService.GetSubclassByIdAsync(InfraclassSelected.SubclassId);

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

        InfraclassItems.Add(new TaxonomyLine<Tbl27Infraclass>
        {
            Item = InfraclassSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (Legio)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async void LoadDirectChildren()
    {
        LegioItems.Clear();
        HasDirectChildren = false;

        var legio = await _dataService.GetLegiosByInfraclassIdAsync(InfraclassSelected.InfraclassId);

        var visibleItems = legio
            .Where(x => IsVisibleTaxonName(x.LegioName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = InfraclassItems.Count > 0 ? InfraclassItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            LegioItems.Add(new TaxonomyLine<Tbl30Legio>
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
        var experts = await _dataService.GetExpertReferencesByInfraclassIdAsync(InfraclassSelected.InfraclassId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesByInfraclassIdAsync(InfraclassSelected.InfraclassId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesByInfraclassIdAsync(InfraclassSelected.InfraclassId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsByInfraclassIdAsync(InfraclassSelected.InfraclassId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        if (InfraclassSelected == null || InfraclassSelected.InfraclassId == 0)
            return;

        try
        {
            var model = await Tbl27InfraclassesDocumentDataSource.GetInfraclassDetailsAsync(InfraclassSelected.InfraclassId);

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

            var document = new Tbl27InfraclassesDocument(model);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var safeTitle = string.IsNullOrWhiteSpace(InfraclassSelected.InfraclassName)
                ? $"Infraclass_{InfraclassSelected.InfraclassId}"
                : string.Concat(InfraclassSelected.InfraclassName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeTitle}_{InfraclassSelected.InfraclassId}.pdf";
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
