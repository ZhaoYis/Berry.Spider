# 变更：修复 SaveThreadStateAsync / ResumePreviousConversationAsync 在线程状态文件不存在时的行为

## 为什么
在 AIGenPlus 的 V2 Agent 基类 `AgentServiceBase` 中，线程状态通过 `SaveThreadStateAsync` / `ResumePreviousConversationAsync` 使用文件进行持久化：
- `SaveThreadStateAsync` 会把 `AgentThread.Serialize(...)` 的结果写入 `AppContext.BaseDirectory/AgentThreads/{taskId}.json`；
- `ResumePreviousConversationAsync` 会在执行 Agent 前尝试通过 `File.ReadAllTextAsync(filePath)` 读取相同路径的文件，并反序列化为 `AgentThread`。

当前实现中，没有考虑“第一次执行时该状态文件尚不存在”的情况：
- 当文件不存在时，`File.ReadAllTextAsync(filePath)` 会抛出异常，而不是简单返回空。
- 这会导致第一次执行某个 `taskId` 相关的 Agent 时就失败，而不是按预期创建一个新的空线程并从头开始对话。

## 变更内容
- 在 `ResumePreviousConversationAsync` 中显式处理文件不存在或内容为空/无效的情况：
  - 如果目标文件不存在，直接返回 `null`，让上层逻辑走“使用新建的 `AgentThread`”分支；
  - 如果文件存在但内容为空或反序列化失败，则记录日志并返回 `null`，避免异常中断正常流程。
- 在 `SaveThreadStateAsync` 中确保持久化路径存在：
  - 如果 `AppContext.BaseDirectory/AgentThreads` 目录不存在，则在保存前自动创建目录；
  - 保持序列化与当前一致，避免改变已存在文件的结构。
- 为线程状态恢复/保存增加最小诊断：在异常场景写入易于排查的日志信息（如 taskId、路径、异常消息），但不影响第一次执行的正常继续。

## 影响
- 受影响规范：桌面端 AIGenPlus 中基于 `AgentServiceBase` 的所有 V2 Agent 的首次执行体验和多轮对话恢复能力。
- 受影响代码：
  - `desktop/Berry.Spider.AIGenPlus/ViewModels/Pages/AgentsV2/AgentServiceBase.cs`
