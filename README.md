# ARPG 训练场原型

基于 Unity 2022.3.62f3c1 和 URP 14.0.12 开发的 2D 俯视角 ARPG 原型。当前版本围绕训练场完成了玩家移动、普通攻击、运行时属性、训练木桩、目标选择、状态 HUD、选中描边和基于脚底 Y 坐标的伪 3D 遮挡排序。

## 当前操作

| 操作 | 按键 |
|---|---|
| 移动 | `WASD` |
| 普通攻击 | 数字键 `1` |
| 选择敌人 | 鼠标左键 |
| 取消选择 | 点击场景空白处 |

## 运行项目

1. 使用 Unity `2022.3.62f3c1` 打开仓库。
2. 打开 `Assets/Scenes/TrainingGround.unity`。
3. 进入 Play Mode。
4. 点击训练木桩查看描边和固定位置目标血条，使用数字键 `1` 进行攻击。

## 已实现系统

- 玩家状态机：普通、攻击和死亡状态。
- 敌人状态机：待机与受击状态。
- `CharacterStatsData` ScriptableObject：保存角色基础模板数据。
- `Health` 与 `ResourcePool`：保存当局运行时生命和资源数据，并通过事件通知 UI。
- `IDamageable`：统一伤害入口。
- 训练木桩：独立 Hurtbox、受击动画和无限生命模式。
- 玩家 HUD：姓名、头像、HP 和 MP。
- 目标 HUD：目标名称、生命值和无限生命显示。
- 鼠标目标选择：选中时切换描边材质，取消选择时恢复原材质。
- 手写 URP 2D Sprite 描边 Shader：8 方向纹理采样，支持动画 Sprite。
- `YSortRenderer`：使用脚底排序点动态计算 `Order in Layer`，实现伪 3D 前后遮挡。

## 主要目录

| 路径 | 用途 |
|---|---|
| `Assets/Scripts/Common` | 生命、资源、状态机和 Y 轴排序 |
| `Assets/Scripts/Player` | 玩家控制、状态和战斗 |
| `Assets/Scripts/Enemy` | 敌人状态、训练木桩和目标选择 |
| `Assets/Scripts/Stats` | ScriptableObject 属性模板 |
| `Assets/Scripts/UI` | 玩家与目标 HUD |
| `Assets/Shader` | 选中描边 Shader |
| `Assets/Data/Characters` | 角色属性资源 |
| `Assets/Scenes` | 可运行场景 |

## 文档

- [项目状态](PROJECT_STATUS.md)
- [训练场工作计划](TRAINING_GROUND_WORK_PLAN.md)
- [UI 现状与扩展约定](UI_DESIGN.md)

## 下一阶段

- 完成地图墙体碰撞和相机边界。
- 配置 Pixel Perfect Camera 并进行完整 Play Mode 验收。
- 增加普通敌人的死亡、掉落或重生流程。
- 添加命中特效、伤害数字、音效和镜头反馈。
- 完成训练目标、出口和独立构建测试。
