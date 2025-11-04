using System.Configuration;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Report.Pdf.Tbl03Regnums;
using ATIS.Views.Report.Pdf.Tbl06Phylums;
using ATIS.Views.Report.Pdf.Tbl09Divisions;
using ATIS.Views.Report.Pdf.Tbl12Subphylums;
using ATIS.Views.Report.Pdf.Tbl15Subdivisions;
using ATIS.Views.Report.Pdf.Tbl18Superclasses;
using ATIS.Views.Report.Pdf.Tbl21Classes;
using ATIS.Views.Report.Pdf.Tbl24Subclasses;
using ATIS.Views.Report.Pdf.Tbl27Infraclasses;
using ATIS.Views.Report.Pdf.Tbl30Legios;
using ATIS.Views.Report.Pdf.Tbl33Ordos;
using ATIS.Views.Report.Pdf.Tbl36Subordos;
using ATIS.Views.Report.Pdf.Tbl39Infraordos;
using ATIS.Views.Report.Pdf.Tbl42Superfamilies;
using ATIS.Views.Report.Pdf.Tbl45Families;
using ATIS.Views.Report.Pdf.Tbl48Subfamilies;
using ATIS.Views.Report.Pdf.Tbl51Infrafamilies;
using ATIS.Views.Report.Pdf.Tbl54Supertribusses;
using ATIS.Views.Report.Pdf.Tbl57Tribusses;
using ATIS.Views.Report.Pdf.Tbl60Subtribusses;
using ATIS.Views.Report.Pdf.Tbl63Infratribusses;
using ATIS.Views.Report.Pdf.Tbl66Genusses;
using ATIS.Views.Report.Pdf.Tbl69FiSpeciesSubs;
using ATIS.Views.Report.Pdf.Tbl72PlSpeciesses;
using ATIS.Views.Report.Pdf.Tbl72PlSpeciesSubs;
using ATIS.Views.Report.Pdf.Tbl75Speciesses;
using ATIS.Views.Report.Pdf.Tbl75SpeciesSubs;
using ATIS.Views.Search.Pdf.Tbl03Regnums;
using ATIS.Views.Search.Pdf.Tbl06Phylums;
using ATIS.Views.Search.Pdf.Tbl09Divisions;
using ATIS.Views.Search.Pdf.Tbl12Subphylums;
using ATIS.Views.Search.Pdf.Tbl15Subdivisions;
using ATIS.Views.Search.Pdf.Tbl18Superclasses;
using ATIS.Views.Search.Pdf.Tbl21Classes;
using ATIS.Views.Search.Pdf.Tbl24Subclasses;
using ATIS.Views.Search.Pdf.Tbl27Infraclasses;
using ATIS.Views.Search.Pdf.Tbl30Legios;
using ATIS.Views.Search.Pdf.Tbl33Ordos;
using ATIS.Views.Search.Pdf.Tbl36Subordos;
using ATIS.Views.Search.Pdf.Tbl39Infraordos;
using ATIS.Views.Search.Pdf.Tbl42Superfamilies;
using ATIS.Views.Search.Pdf.Tbl45Families;
using ATIS.Views.Search.Pdf.Tbl48Subfamilies;
using ATIS.Views.Search.Pdf.Tbl51Infrafamilies;
using ATIS.Views.Search.Pdf.Tbl54Supertribusses;
using ATIS.Views.Search.Pdf.Tbl57Tribusses;
using ATIS.Views.Search.Pdf.Tbl60Subtribusses;
using ATIS.Views.Search.Pdf.Tbl63Infratribusses;
using ATIS.Views.Search.Pdf.Tbl66Genusses;
using ATIS.Views.Search.Pdf.Tbl75Speciesses;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Search;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SearchQuickPage : Page
{
    public SearchViewModel ViewModel { get; }
    private readonly AtisDbContext _context = new();

    public SearchQuickPage()
    {
        ViewModel = App.GetService<SearchViewModel>();
        InitializeComponent();
        //  QuestPDF.Settings.DocumentLayoutExceptionThreshold = 1000;
        QuestPDF.Settings.EnableCaching = true;
        QuestPDF.Settings.License = LicenseType.Community;
        LoadingProgressRing.IsActive = true;
        SimpleLog.Info(" SearchQuickPage aufgerufen ");
    }

    private void SearchAutoSuggestBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SearchAutoSuggestBox.Visibility = Visibility.Visible;
        SearchAutoSuggestBox.Focus(FocusState.Programmatic);

        if (App.SearchQuickString != "")
        {
            SearchAutoSuggestBox.Text = App.SearchQuickString;  // from MainPage to App filled
        }
        else
        {
            SearchAutoSuggestBox.Text = StringsRes.SearchName;
        }
    }
    private void GetNameByName(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel.SearchRun(SearchAutoSuggestBox.Text);
    }


    //----------------------Command PDF---------------------------------------------------
    public ICommand PdfRegnumCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportRegnumsWindow = new ReportRegnumsWindow(ViewModel.RegnumSelected.RegnumId);
        reportRegnumsWindow.GeneratePdfRegnum(ViewModel.RegnumSelected.RegnumId);
    });

    public ICommand PdfPhylumCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportPhylumsWindow = new ReportPhylumsWindow(ViewModel.PhylumSelected.PhylumId);
        reportPhylumsWindow.GeneratePdfPhylum(ViewModel.PhylumSelected.PhylumId);
    });

    public ICommand PdfDivisionCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportDivisionsWindow = new ReportDivisionsWindow(ViewModel.DivisionSelected.DivisionId);
        reportDivisionsWindow.GeneratePdfDivision(ViewModel.DivisionSelected.DivisionId);
    });

    public ICommand PdfSubphylumCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSubphylumsWindow = new ReportSubphylumsWindow(ViewModel.SubphylumSelected.SubphylumId);
        reportSubphylumsWindow.GeneratePdfSubphylum(ViewModel.SubphylumSelected.SubphylumId);
    });

    public ICommand PdfSubdivisionCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSubdivisionsWindow = new ReportSubdivisionsWindow(ViewModel.SubdivisionSelected.SubdivisionId);
        reportSubdivisionsWindow.GeneratePdfSubdivision(ViewModel.SubdivisionSelected.SubdivisionId);
    });

    public ICommand PdfSuperclassCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSuperclassesWindow = new ReportSuperclassesWindow(ViewModel.SuperclassSelected.SuperclassId);
        reportSuperclassesWindow.GeneratePdfSuperclass(ViewModel.SuperclassSelected.SuperclassId);
    });

    public ICommand PdfClassCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportClassesWindow = new ReportClassesWindow(ViewModel.ClassSelected.ClassId);
        reportClassesWindow.GeneratePdfClass(ViewModel.ClassSelected.ClassId);
    });

    public ICommand PdfSubclassCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSubclassesWindow = new ReportSubclassesWindow(ViewModel.SubclassSelected.SubclassId);
        reportSubclassesWindow.GeneratePdfSubclass(ViewModel.SubclassSelected.SubclassId);
    });

    public ICommand PdfInfraclassCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportInfraclassesWindow = new ReportInfraclassesWindow(ViewModel.InfraclassSelected.InfraclassId);
        reportInfraclassesWindow.GeneratePdfInfraclass(ViewModel.InfraclassSelected.InfraclassId);
    });

    public ICommand PdfLegioCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportLegiosWindow = new ReportLegiosWindow(ViewModel.LegioSelected.LegioId);
        reportLegiosWindow.GeneratePdfLegio(ViewModel.LegioSelected.LegioId);
    });

    public ICommand PdfOrdoCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportOrdosWindow = new ReportOrdosWindow(ViewModel.OrdoSelected.OrdoId);
        reportOrdosWindow.GeneratePdfOrdo(ViewModel.OrdoSelected.OrdoId);
    });

    public ICommand PdfSubordoCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSubordosWindow = new ReportSubordosWindow(ViewModel.SubordoSelected.SubordoId);
        reportSubordosWindow.GeneratePdfSubordo(ViewModel.SubordoSelected.SubordoId);
    });

    public ICommand PdfInfraordoCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportInfraordosWindow = new ReportInfraordosWindow(ViewModel.InfraordoSelected.InfraordoId);
        reportInfraordosWindow.GeneratePdfInfraordo(ViewModel.InfraordoSelected.InfraordoId);
    });

    public ICommand PdfSuperfamilyCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSuperfamiliesWindow = new ReportSuperfamiliesWindow(ViewModel.SuperfamilySelected.SuperfamilyId);
        reportSuperfamiliesWindow.GeneratePdfSuperfamily(ViewModel.SuperfamilySelected.SuperfamilyId);
    });

    public ICommand PdfFamilyCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportFamiliesWindow = new ReportFamiliesWindow(ViewModel.FamilySelected.FamilyId);
        reportFamiliesWindow.GeneratePdfFamily(ViewModel.FamilySelected.FamilyId);
    });

    public ICommand PdfSubfamilyCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSubfamiliesWindow = new ReportSubfamiliesWindow(ViewModel.SubfamilySelected.SubfamilyId);
        reportSubfamiliesWindow.GeneratePdfSubfamily(ViewModel.SubfamilySelected.SubfamilyId);
    });

    public ICommand PdfInfrafamilyCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportInfrafamiliesWindow = new ReportInfrafamiliesWindow(ViewModel.InfrafamilySelected.InfrafamilyId);
        reportInfrafamiliesWindow.GeneratePdfInfrafamily(ViewModel.InfrafamilySelected.InfrafamilyId);
    });

    public ICommand PdfSupertribusCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSupertribussesWindow = new ReportSupertribussesWindow(ViewModel.SupertribusSelected.SupertribusId);
        reportSupertribussesWindow.GeneratePdfSupertribus(ViewModel.SupertribusSelected.SupertribusId);
    });

    public ICommand PdfTribusCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportTribussesWindow = new ReportTribussesWindow(ViewModel.TribusSelected.TribusId);
        reportTribussesWindow.GeneratePdfTribus(ViewModel.TribusSelected.TribusId);
    });

    public ICommand PdfSubtribusCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportSubtribussesWindow = new ReportSubtribussesWindow(ViewModel.SubtribusSelected.SubtribusId);
        reportSubtribussesWindow.GeneratePdfSubtribus(ViewModel.SubtribusSelected.SubtribusId);
    });

    public ICommand PdfInfratribusCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportInfratribussesWindow = new ReportInfratribussesWindow(ViewModel.InfratribusSelected.InfratribusId);
        reportInfratribussesWindow.GeneratePdfInfratribus(ViewModel.InfratribusSelected.InfratribusId);
    });

    public ICommand PdfGenusCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportGenussesWindow = new ReportGenussesWindow(ViewModel.GenusSelected.GenusId);
        reportGenussesWindow.GeneratePdfGenus(ViewModel.GenusSelected.GenusId);
    });

    public ICommand PdfSpeciesCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        //var reportFiSpeciessesWindow = new ReportFiSpeciessesWindow(ViewModel.FiSpeciesSelected.FiSpeciesId);
        //reportFiSpeciessesWindow.GeneratePdfFiSpecies(ViewModel.FiSpeciesSelected.FiSpeciesId);
        var reportSpeciessesWindow = new ReportSpeciessesWindow(ViewModel.SpeciesSelected.SpeciesId);
        reportSpeciessesWindow.GeneratePdfSpecies(ViewModel.SpeciesSelected.SpeciesId);
    });

    public ICommand PdfFiSpeciesSubCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportFiSpeciessesSubsWindow = new ReportFiSpeciessesSubsWindow(ViewModel.SpeciesSubSelected.SpeciesId);
        reportFiSpeciessesSubsWindow.GeneratePdfFiSpeciesSubs(ViewModel.SpeciesSubSelected.SpeciesId);
    });

    public ICommand PdfSpeciesSubCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        //var reportFiSpeciessesSubsWindow = new ReportFiSpeciessesSubsWindow(ViewModel.FiSpeciesSubSelected.FiSpeciesId);
        //reportFiSpeciessesSubsWindow.GeneratePdfFiSpeciesSubs(ViewModel.FiSpeciesSubSelected.FiSpeciesId);
        var reportSpeciessesSubsWindow = new ReportSpeciessesSubsWindow(ViewModel.SpeciesSubSelected.SpeciesId);
        reportSpeciessesSubsWindow.GeneratePdfSpeciesSubs(ViewModel.SpeciesSubSelected.SpeciesId);
    });

    //public ICommand PdfPlSpeciesCommand => new WinUICommunity.RelayCommand(execute: delegate
    //{
    //    var reportPlSpeciessesWindow = new ReportPlSpeciessesWindow(ViewModel.PlSpeciesSelected.PlSpeciesId);
    //    reportPlSpeciessesWindow.GeneratePdfPlSpecies(ViewModel.PlSpeciesSelected.PlSpeciesId);
    //});

    //public ICommand PdfPlSpeciesSubCommand => new WinUICommunity.RelayCommand(execute: delegate
    //{
    //    var reportPlSpeciessesSubsWindow = new ReportPlSpeciessesSubsWindow(ViewModel.PlSpeciesSubSelected.PlSpeciesId);
    //    reportPlSpeciessesSubsWindow.GeneratePdfPlSpeciesSubs(ViewModel.PlSpeciesSubSelected.PlSpeciesId);
    //});

    public object PdfListPrintCommand { get; } = new();

    //-------------------------------------------Print List PDF-----

    public async void PdfPrintListAll(object sender, RoutedEventArgs e)
    {
      //  if (string.IsNullOrEmpty(ViewModel.FilterText))
        if (string.IsNullOrEmpty(SearchAutoSuggestBox.Text))
            
        {
            await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
            SimpleLog.Warning(" Kein Datensatz gefunden ");
            return;
        }

        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");

        switch (ViewModel.SelectedMainTabIndex)
        {
            case 8:
                {
                    filePath = $"{filePath}PdfListRegnum.pdf";
                    {
                        if (ViewModel.RegnumsCollection.Count != 0)
                        {
                            var model =
                                Tbl03RegnumsListDocumentDataSource.GetRegnumListDetails(ViewModel.RegnumsCollection);
                            var document = new Tbl03RegnumsListDocument(model, ViewModel.RegnumsCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 7 when ViewModel.SelectedPhylumTabIndex == 0:
                {
                    filePath = $"{filePath}PdfListSubphylum.pdf";
                    {
                        if (ViewModel.SubphylumsCollection.Count != 0)
                        {
                            var model =
                                Tbl12SubphylumsListDocumentDataSource.GetSubphylumListDetails(
                                    ViewModel.SubphylumsCollection);
                            var document = new Tbl12SubphylumsListDocument(model, ViewModel.SubphylumsCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 7 when ViewModel.SelectedPhylumTabIndex == 1:
                {
                    filePath = $"{filePath}PdfListPhylum.pdf";
                    {
                        if (ViewModel.PhylumsCollection.Count != 0)
                        {
                            var model =
                                Tbl06PhylumsListDocumentDataSource.GetPhylumListDetails(ViewModel.PhylumsCollection);
                            var document = new Tbl06PhylumsListDocument(model, ViewModel.PhylumsCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 6 when ViewModel.SelectedDivisionTabIndex == 0:
                {
                    filePath = $"{filePath}PdfListSubdivision.pdf";
                    {
                        if (ViewModel.SubdivisionsCollection.Count != 0)
                        {
                            var model =
                                Tbl15SubdivisionsListDocumentDataSource.GetSubdivisionListDetails(ViewModel
                                    .SubdivisionsCollection);
                            var document = new Tbl15SubdivisionsListDocument(model, ViewModel.SubdivisionsCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 6 when ViewModel.SelectedDivisionTabIndex == 1:
                {
                    filePath = $"{filePath}PdfListDivision.pdf";
                    {
                        if (ViewModel.DivisionsCollection.Count != 0)
                        {
                            var model =
                                Tbl09DivisionsListDocumentDataSource.GetDivisionListDetails(ViewModel.DivisionsCollection);
                            var document = new Tbl09DivisionsListDocument(model, ViewModel.DivisionsCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 5 when ViewModel.SelectedClassTabIndex == 0:
                {
                    filePath = $"{filePath}PdfListInfraclass.pdf";
                    {
                        if (ViewModel.InfraclassesCollection.Count != 0)
                        {
                            var model =
                                Tbl27InfraclassesListDocumentDataSource.GetInfraclassListDetails(ViewModel
                                    .InfraclassesCollection);
                            var document = new Tbl27InfraclassesListDocument(model, ViewModel.InfraclassesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 5 when ViewModel.SelectedClassTabIndex == 1:
                {
                    filePath = $"{filePath}PdfListSubclass.pdf";
                    {
                        if (ViewModel.SubclassesCollection.Count != 0)
                        {
                            var model =
                                Tbl24SubclassesListDocumentDataSource
                                    .GetSubclassListDetails(ViewModel.SubclassesCollection);
                            var document = new Tbl24SubclassesListDocument(model, ViewModel.SubclassesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 5 when ViewModel.SelectedClassTabIndex == 2:
                {
                    filePath = $"{filePath}PdfListClass.pdf";
                    {
                        if (ViewModel.ClassesCollection.Count != 0)
                        {
                            var model = Tbl21ClassesListDocumentDataSource.GetClassListDetails(ViewModel.ClassesCollection);
                            var document = new Tbl21ClassesListDocument(model, ViewModel.ClassesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 5 when ViewModel.SelectedClassTabIndex == 3:
                {
                    filePath = $"{filePath}PdfListSuperclass.pdf";
                    {
                        if (ViewModel.SuperclassesCollection.Count != 0)
                        {
                            var model =
                                Tbl18SuperclassesListDocumentDataSource.GetSuperclassListDetails(ViewModel
                                    .SuperclassesCollection);
                            var document = new Tbl18SuperclassesListDocument(model, ViewModel.SuperclassesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 4 when ViewModel.SelectedOrdoTabIndex == 0:
                {
                    filePath = $"{filePath}PdfListInfraordo.pdf";
                    {
                        if (ViewModel.InfraordosCollection.Count != 0)
                        {
                            var model =
                                Tbl39InfraordosListDocumentDataSource.GetInfraordoListDetails(
                                    ViewModel.InfraordosCollection);
                            var document = new Tbl39InfraordosListDocument(model, ViewModel.InfraordosCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 4 when ViewModel.SelectedOrdoTabIndex == 1:
                {
                    filePath = $"{filePath}PdfListSubordo.pdf";
                    {
                        if (ViewModel.SubordosCollection.Count != 0)
                        {
                            var model =
                                Tbl36SubordosListDocumentDataSource.GetSubordoListDetails(ViewModel.SubordosCollection);
                            var document = new Tbl36SubordosListDocument(model, ViewModel.SubordosCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 4 when ViewModel.SelectedOrdoTabIndex == 2:
                {
                    filePath = $"{filePath}PdfListOrdo.pdf";
                    {
                        if (ViewModel.OrdosCollection.Count != 0)
                        {
                            var model = Tbl33OrdosListDocumentDataSource.GetOrdoListDetails(ViewModel.OrdosCollection);
                            var document = new Tbl33OrdosListDocument(model, ViewModel.OrdosCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 4 when ViewModel.SelectedOrdoTabIndex == 3:
                {
                    filePath = $"{filePath}PdfListLegio.pdf";
                    {
                        if (ViewModel.LegiosCollection.Count != 0)
                        {
                            var model = Tbl30LegiosListDocumentDataSource.GetLegioListDetails(ViewModel.LegiosCollection);
                            var document = new Tbl30LegiosListDocument(model, ViewModel.LegiosCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 3 when ViewModel.SelectedFamilyTabIndex == 0:
                {
                    filePath = $"{filePath}PdfListInfrafamily.pdf";
                    {
                        if (ViewModel.InfrafamiliesCollection.Count != 0)
                        {
                            var model =
                                Tbl51InfrafamiliesListDocumentDataSource.GetInfrafamilyListDetails(ViewModel
                                    .InfrafamiliesCollection);
                            var document = new Tbl51InfrafamiliesListDocument(model, ViewModel.InfrafamiliesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 3 when ViewModel.SelectedFamilyTabIndex == 1:
                {
                    filePath = $"{filePath}PdfListSubfamily.pdf";
                    {
                        if (ViewModel.SubfamiliesCollection.Count != 0)
                        {
                            var model =
                                Tbl48SubfamiliesListDocumentDataSource.GetSubfamilyListDetails(ViewModel
                                    .SubfamiliesCollection);
                            var document = new Tbl48SubfamiliesListDocument(model, ViewModel.SubfamiliesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 3 when ViewModel.SelectedFamilyTabIndex == 2:
                {
                    filePath = $"{filePath}PdfListFamily.pdf";
                    {
                        if (ViewModel.FamiliesCollection.Count != 0)
                        {
                            var model =
                                Tbl45FamiliesListDocumentDataSource.GetFamilyListDetails(ViewModel.FamiliesCollection);
                            var document = new Tbl45FamiliesListDocument(model, ViewModel.FamiliesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 3 when ViewModel.SelectedFamilyTabIndex == 3:
                {
                    filePath = $"{filePath}PdfListSuperfamily.pdf";
                    {
                        if (ViewModel.SuperfamiliesCollection.Count != 0)
                        {
                            var model =
                                Tbl42SuperfamiliesListDocumentDataSource.GetSuperfamilyListDetails(ViewModel
                                    .SuperfamiliesCollection);
                            var document = new Tbl42SuperfamiliesListDocument(model, ViewModel.SuperfamiliesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 2 when ViewModel.SelectedTribusTabIndex == 0:
                {
                    filePath = $"{filePath}PdfListInfratribus.pdf";
                    {
                        if (ViewModel.InfratribussesCollection.Count != 0)
                        {
                            var model =
                                Tbl63InfratribussesListDocumentDataSource.GetInfratribusListDetails(ViewModel
                                    .InfratribussesCollection);
                            var document = new Tbl63InfratribussesListDocument(model, ViewModel.InfratribussesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 2 when ViewModel.SelectedTribusTabIndex == 1:
                {
                    filePath = $"{filePath}PdfListSubtribus.pdf";
                    {
                        if (ViewModel.SubtribussesCollection.Count != 0)
                        {
                            var model =
                                Tbl60SubtribussesListDocumentDataSource.GetSubtribusListDetails(ViewModel
                                    .SubtribussesCollection);
                            var document = new Tbl60SubtribussesListDocument(model, ViewModel.SubtribussesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 2 when ViewModel.SelectedTribusTabIndex == 2:
                {
                    filePath = $"{filePath}PdfListTribus.pdf";
                    {
                        if (ViewModel.TribussesCollection.Count != 0)
                        {
                            var model =
                                Tbl57TribussesListDocumentDataSource.GetTribusListDetails(ViewModel.TribussesCollection);
                            var document = new Tbl57TribussesListDocument(model, ViewModel.TribussesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            case 2 when ViewModel.SelectedTribusTabIndex == 3:
                {
                    filePath = $"{filePath}PdfListSupertribus.pdf";
                    {
                        if (ViewModel.SupertribussesCollection.Count != 0)
                        {
                            var model =
                                Tbl54SupertribussesListDocumentDataSource.GetSupertribusListDetails(ViewModel
                                    .SupertribussesCollection);
                            var document = new Tbl54SupertribussesListDocument(model, ViewModel.SupertribussesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
            //case 2:
            //    {
            //        filePath = $"{filePath}PdfListPLSpecies.pdf";
            //        {
            //            if (ViewModel.PlSpeciessesCollection.Count != 0)
            //            {
            //                var model =
            //                    Tbl72PlSpeciessesListDocumentDataSource.GetPlSpeciesListDetails(ViewModel
            //                        .PlSpeciessesCollection);
            //                var document = new Tbl72PlSpeciessesListDocument(model, ViewModel.PlSpeciessesCollection);
            //                document.GeneratePdf(filePath);
            //            }
            //        }
            //        break;
            //    }
            case 1:
            {
                filePath = $"{filePath}PdfListSpecies.pdf";
                {
                    if (ViewModel.SpeciessesCollection.Count != 0)
                    {
                        var model =
                            Tbl75SpeciessesListDocumentDataSource.GetSpeciesListDetails(ViewModel.SpeciessesCollection);
                        var document = new Tbl75SpeciessesListDocument(model, ViewModel.SpeciessesCollection);
                        document.GeneratePdf(filePath);
                    }
                }
                break;
            }

            //{
            //    filePath = $"{filePath}PdfListFiSpecies.pdf";
            //    {
            //        if (ViewModel.FiSpeciessesCollection.Count != 0)
            //        {
            //            var model =
            //                Tbl69FiSpeciessesListDocumentDataSource.GetFiSpeciesListDetails(ViewModel
            //                    .FiSpeciessesCollection);
            //            var document = new Tbl69FiSpeciessesListDocument(model, ViewModel.FiSpeciessesCollection);
            //            document.GeneratePdf(filePath);
            //        }
            //    }
            //    break;
            //}
            case 0:
                {
                    filePath = $"{filePath}PdfListGenus.pdf";
                    {
                        if (ViewModel.GenussesCollection.Count != 0)
                        {
                            var model =
                                Tbl66GenussesListDocumentDataSource.GetGenusListDetails(ViewModel.GenussesCollection);
                            var document = new Tbl66GenussesListDocument(model, ViewModel.GenussesCollection);
                            document.GeneratePdf(filePath);
                        }
                    }
                    break;
                }
        }

        PrintPdfMethod(filePath);
    }

    private static async void PrintPdfMethod(string filePath)
    {
        // Process.Start("explorer.exe", filePath);

        {
            try
            {
                await File.ReadAllTextAsync(filePath!);
            }
            catch (DirectoryNotFoundException)
            {
                AllDialogs.NoFileFoundInfoWarnDialogAsync();
                SimpleLog.Warning(" Kein Directory gefunden ");
                return;
            }
            catch (FileNotFoundException)
            {
                AllDialogs.NoFileFoundInfoWarnDialogAsync();
                SimpleLog.Warning(" Keine Datei gefunden ");
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath!)
                {
                    UseShellExecute = true
                }
            };
            process.Start();
        }
    }

    //---------------------------DataContextMenu_Click  -------------
    private void RegnumSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.RegnumSelected.RegnumId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportRegnumsWindow(ViewModel.RegnumSelected.RegnumId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void PhylumSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.PhylumSelected.PhylumId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportPhylumsWindow(ViewModel.PhylumSelected.PhylumId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void DivisionSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.DivisionSelected.DivisionId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportDivisionsWindow(ViewModel.DivisionSelected.DivisionId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SubphylumSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SubphylumSelected.SubphylumId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSubphylumsWindow(ViewModel.SubphylumSelected.SubphylumId)
        {
            IsAlwaysOnTop = true
        };
        window.Activate();
    }

    private void SubdivisionSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SubdivisionSelected.SubdivisionId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSubdivisionsWindow(ViewModel.SubdivisionSelected.SubdivisionId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SuperclassSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SuperclassSelected.SuperclassId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSuperclassesWindow(ViewModel.SuperclassSelected.SuperclassId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void ClassSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.ClassSelected.ClassId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportClassesWindow(ViewModel.ClassSelected.ClassId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SubclassSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SubclassSelected.SubclassId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSubclassesWindow(ViewModel.SubclassSelected.SubclassId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void InfraclassSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.InfraclassSelected.InfraclassId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportInfraclassesWindow(ViewModel.InfraclassSelected.InfraclassId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void LegioSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.LegioSelected.LegioId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportLegiosWindow(ViewModel.LegioSelected.LegioId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void OrdoSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.OrdoSelected.OrdoId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportOrdosWindow(ViewModel.OrdoSelected.OrdoId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SubordoSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SubordoSelected.SubordoId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSubordosWindow(ViewModel.SubordoSelected.SubordoId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void InfraordoSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.InfraordoSelected.InfraordoId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportInfraordosWindow(ViewModel.InfraordoSelected.InfraordoId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SuperfamilySelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SuperfamilySelected.SuperfamilyId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSuperfamiliesWindow(ViewModel.SuperfamilySelected.SuperfamilyId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void FamilySelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.FamilySelected.FamilyId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportFamiliesWindow(ViewModel.FamilySelected.FamilyId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SubfamilySelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SubfamilySelected.SubfamilyId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSubfamiliesWindow(ViewModel.SubfamilySelected.SubfamilyId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void InfrafamilySelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.InfrafamilySelected.InfrafamilyId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportInfrafamiliesWindow(ViewModel.InfrafamilySelected.InfrafamilyId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SupertribusSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SupertribusSelected.SupertribusId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSupertribussesWindow(ViewModel.SupertribusSelected.SupertribusId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void TribusSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.TribusSelected.TribusId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportTribussesWindow(ViewModel.TribusSelected.TribusId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SubtribusSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SubtribusSelected.SubtribusId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportSubtribussesWindow(ViewModel.SubtribusSelected.SubtribusId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void InfratribusSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.InfratribusSelected.InfratribusId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportInfratribussesWindow(ViewModel.InfratribusSelected.InfratribusId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void GenusSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.GenusSelected.GenusId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportGenussesWindow(ViewModel.GenusSelected.GenusId);
        window.IsAlwaysOnTop = true;
        window.Activate();
    }

    private void SpeciesSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SpeciesSelected.SpeciesId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }
        if (string.IsNullOrEmpty(ViewModel.SpeciesSelected.Subspecies))
        {

            //   var window = new ReportFiSpeciessesWindow(ViewModel.FiSpeciesSelected.FiSpeciesId);
            var window = new ReportSpeciessesWindow(ViewModel.SpeciesSelected.SpeciesId);
            window.IsAlwaysOnTop = true;
            window.Activate();
        }
        else
        {
            //  var window = new ReportFiSpeciessesSubsWindow(ViewModel.FiSpeciesSelected.FiSpeciesId);
            var window = new ReportSpeciessesSubsWindow(ViewModel.SpeciesSelected.SpeciesId);
            window.IsAlwaysOnTop = true;
            window.Activate();
        }
    }

    private void PlSpeciesSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SpeciesSelected.SpeciesId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }
        if (string.IsNullOrEmpty(ViewModel.SpeciesSelected.Subspecies))
        {
            var window = new ReportPlSpeciessesWindow(ViewModel.SpeciesSelected.SpeciesId);
            window.IsAlwaysOnTop = true;
            window.Activate();
        }
        else
        {
            var window = new ReportPlSpeciessesSubsWindow(ViewModel.SpeciesSelected.SpeciesId);
            window.IsAlwaysOnTop = true;
            window.Activate();
        }
    }

}
