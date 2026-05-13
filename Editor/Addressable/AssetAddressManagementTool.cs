using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Addressable
{
    public sealed class AssetAddressManagementTool : AssetPostprocessor
    {
        private static string AddressableAssetRootDirectory => GameBasicSystemSettingData.Instance.AddressableAssetRootDirectory;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssets)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            ImportAddressableAssets(settings, importedAssets);
            DeleteAddressableAssets(settings, deletedAssets);
            MoveAddressableAssets(settings, movedAssets, movedFromAssets);
        }

        private static void ImportAddressableAssets(AddressableAssetSettings settings, string[] importedAssetsPathList)
        {
            bool isAnyAddressRegistered = false;
            foreach (string assetPath in importedAssetsPathList)
            {
                if (!IsValidPath(assetPath))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(AddressableAssetRootDirectory))
                {
                    Debug.LogError("AddressableAssetRootDirectory data is empty.");
                    return;
                }

                string relativeAssetPath = assetPath.Replace(AddressableAssetRootDirectory, "");
                if (relativeAssetPath.StartsWith("/"))
                {
                    relativeAssetPath = relativeAssetPath.Remove(0, 1);
                }

                if (!relativeAssetPath.Contains("/"))
                {
                    // ルートディレクトリは無視
                    continue;
                }

                string[] splitNameList = relativeAssetPath.Split('/');
                if (splitNameList.Length < 2)
                {
                    continue;
                }

                string groupName = splitNameList[0];
                string address = string.Join("/", splitNameList.Skip(1).ToArray());

                AddressableAssetGroup group = settings.FindGroup(groupName);
                if (group == null)
                {
                    group = CreateAddressableGroup(settings, groupName);
                }

                if (group == null)
                {
                    Debug.LogError($"Failed to create group {groupName}");
                    continue;
                }

                AddressableAssetEntry entry = RegisterAssetPath(settings, group, assetPath, relativeAssetPath);
                isAnyAddressRegistered = true;
            }

            if (isAnyAddressRegistered)
            {
                AssetDatabase.SaveAssets();
            }
        }

        private static void DeleteAddressableAssets(AddressableAssetSettings settings, string[] deletedAssetsPathList)
        {
            bool isAnyAddressableAssetDeleted = false;
            foreach (string assetPath in deletedAssetsPathList)
            {
                if (!IsValidPath(assetPath))
                {
                    continue;
                }

                string[] splitNameList = assetPath.Split('/');
                if (!assetPath.Contains("/"))
                {
                    // ルートディレクトリは無視
                    continue;
                }

                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(guid))
                {
                    continue;
                }

                settings.RemoveAssetEntry(guid);
                isAnyAddressableAssetDeleted = true;
            }

            if (isAnyAddressableAssetDeleted)
            {
                DeleteEmptyGroup(settings);
                AssetDatabase.SaveAssets();
            }
        }

        private static void MoveAddressableAssets(AddressableAssetSettings settings, string[] movedAssetsPathList, string[] movedFromAssetPathList)
        {
            RemoveOldAddressableAssets(settings, movedAssetsPathList, movedFromAssetPathList);
            ImportAddressableAssets(settings, movedAssetsPathList);
        }

        private static void RemoveOldAddressableAssets(AddressableAssetSettings settings, string[] movedAssetsPathList, string[] movedFromAssetPathList)
        {
            bool isAnyOldAddressableAssetRemoved = false;
            for (int i = 0; i < movedFromAssetPathList.Length; i++) 
            {
                if (!IsValidPath(movedAssetsPathList[i]))
                {
                    continue;
                }

                string guid = AssetDatabase.AssetPathToGUID(movedAssetsPathList[i]);
                if (string.IsNullOrEmpty(guid))
                {
                    continue;
                }

                settings.RemoveAssetEntry(guid);
                isAnyOldAddressableAssetRemoved = true;
            }

            if (isAnyOldAddressableAssetRemoved)
            {
                DeleteEmptyGroup(settings);
                AssetDatabase.SaveAssets();
            }
        }

        private static AddressableAssetGroup CreateAddressableGroup(AddressableAssetSettings settings, string groupName)
        {
            var groupTemplate = settings.GetGroupTemplateObject(0) as AddressableAssetGroupTemplate;
            if (!groupTemplate)
            {
                Debug.LogError("Failed to get group template.");
                return null;
            }

            AddressableAssetGroup group = settings.CreateGroup(groupName, false, false, true, null, groupTemplate.GetTypes());
            groupTemplate.ApplyToAddressableAssetGroup(group);

            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema)
            {
                schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash;
            }

            return group;
        }

        private static AddressableAssetEntry RegisterAssetPath(AddressableAssetSettings settings, AddressableAssetGroup group, string assetPath, string assetName)
        {
            AddressableAssetEntry entry = null;
            try
            {
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                entry = settings.CreateOrMoveEntry(guid, group);

                string extension = Path.GetExtension(assetName);
                entry.SetAddress(assetName.Replace(extension, string.Empty));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return entry;
        }

        private static void DeleteEmptyGroup(AddressableAssetSettings settings)
        {
            for (int i = 0; i < settings.groups.Count; i++)
            {
                AddressableAssetGroup group = settings.groups[i];
                if (group == settings.DefaultGroup)
                {
                    continue;
                }

                if (group.entries.Count < 1)
                {
                    settings.RemoveGroup(group);
                }
            }
        }

        private static bool IsValidPath(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }

                if (!GameBasicSystemSettingData.Instance || string.IsNullOrEmpty(AddressableAssetRootDirectory))
                {
                    return false;
                }

                if (!path.Contains(AddressableAssetRootDirectory))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }
    }
}
