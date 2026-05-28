using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator;
using HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using UnityEditor;
using UnityEngine.InputSystem;

namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Drawer
{
    public class InputActionAssetDrawer : ISettingProviderDrawer
    {
        private readonly ActionMapNameCreator _actionMapNameCreator = new();

        string ISettingProviderDrawer.SectionTitle => "Input";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData data = GameBasicSystemSettingData.Instance;

            EditorGUI.BeginChangeCheck();
            InputActionAsset newInputActionAsset = EditorGUILayout.ObjectField("InputAction Asset", data.InputActionAsset, typeof(InputActionAsset), false) as InputActionAsset;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Change InputAction Asset");
                data.SetInputActionAsset(newInputActionAsset);
                EditorUtility.SetDirty(data);

                _actionMapNameCreator.CreateEnum();
            }
        }
    }
}