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

- `Assets/Art/`：制作源图、参考图、候选资源。
- `Assets/Resources/`：当前仍由 Resources 使用的运行时资源。
- `Assets/Data/`：ScriptableObject 配置。
- `Assets/Scripts/`：运行时代码。
- `Assets/Prefab/`：Prefab。
- `Docs/`：仓库级流程、规范和架构决策，不放进 Assets。

## 架构约束

- 当前阶段优先保证训练场闭环，不提前引入重量级全局框架。
- 如果任务需要大规模重构才能完成，应先在 `CODEX_BRIDGE.md` 标记 `BLOCKED` 并说明原因。
- 实际代码是当前行为事实来源；Docs 是设计约束来源。两者冲突时需要明确处理，不静默忽略。
