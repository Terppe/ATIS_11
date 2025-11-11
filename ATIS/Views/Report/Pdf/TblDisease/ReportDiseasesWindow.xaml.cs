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

namespace ATIS.Views.Report.Pdf.TblDisease;
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

        ViewModel.DiseaseSelected = _context.TblDiseases.SingleOrDefault(a => a.DiseaseId == diseaseid);

        Title = StringsRes.Disease;
        Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
        Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
        this.CenterOnScreen();
    }
    #endregion [Constructor]   

    #region [Command]

    private void PdfDiseaseCommand(object sender, RoutedEventArgs e)
    {
        GeneratePdfDisease(ViewModel.DiseaseSelected.DiseaseId);
    }

    #endregion [Command]             

    #region [Methods]

    public void GeneratePdfDisease(int diseaseid)
    {
        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");
        var single = _context.TblDiseases.SingleOrDefault(a => a.DiseaseId == diseaseid);
        if (single != null)
        {
            var name = single.DiseaseName;
            var countid = single.CountId;
            const string space = "_";
            filePath = $"{filePath}{name}{space}{countid}{space}Report.pdf";

            //var filePath = "D:\\Temp\\Animalia b11_57883426_Report.pdf";

            var model = TblDiseasesDocumentDataSource.GetDiseaseDetails(diseaseid);
            var document = new TblDiseasesDocument(model);
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

    #endregion [Methods]
}
