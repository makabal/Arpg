# 青林物语 · Fusion Pixel 字体方案

- 用户要求参考项目已下载的像素字体，统一首页字体风格。
- 字体源文件：`Assets/GameRes/UIFont/FusionPixel/FusionPixel_12px_Proportional_zh-Hans.ttf`。
- 现有 TMP 字体资源：`Assets/GameRes/UIFont/FusionPixel/FusionPixel_12px_Proportional_zh-Hans SDF.asset`。
- `FusionPixel_TypeReference.png` 由上述真实字体以 12px 排字、最近邻整数倍放大生成，包含标题和三个菜单标签的字形参考。
- 标题采用深青绿色、细暖金偏移阴影；菜单采用奶油白，保持像素笔画和内部留白。
- `StartMenu_FusionPixel_v1.png` 为内置 ImageGen 参考字样生成的视觉稿；实际 Unity 接入应以现有 TMP 字体资源渲染独立文字，确保字形精确。
- 动态首页沿用已记录的落叶、光斑、前景视差、骑士待机和按钮反馈方案。本次只调整视觉设计，未修改运行时代码或场景。

## 完整编辑提示词

```text
Edit image 1, the existing woodland pixel-art game start menu. Image 2 is an EXACT typography reference sheet rendered from the user's installed Fusion Pixel 12px Proportional Simplified Chinese TTF. Image 2 is the authority for the glyph shapes, not a rough mood reference.
Replace ONLY the title lettering and the text within the three existing buttons on image 1 using the EXACT crisp grid-based Chinese glyphs shown in image 2. Match the stepped square pixel shapes, single-pixel stroke weight, and proportions of that font faithfully. Do not substitute brush lettering, calligraphy, thick display font, normal smooth Chinese font or metallic 3D block lettering.
Title reads exactly "青林物语". Use the top row of image 2 as its letterform reference. Arrange the four characters on one horizontal line, well centered inside the original parchment sign, at roughly 410-440 pixels wide on this 1672px-wide reference, with comfortable empty parchment around it. Deep forest teal fill with a very subtle thin warm gold down-right offset shadow. Preserve the open spaces in the characters. Absolutely no thick extrusion or chunky outline. It must visibly look like enlarged real low-resolution Fusion Pixel type.
The three buttons must read exactly "开始游戏", "游戏设置", "退出游戏", using corresponding rows of image 2 as their exact shapes. Warm cream text, subtle dark shadow, a consistent pixel grid, keeping approximately their existing text size and all button framing. Keep primary button's little gold triangle.
Preserve the entire original woodland environment, sign structure, parchment surface, menu layout, blue-and-gold knight, dummy, flag, rocks, trees, lighting, all UI shapes and all original image elements other than these text shapes. Do not add any text, flourishes, leaves, icons, subtitles or new UI. Deliver a single full-screen 16:9 game screen.
```
