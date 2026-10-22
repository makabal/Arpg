# ARPG 项目全局纵览

> 本文件是其他 AI、Codex 和新开发会话快速理解项目的首要入口。  
> 每次 Git Commit 前都必须重新检查并同步更新本文件；即使本次变更不影响项目架构，也必须更新“最近同步记录”。

## 1. 项目概述

这是一个使用 Unity 制作的 2D 像素俯视角 ARPG 原型。

当前目标是先完成一个可扩展、可验证的训练场垂直切片，再逐步扩展技能树、敌人 AI、装备、存档和正式关卡流程。

当前已经具备：

- 《青林物语》独立 StartMenu 首场景：木牌像素标题、Fusion Pixel 文字按钮、设置/退出、训练场跳转、飘叶/光斑/光束/骑士待机。2026-10-22 已完成 Unity 编译与主要 Play Mode 路径验证，待开发者视觉验收；说明见 `Docs/Features/START_MENU.md`。
- CoplayDev Unity MCP 10.1.2 已安装，stdio 编辑器连接已验证。

- WASD 移动。
- 普通攻击。
- 目标选择。
- 玩家与目标 HUD。
- 状态机。
- 组合式技能系统。
- 技能点、升级节点、前置/互斥分支和 JSON 进度 DTO 的运行时基础。
- 数值升级、技能形态替换、附加效果与 Buff 的有效技能构建链路。
- 通用 Buff 定义、叠层、持续时间、属性修正和周期伤害/治疗基础。
- 持续技能“剑刃风暴”。
- 六槽技能栏第一版运行时接入。
- `Tab` 键控制的多页面界面、顶部六页签与页面切换基础逻辑（待 Unity 验收）。
- 技能页面视觉初稿，包括技能点、重置按钮、图例和操作说明。
- 训练木桩与基础战斗验证场景。

## 2. 技术环境

- Unity：`2022.3.62f3c1`
- Universal Render Pipeline：`14.0.12`
- Input System：`1.14.2`
- TextMesh Pro：`3.0.7`
- Cinemachine：`2.10.7`
- YooAsset：`3.0.5`
- 当前主要场景：`Assets/GameRes/Scene/TrainingGround.unity`

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

Tab → 打开或关闭 TabPag；打开时默认选择 Character
```

剑刃风暴为持续技能：按住对应输入维持，松开结束。

当前角色页面尚未制作，因此默认选择 `Character` 时内容区域暂时为空。

## 4. 核心运行架构

### Player

```text
PlayerManager
├─ PlayerInputHandler
├─ PlayerMovement
├─ PlayerAnimator
├─ Health
├─ ResourcePool
├─ CharacterStatsRuntime
├─ BuffController
├─ PlayerSkillProgression
├─ PlayerSkillCollection
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
- `PlayerSkillProgression`：技能点、节点等级、前置、互斥、购买和重置。
- `SkillBuildSnapshot`：组合基础技能与升级后的最终数值、效果和形态。
- `PlayerSkillController`：技能使用校验、释放、持续、冷却和资源消耗。
- `SkillCastContext / SkillHitContext`：一次施法与命中的上下文。

ScriptableObject 不保存玩家当前局运行时状态。

### Skill Collection 与 SkillBar

`PlayerSkillCollection` 已从六槽技能栏拆分，由技能成长数据注册玩家拥有的技能；尚未配置技能树资产时，会兼容注册场景现有六槽技能，避免破坏训练场。

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

升级节点通过 `SkillUpgradeModifier` 修改 `SkillBuildSnapshot`，支持数值加成、替换 Aim / Delivery / TargetResolver / Presentation，以及增加或移除 SkillEffect。技能可以通过 `ApplyBuffSkillEffect` 向实现 `IBuffReceiver` 的目标施加 Buff。

详细决策见：

`Docs/Decisions/ADR-001-SKILL-COLLECTION-AND-HOTBAR.md`

`Docs/Decisions/ADR-002-SKILL-PROGRESSION-AND-BUFFS.md`

## 5. 当前 UI

已经实现：

- 玩家状态 HUD。
- 目标状态 HUD。
- 六槽技能栏 Prefab。
- 技能图标显示。
- 普通攻击技能图标配置。
- 1–6 快捷键显示。
- 鼠标技能槽输入。
- 冷却遮罩。
- `TabPag` 显隐控制。
- 顶部六页签选中表现和页面切换基础逻辑。
- 技能页面视觉初稿、技能点区域、重置按钮、图例和操作说明。

主要代码：

- `Assets/GameScript/Runtime/GameLogic/UI/SkillBar/SkillSlotView.cs`
- `Assets/GameScript/Runtime/GameLogic/UI/SkillBar/SkillSlotButton.cs`
- `Assets/GameScript/Runtime/GameLogic/UI/TabPageToggleController.cs`
- `Assets/GameScript/Runtime/GameLogic/UI/TopBar/TopBarTabGroup.cs`
- `Assets/GameScript/Runtime/GameLogic/UI/TopBar/TopBarTabView.cs`
- `Assets/GameScript/Runtime/GameLogic/UI/TopBarPageGroup.cs`

主要 Prefab：

- `Assets/GameRes/UIPanel/SkillBar/SkillSlot.prefab`

“技能栏职责清理”（ARPG-20260920-01）已完成验收（ACCEPTED）：SkillBar / SkillSlotView 只负责当前装备技能的图标、空槽、冷却、快捷键和输入；技能解锁、允许装备与锁定展示不再属于技能栏运行时链路。技能解锁最终归属未来技能界面 / 技能树。

