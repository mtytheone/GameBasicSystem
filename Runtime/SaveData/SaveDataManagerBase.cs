using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System;
using System.Threading;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public class SaveDataManagerBase<T> : Singleton<SaveDataManagerBase<T>> where T : class, new()
    {
        private T _saveData;
        private ISerializer<T> _serializer;
        private ICompressor _compresser;
        private ICryptor _cryptor;
        private IStreamer _streamer;


        public SaveDataManagerBase()
        {
            _serializer = new JsonSerializer<T>();
            _compresser = new GZipCompressor();
            _cryptor = new AESCryptor();
            _streamer = new DataStreamer();
        }

        public void SaveSync()
        {
            string json = _serializer.Serialize(_saveData);
            byte[] compressed = _compresser.Compress(json);
            byte[] encrypted = _cryptor.Encrypt(compressed);
            _streamer.Save(encrypted);
            Debug.Log("SaveData Save Complete.");
        }

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

        public bool LoadSync()
        {
            byte[] encryptedData = _streamer.Load();
            if (encryptedData == null)
            {
                Debug.LogError("SaveData is not found.");
                return false;
            }

            byte[] decrypted = _cryptor.Decrypt(encryptedData);
            string json = _compresser.Decompress(decrypted);
            _saveData = _serializer.Deserialize(json);
            Debug.Log("Load Complete.");
            return true;
        }

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
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data.\n{e.Message}\n{e.StackTrace}");
            }

            return false;
        }

        public T GetSaveData()
        {
            if (_saveData == null)
            {
                Debug.LogError("SaveData is not found.\nLoad SaveData First!");
                return null;
            }

            return _saveData;
        }

        public void CreateNewSaveData()
        {
            _saveData = new T();
        }
    }
}
