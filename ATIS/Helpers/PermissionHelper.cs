
namespace ATIS.Helpers;

public static class PermissionHelper
{
    private static string _lastPageTag = "ATIS.Views.Main.HomePage";

    public static void UpdateLastPage(string tag)
    {
        _lastPageTag = tag;
    }

    public static async Task<bool> CheckOrGoBackAsync(NavigationView navView, Frame frame, string requestedTag)
    {
        if (!Permission.RolePermission())
        {
            await AllDialogs.WarningNoPermissionMessageDialogAsync("");

            // Cursor zurück zur letzten gültigen Page
            var previousItem = navView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(i => (string)i.Tag == _lastPageTag);

            if (previousItem != null)
                navView.SelectedItem = previousItem;

            // 🔥 WICHTIG: Navigation verzögern
            await Task.Delay(10);

            var previousType = Type.GetType(_lastPageTag);
            if (previousType != null)
                frame.Navigate(previousType);

            return false;
        }

        // Permission OK → letzte gültige Page aktualisieren
        _lastPageTag = requestedTag;
        return true;
    }
}
