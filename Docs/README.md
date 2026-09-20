# 项目开发规范

本目录保存 **长期有效的开发流程、项目规范和架构决策**。

职责划分：

- 根目录 `README.md`：项目现状、架构概要和开发进度。
- 根目录 `CODEX_BRIDGE.md`：ChatGPT ↔ Codex 的当前任务、交接与验收。
- `Docs/Workflows/`：某类工作应该按什么流程执行，包括 AI 协作开发。
- `Docs/Standards/`：代码、Unity 资源和系统设计需要遵守的长期规则。
- `Docs/Decisions/`：已经确认的重要架构决策及其原因。

## 使用原则

1. 长期规则不要写进当前任务。
2. 当前任务不要写进长期规范。
3. 架构决策一旦确认，优先记录到 `Docs/Decisions/`。
4. 修改规范时，应同步检查相关 README 和 CODEX_BRIDGE 是否需要更新引用。
5. 实际代码与规范冲突时，不要擅自大规模重构；先确认是代码过时还是规范过时。
6. 每次 Git Commit 前都必须同步更新根目录 `PROJECT_OVERVIEW.md`。

## 当前文档

### Workflows

- `AI_COLLABORATION.md`：开发者确认、Sol 主管、LunaMax 实现和任务清单规则。
- `FEATURE_DEVELOPMENT.md`：功能从需求讨论到验收的完整流程。
- `GIT_WORKFLOW.md`：Git 提交、拉取、推送和提交边界。
- `UNITY_VALIDATION.md`：Unity 功能完成后的验证清单。

### Standards

- `UNITY_PROJECT_RULES.md`：Unity 项目、资源、ScriptableObject、Prefab 等通用规则。
- `SKILL_SYSTEM_RULES.md`：技能系统和技能栏的长期职责边界。

### Decisions

- `ADR-001-SKILL-COLLECTION-AND-HOTBAR.md`：PlayerSkillCollection 与六槽 SkillBar 的职责划分。
