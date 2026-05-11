

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusParentControl : UserControl
{
    public Tbl60SubtribusViewModel? ViewModel => DataContext as Tbl60SubtribusViewModel;

    public Tbl60SubtribusParentControl()
    {
        InitializeComponent();
    }

    private void SubtribusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.SelectedItem = (Models.Tbl60Subtribus)e.ClickedItem;
            ViewModel.IsSubtribusEditVisible = true;
        }
    }
}
