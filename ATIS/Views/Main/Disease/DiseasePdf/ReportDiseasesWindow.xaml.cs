using System.Diagnostics;
using ATIS.Helpers;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Report;
using Microsoft.UI;
using System.Configuration;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main.DiseasePdf;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportDiseasesWindow : WindowEx
{

    #region [Private Data Members]
    private readonly AtisDbContext _context = new();
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public ReportDiseasesWindow(int diseaseid)
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
        ViewModel.ReportSearchAll(diseaseid, "TblDiseases");

        Title = StringsRes.Regnum;
        Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
        Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
        this.CenterOnScreen();

        //if (ViewModel.DiseaseItems.Count == 0)
        //{
        //    TbDirectChildren.Visibility = Visibility.Collapsed;
        //}
    }
    #endregion [Constructor]   

    #region [Command]

    private void PdfDiseaseCommand(object sender, RoutedEventArgs e)
    {
        GeneratePdfDisease(ViewModel.DiseaseSelected.DiseaseId);
    }

    #endregion [Command]             

    #region [Methods]

    public void GeneratePdfDisease(int id)
    {
        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");
        var single = _context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id);
        if (single != null)
        {
            //var name = single.RegnumName;
            //var subName = single.Subregnum;
            //var countid = single.CountId;
            //const string space = "_";
            //filePath = $"{filePath}{name}{subName}{space}{countid}{space}Report.pdf";

            ////var filePath = "D:\\Temp\\Animalia b11_57883426_Report.pdf";

            //var model = Tbl03RegnumsDocumentDataSource.GetRegnumDetails(id);
            //var document = new Tbl03RegnumsDocument(model);
            //document.GeneratePdf(filePath);
            //PrintPdfMethod(filePath);
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

    ////Phylums
    //private void Phylum_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.PhylumSelected.PhylumId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.PhylumSelected.PhylumId, "Tbl06Phylums");
    //    var window = new Tbl06Phylums.ReportPhylumsWindow(ViewModel.PhylumSelected.PhylumId);
    //    window.Activate();
    //}
    ////Divisions
    //private void Division_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.DivisionSelected.DivisionId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.DivisionSelected.DivisionId, "Tbl09Divisions");
    //    var window = new Tbl09Divisions.ReportDivisionsWindow(ViewModel.DivisionSelected.DivisionId);
    //    window.Activate();
    //}
    #endregion [Methods]

}
