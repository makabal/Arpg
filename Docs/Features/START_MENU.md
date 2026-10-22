# 青林物语 · 开始界面

## 场景与操作

- 场景：`Assets/GameRes/Scene/StartMenu.unity`，已添加为 Build Settings 首场景；保留 TrainingGround 为第二场景。
- 开始游戏：异步加载 TrainingGround。
- 游戏设置：音量、全屏显示、返回；Esc 关闭设置。
- 退出游戏：构建版退出程序，Editor 内结束 Play Mode。
- 鼠标与键盘导航由 InputSystemUIInputModule / EventSystem 处理。

## 视觉与动态

- 已定稿的木牌像素标题保留在森林底图中，底图已清除原按钮与骑士。
- 独立真实按钮使用项目 Fusion Pixel 12px 简体中文 TMP 字体；按钮宽度为 380 / 340 设计像素，采用深绿底、细木框、奶油字。
- 木质按钮使用九宫格 Sprite 与 pixelsPerUnitMultiplier=20，将边框控制在约 5–7 设计像素。
- 按钮独立字体材质 FaceDilate=0.035，保留 Bold，未改变原字体文件或原字体材质。
- 14 片飘叶（20–32 像素，约 50–80 像素/秒）、3 组呼吸光斑、3 条暖光束、背景 1.015 倍轻微缓移、骑士 8 帧 / 12fps 待机。
- Canvas 按 1920×1080 设计适配，保持 16:9；设计根节点遮罩避免效果越界。装饰不阻挡按钮射线。

## 代码与重建

- Runtime：`Assets/GameScript/Runtime/GameLogic/UI/StartMenu/`。
- Editor：`Assets/GameScript/Editor/StartMenuSceneBuilder.cs`。
- 美术：`Assets/GameRes/UIPanelArt/StartMenu/`。
- Builder 菜单：Tools → ARPG → Build Start Menu Scene。
- Builder 会保存/替换生成的 StartMenu 场景，请先关闭该场景、保留需要的手工编辑后再运行。它以 Additive 方式生成，不保存训练场的改动。
- 背景和空白按钮由内置 ImageGen 按定稿生成；叶片、光斑、光束、箭头由 Editor 生成简单原生素材。TMP 按钮文字独立渲染。
- 完整美术提示词：`Assets/ArtSource/UI/StartMenu/RuntimeAssetPrompts.md`。

## MCP 安装

- 经用户明确允许，安装 CoplayDev Unity MCP 10.1.2，与现有 Codex stdio 服务版本一致。
- 上游：https://github.com/CoplayDev/unity-mcp/tree/v10.1.2
- Git 下载遇到 TLS 握手失败后，改为官方 codeload 归档，本地重新打包 MCPForUnity 子目录并通过 Unity PackageManager Client.Add 安装。
- 本地包：`Packages/com.coplaydev.unity-mcp-10.1.2.tgz`，需随项目保留。
- 上游 zip SHA256：`2642a0f692f9e6b27c5de50aa945e052a2ef2cf10dd25ea4dc4a993ec4ee757a`。
- 本地 tgz SHA256：`400889f876e477ca2caa67d1050b763b33698b0afcdfe59894999a37385a7f27`。
- 一次性安装脚本已移除。已验证 Arpg@249b0a69 / 6400 连接。

## 2026-10-22 验证

- Unity 2022.3.62f3c1 完成导入、C# 编译；Console 无错误。
- 实际 Play Mode 验证独立首页渲染；字体引用正确、文字无溢出、场景仅一个 EventSystem。
- 通过 EventSystem 验证按钮射线命中、悬停/按压回弹、设置打开/返回、音量改变并恢复。
- 已实际验证 StartGame 回调进入 TrainingGround，训练场资源未修改。
- 多次采样确认骑士帧、落叶位置、光斑透明度和背景位置发生变化。
- 全屏选项已连线；未为验证切换用户显示模式。退出采用 Unity 标准 API，未调用构建版退出。
- 运行截图：`Captures/StartMenu/StartMenu_Final.png`。
- 本次未创建自动测试。开发者于 2026-10-22 明确同意将既有目录迁移与首页一起提交并推送；提交详情见 Git 历史。
