using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Addressable
{
    [InitializeOnLoad]
    public class AddressablesProfileSetupTool
    {
        private const string PROFILE_NAME = "HatzeLaboratoryProfile (Auto)";
        private const string LOCAL_BUILD_PATH = "[UnityEngine.Application.dataPath]/../{0}/[BuildTarget]";
        private const string LOCAL_LOAD_PATH = "{{UnityEngine.Application.streamingAssetsPath}}/{0}/{{BuildTarget}}";

        static AddressablesProfileSetupTool()
        {
            EditorApplication.delayCall += SetupAddressablesProfile;
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Addressables/Setup Profile", false, 0)]
        private static void SetupProfileFromMenu()
        {
            SetupProfile(isManual: true);
        }

        private static void SetupAddressablesProfile()
        {
            EditorApplication.delayCall -= SetupAddressablesProfile;
            if (!GameBasicSystemSettingData.Instance)
            {
                return;
            }

            if (GameBasicSystemSettingData.Instance.IsAddressableProfileSetupDone)
            {
                return;
            }

            SetupProfile(isManual: false);

            SerializedObject serializedObject = new(GameBasicSystemSettingData.Instance);
            serializedObject.FindProperty("_isAddressableProfileSetupDone").boolValue = true;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(GameBasicSystemSettingData.Instance);
            AssetDatabase.SaveAssets();
        }

        private static void SetupProfile(bool isManual)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                if (isManual)
                {
                    EditorUtility.DisplayDialog("Profile Setup",
                        "Addressables is not initialized.\n" +
                        "Please initialize it from Window > Asset Management > Addressables > Groups.",
                        "OK");
                }

                return;
            }

            bool isProfileExists = settings.profileSettings.GetAllProfileNames().Contains(PROFILE_NAME);
            if (isProfileExists)
            {
                if (isManual)
                {
                    EditorUtility.DisplayDialog("Profile Setup", "Addressables profile already exists.", "OK");
                }

                return;
            }

            if (!GameBasicSystemSettingData.Instance)
            {
                if (isManual)
                {
                    EditorUtility.DisplayDialog("Profile Setup", "GameBasicSystemSettingData not found.", "OK");
                }

                return;
            }

            string rootDirecotory = GameBasicSystemSettingData.Instance.AddressableAssetRootDirectory;
            if (string.IsNullOrEmpty(rootDirecotory))
            {
                rootDirecotory = "BuiltInAA";
            }

            string newProfileId = settings.profileSettings.AddProfile(PROFILE_NAME, settings.activeProfileId);
            string localBuildPath = string.Format(LOCAL_BUILD_PATH, rootDirecotory);
            string localLoadPath = string.Format(LOCAL_LOAD_PATH, rootDirecotory);
            settings.profileSettings.SetValue(newProfileId, AddressableAssetSettings.kLocalBuildPath, localBuildPath);
            settings.profileSettings.SetValue(newProfileId, AddressableAssetSettings.kLocalLoadPath, localLoadPath);
            settings.activeProfileId = newProfileId;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            if (isManual)
            {
                EditorUtility.DisplayDialog("Profile Setup", $"Profile '{PROFILE_NAME}' has been created.", "OK");
            }
            else
            {
                Debug.Log($"[GameBasicSystem] Addressables profile '{PROFILE_NAME}' has been created.");
            }
        }
    }
}
