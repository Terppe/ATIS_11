using System.Collections.ObjectModel;
using ATIS.Themes.Controls;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;

public sealed partial class FluentTabView : UserControl
{
    public ObservableCollection<FluentTabItem> Tabs { get; } = new();

    public object SelectedContent { get; set; }

    public FluentTabView()
    {
        InitializeComponent();

        TabRepeater.Tapped += OnTabTapped;
        AddButton.Click += OnAddTab;
    }

    private void OnTabTapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement fe &&
            fe.DataContext is FluentTabItem tab)
        {
            SelectedContent = tab.Content;
            Bindings.Update();
        }
    }

    private void OnAddTab(object sender, RoutedEventArgs e)
    {
        Tabs.Add(new FluentTabItem
        {
            Header = "New Tab",
            Content = new TextBlock { Text = "New Tab Content" }
        });
    }
}
