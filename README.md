# ARPG 项目进度记录

> 本文档是仓库唯一的项目说明与进度记录。它用于集中记录当前功能、设计边界、操作方式、待办事项和验收状态。

## 项目信息

- 项目类型：2D 像素俯视角 ARPG
- Unity 版本：2022.3.62f3c1
- 渲染管线：Universal Render Pipeline 14.0.12，2D Renderer
- 当前场景：`Assets/Scenes/TrainingGround.unity`
- 当前分支：`main`
- 最近更新：2026-09-16

## 当前阶段

训练场核心战斗闭环已经形成：玩家可以移动和攻击，训练木桩能够接收伤害并播放反馈，玩家和目标状态可通过 HUD 查看，鼠标选择会显示描边，单位之间按照脚底 Y 坐标形成伪 3D 前后遮挡。

当前工作重点是地图碰撞、相机收尾、完整运行验收、战斗表现和训练完成流程。

## 当前操作

| 操作 | 输入 |
|---|---|
| 移动 | `WASD` |
| 普通攻击 | 数字键 `1` |
| 选择敌人 | 鼠标左键点击敌人 |
| 取消选择 | 鼠标左键点击场景空白处 |

## 运行项目

1. 使用 Unity `2022.3.62f3c1` 打开仓库。
2. 打开 `Assets/Scenes/TrainingGround.unity`。
3. 进入 Play Mode。
4. 点击训练木桩查看描边和固定位置目标血条。
5. 使用数字键 `1` 攻击木桩。
6. 从木桩上方和下方经过，检查伪 3D 遮挡关系。

## 已完成功能

### 场景与角色

- [x] 创建正式训练场场景并加入 Build Settings
- [x] 搭建地面与围栏 Tilemap
- [x] 配置 Main Camera、Global Light 2D 和 Cinemachine 跟随
- [x] 创建玩家与训练木桩 Prefab
- [x] 配置 Rigidbody2D、Collider2D、SpriteRenderer 和 Animator
- [x] 实现 WASD 二维移动和左右朝向翻转
- [x] 实现 Idle、Run、Attack 和 Hit 动画切换
- [x] 使用脚底 `SortPoint` 动态计算 `Order in Layer`

### 属性与运行时数据

- [x] 使用 `CharacterStatsData` ScriptableObject 保存玩家基础模板
- [x] 玩家运行时创建独立的 `Health` 和 `ResourcePool`
- [x] 使用事件通知 UI 生命和资源变化
- [x] 使用 `IDamageable` 提供统一伤害入口
- [x] 支持普通生命和无限生命两种模式

当前数据边界：

- ScriptableObject 只保存基础配置，不记录当局变化。
- `Health`、`ResourcePool` 和状态机保存当前运行时状态。
- 训练木桩使用 `HealthMode.Infinite`，受击时触发反馈但不会死亡。
- 永久成长和存档尚未实现；后续应使用独立的可序列化存档 DTO，而不是在运行时修改 SO 资源。

### 状态机与代码结构

- [x] 创建可复用的泛型状态机
- [x] 玩家拆分为 Normal、Attack、Dead 状态
- [x] 敌人拆分为 Idle、Hit 状态
- [x] `PlayerManager` 统一组装输入、移动、动画和战斗对象
- [x] `EnemyManager` 统一持有敌人属性、动画和状态机

### 普通攻击

- [x] 使用新版 Input System
- [x] 将攻击输入设置为数字键 `1`
- [x] 使用动画事件调用攻击判定和攻击结束
- [x] 使用 `Physics2D.OverlapCircleAll` 检测攻击目标
- [x] 使用 LayerMask 筛选敌人 Hurtbox
- [x] 添加攻击方向过滤
- [x] 避免一次攻击对同一目标重复结算伤害
- [x] 从玩家属性模板读取攻击伤害
- [x] 木桩受击后播放 Hit 动画并恢复待机状态

### 玩家 HUD

脚本：`Assets/Scripts/UI/PlayerStatusHud.cs`

- [x] 显示角色姓名
- [x] 显示角色头像并预留替换入口
- [x] 使用 Slider 和文本显示 HP
- [x] 使用 Slider 和文本显示 MP
- [x] 通过事件刷新数值，不在 `Update` 中轮询

### 目标选择与 Enemy HUD

脚本：

- `Assets/Scripts/Enemy/EnemyTargetSelector.cs`
- `Assets/Scripts/UI/EnemyTargetHud.cs`

已实现：

- [x] 左键点击 Collider2D 选择敌人
- [x] 点击空白取消选择
- [x] 点击 UI 时不改变场景目标
- [x] 选中目标时切换描边材质
- [x] 取消选择时恢复原材质
- [x] 固定位置显示目标名称和生命值
- [x] 无限生命目标显示 `∞`
- [x] 目标死亡时解绑 UI 和选择状态

选择流程：

```text
鼠标左键
    ↓
Collider2D 命中 EnemyManager
    ↓
切换描边材质 + EnemyTargetHud.Show
    ↓
点击空白或目标死亡
    ↓
恢复原材质 + EnemyTargetHud.Hide
```

### 描边与伪 3D 排序

- 描边材质：`Assets/Materials/M_EnemySelectionOutline.mat`
- 描边 Shader：`Assets/Shader/SpriteSelectionOutline.shader`
- Y 排序脚本：`Assets/Scripts/Common/YSortRenderer.cs`

