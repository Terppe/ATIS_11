

namespace ATIS.Views.Search;

public sealed partial class DetailDialog : ContentDialog
{
    public DetailDialogViewModel ViewModel { get; }

    public DetailDialog(object entity)
    {
        InitializeComponent();
        ViewModel = new DetailDialogViewModel(entity);
        DataContext = this;
    }
}
