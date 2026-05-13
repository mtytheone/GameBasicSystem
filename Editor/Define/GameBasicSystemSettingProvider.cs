using HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer;
using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define
{
    public sealed class GameBasicSystemSettingProvider : SettingsProvider
    {
        private const string SettingsPath = "Project/GameBasicSystem";
        private const string SettingDataPath = "Assets/HatzeLab/Resources/GameBasicSystemSettingData.asset";

        private ISettingProviderDrawer[] _drawerList;
        private Vector2 _scrollPosition;

        public GameBasicSystemSettingProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            CreateDrawerList();
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            GameBasicSystemSettingProvider provider = new(SettingsPath, SettingsScope.Project);
            return provider;
        }

        public override void OnGUI(string searchContext)
        {
            var instance = GameBasicSystemSettingData.Instance;
            if (!instance)
            {
                if (GUILayout.Button("生成する"))
                {
                    CreateSetting();
                    CreateDrawerList();
                }

                return;
            }

            EditorGUI.BeginChangeCheck();
            using (EditorGUILayout.ScrollViewScope scope = new(_scrollPosition))
            {
                _scrollPosition = scope.scrollPosition;
                foreach (ISettingProviderDrawer drawer in _drawerList)
                {
                    drawer.Draw();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(instance);
            }
        }

        private void CreateSetting()
        {
            string fileName = Path.GetFileName(SettingDataPath);
            string rootDirectory = SettingDataPath.Replace(fileName, "");
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }

            GameBasicSystemSettingData setting = ScriptableObject.CreateInstance<GameBasicSystemSettingData>();
            AssetDatabase.CreateAsset(setting, SettingDataPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CreateDrawerList()
        {
            if (!GameBasicSystemSettingData.Instance)
            {
                return;
            }

            _drawerList = new ISettingProviderDrawer[]
            {
                new AddressableAssetRootPathDrawer(),
                new EnablePlayerBuildTaskDrawer(),
                new BootSceneAddressDataDrawer(),
                new ModalAddressDataDrawer(),
                new SceneAddressDataDrawer(),
            };
        }
    }
}