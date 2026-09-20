# 功能开发流程

适用于新功能、较大行为修改和系统级调整。

## 标准流程

```text
开发者提出需求
        ↓
ChatGPT + 开发者讨论需求与技术方案
        ↓
确认边界、非目标、验收标准
        ↓
写入 CODEX_BRIDGE.md
DRAFT → READY
        ↓
Codex 阅读相关 Docs + 现有代码
        ↓
READY → IN_PROGRESS
        ↓
实现 + 自测
        ↓
提交 Git
        ↓
IN_PROGRESS → REVIEW
        ↓
ChatGPT 检查实际 Commit 与代码
        ↓
通过 → ACCEPTED
失败 → CHANGES_REQUESTED
```

## 开始实现前

必须明确：

- 目标是什么。
- 本次包含什么。
- 本次不包含什么。
- 必须复用哪些现有系统。
- 是否允许新增 ScriptableObject、Prefab、组件或数据结构。
- 如何客观判断完成。

## 实现原则

- 优先小步修改，不为单个需求顺便重写无关系统。
- 先阅读现有代码，再决定是否新增类型。
- Unity 序列化、Prefab、Animator、AnimationEvent 等内容必须明确说明是否经过 Editor/Play Mode 验证。
- 运行时状态与静态配置分离。

## 完成条件

Codex 至少回填：

- 实现摘要。
- 关键修改文件。
- 静态/编译/Play Mode 验证结果。
- Commit SHA。
- 已知问题。

最终是否完成，以 `CODEX_BRIDGE.md` 中的验收状态为准。
