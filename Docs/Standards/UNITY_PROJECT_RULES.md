# Unity 项目规范

## 代码与数据

- ScriptableObject 保存静态配置，不保存当前局运行时状态。
- HP、MP、冷却、施法状态等变化数据放在 Runtime 对象或运行时组件中。
- 优先复用已有模块，不为单一需求复制一套平行系统。
- UI 负责展示、订阅状态和转发输入，不实现核心战斗规则。

## Unity 资源

- 修改、移动 Unity 资源时保留正确的 `.meta` 与 GUID。
- 不手工批量重建已有 `.meta`。
- Prefab、Scene、Animator、AnimationEvent 修改后必须进行 Unity Editor 验证。
- 像素资源默认保持 Point Filter、关闭 Mip Maps；是否关闭压缩以实际资源用途为准。

## 目录职责

- `Assets/GameScript/Runtime/GameLogic/`：角色、技能、Buff、UI、输入等现有玩法代码。
- `Assets/GameScript/Editor/`：项目自己的编辑器工具。
- `Assets/GameRes/`：运行资源，按 YooAsset 官方 Space Shooter 的目录职责分类。
- `Assets/GameRes/Config/`：ScriptableObject 配置。
- `Assets/GameRes/Entity/`、`UIPanel/`、`Scene/`：角色 Prefab、UI Prefab 和游戏场景等加载入口。
- `Assets/GameRes/EntityArt/`、`UIPanelArt/`、`SceneArt/`：运行依赖的图片、动画、材质和地图资源；不是制作参考稿。
- `Assets/GameSetting/`：YooAsset 采集等项目资源配置。
- `Assets/ArtSource/`：制作源图、参考图和候选资源，不作为常规运行资源采集根目录。
- `Assets/Resources/`：仅在实际需要时保留更新前必须使用的 Resources 资源；不为目录示例建立空壳资源。
- `Assets/Settings/`：Unity、URP 等项目设置；第三方插件自己的资源保留原有结构。
- `Docs/`：仓库级流程、规范和架构决策，不放进 Assets。

目录分类不等于已完成 YooAsset 接管。Package、Collector、构建规则、运行时初始化与加载需分别实现和验证。新增 Boot、Framework、PatchLogic 或热更新程序集时，应按实际功能需求建立，不仅为凑齐示例目录创建空实现。

## 架构约束

- 当前阶段优先保证训练场闭环，不提前引入重量级全局框架。
- 如果任务需要大规模重构才能完成，应先在 `CODEX_BRIDGE.md` 标记 `BLOCKED` 并说明原因。
- 实际代码是当前行为事实来源；Docs 是设计约束来源。两者冲突时需要明确处理，不静默忽略。
