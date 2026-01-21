using System.Collections.ObjectModel;

namespace ATIS.Views.UserControls;
public partial class BreadCrumbBarViewModel : ObservableObject
{
    //[ObservableProperty]
    //public ObservableCollection<string> breadcrumbBarCollection;

    private ObservableCollection<string> _breadcrumbBarCollection;
    public ObservableCollection<string> BreadcrumbBarCollection
    {
        get => _breadcrumbBarCollection;
        set => SetProperty(ref _breadcrumbBarCollection, value);
    }




    public BreadCrumbBarViewModel()
    {
        BreadcrumbBarCollection = [];
    }

    [RelayCommand]
    private void OnItemClick(BreadcrumbBarItemClickedEventArgs args)
    {
        //int numItemsToGoBack = BreadcrumbBarCollection.Count - args.Index - 1;
        //for (var i = 0; i < numItemsToGoBack; i++)
        //{
        //    JsonNavigationViewService.GoBack();
        //}
    }
}
