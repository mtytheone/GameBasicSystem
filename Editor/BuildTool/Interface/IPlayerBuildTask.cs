using UnityEditor.Build.Reporting;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface
{
    public interface IPlayerBuildTask
    {
        public void RunPreprocess(PlayerBuildTool.BuildType buildType);
        public void RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport);
    }
}
