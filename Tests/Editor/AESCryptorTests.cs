using HatzeLaboratory.GameBasicSystem.Runtime.SaveData;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace HatzeLaboratory.GameBasicSystem.Editor.Tests
{
    public class AESCryptorTests
    {
        // AES-256は32バイトの鍵が必要
        private const string VALID_KEY = "TestKey_ForUnitTest_32BytesKey!!";
        private ICryptor _cryptor;

        [SetUp]
        public void SetUp()
        {
            _cryptor = new AESCryptor(VALID_KEY);
        }

        [Test]
        public void Encrypt_ThenDecrypt_ReturnsOriginalData()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("Hello, GameBasicSystem!");

            byte[] encryptedData = _cryptor.Encrypt(originalData);
            byte[] decryptedData = _cryptor.Decrypt(encryptedData);

            Assert.AreEqual(originalData, decryptedData);
        }

        [Test]
        public void Encrypt_SameInput_ReturnsDifferentCiphertext()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("SameData");

            byte[] encryptedData1 = _cryptor.Encrypt(originalData);
            byte[] encryptedData2 = _cryptor.Encrypt(originalData);
            
            Assert.AreNotEqual(encryptedData1, encryptedData2);
        }

        [Test]
        public void Decrypt_ThrowsInvalidDataException()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("SameData");
            byte[] encryptedData = _cryptor.Encrypt(originalData);

            // HMACの一部改ざん
            encryptedData[5] ^= 0xFF;
            Assert.Throws<InvalidDataException>(() => _cryptor.Decrypt(encryptedData));
        }

        [Test]
        public void Decrypt_TooShortData_ThrowsInvalidDataException()
        {
            byte[] invalidData = new byte[10]; // AESのブロックサイズより短い
            Assert.Throws<InvalidDataException>(() => _cryptor.Decrypt(invalidData));
        }

        [Test]
        public void Encrypt_EmptyData_CanBeDecrypted()
        {
            byte[] originalData = new byte[0];

            byte[] encryptedData = _cryptor.Encrypt(originalData);
            byte[] decryptedData = _cryptor.Decrypt(encryptedData);

            Assert.AreEqual(originalData, decryptedData);
        }

        [Test]
        public void Constructor_InvalidKeyLength_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new AESCryptor("ShortKey"));
            Assert.Throws<ArgumentException>(() => new AESCryptor("ThisKeyIsWayTooLongForAES256Encryption!!"));
        }

        [Test]
        public void Encrypt_NullData_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _cryptor.Encrypt(null));
        }

        [Test]
        public void Decrypt_NullData_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _cryptor.Decrypt(null));
        }

        [Test]
        public void Encrypt_LargeData_CanBeDecrypted()
        {
            byte[] originalData = new byte[1024 * 1024]; // 1MBのデータ
            new Random().NextBytes(originalData);

            byte[] encryptedData = _cryptor.Encrypt(originalData);
            byte[] decryptedData = _cryptor.Decrypt(encryptedData);

            Assert.AreEqual(originalData, decryptedData);
        }

        [Test]
        public void Encrypt_DecryptedDataIsDifferentFromOriginal()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("Hello, GameBasicSystem!");
            byte[] encryptedData = _cryptor.Encrypt(originalData);
            Assert.AreNotEqual(originalData, encryptedData);
        }

        [Test]
        public void Decrypt_ModifiedCiphertext_ThrowsInvalidDataException()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("Hello, GameBasicSystem!");
            byte[] encryptedData = _cryptor.Encrypt(originalData);

            // HMACの一部改ざん
            encryptedData[10] ^= 0xFF;
            Assert.Throws<InvalidDataException>(() => _cryptor.Decrypt(encryptedData));
        }

        [Test]
        public void Encrypt_ThenDecrypt_MultipleTimes_ReturnsOriginalData()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("Hello, GameBasicSystem!");
            for (int i = 0; i < 10; i++)
            {
                byte[] encryptedData = _cryptor.Encrypt(originalData);
                byte[] decryptedData = _cryptor.Decrypt(encryptedData);
                Assert.AreEqual(originalData, decryptedData);
            }
        }

        [Test]
        public void Encrypt_DecryptedDataIsNotReadable()
        {
            byte[] originalData = Encoding.UTF8.GetBytes("Hello, GameBasicSystem!");
            byte[] encryptedData = _cryptor.Encrypt(originalData);
            string encryptedString = Encoding.UTF8.GetString(encryptedData);
            Assert.AreNotEqual("Hello, GameBasicSystem!", encryptedString);
        }
    }
}
