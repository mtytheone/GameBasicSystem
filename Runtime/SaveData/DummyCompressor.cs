using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System.Text;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public sealed class DummyCompressor : ICompressor
    {
        byte[] ICompressor.Compress(string data)
        {
            return Compress(data);
        }

        async UniTask<byte[]> ICompressor.CompressAsync(string data, CancellationToken cancellationtoken)
        {
            return await UniTask.RunOnThreadPool(() => Compress(data), true, cancellationtoken);
        }

        byte[] ICompressor.CompressBinary(byte[] data)
        {
            return CompressBinary(data);
        }

        async UniTask<byte[]> ICompressor.CompressBinaryAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await UniTask.RunOnThreadPool(() => CompressBinary(data), true, cancellationtoken);
        }

        string ICompressor.Decompress(byte[] data)
        {
            return Decompress(data);
        }

        async UniTask<string> ICompressor.DecompressAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await UniTask.RunOnThreadPool(() => Decompress(data), true, cancellationtoken);
        }

        byte[] ICompressor.DecompressBinary(byte[] data)
        {
            return DecompressBinary(data);
        }

        async UniTask<byte[]> ICompressor.DecompressBinaryAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await UniTask.RunOnThreadPool(() => DecompressBinary(data), true, cancellationtoken);
        }

        private byte[] Compress(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
        private byte[] CompressBinary(byte[] data)
        {
            return data;
        }

        private string Decompress(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        private byte[] DecompressBinary(byte[] data)
        {
            return data;
        }
    }
}
