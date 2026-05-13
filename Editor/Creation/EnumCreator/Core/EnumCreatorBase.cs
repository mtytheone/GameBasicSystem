using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Creation.EnumCreator.Core
{
    public class EnumCreatorBase
    {
        /// <returns>
        /// <para>true: ファイルの作成に成功</para>
        /// <para>false: ファイルの作成に失敗</para>
        /// </returns>
        internal bool CreateEnum()
        {
            if (!GameBasicSystemSettingData.Instance)
            {
                return false;
            }

            try
            {
                List<string> outputCodeStringList = new();
                outputCodeStringList.Add($"// {GetCreatorFileName()}.cs による自動生成");
                outputCodeStringList.Add($"namespace {GetNameSpace()}");
                outputCodeStringList.Add("{");
                outputCodeStringList.Add($"\tpublic enum {GetEnumName()}");
                outputCodeStringList.Add("\t{");

                WriteData(outputCodeStringList);

                outputCodeStringList.Add("\t}");
                outputCodeStringList.Add("}");

                string filePath = Path.Combine(Application.dataPath, GetFileRootPath(), GetFileName());
                string fileDirectory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    using (StreamWriter streamWriter = new(fileStream, Encoding.UTF8))
                    {
                        foreach (string codeString in outputCodeStringList)
                        {
                            streamWriter.WriteLine(codeString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return false;
            }

            AssetDatabase.Refresh();
            return true;
        }

        protected virtual void WriteData(List<string> outputStringList)
        {

        }

        protected virtual string GetFileName()
        {
            return string.Empty;
        }

        protected virtual string GetFileRootPath()
        {
            return string.Empty;
        }

        protected virtual string GetCreatorFileName()
        {
            return string.Empty;
        }

        protected virtual string GetNameSpace()
        {
            return string.Empty;
        }

        protected virtual string GetEnumName()
        {
            return string.Empty;
        }
    }
}
