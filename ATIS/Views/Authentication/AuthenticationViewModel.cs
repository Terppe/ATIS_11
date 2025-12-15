using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ATIS.Views.Authentication;
public interface IViewModel
{
}

public partial class AuthenticationViewModel : ObservableObject
{
    #region [Private Data Members]

    private readonly AtisDbContext _context = new();
    private readonly AllDialogs _allDialogs = new();
    private readonly IDataService _dataService;

    private string _email = null!;
    private string _status = null!;
    private string _errormessage = null!;
    private string _authenticatedUser = null!;
    private string _passwordChar = null!;
    private string _passwordCharNew = null!;
    private string _passwordCharOld = null!;
    //   private readonly DelegateCommand _logoutCommand;

    #endregion [Private Data Members]

    #region [Constructor]

    public AuthenticationViewModel(IDataService dataService)
    {
        _dataService = dataService;
        //_authenticationService = authenticationService;
        //_logoutCommand = new DelegateCommand(Logout, CanLogout);
    }
    #endregion [Constructor]

    #region Commands
    public ICommand LoginCommand => new WinUICommunity.RelayCommand(execute: delegate { var task = Login(PasswordChar); });
    public ICommand RegisterCommand => new WinUICommunity.RelayCommand(execute: delegate { var task = Register(PasswordChar); });
    public ICommand GotFocusPasswordChangeCommand => new WinUICommunity.RelayCommand(execute: delegate { var task = GotFocusPasswordChange(PasswordCharOld); });
    public ICommand PasswordChangeCommand => new WinUICommunity.RelayCommand(execute: delegate { var task = PasswordChange(PasswordCharNew); });
    public ICommand PasswordForgotCommand => new WinUICommunity.RelayCommand(execute: delegate { var task = PasswordForgotWithEmail(); });
    public ICommand LogoutCommand => new WinUICommunity.RelayCommand(execute: delegate { var task = Logout(); });

    //public DelegateCommand LogoutCommand => _logoutCommand;

    #endregion

    #region Properties

    public string Email
    {
        get => _email;
        set
        {
            _email = value; OnPropertyChanged(nameof(Email));
        }
    }

    public string Status
    {
        get => _status;
        set
        {
            _status = value; OnPropertyChanged(nameof(Status));
        }
    }

    public string Errormessage
    {
        get => _errormessage;
        set
        {
            _errormessage = value; OnPropertyChanged(nameof(Errormessage));
        }
    }
    public string PasswordChar
    {
        get => _passwordChar;
        set
        {
            _passwordChar = value; OnPropertyChanged(nameof(PasswordChar));
        }
    }
    public string PasswordCharNew
    {
        get => _passwordCharNew;
        set
        {
            _passwordCharNew = value; OnPropertyChanged(nameof(PasswordCharNew));
        }
    }

    public string PasswordCharOld
    {
        get => _passwordCharOld;
        set
        {
            _passwordCharOld = value; OnPropertyChanged(nameof(PasswordCharOld));
        }
    }

    public string AuthenticatedUser
    {
        get
        {
            if (IsAuthenticated)
            {
                return string.Format("Signed in as {0}. {1}", Email,
                    Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.IsInRole("Administrator") ? "You are an administrator!"
                        : "You are NOT a member of the administrators group.");
            }

            return "Not authenticated!";
        }
        set
        {
            _authenticatedUser = value; OnPropertyChanged();
        }

    }

    public bool IsModified { get; set; }

    private TblUserProfile _userProfileSelected;

    public TblUserProfile UserProfileSelected
    {
        get => _userProfileSelected;
        set => SetProperty(ref _userProfileSelected, value);
    }

    #endregion

    #region Methods


