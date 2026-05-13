using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public sealed class DataStreamer : IStreamer
    {
        private const string SaveDataFileName = "SaveData.hld";
        private readonly string _saveDataPath = Path.Combine(Application.persistentDataPath, SaveDataFileName);


        void IStreamer.Save(byte[] data)
        {
            try
            {
                using (FileStream sourceFileStrean = new(_saveDataPath, FileMode.OpenOrCreate, FileAccess.Write))
                using (StreamWriter writer = new(sourceFileStrean, Encoding.UTF8))
                {
                    string base64 = Convert.ToBase64String(data);
                    writer.Write(base64);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data.\n{e.Message}\n{e.StackTrace}");
            }
        }

        async UniTask IStreamer.SaveAsync(byte[] data, CancellationToken cancellationtoken)
        {
            try
            {
                using (FileStream sourceFileStrean = new(_saveDataPath, FileMode.OpenOrCreate, FileAccess.Write))
                using (StreamWriter writer = new(sourceFileStrean, Encoding.UTF8))
                {
                    string base64 = Convert.ToBase64String(data);
                    cancellationtoken.ThrowIfCancellationRequested();
                    await writer.WriteAsync(base64);
                }
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationtoken && cancellationtoken.IsCancellationRequested)
            {
                Debug.Log("Save Canceled.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data.\n{e.Message}\n{e.StackTrace}");
            }
        }

        byte[] IStreamer.Load()
        {
            try
            {
                using (FileStream fileStrean = new(_saveDataPath, FileMode.Open, FileAccess.Read))
                using (StreamReader streamReader = new(fileStrean, Encoding.UTF8, false))
                {
                    string base64 = streamReader.ReadToEnd();
                    return Convert.FromBase64String(base64);
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.LogWarning($"SaveData is not found.\n{e.Message}\n{e.StackTrace}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data.\n{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        async UniTask<byte[]> IStreamer.LoadAsync(CancellationToken cancellationtoken)
        {
            try
            {
                using (FileStream fileStrean = new(_saveDataPath, FileMode.Open, FileAccess.Read))
                using (StreamReader streamReader = new(fileStrean, Encoding.UTF8, false))
                {
                    cancellationtoken.ThrowIfCancellationRequested();
                    string base64 = await streamReader.ReadToEndAsync();
                    return Convert.FromBase64String(base64);
                }
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationtoken && cancellationtoken.IsCancellationRequested)
            {
                Debug.Log("Load Canceled.");
                return null;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogWarning($"SaveData is not found.\n{e.Message}\n{e.StackTrace}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data.\n{e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
}
