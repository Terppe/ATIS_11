using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;
public sealed partial class BreadcrumbBarUserControl : UserControl
{
    public List<string> Items
    {
        get => (List<string>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register("Items", typeof(List<string>), typeof(BreadcrumbBarUserControl), new PropertyMetadata(null));

    public string SingleItem
    {
        get => (string)GetValue(SingleItemProperty);
        set => SetValue(SingleItemProperty, value);
    }

    public static readonly DependencyProperty SingleItemProperty =
        DependencyProperty.Register("SingleItem", typeof(string), typeof(BreadcrumbBarUserControl), new PropertyMetadata(default(string)));

    public BreadCrumbBarViewModel ViewModel { get; }

    public BreadcrumbBarUserControl()
    {
        ViewModel = App.GetService<BreadCrumbBarViewModel>();
        this.InitializeComponent();
        Loaded += BreadcrumbBarUserControl_Loaded;
    }

    private void BreadcrumbBarUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.BreadcrumbBarCollection.Add("Settings");
        if (Items != null)
        {
            foreach (var item in Items)
            {
                ViewModel.BreadcrumbBarCollection.Add(item);
            }
        }
        else
        {
            ViewModel.BreadcrumbBarCollection.Add(SingleItem);
        }
    }
}
