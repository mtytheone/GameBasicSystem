using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    /// <summary>
    /// セーブデータの管理基底クラス。デフォルトでは、JSONシリアライズ・GZip圧縮・AES暗号化を組み合わせてデータを保存・読み込みます。
    /// このクラスを継承し、セーブデータ型を <typeparamref name="T"/> に指定してください。
    /// </summary>
    /// <typeparam name="T">セーブデータの型。引数のないコンストラクタを持つ参照型である必要があります。</typeparam>
    public class SaveDataManagerBase<T> : Singleton<SaveDataManagerBase<T>> where T : class, new()
    {
        private T _saveData;
        private ISerializer<T> _serializer;
        private ICompressor _compresser;
        private ICryptor _cryptor;
        private IStreamer _streamer;

        /// <summary>
        /// ProjectSettings > GameBasicSystem で設定された暗号化キーを使用してインスタンスを初期化します。
        /// </summary>
        public SaveDataManagerBase() : this(null, null, null, null) { }

        /// <summary>
        /// 依存関係を明示的に指定してインスタンスを初期化します。<c>null</c> を渡した引数はデフォルト実装が使用されます。
        /// </summary>
        /// <param name="serializer">保存データを文字列に変換するためのシリアライザ。デフォルトでは、<see cref="JsonSerializer{T}"/>を使用しています。</param>
        /// <param name="compressor">バイトデータのコンプレッサ。デフォルトでは、<see cref="GZipCompressor"/>を使用しています。</param>
        /// <param name="cryptor">暗号化/復号化ツール。デフォルトでは、ProjectSettingsのキーを用いて<see cref="AESCryptor"/>が使用されます。</param>
        /// <param name="streamer">ファイルI/Oハンドラ。デフォルトでは、<see cref="DataStreamer"/>が使用されます。</param>
        public SaveDataManagerBase(ISerializer<T> serializer = null, ICompressor compressor = null, ICryptor cryptor = null, IStreamer streamer = null)
        {
            _serializer = serializer ?? new JsonSerializer<T>();
            _compresser = compressor ?? new GZipCompressor();
            _streamer = streamer ?? new DataStreamer();
            _cryptor = cryptor ?? CreateDefaultCryptor();
        }

        /// <summary>
        /// セーブデータをシリアライズ・圧縮・暗号化してディスクに書き込みします。
        /// </summary>
        public void Save()
        {
            string json = _serializer.Serialize(_saveData);
            byte[] compressed = _compresser.Compress(json);
            byte[] encrypted = _cryptor.Encrypt(compressed);
            _streamer.Save(encrypted);
            Debug.Log("SaveData Save Complete.");
        }

        /// <summary>
        /// セーブデータをシリアライズ・圧縮・暗号化してディスクに非同期書き込みします。
        /// </summary>
        /// <param name="cancellationtoken">キャンセル用のトークン</param>
        public async UniTask SaveAsync(CancellationToken cancellationtoken = default)
        {
            try
            {
                string json = _serializer.Serialize(_saveData);
                byte[] compressed = await _compresser.CompressAsync(json, cancellationtoken);
                byte[] encrypted = await _cryptor.EncryptAsync(compressed, cancellationtoken);
                await _streamer.SaveAsync(encrypted, cancellationtoken);
                Debug.Log("SaveData Save Complete.");
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationtoken)
            {
                if (cancellationtoken.IsCancellationRequested)
                {
                    Debug.Log("SaveData Save Canceled.");
                }
                else
                {
                    Debug.LogError($"Failed to save data.\n{e.Message}\n{e.StackTrace}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data.\n{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// ディスクからデータを読み込み、復号・解凍・デシリアライズします。
        /// ファイルが存在しない、またはデータが破損している場合は新規データを生成して <c>false</c> を返します。
        /// </summary>
        /// <returns>ロード成功時は<c>true</c>を返し、そうでない場合は<c>false</c>を返します。</returns>
        public bool Load()
        {
            byte[] encryptedData = _streamer.Load();
            if (encryptedData == null)
            {
                Debug.LogError("SaveData is not found.");
                return false;
            }

            try
            {
                byte[] decrypted = _cryptor.Decrypt(encryptedData);
                string json = _compresser.Decompress(decrypted);
                _saveData = _serializer.Deserialize(json);
                Debug.Log("Load Complete.");
                return true;
            }
            catch (InvalidDataException e)
            {
                Debug.LogError($"Failed to load data. Initialize data.\n{e.Message}\n{e.StackTrace}");
                CreateNewSaveData();
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data.\n{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// ディスクからデータを読み込み、復号・解凍・デシリアライズします。
        /// ファイルが存在しない、またはデータが破損している場合は新規データを生成して <c>false</c> を返します。
        /// </summary>
        /// <param name="cancellationtoken">キャンセル用のトークン</param>
        /// <returns>ロード成功時は<c>true</c>を返し、そうでない場合は<c>false</c>を返します。</returns>
        public async UniTask<bool> LoadAsync(CancellationToken cancellationtoken = default)
        {
            try
            {
                byte[] encryptedData = await _streamer.LoadAsync();
                if (encryptedData == null)
                {
                    Debug.LogError("SaveData is not found.");
                    return false;
                }

                byte[] decrypted = await _cryptor.DecryptAsync(encryptedData, cancellationtoken);
                string json = await _compresser.DecompressAsync(decrypted, cancellationtoken);
                _saveData = _serializer.Deserialize(json);
                Debug.Log("SaveData Load Complete.");
                return true;
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationtoken)
            {
                if (cancellationtoken.IsCancellationRequested)
                {
                    Debug.Log("SaveData Load Canceled.");
                }
                else
                {
                    Debug.LogError($"Failed to load data.\n{e.Message}\n{e.StackTrace}");
                }
            }
            catch (InvalidDataException e)
            {
                Debug.LogError($"Failed to load data. Initialize data.\n{e.Message}\n{e.StackTrace}");
                CreateNewSaveData();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data.\n{e.Message}\n{e.StackTrace}");
            }

            return false;
        }

        /// <summary>
        /// 現在読み込まれているセーブデータを返します。
        /// このメソッドを呼ぶ前に <see cref="Load"/> または <see cref="LoadAsync"/> を実行してください。
        /// </summary>
        /// <returns>ロード済みであればセーブデータインスタンス、そうでなければ<c>null</c></returns>
        public T GetSaveData()
        {
            if (_saveData == null)
            {
                Debug.LogError("SaveData is not found.\nLoad SaveData First!");
                return null;
            }

            return _saveData;
        }

        /// <summary>
        /// セーブデータを <typeparamref name="T"/> の新しいデフォルトインスタンスで初期化します。
        /// </summary>
        public void CreateNewSaveData()
        {
            _saveData = new T();
        }

        private static ICryptor CreateDefaultCryptor()
        {
            var settings = GameBasicSystemSettingData.Instance;
            if (!settings)
            {
                throw new InvalidOperationException("GameBasicSystemSettingData is not found." +
                                                    " Please configure it in \"Project Settings > GameBasicSystem\"");
            }

            if (string.IsNullOrEmpty(settings.SaveDataEncryptionKey))
            {
                throw new InvalidOperationException("SaveDataEncryptionKey is not set or is not 32 bytes. " +
                                                    "Please configure it in \"Project Settings > GameBasicSystem.\"");
            }

            return new AESCryptor(settings.SaveDataEncryptionKey);
        }
    }
}
