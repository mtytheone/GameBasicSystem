using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer
{
    public sealed class BootSceneAddressDataDrawer : ISettingProviderDrawer
    {
        private const string DEVELOPMENT_LABEL_TEXT = "DevelopmentSceneAddress";
        private const string PRODUCTION_LABEL_TEXT = "ProductionSceneAddress";

        string ISettingProviderDrawer.SectionTitle => "Boot Scene Address Data";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            GameBasicSystemSettingData.BootSceneAddressData bootSceneData = settingData.BootSceneData;
            bootSceneData.DevelopmentSceneAddress = EditorGUILayout.TextField(DEVELOPMENT_LABEL_TEXT, bootSceneData.DevelopmentSceneAddress);
            bootSceneData.ProductionSceneAddress = EditorGUILayout.TextField(PRODUCTION_LABEL_TEXT, bootSceneData.ProductionSceneAddress);
            
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}
