using HatzeLaboratory.GameBasicSystem.Runtime.Scene;
using HatzeLaboratory.GameBasicSystem.Runtime.Sound;
using HatzeLaboratory.GameBasicSystem.Runtime.UI;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation
{
    public sealed class ManagerCreator
    {
        [MenuItem("Assets/Create/HatzeLab/Manager/Create ModalManager", false, -300)]
        public static void CreateModalManager()
        {
            CreateManager<ModalManager>(nameof(ModalManager));
        }

        [MenuItem("Assets/Create/HatzeLab/Manager/Create SceneController", false, -299)]
        public static void CreateSceneController()
        {
            CreateManager<SceneController>(nameof(SceneController));
        }

        [MenuItem("Assets/Create/HatzeLab/Manager/Create SoundManager", false, -298)]
        public static void CreateSoundManager()
        {
            CreateManager<SoundManager>(nameof(SoundManager));
        }

        private static void CreateManager<T>(string objectName) where T : Component
        {
            GameObject manager = EditorUtility.CreateGameObjectWithHideFlags(objectName, HideFlags.HideInHierarchy);
            manager.AddComponent<T>();

            string currentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath = "Assets";
            }

            string absolutePath = Path.Combine(Application.dataPath.Replace("Assets", ""), currentPath);
            bool isDirectory = File.GetAttributes(absolutePath).HasFlag(FileAttributes.Directory);
            if (!isDirectory)
            {
                currentPath = Path.GetDirectoryName(currentPath);
            }

            string outputPath = $"{currentPath}/{objectName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(manager, outputPath, out bool isSuccess);
            if (!isSuccess)
            {
                string message = "Failed to create GameObject.";
                EditorUtility.DisplayDialog("Create GameObject", message, "OK");
            }

            // Prefab作成後に作成したPrefabを選択状態にする
            Object prefabAsset = AssetDatabase.LoadAssetAtPath<Object>(outputPath);
            Selection.activeObject = prefabAsset;

            Object.DestroyImmediate(manager);
        }
    }
}
