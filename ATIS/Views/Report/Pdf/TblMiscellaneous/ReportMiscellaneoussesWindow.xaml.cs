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

namespace ATIS.Views.Report.Pdf.TblMiscellaneous;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportMiscellaneoussesWindow : WindowEx
{
    #region [Private Data Members]
    private readonly AtisDbContext _context = new();
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public ReportMiscellaneoussesWindow(int miscellaneousid)
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
        ViewModel.ReportSearchAll(miscellaneousid, "TblMiscellaneousses");

        ViewModel.MiscellaneousSelected = _context.TblMiscellaneousses.SingleOrDefault(a => a.MiscellaneousId == miscellaneousid);

        Title = StringsRes.Miscellaneous;
        Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
        Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
        this.CenterOnScreen();
    }
    #endregion [Constructor]   

    #region [Command]

    private void PdfMiscellaneousCommand(object sender, RoutedEventArgs e)
    {
        GeneratePdfMiscellaneous(ViewModel.MiscellaneousSelected.MiscellaneousId);
    }

    #endregion [Command]             

    #region [Methods]

    public void GeneratePdfMiscellaneous(int miscellaneousid)
    {
        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");
        var single = _context.TblMiscellaneousses.SingleOrDefault(a => a.MiscellaneousId == miscellaneousid);
        if (single != null)
        {
            var name = single.MiscellaneousName;
            var countid = single.CountId;
            const string space = "_";
            filePath = $"{filePath}{name}{space}{countid}{space}Report.pdf";

            //var filePath = "D:\\Temp\\Animalia b11_57883426_Report.pdf";

            var model = TblMiscellaneoussesDocumentDataSource.GetMiscellaneousDetails(miscellaneousid);
            var document = new TblMiscellaneoussesDocument(model);
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
