using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator.Core;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator
{
    public class SceneTypeNameEnumCreator : EnumCreatorBase
    {
        private const string FILE_NAME = "SceneType.cs";
        private const string FILE_PATH = "HatzeLaboratory/Scripts/Scene";

        protected override void WriteData(List<string> outputStringList)
        {
            if (!GameBasicSystemSettingData.Instance)
            {
                return;
            }

            List<GameBasicSystemSettingData.SceneAddressData> sceneAddressDataList = GameBasicSystemSettingData.Instance.SceneAddressDataList;
            foreach (GameBasicSystemSettingData.SceneAddressData sceneAddressData in sceneAddressDataList)
            {
                string sceneTypeName = sceneAddressData.SceneTypeName;
                if (string.IsNullOrEmpty(sceneTypeName))
                {
                    continue;
                }

                if (sceneAddressData.IsDevelopmentOnly)
                {
                    continue;
                }

                if (sceneTypeName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                {
                    continue;
                }

                outputStringList.Add($"\t\t{sceneTypeName},");
            }

            outputStringList.Add(string.Empty);
            outputStringList.Add("#if DEBUG");
            foreach (GameBasicSystemSettingData.SceneAddressData sceneAddressData in sceneAddressDataList)
            {
                string sceneTypeName = sceneAddressData.SceneTypeName;
                if (string.IsNullOrEmpty(sceneTypeName))
                {
                    continue;
                }

                if (!sceneAddressData.IsDevelopmentOnly)
                {
                    continue;
                }

                if (sceneTypeName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                {
                    continue;
                }

                outputStringList.Add($"\t\t{sceneTypeName},");
            }
            
            outputStringList.Add("#endif");
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
            return nameof(SceneTypeNameEnumCreator);
        }

        protected override string GetNameSpace()
        {
            return "HatzeLaboratory.Scripts.Scene";
        }

        protected override string GetEnumName()
        {
            return Path.GetFileNameWithoutExtension(FILE_NAME);
        }

        [InitializeOnLoadMethod]
        private static void CheckEnumFileExistence()
        {
            SceneTypeNameEnumCreator enumCreator = new();
            if (!enumCreator.IsEnumExisted())
            {
                enumCreator.CreateEnum();
            }
        }
    }
}
