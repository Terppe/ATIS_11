using System.Security.Principal;

namespace ATIS.Views.Auth;

public sealed class CustomPrincipal : IPrincipal
{
    public CustomIdentity Identity { get; set; } = new AnonymousIdentity();
    IIdentity IPrincipal.Identity => Identity;

    public bool IsInRole(string role) =>
        Identity.IsAuthenticated &&
        string.Equals(Identity.Role, role, StringComparison.OrdinalIgnoreCase);
}
