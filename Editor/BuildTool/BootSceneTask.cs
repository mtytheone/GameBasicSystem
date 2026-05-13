using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.Scene;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool
{
    public sealed class BootSceneTask : IPlayerBuildTask
    {
        private const string GAMEOBJECT_NAME = "Boot";

        private string SaveBootSceneDirectory => PlayerBuildTool.SAVE_BOOT_SCENE_DIRECTORY;
        private string BootSceneName => PlayerBuildTool.BOOT_SCENE_NAME;

        void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType)
        {
            try
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

                GameObject gameObject = new(GAMEOBJECT_NAME);
                gameObject.AddComponent<BootSceneController>();

                if (!Directory.Exists(SaveBootSceneDirectory))
                {
                    Directory.CreateDirectory(SaveBootSceneDirectory);
                }

                EditorSceneManager.SaveScene(scene, $"{SaveBootSceneDirectory}/{BootSceneName}.unity");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create boot scene: {e.Message}");
            }
        }

        void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport)
        {
            AssetDatabase.DeleteAsset(SaveBootSceneDirectory);
        }
    }
}
