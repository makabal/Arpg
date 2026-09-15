# ARPG 项目进度记录

> 本文档仅用于记录项目当前完成情况。每次开发电脑推送新进度后，在此更新状态、问题和下一步任务。

## 基本信息

- 项目类型：2D 像素俯视角 ARPG
- Unity 版本：2022.3.62f3c1
- 渲染管线：Universal Render Pipeline（URP）14.0.12
- 项目模板：2D URP
- 当前分支：`main`
- 当前提交：`6d447fb`（更新场景、脚本、预制体等资源）
- 最近更新：2026-09-15
- 工作排期：参见 [`TRAINING_GROUND_WORK_PLAN.md`](TRAINING_GROUND_WORK_PLAN.md)

## 当前阶段

**阶段 1：可移动训练场主体已实现，正在补齐碰撞、像素相机配置和运行验证。**

仓库已加入正式场景 `TrainingGround.unity`、训练场 Tilemap、玩家预制体、Input System 移动输入、角色朝向与 Idle/Run 动画，以及 Cinemachine 跟随相机。当前提交尚未加入墙体 Tilemap 碰撞和 Pixel Perfect Camera；本次进度更新仅完成仓库静态核对，仍需在 Unity 中运行并确认 Console 无报错。

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

## 待完成与待验证

- [ ] 按规划整理为 `Ground`、`Walls`、`Foreground` 三层 Tilemap（当前为地面与围栏两层）
- [ ] 为围栏/墙体添加 `TilemapCollider2D`，验证玩家无法穿墙或离开地图
- [ ] 添加并配置 Pixel Perfect Camera
- [ ] 在 Unity 中运行验证玩家移动、朝向、Idle/Run 动画和相机跟随
- [ ] 确认 Unity Console 中没有红色报错
- [ ] 创建训练木桩
- [ ] 实现普通攻击、生命值与伤害反馈
- [ ] 创建最小 HUD
- [ ] 创建训练场出口和场景切换入口

## 下一个开发目标

### 里程碑 1：完成可移动训练场的收尾验证

当前验收状态：

1. [x] 新建并保存 `Assets/Scenes/TrainingGround.unity`。
2. [ ] 建立 `Ground`、`Walls`、`Foreground` 三层 Tilemap；当前已有地面与围栏两层。
3. [x] 使用已导入的 64×64 像素地块搭建训练区域。
4. [ ] 为墙体配置 2D 碰撞，并验证玩家无法穿墙或离开地图。
5. [ ] 玩家移动、朝向和 Idle/Run 动画已有实现，等待 Play Mode 验证。
6. [ ] Cinemachine 跟随已配置，Pixel Perfect Camera 尚未配置。
7. [x] 将 `TrainingGround.unity` 加入 Build Settings。
8. [ ] 在 Unity 中确认 Console 没有红色报错。

## 当前场景状态

| 场景 | 状态 | 说明 |
|---|---|---|
| `SampleScene.unity` | 已移除 | 已由正式训练场场景替换 |
| `TrainingGround.unity` | 已创建，待运行验收 | 包含地面/围栏 Tilemap、玩家、主相机、全局光源及 Cinemachine 跟随相机 |

## 当前系统状态

| 系统 | 状态 |
|---|---|
| Tilemap 地图 | 进行中：地面与围栏已搭建，三层结构和墙体碰撞待补齐 |
| 玩家移动 | 已实现，待运行验证 |
| 玩家朝向与动画 | 已实现左右翻转及 Idle/Run 切换，待运行验证 |
| 相机 | Cinemachine 跟随已配置，Pixel Perfect 待配置 |
| 玩家战斗 | 未开始 |
| 训练木桩 | 未开始 |
| 敌人 AI | 未开始 |
| 生命与伤害 | 未开始 |
| UI | 未开始 |
| 场景切换 | 未开始 |
| 存档 | 未开始 |
| 联机 | 不属于当前阶段 |

## 本次更新依据

- GitHub 提交：`6d447fb`（由 `84960ea` 快进更新）。
- 已静态检查场景、预制体、输入配置、玩家脚本、Animator、Tile 资源和 Build Settings。
- 未进行 Unity Editor / Play Mode 运行测试，因此所有需要运行确认的项目均保留为“待验证”。

## 更新规则

- 只记录已经在 Unity 中完成并验证过的内容。
- 未经运行验证的代码不能标记为完成。
- 每次更新时修改“最近更新”和“当前阶段”。
- 完成一个里程碑后，记录测试结果，再列出下一个里程碑。
- 本文档不存放详细策划；详细设计应留在本地策划资料中。
