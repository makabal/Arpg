# Skill System UI Assets

All assets follow `Assets/Art/References/GameArtStyle_MasterReference.png`.

## Updated concept

`Assets/Art/UI/Concept/SkillUI_FullConcept_v3.png` uses a simplified tier progression:

- 基础
- 核心 — unlocked after 2 invested points
- 防御 — unlocked after 6 invested points
- 终极 — unlocked after 12 invested points

Only a main skill and its local upgrades are directly connected. Unrelated skills do not use cross-tree prerequisite lines.

## Hotbar

`Hotbar/` contains 26 transparent PNGs:

- six-slot wooden frame and separate potion dock;
- normal, hover, selected, pressed, disabled and legacy locked slot assets;
- keycap assets originally generated for several control schemes; the current runtime hotbar uses numeric slots `1`–`6`;
- selection, hover, cooldown, upgrade, charge and active overlays; lock-related assets are currently legacy and should not drive hotbar logic;
- modular end caps and divider.

Recommended Unity hierarchy:

1. hotbar frame;
2. skill icon;
3. cooldown radial overlay (`Image.Type = Filled`, radial 360);
4. state overlay;
5. charge or upgrade badge;
6. keycap.

Current runtime mapping is: slot 1 = ordinary attack, slots 2–6 = active skill slots. The hotbar itself does not own skill-unlock logic; unlock eligibility belongs to the future skill screen / skill tree.

## World targeting

`WorldTargeting/` contains 12 transparent PNGs for world-space aiming:

- neutral, valid and invalid AOE circles;
- dashed maximum-range ring;
- cone, projectile line, dash lane and melee arc;
- ground reticle and origin marker;
- hostile and friendly target markers.

Use these with a world-space Canvas or SpriteRenderer placed just above the ground. Keep point filtering, disable mipmaps and texture compression, and tint/fade through Unity rather than generating more baked color variants.

## Atlases

The original generated atlases are retained under `Atlases/` for review and optional Unity Sprite Editor slicing.


## Current runtime status

The six-slot hotbar is now assembled in `Assets/Scenes/TrainingGround.unity` using:

- `Assets/Prefab/UI/SkillBar/SkillSlot.prefab`
- `Assets/Scripts/UI/SkillBar/SkillSlotView.cs`
- `Assets/Scripts/UI/SkillBar/SkillSlotButton.cs`

The current runtime responsibility is limited to equipped-skill display, cooldown, hotkey text and input forwarding. Skill unlock, skill points, levels and progression should be implemented in the dedicated skill interface rather than in the hotbar.
