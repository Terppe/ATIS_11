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

        public static void Hook(INotifyDataErrorInfo notifier, string propertyName, TextBox textBox)
        {
            notifier.ErrorsChanged += (_, args) =>
            {
                if (args.PropertyName == propertyName)
                {
                    bool hasError = notifier.GetErrors(propertyName).Cast<object>().Any();
                    SetHasError(textBox, hasError);
                }
            };

            // Initial prüfen
            bool initial = notifier.GetErrors(propertyName).Cast<object>().Any();
            SetHasError(textBox, initial);
        }
    }
}
