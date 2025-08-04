using System.Security.Cryptography;
using System.Text;

namespace ATIS.Helpers;

public static class Crypt
{
    //public static string CalculateHash(string clearTextPassword, string salt)
    //{
    //    // Convert the salted password to a byte array
    //    var saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
    //    // Use the hash algorithm to calculate the hash
    //    //   HashAlgorithm algorithm = new SHA256Managed();
    //    HashAlgorithm algorithm = new SHA512Managed();
    //    var hash = algorithm.ComputeHash(saltedHashBytes);
    //    // Return the hash as a base64 encoded string to be compared to the stored password
    //    return Convert.ToBase64String(hash);
    //}

    public static string CalculateHash(string clearTextPassword, string saltusername)
    {
        var saltedPassword = $"{clearTextPassword}{saltusername}";
        var saltedPasswordAsBytes = Encoding.UTF8.GetBytes(saltedPassword);
        return Convert.ToBase64String(SHA512.HashData(saltedPasswordAsBytes));
    }
    //---------------------------Encrypt and Decrypt only for MailAccountPassword---------------

    public static string EncryptString(string plainText)
    {
        const string key = "b14ca5898a4e4133bbce2ea2315a1916";
        var iv = new byte[16];
        byte[] array;

        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter((Stream)cryptoStream))
            {
                streamWriter.Write(plainText);
            }

            array = memoryStream.ToArray();
        }
        return Convert.ToBase64String(array);
    }

    public static string DecryptString(string cipherText)
    {
        const string key = "b14ca5898a4e4133bbce2ea2315a1916";
        var iv = new byte[16];
        var buffer = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader((Stream)cryptoStream);
        return streamReader.ReadToEnd();
    }


}



