namespace ATIS.Views.Settings;

public sealed partial class AboutUsSettingPage : Page
{
    public AboutUsSettingViewModel ViewModel { get; set; }

    public AboutUsSettingPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel = App.GetService<AboutUsSettingViewModel>();

    }
}


