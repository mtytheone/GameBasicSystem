using Cysharp.Threading.Tasks;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    /// <summary>
    /// データのファイル入出力を行うインターフェース。
    /// カスタムの保存先を実装する場合はこのインターフェースを実装し、<see cref="SaveDataManagerBase{T}"/> に渡してください。
    /// </summary>
    public interface IStreamer
    {
        /// <summary>バイト配列をファイルに書き込み</summary>
        /// <param name="data">書き込むバイト配列</param>
        public void Save(byte[] data);

        /// <summary>バイト配列をファイルに非同期書き込み</summary>
        /// <param name="data">書き込むバイト配列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        public UniTask SaveAsync(byte[] data, CancellationToken cancellationtoken = default);

        /// <summary>ファイルからバイト配列を読み込み</summary>
        /// <returns>読み込まれたバイト配列。ファイルが存在しない場合は <c>null</c></returns>
        public byte[] Load();

        /// <summary>ファイルからバイト配列を非同期読み込み</summary>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>読み込まれたバイト配列。ファイルが存在しない場合は <c>null</c></returns>
        public UniTask<byte[]> LoadAsync(CancellationToken cancellationtoken = default);
    }
}
