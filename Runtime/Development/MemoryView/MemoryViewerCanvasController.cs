#if DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.MemoryView
{
    public class MemoryViewerCanvasController : MonoBehaviour
    {
        private int TOTAL_MEMORY_GB_THRESHOLD = 3;

        private enum ProfilingType
        {
            Unity,
            Script,
            Graphic
        }

        [SerializeField]
        private VisualTreeAsset _barTemplate;
        private Label _totalMemoryTextLabel;
        private readonly List<MemoryViewBarController> _barControllerList = new();
        private readonly Color[] _groupColorList =
        {
            new(1f, 0.5f, 0.5f),    // Unityのメモリ使用量を表示するバーの色
            new(1f, 1f, 0.5f),      // C#のメモリ使用量を表示するバーの色
            new(0.5f, 1f, 0.5f),    // グラフィックのメモリ使用量を表示するバーの色
        };

        [DllImport("TotalUsedMemory")]
        public extern static long GetWorkingSet();

        [DllImport("TotalUsedMemory")]
        public extern static long GetCommitSize();

        private void Start()
        {
            VisualElement rootElement = GetRootElement();
            if (rootElement == null)
            {
                Debug.LogError("Root element is null");
                return;
            }

            _totalMemoryTextLabel = rootElement.Q<Label>("TotalMemoryText");
            VisualElement rootCanvas = rootElement.Q<VisualElement>("Canvas");
            CreateAndAttachBar(rootCanvas, _barTemplate);
        }

        private void OnGUI()
        {
            UpdateTotalUsedMemoryText();
            UpdateAllBarLength();
        }

        private void UpdateTotalUsedMemoryText()
        {
            if (_totalMemoryTextLabel == null)
            {
                Debug.LogError("TotalMemoryTextLabel is null");
                return;
            }

            float totalUsedMemorySizeGB = GetWorkingSet() / 1024f / 1024f / 1024f;
            _totalMemoryTextLabel.text = $"Total: {totalUsedMemorySizeGB:F2}GB / {TOTAL_MEMORY_GB_THRESHOLD}GB";
        }

        private void UpdateAllBarLength()
        {
            for (int i = 0; i < _barControllerList.Count; i++)
            {
                ProfilingType profilingType = (ProfilingType)i;
                long usedMemoryByte = profilingType switch
                {
                    ProfilingType.Unity => Profiler.GetTotalReservedMemoryLong(),
                    ProfilingType.Script => Profiler.GetMonoHeapSizeLong(),
                    ProfilingType.Graphic => Profiler.GetAllocatedMemoryForGraphicsDriver(),
                    _ => throw new NotImplementedException(),
                };

                MemoryViewBarController barController = _barControllerList[i];
                barController?.SetByte(usedMemoryByte);
            }
        }

        private VisualElement GetRootElement()
        {
            bool isUIDocumentFound = TryGetComponent(out UIDocument uiDocument);
            if (!isUIDocumentFound)
            {
                Debug.LogError("UIDocument component is null");
                return null;
            }

            VisualElement rootElement = uiDocument.rootVisualElement;
            if (rootElement == null)
            {
                Debug.LogError("Root element is null");
                return null;
            }

            return rootElement;
        }

        private void CreateAndAttachBar(VisualElement rootCanvas, VisualTreeAsset barTemplate)
        {
            if (rootCanvas == null)
            {
                Debug.LogError("CanvasElement is null");
                return;
            }

            if (!barTemplate)
            {
                Debug.LogError("BarTemplate is null");
                return;
            }

            int profileTypeCount = Enum.GetNames(typeof(ProfilingType)).Length;
            for (int i = 0; i < profileTypeCount; i++)
            {
                TemplateContainer element = barTemplate.Instantiate();
                if (element == null)
                {
                    Debug.LogError("BarElement is null");
                    continue;
                }

                string categoryName = ((ProfilingType)i).ToString();
                Color barColor = _groupColorList[i];
                MemoryViewBarController barController = new(element, categoryName, barColor);
                element.userData = barController;

                rootCanvas.Add(element);
                _barControllerList.Add(barController);
            }
        }
    }
}
#endif