using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator;
using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer
{
    public class ModalAddressDataDrawer : ISettingProviderDrawer
    {
        private const string LIST_HEADER_NAME = "Modal Address List";

        private readonly ReorderableList _reorderableList;
        private readonly ModalNameEnumCreator _modalNameCreator = new();

        private GameBasicSystemSettingData SettingData => GameBasicSystemSettingData.Instance;
        private List<GameBasicSystemSettingData.ModalAddressData> ModalAddressDataList => SettingData.ModalAddressDataList;

        void ISettingProviderDrawer.Draw()
        {
            _reorderableList.DoLayoutList();
        }

        public ModalAddressDataDrawer()
        {
            _reorderableList = CreateReorderableList();
        }

        private ReorderableList CreateReorderableList()
        {
            return new(ModalAddressDataList, typeof(GameBasicSystemSettingData.ModalAddressData))
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
                        bool isSuccess = _modalNameCreator.CreateModalNameEnum();
                        if (!isSuccess)
                        {
                            const string title = "モーダルEnum作成";
                            const string message = "モーダル名のEnum作成に失敗しました。";
                            EditorUtility.DisplayDialog(title, message, "OK");
                        }
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 90;

                    rect.width = rect.size.x / 2.25f;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += 2;

                    GameBasicSystemSettingData.ModalAddressData data = ModalAddressDataList[index];
                    data.ModalName = EditorGUI.TextField(rect, "ModalName", data.ModalName);
                    
                    rect.x += rect.size.x * 1.15f;
                    data.ModalAddress = EditorGUI.TextField(rect, "ModalAddress", data.ModalAddress);

                    EditorGUIUtility.labelWidth = labelWidth;
                },
            };
        }
    }
}