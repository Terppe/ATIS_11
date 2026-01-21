namespace ATIS.Views.Settings;

public sealed partial class AppUpdateSettingPage : Page
{
    public AppUpdateSettingViewModel ViewModel { get; set; }

    public AppUpdateSettingPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel = App.GetService<AppUpdateSettingViewModel>();

    }
}


