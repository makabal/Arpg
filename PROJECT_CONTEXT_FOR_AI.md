# ARPG 项目上下文（供 AI 协作使用）

> 本文用于把项目背景、现状、架构和待决策事项一次性提供给其他 AI。  
> 信息核对时间：2026-09-20  
> 基准分支：`main`  
> 基准提交：`bb92d544`（`docs: 重构开发流程与功能进度文档`）  
> GitHub：<https://github.com/makabal/Arpg>

## 1. 一句话概述

这是一个使用 Unity 制作的 2D 像素俯视角 ARPG 原型。目前已经具备训练场中的基础战斗闭环：玩家移动、普通攻击、目标选择、角色/目标 HUD，以及一套以 ScriptableObject 为配置、支持模块组合的技能系统。当前主要工作是完成持续技能“剑刃风暴”的运行验收，并把已经制作好的技能栏和范围指示器资源接入运行时。

## 2. 项目目标与当前边界

当前目标不是完整商业游戏，而是先构建一个可扩展、可验证的 ARPG 垂直切片。训练场是现阶段唯一实际游戏场景，用于验证角色控制、战斗、技能、敌人交互、HUD、动画和资源组织方式。

已经形成的设计原则：

- 普通攻击与主动技能走同一套技能执行流程。
- 技能通过 Aim、Target、Delivery、Effect 等模块组合，避免把具体技能逻辑写死在玩家类中。
- ScriptableObject 只保存静态配置；生命、法力、冷却和施法状态由运行时对象保存。
- UI 通过事件订阅运行时数据，不在 `Update` 中持续轮询数值。
- 制作/参考资源保存在 `Assets/Art`，确认后的运行时资源放在 `Assets/Resources`。
- 当前优先保证训练场闭环，不提前搭建过重的全局框架。

尚未进入当前范围或尚未完成的内容包括：普通敌人 AI、掉落、物品/装备、技能树、存档、完整关卡流转、正式资源加载流程、声音与完整战斗表现。

## 3. 技术环境

| 项目 | 当前版本/状态 |
|---|---|
| Unity | `2022.3.62f3c1` |
| 渲染管线 | Universal Render Pipeline `14.0.12` |
| 输入 | Input System `1.14.2` |
| 摄像机 | Cinemachine `2.10.7` |
| UI 文本 | TextMesh Pro `3.0.7` |
| 资源管理 | YooAsset `3.0.5`，仅完成安装和初始设置资产 |
| 主要场景 | `Assets/Scenes/TrainingGround.unity` |
| Build Settings | 当前只启用 `TrainingGround.unity` |
| 代码组织 | 目前没有自定义 Assembly Definition，主要代码进入默认程序集 |
| 自动化测试 | 未发现项目测试目录或已实现测试 |

仓库当前约有 1597 个受 Git 跟踪的文件，其中约 61 个 C# 文件。仓库包含大量像素图、Sprite Sheet、动画和 UI 制作资源，因此资源文件数量明显高于代码文件数量。

## 4. 当前可操作内容

| 操作 | 输入 | 说明 |
|---|---|---|
| 移动 | `WASD` | 由 Input System 读取，Rigidbody2D 在 FixedUpdate 中移动 |
| 普通攻击 | 数字键 `1` | 技能槽 `BasicAttack` |
| 剑刃风暴 | 按住数字键 `2` | 起手后持续生效，松开结束 |
| 选择敌人 | 鼠标左键点击敌人 | 显示目标 HUD 和选中材质 |
| 取消选择 | 鼠标左键点击空白处 | 清除选中与目标 HUD |

输入配置还预留了数字键 `3`、`4`、`5` 和鼠标右键，对应其余技能槽，但当前玩家 Prefab 只装备了普通攻击和剑刃风暴。

## 5. 核心运行架构

### 5.1 玩家状态流

```text
PlayerManager
├─ PlayerInputHandler：读取移动和六个技能槽输入
├─ PlayerMovement：Rigidbody2D 移动与左右朝向
├─ PlayerAnimator：设置移动/攻击/技能动画参数
├─ Health / ResourcePool：运行时 HP、MP
├─ PlayerSkillController：技能校验、释放、冷却、持续耗蓝
└─ StateMachine<PlayerManager>
   ├─ PlayerNormalState：移动并监听技能按下
   ├─ PlayerSkillState：施法、维持、释放、结束
   └─ PlayerDeadState：停止移动
```

`PlayerManager` 是 MonoBehaviour 入口和模块装配点。普通状态中按下技能后，`PlayerSkillController.TryBeginUse` 完成槽位、死亡、忙碌、冷却、法力、瞄准和条件校验；校验成功后进入 `PlayerSkillState`。

### 5.2 技能执行流

