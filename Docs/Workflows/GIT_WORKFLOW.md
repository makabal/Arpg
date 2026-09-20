# Git 工作流

当前项目默认使用 `main` 作为主要开发分支。

## 开始工作前

本地工作区干净时：

```bash
git pull --rebase
```

如果存在本地未提交修改，先根据情况：

```bash
git status
git add <需要提交的文件>
git commit -m "..."
```

或临时保存：

```bash
git stash
git pull --rebase
git stash pop
```

## 提交前

### 强制同步项目纵览

每一次 Commit 前都必须检查并更新根目录：

`PROJECT_OVERVIEW.md`

要求：

- 代码、架构、功能状态、输入、目录或已知限制变化时，更新对应章节。
- 即使提交不改变项目运行状态，也要更新“最近同步记录”，保证每个 Commit 都包含一次 PROJECT_OVERVIEW 同步。
- 未同步 PROJECT_OVERVIEW，不允许提交。

完成纵览同步后，至少执行：

```bash
git status
git diff --cached --stat
```

确认：

- 没有提交 `Library/`、`Temp/`、`Logs/`、`obj/`、`UserSettings/`。
- 没有提交 `.codex-dotnet/` 等工具缓存。
- Unity 资源与对应 `.meta` 一致。
- 没有无关的大量重导入变化。

## Commit 类型

推荐：

- `feat:` 新功能
- `fix:` Bug 修复
- `refactor:` 不改变行为的重构
- `docs:` 文档
- `test:` 测试
- `chore:` 工具和维护

一个 Commit 尽量只表达一个完整目的。

## 推送

```bash
git push origin main
```

推送后获取提交号：

```bash
git rev-parse --short HEAD
```

需要 ChatGPT 验收时，以该 Commit SHA 为准。
