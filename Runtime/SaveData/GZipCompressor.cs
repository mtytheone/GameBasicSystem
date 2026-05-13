using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public sealed class GZipCompressor : ICompressor
    {
        byte[] ICompressor.Compress(string data)
        {
            return CompressBinary(Encoding.UTF8.GetBytes(data));
        }

        async UniTask<byte[]> ICompressor.CompressAsync(string data, CancellationToken cancellationtoken)
        {
            return await CompressBinaryAsync(Encoding.UTF8.GetBytes(data), cancellationtoken);
        }

        byte[] ICompressor.CompressBinary(byte[] data)
        {
            return CompressBinary(data);
        }

        async UniTask<byte[]> ICompressor.CompressBinaryAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await CompressBinaryAsync(data, cancellationtoken);
        }

        string ICompressor.Decompress(byte[] data)
        {
            return Encoding.UTF8.GetString(DecompressBinary(data));
        }

        async UniTask<string> ICompressor.DecompressAsync(byte[] data, CancellationToken cancellationtoken)
        {
            byte[] decompressedData = await DecompressBinaryAsync(data, cancellationtoken);
            return Encoding.UTF8.GetString(decompressedData);
        }

        byte[] ICompressor.DecompressBinary(byte[] data)
        {
            return DecompressBinary(data);
        }

        async UniTask<byte[]> ICompressor.DecompressBinaryAsync(byte[] data, CancellationToken cancellationtoken)
        {
            return await DecompressBinaryAsync(data, cancellationtoken);
        }

        private byte[] CompressBinary(byte[] data)
        {
            byte[] compressedData = null;
            using (MemoryStream compressFileStream = new())
            using (GZipStream gzipStream = new(compressFileStream, CompressionMode.Compress))
            {
                gzipStream.Write(data, 0, data.Length);
                gzipStream.Close();
                compressedData = compressFileStream.ToArray();
            }

            return compressedData;
        }

        private async UniTask<byte[]> CompressBinaryAsync(byte[] data, CancellationToken cancellationtoken)
        {
            byte[] compressedData = null;
            using (MemoryStream compressFileStream = new())
            using (GZipStream gzipStream = new(compressFileStream, CompressionMode.Compress))
            {
                await gzipStream.WriteAsync(data, 0, data.Length, cancellationtoken);
                gzipStream.Close();
                compressedData = compressFileStream.ToArray();
            }

            return compressedData;
        }

        private byte[] DecompressBinary(byte[] data)
        {
            byte[] decompressedData = null;
            using (MemoryStream compressedDataStream = new(data))
            using (GZipStream gzipStream = new(compressedDataStream, CompressionMode.Decompress))
            using (MemoryStream decompressFileStrean = new())
            {
                gzipStream.CopyTo(decompressFileStrean);
                decompressedData = decompressFileStrean.ToArray();
            }

            return decompressedData;
        }

        private async UniTask<byte[]> DecompressBinaryAsync(byte[] data, CancellationToken cancellationtoken)
        {
            byte[] decompressedData = null;
            using (MemoryStream compressedDataStream = new(data))
            using (GZipStream gzipStream = new(compressedDataStream, CompressionMode.Decompress))
            using (MemoryStream decompressFileStrean = new())
            {
                await gzipStream.CopyToAsync(decompressFileStrean, cancellationtoken);
                decompressedData = decompressFileStrean.ToArray();
            }

            return decompressedData;
        }
    }
}
