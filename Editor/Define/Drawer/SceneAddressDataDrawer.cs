using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator;
using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer
{
    public class SceneAddressDataDrawer : ISettingProviderDrawer
    {
        private const string LIST_HEADER_NAME = "Scene Address List";

        private readonly ReorderableList _reorderableList;
        private readonly SceneTypeNameEnumCreator _typeNameEnumCreator = new();

        private GameBasicSystemSettingData SettingData => GameBasicSystemSettingData.Instance;
        private List<GameBasicSystemSettingData.SceneAddressData> SceneAddressDataList => SettingData.SceneAddressDataList;

        void ISettingProviderDrawer.Draw()
        {
            _reorderableList.DoLayoutList();
        }

        public SceneAddressDataDrawer()
        {
            _reorderableList = CreateReorderableList();
        }

        private ReorderableList CreateReorderableList()
        {
            return new(SceneAddressDataList, typeof(GameBasicSystemSettingData.SceneAddressData))
            {
                draggable = true,
                drawHeaderCallback = rect =>
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(rect, LIST_HEADER_NAME);

                    const float applyButtonWidth = 60;
                    rect.x = rect.size.x - applyButtonWidth;
                    rect.width = applyButtonWidth;
                    if (GUI.Button(rect, "Apply", EditorStyles.miniButton))
                    {
                        bool isSuccess = _typeNameEnumCreator.CreateModalNameEnum();
                        if (!isSuccess)
                        {
                            const string title = "シーンタイプEnum作成";
                            const string message = "シーンタイプのEnum作成に失敗しました。";
                            EditorUtility.DisplayDialog(title, message, "OK");
                        }
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    float width = rect.width;
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 120;

                    rect.width = width / 2.25f;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += 2;

                    GameBasicSystemSettingData.SceneAddressData data = SceneAddressDataList[index];
                    data.SceneTypeName = EditorGUI.TextField(rect, "SceneTypeName", data.SceneTypeName);

                    rect.x += rect.size.x * 1.15f;
                    data.SceneAddress = EditorGUI.TextField(rect, "SceneAddress", data.SceneAddress);

                    EditorGUIUtility.labelWidth = labelWidth;

                    rect.x = width * 0.9975f;
                    data.IsEditorOnly = EditorGUI.Toggle(rect, data.IsEditorOnly);
                },
            };
        }
    }
}