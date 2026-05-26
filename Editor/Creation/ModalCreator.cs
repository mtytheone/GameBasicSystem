using HatzeLaboratory.GameBasicSystem.Runtime.UI;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation
{
    public sealed class ModalCreator
    {
        private const float BASIC_MODAL_ITEM_VERTICAL_SPACING = 15.5f;
        private const float BASIC_MODAL_TITLE_TEXT_SIZE = 70.2f;
        private const float BASIC_MODAL_BUTTON_HORIZONTAL_SPACING = 50.0f;
        private const float BUTTON_TEXT_SIZE = 34.0f;
        private static readonly Vector2 BASIC_MODAL_SIZE = new(1536, 864);
        private static readonly Vector2 BASIC_MODAL_TEXT_AREA_SIZE = new(1000, 100);
        private static readonly Color BASIC_MODAL_TITLE_TEXT_COLOR = Color.black;
        private static readonly Vector2 BASIC_MODAL_IMAGE_SIZE = new(768, 432);
        private static readonly Vector2 BASIC_MODAL_DESCRIPTION_SIZE = new(1400, 150);
        private static readonly Color BASIC_MODAL_DESCRIPTION_TEXT_COLOR = Color.black;
        private static readonly Vector2 BASIC_MODAL_BUTTON_ROOT_SIZE = new(750, 80);
        private static readonly Vector2 BASIC_MODAL_BUTTON_SIZE = new(320, 80);
        private static readonly Color BASIC_MODAL_BUTTON_TEXT_COLOR = new(0.1960784f, 0.1960784f, 0.1960784f, 1);

        private static readonly Vector2 LOADING_MODAL_BACKGROUND_IMAGE_SIZE = new(2304, 1296);
        private static readonly Color LOADING_MODAL_BACKGROUND_COLOR = Color.black;
        private static readonly Vector2 LOADING_MODAL_LOADING_TEXT_POSITION = new(960, -540);
        private static readonly Vector2 LOADING_MODAL_LOADING_TEXT_AREA_SIZE = new(600, 150);

        [MenuItem("Assets/Create/HatzeLab/UI/Create BasicModal", false, -297)]
        public static void CreateBasicModal()
        {
            GameObject modalObject = CreateBaseModal(nameof(BasicModal));
            modalObject.AddComponent<GraphicRaycaster>();
            Transform canvasGroupTransform = modalObject.transform.GetChild(0);

            GameObject backgroundObject = new("Background");
            backgroundObject.transform.SetParent(canvasGroupTransform, false);
            RectTransform backgroundTransform = backgroundObject.AddComponent<RectTransform>();
            backgroundTransform.sizeDelta = BASIC_MODAL_SIZE;
            backgroundObject.AddComponent<CanvasRenderer>();
            backgroundObject.AddComponent<Image>();

            GameObject itemRootObject = new("ItemRoot");
            itemRootObject.transform.SetParent(canvasGroupTransform, false);
            RectTransform itemRootTransform = itemRootObject.AddComponent<RectTransform>();
            itemRootTransform.sizeDelta = BASIC_MODAL_SIZE;
            VerticalLayoutGroup verticalLayoutGroup = itemRootObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = BASIC_MODAL_ITEM_VERTICAL_SPACING;
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;

            GameObject titleTextObject = new("Title");
            titleTextObject.transform.SetParent(itemRootTransform, false);
            RectTransform titleTextTransform = titleTextObject.AddComponent<RectTransform>();
            titleTextTransform.sizeDelta = BASIC_MODAL_TEXT_AREA_SIZE;
            titleTextObject.AddComponent<CanvasRenderer>();
            TextMeshProUGUI titleText = titleTextObject.AddComponent<TextMeshProUGUI>();
            titleText.text = "Sample Title Text";
            titleText.fontStyle = FontStyles.Bold;
            titleText.fontSize = BASIC_MODAL_TITLE_TEXT_SIZE;
            titleText.color = BASIC_MODAL_TITLE_TEXT_COLOR;
            titleText.verticalAlignment = VerticalAlignmentOptions.Middle;
            titleText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            GameObject imageObject = new("Image");
            imageObject.transform.SetParent(itemRootTransform, false);
            RectTransform imageTransform = imageObject.AddComponent<RectTransform>();
            imageTransform.sizeDelta = BASIC_MODAL_IMAGE_SIZE;
            imageObject.AddComponent<CanvasRenderer>();
            imageObject.AddComponent<Image>();

            GameObject descriptionTextObject = new("Description");
            descriptionTextObject.transform.SetParent(itemRootTransform, false);
            RectTransform descriptionTextTransform = descriptionTextObject.AddComponent<RectTransform>();
            descriptionTextTransform.sizeDelta = BASIC_MODAL_DESCRIPTION_SIZE;
            descriptionTextObject.AddComponent<CanvasRenderer>();
            TextMeshProUGUI descriptionText = descriptionTextObject.AddComponent<TextMeshProUGUI>();
            descriptionText.text = "This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.This is sample text.";
            descriptionText.color = BASIC_MODAL_DESCRIPTION_TEXT_COLOR;
            descriptionText.verticalAlignment = VerticalAlignmentOptions.Middle;
            descriptionText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            GameObject buttonRootObject = new("ButtonRoot");
            buttonRootObject.transform.SetParent(itemRootTransform, false);
            RectTransform buttonRootTransform = buttonRootObject.AddComponent<RectTransform>();
            buttonRootTransform.sizeDelta = BASIC_MODAL_BUTTON_ROOT_SIZE;
            HorizontalLayoutGroup horizontalLayoutGroup = buttonRootObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = BASIC_MODAL_BUTTON_HORIZONTAL_SPACING;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandHeight = false;

            GameObject leftButtonTextObject = CreateButton("LeftButton", buttonRootTransform);
            GameObject rightButtonTextObject = CreateButton("RightButton", buttonRootTransform);

            BasicModal basicModal = modalObject.AddComponent<BasicModal>();
            SerializedObject serializedObject = new(basicModal);
            serializedObject.FindProperty("_canvasGroup").objectReferenceValue = canvasGroupTransform.GetComponent<CanvasGroup>();
            serializedObject.FindProperty("_titleText").objectReferenceValue = titleText;
            serializedObject.FindProperty("_image").objectReferenceValue = imageObject.GetComponent<Image>();
            serializedObject.FindProperty("_text").objectReferenceValue = descriptionText;
            serializedObject.FindProperty("_buttonRoot").objectReferenceValue = buttonRootTransform;
            serializedObject.FindProperty("_leftButton").objectReferenceValue = leftButtonTextObject.GetComponent<Button>();
            serializedObject.FindProperty("_rightButton").objectReferenceValue = rightButtonTextObject.GetComponent<Button>();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            SetUILayer(modalObject);
            SavePrefab(modalObject);
        }

        [MenuItem("Assets/Create/HatzeLab/UI/Create LoadingModal", false, -296)]
        public static void CreateLoadingModal()
        {
            GameObject modalObject = CreateBaseModal(nameof(LoadingModal));
            Transform canvasGroupTransform = modalObject.transform.GetChild(0);

            GameObject backgroundObject = new("Background");
            backgroundObject.transform.SetParent(canvasGroupTransform, false);
            RectTransform backgroundTransform = backgroundObject.AddComponent<RectTransform>();
            backgroundTransform.sizeDelta = LOADING_MODAL_BACKGROUND_IMAGE_SIZE;
            backgroundObject.AddComponent<CanvasRenderer>();
            Image backgroundImage = backgroundObject.AddComponent<Image>();
            backgroundImage.color = LOADING_MODAL_BACKGROUND_COLOR;

            GameObject loadingTextObject = new("LoadingText");
            loadingTextObject.transform.SetParent(canvasGroupTransform, false);
            RectTransform loadingTextTransform = loadingTextObject.AddComponent<RectTransform>();
            loadingTextTransform.anchorMin = new Vector2(1, 0);
            loadingTextTransform.anchorMax = new Vector2(1, 0);
            loadingTextTransform.pivot = new Vector2(1, 0);
            loadingTextTransform.anchoredPosition = LOADING_MODAL_LOADING_TEXT_POSITION;
            loadingTextTransform.sizeDelta = LOADING_MODAL_LOADING_TEXT_AREA_SIZE;
            loadingTextObject.AddComponent<CanvasRenderer>();
            TextMeshProUGUI loadingText = loadingTextObject.AddComponent<TextMeshProUGUI>();
            loadingText.text = "Loading...";
            loadingText.enableAutoSizing = true;
            loadingText.verticalAlignment = VerticalAlignmentOptions.Bottom;
            loadingText.horizontalAlignment = HorizontalAlignmentOptions.Right;

            LoadingModal loadingModal = modalObject.AddComponent<LoadingModal>();
            SerializedObject serializedObject = new(loadingModal);
            serializedObject.FindProperty("_canvasGroup").objectReferenceValue = canvasGroupTransform.GetComponent<CanvasGroup>();
            serializedObject.FindProperty("_loadingText").objectReferenceValue = loadingText;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            SetUILayer(modalObject);
            SavePrefab(modalObject);
        }

        private static GameObject CreateBaseModal(string objectName)
        {
            GameObject rootObject = new(objectName);
            RectTransform rootRectTransform = rootObject.AddComponent<RectTransform>();
            rootRectTransform.sizeDelta = Vector2.zero;
            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.anchorMax = Vector2.zero;
            rootRectTransform.pivot = Vector2.zero;
            Canvas canvas = rootObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
            canvas.vertexColorAlwaysGammaSpace = true;

            GameObject childObject = new("CanvasGroup");
            childObject.AddComponent<CanvasGroup>();
            childObject.transform.SetParent(rootObject.transform, false);

            return rootObject;
        }

        private static GameObject CreateButton(string objectName, Transform parent)
        {
            GameObject buttonObject = new(objectName);
            buttonObject.transform.SetParent(parent, false);
            RectTransform leftButtonTransform = buttonObject.AddComponent<RectTransform>();
            leftButtonTransform.sizeDelta = BASIC_MODAL_BUTTON_SIZE;
            buttonObject.AddComponent<CanvasRenderer>();

            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            buttonImage.type = Image.Type.Sliced;
            Button button = buttonObject.AddComponent<Button>();
            button.image = buttonImage;

            GameObject buttonTextObject = new($"{objectName}Text");
            buttonTextObject.transform.SetParent(leftButtonTransform, false);
            RectTransform leftButtonTextTransform = buttonTextObject.AddComponent<RectTransform>();
            leftButtonTextTransform.anchorMin = Vector2.zero;
            leftButtonTextTransform.anchorMax = Vector2.one;
            leftButtonTextTransform.pivot = Vector2.one * 0.5f;
            leftButtonTextTransform.anchoredPosition = Vector2.zero;
            leftButtonTextTransform.sizeDelta = Vector2.zero;
            buttonTextObject.AddComponent<CanvasRenderer>();
            TextMeshProUGUI leftButtonText = buttonTextObject.AddComponent<TextMeshProUGUI>();
            leftButtonText.text = "Button";
            leftButtonText.fontSize = BUTTON_TEXT_SIZE;
            leftButtonText.color = BASIC_MODAL_BUTTON_TEXT_COLOR;
            leftButtonText.verticalAlignment = VerticalAlignmentOptions.Middle;
            leftButtonText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            return buttonObject;
        }

        private static void SetUILayer(GameObject rootObject)
        {
            rootObject.layer = LayerMask.NameToLayer("UI");
            for (int i = 0; i < rootObject.transform.childCount; i++)
            {
                Transform childTransform = rootObject.transform.GetChild(i);
                SetUILayer(childTransform.gameObject);
            }
        }

        private static void SavePrefab(GameObject rootObject, string prefabName = "")
        {
            string currentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath = "Assets";
            }

            string absolutePath = Path.Combine(Application.dataPath.Replace("Assets", ""), currentPath);
            bool isDirectory = File.GetAttributes(absolutePath).HasFlag(FileAttributes.Directory);
            if (!isDirectory)
            {
                currentPath = Path.GetDirectoryName(currentPath);
            }

            if (string.IsNullOrEmpty(prefabName))
            {
                prefabName = rootObject.name;
            }

            string outputPath = $"{currentPath}/{prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(rootObject, outputPath, out bool isSuccess);
            if (!isSuccess)
            {
                string message = "Failed to create GameObject.";
                EditorUtility.DisplayDialog("Create GameObject", message, "OK");
            }

            // Prefab作成後に作成したPrefabを選択状態にする
            Object prefabAsset = AssetDatabase.LoadAssetAtPath<Object>(outputPath);
            Selection.activeObject = prefabAsset;

            Object.DestroyImmediate(rootObject);
        }
    }
}