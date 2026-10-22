using ATIS.Helpers;
using ATIS.Services;

namespace ATIS.Views.Auth;

public interface IAuthenticationService
{
    AuthenticationService.User AuthenticateUser(string username, string password);
}

public class AuthenticationService : ObservableObject, IAuthenticationService
{
    private readonly AtisDbContext _context = new();

    private class InternalUserData
    {
        public InternalUserData(string email, string hashedPassword, string role)
        {
            Email = email;
            HashedPassword = hashedPassword;
            Role = role;
        }
        //public string Username
        //{
        //    get;
        //    private set;
        //}

        public string Email
        {
            get;
            private set;
        }

        public string HashedPassword
        {
            get;
            private set;
        }

        public string Role
        {
            get;
            private set;
        }
    }

    public User AuthenticateUser(string username, string clearTextPassword)
    {

        var hashedPassword = Crypt.CalculateHash(clearTextPassword, username);
        if (_context.TblUserProfiles != null)
        {
            //     var userData = _context.TblUserProfiles.SingleOrDefault(i => i.Email!.Equals(username) && i.Password!.Equals(hashedPassword));
            var userData = _context.TblUserProfiles.FirstOrDefault(i => i.Email == username && i.Password == hashedPassword);

            //     userData = _businessLayer.SingleListTblUserProfilesByEmailAndPassword(username, hashedPassword);

            //var userData = Users.FirstOrDefault(u => u.Username.Equals(username)
            //                                                   && u.HashedPassword.Equals(
            //                                                       Crypt.CalculateHash(clearTextPassword,
            //                                                           username)));

            if (userData != null)
            {
                return new User(userData.Email, userData.Role);
            }

            throw new UnauthorizedAccessException("Access denied. Please provider some valide credentials.");

        }

        return null!;
    }

    public class User
    {
        public User(string email, string role)
        {
            Email = email;
            Role = role;
        }
        //public string Username
        //{
        //    get;
        //    set;
        //}

        public string Email
        {
            get;
            set;
        }

        public string Role
        {
            get;
            set;
        }
    }


    public AuthenticationService()
    {

    }
}
