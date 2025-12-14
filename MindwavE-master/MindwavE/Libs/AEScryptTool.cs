using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptLib
{
    public class AESCryptoResult
    {
        public string CipherText { get; set; }
        public string Key { get; set; }
        public string IV { get; set; }
    }
    public class AEScryptTool
    {
        public static AESCryptoResult Encrypt(string plainText)
        {
            //Encrypt
            // 1. Generate Key и IV
            byte[] key = new byte[32]; // 256 bit
            byte[] iv = new byte[16];  // 128 bit block size

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }

            byte[] cipherBytes;

            // 2. Creating AES
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // 3. Create encryptor
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    // 4. convert plain text to bytes
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                    // 5. encrypt
                    cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }

            // 6. return result
            AESCryptoResult result = new AESCryptoResult();
            result.CipherText = Convert.ToBase64String(cipherBytes);
            result.Key = Convert.ToBase64String(key);
            result.IV = Convert.ToBase64String(iv);

            return result;
        }
        //Decrypt
        public static string Decrypt(string cipherBase64, string keyBase64, string ivBase64)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherBase64);
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);

            byte[] decryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
