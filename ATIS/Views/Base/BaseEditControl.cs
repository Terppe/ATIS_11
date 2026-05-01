namespace ATIS.Views.Base;
public abstract class BaseEditControl : UserControl
{
    protected void UpdateValidationState(TextBox box, bool hasError)
    {
        if (box == null)
            return;

        if (hasError)
        {
            VisualStateManager.GoToState(box, "Invalid", true);
            return;
        }

        if (box.FocusState == FocusState.Pointer ||
            box.FocusState == FocusState.Keyboard)
        {
            VisualStateManager.GoToState(box, "Focused", true);
        }
        else
        {
            VisualStateManager.GoToState(box, "Valid", true);
        }
    }
}
