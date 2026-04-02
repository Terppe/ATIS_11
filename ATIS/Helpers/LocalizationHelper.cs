namespace ATIS.Helpers;

public static class LocalizationHelper
{
    public static readonly DependencyProperty UidProperty =
        DependencyProperty.RegisterAttached(
            "Uid",
            typeof(string),
            typeof(LocalizationHelper),
            new PropertyMetadata(null));

    public static void SetUid(DependencyObject obj, string value)
        => obj.SetValue(UidProperty, value);

    public static string GetUid(DependencyObject obj)
        => (string)obj.GetValue(UidProperty);
}