```text
输入或未来的技能栏 UI
→ PlayerSkillController.TryBeginUse
→ SkillAimResolver 生成方向、位置和可选锁定目标
→ PlayerSkillState 播放动画并等待动画事件或计时释放点
→ SkillDelivery 决定即时、投射物或持续范围形式
→ SkillTargetResolver 查询实际命中目标
→ SkillExecution 对所有目标执行 SkillEffect
→ 结束技能并按配置启动冷却
```

技能由 `SkillDefinition` 组合以下部分：

| 模块 | 职责 | 当前实现 |
|---|---|---|
| `SkillAimResolver` | 决定施法方向/位置/锁定对象 | 面朝方向、鼠标方向、自身、选中敌人 |
| `SkillTargetResolver` | 决定真正受到效果的目标 | 自身、选中目标、近战范围、物理范围 |
| `SkillDelivery` | 决定效果如何到达目标 | 即时、投射物、持续范围 |
| `SkillEffect` | 对目标执行结果 | 固定伤害、基于攻击力伤害、治疗 |
| `SkillCondition` | 额外施法条件 | 抽象入口已存在，暂无具体条件资源 |
| `SkillPassiveTrigger` | 被动触发来源 | 抽象入口和运行时包装已存在，暂无具体触发实现 |
| `SkillPresentationData` | 动画、音效、特效引用 | 动画参数已使用；音效/VFX 尚未实际接入 |

### 5.3 数据与运行时状态分离

- `CharacterStatsData`：角色静态基础属性。
- `SkillDefinition`：技能静态定义和模块引用。
- `SkillRuntime`：每个角色独立的技能冷却状态。
- `Health`：当前生命、死亡事件和无限生命模式。
- `ResourcePool`：当前法力与资源变化事件。
- `SkillCastContext` / `SkillHitContext`：一次施法和一次命中的上下文。

不要在运行时修改共享 ScriptableObject 来保存角色状态，否则多个角色会共享或污染资产数据。

### 5.4 敌人与目标选择

`EnemyManager` 实现 `IDamageable`，内部使用 Idle/Hit 状态和 `Health`。`EnemyTargetSelector` 使用鼠标世界坐标及 `Physics2D.OverlapPointAll` 选择敌人；选中后替换 SpriteRenderer 材质并绑定目标 HUD。目标死亡或选择器禁用时会解除事件和恢复材质。

训练木桩配置为无限生命：会触发受伤反馈，但生命不会下降，也不会死亡。

### 5.5 UI 与场景

- `PlayerStatusHud` 订阅玩家 Health 和 Mana 的变化事件。
- `EnemyTargetHud` 订阅当前目标生命事件，并支持无限生命显示为 `∞`。
- 当前没有全局 `UIManager`，也没有窗口栈。
- 场景使用 Tilemap、Cinemachine、Global Light 2D。
- `YSortRenderer` 根据脚底参考点的世界 Y 坐标更新渲染顺序。

## 6. 当前真实配置数据

### 6.1 玩家属性

来源：`Assets/Data/Characters/WarriorStats.asset`

| 属性 | 数值 |
|---|---:|
| 名称 | 玛卡巴卡 |
| 最大生命 | 100 |
| 最大法力 | 100 |
| 攻击力 | 25 |
| 移动速度 | 5 |

### 6.2 普通攻击

来源：`Assets/Data/Skills/Definitions/BasicAttack.asset`

- 技能 ID：`basic_attack`
- 初始耗蓝、持续耗蓝、冷却：均为 0
- 瞄准：角色当前面朝方向
- 投递：即时近战结算
- 检测半径：0.8
- 目标层：LayerMask 数值 8（第 3 层）
- 伤害：攻击力 × 0.35 并四舍五入，当前攻击力 25 时为 9 点
- 释放点：动画事件
- 允许施法时移动，允许被中断

### 6.3 剑刃风暴

来源：`Assets/Data/Skills/Definitions/BladeStorm.asset`

- 技能 ID：`blade_storm`
- 启动耗蓝：20
- 持续耗蓝：5/秒
- 冷却：8 秒，从技能结束时开始
- 持续方式：按住输入维持；`totalDuration = 0` 表示没有固定最长时间
- 释放点：动画事件；配置的效果时刻为约 0.5833 秒
- 允许移动，允许被中断
- 投递：跟随施法者的持续范围区域，技能结束时销毁
- 立即造成一次伤害，此后每 0.3 秒结算一次
- 范围：以施法者为中心、半径 1.65 的圆形，无最大目标数
- 每次命中固定造成 10 点伤害
- 动画：`BladeStorm` 起手 Trigger + `BladeStormActive` 持续 Bool

## 7. 关键目录和文件

