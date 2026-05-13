using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer
{
    public sealed class AddressableAssetRootPathDrawer : ISettingProviderDrawer
    {
        private const string LABEL_TEXT = "Addressable Asset Root Directory";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;
            string newPath = EditorGUILayout.TextField(LABEL_TEXT, settingData.AddressableAssetRootDirectory);
            EditorGUIUtility.labelWidth = labelWidth;

            settingData.SetAddressableAssetRootDirectory(newPath);
        }
    }
}
