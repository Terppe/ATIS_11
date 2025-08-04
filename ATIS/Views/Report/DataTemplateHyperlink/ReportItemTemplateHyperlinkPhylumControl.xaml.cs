using ATIS.Helpers;
using ATIS.Views.Report.Pdf.Tbl06Phylums;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Report.DataTemplateHyperlink;
public sealed partial class ReportItemTemplateHyperlinkPhylumControl : UserControl
{
    #region [Private Data Members]
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public ReportItemTemplateHyperlinkPhylumControl()
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
    }
    #endregion [Constructor]

    private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var id = Convert.ToInt32(TbPhylum.Tag);
        if (id == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        ViewModel.ReportSearchAll(id, "Tbl06Phylums");
        var window = new ReportPhylumsWindow(id);
        window.Activate();
    }
}
