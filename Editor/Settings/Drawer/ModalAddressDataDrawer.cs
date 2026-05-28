using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator;
using HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Drawer
{
    public class ModalAddressDataDrawer : ISettingProviderDrawer
    {
        private const string LIST_HEADER_NAME = "Modal Address List";
        private const string APPLY_BUTTON_TEXT = "Apply";
        private const float APPLY_BUTTON_WIDTH = 60f;
        private const float MIN_COLUMN_WIDTH = 120f;
        private const float SEPARATOR_WIDTH = 2f;
        private const float SEPARATOR_HIT_PAD = 4f;

        private GUIStyle _columnHeaderStyle;
        private Rect[] _headerColumnRectList;
        private Rect[] _elementColumnRectList;
        private float _resizeColumn1Width = MIN_COLUMN_WIDTH;

        private int _resizingSeparatorIndex = -1;
        private float _resizeDragStartX;
        private float _resizeStartColumnWidth;
        private float _elementRectX = -1f;
        private float _cachedAdjustedWidth = -1f;

        private readonly ReorderableList _reorderableList;
        private readonly ModalNameEnumCreator _modalNameCreator = new();

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
        private List<GameBasicSystemSettingData.ModalAddressData> ModalAddressDataList => SettingData.ModalAddressDataList;

        string ISettingProviderDrawer.SectionTitle => "Modal Address Setting";

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
                    Rect elementRowRect = new(elementAreaRect.x, elementAreaRect.y + 2f, elementAreaRect.width, EditorGUIUtility.singleLineHeight);
                    _elementColumnRectList = SplitColumn(elementRowRect);
                    DrawRowSeparator(elementAreaRect);
                    DrawRowElement(index);
                },
            };
        }

        private void DrawRowElement(int index)
        {
            GameBasicSystemSettingData.ModalAddressData data = ModalAddressDataList[index];

            const float separatorMargin = 6f;
            float modalNameTextFieldX = _elementColumnRectList[0].x + separatorMargin;
            float modalNameTextFieldWidth = _elementColumnRectList[0].width - separatorMargin * 2;
            Rect modalNameTextFieldRect = new(modalNameTextFieldX, _elementColumnRectList[0].y, modalNameTextFieldWidth, _elementColumnRectList[0].height);
            data.ModalName = EditorGUI.TextField(modalNameTextFieldRect, data.ModalName);

            float modalAddressTextFieldX = _elementColumnRectList[1].x + separatorMargin;
            float modalAddressTextFieldWidth = _elementColumnRectList[1].width - separatorMargin * 2;
            Rect modalAddressTextFieldRect = new(modalAddressTextFieldX, _elementColumnRectList[1].y, modalAddressTextFieldWidth, _elementColumnRectList[1].height);
            data.ModalAddress = EditorGUI.TextField(modalAddressTextFieldRect, data.ModalAddress);
        }

        private Rect DrawListHeader(Rect rect)
        {
            float drawAreaWidth = rect.width;
            float drawAreaHeight = EditorGUIUtility.singleLineHeight;

            float labelTextWidth = drawAreaWidth - APPLY_BUTTON_WIDTH - 4f;
            Rect labelTextRect = new(rect.x, rect.y, labelTextWidth, drawAreaHeight);
            EditorGUI.LabelField(labelTextRect, LIST_HEADER_NAME);

            bool hasPendingChanges = _modalNameCreator.HasPendingChanges();
            using (new EditorGUI.DisabledGroupScope(!hasPendingChanges))
            {
                float applyButtonX = rect.x + drawAreaWidth - APPLY_BUTTON_WIDTH;
                Rect applyButtonRect = new(applyButtonX, rect.y, APPLY_BUTTON_WIDTH, drawAreaHeight);
                if (GUI.Button(applyButtonRect, APPLY_BUTTON_TEXT, EditorStyles.miniButton))
                {
                    bool isSuccess = _modalNameCreator.CreateEnum();
                    if (!isSuccess)
                    {
                        EditorUtility.DisplayDialog("Create Modal Enum", "Failed to create modal name enum.", "OK");
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

            float separator1HitX = _headerColumnRectList[0].xMax - SEPARATOR_HIT_PAD;
            float separator1HitWidth = SEPARATOR_WIDTH + SEPARATOR_HIT_PAD * 2;
            Rect separator1HitRect = new(separator1HitX, rowHeaderRect.y, separator1HitWidth, rowHeaderRect.height);

            EditorGUIUtility.AddCursorRect(separator1HitRect, MouseCursor.ResizeHorizontal);

            Event currentEvent = Event.current;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (currentEvent.type)
            {
                case EventType.MouseDown when separator1HitRect.Contains(currentEvent.mousePosition):
                {
                    _resizingSeparatorIndex = 0;
                    _resizeDragStartX = currentEvent.mousePosition.x;
                    _resizeStartColumnWidth = _resizeColumn1Width;
                    GUIUtility.hotControl = controlId;
                    currentEvent.Use();
                    break;
                }
                case EventType.MouseDrag when _resizingSeparatorIndex >= 0:
                {
                    float positionDelta = currentEvent.mousePosition.x - _resizeDragStartX;
                    float maxWidth = _cachedAdjustedWidth - MIN_COLUMN_WIDTH - SEPARATOR_WIDTH;
                    _resizeColumn1Width = Mathf.Clamp(_resizeStartColumnWidth + positionDelta, MIN_COLUMN_WIDTH, maxWidth);
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
            EditorGUI.LabelField(_headerColumnRectList[0], "Modal Name", ColumnHeaderStyle);
            EditorGUI.LabelField(_headerColumnRectList[1], "Modal Address", ColumnHeaderStyle);
        }

        private Rect[] SplitColumn(Rect allDrawAreaRect)
        {
            Rect[] splitColumnRectList = new Rect[2];

            float column1MaxWidth = allDrawAreaRect.width - MIN_COLUMN_WIDTH - SEPARATOR_WIDTH;
            float column1Width = Mathf.Clamp(_resizeColumn1Width, MIN_COLUMN_WIDTH, column1MaxWidth);
            splitColumnRectList[0] = new Rect(allDrawAreaRect.x, allDrawAreaRect.y, column1Width, allDrawAreaRect.height);

            float column2X = allDrawAreaRect.x + column1Width + SEPARATOR_WIDTH;
            float column2Width = allDrawAreaRect.width - column1Width - SEPARATOR_WIDTH;
            splitColumnRectList[1] = new Rect(column2X, allDrawAreaRect.y, column2Width, allDrawAreaRect.height);

            return splitColumnRectList;
        }

        private void DrawRowHeaderBackground(Rect rowHeaderRect)
        {
            EditorGUI.DrawRect(rowHeaderRect, HeaderBackgroundColor);
        }

        private void DrawRowHeaderSeparator(Rect rowHeaderRect)
        {
            float separatorX = _headerColumnRectList[0].xMax;
            Rect separatorRect = new(separatorX, rowHeaderRect.y, SEPARATOR_WIDTH, rowHeaderRect.height);
            EditorGUI.DrawRect(separatorRect, SeparatorColor);
        }

        private void DrawRowHeaderBottomLine(Rect currentRect, Rect rowHeaderRect, float drawAreaWidth)
        {
            Rect lineRect = new(currentRect.x, rowHeaderRect.yMax, drawAreaWidth, 1f);
            EditorGUI.DrawRect(lineRect, SeparatorColor);
        }

        private void DrawRowSeparator(Rect currentRect)
        {
            Rect separatorRect = new(_elementColumnRectList[0].xMax, currentRect.y, SEPARATOR_WIDTH, currentRect.height);
            EditorGUI.DrawRect(separatorRect, SeparatorColor);
        }
    }
}