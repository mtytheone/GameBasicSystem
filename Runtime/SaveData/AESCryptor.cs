using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public sealed class AESCryptor : ICryptor
    {
        private const string AES_KEY_256 = @"$y38%Yf8h4o34d(?N~h7>f2gx0B_k9vq";
        private const string AES_IV_256 = @"H#=$HDG#'&JMGW)#";


        byte[] ICryptor.Encrypt(byte[] data)
        {
            return EncryptAes(data);
        }

        byte[] ICryptor.Decrypt(byte[] data)
        {   
            return DecryptAes(data);
        }

        async UniTask<byte[]> ICryptor.EncryptAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await EncryptAesAsync(data, cancellationtoken);
        }

        async UniTask<byte[]> ICryptor.DecryptAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await DecryptAesAsync(data, cancellationtoken);
        }

        private byte[] EncryptAes(byte[] sourceData)
        {
            byte[] encryptData = null;
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Encoding.UTF8.GetBytes(AES_KEY_256);
                aes.IV = Encoding.UTF8.GetBytes(AES_IV_256);

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new())
                using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write, true))
                {
                    cryptoStream.Write(sourceData, 0, sourceData.Length);
                    cryptoStream.FlushFinalBlock();
                    encryptData = memoryStream.ToArray();
                }
            }

            return encryptData;
        }

        private async UniTask<byte[]> EncryptAesAsync(byte[] sourceData, CancellationToken cancellationtoken)
        {
            byte[] encryptData = null;
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Encoding.UTF8.GetBytes(AES_KEY_256);
                aes.IV = Encoding.UTF8.GetBytes(AES_IV_256);

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new())
                using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write, true))
                {
                    await cryptoStream.WriteAsync(sourceData, 0, sourceData.Length, cancellationtoken);
                    cryptoStream.FlushFinalBlock();
                    encryptData = memoryStream.ToArray();
                }
            }

            return encryptData;
        }

        private byte[] DecryptAes(byte[] encryptedData)
        {
            byte[] sourceData = new byte[encryptedData.Length];
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Encoding.UTF8.GetBytes(AES_KEY_256);
                aes.IV = Encoding.UTF8.GetBytes(AES_IV_256);

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new(encryptedData))
                using (CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read, true))
                {
                    cryptoStream.Read(sourceData, 0, sourceData.Length);
                }
            }

            return sourceData;
        }

        private async UniTask<byte[]> DecryptAesAsync(byte[] encryptedData, CancellationToken cancellationtoken)
        {
            byte[] sourceData = new byte[encryptedData.Length];
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Encoding.UTF8.GetBytes(AES_KEY_256);
                aes.IV = Encoding.UTF8.GetBytes(AES_IV_256);

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new(encryptedData))
                using (CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read, true))
                {
                    await cryptoStream.ReadAsync(sourceData, 0, sourceData.Length, cancellationtoken);
                }
            }

            return sourceData;
        }
    }
}
