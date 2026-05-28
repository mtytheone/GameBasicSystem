using HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Drawer
{
    public sealed class AddressableAssetRootPathDrawer : ISettingProviderDrawer
    {
        private const string ROOT_ADDRESSABLE_ASSET_LABEL_TEXT = "Addressable Asset Root Directory";
        private const string BUILD_ADDRESSABLE_ASSET_LABEL_TEXT = "Build Addressable Asset Save Directory";

        string ISettingProviderDrawer.SectionTitle => "Addressables";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;
            DrawAddressableAssetRootPathField(settingData);
            DrawBuildAddressableAssetSaveDirectoryField(settingData);
        }

        private void DrawAddressableAssetRootPathField(GameBasicSystemSettingData settingData)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            string newPath = "";
            using (new EditorGUILayout.HorizontalScope())
            {
                newPath = EditorGUILayout.TextField(ROOT_ADDRESSABLE_ASSET_LABEL_TEXT, settingData.AddressableAssetRootDirectory);
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

        private void DrawBuildAddressableAssetSaveDirectoryField(GameBasicSystemSettingData settingData)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            string newPath = "";
            using (new EditorGUILayout.HorizontalScope())
            {
                newPath = EditorGUILayout.TextField(BUILD_ADDRESSABLE_ASSET_LABEL_TEXT, settingData.BuildAddressableAssetSaveDirectory);
                if (GUILayout.Button("Open", GUILayout.Width(50)))
                {
                    string selectedDirectoryPath = EditorUtility.OpenFolderPanel("Select Build Addressable Asset Save Directory", "Assets", "");
                    if (!string.IsNullOrEmpty(selectedDirectoryPath))
                    {
                        newPath = selectedDirectoryPath;
                    }
                }
            }

            EditorGUIUtility.labelWidth = labelWidth;
            settingData.SetBuildAddressableAssetSaveDirectory(newPath);
        }
    }
}
