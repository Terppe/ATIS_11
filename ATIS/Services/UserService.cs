using ATIS.Interfaces;

namespace ATIS.Services;

public class UserService : IUserService
{
    public string CurrentUser { get; private set; }
    public UserService()
    {
        // Beispiel: Windows-Benutzername
        CurrentUser = Environment.UserName;
        // Oder später: Login-Mechanismus
        // // CurrentUser = "Rudolf";
    }

    public void SetUser(string username)
    {
        CurrentUser = username;
    }
}
