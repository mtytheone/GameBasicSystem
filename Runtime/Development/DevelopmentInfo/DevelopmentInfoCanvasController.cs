#if DEBUG
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.DevelopmentInfo
{
    public sealed class DevelopmentInfoCanvasController
    {
#if DEBUG
        private readonly VisualElement _listRootElement;
        private readonly VisualTreeAsset _labelTemplate;


        public DevelopmentInfoCanvasController(VisualElement rootElement, VisualTreeAsset labelTemplate)
        {
            _listRootElement = rootElement.Q<VisualElement>("DevelopmentLabelList");
            _labelTemplate = labelTemplate;
        }

        public void SetData(List<string> textList)
        {
            if (textList == null)
            {
                Debug.LogError("TextList is null");
                return;
            }

            if (_listRootElement == null)
            {
                Debug.LogError("ListRootElement is null");
                return;
            }

            _listRootElement.Clear();
            foreach (string text in textList)
            {
                TemplateContainer element = _labelTemplate.Instantiate();
                DevelopmentInfoLabelController labelController = new(element);
                labelController.SetText(text);
                element.userData = labelController;

                _listRootElement.Add(element);
            }
        }
#endif
    }
}