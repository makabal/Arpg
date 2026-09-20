# ChatGPT ↔ Codex 协作桥梁

本文件用于约定 **ChatGPT（需求讨论/方案确认/验收）** 与 **Codex（代码实现/修改/自测）** 在本仓库中的协作方式。

目标：让需求、实现、验收都通过 Git 仓库形成可追踪闭环，避免“聊天里说过但仓库里没有记录”的问题。

---

## 1. 角色分工

### ChatGPT
负责：

- 与开发者讨论需求、技术方案和系统边界。
- 把已经确认的需求整理成可执行任务。
- 明确验收标准、限制条件和非目标。
- Codex 完成后读取 GitHub 最新代码和任务回填内容。
- 根据实际代码、提交记录和验收标准进行验收。
- 验收通过后将任务标记为 `ACCEPTED`；不通过则给出返工项。

ChatGPT **不应把尚未确认的讨论内容直接当成实现要求**。

### Codex

Codex 内部采用：

```text
GPT-5.6 Sol     → 主管
GPT-5.6 Luna（推理强度：极高 / xhigh） → 代码实现
```

详细规则见 `Docs/Workflows/AI_COLLABORATION.md`。

负责：

- 在开始工作前先阅读 `PROJECT_OVERVIEW.md`、本文件和当前任务。
- 任何任务从 `DRAFT` 变为 `READY` 前，必须先向开发者展示任务确认表格，并得到明确确认；开发者明确免除确认时除外。
- 只执行状态为 `READY` 的任务。
- 基于仓库当前代码实现，不擅自扩大需求范围。
- 完成后进行必要的编译、静态检查或可执行测试。
- 提交 Git，并回填修改文件、测试结果、提交 SHA、已知问题。
- 将任务状态更新为 `REVIEW`，等待 ChatGPT/开发者验收。

Codex **不能自行把任务标记为 `ACCEPTED`**。

### 开发者
负责最终产品决策，可以随时修改需求、终止任务或覆盖本协议中的流程。

---

## 2. 标准工作流

```text
开发者提出想法
    ↓
GPT-5.6 Sol 阅读项目并理解需求
    ↓
Sol 判断是否需要拆分
    ↓
Sol 给开发者 READY 前任务确认表格
    ↓
开发者确认
    ↓
ChatGPT + 开发者讨论并固化需求
    ↓
ChatGPT + 开发者讨论需求/方案
    ↓
需求确认
    ↓
在本文件创建任务：DRAFT → READY
    ↓
Sol 读取 READY 任务并给 GPT-5.6 Luna（推理强度：极高 / xhigh）下发规范指令
    ↓
GPT-5.6 Luna（推理强度：极高 / xhigh）实现 + 自测
    ↓
Sol 主管审查
    ↓
更新 PROJECT_OVERVIEW.md
    ↓
提交 Git
    ↓
Codex 回填结果：READY → IN_PROGRESS → REVIEW
    ↓
ChatGPT 读取 GitHub 最新代码、提交和任务记录
    ↓
验收
    ├─ 通过：REVIEW → ACCEPTED
    └─ 不通过：REVIEW → CHANGES_REQUESTED
                         ↓
                      Codex 返工
```

---

## 3. 任务状态

只使用以下状态：

| 状态 | 含义 |
|---|---|
| `DRAFT` | 需求仍在讨论，不允许 Codex 开始实现 |
| `READY` | 需求已确认，Codex 可以开始 |
| `IN_PROGRESS` | Codex 正在实现 |
| `BLOCKED` | 实现受阻，需要开发者或 ChatGPT 决策 |
| `REVIEW` | Codex 已完成并提交，等待验收 |
| `CHANGES_REQUESTED` | 验收未通过，需要返工 |
| `ACCEPTED` | 已验收完成 |
| `CANCELLED` | 任务取消 |

---

## 4. 任务编号

格式：

```text
ARPG-YYYYMMDD-NN
```

示例：

```text
ARPG-20260920-01
```

同一天从 `01` 开始递增。

---

## 5. READY 前确认规则

任何准备交给 Codex 的任务，在状态从 `DRAFT` 改为 `READY` 前，ChatGPT / Sol 必须先向开发者展示任务确认表格。

最低格式：

| 任务 | 要做什么 | 完成后状态 | 范围 / 备注 |
|---|---|---|---|
| 示例任务 | 简述本次修改内容 | 简述完成后的结果 | 非目标或关键约束 |

规则：

- 多个子任务必须逐项列出。
- 开发者确认后，才允许将任务标记为 `READY`。
- 开发者明确说“不需要确认”或“直接执行”时，可以跳过等待确认，但仍建议保留表格作为任务记录。
- 未确认的任务保持 `DRAFT`。

