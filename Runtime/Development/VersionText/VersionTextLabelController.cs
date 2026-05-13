#if DEBUG
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.VersionText
{
    public sealed class VersionTextLabelController : MonoBehaviour
    {
        private Coroutine _coroutine;


        private void OnEnable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(ShowVersionText());
        }

        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator ShowVersionText()
        {
            bool isUIDocumentFound = TryGetComponent(out UIDocument uiDocument);
            if (!isUIDocumentFound)
            {
                Debug.LogError("UIDocument component is null");
                _coroutine = null;
                yield break;
            }

            Label label = uiDocument.rootVisualElement.Q<Label>("Label");
            if (label == null)
            {
                Debug.LogError("Label component is null");
                _coroutine = null;
                yield break;
            }

            DateTime buildTimestamp;

#if UNITY_EDITOR
            buildTimestamp = DateTime.Now;
#else
            AsyncOperationHandle<BuildTimestampData> handle = Addressables.LoadAssetAsync<BuildTimestampData>("Development/BuildTimestampData");
            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load BuildTimestampData");
                handle.Release();
                _coroutine = null;
                yield break;
            }

            buildTimestamp = handle.Result.GetBuildTimeStamp();
            handle.Release();
#endif

            label.text = string.Format
            (
                "{0}_{1}_{2}_{3}_{4}",
                Application.productName,
                "DEV",
                Application.version,
                buildTimestamp.ToString("yyMMddHHmmss"),
                PlatformDefine.GetPlatformName(Application.platform)
            );

            _coroutine = null;
        }
    }
}
#endif