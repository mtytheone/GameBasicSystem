using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator;
using HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Drawer
{
    public class SceneAddressDataDrawer : ISettingProviderDrawer
    {
        private const string LIST_HEADER_NAME = "Scene Address List";
        private const string APPLY_BUTTON_TEXT = "Apply";
        private const float APPLY_BUTTON_WIDTH = 60f;
        private const float MIN_COLUMN_WIDTH = 120f;
        private const float SEPARATOR_WIDTH = 2f;
        private const float SEPARATOR_HIT_PAD = 4f;

        private GUIStyle _columnHeaderStyle;
        private Rect[] _headerColumnRectList;
        private Rect[] _elementColumnRectList;
        private float _resizeColumn1Width = MIN_COLUMN_WIDTH;
        private float _resizeColumn3Width = MIN_COLUMN_WIDTH;

        private int _resizingSeparatorIndex = -1;
        private float _resizeDragStartX;
        private float _resizeStartColumnWidth;
        private float _elementRectX = -1;
        private float _cachedAdjustedWidth = -1;

        private readonly ReorderableList _reorderableList;
        private readonly SceneTypeNameEnumCreator _typeNameEnumCreator = new();

        private GUIStyle ColumnHeaderStyle => _columnHeaderStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
        };
        private Color HeaderBackgroundColor => EditorGUIUtility.isProSkin
            ? new Color(0.16f, 0.16f, 0.16f)
            : new Color(0.55f, 0.55f, 0.55f);
        private Color SeparatorColor => EditorGUIUtility.isProSkin
            ? new Color(0.10f, 0.10f, 0.10f)
            : new Color(0.35f, 0.35f, 0.35f);
        private GameBasicSystemSettingData SettingData => GameBasicSystemSettingData.Instance;
        private List<GameBasicSystemSettingData.SceneAddressData> SceneAddressDataList => SettingData.SceneAddressDataList;

        string ISettingProviderDrawer.SectionTitle => "Scene Address Data";

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
                headerHeight = EditorGUIUtility.singleLineHeight * 3,
                drawHeaderCallback = headerAreaRect =>
                {
                    headerAreaRect = DrawListHeader(headerAreaRect);
                    headerAreaRect.y += 2f;
                    DrawRowHeader(headerAreaRect);
                },

                drawElementCallback = (elementAreaRect, index, isActive, isFocused) =>
                {
                    _elementRectX = elementAreaRect.x;
                    Rect allDrawElementAreaRect = new(elementAreaRect.x, elementAreaRect.y + 2, elementAreaRect.width, EditorGUIUtility.singleLineHeight);
                    _elementColumnRectList = SplitColumn(allDrawElementAreaRect);
                    DrawRowSeparator(elementAreaRect);
                    DrawRowElement(index, EditorGUIUtility.singleLineHeight);
                },
            };
        }

        private void DrawRowElement(int index, float lineHeight)
        {
            GameBasicSystemSettingData.SceneAddressData data = SceneAddressDataList[index];

            const float separatorMargin = 6f;
            float sceneTypeTextFieldX = _elementColumnRectList[0].x + separatorMargin;
            float sceneTypeTextFieldWidth = _elementColumnRectList[0].width - separatorMargin * 2;
            Rect sceneTypeTextFieldRect = new(sceneTypeTextFieldX, _elementColumnRectList[0].y, sceneTypeTextFieldWidth, _elementColumnRectList[0].height);
            data.SceneTypeName = EditorGUI.TextField(sceneTypeTextFieldRect, data.SceneTypeName);
            
            float sceneAddressTextFieldX = _elementColumnRectList[1].x + separatorMargin;
            float sceneAddressTextFieldWidth = _elementColumnRectList[1].width - separatorMargin * 2;
            Rect sceneAddressTextFieldRect = new(sceneAddressTextFieldX, _elementColumnRectList[1].y, sceneAddressTextFieldWidth, _elementColumnRectList[1].height);
            data.SceneAddress = EditorGUI.TextField(sceneAddressTextFieldRect, data.SceneAddress);
            
            float toggleWidth = lineHeight;
            float toggleX = _elementColumnRectList[2].x + (_elementColumnRectList[2].width - toggleWidth) * 0.5f;
            Rect toggleRect = new(toggleX, _elementColumnRectList[2].y, toggleWidth, toggleWidth);
            data.IsDevelopmentOnly = EditorGUI.Toggle(toggleRect, data.IsDevelopmentOnly);
        }

        private Rect DrawListHeader(Rect rect)
        {
            float drawAreaWidth = rect.width;
            float drawAreaHeight = EditorGUIUtility.singleLineHeight;

            // タイトル
            float labelTextWidth = drawAreaWidth - APPLY_BUTTON_WIDTH - 4f;
            Rect labelTextRect = new(rect.x, rect.y, labelTextWidth, drawAreaHeight);
            EditorGUI.LabelField(labelTextRect, LIST_HEADER_NAME);

            // Applyボタン
            bool hasPendingChanges = _typeNameEnumCreator.HasPendingChanges();
            using (new EditorGUI.DisabledGroupScope(!hasPendingChanges))
            {
                float applyButtonX = rect.x + drawAreaWidth - APPLY_BUTTON_WIDTH;
                Rect applyButtonRect = new(applyButtonX, rect.y, APPLY_BUTTON_WIDTH, drawAreaHeight);
                if (GUI.Button(applyButtonRect, APPLY_BUTTON_TEXT, EditorStyles.miniButton))
                {
                    bool isSuccess = _typeNameEnumCreator.CreateEnum();
                    if (!isSuccess)
                    {
                        const string title = "Create Scene Type Enum";
                        const string message = "Failed to create scene type name enum.";
                        EditorUtility.DisplayDialog(title, message, "OK");
                    }
                }
            }

            return rect;
        }

        private void DrawRowHeader(Rect currentRect)
        {
            float drawAreaWidth = currentRect.width;
            float drawAreaHeight = EditorGUIUtility.singleLineHeight;

            float xOffset = _elementRectX >= 0f ? _elementRectX - currentRect.x + 7 : 0f;
            float adjustedWidth = drawAreaWidth - xOffset;
            _cachedAdjustedWidth = adjustedWidth;

            float headerY = currentRect.y + drawAreaHeight + 2f;
            float headerHeight = drawAreaHeight + 6f;
            Rect rowHeaderRect = new(currentRect.x, headerY, drawAreaWidth, headerHeight);
            Rect allDrawHeaderAreaRect = new(currentRect.x + xOffset, rowHeaderRect.y, adjustedWidth, rowHeaderRect.height);
            _headerColumnRectList = SplitColumn(allDrawHeaderAreaRect);

            DrawRowHeaderBackground(rowHeaderRect);
            DrawRowHeaderSeparator(rowHeaderRect);
            DrawRowHeaderBottomLine(currentRect, rowHeaderRect, drawAreaWidth);
            DrawRowHeaderText();

            // セパレーターのヒット領域（視覚幅より広くして掴みやすく）
            float separator1HitX = _headerColumnRectList[0].xMax - SEPARATOR_HIT_PAD;
            float separator1HitWidth = SEPARATOR_WIDTH + SEPARATOR_HIT_PAD * 2;
            Rect separator1HitRect = new(separator1HitX, rowHeaderRect.y, separator1HitWidth, rowHeaderRect.height);
            
            float separator2HitX = _headerColumnRectList[1].xMax - SEPARATOR_HIT_PAD;
            float separator2HitWidth = SEPARATOR_WIDTH + SEPARATOR_HIT_PAD * 2;
            Rect separator2HitRect = new(separator2HitX, rowHeaderRect.y, separator2HitWidth, rowHeaderRect.height);

            EditorGUIUtility.AddCursorRect(separator1HitRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(separator2HitRect, MouseCursor.ResizeHorizontal);

            Event currentEvent = Event.current;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (currentEvent.type)
            {
                case EventType.MouseDown when separator1HitRect.Contains(currentEvent.mousePosition) || separator2HitRect.Contains(currentEvent.mousePosition):
                {
                    _resizingSeparatorIndex = separator1HitRect.Contains(currentEvent.mousePosition) ? 0 : 1;
                    _resizeDragStartX = currentEvent.mousePosition.x;
                    _resizeStartColumnWidth = _resizingSeparatorIndex == 0 ? _resizeColumn1Width : _resizeColumn3Width;
                   
                    GUIUtility.hotControl = controlId;
                    currentEvent.Use();
                    break;
                }
                case EventType.MouseDrag when _resizingSeparatorIndex >= 0:
                {
                    float positionDelta = currentEvent.mousePosition.x - _resizeDragStartX;
                    if (_resizingSeparatorIndex == 0)
                    {
                        float availableWidth = _cachedAdjustedWidth - _resizeColumn3Width - SEPARATOR_WIDTH * 2;
                        float resizeWidth = _resizeStartColumnWidth + positionDelta;
                        float maxWidth = availableWidth - MIN_COLUMN_WIDTH;
                        _resizeColumn1Width = Mathf.Clamp(resizeWidth, MIN_COLUMN_WIDTH, maxWidth);
                    }
                    else
                    {
                        float maxWidth = _cachedAdjustedWidth - Mathf.Max(_resizeColumn1Width, MIN_COLUMN_WIDTH) - MIN_COLUMN_WIDTH - SEPARATOR_WIDTH * 2;
                        _resizeColumn3Width = Mathf.Clamp(_resizeStartColumnWidth - positionDelta, MIN_COLUMN_WIDTH, maxWidth);
                    }

                    currentEvent.Use();
                    break;
                }
                case EventType.MouseUp when _resizingSeparatorIndex >= 0:
                {
                    _resizingSeparatorIndex = -1;
                    if (GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                    }

                    currentEvent.Use();
                    break;
                }
            }
        }

        private void DrawRowHeaderText()
        {
            EditorGUI.LabelField(_headerColumnRectList[0], "Scene Type Name", ColumnHeaderStyle);
            EditorGUI.LabelField(_headerColumnRectList[1], "Scene Address", ColumnHeaderStyle);
            EditorGUI.LabelField(_headerColumnRectList[2], "Development Only", ColumnHeaderStyle);
        }

        private Rect[] SplitColumn(Rect allDrawAreaRect)
        {
            Rect[] splitColumnRectList = new Rect[3];

            float column1MaxWidth = allDrawAreaRect.width - _resizeColumn3Width - MIN_COLUMN_WIDTH - SEPARATOR_WIDTH * 2;
            float column1Width = Mathf.Clamp(_resizeColumn1Width, MIN_COLUMN_WIDTH, column1MaxWidth);
            splitColumnRectList[0] = new Rect(allDrawAreaRect.x, allDrawAreaRect.y, column1Width, allDrawAreaRect.height);

            float column2X = allDrawAreaRect.x + column1Width + SEPARATOR_WIDTH;
            float column2Width = allDrawAreaRect.width - column1Width - _resizeColumn3Width - SEPARATOR_WIDTH * 2;
            splitColumnRectList[1] = new Rect(column2X, allDrawAreaRect.y, column2Width, allDrawAreaRect.height);

            float column3X = allDrawAreaRect.x + column1Width + column2Width + SEPARATOR_WIDTH * 2;
            splitColumnRectList[2] = new Rect(column3X, allDrawAreaRect.y, _resizeColumn3Width, allDrawAreaRect.height);

            return splitColumnRectList;
        }

        private void DrawRowHeaderBackground(Rect rowHeaderRect)
        {
            EditorGUI.DrawRect(rowHeaderRect, HeaderBackgroundColor);
        }

        private void DrawRowHeaderSeparator(Rect rowHeaderRect)
        {
            float separatorX = _headerColumnRectList[0].xMax;
            float separatorHeight = rowHeaderRect.height;
            Rect separatorRect = new(separatorX, rowHeaderRect.y, SEPARATOR_WIDTH, separatorHeight);
            EditorGUI.DrawRect(separatorRect, SeparatorColor);

            separatorX = _headerColumnRectList[1].xMax;
            separatorRect = new(separatorX, rowHeaderRect.y, SEPARATOR_WIDTH, separatorHeight);
            EditorGUI.DrawRect(separatorRect, SeparatorColor);
        }

        private void DrawRowHeaderBottomLine(Rect currentRect, Rect rowHeaderRect, float drawAreaWidth)
        {
            float lineHeight = 1f;
            Rect lineRect = new(currentRect.x, rowHeaderRect.yMax, drawAreaWidth, lineHeight);
            EditorGUI.DrawRect(lineRect, SeparatorColor);
        }

        private void DrawRowSeparator(Rect currentRect)
        {
            Rect separatorRect = new(_elementColumnRectList[0].xMax, currentRect.y, SEPARATOR_WIDTH, currentRect.height);
            EditorGUI.DrawRect(separatorRect, SeparatorColor);

            separatorRect.x = _elementColumnRectList[1].xMax;
            EditorGUI.DrawRect(separatorRect, SeparatorColor);
        }
    }
}