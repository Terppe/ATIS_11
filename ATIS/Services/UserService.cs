using ATIS.Interfaces;
using ATIS.Views.User;

namespace ATIS.Services;

public class UserService : IUserService
{
    public string CurrentUser { get; private set; } = string.Empty;
    public string CurrentRole { get; private set; } = string.Empty;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(CurrentUser);

    public void SetUser(string username, string role)
    {
        CurrentUser = username ?? string.Empty;
        CurrentRole = role ?? string.Empty;
    }

    public void Clear()
    {
        CurrentUser = string.Empty;
        CurrentRole = string.Empty;
    }
}
