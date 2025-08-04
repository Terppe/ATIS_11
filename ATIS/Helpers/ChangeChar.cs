

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Helpers;
public static class ChangeChar 
{
    public static void UpperCaseFirstChar(object sender)
    {
        var textBox = sender as TextBox;
        if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
        {
            // Capitalize the first character and keep the rest as is
            textBox.Text = char.ToUpper(textBox.Text[0]) + textBox.Text[1..];
        }
    }
}
