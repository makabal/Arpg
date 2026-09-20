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

至少执行：

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
