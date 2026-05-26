using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    /// <summary>
    /// AES-256-CBC 暗号化と HMAC-SHA256 による改ざん検知を組み合わせた <see cref="ICryptor"/> 実装。
    /// 暗号化データのフォーマットは <c>[HMAC(32バイト) | IV(16バイト) | 暗号文]</c> です。
    /// </summary>
    public sealed class AESCryptor : ICryptor
    {
        private readonly byte[] _key;

        /// <summary>
        /// 指定されたキーで <see cref="AESCryptor"/> を初期化します。
        /// </summary>
        /// <param name="key">UTF-8 エンコードで正確に 32 バイトになる暗号化キー</param>
        /// <exception cref="ArgumentException">キーの UTF-8 バイト長が 32 でない場合</exception>
        public AESCryptor(string key)
        {
            _key = Encoding.UTF8.GetBytes(key);
            if (_key.Length != 32)
            {
                throw new ArgumentException($"Key must be exactly 32 bytes in UTF-8" +
                    $" (actual: {_key.Length} bytes).",
                    nameof(key));
            }
        }

        byte[] ICryptor.Encrypt(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return EncryptAes(data);
        }

        byte[] ICryptor.Decrypt(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return DecryptAes(data);
        }

        async UniTask<byte[]> ICryptor.EncryptAsync(byte[] data, CancellationToken cancellationtoken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return await EncryptAesAsync(data, cancellationtoken);
        }

        async UniTask<byte[]> ICryptor.DecryptAsync(byte[] data, CancellationToken cancellationtoken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return await DecryptAesAsync(data, cancellationtoken);
        }

        private byte[] EncryptAes(byte[] sourceData)
        {
            byte[] encryptData = null;
            using (Aes aes = CreateAes())
            {
                aes.GenerateIV();
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new())
                using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write, true))
                {
                    cryptoStream.Write(sourceData, 0, sourceData.Length);
                    cryptoStream.FlushFinalBlock();

                    encryptData = CombineByte(aes.IV, memoryStream.ToArray());
                    byte[] hmac = ComputeHmac(encryptData);
                    encryptData = CombineByte(hmac, encryptData);
                }
            }

            return encryptData;
        }

        private async UniTask<byte[]> EncryptAesAsync(byte[] sourceData, CancellationToken cancellationtoken)
        {
            byte[] encryptData = null;
            using (Aes aes = CreateAes())
            {
                aes.GenerateIV();
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new())
                using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write, true))
                {
                    await cryptoStream.WriteAsync(sourceData, 0, sourceData.Length, cancellationtoken);
                    cryptoStream.FlushFinalBlock();

                    encryptData = CombineByte(aes.IV, memoryStream.ToArray());
                    byte[] hmac = ComputeHmac(encryptData);
                    encryptData = CombineByte(hmac, encryptData);
                }
            }

            return encryptData;
        }

        private byte[] DecryptAes(byte[] encryptedData)
        {
            VerifyHmac(encryptedData);

            byte[] ivAndCipher = encryptedData[32..];
            byte[] iv = ivAndCipher[..16];
            byte[] cipher = ivAndCipher[16..];
            using (Aes aes = CreateAes())
            {
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new(cipher))
                using (CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read, true))
                using (MemoryStream outputStream = new())
                {
                    cryptoStream.CopyTo(outputStream);
                    return outputStream.ToArray();
                }
            }
        }

        private async UniTask<byte[]> DecryptAesAsync(byte[] encryptedData, CancellationToken cancellationtoken)
        {
            VerifyHmac(encryptedData);

            byte[] ivAndCipher = encryptedData[32..];
            byte[] iv = ivAndCipher[..16];
            byte[] cipher = ivAndCipher[16..];
            using (Aes aes = CreateAes())
            {
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream memoryStream = new(cipher))
                using (CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read, true))
                using (MemoryStream outputStream = new())
                {
                    await cryptoStream.CopyToAsync(outputStream, cancellationtoken);
                    return outputStream.ToArray();
                }
            }
        }

        private Aes CreateAes()
        {
            Aes aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = _key;
            return aes;
        }

        private static byte[] CombineByte(byte[] a, byte[] b)
        {
            byte[] combinedByte = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, combinedByte, 0, a.Length);
            Buffer.BlockCopy(b, 0, combinedByte, a.Length, b.Length);
            return combinedByte;
        }

        private byte[] ComputeHmac(byte[] data)
        {
            using (HMACSHA256 hmac = new(_key))
            {
                return hmac.ComputeHash(data);
            }
        }

        private void VerifyHmac(byte[] data)
        {
            if (data.Length < 32)
            {
                throw new InvalidDataException("SaveData Invalid: Invalid data length.");
            }

            byte[] storedHmac = data[..32];
            byte[] ivAndCipher = data[32..];
            byte[] expectedHmac = ComputeHmac(ivAndCipher);
            if (!CryptographicOperations.FixedTimeEquals(storedHmac, expectedHmac))
            {
                throw new InvalidDataException("SaveData Invalid: HMAC verification failed.");
            }
        }
    }
}
