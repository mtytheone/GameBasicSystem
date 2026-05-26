using HatzeLaboratory.GameBasicSystem.Editor.BuildTool;
using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Task
{
    public class ActiveSceneTask : IPlayerBuildTask
    {
        private string _previousActiveScenePath;

        void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid())
            {
                _previousActiveScenePath = activeScene.path;
            }
        }

        void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport)
        {
            if (string.IsNullOrEmpty(_previousActiveScenePath))
            {
                return;
            }

            EditorSceneManager.OpenScene(_previousActiveScenePath);
        }
    }
}
