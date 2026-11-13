using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using ATIS.Helpers;

namespace ATIS.Services;

[SupportedOSPlatform("windows")]
public sealed class MailCredentialStore
{
    private readonly string _passwordPath;
    private readonly string _legacyPath;

    public MailCredentialStore(string? directory = null)
    {
        directory ??= Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ATIS", "Credentials");
        _passwordPath = Path.Combine(directory, "smtp-password.dpapi");
        _legacyPath = Path.Combine(directory, "legacy-mail-password.dpapi");
    }

    public bool HasPassword => File.Exists(_passwordPath);

    public string LoadPassword()
    {
        if (!HasPassword)
            throw new InvalidOperationException(
                "Kein SMTP-Passwort gespeichert. Bitte in den Einstellungen einrichten.");

        return ReadProtected(_passwordPath);
    }

    public void SavePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Das SMTP-Passwort darf nicht leer sein.", nameof(password));

        WriteProtected(_passwordPath, password, overwrite: true);
    }

    public bool MigrateLegacyValues(string? encryptedMailAccountPassword, string? cryptMailPassword)
    {
        var migrated = false;
        if (HasPassword)
        {
            _ = LoadPassword();
        }
        else if (!string.IsNullOrWhiteSpace(encryptedMailAccountPassword))
        {
            var password = Crypt.DecryptString(encryptedMailAccountPassword);
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Das bisherige SMTP-Passwort ist leer.");

            WriteProtected(_passwordPath, password, overwrite: false);
            _ = LoadPassword();
            migrated = true;
        }

        // The unused legacy value has an unknown format; preserve it without interpreting it.
        if (File.Exists(_legacyPath))
        {
            _ = ReadProtected(_legacyPath);
        }
        else if (!string.IsNullOrWhiteSpace(cryptMailPassword))
        {
            WriteProtected(_legacyPath, cryptMailPassword, overwrite: false);
            _ = ReadProtected(_legacyPath);
            migrated = true;
        }

        return migrated;
    }

    private static string ReadProtected(string path)
    {
        var protectedBytes = File.ReadAllBytes(path);
        var clearBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
        try
        {
            return Encoding.UTF8.GetString(clearBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(clearBytes);
        }
    }

    private static void WriteProtected(string path, string value, bool overwrite)
    {
        var clearBytes = Encoding.UTF8.GetBytes(value);
        byte[] protectedBytes;
        try
        {
            protectedBytes = ProtectedData.Protect(clearBytes, null, DataProtectionScope.CurrentUser);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(clearBytes);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllBytes(temporaryPath, protectedBytes);
            File.Move(temporaryPath, path, overwrite);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }
}
