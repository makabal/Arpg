# UI 素材目录

## Generated

由完整 UI 视觉稿拆分出的透明 PNG。命名规范：

- `UI_Atlas_*`：保留用的完整图集，不建议直接放入界面。
- `UI_HUD_*`：常驻 HUD 模块。
- `UI_Player_*`：玩家状态组件。
- `UI_Tab_*`：选项卡状态。
- `UI_Panel_*`：大型窗口。
- `Icon_Item_*`：物品和状态图标。

### 玩家状态栏

`Generated/PlayerStatus/` 中的头像框、名称牌、等级牌、状态条外框和两种填充图均为独立素材：

- `UI_Player_PortraitFrame.png`：透明中心的头像框。
- `UI_Player_StatusBarFrame.png`：生命/体力共用外框。
- `UI_Player_HealthFill.png`：生命值红色填充。
- `UI_Player_StaminaFill.png`：体力值蓝色填充。

头像内容不要合并进头像框。可选头像位于 `SourcePack/Human Avatars/`，当前共 25 个。

在 Unity 中，红条和蓝条应使用 `Image Type = Filled`、`Fill Method = Horizontal`，通过 `fillAmount` 表示当前比例。

### 背包选项卡

`Generated/Tabs/` 提供完全相同尺寸的两种状态：

- `UI_Tab_Selected.png`：选中状态。
- `UI_Tab_Unselected.png`：未选中状态。

选项卡文字应由 Unity 文本组件绘制，不要烘焙进图片。切换页面时交换对应 Sprite。

## SourcePack

Tiny Swords 原始 UI PNG，共 70 个，保留原始目录和文件名，方便与素材包文档对应。运行时优先使用这里的原图；生成素材用于补齐原包中没有的组合模块和新物品。

## 导入设置

`Assets/Editor/PixelArtTextureImporter.cs` 会为上述素材自动配置：

- Texture Type：Sprite (2D and UI)
- Filter Mode：Point
- Compression：None
- Mip Maps：关闭
- Alpha Is Transparency：开启
- Wrap Mode：Clamp

