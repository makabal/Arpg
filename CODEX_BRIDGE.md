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
负责：

- 在开始工作前阅读本文件和当前任务。
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
ChatGPT + 开发者讨论需求/方案
    ↓
需求确认
    ↓
在本文件创建任务：DRAFT → READY
    ↓
Codex 读取 READY 任务
    ↓
Codex 实现 + 自测 + 提交 Git
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

## 5. ChatGPT 创建任务时必须填写

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

## 6. Codex 开始任务前

Codex必须：

- 确认任务状态为 `READY` 或 `CHANGES_REQUESTED`。
- 阅读任务的目标、边界和验收标准。
- 阅读相关现有代码，不重复创建已有系统。
- 将状态改为 `IN_PROGRESS`。
- 如果发现需求与现有架构冲突，先标记 `BLOCKED` 并说明原因，而不是自行重新设计整个系统。

---

## 7. Codex 实现原则

### 7.1 优先复用现有系统

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

### 7.2 不进行无关重构

如果任务是：

> 给技能栏增加冷却遮罩

不要顺便：

- 重写整个 UI 框架。
- 修改技能系统核心架构。
- 更换 Input System。
- 大规模重命名无关类。

如确实需要重构才能完成，应先进入 `BLOCKED` 并说明理由。

### 7.3 ScriptableObject 边界

项目约定：

- ScriptableObject 保存静态配置。
- 运行时变化保存在 Runtime 对象或运行时组件中。
- 不在运行时修改 SO 来保存玩家当局状态。

### 7.4 UI 边界

UI主要负责：

- 展示状态。
- 转发用户输入。
- 订阅系统事件。

UI不应直接持有或实现核心战斗规则。

### 7.5 保持现有项目风格

修改前优先参考相邻代码的：

- 命名。
- 目录结构。
- 序列化字段。
- 事件使用方式。
- Unity 生命周期组织方式。

---

## 8. Codex 完成后必须回填

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

## 9. ChatGPT 验收规则

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

## 10. 需求变更规则

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

## 11. 一次只保留一个主要执行任务

为了避免 ChatGPT 与 Codex 对“当前该做什么”理解不同：

- 默认最多只有一个 `READY` / `IN_PROGRESS` 的主要功能任务。
- 其他想法放在 `DRAFT`。
- 紧急 Bug 可以作为独立任务例外处理。

---

## 12. Git 作为事实来源

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

## 13. 已确认架构约定

### PlayerSkillCollection 与六槽技能栏

当前实现中，`PlayerSkillCollection` 只注册**当前六个技能栏里的技能**。这里的“当前六个技能栏里的技能”是对现状的准确描述，不代表最终长期职责。

后续当玩家可拥有超过 6 个技能，并加入技能面板 / 技能树 / 拖拽装配后，约定调整为：

```text
PlayerSkillCollection
= 玩家已拥有 / 已解锁的全部技能

SkillBar
= 当前装备到 6 个快捷栏槽位中的技能
```

推荐的数据关系为：

```text
技能树 / 技能解锁
        ↓
PlayerSkillCollection
        ↓ 拖拽 / 装配
六槽 SkillBar
```

因此：

- `PlayerSkillCollection` 继续保留，作为未来“玩家全部可用技能”的运行时集合基础。
- 六槽 `SkillBar` 只负责当前装备与快捷键映射，不承担全部技能库存职责。
- 当前阶段只有少量技能，不要求立即为此重构。
- 当正式开始“技能数量超过 6 个、技能面板、技能树或拖拽装配”相关任务时，再拆分 Collection 的初始化来源，避免继续只从六槽 SkillBar 注册技能。


### 技能解锁与技能栏职责边界

已确认：**六槽技能栏不负责技能解锁逻辑。**

技能栏存在的目的，是展示和操作玩家当前已经装备到快捷栏中的技能，因此技能栏链路只负责：

- 当前槽位装备的技能。
- 技能图标与空槽状态。
- 冷却显示。
- 快捷键显示。
- 鼠标/键盘输入转发。
- 后续的拖拽、替换和交换。

技能是否解锁，应由未来专门的技能界面 / 技能树系统负责。在技能进入 SkillBar 之前完成资格校验，而不是把“未解锁技能”放进技能栏后再由 SkillBar 判断。

推荐职责关系：

```text
技能树 / 技能界面
负责：解锁、技能等级、技能点、分支选择、是否允许装备
        ↓
PlayerSkillCollection
负责：玩家当前拥有的技能及其运行时数据
        ↓
SkillBar
负责：6 个当前装备槽位、拖拽、替换、交换
        ↓
SkillSlotView
负责：图标、冷却、快捷键和输入展示
```

因此后续清理技能栏实现时，原则上应移除或下沉以下与“解锁”相关的技能栏职责：

- `PlayerSkillEntry.IsUnlocked`
- `PlayerSkillEntry.SetUnlocked(...)`
- `PlayerSkillCollection.UnlockChanged`
- `SkillUseFailure.SkillLocked`
- `SkillSlotView` 中基于解锁状态决定图标、按钮或锁图层的逻辑。
- 技能栏中的锁图标/锁定状态。

最终原则：

> 解锁是技能系统/技能树的资格问题；SkillBar 只处理已经允许装备的技能。

---

# 当前任务

> 没有任务时保持本区域为空模板。
> ChatGPT 在需求确认后填写。
> Codex 只执行状态为 READY 或 CHANGES_REQUESTED 的任务。

## Task

**ID:**  
**Status:** DRAFT  
**Title:**  

### 目标

-

### 背景

-

### 实现边界

#### 本次包含

-

#### 本次不包含

-

### 技术约束

-

### 建议关注文件

-

### 验收标准

- [ ]

### Codex 实现记录

#### 实现摘要

-

#### 修改文件

-

#### 验证结果

-

#### Git 信息

```text
Branch:
Commit:
```

#### 已知问题

-

### 验收反馈

-

---

## 历史任务

完成的任务可以从“当前任务”移动到这里。

建议每条只保留摘要：

```text
ARPG-20260920-01 | ACCEPTED | 技能栏运行时 UI
Commit: abcdef1
```

详细实现历史以 Git commit 为准。
