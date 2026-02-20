using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.Views.Base;

public abstract partial class Basic1DataGrid1DetailPage : Page
{
    public object ViewModel
    {
        get => DataContext;
        protected set => DataContext = value;
    }

    public Basic1DataGrid1DetailPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is IBasePageLifecycle lifecycle)
            await lifecycle.OnPageLoadedAsync();
    }

    public void SetToolbar(UIElement element) => ToolbarPresenter.Content = element;
    public void SetGrid(UIElement element) => GridPresenter.Content = element;
    public void SetDetail(UIElement element) => DetailPresenter.Content = element;
}