---

## 6. ChatGPT 创建任务时必须填写

每个准备交给 Codex 的任务必须包含：

1. **目标**
   - 要解决什么问题。
   - 完成后玩家/系统应该有什么变化。

2. **背景**
   - 为什么现在要做。
   - 与现有系统的关系。

3. **实现边界**
   - 应该修改什么。
   - 明确哪些内容本次不要修改。

4. **技术约束**
   - 必须复用哪些现有系统。
   - 禁止引入哪些不必要架构。
   - 是否允许新增 ScriptableObject、Prefab、组件等。

5. **验收标准**
   - 必须是可以明确验证的结果。
   - 避免使用“效果更好”“更加合理”这种无法验收的描述。

6. **建议关注文件**
   - 仅用于帮助 Codex定位。
   - Codex仍应以仓库实际结构为准。

---

## 7. Codex 开始任务前

Codex必须：

- 确认任务状态为 `READY` 或 `CHANGES_REQUESTED`。
- 阅读 `PROJECT_OVERVIEW.md`。
- 阅读任务的目标、边界和验收标准。
- 阅读 `Docs/README.md`，并阅读与当前任务相关的 `Docs/Workflows/`、`Docs/Standards/` 和 `Docs/Decisions/`。
- 阅读相关现有代码，不重复创建已有系统。
- Sol 判断是否需要拆分；简单任务无需强行拆分。
- Sol 给 GPT-5.6 Luna（推理强度：极高 / xhigh）的任务必须包含目标、背景、范围、非目标、约束、验收结果和建议关注文件。
- 将状态改为 `IN_PROGRESS`。
- 如果发现需求与现有架构冲突，先标记 `BLOCKED` 并说明原因，而不是自行重新设计整个系统。

---

## 8. Codex 实现原则

### 8.1 优先复用现有系统

当前项目已有明确架构时，优先扩展而不是绕过。

例如技能相关功能优先复用：

```text
SkillDefinition
SkillRuntime
PlayerSkillController
SkillAimResolver
SkillDelivery
SkillTargetResolver
SkillEffect
SkillPresentationData
```

不要为了一个具体技能重新写一套独立技能流程。

### 8.2 不进行无关重构

如果任务是：

> 给技能栏增加冷却遮罩

不要顺便：

- 重写整个 UI 框架。
- 修改技能系统核心架构。
- 更换 Input System。
- 大规模重命名无关类。

如确实需要重构才能完成，应先进入 `BLOCKED` 并说明理由。

### 8.3 ScriptableObject 边界

项目约定：

- ScriptableObject 保存静态配置。
- 运行时变化保存在 Runtime 对象或运行时组件中。
- 不在运行时修改 SO 来保存玩家当局状态。

### 8.4 UI 边界

UI主要负责：

- 展示状态。
- 转发用户输入。
- 订阅系统事件。

UI不应直接持有或实现核心战斗规则。

### 8.5 保持现有项目风格

修改前优先参考相邻代码的：

- 命名。
- 目录结构。
- 序列化字段。
- 事件使用方式。
- Unity 生命周期组织方式。

---

## 9. Codex 完成后必须回填

提交前必须先同步更新 `PROJECT_OVERVIEW.md`。

Codex完成任务后，在任务中填写：

### 实现摘要

用 3～8 条说明实际完成了什么。

### 修改文件

只列关键文件，例如：

```text
Assets/Scripts/UI/SkillSlotView.cs
Assets/Scripts/UI/SkillHotbar.cs
Assets/Prefabs/UI/SkillHotbar.prefab
```

### 验证结果

必须区分：

- 已静态确认。
- 已编译确认。
- 已在 Unity Play Mode 验证。
- 无法在当前环境验证。

禁止把“理论上应该能运行”写成“已验证”。

### Git 信息

至少填写：

```text
Commit: <SHA>
Branch: <branch>
```

### 已知问题

没有则写：

```text
无
```

### 状态

完成并提交后改为：

```text
REVIEW
```

---

## 10. ChatGPT 验收规则

看到 `REVIEW` 后，ChatGPT应：

1. 读取任务内容。
2. 读取 Codex 填写的 Commit SHA。
3. 查看实际代码修改。
4. 对照验收标准逐项确认。
5. 必要时检查相关现有系统是否被破坏。
6. 给出验收结论。

验收通过：

```text
Status: ACCEPTED
```

验收不通过：

```text
Status: CHANGES_REQUESTED
```

并填写：

```text
## 验收反馈

- [ ] 问题 1
- [ ] 问题 2
- [ ] 问题 3
```

