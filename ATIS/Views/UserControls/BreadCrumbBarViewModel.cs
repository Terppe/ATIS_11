using System.Collections.ObjectModel;

namespace ATIS.Views.UserControls;
public partial class BreadCrumbBarViewModel : ObservableRecipient
{
    [ObservableProperty]
    public ObservableCollection<string> breadcrumbBarCollection;

    public IJsonNavigationViewService JsonNavigationViewService;

    public BreadCrumbBarViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        breadcrumbBarCollection = new ObservableCollection<string>();
    }

    [RelayCommand]
    private void OnItemClick(BreadcrumbBarItemClickedEventArgs args)
    {
        int numItemsToGoBack = BreadcrumbBarCollection.Count - args.Index - 1;
        for (var i = 0; i < numItemsToGoBack; i++)
        {
            JsonNavigationViewService.GoBack();
        }
    }
}
