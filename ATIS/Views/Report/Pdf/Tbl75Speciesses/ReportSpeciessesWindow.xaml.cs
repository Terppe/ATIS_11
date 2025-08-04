using System.Configuration;
using System.Diagnostics;
using ATIS.Helpers;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.UI;
using QuestPDF.Fluent;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Report.Pdf.Tbl75Speciesses;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportSpeciessesWindow : WindowEx
{
    #region [Private Data Members]

    private readonly AtisDbContext _context = new();
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]  

    #region [Constructor]

    public ReportSpeciessesWindow(int id)
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
        ViewModel.ReportSearchAll(id, "Tbl75Speciesses");

        Title = StringsRes.Species;
        Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
        Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
        this.CenterOnScreen();

        if (ViewModel.FiSpeciesSubItems.Count == 0)
        {
            TbDirectChildren.Visibility = Visibility.Collapsed;
        }
    }
    #endregion [Constructor]                 

    #region [Command]

    private void PdfSpeciesCommand(object sender, RoutedEventArgs e)
    {
        GeneratePdfSpecies(ViewModel.FiSpeciesSelected.FiSpeciesId);
    }

    #endregion [Command]             

    #region [Methods]

    public void GeneratePdfSpecies(int id)
    {
        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");
        var single = _context.Tbl69FiSpeciesses.SingleOrDefault(a => a.FiSpeciesId == id);
        if (single != null)
        {
            var name = single.FiSpeciesName;
            var countid = single.CountId;
            const string space = "_";
            filePath = $"{filePath}{name}{space}{countid}{space}Report.pdf";

            //var filePath = "D:\\Temp\\Animalia b11_57883426_Report.pdf";

            var model = Tbl75SpeciessesDocumentDataSource.GetSpeciesDetails(id);
            var document = new Tbl75SpeciessesDocument(model);
            document.GeneratePdf(filePath);
            PrintPdfMethod(filePath);
        }
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
                return;
            }
            catch (FileNotFoundException)
            {
                AllDialogs.NoFileFoundInfoWarnDialogAsync();
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

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        if (appWindow != null)
        {
            App.WindowsCounter -= 1;  //counter 1 window back
            appWindow.Hide();
        }
    }

    ////Tbl03Regnums  -->
    //public void Regnum_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.RegnumSelected.RegnumId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.RegnumSelected.RegnumId, "Tbl03Regnums");
    //    var window = new Tbl03Regnums.ReportRegnumsWindow(ViewModel.RegnumSelected.RegnumId);
    //    window.Activate();
    //}

    ////Tbl06Phylums  -->
    //public void Phylum_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.PhylumSelected.PhylumId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.PhylumSelected.PhylumId, "Tbl06Phylums");
    //    var window = new Tbl06Phylums.ReportPhylumsWindow(ViewModel.PhylumSelected.PhylumId);
    //    window.Activate();
    //}

    //// Tbl09Divisions  -->
    //public void Division_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.DivisionSelected.DivisionId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.DivisionSelected.DivisionId, "Tbl09Divisions");
    //    var window = new Tbl09Divisions.ReportDivisionsWindow(ViewModel.DivisionSelected.DivisionId);
    //    window.Activate();
    //}

    ////Tbl12Subphylums  -->
    //private void Subphylum_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SubphylumSelected.SubphylumId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SubphylumSelected.SubphylumId, "Tbl12Subphylums");
    //    var window = new Tbl12Subphylums.ReportSubphylumsWindow(ViewModel.SubphylumSelected.SubphylumId);
    //    window.Activate();
    //}

    //// Tbl15Subdivisions  -->
    //private void Subdivision_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SubdivisionSelected.SubdivisionId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SubdivisionSelected.SubdivisionId, "Tbl15Subdivisions");
    //    var window = new Tbl15Subdivisions.ReportSubdivisionsWindow(ViewModel.SubdivisionSelected.SubdivisionId);
    //    window.Activate();
    //}

    //// Tbl18Superclasses  -->
    //private void Superclass_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SuperclassSelected.SuperclassId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SuperclassSelected.SuperclassId, "Tbl18Superclasses");
    //    var window = new Tbl18Superclasses.ReportSuperclassesWindow(ViewModel.SuperclassSelected.SuperclassId);
    //    window.Activate();
    //}

    //// Tbl21Classes  -->
    //private void Class_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.ClassSelected.ClassId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.ClassSelected.ClassId, "Tb21Classes");
    //    var window = new Tbl21Classes.ReportClassesWindow(ViewModel.ClassSelected.ClassId);
    //    window.Activate();
    //}

    //// Tbl24Subclasses  -->
    //private void Subclass_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SubclassSelected.SubclassId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SubclassSelected.SubclassId, "Tbl24Subclasses");
    //    var window = new Tbl24Subclasses.ReportSubclassesWindow(ViewModel.SubclassSelected.SubclassId);
    //    window.Activate();
    //}

    //// Tbl27Infraclasses  -->
    //private void Infraclass_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.InfraclassSelected.InfraclassId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.InfraclassSelected.InfraclassId, "Tbl27Infraclasses");
    //    var window = new Tbl27Infraclasses.ReportInfraclassesWindow(ViewModel.InfraclassSelected.InfraclassId);
    //    window.Activate();
    //}
    //// Tbl30Legios  -->
    //private void Legio_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.LegioSelected.LegioId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.LegioSelected.LegioId, "Tbl30Legios");
    //    var window = new Tbl30Legios.ReportLegiosWindow(ViewModel.LegioSelected.LegioId);
    //    window.Activate();
    //}
    //// Tbl33Ordos  -->
    //private void Ordo_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.OrdoSelected.OrdoId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.OrdoSelected.OrdoId, "Tbl33Ordos");
    //    var window = new Tbl33Ordos.ReportOrdosWindow(ViewModel.OrdoSelected.OrdoId);
    //    window.Activate();
    //}
    //// Tbl36Subordos  -->
    //private void Subordo_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SubordoSelected.SubordoId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SubordoSelected.SubordoId, "Tbl36Subordos");
    //    var window = new Tbl36Subordos.ReportSubordosWindow(ViewModel.SubordoSelected.SubordoId);
    //    window.Activate();
    //}
    //// Tbl39Infraordos  -->
    //private void Infraordo_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.InfraordoSelected.InfraordoId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.InfraordoSelected.InfraordoId, "Tbl39Infraordos");
    //    var window = new Tbl39Infraordos.ReportInfraordosWindow(ViewModel.InfraordoSelected.InfraordoId);
    //    window.Activate();
    //}
    //// Tbl42Superfamilies  -->
    //private void Superfamily_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SuperfamilySelected.SuperfamilyId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SuperfamilySelected.SuperfamilyId, "Tbl42Superfamilies");
    //    var window = new Tbl42Superfamilies.ReportSuperfamiliesWindow(ViewModel.SuperfamilySelected.SuperfamilyId);
    //    window.Activate();
    //}
    //// Tbl45Families  -->
    //private void Family_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.FamilySelected.FamilyId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.FamilySelected.FamilyId, "Tbl45Families");
    //    var window = new Tbl45Families.ReportFamiliesWindow(ViewModel.FamilySelected.FamilyId);
    //    window.Activate();
    //}
    //// Tbl48Subfamilies  -->
    //private void Subfamily_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SubfamilySelected.SubfamilyId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SubfamilySelected.SubfamilyId, "Tbl48Subfamilies");
    //    var window = new Tbl48Subfamilies.ReportSubfamiliesWindow(ViewModel.SubfamilySelected.SubfamilyId);
    //    window.Activate();
    //}
    //// Tbl51Infrafamilies  -->
    //private void Infrafamily_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.InfrafamilySelected.InfrafamilyId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.InfrafamilySelected.InfrafamilyId, "Tbl51Infrafamilies");
    //    var window = new Tbl51Infrafamilies.ReportInfrafamiliesWindow(ViewModel.InfrafamilySelected.InfrafamilyId);
    //    window.Activate();
    //}
    //// Tbl54Supertribusses
    //private void Supertribus_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SupertribusSelected.SupertribusId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SupertribusSelected.SupertribusId, "Tbl54Supertribusses");
    //    var window = new Tbl54Supertribusses.ReportSupertribussesWindow(ViewModel.SupertribusSelected.SupertribusId);
    //    window.Activate();
    //}
    //// Tbl57Tribusses
    //private void Tribus_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.TribusSelected.TribusId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.TribusSelected.TribusId, "Tbl57Tribusses");
    //    var window = new Tbl57Tribusses.ReportTribussesWindow(ViewModel.TribusSelected.TribusId);
    //    window.Activate();
    //}
    //// Tbl60Subtribusses
    //private void Subtribus_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SubtribusSelected.SubtribusId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SubtribusSelected.SubtribusId, "Tbl60Subtribusses");
    //    var window = new Tbl60Subtribusses.ReportSubtribussesWindow(ViewModel.SubtribusSelected.SubtribusId);
    //    window.Activate();
    //}
    //// Tbl63Infratribusses
    //private void Infratribus_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.InfratribusSelected.InfratribusId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.InfratribusSelected.InfratribusId, "Tbl63Infratribusses");
    //    var window = new Tbl63Infratribusses.ReportInfratribussesWindow(ViewModel.InfratribusSelected.InfratribusId);
    //    window.Activate();
    //}
    //// Tbl66Genusses
    //private void Genus_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.GenusSelected.GenusId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.GenusSelected.GenusId, "Tbl66Genusses");
    //    var window = new Tbl66Genusses.ReportGenussesWindow(ViewModel.GenusSelected.GenusId);
    //    window.Activate();
    //}
    ////Tbl69FiSpeciesses
    //private void FiSpecies_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.FiSpeciesSelected.FiSpeciesId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.FiSpeciesSelected.FiSpeciesId, "Tbl69FiSpeciesses");
    //    var window = new Tbl69FiSpeciesses.ReportFiSpeciessesWindow(ViewModel.FiSpeciesSelected.FiSpeciesId);
    //    window.Activate();
    //}

    //private void FiSpeciesSub_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.FiSpeciesSelected.FiSpeciesId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.FiSpeciesSelected.FiSpeciesId, "Tbl69FiSpeciesses");
    //    var window = new Tbl69FiSpeciesSubs.ReportFiSpeciessesSubsWindow(ViewModel.FiSpeciesSelected.FiSpeciesId);
    //    window.Activate();
    //}

    #endregion [Methods]
}
