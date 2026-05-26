using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Input
{
    /// <summary>
    /// Input System のアクションマップとコントローラーを一元管理するシングルトンクラス
    /// </summary>
    [RequireComponent(typeof(PlayerInput))] 
    public class InputManager : SingletonBehaviour<InputManager>
    {
        /// <summary>
        /// 入力デバイスの種類
        /// </summary>
        public enum InputType
        {
            Keyboard,
            Mouse,
            Gamepad,
        }

        private PlayerInput _playerInput;
        private readonly Dictionary<string, InputControllerBase> _controllerList = new();

        /// <summary>
        /// アクションマップ名（Enum）でコントローラーを登録
        /// </summary>
        /// <param name="actionMapName">アクションマップ名を表すEnum値</param>
        /// <param name="createController">PlayerInput を受け取りコントローラーを生成する関数</param>
        public void RegisterController(Enum actionMapName, Func<PlayerInput, InputControllerBase> createController) => RegisterController(actionMapName.ToString(), createController);

        /// <summary>
        /// アクションマップ名（文字列）でコントローラーを登録
        /// </summary>
        /// <param name="actionMapName">アクションマップ名</param>
        /// <param name="createController">PlayerInput を受け取りコントローラーを生成する関数</param>
        public void RegisterController(string actionMapName, Func<PlayerInput, InputControllerBase> createController)
        {
            if (_controllerList.ContainsKey(actionMapName))
            {
                Debug.LogWarning($"Controller for ActionMap '{actionMapName}' is already registered. Overwriting with new controller.");
                return;
            }

            _controllerList[actionMapName] = createController(_playerInput);
        }

        /// <summary>
        /// アクションマップ名（Enum）・アクション名でコールバックを登録
        /// </summary>
        /// <param name="actionMapName">アクションマップ名を表すEnum値</param>
        /// <param name="actionName">アクション名</param>
        /// <param name="actionCallback">登録するコールバック</param>
        public void AddAction(Enum actionMapName, string actionName, Action<InputAction.CallbackContext> actionCallback) => AddAction(actionMapName.ToString(), actionName, actionCallback);

        /// <summary>
        /// アクションマップ名（文字列）・アクション名でコールバックを登録
        /// </summary>
        /// <param name="actionMapName">アクションマップ名</param>
        /// <param name="actionName">アクション名</param>
        /// <param name="actionCallback">登録するコールバック</param>
        public void AddAction(string actionMapName, string actionName, Action<InputAction.CallbackContext> actionCallback)
        {
            if (!_controllerList.TryGetValue(actionMapName, out InputControllerBase controller))
            {
                Debug.LogError($"No controller found for ActionMap '{actionMapName}'. Cannot add action.");
                return;
            }
    
            controller.AddActionEvent(actionName, actionCallback);
        }

        /// <summary>
        /// アクションマップ名（Enum）・アクション名でコールバックを解除
        /// </summary>
        /// <param name="actionMapName">アクションマップ名を表すEnum値</param>
        /// <param name="actionName">アクション名</param>
        /// <param name="actionCallback">解除するコールバック</param>
        public void RemoveAction(Enum actionMapName, string actionName, Action<InputAction.CallbackContext> actionCallback) => RemoveAction(actionMapName.ToString(), actionName, actionCallback);

        /// <summary>
        /// アクションマップ名（文字列）・アクション名でコールバックを解除
        /// </summary>
        /// <param name="actionMapName">アクションマップ名</param>
        /// <param name="actionName">アクション名</param>
        /// <param name="actionCallback">解除するコールバック</param>
        public void RemoveAction(string actionMapName, string actionName, Action<InputAction.CallbackContext> actionCallback)
        {
            if (!_controllerList.TryGetValue(actionMapName, out InputControllerBase controller))
            {
                Debug.LogError($"No controller found for ActionMap '{actionMapName}'. Cannot remove action.");
                return;
            }

            controller.RemoveActionEvent(actionName, actionCallback);
        }

        /// <summary>
        /// アクティブなアクションマップを切り替えます（Enum 指定）
        /// </summary>
        /// <param name="actionMapName">切り替え先のアクションマップ名を表す Enum 値。</param>
        public void SwitchActionMap(Enum actionMapName) => SwitchActionMap(actionMapName.ToString());

        /// <summary>
        /// アクティブなアクションマップを切り替えます（文字列指定）
        /// </summary>
        /// <param name="actionMapName">切り替え先のアクションマップ名。</param>
        public void SwitchActionMap(string actionMapName)
        {
            _playerInput.SwitchCurrentActionMap(actionMapName);
        }

        /// <summary>
        /// 入力の有効・無効を切り替え
        /// </summary>
        /// <param name="isEnabled">有効にする場合は <c>true</c>、無効にする場合は <c>false</c></param>
        public void SetInputEnable(bool isEnabled)
        {
            if (isEnabled)
            {
                _playerInput.actions.Enable();
            }
            else
            {
                _playerInput.actions.Disable();
            }
        }

        /// <summary>
        /// PlayerInput に使用するカメラを設定
        /// </summary>
        /// <param name="camera">設定するカメラ</param>
        public void SetPlayer(Camera camera)
        {
            if (!camera)
            {
                Debug.LogError("Camera is null. Cannot set player camera.");
                return;
            }

            _playerInput.camera = camera;
        }

        /// <summary>指定したアクションのキーバインドをインタラクティブに変更（Enum 指定）</summary>
        /// <param name="actionMapName">アクションマップ名を表すEnum値</param>
        /// <param name="actionName">リバインドするアクション名</param>
        /// <param name="inputType">対象のデバイス種類</param>
        /// <param name="offsetBindingIndex">バインディングインデックスのオフセット</param>
        /// <param name="OnCompleted">リバインド完了時のコールバック</param>
        /// <param name="OnCanceled">リバインドキャンセル時のコールバック</param>
        public void RebindKey(Enum actionMapName, string actionName, InputType inputType, int offsetBindingIndex = 0, Action OnCompleted = null, Action OnCanceled = null) => RebindKey(actionMapName.ToString(), actionName, inputType, offsetBindingIndex, OnCompleted, OnCanceled);

        /// <summary>指定したアクションのキーバインドをインタラクティブに変更（文字列指定）</summary>
        /// <param name="actionMapName">アクションマップ名</param>
        /// <param name="actionName">リバインドするアクション名</param>
        /// <param name="inputType">対象のデバイス種類</param>
        /// <param name="offsetBindingIndex">バインディングインデックスのオフセット</param>
        /// <param name="OnCompleted">リバインド完了時のコールバック</param>
        /// <param name="OnCanceled">リバインドキャンセル時のコールバック</param>
        public void RebindKey(string actionMapName, string actionName, InputType inputType, int offsetBindingIndex = 0, Action OnCompleted = null, Action OnCanceled = null)
        {
            if (!_controllerList.TryGetValue(actionMapName, out InputControllerBase controller))
            {
                Debug.LogError($"No controller found for ActionMap '{actionMapName}'. Cannot rebind key.");
                return;
            }

            controller.RebindKey(actionName, inputType, offsetBindingIndex, OnCompleted, OnCanceled);
        }

        /// <summary>
        /// 全アクションマップのキーバインドオーバーライドをリセット
        /// </summary>
        public void ResetOverrideKeys()
        {
            foreach (InputControllerBase controller in _controllerList.Values)
            {
                controller.ResetOverrideKeys();
            }
        }

        /// <summary>指定したアクションの現在のキーバインド表示文字列を返す（Enum 指定）</summary>
        /// <param name="actionMapName">アクションマップ名を表すEnum値</param>
        /// <param name="actionName">アクション名</param>
        /// <param name="inputType">対象のデバイス種類</param>
        /// <param name="offsetBindingIndex">バインディングインデックスのオフセット</param>
        /// <returns>キーバインドの表示文字列</returns>
        public string GetActionBindingDisplayString(Enum actionMapName, string actionName, InputType inputType, int offsetBindingIndex = 0) => GetActionBindingDisplayString(actionMapName.ToString(), actionName, inputType, offsetBindingIndex);

        /// <summary>指定したアクションの現在のキーバインド表示文字列を返す（文字列指定）</summary>
        /// <param name="actionMapName">アクションマップ名</param>
        /// <param name="actionName">アクション名</param>
        /// <param name="inputType">対象のデバイス種類</param>
        /// <param name="offsetBindingIndex">バインディングインデックスのオフセット</param>
        /// <returns>キーバインドの表示文字列</returns>
        public string GetActionBindingDisplayString(string actionMapName, string actionName, InputType inputType, int offsetBindingIndex = 0)
        {
            if (!_controllerList.TryGetValue(actionMapName, out InputControllerBase controller))
            {
                Debug.LogError($"No controller found for ActionMap '{actionMapName}'. Cannot get binding display string.");
                return string.Empty;
            }

            return controller.GetBindingDisplayString(actionName, inputType, offsetBindingIndex);
        }

        /// <summary>
        /// JSON 文字列からキーバインドオーバーライドを適用
        /// </summary>
        /// <param name="json"><see cref="GetBindingOverrideJson"/> で取得した JSON文字列</param>
        public void ApplyBindingOverrideJson(string json)
        {
            InputActionAsset actionAsset = _playerInput.actions;
            if (actionAsset == null)
            {
                Debug.LogError("InputActionAsset is null. Cannot apply binding override from JSON.");
                return;
            }

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("JSON string is null or empty. No binding overrides will be applied.");
                return;
            }

            actionAsset.LoadBindingOverridesFromJson(json);
        }

        /// <summary>
        /// 現在のキーバインドオーバーライドを JSON 文字列として返す
        /// </summary>
        /// <returns>キーバインドオーバーライドの JSON文字列</returns>
        public string GetBindingOverrideJson()
        {
            InputActionAsset actionAsset = _playerInput.actions;
            if (actionAsset == null)
            {
                Debug.LogError("InputActionAsset is null. Cannot get binding override JSON.");
                return string.Empty;
            }

            return actionAsset.SaveBindingOverridesAsJson();
        }

        private new void Awake()
        {
            base.Awake();
            if (TryGetComponent(out PlayerInput playerInput))
            {
                _playerInput = playerInput;
            }
            else
            {
                Debug.LogError("PlayerInput component is missing on InputManager GameObject.");
            }
        }
    }
}
