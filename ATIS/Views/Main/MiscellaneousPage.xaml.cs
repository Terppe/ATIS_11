using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
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

namespace ATIS.Views.Main;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MiscellaneousPage : Page
{
    #region [Private Data Members]

    public MiscellaneousViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();

    #endregion [Private Data Members]   

    #region [Constructor]

    public MiscellaneousPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<MiscellaneousViewModel>();
            InitializeComponent();
            SimpleLog.Info(" MiscellaneousPage opened ");
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]
}
