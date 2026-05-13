using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using HatzeLaboratory.GameBasicSystem.Runtime.System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HatzeLaboratory.GameBasicSystem.Runtime.Addressable
{
    public static class AddressableDefine
    {
        public const string RootDirectoryName = "BuiltInAA";


#if UNITY_EDITOR
        public static string SaveAddressableAssetPath => GetSaveAddressableAssetPath();
#endif

        public static string LoadAddressableAssetPath => GetLoadAddressableAssetPath();


#if UNITY_EDITOR
        private static string GetSaveAddressableAssetPath()
        {
            // <Root>/BuildInAA/<Version>/<PlatformName>/
            string projectRootDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            return Path.Combine
            (
                projectRootDirectory,
                RootDirectoryName,
                GetVersionName(),
                PlatformDefine.GetPlatformName(EditorUserBuildSettings.activeBuildTarget)
            );
        }
#endif

        private static string GetLoadAddressableAssetPath()
        {
            // <Root>/Assets/StreamingAssets/BuildInAA/<Version>/<PlatformName>/
            string loadPath = Path.Combine
            (
                Application.streamingAssetsPath,
                RootDirectoryName,
                GetVersionName(),
#if UNITY_EDITOR
                PlatformDefine.GetPlatformName(EditorUserBuildSettings.activeBuildTarget)
#else
                PlatformDefine.GetPlatformName(Application.platform)
#endif
            );

            return Path.GetFullPath(loadPath);
        }

        private static string GetVersionName()
        {
            List<char> invalidCharacterList = Path.GetInvalidFileNameChars().ToList();
            invalidCharacterList.Add('.');
            string versionText = Application.version.Trim();
            return new string(versionText.Select(c => invalidCharacterList.Contains(c) ? '_' : c).ToArray());
        }
    }
}
