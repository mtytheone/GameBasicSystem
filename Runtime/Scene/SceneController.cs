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
    /// <summary>
    /// Addressables を使用してシーン遷移を管理するシングルトンクラス。
    /// シーンは Project Settings > GameBasicSystem に登録した SceneType 名で識別されます。
    /// </summary>
    public sealed class SceneController : SingletonBehaviour<SceneController>
    {
        private string _currentSceneTypeName;
        private AsyncOperationHandle<SceneInstance> _currentSceneHandle;

        /// <summary>
        /// 指定した SceneType 名に対応するシーンを非同期でロードします。
        /// 既に同じシーンがアクティブな場合は何もしません。
        /// </summary>
        /// <param name="sceneTypeName">Project Settings > GameBasicSystemで設定されたシーンの名前</param>
        /// <param name="cancellationToken">実際のシーンロードが始まる前のキャンセル用トークン</param>
        /// <param name="onEndCallback">シーンロード完了後に呼び出されるコールバック</param>
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

            cancellationToken.ThrowIfCancellationRequested();
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single);
            await handle.ToUniTask(this);

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

        /// <summary>
        /// 現在アクティブなシーンの SceneType 名を返します。
        /// </summary>
        /// <returns>現在アクティブなシーンの名前</returns>
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