using System.Security.Cryptography;
using System.Text;

namespace SpotifyDeviceFlow.Helpers
{
    public static class Crypto
    {
        public static string Encrypt(string text, string key)
        {
            using Aes aes = Aes.Create();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            Array.Resize(ref keyBytes, aes.Key.Length);
            aes.Key = keyBytes;
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

            byte[] result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);
            string encryptedText = Convert.ToBase64String(result);
            Console.WriteLine(encryptedText);
            return encryptedText;
        }

        public static string Descrypt(string encryptedText, string key)
        {
            using Aes aes = Aes.Create();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            Array.Resize(ref keyBytes, aes.Key.Length);
            aes.Key = keyBytes;

            byte[] iv = new byte[aes.IV.Length];
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
            byte[] encryptedTextBytes = new byte[encryptedBytes.Length - iv.Length];
            Buffer.BlockCopy(
                encryptedBytes,
                iv.Length,
                encryptedTextBytes,
                0,
                encryptedTextBytes.Length
            );

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);
            byte[] textBytes = decryptor.TransformFinalBlock(
                encryptedTextBytes,
                0,
                encryptedTextBytes.Length
            );
            return Encoding.UTF8.GetString(textBytes);
        }
    }
}
