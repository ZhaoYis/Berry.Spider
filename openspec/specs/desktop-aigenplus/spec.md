# desktop-aigenplus Specification

## Purpose
TBD - created by archiving change update-article-writer-v2-autoscroll. Update Purpose after archive.
## 需求
### 需求：ArticleWriterAgentV2 AI 回答文本框自动滚动
桌面端 AIGenPlus 中的 ArticleWriterAgentV2 页面必须在 AI 回答内容持续追加时，自动滚动到最新内容，以便用户实时查看生成结果。

#### 场景：AI 回答正常生成时自动滚动到底部
- **当** 用户在 ArticleWriterAgentV2 页面输入提示词并点击“开始生成”，AI 持续向 `AiResponseText` 追加文本
- **那么** 右侧 AI 回答文本框在内容超出可视高度后，应自动滚动到文本末尾，始终展示最新一段回答

#### 场景：用户手动向上滚动时不强制抢占滚动位置
- **当** 用户在 AI 回答文本框中手动向上滚动查看历史内容
- **那么** 即使 AI 继续向 `AiResponseText` 追加新文本，文本框也不应强制滚动回底部，保留用户当前的滚动位置

#### 场景：用户恢复到底部后继续自动滚动
- **当** 用户在查看历史内容后，将滚动条重新拖动到底部
- **那么** 在 AI 继续向 `AiResponseText` 追加新文本时，文本框应恢复为自动滚动到最新内容的行为

### 需求：ArticleWriterAgentV3 工作流输入类型与事件流可观测性
桌面端 AIGenPlus 中的 ArticleWriterAgentV3 页面必须确保工作流输入类型与执行器定义一致，并在触发生成时可靠地产生可被消费的 `WorkflowEvent`，同时在无事件或错误场景下具备基本可观测性。

#### 场景：正常生成时事件流被消费并更新 UI
- **当** 用户在 ArticleWriterAgentV3 页面输入提示词并点击“开始生成”，并且上游模型/Agent 配置正确
- **那么** 工作流应以与执行器输入类型一致的消息（例如 `string`）启动，并产生一系列 `WorkflowEvent` 被 `WatchStreamAsync` 循环消费，至少包括一个携带输出数据的事件，用于追加更新 `AiResponseText`

#### 场景：输入类型不匹配或未产生事件时有诊断信息
- **当** 工作流因为输入类型与执行器期望不一致或其他原因导致 `WatchStreamAsync` 在未产生任何事件的情况下结束
- **那么** 系统必须至少输出一条诊断日志，说明未接收到事件或事件流提前结束，以避免静默失败

#### 场景：发生错误事件时有可观测性
- **当** 工作流执行过程中产生 `WorkflowErrorEvent` 或等价错误事件
- **那么** 系统必须记录错误详情（例如异常信息、上下文），便于后续排查，而不是完全吞掉错误

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

