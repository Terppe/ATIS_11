using System.ComponentModel;

namespace ATIS.Helpers
{
    public static class ValidationHelper
    {
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.RegisterAttached(
                "HasError",
                typeof(bool),
                typeof(ValidationHelper),
                new PropertyMetadata(false));

        public static void SetHasError(DependencyObject obj, bool value)
            => obj.SetValue(HasErrorProperty, value);

        public static bool GetHasError(DependencyObject obj)
            => (bool)obj.GetValue(HasErrorProperty);

        public static void Hook(INotifyDataErrorInfo notifier, string propertyName, Control control)
        {
            notifier.ErrorsChanged += (_, args) =>
            {
                if (args.PropertyName == propertyName)
                {
                    bool hasError = notifier.GetErrors(propertyName).Cast<object>().Any();
                    SetHasError(control, hasError);

                    VisualStateManager.GoToState(control, hasError ? "Invalid" : "Valid", true);
                }
            };

            bool initial = notifier.GetErrors(propertyName).Cast<object>().Any();
            SetHasError(control, initial);
            VisualStateManager.GoToState(control, initial ? "Invalid" : "Valid", false);
        }
    }
}
