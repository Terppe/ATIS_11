using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusRowControl : UserControl
{
    public Tbl60SubtribusRowControl()
    {
        InitializeComponent();

        // Pointer events for hover
        PointerEntered += (_, __) => VisualStateManager.GoToState(this, "PointerOver", true);
        PointerExited += (_, __) => VisualStateManager.GoToState(this, "Normal", true);

        // Selection is handled by ListView automatically
        Loaded += (_, __) =>
        {
            var listViewItem = this.FindParent<ListViewItem>();
            if (listViewItem != null)
            {
                listViewItem.RegisterPropertyChangedCallback(
                    ListViewItem.IsSelectedProperty,
                    (_, __) =>
                    {
                        if (listViewItem.IsSelected)
                            VisualStateManager.GoToState(this, "Selected", true);
                        else
                            VisualStateManager.GoToState(this, "Normal", true);
                    });
            }
        };
    }
}
