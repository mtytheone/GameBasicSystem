using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator.Core;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.InputSystem;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator
{
    public class ActionMapNameCreator : EnumCreatorBase
    {
        private const string FILE_NAME = "ActionMapType.cs";
        private const string FILE_PATH = "HatzeLaboratory/Scripts/ActionMap";

        protected override void WriteData(List<string> outputStringList)
        {
            if (!GameBasicSystemSettingData.Instance)
            {
                return;
            }

            InputActionAsset inputActionAsset = GameBasicSystemSettingData.Instance.InputActionAsset;
            if (!inputActionAsset)
            {
                return;
            }

            foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
            {
                if (string.IsNullOrEmpty(actionMap.name))
                {
                    continue;
                }

                outputStringList.Add($"\t\t{actionMap.name},");
            }
        }

        protected override string GetFileName()
        {
            return FILE_NAME;
        }

        protected override string GetFileRootPath()
        {
            return FILE_PATH;
        }

        protected override string GetCreatorFileName()
        {
            return nameof(ActionMapNameCreator);
        }

        protected override string GetNameSpace()
        {
            return "HatzeLaboratory.Scripts.ActionMap";
        }

        protected override string GetEnumName()
        {
            return Path.GetFileNameWithoutExtension(FILE_NAME);
        }

        [InitializeOnLoadMethod]
        private static void CheckEnumFileExistence()
        {
            ActionMapNameCreator enumCreator = new();
            if (!enumCreator.IsEnumExisted())
            {
                enumCreator.CreateEnum();
            }
        }
    }
}
