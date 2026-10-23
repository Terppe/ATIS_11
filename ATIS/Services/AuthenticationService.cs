using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Configuration;

namespace ATIS.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDbContextFactory<AtisDbContext> _factory;
    private readonly IDataService _dataService;

    public AuthenticationService(
        IDbContextFactory<AtisDbContext> factory,
        IDataService dataService)
    {
        _factory = factory;
        _dataService = dataService;
    }

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


public async Task<bool> RequestPasswordResetAsync(
    string email,
    CancellationToken ct = default)
{
    email = (email ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(email))
        return false;

    await using var db = await _factory.CreateDbContextAsync(ct);

    var user = await db.TblUserProfiles.FirstOrDefaultAsync(u => u.Email == email, ct);
    if (user is null)
        return false;

    // 1) neues Klartext-Passwort erzeugen
    var newPassword = GeneratePassword(10);

    // 2) gehasht speichern (wie Alt-System: Passwort + Email)
    user.Password = Crypt.CalculateHash(newPassword, user.Email);

    await _dataService.UpsertUserProfileAsync(user);

    // 3) Mail senden
    var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
    var smtpPortText = ConfigurationManager.AppSettings["SmtpPort"];
    var sender = ConfigurationManager.AppSettings["Sender"];
    var encryptedMailPwd = ConfigurationManager.AppSettings["EncryptMailAccountPassword"];

    if (string.IsNullOrWhiteSpace(smtpHost) ||
        string.IsNullOrWhiteSpace(smtpPortText) ||
        string.IsNullOrWhiteSpace(sender) ||
        string.IsNullOrWhiteSpace(encryptedMailPwd))
    {
        throw new InvalidOperationException("SMTP-Konfiguration ist unvollständig.");
    }

    if (!int.TryParse(smtpPortText, out var smtpPort))
        throw new InvalidOperationException("SMTP-Port ist ungültig.");

    var mailPassword = Crypt.DecryptString(encryptedMailPwd);

    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Hotline T-Soft", sender));
    message.To.Add(new MailboxAddress("User", email));
    message.Subject = "Passwort zurückgesetzt";
    message.Body = new TextPart("plain")
    {
        Text = $"Ihr neues Passwort lautet: {newPassword}"
    };

    using var smtp = new SmtpClient();
    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // wie Alt-Code
    await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, ct);
    await smtp.AuthenticateAsync(sender, mailPassword, ct);
    await smtp.SendAsync(message, ct);
    await smtp.DisconnectAsync(true, ct);

    return true;
}

private static string GeneratePassword(int length)
{
    const string chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
}
public async Task ChangePasswordAsync(
        int userProfileId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
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

    public async Task ChangePasswordByEmailAsync(
        string email,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
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
}


//public class AuthenticationService : IAuthenticationService
//{
//    public TblUserProfile? AuthenticateUser(string email, string password)
//        => throw new NotImplementedException();

//    public Task<TblUserProfile> RegisterAsync(
//        string email,
//        string password,
//        string? firstName = null,
//        string? lastName = null,
//        string? role = null,
//        CancellationToken ct = default)
//        => throw new NotImplementedException();

//    public Task<bool> RequestPasswordResetAsync(
//        string email,
//        CancellationToken ct = default)
//        => throw new NotImplementedException();

//    public Task ChangePasswordAsync(
//        int userProfileId,
//        string currentPassword,
//        string newPassword,
//        CancellationToken ct = default)
//        => throw new NotImplementedException();

//    public Task ChangePasswordByEmailAsync(
//        string email,
//        string currentPassword,
//        string newPassword,
//        CancellationToken ct = default)
//        => throw new NotImplementedException();
//}
