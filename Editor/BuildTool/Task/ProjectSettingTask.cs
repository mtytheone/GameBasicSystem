using HatzeLaboratory.GameBasicSystem.Editor.BuildTool;
using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Task
{
    public class ProjectSettingTask : IPlayerBuildTask
    {
        private readonly NamedBuildTarget _namedBuildTarget = NamedBuildTarget.Standalone;


        void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType)
        {
            switch (buildType)
            {
                case PlayerBuildTool.BuildType.Debug:
                    SetIL2CPPSetting(isReleaseBuild: false);
                    break;

                case PlayerBuildTool.BuildType.Development:
                    SetIL2CPPSetting(isReleaseBuild: false);
                    break;

                case PlayerBuildTool.BuildType.Release:
                    SetIL2CPPSetting(isReleaseBuild: true);
                    break;
            }
        }

        void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport)
        {
            SetIL2CPPSetting(isReleaseBuild: false);
        }

        private void SetIL2CPPSetting(bool isReleaseBuild)
        {
            if (isReleaseBuild)
            {
                PlayerSettings.SetScriptingBackend(_namedBuildTarget, ScriptingImplementation.IL2CPP);
                PlayerSettings.SetIl2CppCodeGeneration(_namedBuildTarget, Il2CppCodeGeneration.OptimizeSpeed);
                PlayerSettings.SetIl2CppCompilerConfiguration(_namedBuildTarget, Il2CppCompilerConfiguration.Master);
                PlayerSettings.SetIl2CppStacktraceInformation(_namedBuildTarget, Il2CppStacktraceInformation.MethodOnly);
            }
            else
            {
                PlayerSettings.SetScriptingBackend(_namedBuildTarget, ScriptingImplementation.Mono2x);
            }
        }
    }
}
