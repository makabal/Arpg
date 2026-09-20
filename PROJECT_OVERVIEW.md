# ARPG 项目全局纵览

> 本文件是其他 AI、Codex 和新开发会话快速理解项目的首要入口。  
> 每次 Git Commit 前都必须重新检查并同步更新本文件；即使本次变更不影响项目架构，也必须更新“最近同步记录”。

## 1. 项目概述

这是一个使用 Unity 制作的 2D 像素俯视角 ARPG 原型。

当前目标是先完成一个可扩展、可验证的训练场垂直切片，再逐步扩展技能树、敌人 AI、装备、存档和正式关卡流程。

当前已经具备：

- WASD 移动。
- 普通攻击。
- 目标选择。
- 玩家与目标 HUD。
- 状态机。
- 组合式技能系统。
- 持续技能“剑刃风暴”。
- 六槽技能栏第一版运行时接入。
- 训练木桩与基础战斗验证场景。

## 2. 技术环境

- Unity：`2022.3.62f3c1`
- Universal Render Pipeline：`14.0.12`
- Input System：`1.14.2`
- TextMesh Pro：`3.0.7`
- Cinemachine：`2.10.7`
- YooAsset：`3.0.5`
- 当前主要场景：`Assets/Scenes/TrainingGround.unity`

当前仍以 Unity 默认程序集为主，尚未建立完整自定义 Assembly Definition 体系。

## 3. 当前输入

```text
WASD → 移动

1 → BasicAttack
2 → Skill1（当前剑刃风暴）
3 → Skill2
4 → Skill3
5 → Skill4
6 → Skill5

鼠标左键 → 场景目标选择 / UI 技能槽点击
```

剑刃风暴为持续技能：按住对应输入维持，松开结束。

## 4. 核心运行架构

### Player

```text
PlayerManager
├─ PlayerInputHandler
├─ PlayerMovement
├─ PlayerAnimator
├─ Health
├─ ResourcePool
├─ PlayerSkillController
└─ StateMachine<PlayerManager>
   ├─ PlayerNormalState
   ├─ PlayerSkillState
   └─ PlayerDeadState
```

`PlayerManager` 是玩家运行时装配入口。

### Skill System

```text
SkillDefinition
├─ SkillAimResolver
├─ SkillTargetResolver
├─ SkillDelivery
├─ SkillEffect
├─ SkillCondition
└─ SkillPresentationData
```

普通攻击与主动技能使用同一套技能执行流程。

主要职责：

- `SkillDefinition`：静态技能定义。
- `SkillRuntime`：冷却等玩家独立运行时状态。
- `PlayerSkillController`：技能使用校验、释放、持续、冷却和资源消耗。
- `SkillCastContext / SkillHitContext`：一次施法与命中的上下文。

ScriptableObject 不保存玩家当前局运行时状态。

### Skill Collection 与 SkillBar

当前代码中的 `PlayerSkillCollection` 暂时只从六槽技能栏中的技能注册运行时对象。

长期设计已经确认：

```text
技能树 / 技能界面
负责：解锁、技能点、等级、分支、是否允许装备
        ↓
PlayerSkillCollection
负责：玩家拥有的全部技能及其运行时数据
        ↓
SkillBar
负责：当前装备的 6 个快捷槽位
        ↓
SkillSlotView
负责：图标、冷却、快捷键和输入
```

六槽 SkillBar 不负责技能解锁。

详细决策见：

`Docs/Decisions/ADR-001-SKILL-COLLECTION-AND-HOTBAR.md`

## 5. 当前 UI

已经实现：

- 玩家状态 HUD。
- 目标状态 HUD。
- 六槽技能栏 Prefab。
- 技能图标显示。
- 1–6 快捷键显示。
- 鼠标技能槽输入。
- 冷却遮罩。

主要代码：

- `Assets/Scripts/UI/SkillBar/SkillSlotView.cs`
- `Assets/Scripts/UI/SkillBar/SkillSlotButton.cs`

主要 Prefab：

- `Assets/Prefab/UI/SkillBar/SkillSlot.prefab`

“技能栏职责清理”（ARPG-20260920-01）已完成验收（ACCEPTED）：SkillBar / SkillSlotView 只负责当前装备技能的图标、空槽、冷却、快捷键和输入；技能解锁、允许装备与锁定展示不再属于技能栏运行时链路。技能解锁最终归属未来技能界面 / 技能树。

## 6. 当前技能

### Basic Attack

- 输入：`1`
- 走统一技能系统。
- 当前玩家攻击力配置下使用基于攻击力的近战伤害。

### Blade Storm

- 输入：按住 `2`
- 启动耗蓝：20
- 持续耗蓝：5 / 秒
- 冷却：8 秒，从技能结束后开始
- 持续方式：按住输入
- 使用持续范围伤害区域
- 动画结构：Start / Loop / End

