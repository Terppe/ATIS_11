using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace ATIS.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDbContextFactory<AtisDbContext> _factory;
    private readonly IDataService _dataService;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public AuthenticationService(
        IDbContextFactory<AtisDbContext> factory,
        IDataService dataService)
    {
        _factory = factory;
        _dataService = dataService;
    }

    #region [ METHODS ]

    public async Task<TblUserProfile?> AuthenticateUserAsync(string email, string password, CancellationToken ct = default)
    {
        email = (email ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var hash = Crypt.CalculateHash(password, email);

        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.TblUserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == hash, ct);
    }

    public async Task<TblUserProfile> RegisterAsync(
        string email,
        string password,
        string? role = null,
        CancellationToken ct = default)
    {
        email = (email ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("E-Mail ist erforderlich.", nameof(email));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Passwort ist erforderlich.", nameof(password));

        await using var db = await _factory.CreateDbContextAsync(ct);
        if (await db.TblUserProfiles.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("E-Mail ist bereits registriert.");

        var profile = new TblUserProfile
        {
            Email = email,
            Password = Crypt.CalculateHash(password, email), // WICHTIG
            Role = string.IsNullOrWhiteSpace(role) ? "User" : role.Trim(),
        };

        return await _dataService.UpsertUserProfileAsync(profile);
    }

    public async Task<bool> RequestPasswordResetAsync(string email, CancellationToken ct = default)
    {
        email = (email ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(email)) return false;

        await using var db = await _factory.CreateDbContextAsync(ct);
        var user = await db.TblUserProfiles.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null) return false;

        // 8-stelliger Code
        var code = RandomNumberGenerator.GetInt32(0, 100_000_000).ToString("D8");

        user.PasswordResetToken = HashToken(code);
        user.PasswordResetTokenExpiryUtc = DateTime.UtcNow.AddMinutes(30);
        await _dataService.UpsertUserProfileAsync(user);

        // SMTP-Konfig laden
        var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        var smtpPortText = ConfigurationManager.AppSettings["SmtpPort"];
        var sender = ConfigurationManager.AppSettings["Sender"];
        var encryptedMailPwd = ConfigurationManager.AppSettings["EncryptMailAccountPassword"];

        if (string.IsNullOrWhiteSpace(smtpHost) ||
            string.IsNullOrWhiteSpace(smtpPortText) ||
            string.IsNullOrWhiteSpace(sender) ||
            string.IsNullOrWhiteSpace(encryptedMailPwd))
            throw new InvalidOperationException("SMTP-Konfiguration ist unvollständig.");

        if (!int.TryParse(smtpPortText, out var smtpPort))
            throw new InvalidOperationException("SMTP-Port ist ungültig.");

        var mailPassword = Crypt.DecryptString(encryptedMailPwd);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("ATIS Support", sender));
        message.To.Add(MailboxAddress.Parse(user.Email));
        message.Subject = "Passwort zurücksetzen";
        message.Body = new TextPart("plain")
        {
            Text = $"Ihr Code zum Zurücksetzen lautet: {code}\n\nGültig für 30 Minuten."
        };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls, ct);
        await smtp.AuthenticateAsync(sender, mailPassword, ct);
        await smtp.SendAsync(message, ct);
        await smtp.DisconnectAsync(true, ct);

        return true;
    }

    public async Task ResetPasswordWithCodeAsync(string email, string code, string newPassword, CancellationToken ct = default)
    {
        email = (email ?? string.Empty).Trim();
        code = (code ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            throw new InvalidOperationException("E-Mail und Code sind erforderlich.");
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Neues Passwort ist erforderlich.", nameof(newPassword));

        await using var db = await _factory.CreateDbContextAsync(ct);
        var user = await db.TblUserProfiles.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user?.PasswordResetToken is null
            || user.PasswordResetTokenExpiryUtc is null
            || user.PasswordResetTokenExpiryUtc < DateTime.UtcNow
            || !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(user.PasswordResetToken),
                Encoding.UTF8.GetBytes(HashToken(code))))
        {
            throw new InvalidOperationException("Code ungültig oder abgelaufen.");
        }

        user.Password = Crypt.CalculateHash(newPassword, user.Email);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiryUtc = null;

        await _dataService.UpsertUserProfileAsync(user);
    }
    private static string HashToken(string token)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static string GeneratePassword(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
    public async Task ChangePasswordAsync(int userProfileId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        if (userProfileId <= 0)
            throw new ArgumentException("Ungültige UserProfileId.", nameof(userProfileId));

        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("Aktuelles Passwort ist erforderlich.", nameof(currentPassword));

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Neues Passwort ist erforderlich.", nameof(newPassword));

        await using var db = await _factory.CreateDbContextAsync(ct);

        var user = await db.TblUserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserProfileId == userProfileId, ct);

        if (user is null)
            throw new InvalidOperationException("Benutzer nicht gefunden.");

        if (user.Password != Crypt.CalculateHash(currentPassword, user.Email))
            throw new InvalidOperationException("Aktuelles Passwort ist falsch.");

        user.Password = Crypt.CalculateHash(newPassword, user.Email);

        await _dataService.UpsertUserProfileAsync(user);
    }

    public async Task ChangePasswordByEmailAsync(string email, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        email = (email ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-Mail ist erforderlich.", nameof(email));

        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("Aktuelles Passwort ist erforderlich.", nameof(currentPassword));

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Neues Passwort ist erforderlich.", nameof(newPassword));

        await using var db = await _factory.CreateDbContextAsync(ct);

        var user = await db.TblUserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, ct);

        if (user is null)
            throw new InvalidOperationException("Benutzer nicht gefunden.");

        if (user.Password != Crypt.CalculateHash(currentPassword, user.Email))
            throw new InvalidOperationException("Aktuelles Passwort ist falsch.");

        user.Password = Crypt.CalculateHash(newPassword, user.Email);

        await _dataService.UpsertUserProfileAsync(user);
    }

    public async Task DeleteOwnAccountAsync(string email, string currentPassword, CancellationToken ct = default)
    {
        email = (email ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-Mail ist erforderlich.", nameof(email));
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("Aktuelles Passwort ist erforderlich.", nameof(currentPassword));

        await using var db = await _factory.CreateDbContextAsync(ct);
        var user = await db.TblUserProfiles.FirstOrDefaultAsync(x => x.Email == email, ct);

        if (user is null)
            throw new InvalidOperationException("Benutzer nicht gefunden.");

        var hash = Crypt.CalculateHash(currentPassword, user.Email);
        if (user.Password != hash)
            throw new InvalidOperationException("Passwort ist falsch.");

        db.TblUserProfiles.Remove(user);
        await db.SaveChangesAsync(ct);
    }
    #endregion
}
