using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Taxonomy;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class Tbl06PhylumDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Phylum: {PhylumSelected?.PhylumName}";

    // -----------------------------
    //  SELECTED PHYLUM
    // -----------------------------
    public Tbl06Phylum PhylumSelected { get; set; }

    // -----------------------------
    //  TAXONOMY OBJECTS
    // -----------------------------
    public Tbl03Regnum? RegnumSelected { get; private set; }
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
    public Tbl06PhylumDetailViewModel(Tbl06Phylum selectedPhylum, IDataService? dataService)
    {
        PhylumSelected = selectedPhylum;
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

        // <<< HIER: aktuellen Knoten entfernen
        Breadcrumb = new ObservableCollection<BreadcrumbItem>(
            Breadcrumb.Where(x => !(x.EntityType == "Phylum" && x.EntityId == PhylumSelected.PhylumId)));

    }

    public async Task NavigateBreadcrumbAsync(BreadcrumbItem item)
    {
        switch (item.EntityType)
        {
            case "Regnum":
                App.MainPageInstance.NavFrame.Navigate(typeof(Tbl03RegnumDetailPage), item.EntityId);
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

        Tbl03Regnum? regnum = null;

        // technisch laden
        if (PhylumSelected.RegnumId != 0)
            regnum = await _dataService.GetRegnumByIdAsync(PhylumSelected.RegnumId);

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

        PhylumItems.Add(new TaxonomyLine<Tbl06Phylum>
        {
            Item = PhylumSelected,
            HeaderMargin = CreateHeaderMargin(visibleIndex++),
            ShowAsHyperlink = false
        });
    }
    // -----------------------------
    //  LOAD DIRECT CHILDREN (SUBPHYLUM)
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

        var subphylum = await _dataService.GetSubphylumsByPhylumIdAsync(PhylumSelected.PhylumId);
        HasDirectChildren = false;

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
