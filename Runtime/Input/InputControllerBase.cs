using HatzeLaboratory.GameBasicSystem.Runtime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Input
{
    /// <summary>
    /// アクションマップ単位で入力を管理するコントローラー基底クラス。
    /// このクラスを継承して <see cref="GetActionMapName"/> をオーバーライドし、<see cref="InputManager"/> に登録してください。
    /// </summary>
    public class InputControllerBase
    {
        private const string REBIND_CANCEL_KEY_SCHEME = "<Keyboard>/escape";
        private const string REBIND_MODAL_NAME = "BaseModal";

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        private readonly Dictionary<string, InputAction> _inputActionMap = new();
        private readonly Dictionary<string, Action<InputAction.CallbackContext>> _inputActionCallbackMap = new();

        /// <summary>
        /// 指定された <see cref="PlayerInput"/> でコントローラーを初期化
        /// </summary>
        /// <param name="playerInput">使用する PlayerInput コンポーネント</param>
        /// <exception cref="ArgumentNullException"><paramref name="playerInput"/> が null の場合</exception>
        public InputControllerBase(PlayerInput playerInput)
        {
            if (!playerInput)
            {
                throw new ArgumentNullException(nameof(playerInput), "PlayerInput cannot be null.");
            }

            CreateInputActionMap(playerInput);
            CreateInputActionCallbackMap(playerInput);
            playerInput.onActionTriggered += OnActionTriggered;
        }

        /// <summary>
        /// 指定したアクション名にコールバックを登録
        /// </summary>
        /// <param name="actionName">登録するアクション名</param>
        /// <param name="action">登録するコールバック</param>
        public void AddActionEvent(string actionName, Action<InputAction.CallbackContext> action)
        {
            if (!_inputActionCallbackMap.ContainsKey(actionName))
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
                return;
            }

            _inputActionCallbackMap[actionName] += action;
        }

        /// <summary>指定したアクション名からコールバックを解除</summary>
        /// <param name="actionName">解除するアクション名</param>
        /// <param name="action">解除するコールバック</param>
        public void RemoveActionEvent(string actionName, Action<InputAction.CallbackContext> action)
        {
            if (!_inputActionCallbackMap.ContainsKey(actionName))
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
                return;
            }

            _inputActionCallbackMap[actionName] -= action;
        }

        /// <summary>
        /// 指定したアクションのキーバインドをインタラクティブに変更。
        /// 変更中はモーダルが表示され、Esc キーでキャンセルできます。
        /// </summary>
        /// <param name="actionName">リバインドするアクション名</param>
        /// <param name="inputType">対象のデバイス種類</param>
        /// <param name="offsetBindingIndex">バインディングインデックスのオフセット</param>
        /// <param name="OnCompleted">リバインド完了時のコールバック</param>
        /// <param name="OnCanceled">リバインドキャンセル時のコールバック</param>
        public void RebindKey(string actionName, InputManager.InputType inputType, int offsetBindingIndex = 0, Action OnCompleted = null, Action OnCanceled = null)
        {
            if (!_inputActionCallbackMap.ContainsKey(actionName))
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
                return;
            }

            InputAction action = _inputActionMap[actionName];
            if (action == null)
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
                return;
            }

            InputBinding inputBinding = InputBinding.MaskByGroup(inputType.ToString());
            int bindingIndex = action.GetBindingIndex(inputBinding);
            bindingIndex += offsetBindingIndex;

            SetActionEnable(action, false);
            ShowWaitingInputModal();
            _rebindingOperation = action
                .PerformInteractiveRebinding(bindingIndex)
                .OnComplete(operation =>
                {
                    SetActionEnable(action, true);
                    ModalManager.Instance.HideTopModal();
                    OnCompleted?.Invoke();
                })
                .OnCancel(operation =>
                {
                    SetActionEnable(action, true);
                    ModalManager.Instance.HideTopModal();
                    OnCanceled?.Invoke();
                })
                .WithCancelingThrough(REBIND_CANCEL_KEY_SCHEME)
                .Start();
        }

        /// <summary>
        /// このコントローラーのすべてのキーバインドオーバーライドをリセット
        /// </summary>
        public void ResetOverrideKeys()
        {
            foreach (InputAction action in _inputActionMap.Values)
            {
                action.RemoveAllBindingOverrides();
            }
        }

        /// <summary>指定したアクションの現在のキーバインド表示文字列を返す</summary>
        /// <param name="actionName">取得するアクション名</param>
        /// <param name="inputType">対象のデバイス種類</param>
        /// <param name="offsetBindingIndex">バインディングインデックスのオフセット</param>
        /// <returns>キーバインドの表示文字列</returns>
        public string GetBindingDisplayString(string actionName, InputManager.InputType inputType, int offsetBindingIndex = 0)
        {
            if (!_inputActionMap.ContainsKey(actionName))
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
                return string.Empty;
            }

            InputAction action = _inputActionMap[actionName];
            if (action == null)
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
                return string.Empty;
            }

            InputBinding inputBinding = InputBinding.MaskByGroup(inputType.ToString());
            int bindingIndex = action.GetBindingIndex(inputBinding);
            if (bindingIndex < action.bindings.Count - 1)
            {
                bindingIndex += offsetBindingIndex;
            }

            return bindingIndex == -1 
                ? action.GetBindingDisplayString() 
                : action.GetBindingDisplayString(bindingIndex);
        }

        protected virtual string GetActionMapName()
        {
            return string.Empty;
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (context.action.actionMap.name != GetActionMapName())
            {
                return;
            }

            string actionName = context.action.name;
            if (_inputActionCallbackMap.TryGetValue(actionName, out Action<InputAction.CallbackContext> action))
            {
                action?.Invoke(context);
            }
            else
            {
                Debug.LogError($"Action '{actionName}' not found in {GetActionMapName()}InputController.");
            }
        }

        private void CreateInputActionMap(PlayerInput playerInput)
        {
            InputActionMap inputActionMap = GetInputActionMap(playerInput);
            if (inputActionMap == null)
            {
                return;
            }

            foreach (InputAction action in inputActionMap.actions)
            {
                if (action == null)
                {
                    continue;
                }

                _inputActionMap.Add(action.name, action);
            }
        }

        private void CreateInputActionCallbackMap(PlayerInput playerInput)
        {
            InputActionMap inputActionMap = GetInputActionMap(playerInput);
            if (inputActionMap == null)
            {
                return;
            }

            foreach (InputAction action in inputActionMap.actions)
            {
                if (string.IsNullOrEmpty(action?.name))
                {
                    continue;
                }

                _inputActionCallbackMap.Add(action.name, null);
            }
        }

        private InputActionMap GetInputActionMap(PlayerInput playerInput)
        {
            ReadOnlyArray<InputActionMap> actionMapList = playerInput.actions.actionMaps;
            int actionMapIndex = actionMapList.Select(x => x.name).ToList().IndexOf(GetActionMapName());
            if (actionMapIndex == -1)
            {
                Debug.LogError($"Action map '{GetActionMapName()}' not found in PlayerInput.");
                return null;
            }

            return actionMapList[actionMapIndex];
        }

        private void SetActionEnable(InputAction inputAction, bool isEnable)
        {
            if (inputAction == null)
            {
                Debug.LogError("InputAction is null.");
                return;
            }

            if (isEnable)
            {
                _rebindingOperation?.Dispose();
                _rebindingOperation = null;

                inputAction.Enable();
            }
            else
            {
                _rebindingOperation?.Cancel();
                inputAction.Disable();
            }
        }

        private void ShowWaitingInputModal()
        {
            BasicModal baseModal = ModalManager.Instance.ShowModal(REBIND_MODAL_NAME) as BasicModal;
            if (baseModal == null)
            {
                Debug.LogError("BaseModal is not loaded.");
                return;
            }

            const bool isTitleEnabled = false;
            const bool isImageEnabled = false;
            const bool isTextEnabled = true;
            const bool isLeftButtonEnabled = false;
            const bool isRightButtonEnabled = false;
            baseModal.SetView(isTitleEnabled, isImageEnabled, isTextEnabled, isLeftButtonEnabled, isRightButtonEnabled);
            baseModal.SetText("Press any key to rebind.\nEsc Key is canceled.");
        }
    }
}
