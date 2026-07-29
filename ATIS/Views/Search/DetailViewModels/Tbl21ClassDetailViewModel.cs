using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailPages;

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
    public ObservableCollection<Tbl03Regnum> RegnumItems { get; } = new();
    public ObservableCollection<Tbl06Phylum> PhylumItems { get; } = new();
    public ObservableCollection<Tbl12Subphylum> SubphylumItems { get; } = new();
    public ObservableCollection<Tbl18Superclass> SuperclassItems { get; } = new();
    public ObservableCollection<Tbl21Class> ClassItems { get; } = new();
    public ObservableCollection<Tbl24Subclass> SubclassItems { get; } = new();

    // -----------------------------
    //  REFERENCES
    // -----------------------------
    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = new();
    public ObservableCollection<BreadcrumbItem> Breadcrumb { get; } = new();

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
    public Tbl21ClassDetailViewModel(Tbl21Class selectedClass, IDataService? dataService)
    {
        ClassSelected = selectedClass;
        _dataService = dataService;

        InitializeAsync();

        ExportPdfCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExportPdf);

        // ⭐ BackCommand initialisieren
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }
    private async void InitializeAsync()
    {
        await LoadTaxonomy();        // WICHTIG: jetzt wird gewartet
        LoadDirectChildren();
        LoadReferences();
        LoadComments();

        BuildBreadcrumb();           // jetzt sind alle Selected-Objekte vorhanden
    }

    private void BuildBreadcrumb()
    {
        //Initialisierung der Breadcrumb-Navigation
        Breadcrumb.Clear();

        //Breadcrumb.Add(new BreadcrumbItem
        //{
        //    Label = RegnumSelected.RegnumName,
        //    NavigateCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => NavigateToRegnum(RegnumSelected.RegnumId))
        //});

        //Breadcrumb.Add(new BreadcrumbItem
        //{
        //    Label = PhylumSelected.PhylumName,
        //    NavigateCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => NavigateToPhylum(PhylumSelected.PhylumId))
        //});

        //Breadcrumb.Add(new BreadcrumbItem
        //{
        //    Label = SubphylumSelected.SubphylumName,
        //    NavigateCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => NavigateToSubphylum(SubphylumSelected.SubphylumId))
        //});

        //Breadcrumb.Add(new BreadcrumbItem
        //{
        //    Label = SuperclassSelected.SuperclassName,
        //    NavigateCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => NavigateToSuperclass(SuperclassSelected.SuperclassId))
        //});


        Breadcrumb.Add(new BreadcrumbItem
        {
            Label = ClassSelected.ClassName,
            NavigateCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => NavigateToClass(ClassSelected.ClassId))
        });

    }
    private void ExecuteBack()
    {
        var frame = App.MainPageInstance.NavFrame;

        if (frame.CanGoBack)
            frame.GoBack();
    }

    // -----------------------------
    //  LOAD TAXONOMY
    // -----------------------------
    private async Task LoadTaxonomy()
    {
        // -------------------------
        // CLASS → SUPERCLASS
        // -------------------------
        if (ClassSelected.SuperclassId != 0)
        {
            SuperclassSelected = await _dataService.GetSuperclassByIdAsync(ClassSelected.SuperclassId);
            if (SuperclassSelected != null)
                SuperclassItems.Add(SuperclassSelected);
        }

        // -------------------------
        // SUPERCLASS → SUBPHYLUM
        // -------------------------
        if (SuperclassSelected?.SubphylumId != 0)
        {
            SubphylumSelected = await _dataService.GetSubphylumByIdAsync(SuperclassSelected.SubphylumId);
            if (SubphylumSelected != null)
                SubphylumItems.Add(SubphylumSelected);
        }

        // -------------------------
        // SUBPHYLUM → PHYLUM
        // -------------------------
        if (SubphylumSelected?.PhylumId != 0)
        {
            PhylumSelected = await _dataService.GetPhylumByIdAsync(SubphylumSelected.PhylumId);
            if (PhylumSelected != null)
                PhylumItems.Add(PhylumSelected);
        }

        // -------------------------
        // PHYLUM → REGNUM
        // -------------------------
        if (PhylumSelected?.RegnumId != 0)
        {
            RegnumSelected = await _dataService.GetRegnumByIdAsync(PhylumSelected.RegnumId);
            if (RegnumSelected != null)
                RegnumItems.Add(RegnumSelected);
        }

        // -------------------------
        // CLASS (immer vorhanden)
        // -------------------------
        ClassItems.Add(ClassSelected);
    }

    // -----------------------------
    //  LOAD DIRECT CHILDREN (SUBCLASS)
    // -----------------------------
    private async void LoadDirectChildren()
    {
        var subclasses = await _dataService.GetSubclassesByClassIdAsync(ClassSelected.ClassId);
        foreach (var item in subclasses)
            SubclassItems.Add(item);
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
    private void ExportPdf()
    {
        // Hier später QuestPDF oder Syncfusion einbauen
    }

    // -----------------------------
    //  NAVIGATION (Hyperlink)
    // -----------------------------
    private void NavigateToClass(int classId)
    {
        var selectedClass = _dataService.GetClassByIdAsync(classId).Result;

        var vm = new Tbl21ClassDetailViewModel(selectedClass, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl21ClassDetailPage), vm);
    }
}
