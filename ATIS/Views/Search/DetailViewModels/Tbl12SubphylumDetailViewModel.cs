using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl12SubphylumDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Subphylum: {SubphylumSelected?.SubphylumName}";

    // -----------------------------
    //  SELECTED SUBPHYLUM
    // -----------------------------
    public Tbl12Subphylum SubphylumSelected { get; }

    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
    public Tbl06Phylum? PhylumSelected { get; private set; }
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
    public Tbl12SubphylumDetailViewModel(Tbl12Subphylum selectedSubphylum, IDataService? dataService)
    {
        SubphylumSelected = selectedSubphylum;
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
        // <<< HIER: aktuellen Knoten entfernen
        Breadcrumb = new ObservableCollection<BreadcrumbItem>(
            Breadcrumb.Where(x => !(x.EntityType == "Subphylum" && x.EntityId == SubphylumSelected.SubphylumId)));


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

        Tbl06Phylum? phylum = null;
        Tbl03Regnum? regnum = null;

        // technisch laden
        if (SubphylumSelected.PhylumId != 0)
            phylum = await _dataService.GetPhylumByIdAsync(SubphylumSelected.PhylumId);

        if (phylum?.RegnumId != 0)
            regnum = await _dataService.GetRegnumByIdAsync(phylum.RegnumId);

        // sichtbare Reihenfolge
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

        SubphylumItems.Add(new TaxonomyLine<Tbl12Subphylum>
        {
            Item = SubphylumSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (SUPERCLASS)
    // -----------------------------
    private bool _hasDirectChildren;
    public bool HasDirectChildren
    {
        get => _hasDirectChildren;
        set => SetProperty(ref _hasDirectChildren, value);
    }

    private async void LoadDirectChildren()
    {
        SuperclassItems.Clear();

        var superclasses = await _dataService.GetSuperclassesBySubphylumIdAsync(SubphylumSelected.SubphylumId);
        HasDirectChildren = false;

        var visibleItems = superclasses
            .Where(x => IsVisibleTaxonName(x.SuperclassName))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        var parentLeft = SubphylumItems.Count > 0 ? SubphylumItems[0].HeaderMargin.Left : 8;
        var childMargin = new Thickness(parentLeft + 8, 0, 0, 0);

        foreach (var item in visibleItems)
        {
            SuperclassItems.Add(new TaxonomyLine<Tbl18Superclass>
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
        var experts = await _dataService.GetExpertReferencesBySubphylumIdAsync(SubphylumSelected.SubphylumId);
        foreach (var item in experts)
            ReferenceExpertItems.Add(item);

        // -------------------------
        // SOURCE REFERENCES
        // -------------------------
        var sources = await _dataService.GetSourceReferencesBySubphylumIdAsync(SubphylumSelected.SubphylumId);
        foreach (var item in sources)
            ReferenceSourceItems.Add(item);

        // -------------------------
        // AUTHOR REFERENCES
        // -------------------------
        var authors = await _dataService.GetAuthorReferencesBySubphylumIdAsync(SubphylumSelected.SubphylumId);
        foreach (var item in authors)
            ReferenceAuthorItems.Add(item);
    }

    // -----------------------------
    //  LOAD COMMENTS
    // -----------------------------
    private async void LoadComments()
    {
        var comments = await _dataService.GetCommentsBySubphylumIdAsync(SubphylumSelected.SubphylumId);
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

    // -----------------------------
    //  NAVIGATION (Hyperlink)
    // -----------------------------
    private async Task NavigateToRegnumAsync(int regnumId)
    {
        var selectedRegnum = await _dataService.GetRegnumByIdAsync(regnumId);

        var vm = new Tbl03RegnumDetailViewModel(selectedRegnum, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl03RegnumDetailPage), vm);
    }

    private async Task NavigateToPhylumAsync(int phylumId)
    {
        var selectedPhylum = await _dataService.GetPhylumByIdAsync(phylumId);

        var vm = new Tbl06PhylumDetailViewModel(selectedPhylum, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl06PhylumDetailPage), vm);
    }
    private async Task NavigateToSubphylumAsync(int subphylumId)
    {
        var selectedSubphylum = await _dataService.GetSubphylumByIdAsync(subphylumId);

        var vm = new Tbl12SubphylumDetailViewModel(selectedSubphylum, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl12SubphylumDetailPage), vm);
    }

    private async Task NavigateToSuperclassAsync(int superclassId)
    {
        var selectedSuperclass = await _dataService.GetSuperclassByIdAsync(superclassId);
        if (selectedSuperclass == null)
            return;

        var vm = new Tbl18SuperclassDetailViewModel(selectedSuperclass, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl18SuperclassDetailPage), vm);
    }

    private async Task NavigateToClassAsync(int classId)
    {
        var selectedClass = await _dataService.GetClassByIdAsync(classId);
        if (selectedClass == null)
            return;

        var vm = new Tbl21ClassDetailViewModel(selectedClass, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl21ClassDetailPage), vm);
    }


}
