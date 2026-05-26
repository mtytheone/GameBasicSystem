# HatzeGameBasicSystem

> [日本語版 README はこちら](README.ja.md)

A UPM package providing common systems for Unity game development.

## Requirements

- Unity 6000.0 or later
- UniTask (install separately)
- Addressables 2.2.2 or later
- Input System 1.13.1 or later

## Installation

### 1. Install UniTask first

Add to `dependencies` in `Packages/manifest.json`:

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
```

### 2. Install this package

Add to the same `dependencies`:

```json
"com.hatzelaboratory.gamebasicsystem": "https://github.com/mtytheone/GameBasicSystem.git"
```

### 3. Generate the settings asset

Open **Edit > Project Settings > GameBasicSystem** in the Unity menu and click the **"Generate"** button.

---

## Features

### Runtime

| Feature | Description |
|---|---|
| `Singleton<T>` / `SingletonBehaviour<T>` | Singleton pattern |
| `GameSystemBase` | Base class for game systems |
| `SceneController` | Scene transition management using Addressables |
| `BootSceneController` | Boot scene control |
| `SoundManager` / `BgmController` / `SeController` | BGM and SE playback management |
| `ModalManager` / `IModal` | Modal UI management |
| `ObjectPool<T>` / `IObjectPoolItem` | Object pooling |
| `SaveDataManagerBase` | Save/load base class with encryption and compression |
| `KeyRepeatInteraction` | Key repeat interaction for Input System |
| `AddressableDefine` | Addressables path constants |
| `PlatformDefine` | Platform detection constants |
| `GameBasicSystemSettingData` | Package settings data (ScriptableObject) |

**Save/Load options:**
- Encryption: `AESCryptor` (AES-256-CBC + HMAC-SHA256)
- Compression: `GZipCompressor` (GZip)
- Serialization: `JsonSerializer` (JSON)

**Development UI (DEBUG builds only):**
- `DevelopmentInfoViewer` — Displays development info such as FPS
- `MemoryViewerCanvasController` — Memory usage bar display
- `VersionTextLabelController` — Build timestamp display

### Editor

| Feature | Description |
|---|---|
| `PlayerBuildTool` | Custom build pipeline (task-based) |
| `BootSceneTask` | Automatically sets the boot scene at build time |
| `ExcludeDebugNativePluginsTask` | Excludes debug native plugins in Release builds |
| `AddressableBuildTool` | Addressables build tool |
| `AssetAddressManagementTool` | Address management tool |
| `ModalNameEnumCreator` | Auto-generates ModalType enum |
| `SceneTypeNameEnumCreator` | Auto-generates SceneType enum |
| `ManagerCreator` | Generates manager class templates |

---

## Setup

### Extend GameSystemBase

```csharp
public class MyGameSystem : GameSystemBase
{
    protected override async UniTask OnInitialize()
    {
        // initialization
    }
}
```

### Add a custom build task

Implement `IPlayerBuildTask` and register it in the build task list under Project Settings > GameBasicSystem.

```csharp
public class MyBuildTask : IPlayerBuildTask
{
    void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType) { }
    void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport report) { }
}
```

---

## License

Copyright © 2026 Hatze Laboratory. All Rights Reserved.

This software is published and reference purposes only.
Use, copying, distribution, or modification without permission from the author or Hatze Laboratory is prohibited.

---

## Third-Party Notices

### UniTask

- **Author:** Yoshifumi Kawai / Cysharp, Inc.
- **Repository:** https://github.com/Cysharp/UniTask
- **License:** MIT License — Copyright (c) 2019 Yoshifumi Kawai / Cysharp, Inc.
