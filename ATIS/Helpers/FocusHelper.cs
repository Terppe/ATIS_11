namespace ATIS.Helpers
{
    public static class FocusHelper
    {
        public static bool GetIsFocused(DependencyObject obj)
            => (bool)obj.GetValue(IsFocusedProperty);

        public static void SetIsFocused(DependencyObject obj, bool value)
            => obj.SetValue(IsFocusedProperty, value);

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused",
                typeof(bool),
                typeof(FocusHelper),
                new PropertyMetadata(false, OnIsFocusedChanged));

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Control control)
                return;

            if ((bool)e.NewValue)
            {
                control.DispatcherQueue.TryEnqueue(() =>
                {
                    control.Focus(FocusState.Programmatic);
                });
            }
        }
    }
}
