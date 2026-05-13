using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.Backend.System.UI.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Backend.System.UI
{
    public sealed class ModalManager : SingletonBehaviour<ModalManager>
    {
        [SerializeField]
        private Transform _modalRoot;

        private readonly Stack<IModal> _viewModalStack = new();
        private readonly List<IModal> _loadedModalObjectList = new();
        private readonly Dictionary<string, LoadModalData> _loadedModalDataList = new();

        public IModal ShowModal(string modalName)
        {
            if (_loadedModalDataList.TryGetValue(modalName, out LoadModalData loadModalData))
            {
                IModal modalTemplate = loadModalData.ModalTemplate;
                if (modalTemplate == null)
                {
                    Debug.LogError($"Modal template for {modalName} is not loaded.");
                    return null;
                }

                if (_viewModalStack.Any(modal => modal.GetType() == modalTemplate.GetType()))
                {
                    Debug.LogError($"Modal for {modalTemplate.GetType()} is already shown.");
                    return null;
                }

                if (_loadedModalObjectList.Exists(modal => modal.GetType() == modalTemplate.GetType()))
                {
                    IModal findModal = _loadedModalObjectList.FirstOrDefault(modal => modal.GetType() == modalTemplate.GetType());
                    if (findModal == default)
                    {
                        Debug.LogError($"Modal for {modalTemplate.GetType()} is not found in pool.");
                        return null;
                    }

                    findModal.Show();
                    _viewModalStack.Push(findModal);
                    return findModal;
                }

                GameObject modalInstance = Instantiate(modalTemplate.GameObject);
                if (modalInstance.TryGetComponent(out IModal modalInterface))
                {
                    modalInterface.GameObject.transform.SetParent(_modalRoot, false);
                    RectTransform rectTransform = modalInterface.GameObject.GetComponent<RectTransform>();
                    rectTransform.anchorMin = 0.5f * Vector2.one;
                    rectTransform.anchorMax = 0.5f * Vector2.one;
                    rectTransform.anchoredPosition = Vector2.zero;

                    modalInterface.Show();
                    _viewModalStack.Push(modalInterface);
                    _loadedModalObjectList.Add(modalInterface);
                    return modalInterface;
                }
                else
                {
                    Debug.LogError($"Modal prefab {modalInstance.name} does not implement IModal interface.");
                }
            }
            else
            {
                Debug.LogError($"Modal type {modalName} is not registered.");
            }

            return null;
        }

        public void HideTopModal()
        {
            if (GetModalCount() < 1)
            {
                Debug.LogWarning("Showing no modals.");
                return;
            }

            IModal modal = _viewModalStack.Pop();
            modal.Hide();
        }

        public bool IsModalShown<T>() where T : IModal
        {
            return _viewModalStack.Any(modal => modal.GetType() == typeof(T));
        }

        public bool IsModalShownAtTop<T>() where T : IModal
        {
            if (GetModalCount() < 1)
            {
                return false;
            }

            IModal topModal = _viewModalStack.Peek();
            return topModal.GetType() == typeof(T);
        }

        public int GetModalCount()
        {
            return _viewModalStack.Count;
        }

        private void Start()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            Initialize(token).Forget();
        }

        private void OnDestroy()
        {
            foreach (LoadModalData loadedModalData in _loadedModalDataList.Values)
            {
                if (!loadedModalData.Handle.IsValid())
                {
                    continue;
                }

                Addressables.Release(loadedModalData.Handle);
            }
        }

        private async UniTask Initialize(CancellationToken cancellationToken)
        {
            foreach (string modalName in GameBasicSystemSettingData.Instance.ModalNameList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await CreateModal(modalName, cancellationToken);
            }
        }

        private async UniTask CreateModal(string modalName, CancellationToken cancellationToken)
        {
            string address = GameBasicSystemSettingData.Instance.GetModalAddress(modalName);
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogError($"ModalName {modalName} is invalid.");
                return;
            }

            if (_loadedModalDataList.ContainsKey(modalName))
            {
                Debug.LogWarning($"Modal {modalName} is already loaded.");
                return;
            }

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
            await handle;
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject modal = handle.Result;
                if (!modal)
                {
                    Debug.LogError($"Failed to load {address} prefab.");
                    return;
                }

                if (modal.TryGetComponent(out IModal modalInterface))
                {
                    _loadedModalDataList.Add(modalName, new()
                    {
                        Handle = handle,
                        ModalTemplate = modalInterface,
                    });
                }
                else
                {
                    Debug.LogError($"Modal prefab {modal.name} does not implement IModal interface.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load {address} prefab.");
            }
        }

        private struct LoadModalData
        {
            public AsyncOperationHandle<GameObject> Handle;
            public IModal ModalTemplate;
        }
    }
}
