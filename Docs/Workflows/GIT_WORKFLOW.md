# Git 工作流

当前项目默认使用 `main` 作为主要开发分支。

## 固定执行流程

每次需要提交并推送项目变更时，先进入项目根目录，然后严格按以下顺序执行：

```bash
# 1. 查看当前状态
git status

# 2. 拉取远端最新代码
git pull --rebase origin main

# 3. 添加全部修改
git add .

# 4. 提交
git commit -m "描述本次修改"

# 5. 推送到远端
git push origin main
```

执行规则：

- 提交信息应准确描述本次修改。
- 未经开发者明确要求，不额外创建或切换分支，也不执行 `stash`、`reset`、`cherry-pick` 等其他 Git 操作。
- 任一步骤失败时立即停止，直接报告失败原因，不自行增加替代操作。

## 提交前文档要求

开始上述 Git 流程前，必须检查并同步根目录 `PROJECT_OVERVIEW.md`：

- 功能、输入、架构、目录或已知限制变化时，更新对应章节。
- 每次 Commit 都更新“最近同步记录”。
- 未同步 `PROJECT_OVERVIEW.md` 时不执行提交。

## Commit 类型

提交信息可按修改性质使用：

- `feat:` 新功能
- `fix:` Bug 修复
- `refactor:` 不改变行为的重构
- `docs:` 文档
- `test:` 测试
- `chore:` 工具和维护

一个 Commit 尽量只表达一个完整目的。