描边使用亮黄色和 2.5 像素宽度。Shader 通过 8 方向透明度采样生成轮廓，能够随 Sprite 动画帧变化。

Y 排序读取角色脚底 `SortPoint` 的世界坐标：

```text
脚底 Y 越小 → Order 越大 → 显示在前面
脚底 Y 越大 → Order 越小 → 显示在后面
```

该排序只影响渲染顺序，不改变角色坐标、碰撞体、动画或描边逻辑。

## 主要目录

| 路径 | 用途 |
|---|---|
| `Assets/Data/Characters` | 角色属性 ScriptableObject 资源 |
| `Assets/Scripts/Common` | 生命、资源、状态机和 Y 轴排序 |
| `Assets/Scripts/Player` | 玩家控制、状态和战斗 |
| `Assets/Scripts/Enemy` | 敌人状态、训练木桩和目标选择 |
| `Assets/Scripts/Stats` | 属性模板定义 |
| `Assets/Scripts/UI` | 玩家与目标 HUD |
| `Assets/Materials` | 运行时材质 |
| `Assets/Shader` | 手写描边 Shader |
| `Assets/Resources/UI` | UI 图片和字体资源 |
| `Assets/Scenes` | 可运行场景 |

## UI 维护约定

- 当前界面数量较少，不使用全局单例 `UIManager`。
- 每个 HUD 只负责展示和数据解绑，不持有战斗规则。
- UI 读取公开属性并订阅事件，不直接修改 ScriptableObject。
- 目标选择逻辑由 `EnemyTargetSelector` 管理，不写进 Enemy HUD。
- 后续界面数量增加后，再引入负责打开、关闭、层级和返回栈的 UI 导航服务。
- 头像为空时保留现有占位图，后续可直接在角色 SO 中配置 Sprite。

## 当前系统状态

| 系统 | 状态 |
|---|---|
| Tilemap 地图 | 基础地面与围栏完成，碰撞和最终分层待补齐 |
| 玩家移动 | 已实现，待完整手感和边界测试 |
| 玩家战斗 | 已实现实际伤害、攻击状态和动画事件判定 |
| 属性系统 | 基础模板与当局运行时数据已分离 |
| 训练木桩 | 已实现受击、无限生命和目标选择 |
| 玩家 HUD | 已接入姓名、头像、HP、MP |
| 目标 HUD | 已接入名称、生命和无限生命显示 |
| 选中反馈 | 手写 Shader 描边已接入 |
| 伪 3D 排序 | 玩家与木桩已使用脚底 Y 排序 |
| 敌人 AI | 未开始 |
| 永久成长 | 未开始 |
| 存档 | 未开始 |
| 训练完成流程 | 未开始 |

## 下一阶段待办

### P0：完成训练场基础

- [ ] 整理 Ground、Walls、Foreground Tilemap 分层
- [ ] 为围栏或墙体添加 TilemapCollider2D
- [ ] 视需要加入 CompositeCollider2D
- [ ] 配置 Cinemachine 相机边界
- [ ] 配置 Pixel Perfect Camera
- [ ] 验证玩家无法穿出地图

### P0：完整运行验收

- [ ] 验证斜向移动速度是否需要归一化
- [ ] 验证连续攻击、正反方向命中和攻击范围边界
- [ ] 验证选择、取消选择和多个目标切换
- [ ] 验证可死亡目标死亡后自动取消选择
- [ ] 验证玩家从目标上下经过时前后遮挡正确
- [ ] 确认 Unity Console 没有影响运行的红色报错

### P1：反馈和训练流程

- [ ] 添加受击闪白或变色
- [ ] 添加伤害数字
- [ ] 添加攻击和命中特效
- [ ] 添加镜头震动和音效
- [ ] 添加移动、选择和攻击提示
- [ ] 记录训练命中次数或完成条件
- [ ] 显示训练完成提示并开启出口
- [ ] 完成一次独立构建和试玩测试

### 后续系统

- [ ] 主动技能、冷却和 MP 消耗
- [ ] 普通敌人的追击、攻击和死亡流程
- [ ] Buff、Debuff 和状态图标
- [ ] 背包、装备、任务和地图界面
- [ ] 永久成长和版本化存档

## 建议实施顺序

```text
墙体碰撞与相机收尾
    ↓
移动、攻击、选择与排序完整验收
    ↓
命中表现和音效
    ↓
训练完成条件与出口
    ↓
独立构建测试与修复
```

第一版目标保持为：**能移动 → 能选中 → 能攻击 → 能看懂状态 → 有清晰反馈 → 能完成训练并离开。**

## 已知问题

- Unity Animator 图形编辑器偶尔输出 `UnityEditor.Graphs.Edge.WakeUp` 编辑器缓存异常；目前未发现对应的 C# 编译错误。
- Y 轴排序依赖 `SortPoint` 位于实际脚底；切换位置不准确时应调整该点，而不是修改 Shader。
- 描边依赖 Sprite 区域拥有足够透明边距；紧贴纹理边缘的图片可能出现轮廓裁切。

## 文档更新规则

- 只在本文件记录项目状态和计划，不再拆分多个根目录 Markdown 文档。
- 只把已经进入项目的内容标记为完成。
- 未完整验证的功能保留在待验收项中。
- 每次主要提交后更新“最近更新”“当前阶段”和“下一阶段待办”。
- `Assets` 下的字体许可和第三方资源说明必须保留，不属于本进度文档的合并范围。
