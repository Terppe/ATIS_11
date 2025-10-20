using ATIS.Strings;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Helpers;
internal static class SetFocusses
{
    public static void SetFocusAndChangePlaceholderText(TextBox textBox)
    {
        //needed for Focus
        textBox.IsReadOnly = false;

        // Set focus programmatically
        var val = textBox.Focus(FocusState.Programmatic);

        // Change the background color
        //     textBox.Background = new SolidColorBrush(Colors.SlateGray);

        //Placeholdertext and Foreground
        textBox.PlaceholderText = StringsRes.EnterText;
        textBox.PlaceholderForeground = new SolidColorBrush(Colors.Red);
    }

}
