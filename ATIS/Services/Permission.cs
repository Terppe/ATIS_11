using ATIS;
using ATIS.Interfaces;

public static class Permission
{
    private static readonly HashSet<string> DefaultAllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Administrator", "Developer", "Zoologist", "Biologist"
    };

    public static bool HasAccess(params string[] allowedRoles)
    {
        var user = App.GetService<IUserService>();
        if (!user.IsAuthenticated) return false;

        return allowedRoles.Contains(user.CurrentRole, StringComparer.OrdinalIgnoreCase);
    }
}
