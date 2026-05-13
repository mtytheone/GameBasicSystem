using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public sealed class DummyCryptor : ICryptor
    {
        byte[] ICryptor.Encrypt(byte[] data)
        {
            return data;
        }

        byte[] ICryptor.Decrypt(byte[] data)
        {
            return data;
        }

        async UniTask<byte[]> ICryptor.EncryptAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await UniTask.RunOnThreadPool(() => Encrypt(data), true, cancellationtoken);
        }

        async UniTask<byte[]> ICryptor.DecryptAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await UniTask.RunOnThreadPool(() => Decrypt(data), true, cancellationtoken);
        }

        private byte[] Encrypt(byte[] data)
        {
            return data;
        }

        private byte[] Decrypt(byte[] data)
        {
            return data;
        }
    }
}