Codex之后只需要处理这些明确的返工项，不应重新解释整个需求。

---

## 11. 需求变更规则

### Codex 尚未开始

直接修改任务内容即可。

### Codex 已经 IN_PROGRESS

如果是小修改，可以追加：

```text
## 需求变更
- ...
```

如果会明显改变实现方向，建议：

1. 当前任务改为 `CANCELLED`。
2. 新建一个任务。

### REVIEW 后新增需求

新增需求默认属于**新任务**，不要混进已经完成的任务验收中。

---

## 12. 一次只保留一个主要执行任务

为了避免 ChatGPT 与 Codex 对“当前该做什么”理解不同：

- 默认最多只有一个 `READY` / `IN_PROGRESS` 的主要功能任务。
- 其他想法放在 `DRAFT`。
- 紧急 Bug 可以作为独立任务例外处理。

---

## 13. Git 作为事实来源

出现以下信息冲突时，优先级为：

```text
当前 GitHub 代码
>
本文件最新任务记录
>
README
>
聊天中的历史描述
```

任何人都不应该根据旧聊天内容假定代码仍然保持旧状态。


---

## 14. 长期规范与架构决策

长期有效的开发规范与架构决策统一维护在 `Docs/`：

- `Docs/Workflows/`：开发和验证流程。
- `Docs/Standards/`：项目与系统规范。
- `Docs/Decisions/`：已确认的重要架构决策。

当前已确认的 PlayerSkillCollection / SkillBar 职责决策见：

`Docs/Decisions/ADR-001-SKILL-COLLECTION-AND-HOTBAR.md`

`CODEX_BRIDGE.md` 只保留当前任务、任务状态、交接记录和验收反馈，避免与长期规范重复。

---

# 当前任务

## Task

**ID:** ARPG-20260920-01  
**Status:** ACCEPTED
**Title:** 技能栏职责清理

### 目标

移除技能栏链路中的解锁/锁定职责，使 SkillBar 和 SkillSlotView 只负责当前已装备技能的展示与输入。

### 背景

当前技能栏已经完成六槽运行时接入，但仍保留 `IsUnlocked`、`UnlockChanged`、`SkillLocked` 和锁图标/锁定判断等逻辑。

项目长期架构已经确认：

- 技能解锁属于未来技能界面 / 技能树。
- SkillBar 只管理当前装备的 6 个技能。
- SkillSlotView 只负责展示和输入。

相关规范：

- `Docs/Standards/SKILL_SYSTEM_RULES.md`
- `Docs/Decisions/ADR-001-SKILL-COLLECTION-AND-HOTBAR.md`

### 实现边界

#### 本次包含

- 移除技能栏链路中的 `PlayerSkillEntry.IsUnlocked` 使用。
- 移除与技能栏相关的 `PlayerSkillEntry.SetUnlocked(...)` / `PlayerSkillCollection.UnlockChanged` 使用。
- 移除 `SkillUseFailure.SkillLocked` 在当前技能栏使用链路中的作用。
- 移除 `SkillSlotView` 中的解锁判断、锁图标和锁定状态逻辑。
- 保留现有技能图标、空槽、冷却、快捷键、鼠标/键盘输入行为。
- 必要时清理已经失去用途的相关字段、事件或序列化引用。

#### 本次不包含

- 不实现技能树。
- 不实现技能面板。
- 不实现拖拽装配。
- 不重构 `PlayerSkillCollection` 为完整的“全部技能库”。
- 不修改现有技能释放、伤害、耗蓝、冷却规则。
- 不进行与本任务无关的 UI 或技能系统重构。

### 需求变更

- 2026-09-20：统一实现模型称谓为 `GPT-5.6 Luna（推理强度：极高 / xhigh）`；后续实现任务固定使用 `gpt-5.6-luna` + `xhigh`，不使用 `max`。不改变 Sol 主管与 GPT-5.6 Luna（推理强度：极高 / xhigh）执行的原有分工。

### 技术约束

- 必须遵守 `Docs/Workflows/AI_COLLABORATION.md`。
- 由 GPT-5.6 Sol 负责需求理解、任务审查和是否拆分；本任务较小，不要求为了形式强行拆分。
- 具体代码实现交给 GPT-5.6 Luna（推理强度：极高 / xhigh）。
- Sol 给 GPT-5.6 Luna（推理强度：极高 / xhigh）的任务必须明确包含目标、范围、非目标、约束和验收结果。
- UI 只负责展示、事件订阅和输入转发，不加入战斗逻辑。
- ScriptableObject 与 Runtime 的现有职责不变。
- 提交前必须同步更新 `PROJECT_OVERVIEW.md`。

