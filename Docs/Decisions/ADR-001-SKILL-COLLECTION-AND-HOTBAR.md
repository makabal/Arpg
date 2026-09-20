# ADR-001：PlayerSkillCollection 与 SkillBar 职责

**Status:** Accepted  
**Date:** 2026-09-20

## 背景

当前项目有六个快捷技能槽，但未来玩家拥有的技能数量会超过 6 个，并计划加入技能界面、技能树和拖拽装配。

当前 `PlayerSkillCollection` 暂时只注册六槽中已经配置的技能，因此容易与 SkillBar 的职责混淆。

## 决定

长期职责定义为：

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
负责：展示、冷却、快捷键和输入转发
```

六槽 SkillBar 不承担技能解锁逻辑。

## 原因

这样可以：

- 支持玩家拥有超过 6 个技能。
- 让技能树与快捷栏解耦。
- 让拖拽装配流程自然变成 Collection → SkillBar。
- 避免 UI 槽位承担技能成长和资格判断。

## 当前阶段

当前只有少量技能，不要求立即重构 `PlayerSkillCollection` 的初始化来源。

在正式实现以下任一功能时重新处理：

- 技能数量超过 6 个。
- 技能面板。
- 技能树。
- 技能拖拽装配。

## 后续影响

届时应清理或迁移技能栏链路中的解锁职责，例如：

- `PlayerSkillEntry.IsUnlocked`
- `PlayerSkillEntry.SetUnlocked(...)`
- `PlayerSkillCollection.UnlockChanged`
- `SkillUseFailure.SkillLocked`
- `SkillSlotView` 的锁定展示和解锁判断
