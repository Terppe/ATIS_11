using System.Security.Principal;

namespace ATIS.Views.Authentication;
public class CustomIdentity : IIdentity
{
    public CustomIdentity(string email, string role)
    {
        Email = email;
        Role = role;
    }

    public string Name
    {
        get; private set;
    } = null!;

    public string Email
    {
        get; private set;
    }
    public string Role
    {
        get; private set;
    }

    #region IIdentity Members
    public string AuthenticationType => "Custom authentication";

    public bool IsAuthenticated => !string.IsNullOrEmpty(Email);


    #endregion
}
