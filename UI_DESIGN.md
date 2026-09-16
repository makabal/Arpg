# UI 现状与扩展约定

## 当前实现

当前阶段使用最小可运行 HUD，不依赖统一 `UIManager`。每个界面组件直接订阅对应运行时数据的事件，职责保持独立。

### Player HUD

脚本：`Assets/Scripts/UI/PlayerStatusHud.cs`

显示内容：

- 角色姓名
- 角色头像
- HP Slider 与数值
- MP Slider 与数值

数据来源：

- 姓名、头像和最大值来自 `CharacterStatsData`
- 当前 HP 来自运行时 `Health`
- 当前 MP 来自运行时 `ResourcePool`
- HP、MP 通过 `Changed` 事件刷新，不在 `Update` 中轮询

### Enemy HUD

脚本：`Assets/Scripts/UI/EnemyTargetHud.cs`

显示内容：

- 当前目标名称
- 目标 HP Slider
- 当前值与最大值
- 无限生命目标显示 `∞`

该 HUD 固定在屏幕位置，不跟随敌人移动。选择目标时显示，点击空白、目标死亡或选择器失效时隐藏。

## 目标选择关系

`EnemyTargetSelector` 负责连接输入、目标表现和 UI：

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

点击 UI 时通过 `EventSystem.IsPointerOverGameObject` 阻止场景选择，避免 UI 操作误选目标。

## 视觉资源

- 描边材质：`Assets/Materials/M_EnemySelectionOutline.mat`
- 描边 Shader：`Assets/Shader/SpriteSelectionOutline.shader`
- UI 图像与字体：`Assets/Resources/UI/`
- TextMesh Pro 资源：`Assets/TextMesh Pro/`

当前描边使用亮黄色、2.5 像素宽度。Shader 通过 8 方向透明度采样生成轮廓，能够随 Sprite 动画帧变化。

## 维护约定

- ScriptableObject 只提供基础配置，不由 UI 修改。
- UI 只读取公开属性并订阅事件，不持有战斗规则。
- 选择逻辑不写进 HUD；HUD 仅负责展示和解绑数据。
- 后续界面数量增加后，再引入负责打开、关闭和层级管理的 UI 导航服务；当前无需全局单例 `UIManager`。
- 头像为空时保留现有占位图，后续可直接在角色 SO 中配置 Sprite。

## 后续扩展

- 攻击与交互按键提示
- 技能栏、冷却和 MP 消耗反馈
- Buff、Debuff 与状态图标
- 伤害数字和受击提示
- Boss 或精英目标血条样式
- 暂停、设置和按键绑定
- 背包、装备、任务和地图界面
- 训练完成与场景出口提示
