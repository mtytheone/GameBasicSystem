using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using HatzeLaboratory.GameBasicSystem.Editor.Define.Interface;
using HatzeLaboratory.GameBasicSystem.Runtime.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Editor.Define.Drawer
{
    public sealed class EnablePlayerBuildTaskDrawer : ISettingProviderDrawer
    {
        private const string LIST_HEADER_NAME = "Player Build Task List";

        private readonly ReorderableList _reorderableList;
        private GameBasicSystemSettingData SettingData => GameBasicSystemSettingData.Instance;
        private List<GameBasicSystemSettingData.PlayerBuildTaskData> TaskDataList => SettingData.PlayerBuildTaskDataList;

        void ISettingProviderDrawer.Draw()
        {
            _reorderableList.DoLayoutList();
        }

        public EnablePlayerBuildTaskDrawer()
        {
            CreateTaskDataList();
            _reorderableList = CreateReorderableList();
        }

        private void CreateTaskDataList()
        {
            if (TaskDataList == null)
            {
                Debug.LogError("TaskDataList is null.");
                return;
            }

            List<Type> foundClassTypeList = GetImplementClassList<IPlayerBuildTask>();
            if (TaskDataList.Count < 1)
            {
                var newTaskDataList = foundClassTypeList
                    .Select(type => new GameBasicSystemSettingData.PlayerBuildTaskData()
                    {
                        TaskClassType = type,
                        IsEnabled = true,
                    })
                    .ToList();

                foreach (GameBasicSystemSettingData.PlayerBuildTaskData data in newTaskDataList)
                {
                    TaskDataList.Add(data);
                }
            }
            else
            {
                var appendTaskDataList = foundClassTypeList
                    .Where(type => !TaskDataList.Select(data => data.TaskClassType).Contains(type))
                    .Select(type => new GameBasicSystemSettingData.PlayerBuildTaskData()
                    {
                        TaskClassType = type,
                        IsEnabled = true,
                    })
                    .ToList();

                var newTaskDataList = TaskDataList
                    .Where(data => foundClassTypeList.Contains(data.TaskClassType))
                    .Concat(appendTaskDataList)
                    .ToList();

                TaskDataList.Clear();
                foreach (GameBasicSystemSettingData.PlayerBuildTaskData data in newTaskDataList)
                {
                    TaskDataList.Add(data);
                }
            }
        }

        private ReorderableList CreateReorderableList()
        {
            return new(TaskDataList, typeof(Type))
            {
                draggable = true,
                drawHeaderCallback = rect =>
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(rect, LIST_HEADER_NAME);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += 2;

                    GameBasicSystemSettingData.PlayerBuildTaskData data = TaskDataList[index];
                    data.IsEnabled = EditorGUI.Toggle(rect, data.IsEnabled);
                    rect.x += 20;

                    Type type = data.TaskClassType;
                    EditorGUI.LabelField(rect, type != null ? type.Name : "null");
                },

                drawFooterCallback = rect =>
                {
                    // No footer needed
                },
            };
        }

        private List<Type> GetImplementClassList<TInterface>()
        {
            Type interfaceType = typeof(TInterface);
            List<Type> typeList = new();

            Assembly[] assembleList = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assembleList)
            {
                Type[] assemblyTypeList;
                try
                {
                    assemblyTypeList = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    assemblyTypeList = e.Types.Where(Type => Type != null).ToArray();
                }

                foreach (Type type in assemblyTypeList)
                {
                    if (type == null)
                    {
                        continue;
                    }

                    if (!type.IsClass)
                    {
                        continue;
                    }

                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    if (interfaceType.IsAssignableFrom(type))
                    {
                        typeList.Add(type);
                    }
                }
            }

            return typeList;
        }
    }
}