    public static bool IsAuthenticated => Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity!.IsAuthenticated;
    private Task Login(string parameter)
    {
        if (parameter == null)
        {
            return Task.CompletedTask;
        }

        var clearTextPassword = parameter;
        Errormessage = "";
        Status = "";

        if (Email == null)
        {
            Errormessage = "Email is a required field.";
        }
        else if (!Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
        {
            Errormessage = "Email must formated like so; xxx@xxx.xxx";
        }
        else
        {
            try
            {
                var authenticationService = new AuthenticationService();
                //Validate credentials through the authentication service
                var user = authenticationService.AuthenticateUser(Email, clearTextPassword);

                //Get the current principal object

                if (Thread.CurrentPrincipal is not CustomPrincipal customPrincipal)
                {
                    throw new ArgumentException(
                        "The application's default thread principal must be set to a CustomPrincipal object on startup.");
                }

                //Authenticate the user
                customPrincipal.Identity = new CustomIdentity(user.Email, user.Role);

                Status = StringsRes.LoginSuccsessful;

                //Update UI
                OnPropertyChanged(nameof(AuthenticatedUser));
                OnPropertyChanged(nameof(IsAuthenticated));

                Email = string.Empty; //reset
                PasswordChar = string.Empty; //reset
                Status = string.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                Status = StringsRes.LoginFail;
            }
            catch (Exception ex)
            {
                Status = $"ERROR: {ex.Message}";
            }
        }

        return Task.CompletedTask;
    }
    private async Task Register(string parameter)
    {
        if (parameter == null)
        {
            return;
        }

        {
            var clearTextPassword = parameter;
            Errormessage = "";
            Status = "";

            if (Email == null)
            {
                Errormessage = "Email is a required field.";
            }
            else if (!Regex.IsMatch(Email,
                @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                Errormessage = "Email must formated like so; x@x.x";
            }
            else
            {
                try
                {
                    if (_context.TblUserProfiles != null)
                    {
                        var userprofile = _context.TblUserProfiles.SingleOrDefault(i => i.Email == Email);

                        if (userprofile == null)
                        {
                            userprofile = new TblUserProfile //add new
                            {
                                Email = Email,
                                CountId = RandomHelper.Randomnumber(),
                                Password = Crypt.CalculateHash(clearTextPassword, Email),
                                Role = "User",
                                BirthDate = DateTime.Now,
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now,
                                Writer = Environment.UserName,
                                WriterDate = DateTime.Now,
                                Updater = Environment.UserName,
                                UpdaterDate = DateTime.Now
                            };

                            _context.TblUserProfiles.Add(userprofile);
                            await _context.SaveChangesAsync();
                            Status = StringsRes.SaveSuccess;

                            await Login(parameter);   //Login
                            Status = StringsRes.LoginSuccsessful;
                        }
                        else
                        {
                            Status = StringsRes.DatasetExist;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Status = $"ERROR: {ex.Message}";
                }
            }
        }
    }
    private static bool CanLogin()
    {
        return !IsAuthenticated;
    }

    private Task<Task> Logout()
    {
        if (Thread.CurrentPrincipal as CustomPrincipal is { } customPrincipal)
        {
            customPrincipal.Identity = new AnonymousIdentity();
            OnPropertyChanged(nameof(AuthenticatedUser));
            OnPropertyChanged(nameof(IsAuthenticated));
            Status = string.Empty;
        }
        return (Task<Task>)Task.CompletedTask;
    }

    private static bool CanLogout()
    {
        return IsAuthenticated;
    }

    private static bool CanRegister()
    {
        return !IsAuthenticated;
    }
    public static string GeneratePassword(int passLength)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();

        var result = new string(
            [.. Enumerable.Repeat(chars, passLength).Select(s => s[random.Next(s.Length)])]);
        return result;
    }

    private async Task PasswordForgotWithEmail()
    {
        Errormessage = "";
        Status = "";

        var clearTextPasswordNew = GeneratePassword(8);

        if (Email == null)
        {
            Errormessage = "Email is a required field";
        }
        else if (!Regex.IsMatch(Email,
                     @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
        {
            Errormessage = "Email must formated like so; x@x.xx";
        }
        else
        {
            //---------------save-----------------------
            try
            {
                if (_context.TblUserProfiles != null)
                {
                    var userprofile = _context.TblUserProfiles.SingleOrDefault(i => i.Email == Email);

                    if (userprofile != null)
                    {
                        userprofile.Password = Crypt.CalculateHash(clearTextPasswordNew, Email);
                        userprofile.Writer = Environment.UserName;
                        userprofile.WriterDate = DateTime.Now;
                        userprofile.Updater = Environment.UserName;
                        userprofile.UpdaterDate = DateTime.Now;

                        await _dataService.UpsertEmailFromUserProfileAsync(userprofile);
                    }

                    await _context.SaveChangesAsync();

                    Status = StringsRes.SaveSuccess;
                }
            }
            catch (Exception ex)
            {
                Status = $"ERROR: {ex.Message}";
            }

            //---------------mail von info@terppe.de ----------------------------
            try
            {
                //for Provider 1&1
                var smtpServer = ConfigurationManager.AppSettings["SmtpHost"];
                var smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                var imapServer = ConfigurationManager.AppSettings["ImapHost"];
                var imapPort = Convert.ToInt32(ConfigurationManager.AppSettings["ImapPort"]);
                var email = ConfigurationManager.AppSettings["Sender"];

                //from
                //var smtpServer = "smtp.1und1.de";
                //var smtpPort = 587;  //465
                //var imapServer = "imap.1und1.de";
                //var imapPort = 993;

                //string email = "info@terppe.de";
                //string password = "elsaQ0424";  //at 1und1

                var encryptMailAccountPassword = ConfigurationManager.AppSettings.Get("EncryptMailAccountPassword");

                if (encryptMailAccountPassword != null)
                {
                    var password = Crypt.DecryptString(encryptMailAccountPassword);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Hotline T-Soft", email)); //Deine E-Mailadresse, von der gesendet werden soll, entspricht oftmals deinem Benutzernamen
                    message.To.Add(new MailboxAddress("User", Email));
                    message.Subject = StringsRes.ForgotPassword;
                    message.Body = new TextPart("plain")
                    {
                        Text = StringsRes.NewGeneratedPassord + " " + clearTextPasswordNew
                    };

                    var smtpClient = new SmtpClient();

                    try
                    {
                        if (smtpServer != null)
                        {
                            if (email != null)
                            {
                                SendEmail(smtpClient, message, smtpServer, smtpPort, email, password);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Errormessage = StringsRes.Error + ex.Message;
                        Errormessage = "Stack: " + ex.StackTrace;
                    }
                    finally
                    {
                        await smtpClient.DisconnectAsync(true);
                        smtpClient.Dispose();
                    }
                }
                //alternate not ok
                //var imapClient = new ImapClient();
                //var password1 = Crypt.DecryptString(encryptMailAccountPassword);

                //try
                //{
                //    RetrieveInbox(imapClient, imapServer, imapPort, email, password1);
                //}
                //catch (Exception ex)
                //{
                //    Errormessage = "Error: " + ex.Message;
                //    Errormessage = "Stack: " + ex.StackTrace;
                //    Console.WriteLine(ex.Message);
                //    Console.WriteLine(ex.StackTrace);
                //}
                //finally
                //{
                //    await imapClient.DisconnectAsync(true);
                //    imapClient.Dispose();
                //}
            }
            catch (Exception ex)
            {
                Errormessage = "Error: " + ex.Message;
            }
        }
    }

    private static void SendEmail(SmtpClient client, MimeMessage message, string server, int port, string email, string password)
    {
        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
        client.Connect(server, port, SecureSocketOptions.StartTls);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate(email, password);
        client.Send(message);
    }

    private Task GotFocusPasswordChange(string parameter)
    {
        if (parameter == null)
        {
            return Task.CompletedTask;
        }

        {
            var clearTextPasswordOld = parameter;

            Errormessage = "";
            Status = "";

            if (Email == null)
            {
                Errormessage = "Email is a required field.";
            }
            else if (!Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                Errormessage = "Email must formated like so; x@x.xx";
            }
            else
            {
                try
                {
                    var crypPassword = Crypt.CalculateHash(clearTextPasswordOld, Email);

                    if (_context.TblUserProfiles != null)
                    {
                        var userprofile = _context.TblUserProfiles.SingleOrDefault(i => i.Email == Email && i.Password == crypPassword);

                        if (userprofile == null)
                        {
                            Status = StringsRes.DatasetNotExist;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Status = $"ERROR: {ex.Message}";
                }
            }
        }
        return Task.CompletedTask;
    }

    private async Task PasswordChange(string parameter)
    {
        if (parameter == null)
        {
            return;
        }

        {
            var clearTextPasswordNew = parameter;
            Errormessage = "";
            Status = "";

            if (Email == null)
            {
                Errormessage = "Email is a required field.";
            }
            else if (!Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                Errormessage = "Email must formated like so; x@x.xx";
            }
            else
            {
                try
                {
                    var mail = _context.TblUserProfiles.SingleOrDefault(i => i.Email == Email);
                    if (mail == null)
                    {
                        Status = StringsRes.EMailNotExist;
                        return;
                    }

                    if (_context.TblUserProfiles != null)
                    {
                        var userprofile = _context.TblUserProfiles.FirstOrDefault(i => i.Email == Email);

                        if (userprofile != null)  //update
                        {
                            {
                                userprofile.Password = Crypt.CalculateHash(clearTextPasswordNew, Email);
                                userprofile.Writer = Environment.UserName;
                                userprofile.WriterDate = DateTime.Now;
                                userprofile.Updater = Environment.UserName;
                                userprofile.UpdaterDate = DateTime.Now;
                            }


                            //if (!await _allDialogs.SaveDatasetQuestionConfirmationDialogAsync(Email))
                            //{
                            //    return;
                            //}

                            _context.TblUserProfiles.Update(userprofile);

                            await _context.SaveChangesAsync();
                            Status = StringsRes.SaveSuccess;
                        }
                        else
                        {
                            Status = StringsRes.DatasetNotExist;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Status = $"ERROR: {ex.Message}";
                }
            }
        }
    }

    private static void RetrieveInbox(ImapClient client, string server, int port, string email, string password)
    {
        client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
        client.Connect(server, port, true);
        client.Authenticate(email, password);

        var inbox = client.Inbox;
        inbox.Open(FolderAccess.ReadOnly);

        //Console.WriteLine("Total messages: {0}", inbox.Count);
        //Console.WriteLine("Recent messages: {0}", inbox.Recent);

        for (var i = 0; i < inbox.Count; i++)
        {
            var message = inbox.GetMessage(i);
            //Console.WriteLine("Subject: {0}", message.Subject);
        }
    }

    #endregion Methods

    private static string _Key = "";
    protected static string EncryptionKey
    {
        get
        {
            if (string.IsNullOrEmpty(_Key))
            {
                _Key = ConfigurationManager.AppSettings.Get("EncryptMailAccountPassword");
            }

            return _Key;
        }
    }

    #region INotifyPropertyChanged Members
    //public event PropertyChangedEventHandler PropertyChanged;

    //private void NotifyPropertyChanged(string propertyName)
    //{
    //    if (PropertyChanged != null)
    //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //}
    #endregion

    public AuthenticationViewModel()
    {

    }
}
