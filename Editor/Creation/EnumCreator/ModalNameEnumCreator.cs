using HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator.Core;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator
{
    public class ModalNameEnumCreator : EnumCreatorBase
    {
        private const string FILE_NAME = "ModalType.cs";
        private const string FILE_PATH = "HatzeLaboratory/Scripts/Modal";

        protected override void WriteData(List<string> outputStringList)
        {
            if (!GameBasicSystemSettingData.Instance)
            {
                return;
            }

            List<GameBasicSystemSettingData.ModalAddressData> modalAddressDataList = GameBasicSystemSettingData.Instance.ModalAddressDataList;
            foreach (GameBasicSystemSettingData.ModalAddressData modalAddressData in modalAddressDataList)
            {
                string modalName = modalAddressData.ModalName;
                if (string.IsNullOrEmpty(modalName))
                {
                    continue;
                }

                if (modalName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                {
                    continue;
                }

                outputStringList.Add($"\t\t{modalName},");
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
            return nameof(ModalNameEnumCreator);
        }

        protected override string GetNameSpace()
        {
            return "HatzeLaboratory.Scripts.Modal";
        }

        protected override string GetEnumName()
        {
            return Path.GetFileNameWithoutExtension(FILE_NAME);
        }

        [InitializeOnLoadMethod]
        private static void CheckEnumFileExistence()
        {
            ModalNameEnumCreator enumCreator = new();
            if (!enumCreator.IsEnumExisted())
            {
                enumCreator.CreateEnum();
            }
        }
    }
}
