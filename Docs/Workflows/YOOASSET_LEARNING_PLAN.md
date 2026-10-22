# YooAsset 项目实践学习计划

建立日期：2026-10-20。资源路径已同步本日官方示例风格目录迁移。适用环境：Unity 2022.3.62f3c1、YooAsset 3.0.5。

## 目标与学习方式

通过本项目掌握资源采集、打包、加载、生命周期与资源更新，之后再接入 HybridCLR。

每次只完成一个可验证的小闭环：先说明概念，开发者实际操作，再检查结果并记录原因。以验收结果推进，不以固定天数推进。本计划是学习安排，不代表已经修改代码、完成配置或通过验证；具体代码与资源变更仍遵循项目的任务确认流程。

## 阶段安排

| 阶段 | 学习内容与项目实践 | 验收结果 |
|---|---|---|
| 1. 采集与地址 | 区分 Package、Group、Collector、Bundle、Address、Tag；用训练木桩配置第一个采集器 | 能说明木桩被哪个采集器收集、用什么地址加载、采用什么打包粒度 |
| 2. 最小加载与释放 | 在独立学习场景使用编辑器模拟模式；直接使用 YooAsset 原生 API，异步加载、实例化、销毁、释放 Handle；观察失败状态 | 能重复加载和清理木桩，并用 Bundle Debugger 检查 Handle 与引用；理解模拟模式未验证真实 Bundle |
| 3. 代码拆分与资源管理器 | 在现有 GameScript 目录和已跑通的 API 上建立启动与资源管理职责；按实际需要拆程序集；封装初始化、资源加载和场景加载 | 程序集依赖清晰、编译通过；业务通过薄 ResourceManager 访问资源；Handle 所有者与释放时机明确 |
| 4. 分类与真实构建 | 基于已整理的 GameRes/Entity、EntityArt、Scene、SceneArt、UIPanel、UIPanelArt、Config 等目录配置采集；学习主资源、依赖资源和静态资源采集；比较 PackSeparately、PackDirectory、PackTopDirectory；使用离线模式加载真实包 | 在 Bundle Reporter 中解释依赖与共享包；比较改动前后 Bundle 数量、大小；构建后的 Player 能离线加载 |
| 5. 项目资源接管 | 加载技能 ScriptableObject、Prefab、Sprite 子资源与场景；理解 Inspector 引用自动带入依赖；明确场景卸载、对象销毁和资源释放顺序；按需实践原生文件加载 | 训练场中至少一条业务链由 YooAsset 加载；往返加载场景后引用可解释；检查底包与 Bundle 的重复收录 |
| 6. 资源更新与维护 | 本地 HTTP 服务、联机模式、版本与清单、增量更新、Tag 下载、内置资源与缓存；实验中断重试、损坏文件、断网启动、版本回退及缓存清理 | 同一个安装包从资源 v1 更新到 v2；观察下载差异；能够解释缓存命中、离线条件和回退结果 |

阶段 1～2 先保留直接 API 调用，阶段 3 再提炼资源管理器。这样每一层封装都有已经验证的用途，便于理解 YooAsset，而不是只会调用自己的封装。

阶段 4 的初始分类建议：先保留一个 DefaultPackage，在其内建立业务 Group；不要为了目录分类建立多个 Package。Tag 是下载筛选标记，不代替 Bundle 打包规则。需要时再学习多 Package、加密、自定义构建扩展和 CI 发布。

## 第一课：训练木桩采集配置

本课只配置采集，不修改训练场、不搬资源、不拆程序集、不安装 HybridCLR。

### 先理解六个概念

| 概念 | 本课对应 | 含义 |
|---|---|---|
| Package | DefaultPackage | 运行时初始化、版本和资源清单的管理单位 |
| Group | Characters | 采集配置的组织单位；分组本身不意味着一个 Bundle |
| Collector | 木桩所在目录的采集器 | 决定从哪里采集、采什么、如何寻址和打包 |
| Bundle | 由 PackSeparately 规则生成 | 实际构建出的资源包；依赖关系可能引入其他包 |
| Address | Characters_TrainingDummy | 代码加载资源时使用的定位地址 |
| Tag | 本课先留空 | 后续按标签筛选下载内容时学习 |

### Unity Editor 操作

1. 打开顶部菜单 `YooAsset > Bundle Collector`。
2. 新建 Package，命名为 `DefaultPackage`。
3. 在 Package 中新建 Group，命名为 `Characters`。
4. 添加 Collector，并设置下表中的属性；界面可能显示中文名称，对应类名一并列出。
5. 保存配置，确认 Package 与 Group 下能看到该采集器。

| 属性 | 设置值 |
|---|---|
| 采集路径 | `Assets/GameRes/Entity/Enemy` |
| 采集器类型 | `MainAssetCollector`，主资源采集器 |
| 过滤规则 | `CollectPrefab`，只采集 Prefab |
| 寻址规则 | `AddressByGroupAndFileName`，分组名_文件名 |
| 打包规则 | `PackSeparately`，按文件分别打包 |
| 标签 | 留空 |

当前该目录的实际练习资源为 `Assets/GameRes/Entity/Enemy/TrainingDummy.prefab`。依据 YooAsset 3.0.5 的地址规则，它的地址应为 `Characters_TrainingDummy`。本课目录收集器的范围是 Enemy 目录内的全部 Prefab，未来放入其他 Prefab 时也会被采集。

### 本课验收

- [ ] 能在采集窗口看到 DefaultPackage / Characters / Collector 的层级。
- [ ] 采集路径与三项规则和上表一致，保存后重新打开配置仍然存在。
- [ ] 能回答：加载地址是什么？为什么 Group 不一定等于一个 Bundle？木桩引用的贴图为什么不一定要另设成可寻址主资源？

达到这些条件后进入第二课：在独立学习场景用编辑器模拟模式加载并实例化该木桩。真实 Bundle 内容与依赖是否符合预期，留到阶段 4 的构建报告验证，不能仅凭采集配置宣称验证通过。

## 学习记录

- 已确认基线：YooAsset 3.0.5 已安装；`Assets/GameSetting/BundleCollectorSetting.asset` 的 Packages 为空；尚无自定义 asmdef。
- 当前进度：计划已建立，项目已完成目录整理；采集配置仍为空，第一课待开发者操作与验收。
- 后续每课记录：修改了什么、实际观察到什么、验证方式、还有什么没理解。

## 参考

- 官方教程：https://www.yooasset.com/
- 以项目安装版本为准核对 API 与规则：`Library/PackageCache/com.tuyoogame.yooasset@3.0.5/`。该目录仅供阅读，不修改生成或缓存文件。
- 本课菜单与规则已根据该版本 Editor 源码核对；Unity Editor 内的实际操作尚未执行。
