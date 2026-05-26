using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.SaveData
{
    public static class SaveDataEditorTool
    {
        private static string SaveDataFilePath
        {
            get
            {
                string fileName = GameBasicSystemSettingData.Instance != null
                    ? GameBasicSystemSettingData.Instance.SaveDataFileName
                    : "SaveData.hld";

                return Path.Combine(Application.persistentDataPath, fileName);
            }
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Save Data/Open Save Data Folder", false, 100)]
        private static void OpenSaveDataFolder()
        {
            if (!Directory.Exists(SaveDataFilePath))
            {
                Debug.LogWarning($"Save data folder does not exist: {SaveDataFilePath}");
                return;
            }

            EditorUtility.RevealInFinder(SaveDataFilePath);
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Save Data/Delete Save Data", false, 101)]
        private static void DeleteSaveData()
        {
            if (!File.Exists(SaveDataFilePath))
            {
                Debug.LogWarning($"Save data file does not exist: {SaveDataFilePath}");
                return;
            }

            bool isConfirmed = EditorUtility.DisplayDialog(
                "Delete Save Data",
                $"Are you sure you want to delete the save data?\n{SaveDataFilePath}",
                "Delete",
                "Cancel");
            if (!isConfirmed)
            {
                return;
            }

            File.Delete(SaveDataFilePath);
            Debug.Log($"Save data deleted: {SaveDataFilePath}");
        }
    }
}
