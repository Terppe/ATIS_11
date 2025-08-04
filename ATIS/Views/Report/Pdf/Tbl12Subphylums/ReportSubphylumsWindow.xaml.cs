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

namespace ATIS.Views.Report.Pdf.Tbl12Subphylums;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportSubphylumsWindow : WindowEx
{
    #region [Private Data Members]

    private readonly AtisDbContext _context = new();
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public ReportSubphylumsWindow(int id)
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
        ViewModel.ReportSearchAll(id, "Tbl12Subphylums");

        Title = StringsRes.Subphylum;
        Width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
        Height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
        this.CenterOnScreen();

        if (ViewModel.SuperclassItems.Count == 0)
        {
            TbDirectChildren.Visibility = Visibility.Collapsed;
        }

    }
    #endregion [Constructor]

    #region [Command]
    private void PdfSubphylumCommand(object sender, RoutedEventArgs e)
    {
        GeneratePdfSubphylum(ViewModel.SubphylumSelected.SubphylumId);
    }

    #endregion [Command]             

    #region [Methods]

    public void GeneratePdfSubphylum(int id)
    {
        var filePath = ConfigurationManager.AppSettings.Get("PdfPath");
        var single = _context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id);
        if (single != null)
        {
            var name = single.SubphylumName;
            var countid = single.CountId;
            const string space = "_";
            filePath = $"{filePath}{name}{space}{countid}{space}Report.pdf";

            //var filePath = "D:\\Temp\\Animalia b11_57883426_Report.pdf";

            var model = Tbl12SubphylumsDocumentDataSource.GetSubphylumDetails(id);
            var document = new Tbl12SubphylumsDocument(model);
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

    //// Tbl18Superclasses  -->
    //public void Superclass_OnTapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (ViewModel.SuperclassSelected.SuperclassId == 0)
    //    {
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(ViewModel.SuperclassSelected.SuperclassId, "Tbl18Superclasses");
    //    var window = new Tbl18Superclasses.ReportSuperclassesWindow(ViewModel.SuperclassSelected.SuperclassId);
    //    window.Activate();
    //}


    #endregion [Methods]    
}
