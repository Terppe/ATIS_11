using System.Security.Principal;

namespace ATIS.Views.Auth;

public class CustomIdentity : IIdentity
{
    public CustomIdentity(string email, string role)
    {
        Email = email ?? string.Empty;
        Role = role ?? string.Empty;
        Name = Email;
    }

    public string Name { get; }
    public string Email { get; }
    public string Role { get; }

    public string AuthenticationType => "Custom authentication";
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Email);
}
