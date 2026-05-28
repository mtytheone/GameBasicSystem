using HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Drawer
{
    public class SaveDataEncryptionKeyDrawer : ISettingProviderDrawer
    {
        private const string LABEL_TEXT = "Save Data Encryption Key (32 bytes)";
        private bool _showKey = false;

        string ISettingProviderDrawer.SectionTitle => "Save Data";

        void ISettingProviderDrawer.Draw()
        {
            GameBasicSystemSettingData settingData = GameBasicSystemSettingData.Instance;

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            string newKey = "";
            using (new EditorGUILayout.HorizontalScope())
            {
                string currentKey = settingData.SaveDataEncryptionKey ?? "";
                newKey = _showKey
                    ? EditorGUILayout.TextField(LABEL_TEXT, currentKey)
                    : EditorGUILayout.PasswordField(LABEL_TEXT, currentKey);

                _showKey = GUILayout.Toggle(_showKey, _showKey ? "Hide" : "Show", "Button", GUILayout.Width(50));
            }

            EditorGUIUtility.labelWidth = labelWidth;
            if (!string.IsNullOrEmpty(newKey))
            {
                int byteLength = Encoding.UTF8.GetByteCount(newKey);
                if (byteLength != 32)
                {
                    EditorGUILayout.HelpBox(
                        $"Key must be exactly 32 bytes in UTF-8 (current: {byteLength} bytes).",
                        MessageType.Warning);
                }
            }

            settingData.SetSaveDataEncryptionKey(newKey);
        }
    }
}