```text
Assets/
├─ Scenes/TrainingGround.unity        # 当前唯一实际游戏场景
├─ Prefab/player/player.prefab        # 玩家 Prefab；当前装备前两个技能槽
├─ Prefab/Enemy/TrainingDummy.prefab  # 无限生命训练木桩
├─ Scripts/
│  ├─ Common/                         # 状态机、生命、资源池、Y 排序
│  ├─ Player/                         # 玩家入口、输入、移动、动画、状态
│  ├─ Enemy/                          # 敌人入口、受击状态、目标选择
│  ├─ Skills/                         # 技能核心、数据、瞄准、选目标、投递、效果
│  ├─ Stats/                          # 角色静态属性 SO
│  └─ UI/                             # 玩家 HUD、目标 HUD
├─ Data/
│  ├─ Characters/WarriorStats.asset
│  └─ Skills/                         # 技能定义及其组合模块资产
├─ Animations/player/                 # 玩家和剑刃风暴动画片段/控制器
├─ Resources/UI/SkillSystem/          # 运行时 UI 图像资源
└─ Art/                               # 概念稿、参考图、源图、候选资源

Packages/manifest.json                # Unity 包依赖
ProjectSettings/ProjectVersion.txt    # Unity 版本
README.md                             # 面向开发过程的进度记录
PROJECT_CONTEXT_FOR_AI.md             # 本文，面向 AI 协作的上下文快照
```

## 8. 完成度快照

### 已实现

- WASD 移动、左右朝向以及 Idle/Run 动画参数。
- 玩家 HP、MP、死亡状态入口。
- 普通攻击统一接入组合式技能系统。
- 主动技能的初始耗蓝、持续耗蓝、冷却和中断流程。
- 多种 Aim、Target、Delivery 和 Effect 模块。
- 剑刃风暴 Start/Loop/End 动画结构及持续范围伤害逻辑。
- 训练木桩、受击反馈、无限生命模式。
- 鼠标选中/取消敌人、选中材质、玩家 HUD、目标 HUD。
- 训练场、相机跟随、2D 光照、脚底 Y 排序。
- 技能栏、按键帽、状态叠层、范围指示器和技能图标的图片资源。
- YooAsset 包与 `BundleCollectorSetting.asset` 入口。

### 已有基础但尚未完整接入或验收

- 剑刃风暴的长按、耗蓝、伤害范围、动画收尾和异常中断需要在 Unity Play Mode 中完整验收。
- 六槽技能栏已有美术资源，但尚未发现运行时技能栏脚本和完整场景组装。
- 世界范围圈、扇形、直线、目标标记已有资源，但尚未接入技能瞄准流程。
- YooAsset 已安装，但 Package、Collector、构建规则和运行时初始化未配置完成。
- 地图层次已有基础，墙体碰撞、相机边界和训练场完成条件未完成。

### 尚未实现

- 普通敌人的移动、追击、攻击、死亡、销毁和掉落。
- 技能升级、解锁关系、技能装配和技能树 UI。
- 物品、装备、背包、任务、成长和正式存档。
- 正式的伤害数字、命中特效、音效、镜头震动。
- 场景出口、场景切换和完整游戏流程。
- 自动化 EditMode/PlayMode 测试与持续集成。

## 9. 已知限制与技术风险

1. **尚未完成 Unity 编辑器运行验收。** 文档和代码表明功能已接入，但剑刃风暴仍被明确标记为需要手动 Play Mode 验收；不要仅凭静态代码判断功能完全正确。
2. **动画事件是核心依赖。** 普通攻击和剑刃风暴使用 `AnimationEvents` 释放模式，动画片段必须正确调用 `SkillCastPoint`/`AttackHit` 和 `EndSkill`/`EndAttack`；事件遗漏会导致不出伤害或无法退出技能状态。
3. **玩家对场景目标选择器存在查找依赖。** Prefab 中 `skillTargetSelector` 当前为空，`PlayerManager` 会在 Awake 中使用 `FindObjectOfType<EnemyTargetSelector>()` 回退查找；多场景或多个选择器时需要改为明确注入。
4. **技能控制器仍依赖 PlayerManager。** 当前组合方式适合单玩家原型，但若技能系统要复用于敌人，需要抽象施法者接口、属性来源和阵营/目标过滤。
5. **敌人死亡链路未完成。** `EnemyManager.OnDied` 只有后续实现注释，不会播放死亡动画、掉落或销毁。
6. **技能失败只发事件。** 当前未发现用户可见的失败提示；冷却中、法力不足、无目标等原因需要由未来 UI 消费 `SkillFailed`。
7. **资源数量和重复图片较多。** `Assets/Art` 与 `Assets/Resources` 同时保存源图和运行图，继续扩展前应明确导入、压缩、Atlas 和 YooAsset 收集规则，避免构建体积快速增长。
8. **没有自动化测试保护。** 对技能时序、持续耗蓝、冷却开始点、重复目标过滤和死亡解绑的修改需要重点回归。
9. **当前命名空间为空。** 全部类型处于全局命名空间，规模继续增长后容易产生命名冲突，应在架构稳定后规划命名空间和程序集边界。

