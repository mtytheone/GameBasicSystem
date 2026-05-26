using HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer;
using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        private bool _isStatudsFoldout;

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
            EditorGUILayout.Space();

            var instance = GameBasicSystemSettingData.Instance;
            if (!instance)
            {
                EditorGUILayout.HelpBox(
                    "GameBasicSystemSettingData not found. Click the \"Generate\" button to create it.",
                    MessageType.Error);
                if (GUILayout.Button("Generate"))
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
                    if (!string.IsNullOrEmpty(drawer.SectionTitle))
                    {
                        EditorGUILayout.LabelField(drawer.SectionTitle, EditorStyles.boldLabel);
                    }

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.Space(2);
                    drawer.Draw();
                    EditorGUILayout.Space(2);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(6);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(instance);
            }

            EditorGUILayout.Space(12);
            DrawValidationStatus(instance);
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
            setting.SetAddressableAssetRootDirectory("BuiltInAA");
            setting.SetSaveDataFileName("SaveData.hld");
            setting.ModalAddressDataList.Add(new GameBasicSystemSettingData.ModalAddressData
            {
                ModalName = "BaseModal", 
                ModalAddress = "Modal/BaseModal" 
            });
            setting.ModalAddressDataList.Add(new GameBasicSystemSettingData.ModalAddressData
            {
                ModalName = "Loading", 
                ModalAddress = "Modal/LoadingModal" 
            });

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
                new BootSceneAddressDataDrawer(),
                new SceneAddressDataDrawer(),
                new ModalAddressDataDrawer(),
                new InputActionAssetDrawer(),
                new SaveDataEncryptionKeyDrawer(),
                new EnablePlayerBuildTaskDrawer(),
            };
        }

        private void DrawValidationStatus(GameBasicSystemSettingData instance)
        {
            EditorGUILayout.LabelField("Setup Status", EditorStyles.boldLabel);
            bool hasRootDirectory = !string.IsNullOrWhiteSpace(instance.AddressableAssetRootDirectory);
            bool hasInputActionAsset = instance.InputActionAsset != null;
            bool hasEncryptionKey = !string.IsNullOrWhiteSpace(instance.SaveDataEncryptionKey)
                && Encoding.UTF8.GetByteCount(instance.SaveDataEncryptionKey) == 32;
            bool hasBootDevScene = !string.IsNullOrWhiteSpace(instance.BootSceneData?.DevelopmentSceneAddress);
            bool hasBootProdScene = !string.IsNullOrWhiteSpace(instance.BootSceneData?.ProductionSceneAddress);
            bool hasModals = instance.ModalAddressDataList.Count > 0;
            bool hasScenes = instance.SceneAddressDataList.Count > 0;
            bool hasSaveFileName = !string.IsNullOrWhiteSpace(instance.SaveDataFileName);
            bool allOk = hasRootDirectory 
                && hasInputActionAsset 
                && hasEncryptionKey 
                && hasBootDevScene 
                && hasBootProdScene 
                && hasModals 
                && hasScenes 
                && hasSaveFileName;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    Color previousColor = GUI.contentColor;
                    GUI.contentColor = allOk ? Color.green : Color.yellow;

                    GUIContent iconContent = new(EditorGUIUtility.IconContent(allOk ? "d_Valid" : "d_console.warnicon.sml").image);
                    EditorGUILayout.LabelField(iconContent, GUILayout.Width(20));

                    string summary = allOk ? "All settings are complete." : "Some settings are missing.";
                    _isStatudsFoldout = EditorGUILayout.Foldout(_isStatudsFoldout, summary, true, EditorStyles.foldoutHeader);
                    
                    GUI.contentColor = previousColor;
                }

                if (_isStatudsFoldout)
                {
                    EditorGUILayout.Space(2);
                    using (new EditorGUI.IndentLevelScope())
                    {
                        DrawStatusRow("Addressable Asset Root Directory", hasRootDirectory);
                        DrawStatusRow("Input Action Asset", hasInputActionAsset);
                        DrawStatusRow("Save Data Encryption Key (32 bytes)", hasEncryptionKey);
                        DrawStatusRow("Boot Scene - Development Address", hasBootDevScene);
                        DrawStatusRow("Boot Scene - Production Address", hasBootProdScene);
                        DrawStatusRow("Modal Address Data", hasModals);
                        DrawStatusRow("Scene Address Data", hasScenes);
                        DrawStatusRow("Save Data File Name", hasSaveFileName);
                    }
                }
            }
        }

        private static void DrawStatusRow(string label, bool isOk)
        {
            GUIContent content = new(
                EditorGUIUtility.IconContent(isOk ? "d_Valid" : "d_console.warnicon.sml").image,
                label
            );

            Color previousColor = GUI.contentColor;
            GUI.contentColor = isOk ? Color.green : Color.yellow;
            content.text = $"  {label}";
            EditorGUILayout.LabelField(content);
            GUI.contentColor = previousColor;
        }
    }
}