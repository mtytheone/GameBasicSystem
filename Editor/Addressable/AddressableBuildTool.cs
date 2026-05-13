using HatzeLaboratory.GameBasicSystem.Runtime.Addressable;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Addressable
{
    public sealed class AddressableBuildTool
    {
        private const string COPYING_FILE_PROGRESS_BAR_TITLE = "AddressableAsset追加";
        private const string COPYING_FILE_PROGRESS_BAR_TEXT = "Assets内にファイルをコピー中...";

        [MenuItem("HatzeLaboratory/GameBasicSystem/Addressables/BuildAA", false, 0)]
        public static void BuildAddressableAssets()
        {
            try
            {
                string saveAddressableAssetPath = AddressableDefine.SaveAddressableAssetPath;
                if (Directory.Exists(saveAddressableAssetPath))
                {
                    int fileCount = GetFileSystemEntryCount(saveAddressableAssetPath);
                    if (fileCount > 0)
                    {
                        Directory.Delete(saveAddressableAssetPath, recursive: true);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return;
            }

            BuildScript.buildCompleted += OnBuildCompleted;
            AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.BuildPlayerContent();
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Addressables/AddAA")]
        public static void AddAddressableAssetsInStreamingDirectory()
        {
            string sourceDirectory = GetLocalBuildPath();
            string targetDirectory = GetLocalLoadPath();
            int maxCopiedFileCount = GetFileSystemEntryCount(sourceDirectory);
            int currentCopiedFileCount = 0;
            CopyDirectory(sourceDirectory, targetDirectory, maxCopiedFileCount, ref currentCopiedFileCount);
            AssetDatabase.Refresh();

            ClearProgressBar();
            PlayBeep();
            Debug.Log("Addressable Asset Copy Completed.");
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Addressables/RemoveAA")]
        public static void RemoveAddressableAssetsInStreamingDirectory()
        {
            try
            {
                string targetDirectory = GetLocalLoadPath();
                Directory.Delete(targetDirectory, recursive: true);
            }
            catch(Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
            
            AssetDatabase.Refresh();
            PlayBeep();
            Debug.Log("Addressable Asset Remove Completed.");
        }

        private static void OnBuildCompleted(AddressableAssetBuildResult result)
        {
            if (result.LocationCount < 1)
            {
                Debug.LogWarning("No Addressable Asset.");
                return;
            }

            string saveAddressableAssetDirectory = GetLocalBuildPath();
            int buildDataCount = GetFileSystemEntryCount(saveAddressableAssetDirectory);
            if (buildDataCount < 1)
            {
                Debug.LogError("Build Data Not Found.");
                return;
            }

            try
            {
                string targetDirectory = Path.Combine(Application.streamingAssetsPath, AddressableDefine.RootDirectoryName);
                Directory.Delete(targetDirectory, recursive: true);
            }
            catch(Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return;
            }

            PlayBeep();
            Debug.Log("Addressable Asset Build Completed.");
            BuildScript.buildCompleted -= OnBuildCompleted;
        }

        private static void CopyDirectory(string sourceDirectory, string targetDirectory, int maxCopiedFileCount, ref int copiedFileCount)
        {
            float progress = (float)copiedFileCount / maxCopiedFileCount;
            bool isCanceled = DisplayCancelableProgressBar(COPYING_FILE_PROGRESS_BAR_TITLE, COPYING_FILE_PROGRESS_BAR_TEXT, progress);
            if (isCanceled)
            {
                Debug.LogError("It is canceled to copy AddressableAssetFiles.");
                return;
            }

            try
            {
                DirectoryInfo sourceDirectoryInfo = new(sourceDirectory);
                DirectoryInfo targetDirectoryInfo = new(targetDirectory);
                if (!targetDirectoryInfo.Exists)
                {
                    targetDirectoryInfo.Create();
                    targetDirectoryInfo.Attributes = sourceDirectoryInfo.Attributes;
                }

                FileInfo[] fileInfoList = sourceDirectoryInfo.GetFiles();
                for (int i = 0; i < fileInfoList.Length; i++)
                {
                    FileInfo sourceFileInfo = fileInfoList[i];
                    string targetFilePath = Path.Combine(targetDirectory, sourceFileInfo.Name);
                    sourceFileInfo.CopyTo(targetFilePath, overwrite: true);
                    copiedFileCount++;
                }

                DirectoryInfo[] directoryInfoList = sourceDirectoryInfo.GetDirectories();
                for (int i = 0; i < directoryInfoList.Length; i++)
                {
                    DirectoryInfo sourceSubDirectoryInfo = directoryInfoList[i];
                    string newTargetDirectory = Path.Combine(targetDirectory, sourceSubDirectoryInfo.Name);
                    CopyDirectory(sourceSubDirectoryInfo.FullName, newTargetDirectory, maxCopiedFileCount, ref copiedFileCount);
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool DisplayCancelableProgressBar(string title, string info, float progress)
        {
            if (Application.isBatchMode)
            {
                return false;
            }

            return EditorUtility.DisplayCancelableProgressBar(title, info, progress);
        }

        private static void ClearProgressBar()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            EditorUtility.ClearProgressBar();
        }

        private static int GetFileSystemEntryCount(string path)
        {
            string[] fileSystemEntryList = Directory.GetFileSystemEntries(path, "*.*", SearchOption.AllDirectories);
            return fileSystemEntryList.Length;
        }

        private static void PlayBeep()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            EditorApplication.Beep();
        }

        private static string GetLocalBuildPath()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettingsが見つかりません。");
                return string.Empty;
            }

            string currentProfileId = settings.activeProfileId;
            string localBuildPath = settings.profileSettings.GetValueByName(currentProfileId, "Local.BuildPath");
            return settings.profileSettings.EvaluateString(currentProfileId, localBuildPath);
        }

        private static string GetLocalLoadPath()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettingsが見つかりません。");
                return string.Empty;
            }

            string currentProfileId = settings.activeProfileId;
            string localBuildPath = settings.profileSettings.GetValueByName(currentProfileId, "Local.LoadPath");
            localBuildPath = localBuildPath.Replace("{", "[");
            localBuildPath = localBuildPath.Replace("}", "]");
            return settings.profileSettings.EvaluateString(currentProfileId, localBuildPath);
        }
    }
}