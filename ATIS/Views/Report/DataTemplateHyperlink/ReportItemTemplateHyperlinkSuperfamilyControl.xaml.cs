using ATIS.Helpers;
using ATIS.Views.Report.Pdf.Tbl42Superfamilies;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Report.DataTemplateHyperlink;
public sealed partial class ReportItemTemplateHyperlinkSuperfamilyControl : UserControl
{
    #region [Private Data Members]
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public ReportItemTemplateHyperlinkSuperfamilyControl()
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
    }
    #endregion [Constructor]

    private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var id = Convert.ToInt32(TbSuperfamily.Tag);
        if (id == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        ViewModel.ReportSearchAll(id, "Tbl42Superfamilies");
        var window = new ReportSuperfamiliesWindow(id);
        window.Activate();
    }
}
