# 剑刃旋风 / Blade Whirlwind

## Final prototype-exact animation

Use `PrototypeExact/Warrior_BladeWhirlwind_Exact_6F.png`.

- Layout: 6 horizontal frames
- Frame size: 192×192
- Loop: false
- Suggested playback: 10–12 FPS
- Damage window: frames 3–5
- Strongest visual frame: frame 4
- Character and slash VFX are combined

The six frames are assembled without repainting the warrior:

1. Attack2 frame 1 — anticipation
2. Attack2 frame 2 — initial swing
3. Attack2 frame 3 — first spin
4. Attack1 frame 3 — strongest circular slash
5. Attack1 frame 4 — follow-through
6. Idle frame 1 — recovery

Separate final frames are under `PrototypeExact/Frames/`.

## Skill icon

Use:

`Assets/Resources/UI/SkillSystem/Skills/BladeWhirlwind/Icons/Skill_BladeWhirlwind_Icon_PrototypeExact_256.png`

The icon is extracted from the original Attack2 spin frame. The earlier generated icon can remain as a concept alternative but is not the prototype-exact default.

## Draft notice

The `Combined/` folder and the earlier separate Player/VFX folders are generated drafts. Do not use them for the final character animation because they redraw the character.
