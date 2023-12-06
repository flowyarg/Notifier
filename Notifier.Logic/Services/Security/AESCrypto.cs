using Notifier.Logic.Extensions;
using System.Security.Cryptography;

namespace Notifier.Logic.Services.Security
{
    public class AESCrypto
    {
        private const int _keyLength = 256 / 8;
        private const int _blockSize = 128 / 8;

        public string Encrypt(string data, string key, string iv)
        {
            using var aes = Aes.Create();

            aes.Key = GetKeyFromString(key, _keyLength);
            aes.IV = GetKeyFromString(iv, _blockSize);

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(data);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string data, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Key = GetKeyFromString(key, _keyLength);
            aes.IV = GetKeyFromString(iv, _blockSize);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(data));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var swDecrypt = new StreamReader(csDecrypt);
            return swDecrypt.ReadToEnd();
        }

        public byte[] GetKeyFromString(string value, int size) => value.GetStringHash512()[..size];
    }
}
