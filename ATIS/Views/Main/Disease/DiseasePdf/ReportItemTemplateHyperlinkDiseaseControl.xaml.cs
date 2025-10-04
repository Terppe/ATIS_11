using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main.DiseasePdf;

public sealed partial class ReportItemTemplateHyperlinkDiseaseControl : UserControl
{
    public ReportItemTemplateHyperlinkDiseaseControl()
    {
        InitializeComponent();
    }

    private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
