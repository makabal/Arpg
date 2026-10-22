# ADR-003：对齐 YooAsset 官方示例的项目目录

- 日期：2026-10-20
- 授权：开发者明确要求按已展示的官方 Space Shooter 风格目录方案迁移项目。
- 关联任务：ARPG-20261020-01

## 决策

沿用项目安装的 YooAsset 3.0.5 官方 Space Shooter 示例中的 `GameScript / GameRes / GameSetting` 命名，将制作源文件与运行资源分开。保留项目现有 Player、Enemy、Skills、Buffs、Stats、UI 等模块，不复制示例游戏逻辑。

```text
Assets/
├── GameScript/
│   ├── Runtime/GameLogic/   现有玩法代码、输入配置与生成代码
│   └── Editor/              项目编辑器工具
├── GameRes/
│   ├── Config/              ScriptableObject 配置
│   ├── Entity/              角色与敌人 Prefab
│   ├── EntityArt/           角色运行图片、动画与相关依赖
│   ├── Scene/               游戏场景
│   ├── SceneArt/            地图与场景运行资源
│   ├── UIPanel/             UI Prefab
│   ├── UIPanelArt/          UI 运行图片与面板依赖
│   ├── UISprite/            UI 图标、按钮与独立精灵
│   ├── UIFont/              运行字体
│   └── Shader/              项目 Shader
├── GameSetting/             YooAsset 采集配置
├── ArtSource/               制作参考、源文件与候选稿
├── Settings/                Unity / URP 设置
└── TextMesh Pro/            第三方插件自带资源
```

只建立当前有内容的目录。`Resources` 仅在实际存在启动前必须读取的资源时保留；后续实现 Boot、PatchLogic、Framework、资源管理器或 HybridCLR 时再增加对应目录和实现。

## 边界

- 这是目录与路径迁移，不表示已经完成资源热更新或代码热更新。
- 保留 Unity 默认程序集，不在本批引入 asmdef。
- YooAsset 采集配置迁到 GameSetting，本批不新增 Package、Collector 或打包规则。
- Prefab、Scene、ScriptableObject 的现有序列化内容与 `.meta` GUID 保持不变。
- 只调整目录迁移直接涉及的代码路径、Build Settings 场景路径和导入配置路径。
- `*Art` 目录是运行资源依赖，不等于参考稿；按引用关系分类，不以名字猜测。

## 本批迁移对应关系

| 原位置 | 当前位置 |
|---|---|
| `Assets/Scripts/` | `Assets/GameScript/Runtime/GameLogic/` |
| `Assets/Editor/` | `Assets/GameScript/Editor/` |
| 根目录的 `New Actions.cs` 与 `.inputactions` | `Assets/GameScript/Runtime/GameLogic/Input/` |
| `Assets/Data/` | `Assets/GameRes/Config/` |
| `Assets/Prefab/player/`、`Enemy/` | `Assets/GameRes/Entity/player/`、`Enemy/` |
| `Assets/Prefab/UI/` | `Assets/GameRes/UIPanel/` |
| `Assets/Scenes/` | `Assets/GameRes/Scene/` |
| `Assets/Animations/` | `Assets/GameRes/EntityArt/Animations/` |
| `Assets/Tiles/` | `Assets/GameRes/SceneArt/Tiles/` |
| `Assets/Materials/` | `Assets/GameRes/EntityArt/Materials/` |
| `Assets/Shader/` | `Assets/GameRes/Shader/` |
| `Assets/Art/UI/Generated/` | `Assets/GameRes/UIPanelArt/Generated/` |
| `Assets/Resources/` 下的实际运行内容 | 按用途分入 EntityArt、SceneArt、UIPanelArt、UISprite、UIFont |
| 未作为运行资源使用的 Art 及 Fonts/Art 素材包 | `Assets/ArtSource/`，Fonts/Art 中保留的源素材位于 `FontArt/` |
| `Assets/BundleCollectorSetting.asset` | `Assets/GameSetting/BundleCollectorSetting.asset` |

原文件与文件夹 `.meta` 通过 GUID 比对保留；新增组织目录使用新 GUID。目录迁移不更改现有序列化资源中的 GUID 引用。

## 参考

- 官方示例：https://github.com/tuyoogame/YooAsset/tree/yoo3/Assets/YooAsset/Samples~/Space%20Shooter
- 对应本项目安装版本：`Library/PackageCache/com.tuyoogame.yooasset@3.0.5/Samples~/Space Shooter/`

实际迁移和验证结果记录在 CODEX_BRIDGE.md 对应任务中。
