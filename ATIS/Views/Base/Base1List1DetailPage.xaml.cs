using ATIS.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ATIS.Views.Base
{
    public abstract partial class Base1List1DetailPage : Page
    {
        public object ViewModel
        {
            get => DataContext;
            protected set => DataContext = value;
        }

        public Base1List1DetailPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            SizeChanged += (_, _) => PageWidth = ActualWidth;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Lifecycle
            if (ViewModel is IBasePageLifecycle lifecycle)
                await lifecycle.OnPageLoadedAsync();

            // Automatische Validierung aktivieren
            TryWireValidation();
        }

        // ---------------------------------------------------------
        // AUTOMATISCHE VALIDIERUNG
        // ---------------------------------------------------------
        private void TryWireValidation()
        {
            if (ViewModel is not Base1List1DetailViewModelBase vm)
                return;

            if (vm.EditItemUntyped is not INotifyDataErrorInfo notifier)
                return;

            foreach (var tb in FindTextBoxes(this))
            {
                var binding = tb.GetBindingExpression(TextBox.TextProperty);
                if (binding?.ParentBinding?.Path?.Path is string fullPath)
                {
                    var propertyName = fullPath.Split('.').Last(); // <-- WICHTIG
                    ValidationHelper.Hook(notifier, propertyName, tb);
                }
            }
        }

        private static IEnumerable<TextBox> FindTextBoxes(DependencyObject root)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                int count = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    if (child is TextBox tb)
                        yield return tb;

                    queue.Enqueue(child);
                }
            }
        }

        // ---------------------------------------------------------
        // DEINE BESTEHENDEN PROPERTIES
        // ---------------------------------------------------------

        public double PageWidth
        {
            get => (double)GetValue(PageWidthProperty);
            set => SetValue(PageWidthProperty, value);
        }

        public static readonly DependencyProperty PageWidthProperty =
            DependencyProperty.Register(nameof(PageWidth), typeof(double), typeof(Base1List1DetailPage), new PropertyMetadata(0.0));

        public void SetToolbar(UIElement element) => ToolbarPresenter.Content = element;
        public void SetGrid(UIElement element) => GridPresenter.Content = element;
        public void SetDetail(UIElement element) => DetailPresenter.Content = element;

        public bool ShouldFocusRefExpertName
        {
            get => (bool)GetValue(ShouldFocusRefExpertNameProperty);
            set => SetValue(ShouldFocusRefExpertNameProperty, value);
        }

        public static readonly DependencyProperty ShouldFocusRefExpertNameProperty =
            DependencyProperty.Register(nameof(ShouldFocusRefExpertName), typeof(bool),
                typeof(Base1List1DetailPage), new PropertyMetadata(false));
    }
}
