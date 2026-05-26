using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer
{
    public sealed class AddressableAssetRootPathDrawer : ISettingProviderDrawer
    {
        private const string LABEL_TEXT = "Addressable Asset Root Directory";

        string ISettingProviderDrawer.SectionTitle => "Addressables";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            string newPath = "";
            using (new EditorGUILayout.HorizontalScope())
            {
                newPath = EditorGUILayout.TextField(LABEL_TEXT, settingData.AddressableAssetRootDirectory);
                if (GUILayout.Button("Open", GUILayout.Width(50)))
                {
                    string selectedDirectoryPath = EditorUtility.OpenFolderPanel("Select Addressable Asset Root", "Assets", "");
                    if (!string.IsNullOrEmpty(selectedDirectoryPath))
                    {
                        string projectPath = Application.dataPath.Replace("/Assets", "/");
                        if (selectedDirectoryPath.StartsWith(projectPath))
                        {
                            newPath = selectedDirectoryPath[projectPath.Length..];
                        }
                        else
                        {
                            Debug.LogWarning("Selected folder is outside the project.");
                        }
                    }
                }
            }

            EditorGUIUtility.labelWidth = labelWidth;
            settingData.SetAddressableAssetRootDirectory(newPath);
        }
    }
}
