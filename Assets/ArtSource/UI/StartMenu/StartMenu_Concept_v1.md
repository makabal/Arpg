# 开始界面设计 v1

- 设计稿：`StartMenu_Concept_v1.png`，仅视觉方案，尚未接入 Unity 场景。
- 延续现有俯视角像素风、林间训练场、蓝金骑士、木框羊皮纸与深蓝金边按钮。
- 菜单：开始游戏（主按钮）、游戏设置、退出游戏；标题暂用项目名 ARPG。
- 后续实现可拆分背景、标题牌、按钮和角色；文字使用独立 UI 文本，悬停提亮金边，按下下移少量像素。
- 生成方式：内置 ImageGen；以下为完整提示词。

```text
Use case: ui-mockup.
Create a polished full-screen start menu concept for the user's existing 2D top-down pixel-art ARPG. Output a single landscape 16:9 game screen, ideally 1920x1080, edge to edge, no device frame, no presentation board, no annotations.
Input references: image 1 Template.png is the primary existing game art and UI style reference; image 2 Player HUD.png is the exact navy, warm gold beveled UI frame material reference; image 3 background.png is the woodland and training-ground environment reference; image 4 Warrior_Idle.png is a sprite strip reference ONLY, use a single matching blue-plumed gold-helmeted tiny chibi knight with blue-and-gold shield and pale teal sword, never the strip.
Design a NEW start screen faithfully matching those references: crisp intentional pixel clusters, thick dark navy pixel outlines, rich lime-green grass, teal-grey stone cliffs, warm wood, parchment cream, subtle gold metal edges. Keep the same cheerful miniature top-down fantasy look, no realistic illustration or high fantasy painting.
Composition: a beautifully framed woodland clearing, foliage and cliffs around the screen edges, warm sandy footpath winding from the lower foreground to an opening in the upper right. Wooden fence, one training dummy, a blue sword-emblem banner and a wooden barrel are environmental accents derived from the reference. A single recognizable little player knight stands on the right half of the clearing near the blue banner, with a ground shadow. Restrained sunbeams, tiny leaf particles, small white flowers. Environment remains readable and naturally supports menu negative space.
UI composition: centered slightly left of screen (at about x=42%), a modest wooden-and-parchment hanging title sign at y=20-35%, with exact title text 'ARPG' in large beautifully crafted chunky ivory-and-gold pixel lettering with dark navy shadow. Do NOT invent a game title or subtitle. Beneath it three vertically stacked distinct menu buttons with generous spacing at y=49%, 61%, 72%. Primary button wider and more luminous: wood-and-gold beveled border, rich deep navy interior, light cream bold pixel-style Chinese text '开始游戏', small pale gold right-facing triangular selection marker at its left. Secondary buttons slightly smaller and quieter but same material family, exact Chinese text '游戏设置' and '退出游戏'. All text accurately readable, large enough. Do not include continue game, load save, currencies, HUD, minimap, inventory, quests, version numbers or fabricated data. No extra text anywhere. Menu must have strong contrast and feel actually buildable in Unity uGUI with separate sprites. Elegant restrained ornament, balanced breathing room, visibly pixel-art rather than smooth vector. Deliver a finished attractive game start menu, not a wireframe.
```
