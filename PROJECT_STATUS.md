# ARPG 项目进度记录

> 本文档仅用于记录项目当前完成情况。每次开发电脑推送新进度后，在此更新状态、问题和下一步任务。

## 基本信息

- 项目类型：2D 像素俯视角 ARPG
- Unity 版本：2022.3.62f3c1
- 渲染管线：Universal Render Pipeline（URP）14.0.12
- 项目模板：2D URP
- 当前分支：`main`
- 开发基线：`1d34b12`（本次战斗与训练木桩进度基于该提交继续开发）
- 最近更新：2026-09-15
- 工作排期：参见 [`TRAINING_GROUND_WORK_PLAN.md`](TRAINING_GROUND_WORK_PLAN.md)

## 当前阶段

**阶段 2：玩家普通攻击与训练木桩受击原型已实现，正在补齐伤害数值、战斗反馈和完整运行验证。**

仓库已加入正式场景 `TrainingGround.unity`、训练场 Tilemap、玩家预制体、Input System 移动与攻击输入、角色 Idle/Run/Attack 动画，以及 Cinemachine 跟随相机。当前工作区已经创建训练木桩预制体、独立 Hurtbox 和受击动画，并通过动画事件同步攻击判定与状态复位。墙体 Tilemap 碰撞、Pixel Perfect Camera、生命值和实际伤害数值仍待实现。

## 已完成

- [x] 创建 Unity 2022.3 项目
- [x] 使用 2D URP 项目结构
- [x] 配置 URP 2D Renderer
- [x] 创建 GitHub 仓库并提交初始项目
- [x] 创建正式训练场场景 `Assets/Scenes/TrainingGround.unity`
- [x] 用 `TrainingGround.unity` 替换模板 `SampleScene.unity` 并加入 Build Settings
- [x] 场景内存在 `Main Camera`
- [x] 场景内存在 `Global Light 2D`
- [x] 导入并切割地图图集，生成地面与围栏 Tile 资源
- [x] 使用 `Tilemap_ground` 和 `Tilemap_Fence` 两层 Tilemap 搭建训练场地图
- [x] 创建玩家预制体，并配置 `Rigidbody2D`、`CapsuleCollider2D`、SpriteRenderer 和 Animator
- [x] 接入新版 Input System，配置 WASD 二维移动输入
- [x] 实现基于 `Rigidbody2D` 的玩家移动和左右朝向翻转
- [x] 创建 Idle/Run 动画和 Animator Controller，并按移动速度切换动画
- [x] 添加 Cinemachine 虚拟相机并跟随玩家
- [x] 导入战士 Attack1、Attack2、Guard、Idle、Run 动画素材
- [x] 创建并分类保存三套训练木桩四帧横向 Sprite Sheet
- [x] 创建 `TrainingDummy` 预制体、实体碰撞器和独立 `Hurtbox` 子物体
- [x] 创建 `EnemyHurtbox` Layer，并用于玩家攻击范围筛选
- [x] 添加鼠标左键普通攻击输入
- [x] 创建 `PlayerCombat`，使用 `attackPoint` 和 `OverlapCircleAll` 检测目标
- [x] 使用 Attack 动画事件调用 `AttackHit` 和 `EndAttack`
- [x] 创建木桩受击 Animator 与独立动画控制脚本
- [x] 使用受击动画事件调用 `EndHitAnimation`，允许木桩重复播放受击动画
- [x] 玩家转向时翻转整个 Player，使 `attackPoint` 自动跟随朝向
- [x] 增加攻击方向过滤，避免命中玩家身后的目标

## 待完成与待验证

- [ ] 按规划整理为 `Ground`、`Walls`、`Foreground` 三层 Tilemap（当前为地面与围栏两层）
- [ ] 为围栏/墙体添加 `TilemapCollider2D`，验证玩家无法穿墙或离开地图
- [ ] 添加并配置 Pixel Perfect Camera
- [ ] 完整验证玩家移动、朝向、Idle/Run/Attack 动画和相机跟随
- [ ] 确认 Unity Console 中没有红色报错
- [ ] 清理 Animator 窗口 `UnityEditor.Graphs.Edge.WakeUp` 缓存异常
- [ ] 为普通攻击增加实际伤害数值
- [ ] 为玩家与训练木桩实现生命值、死亡或重置逻辑
- [ ] 验证连续攻击、正反方向命中和攻击范围边界
- [ ] 创建最小 HUD
- [ ] 创建训练场出口和场景切换入口

## 下一个开发目标

### 里程碑 2：完成普通攻击与训练木桩伤害闭环

当前验收状态：

1. [x] 添加鼠标左键普通攻击输入并触发 Attack 动画。
2. [x] 使用动画事件在指定帧进行攻击检测，并在末帧解除攻击锁定。
3. [x] 创建带独立 Hurtbox 的训练木桩，并正确筛选 `EnemyHurtbox` Layer。
4. [x] 木桩受击后播放一次动画，并在末帧复位 `Hit` 参数。
5. [x] 攻击点随玩家左右朝向翻转，并过滤玩家身后的目标。
6. [ ] 添加生命值和实际伤害结算入口。
7. [ ] 添加木桩受伤、死亡及重置逻辑。
8. [ ] 在 Play Mode 完整验证连续攻击和边界情况，并清空 Console 红色报错。

## 当前场景状态

| 场景 | 状态 | 说明 |
|---|---|---|
| `SampleScene.unity` | 已移除 | 已由正式训练场场景替换 |
| `TrainingGround.unity` | 战斗原型搭建中 | 包含地面/围栏 Tilemap、玩家、训练木桩、主相机、全局光源及 Cinemachine 跟随相机 |

## 当前系统状态

| 系统 | 状态 |
|---|---|
| Tilemap 地图 | 进行中：地面与围栏已搭建，三层结构和墙体碰撞待补齐 |
| 玩家移动 | 已实现，仍需完整运行验收 |
| 玩家朝向与动画 | 已实现整体左右翻转及 Idle/Run/Attack 切换 |
| 相机 | Cinemachine 跟随已配置，Pixel Perfect 待配置 |
| 玩家战斗 | 原型已实现：左键攻击、动画事件判定、攻击锁定、方向过滤 |
| 训练木桩 | 原型已实现：实体碰撞、独立 Hurtbox、受击动画及事件复位 |
| 敌人 AI | 未开始 |
| 生命与伤害 | 未开始：当前命中只驱动受击动画 |
| UI | 未开始 |
| 场景切换 | 未开始 |
| 存档 | 未开始 |
| 联机 | 不属于当前阶段 |

## 本次更新依据

- Git 开发基线：`1d34b12`；本次在此基础上完成战斗与木桩相关内容。
- 已静态检查场景、玩家与木桩预制体、Input Actions、玩家战斗脚本、木桩动画脚本及两个 Animator Controller。
- 玩家 Attack 动画已关闭循环，并包含 `AttackHit`、`EndAttack` 事件。
- 木桩受击动画已关闭循环，并包含 `EndHitAnimation` 事件。
- Unity 日志未发现脚本编译错误；仍存在 Animator 编辑器图形缓存产生的 `UnityEditor.Graphs.Edge.WakeUp` 异常，需重开窗口或重启 Unity 后复查。

## 更新规则

- 只记录已经在 Unity 中完成并验证过的内容。
- 未经运行验证的代码不能标记为完成。
- 每次更新时修改“最近更新”和“当前阶段”。
- 完成一个里程碑后，记录测试结果，再列出下一个里程碑。
- 本文档不存放详细策划；详细设计应留在本地策划资料中。
