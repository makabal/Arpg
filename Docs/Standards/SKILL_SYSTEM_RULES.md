# 技能系统规范

## 核心组合

现有技能优先通过以下模块组合：

```text
SkillDefinition
├─ SkillAimResolver
├─ SkillTargetResolver
├─ SkillDelivery
├─ SkillEffect
├─ SkillCondition
└─ SkillPresentationData
```

不要为了某一个具体技能在 `PlayerManager` 中加入专用技能分支。

## 静态配置与运行时状态

- `SkillDefinition`：技能静态定义。
- `SkillRuntime`：冷却等玩家独立运行时状态。
- ScriptableObject 不保存玩家当前局技能状态。

## PlayerSkillCollection

长期目标：

```text
PlayerSkillCollection
= 玩家当前拥有 / 可使用的全部技能及其运行时数据
```

当前实现暂时只从六槽 SkillBar 中注册技能，这是现阶段实现状态，不是最终职责。

当开始技能树、技能面板或技能数量超过 6 个时，应把 Collection 的初始化来源与 SkillBar 拆分。

## SkillBar

```text
SkillBar
= 当前装备到 6 个快捷栏槽位中的技能
```

SkillBar 负责：

- 六个槽位的技能引用。
- 快捷键映射。
- 后续的拖拽、替换和交换。

SkillBar **不负责技能解锁**。

技能是否已解锁、是否允许装备，应由技能界面/技能树在技能进入 SkillBar 前完成判断。

## SkillSlotView

只负责：

- 图标。
- 空槽状态。
- 冷却。
- 快捷键。
- 输入转发。

不负责：

- 技能解锁。
- 技能点。
- 技能等级。
- 技能分支。
- 战斗判定。

## 输入映射

当前六槽：

```text
1 → BasicAttack
2 → Skill1
3 → Skill2
4 → Skill3
5 → Skill4
6 → Skill5
```

鼠标点击技能槽与键盘输入应尽量复用同一条技能执行路径。
