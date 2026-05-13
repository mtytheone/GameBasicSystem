using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Scene
{
    public sealed class BootSceneController : MonoBehaviour
    {
        private async void Start()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;

            string sceneAddress =
#if DEBUG
                            settingData.BootSceneData.DevelopmentSceneAddress;
#else
                            settingData.BootSceneData.ProductionSceneAddress;
#endif

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneAddress);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load scene: {sceneAddress}. Status: {handle.Status}");
                return;
            }

            Debug.Log($"Successfully loaded scene: {sceneAddress}");
        }
    }
}
