using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.DesignSystem.Foundations
{
    public static class ElevationHelper
    {
        public static int GetElevation(DependencyObject obj)
            => (int)obj.GetValue(ElevationProperty);

        public static void SetElevation(DependencyObject obj, int value)
            => obj.SetValue(ElevationProperty, value);

        public static readonly DependencyProperty ElevationProperty =
            DependencyProperty.RegisterAttached(
                "Elevation",
                typeof(int),
                typeof(ElevationHelper),
                new PropertyMetadata(0, OnElevationChanged));

        private static void OnElevationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
                return;

            int level = (int)e.NewValue;

            // Nur Panels und Border bekommen überhaupt ThemeShadow
            if (element is not Panel && element is not Border)
                return;

            try
            {
                ApplyThemeShadow(element, level);
            }
            catch
            {
                // Sicherheitsnetz: niemals crashen, wenn WinUI intern spinnt
            }
        }

        private static void ApplyThemeShadow(UIElement element, int level)
        {
            string shadowKey = $"Elevation{level}Shadow";
            string opacityKey = $"Elevation{level}Opacity";

            if (Application.Current.Resources.TryGetValue(shadowKey, out var shadow))
            {
                element.SetValue(UIElement.ShadowProperty, shadow);
            }

            if (Application.Current.Resources.TryGetValue(opacityKey, out var opacity))
            {
                element.Opacity = (double)opacity;
            }
        }
    }
}
