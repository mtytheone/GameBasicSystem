using Cysharp.Threading.Tasks;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    /// <summary>
    /// データの圧縮・解凍を行うインターフェース。
    /// カスタム圧縮を実装する場合はこのインターフェースを実装し、<see cref="SaveDataManagerBase{T}"/> に渡してください。
    /// </summary>
    public interface ICompressor
    {
        /// <summary>文字列をバイト配列に圧縮</summary>
        /// <param name="data">圧縮する文字列</param>
        /// <returns>圧縮されたバイト配列</returns>
        public byte[] Compress(string data);

        /// <summary>文字列を非同期でバイト配列に圧縮</summary>
        /// <param name="data">圧縮する文字列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>圧縮されたバイト配列</returns>
        public UniTask<byte[]> CompressAsync(string data, CancellationToken cancellationtoken = default);

        /// <summary>圧縮されたバイト配列を文字列に解凍</summary>
        /// <param name="data">解凍するバイト配列</param>
        /// <returns>解凍された文字列</returns>
        public string Decompress(byte[] data);

        /// <summary>圧縮されたバイト配列を非同期で文字列に解凍</summary>
        /// <param name="data">解凍するバイト配列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>解凍された文字列</returns>
        public UniTask<string> DecompressAsync(byte[] data, CancellationToken cancellationtoken = default);

        /// <summary>バイト配列をバイナリ形式で圧縮</summary>
        /// <param name="data">圧縮するバイト配列</param>
        /// <returns>圧縮されたバイト配列</returns>
        public byte[] CompressBinary(byte[] data);

        /// <summary>バイト配列を非同期でバイナリ形式で圧縮</summary>
        /// <param name="data">圧縮するバイト配列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>圧縮されたバイト配列</returns>
        public UniTask<byte[]> CompressBinaryAsync(byte[] data, CancellationToken cancellationtoken = default);

        /// <summary>バイナリ圧縮されたバイト配列を解凍</summary>
        /// <param name="data">解凍するバイト配列</param>
        /// <returns>解凍されたバイト配列</returns>
        public byte[] DecompressBinary(byte[] data);

        /// <summary>バイナリ圧縮されたバイト配列を非同期で解凍</summary>
        /// <param name="data">解凍するバイト配列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>解凍されたバイト配列</returns>
        public UniTask<byte[]> DecompressBinaryAsync(byte[] data, CancellationToken cancellationtoken = default);
    }
}
