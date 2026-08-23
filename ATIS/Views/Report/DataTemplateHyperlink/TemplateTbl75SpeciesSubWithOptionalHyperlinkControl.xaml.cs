

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Report.DataTemplateHyperlink;
public sealed partial class TemplateTbl75SpeciesSubWithOptionalHyperlinkControl : UserControl
{
    #region [Private Data Members]
    public ReportViewModel ViewModel { get; set; }

    #endregion [Private Data Members]   

    #region [Constructor]

    public TemplateTbl75SpeciesSubWithOptionalHyperlinkControl()
    {
        ViewModel = App.GetService<ReportViewModel>();
        InitializeComponent();
    }
    #endregion [Constructor]

    //private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
    //{
    //    var id = Convert.ToInt32(TbFiSpecies.Tag);
    //    if (id == 0)
    //    {
    //        _ = _allDialogs.NoDatasetSelectedWarnMessageDialogAsync();
    //        return;
    //    }

    //    ViewModel.ReportSearchAll(id, "Tbl69FiSpeciesses");
    //    var window = new ReportFiSpeciessesWindow(id);
    //    window.Activate();
    //}
}
