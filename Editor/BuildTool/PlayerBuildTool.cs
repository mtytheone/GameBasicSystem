using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool
{
    public sealed class PlayerBuildTool
    {
        public const string BOOT_SCENE_NAME = "BootScene";
        public const string SAVE_BOOT_SCENE_DIRECTORY = "Assets/TempBootScene";

        public enum BuildType
        {
            Debug,
            Development,
            Release
        }

        private static readonly string[] _notCopyDirectoryKeywordListForReleaseBuild =
        {
            "BackUpThisFolder_ButDontShipItWithYourGame",
            "BurstDebugInformation_DoNotShip",
        };


        [MenuItem("HatzeLaboratory/GameBasicSystem/Build/Rom/Debug", false, 0)]
        private static void CreateDebugRom()
        {
            bool isZipFileNeeded = ShowAskingCreatingZipFileDialog();
            BuildRom(BuildType.Debug, isZipFileNeeded);
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Build/Rom/Development", false, 1)]
        private static void CreateDevelopmentRom()
        {
            bool isZipFileNeeded = ShowAskingCreatingZipFileDialog();
            BuildRom(BuildType.Development, isZipFileNeeded);
        }

        [MenuItem("HatzeLaboratory/GameBasicSystem/Build/Rom/Release", false, 2)]
        private static void CreateReleaseRom()
        {
            bool isZipFileNeeded = ShowAskingCreatingZipFileDialog();
            BuildRom(BuildType.Release, isZipFileNeeded);
        }

        private static bool ShowAskingCreatingZipFileDialog()
        {
            return EditorUtility.DisplayDialog("Rom Build", "Do you want to create a zipped Rom?", "Yes", "No");
        }

        /// <summary>
        /// バッチファイルから呼ぶための関数。他の関数から呼ばれることは想定していない。
        /// </summary>
        private static void CreateRomFromBatchFile()
        {
            string[] args = Environment.GetCommandLineArgs();

            BuildType buildType = BuildType.Development;
            bool isZipFileNeeded = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-BuildType")
                {
                    switch (args[i + 1])
                    {
                        case "Debug":
                            buildType = BuildType.Debug;
                            break;

                        case "Development":
                            buildType = BuildType.Development;
                            break;

                        case "Release":
                            buildType = BuildType.Release;
                            break;

                        default:
                            return;
                    }
                }
                else if (args[i] == "-CreateZipFile")
                {
                    isZipFileNeeded = true;
                }
            }

            BuildRom(buildType, isZipFileNeeded);
        }

        private static void BuildRom(BuildType buildType, bool isZipFileNeeded)
        {
            List<IPlayerBuildTask> taskList = GameBasicSystemSettingData.Instance.PlayerBuildTaskDataList
                .Where(taskData => taskData != null && taskData.TaskClassType != null && taskData.IsEnabled)
                .Select(taskData => (IPlayerBuildTask)Activator.CreateInstance(taskData.TaskClassType))
                .ToList();

            BuildInfo buildInfo = new(buildType);
            CreateSaveDirectory(buildInfo.ApplicationSaveDirectory);
            BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions
                (
                    buildType,
                    buildInfo.ApplicationSavePath
                );

            taskList.ForEach(task => task?.RunPreprocess(buildType));
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            taskList.ForEach(task => task?.RunPostprocess(buildType, report));

            ShowBuildResult(buildType, report.summary, isZipFileNeeded);
            if (isZipFileNeeded && report.summary.result == BuildResult.Succeeded)
            {
                CreateZipFile(buildType, buildInfo);
                ShowZipFileCreationResult(buildType, report.summary);
            }
        }

        private static BuildPlayerOptions GetBuildPlayerOptions(BuildType buildType, string saveApplicationPath)
        {
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = new[]
                {
                    $"{SAVE_BOOT_SCENE_DIRECTORY}/{BOOT_SCENE_NAME}.unity"
                },
                locationPathName = saveApplicationPath,
                target = EditorUserBuildSettings.activeBuildTarget
            };

            switch (buildType)
            {
                case BuildType.Debug:
                    buildPlayerOptions.options |= BuildOptions.Development;
                    buildPlayerOptions.options |= BuildOptions.EnableDeepProfilingSupport;
                    buildPlayerOptions.options |= BuildOptions.AllowDebugging;
                    break;

                case BuildType.Development:
                    buildPlayerOptions.options = BuildOptions.Development;
                    break;

                case BuildType.Release:
                    buildPlayerOptions.options = BuildOptions.CleanBuildCache;
                    break;

                default:
                    break;
            }

            return buildPlayerOptions;
        }

        private static void ShowBuildResult(BuildType buildType, BuildSummary summary, bool isZipFileNeeded)
        {
            string message = GetBuildResultMessage(buildType, summary);
            Debug.Log(message);

            if (Application.isBatchMode)
            {
                return;
            }

            if (isZipFileNeeded && summary.result == BuildResult.Succeeded)
            {
                return;
            }

            PlayBeep();
            EditorUtility.DisplayDialog("Rom Build Result", message, "OK");
        }

        private static void CreateZipFile(BuildType buildType, BuildInfo buildInfo)
        {
            string saveDirectory = buildInfo.ZipFileSaveDirectory;
            CreateSaveDirectory(saveDirectory);

            using (ZipArchive zipArchive = ZipFile.Open(buildInfo.ZipFileSavePath, ZipArchiveMode.Create))
            {
                IEnumerable<string> fileSystemList = Directory.EnumerateFileSystemEntries(buildInfo.ApplicationSaveDirectory, "*.*", SearchOption.AllDirectories);
                foreach (string filePath in fileSystemList)
                {
                    string baseDirectory = buildInfo.ApplicationSaveDirectory;
                    string relativePath = Path.GetRelativePath(baseDirectory, filePath);
                    if (buildType == BuildType.Release && _notCopyDirectoryKeywordListForReleaseBuild.Any(keyword => filePath.Contains(keyword)))
                    {
                        continue;
                    }

                    if (File.Exists(filePath))
                    {
                        zipArchive.CreateEntryFromFile(filePath, relativePath);
                    }
                    else if (Directory.Exists(filePath))
                    {
                        zipArchive.CreateEntry($"{relativePath}/");
                    }
                }
            }
        }

        private static void ShowZipFileCreationResult(BuildType buildType, BuildSummary summary)
        {
            Debug.Log("End to create zip file.");
            if (Application.isBatchMode)
            {
                return;
            }

            PlayBeep();

            string message = GetBuildResultMessage(buildType, summary, isZipFileCreated: true);
            EditorUtility.DisplayDialog("Romビルド結果", message, "OK");
        }

        private static string GetBuildResultMessage(BuildType buildType, BuildSummary summary, bool isZipFileCreated = false)
        {
            string message = "";
            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    message = $"{buildType.ToString()}Build {(isZipFileCreated ? "And Create Zipfile" : "")} succeeded.\n\n"
                        + $"Rom Size: {summary.totalSize / 1024 / 1024}MB\n"
                        + $"BuildTime: {summary.totalTime.Hours:00}:{summary.totalTime.Minutes:00}:{summary.totalTime.Seconds:00}";
                    break;

                default:
                    message = $"{buildType.ToString()}Build failed.";
                    break;
            }

            return message;
        }

        private static void CreateSaveDirectory(string saveDirectory)
        {
            if (Directory.Exists(saveDirectory))
            {
                return;
            }

            Directory.CreateDirectory(saveDirectory);
        }

        private static void PlayBeep()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            EditorApplication.Beep();
        }

        private struct BuildInfo
        {
            private BuildType _buildType;

            public string Date { get; private set; }

            public string VersionText => GetVersionText();
            public string PlatformName => PlatformDefine.GetPlatformName(EditorUserBuildSettings.activeBuildTarget);

            public string ApplicationSaveDirectory => GetSaveDirectory(isZipFile: false);
            public string ZipFileSaveDirectory => GetSaveDirectory(isZipFile: true);

            public string ApplicationSavePath => Path.Combine(ApplicationSaveDirectory, GetApplicationName(isZipFile: false));

            public string ZipFileSavePath => Path.Combine(ZipFileSaveDirectory, GetApplicationName(isZipFile: true));

            public BuildInfo(BuildType buildType)
            {
                _buildType = buildType;
                Date = DateTime.Now.ToString("yyMMddHHmmss");
            }

            private string GetSaveDirectory(bool isZipFile)
            {
                string rootDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
                string saveDirectory = Path.Combine
                (
                    rootDirectory,
                    "Rom",
                    PlatformName,
                    _buildType.ToString(),
                    GetApplicationName(isZipFile).Split(".")[0],
                    isZipFile ? "Zip" : "Raw"
                );

                return saveDirectory;
            }

            private string GetApplicationName(bool isZipFile)
            {
                return string.Format
                (
                    isZipFile ? "{0}_{1}_{2}_{3}_{4}.zip" : "{0}_{1}_{2}_{3}_{4}.exe",
                    Application.productName,
                    _buildType.ToString(),
                    VersionText,
                    Date,
                    PlatformName
                );
            }

            private string GetVersionText()
            {
                return Application.version.Replace(".", "");
            }
        }
    }
}