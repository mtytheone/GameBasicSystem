using HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;

namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Drawer
{
    public class MemoryViewerDrawer : ISettingProviderDrawer
    {
        string ISettingProviderDrawer.SectionTitle => "Development";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;

            int newThreshold = EditorGUILayout.IntField(
                "Memory Viewer Threshold (GB)",
                settingData.MemoryViewerThresholdGb
            );

            if (newThreshold < 1)
            {
                newThreshold = 1;
            }

            settingData.SetMemoryViewerThresholdGb(newThreshold);
        }
    }
}