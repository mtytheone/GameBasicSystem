using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Scene
{
    public sealed class SceneController : SingletonBehaviour<SceneController>
    {
        private string _currentSceneTypeName;
        private AsyncOperationHandle<SceneInstance> _currentSceneHandle;

        public async UniTask LoadSceneAsync(string sceneTypeName, CancellationToken cancellationToken, Action onEndCallback)
        {
            if (_currentSceneTypeName == sceneTypeName)
            {
                Debug.LogWarning($"Scene type {sceneTypeName} is already loaded.");
                return;
            }

            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;
            if (!settingData)
            {
                Debug.LogError("SettingData is null.");
                return;
            }

            string address = settingData.GetSceneAddress(sceneTypeName);
            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single);
            await handle.ToUniTask(this);

            cancellationToken.ThrowIfCancellationRequested();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                SetActiveScene(handle.Result);
                UpdateCurrentSceneType(sceneTypeName);
                UpdateCacheHandle(handle);
            }
            else
            {
                Debug.LogError($"Failed to load scene: {address}");
            }

            onEndCallback?.Invoke();
        }

        public string GetCurrentSceneType()
        {
            return _currentSceneTypeName;
        }

        private void SetActiveScene(SceneInstance sceneInstance)
        {
            UnityEngine.SceneManagement.Scene scene = sceneInstance.Scene;
            SceneManager.SetActiveScene(scene);
        }

        private void UpdateCurrentSceneType(string newSceneTypeName)
        {
            if (_currentSceneTypeName == newSceneTypeName)
            {
                Debug.LogWarning($"Scene type {newSceneTypeName} is already active.");
                return;
            }

            _currentSceneTypeName = newSceneTypeName;
        }

        private void UpdateCacheHandle(AsyncOperationHandle<SceneInstance> newHandle)
        {
            if (_currentSceneHandle.IsValid())
            {
                Addressables.Release(_currentSceneHandle);
            }

            _currentSceneHandle = newHandle;
        }
    }
}