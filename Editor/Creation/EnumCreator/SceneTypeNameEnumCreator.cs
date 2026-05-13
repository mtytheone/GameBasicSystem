using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator.Core;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator
{
    public class SceneTypeNameEnumCreator : EnumCreatorBase
    {
        private const string FILE_NAME = "SceneType.cs";
        private const string FILE_PATH = "HatzeLaboratory/Scripts/Scene";

        /// <summary>
        /// SceneType.csを作成
        /// </summary>
        /// <returns>
        /// <para>true: ファイルの作成に成功</para>
        /// <para>false: ファイルの作成に失敗</para>
        /// </returns>
        internal bool CreateModalNameEnum()
        {
            return CreateEnum();
        }

        protected override void WriteData(List<string> outputStringList)
        {
            List<GameBasicSystemSettingData.SceneAddressData> sceneAddressDataList = GameBasicSystemSettingData.Instance.SceneAddressDataList;
            foreach (GameBasicSystemSettingData.SceneAddressData sceneAddressData in sceneAddressDataList)
            {
                string sceneTypeName = sceneAddressData.SceneTypeName;
                if (string.IsNullOrEmpty(sceneTypeName))
                {
                    continue;
                }

                if (sceneAddressData.IsEditorOnly)
                {
                    continue;
                }

                if (sceneTypeName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                {
                    continue;
                }

                outputStringList.Add($"\t\t{sceneTypeName},");
            }

            outputStringList.Add($"\n#if DEBUG");
            foreach (GameBasicSystemSettingData.SceneAddressData sceneAddressData in sceneAddressDataList)
            {
                string sceneTypeName = sceneAddressData.SceneTypeName;
                if (string.IsNullOrEmpty(sceneTypeName))
                {
                    continue;
                }

                if (!sceneAddressData.IsEditorOnly)
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
            return "SceneType";
        }
    }
}
