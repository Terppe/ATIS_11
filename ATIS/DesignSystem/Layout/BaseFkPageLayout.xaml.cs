

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.DesignSystem.Layout;

public sealed partial class BaseFkPageLayout : UserControl
{
    public BaseFkPageLayout()
    {
        InitializeComponent();
    }

    public UIElement Header
    {
        get => HeaderPresenter.Content as UIElement;
        set => HeaderPresenter.Content = value;
    }

    public UIElement AppBar
    {
        get => AppBarPresenter.Content as UIElement;
        set => AppBarPresenter.Content = value;
    }

    public UIElement Content
    {
        get => ContentPresenter.Content as UIElement;
        set => ContentPresenter.Content = value;
    }
}
