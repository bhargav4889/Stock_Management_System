/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Stock_Management_System.UrlEncryption
{
    public static class UrlEncryptor
    {
        private static readonly string EncryptionKey = "A1b@C#d$E%F^G&H*";




        public static string Encrypt(string text)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[16];

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[16];

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }

}

*/

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Stock_Management_System.UrlEncryption
{
    public class UrlEncryptor
    {
        public static byte[] GenerateKey()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private static readonly byte[] Key = GenerateKey();
        private static readonly byte[] IV = GenerateKey().Take(16).ToArray(); // IV should be 16 bytes for AES.

        public static string Encrypt(string plainText)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    var encrypted = msEncrypt.ToArray();
                    // Use URL-safe Base64 encoding and replace '+' with '$'
                    return Convert.ToBase64String(encrypted).Replace('+', '$').Replace('/', '_').TrimEnd('=');
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace('$', '+').Replace('_', '/') + "==";
            var buffer = Convert.FromBase64String(cipherText);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(buffer))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}


