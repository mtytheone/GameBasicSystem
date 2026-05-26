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
    /// <summary>
    /// Addressables のパス定数を提供するユーティリティクラス。
    /// パスは Project Settings &gt; GameBasicSystem の設定とアプリバージョンに基づいて動的に生成されます。
    /// </summary>
    public static class AddressableDefine
    {
        /// <summary>
        /// Addressables アセットのルートディレクトリ名を取得します。
        /// Project Settings で未設定の場合は "BuiltInAA" を返します。
        /// </summary>
        public static string RootDirectoryName
        {
            get
            {
                if (!GameBasicSystemSettingData.Instance)
                {
                    return "BuiltInAA";
                }

                string rootDirectoryPath = GameBasicSystemSettingData.Instance.AddressableAssetRootDirectory;
                if (string.IsNullOrEmpty(rootDirectoryPath))
                {
                    return "BuiltInAA";
                }

                return Path.GetFileName(rootDirectoryPath);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Addressables アセットのビルド出力先パスを取得します（Editor のみ）。
        /// パス形式: <c>{ProjectRoot}/{RootDir}/{Version}/{Platform}/</c>
        /// </summary>
        public static string SaveAddressableAssetPath => GetSaveAddressableAssetPath();
#endif

        /// <summary>
        /// Addressables アセットの実行時読み込みパスを取得します。
        /// パス形式: <c>StreamingAssets/{RootDir}/{Version}/{Platform}/</c>
        /// </summary>
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
