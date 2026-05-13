using Cysharp.Threading.Tasks;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    public interface ICryptor
    {
        public byte[] Encrypt(byte[] data);
        public UniTask<byte[]> EncryptAsync(byte[] data, CancellationToken cancellationtoken = default);
        public byte[] Decrypt(byte[] data);
        public UniTask<byte[]> DecryptAsync(byte[] data, CancellationToken cancellationtoken = default);
    }
}
