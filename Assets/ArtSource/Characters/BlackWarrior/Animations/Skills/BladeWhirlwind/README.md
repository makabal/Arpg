# 剑刃旋风 / Blade Whirlwind

> 状态：**美术原型 / Archive**。当前项目运行时技能定义中不要把本目录视为已接入技能。

## Prototype-exact animation

可用的原型动画为：

`PrototypeExact/Warrior_BladeWhirlwind_Exact_6F.png`

- Layout: 6 horizontal frames
- Frame size: 192×192
- Loop: false
- Suggested playback: 10–12 FPS
- Damage window: frames 3–5
- Strongest visual frame: frame 4
- Character and slash VFX are combined

六帧由现有角色动作组合而成：

1. Attack2 frame 1 — anticipation
2. Attack2 frame 2 — initial swing
3. Attack2 frame 3 — first spin
4. Attack1 frame 3 — strongest circular slash
5. Attack1 frame 4 — follow-through
6. Idle frame 1 — recovery

独立帧位于 `PrototypeExact/Frames/`。

## Runtime status

此前 README 指向的运行时图标路径：

`Assets/Resources/UI/SkillSystem/Skills/BladeWhirlwind/...`

已经不存在，因此不再作为有效资源路径。

如果未来正式启用 Blade Whirlwind，应重新创建对应的 `SkillDefinition`、运行时图标和必要配置，并按当时的技能系统结构接入，而不是恢复旧路径。

## Draft assets

`Combined/` 以及更早的 Player/VFX 组合方案属于生成草稿，不作为当前正式角色动画来源。需要继续使用本技能原型时，优先参考 `PrototypeExact/`。
