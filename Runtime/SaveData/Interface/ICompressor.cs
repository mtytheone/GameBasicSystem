using Cysharp.Threading.Tasks;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    public interface ICompressor
    {
        public byte[] Compress(string data);
        public UniTask<byte[]> CompressAsync(string data, CancellationToken cancellationtoken = default);
        public string Decompress(byte[] data);
        public UniTask<string> DecompressAsync(byte[] data, CancellationToken cancellationtoken = default);
        public byte[] CompressBinary(byte[] data);
        public UniTask<byte[]> CompressBinaryAsync(byte[] data, CancellationToken cancellationtoken = default);
        public byte[] DecompressBinary(byte[] data);
        public UniTask<byte[]> DecompressBinaryAsync(byte[] data, CancellationToken cancellationtoken = default);
    }
}