### 建议关注文件

- `Assets/Scripts/Skills/Runtime/PlayerSkillCollection.cs`
- `Assets/Scripts/Skills/Runtime/PlayerSkillController.cs`
- `Assets/Scripts/Skills/Runtime/SkillRuntime.cs`
- `Assets/Scripts/Skills/Data/SkillEnums.cs`
- `Assets/Scripts/UI/SkillBar/SkillSlotView.cs`
- `Assets/Prefab/UI/SkillBar/SkillSlot.prefab`

Codex 应以仓库实际依赖关系为准，不应只机械修改上述文件。

### 验收标准

- [x] SkillBar / SkillSlotView 不再判断技能是否解锁。
- [x] 技能栏运行时链路中不再依赖 `UnlockChanged`。
- [x] 当前技能栏使用流程不再返回或依赖 `SkillLocked`。
- [x] 技能栏不再显示或维护锁图标/锁定状态。
- [x] 空槽仍正常显示。
- [x] 已装备技能仍正常显示图标和快捷键。
- [x] 冷却遮罩仍由现有 `SkillRuntime` 状态驱动。
- [x] 数字键 `1–6` 和鼠标技能槽输入行为不因本次清理被破坏。
- [x] 不引入技能树、拖拽或 Collection 全量技能库重构。
- [x] 已明确记录静态检查、编译检查和 Unity Play Mode 验证中实际完成的部分。
- [x] 提交前已同步更新 `PROJECT_OVERVIEW.md`。

### Codex 实现记录

#### 实现摘要

- 移除 `PlayerSkillEntry` 的解锁字段、构造参数和变更方法，并移除 `PlayerSkillCollection` 的解锁事件与设置接口。
- `PlayerSkillController.TryBeginUse` 不再判断解锁状态；`SkillUseFailure` 不再包含 `SkillLocked`。
- `PlayerManager` 保留槽位边界、基础攻击固定、重复技能限制和 Collection 注册约束，仅移除放置时的解锁判断。
- `SkillSlotView` 仅保留图标、空槽、冷却、快捷键和输入；已装备技能直接显示 `SkillDefinition.Icon`，主动技能按钮可用。
- 从 `SkillSlot.prefab` 移除 `LockImage` 子对象及其序列化引用，保留其余美术、布局和默认 Icon。

#### 修改文件

- `Assets/Scripts/Skills/Runtime/PlayerSkillCollection.cs`
- `Assets/Scripts/Skills/Runtime/PlayerSkillController.cs`
- `Assets/Scripts/Skills/Data/SkillEnums.cs`
- `Assets/Scripts/Player/PlayerManager.cs`
- `Assets/Scripts/UI/SkillBar/SkillSlotView.cs`
- `Assets/Prefab/UI/SkillBar/SkillSlot.prefab`
- `PROJECT_OVERVIEW.md`
- `CODEX_BRIDGE.md`
- `Docs/README.md`
- `Docs/Workflows/AI_COLLABORATION.md`
- `Docs/Workflows/FEATURE_DEVELOPMENT.md`

#### 验证结果

- 静态确认：运行时技能栏链路与 `SkillSlot.prefab` 已无 `IsUnlocked`、`SetUnlocked`、`UnlockChanged`、`SkillLocked`、`lockImage` 或 `LockImage` 残留；训练场仍保留六个 `SkillSlot` Prefab 实例，未见 `Missing Script` 或空脚本引用。
- 编译确认：已通过现有 Unity Editor 日志确认；本次相关脚本导入后出现 `Tundra build success`，其后未出现新的 C# 编译错误。
- Unity Play Mode 验证：开发者已人工验证通过。

#### Git 信息

```text
Branch: main
Commit: 7468568
Push: 未完成；环境安全策略拒绝向未验证归属的 `origin/main` 推送，未执行替代操作。
```

#### 已知问题

- Unity Editor 编译由现有日志确认；数字键 1–6、鼠标输入、冷却和持续技能路径已由开发者在 Unity Play Mode 中人工验证通过。

### 验收反馈

- 2026-09-20：Sol 主管已核对实现提交范围、运行时代码、Prefab 本地引用、训练场六槽实例和 Unity 最终编译日志。
- 验收结论：`ACCEPTED`。
- Play Mode 中的鼠标点击、数字键、持续施法和冷却表现已由开发者人工验证通过。

---

## 历史任务

完成的任务可以从“当前任务”移动到这里。

建议每条只保留摘要：

```text
ARPG-20260920-01 | ACCEPTED | 技能栏运行时 UI
Commit: abcdef1
```

详细实现历史以 Git commit 为准。
