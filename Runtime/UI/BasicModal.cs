using HatzeLaboratory.GameBasicSystem.Runtime.UI.Interface;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HatzeLaboratory.GameBasicSystem.Runtime.UI
{
    public class BasicModal : MonoBehaviour, IModal
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private RectTransform _buttonRoot;

        [SerializeField]
        private Button _leftButton;

        [SerializeField]
        private Button _rightButton;

        GameObject IModal.GameObject => gameObject;

        void IModal.Show()
        {
            SetCanvasDisplayEnabled(true);
        }

        void IModal.Hide()
        {
            SetCanvasDisplayEnabled(false);
        }

        Type IModal.GetModalType()
        {
            return GetType();
        }

        public void SetView(bool isTitleEnabled, bool isImageEnabled, bool isTextEnabled, bool isLeftButtonEnabled, bool isRightButtonEnabled)
        {
            SetTitleEnabled(isTitleEnabled);
            SetImageEnabled(isImageEnabled);
            SetTextEnabled(isTextEnabled);
            SetButtonEnabled(isLeftButtonEnabled || isRightButtonEnabled);
            SetLeftButtonEnabled(isLeftButtonEnabled);
            SetRightButtonEnabled(isRightButtonEnabled);
        }

        public void SetTitleText(string titleText)
        {
            if (!_titleText)
            {
                Debug.LogError("Title Text is not assigned.");
                return;
            }

            _titleText.text = titleText;
        }

        public void SetImage(Sprite image)
        {
            if (!_image)
            {
                Debug.LogError("Image is not assigned.");
                return;
            }

            _image.sprite = image;
        }

        public void SetText(string text)
        {
            if (!_text)
            {
                Debug.LogError("Text is not assigned.");
                return;
            }

            _text.text = text;
        }

        public void SetLeftButtonText(string text)
        {
            if (!_leftButton)
            {
                Debug.LogError("Left Button is not assigned.");
                return;
            }

            _leftButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        public void SetRightButtonText(string text)
        {
            if (!_rightButton)
            {
                Debug.LogError("Right Button is not assigned.");
                return;
            }

            _rightButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        public void SetLeftButtonCallback(Action callback)
        {
            if (!_leftButton)
            {
                Debug.LogError("Left Button is not assigned.");
                return;
            }

            _leftButton.onClick.RemoveAllListeners();
            _leftButton.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }

        public void SetRightButtonCallback(Action callback)
        {
            if (!_rightButton)
            {
                Debug.LogError("Right Button is not assigned.");
                return;
            }

            _rightButton.onClick.RemoveAllListeners();
            _rightButton.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }

        private void Start()
        {
            Assert.IsNotNull(_canvasGroup, "Canvas Group is not assigned.");
            Assert.IsNotNull(_titleText, "Title Text is not assigned.");
            Assert.IsNotNull(_image, "Image is not assigned.");
            Assert.IsNotNull(_text, "Text is not assigned.");
            Assert.IsNotNull(_buttonRoot, "Button Root is not assigned.");
            Assert.IsNotNull(_leftButton, "Left Button is not assigned.");
            Assert.IsNotNull(_rightButton, "Right Button is not assigned.");
        }

        private void SetCanvasDisplayEnabled(bool isEnabled)
        {
            if (!_canvasGroup)
            {
                Debug.LogError("Canvas Group is not assigned.");
                return;
            }

            _canvasGroup.alpha = isEnabled ? 1 : 0;
        }

        private void SetTitleEnabled(bool isEnabled)
        {
            if (!_titleText)
            {
                Debug.LogError("Title Text is not assigned.");
                return;
            }

            _titleText.gameObject.SetActive(isEnabled);
        }

        private void SetImageEnabled(bool isEnabled)
        {
            if (!_image)
            {
                Debug.LogError("Image is not assigned.");
                return;
            }

            _image.gameObject.SetActive(isEnabled);
        }

        private void SetTextEnabled(bool isEnabled)
        {
            if (!_text)
            {
                Debug.LogError("Text is not assigned.");
                return;
            }

            _text.gameObject.SetActive(isEnabled);
        }

        private void SetButtonEnabled(bool isEnabled)
        {
            if (!_buttonRoot)
            {
                Debug.LogError("Button Root is not assigned.");
                return;
            }

            _buttonRoot.gameObject.SetActive(isEnabled);
        }

        private void SetLeftButtonEnabled(bool isEnabled)
        {
            if (!_leftButton)
            {
                Debug.LogError("Left Button is not assigned.");
                return;
            }

            _leftButton.gameObject.SetActive(isEnabled);
        }

        private void SetRightButtonEnabled(bool isEnabled)
        {
            if (!_rightButton)
            {
                Debug.LogError("Right Button is not assigned.");
                return;
            }

            _rightButton.gameObject.SetActive(isEnabled);
        }
    }
}
