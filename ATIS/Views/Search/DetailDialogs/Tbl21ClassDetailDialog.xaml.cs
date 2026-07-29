
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailViewModels;

namespace ATIS.Views.Search.DetailDialogs;

public sealed partial class Tbl21ClassDetailDialog : ContentDialog
{
    public Tbl21ClassDetailDialog(Tbl21Class selectedClass)
    {
        InitializeComponent();

        var dataService = App.GetService<IDataService>();   // ⭐ MUSS HIER SEIN

        ViewModel = new Tbl21ClassDetailViewModel(selectedClass, dataService);
        DataContext = ViewModel;
    }

    public Tbl21ClassDetailViewModel ViewModel { get; }
}
