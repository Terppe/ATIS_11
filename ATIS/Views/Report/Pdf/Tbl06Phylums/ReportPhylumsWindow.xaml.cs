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

namespace ATIS.Views.Report.Pdf.Tbl06Phylums;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportPhylumsWindow : WindowEx
{
    #region [Private Data Members]
    private readonly AtisDbContext _context = new();
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public ReportPhylumsWindow(int phylumid)
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
        ViewModel.ReportSearchAll(phylumid, "Tbl06Phylums");

        Title = StringsRes.Phylum;
        Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
        Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
        this.CenterOnScreen();

        if (ViewModel.SubphylumItems.Count == 0)
        {
            TbDirectChildren.Visibility = Visibility.Collapsed;
        }
    }
    #endregion [Constructor]

    #region [Command]

    private void PdfPhylumCommand(object sender, RoutedEventArgs e)
    {
        GeneratePdfPhylum(ViewModel.PhylumSelected.PhylumId);
    }

    #endregion [Command]             

    #region [Methods]

    public void GeneratePdfPhylum(int id)
    {
        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");
        var single = _context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id);
        if (single != null)
        {
            var name = single.PhylumName;
            var countid = single.CountId;
            const string space = "_";
            filePath = $"{filePath}{name}{space}{countid}{space}Report.pdf";

            //var filePath = "D:\\Temp\\Animalia b11_57883426_Report.pdf";

            var model = Tbl06PhylumsDocumentDataSource.GetPhylumDetails(id);
            var document = new Tbl06PhylumsDocument(model);
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

    #endregion [Method PDF]
}
