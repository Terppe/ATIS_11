
namespace ATIS.Interfaces;

public interface IUserService
{
    string CurrentUser { get; }
    string CurrentRole { get; }
    bool IsAuthenticated { get; }

    void SetUser(string username, string role);
    void Clear();
}
