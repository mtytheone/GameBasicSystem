using Cysharp.Threading.Tasks;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    /// <summary>
    /// データの暗号化・復号を行うインターフェース。
    /// カスタム暗号化を実装する場合はこのインターフェースを実装し、<see cref="SaveDataManagerBase{T}"/> に渡してください。
    /// </summary>
    public interface ICryptor
    {
        /// <summary>データを暗号化</summary>
        /// <param name="data">暗号化するバイト配列</param>
        /// <returns>暗号化されたバイト配列</returns>
        public byte[] Encrypt(byte[] data);

        /// <summary>データを非同期で暗号化します</summary>
        /// <param name="data">暗号化するバイト配列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>暗号化されたバイト配列</returns>
        public UniTask<byte[]> EncryptAsync(byte[] data, CancellationToken cancellationtoken = default);

        /// <summary>データを復号</summary>
        /// <param name="data">復号するバイト配列</param>
        /// <returns>復号されたバイト配列</returns>
        public byte[] Decrypt(byte[] data);

        /// <summary>データを非同期で復号</summary>
        /// <param name="data">復号するバイト配列</param>
        /// <param name="cancellationtoken">キャンセルトークン</param>
        /// <returns>復号されたバイト配列</returns>
        public UniTask<byte[]> DecryptAsync(byte[] data, CancellationToken cancellationtoken = default);
    }
}
