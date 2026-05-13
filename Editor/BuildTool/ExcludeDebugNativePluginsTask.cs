using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool
{
    public class ExcludeDebugNativePluginsTask : IPlayerBuildTask
    {
        void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType)
        {
            if (buildType != PlayerBuildTool.BuildType.Release)
            {
                return;
            }

            SetDevelopmentPluginsIncludeInBuild(false);
        }

        void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport)
        {
            if (buildType != PlayerBuildTool.BuildType.Release)
            {
                return;
            }

            SetDevelopmentPluginsIncludeInBuild(true);
        }

        private void SetDevelopmentPluginsIncludeInBuild(bool isIncluded)
        {
            string[] guidList = AssetDatabase.FindAssets("TotalUsedMemory");
            foreach (string guid in guidList)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains("com.hatzelaboratory.gamebasicsystem"))
                {
                    continue;
                }

                PluginImporter importer = AssetImporter.GetAtPath(path) as PluginImporter;
                if (!importer)
                {
                    continue;
                }

                importer.SetIncludeInBuildDelegate(path => { return isIncluded; });
            }
        }
    }
}