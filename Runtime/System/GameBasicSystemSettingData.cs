using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.System
{
    public sealed class GameBasicSystemSettingData : ScriptableObject
    {
        private static GameBasicSystemSettingData _instance;

        [SerializeField]
        private string _addressableAssetRootDirectory;

        [SerializeField]
        private List<PlayerBuildTaskData> _playerBuildTaskDataList;

        [SerializeField]
        private BootSceneAddressData _bootSceneData;

        [SerializeField]
        private List<ModalAddressData> _modalAddressDataList;

        [SerializeField]
        private List<SceneAddressData> _sceneAddressDataList;

        public static GameBasicSystemSettingData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameBasicSystemSettingData>(nameof(GameBasicSystemSettingData));
                }

                return _instance;
            }
        }

        public string AddressableAssetRootDirectory => _addressableAssetRootDirectory;

        public List<PlayerBuildTaskData> PlayerBuildTaskDataList
        {
            get
            {
                _playerBuildTaskDataList ??= new List<PlayerBuildTaskData>();
                return _playerBuildTaskDataList;
            }
        }
        
        public BootSceneAddressData BootSceneData => _bootSceneData;

        public List<ModalAddressData> ModalAddressDataList
        {
            get
            {
                _modalAddressDataList ??= new List<ModalAddressData>();
                return _modalAddressDataList;
            }
        }

        public List<SceneAddressData> SceneAddressDataList
        {
            get
            {
                _sceneAddressDataList ??= new List<SceneAddressData>();
                return _sceneAddressDataList;
            }
        }

        public List<string> ModalNameList
        {
            get
            {
                if (_modalAddressDataList == null || _modalAddressDataList.Count == 0)
                {
                    return null;
                }

                return _modalAddressDataList.Select(data => data.ModalName).ToList();
            }
        }

        public void SetAddressableAssetRootDirectory(string path)
        {
            _addressableAssetRootDirectory = path;
        }

        public string GetModalAddress(string modalName)
        {
            if (_modalAddressDataList == null)
            {
                return null;
            }

            ModalAddressData data = _modalAddressDataList.FirstOrDefault(data => data.ModalName == modalName);
            if (data == null || data == default)
            {
                return null;
            }

            return data.ModalAddress;
        }

        public string GetSceneAddress(string sceneTypeName)
        {
            if (_sceneAddressDataList == null)
            {
                return null;
            }

            SceneAddressData data = _sceneAddressDataList.FirstOrDefault(data => data.SceneTypeName == sceneTypeName);
            if (data == null || data == default)
            {
                return null;
            }

            return data.SceneAddress;
        }

        [Serializable]
        public class PlayerBuildTaskData
        {
            public bool IsEnabled;

            [SerializeField]
            private string _taskClassTypeName;
            private Type _taskClassType;

            public Type TaskClassType
            {
                get
                {
                    if (_taskClassType == null)
                    {
                        if (string.IsNullOrEmpty(_taskClassTypeName))
                        {
                            return null;
                        }

                        _taskClassType = Type.GetType(_taskClassTypeName);
                    }

                    return _taskClassType;
                }
                set
                {
                    _taskClassTypeName = value.AssemblyQualifiedName;
                    _taskClassType = null;
                }
            }
        }

        [Serializable]
        public class BootSceneAddressData
        {
            public string DevelopmentSceneAddress;
            public string ProductionSceneAddress;
        }

        [Serializable]
        public class ModalAddressData
        {
            public string ModalName;
            public string ModalAddress;
        }

        [Serializable]
        public class SceneAddressData
        {
            public string SceneTypeName;
            public string SceneAddress;
            public bool IsEditorOnly;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GameBasicSystemSettingData))]
    public class GameBasicSystemSettingDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            using (new UnityEditor.EditorGUI.DisabledScope(true))
            {
                base.OnInspectorGUI();
            }
        }
    }
#endif
}