using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HatzeLaboratory.GameBasicSystem.Runtime.System
{
    /// <summary>
    /// GameBasicSystem パッケージ全体の設定データ。
    /// Project Settings > GameBasicSystem から設定し、<see cref="Instance"/> でアクセスします。
    /// </summary>
    public sealed class GameBasicSystemSettingData : ScriptableObject
    {
        private static GameBasicSystemSettingData _instance;

        [SerializeField]
        private string _addressableAssetRootDirectory;

        [SerializeField]
        private string _buildAddressableAssetSaveDirectory;

        [SerializeField]
        private List<PlayerBuildTaskData> _playerBuildTaskDataList;

        [SerializeField]
        private BootSceneAddressData _bootSceneData;

        [SerializeField]
        private List<ModalAddressData> _modalAddressDataList;

        [SerializeField]
        private List<SceneAddressData> _sceneAddressDataList;

        [SerializeField]
        private string _saveDataFileName = "SaveData.hld";

        [SerializeField]
        private string _saveDataEncryptionKey;

        [SerializeField]
        private InputActionAsset _inputActionAsset;

        [SerializeField]
        private bool _isAddressableProfileSetupDone;

        /// <summary>
        /// 設定データのシングルトンインスタンスを取得します。Resourcesフォルダから自動ロードされます。
        /// </summary>
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

        /// <summary>
        /// Addressablesアセットのルートディレクトリパスを取得します。
        /// </summary>
        public string AddressableAssetRootDirectory => _addressableAssetRootDirectory;

        /// <summary>
        /// ビルドされたAddressablesアセットの保存ディレクトリパスを取得します。
        /// </summary>
        public string BuildAddressableAssetSaveDirectory => _buildAddressableAssetSaveDirectory;

        /// <summary>
        /// プレイヤービルドタスクの設定リストを取得します。
        /// </summary>
        public List<PlayerBuildTaskData> PlayerBuildTaskDataList
        {
            get
            {
                _playerBuildTaskDataList ??= new List<PlayerBuildTaskData>();
                return _playerBuildTaskDataList;
            }
        }

        /// <summary>
        /// 起動シーンのアドレスデータを取得します。
        /// </summary>
        public BootSceneAddressData BootSceneData => _bootSceneData;

        /// <summary>
        /// モーダルのアドレスデータリストを取得します。
        /// </summary>
        public List<ModalAddressData> ModalAddressDataList
        {
            get
            {
                _modalAddressDataList ??= new List<ModalAddressData>();
                return _modalAddressDataList;
            }
        }

        /// <summary>
        /// シーンのアドレスデータリストを取得します。
        /// </summary>
        public List<SceneAddressData> SceneAddressDataList
        {
            get
            {
                _sceneAddressDataList ??= new List<SceneAddressData>();
                return _sceneAddressDataList;
            }
        }

        /// <summary>
        /// 登録されているモーダル名のリストを取得します。
        /// </summary>
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

        /// <summary>
        /// セーブデータのファイル名を取得します。未設定の場合は "SaveData.hld"。
        /// </summary>
        public string SaveDataFileName => string.IsNullOrWhiteSpace(_saveDataFileName) 
                                        ? "SaveData.hld" 
                                        : _saveDataFileName;

        /// <summary>
        /// セーブデータの暗号化キーを取得します。
        /// </summary>
        public string SaveDataEncryptionKey => _saveDataEncryptionKey;

        /// <summary>
        /// 使用するInputActionAssetを取得します。
        /// </summary>
        public InputActionAsset InputActionAsset => _inputActionAsset;

        /// <summary>
        /// Addressablesプロファイルのセットアップが完了しているかどうかを取得します。
        /// </summary>
        public bool IsAddressableProfileSetupDone => _isAddressableProfileSetupDone;

        /// <summary>
        /// Addressables アセットのルートディレクトリパスを設定します。
        /// </summary>
        /// <param name="path">設定するパス</param>
        public void SetAddressableAssetRootDirectory(string path)
        {
            _addressableAssetRootDirectory = path;
        }

        /// <summary>
        /// ビルドされたAddressablesアセットの保存ディレクトリパスを設定します。
        /// </summary>
        /// <param name="path">設定するパス</param>
        public void SetBuildAddressableAssetSaveDirectory(string path)
        {
            _buildAddressableAssetSaveDirectory = path;
        }

        /// <summary>
        /// セーブデータのファイル名を設定します。
        /// </summary>
        /// <param name="fileName">設定するファイル名</param>
        public void SetSaveDataFileName(string fileName)
        {
            _saveDataFileName = fileName;
        }

        /// <summary>
        /// セーブデータの暗号化キーを設定します。
        /// </summary>
        /// <param name="encryptionKey">設定する暗号化キー（UTF-8 で 32 バイト）</param>
        public void SetSaveDataEncryptionKey(string encryptionKey)
        {
            _saveDataEncryptionKey = encryptionKey;
        }

        /// <summary>
        /// 使用する InputActionAsset を設定します。
        /// </summary>
        /// <param name="inputActionAsset">設定する InputActionAsset</param>
        public void SetInputActionAsset(InputActionAsset inputActionAsset)
        {
            _inputActionAsset = inputActionAsset;
        }

        /// <summary>
        /// モーダル名に対応する Addressables アドレスを返します。
        /// </summary>
        /// <param name="modalName">検索するモーダル名</param>
        /// <returns>対応するアドレス。見つからない場合は <c>null</c></returns>
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

        /// <summary>
        /// シーンタイプ名に対応する Addressables アドレスを返します。
        /// </summary>
        /// <param name="sceneTypeName">検索するシーンタイプ名</param>
        /// <returns>対応するアドレス。見つからない場合は <c>null</c></returns>
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

        /// <summary>
        /// プレイヤービルドタスクの設定データ
        /// </summary>
        [Serializable]
        public class PlayerBuildTaskData
        {
            /// <summary>
            /// このタスクが有効かどうか
            /// </summary>
            public bool IsEnabled;

            [SerializeField]
            private string _taskClassTypeName;
            private Type _taskClassType;

            /// <summary>
            /// タスクのクラス型を取得または設定
            /// </summary>
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

        /// <summary>
        /// 起動シーンのアドレスデータ
        /// </summary>
        [Serializable]
        public class BootSceneAddressData
        {
            /// <summary>
            /// 開発用起動シーンのAddressablesアドレス
            /// </summary>
            public string DevelopmentSceneAddress;

            /// <summary>
            /// 本番用起動シーンのAddressablesアドレス
            /// </summary>
            public string ProductionSceneAddress;
        }

        /// <summary>
        /// モーダルのアドレスデータ
        /// </summary>
        [Serializable]
        public class ModalAddressData
        {
            /// <summary>
            /// モーダルの識別名
            /// </summary>
            public string ModalName;

            /// <summary>
            /// モーダルプレハブのAddressablesアドレス
            /// </summary>
            public string ModalAddress;
        }

        /// <summary>
        /// シーンのアドレスデータ
        /// </summary>
        [Serializable]
        public class SceneAddressData
        {
            /// <summary>
            /// SceneTypeの識別名
            /// </summary>
            public string SceneTypeName;

            /// <summary>
            /// シーンのAddressablesアドレス
            /// </summary>
            public string SceneAddress;

            /// <summary>
            /// 開発専用シーンかどうか
            /// </summary>
            public bool IsDevelopmentOnly;
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