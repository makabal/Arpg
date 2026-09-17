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
- normal, hover, selected, pressed, disabled and locked slots;
- LMB, RMB, Q and 1–4 keycaps;
- selection, hover, cooldown, lock, upgrade, charge and active overlays;
- modular end caps and divider.

Recommended Unity hierarchy:

1. hotbar frame;
2. skill icon;
3. cooldown radial overlay (`Image.Type = Filled`, radial 360);
4. state overlay;
5. charge or upgrade badge;
6. keycap.

The ordinary attack occupies the LMB slot inside the same six-skill bar.

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
