using System.Security.Cryptography;
using System.Runtime.Versioning;
using System.Text;
using ATIS.Helpers;
using ATIS.Services;

[assembly: SupportedOSPlatform("windows")]

if (!OperatingSystem.IsWindows())
{
    Console.WriteLine("DPAPI checks require Windows. Run this project on Windows or in CI.");
    return;
}

var directory = Path.Combine(Path.GetTempPath(), $"atis-mail-{Guid.NewGuid():N}");
Directory.CreateDirectory(directory);
try
{
    void Check(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }

    void ExpectFailure<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T)
        {
            return;
        }
        throw new Exception($"Expected {typeof(T).Name}");
    }

    var store = new MailCredentialStore(directory);
    Check(!store.HasPassword, "Missing password reported as stored");
    ExpectFailure<InvalidOperationException>(() => store.LoadPassword());
    Check(!store.MigrateLegacyValues(null, null), "Missing legacy values created credentials");
    Check(Directory.GetFiles(directory).Length == 0, "Empty migration wrote files");
    ExpectFailure<ArgumentException>(() => store.SavePassword(" "));

    const string firstPassword = "Synthetic-test-password-1";
    const string unusedLegacyValue = "Synthetic-unused-legacy-value";
    Check(store.MigrateLegacyValues(Crypt.EncryptString(firstPassword), unusedLegacyValue),
        "Legacy values not migrated");
    Check(new MailCredentialStore(directory).LoadPassword() == firstPassword, "DPAPI round trip failed");

    var passwordPath = Path.Combine(directory, "smtp-password.dpapi");
    var legacyPath = Path.Combine(directory, "legacy-mail-password.dpapi");
    var protectedPassword = File.ReadAllBytes(passwordPath);
    var protectedLegacy = File.ReadAllBytes(legacyPath);
    Check(!Encoding.UTF8.GetString(protectedPassword).Contains(firstPassword),
        "Password stored in plaintext");
    Check(!Encoding.UTF8.GetString(protectedLegacy).Contains(unusedLegacyValue),
        "Legacy backup stored in plaintext");
    var legacyBytes = ProtectedData.Unprotect(protectedLegacy, null, DataProtectionScope.CurrentUser);
    try
    {
        Check(Encoding.UTF8.GetString(legacyBytes) == unusedLegacyValue, "Legacy backup changed");
    }
    finally
    {
        CryptographicOperations.ZeroMemory(legacyBytes);
    }
    Check(!store.MigrateLegacyValues("obsolete-value", "obsolete-value"), "Existing credentials replaced");
    Check(protectedPassword.SequenceEqual(File.ReadAllBytes(passwordPath)), "Migration rewrote password");
    Check(protectedLegacy.SequenceEqual(File.ReadAllBytes(legacyPath)), "Migration rewrote legacy backup");

    store.SavePassword("Synthetic-replacement-password");
    Check(new MailCredentialStore(directory).LoadPassword() == "Synthetic-replacement-password",
        "Replacement not persisted");
    Check(!store.MigrateLegacyValues(Crypt.EncryptString(firstPassword), unusedLegacyValue),
        "Legacy values overwrote replacement");
    Check(store.LoadPassword() == "Synthetic-replacement-password", "Old config restored old password");

    File.WriteAllBytes(passwordPath, [1, 2, 3]);
    ExpectFailure<CryptographicException>(() => store.LoadPassword());
    ExpectFailure<CryptographicException>(() =>
        store.MigrateLegacyValues(Crypt.EncryptString(firstPassword), null));
    Check(File.ReadAllBytes(passwordPath).SequenceEqual(new byte[] { 1, 2, 3 }),
        "Corrupt file silently replaced from config");
    store.SavePassword(firstPassword);
    Check(store.LoadPassword() == firstPassword, "Recovery by saving new password failed");
    Check(!Directory.GetFiles(directory).Any(path => path.EndsWith(".tmp")), "Temporary files remain");

    var invalidDirectory = Path.Combine(directory, "invalid");
    var invalidStore = new MailCredentialStore(invalidDirectory);
    ExpectFailure<FormatException>(() => invalidStore.MigrateLegacyValues("not base64", null));
    Check(!invalidStore.HasPassword, "Invalid legacy value created password");

    var blockedDirectory = Path.Combine(directory, "blocked");
    File.WriteAllText(blockedDirectory, "not a directory");
    var blockedStore = new MailCredentialStore(blockedDirectory);
    ExpectFailure<IOException>(() => blockedStore.SavePassword(firstPassword));

    Console.WriteLine("Mail credential DPAPI checks passed.");
}
finally
{
    Directory.Delete(directory, recursive: true);
}
