#if DEBUG
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.DevelopmentInfo
{
    public sealed class DevelopmentInfoLabelController
    {
#if DEBUG
        private readonly Label _label;


        public DevelopmentInfoLabelController(VisualElement visualElement)
        {
            _label = visualElement.Q<Label>("DevelopmentInfoLabel");
        }

        public void SetText(string text)
        {
            if (_label == null)
            {
                Debug.LogError("Label is null. Check VisualElement Name.");
                return;
            }

            _label.text = text;
        }
#endif
    }
}
