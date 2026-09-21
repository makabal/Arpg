# ADR-002：技能成长、有效构建与 Buff 职责

**Status:** Accepted
**Date:** 2026-09-21

## 决定

技能成长使用以下数据流：

```text
SkillTreeDefinition / SkillUpgradeNodeDefinition
静态配置节点、前置、互斥、消耗和修改器
        ↓
PlayerSkillProgression
保存技能点与已购买节点，负责购买、重置和存档 DTO
        ↓
SkillBuildSnapshot
组合基础技能与升级后的最终数值、效果和形态
        ↓
PlayerSkillController / SkillDelivery / SkillEffect
```

Buff 使用以下职责：

```text
BuffDefinition（静态配置）
        ↓
ApplyBuffSkillEffect
        ↓
BuffController / BuffInstance（角色运行时状态）
        ↓
CharacterStatsRuntime / 周期效果
```

## 约束

- 不在运行时修改 `SkillDefinition`、`BuffDefinition` 或其他 ScriptableObject。
- 技能点由命令方法修改，事件只通知结果。
- JSON 只保存技能点和稳定节点 ID / 等级，不保存计算后的技能数值或 Unity 对象引用。
- 临时 Buff 默认不写入角色存档；永久被动由技能成长数据重新建立。
- SkillBar 仍只负责当前装备的六个技能。
- 一次施法使用固定 `SkillBuildSnapshot`，施法途中升级不改变已经开始的技能实例。

## 原因

该结构允许同一升级节点同时修改数值、替换技能形态和增加 Buff，同时保持技能静态配置、玩家进度、当前施法状态和 UI 相互独立。