## 10. 本地打开与验证建议

1. 使用 Unity Hub 以 Unity `2022.3.62f3c1` 打开仓库根目录。
2. 等待 Package Manager 和资源导入完成。
3. 打开 `Assets/Scenes/TrainingGround.unity`。
4. 检查 Console 是否存在编译错误、丢失脚本或资源引用。
5. 进入 Play Mode，依次验证：
   - WASD 移动、斜向速度、朝向和地图边界；
   - 数字键 1 普攻的动画事件、范围和伤害；
   - 按住数字键 2 时剑刃风暴起手、持续伤害和持续耗蓝；
   - 松开数字键 2、法力耗尽、死亡或中断时是否正确收尾；
   - 冷却是否从技能结束后开始计算；
   - 点击训练木桩、点击空白处、目标 HUD 与描边是否同步。

目前没有可替代 Unity Play Mode 验证的自动化测试。静态分析或普通 .NET 编译不能完整验证 Unity 序列化引用、Animator 状态机和动画事件。

## 11. 推荐的近期开发顺序

1. 在 Unity 中完成剑刃风暴端到端验收，并修复动画事件/状态退出/持续伤害问题。
2. 实现六槽技能栏运行时组件，订阅 `PlayerSkillController` 的 Started、Released、Finished、Failed 事件，并读取 `SkillRuntime` 冷却状态。
3. 接入图标、冷却遮罩、法力不足、锁定、选中和按键提示。
4. 接入世界范围指示器，并明确指示器由 Aim 阶段还是施法状态控制。
5. 增加最小化 PlayMode 测试，覆盖持续耗蓝、技能结束和重复目标过滤。
6. 完成训练场墙体碰撞、相机边界、完成条件和出口。
7. 再开始技能升级/解锁、普通敌人 AI 和 YooAsset 正式加载流程。

## 12. 需要与 AI 重点讨论的决策

可以把以下问题直接交给其他 AI 评审：

1. 当前 `SkillDefinition + Aim + Target + Delivery + Effect` 的组合边界是否合理？是否存在职责重叠？
2. 如何在不推翻现有原型的前提下，让技能系统同时支持玩家、敌人和召唤物？
3. 动画事件与计时释放两种模式应如何统一，怎样避免动画事件丢失导致状态卡死？
4. 技能栏 UI 应直接订阅 `PlayerSkillController`，还是增加只读 ViewModel/Presenter 层？
5. 技能升级应修改基础 `SkillDefinition`、使用等级配置表，还是在运行时叠加 Modifier？
6. 如何定义阵营、友军/敌军过滤和命中规则，以替换当前主要依赖 LayerMask 的做法？
7. YooAsset 应在什么阶段接管 Resources 中的技能/UI 资源，如何规划 Package 与 Collector？
8. 哪些核心逻辑适合先写 EditMode 测试，哪些必须写 PlayMode 测试？

## 13. 给接手 AI 的工作约束

- 先阅读本文件、根目录 `README.md` 和任务涉及的实际代码/资产，不要只根据文件名推断。
- 修改 Unity 资产时保留对应 `.meta` 文件及 GUID；不要批量重新生成已有 `.meta`。
- 不要把运行时状态写回 ScriptableObject。
- 新技能优先复用或新增可组合模块，不要在 `PlayerManager` 中添加具体技能分支。
- 任何涉及 Animator、AnimationEvent、Prefab 和 Scene 引用的修改，都应说明仍需 Unity Editor 验证的部分。
- 不要把 `Library`、`Temp`、`Logs`、`obj`、`UserSettings` 或 IDE 文件提交到 Git。
- 提交前检查 `git status`，避免把无关的资源重导入变化混进同一个提交。

## 14. 可直接复制给其他 AI 的简短提示词

```text
这是一个 Unity 2022.3.62f3c1 的 2D 像素俯视角 ARPG 原型，仓库是
https://github.com/makabal/Arpg 。请先阅读仓库根目录的
PROJECT_CONTEXT_FOR_AI.md 和 README.md，再结合实际代码回答。

当前已完成训练场中的移动、普通攻击、目标选择、HUD 和组合式技能框架，正在验收持续技能“剑刃风暴”，并准备接入六槽技能栏。技能由 ScriptableObject 配置，通过 Aim、Target、Delivery、Effect 等模块组合；运行时状态不能写回 ScriptableObject。

请明确区分：已经实现、只有资源/接口但尚未接入、以及仍需 Unity Play Mode 验证的内容。针对我接下来提出的问题，先分析现有设计和兼容性，再给出尽量小步、可验证的修改方案。
```
