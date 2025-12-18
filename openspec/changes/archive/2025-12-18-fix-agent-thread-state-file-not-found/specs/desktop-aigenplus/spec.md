## 新增需求

### 需求：Agent 线程状态文件缺失或损坏时的健壮恢复
桌面端 AIGenPlus 中基于 `AgentServiceBase` 的 Agent，在恢复线程状态时必须优雅处理状态文件缺失或内容损坏的情况，保证不会因为持久化问题导致首次或后续对话无法执行。

#### 场景：首次执行时线程状态文件不存在
- **当** 某个 `taskId` 对应的 `AgentThreads/{taskId}.json` 文件尚未被创建（例如首次执行该任务）
- **那么** 系统在调用 `ResumePreviousConversationAsync` 时应直接返回 `null`，让上层逻辑使用新建的 `AgentThread`，而不是抛出文件不存在异常

#### 场景：线程状态文件存在但内容为空或损坏
- **当** `AgentThreads/{taskId}.json` 文件存在，但内容为空、非 JSON 或反序列化失败
- **那么** 系统应记录一条简明的诊断日志，并返回 `null` 以便创建新线程，而不是中断整个 Agent 执行

#### 场景：保存线程状态时自动创建目录
- **当** 系统调用 `SaveThreadStateAsync` 保存线程状态且 `AgentThreads` 目录尚不存在
- **那么** 系统必须自动创建该目录并成功写入状态文件，而不是因为目录缺失导致写入失败