仍需要持续进行 Unity Play Mode 端到端验证，尤其是动画事件、异常中断、持续耗蓝和结束状态。

## 7. 当前敌人与场景

已经存在：

- `EnemyManager`
- `EnemyTargetSelector`
- 训练木桩
- 无限生命模式
- 目标选择与取消选择
- 目标 HUD
- 选中表现
- Tilemap 训练场
- Cinemachine 跟随
- Global Light 2D
- Y 轴排序

普通敌人的移动、追击、攻击、死亡、掉落仍未完整实现。

## 8. 数据与资源原则

- ScriptableObject：静态配置。
- Runtime 对象 / 运行时组件：当前局变化数据。
- `Assets/Art/`：制作源图、参考图、候选资源。
- `Assets/Resources/`：当前仍使用 Resources 的运行时资源。
- `Assets/Data/`：ScriptableObject 配置。
- `Docs/`：项目长期流程、规范和架构决策。

YooAsset 已安装，但正式运行时资源加载流程尚未完整接管 Resources。

## 9. 当前主要未完成内容

- 技能面板和技能树。
- 超过 6 个技能后的拖拽装配。
- 普通敌人 AI。
- 物品、装备和背包。
- 正式存档。
- 正式资源加载流程。
- 完整墙体碰撞和相机边界。
- 正式关卡出口与场景切换。
- 命中特效、伤害数字、声音和镜头表现。
- 自动化 EditMode / PlayMode 测试体系。

## 10. 关键文档

```text
PROJECT_OVERVIEW.md
→ 当前整个项目的真实状态

README.md
→ 项目介绍、开发进度和面向开发者的说明

Docs/
→ 长期流程、规范和架构决策

CODEX_BRIDGE.md
→ 当前任务、Codex 执行记录、提交和验收
```

重要规范：

- `Docs/Workflows/AI_COLLABORATION.md`
- `Docs/Workflows/FEATURE_DEVELOPMENT.md`
- `Docs/Workflows/GIT_WORKFLOW.md`
- `Docs/Workflows/UNITY_VALIDATION.md`
- `Docs/Standards/UNITY_PROJECT_RULES.md`
- `Docs/Standards/SKILL_SYSTEM_RULES.md`

## 11. AI 接手顺序

任何新 AI / Codex 会话开始处理项目任务前，应按以下顺序阅读：

1. `PROJECT_OVERVIEW.md`
2. `CODEX_BRIDGE.md`
3. `Docs/README.md`
4. 当前任务相关的 `Docs/Workflows/`
5. 当前任务相关的 `Docs/Standards/`
6. 当前任务相关的 `Docs/Decisions/`
7. 实际相关代码和 Unity 资源

事实优先级：

```text
当前 GitHub 实际代码 / 资源
>
PROJECT_OVERVIEW.md
>
Docs 中已确认规范与决策
>
CODEX_BRIDGE 当前任务
>
README
>
旧聊天描述
```

如果代码与文档不一致，不要静默选择其中一个；应确认是代码未同步还是规范已经变化。

## 12. 项目协作强制规则

- 默认情况下，每一个功能和代码变更都必须先得到开发者确认。
- 只有开发者明确说“不需要确认”时才允许跳过确认等待。
- Codex 中由 GPT-5.6 Sol 作为主管负责需求理解、任务拆分和审查。
- GPT-5.6 Luna（推理强度：极高 / xhigh）负责具体代码实现。
- Sol 给 GPT-5.6 Luna（推理强度：极高 / xhigh）的任务必须清楚、完整、可验收。
- 任何任务在标记为 `READY` 前，必须先向开发者展示任务确认表格并得到确认。
- 编码开始前必须先向开发者展示简洁任务清单。
- 每次 Git Commit 前必须同步更新本文件。

详细规则见：

`Docs/Workflows/AI_COLLABORATION.md`

## 13. 最近同步记录

- 2026-09-20：开发者确认 ARPG-20260920-01 的 Unity Play Mode 人工验证已通过。
- 2026-09-20：ARPG-20260920-01「技能栏职责清理」已通过 READY 前确认表格，正式进入 READY。
- 2026-09-20：ARPG-20260920-01「技能栏职责清理」已通过主管验收并标记为 ACCEPTED；Unity 编译确认通过，开发者已完成 Play Mode 人工验证。
- 2026-09-20：新增强制规则：任务从 DRAFT 进入 READY 前，必须先向开发者展示任务确认表格并得到确认。
- 2026-09-20：统一实现模型称谓为 GPT-5.6 Luna（推理强度：极高 / xhigh），后续实现任务固定使用 `gpt-5.6-luna` + `xhigh`。
- 2026-09-20：建立 AI 协作确认机制、Sol / GPT-5.6 Luna（推理强度：极高 / xhigh）分工规则和全局项目纵览同步规则。
