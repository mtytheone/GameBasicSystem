# HatzeGameBasicSystem

Unity ゲーム開発の基盤となる共通システムを提供する UPM パッケージです。

## 動作環境

- Unity 6000.0 以降
- UniTask（別途インストール必要）
- Addressables 2.2.2 以降
- Input System 1.13.1 以降

## インストール

### 1. UniTask をインストール（先に行うこと）

`Packages/manifest.json` の `dependencies` に追加：

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
```

### 2. このパッケージをインストール

同じく `dependencies` に追加：

```json
"com.hatzelaboratory.gamebasicsystem": "https://github.com/mtytheone/GameBasicSystem.git"
```

### 3. 設定ファイルを生成

Unity メニュー **Edit > Project Settings > GameBasicSystem** を開き、「生成する」ボタンをクリックします。

---

## 機能一覧

### Runtime

| 機能 | 説明 |
|---|---|
| `Singleton<T>` / `SingletonBehaviour<T>` | シングルトンパターン |
| `GameSystemBase` | ゲームシステムの基底クラス |
| `SceneController` | Addressables を使ったシーン遷移管理 |
| `BootSceneController` | 起動シーンの制御 |
| `SoundManager` / `BgmController` / `SeController` | BGM・SE の再生管理 |
| `ModalManager` / `IModal` | モーダル UI の表示管理 |
| `ObjectPool<T>` / `IObjectPoolItem` | オブジェクトプール |
| `SaveDataManagerBase` | セーブ・ロードの基底クラス（暗号化・圧縮対応） |
| `KeyRepeatInteraction` | Input System 向けキーリピートインタラクション |
| `AddressableDefine` | Addressables パス定数 |
| `PlatformDefine` | プラットフォーム判定定数 |
| `GameBasicSystemSettingData` | パッケージ設定データ（ScriptableObject） |

**セーブ・ロード オプション：**
- 暗号化：`AESCryptor`（AES）
- 圧縮：`GZipCompressor`（GZip）
- シリアライズ：`JsonSerializer`（JSON）

**開発用 UI（DEBUG ビルドのみ）：**
- `DevelopmentInfoViewer` — FPS などの開発情報表示
- `MemoryViewerCanvasController` — メモリ使用量のバー表示
- `VersionTextLabelController` — ビルドタイムスタンプ表示

### Editor

| 機能 | 説明 |
|---|---|
| `PlayerBuildTool` | カスタムビルドパイプライン（タスク方式） |
| `BootSceneTask` | ビルド時のブートシーン自動設定 |
| `ExcludeDebugNativePluginsTask` | Release ビルド時にデバッグ用ネイティブプラグインを除外 |
| `AddressableBuildTool` | Addressables のビルドツール |
| `AssetAddressManagementTool` | アドレス管理ツール |
| `ModalNameEnumCreator` | ModalType enum の自動生成 |
| `SceneTypeNameEnumCreator` | SceneType enum の自動生成 |
| `ManagerCreator` | マネージャークラスの雛形生成 |

---

## セットアップ手順

### GameSystemBase を継承する

```csharp
public class MyGameSystem : GameSystemBase
{
    protected override async UniTask OnInitialize()
    {
        // 初期化処理
    }
}
```

### カスタムビルドタスクを追加する

`IPlayerBuildTask` を実装し、Project Settings > GameBasicSystem のビルドタスク一覧に登録します。

```csharp
public class MyBuildTask : IPlayerBuildTask
{
    void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType) { }
    void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport report) { }
}
```

---

## ライセンス

© Hatze Laboratory 2026
