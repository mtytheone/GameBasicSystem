#if DEBUG
using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.DevelopmentInfo
{
    public sealed class DevelopmentInfoViewer : SingletonBehaviour<DevelopmentInfoViewer>
    {
        [SerializeField]
        private VisualTreeAsset _labelTemplate;
        private DevelopmentInfoCanvasController _canvasController;
        private readonly List<string> _outputTextList = new();


        public void AddText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _outputTextList.Add(text);
        }


        private void OnEnable()
        {
            if (TryGetComponent(out UIDocument uiDocument))
            {
                _canvasController = new DevelopmentInfoCanvasController(uiDocument.rootVisualElement, _labelTemplate);
            }
            else
            {
                Debug.LogError("UIDocument component is null");
            }
        }

        private void LateUpdate()
        {
            if (_canvasController == null)
            {
                Debug.LogError("CanvasController is null");
                return;
            }

            _canvasController.SetData(_outputTextList);
            _outputTextList.Clear();
        }
    }
}
#endif