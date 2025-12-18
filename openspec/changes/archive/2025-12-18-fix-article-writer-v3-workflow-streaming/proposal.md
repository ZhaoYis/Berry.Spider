# 变更：修复 ArticleWriterAgentV3 工作流事件流未触发问题

## 为什么
在桌面端 AIGenPlus 的 ArticleWriterAgentV3 页面中，触发生成后工作流执行使用 `InProcessExecution.StreamAsync` + `await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync())` 监听事件。但当前实现中，循环在进入前就结束，`switch` 分支从未被命中，导致：
- 不会收到任何 `SummarizeWriterFinishedEvent` / `MainWriterFinishedEvent` / `ReviewerFinishedEvent` / `WorkflowOutputEvent`，UI 上 `AiResponseText` 也不会被追加更新；
- 异常或错误路径不可见，只能通过外部调试猜测原因，缺乏可观测性。

结合当前代码与 Microsoft.Agents.AI.Workflows 的反射执行模型，可以发现：
- V3 使用 `WorkflowBuilder(summarizeWriterExecutor)` 构建的工作流，其输入类型由第一个执行器的 `IMessageHandler<TInput, TOutput>` 泛型 `TInput` 决定；
- `SummarizeWriterExecutor` 实现的是 `IMessageHandler<string, SummarizeOutput>`，因此整个工作流期望的输入类型是 `string`；
- 但当前 V3 调用 `InProcessExecution.StreamAsync(workflow, messages)`，其中 `messages` 类型为 `List<ChatMessage>`，与工作流输入类型不匹配。

这种类型不匹配不会在编译期报错，但会导致工作流在没有任何可路由消息的情况下立即完成，从而 `WatchStreamAsync()` 不会产生任何 `WorkflowEvent`。这也解释了即使不依赖 `TurnToken`，事件流仍然为空的根本原因。

## 变更内容
- 明确 ArticleWriterAgentV3 工作流的输入类型为 `string`（与 `SummarizeWriterExecutor` 的 `IMessageHandler<string, SummarizeOutput>` 一致），避免使用 `List<ChatMessage>` 作为输入。
- 调整 `ArticleWriterAgentV3ViewModel.GeneratingAsync` 中对 `InProcessExecution.StreamAsync` 的调用，使其直接传入 `this.UserInput`（或构造好的 prompt 字符串），而不是 `List<ChatMessage>`，保证工作流入口消息类型与执行器期望一致。
- 在必要时为 V3 增加最小的日志/诊断逻辑，用于在发生输入类型不匹配或无事件产生时输出提示，便于后续排查。

## 影响
- 受影响规范：桌面端 AIGenPlus 中 ArticleWriterAgentV3 的生成体验与可观测性（事件流到达 UI 的行为）。
- 受影响代码：
  - `desktop/Berry.Spider.AIGenPlus/ViewModels/Pages/AgentsV3/ArticleWriterAgentV3ViewModel.cs`
  - 如有需要，可能影响 `SummarizeWriterExecutor` 等执行器对输入类型的约定文档（但本次优先限制在工作流触发与输入类型对齐）。
