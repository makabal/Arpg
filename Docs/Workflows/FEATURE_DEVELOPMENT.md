# 功能开发流程

适用于新功能、Bug 修复、行为修改、重构和系统级调整。

完整 AI 分工规则见 `AI_COLLABORATION.md`。

## 标准流程

```text
开发者提出需求
        ↓
GPT-5.6 Sol 阅读 PROJECT_OVERVIEW / CODEX_BRIDGE / Docs / 实际代码
        ↓
Sol 理解需求并判断是否需要拆分
        ↓
Sol 给开发者 READY 前任务确认表格
（任务 / 要做什么 / 完成后状态 / 范围或备注）
        ↓
开发者确认
        ↓
写入 CODEX_BRIDGE.md
DRAFT → READY
        ↓
Sol 给 GPT-5.6 LunaMax 下发规范实现任务
        ↓
READY → IN_PROGRESS
        ↓
LunaMax 实现 + 自检
        ↓
Sol 主管审查
        ↓
必要的静态 / 编译 / Unity Play Mode 验证
        ↓
同步更新 PROJECT_OVERVIEW.md
        ↓
提交 Git
        ↓
IN_PROGRESS → REVIEW
        ↓
ChatGPT / 开发者验收
        ↓
通过 → ACCEPTED
失败 → CHANGES_REQUESTED
```

除非开发者明确表示“不需要确认”或“直接执行”，否则不得跳过 READY 前任务确认表格和确认等待。未经过该表格确认的任务不得标记为 `READY`。

## 开始实现前

Sol 必须明确：

- 目标是什么。
- 本次包含什么。
- 本次不包含什么。
- 必须复用哪些现有系统。
- 是否允许新增 ScriptableObject、Prefab、组件或数据结构。
- 如何客观判断完成。

任务复杂时拆分；简单任务不为了形式强行拆分。

## 实现原则

- LunaMax 只执行 Sol 已明确下发的任务范围。
- 优先小步修改，不为单个需求顺便重写无关系统。
- 先阅读现有代码，再决定是否新增类型。
- Unity 序列化、Prefab、Animator、AnimationEvent 等内容必须明确说明是否经过 Editor/Play Mode 验证。
- 运行时状态与静态配置分离。
- 实现过程中如需明显扩大范围，重新交给开发者确认。

## 完成条件

Codex 至少回填：

- 实现摘要。
- 关键修改文件。
- 静态/编译/Play Mode 验证结果。
- Commit SHA。
- 已知问题。

每次 Commit 前必须同步更新 `PROJECT_OVERVIEW.md`。

最终是否完成，以 `CODEX_BRIDGE.md` 中的验收状态为准。
