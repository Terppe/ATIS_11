using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl21Classes;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;
using QuestPDF.Fluent;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl21ClassDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Class: {ClassSelected?.ClassName}";

    // -----------------------------
    //  SELECTED CLASS
    // -----------------------------
    public Tbl21Class ClassSelected { get; }

    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
    public Tbl06Phylum? PhylumSelected { get; private set; }
    public Tbl12Subphylum? SubphylumSelected { get; private set; }
    public Tbl18Superclass? SuperclassSelected { get; private set; }

    // -----------------------------
    //  Breadcrumb
    // -----------------------------
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToRegnumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToPhylumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSubphylumCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToSuperclassCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand NavigateToClassCommand { get; }

    // -----------------------------
    //  COLLECTIONS FOR LISTVIEWS
    // -----------------------------
    public ObservableCollection<TaxonomyLine<Tbl03Regnum>> RegnumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl06Phylum>> PhylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl12Subphylum>> SubphylumItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl18Superclass>> SuperclassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl21Class>> ClassItems { get; } = new();
    public ObservableCollection<TaxonomyLine<Tbl24Subclass>> SubclassItems { get; } = new();

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
    public Tbl21ClassDetailViewModel(Tbl21Class selectedClass, IDataService? dataService)
    {
        ClassSelected = selectedClass;
        _dataService = dataService;

     //   ExportPdfCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExportPdf);
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

        // <<< HIER: aktuellen Knoten entfernen
        for (int i = Breadcrumb.Count - 1; i >= 0; i--)
        {
            var x = Breadcrumb[i];
            if (x.EntityType == "Class" && x.EntityId == ClassSelected.ClassId)
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

        }
    }
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

        Tbl18Superclass? superclass = null;
        Tbl12Subphylum? subphylum = null;
        Tbl06Phylum? phylum = null;
        Tbl03Regnum? regnum = null;

        if (ClassSelected.SuperclassId != 0)
            superclass = await _dataService.GetSuperclassByIdAsync(ClassSelected.SuperclassId);

        if (superclass?.SubphylumId != 0)
            subphylum = await _dataService.GetSubphylumByIdAsync(superclass.SubphylumId);

        if (subphylum?.PhylumId != 0)
            phylum = await _dataService.GetPhylumByIdAsync(subphylum.PhylumId);

        if (phylum?.RegnumId != 0)
            regnum = await _dataService.GetRegnumByIdAsync(phylum.RegnumId);

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

        ClassItems.Add(new TaxonomyLine<Tbl21Class>
        {
            Item = ClassSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (SUBCLASS)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }
    private async void LoadDirectChildren()
    {
        SubclassItems.Clear();
        HasDirectChildren = false;

        var subclasses = await _dataService.GetSubclassesByClassIdAsync(ClassSelected.ClassId);

        var visibleItems = subclasses
            .Where(x => IsVisibleTaxonName(x.SubclassName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = ClassItems.Count > 0 ? ClassItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            SubclassItems.Add(new TaxonomyLine<Tbl24Subclass>
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
        var experts = await _dataService.GetExpertReferencesByClassIdAsync(ClassSelected.ClassId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesByClassIdAsync(ClassSelected.ClassId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesByClassIdAsync(ClassSelected.ClassId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsByClassIdAsync(ClassSelected.ClassId);
        foreach (var item in comments)
            CommentItems.Add(item);
    }

    // -----------------------------
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        if (ClassSelected == null || ClassSelected.ClassId == 0)
            return;

        try
        {
            var model = await Tbl21ClassesDocumentDataSource.GetClassDetailsAsync(ClassSelected.ClassId);
            var document = new Tbl21ClassesDocument(model);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var safeTitle = string.IsNullOrWhiteSpace(ClassSelected.ClassName)
                ? $"Class_{ClassSelected.ClassId}"
                : string.Concat(ClassSelected.ClassName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeTitle}_{ClassSelected.ClassId}.pdf";
            var filePath = Path.Combine(desktopPath, fileName);

            document.GeneratePdf(filePath);

            var successDialog = new ContentDialog
            {
                Title = "PDF exportiert",
                Content = $"Die PDF wurde erstellt:\n{filePath}",
                CloseButtonText = "OK",
                XamlRoot = App.MainPageInstance.Content.XamlRoot
            };

            await successDialog.ShowAsync();
        }
        catch (Exception ex)
        {
            var errorDialog = new ContentDialog
            {
                Title = "PDF-Fehler",
                Content = ex.Message,
                CloseButtonText = "OK",
                XamlRoot = App.MainPageInstance.Content.XamlRoot
            };

            await errorDialog.ShowAsync();
        }
    }
}