技能界面当前加入了视觉参考、背景资源和 `TrainingGround` 中的 `TabPag / PageBox / SkillPage` 场景节点。技能点、升级分支和重置的运行时领域代码已经建立，但尚未创建实际技能树配置资产，也未把节点按钮、技能点文本和重置按钮绑定到运行时接口。

多页面界面当前行为：`TabPageToggleController` 切换 `TabPag`；打开时由顶部页签组选择 `Character`。当前场景中 `TopBar` 是 `TabPag` 的子节点，随其一起显隐。场景目前只有 `SkillPage` 视觉内容，没有 `CharacterPage`，因此默认角色页内容为空。页签与输入行为仍待 Play Mode 验收。

训练场 UI 使用 `CanvasScaler / Scale With Screen Size`，参考分辨率为 `1920×1080`，匹配模式为 `Expand`。玩家 HUD 保持左上定位，目标血条锚定顶部，技能栏锚定底部，`TabPag` 铺满画布，顶部页签锚定顶部。已通过 720p、1080p、16:10、超宽屏、4:3 和 4K 的静态布局检查；实际渲染、文字清晰度及交互仍待 Unity Game View / Play Mode 验收。尚未实现分辨率或窗口模式设置。

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
- `CharacterSaveData / SkillProgressSaveData`：角色 JSON 中的技能点和节点等级 DTO。
- `Assets/ArtSource/`：制作源图、参考图、候选资源。
- `Assets/GameRes/`：运行资源，按 Entity / EntityArt / Scene / SceneArt / UIPanel / UIPanelArt / UIFont 等职责分类。
- `Assets/GameScript/`：项目运行时代码和编辑器工具。
- `Assets/GameSetting/`：YooAsset 采集等资源配置。
- `Assets/GameRes/Config/`：ScriptableObject 配置。
- `Docs/`：项目长期流程、规范和架构决策。

2026-10-20 已按 YooAsset 3.0.5 官方 Space Shooter 风格完成目录整理；2026-10-22 新增 StartMenu 作为首场景，点击开始游戏进入 TrainingGround，保留 Unity 默认程序集。YooAsset 的 Package / Collector、运行时初始化和加载流程尚未实现；本批未新增 Boot、资源管理器或 HybridCLR。目录决策见 `Docs/Decisions/ADR-003-YOOASSET-PROJECT-LAYOUT.md`。

## 9. 当前主要未完成内容

- 角色页面及其他顶部页签对应页面。
- 技能树配置资产、UI 节点绑定和实际升级内容配置。
- 超过 6 个技能后的拖拽装配。
- Buff 图标栏、驱散、控制状态和更完整的战斗属性接入。
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

- 2026-10-22：完成《青林物语》StartMenu、独立 Fusion Pixel 字体按钮、细木框与紧凑布局、飘叶/光斑/光束/背景缓移及骑士待机；安装并验证 MCP 10.1.2 连接。Unity 编译、设置/音量和进入训练场已通过 Play Mode 验证。用户明确同意将此前目录迁移与首页一起同步，并采用先提交、再拉取变基、最后推送的顺序；提交详情见 Git 历史。

- 2026-10-21：完成训练场现有 UI 的 Canvas 缩放与 HUD 锚点调整；六种分辨率静态检查通过，实际显示与交互待 Unity 验收，见 ARPG-20261021-01。
- 2026-10-20：按开发者确认迁移为 GameScript / GameRes / GameSetting / ArtSource，保留原 GUID、训练场现有修改和玩法行为；验证结果见 ARPG-20261020-01。

- 2026-09-21：新增技能点与升级分支运行时基础、有效技能构建快照、动态技能集合、角色 JSON 技能进度 DTO，以及通用 Buff/运行时属性链路；Unity C# 编译已通过，配置资产和 Play Mode 行为仍待验收。
- 2026-09-21：将项目 Git 规则统一为根目录下依次执行 `status → pull --rebase origin main → add . → commit → push origin main`；失败时停止并报告，不自行增加额外 Git 操作。
- 2026-09-21：同步 `Tab` Action、`TabPag` 显隐、顶部六页签、默认角色页和技能页面视觉初稿；角色页及技能点/重置业务逻辑尚未实现，本批变更待 Unity 验收。
- 2026-09-20：同步根 README 的技能栏完成状态，并记录普通攻击图标与技能树界面初步资源/场景节点；技能树功能仍未完成或验收。
- 2026-09-20：开发者确认 ARPG-20260920-01 的 Unity Play Mode 人工验证已通过。
- 2026-09-20：ARPG-20260920-01「技能栏职责清理」已通过 READY 前确认表格，正式进入 READY。
- 2026-09-20：ARPG-20260920-01「技能栏职责清理」已通过主管验收并标记为 ACCEPTED；Unity 编译确认通过，开发者已完成 Play Mode 人工验证。
- 2026-09-20：新增强制规则：任务从 DRAFT 进入 READY 前，必须先向开发者展示任务确认表格并得到确认。
- 2026-09-20：统一实现模型称谓为 GPT-5.6 Luna（推理强度：极高 / xhigh），后续实现任务固定使用 `gpt-5.6-luna` + `xhigh`。
- 2026-09-20：建立 AI 协作确认机制、Sol / GPT-5.6 Luna（推理强度：极高 / xhigh）分工规则和全局项目纵览同步规则。
