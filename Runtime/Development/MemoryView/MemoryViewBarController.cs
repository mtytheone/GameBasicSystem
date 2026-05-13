#if DEBUG
using UnityEngine;
using UnityEngine.UIElements;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.MemoryView
{
    public class MemoryViewBarController
    {
        private const float BAR_WIDTH_PERCENT_PER_GB = 100;

        private string _categoryName;
        private readonly VisualElement _barElement;
        private readonly Label _textLabel;

        public MemoryViewBarController(VisualElement visualElement, string CategoryName, Color groupColor)
        {
            _barElement = visualElement.Q<VisualElement>("MemoryBar");
            _textLabel = visualElement.Q<Label>("MemoryText");
            _categoryName = CategoryName;
            if (_barElement != null)
            {
                _barElement.style.backgroundColor = groupColor;
            }

            SetByte(0);
        }

        public void SetByte(long byteAmount)
        {
            if (_barElement == null)
            {
                Debug.LogError("BarElement is null.");
                return;
            }

            if (_textLabel == null)
            {
                Debug.LogError("TextLabel is null.");
                return;
            }

            float gigaByteAmount = byteAmount / 1024f / 1024f / 1024f;
            Length length = new(gigaByteAmount * BAR_WIDTH_PERCENT_PER_GB, LengthUnit.Percent);
            _barElement.style.width = new StyleLength(length);
            _textLabel.text = $"{_categoryName}: {gigaByteAmount:F2}GB";
        }
    }
}
#endif